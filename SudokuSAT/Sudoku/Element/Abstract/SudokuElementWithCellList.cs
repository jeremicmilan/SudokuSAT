using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SudokuSAT
{
    public abstract class SudokuElementWithCellList : SudokuElement
    {
        public List<SudokuCell> SudokuCells { get; set; }

        protected Dictionary<SudokuCell, int> SudokuCellsOrderDictionary { get; private set; }

        public SudokuElementWithCellList(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuElementId, grid)
        {
            if (sudokuCells.Count < 2)
            {
                throw new Exception("Element must have at least 2 cells.");
            }

            SudokuCells = new();
            foreach (SudokuCell sudokuCell in sudokuCells)
            {
                // If we are creating this through clone,
                // we need to point to cells in the correct sudoku object.
                //
                SudokuCells.Add(sudoku.SudokuGrid[sudokuCell.Column, sudokuCell.Row]);
            }

            SudokuCellsOrderDictionary = new();
            int i = 0;
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                SudokuCellsOrderDictionary[sudokuCell] = i++;
            }
        }

        protected bool AreConsecutive(List<SudokuCell> sudokuCells)
        {
            for (int i = 1; i < sudokuCells.Count; i++)
            {
                if (SudokuCellsOrderDictionary[sudokuCells[i]] - SudokuCellsOrderDictionary[sudokuCells[i - 1]] != 1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
