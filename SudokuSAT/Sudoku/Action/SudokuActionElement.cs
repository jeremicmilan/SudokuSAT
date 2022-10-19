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
            Sudoku.SudokuElements.Remove(SudokuElement);
        }

        public override void Redo()
        {
            Sudoku.SudokuElements.Add(SudokuElement);
        }
    }
}
