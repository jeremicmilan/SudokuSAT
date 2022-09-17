using Google.OrTools.ConstraintSolver;
using Google.OrTools.Sat;
using System;

namespace SudokuSAT
{
    public class SolutionCounter : CpSolverSolutionCallback
    {
        public const int MaxSolutionCount = 100;

        SudokuSolver SudokuSolver { get; set; }
        CpSolver Solver { get; set; }

        private int SolutionCount { get; set; }

        public SolutionCounter(SudokuSolver sudokuSolver, CpSolver solver)
        {
            SudokuSolver = sudokuSolver;
            Solver = solver;
        }

        public override void OnSolutionCallback()
        {
            SolutionCount++;
            SudokuSolver.Window.solutionCount.Content = SolutionCount;

            if (SolutionCount > MaxSolutionCount)
            {
                SudokuSolver.Window.solutionCount.Content = MaxSolutionCount + "+";
                Solver.StopSearch();
            }
        }
    }
}
