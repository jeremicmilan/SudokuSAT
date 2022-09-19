using Google.OrTools.Sat;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SudokuSAT.SudokuCell;

namespace SudokuSAT
{
    public class SudokuCell
    {
        public const int MinValue = 1;
        public const int MaxValue = 9;

        public virtual Sudoku Sudoku { get; private set; }

        public int Column { get; set; }
        public int Row { get; set; }

        public int? Value { get; set; }
        public ValueType? Type { get; set; }
        public HashSet<int> PossibleValues { get; set; } = new();
        public IntVar? ValueVar { get; set; }

        public SudokuCell(Sudoku sudoku, int column, int row, int? value = null)
        {
            Sudoku = sudoku; 
            Column = column;
            Row = row;
            Value = value;
        }

        public SudokuCell Clone(Sudoku sudoku)
        {
            return new(sudoku, Column, Row, Value);
        }

        public void AddValueConstrainct(CpModel model)
        {
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + Column + "_r" + Row);

            if (Value.HasValue)
            {
                model.Add(ValueVar == Value.Value);
            }
        }

        public virtual void SetValue(int value, ValueType valueType)
        {
            Value = value;
            Type = valueType;
        }

        public virtual void ClearValue()
        {
            Value = null;
            Type = null;
        }

        internal void UpdateSolvedValue(CpSolver solver)
        {
            SetValue((int)solver.Value(ValueVar), ValueType.Solver);
        }

        public bool Adjacent(SudokuCell sudokuCell)
        {
            return Math.Abs(Column - sudokuCell.Column) <= 1
                && Math.Abs(Row - sudokuCell.Row) <= 1
                && !(Column == sudokuCell.Column && Row == sudokuCell.Row);
        }

        public bool OrthoAdjacent(SudokuCell sudokuCell)
        {
            return Math.Abs(Column - sudokuCell.Column) == 1 && Row == sudokuCell.Row
                || Math.Abs(Row - sudokuCell.Row) == 1 && Column == sudokuCell.Column;
        }
    }
}
