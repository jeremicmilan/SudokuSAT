using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionValue : SudokuAction
    {
        SudokuCell SudokuCell;

        int? Value;
        ValueType? Type;
        int? ValueOld;
        ValueType? TypeOld;

        public SudokuActionValue(
            Sudoku sudoku, SudokuCell sudokuCell,
            int? value, ValueType? type,
            int? valueOld, ValueType? typeOld)
            : base(sudoku)
        {
            SudokuCell = sudokuCell;
            Value = value;
            Type = type;
            ValueOld = valueOld;
            TypeOld = typeOld;
        }

        public override void Undo()
        {
            SudokuCell.Value = ValueOld;
            SudokuCell.Type = TypeOld;
        }

        public override void Redo()
        {
            SudokuCell.Value = Value;
            SudokuCell.Type = Type;
        }
    }
}
