using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;
using GraphicsModeler.Helper;
using static GraphicsModeler.Scene.MaterialData;

namespace GraphicsModeler.Scene
{
    public class Drawer
    {
        private byte[] lightBuffer;
        private float[] depthBuffer;
        private ExtendedBitmap bmp;
        private int renderWidth;
        private int renderHeight;
        private Model _model;
        private Camera _camera;

        private MaterialData _materialData;
        
        private Vector3 _pointLightPos;

        public Drawer() {}

        public void Draw(ExtendedBitmap bitmap, Model model, Camera camera)
        {
            InitNewFrame(bitmap, model, camera);
            Render();
        }

        private void InitNewFrame(ExtendedBitmap bitmap, Model model, Camera camera)
        {
            bmp = bitmap;
            _model = model;
            _camera = camera;

            renderWidth = bmp.Width;
            renderHeight = bmp.Height;
            
            _pointLightPos = new Vector3(0, -1, 2);

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
        
        private void Render()
        {
            var polygons = _model.Mesh.Polygons;
            var vertices = _model.Mesh.Vertices;
            
            foreach(var material in _model.Materials)
            {
                _materialData = material;
                bmp.LockBits();
                Parallel.ForEach(polygons, p =>
                {
                    if (IsCulled(p)) return;
                    ProcessTriangle(p, vertices);
                });
                bmp.UnlockBits();
            }
        }

        private void ProcessTriangle(Polygon p, List<Vector3> vertices)
        {
            if (p.VerticesIndexes.Count == 3)
            {
                var p1 = vertices[p.VerticesIndexes[0]];
                var p2 = vertices[p.VerticesIndexes[1]];
                var p3 = vertices[p.VerticesIndexes[2]];
                
                var n1 = _model.Normals[p.NormalsIndexes[0]];
                var n2 = _model.Normals[p.NormalsIndexes[1]];
                var n3 = _model.Normals[p.NormalsIndexes[2]];

                var wp1 = _model.WorldVertices[p.VerticesIndexes[0]];
                var wp2 = _model.WorldVertices[p.VerticesIndexes[1]];
                var wp3 = _model.WorldVertices[p.VerticesIndexes[2]];

                var uv1 = _model.Textures[p.TexturesIndexes[0]];
                var uv2 = _model.Textures[p.TexturesIndexes[1]];
                var uv3 = _model.Textures[p.TexturesIndexes[2]];

                var w1 = _model.Mesh.W[p.VerticesIndexes[0]];
                var w2 = _model.Mesh.W[p.VerticesIndexes[1]];
                var w3 = _model.Mesh.W[p.VerticesIndexes[2]];

                var vertex1 = new Vertex { Coordinates = p1, Normal = n1, WorldCoordinates = wp1, UV = new Vector2(uv1.X, uv1.Y), W = w1 };
                var vertex2 = new Vertex { Coordinates = p2, Normal = n2, WorldCoordinates = wp2, UV = new Vector2(uv2.X, uv2.Y), W = w2 };
                var vertex3 = new Vertex { Coordinates = p3, Normal = n3, WorldCoordinates = wp3, UV = new Vector2(uv3.X, uv3.Y), W = w3 };
                
                DrawTriangle(p, vertex1, vertex2, vertex3, Color.DarkOliveGreen);   
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

        private void DrawPoint(Vector3 point, Color color, PixelColorData pixelData)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < renderWidth && point.Y < renderHeight)
            {
                var ambient = pixelData.AmbientColor * pixelData.AmbientCoef;
                var diffuse = pixelData.DiffuseColor * pixelData.DiffuseCoef * pixelData.DiffuseIntensity;
                var specular = pixelData.SpecularColor * pixelData.SpecularCoef;
                var newColor = ambient + diffuse + specular;

                int red = GetIntColor(newColor.R);
                int green = GetIntColor(newColor.G);
                int blue = GetIntColor(newColor.B);

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
            bmp[x, y] = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
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

        Vector2 InterpolateTexture(Vector2 startUv, Vector2 endUv, float startZ, float endZ, float gradient)
        {
            var uRec = Interpolate(startUv.X / startZ, endUv.X / endZ, gradient);
            var vRec = Interpolate(startUv.Y / startZ, endUv.Y / endZ, gradient);

            var u = uRec / Interpolate(1 / startZ, 1 / endZ, gradient);
            var v = vRec / Interpolate(1 / startZ, 1 / endZ, gradient);

            return new Vector2(u, v);
        }
        
        // drawing line between 2 points from left to right
        // papb -> pcpd
        // pa, pb, pc, pd must then be sorted before
        void ProcessScanLine(ScanLineData data, Vertex va, Vertex vb, Vertex vc, Vertex vd, Color color)
        {
            Vector3 pa = va.Coordinates;
            Vector3 pb = vb.Coordinates;
            Vector3 pc = vc.Coordinates;
            Vector3 pd = vd.Coordinates;

            Vector3 wa = va.WorldCoordinates;
            Vector3 wb = vb.WorldCoordinates;
            Vector3 wc = vc.WorldCoordinates;
            Vector3 wd = vd.WorldCoordinates;
            
            // Thanks to current Y, we can compute the gradient to compute others values like
            // the starting X (sx) and ending X (ex) to draw between
            // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
            var leftGradientY = pa.Y != pb.Y ? (data.CurrentY - pa.Y) / (pb.Y - pa.Y) : 1;
            var rightGradientY = pc.Y != pd.Y ? (data.CurrentY - pc.Y) / (pd.Y - pc.Y) : 1;
            
            var leftWc = Vector3.Lerp(wa, wb, leftGradientY);
            var rightWc = Vector3.Lerp(wc, wd, rightGradientY);
            
            var leftNormal = Vector3.Lerp(va.Normal, vb.Normal, leftGradientY);
            var rightNormal = Vector3.Lerp(vc.Normal, vd.Normal, rightGradientY);
            
            var leftX = (int)Interpolate(pa.X, pb.X, leftGradientY);
            var rightX = (int)Interpolate(pc.X, pd.X, rightGradientY);

            var leftZ = Interpolate(pa.Z, pb.Z, leftGradientY);
            var rightZ = Interpolate(pc.Z, pd.Z, rightGradientY);

            // Interpolating texture coords.
            //var leftUv = InterpolateTexture(va.UV, vb.UV, pa.Z, pb.Z, leftGradientY);
            var leftW = Interpolate(va.W, vb.W, leftGradientY);
            var leftUv = InterpolateTexture(va.UV, vb.UV, va.W, vb.W, leftGradientY);
            //var rightUv = InterpolateTexture(vc.UV, vd.UV, pc.Z, pd.Z, rightGradientY);
            var rightW = Interpolate(vc.W, vd.W, rightGradientY);
            var rightUv = InterpolateTexture(vc.UV, vd.UV, vc.W, vd.W, rightGradientY);
            //

            for (var x = leftX; x < rightX; x++)
            {
                var gradientX = (x - leftX) / (float)(rightX - leftX);
                
                var z = Interpolate(leftZ, rightZ, gradientX);

                var uv = InterpolateTexture(leftUv, rightUv, leftW, rightW, gradientX);
                //Color kd = (_materialData.DiffuseMap != null) ? _materialData.DiffuseMap.Map(uv.X, uv.Y) : Color.FromArgb(255, 153, 153, 153);
                Color kd = _materialData.DiffuseMap.Map(uv.X, uv.Y);
                Color ks = (_materialData.SpecularMap != null) ? _materialData.SpecularMap.Map(uv.X, uv.Y) : Color.FromArgb(255, 127, 127, 127);

                var wVertex = Vector3.Lerp(leftWc, rightWc, gradientX);
                var wVertexNormal = Vector3.Lerp(leftNormal, rightNormal, gradientX);

                // changing the color value using the cosine of the angle
                // between the light vector and the normal vector
                var diffuseIntensity = ComputeLightIntensity(wVertex, wVertexNormal, _pointLightPos);

                var specularIntensity = ComputeSpecularIntensity(wVertex, wVertexNormal, 
                    _pointLightPos, _camera.Position, (int)_materialData.SpecularExponent);

                //var ksRgba = new RgbaColor { R = 0.5f, G = 0.5f, B = 0.5f, A = 1.0f };
                var ksRgba = new RgbaColor { R = ks.R / 255.0f, G = ks.G / 255.0f, B = ks.B / 255.0f, A = 1.0f };
                ksRgba *= specularIntensity;

                var kdRgba = new RgbaColor { R = kd.R / 255.0f, G = kd.G / 255.0f, B = kd.B / 255.0f, A = 1.0f };

                Color clr = Color.DarkOliveGreen;
                var pixelData = new PixelColorData
                {
                    AmbientColor = _materialData.AmbientReflectivity,
                    AmbientCoef = kdRgba,
                    DiffuseCoef = kdRgba,
                    DiffuseIntensity = diffuseIntensity,
                    DiffuseColor = _materialData.DiffuseReflectivity,
                    SpecularCoef = ksRgba,
                    SpecularColor = _materialData.SpecularReflectivity
                };

                DrawPoint(new Vector3(x, data.CurrentY, z), color, pixelData);
            }
        }

        private static int GetIntColor(float value)
        {
            return (int)(value * 255);
        }

        private float ComputeSpecularIntensity(Vector3 fragPosition, Vector3 fragNormal, Vector3 lightPosition, 
            Vector3 viewPosition, int power)
        {
            var viewDirection = Vector3.Normalize(viewPosition - fragPosition);
            var lightDirection = Vector3.Normalize(lightPosition - fragPosition);
            var reflectDirection = Vector3.Reflect(-lightDirection, fragNormal);
            
            return (float)Math.Pow(Math.Max(0, Vector3.Dot(viewDirection, reflectDirection)), power);
        }
        
        // Compute the cosine of the angle between the light vector and the normal vector
        // Returns a value between 0 and 1
        private float ComputeLightIntensity(Vector3 fragPosition, Vector3 normal, Vector3 lightPosition) 
        {
            var lightDirection = lightPosition - fragPosition;

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

            if (Vector3.Dot(faceNormal, viewDirection) < 0.0f)
                result = false;

            return result;
        }

        private bool IsCulled(Polygon polygon)
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
        
        public void DrawTriangle(Polygon polygon, Vertex v1, Vertex v2, Vertex v3, Color color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (v1.Coordinates.Y > v2.Coordinates.Y) (v2, v1) = (v1, v2);

            if (v2.Coordinates.Y > v3.Coordinates.Y) (v2, v3) = (v3, v2);

            if (v1.Coordinates.Y > v2.Coordinates.Y) (v2, v1) = (v1, v2);

            var p1 = v1.Coordinates;
            var p2 = v2.Coordinates;
            var p3 = v3.Coordinates;

            // inverse slopes
            float dP1P2, dP1P3;

            // http://en.wikipedia.org/wiki/Slope
            // Computing inverse slopes
            if (p2.Y - p1.Y > 0.0f)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0.0f)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            var data = new ScanLineData();
            
            
            // First case where triangles are like that:
            // P3
            // -
            // -- 
            // - -
            // -  -
            // -   - P2
            // -  -
            // - -
            // -
            // P1
            if (dP1P2 > dP1P3)
            {
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.CurrentY = y;
                    
                    if (y < p2.Y)
                    {
                        ProcessScanLine(data, v1, v3, v1, v2, color);
                    }
                    else
                    {
                        ProcessScanLine(data, v1, v3, v2, v3, color);
                    }
                }
            }
            // First case where triangles are like that:
            //       P3
            //        -
            //       -- 
            //      - -
            //     -  -
            // P2 -   - 
            //     -  -
            //      - -
            //        -
            //       P1
            else
            {
                //if (p2.X >= p1.X || p2.X >= p3.X) throw new Exception("Incorrect vert X pos");
                for (var y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    data.CurrentY = y;
                    
                    if (y < p2.Y)
                    {
                        ProcessScanLine(data, v1, v2, v1, v3, color);
                    }
                    else
                    {
                        ProcessScanLine(data, v2, v3, v1, v3, color);
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
        
        private struct PixelColorData
        {
            public RgbaColor AmbientColor;
            public RgbaColor AmbientCoef;
            public RgbaColor DiffuseCoef;
            public float DiffuseIntensity;
            public RgbaColor DiffuseColor;
            public RgbaColor SpecularCoef;
            public RgbaColor SpecularColor;
        }
    }
}