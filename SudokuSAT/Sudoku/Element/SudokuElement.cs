using Google.OrTools.Sat;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SudokuSAT
{
    public abstract class SudokuElement
    {
        private static int SudokuElementCount = 0;
        public int SudokuElementId { get; set; }

        public Sudoku Sudoku { get; private set; }
        public Grid Grid { get; private set; }

        protected SudokuElement(Sudoku sudoku)
        {
            Sudoku = sudoku;
            SudokuElementId = SudokuElementCount++;
            Grid = new Grid() { Name = Name };
        }

        public string Name => "_" + SudokuElementId + "_" + this.GetType().Name;

        public abstract SudokuElement Clone(Sudoku sudoku);

        public abstract void AddConstraints(CpModel model);

        public abstract void Visualize();
    }
}
