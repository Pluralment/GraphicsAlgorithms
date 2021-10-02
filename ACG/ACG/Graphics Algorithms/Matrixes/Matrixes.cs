using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsModeler.Matrixes
{
    public static class Matrixes
    {
        public static Matrix4x4 GetRotateMatrix(float xDegree, float yDegree, float zDegree)
        {
            return Matrix4x4.CreateRotationX(xDegree) * Matrix4x4.CreateRotationY(yDegree) * Matrix4x4.CreateRotationZ(zDegree);
        }

        public static Matrix4x4 GetTranslationMatrix(float deltaX, float deltaY, float deltaZ)
        {
            return Matrix4x4.CreateTranslation(new Vector3(deltaX, deltaY, deltaZ));
        }

        public static Matrix4x4 GetScaleMatrix(float scaleCofficient)
        {
            return Matrix4x4.CreateScale(scaleCofficient);
        }
    }
}
