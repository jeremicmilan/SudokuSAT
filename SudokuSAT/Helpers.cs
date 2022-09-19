using System.Windows.Input;

namespace SudokuSAT
{
    public class Helpers
    {
        public static int? KeyToValue(Key key)
        {
            int? value = null;

            switch (key)
            {
                case Key.D1:
                case Key.NumPad1:
                    value = 1;
                    break;

                case Key.D2:
                case Key.NumPad2:
                    value = 2;
                    break;

                case Key.D3:
                case Key.NumPad3:
                    value = 3;
                    break;

                case Key.D4:
                case Key.NumPad4:
                    value = 4;
                    break;

                case Key.D5:
                case Key.NumPad5:
                    value = 5;
                    break;

                case Key.D6:
                case Key.NumPad6:
                    value = 6;
                    break;

                case Key.D7:
                case Key.NumPad7:
                    value = 7;
                    break;

                case Key.D8:
                case Key.NumPad8:
                    value = 8;
                    break;

                case Key.D9:
                case Key.NumPad9:
                    value = 9;
                    break;
            }

            return value;
        }
    }
}
