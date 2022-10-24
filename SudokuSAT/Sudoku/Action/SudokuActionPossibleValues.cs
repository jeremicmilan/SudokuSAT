using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSAT
{
    public class SudokuActionPossibleValues : SudokuActionCell
    {
        public HashSet<int>? PossibleValues { get; set; }
        public HashSet<int>? PossibleValuesOld { get; set; }

        public SudokuActionPossibleValues() { }

        public SudokuActionPossibleValues(Sudoku sudoku, SudokuCell sudokuCell, HashSet<int>? possibleValues, HashSet<int>? possibleValuesOld)
            : base(sudoku, sudokuCell)
        {
            PossibleValues = possibleValues;
            PossibleValuesOld = possibleValuesOld;
        }

        public override void Undo()
        {
            SudokuCell.PossibleValues = PossibleValuesOld;
        }

        public override void Redo()
        {
            SudokuCell.PossibleValues = PossibleValues;
        }
    }
}
