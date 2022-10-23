using System.Diagnostics;
using System.Windows.Controls;

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
            Debug.Assert(SudokuElement.Grid != null);
            Debug.Assert(Sudoku.Grid != null);

            Sudoku.SudokuElements.Remove(SudokuElement);
            Sudoku.Grid.Children.Remove(SudokuElement.Grid);
            SudokuElement.Grid.Children.Clear();
        }

        public override void Redo()
        {
            Debug.Assert(SudokuElement.Grid != null);
            Debug.Assert(Sudoku.Grid != null);

            Sudoku.SudokuElements.Add(SudokuElement);
            Sudoku.Grid.Children.Add(SudokuElement.Grid);
            SudokuElement.Visualize();
        }
    }
}
