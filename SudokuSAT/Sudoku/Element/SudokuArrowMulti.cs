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
        public SudokuArrowMulti(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int elementCount,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, elementCount, sudokuElementId, grid)
        { }

        public override SudokuElement Clone(Sudoku sudoku)
        {
            return new SudokuArrowMulti(sudoku, SudokuCells, ElementCount, -SudokuElementId, Grid);
        }

        public override SudokuElementLine InstantiateSubElement(List<SudokuCell> sudokuCells)
        {
            return new SudokuArrow(Sudoku, sudokuCells, -SudokuElementId, Grid);
        }
    }
}
