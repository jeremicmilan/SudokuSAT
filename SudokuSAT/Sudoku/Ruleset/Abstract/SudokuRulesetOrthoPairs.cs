using Google.OrTools.Sat;
using System.Collections.Generic;

namespace SudokuSAT
{
    public abstract class SudokuRulesetOrthoPairs : SudokuRuleset
    {
        protected SudokuRulesetOrthoPairs(Sudoku sudoku)
            : base(sudoku)
        { }

        public sealed override void AddConstraints(CpModel model)
        {
            foreach (List<SudokuCell> column in Sudoku.GetColumns())
            {
                AddOrthoPairConstraints(model, column);
            }

            foreach (List<SudokuCell> row in Sudoku.GetRows())
            {
                AddOrthoPairConstraints(model, row);
            }
        }

        private void AddOrthoPairConstraints(
            CpModel model,
            List<SudokuCell> sudokuCells)
        {
            for (int i = 1; i < sudokuCells.Count; i++)
            {
                AddOrthoPairConstraints(model, sudokuCells[i - 1], sudokuCells[i]);
            }
        }

        protected abstract void AddOrthoPairConstraints(
            CpModel model,
            SudokuCell sudokuCell1,
            SudokuCell sudokuCell2);
    }
}
