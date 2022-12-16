using System.Collections.Generic;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class SudokuVMulti : SudokuElementPairMulti
    {
        public SudokuVMulti(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int elementCount,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, elementCount, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuVMulti(sudoku, SudokuCells, ElementCount, -SudokuElementId, Grid);
        }

        public override SudokuElementPair InstantiateSubElement(List<SudokuCell> sudokuCells)
        {
            return new SudokuV(Sudoku, sudokuCells, -SudokuElementId, Grid);
        }
    }
}
