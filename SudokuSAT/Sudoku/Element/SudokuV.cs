using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class SudokuV : SudokuElementPair
    {
        public SudokuV(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuV(sudoku, SudokuCells, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            model.Add(FirstSudokuCell.ValueVar + SecondSudokuCell.ValueVar == 5);
        }

        protected override void VisualizeInternal()
        {
            VisualizeContentOnLine("V");
        }
    }
}
