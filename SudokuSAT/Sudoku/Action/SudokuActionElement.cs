using System.Diagnostics;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class SudokuActionElement : SudokuAction
    {
        public SudokuElement SudokuElement { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SudokuActionElement() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
