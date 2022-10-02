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
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + (Column + 1) + "_r" + (Row + 1));

            if (Value.HasValue)
            {
                model.Add(ValueVar == Value.Value);
            }
        }

        public virtual void SetValue(int value, ValueType valueType)
        {
            Value = value;
            Type = valueType;

            UpdateGrid();
        }

        public virtual void ClearValue()
        {
            Value = null;
            Type = null;

            UpdateGrid();
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

#pragma warning disable CS8602,CS8629 // Using Grid should be safe during visualization
        public bool IsSelected { get; set; } = false;
        private static bool IsHoldingDownMouseSelecting = true;
        public static void ClearGlobalSelectionCount() => GlobalSelectionCount = 1;
        private static int GlobalSelectionCount = 1;
        public int? SelectionOrderId = null;

        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            if (isSelected)
            {
                SelectionOrderId = GlobalSelectionCount++;
                Grid.Background = Brushes.Yellow;
                Grid.Children.Add(new Label()
                {
                    Content = SelectionOrderId,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = Grid.ActualHeight * 0.15,
                    Foreground = Brushes.Orange,
                });
            }
            else
            {
                SelectionOrderId = null;
                Grid.Background = null;
                UpdateGrid();
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

        private void UpdateGrid()
        {
            if (Grid != null)
            {
                Grid.Children.Clear();

                // Create dummy label for selecting
                //
                Grid.Children.Add(new Label());

                if (Value != null)
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
                        Foreground = digitToColor[Type.Value],
                        Content = Value > 0 ? Value : "X"
                    });
                }
            }
        }

        public void Visualize()
        {
            UpdateGrid();

            Grid.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler((_, _) =>
            {
                bool isSelected = IsSelected;
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    Sudoku.ClearSelection();
                }

                SetIsSelected(!isSelected);
                IsHoldingDownMouseSelecting = !isSelected;
            }));
            Grid.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler((_, _) =>
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && IsSelected != IsHoldingDownMouseSelecting)
                {
                    SetIsSelected(!IsSelected);
                }
            }));
        }
#pragma warning restore CS8602,CS8629 // Using Grid should be safe during visualization
    }
}
