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
    public abstract class SudokuElementWithCellList : SudokuElement
    {
        public List<SudokuCell> SudokuCells { get; set; }

        public SudokuElementWithCellList(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, grid)
        {
            if (sudokuCells.Count < 2)
            {
                throw new Exception("Element must have at least 2 cells.");
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
    }
}
