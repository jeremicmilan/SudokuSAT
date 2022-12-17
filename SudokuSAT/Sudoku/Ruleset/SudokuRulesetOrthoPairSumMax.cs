using Google.OrTools.Sat;

namespace SudokuSAT
{
    public class SudokuRulesetOrthoPairSumMax : SudokuRulesetOrthoPairSum
    {
        public SudokuRulesetOrthoPairSumMax(Sudoku sudoku, int sum)
            : base(sudoku, sum)
        { }

        public override SudokuRulesetOrthoPairSumMax Clone(Sudoku sudoku)
        {
            return new SudokuRulesetOrthoPairSumMax(sudoku, Sum);
        }

        protected override void AddOrthoPairConstraints(
            CpModel model,
            SudokuCell sudokuCell1,
            SudokuCell sudokuCell2)
        {
            model.Add(sudokuCell1.ValueVar + sudokuCell2.ValueVar <= Sum);
        }
    }
}
