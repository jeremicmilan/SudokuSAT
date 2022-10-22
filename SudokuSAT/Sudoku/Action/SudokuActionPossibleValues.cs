using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            if (PossibleValues != null && PossibleValues.Count == 1)
            {
                SudokuCell.Value = null;
                SudokuCell.Type = null;
            }
        }

        public override void Redo()
        {
            SudokuCell.PossibleValues = PossibleValues;
            if (PossibleValues != null && PossibleValues.Count == 1)
            {
                SudokuCell.Value = PossibleValues.First();
                SudokuCell.Type = ValueType.Solver;
            }
        }
    }
}
