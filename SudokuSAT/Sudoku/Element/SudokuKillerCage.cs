using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuKillerCage : SudokuElementCage
    {
        public int Sum { get; private set; }

        public SudokuKillerCage(Sudoku sudoku, List<SudokuCell> sudokuCells, int sum, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        {
            Sum = sum;
        }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuWhispers(sudoku, sudokuCells, Sum);
        }

        public override void AddConstraints(CpModel model)
        {
            model.Add(LinearExpr.Sum(SudokuCells.Select(cell => cell.ValueVar)) == Sum);
        }

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        public override void Visualize()
        {
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                double offsetFactor = 0.1;
                double widthOffset  = sudokuCell.Grid.ActualWidth  * offsetFactor;
                double heightOffset = sudokuCell.Grid.ActualHeight * offsetFactor;

                if (sudokuCell.Top == null || !SudokuCells.Contains(sudokuCell.Top))
                {
                    Grid.Children.Add(new Line()
                    {
                        X1 = sudokuCell.TopLeftPosition.X + heightOffset,
                        Y1 = sudokuCell.TopLeftPosition.Y + widthOffset * (sudokuCell.Left != null ? -1 : 1),
                        X2 = sudokuCell.TopRightPosition.X + heightOffset,
                        Y2 = sudokuCell.TopRightPosition.Y + widthOffset * (sudokuCell.Right != null ? -1 : 1),
                        StrokeDashArray = new DoubleCollection(new [] { 2.0, 2.0 }),
                        StrokeThickness = 1,
                    });
                }
            }
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
