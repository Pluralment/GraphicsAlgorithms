using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACG.Vectors;

namespace ACG.Extensions
{
    internal static class MatrixExtensions
    {
        public static int RowsCount(this float[,] matrix)
        {
            return matrix.GetUpperBound(0) + 1;
        }
        public static int ColumnsCount(this float[,] matrix)
        {
            return matrix.GetUpperBound(1) + 1;
        }
        public static float[,] Multiply(this float[,] matrixA, float[,] matrixB)
        {
            if (matrixA.ColumnsCount() != matrixB.RowsCount())
            {
                throw new Exception("Умножение не возможно! Количество столбцов первой матрицы не равно количеству строк второй матрицы.");
            }

            var matrixC = new float[matrixA.RowsCount(), matrixB.ColumnsCount()];

            for (var i = 0; i < matrixA.RowsCount(); i++)
            {
                for (var j = 0; j < matrixB.ColumnsCount(); j++)
                {
                    matrixC[i, j] = 0;

                    for (var k = 0; k < matrixA.ColumnsCount(); k++)
                    {
                        matrixC[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }

            return matrixC;
        }
    }
}
