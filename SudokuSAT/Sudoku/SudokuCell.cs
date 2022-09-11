using Google.OrTools.Sat;
using System;
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

        public void SetValue(int value)
        {
            Value = value;
            TextBox.Text = value.ToString();
        }

        public IntVar? ValueVar { get; set; }

        public TextBox TextBox { get; set; }

        public SudokuCell(int column, int row, TextBox textBox)
        {
            Column = column;
            Row = row;
            TextBox = textBox;
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

            TextBox.Text = Value?.ToString();
        }

        internal void AddValueConstrainct(CpModel model)
        {
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + Column + "_r" + Row);
        }

        internal void UpdateSolvedValue(CpSolver solver)
        {
            SetValue((int)solver.Value(ValueVar));
        }
    }
}
