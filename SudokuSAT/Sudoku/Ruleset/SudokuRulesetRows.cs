using System.Collections.Generic;
using System.Linq;

namespace SudokuSAT
{
    public class SudokuRulesetRows : SudokuRulesetComposite
    {
        public SudokuRulesetRows(Sudoku sudoku)
            : base(sudoku, GetRulesetsRows(sudoku))
        { }

        public override SudokuRulesetRows Clone(Sudoku sudoku)
        {
            return new SudokuRulesetRows(sudoku);
        }

        private static List<SudokuRuleset> GetRulesetsRows(Sudoku sudoku)
        {
            return sudoku
                .GetRows()
                .Select(column => (SudokuRuleset)new SudokuRulesetCellsUnique(sudoku, column))
                .ToList();
        }
    }
}
