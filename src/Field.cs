using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTetris
{
    public class Field
    {
        public List<Cell> Cells;

        public Field(int width, int height)
        {
            if (width < 2 || width > 50)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height < 2 || height > 100)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            Cells = new List<Cell>(width * height);

            for (int h = 0; h < height; h++)
            {
                if (h == 0 || h == height - 1)
                {
                    for (int w = 0; w < width; w++)
                        Cells.Add(new Cell(w, h, "-"));
                }
                else
                {
                    Cells.Add(new Cell(0, h, "|"));
                    for (int w = 1; w < width - 1; w++)
                        Cells.Add(new Cell(w, h, " "));
                    Cells.Add(new Cell(width - 1, h, "|"));
                }
            }
        }
    }
}
