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
    public abstract class SudokuElementLine : SudokuElementWithCellList
    {
        public SudokuElementLine(
            Sudoku sudoku,
            List<SudokuCell> sudokuCells,
            int? sudokuElementId = null,
            Grid? grid = null)
            : base(sudoku, sudokuCells, sudokuElementId, grid)
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

        protected void VisualizeLine(Brush brush)
        {
            Debug.Assert(Grid != null);
            Debug.Assert(Grid.Children.Count == 0);
            SudokuCell firstSudokuCell = SudokuCells.First();
            Debug.Assert(firstSudokuCell.Grid != null);
            Polyline polyline = new()
            {
                Stroke = brush,
                StrokeThickness = 15,
                Opacity = .5,
                IsHitTestVisible = false,
            };
            void updatePolylinePosition()
            {
                polyline.Points = new PointCollection(SudokuCells.Select(cell => cell.CenterPosition));
            }
            updatePolylinePosition();
            firstSudokuCell.Grid.SizeChanged += (_, _) => updatePolylinePosition();
            Grid.Children.Add(polyline);
        }
    }
}
