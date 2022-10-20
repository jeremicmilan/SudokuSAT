using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionValues : SudokuAction
    {
        List<SudokuActionValue> ActionValues;

        public SudokuActionValues(Sudoku sudoku, List<SudokuCell> sudokuCells, int? value, ValueType? type)
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

        public SudokuActionValues(Sudoku sudoku, CpSolver solver)
            : base(sudoku)
        {
            ActionValues = new();
            foreach (SudokuCell sudokuCell in sudoku.SudokuCells)
            {
                if (!sudokuCell.Value.HasValue)
                {
                    ActionValues.Add(new SudokuActionValue(
                        Sudoku, sudokuCell,
                        (int)solver.Value(sudokuCell.ValueVar), ValueType.Solver,
                        sudokuCell.Value, sudokuCell.Type));
                }
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
