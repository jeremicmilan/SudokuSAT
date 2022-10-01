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
            Grid.Children.Add(new Polyline
            {
                Points = new PointCollection(SudokuCells.Select(cell => cell.CenterPosition)),
                Stroke = Brushes.Orange,
                StrokeThickness = 15,
                Opacity = .5
            });

            Sudoku.Grid.Children.Add(Grid);
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
