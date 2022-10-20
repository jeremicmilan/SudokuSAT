using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionPossibleValues : SudokuActionCell
    {
        public HashSet<int> PossibleValues { get; private set; }

        public SudokuActionPossibleValues(Sudoku sudoku, SudokuCell sudokuCell, HashSet<int> possibleValues)
            : base(sudoku, sudokuCell)
        {
            PossibleValues = possibleValues;
        }

        public override void Undo()
        {
            SudokuCell.PossibleValues = new();
        }

        public override void Redo()
        {
            SudokuCell.PossibleValues = PossibleValues;
        }
    }
}
