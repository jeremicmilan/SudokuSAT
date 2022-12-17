using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuPalindrome : SudokuElementLine
    {
        public SudokuPalindrome(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuPalindrome(sudoku, SudokuCells, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            for (int i = 0; i < SudokuCells.Count / 2; i++)
            {
                model.Add(SudokuCells[i].ValueVar == SudokuCells[SudokuCells.Count - i - 1].ValueVar)
                    .OnlyEnforceIf(boolVar);
            }
        }

        protected override void VisualizeInternal()
        {
            VisualizeLine(Brushes.Orange);
        }
    }
}
