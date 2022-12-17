using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace SudokuSAT
{
    public abstract class SudokuRuleset
    {
        public Sudoku Sudoku { get; set; }

        protected SudokuRuleset(Sudoku sudoku)
        {
            Sudoku = sudoku;
        }

        public abstract SudokuRuleset Clone(Sudoku sudoku);

        public string Name => GetType().Name;

        public abstract void AddConstraints(CpModel model);
    }
}
