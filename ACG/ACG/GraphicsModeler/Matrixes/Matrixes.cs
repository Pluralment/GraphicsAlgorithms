using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphicsModeler.Matrixes
{
    internal static class Matrixes
    {
        public static Matrix4x4 GetZAxisRotateMatrix(float degree)
        {
            return new Matrix4x4(
                (float)Math.Cos(degree), (float)Math.Sin(degree), 0, 0,
                -(float)Math.Sin(degree), (float)Math.Cos(degree), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
        }

        public static Matrix4x4 GetXAxisRotateMatrix(float degree)
        {
            return new  Matrix4x4(1, 0, 0, 0,
                                  0, (float)Math.Cos(degree), -(float)Math.Sin(degree), 0 ,
                                  0, (float)Math.Sin(degree), (float)Math.Cos(degree), 0 ,
                                  0, 0, 0, 1);
        }
        public static Matrix4x4 GetYAxisRotateMatrix(float degree)
        {
            return new Matrix4x4((float)Math.Cos(degree), 0, (float)Math.Sin(degree), 0,
                                 0, 1, 0, 0,
                                 -(float)Math.Sin(degree), 0, (float)Math.Cos(degree), 0,
                                 0, 0, 0, 1);
        }
        public static Matrix4x4 GetTranslationMatrix(float deltaX, float deltaY, float deltaZ)
        {
            return new Matrix4x4(1, 0, 0, deltaX,
                                 0, 1, 0, deltaY,
                                 0, 0, 1, deltaZ,
                                 0, 0, 0, 1);
        }

        public static Matrix4x4 GetScaleMatrix(float scaleCofficient)
        {
            return new Matrix4x4(scaleCofficient, 0, 0, 0,
                                 0, scaleCofficient, 0, 0,
                                 0, 0, scaleCofficient, 0,
                                 0, 0, 0, 1);
        }
    }
}
