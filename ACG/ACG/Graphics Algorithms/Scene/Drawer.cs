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
        private byte[] backBuffer;
        private float[] depthBuffer;
        private ExtendedBitmap bmp;
        private int renderWidth;
        private int renderHeight;
        
        public Drawer() {}

        public void Draw(ExtendedBitmap bitmap, Model model, List<Vector4> vertices)
        {
            bmp = bitmap;
            renderWidth = bmp.Width;
            renderHeight = bmp.Height;
            depthBuffer = new float[renderWidth * renderHeight];
            ClearDepthBuffer();
            
            Rasterize(model.Mesh.Polygons, vertices);
        }

        private void DrawMesh(List<List<int>> polygons, List<Vector4> vertices)
        {
            bmp.LockBits();
            Parallel.ForEach(polygons, p =>
            {
                for (var i = 0; i < p.Count - 1; i++)
                {
                    DrawLine(vertices[p[i]], vertices[p[i + 1]], Color.DarkOliveGreen);
                }

                if (p.Count > 0)
                    DrawLine(vertices[p.First()], vertices[p.Last()], Color.DarkOliveGreen);
            });
            bmp.UnlockBits();
        }
        
        private void Rasterize(List<List<int>> polygons, List<Vector4> vertices)
        {
            bmp.LockBits();
            Parallel.ForEach(polygons, p =>
            {
                if (p.Count == 3)
                {
                    var p1 = vertices[p[0]];
                    var p2 = vertices[p[1]];
                    var p3 = vertices[p[2]];
                    DrawTriangle(
                        new Vector3(p1.X, p1.Y, p1.Z),
                        new Vector3(p2.X, p2.Y, p2.Z),
                        new Vector3(p3.X, p3.Y, p3.Z),
                        Color.DarkOliveGreen);   
                }
            });
            bmp.UnlockBits();
        }
        

        private void DrawLine(Vector4 p1, Vector4 p2, Color color)
        {
            var line = GetBresenhamLine(p1, p2);
            foreach (var pt in line)
                bmp[(int)pt.X, (int)pt.Y] = Color.FromArgb(255, color);
        }

        private void ClearDepthBuffer()
        {
            for (var i = 0; i < depthBuffer.Length; i++)
                depthBuffer[i] = float.MaxValue;
        }

        public void DrawPoint(Vector3 point, Color color)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < bmp.Width && point.Y < bmp.Height)
            {
                PutPixel((int)point.X, (int)point.Y, point.Z, color);
            }
        }

        private void PutPixel(int x, int y, float z, Color color)
        {
            var index = (x + y * renderWidth);

            if (depthBuffer[index] < z)
            {
                return;
            }

            depthBuffer[index] = z;
            bmp[x, y] = Color.FromArgb(255, color);
        }
        
        // Clamping values to keep them between 0 and 1
        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }
        
        float Interpolate(float start, float end, float gradient)
        {
            return start + (end - start) * Clamp(gradient);
        }
        
        // drawing line between 2 points from left to right
        // papb -> pcpd
        // pa, pb, pc, pd must then be sorted before
        void ProcessScanLine(int y, Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pd, Color color)
        {
            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var leftGradient = pa.Y != pb.Y ? (y - pa.Y) / (pb.Y - pa.Y) : 1;
            var rightGradient = pc.Y != pd.Y ? (y - pc.Y) / (pd.Y - pc.Y) : 1;

            var leftX = (int)Interpolate(pa.X, pb.X, leftGradient);
            var rightX = (int)Interpolate(pc.X, pd.X, rightGradient);

            var z1 = Interpolate(pa.Z, pb.Z, leftGradient);
            var z2 = Interpolate(pc.Z, pd.Z, rightGradient);
            
            for (var x = leftX; x < rightX; x++)
            {
                var gradientZ = (x - leftX) / (float)(rightX - leftX);

                var z = Interpolate(z1, z2, gradientZ);
                
                DrawPoint(new Vector3(x, y, z), color);
            }
        }
        
        public void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Y > p2.Y) (p2, p1) = (p1, p2);

            if (p2.Y > p3.Y) (p2, p3) = (p3, p2);

            if (p1.Y > p2.Y) (p2, p1) = (p1, p2);

            // inverse slopes
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing inverse slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            // First case where triangles are like that:
            // P1
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P3
            if (dP1P2 > dP1P3)
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        ProcessScanLine(y, p1, p3, p1, p2, color);
                    }
                    else
                    {
                        ProcessScanLine(y, p1, p3, p2, p3, color);
                    }
                }
            }
            // First case where triangles are like that:
            //       P1
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P3
            else
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        ProcessScanLine(y, p1, p2, p1, p3, color);
                    }
                    else
                    {
                        ProcessScanLine(y, p2, p3, p1, p3, color);
                    }
                }
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