using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Google.OrTools.Sat;
using System.Windows.Input;

namespace SudokuSAT
{
    public class Sudoku
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int BoxSize { get; private set; }

        public SudokuCell[,] SudokuGrid { get; private set; }

        public Sudoku(int width, int height, int boxSize)
        {
            Width = width;
            Height = height;
            SudokuGrid = new SudokuCell[Width, Height];
            BoxSize = boxSize;
        }

        public Sudoku Clone()
        {
            Sudoku sudoku = new(Width, Height, BoxSize);
            foreach (var cell in SudokuCells)
            {
                sudoku.SudokuGrid[cell.Column, cell.Row] = cell.Clone(sudoku);
            }

            foreach (SudokuElement sudokuElement in SudokuElements)
            {
                sudoku.SudokuElements.Add(sudokuElement.Clone(sudoku));
            }

            return sudoku;
        }

        public List<SudokuCell> SudokuCells
        {
            get
            {
                List<SudokuCell> sudokuCells = new();

                for (var column = 0; column < Width; column++)
                {
                    for (var row = 0; row < Height; row++)
                    {
                        sudokuCells.Add(SudokuGrid[column, row]);
                    }
                }

                return sudokuCells;
            }
        }

        internal void Clear()
        {
            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                sudokuCell.ClearValue();
            }
        }

        public CpModel GenerateModel()
        {
            CpModel model = new();
            AddCellConstraints(model); // This must always be first
            AddColumnConstraints(model);
            AddRowConstraints(model);
            AddBoxConstraints(model);
            AddElementConstraints(model);
            return model;
        }

        private void AddCellConstraints(CpModel model)
        {
            foreach (SudokuCell sudokuCell in SudokuGrid)
            {
                sudokuCell.AddValueConstrainct(model);
            }
        }

        private void AddColumnConstraints(CpModel model)
        {
            for (var column = 0; column < Width; column++)
            {
                AddAllDifferent(model, GetColumn(column));
            }
        }

        private SudokuCell[] GetColumn(int columnNumber)
        {
            return Enumerable.Range(0, SudokuGrid.GetLength(1))
                    .Select(x => SudokuGrid[columnNumber, x])
                    .ToArray();
        }

        private void AddRowConstraints(CpModel model)
        {
            for (var row = 0; row < Height; row++)
            {
                AddAllDifferent(model, GetRow(row));
            }
        }

        private SudokuCell[] GetRow(int rowNumber)
        {
            return Enumerable.Range(0, SudokuGrid.GetLength(0))
                    .Select(x => SudokuGrid[x, rowNumber])
                    .ToArray();
        }

        private void AddBoxConstraints(CpModel model)
        {
            for (int columnBox = 0; columnBox <= Width / BoxSize; columnBox++)
            {
                for (int rowBox = 0; rowBox <= Height / BoxSize; rowBox++)
                {
                    List<SudokuCell> boxCells = new();

                    for (int column = columnBox * BoxSize;
                        column < Math.Min((columnBox + 1) * BoxSize, Width);
                        column++)
                    {
                        for (int row = rowBox * BoxSize;
                            row < Math.Min((rowBox + 1) * BoxSize, Height);
                            row++)
                        {
                            boxCells.Add(SudokuGrid[column, row]);
                        }
                    }

                    AddAllDifferent(model, boxCells);
                }
            }
        }

        private static void AddAllDifferent(CpModel model, IEnumerable<SudokuCell> cells)
        {
            model.AddAllDifferent(cells.Select(cell => cell.ValueVar));
        }

        public List<SudokuElement> SudokuElements { get; private set; } = new();

        private void AddElementConstraints(CpModel model)
        {
            foreach (SudokuElement sudokuElement in SudokuElements)
            {
                sudokuElement.AddConstraints(model);
            }
        }

        internal void Load()
        {
            int[,] values = new int[9, 9]
            {
                { 0, 0, 5, 0, 0, 0, 3, 1, 0, },
                { 0, 8, 0, 4, 1, 0, 0, 7, 0, },
                { 1, 3, 0, 0, 0, 0, 0, 2, 0, },
                { 0, 5, 0, 0, 6, 7, 8, 0, 0, },
                { 3, 0, 0, 0, 0, 9, 0, 0, 4, },
                { 0, 9, 0, 0, 0, 0, 0, 0, 0, },
                { 0, 0, 0, 1, 0, 5, 7, 4, 0, },
                { 0, 0, 0, 0, 0, 4, 0, 0, 0, },
                { 0, 0, 6, 7, 0, 0, 0, 0, 0, },
            };

            foreach (SudokuCell sudokuCell in SudokuCells)
            {
                int value = values[sudokuCell.Column, sudokuCell.Row];
                if (value > 0)
                {
                    sudokuCell.SetValue(value, ValueType.Given);
                }
                else
                {
                    sudokuCell.ClearValue();
                }
            }
        }
    }
}
