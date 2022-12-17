using Google.OrTools.Sat;

namespace SudokuSAT
{
    public class SudokuRulesetOrthoPairSumMin : SudokuRulesetOrthoPairSum
    {
        public SudokuRulesetOrthoPairSumMin(Sudoku sudoku, int sum)
            : base(sudoku, sum)
        { }

        public override SudokuRulesetOrthoPairSumMin Clone(Sudoku sudoku)
        {
            return new SudokuRulesetOrthoPairSumMin(sudoku, Sum);
        }

        protected override void AddOrthoPairConstraints(
            CpModel model,
            SudokuCell sudokuCell1,
            SudokuCell sudokuCell2)
        {
            model.Add(sudokuCell1.ValueVar + sudokuCell2.ValueVar >= Sum);
        }
    }
}
