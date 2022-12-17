using System.Collections.Generic;
using System.Linq;

namespace SudokuSAT
{
    public class SudokuRulesetBoxes : SudokuRulesetComposite
    {
        public SudokuRulesetBoxes(Sudoku sudoku)
            : base(sudoku, GetRulestBoxes(sudoku))
        { }

        public override SudokuRulesetBoxes Clone(Sudoku sudoku)
        {
            return new SudokuRulesetBoxes(sudoku);
        }

        private static List<SudokuRuleset> GetRulestBoxes(Sudoku sudoku)
        {
            return sudoku
                .GetBoxes()
                .Select(column => (SudokuRuleset)new SudokuRulesetCellsUnique(sudoku, column))
                .ToList();
        }
    }
}
