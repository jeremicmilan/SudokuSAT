using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Google.OrTools.Sat;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Diagnostics;

namespace SudokuSAT
{
    public class Sudoku
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int BoxSize { get; set; }

        public SudokuCell[,] SudokuGrid { get; set; }
        public List<SudokuElement> SudokuElements { get; set; }
        public Stack<SudokuAction> SudokuActions { get; private set; } = new();
        public Stack<SudokuAction> NextSudokuActions { get; private set; } = new();

        [JsonIgnore] public Grid? Grid { get; set; }
        [JsonIgnore] private UniformGrid? SudokuUniformGrid { get; set; }
        [JsonIgnore] public MainWindow MainWindow => (MainWindow)Window.GetWindow(Grid);

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Sudoku() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Sudoku(int width, int height, int boxSize, Grid? grid = null)
        {
            Width = width;
            Height = height;
            SudokuGrid = new SudokuCell[Width, Height];
            SudokuElements = new();
            BoxSize = boxSize;
            Grid = grid;
        }

        public Sudoku Clone()
        {
            Sudoku sudoku = new(Width, Height, BoxSize);
            foreach (var cell in SudokuCells)
            {
                sudoku.SudokuGrid[cell.Column, cell.Row] = cell.Clone(sudoku);
            }

            foreach (SudokuElement sudokuElement in SudokuElements)
            {
                sudoku.SudokuElements.Add(sudokuElement.Clone(sudoku));
            }

            return sudoku;
        }

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

        public void Undo()
        {
            if (SudokuActions.Any())
            {
                SudokuAction sudokuAction = SudokuActions.Pop();
                sudokuAction.Undo();
                NextSudokuActions.Push(sudokuAction);

                Visualize();
                MainWindow.SudokuSolver.Solve(this, updateSolvedValue: false);
            }
        }

        public void Redo()
        {
            if (NextSudokuActions.Any())
            {
                SudokuAction sudokuAction = NextSudokuActions.Pop();
                sudokuAction.Redo();
                SudokuActions.Push(sudokuAction);

                Visualize();
                MainWindow.SudokuSolver.Solve(this, updateSolvedValue: false);
            }
        }

        public CpModel GenerateModel()
        {
            CpModel model = new();
            AddCellConstraints(model); // This must always be first
            AddColumnConstraints(model);
            AddRowConstraints(model);
            AddBoxConstraints(model);
            AddElementConstraints(model);
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
            return Enumerable.Range(0, SudokuGrid.GetLength(1))
                    .Select(x => SudokuGrid[columnNumber, x])
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
            return Enumerable.Range(0, SudokuGrid.GetLength(0))
                    .Select(x => SudokuGrid[x, rowNumber])
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

        private void AddElementConstraints(CpModel model)
        {
            foreach (SudokuElement sudokuElement in SudokuElements)
            {
                sudokuElement.AddConstraints(model);
            }
        }

        public void PerformSudokuAction(SudokuAction sudokuAction)
        {
            SudokuActions.Push(sudokuAction);
            NextSudokuActions.Clear();

            sudokuAction.Redo();
            Visualize();

            MainWindow.UpdateUndoRedoButtons();
            MainWindow.SudokuSolver.Solve(this, updateSolvedValue: false);
        }

        public void AddElement(SudokuElement sudokuElement)
        {
            PerformSudokuAction(new SudokuActionElement(this, sudokuElement));
        }

        public void RemoveElement(SudokuElement sudokuElement)
        {
            SudokuElements.Remove(sudokuElement);
        }

        public void SetValues(int? value, List<SudokuCell> sudokuCells)
        {
            PerformSudokuAction(new SudokuActionsValue(this, sudokuCells, value, ValueType.Given));
        }

        public List<SudokuCell> SelectedSudokuCells => SudokuCells
            .Where(cell => cell.IsSelected)
            .OrderBy(cell => cell.SelectionOrderId)
            .ToList();

        public void ClearSelection()
        {
            foreach (SudokuCell sudokuCell in SelectedSudokuCells)
            {
                sudokuCell.SetIsSelected(isSelected: false);
            }

            SudokuCell.ClearGlobalSelectionCount();
        }

        private Border CreateBorder(int column, int row)
        {
            int thick = 3, thin = 1;
            var top = row % BoxSize == 0 ? thick : thin;
            var left = column % BoxSize == 0 ? thick : thin;
            var bottom = row == Height - 1 ? thick : 0;
            var right = column == Width - 1 ? thick : 0;

            return new Border
            {
                BorderThickness = new Thickness(left, top, right, bottom),
                BorderBrush = Brushes.Black,
            };
        }

        public void Visualize(bool clearGrid = false)
        {
            Debug.Assert(Grid != null);

            if (clearGrid || SudokuUniformGrid == null)
            {
                SudokuUniformGrid = new()
                {
                    Rows = Height,
                    Columns = Width,
                    Name = "SudokuCells"
                };
            }

            Grid.Children.Clear();
            Grid.Children.Add(SudokuUniformGrid);

            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    if (clearGrid || SudokuGrid[column, row] == null)
                    {
                        Border border = CreateBorder(column, row);
                        SudokuUniformGrid.Children.Add(border);

                        Grid grid = new();
                        border.Child = grid;

                        SudokuGrid[column, row] = new SudokuCell(
                            sudoku: this,
                            column: column,
                            row: row,
                            grid: grid);
                    }

                    SudokuGrid[column, row].Visualize();
                }
            }

            foreach (SudokuElement sudokuElement in SudokuElements)
            {
                sudokuElement.Visualize();
            }
        }
    }
}
