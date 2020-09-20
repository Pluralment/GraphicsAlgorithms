using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ACG.Extensions;

namespace ACG.Vectors
{
    internal class Vector3D
    {
        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3D MultiplyWith(float[,] matrix)
        {
            float[,] thisMatrix = new float[,] { { X }, { Y }, { Z }, { 1 } };
            var completeVector = matrix.Multiply(thisMatrix);
            return new Vector3D(completeVector[0, 0], completeVector[1, 0], completeVector[2, 0]);
        }
        public void DeltaOffsets(float dX, float dY, float dZ)
        {
            X += dX;
            Y += dY;
            Z += dZ;
        }
        public void ScaleVector(float scaleFactor, float xPivot, float yPivot, float zPivot)
        {
            var completeMatrix = Matrixes.GetTranslationMatrix(xPivot, yPivot, zPivot)
                                .Multiply(Matrixes.GetScaleMatrix(scaleFactor))
                                .Multiply(Matrixes.GetTranslationMatrix(-xPivot, -yPivot, -zPivot));
            Vector3D vector = this.MultiplyWith(completeMatrix);
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }
        public void RotateVectorX(float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrixes.GetTranslationMatrix(dX, dY, dZ)
                                 .Multiply(Matrixes.GetXAxisRotateMatrix(degree))
                                 .Multiply(Matrixes.GetTranslationMatrix(-dX, -dY, -dZ));
            Vector3D vector = this.MultiplyWith(completeMatrix);
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }
        public void RotateVectorY(float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrixes.GetTranslationMatrix(dX, dY, dZ)
                                 .Multiply(Matrixes.GetYAxisRotateMatrix(degree))
                                 .Multiply(Matrixes.GetTranslationMatrix(-dX, -dY, -dZ));
            Vector3D vector = this.MultiplyWith(completeMatrix);
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }
        public void RotateVectorZ(float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrixes.GetTranslationMatrix(dX, dY, dZ)
                                 .Multiply(Matrixes.GetZAxisRotateMatrix(degree))
                                 .Multiply(Matrixes.GetTranslationMatrix(-dX, -dY, -dZ));
            Vector3D vector = this.MultiplyWith(completeMatrix);
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }
    }
}
