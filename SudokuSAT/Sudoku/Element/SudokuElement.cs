using Google.OrTools.Sat;
using System;

namespace SudokuSAT
{
    public abstract class SudokuElement
    {
        public Sudoku Sudoku { get; private set; }

        protected SudokuElement(Sudoku sudoku)
        {
            Sudoku = sudoku;
        }

        public abstract SudokuElement Clone(Sudoku sudoku);

        public abstract void AddConstraints(CpModel model);

        public abstract void Visualize();
    }
}
