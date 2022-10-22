using Google.OrTools.Sat;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuCell
    {
        public const int MinValue = 1;
        public const int MaxValue = 9;

        private Sudoku Sudoku { get; set; }

        public int Column { get; set; }
        public int Row { get; set; }

        public int? Value { get; set; }
        public ValueType? Type { get; set; }
        public HashSet<int> PossibleValues { get; set; } = new();
        [JsonIgnore] public IntVar? ValueVar { get; set; }

        [JsonIgnore] public Grid? Grid { get; set; }

        public string Name => "_" + this.GetType().Name + "_R" + Row + "_C" + Column;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SudokuCell() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public SudokuCell(Sudoku sudoku, int column, int row, int? value = null, ValueType? type = null, Grid? grid = null)
        {
            Sudoku = sudoku;
            Column = column;
            Row = row;
            Value = value;
            Type = type;

            Grid = grid;
            if (Grid != null)
            {
                Grid.Name = Name;
            }
        }

        public SudokuCell Clone(Sudoku sudoku)
        {
            return new(sudoku, Column, Row, Value, Type);
        }

        public void AddValueConstrainct(CpModel model)
        {
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + (Column + 1) + "_r" + (Row + 1));

            if (Value.HasValue)
            {
                model.Add(ValueVar == Value.Value);
            }
        }

        public void SetValue(int value, ValueType valueType)
        {
            Value = value;
            Type = valueType;
            Visualize();
        }

        public void AddPossibleValue(int value)
        {
            PossibleValues.Add(value);
            Visualize();
        }

        public void AddPossibleValues(HashSet<int> values)
        {
            values.ForEach(value => PossibleValues.Add(value));
            Visualize();
        }

        public void ClearValue()
        {
            Value = null;
            Type = null;

            Visualize();
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

        [JsonIgnore] public SudokuCell? Top    => Row - 1    >= 0             ? Sudoku.SudokuGrid[Column    , Row - 1] : null;
        [JsonIgnore] public SudokuCell? Bottom => Row + 1    <  Sudoku.Height ? Sudoku.SudokuGrid[Column    , Row + 1] : null;
        [JsonIgnore] public SudokuCell? Left   => Column - 1 >= 0             ? Sudoku.SudokuGrid[Column - 1, Row    ] : null;
        [JsonIgnore] public SudokuCell? Right  => Column + 1 <  Sudoku.Width  ? Sudoku.SudokuGrid[Column + 1, Row    ] : null;

        public IEnumerable<SudokuCell> AdjacentSudokuCells()
        {
            HashSet<SudokuCell> sudokuCells = new();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(
                        i < 0 && Column == 0 || i > 0 && Column == Sudoku.Width - 1 ||
                        j < 0 && Row == 0 || j > 0 && Row == Sudoku.Height - 1))
                    {
                        sudokuCells.Add(Sudoku.SudokuGrid[Column + i, Row + j]);
                    }
                }
            }

            return sudokuCells;
        }

        public IEnumerable<SudokuCell> OrthoAdjacentSudokuCells()
        {
            HashSet<SudokuCell> sudokuCells = new();
            for (int i = -1; i <= 1; i++)
            {
                if (!(i < 0 && Column == 0 || i > 0 && Column == Sudoku.Width - 1))
                {
                    sudokuCells.Add(Sudoku.SudokuGrid[Column + i, Row]);
                }

                if (!(i < 0 && Row == 0 || i > 0 && Row == Sudoku.Height - 1))
                {
                    sudokuCells.Add(Sudoku.SudokuGrid[Column, Row + i]);
                }
            }

            return sudokuCells;
        }

        [JsonIgnore] public Border Border
        {
            get
            {
                Debug.Assert(Grid != null);
                return (Border)Grid.Parent;
            }
        }

        public bool IsSelected { get; set; } = false;
        public static void ClearGlobalSelectionCount() => GlobalSelectionCount = 1;
        private static int GlobalSelectionCount = 1;
        public int? SelectionOrderId = null;
        public static void ClearIsSudokuCellClicked() => IsSudokuCellClicked = false;
        public static bool IsSudokuCellClicked { get; private set; } = false;

        private static SudokuCell? LastDeselectedSudokuCell { get; set; } = null;

        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            if (isSelected)
            {
                SelectionOrderId = GlobalSelectionCount++;
            }
            else
            {
                SelectionOrderId = null;
            }

            Visualize();
        }

        private readonly Dictionary<ValueType, SolidColorBrush> digitToColor = new()
        {
            { ValueType.Given,  Brushes.Black },
            { ValueType.Solver, Brushes.Green },
            { ValueType.User,   Brushes.Blue  }
        };

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        private Point TranslatePoint(Point point) => Grid.TranslatePoint(point, Sudoku.Grid);
        [JsonIgnore] public Point CenterPosition      => TranslatePoint(new Point(.5 * Grid.ActualWidth, .5 * Grid.ActualHeight));
        [JsonIgnore] public Point TopPosition         => TranslatePoint(new Point(.5 * Grid.ActualWidth, 0                     ));
        [JsonIgnore] public Point BottomPosition      => TranslatePoint(new Point(.5 * Grid.ActualWidth, Grid.ActualHeight     ));
        [JsonIgnore] public Point LeftPosition        => TranslatePoint(new Point(0                    , .5 * Grid.ActualHeight));
        [JsonIgnore] public Point RightPosition       => TranslatePoint(new Point(Grid.ActualWidth     , .5 * Grid.ActualHeight));
        [JsonIgnore] public Point TopLeftPosition     => TranslatePoint(new Point(0                    , 0                     ));
        [JsonIgnore] public Point TopRightPosition    => TranslatePoint(new Point(Grid.ActualWidth     , 0                     ));
        [JsonIgnore] public Point BottomLeftPosition  => TranslatePoint(new Point(0                    , Grid.ActualHeight     ));
        [JsonIgnore] public Point BottomRightPosition => TranslatePoint(new Point(Grid.ActualWidth     , Grid.ActualHeight     ));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        public void Visualize()
        {
            if (Grid != null)
            {
                Grid.Children.Clear();
                Grid.SizeChanged += (_, _) => Visualize();

                // Create dummy label for selecting
                //
                Label selectLabel = new Label();
                Grid.Children.Add(selectLabel);
                selectLabel.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler((_, _) =>
                {
                    IsSudokuCellClicked = true;

                    bool newIsSelected = !IsSelected;
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        if (Sudoku.SelectedSudokuCells.Count > 1)
                        {
                            newIsSelected = true;
                        }

                        Sudoku.ClearSelection();
                    }

                    SetIsSelected(newIsSelected);
                    if (!newIsSelected)
                    {
                        LastDeselectedSudokuCell = this;
                    }

                    if (!Sudoku.SelectedSudokuCells.Any())
                    {
                        Sudoku.ClearSelection();
                    }
                }));
                selectLabel.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler((_, _) =>
                {
                    if (Mouse.LeftButton == MouseButtonState.Pressed && !IsSelected && LastDeselectedSudokuCell != this)
                    {
                        SetIsSelected(!IsSelected);
                    }
                }));

                if (Grid.ActualHeight > 0)
                {
                    if (Value != null)
                    {
                        Debug.Assert(Type != null);
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

                    if (IsSelected)
                    {
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
                        Grid.Background = null;
                    }

                    if (Value == null && PossibleValues != null && PossibleValues.Count > 0)
                    {
                        UniformGrid cellGrid = new()
                        {
                            Rows = 3,
                            Columns = 3
                        };
                        Grid.Children.Add(cellGrid);

                        for (int i = 1; i <= cellGrid.Rows * cellGrid.Columns; i++)
                        {
                            if (PossibleValues.Contains(i))
                            {
                                Label label = new()
                                {
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    HorizontalContentAlignment = HorizontalAlignment.Center,
                                    VerticalContentAlignment = VerticalAlignment.Center,
                                    MinWidth = Grid.ActualWidth / 3,
                                    MinHeight = Grid.ActualHeight / 3,
                                    FontSize = Grid.ActualHeight * 0.18,
                                    Foreground = Brushes.Green,
                                    Content = i
                                };
                                cellGrid.Children.Add(label);
                            }
                            else
                            {
                                cellGrid.Children.Add(new Label());
                            }
                        }
                    }
                }
            }
        }
    }
}
