using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSAT
{
    public abstract class SudokuElementPair : SudokuElementLine
    {
        public SudokuCell FirstSudokuCell { get { Debug.Assert(SudokuCells.Count == 2); return SudokuCells[0]; } }
        public SudokuCell SecondSudokuCell { get { Debug.Assert(SudokuCells.Count == 2); return SudokuCells[1]; } }

        public SudokuElementPair(
            Sudoku sudoku,
            SudokuCell first,
            SudokuCell second,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, new List<SudokuCell> { first, second }, sudokuElementId, grid)
        { }

        public SudokuElementPair(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        {
            if (sudokuCells.Count != 2)
            {
                throw new Exception("A pair must contain exactly two cells.");
            }
        }

        protected void VisualizeContentOnLine(string content)
        {
            Debug.Assert(Grid != null);
            Debug.Assert(Grid.Children.Count == 0);
            Debug.Assert(FirstSudokuCell.Grid != null);
            Debug.Assert(SecondSudokuCell.Grid != null);

            Label label = new()
            {
                Content = content,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Black,
                IsHitTestVisible = false,
            };
            void updateLabelPosition()
            {
                double labelScalingFactor = 0.4;
                Point centerPosition = FirstSudokuCell.CenterPosition;
                label.Width = FirstSudokuCell.Grid.ActualWidth;
                label.Height = FirstSudokuCell.Grid.ActualHeight;
                label.FontSize = FirstSudokuCell.Grid.ActualWidth * labelScalingFactor;
                Point midpointPosition = new(
                    (FirstSudokuCell.CenterPosition.X + SecondSudokuCell.CenterPosition.X) / 2,
                    (FirstSudokuCell.CenterPosition.Y + SecondSudokuCell.CenterPosition.Y) / 2);
                label.Margin = new Thickness(
                    midpointPosition.X - FirstSudokuCell.Grid.ActualWidth / 2,
                    midpointPosition.Y - FirstSudokuCell.Grid.ActualHeight / 2,
                    0,
                    0);
            }
            updateLabelPosition();
            FirstSudokuCell.Grid.SizeChanged += (_, _) => updateLabelPosition();
            SecondSudokuCell.Grid.SizeChanged += (_, _) => updateLabelPosition();
            Grid.Children.Add(label);
        }
    }
}
