using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using GraphicsModeler.Matrixes;

namespace GraphicsModeler.Extensions
{
    public static class Extensions
    {

        public static Vector4 DeltaOffsets(this Vector4 vector, float dX, float dY, float dZ)
        {
            return new Vector4(vector.X + dX, vector.Y + dY, vector.Z + dZ, 0);
        }

        public static Vector4 ScaleVector(this Vector4 vector, float scaleFactor, float xPivot, float yPivot, float zPivot)
        {
            var completeMatrix = Matrix4x4.Multiply(
                                    Matrix4x4.Multiply(Matrixes.Matrixes.GetTranslationMatrix(xPivot, yPivot, zPivot),
                                    Matrixes.Matrixes.GetScaleMatrix(scaleFactor)),
                                    Matrixes.Matrixes.GetTranslationMatrix(-xPivot, -yPivot, -zPivot));
            return Vector4.Transform(vector, completeMatrix);
        }
        public static Vector4 RotateVectorX(this Vector4 vector, float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrix4x4.Multiply(
                                    Matrix4x4.Multiply(
                                    Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ),
                                    Matrixes.Matrixes.GetXAxisRotateMatrix(degree)),
                                    Matrixes.Matrixes.GetTranslationMatrix(-dX, -dY, -dZ));
            return Vector4.Transform(vector, completeMatrix);
        }
        public static Vector4 RotateVectorY(this Vector4 vector, float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrix4x4.Multiply(
                                    Matrix4x4.Multiply(
                                    Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ),
                                    Matrixes.Matrixes.GetYAxisRotateMatrix(degree)),
                                    Matrixes.Matrixes.GetTranslationMatrix(-dX, -dY, -dZ));
            return Vector4.Transform(vector, completeMatrix);
        }
        public static Vector4 RotateVectorZ(this Vector4 vector, float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrix4x4.Multiply(
                                    Matrix4x4.Multiply(
                                    Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ),
                                    Matrixes.Matrixes.GetZAxisRotateMatrix(degree)),
                                    Matrixes.Matrixes.GetTranslationMatrix(-dX, -dY, -dZ));
            return Vector4.Transform(vector, completeMatrix);
        }
    }
}
