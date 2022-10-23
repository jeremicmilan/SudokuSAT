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

        protected Dictionary<SudokuCell, int> SudokuCellsOrderDictionary { get; private set; }

        public SudokuElementWithCellList(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, grid)
        {
            if (sudokuCells.Count < 2)
            {
                throw new Exception("Element must have at least 2 cells.");
            }

            SudokuCells = sudokuCells;
            SudokuCellsOrderDictionary = new();
            int i = 0;
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                SudokuCellsOrderDictionary[sudokuCell] = i++;
            }
        }

        protected abstract SudokuElementWithCellList Instantiate(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            Grid? grid = null);

        public override SudokuElement Clone(Sudoku sudoku, Grid? grid = null)
        {
            List<SudokuCell> sudokuCells = new();
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                sudokuCells.Add(sudoku.SudokuGrid[sudokuCell.Column, sudokuCell.Row]);
            }

            return Instantiate(sudoku, sudokuCells, grid);
        }
    }
}
