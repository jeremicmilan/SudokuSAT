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
        public SudokuCellState State = SudokuCellState.Idle;

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

        public void OnClick(object sender, RoutedEventArgs e)
        {
            ToogleState();
        }

        public void ToogleState()
        {
            switch (State)
            {
                case SudokuCellState.Idle:
                    SetState(SudokuCellState.Selected);
                    break;
                case SudokuCellState.Selected:
                    SetState(SudokuCellState.Idle);
                    break;
                default:
                    throw new System.Exception("Unknown sudoku cell state: " + State);
            }
        }

        public void SetState(SudokuCellState state)
        {
            State = state;
            switch (state)
            {
                case SudokuCellState.Idle:
                    Border.Background = null;
                    break;
                case SudokuCellState.Selected:
                    Border.Background = Brushes.Yellow;
                    break;
                default:
                    throw new System.Exception("Unknown sudoku cell state: " + state);
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

        private Dictionary<ValueType, SolidColorBrush> digitToColor = new()
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
        }

        internal void UpdateSolvedValue(CpSolver solver)
        {
            SetValue((int)solver.Value(ValueVar), ValueType.Solver);
        }
    }
}
