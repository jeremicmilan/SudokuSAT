using System.Threading.Tasks;
using System.Threading;
using Google.OrTools.Sat;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SudokuSAT
{
    public class SudokuSolver
    {
        public MainWindow Window { get; private set; }

        public bool IsExploreActive { get; private set; } = false;
        public CancellationTokenSource? CancellationTokenSource { get; private set; } = null;

        public SudokuSolver(MainWindow window)
        {
            Window = window;
        }
        public void Explore(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            IsExploreActive = true;
            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = CancellationTokenSource.Token;
            Window.Dispatcher.Invoke(() => Window.ExploreButton.Content = "Stop");
            Dictionary<SudokuCell, HashSet<int>?> sudokuCellToOldPossibleValuesDictionary = new();
            try
            {
                foreach (SudokuCell sudokuCell in sudoku.SudokuCells)
                {
                    if (!sudokuCell.ComputedValue.HasValue)
                    {
                        sudokuCellToOldPossibleValuesDictionary[sudokuCell] = sudokuCell.PossibleValues;
                        Window.Dispatcher.Invoke(() => sudokuCell.SetPossibleValues(null));
                    }
                }

                Parallel.ForEach(
                    sudokuCells != null && sudokuCells.Any() ? sudokuCells : sudoku.SudokuCells,
                    new() { MaxDegreeOfParallelism = sudoku.Width },
                    (sudokuCell) =>
                {
                    Thread.CurrentThread.Name = "Explore_" + sudokuCell.Name;

                    if (!sudokuCell.ComputedValue.HasValue)
                    {
                        Sudoku sudokuTemp = sudoku.Clone();
                        HashSet<int> possibleValues = new();
                        for (int i = SudokuCell.MinValue; i <= SudokuCell.MaxValue; i++)
                        {
                            CpSolver solver = new();
                            CpModel model = sudokuTemp.GenerateModel();

                            model.Add(sudokuTemp.SudokuGrid[sudokuCell.Column, sudokuCell.Row].ValueVar == i);

                            CpSolverStatus solverStatus = solver.Solve(model);

                            if (cancellationToken.IsCancellationRequested)
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

                        Window.Dispatcher.Invoke(() => sudokuCell.SetPossibleValues(possibleValues));
                    }
                });
            }
            catch (Exception)
            {
                if (IsExploreActive)
                {
                    throw;
                }
            }
            finally
            {
                Window.Dispatcher.Invoke(() => sudoku.PerformSudokuAction(
                    new SudokuActionsPossibleValues(sudoku, sudokuCellToOldPossibleValuesDictionary)));
                Window.Dispatcher.Invoke(() => Window.ExploreButton.Content = "Explore");
                IsExploreActive = false;
            }
        }

        public void CheckIsExploreActive()
        {
            if (IsExploreActive)
            {
                throw new Exception("Explore in progress... Stop it or wait for it to finish.");
            }
        }

        public void Solve(Sudoku sudoku, bool updateSolvedValue)
        {
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
                    Window.SolutionCount.Content = 0;
                    break;

                case CpSolverStatus.Feasible:
                case CpSolverStatus.Optimal:
                    if (updateSolvedValue)
                    {
                        sudoku.PerformSudokuAction(new SudokuActionsValue(sudoku, solver));
                    }
                    break;
            }
        }
    }
}
