﻿using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SudokuSAT
{
    public abstract class SudokuElementLine : SudokuElementWithCellList
    {
        public SudokuElementLine(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        {
            if (!AreConsecutiveCellsAdjacent(sudokuCells))
            {
                throw new Exception("Consecutive cells are not adjacent.");
            }
        }

        public static bool AreConsecutiveCellsAdjacent(List<SudokuCell> sudokuCells)
        {
            for (int i = 1; i < sudokuCells.Count; i++)
            {
                if (!sudokuCells[i - 1].Adjacent(sudokuCells[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
