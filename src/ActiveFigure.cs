using ConsoleTetris.Figures;
using System;
using System.Collections.Generic;

namespace ConsoleTetris
{
    public class ActiveFigure
    {
        private readonly int _fieldWidth;
        private readonly int _fieldHeight;
        private readonly Random _rnd;
        private int _state;

        public FigureType Type { get; set; }
        public int BottomX { get; set; }
        public int BottomY { get; set; }
        public int ShapeWidth { get; set; }
        public int Color { get; set; }
        public List<Cell> Cells { get; set; }

        public ActiveFigure(int fieldWidth, int fieldHeight)
        {
            if (fieldWidth < 2 || fieldWidth > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldWidth));
            }

            if (fieldHeight < 2 || fieldHeight > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(fieldHeight));
            }

            _fieldWidth = fieldWidth;
            _fieldHeight = fieldHeight;

            Cells = new List<Cell>();

            _rnd = new Random();

            ReInit();
        }

        public void ReInit()
        {
            _state = _rnd.Next(0, 3);
            Type = (FigureType)_rnd.Next(0, 5);
            ShapeWidth = GenerateCells();

            BottomX = _rnd.Next(1, _fieldWidth - ShapeWidth);
            BottomY = _fieldHeight;

            Project();
        }

        public void MoveNext(int step = 1)
        {
            BottomY -= step;

            ShapeWidth = GenerateCells();
            Project();
        }

        public void MoveLeft(int step = 1)
        {
            BottomX -= step;

            ShapeWidth = GenerateCells();
            Project();
        }

        public void MoveRight(int step = 1)
        {
            BottomX += step;

            ShapeWidth = GenerateCells();
            Project();
        }

        public int ShapeWidthAfterTurn()
        {
            int state = _state;
            if (state < 3)
                state += 1;
            else
                state = 0;

            switch (Type)
            {
                case FigureType.Square:
                    return Square.ShapeWidth;
                case FigureType.Stick:
                    if (state == 0 || state == 2)
                        return Stick.ShapeWidth1;
                    else
                        return Stick.ShapeWidth2;
                case FigureType.ZLeft:
                    if (state == 0 || state == 2)
                        return ZLeft.ShapeWidth1;
                    else
                        return ZLeft.ShapeWidth2;
                case FigureType.ZRight:
                    if (state == 0 || state == 2)
                        return ZRight.ShapeWidth1;
                    else
                        return ZRight.ShapeWidth2;
                case FigureType.LLeft:
                    if (state == 0 || state == 2)
                        return LLeft.ShapeWidth1;
                    else
                        return LLeft.ShapeWidth2;
                case FigureType.LRight:
                    if (state == 0 || state == 2)
                        return LRight.ShapeWidth1;
                    else
                        return LRight.ShapeWidth2;
                default:
                    throw new ArgumentException("FigureType is not declared!");
            }
        }

        public void Turn() // TurnLeft
        {
            if (_state < 3)
                _state += 1;
            else
                _state = 0;

            ShapeWidth = GenerateCells();
            Project();
        }

        private int GenerateCells()
        {
            Cells.Clear();

            switch (Type)
            {
                case FigureType.Square:
                    Cells.AddRange(Square.Cells);
                    return Square.ShapeWidth;
                case FigureType.Stick:
                    if (_state == 0 || _state == 2)
                    {
                        Cells.AddRange(Stick.Cells1);
                        return Stick.ShapeWidth1;
                    }
                    else
                    {
                        Cells.AddRange(Stick.Cells2);
                        return Stick.ShapeWidth2;
                    }
                case FigureType.ZLeft:
                    if (_state == 0 || _state == 2)
                    {
                        Cells.AddRange(ZLeft.Cells1);
                        return ZLeft.ShapeWidth1;
                    }
                    else
                    {
                        Cells.AddRange(ZLeft.Cells2);
                        return ZLeft.ShapeWidth2;
                    }
                case FigureType.ZRight:
                    if (_state == 0 || _state == 2)
                    {
                        Cells.AddRange(ZRight.Cells1);
                        return ZRight.ShapeWidth1;
                    }
                    else
                    {
                        Cells.AddRange(ZRight.Cells2);
                        return ZRight.ShapeWidth2;
                    }
                case FigureType.LLeft:
                    if (_state == 0)
                    {
                        Cells.AddRange(LLeft.Cells1);
                        return LLeft.ShapeWidth1;
                    }
                    else if (_state == 1)
                    {
                        Cells.AddRange(LLeft.Cells2);
                        return LLeft.ShapeWidth2;
                    }
                    else if (_state == 2)
                    {
                        Cells.AddRange(LLeft.Cells3);
                        return LLeft.ShapeWidth3;
                    }
                    else // if (_state == 3)
                    {
                        Cells.AddRange(LLeft.Cells4);
                        return LLeft.ShapeWidth4;
                    }
                case FigureType.LRight:
                    if (_state == 0)
                    {
                        Cells.AddRange(LRight.Cells1);
                        return LRight.ShapeWidth1;
                    }
                    else if (_state == 1)
                    {
                        Cells.AddRange(LRight.Cells2);
                        return LRight.ShapeWidth2;
                    }
                    else if (_state == 2)
                    {
                        Cells.AddRange(LRight.Cells3);
                        return LRight.ShapeWidth3;
                    }
                    else // if (_state == 3)
                    {
                        Cells.AddRange(LRight.Cells4);
                        return LRight.ShapeWidth4;
                    }
                default:
                    throw new ArgumentException("FigureType is not declared!");
            }
        }

        /// <summary>
        /// Makes a projection
        /// </summary>
        private void Project()
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                var cell = Cells[i];
                cell.X += BottomX;
                cell.Y += BottomY;
                Cells[i] = cell;
            }
        }
    }
}
