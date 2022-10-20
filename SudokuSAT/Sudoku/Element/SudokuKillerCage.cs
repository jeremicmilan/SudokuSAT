using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuKillerCage : SudokuElementCage
    {
        public int? Sum { get; private set; }

        public SudokuKillerCage(Sudoku sudoku, List<SudokuCell> sudokuCells, int? sum, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        {
            Sum = sum > 0 ? sum : null;
            if (Sum < 0 || Sum > 45)
            {
                throw new Exception("Killer cage sum must be between 0 and 45 or non-existant.");
            }
        }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuKillerCage(sudoku, sudokuCells, Sum);
        }

        public override void AddConstraints(CpModel model)
        {
            model.AddAllDifferent(SudokuCells.Select(cell => cell.ValueVar));

            if (Sum != null)
            {
                model.Add(LinearExpr.Sum(SudokuCells.Select(cell => cell.ValueVar)) == Sum.Value);
            }
        }

        protected override void VisualizeInternal()
        {
            Debug.Assert(Grid != null);
            int minimumDistance = SudokuCells.Min(cell => cell.Column + cell.Row);
            SudokuCell? topLeftmostSudokuCell = SudokuCells.Where(cell => cell.Column + cell.Row == minimumDistance).MinBy(cell => cell.Column);

            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                Debug.Assert(sudokuCell.Grid != null);
                double offsetFactor = 0.08;
                double widthOffset  = sudokuCell.Grid.ActualWidth  * offsetFactor;
                double heightOffset = sudokuCell.Grid.ActualHeight * offsetFactor;
                double strokeDashLength = (sudokuCell.Grid.ActualWidth + sudokuCell.Grid.ActualHeight) / 30;
                double offsetForSum = 0;
                if (topLeftmostSudokuCell != null && sudokuCell == topLeftmostSudokuCell && Sum != null)
                {
                    offsetForSum = sudokuCell.Grid.ActualHeight * 0.18;

                    Grid.Children.Add(new Label()
                    {
                        Content = Sum,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = new TranslateTransform(
                            topLeftmostSudokuCell.TopLeftPosition.X,
                            topLeftmostSudokuCell.TopLeftPosition.Y),
                        FontSize = offsetForSum * 0.8,
                        Foreground = Brushes.Black,
                        IsHitTestVisible = false,
                    });
                }

                if (sudokuCell.Top == null || !SudokuCells.Contains(sudokuCell.Top))
                {
                    int leftOffsetDirection =
                        sudokuCell.Left != null && SudokuCells.Contains(sudokuCell.Left) &&
                        sudokuCell.Left.Top != null && SudokuCells.Contains(sudokuCell.Left.Top)
                        ? -1 : 1;
                    int rightOffsetDirection = sudokuCell.Right != null && SudokuCells.Contains(sudokuCell.Right) ? 1 : -1;
                    Grid.Children.Add(new Line()
                    {
                        X1 = sudokuCell.TopLeftPosition.X + widthOffset * leftOffsetDirection + offsetForSum,
                        Y1 = sudokuCell.TopLeftPosition.Y + heightOffset,
                        X2 = sudokuCell.TopRightPosition.X + widthOffset * rightOffsetDirection,
                        Y2 = sudokuCell.TopRightPosition.Y + heightOffset,
                        StrokeDashArray = new DoubleCollection(new[] { strokeDashLength, strokeDashLength }),
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    });
                }

                if (sudokuCell.Right == null || !SudokuCells.Contains(sudokuCell.Right))
                {
                    int topOffsetDirection =
                        sudokuCell.Top != null && SudokuCells.Contains(sudokuCell.Top) &&
                        sudokuCell.Top.Right != null && SudokuCells.Contains(sudokuCell.Top.Right)
                        ? -1 : 1;
                    int bottomOffsetDirection = sudokuCell.Bottom != null && SudokuCells.Contains(sudokuCell.Bottom) ? 1 : -1;
                    Grid.Children.Add(new Line()
                    {
                        X1 = sudokuCell.TopRightPosition.X - widthOffset,
                        Y1 = sudokuCell.TopRightPosition.Y + heightOffset * topOffsetDirection,
                        X2 = sudokuCell.BottomRightPosition.X - widthOffset,
                        Y2 = sudokuCell.BottomRightPosition.Y + heightOffset * bottomOffsetDirection,
                        StrokeDashArray = new DoubleCollection(new[] { strokeDashLength, strokeDashLength }),
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    });
                }

                if (sudokuCell.Bottom == null || !SudokuCells.Contains(sudokuCell.Bottom))
                {
                    int rightOffsetDirection =
                        sudokuCell.Right != null && SudokuCells.Contains(sudokuCell.Right) &&
                        sudokuCell.Right.Bottom != null && SudokuCells.Contains(sudokuCell.Right.Bottom)
                        ? 1 : -1;
                    int leftOffsetDirection = sudokuCell.Left != null && SudokuCells.Contains(sudokuCell.Left) ? -1 : 1;
                    Grid.Children.Add(new Line()
                    {
                        X1 = sudokuCell.BottomRightPosition.X + widthOffset * rightOffsetDirection,
                        Y1 = sudokuCell.BottomRightPosition.Y - heightOffset,
                        X2 = sudokuCell.BottomLeftPosition.X + widthOffset * leftOffsetDirection,
                        Y2 = sudokuCell.BottomLeftPosition.Y - heightOffset,
                        StrokeDashArray = new DoubleCollection(new[] { strokeDashLength, strokeDashLength }),
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    });
                }

                if (sudokuCell.Left == null || !SudokuCells.Contains(sudokuCell.Left))
                {
                    int bottomOffsetDirection =
                        sudokuCell.Bottom != null && SudokuCells.Contains(sudokuCell.Bottom) &&
                        sudokuCell.Bottom.Left != null && SudokuCells.Contains(sudokuCell.Bottom.Left)
                        ? 1 : -1;
                    int topOffsetDirection = sudokuCell.Top != null && SudokuCells.Contains(sudokuCell.Top) ? -1 : 1;
                    Grid.Children.Add(new Line()
                    {
                        X1 = sudokuCell.BottomLeftPosition.X + widthOffset,
                        Y1 = sudokuCell.BottomLeftPosition.Y + heightOffset * bottomOffsetDirection,
                        X2 = sudokuCell.TopLeftPosition.X + widthOffset,
                        Y2 = sudokuCell.TopLeftPosition.Y + heightOffset * topOffsetDirection + offsetForSum,
                        StrokeDashArray = new DoubleCollection(new[] { strokeDashLength, strokeDashLength }),
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    });
                }
            }
        }
    }
}
