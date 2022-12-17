using Google.OrTools.Sat;
using System.Collections.Generic;

namespace SudokuSAT
{
    public abstract class SudokuRulesetComposite : SudokuRuleset
    {
        public List<SudokuRuleset> SudokuRulesets { get; set; }

        protected SudokuRulesetComposite(Sudoku sudoku, List<SudokuRuleset> sudokuRulesets)
            : base(sudoku)
        {
            SudokuRulesets = sudokuRulesets;
        }

        public override void AddConstraints(CpModel model)
        {
            foreach (SudokuRuleset sudokuRuleset in SudokuRulesets)
            {
                sudokuRuleset.AddConstraints(model);
            }
        }

        public override bool Equal(SudokuRuleset other)
        {
            return base.Equal(other) && SudokuRulesets == ((SudokuRulesetComposite)other).SudokuRulesets;
        }
    }
}
