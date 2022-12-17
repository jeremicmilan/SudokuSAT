using System.Collections.Generic;

namespace SudokuSAT
{
    public class SudokuRulesetSudoku : SudokuRulesetComposite
    {
        public SudokuRulesetSudoku(Sudoku sudoku)
            : base(sudoku, new List<SudokuRuleset>
            {
                new SudokuRulesetRows(sudoku),
                new SudokuRulesetColumns(sudoku),
                new SudokuRulesetBoxes(sudoku),
            })
        { }

        public override SudokuRulesetSudoku Clone(Sudoku sudoku)
        {
            return new SudokuRulesetSudoku(sudoku);
        }
    }
}
