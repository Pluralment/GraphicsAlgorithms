using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using GraphicsModeler.Extensions;
using System.Drawing;

namespace GraphicsModeler.Shapes
{
    public class Polygon
    {

        public IEnumerable<PointF> GetDDALine(int vectorIndex, int vectorIndex2)
        {
            var p1 = Vectors[vectorIndex];
            var p2 = Vectors[vectorIndex2];
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var L = Math.Max(Math.Abs(dx), Math.Abs(dy));
            var xInc = dx / L;
            var yInc = dy / L;
            var x = p1.X;
            var y = p1.Y;

            for (int i = 0; i <= L; i++)
            {
                yield return new PointF(x, y);
                x += xInc;
                y += yInc;
            }
        }

        public List<Vector4> Vectors { get; set; }
        public Polygon(List<Vector4> vectorList)
        {
            if (vectorList is null)
                throw new ArgumentNullException(nameof(vectorList));

            Vectors = vectorList;
        }

        public void ScalePolygon(float scaleFactor, float xPivot, float yPivot, float zPivot)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i] = Vectors[i].ScaleVector(scaleFactor, xPivot, yPivot, zPivot);
            }
        }
        public void DeltaOffsets(float dX, float dY, float dZ)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i] = Vectors[i].DeltaOffsets(dX, dY, dZ);
            }
        }
        public void RotateVectorX(float degree, float dX, float dY, float dZ)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i] = Vectors[i].RotateVectorX(degree, dX, dY, dZ);
            }
        }
        public void RotateVectorY(float degree, float dX, float dY, float dZ)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i].RotateVectorY(degree, dX, dY, dZ);
            }
        }
        public void RotateVectorZ(float degree, float dX, float dY, float dZ)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i] = Vectors[i].RotateVectorZ(degree, dX, dY, dZ);
            }
        }
    }
}
