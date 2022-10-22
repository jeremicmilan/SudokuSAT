using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuThermometer : SudokuElementLine
    {
        public SudokuThermometer(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuThermometer(sudoku, sudokuCells);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            for (int i = 1; i < SudokuCells.Count; i++)
            {
                model.Add(SudokuCells[i].ValueVar > SudokuCells[i - 1].ValueVar).OnlyEnforceIf(boolVar);
            }
        }

        protected override void VisualizeInternal()
        {
            Debug.Assert(Grid != null);
            SudokuCell firstSudokuCell = SudokuCells[0];
            Debug.Assert(firstSudokuCell.Grid != null);
            double circleScalingFactor = .7;
            Point topLeftPosition = firstSudokuCell.TopLeftPosition;
            Grid.Children.Add(new Ellipse()
            {
                Width = firstSudokuCell.Grid.ActualWidth * circleScalingFactor,
                Height = firstSudokuCell.Grid.ActualHeight * circleScalingFactor,
                Margin = new Thickness(
                    topLeftPosition.X + firstSudokuCell.Grid.ActualWidth * (1 - circleScalingFactor) / 2,
                    topLeftPosition.Y + firstSudokuCell.Grid.ActualHeight * (1 - circleScalingFactor) / 2,
                    0,
                    0),
                Fill = Brushes.Gray,
                Opacity = .5,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false,
            });

            Grid.Children.Add(new Polyline
            {
                Points = new PointCollection(SudokuCells.Select(cell => cell.CenterPosition)),
                Stroke = Brushes.Gray,
                StrokeThickness = 15,
                Opacity = .5,
                IsHitTestVisible = false,
            });
        }
    }
}
