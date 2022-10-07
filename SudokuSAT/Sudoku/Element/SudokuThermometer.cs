using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
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

        public override void AddConstraints(CpModel model)
        {
            for (int i = 1; i < SudokuCells.Count; i++)
            {
                model.Add(SudokuCells[i].ValueVar > SudokuCells[i - 1].ValueVar);
            }
        }

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        protected override void VisualizeInternal()
        {
            double circleScalingFactor = .7;
            Point topLeftPosition = SudokuCells[0].TopLeftPosition;
            Grid.Children.Add(new Ellipse()
            {
                Width = SudokuCells[0].Grid.ActualWidth * circleScalingFactor,
                Height = SudokuCells[0].Grid.ActualHeight * circleScalingFactor,
                Margin = new Thickness(
                    topLeftPosition.X + SudokuCells[0].Grid.ActualWidth * (1 - circleScalingFactor) / 2,
                    topLeftPosition.Y + SudokuCells[0].Grid.ActualHeight * (1 - circleScalingFactor) / 2,
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
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
