using System.Collections.Generic;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class SudokuXMulti : SudokuElementPairMulti
    {
        public SudokuXMulti(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int elementCount,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, elementCount, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuXMulti(sudoku, SudokuCells, ElementCount, -SudokuElementId, Grid);
        }

        public override SudokuElementPair InstantiateSubElement(List<SudokuCell> sudokuCells)
        {
            return new SudokuX(Sudoku, sudokuCells, -SudokuElementId, Grid);
        }
    }
}
