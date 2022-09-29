using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SudokuSAT
{
    public class Visual <TElement>
        where TElement : class, IVisualizable<TElement>
    {
        public TElement Element { get; private set; }
        public Grid Grid { get; private set; }

        public Visual(TElement element, Grid grid)
        {
            Element = element;
            Grid = grid;
            Element.Visual = this;
        }

        public void Visualize()
        {
            Grid.Children.Add(Element.Visualize());
        }
    }
}
