using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuSAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int GridWidth => (int)gridWidth.Value;
        public int GridHeight => (int)gridHeight.Value;
        public int BoxSize => (int)boxSize.Value;

        private readonly SudokuSolver SudokuSolver;

        private Sudoku Sudoku;

        public MainWindow()
        {
            InitializeComponent();

            SudokuSolver = new(this);
            Sudoku = new(GridWidth, GridHeight, BoxSize); // For compiler to shut up
            GenerateGrid();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(GenerateGrid);
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                SudokuSolver.Solve(Sudoku);
                stopwatch.Stop();
                solveTime.Content = stopwatch.Elapsed;
            });
        }

        private void Explore_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => HandleClickFailure(() => SudokuSolver.Explore(Sudoku))).Start();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(Sudoku.Load);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(Sudoku.Clear);
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                SudokuArrow sudokuArrow = new SudokuArrow(Sudoku.SelectedSudokuCells);
                Sudoku.SudokuElements.Add(sudokuArrow);
                sudokuArrow.Visualize();
            });
        }

        private void HandleClickFailure(Action action)
        {
            try
            {
                action();

                StatusBox.Foreground = Brushes.Black;
                StatusBox.Text = null;
            }
            catch (Exception exception)
            {
                StatusBox.Foreground = Brushes.Red;
                StatusBox.Text = exception.Message;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = e.NewSize.Width - 200 /* sidebar */ + 30 /* top */;
            height = Math.Max(800, height);
            MinHeight = height;
            MaxHeight = height;
        }

        private void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            int? value = null;
            bool shouldDelete = false;
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                case Key.Delete:
                case Key.Back:
                    shouldDelete = true;
                    break;

                case Key.D1:
                case Key.NumPad1:
                    value = 1;
                    break;

                case Key.D2:
                case Key.NumPad2:
                    value = 2;
                    break;

                case Key.D3:
                case Key.NumPad3:
                    value = 3;
                    break;

                case Key.D4:
                case Key.NumPad4:
                    value = 4;
                    break;

                case Key.D5:
                case Key.NumPad5:
                    value = 5;
                    break;

                case Key.D6:
                case Key.NumPad6:
                    value = 6;
                    break;

                case Key.D7:
                case Key.NumPad7:
                    value = 7;
                    break;

                case Key.D8:
                case Key.NumPad8:
                    value = 8;
                    break;

                case Key.D9:
                case Key.NumPad9:
                    value = 9;
                    break;
            }

            foreach (SudokuCell sudokuCell in Sudoku.SelectedSudokuCells)
            {
                if (shouldDelete)
                {
                    sudokuCell.ClearValue();
                }
                else if (value.HasValue)
                {
                    sudokuCell.SetValue(value.Value, ValueType.User);
                }
            }
        }

        private void GenerateGrid()
        {
            Sudoku = new(GridWidth, GridHeight, BoxSize);
            UniformGrid mainGrid = new()
            {
                Rows = GridHeight,
                Columns = GridWidth
            };

            for (var row = 0; row < GridHeight; row++)
            {
                for (var column = 0; column < GridWidth; column++)
                {
                    Border border = CreateBorder(column, row);
                    mainGrid.Children.Add(border);

                    Grid grid = new();
                    border.Child = grid;
                    grid.Children.Add(new Label()); // for clicking
                    SudokuCell sudokuCell = new(column, row, grid);
                    grid.AddHandler(MouseLeftButtonDownEvent, new RoutedEventHandler((_, _) =>
                    {
                        bool isSelected = sudokuCell.IsSelected;
                        if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            Sudoku.ClearSelection();
                        }

                        sudokuCell.SetIsSelected(!isSelected);
                    }));
                    Sudoku.SudokuGrid[column, row] = sudokuCell;
                }
            }

            SudokuGridPlaceholder.Child = mainGrid;
        }

        private Border CreateBorder(int column, int row)
        {
            int thick = 3, thin = 1;
            var top = row % BoxSize == 0 ? thick : thin;
            var left = column % BoxSize == 0 ? thick : thin;
            var bottom = row == GridHeight - 1 ? thick : 0;
            var right = column == GridWidth - 1 ? thick : 0;

            return new Border
            {
                BorderThickness = new Thickness(left, top, right, bottom),
                BorderBrush = Brushes.Black
            };
        }
    }
}
