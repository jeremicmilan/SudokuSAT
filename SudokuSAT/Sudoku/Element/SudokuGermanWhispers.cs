using Google.OrTools.Sat;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public class SudokuGermanWhispers : SudokuWhispers
    {
        public const int GermanWhispersValueDiff = 5;

        public SudokuGermanWhispers(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, GermanWhispersValueDiff, grid)
        { }

        protected override SudokuElementWithCellList Instantiate(Sudoku sudoku, List<SudokuCell> sudokuCells)
        {
            return new SudokuGermanWhispers(sudoku, sudokuCells);
        }
    }
}
