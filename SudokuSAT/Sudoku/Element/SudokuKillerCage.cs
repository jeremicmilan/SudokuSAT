using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (Sum < 1 || Sum > 45)
            {
                throw new Exception("Killer cage sum must be between 1 and 45 or non-existant.");
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

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        protected override void VisualizeInternal()
        {
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                double offsetFactor = 0.05;
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
                        StrokeDashArray = new DoubleCollection(new [] { 5.0, 5.0 }),
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                    });
                }
            }
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
