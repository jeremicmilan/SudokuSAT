﻿using Google.OrTools.Sat;
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
    public class SudokuPalindrome : SudokuElementLine
    {
        public SudokuPalindrome(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            Grid? grid = null)
        {
            return new SudokuPalindrome(sudoku, sudokuCells, grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            for (int i = 0; i < SudokuCells.Count / 2; i++)
            {
                model.Add(SudokuCells[i].ValueVar == SudokuCells[SudokuCells.Count - i - 1].ValueVar)
                    .OnlyEnforceIf(boolVar);
            }
        }

        public override void Visualize()
        {
            Debug.Assert(Grid != null);
            Grid.Children.Add(new Polyline
            {
                Points = new PointCollection(SudokuCells.Select(cell => cell.CenterPosition)),
                Stroke = Brushes.Orange,
                StrokeThickness = 15,
                Opacity = .5,
                IsHitTestVisible = false,
            });
        }
    }
}
