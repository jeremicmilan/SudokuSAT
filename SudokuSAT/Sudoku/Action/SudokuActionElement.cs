namespace SudokuSAT
{
    public class SudokuActionElement : SudokuAction
    {
        SudokuElement SudokuElement;

        public SudokuActionElement(Sudoku sudoku, SudokuElement sudokuElement)
            : base(sudoku)
        {
            SudokuElement = sudokuElement;
        }

        public override void Undo()
        {
        }

        public override void Redo()
        {
        }
    }
}
