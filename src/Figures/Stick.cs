using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConsoleTetris.Figures
{
    public struct Stick
    {
        public const int ShapeWidth1 = 1;
        public const int ShapeWidth2 = 4;
        public const FigureType Type = FigureType.Stick;
        public static readonly IList<Cell> Cells1 = new ReadOnlyCollection<Cell>
        (new[] {
            // As you can see, figure shape contains both filled and empty cells.
            // We have to handle this during collision detection.
            new Cell (0, 0, "*"),
            new Cell (0, 1, "*"),
            new Cell (0, 2, "*"),
            new Cell (0, 3, "*")
        });
        public static readonly IList<Cell> Cells2 = new ReadOnlyCollection<Cell>
        (new[] {
            new Cell (0, 0, "*"),
            new Cell (1, 0, "*"),
            new Cell (2, 0, "*"),
            new Cell (3, 0, "*")
        });
    }
}
