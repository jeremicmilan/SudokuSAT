using System.Linq;

namespace SudokuSAT
{
    public class SudokuActionsPossibleValues : SudokuActionsCell
    {
        public SudokuActionsPossibleValues(Sudoku sudoku)
            : base(sudoku)
        {
            foreach (SudokuCell sudokuCell in sudoku.SudokuCells)
            {
                if (sudokuCell.PossibleValues.Any())
                {
                    SudokuActionCells.Add(new SudokuActionPossibleValues(
                        Sudoku, sudokuCell, sudokuCell.PossibleValues));
                }
            }
        }
    }
}
