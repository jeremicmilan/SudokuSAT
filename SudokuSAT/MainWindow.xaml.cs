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
        public int GridWidth => (int)GridWidthSlider.Value;
        public int GridHeight => (int)GridHeightSlider.Value;
        public int BoxSize => (int)BoxSizeSlider.Value;

        private readonly SudokuSolver SudokuSolver;
        private Sudoku Sudoku { get; set; }

        private Sudoku CreateSudoku()
        {
            SudokuPlaceholder.Children.Clear();
            Sudoku = new(GridWidth, GridHeight, BoxSize, SudokuPlaceholder);
            Sudoku.GenerateAndVisualize();
            return Sudoku;
        }

        public MainWindow()
        {
            InitializeComponent();

            SudokuSolver = new(this);
            Sudoku = CreateSudoku(); // Assigning to make the compiler happy
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                CreateSudoku();
            });
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                SudokuSolver.Solve(Sudoku);
                stopwatch.Stop();
                SolveTime.Content = stopwatch.Elapsed;
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
                SudokuArrow sudokuArrow = new(Sudoku, Sudoku.SelectedSudokuCells, new Grid());
                Sudoku.SudokuElements.Add(sudokuArrow);
                sudokuArrow.Visualize();
            });
        }

        private void Whispers_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                SudokuWhispers sudokuWhispers = new(Sudoku, Sudoku.SelectedSudokuCells, (int)WhisperValueDiff.Value, new Grid());
                Sudoku.SudokuElements.Add(sudokuWhispers);
                sudokuWhispers.Visualize();
            });
        }

        private void Palindrome_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                SudokuPalindrome sudokuPalindrome = new(Sudoku, Sudoku.SelectedSudokuCells, new Grid());
                Sudoku.SudokuElements.Add(sudokuPalindrome);
                sudokuPalindrome.Visualize();
            });
        }

        private void Renban_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                SudokuRenban sudokuRenban = new(Sudoku, Sudoku.SelectedSudokuCells, new Grid());
                Sudoku.SudokuElements.Add(sudokuRenban);
                sudokuRenban.Visualize();
            });
        }

        private void HandleClickFailure(Action action)
        {
            try
            {
                action();

                Dispatcher.Invoke(() =>
                {
                    StatusBox.Foreground = Brushes.Black;
                    StatusBox.Text = null;
                });
            }
            catch (Exception exception)
            {
                Dispatcher.Invoke(() =>
                {
                    StatusBox.Foreground = Brushes.Red;
                    StatusBox.Text = exception.Message;
                });
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
            int? value = Helpers.KeyToValue(e.Key);
            bool shouldDelete = e.Key == Key.D0 || e.Key == Key.NumPad0 || e.Key == Key.Delete || e.Key == Key.Back;
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
    }
}
