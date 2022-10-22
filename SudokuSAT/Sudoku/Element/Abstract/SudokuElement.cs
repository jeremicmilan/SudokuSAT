using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
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

        public abstract void AddConstraints(CpModel model, BoolVar boolVar);
        public void AddConstraints(CpModel model)
        {
            BoolVar boolVar = model.NewBoolVar(Name + "_dummy_" + Random.Shared.Next());
            model.Add(boolVar == 1);
            AddConstraints(model, boolVar);
        }

        public virtual void AddNegativeConstraints(CpModel model, BoolVar boolVar) { }
        public void AddNegativeConstraints(CpModel model)
        {
            BoolVar boolVar = model.NewBoolVar(Name + "_dummy_" + Random.Shared.Next());
            model.Add(boolVar == 1);
            AddNegativeConstraints(model, boolVar);
        }

        protected abstract void VisualizeInternal();
        public void Visualize(bool clearGrid = true)
        {
            if (clearGrid)
            {
                if (Grid == null)
                {
                    Grid = new Grid();
                }

                Debug.Assert(Sudoku.Grid != null);
                Grid.Children.Clear();
                Sudoku.Grid.Children.Add(Grid);
            }

            VisualizeInternal();
        }
    }
}
