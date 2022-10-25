using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SudokuSAT
{
    public abstract class SudokuElementLineMulti : SudokuElementLine
    {
        public int ElementCount { get; private set; }

        private SudokuElementLine SudokuElementLineVisual { get; set; }

        public SudokuElementLineMulti(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int elementCount,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
        {
            ElementCount = elementCount;
            SudokuElementLineVisual = InstantiateSubElement(sudokuCells);
        }

        public abstract SudokuElementLine InstantiateSubElement(List<SudokuCell> sudokuCells);

        private bool AreConsecutive(List<SudokuCell> sudokuCells)
        {
            for (int i = 1; i < sudokuCells.Count; i++)
            {
                if (SudokuCellsOrderDictionary[sudokuCells[i]] - SudokuCellsOrderDictionary[sudokuCells[i - 1]] != 1)
                {
                    return false;
                }
            }

            return true;
        }

        public override void AddConstraints(CpModel model, BoolVar boolVar)
        {
            List<List<SudokuCell>> sudokuCellsSubsets = SudokuCells
                .Subsets()
                .Where (sudokuCells => sudokuCells.Count >= 2)
                .Select(sudokuCells => sudokuCells.OrderBy(cell => SudokuCellsOrderDictionary[cell]).ToList())
                .Where (AreConsecutive)
                .Where (sudokuCells => AreConsecutiveCellsAdjacent(sudokuCells.ToList()))
                .Select(sudokuCells => sudokuCells.ToList())
                .ToList();
            int i = 0;
            List<BoolVar> subsetBoolVars = new();
            foreach (List<SudokuCell> sudokuCells in sudokuCellsSubsets)
            {
                BoolVar subsetBoolVar = model.NewBoolVar(Name + "_" + i++);
                subsetBoolVars.Add(subsetBoolVar);
                SudokuElementLine sudokuElementLine = InstantiateSubElement(sudokuCells);
                sudokuElementLine.AddConstraints(model, subsetBoolVar);

                /* TODO: Code for 'exactly', but it turned out pretty slow...
                List<List<SudokuCell>> sudokuCellsSubsetsWithoutSudokuCell = new();
                sudokuCellsSubsetsWithoutSudokuCell.AddRange(sudokuCellsSubsets);
                sudokuCellsSubsetsWithoutSudokuCell.Remove(sudokuCells);
                foreach (List<SudokuCell> sudokuCellsWithoutSudokuCell in sudokuCellsSubsetsWithoutSudokuCell)
                {
                    sudokuElementLine = InstantiateSubElement(sudokuCellsWithoutSudokuCell);
                    sudokuElementLine.AddNegativeConstraints(model, subsetBoolVar);
                }
                */
            }

            model.Add(LinearExpr.Sum(subsetBoolVars) == ElementCount).OnlyEnforceIf(boolVar);
        }

        protected override void VisualizeInternal()
        {
            SudokuElementLineVisual.Grid = Grid;
            SudokuElementLineVisual.Visualize(); // TODO: change color
        }
    }
}
