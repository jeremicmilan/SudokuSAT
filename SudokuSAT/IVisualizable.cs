using System.Windows;
using System.Windows.Controls;

namespace SudokuSAT
{
    public interface IVisualizable<TElement>
        where TElement : class, IVisualizable<TElement>
    {
        public abstract UIElement Visualize();

        public Visual<TElement>? Visual { get; set; }
    }
}
