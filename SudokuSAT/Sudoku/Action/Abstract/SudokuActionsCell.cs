using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public abstract class SudokuActionsCell : SudokuAction
    {
        public List<SudokuActionCell> SudokuActionCells { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected SudokuActionsCell() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected SudokuActionsCell(Sudoku sudoku)
            : base(sudoku)
        {
            SudokuActionCells = new();
        }

        public override void Undo()
        {
            foreach (SudokuActionCell sudokuActionCell in SudokuActionCells)
            {
                sudokuActionCell.Undo();
            }
        }

        public override void Redo()
        {
            foreach (SudokuActionCell sudokuActionCell in SudokuActionCells)
            {
                sudokuActionCell.Redo();
            }
        }
    }
}
