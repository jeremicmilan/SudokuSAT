using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
                SudokuSolver.Solve(Sudoku, updateSolvedValue: true);
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
            SudokuSolver.IsExploreActive = false;
            HandleClickFailure(Sudoku.Clear);
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuArrow(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                new Grid()));
        }

        private void Whispers_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuWhispers(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                (int)WhisperValueDiff.Value,
                new Grid()));
        }

        private void Palindrome_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuPalindrome(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                new Grid()));
        }

        private void Renban_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuRenban(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                new Grid()));
        }

        private void KillerCage_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuKillerCage(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                (int)KillerCageSumSlider.Value,
                new Grid()));
        }

        private void Thermometer_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuThermometer(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                new Grid()));
        }

        private void AddSudokuElement(Func<SudokuElement> instantiateSudokuElement)
        {
            HandleClickFailure(() =>
            {
                SudokuElement sudokuElement = instantiateSudokuElement();
                Sudoku.SudokuElements.Add(sudokuElement);
                sudokuElement.Visualize();

                SudokuSolver.Solve(Sudoku, updateSolvedValue: false);
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

        private void MainGrid_Click(object sender, RoutedEventArgs e)
        {
            if (SudokuCell.IsSudokuCellClicked)
            {
                SudokuCell.ClearIsSudokuCellClicked();
            }
            else
            {
                Sudoku.ClearSelection();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
