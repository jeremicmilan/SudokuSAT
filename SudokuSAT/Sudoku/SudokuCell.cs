using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SudokuSAT
{
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

        public HashSet<int> PossibleValues { get; set; } = new();

        public void SetValue(int? value)
        {
            Value = value;
            ValueTextBox.Text = value.HasValue ? value.ToString() : "";
        }

        public IntVar? ValueVar { get; set; }

        public TextBox ValueTextBox { get; set; }
        public Label SolutionsLabel { get; set; }

        public SudokuCell(int column, int row, TextBox textBox, Label solutionsLabel, int? value = null)
        {
            Column = column;
            Row = row;
            ValueTextBox = textBox;
            SolutionsLabel = solutionsLabel;
            Value = value;
        }

        public SudokuCell Clone()
        {
            return new(Column, Row, ValueTextBox, SolutionsLabel, Value);
        }

        public void OnValueChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            string stringValue = ((TextBox)sender).Text;
            if (int.TryParse(stringValue, out int value))
            {
                if (value >= MinValue && value <= MaxValue)
                {
                    Value = value;
                    return;
                }
            }

            ValueTextBox.Text = Value?.ToString();
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
            SetValue((int)solver.Value(ValueVar));
        }
    }
}
