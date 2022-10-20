using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionValues : SudokuAction
    {
        List<SudokuActionValue> ActionValues;

        public SudokuActionValues(Sudoku sudoku, int? value, ValueType? type, List<SudokuCell> sudokuCells)
            : base(sudoku)
        {
            ActionValues = new();
            foreach (SudokuCell sudokuCell in sudokuCells)
            {
                ActionValues.Add(new SudokuActionValue(
                    Sudoku, sudokuCell,
                    value, type,
                    sudokuCell.Value, sudokuCell.Type));
            }
        }

        public override void Undo()
        {
            foreach (SudokuActionValue sudokuActionValue in ActionValues)
            {
                sudokuActionValue.Undo();
            }
        }

        public override void Redo()
        {
            foreach (SudokuActionValue sudokuActionValue in ActionValues)
            {
                sudokuActionValue.Redo();
            }
        }
    }
}
