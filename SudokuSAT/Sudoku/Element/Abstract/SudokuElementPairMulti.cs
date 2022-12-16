using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SudokuSAT
{
    public abstract class SudokuElementPairMulti : SudokuElementLineMulti
    {
        public SudokuElementPairMulti(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int elementCount,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, elementCount, sudokuElementId, grid)
        { }

        public override abstract SudokuElementPair InstantiateSubElement(List<SudokuCell> sudokuCells);

        public override SudokuElementLine InstantiateVisual(List<SudokuCell> sudokuCells)
        {
            // Maybe we should have a generic line visual here, instead of the Palindrome hack?
            //
            return new SudokuPalindrome(Sudoku, sudokuCells, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            List<BoolVar> pairBoolVars = new();
            for (int i = 0; i < SudokuCells.Count - 1; i++)
            {
                BoolVar pairBoolVar = model.NewBoolVar(Name + "_" + i);
                pairBoolVars.Add(pairBoolVar);
                SudokuElementPair sudokuElementPair = InstantiateSubElement(new List<SudokuCell>{ SudokuCells[i], SudokuCells[i + 1] });
                sudokuElementPair.AddConstraints(model, pairBoolVar);
            }

            model.Add(LinearExpr.Sum(pairBoolVars) == ElementCount).OnlyEnforceIf(boolVar);
        }
    }
}
