using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSAT
{
    public class SudokuRulesetCellsUnique : SudokuRulesetWithCellList
    {
        public SudokuRulesetCellsUnique(Sudoku sudoku, List<SudokuCell> sudokuCells)
            : base(sudoku, sudokuCells)
        { }

        public override SudokuRulesetCellsUnique Clone(Sudoku sudoku)
        {
            return new SudokuRulesetCellsUnique(sudoku, SudokuCells);
        }

        public override void AddConstraints(CpModel model)
        {
            model.AddAllDifferent(SudokuCells.Select(cell => cell.ValueVar));
        }
    }
}
