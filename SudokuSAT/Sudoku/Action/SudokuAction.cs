namespace SudokuSAT
{
    public abstract class SudokuAction
    {
        protected Sudoku Sudoku { get; private set; }

        protected SudokuAction(Sudoku sudoku)
        {
            Sudoku = sudoku;
        }

        public abstract void Undo();
        public abstract void Redo();
    }
}
