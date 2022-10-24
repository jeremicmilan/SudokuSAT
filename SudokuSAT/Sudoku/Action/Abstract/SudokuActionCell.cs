using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public abstract class SudokuActionCell : SudokuAction
    {
        public SudokuCell SudokuCell { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected SudokuActionCell() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected SudokuActionCell(Sudoku sudoku, SudokuCell sudokuCell)
            : base(sudoku)
        {
            SudokuCell = sudokuCell;
        }
    }
}
