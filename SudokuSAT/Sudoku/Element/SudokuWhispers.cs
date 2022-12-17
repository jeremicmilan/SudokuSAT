using Google.OrTools.Sat;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuWhispers : SudokuElementLine
    {
        public int ValueDiff { get; private set; }

        public SudokuWhispers(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int valueDiff,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        {
            ValueDiff = valueDiff;
        }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuWhispers(sudoku, SudokuCells, ValueDiff, -SudokuElementId, Grid);
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

        protected override void VisualizeInternal()
        {
            VisualizeLine(Brushes.Green);
        }
    }
}
