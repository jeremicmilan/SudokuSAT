using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace SudokuSAT
{
    public abstract class SudokuRulesetWithCellList : SudokuRuleset
    {
        public List<SudokuCell> SudokuCells { get; set; }

        protected SudokuRulesetWithCellList(Sudoku sudoku, List<SudokuCell> sudokuCells)
            : base(sudoku)
        {
            SudokuCells = new();
            foreach (SudokuCell sudokuCell in sudokuCells)
            {
                // If we are creating this through clone,
                // we need to point to cells in the correct sudoku object.
                //
                SudokuCells.Add(sudoku.SudokuGrid[sudokuCell.Column, sudokuCell.Row]);
            }
        }
    }
}
