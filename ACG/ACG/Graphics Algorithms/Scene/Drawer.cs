using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;
using GraphicsModeler.Helper;

namespace GraphicsModeler.Scene
{
    public class Drawer
    {
        public Drawer() {}

        public void Draw(ExtendedBitmap bmp, Model model, List<Vector4> vertices)
        {
            bmp.LockBits();
            Parallel.ForEach(model.Mesh.Polygons, p =>
            {
                for (var i = 0; i < p.Count - 1; i++)
                {
                    var line = GetBresenhamLine(vertices[p[i]], vertices[p[i + 1]]);
                    foreach (var pt in line)
                        bmp[(int)pt.X, (int)pt.Y] = Color.FromArgb(255, Color.Firebrick);
                }

                if (p.Count > 0)
                    foreach (var pt in GetBresenhamLine(vertices[p.First()], vertices[p.Last()]))
                        bmp[(int)pt.X, (int)pt.Y] = Color.FromArgb(255, Color.Firebrick);
            });
            bmp.UnlockBits();
        }


        private IEnumerable<Vector4> GetDDALine(Vector4 vector1, Vector4 vector2)
        {
            var dx = vector2.X - vector1.X;
            var dy = vector2.Y - vector1.Y;
            var L = Math.Max(Math.Abs(dx), Math.Abs(dy));
            var xInc = dx / L;
            var yInc = dy / L;
            var x = vector1.X;
            var y = vector1.Y;

            for (int i = 0; i <= L; i++)
            {
                yield return new Vector4(x, y, 0, 1);
                x += xInc;
                y += yInc;
            }
        }

        private IEnumerable<Vector4> GetBresenhamLine(Vector4 p1, Vector4 p2)
        {
            if (Math.Abs(p2.Y - p1.Y) < Math.Abs(p2.X - p1.X))
            {
                if (p1.X > p2.X)
                {
                    return GetBresenhamLineLow(p2.X, p2.Y, p1.X, p1.Y);
                }
                else
                {
                    return GetBresenhamLineLow(p1.X, p1.Y, p2.X, p2.Y);
                }
            }
            else
            {
                if (p1.Y > p2.Y) 
                {
                    return GetBresenhamLineHigh(p2.X, p2.Y, p1.X, p1.Y);
                } 
                else 
                {
                    return GetBresenhamLineHigh(p1.X, p1.Y, p2.X, p2.Y);
                }

            }
        }
        
        private IEnumerable<Vector4> GetBresenhamLineHigh(float x0, float y0, float x1, float y1)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }

            var d = 2 * dx - dy;
            var x = x0;
            for (var y = y0; y <= y1; y++) 
            {
                yield return new Vector4(x, y, 0, 1);
                if (d > 0)
                {
                    x += xi;
                    d -= 2 * dy;
                }

                d += 2 * dx;
            }
        }
        
        private IEnumerable<Vector4> GetBresenhamLineLow(float x0, float y0, float x1, float y1)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }

            var d = 2 * dy - dx;
            var y = y0;
            for (var x = x0; x <= x1; x++) 
            {
                yield return new Vector4(x, y, 0, 1);
                if (d > 0)
                {
                    y += yi;
                    d -= 2 * dx;
                }

                d += 2 * dy;
            }
        }
    }
}