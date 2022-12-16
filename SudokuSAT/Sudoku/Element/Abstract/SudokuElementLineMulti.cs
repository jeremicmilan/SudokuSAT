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
            SudokuElementLineVisual = InstantiateVisual(sudokuCells);
        }

        public virtual SudokuElementLine InstantiateVisual(List<SudokuCell> sudokuCells) => InstantiateSubElement(sudokuCells);
        public abstract SudokuElementLine InstantiateSubElement(List<SudokuCell> sudokuCells);

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
