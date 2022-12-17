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
    public class SudokuRulesetBoxes : SudokuRulesetComposite
    {
        public SudokuRulesetBoxes(Sudoku sudoku)
            : base(sudoku, GetRulestBoxes(sudoku))
        { }

        public override SudokuRulesetBoxes Clone(Sudoku sudoku)
        {
            return new SudokuRulesetBoxes(sudoku);
        }

        private static List<SudokuRuleset> GetRulestBoxes(Sudoku sudoku)
        {
            List<SudokuRuleset> sudokuRulesets = new();
            for (int columnBox = 0; columnBox <= sudoku.Width / sudoku.BoxSize; columnBox++)
            {
                for (int rowBox = 0; rowBox <= sudoku.Height / sudoku.BoxSize; rowBox++)
                {
                    List<SudokuCell> boxCells = new();

                    for (int column = columnBox * sudoku.BoxSize;
                        column < Math.Min((columnBox + 1) * sudoku.BoxSize, sudoku.Width);
                        column++)
                    {
                        for (int row = rowBox * sudoku.BoxSize;
                            row < Math.Min((rowBox + 1) * sudoku.BoxSize, sudoku.Height);
                            row++)
                        {
                            boxCells.Add(sudoku.SudokuGrid[column, row]);
                        }
                    }

                    sudokuRulesets.Add(new SudokuRulesetCellsUnique(sudoku, boxCells));
                }
            }

            return sudokuRulesets;
        }
    }
}
