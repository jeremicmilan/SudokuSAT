namespace SudokuSAT
{
    public abstract class SudokuRulesetOrthoPairSum : SudokuRulesetOrthoPairs
    {
        public int Sum { get; set; }

        protected SudokuRulesetOrthoPairSum(Sudoku sudoku, int sum)
            : base(sudoku)
        {
            Sum = sum;
        }

        public override bool Equal(SudokuRuleset other)
        {
            return base.Equal(other) && Sum == ((SudokuRulesetOrthoPairSum)other).Sum;
        }
    }
}
