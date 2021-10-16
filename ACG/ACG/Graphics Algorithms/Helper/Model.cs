using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using GraphicsModeler.Extensions;
using System.Threading.Tasks;
using System.Drawing;
using GraphicsModeler.Scene;

namespace GraphicsModeler.Helper
{
    public class Model
    {
        public Mesh Mesh { get; private set; }
        public Vector3 Position { get; set; } = Vector3.Zero;
        public int Scale { get; set; }

        public Model(Mesh mesh)
        {
            Mesh = mesh;
        }

        public void Draw(ExtendedBitmap bmp)
        {
            bmp.LockBits();
            Parallel.ForEach(Mesh.Polygons, p =>
            {
                for (var i = 0; i < p.Count - 1; i++)
                {
                    var line = GetDDALine(Mesh.Vertices[p[i]], Mesh.Vertices[p[i + 1]]);
                    foreach (var pt in line)
                        bmp[(int)pt.X, (int)pt.Y] = Color.FromArgb(255, Color.Firebrick);
                }

                if (p.Count > 0)
                    foreach (var pt in GetDDALine(Mesh.Vertices[p.First()], Mesh.Vertices[p.Last()]))
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
    }
}
