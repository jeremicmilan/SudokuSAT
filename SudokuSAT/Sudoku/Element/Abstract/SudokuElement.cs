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

        protected SudokuElement(Sudoku sudoku, int? sudokuElementId = null, Grid? grid = null)
        {
            Sudoku = sudoku;
            
            SudokuElementId = sudokuElementId ?? ++SudokuElementCount;
            if (SudokuElementCount < SudokuElementId) SudokuElementCount = SudokuElementId;

            Grid = grid;
        }

        public string Name =>
            "_" + (SudokuElementId > 0 ? SudokuElementId : "0" + (-SudokuElementId)) +
            "_" + GetType().Name;

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
        public void Visualize()
        {
            Debug.Assert(Grid != null);
            Grid.Name = Name;
            VisualizeInternal();
        }
    }
}
