using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuVisual : Sudoku
    {
        public Grid SudokuPlaceholder { get; private set; }

        public SudokuVisual(int width, int height, int boxSize, Grid sudokuPlaceholder)
            : base(width, height, boxSize)
        {
            SudokuPlaceholder = sudokuPlaceholder;
        }

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        public List<SudokuCellVisual> SudokuCellsVisual => SudokuCells.Select(cell => cell as SudokuCellVisual).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

        public List<SudokuCellVisual> SelectedSudokuCells => SudokuCellsVisual
            .Where(cell => cell.IsSelected)
            .OrderBy(cell => cell.SelectionOrderId)
            .ToList();

        public void ClearSelection()
        {
            foreach (SudokuCellVisual sudokuCell in SelectedSudokuCells)
            {
                sudokuCell.SetIsSelected(isSelected: false);
            }

            SudokuCellVisual.GlobalSelectionCount = 0;
        }

        public void GenerateGrid()
        {
            UniformGrid sudokuGrid = new()
            {
                Rows = Height,
                Columns = Width
            };

            for (var row = 0; row < Height; row++)
            {
                for (var column = 0; column < Width; column++)
                {
                    Border border = CreateBorder(column, row);
                    sudokuGrid.Children.Add(border);

                    Grid grid = new();
                    border.Child = grid;
                    grid.Children.Add(new Label()); // for clicking
                    SudokuCellVisual sudokuCell = new(grid, this, column, row);
                    grid.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler((_, _) =>
                    {
                        bool isSelected = sudokuCell.IsSelected;
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            ClearSelection();
                        }

                        sudokuCell.SetIsSelected(!isSelected);
                    }));
                    SudokuGrid[column, row] = sudokuCell;
                }
            }

            SudokuPlaceholder?.Children.Add(sudokuGrid);
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
                BorderBrush = Brushes.Black
            };
        }
    }
}
