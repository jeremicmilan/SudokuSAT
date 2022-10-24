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
        public HashSet<int>? PossibleValues { get; set; } = null;
        private int? PossibleValue
        {
            get
            {
                if (PossibleValues == null)
                {
                    return null;
                }

                return PossibleValues.Count switch
                {
                    0 => 0,
                    1 => PossibleValues.First(),
                    _ => null,
                };
            }
        }
        public int? ComputedValue => Value ?? PossibleValue;

        [JsonIgnore] public IntVar? ValueVar { get; set; }

        [JsonIgnore] public Grid? Grid { get; set; }

        public string Name => "_" + this.GetType().Name + "_R" + Row + "_C" + Column;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SudokuCell() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public SudokuCell(
            Sudoku sudoku,
            int column, int row,
            int? value = null, ValueType? type = null,
            HashSet<int>? possibleValues = null,
            Grid? grid = null)
        {
            Sudoku = sudoku;
            Column = column;
            Row = row;
            Value = value;
            Type = type;
            PossibleValues = possibleValues;

            Grid = grid;
            if (Grid != null)
            {
                Grid.Name = Name;
            }
        }

        public SudokuCell Clone(Sudoku sudoku, Grid? grid = null)
        {
            return new(sudoku, Column, Row, Value, Type, PossibleValues, grid);
        }

        public void AddValueConstrainct(CpModel model)
        {
            ValueVar = model.NewIntVar(MinValue, MaxValue, "cell_c" + (Column + 1) + "_r" + (Row + 1));

            if (ComputedValue.HasValue)
            {
                model.Add(ValueVar == ComputedValue.Value);
            }
        }

        public void SetValue(int value, ValueType valueType)
        {
            Value = value;
            Type = valueType;
            Visualize();
        }

        public void SetPossibleValues(HashSet<int>? possibleValues)
        {
            PossibleValues = possibleValues;
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

        private Label? SelectLabel { get; set; }
        private HashSet<int>? VisualizedPossibleValues { get; set; }
        private bool VisualizedIsSelected { get; set; } = false;
        private int? VisualizedComputedValue { get; set; } = null;

        public void Visualize(bool recreateElements = false)
        {
            recreateElements = recreateElements
                || SelectLabel == null
                || VisualizedIsSelected != IsSelected
                || VisualizedComputedValue != ComputedValue
                || VisualizedPossibleValues != PossibleValues;
            if (Grid == null || !recreateElements)
            {
                return;
            }

            Grid.Children.Clear();

            SelectLabel = new();
            Panel.SetZIndex(SelectLabel, 100);
            SelectLabel.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler((_, _) =>
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
            SelectLabel.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler((_, _) =>
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed && !IsSelected && LastDeselectedSudokuCell != this)
                {
                    SetIsSelected(!IsSelected);
                }
            }));
            Grid.Children.Add(SelectLabel);

            VisualizedComputedValue = ComputedValue;
            if (ComputedValue != null)
            {
                double fontSizeFactor = 0.65;

                Label valueLabel = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Foreground = digitToColor[Type ?? ValueType.Solver],
                    Content = ComputedValue > 0 ? ComputedValue : "X"
                };
                void updateValueLabel()
                {
                    valueLabel.MinWidth = Grid.ActualWidth;
                    valueLabel.MinHeight = Grid.ActualHeight;
                    valueLabel.FontSize = (Grid.ActualHeight + 1) * fontSizeFactor;
                }
                updateValueLabel();
                Grid.SizeChanged += (_, _) => updateValueLabel();
                Grid.Children.Add(valueLabel);
            }

            VisualizedIsSelected = IsSelected;
            if (IsSelected)
            {
                Grid.Background = Brushes.Yellow;
                Label selectionOrderIdLabel = new()
                {
                    Content = SelectionOrderId,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Foreground = Brushes.Orange,
                };
                void updateSelectionOrderIdLabel()
                {
                    double fontSizeFactor = 0.15;
                    selectionOrderIdLabel.FontSize = Grid.ActualHeight * fontSizeFactor;
                }
                updateSelectionOrderIdLabel();
                Grid.SizeChanged += (_, _) => updateSelectionOrderIdLabel();
                Grid.Children.Add(selectionOrderIdLabel);
            }
            else
            {
                Grid.Background = null;
            }

            VisualizedPossibleValues = PossibleValues;
            if (ComputedValue == null && PossibleValues != null)
            {
                UniformGrid possibleValuesUniformGrid = new()
                {
                    Rows = 3,
                    Columns = 3
                };

                for (int i = 1; i <= possibleValuesUniformGrid.Rows * possibleValuesUniformGrid.Columns; i++)
                {
                    if (PossibleValues.Contains(i))
                    {
                        Label possibleValuesLabel = new()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Foreground = Brushes.Green,
                            Content = i
                        };
                        void updatePossibleValuesLabel()
                        {
                            double minWidthFactor = 0.33, minHeightFactor = 0.33, fontSizeFactor = 0.18;
                            possibleValuesLabel.MinWidth = Grid.ActualWidth * minWidthFactor;
                            possibleValuesLabel.MinHeight = Grid.ActualHeight * minHeightFactor;
                            possibleValuesLabel.FontSize = Grid.ActualHeight * fontSizeFactor;
                        }
                        updatePossibleValuesLabel();
                        Grid.SizeChanged += (_, _) => updatePossibleValuesLabel();
                        possibleValuesUniformGrid.Children.Add(possibleValuesLabel);
                    }
                    else
                    {
                        possibleValuesUniformGrid.Children.Add(new Label());
                    }
                }

                Grid.Children.Add(possibleValuesUniformGrid);
            }
        }
    }
}
