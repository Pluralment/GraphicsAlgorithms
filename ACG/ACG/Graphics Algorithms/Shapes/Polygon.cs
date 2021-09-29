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
            var xInitial = Vectors[vectorIndex].X;
            var yInitial = Vectors[vectorIndex].Y;
            var xFinal = Vectors[vectorIndex2].X;
            var yFinal = Vectors[vectorIndex2].Y;

            float dx = xFinal - xInitial, dy = yFinal - yInitial, steps, k, xf, yf;
            float xIncrement, yIncrement, x = xInitial, y = yInitial;

            if (Math.Abs(dx) > Math.Abs(dy)) steps = Math.Abs(dx);

            else steps = Math.Abs(dy);
            xIncrement = dx / (float)steps;
            yIncrement = dy / (float)steps;
            for (k = 0; k < steps; k++)
            {
                x += xIncrement;
                xf = (int)x;
                y += yIncrement;
                yf = (int)y;
                yield return new PointF(xf, yf);
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
        public void Translate(float dX, float dY, float dZ)
        {
            for (int i = 0; i < Vectors.Count; i++)
            {
                Vectors[i] = Vectors[i].Translate(dX, dY, dZ);
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
                Vectors[i] = Vectors[i].RotateVectorY(degree, dX, dY, dZ);
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
