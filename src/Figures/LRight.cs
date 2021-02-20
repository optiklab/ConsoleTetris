using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConsoleTetris.Figures
{
    public struct LRight
    {
        public const int ShapeWidth1 = 3;
        public const int ShapeWidth2 = 2;
        public const int ShapeWidth3 = 3;
        public const int ShapeWidth4 = 2;
        public const FigureType Type = FigureType.LRight;
        public static readonly IList<Cell> Cells1 = new ReadOnlyCollection<Cell>
        (new[] {
            // As you can see, figure shape contains both filled and empty cells.
            // We have to handle this during collision detection.
            new Cell (0, 0, "*"),
            new Cell (1, 0, " "),
            new Cell (2, 0, " "),
            new Cell (0, 1, "*"),
            new Cell (1, 1, "*"),
            new Cell (2, 1, "*")
        });
        public static readonly IList<Cell> Cells2 = new ReadOnlyCollection<Cell>
        (new[] {
            new Cell (0, 0, " "),
            new Cell (1, 0, "*"),
            new Cell (0, 1, " "),
            new Cell (1, 1, "*"),
            new Cell (0, 2, "*"),
            new Cell (1, 2, "*")
        });
        public static readonly IList<Cell> Cells3 = new ReadOnlyCollection<Cell>
        (new[] {
            new Cell (0, 0, "*"),
            new Cell (1, 0, "*"),
            new Cell (2, 0, "*"),
            new Cell (0, 1, " "),
            new Cell (1, 1, " "),
            new Cell (2, 1, "*")
        });
        public static readonly IList<Cell> Cells4 = new ReadOnlyCollection<Cell>
        (new[] {
            new Cell (0, 0, "*"),
            new Cell (1, 0, "*"),
            new Cell (0, 1, "*"),
            new Cell (1, 1, " "),
            new Cell (0, 2, "*"),
            new Cell (1, 2, " ")
        });
    }
}