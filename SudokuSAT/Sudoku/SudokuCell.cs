using Google.OrTools.Sat;
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

        public int Column { get; set; }
        public int Row { get; set; }

        public int? Value { get; set; }
        public ValueType? Type { get; set; }
        public HashSet<int> PossibleValues { get; set; } = new();
        public IntVar? ValueVar { get; set; }


        public Border Border;
        public bool IsSelected { get; set; } = false;

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

        public static int GlobalSelectionCount = 0;
        public int? SelectionOrderId = null;
        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            if (isSelected)
            {
                SelectionOrderId = GlobalSelectionCount++;
                Border.Background = Brushes.Yellow;
            }
            else
            {
                SelectionOrderId = null;
                Border.Background = null;
            }
        }

        public void AddValueConstrainct(CpModel model)
        {
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + Column + "_r" + Row);

            if (Value.HasValue)
            {
                model.Add(ValueVar == Value.Value);
            }
        }

        private readonly Dictionary<ValueType, SolidColorBrush> digitToColor = new()
        {
            { ValueType.Given,  Brushes.Black },
            { ValueType.Solver, Brushes.Green },
            { ValueType.User,   Brushes.Blue  }
        };

        public void SetValue(int value, ValueType valueType)
        {
            Value = value;
            Type = valueType;

            Border.Child = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                MinWidth = Border.ActualWidth,
                MinHeight = Border.ActualHeight,
                FontSize = Border.ActualHeight * 0.65,
                Foreground = digitToColor[valueType],
                Content = value > 0 ? value : "X"
            };
        }

        public void ClearValue()
        {
            Value = null;
            Type = null;

            Border.Child = new Label();
        }

        internal void UpdateSolvedValue(CpSolver solver)
        {
            SetValue((int)solver.Value(ValueVar), ValueType.Solver);
        }
    }
}
