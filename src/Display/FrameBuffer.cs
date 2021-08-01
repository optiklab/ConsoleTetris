using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTetris.Display
{
    public struct FrameBuffer
    {
        public int Width;
        public int Height;

        public int PointsEarned;

        public Cell[] Pixels;
    }
}
