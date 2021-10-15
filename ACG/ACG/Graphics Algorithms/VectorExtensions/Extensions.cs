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

        public static void Translate(this List<Vector4> points, Vector3 translation)
        {
            Parallel.For(0, points.Count, i =>
            {
                points[i] = points[i].TransformBy(Matrixes.Matrixes.GetTranslationMatrix(translation.X, translation.Y, translation.Z));
            });
        }

        public static void ToWorld(this List<Vector4> points, Vector3 position)
        {
            Parallel.For(0, points.Count, i =>
            {
                points[i] = points[i].TransformBy(Matrixes.Matrixes.GetWorldMatrix(position.X, position.Y, position.Z));
            });
        }
        
        public static void ToView(this List<Vector4> points, Vector3 camera, Vector3 target, Vector3 up)
        {
            Parallel.For(0, points.Count, i =>
            {
                points[i] = points[i].TransformBy(Matrixes.Matrixes.GetViewMatrix(camera, target, up));
            });
        }

        
        public static void ToPerspective(this List<Vector4> points, float fov, float width, float height)
        {
            var perspectiveMatrix = Matrixes.Matrixes.GetPerspectiveMatrix(fov, (float)width / height,
                1f, 100f);
            Parallel.For(0, points.Count, i =>
            {
                var point = points[i];
                point = point.TransformBy(perspectiveMatrix);
                
                // Perspective divide to get NDC.
                if (point.W != 0.0f)
                {
                    point /= point.W;   
                }
                
                points[i] = point;
            });
        }
        
        public static void ToViewport(this List<Vector4> points, float width, float height, float Xmin = 0, float Ymin = 0)
        {
            var viewPortMatrix = Matrixes.Matrixes.GetViewportMatrix(width, height, Xmin, Ymin);
            Parallel.For(0, points.Count, i =>
            {
                var point = points[i];
                point = point.TransformBy(viewPortMatrix);
                points[i] = point;
            });
        }
        

        public static void ScaleVectors(this List<Vector4> points, float scaleFactor, Vector3 translation)
        {
            var completeMatrix = Matrixes.Matrixes
                .GetTranslationMatrix(-translation.X, -translation.Y, -translation.Z) *
                Matrixes.Matrixes.GetScaleMatrix(scaleFactor) *
                Matrixes.Matrixes.GetTranslationMatrix(translation.X, translation.Y, translation.Z);
            Parallel.For(0, points.Count, i =>
            {
                points[i] = points[i].TransformBy(completeMatrix);
            });
        }

        public static void RotateVectors(this List<Vector4> points, Vector3 degrees, Vector3 translation)
        {
            var completeMatrix = Matrixes.Matrixes.GetTranslationMatrix(-translation.X, -translation.Y, -translation.Z) *
                Matrixes.Matrixes.GetRotateMatrix(degrees.X, degrees.Y, degrees.Z) *
                Matrixes.Matrixes.GetTranslationMatrix(translation.X, translation.Y, translation.Z);
            Parallel.For(0, points.Count, i =>
            {
                points[i] = points[i].TransformBy(completeMatrix);
            });
        }


        public static Vector4 TransformBy(this Vector4 vector, Matrix4x4 matrix)
        {
            return Vector4.Transform(vector, matrix);
        }
    }
}
