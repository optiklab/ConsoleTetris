using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTetris.Display
{
    public class ConsoleScreen
    {
        private StringBuilder _renderBuffer;

        public void Init(int width, int height)
        {
            // Avoid blinking by optimizing the buffering space in console
            // https://stackoverflow.com/questions/28490246/console-clear-blinking
            Console.SetWindowSize(2 * width, 2 * height);
            Console.SetBufferSize(2 * width + 1, 2 * height + 1);

            _renderBuffer = new StringBuilder(width);
        }

        public void Draw(Scene scene)
        {
            Console.Clear();

            scene.Draw();

            // TODO: Colors Console.ForegroundColor = ConsoleColor.Blue;

            FrameBuffer frameToDraw = scene.GetCurrentFrame();

            PutFrameOnScreen(scene, frameToDraw);
        }

        public void DrawBye()
        {
            Console.Clear();
            Console.WriteLine("BYE! THANK YOU!");
        }

        public void DrawDebug(string text)
        {
            Console.WriteLine(text);
        }

        private void PutFrameOnScreen(Scene scene, FrameBuffer frameToDraw)
        {
            List<string> renderPipeline = new List<string>();
            int tempW = 0;
            for (int i = 0; i < frameToDraw.Pixels.Length; i++)
            {
                if (tempW == frameToDraw.Width)
                {
                    tempW = 0;
                    renderPipeline.Add(_renderBuffer.ToString());
                    _renderBuffer = _renderBuffer.Clear();
                }

                _renderBuffer.Append(frameToDraw.Pixels[i].Value);

                ++tempW;
            }

            renderPipeline.Add(_renderBuffer.ToString());
            _renderBuffer = _renderBuffer.Clear();

            // Draw
            for (int i = 0; i < renderPipeline.Count; i++)
            {
                Console.WriteLine(renderPipeline[i]);
            }
            Console.WriteLine("Points earned: " + frameToDraw.PointsEarned.ToString());
        }
    }
}
