using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSAT
{
    public class SudokuActionValue : SudokuActionCell
    {
        public int? Value { get; set; }
        public ValueType? Type { get; set; }
        public int? ValueOld { get; set; }
        public ValueType? TypeOld { get; set; }

        public SudokuActionValue() { }

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
