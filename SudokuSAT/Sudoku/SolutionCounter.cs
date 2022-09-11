using Google.OrTools.ConstraintSolver;
using Google.OrTools.Sat;
using System;

namespace SudokuSAT
{
    public class SolutionCounter : CpSolverSolutionCallback
    {
        public const int MaxSolutionCount = 100;

        Sudoku Sudoku { get; set; }
        CpSolver Solver { get; set; }

        private int SolutionCount { get; set; }

        public SolutionCounter(Sudoku sudoku, CpSolver solver)
        {
            Sudoku = sudoku;
            Solver = solver;
        }

        public override void OnSolutionCallback()
        {
            SolutionCount++;
            Sudoku.Window.solutionCount.Content = SolutionCount;

            if (SolutionCount > MaxSolutionCount)
            {
                Sudoku.Window.solutionCount.Content = MaxSolutionCount + "+";
                Solver.StopSearch();
            }
        }
    }
}
