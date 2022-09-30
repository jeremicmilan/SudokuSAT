using Google.OrTools.Sat;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public Grid? Grid { get; private set; }

        public string Name => "_" + this.GetType().Name + "_R" + Row + "_C" + Column;

        public SudokuCell(Sudoku sudoku, int column, int row, int? value = null, Grid? grid = null)
        {
            Sudoku = sudoku;
            Column = column;
            Row = row;
            Value = value;

            Grid = grid;
            if (Grid != null)
            {
                Grid.Name = Name;
            }
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

            if (Grid != null)
            {
                Grid.Children.Add(new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    MinWidth = Grid.ActualWidth,
                    MinHeight = Grid.ActualHeight,
                    FontSize = Grid.ActualHeight * 0.65,
                    Foreground = digitToColor[valueType],
                    Content = value > 0 ? value : "X"
                });
            }
        }

        public virtual void ClearValue()
        {
            Value = null;
            Type = null;

            if (Grid != null)
            {
                Grid.Children.Clear();
                Grid.Children.Add(new Label()); // dummy label for selecting
            }
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

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        public bool IsSelected { get; set; } = false;
        internal static void ClearGlobalSelectionCount() => GlobalSelectionCount = 0;
        static int GlobalSelectionCount = 0;
        public int? SelectionOrderId = null;

        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            if (isSelected)
            {
                SelectionOrderId = GlobalSelectionCount++;
                Grid.Background = Brushes.Yellow;
            }
            else
            {
                SelectionOrderId = null;
                Grid.Background = null;
            }
        }

        private readonly Dictionary<ValueType, SolidColorBrush> digitToColor = new()
        {
            { ValueType.Given,  Brushes.Black },
            { ValueType.Solver, Brushes.Green },
            { ValueType.User,   Brushes.Blue  }
        };

        public Point CenterPosition => Grid.TranslatePoint(
            new Point(.5 * Grid.ActualWidth, .5 * Grid.ActualHeight),
            Sudoku.Grid);
        public Point TopLeftPosition => Grid.TranslatePoint(new Point(0, 0), Sudoku.Grid);

        public void Visualize()
        {
            Grid.Children.Add(new Label()); // for clicking
            Grid.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler((_, _) =>
            {
                bool isSelected = IsSelected;
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    Sudoku.ClearSelection();
                }

                SetIsSelected(!isSelected);
            }));
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
