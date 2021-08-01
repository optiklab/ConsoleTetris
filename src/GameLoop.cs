using ConsoleTetris.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleTetris
{
    public class GameLoop
    {
        private int _height;
        private int _width;

        private static int _commandMultiplier;
        private static Queue<Commands> _commands;
        private static Object _sync = new Object();

        private static ConsoleScreen _consoleScreen;

        private StringBuilder _commandsToLog;

        public GameLoop(int width, int height)
        {
            _height = height;
            _width = width;

            _consoleScreen = new ConsoleScreen();
        }

        public void Run()
        {
            _commands = new Queue<Commands>();
            _commandMultiplier = 1;

            _consoleScreen.Init(_width, _height);

            Scene scene = new Scene(_width, _height);

            // Create separate thread to listen key inputs from user
            var inputsThread = new Thread(ListenKeys);
            inputsThread.Start();

            // TODO Handle closing event

            _commandsToLog = new StringBuilder();

            while (true)
            {
                if (scene.Calculate())
                {
                    lock (_sync)
                    {
                        _commands.Clear(); // Forget all previous commands that were not executed.
                        _commandsToLog.Clear();
                        _commandMultiplier = 1;
                    }
                }

                _consoleScreen.Draw(scene);

                _consoleScreen.DrawDebug("Commands queued: " + _commandsToLog);
                _consoleScreen.DrawDebug("Multiplier: " + _commandMultiplier);

                if (!HandleUserInput(scene))
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

        private bool HandleUserInput(Scene scene)
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
                        scene.HandleDown();
                        break;
                    case Commands.Left:
                        scene.HandleLeft(_commandMultiplier);
                        break;
                    case Commands.Right:
                        scene.HandleRight(_commandMultiplier);
                        break;
                    case Commands.Rotate:
                        scene.HandleRotate();
                        break;
                    case Commands.Pause:
                        //??
                        break;
                    case Commands.Reset:
                        Run();
                        break;
                    case Commands.Exit:
                    default:
                        _consoleScreen.DrawBye();
                        return false;
                }
            }
            _commandMultiplier = 1;
            return true;
        }
    }
}
