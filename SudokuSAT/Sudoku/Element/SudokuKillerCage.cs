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
    public class SudokuKillerCage : SudokuElementCage
    {
        public int? Sum { get; private set; }

        public SudokuKillerCage(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sum,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        {
            Sum = sum > 0 ? sum : null;
            if (Sum < 0 || Sum > 45)
            {
                throw new Exception("Killer cage sum must be between 0 and 45 or non-existent.");
            }
        }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuKillerCage(sudoku, SudokuCells, Sum, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            model.AddAllDifferent(SudokuCells.Select(cell => cell.ValueVar))
                .OnlyEnforceIf(boolVar);

            if (Sum != null)
            {
                model.Add(LinearExpr.Sum(SudokuCells.Select(cell => cell.ValueVar)) == Sum.Value)
                    .OnlyEnforceIf(boolVar);
            }
        }

        protected override void VisualizeInternal()
        {
            Debug.Assert(Grid != null);
            Debug.Assert(Grid.Children.Count == 0);
            int minimumDistance = SudokuCells.Min(cell => cell.Column + cell.Row);
            SudokuCell? topLeftmostSudokuCell = SudokuCells
                .Where(cell => cell.Column + cell.Row == minimumDistance)
                .MinBy(cell => cell.Column);

            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                Debug.Assert(sudokuCell.Grid != null);
                double offsetFactor = 0.08;
                double widthOffset() { return sudokuCell.Grid.ActualWidth * offsetFactor; }
                double heightOffset() { return sudokuCell.Grid.ActualHeight * offsetFactor; }
                double strokeDashLength() { return (sudokuCell.Grid.ActualWidth + sudokuCell.Grid.ActualHeight) / 30; }
                double offsetForSum = 0;
                if (topLeftmostSudokuCell != null && sudokuCell == topLeftmostSudokuCell && Sum != null)
                {
                    offsetForSum = (sudokuCell.Grid.ActualHeight + 1) * 0.18;

                    Label sumLabel = new()
                    {
                        Content = Sum,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        FontSize = offsetForSum * 0.8,
                        Foreground = Brushes.Black,
                        IsHitTestVisible = false,
                    };
                    void updateSumLabelPosition()
                    {
                        sumLabel.RenderTransform = new TranslateTransform(
                            topLeftmostSudokuCell.TopLeftPosition.X,
                            topLeftmostSudokuCell.TopLeftPosition.Y);
                    }
                    sudokuCell.Grid.SizeChanged += (_, _) => updateSumLabelPosition();
                    Grid.Children.Add(sumLabel);
                }

                if (sudokuCell.Top == null || !SudokuCells.Contains(sudokuCell.Top))
                {
                    int leftOffsetDirection =
                        sudokuCell.Left != null && SudokuCells.Contains(sudokuCell.Left) &&
                        sudokuCell.Left.Top != null && SudokuCells.Contains(sudokuCell.Left.Top)
                        ? -1 : 1;
                    int rightOffsetDirection = sudokuCell.Right != null && SudokuCells.Contains(sudokuCell.Right) ? 1 : -1;
                    Line line = new()
                    {
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    };
                    void updateLinePosition()
                    {
                        line.X1 = sudokuCell.TopLeftPosition.X + widthOffset() * leftOffsetDirection + offsetForSum;
                        line.Y1 = sudokuCell.TopLeftPosition.Y + heightOffset();
                        line.X2 = sudokuCell.TopRightPosition.X + widthOffset() * rightOffsetDirection;
                        line.Y2 = sudokuCell.TopRightPosition.Y + heightOffset();
                        line.StrokeDashArray = new DoubleCollection(new[] { strokeDashLength(), strokeDashLength() });
                    }
                    updateLinePosition();
                    sudokuCell.Grid.SizeChanged += (_, _) => updateLinePosition();
                    Grid.Children.Add(line);
                }

                if (sudokuCell.Right == null || !SudokuCells.Contains(sudokuCell.Right))
                {
                    int topOffsetDirection =
                        sudokuCell.Top != null && SudokuCells.Contains(sudokuCell.Top) &&
                        sudokuCell.Top.Right != null && SudokuCells.Contains(sudokuCell.Top.Right)
                        ? -1 : 1;
                    int bottomOffsetDirection = sudokuCell.Bottom != null && SudokuCells.Contains(sudokuCell.Bottom) ? 1 : -1;
                    Line line = new()
                    {
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    };
                    void updateLinePosition()
                    {
                        line.X1 = sudokuCell.TopRightPosition.X - widthOffset();
                        line.Y1 = sudokuCell.TopRightPosition.Y + heightOffset() * topOffsetDirection;
                        line.X2 = sudokuCell.BottomRightPosition.X - widthOffset();
                        line.Y2 = sudokuCell.BottomRightPosition.Y + heightOffset() * bottomOffsetDirection;
                        line.StrokeDashArray = new DoubleCollection(new[] { strokeDashLength(), strokeDashLength() });
                    }
                    updateLinePosition();
                    sudokuCell.Grid.SizeChanged += (_, _) => updateLinePosition();
                    Grid.Children.Add(line);
                }

                if (sudokuCell.Bottom == null || !SudokuCells.Contains(sudokuCell.Bottom))
                {
                    int rightOffsetDirection =
                        sudokuCell.Right != null && SudokuCells.Contains(sudokuCell.Right) &&
                        sudokuCell.Right.Bottom != null && SudokuCells.Contains(sudokuCell.Right.Bottom)
                        ? 1 : -1;
                    int leftOffsetDirection = sudokuCell.Left != null && SudokuCells.Contains(sudokuCell.Left) ? -1 : 1;
                    Line line = new()
                    {
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    };
                    void updateLinePosition()
                    {
                        line.X1 = sudokuCell.BottomRightPosition.X + widthOffset() * rightOffsetDirection;
                        line.Y1 = sudokuCell.BottomRightPosition.Y - heightOffset();
                        line.X2 = sudokuCell.BottomLeftPosition.X + widthOffset() * leftOffsetDirection;
                        line.Y2 = sudokuCell.BottomLeftPosition.Y - heightOffset();
                        line.StrokeDashArray = new DoubleCollection(new[] { strokeDashLength(), strokeDashLength() });
                    }
                    updateLinePosition();
                    sudokuCell.Grid.SizeChanged += (_, _) => updateLinePosition();
                    Grid.Children.Add(line);
                }

                if (sudokuCell.Left == null || !SudokuCells.Contains(sudokuCell.Left))
                {
                    int bottomOffsetDirection =
                        sudokuCell.Bottom != null && SudokuCells.Contains(sudokuCell.Bottom) &&
                        sudokuCell.Bottom.Left != null && SudokuCells.Contains(sudokuCell.Bottom.Left)
                        ? 1 : -1;
                    int topOffsetDirection = sudokuCell.Top != null && SudokuCells.Contains(sudokuCell.Top) ? -1 : 1;
                    Line line = new()
                    {
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        IsHitTestVisible = false,
                    };
                    void updateLinePosition()
                    {
                        line.X1 = sudokuCell.BottomLeftPosition.X + widthOffset();
                        line.Y1 = sudokuCell.BottomLeftPosition.Y + heightOffset() * bottomOffsetDirection;
                        line.X2 = sudokuCell.TopLeftPosition.X + widthOffset();
                        line.Y2 = sudokuCell.TopLeftPosition.Y + heightOffset() * topOffsetDirection + offsetForSum;
                        line.StrokeDashArray = new DoubleCollection(new[] { strokeDashLength(), strokeDashLength() });
                    }
                    updateLinePosition();
                    sudokuCell.Grid.SizeChanged += (_, _) => updateLinePosition();
                    Grid.Children.Add(line);
                }
            }
        }
    }
}
