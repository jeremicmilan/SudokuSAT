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
    public class SudokuArrow : SudokuElement
    {
        public List<SudokuCell> SudokuCells { get; set; }

        public SudokuArrow(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, grid)
        {
            if (sudokuCells.Count < 2)
            {
                throw new Exception("Arrow must be of at least of length 2.");
            }

            if (sudokuCells.Count > 4)
            {
                throw new Exception("Arrow must be of at most of length 4.");
            }

            for (int i = 1; i < sudokuCells.Count; i++)
            {
                if (!sudokuCells[i - 1].Adjacent(sudokuCells[i]))
                {
                    throw new Exception("Invalid arrow.");
                }
            }

            SudokuCells = sudokuCells;
        }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            List<SudokuCell> sudokuCells = new();
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                sudokuCells.Add(sudoku.SudokuGrid[sudokuCell.Column, sudokuCell.Row]);
            }

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
                if (i == 0)
                {
                    Point topLeftPosition = SudokuCells[i].TopLeftPosition;
                    double scalingFactor = .7;
                    Grid.Children.Add(new Ellipse()
                    {
                        Width = SudokuCells[i].Grid.ActualWidth * scalingFactor,
                        Height = SudokuCells[i].Grid.ActualHeight * scalingFactor,
                        Margin = new Thickness(
                            topLeftPosition.X + SudokuCells[i].Grid.ActualWidth * (1 - scalingFactor) / 2,
                            topLeftPosition.Y + SudokuCells[i].Grid.ActualHeight * (1 - scalingFactor) / 2,
                            0,
                            0),
                        Stroke = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                    });
                }
                else
                {
                    Point centerPosition1 = SudokuCells[i].CenterPosition;
                    Point centerPosition2 = SudokuCells[i - 1].CenterPosition;
                    Grid.Children.Add(new Line()
                    {
                        X1 = centerPosition1.X,
                        Y1 = centerPosition1.Y,
                        X2 = centerPosition2.X,
                        Y2 = centerPosition2.Y,
                        Stroke = Brushes.Black,
                    });
                }

                if (i == SudokuCells.Count - 1)
                {
                    // TODO: add arrow tip
                }
            }

            Sudoku.Grid.Children.Add(Grid);
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
