using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public abstract class SudokuActionCell : SudokuAction
    {
        public SudokuCell SudokuCell { get; private set; }

        public SudokuActionCell(Sudoku sudoku, SudokuCell sudokuCell)
            : base(sudoku)
        {
            SudokuCell = sudokuCell;
        }
    }
}
