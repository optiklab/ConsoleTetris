using ConsoleTetris.Display;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConsoleTetris
{
    public class Scene
    {
        private const int BorderWidth = 2;

        private readonly int _width;
        private readonly int _height;

        private readonly Random _rnd;
        private Field _gameField;
        private Ground _ground;
        private ActiveFigure _figure;

        private int _pointsEarned;

        private FrameBuffer _currentFrameBuffer;
        // private Cell[] _nextFrame; // TODO: 2nd buffer

        private static readonly IList<string> Phrases = new ReadOnlyCollection<string>
        (new[] {
            "WOW! NICE!",
            "DOING GOOD!",
            "YOU ARE THE BEST!"
        });

        private List<int> _levelsToKill;

        public Scene(int width, int height)
        {
            _height = height;
            _width = width;
            _pointsEarned = 0;

            _gameField = new Field(width, height);
            _ground = new Ground(width, height);
            _figure = new ActiveFigure(width, height);
            _rnd = new Random();

            _levelsToKill = new List<int>();
        }

        public void Draw()
        {
            // TODO: 2nd buffer

            _currentFrameBuffer = new FrameBuffer
            {
                Pixels = CalculateRawCells(_ground.Cells, _gameField.Cells, _figure),
                Width = _width,
                Height = _height,
                PointsEarned = _pointsEarned
            };
        }

        public FrameBuffer GetCurrentFrame()
        {
            return _currentFrameBuffer;
        }

        public bool Calculate()
        {
            bool result = false;

            if (KillLevels(_ground, _levelsToKill)) // If we just burned levels
            {
                _pointsEarned += _levelsToKill.Count * 100;
                _levelsToKill.Clear(); // Reset so we don't keep burning
                                       // After burning, do NOT return early; continue with normal game logic
            }

            _levelsToKill = DetectAndMarkLevelsToKill(_ground); // Mark levels that need to be killed...

            if (!_levelsToKill.Any()) // ...if would found so, world will be on pause for 1 iteration.
            {
                if (IsTouchdown(_figure, _ground))
                {
                    result = true;

                    // Copy figure to ground:
                    // only non-empty cells of the figure to do not override non-empty ground cells.
                    _ground.Cells.AddRange(_figure.Cells.Where(c => c.Value != " "));
                    _figure.ReInit();

                    // Game over check after spawning new figure
                    if (IsCollisionOnSpawn(_figure, _ground))
                    {
                        DrawGameOver();
                        // Optionally: throw new Exception("Game Over");
                        // Or set a game over flag
                    }
                }
                else
                {
                    // Move current figure
                    _figure.MoveNext();
                }
            }

            return result;
        }

        private bool IsCollisionOnSpawn(ActiveFigure figure, Ground ground)
        {
            foreach (var figureCell in figure.Cells)
            {
                foreach (var groundCell in ground.Cells)
                {
                    if (figureCell.Value != " " &&
                        groundCell.Value != " " &&
                        groundCell.X == figureCell.X &&
                        groundCell.Y == figureCell.Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void DrawGameOver()
        {
            Console.Clear();
            Console.WriteLine("GAME OVER!");
            // Optionally: Environment.Exit(0);
        }

        public void HandleDown()
        {
            if (_figure.BottomY > 4 && CanMoveDown(_figure, _ground, 2))
                _figure.MoveNext(2);
        }

        public void HandleLeft(int commandMultiplier)
        {
            if (_figure.BottomX > commandMultiplier && CanMoveLeft(_figure, _ground, commandMultiplier))
                _figure.MoveLeft(commandMultiplier);
        }

        public void HandleRight(int commandMultiplier)
        {
            if (_figure.BottomX + _figure.ShapeWidth < _width - 1 && CanMoveRight(_figure, _ground, commandMultiplier))
                _figure.MoveRight(
                    (_figure.BottomX + _figure.ShapeWidth + commandMultiplier >= _width - 1) ?
                        1 : commandMultiplier);
        }

        public void HandleRotate()
        {
            int width = _figure.ShapeWidthAfterTurn();
            if (width + _figure.BottomX <= _width - 1) // Is in screen limits still?
            {
                _figure.Turn();
            }
        }

        private bool KillLevels(Ground ground, List<int> levelsToKill)
        {
            if (levelsToKill.Any())
            {
                levelsToKill.Sort();
                int counter = 0;

                for (int i = 0; i < levelsToKill.Count; i++)
                {
                    for (int j = 0; j < ground.Cells.Count; j++)
                    {
                        if (ground.Cells[j].Y > levelsToKill[i] - counter)
                        {
                            Cell cell = ground.Cells[j];
                            cell.Y -= 1;
                            ground.Cells[j] = cell;
                        }
                    }

                    ++counter;
                }

                ground.Cells = ground.Cells.Where(c => c.Value != "O").ToList();

                int index = _rnd.Next(0, Phrases.Count - 1);
                Console.Clear();
                Console.WriteLine(Phrases[index]);

                return true;
            }
            return false;
        }

        private List<int> DetectAndMarkLevelsToKill(Ground ground)
        {
            // Find all levels to deminish
            var groups = ground.Cells
                .Where(c => c.Value != " ") // Non-empty
                .GroupBy(c => c.Y);
            List<int> levelsToKill = new List<int>();
            foreach (var group in groups)
            {
                if (group.Count() >= _width - BorderWidth) // magic number is 2 frames, left and right
                {
                    levelsToKill.Add(group.Key); //Y
                }
            }

            // Mark as bombed
            foreach (int level in levelsToKill)
            {
                for (int i = 0; i < ground.Cells.Count; i++)
                {
                    if (ground.Cells[i].Y == level)
                    {
                        Cell cell = ground.Cells[i];
                        cell.Value = "O";
                        ground.Cells[i] = cell;
                    }
                }
            }

            return levelsToKill;
        }

        private bool IsTouchdown(ActiveFigure figure, Ground ground)
        {
            foreach (Cell figureCell in figure.Cells)
            {
                foreach (Cell groundCell in ground.Cells)
                {
                    if (figureCell.Value != " " &&
                        groundCell.Value != " " &&
                        groundCell.X == figureCell.X &&
                        groundCell.Y + 1 == figureCell.Y)
                    {
                        return true;
                    }
                }
            }

            if (figure.BottomY <= 0)
            {
                return true;
            }

            return false;
        }

        private Cell[] CalculateRawCells(List<Cell> groundCells, List<Cell> gameFieldCells, ActiveFigure figure)
        {
            // Form a frame buffer by projecting Ground and Active Figure to Game Field.
            Cell[] frame = new Cell[_width * _height];
            gameFieldCells.CopyTo(frame);

            List<Cell> toMerge = new List<Cell>(groundCells);
            toMerge.AddRange(figure.Cells);

            // TODO Replace with matrix
            for (int i = 0; i < toMerge.Count; i++)
            {
                Cell cell = toMerge[i];

                for (int j = 0; j < frame.Length; j++)
                {
                    if (frame[j].X == cell.X && frame[j].Y == cell.Y)
                    {
                        // A-la z-buffering:
                        string oldValue = frame[j].Value;
                        frame[j] = cell;

                        if (cell.Value == " ") // Do not replace existing filled cell with empty on by mistake.
                        {
                            frame[j].Value = oldValue;
                        }
                        break;
                    }
                }
            }

            Array.Sort(frame);

            return frame;
        }

        private bool CanMoveLeft(ActiveFigure figure, Ground ground, int step)
        {
            foreach (Cell figureCell in figure.Cells)
            {
                foreach (Cell groundCell in ground.Cells)
                {
                    if (figureCell.Value != " " &&
                        groundCell.Value != " " &&
                        groundCell.Y == figureCell.Y &&
                        figureCell.X - step == groundCell.X)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CanMoveRight(ActiveFigure figure, Ground ground, int step)
        {
            foreach (Cell figureCell in figure.Cells)
            {
                foreach (Cell groundCell in ground.Cells)
                {
                    if (figureCell.Value != " " &&
                        groundCell.Value != " " &&
                        groundCell.Y == figureCell.Y &&
                        figureCell.X + step == groundCell.X)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CanMoveDown(ActiveFigure figure, Ground ground, int step)
        {
            foreach (Cell figureCell in figure.Cells)
            {
                foreach (Cell groundCell in ground.Cells)
                {
                    if (figureCell.Value != " " &&
                        groundCell.Value != " " &&
                        groundCell.X == figureCell.X &&
                        groundCell.Y + step == figureCell.Y)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
