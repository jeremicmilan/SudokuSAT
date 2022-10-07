using System.Threading.Tasks;
using System.Threading;
using Google.OrTools.Sat;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System;
using System.Data;
using System.Collections.Generic;
using MoreLinq;

namespace SudokuSAT
{
    public class SudokuSolver
    {
        public MainWindow Window { get; private set; }

        public SudokuSolver(MainWindow window)
        {
            Window = window;
        }

        public bool IsExploreActive = false;
        public void Explore(Sudoku sudoku)
        {
            IsExploreActive = true;
            Parallel.ForEach(
                sudoku.SudokuCells,
                new() { MaxDegreeOfParallelism = sudoku.Width },
                (sudokuCell) =>
            {
                Thread.CurrentThread.Name = "Explore";
                Sudoku sudokuTemp = sudoku.Clone();

                HashSet<int> possibleValues = new();

                if (!sudokuCell.Value.HasValue)
                {
                    for (int i = SudokuCell.MinValue; i <= SudokuCell.MaxValue; i++)
                    {
                        CpSolver solver = new();
                        CpModel model = sudokuTemp.GenerateModel();

                        model.Add(sudokuTemp.SudokuGrid[sudokuCell.Column, sudokuCell.Row].ValueVar == i);

                        CpSolverStatus solverStatus = solver.Solve(model);

                        if (!IsExploreActive)
                        {
                            return;
                        }

                        switch (solverStatus)
                        {
                            case CpSolverStatus.Unknown:
                            case CpSolverStatus.ModelInvalid:
                                throw new Exception("Solver status: " + solverStatus);

                            case CpSolverStatus.Infeasible:
                                break;

                            case CpSolverStatus.Feasible:
                            case CpSolverStatus.Optimal:
                                Window.Dispatcher.Invoke(() => sudokuCell.AddPossibleValue(i));
                                possibleValues.Add(i);
                                break;
                        }
                    }

                    Window.Dispatcher.Invoke(() =>
                    {
                        switch (possibleValues.Count)
                        {
                            case 0:
                                sudokuCell.SetValue(0, ValueType.Solver);
                                break;

                            case 1:
                                sudokuCell.SetValue(possibleValues.First(), ValueType.Solver);
                                break;
                        }
                    });
                }
            });

            IsExploreActive = false;
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
                    throw new Exception("Solver status: " + solverStatus);

                case CpSolverStatus.Infeasible:
                    Window.SolutionCount.Content = 0;
                    break;

                case CpSolverStatus.Feasible:
                case CpSolverStatus.Optimal:
                    if (updateSolvedValue)
                    {
                        foreach (var sudokuCell in sudoku.SudokuGrid)
                        {
                            sudokuCell.UpdateSolvedValue(solver);
                        }
                    }
                    break;
            }
        }
    }
}
