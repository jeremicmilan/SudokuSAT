using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class SudokuX : SudokuElementPair
    {
        public SudokuX(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuX(sudoku, SudokuCells, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            model.Add(FirstSudokuCell.ValueVar + SecondSudokuCell.ValueVar == 10).OnlyEnforceIf(boolVar);
        }

        protected override void VisualizeInternal()
        {
            VisualizeContentOnLine("X");
        }
    }
}
