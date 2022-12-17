using Google.OrTools.Sat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSAT
{
    public class SudokuSolver
    {
        public MainWindow Window { get; private set; }

        private CancellationTokenSource CancellationTokenSource { get; set; } = new();
        private CancellationToken CancellationToken { get; set; }


        public volatile bool IsSolveActive = false;

        public SudokuSolver(MainWindow window)
        {
            Window = window;
        }

        public void HandleSolveAction(Action solveAction)
        {
            IsSolveActive = true;

            Window.SolveButton.IsEnabled = false;
            Window.ExploreButton.IsEnabled = false;
            Window.PossibilitiesButton.IsEnabled = false;
            Window.StopButton.IsEnabled = true;

            CancellationTokenSource = new();
            CancellationToken = CancellationTokenSource.Token;

            new Thread(() => Window.HandleFailure(() =>
            {
                try
                {
                    solveAction();
                }
                catch (Exception)
                {
                    if (IsSolveActive)
                    {
                        throw;
                    }
                }
                finally
                {
                    Window.Dispatcher.Invoke(() =>
                    {
                        Window.SolveButton.IsEnabled = true;
                        Window.ExploreButton.IsEnabled = true;
                        Window.PossibilitiesButton.IsEnabled = true;
                        Window.StopButton.IsEnabled = false;
                    });

                    IsSolveActive = false;
                }
            })).Start();
        }

        public void StopSolveAction()
        {
            if (IsSolveActive)
            {
                CancellationTokenSource.Cancel();
            }
        }

        public void CheckIsSolveActive()
        {
            if (IsSolveActive)
            {
                throw new Exception("Explore in progress... Stop it or wait for it to finish.");
            }
        }

        public void Solve(Sudoku sudoku, bool updateSolvedValue)
        {
            Window.Dispatcher.Invoke(() => Window.SolutionCount.Content = "...");

            CpSolver solver = new()
            {
                StringParameters = "enumerate_all_solutions:true"
            };

            CpModel model = sudoku.GenerateModel();
            CpSolverStatus solverStatus = solver.Solve(model, new SolutionCounter(this, solver));

            switch (solverStatus)
            {
                case CpSolverStatus.Unknown:
                case CpSolverStatus.ModelInvalid:
                    throw new Exception("Solver status: " + solverStatus
                        + " - model validation: " + model.Validate());

                case CpSolverStatus.Infeasible:
                    Window.Dispatcher.Invoke(() => Window.SolutionCount.Content = 0);
                    break;

                case CpSolverStatus.Feasible:
                case CpSolverStatus.Optimal:
                    if (updateSolvedValue)
                    {
                        Window.Dispatcher.Invoke(() => sudoku.PerformSudokuAction(
                            new SudokuActionsValue(sudoku, solver),
                            solver: true));
                    }
                    break;
            }
        }

        internal void ClearExploreIfNeeded(Sudoku sudoku)
        {
            Dictionary<SudokuCell, HashSet<int>?> oldPossibleValues = new();
            foreach (SudokuCell sudokuCell in sudoku.SudokuCells)
            {
                if (sudokuCell.PossibleValue == null &&
                    sudokuCell.PossibleValues != null && sudokuCell.PossibleValues.Count > 0)
                {
                    oldPossibleValues[sudokuCell] = sudokuCell.PossibleValues;
                    sudokuCell.PossibleValues = null;
                }
            }

            if (oldPossibleValues.Count > 0)
            {
                sudoku.PerformSudokuAction(new SudokuActionsPossibleValues(sudoku, oldPossibleValues), solver: true);
            }
        }

        public void Explore(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            ConcurrentDictionary<SudokuCell, HashSet<int>?> oldPossibleValues = new();
            try
            {
                Parallel.ForEach(
                    sudokuCells != null && sudokuCells.Any() ? sudokuCells : sudoku.SudokuCells,
                    new() { MaxDegreeOfParallelism = sudoku.Width },
                    (sudokuCell) =>
                {
                    Thread.CurrentThread.Name = "Explore_" + sudokuCell.Name;

                    if (!sudokuCell.ComputedValue.HasValue && sudokuCell.PossibleValues == null)
                    {
                        Sudoku sudokuTemp = sudoku.Clone();
                        HashSet<int> possibleValues = new();
                        for (int i = sudokuCell.MinValue; i <= sudokuCell.MaxValue; i++)
                        {
                            CpSolver solver = new();
                            CpModel model = sudokuTemp.GenerateModel();

                            model.Add(sudokuTemp.SudokuGrid[sudokuCell.Column, sudokuCell.Row].ValueVar == i);

                            CpSolverStatus solverStatus = solver.Solve(model);

                            if (CancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            switch (solverStatus)
                            {
                                case CpSolverStatus.Unknown:
                                case CpSolverStatus.ModelInvalid:
                                    throw new Exception("Solver status: " + solverStatus
                                        + " - model validation: " + model.Validate());

                                case CpSolverStatus.Infeasible:
                                    break;

                                case CpSolverStatus.Feasible:
                                case CpSolverStatus.Optimal:
                                    possibleValues.Add(i);
                                    break;
                            }
                        }

                        oldPossibleValues[sudokuCell] = null;
                        Window.Dispatcher.Invoke(() => sudokuCell.SetPossibleValues(possibleValues));
                    }
                });
            }
            finally
            {
                if (oldPossibleValues.Count > 0)
                {
                    Window.Dispatcher.Invoke(() => sudoku.PerformSudokuAction(
                        new SudokuActionsPossibleValues(sudoku, oldPossibleValues.ToDictionary(
                            kvp => kvp.Key, kvp => kvp.Value, oldPossibleValues.Comparer)),
                        solver: true));
                }
            }
        }

        internal void Possibilities(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            List<List<int>> solutions = new();
            CpSolverStatus solverStatus = CpSolverStatus.Unknown;
            Window.Dispatcher.Invoke(() =>
            {
                Window.PossibilitiesListBox.Items.Clear();
            });
            for (int i = 0; i < SolutionCounter.MaxSolutionCount; i++)
            {
                CpSolver solver = new();
                CpModel model = sudoku.GenerateModel();

                foreach (List<int> solution in solutions)
                {
                    Debug.Assert(solution.Count == sudokuCells.Count);
                    List<BoolVar> boolVars = new();
                    for (int j = 0; j < solution.Count; j++)
                    {
                        BoolVar boolVar = model.NewBoolVar("possibilities_solution" + i + "_" + sudokuCells[j].Name);
                        model.Add(sudokuCells[j].ValueVar == solution[j]).OnlyEnforceIf(boolVar);
                        model.Add(sudokuCells[j].ValueVar != solution[j]).OnlyEnforceIf(boolVar.Not());
                        boolVars.Add(boolVar);
                    }

                    model.AddBoolOr(boolVars.Select(boolVar => boolVar.Not()));
                }

                solverStatus = solver.Solve(model);

                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (solverStatus == CpSolverStatus.Infeasible)
                {
                    break;
                }

                switch (solverStatus)
                {
                    case CpSolverStatus.Unknown:
                    case CpSolverStatus.ModelInvalid:
                        throw new Exception("Solver status: " + solverStatus
                            + " - model validation: " + model.Validate());

                    case CpSolverStatus.Feasible:
                    case CpSolverStatus.Optimal:
                        List<int> solution = sudokuCells.Select(sudokuCell => (int)solver.Value(sudokuCell.ValueVar)).ToList();
                        solutions.Add(solution);
                        Window.Dispatcher.Invoke(() =>
                        {
                            Window.PossibilitiesListBox.Items.Add(string.Join(',', solution));
                            Window.PossibilitiesListBox.Items.SortDescriptions.Add(
                                new System.ComponentModel.SortDescription("", System.ComponentModel.ListSortDirection.Ascending));
                        });
                        break;
                }
            }
        }
    }
}
