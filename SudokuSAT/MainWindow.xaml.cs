using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public Sudoku Sudoku { get; private set; }

        public const string SavePath = "..\\..\\..\\Saves";
        public string SelectedSaveName => (string)SaveNames.SelectedItem;
        public static string SaveNameToFile(string saveName) => SavePath + "\\" + saveName + ".save";
        public static string SaveFileToName(string fileName) => Path.GetFileNameWithoutExtension(fileName);
        public static readonly JsonSerializerSettings JsonSerializerSettings = new() { TypeNameHandling = TypeNameHandling.All };

        public Stack<Sudoku> Sudokus { get; private set; } = new();
        public Stack<Sudoku> NextSudokus { get; private set; } = new();

        public MainWindow()
        {
            InitializeComponent();

            SudokuSolver = new(this);
            Sudoku = CreateSudoku(); // Assigning to make the compiler happy

            foreach (string fileName in Directory.GetFiles(SavePath))
            {
                SaveNames.Items.Add(SaveFileToName(fileName));
            }
        }

        private void AddSudoku(Sudoku sudoku)
        {
            Sudokus.Push(sudoku);
            NextSudokus.Clear();

            NextButton.IsEnabled = false;
            if (Sudokus.Count > 1)
            {
                PreviousButton.IsEnabled = true;
            }
        }

        private Sudoku CreateSudoku()
        {
            SudokuPlaceholder.Children.Clear();
            Sudoku = new(GridWidth, GridHeight, BoxSize, SudokuPlaceholder);
            Sudoku.GenerateAndVisualize();
            AddSudoku(Sudoku);
            return Sudoku;
        }

        private void ReplaceSudoku(Sudoku sudoku)
        {
            GridWidthSlider.Value = sudoku.Width;
            GridHeightSlider.Value = sudoku.Height;
            BoxSizeSlider.Value = sudoku.BoxSize;

            SudokuPlaceholder.Children.Clear();

            Sudoku = sudoku;
            Sudoku.Grid = SudokuPlaceholder;
            Sudoku.GenerateAndVisualize();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (Sudokus.Count > 1)
            {
                Sudoku sudoku = Sudokus.Pop();
                NextSudokus.Push(sudoku);
                ReplaceSudoku(Sudokus.Peek());
            }

            NextButton.IsEnabled = true;
            if (Sudokus.Count == 1)
            {
                PreviousButton.IsEnabled = false;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (NextSudokus.Any())
            {
                Sudoku sudoku = NextSudokus.Pop();
                Sudokus.Push(sudoku);
                ReplaceSudoku(sudoku);
            }

            PreviousButton.IsEnabled = true;
            if (!NextSudokus.Any())
            {
                NextButton.IsEnabled = false;
            }
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() => CreateSudoku());
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                if (SelectedSaveName == "")
                {
                    throw new Exception("Please select save name to load.");
                }

                string sudokuJson = File.ReadAllText(SaveNameToFile(SelectedSaveName));
                Sudoku? sudoku = JsonConvert.DeserializeObject<Sudoku>(sudokuJson, JsonSerializerSettings);
                if (sudoku == null)
                {
                    throw new Exception("Failed to parse sudoku from file.");
                }

                ReplaceSudoku(sudoku);
                AddSudoku(sudoku);
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                if (SaveNameTextbox.Text == "")
                {
                    throw new Exception("Please specify save name.");
                }

                string fileName = SaveNameToFile(SaveNameTextbox.Text);
                string sudokuJson = JsonConvert.SerializeObject(Sudoku, JsonSerializerSettings);

                if (File.Exists(fileName))
                {
                    throw new Exception("Save with the specified name already exists.");
                }

                File.Create(fileName).Close();
                File.WriteAllText(fileName, sudokuJson);
                SaveNames.Items.Add(SaveNameTextbox.Text);
            });
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() =>
            {
                File.Delete(SaveNameToFile(SelectedSaveName));
                SaveNames.Items.Remove(SelectedSaveName);
            });
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            Sudoku.Undo();

            RedoButton.IsEnabled = true;
            if (!Sudoku.SudokuActions.Any())
            {
                NextButton.IsEnabled = false;
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            Sudoku.Redo();

            UndoButton.IsEnabled = true;
            if (!Sudoku.NextSudokuActions.Any())
            {
                NextButton.IsEnabled = false;
            }
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            HandleClickFailure(() => SudokuSolver.Solve(Sudoku, updateSolvedValue: true));
        }

        private void Explore_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => HandleClickFailure(() => SudokuSolver.Explore(Sudoku))).Start();
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
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NameValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9a-zA-Z]+");
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
