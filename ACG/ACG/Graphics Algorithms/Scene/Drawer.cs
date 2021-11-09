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
        private Vector3 lightPos;
        private Model _model;
        private Camera _camera;

        public Drawer() {}

        public void Draw(ExtendedBitmap bitmap, Model model, Camera camera)
        {
            InitNewFrame(bitmap, model, camera);
            Rasterize();
        }

        private void InitNewFrame(ExtendedBitmap bitmap, Model model, Camera camera)
        {
            bmp = bitmap;
            _model = model;
            _camera = camera;
            renderWidth = bmp.Width;
            renderHeight = bmp.Height;
            lightPos = new Vector3(2, 2, 2);
            depthBuffer = new float[renderWidth * renderHeight];
            ClearDepthBuffer();
        }

        private void DrawMesh(List<Polygon> polygons, List<Vector4> vertices)
        {
            bmp.LockBits();
            Parallel.ForEach(polygons, p =>
            {
                for (var i = 0; i < p.VerticesIndexes.Count - 1; i++)
                {
                    DrawLine(vertices[p.VerticesIndexes[i]], vertices[p.VerticesIndexes[i + 1]], Color.DarkOliveGreen);
                }

                if (p.VerticesIndexes.Count > 0)
                    DrawLine(vertices[p.VerticesIndexes.First()], vertices[p.VerticesIndexes.Last()], Color.DarkOliveGreen);
            });
            bmp.UnlockBits();
        }
        
        private void Rasterize()
        {
            var polygons = _model.Mesh.Polygons;
            var vertices = _model.Mesh.Vertices;
            
            bmp.LockBits();
            Parallel.ForEach(polygons, p =>
            {
                if (!BackfaceCulling(p)) return;
                ProcessTriangle(p, vertices);
            });
            bmp.UnlockBits();
        }

        private void ProcessTriangle(Polygon p, List<Vector3> vertices)
        {
            if (p.VerticesIndexes.Count == 3)
            {
                var p1 = vertices[p.VerticesIndexes[0]];
                var p2 = vertices[p.VerticesIndexes[1]];
                var p3 = vertices[p.VerticesIndexes[2]];
                DrawTriangle(
                    p,
                    new Vector3(p1.X, p1.Y, p1.Z),
                    new Vector3(p2.X, p2.Y, p2.Z),
                    new Vector3(p3.X, p3.Y, p3.Z),
                    Color.DarkOliveGreen);   
            }
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

        private void DrawPoint(Vector3 point, Color color, float intensity)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < renderWidth && point.Y < renderHeight)
            {
                int red = (int)(color.R * intensity);
                int green = (int)(color.G * intensity);
                int blue = (int)(color.B * intensity);
                PutPixel((int)point.X, (int)point.Y, point.Z, red, green, blue);
            }
        }

        private void PutPixel(int x, int y, float z, int red, int green, int blue)
        {
            var index = (x + y * renderWidth);

            if (depthBuffer[index] < z)
            {
                return;
            }

            depthBuffer[index] = z;
            bmp[x, y] = Color.FromArgb(255, red, green, blue);
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
        void ProcessScanLine(ScanLineData data, Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pd, Color color)
        {
            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var leftGradient = pa.Y != pb.Y ? (data.CurrentY - pa.Y) / (pb.Y - pa.Y) : 1;
            var rightGradient = pc.Y != pd.Y ? (data.CurrentY - pc.Y) / (pd.Y - pc.Y) : 1;

            var leftX = (int)Interpolate(pa.X, pb.X, leftGradient);
            var rightX = (int)Interpolate(pc.X, pd.X, rightGradient);

            var z1 = Interpolate(pa.Z, pb.Z, leftGradient);
            var z2 = Interpolate(pc.Z, pd.Z, rightGradient);
            
            for (var x = leftX; x < rightX; x++)
            {
                var gradientZ = (x - leftX) / (float)(rightX - leftX);

                var z = Interpolate(z1, z2, gradientZ);

                // changing the color value using the cosine of the angle
                // between the light vector and the normal vector
                var intensity = data.NDotla;
                DrawPoint(new Vector3(x, data.CurrentY, z), color, intensity);
            }
        }
        
        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        float ComputeNDotL(Vector3 vertex, Vector3 normal, Vector3 lightPosition) 
        {
            var lightDirection = lightPosition - vertex;

            normal = Vector3.Normalize(normal);
            lightDirection = Vector3.Normalize(lightDirection);

            return Math.Max(0, Vector3.Dot(normal, lightDirection));
        }

        private bool Cull(Vector3 faceNormal, Vector3 point)
        {
            var result = true;
            
            var viewPos = _camera.Position;
            
            var viewDirection = point - viewPos;

            faceNormal = Vector3.Normalize(faceNormal);
            viewDirection = Vector3.Normalize(viewDirection);

            if (Vector3.Dot(faceNormal, viewDirection) > 0.0f)
                result = false;

            return result;
        }

        private bool BackfaceCulling(Polygon polygon)
        {
            var n1 = _model.Normals[polygon.NormalsIndexes[0]];
            var n2 = _model.Normals[polygon.NormalsIndexes[1]];
            var n3 = _model.Normals[polygon.NormalsIndexes[2]];

            var wp1 = _model.WorldVertices[polygon.VerticesIndexes[0]];
            var wp2 = _model.WorldVertices[polygon.VerticesIndexes[1]];
            var wp3 = _model.WorldVertices[polygon.VerticesIndexes[2]];

            var centralPoint = (wp1 + wp2 + wp3) / 3;
            return Cull(Vector3.Cross(wp2 - wp1, wp3 - wp1), centralPoint);
        }
        
        public void DrawTriangle(Polygon polygon, Vector3 p1, Vector3 p2, Vector3 p3, Color color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Y > p2.Y) (p2, p1) = (p1, p2);

            if (p2.Y > p3.Y) (p2, p3) = (p3, p2);

            if (p1.Y > p2.Y) (p2, p1) = (p1, p2);


            var n1 = _model.Normals[polygon.NormalsIndexes[0]];
            var n2 = _model.Normals[polygon.NormalsIndexes[1]];
            var n3 = _model.Normals[polygon.NormalsIndexes[2]];

            var wp1 = _model.WorldVertices[polygon.VerticesIndexes[0]];
            var wp2 = _model.WorldVertices[polygon.VerticesIndexes[1]];
            var wp3 = _model.WorldVertices[polygon.VerticesIndexes[2]];


            // Computing the cos of the angle between the light vector and the normal vector
            // it will return a value between 0 and 1 that will be used as the intensity of the color
            float nDotL1 = ComputeNDotL(wp1, n1, lightPos);
            float nDotL2 = ComputeNDotL(wp2, n2, lightPos);
            float nDotL3 = ComputeNDotL(wp3, n3, lightPos);
            float nDotL = (nDotL1 + nDotL2 + nDotL3) / 3;
            
            var data = new ScanLineData { NDotla = nDotL };
            

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
                    data.CurrentY = y;
                    
                    if (y < p2.Y)
                    {
                        ProcessScanLine(data, p1, p3, p1, p2, color);
                    }
                    else
                    {
                        ProcessScanLine(data, p1, p3, p2, p3, color);
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
                    data.CurrentY = y;
                    
                    if (y < p2.Y)
                    {
                        ProcessScanLine(data, p1, p2, p1, p3, color);
                    }
                    else
                    {
                        ProcessScanLine(data, p2, p3, p1, p3, color);
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
        
        private struct ScanLineData
        {
            public int CurrentY;
            public float NDotla;
            public float NDotlb;
            public float NDotlc;
            public float NDotld;
        }
    }
}