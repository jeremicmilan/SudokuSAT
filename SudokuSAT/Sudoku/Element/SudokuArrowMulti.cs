using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuArrowMulti : SudokuElementLineMulti
    {
        public SudokuArrowMulti(Sudoku sudoku, List<SudokuCell> sudokuCells, int elementCount, Grid? grid = null)
            : base(sudoku, sudokuCells, elementCount, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuArrowMulti(sudoku, sudokuCells, ElementCount);
        }

        public override SudokuElementLine InstantiateSubElement(List<SudokuCell> sudokuCells, Grid? grid = null)
        {
            return new SudokuArrow(Sudoku, sudokuCells, grid);
        }
    }
}
