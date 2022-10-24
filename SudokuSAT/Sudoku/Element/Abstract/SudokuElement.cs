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

        public Sudoku Sudoku { get; set; }

        [JsonIgnore] public Grid? Grid { get; set; }

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

        public abstract SudokuElement Clone(Sudoku sudoku, Grid? grid = null);

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

        public abstract void Visualize();
    }
}
