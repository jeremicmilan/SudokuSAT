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
    public class SudokuWhispers : SudokuElementLine
    {
        public int ValueDiff { get; private set; }

        public SudokuWhispers(Sudoku sudoku, List<SudokuCell> sudokuCells, int valueDiff, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        {
            ValueDiff = valueDiff;
        }

        protected override SudokuElementWithCellList Instantiate(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            Grid? grid = null)
        {
            return new SudokuWhispers(sudoku, sudokuCells, ValueDiff, grid);
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            for (int i = 1; i < SudokuCells.Count; i++)
            {
                BoolVar boolVarPositive = model.NewBoolVar(Name + "_combination" + i + "_positive");
                model.Add(SudokuCells[i].ValueVar - SudokuCells[i - 1].ValueVar >= ValueDiff).OnlyEnforceIf(boolVarPositive);

                BoolVar boolVarNegative = model.NewBoolVar(Name + "_combination" + i + "_negative");
                model.Add(SudokuCells[i - 1].ValueVar - SudokuCells[i].ValueVar >= ValueDiff).OnlyEnforceIf(boolVarNegative);

                model.Add(LinearExpr.Sum(new[] { boolVarPositive, boolVarNegative }) == 1).OnlyEnforceIf(boolVar);
            }
        }

        public override void Visualize()
        {
            VisualizeLine(Brushes.Green);
        }
    }
}
