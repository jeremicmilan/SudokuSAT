using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace SudokuSAT
{
    public class SudokuRulesetRows : SudokuRulesetComposite
    {
        public SudokuRulesetRows(Sudoku sudoku)
            : base(sudoku, GetRulesetsRows(sudoku))
        { }

        public override SudokuRulesetRows Clone(Sudoku sudoku)
        {
            return new SudokuRulesetRows(sudoku);
        }

        private static List<SudokuRuleset> GetRulesetsRows(Sudoku sudoku)
        {
            List<SudokuRuleset> sudokuRulesets = new();
            for (int row = 0; row < sudoku.Height; row++)
            {
                sudokuRulesets.Add(new SudokuRulesetCellsUnique(sudoku, GetRow(sudoku, row)));
            }

            return sudokuRulesets;
        }

        private static List<SudokuCell> GetRow(Sudoku sudoku, int rowNumber)
        {
            return Enumerable.Range(0, sudoku.SudokuGrid.GetLength(0))
                .Select(x => sudoku.SudokuGrid[x, rowNumber])
                .ToList();
        }
    }
}
