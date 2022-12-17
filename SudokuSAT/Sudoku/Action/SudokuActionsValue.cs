using Google.OrTools.Sat;
using System.Collections.Generic;

namespace SudokuSAT
{
    public class SudokuActionsValue : SudokuActionsCell
    {
        public SudokuActionsValue() { }

        public SudokuActionsValue(Sudoku sudoku, List<SudokuCell> sudokuCells, int? value, ValueType? type)
            : base(sudoku)
        {
            foreach (SudokuCell sudokuCell in sudokuCells)
            {
                SudokuActionCells.Add(new SudokuActionValue(
                    Sudoku, sudokuCell,
                    value, type,
                    sudokuCell.Value, sudokuCell.Type));
            }
        }

        public SudokuActionsValue(Sudoku sudoku, CpSolver solver)
            : base(sudoku)
        {
            foreach (SudokuCell sudokuCell in sudoku.SudokuCells)
            {
                if (!sudokuCell.Value.HasValue)
                {
                    SudokuActionCells.Add(new SudokuActionValue(
                        Sudoku, sudokuCell,
                        (int)solver.Value(sudokuCell.ValueVar), ValueType.Solver,
                        sudokuCell.Value, sudokuCell.Type));
                }
            }
        }
    }
}
