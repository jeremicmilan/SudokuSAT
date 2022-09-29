using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SudokuSAT
{
    public class SudokuVisual : Sudoku
    {
        public Grid SudokuPlaceholder { get; private set; }

        public SudokuVisual(int width, int height, int boxSize, Grid sudokuPlaceholder)
            : base(width, height, boxSize)
        {
            SudokuPlaceholder = sudokuPlaceholder;
        }
    }
}
