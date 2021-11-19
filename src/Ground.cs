using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTetris
{
    
    /// <summary>
    /// Just a wrapper for List<Cell> container
    /// </summary>
    public class Ground
    {
        public List<Cell> Cells = new List<Cell>();

        public Ground(int width, int height)
        {
            if (width < 2 || width > 50)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height < 2 || height > 100)
            {
                throw new ArgumentOutOfRangeException("height");
            }
        }

        public void Add(List<Cell> list)
        {
            Cells.AddRange(list);
        }
    }
}
