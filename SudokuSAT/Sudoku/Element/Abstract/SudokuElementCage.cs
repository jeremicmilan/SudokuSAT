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
    public abstract class SudokuElementCage : SudokuElementWithCellList
    {
        public SudokuElementCage(Sudoku sudoku, List<SudokuCell> sudokuCells, Grid? grid = null)
            : base(sudoku, sudokuCells, grid)
        {
            HashSet<SudokuCell> sudokuCellsTraversed = new();
            Queue<SudokuCell> sudokuCellsUnvisited = new();
            sudokuCellsUnvisited.Enqueue(sudokuCells[0]);
            while (sudokuCellsUnvisited.Count > 0)
            {
                SudokuCell sudokuCell = sudokuCellsUnvisited.Dequeue();
                foreach (SudokuCell sudokuCellAdjacent in sudokuCell.OrthoAdjacentSudokuCells())
                {
                    if (sudokuCells.Contains(sudokuCellAdjacent) &&
                        !sudokuCellsTraversed.Contains(sudokuCellAdjacent))
                    {
                        sudokuCellsTraversed.Add(sudokuCellAdjacent);
                        sudokuCellsUnvisited.Enqueue(sudokuCellAdjacent);
                    }
                }
            }

            if (sudokuCellsTraversed.Count != sudokuCells.Count)
            {
                throw new Exception("Invalid cage as not all fields are ortho adjacent");
            }

            foreach (SudokuCell sudokuCell in sudokuCellsTraversed)
            {
                if (!sudokuCells.Contains(sudokuCell))
                {
                    throw new Exception("Error while determining cage validity.");
                }
            }
        }
    }
}
