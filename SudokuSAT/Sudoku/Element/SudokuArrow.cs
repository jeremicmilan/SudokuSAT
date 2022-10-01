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
    public class SudokuArrow : SudokuElementWithCellListAdjacent
    {
        public SudokuArrow(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuArrow(sudoku, sudokuCells);
        }

        public override void AddConstraints(CpModel model)
        {
            BoundedLinearExpression boundedLinearExpression = SudokuCells[0].ValueVar ==
                LinearExpr.Sum(SudokuCells.Skip(1).Select(cell => cell.ValueVar));

            model.Add(boundedLinearExpression);
        }

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        public override void Visualize()
        {
            for (int i = 0; i < SudokuCells.Count; i++)
            {
                double circleScalingFactor = .7;

                if (i == 0)
                {
                    // Arrow circle beginning
                    //
                    Point topLeftPosition = SudokuCells[i].TopLeftPosition;
                    Grid.Children.Add(new Ellipse()
                    {
                        Width = SudokuCells[i].Grid.ActualWidth * circleScalingFactor,
                        Height = SudokuCells[i].Grid.ActualHeight * circleScalingFactor,
                        Margin = new Thickness(
                            topLeftPosition.X + SudokuCells[i].Grid.ActualWidth  * (1 - circleScalingFactor) / 2,
                            topLeftPosition.Y + SudokuCells[i].Grid.ActualHeight * (1 - circleScalingFactor) / 2,
                            0,
                            0),
                        Stroke = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        IsHitTestVisible = false,
                    });
                }
                else
                {
                    SudokuCell sudokuCell1 = SudokuCells[i - 1];
                    SudokuCell sudokuCell2 = SudokuCells[i];
                    Point position1 = sudokuCell1.CenterPosition;
                    Point position2 = sudokuCell2.CenterPosition;

                    int columnDirection = sudokuCell2.Column - sudokuCell1.Column;
                    int rowDirection = sudokuCell2.Row - sudokuCell1.Row;
                    double coeficient = (sudokuCell1.OrthoAdjacent(sudokuCell2) ? 1 : (Math.Sqrt(2) / 2)) * circleScalingFactor / 2;
                    if (i == 1)
                    {
                        // First line should start from circle edge and not the center
                        //
                        position1.X += coeficient * columnDirection * sudokuCell1.Grid.ActualWidth;
                        position1.Y += coeficient * rowDirection * sudokuCell1.Grid.ActualHeight;
                    }

                    Grid.Children.Add(new Line()
                    {
                        X1 = position1.X,
                        Y1 = position1.Y,
                        X2 = position2.X,
                        Y2 = position2.Y,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    });

                    if (i == SudokuCells.Count - 1)
                    {
                        // Arrow tip
                        //
                        position1 = sudokuCell1.CenterPosition;
                        position2 = sudokuCell2.CenterPosition;
                        foreach (int rotationDirection in new[] { -1, 1 })
                        {
                            Grid.Children.Add(new Line()
                            {
                                X1 = position2.X - (position2.X - position1.X) * coeficient,
                                Y1 = position2.Y - (position2.Y - position1.Y) * coeficient,
                                X2 = position2.X,
                                Y2 = position2.Y,
                                Stroke = Brushes.Black,
                                RenderTransform = new RotateTransform(rotationDirection * 10)
                                {
                                    CenterX = position2.X,
                                    CenterY = position2.Y
                                },
                                IsHitTestVisible = false,\
                            });
                        }
                    }
                }
            }

            Sudoku.Grid.Children.Add(Grid);
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
