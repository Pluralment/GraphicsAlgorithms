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

        public static Vector4 Translate(this Vector4 vector, float dX, float dY, float dZ)
        {
            return vector.TransformBy(Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ));
                
        }

        public static Vector4 ScaleVector(this Vector4 vector, float scaleFactor, float xPivot, float yPivot, float zPivot)
        {
            var completeMatrix = Matrixes.Matrixes.GetTranslationMatrix(-xPivot, -yPivot, -zPivot)
                .MultiplyBy(Matrixes.Matrixes.GetScaleMatrix(scaleFactor))
                .MultiplyBy(Matrixes.Matrixes.GetTranslationMatrix(xPivot, yPivot, zPivot));
            return Vector4.Transform(vector, completeMatrix);
        }
        public static Vector4 RotateVectorX(this Vector4 vector, float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrixes.Matrixes.GetTranslationMatrix(-dX, -dY, -dZ)
                .MultiplyBy(Matrixes.Matrixes.GetXAxisRotateMatrix(degree))
                .MultiplyBy(Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ));
            return Vector4.Transform(vector, completeMatrix);
        }
        public static Vector4 RotateVectorY(this Vector4 vector, float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrixes.Matrixes.GetTranslationMatrix(-dX, -dY, -dZ)
                .MultiplyBy(Matrixes.Matrixes.GetYAxisRotateMatrix(degree))
                .MultiplyBy(Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ));
            return Vector4.Transform(vector, completeMatrix);
        }
        public static Vector4 RotateVectorZ(this Vector4 vector, float degree, float dX, float dY, float dZ)
        {
            var completeMatrix = Matrixes.Matrixes.GetTranslationMatrix(-dX, -dY, -dZ)
                .MultiplyBy(Matrixes.Matrixes.GetZAxisRotateMatrix(degree))
                .MultiplyBy(Matrixes.Matrixes.GetTranslationMatrix(dX, dY, dZ));
            return Vector4.Transform(vector, completeMatrix);
        }

        public static Matrix4x4 MultiplyBy(this Matrix4x4 value1, Matrix4x4 value2)
        {
            return Matrix4x4.Multiply(value1, value2);
        }

        public static Vector4 TransformBy(this Vector4 vector, Matrix4x4 matrix)
        {
            return Vector4.Transform(vector, matrix);
        }
    }
}
