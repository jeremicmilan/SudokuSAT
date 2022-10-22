using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuRenban : SudokuElementLine
    {
        public SudokuRenban(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuRenban(sudoku, sudokuCells);
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
            Debug.Assert(Grid != null);
            Grid.Children.Add(new Polyline
            {
                Points = new PointCollection(SudokuCells.Select(cell => cell.CenterPosition)),
                Stroke = Brushes.DeepPink,
                StrokeThickness = 15,
                Opacity = .5,
                IsHitTestVisible = false,
            });
        }
    }
}
