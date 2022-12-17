using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class SudokuRulesetColumns : SudokuRulesetComposite
    {
        public SudokuRulesetColumns(Sudoku sudoku)
            : base(sudoku, GetRulesetsColumns(sudoku))
        { }

        public override SudokuRulesetColumns Clone(Sudoku sudoku)
        {
            return new SudokuRulesetColumns(sudoku);
        }

        private static List<SudokuRuleset> GetRulesetsColumns(Sudoku sudoku)
        {
            List<SudokuRuleset> sudokuRulesets = new();
            for (int column = 0; column < sudoku.Width; column++)
            {
                sudokuRulesets.Add(new SudokuRulesetCellsUnique(sudoku, GetColumn(sudoku, column)));
            }

            return sudokuRulesets;
        }

        private static List<SudokuCell> GetColumn(Sudoku sudoku, int columnNumber)
        {
            return Enumerable.Range(0, sudoku.SudokuGrid.GetLength(1))
                .Select(x => sudoku.SudokuGrid[columnNumber, x])
                .ToList();
        }
    }
}
