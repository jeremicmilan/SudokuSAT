using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuArrow : SudokuElement
    {
        public List<SudokuCell> SudokuCells { get; set; }

        public SudokuArrow(List<SudokuCell> sudokuCells)
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

            return new SudokuArrow(sudokuCells);
        }

        public override void AddConstraints(CpModel model)
        {
            BoundedLinearExpression boundedLinearExpression = SudokuCells[0].ValueVar ==
                LinearExpr.Sum(SudokuCells.Skip(1).Select(cell => cell.ValueVar));

            model.Add(boundedLinearExpression);
        }

        public override void Visualize()
        {
            for (int i = 0; i < SudokuCells.Count; i++)
            {
                Grid grid = SudokuCells[i].Grid;

                if (i == 0)
                {
                    grid.Children.Add(new Ellipse()
                    {
                        Height = grid.ActualHeight / 1.5,
                        Width = grid.ActualWidth / 1.5,
                        Stroke = Brushes.Black,
                    });
                }

                if (i < SudokuCells.Count - 1)
                {
                    grid.Children.Add(new Line()
                    {
                        X1 = grid.ActualWidth / 2,
                        Y1 = grid.ActualHeight / 2,
                        X2 = grid.ActualWidth / 2 + grid.ActualWidth / 2 * (SudokuCells[i + 1].Column - SudokuCells[i].Column),
                        Y2 = grid.ActualHeight / 2 + grid.ActualHeight / 2 * (SudokuCells[i + 1].Row - SudokuCells[i].Row),
                        Stroke = Brushes.Black,
                    });
                }

                if (i > 0)
                {
                    grid.Children.Add(new Line()
                    {
                        X1 = grid.ActualWidth / 2,
                        Y1 = grid.ActualHeight / 2,
                        X2 = grid.ActualWidth / 2 + grid.ActualWidth / 2 * (SudokuCells[i - 1].Column - SudokuCells[i].Column),
                        Y2 = grid.ActualHeight / 2 + grid.ActualHeight / 2 * (SudokuCells[i - 1].Row - SudokuCells[i].Row),
                        Stroke = Brushes.Black,
                    });
                }

                if (i == SudokuCells.Count - 1)
                {
                    // TODO: add arrow tip
                }
            }
        }
    }
}
