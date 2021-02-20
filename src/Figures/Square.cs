using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConsoleTetris.Figures
{
    public struct Square
    {
        public const int ShapeWidth = 2;
        public const FigureType Type = FigureType.Square;
        public static readonly IList<Cell> Cells = new ReadOnlyCollection<Cell>
            (new[] {
            // As you can see, figure shape contains both filled and empty cells.
            // We have to handle this during collision detection.
                new Cell (0, 0, "*"),
                new Cell (0, 1, "*"),
                new Cell (1, 0, "*"),
                new Cell (1, 1, "*")
            });
    }
}
