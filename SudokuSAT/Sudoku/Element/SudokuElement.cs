using Google.OrTools.Sat;
using System;

namespace SudokuSAT
{
    public abstract class SudokuElement
    {
        public abstract SudokuElement Clone(Sudoku sudoku);

        public abstract void AddConstraints(CpModel model);

        public abstract void Visualize();
    }
}
