using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        private Sudoku SudokuGrid;

        public MainWindow()
        {
            InitializeComponent();

            SudokuGrid = new(GridWidth, GridHeight, BoxSize, this); // For compiler to shut up
            GenerateGrid();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            GenerateGrid();
        }

        private void Solve_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            SudokuGrid.Solve();
            stopwatch.Stop();
            solveTime.Content = stopwatch.Elapsed;
        }

        private void Explore_Click(object sender, RoutedEventArgs e)
        {
            SudokuGrid.Explore();
        }

        private void GenerateGrid()
        {
            SudokuGrid = new(GridWidth, GridHeight, BoxSize, this);
            UniformGrid grid = new()
            {
                Rows = GridWidth,
                Columns = GridHeight
            };

            for (var column = 0; column < GridWidth; column++)
            {
                for (var row = 0; row < GridHeight; row++)
                {
                    Border border = CreateBorder(column, row);
                    SudokuCell sudokuCell = CreateCell(column, row);
                    SudokuGrid.SudokuGrid[column, row] = sudokuCell;
                    UniformGrid cellGrid = new()
                    {
                        Rows = 3,
                        Columns = 3
                    };
                    cellGrid.Children.Add(sudokuCell.ValueTextBox);
                    cellGrid.Children.Add(sudokuCell.SolutionsLabel);
                    border.Child = cellGrid;
                    grid.Children.Add(border);
                }
            }

            SudokuGridPlaceholder.Child = grid;
        }

        private Border CreateBorder(int column, int row)
        {
            int thick = 3, thin = 1;
            var left = row % BoxSize == 0 ? thick : thin;
            var top = column % BoxSize == 0 ? thick : thin;
            var right = row == GridHeight - 1 ? thick : 0;
            var bottom = column == GridWidth - 1 ? thick : 0;

            return new Border
            {
                BorderThickness = new Thickness(left, top, right, bottom),
                BorderBrush = Brushes.Black
            };
        }

        private static SudokuCell CreateCell(int column, int row)
        {
            var valueTextBox = new TextBox
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var solutionsLabel = new Label
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            SudokuCell sudokuCell = new(column, row, valueTextBox, solutionsLabel);

            valueTextBox.TextChanged += sudokuCell.OnValueChanged;

            return sudokuCell;
        }
    }
}
