using Google.OrTools.Sat;

namespace SudokuSAT
{
    public abstract class SudokuRuleset
    {
        public Sudoku Sudoku { get; set; }

        protected SudokuRuleset(Sudoku sudoku)
        {
            Sudoku = sudoku;
        }

        public abstract SudokuRuleset Clone(Sudoku sudoku);

        public string Name => GetType().Name;

        public abstract void AddConstraints(CpModel model);

        public virtual bool Equal(SudokuRuleset other)
        {
            return GetType() == other.GetType() && Sudoku == other.Sudoku;
        }
    }
}
