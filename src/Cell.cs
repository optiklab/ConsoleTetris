using System;

namespace ConsoleTetris
{
    public struct Cell : IComparable<Cell>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Value { get; set; }
        //public ConsoleColor Color { get; set; }

        public Cell(int x, int y, string value)
        {
            Value = value;
            X = x;
            Y = y;
            //Color = 0;
        }

        public static int Compare(int a, int b)
        {
            if (a > b)
            {
                return 1;
            }
            else if (a < b)
            {
                return -1;
            }

            return 0;
        }

        public int CompareTo(Cell other)
        {
            if (Y > other.Y)
            {
                return -1;
            }
            if (Y < other.Y)
            {
                return 1;
            }
            return Helpers.Compare(X, other.X);
        }
    }
}
