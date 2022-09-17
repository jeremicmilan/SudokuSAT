﻿using System.Threading.Tasks;
using System.Threading;
using System;
using Google.OrTools.Sat;
using System.Windows;

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
                MaxDegreeOfParallelism = sudoku.Height
            };
            Parallel.ForEach(sudoku.SudokuCells, parallelOptions, (sudokuCell) =>
            {
                for (int i = SudokuCell.MinValue; i <= SudokuCell.MaxValue; i++)
                {
                    CpSolver solver = new();
                    BoundedLinearExpression boundedLinearExpression = sudokuCell.ValueVar == i;
                    CpModel model = sudoku.GenerateModel();
                    model.Add(sudokuCell.ValueVar == i);
                    CpSolverStatus solverStatus = solver.Solve(model);
                    switch (solverStatus)
                    {
                        case CpSolverStatus.Unknown:
                        case CpSolverStatus.ModelInvalid:
                            Window.statusLabel.Content = "Solver status: " + solverStatus;
                            return;

                        case CpSolverStatus.Infeasible:
                            Window.solutionCount.Content = 0;
                            break;

                        case CpSolverStatus.Feasible:
                        case CpSolverStatus.Optimal:
                            sudokuCell.PossibleValues.Add(i);
                            Window.Dispatcher.Invoke(() =>
                            {
                                sudokuCell.SolutionsLabel.Content = sudokuCell.PossibleValues.Count;
                            });
                            break;
                    }
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
                    Window.statusLabel.Content = "Solver status: " + solverStatus;
                    return;

                case CpSolverStatus.Infeasible:
                    Window.solutionCount.Content = 0;
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
