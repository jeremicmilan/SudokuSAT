using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SudokuSAT
{
    public class Sudoku
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int BoxSize { get; set; }

        public SudokuCell[,] SudokuGrid { get; set; }
        public List<SudokuElement> SudokuElements { get; set; }
        public List<SudokuRuleset> SudokuRulesets { get; set; }

        public List<SudokuAction> SudokuActions { get; set; } = new();
        public List<SudokuAction> NextSudokuActions { get; set; } = new();

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
            BoxSize = boxSize;
            SudokuGrid = new SudokuCell[Width, Height];
            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    SudokuGrid[column, row] = new SudokuCell(sudoku: this, column, row);
                }
            }

            SudokuElements = new();
            SudokuRulesets = new();

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

            foreach (SudokuRuleset sudokuRuleset in SudokuRulesets)
            {
                sudoku.SudokuRulesets.Add(sudokuRuleset.Clone(sudoku));
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

        public List<List<SudokuCell>> GetRows()
        {
            List<List<SudokuCell>> rows = new();
            for (int row = 0; row < Height; row++)
            {
                rows.Add(GetRow(row));
            }

            return rows;
        }

        public List<SudokuCell> GetRow(int rowNumber)
        {
            return Enumerable.Range(0, SudokuGrid.GetLength(0))
                .Select(x => SudokuGrid[x, rowNumber])
                .ToList();
        }

        public List<List<SudokuCell>> GetColumns()
        {
            List<List<SudokuCell>> columns = new();
            for (int column = 0; column < Width; column++)
            {
                columns.Add(GetColumn(column));
            }

            return columns;
        }

        public List<SudokuCell> GetColumn(int columnNumber)
        {
            return Enumerable.Range(0, SudokuGrid.GetLength(1))
                .Select(x => SudokuGrid[columnNumber, x])
                .ToList();
        }

        public List<List<SudokuCell>> GetBoxes()
        {
            List<List<SudokuCell>> boxes = new();
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

                    boxes.Add(boxCells);
                }
            }

            return boxes;
        }

        public TSudokuRuleset? GetSudokuRuleset<TSudokuRuleset>()
            where TSudokuRuleset : SudokuRuleset
        {
            return (TSudokuRuleset?)SudokuRulesets
                ?.Where(ruleset => ruleset.GetType() == typeof(TSudokuRuleset))
                ?.FirstOrDefault();
        }

        public void RemoveRulesetIfExists<TSudokuRuleset>()
            where TSudokuRuleset : SudokuRuleset
        {
            TSudokuRuleset? sudokuRuleset = GetSudokuRuleset<TSudokuRuleset>();
            if (sudokuRuleset != null)
            {
                SudokuRulesets.Remove(sudokuRuleset);

            }
        }

        public void PerformRulesetAction<TSudokuRuleset>(TSudokuRuleset? sudokuRuleset)
            where TSudokuRuleset : SudokuRuleset
        {
            TSudokuRuleset? sudokuRulesetOld = GetSudokuRuleset<TSudokuRuleset>();
            bool sameAsOld = sudokuRuleset == null && sudokuRulesetOld == null
                || sudokuRuleset != null && sudokuRulesetOld != null && sudokuRuleset.Equal(sudokuRulesetOld);
            if (!sameAsOld)
            {
                PerformSudokuAction(new SudokuActionRuleset<TSudokuRuleset>(this, sudokuRuleset, sudokuRulesetOld));
            }
        }

        public void AddOrUpdateRuleset<TSudokuRuleset>(TSudokuRuleset sudokuRuleset)
            where TSudokuRuleset : SudokuRuleset
        {
            RemoveRulesetIfExists<TSudokuRuleset>();
            SudokuRulesets.Add(sudokuRuleset);
        }

        public CpModel GenerateModel()
        {
            CpModel model = new();
            AddCellConstraints(model); // This must always be first
            AddElementConstraints(model);
            AddRulesetConstraints(model);
            return model;
        }

        private void AddCellConstraints(CpModel model)
        {
            foreach (SudokuCell sudokuCell in SudokuGrid)
            {
                sudokuCell.AddValueConstrainct(model);
            }
        }

        private void AddElementConstraints(CpModel model)
        {
            foreach (SudokuElement sudokuElement in SudokuElements)
            {
                sudokuElement.AddConstraints(model);
            }
        }

        private void AddRulesetConstraints(CpModel model)
        {
            foreach (SudokuRuleset sudokuRuleset in SudokuRulesets)
            {
                sudokuRuleset.AddConstraints(model);
            }
        }

        public void Undo()
        {
            if (SudokuActions.Any())
            {
                SudokuAction sudokuAction = SudokuActions.Last();
                sudokuAction.Undo();
                SudokuActions.Remove(sudokuAction);
                NextSudokuActions.Add(sudokuAction);

                Visualize();
                MainWindow.SolveAsync();
            }
        }

        public void Redo()
        {
            if (NextSudokuActions.Any())
            {
                SudokuAction sudokuAction = NextSudokuActions.Last();
                sudokuAction.Redo();
                NextSudokuActions.Remove(sudokuAction);
                SudokuActions.Add(sudokuAction);

                Visualize();
                MainWindow.SudokuSolver.Solve(this, updateSolvedValue: false);
            }
        }

        public void PerformSudokuAction(SudokuAction sudokuAction, bool solver = false)
        {
            if (!solver)
            {
                MainWindow.SudokuSolver.CheckIsSolveActive();
                MainWindow.SudokuSolver.ClearExploreIfNeeded(this);
            }

            SudokuActions.Add(sudokuAction);
            NextSudokuActions.Clear();

            sudokuAction.Redo();
            Visualize();

            MainWindow.UpdateUndoRedoToolbar();
            MainWindow.SolveAsync();
        }

        public void AddElement(SudokuElement sudokuElement)
        {
            PerformSudokuAction(new SudokuActionElement(this, sudokuElement));
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

        public void Visualize(bool recreateGrid = false)
        {
            Debug.Assert(Grid != null);

            if (recreateGrid)
            {
                Grid.Children.Clear();
            }

            if (recreateGrid || SudokuUniformGrid == null)
            {
                SudokuUniformGrid = new()
                {
                    Rows = Height,
                    Columns = Width,
                    Name = "SudokuCells"
                };
                Grid.Children.Add(SudokuUniformGrid);
            }

            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    if (recreateGrid)
                    {
                        Border border = CreateBorder(column, row);
                        SudokuUniformGrid.Children.Add(border);

                        Grid grid = new();
                        border.Child = grid;

                        SudokuGrid[column, row].Grid = grid;
                    }

                    SudokuGrid[column, row].Visualize(recreateGrid);
                }
            }

            if (recreateGrid)
            {
                foreach (SudokuElement sudokuElement in SudokuElements)
                {
                    sudokuElement.Grid = new Grid();
                    Grid.Children.Add(sudokuElement.Grid);
                    sudokuElement.Visualize();
                }
            }
        }
    }
}
