using System.Threading.Tasks;
using System.Threading;
using Google.OrTools.Sat;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System;

namespace SudokuSAT
{
    public class SudokuSolver
    {
        public MainWindow Window { get; private set; }

        public SudokuSolver(MainWindow window)
        {
            Window = window;
        }

        public void Explore(Sudoku sudoku)
        {
            Thread.CurrentThread.Name = "Explore";

            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = sudoku.Width
            };
            Parallel.ForEach(sudoku.SudokuCells, parallelOptions, (sudokuCell) =>
            {
                Sudoku sudokuTemp = sudoku.Clone();

                if (!sudokuCell.Value.HasValue)
                {
                    sudokuCell.PossibleValues = new();

                    for (int i = SudokuCell.MinValue; i <= SudokuCell.MaxValue; i++)
                    {
                        CpSolver solver = new();
                        CpModel model = sudokuTemp.GenerateModel();

                        model.Add(sudokuTemp.SudokuGrid[sudokuCell.Column, sudokuCell.Row].ValueVar == i);

                        CpSolverStatus solverStatus = solver.Solve(model);
                        switch (solverStatus)
                        {
                            case CpSolverStatus.Unknown:
                            case CpSolverStatus.ModelInvalid:
                                throw new Exception("Solver status: " + solverStatus);

                            case CpSolverStatus.Infeasible:
                                break;

                            case CpSolverStatus.Feasible:
                            case CpSolverStatus.Optimal:
                                sudokuCell.PossibleValues.Add(i);
                                break;
                        }
                    }

                    Window.Dispatcher.Invoke(() =>
                    {

                        switch (sudokuCell.PossibleValues.Count)
                        {
                            case 0:
                                sudokuCell.SetValue(0, ValueType.Solver);
                                break;

                            case 1:
                                sudokuCell.SetValue(sudokuCell.PossibleValues.First(), ValueType.Solver);
                                break;

                            default:
                                UniformGrid cellGrid = new()
                                {
                                    Rows = 3,
                                    Columns = 3
                                };

                                for (int i = 1; i <= cellGrid.Rows * cellGrid.Columns; i++)
                                {
                                    if (sudokuCell.PossibleValues.Contains(i))
                                    {
                                        Label label = new()
                                        {
                                            HorizontalAlignment = HorizontalAlignment.Center,
                                            VerticalAlignment = VerticalAlignment.Center,
                                            HorizontalContentAlignment = HorizontalAlignment.Center,
                                            VerticalContentAlignment = VerticalAlignment.Center,
                                            MinWidth = sudokuCell.Grid.ActualWidth / 3,
                                            MinHeight = sudokuCell.Grid.ActualHeight / 3,
                                            FontSize = sudokuCell.Grid.ActualHeight * 0.18,
                                            Foreground = Brushes.Green,
                                            Content = i
                                        };
                                        cellGrid.Children.Add(label);
                                    }
                                    else
                                    {
                                        cellGrid.Children.Add(new Label());
                                    }
                                }

                                sudokuCell.Grid.Children.Add(cellGrid);
                                break;
                        }

                    });
                }
            });
        }

        public void Solve(Sudoku sudoku)
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
                    foreach (var sudokuCell in sudoku.SudokuGrid)
                    {
                        sudokuCell.UpdateSolvedValue(solver);
                    }
                    break;
            }
        }
    }
}
