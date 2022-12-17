using System.Collections.Generic;

namespace SudokuSAT
{
    public abstract class SudokuRulesetWithCellList : SudokuRuleset
    {
        public List<SudokuCell> SudokuCells { get; set; }

        protected SudokuRulesetWithCellList(Sudoku sudoku, List<SudokuCell> sudokuCells)
            : base(sudoku)
        {
            SudokuCells = new();
            foreach (SudokuCell sudokuCell in sudokuCells)
            {
                // If we are creating this through clone,
                // we need to point to cells in the correct sudoku object.
                //
                SudokuCells.Add(sudoku.SudokuGrid[sudokuCell.Column, sudokuCell.Row]);
            }
        }

        public override bool Equal(SudokuRuleset other)
        {
            return base.Equal(other) && SudokuCells == ((SudokuRulesetWithCellList)other).SudokuCells;
        }
    }
}
