using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuCellVisual : SudokuCell
    {
        public Grid Grid;

        public bool IsSelected { get; set; } = false;
        public virtual SudokuVisual SudokuVisual => (SudokuVisual)Sudoku;

        public SudokuCellVisual(Grid grid, SudokuVisual sudoku, int column, int row, int? value = null)
            : base (sudoku, column, row, value)
        {
            Grid = grid;
        }

#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static int GlobalSelectionCount = 0;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        public int? SelectionOrderId = null;
        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            if (isSelected)
            {
                SelectionOrderId = GlobalSelectionCount++;
                Grid.Background = Brushes.Yellow;
            }
            else
            {
                SelectionOrderId = null;
                Grid.Background = null;
            }
        }

        private readonly Dictionary<ValueType, SolidColorBrush> digitToColor = new()
        {
            { ValueType.Given,  Brushes.Black },
            { ValueType.Solver, Brushes.Green },
            { ValueType.User,   Brushes.Blue  }
        };

        public override void SetValue(int value, ValueType valueType)
        {
            base.SetValue(value, valueType);

            Grid.Children.Add(new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                MinWidth = Grid.ActualWidth,
                MinHeight = Grid.ActualHeight,
                FontSize = Grid.ActualHeight * 0.65,
                Foreground = digitToColor[valueType],
                Content = value > 0 ? value : "X"
            });
        }

        public override void ClearValue()
        {
            base.ClearValue();

            Grid.Children.Clear();
            Grid.Children.Add(new Label()); // dummy label for selecting
        }

        public Point CenterPosition => Grid.TranslatePoint(
            new Point(.5 * Grid.ActualWidth, .5 * Grid.ActualHeight),
            SudokuVisual.SudokuPlaceholder);
        public Point TopLeftPosition => Grid.TranslatePoint(new Point(0, 0), SudokuVisual.SudokuPlaceholder);
    }
}
