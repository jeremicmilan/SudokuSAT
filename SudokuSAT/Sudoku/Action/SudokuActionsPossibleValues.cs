using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSAT
{
    public class SudokuActionsPossibleValues : SudokuActionsCell
    {
        public SudokuActionsPossibleValues() { }

        public SudokuActionsPossibleValues(
            Sudoku sudoku,
            Dictionary<SudokuCell, HashSet<int>?> sudokuCellToOldPossibleValuesDictionary)
            : base(sudoku)
        {
            foreach ((SudokuCell sudokuCell, HashSet<int>? possibleValues) in sudokuCellToOldPossibleValuesDictionary)
            {
                SudokuActionCells.Add(new SudokuActionPossibleValues(
                    Sudoku, sudokuCell, sudokuCell.PossibleValues, possibleValues));
            }
        }
    }
}
