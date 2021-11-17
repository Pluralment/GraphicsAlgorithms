using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsModeler.Scene
{
    public class Texture
    {
        private int width;
        private int height;

        public Color[] ColorBuffer { get; private set; }

        public void Load(string file)
        {
            Bitmap bmp = new Bitmap(file);
            width = bmp.Width;
            height = bmp.Height;
            ColorBuffer = new Color[bmp.Width * bmp.Height];

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    ColorBuffer[i * bmp.Width + j] = bmp.GetPixel(j, i);
                }
            }
        }

        public Color Map(float tu, float tv)
        {
            // Image is not loaded yet.
            if (ColorBuffer == null)
            {
                return Color.DarkBlue;
            }

            // Using a % operator to cycle/repeat the texture if needed.
            int u = Math.Abs((int)(tu * width) % width);
            int v = Math.Abs((int)(tv * height) % height);

            return ColorBuffer[v * width + u];
        }
    }
}
