using System;

namespace ConsoleTetris
{
    public class Program
    {
        private const int Height = 22;
        private const int Width = 14;

        static void Main(string[] args)
        {
            var game = new GameLoop(Width, Height);

            game.Run();

            Console.WriteLine("Press Ctrl+C to exit!");
        }
    }
}
