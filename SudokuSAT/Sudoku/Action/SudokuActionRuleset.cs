using System.CodeDom;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionRuleset <TSudokuRuleset> : SudokuAction
        where TSudokuRuleset : SudokuRuleset
    {
        public TSudokuRuleset? SudokuRulesetNew { get; set; }
        public TSudokuRuleset? SudokuRulesetOld { get; set; }

        public SudokuActionRuleset() { }

        public SudokuActionRuleset(
            Sudoku sudoku,
            TSudokuRuleset? sudokuRulesetNew,
            TSudokuRuleset? sudokuRulesetOld)
            : base(sudoku)
        {
            SudokuRulesetNew = sudokuRulesetNew;
            SudokuRulesetOld = sudokuRulesetOld;
        }

        public override void Undo()
        {
            Sudoku.RemoveRulesetIfExists<TSudokuRuleset>();
            if (SudokuRulesetOld != null)
            {
                Sudoku.AddOrUpdateRuleset(SudokuRulesetOld);
            }

            Sudoku.MainWindow.UpdateRulesetToolbar();
        }

        public override void Redo()
        {
            Sudoku.RemoveRulesetIfExists<TSudokuRuleset>();
            if (SudokuRulesetNew != null)
            {
                Sudoku.AddOrUpdateRuleset(SudokuRulesetNew);
            }

            Sudoku.MainWindow.UpdateRulesetToolbar();
        }
    }
}
