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
    public class SudokuArrow : SudokuElementLine
    {
        public SudokuArrow(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuArrow(sudoku, SudokuCells, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            model.Add(SudokuCells[0].ValueVar == LinearExpr.Sum(SudokuCells.Skip(1).Select(cell => cell.ValueVar)))
                .OnlyEnforceIf(boolVar);
        }

        public override void AddNegativeConstraints(CpModel model, BoolVar boolVar)
        {
            model.Add(SudokuCells[0].ValueVar != LinearExpr.Sum(SudokuCells.Skip(1).Select(cell => cell.ValueVar)))
                .OnlyEnforceIf(boolVar);
        }

        protected override void VisualizeInternal()
        {
            Debug.Assert(Grid != null);
            Debug.Assert(Grid.Children.Count == 0);

            // Arrow circle
            //
            double circleScalingFactor = .7;
            SudokuCell firstSudokuCell = SudokuCells[0];
            Debug.Assert(firstSudokuCell.Grid != null);
            Ellipse ellipse = new()
            {
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                IsHitTestVisible = false,
            };
            void updateEllipsePosition()
            {
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

            for (int i = 1; i < SudokuCells.Count; i++)
            {
                SudokuCell sudokuCell1 = SudokuCells[i - 1];
                SudokuCell sudokuCell2 = SudokuCells[i];
                Debug.Assert(sudokuCell1.Grid != null);
                double coeficient = (sudokuCell1.OrthoAdjacent(sudokuCell2) ? 1 : (Math.Sqrt(2) / 2)) * circleScalingFactor / 2;

                Line line = new()
                {
                    Stroke = Brushes.Black,
                    IsHitTestVisible = false,
                };
                void updateLinePosition()
                {
                    Point position1 = sudokuCell1.CenterPosition;
                    Point position2 = sudokuCell2.CenterPosition;

                    int columnDirection = sudokuCell2.Column - sudokuCell1.Column;
                    int rowDirection = sudokuCell2.Row - sudokuCell1.Row;
                    if (sudokuCell1 == firstSudokuCell)
                    {
                        // First line should start from circle edge and not the center
                        //
                        position1.X += coeficient * columnDirection * sudokuCell1.Grid.ActualWidth;
                        position1.Y += coeficient * rowDirection * sudokuCell1.Grid.ActualHeight;
                    }

                    line.X1 = position1.X;
                    line.Y1 = position1.Y;
                    line.X2 = position2.X;
                    line.Y2 = position2.Y;
                }
                updateLinePosition();
                sudokuCell1.Grid.SizeChanged += (_, _) => updateLinePosition();
                Grid.Children.Add(line);

                if (i == SudokuCells.Count - 1)
                {
                    // Arrow tip
                    //
                    foreach (int rotationDirection in new[] { -1, 1 })
                    {
                        Line tipLine = new()
                        {
                            Stroke = Brushes.Black,
                            IsHitTestVisible = false,
                        };
                        void updateTipLinePosition()
                        {
                            Point position1 = sudokuCell1.CenterPosition;
                            Point position2 = sudokuCell2.CenterPosition;
                            tipLine.X1 = position2.X - (position2.X - position1.X) * coeficient;
                            tipLine.Y1 = position2.Y - (position2.Y - position1.Y) * coeficient;
                            tipLine.X2 = position2.X;
                            tipLine.Y2 = position2.Y;
                            tipLine.RenderTransform = new RotateTransform(rotationDirection * 10)
                            {
                                CenterX = position2.X,
                                CenterY = position2.Y
                            };
                        }
                        updateTipLinePosition();
                        sudokuCell1.Grid.SizeChanged += (_, _) => updateTipLinePosition();
                        Grid.Children.Add(tipLine);
                    }
                }
            }
        }
    }
}
