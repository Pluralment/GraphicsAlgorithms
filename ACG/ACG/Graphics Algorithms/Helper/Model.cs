using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using GraphicsModeler.Extensions;
using System.Threading.Tasks;
using System.Drawing;

namespace GraphicsModeler.Helper
{
    public class Model
    {
        public Model(List<List<int>> polygons, List<Vector4> vertexes)
        {
            Polygons = polygons;
            Vertexes = vertexes;
        }

        public List<List<int>> Polygons { get; set; }
        public List<Vector4> Vertexes { get; set; }


        public void DrawToBitmap(ExtendedBitmap bmp)
        {
            bmp.LockBits();
            Parallel.ForEach(Polygons, p =>
            {
                for (var i = 0; i < p.Count - 1; i++)
                {
                    var line = GetDDALine(Vertexes[p[i]], Vertexes[p[i + 1]]);
                    foreach (var pt in line)
                        bmp[(int)pt.X, (int)pt.Y] = Color.FromArgb(255, Color.Firebrick);
                }

                if (p.Count > 0)
                    foreach (var pt in GetDDALine(Vertexes[p.First()], Vertexes[p.Last()]))
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
