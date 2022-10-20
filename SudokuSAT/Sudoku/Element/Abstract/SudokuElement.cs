using Google.OrTools.Sat;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Controls;

namespace SudokuSAT
{
    public abstract class SudokuElement
    {
        private static int SudokuElementCount = 0;
        public int SudokuElementId { get; set; }

        protected Sudoku Sudoku { get; private set; }

        [JsonIgnore] public Grid? Grid { get; private set; }

        protected SudokuElement(Sudoku sudoku, Grid? grid = null)
        {
            Sudoku = sudoku;
            SudokuElementId = SudokuElementCount++;

            Grid = grid;
            if (Grid != null)
            {
                Grid.Name = Name;
            }
        }

        public string Name => "_" + SudokuElementId + "_" + GetType().Name;

        public abstract SudokuElement Clone(Sudoku sudoku);

        public abstract void AddConstraints(CpModel model);

        protected abstract void VisualizeInternal();
        public void Visualize()
        {
            Debug.Assert(Grid != null);
            Debug.Assert(Sudoku.Grid != null);

            Grid.Children.Clear();
            VisualizeInternal();
            Sudoku.Grid.Children.Add(Grid);
        }
    }
}
