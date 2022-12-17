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
    public class SudokuRulesetSudoku : SudokuRulesetComposite
    {
        public SudokuRulesetSudoku(Sudoku sudoku)
            : base(sudoku, new List<SudokuRuleset>
            {
                new SudokuRulesetRows(sudoku),
                new SudokuRulesetColumns(sudoku),
                new SudokuRulesetBoxes(sudoku),
            })
        { }

        public override SudokuRulesetSudoku Clone(Sudoku sudoku)
        {
            return new SudokuRulesetSudoku(sudoku);
        }
    }
}
