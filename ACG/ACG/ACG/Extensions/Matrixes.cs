using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACG.Vectors;

namespace ACG.Extensions
{
    internal static class Matrixes
    {

        public static float[,] GetZAxisRotateMatrix(float degree)
        {
           return new float[4, 4]{ { (float)Math.Cos(degree), (float)Math.Sin(degree), 0, 0 },
                                     { -(float)Math.Sin(degree), (float)Math.Cos(degree), 0, 0 },
                                     { 0, 0, 1, 0 },
                                     { 0, 0, 0, 1 } };
        }
        public static float[,] GetXAxisRotateMatrix(float degree)
        {
            return new float[4, 4]{ { 1, 0, 0, 0 },
                                    { 0, (float)Math.Cos(degree), -(float)Math.Sin(degree), 0 },
                                    { 0, (float)Math.Sin(degree), (float)Math.Cos(degree), 0 },
                                    { 0, 0, 0, 1 } };

        }
        public static float[,] GetYAxisRotateMatrix(float degree)
        {
            return new float[4, 4]{ { (float)Math.Cos(degree), 0, (float)Math.Sin(degree), 0 },
                                    { 0, 1, 0, 0 },
                                    { -(float)Math.Sin(degree), 0, (float)Math.Cos(degree), 0 },
                                    { 0, 0, 0, 1 } };
        }
        public static float[,] GetTranslationMatrix(float deltaX, float deltaY, float deltaZ)
        {
            return new float[4, 4]{ { 1, 0, 0, deltaX },
                                    { 0, 1, 0, deltaY },
                                    { 0, 0, 1, deltaZ },
                                    { 0, 0, 0, 1 } };
        }
        public static float[,] GetScaleMatrix(float scaleCofficient)
        {
            return new float[4, 4]{ { scaleCofficient, 0, 0, 0 },
                                    { 0, scaleCofficient, 0, 0 },
                                    { 0, 0, scaleCofficient, 0 },
                                    { 0, 0, 0, 1 } };
        }
    }
}
