using Google.OrTools.Sat;

namespace SudokuSAT
{
    public class SolutionCounter : CpSolverSolutionCallback
    {
        public const int MaxSolutionCount = 100;

        private SudokuSolver SudokuSolver { get; set; }
        private CpSolver Solver { get; set; }

        private int SolutionCount { get; set; }

        private MainWindow Window => SudokuSolver.Window;

        public SolutionCounter(SudokuSolver sudokuSolver, CpSolver solver)
        {
            SudokuSolver = sudokuSolver;
            Solver = solver;
        }

        public override void OnSolutionCallback()
        {
            SolutionCount++;
            Window.Dispatcher.Invoke(() => Window.SolutionCount.Content = SolutionCount);

            if (SolutionCount > MaxSolutionCount)
            {
                Window.Dispatcher.Invoke(() => Window.SolutionCount.Content = MaxSolutionCount + "+");
                Solver.StopSearch();
            }
        }
    }
}
