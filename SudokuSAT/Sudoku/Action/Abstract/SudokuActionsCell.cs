using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public abstract class SudokuActionsCell : SudokuAction
    {
        public List<SudokuActionCell> SudokuActionCells { get; private set; }

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
