using System.Collections.Generic;
using System.Linq;

namespace SudokuSAT
{
    public class SudokuRulesetColumns : SudokuRulesetComposite
    {
        public SudokuRulesetColumns(Sudoku sudoku)
            : base(sudoku, GetRulesetsColumns(sudoku))
        { }

        public override SudokuRulesetColumns Clone(Sudoku sudoku)
        {
            return new SudokuRulesetColumns(sudoku);
        }

        private static List<SudokuRuleset> GetRulesetsColumns(Sudoku sudoku)
        {
            return sudoku
                .GetColumns()
                .Select(column => (SudokuRuleset)new SudokuRulesetCellsUnique(sudoku, column))
                .ToList();
        }
    }
}
