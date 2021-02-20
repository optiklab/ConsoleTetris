using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTetris
{
    public class GameLoop
    {
        private int _height;
        private int _width;
        private int _pointsEarned;
        private readonly Random _rnd;

        public static readonly IList<string> Phrases = new ReadOnlyCollection<string>
        (new[] {
            "WOW! NICE!",
            "DOING GOOD!",
            "YOU ARE THE BEST!"
        });
        private static int _commandMultiplier;
        private static Queue<Commands> _commands;
        private static Object _sync = new Object();

        private StringBuilder _renderBuffer;
        private StringBuilder _commandsToLog;

        public GameLoop(int width, int height)
        {
            _height = height;
            _width = width;
            _pointsEarned = 0;
            _commandMultiplier = 1;
            _rnd = new Random();
        }

        public void Run()
        {
            _pointsEarned = 0;
            _commands = new Queue<Commands>();
            _commandMultiplier = 1;

            Field gameField = new Field(_width, _height);
            Ground ground = new Ground(_width, _height);
            ActiveFigure figure = new ActiveFigure(_width, _height);

            // Create separate thread to listen key inputs from user
            var inputsThread = new Thread(ListenKeys);
            inputsThread.Start();

            // TODO Handle closing event

            _renderBuffer = new StringBuilder(_width);
            _commandsToLog = new StringBuilder();

            List<int> levelsToKill = new List<int>();

            while (true)
            {
                Console.Clear();

                if (!KillLevels(ground, levelsToKill)) // Kill levels marked during last iteration.
                {
                    levelsToKill = DetectAndMarkLevelsToKill(ground); // Mark levels that need to be killed...

                    if (!levelsToKill.Any()) // ...if would found so, world will be on pause for 1 iteration.
                    {
                        if (IsTouchdown(figure, ground))
                        {
                            lock (_sync)
                            {
                                _commands.Clear(); // Forget all previous commands that were not executed.
                                _commandsToLog.Clear();
                                _commandMultiplier = 1;
                            }

                            // Copy figure to ground:
                            // only non-empty cells of the figure to do not override non-empty ground cells.
                            ground.Cells.AddRange(figure.Cells.Where(c => c.Value != " "));
                            figure.ReInit();
                        }
                        else
                        {
                            // Move current figure
                            figure.MoveNext();
                        }
                    }
                }
                else
                {
                    _pointsEarned += levelsToKill.Count * 100;
                }

                Cell[] frame = CreateFrame(ground.Cells, gameField.Cells, figure);

                // TODO: 2nd buffer

                // TODO: Colors Console.ForegroundColor = ConsoleColor.Blue;

                RenderFrame(frame, figure);

                if (!HandleUserInput(figure, ground))
                {
                    break;
                }

                Thread.Sleep(400);
            }
        }

        private static void ListenKeys()
        {
            Commands lastCommand = Commands.None;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo ki = Console.ReadKey();

                    Commands command = Commands.None;
                    switch (ki.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            command = Commands.Left;
                            break;
                        case ConsoleKey.RightArrow:
                            command = Commands.Right;
                            break;
                        case ConsoleKey.DownArrow:
                            command = Commands.Down;
                            break;
                        case ConsoleKey.Spacebar:
                            command = Commands.Rotate;
                            break;
                        case ConsoleKey.P:
                            command = Commands.Pause;
                            break;
                        case ConsoleKey.R:
                            command = Commands.Reset;
                            break;
                        case ConsoleKey.E:
                            command = Commands.Exit;
                            break;
                        default:
                            // Do nothing;
                            command = Commands.None;
                            break;
                    }

                    if (command != Commands.None)
                    {
                        lock (_sync)
                        {
                            if (_commands.Count > 2 && lastCommand == command)
                            {
                                while (_commands.Count > 1) // Forget all commands from queue. Use waited long.
                                {
                                    _commands.Dequeue();
                                }
                                _commandMultiplier = 3;

                                lastCommand = Commands.None;
                            }
                            else
                            {
                                _commands.Enqueue(command); // TODO
                                lastCommand = command;
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
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
                if (group.Count() >= _width - 2) // magic number is 2 frames, left and right
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

        private bool HandleUserInput(ActiveFigure figure, Ground ground)
        {
            if (_commands.Any())
            {
                Commands command;
                _commandsToLog.Clear();
                lock (_sync)
                {
                    command = _commands.Dequeue();
                    _commandsToLog.Append(string.Join(',', _commands.ToList()));
                }
                switch (command)
                {
                    case Commands.Down:
                        if (figure.BottomY > 4 && CanMoveDown(figure, ground, 2))
                            figure.MoveNext(2);
                        break;
                    case Commands.Left:
                        if (figure.BottomX > _commandMultiplier && CanMoveLeft(figure, ground, _commandMultiplier))
                            figure.MoveLeft(_commandMultiplier);
                        break;
                    case Commands.Right:
                        if (figure.BottomX + figure.ShapeWidth < _width - 1 && CanMoveRight(figure, ground, _commandMultiplier))
                            figure.MoveRight(
                                (figure.BottomX + figure.ShapeWidth + _commandMultiplier >= _width - 1) ?
                                    1 : _commandMultiplier);
                        break;
                    case Commands.Rotate:
                        int width = figure.ShapeWidthAfterTurn();
                        if (width + figure.BottomX <= _width - 1) // Is in screen limits still?
                        {
                            figure.Turn();
                        }
                        break;
                    case Commands.Pause:
                        //??
                        break;
                    case Commands.Reset:
                        Run();
                        break;
                    case Commands.Exit:
                    default:
                        Console.Clear();
                        Console.WriteLine("BYE! THANK YOU!");
                        return false;
                }
            }
            _commandMultiplier = 1;
            return true;
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

        private Cell[] CreateFrame(List<Cell> groundCells, List<Cell> gameFieldCells, ActiveFigure figure)
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

        private void RenderFrame(Cell[] frameCells, ActiveFigure figure)
        {
            List<string> renderPipeline = new List<string>();
            int tempW = 0;
            for (int i = 0; i < frameCells.Length; i++)
            {
                if (tempW == _width)
                {
                    tempW = 0;
                    renderPipeline.Add(_renderBuffer.ToString());
                    _renderBuffer = _renderBuffer.Clear();
                }

                _renderBuffer.Append(frameCells[i].Value);

                ++tempW;
            }

            renderPipeline.Add(_renderBuffer.ToString());
            _renderBuffer = _renderBuffer.Clear();

            // Draw
            for (int i = 0; i < renderPipeline.Count; i++)
            {
                Console.WriteLine(renderPipeline[i]);
            }

            Console.WriteLine("Points earned: " + _pointsEarned.ToString());
            Console.WriteLine("Commands queued: " + _commandsToLog.ToString());
            Console.WriteLine("Multiplier: " + _commandMultiplier.ToString());
        }
    }
}
