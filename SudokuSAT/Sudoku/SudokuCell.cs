using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static SudokuSAT.SudokuCell;

namespace SudokuSAT
{
    public enum ValueType
    {
        Given,
        Solver,
        User
    }

    public class SudokuCell
    {
        public const int MinValue = 1;
        public const int MaxValue = 9;

        public int Column { get; set; }
        public int Row { get; set; }

        public int? Value
        {
            get;
            set;
        }

        public ValueType? Type
        {
            get;
            set;
        }

        private Dictionary<ValueType, SolidColorBrush> digitToColor = new()
        {
            { ValueType.Given,  Brushes.Black },
            { ValueType.Solver, Brushes.Green },
            { ValueType.User,   Brushes.Blue  }
        };

        public HashSet<int> PossibleValues { get; set; } = new();

        public void SetValue(int value, ValueType valueType)
        {
            Value = value;
            Type = valueType;

            Border.Child = new Label()
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = Border.ActualHeight * 0.65,
                Foreground = digitToColor[valueType],
                Content = value > 0 ? value : "X"
            };
        }

        public void ClearValue()
        {
            Value = null;
            Type = null;
        }

        public IntVar? ValueVar { get; set; }

        public Border Border;

        public SudokuCell(int column, int row, Border border, int? value = null)
        {
            Column = column;
            Row = row;
            Border = border;
            Value = value;
        }

        public SudokuCell Clone()
        {
            return new(Column, Row, Border, Value);
        }

        internal void AddValueConstrainct(CpModel model)
        {
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + Column + "_r" + Row);

            if (Value.HasValue)
            {
                model.Add(ValueVar == Value.Value);
            }
        }

        internal void UpdateSolvedValue(CpSolver solver)
        {
            SetValue((int)solver.Value(ValueVar), ValueType.Solver);
        }
    }
}
