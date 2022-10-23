using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuThermometer : SudokuElementLine
    {
        public SudokuThermometer(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            Grid? grid = null)
        {
            return new SudokuThermometer(sudoku, sudokuCells, grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            for (int i = 1; i < SudokuCells.Count; i++)
            {
                model.Add(SudokuCells[i].ValueVar > SudokuCells[i - 1].ValueVar).OnlyEnforceIf(boolVar);
            }
        }

        public override void Visualize()
        {
            Debug.Assert(Grid != null);
            Debug.Assert(Grid.Children.Count == 0);

            VisualizeLine(Brushes.Gray);

            // Thermometer bulb
            //
            SudokuCell firstSudokuCell = SudokuCells[0];
            Debug.Assert(firstSudokuCell.Grid != null);
            Ellipse ellipse = new()
            {
                Fill = Brushes.Gray,
                Opacity = .5,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false,
            };
            void updateEllipsePosition()
            {
                double circleScalingFactor = .7;
                Point topLeftPosition = firstSudokuCell.TopLeftPosition;
                ellipse.Width = firstSudokuCell.Grid.ActualWidth * circleScalingFactor;
                ellipse.Height = firstSudokuCell.Grid.ActualHeight * circleScalingFactor;
                ellipse.Margin = new Thickness(
                    topLeftPosition.X + firstSudokuCell.Grid.ActualWidth * (1 - circleScalingFactor) / 2,
                    topLeftPosition.Y + firstSudokuCell.Grid.ActualHeight * (1 - circleScalingFactor) / 2,
                    0,
                    0);

            }
            updateEllipsePosition();
            firstSudokuCell.Grid.SizeChanged += (_, _) => updateEllipsePosition();
            Grid.Children.Add(ellipse);
        }
    }
}
