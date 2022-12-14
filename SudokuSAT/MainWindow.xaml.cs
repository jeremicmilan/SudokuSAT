using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public readonly SudokuSolver SudokuSolver;
        public Sudoku Sudoku { get; private set; }

        public const string SavePath = "..\\..\\..\\Saves";
        public string SelectedSaveName => (string)SaveNames.SelectedItem;
        public bool MultiElement => MultiElementCheckbox.IsChecked != null && MultiElementCheckbox.IsChecked.Value;
        public int MultiElementCount => (int)MultipleElementCountSlider.Value;
        public static string SaveNameToFolder(string saveName) => SavePath + "\\" + saveName;
        public static string SaveNameToFile(string saveName) => SaveNameToFolder(saveName) + "\\" + saveName + ".save";
        public static string SaveFileToName(string fileName) => Path.GetFileNameWithoutExtension(fileName);
        public static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented,
        };

        public Stack<Sudoku> Sudokus { get; private set; } = new();
        public Stack<Sudoku> NextSudokus { get; private set; } = new();

        public MainWindow()
        {
            InitializeComponent();

            SudokuSolver = new(this);
            Sudoku = CreateSudoku(); // Assigning to make the compiler happy

            foreach (string fileName in Directory.GetFiles(SavePath, "*.save", SearchOption.AllDirectories))
            {
                SaveNames.Items.Add(SaveFileToName(fileName));
            }

            SudokuPlaceholder.SizeChanged += (_, _) => Sudoku.Visualize();

            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(~((~0) << Environment.ProcessorCount - 2) << 2);
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
            Sudoku = new(GridWidth, GridHeight, BoxSize, SudokuPlaceholder);
            Sudoku.SudokuRulesets = new List<SudokuRuleset> { new SudokuRulesetSudoku(Sudoku) };
            UpdateRulesetToolbar();

            Sudoku.Visualize(recreateGrid: true);
            AddSudoku(Sudoku);
            return Sudoku;
        }

        private void ReplaceSudoku(Sudoku sudoku)
        {
            Sudoku.Grid = null;
            sudoku.Grid = SudokuPlaceholder;

            Sudoku = sudoku;
            Sudoku.Visualize(recreateGrid: true);

            // Update tool-bar
            //
            UpdateUndoRedoToolbar();

            GridWidthSlider.Value = sudoku.Width;
            GridHeightSlider.Value = sudoku.Height;
            BoxSizeSlider.Value = sudoku.BoxSize;

            UpdateRulesetToolbar();
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

            SolveAsync();
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

            SolveAsync();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            HandleFailure(() => CreateSudoku());
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            HandleFailure(() =>
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

                // Old saves do not have SudokuRulesets initialized.
                //
                sudoku.SudokuRulesets ??= new List<SudokuRuleset> { new SudokuRulesetSudoku(sudoku) };

                ReplaceSudoku(sudoku);
                AddSudoku(sudoku);

                SolveAsync();
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            HandleFailure(() =>
            {
                string saveName = SaveNameTextbox.Text;
                if (saveName == "")
                {
                    throw new Exception("Please specify save name.");
                }

                string fileName = SaveNameToFile(saveName);
                string sudokuJson = JsonConvert.SerializeObject(Sudoku, JsonSerializerSettings);

                if (File.Exists(fileName))
                {
                    throw new Exception("Save with the specified name already exists.");
                }

                Directory.CreateDirectory(SaveNameToFolder(saveName));
                File.Create(fileName).Close();
                File.WriteAllText(fileName, sudokuJson);
                SaveNames.Items.Add(SaveNameTextbox.Text);
            });
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            HandleFailure(() =>
            {
                Directory.Delete(SaveNameToFolder(SelectedSaveName), recursive: true);
                SaveNames.Items.Remove(SelectedSaveName);
            });
        }

        public void UpdateUndoRedoToolbar()
        {
            UndoButton.IsEnabled = Sudoku.SudokuActions.Any();
            RedoButton.IsEnabled = Sudoku.NextSudokuActions.Any();
        }

        private void PerformUndoRedoAction(Action action)
        {
            HandleFailure(() =>
            {
                SudokuSolver.CheckIsSolveActive();
                action();
                UpdateUndoRedoToolbar();
            });
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            PerformUndoRedoAction(Sudoku.Undo);
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            PerformUndoRedoAction(Sudoku.Redo);
        }

        public void SolveAsync()
        {
            new Thread(() => HandleFailure(() =>
            {
                SudokuSolver.Solve(Sudoku, updateSolvedValue: false);
            })).Start();
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            SudokuSolver.HandleSolveAction(() => SudokuSolver.Solve(Sudoku, updateSolvedValue: true));
        }

        private void Explore_Click(object sender, RoutedEventArgs e)
        {
            SudokuSolver.HandleSolveAction(() => SudokuSolver.Explore(Sudoku, Sudoku.SelectedSudokuCells));
        }

        private void Possibilities_Click(object sender, RoutedEventArgs e)
        {
            SudokuSolver.HandleSolveAction(() => SudokuSolver.Possibilities(Sudoku, Sudoku.SelectedSudokuCells));
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            SudokuSolver.StopSolveAction();
            StopButton.IsEnabled = false;
        }

        private void Arrow_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => MultiElement ?
                new SudokuArrowMulti(Sudoku, Sudoku.SelectedSudokuCells, MultiElementCount, grid: new Grid()) :
                new SudokuArrow(Sudoku, Sudoku.SelectedSudokuCells, grid: new Grid()));
        }

        private void Whispers_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuWhispers(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                (int)WhisperValueDiff.Value,
                grid: new Grid()));
        }

        private void Palindrome_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuPalindrome(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                grid: new Grid()));
        }

        private void Renban_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuRenban(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                grid: new Grid()));
        }

        private void KillerCage_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuKillerCage(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                (int)KillerCageSumSlider.Value,
                grid: new Grid()));
        }

        private void Thermometer_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => new SudokuThermometer(
                Sudoku,
                Sudoku.SelectedSudokuCells,
                grid: new Grid()));
        }

        private void X_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => MultiElement ?
                new SudokuXMulti(Sudoku, Sudoku.SelectedSudokuCells, MultiElementCount, grid: new Grid()) :
                new SudokuX(Sudoku, Sudoku.SelectedSudokuCells, grid: new Grid()));
        }

        private void V_Click(object sender, RoutedEventArgs e)
        {
            AddSudokuElement(() => MultiElement ?
                new SudokuVMulti(Sudoku, Sudoku.SelectedSudokuCells, MultiElementCount, grid: new Grid()) :
                new SudokuV(Sudoku, Sudoku.SelectedSudokuCells, grid: new Grid()));
        }

        private void AddSudokuElement(Func<SudokuElement> instantiateSudokuElement)
        {
            HandleFailure(() => Sudoku.AddElement(instantiateSudokuElement()));
        }

        public void HandleFailure(Action action)
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
                if (!ClosingWindow)
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusBox.Foreground = Brushes.Red;
                        StatusBox.Text = exception.Message;
                    });
                }
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
            Height = Math.Max(800, height);
            MinHeight = Height;
            MaxHeight = Height;

            if (height < 800)
            {
                Width = 800 + 200 - 30;
                MinWidth = Width;
            }
        }

        private void Keyboard_KeyDown(object sender, KeyEventArgs e)
        {
            HandleFailure(() =>
            {
                int? value = Helpers.KeyToValue(e.Key);
                bool shouldDelete = e.Key == Key.D0 || e.Key == Key.NumPad0 || e.Key == Key.Delete || e.Key == Key.Back;
                if (value != null)
                {
                    Sudoku.SetValues(value.Value, Sudoku.SelectedSudokuCells);
                }
                else if (shouldDelete)
                {
                    Sudoku.SetValues(null, Sudoku.SelectedSudokuCells);
                }
            });
        }

        private bool ClosingWindow = false;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ClosingWindow = true;
            SudokuSolver.StopSolveAction();
        }

        private void CopyPossibilities_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(String.Join('\n', PossibilitiesListBox.Items.Flatten()));
        }

        public void UpdateRulesetToolbar()
        {
            CheckboxRulesetSudoku.IsChecked = Sudoku.SudokuRulesets
                .Any(ruleset => ruleset.GetType() == typeof(SudokuRulesetSudoku));

            SudokuRulesetOrthoPairSumMax? sudokuRulesetOrthoPairSumMax = Sudoku.GetSudokuRuleset<SudokuRulesetOrthoPairSumMax>();
            CheckboxRulesetOrthoPairSumMax.IsChecked = sudokuRulesetOrthoPairSumMax != null;
            SliderRulesetOrthoPairSumMax.Value = sudokuRulesetOrthoPairSumMax?.Sum ?? SliderRulesetOrthoPairSumMax.Value;

            SudokuRulesetOrthoPairSumMin? sudokuRulesetOrthoPairSumMin = Sudoku.GetSudokuRuleset<SudokuRulesetOrthoPairSumMin>();
            CheckboxRulesetOrthoPairSumMin.IsChecked = sudokuRulesetOrthoPairSumMin != null;
            SliderRulesetOrthoPairSumMin.Value = sudokuRulesetOrthoPairSumMin?.Sum ?? SliderRulesetOrthoPairSumMin.Value;
        }

        private void CheckboxRulesetSudoku_Checked(object sender, RoutedEventArgs e)
        {
            Sudoku?.PerformRulesetAction(new SudokuRulesetSudoku(Sudoku));
        }

        private void CheckboxRulesetSudoku_Unchecked(object sender, RoutedEventArgs e)
        {
            Sudoku?.PerformRulesetAction<SudokuRulesetSudoku>(sudokuRuleset: null);
        }

        private void CheckboxRulesetOrthoPairSumMax_Checked(object sender, RoutedEventArgs e)
        {
            Sudoku?.PerformRulesetAction(new SudokuRulesetOrthoPairSumMax(Sudoku, (int)SliderRulesetOrthoPairSumMax.Value));
        }

        private void CheckboxRulesetOrthoPairSumMax_Unchecked(object sender, RoutedEventArgs e)
        {
            Sudoku?.PerformRulesetAction<SudokuRulesetOrthoPairSumMax>(sudokuRuleset: null);
        }

        private void SliderRulesetOrthoPairSumMax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CheckboxRulesetOrthoPairSumMax.IsChecked ?? false)
            {
                Sudoku?.PerformRulesetAction(new SudokuRulesetOrthoPairSumMax(Sudoku, (int)SliderRulesetOrthoPairSumMax.Value));
            }
        }

        private void CheckboxRulesetOrthoPairSumMin_Checked(object sender, RoutedEventArgs e)
        {
            Sudoku?.PerformRulesetAction(new SudokuRulesetOrthoPairSumMin(Sudoku, (int)SliderRulesetOrthoPairSumMin.Value));
        }

        private void CheckboxRulesetOrthoPairSumMin_Unchecked(object sender, RoutedEventArgs e)
        {
            Sudoku?.PerformRulesetAction<SudokuRulesetOrthoPairSumMin>(sudokuRuleset: null);
        }

        private void SliderRulesetOrthoPairSumMin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CheckboxRulesetOrthoPairSumMin.IsChecked ?? false)
            {
                Sudoku?.PerformRulesetAction(new SudokuRulesetOrthoPairSumMin(Sudoku, (int)SliderRulesetOrthoPairSumMin.Value));
            }
        }
    }
}
