using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
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

        public override void AddConstraints(CpModel model)
        {
            int permutationNumber = 0;
            List<BoolVar> permutationBoolVars = new();
            foreach (IList<SudokuCell> sudokuCells in SudokuCells.Permutations())
            {
                BoolVar boolVar = model.NewBoolVar(Name + "_permutation" + permutationNumber);
                permutationBoolVars.Add(boolVar);
                for (int i = 1; i < sudokuCells.Count; i++)
                {
                    model.Add(sudokuCells[i - 1].ValueVar - sudokuCells[i].ValueVar == 1).OnlyEnforceIf(boolVar);
                }

                permutationNumber++;
            }

            model.AddExactlyOne(permutationBoolVars);
        }

#pragma warning disable CS8602 // Using Grid should be safe during visualization
        public override void Visualize()
        {
            Grid.Children.Add(new Polyline
            {
                Points = new PointCollection(SudokuCells.Select(cell => cell.CenterPosition)),
                Stroke = Brushes.DeepPink,
                StrokeThickness = 15,
                Opacity = .5,
                IsHitTestVisible = false,
            });

            Sudoku.Grid.Children.Add(Grid);
        }
#pragma warning restore CS8602 // Using Grid should be safe during visualization
    }
}
