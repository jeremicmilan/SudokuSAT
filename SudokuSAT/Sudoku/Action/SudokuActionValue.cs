using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionValue : SudokuActionCell
    {
        public int? Value { get; private set; }
        public ValueType? Type { get; private set; }
        public int? ValueOld { get; private set; }
        public ValueType? TypeOld { get; private set; }

        public SudokuActionValue(
            Sudoku sudoku, SudokuCell sudokuCell,
            int? value, ValueType? type,
            int? valueOld, ValueType? typeOld)
            : base(sudoku, sudokuCell)
        {
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
