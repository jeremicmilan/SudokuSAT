using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuRenban : SudokuElementLine
    {
        public SudokuRenban(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuRenban(sudoku, SudokuCells, -SudokuElementId, Grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            int permutationNumber = 0;
            List<BoolVar> permutationBoolVars = new();
            foreach (IList<SudokuCell> sudokuCells in SudokuCells.Permutations())
            {
                BoolVar permutationBoolVar = model.NewBoolVar(Name + "_permutation" + permutationNumber);
                permutationBoolVars.Add(permutationBoolVar);
                for (int i = 1; i < sudokuCells.Count; i++)
                {
                    model.Add(sudokuCells[i - 1].ValueVar - sudokuCells[i].ValueVar == 1).OnlyEnforceIf(permutationBoolVar);
                }

                permutationNumber++;
            }

            model.Add(LinearExpr.Sum(permutationBoolVars) == 1).OnlyEnforceIf(boolVar);
        }

        protected override void VisualizeInternal()
        {
            VisualizeLine(Brushes.DeepPink);
        }
    }
}
