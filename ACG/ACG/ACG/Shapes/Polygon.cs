using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACG.Vectors;
using ACG.Extensions;
using System.Drawing.Drawing2D;

namespace ACG.Shapes
{
    internal class Polygon
    {
        public Vector3D FirstVector { get; set; }
        public Vector3D SecondVector { get; set; }
        public Vector3D ThirdVector { get; set; }
        public Polygon()
        {
            FirstVector = new Vector3D();
            SecondVector = new Vector3D();
            ThirdVector = new Vector3D();
        }
        public Polygon(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            if (v1 is null)
                throw new ArgumentNullException(nameof(v1));
            if (v2 is null)
                throw new ArgumentNullException(nameof(v2));
            if (v3 is null)
                throw new ArgumentNullException(nameof(v3));

            FirstVector = v1;
            SecondVector = v2;
            ThirdVector = v3;
        }
        public void ScalePolygon(float scaleFactor, float xPivot, float yPivot, float zPivot)
        {
            foreach (var vector in GetVectors())
            {
                vector.ScaleVector(scaleFactor, xPivot, yPivot, zPivot);
            }
        }
        public void DeltaOffsets(float dX, float dY, float dZ)
        {
            foreach (var vector in GetVectors())
            {
                vector.DeltaOffsets(dX, dY, dZ);
            }
        }
        public void RotateVectorX(float degree, float dX, float dY, float dZ)
        {
            foreach (var vector in GetVectors())
            {
                vector.RotateVectorX(degree, dX, dY, dZ);
            }
        }
        public void RotateVectorY(float degree, float dX, float dY, float dZ)
        {
            foreach (var vector in GetVectors())
            {
                vector.RotateVectorY(degree, dX, dY, dZ);
            }
        }
        public void RotateVectorZ(float degree, float dX, float dY, float dZ)
        {
            foreach (var vector in GetVectors())
            {
                vector.RotateVectorZ(degree, dX, dY, dZ);
            }
        }
        public GraphicsPath GetGraphicsPath()
        {
            var result = new GraphicsPath();
            result.AddLine(FirstVector.X, FirstVector.Y, SecondVector.X, SecondVector.Y);
            result.AddLine(SecondVector.X, SecondVector.Y, ThirdVector.X, ThirdVector.Y);
            result.AddLine(ThirdVector.X, ThirdVector.Y, FirstVector.X, FirstVector.Y);
            return result;
        }

        private IEnumerable<Vector3D> GetVectors()
        {
            yield return FirstVector;
            yield return SecondVector;
            yield return ThirdVector;
        }
    }
}
