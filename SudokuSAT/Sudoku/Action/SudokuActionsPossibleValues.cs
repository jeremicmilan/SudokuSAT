using System.Collections.Generic;

namespace SudokuSAT
{
    public class SudokuActionsPossibleValues : SudokuActionsCell
    {
        public SudokuActionsPossibleValues() { }

        public SudokuActionsPossibleValues(
            Sudoku sudoku,
            Dictionary<SudokuCell, HashSet<int>?> oldPossibleValues)
            : base(sudoku)
        {
            foreach ((SudokuCell sudokuCell, HashSet<int>? possibleValues) in oldPossibleValues)
            {
                SudokuActionCells.Add(new SudokuActionPossibleValues(
                    Sudoku, sudokuCell, sudokuCell.PossibleValues, possibleValues));
            }
        }
    }
}
