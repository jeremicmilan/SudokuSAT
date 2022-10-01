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
    public class SudokuPalindrome : SudokuElementWithCellListAdjacent
    {
        public SudokuPalindrome(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        { }

        public override void AddConstraints(CpModel model)
        {
            for (int i = 0; i < SudokuCells.Count / 2; i++)
            {
                model.Add(SudokuCells[i].ValueVar == SudokuCells[SudokuCells.Count - i - 1].ValueVar);
            }
        }

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        public override void Visualize()
        {
            for (int i = 1; i < SudokuCells.Count; i++)
            {
                SudokuCell sudokuCell1 = SudokuCells[i - 1];
                SudokuCell sudokuCell2 = SudokuCells[i];
                Point position1 = sudokuCell1.CenterPosition;
                Point position2 = sudokuCell2.CenterPosition;
                Grid.Children.Add(new Line()
                {
                    X1 = position1.X,
                    Y1 = position1.Y,
                    X2 = position2.X,
                    Y2 = position2.Y,
                    Stroke = Brushes.Orange,
                    StrokeThickness = 15,
                    Opacity = 50,
                });
            }

            Sudoku.Grid.Children.Add(Grid);
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
