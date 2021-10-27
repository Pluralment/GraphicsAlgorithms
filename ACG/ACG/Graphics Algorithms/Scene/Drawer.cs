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
            DrawModel(bmp, model.Mesh.Polygons, vertices);
            //Rasterize(bmp, model.Mesh.Polygons, vertices);
        }

        private void DrawModel(ExtendedBitmap bmp, List<List<int>> polygons, List<Vector4> vertices)
        {
            bmp.LockBits();
            Parallel.ForEach(polygons, p =>
            {
                for (var i = 0; i < p.Count - 1; i++)
                {
                    DrawLine(bmp, vertices[p[i]], vertices[p[i + 1]], Color.DarkOliveGreen);
                }

                if (p.Count > 0)
                    DrawLine(bmp, vertices[p.First()], vertices[p.Last()], Color.DarkOliveGreen);
                
                if (p.Count == 3)
                    FillTriangle(bmp, vertices[p[0]], vertices[p[1]], vertices[p[2]], Color.DarkOliveGreen);
            });
            bmp.UnlockBits();
        }
        
        private void Rasterize(ExtendedBitmap bmp, List<List<int>> polygons, List<Vector4> vertices)
        {
            bmp.LockBits();
            Parallel.ForEach(polygons, p =>
            {
                FillTriangle(bmp, vertices[p[0]], vertices[p[1]], vertices[p[2]], Color.DarkOliveGreen);
            });
            bmp.UnlockBits();
        }

        private void DrawLine(ExtendedBitmap bmp, Vector4 p1, Vector4 p2, Color color)
        {
            var line = GetBresenhamLine(p1, p2);
            foreach (var pt in line)
                bmp[(int)pt.X, (int)pt.Y] = Color.FromArgb(255, color);
        }

        private void FillTriangle(ExtendedBitmap bmp, Vector4 v1, Vector4 v2, Vector4 v3, Color color)
        {
            // sort three vertices to guarantee v1.Y > v2.Y > v3.Y
            if (v1.Y > v2.Y && v2.Y > v3.Y) {}
            else if (v1.Y > v3.Y && v3.Y > v2.Y) (v2, v3) = (v3, v2);
            else if (v3.Y > v1.Y && v1.Y > v2.Y) (v1, v2, v3) = (v3, v1, v2);
            else if (v2.Y > v1.Y && v1.Y > v3.Y) (v1, v2) = (v2, v1);
            else if (v2.Y > v3.Y && v3.Y > v1.Y) (v1, v2, v3) = (v2, v3, v1);
            else if (v3.Y > v2.Y && v2.Y > v1.Y) (v1, v3) = (v3, v1);
            if (Math.Abs(v2.Y - v3.Y) < 0.1f)
            {
                FillTriangleBottom(bmp, v1, v2, v3, color);
                return;
            }
            if (Math.Abs(v1.Y - v2.Y) < 0.1f)
            {
                FillTriangleTop(bmp, v1, v2, v3, color);
                return;
            }

            var v4 = new Vector4(v1.X + ((v2.Y - v1.Y) / (v3.Y - v1.Y)) * (v3.X - v1.X), v2.Y, 0, 0);
            FillTriangleTop(bmp, v2, v4, v1, color);
            FillTriangleBottom(bmp, v3, v4, v2, color);
        }

        private void FillTriangleBottom(ExtendedBitmap bmp, Vector4 v1, Vector4 v2, Vector4 v3, Color color)
        {
            var invsploe1 = (v2.X - v1.X) / (v2.Y - v1.Y);
            var invsploe2 = (v3.X - v1.X) / (v3.Y - v1.Y);
            var curx1 = v1.X;
            var curx2 = v1.X;

            for (var scanlineY = v1.Y; scanlineY <= v2.Y; scanlineY++)
            {
                DrawLine(
                    bmp, 
                    new Vector4(curx1, scanlineY, 0, 0),
                    new Vector4(curx2, scanlineY, 0, 0),
                    color
                    );
                curx1 += invsploe1;
                curx2 += invsploe2;
            }
        }
        
        private void FillTriangleTop(ExtendedBitmap bmp, Vector4 v1, Vector4 v2, Vector4 v3, Color color)
        {
            var invsploe1 = (v3.X - v1.X) / (v3.Y - v1.Y);
            var invsploe2 = (v3.X - v2.X) / (v3.Y - v2.Y);
            var curx1 = v3.X;
            var curx2 = v3.X;
            
            for (var scanlineY = v3.Y; scanlineY > v1.Y; scanlineY--)
            {
                DrawLine(
                    bmp, 
                    new Vector4(curx1, scanlineY, 0, 0),
                    new Vector4(curx2, scanlineY, 0, 0),
                    color
                );
                curx1 -= invsploe1;
                curx2 -= invsploe2;
            }
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