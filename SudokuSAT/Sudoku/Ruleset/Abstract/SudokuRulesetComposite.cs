using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

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
    }
}
