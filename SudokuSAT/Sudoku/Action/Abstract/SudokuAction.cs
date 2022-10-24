namespace SudokuSAT
{
    public abstract class SudokuAction
    {
        public Sudoku Sudoku { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected SudokuAction() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected SudokuAction(Sudoku sudoku)
        {
            Sudoku = sudoku;
        }

        public abstract void Undo();
        public abstract void Redo();
    }
}
