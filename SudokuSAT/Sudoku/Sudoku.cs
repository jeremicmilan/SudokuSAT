using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Google.OrTools.Sat;

namespace SudokuSAT
{
    public class Sudoku
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BoxSize { get; private set; }

        public SudokuCell[,] SudokuGrid { get; private set; }
        public List<SudokuCell> SudokuCells
        {
            get
            {
                List<SudokuCell> sudokuCells = new();

                for (var column = 0; column < Width; column++)
                {
                    for (var row = 0; row < Height; row++)
                    {
                        sudokuCells.Add(SudokuGrid[column, row]);
                    }
                }

                return sudokuCells;
            }
        }

        public MainWindow Window { get; private set; }

        public Sudoku(int width, int height, int boxSize, MainWindow window)
        {
            Width = width;
            Height = height;
            SudokuGrid = new SudokuCell[Width, Height];
            BoxSize = boxSize;
            Window = window;
        }

        public void Solve()
        {
            CpSolver solver = new()
            {
                StringParameters = "enumerate_all_solutions:true"
            };

            CpModel model = GenerateModel();
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
                    foreach (var sudokuCell in SudokuGrid)
                    {
                        sudokuCell.UpdateSolvedValue(solver);
                    }
                    break;
            }
        }

        public void Explore()
        {
            new Thread(ExploreInternal).Start();
        }

        public void ExploreInternal()
        {
            Thread.CurrentThread.Name = "Explore";

            ParallelOptions parallelOptions = new();
            parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount - 2;
            Parallel.ForEach(SudokuCells, parallelOptions, (sudokuCell) =>
            {
                for (int i = SudokuCell.MinValue; i <= SudokuCell.MaxValue; i++)
                {
                    CpSolver solver = new();
                    BoundedLinearExpression boundedLinearExpression = sudokuCell.ValueVar == i;
                    CpModel model = GenerateModel();
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


        private CpModel GenerateModel()
        {
            CpModel model = new();
            AddCellConstraints(model); // This must always be first
            AddColumnConstraints(model);
            AddRowConstraints(model);
            AddBoxConstraints(model);
            return model;
        }

        private void AddCellConstraints(CpModel model)
        {
            foreach (SudokuCell sudokuCell in SudokuGrid)
            {
                sudokuCell.AddValueConstrainct(model);
            }
        }

        private void AddColumnConstraints(CpModel model)
        {
            for (var column = 0; column < Width; column++)
            {
                AddAllDifferent(model, GetColumn(column));
            }
        }

        private SudokuCell[] GetColumn(int columnNumber)
        {
            return Enumerable.Range(0, SudokuGrid.GetLength(0))
                    .Select(x => SudokuGrid[x, columnNumber])
                    .ToArray();
        }

        private void AddRowConstraints(CpModel model)
        {
            for (var row = 0; row < Height; row++)
            {
                AddAllDifferent(model, GetRow(row));
            }
        }

        private SudokuCell[] GetRow(int rowNumber)
        {
            return Enumerable.Range(0, SudokuGrid.GetLength(1))
                    .Select(x => SudokuGrid[rowNumber, x])
                    .ToArray();
        }

        private void AddBoxConstraints(CpModel model)
        {
            for (int columnBox = 0; columnBox <= Width / BoxSize; columnBox++)
            {
                for (int rowBox = 0; rowBox <= Height / BoxSize; rowBox++)
                {
                    List<SudokuCell> boxCells = new();

                    for (int column = columnBox * BoxSize;
                        column < Math.Min((columnBox + 1) * BoxSize, Width);
                        column++)
                    {
                        for (int row = rowBox * BoxSize;
                            row < Math.Min((rowBox + 1) * BoxSize, Height);
                            row++)
                        {
                            boxCells.Add(SudokuGrid[column, row]);
                        }
                    }

                    AddAllDifferent(model, boxCells);
                }
            }
        }

        private static void AddAllDifferent(CpModel model, IEnumerable<SudokuCell> cells)
        {
            model.AddAllDifferent(cells.Select(cell => cell.ValueVar));
        }
    }
}
