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
        public static Matrix4x4 GetWorldMatrix(float deltaX, float deltaY, float deltaZ)
        {
            return Matrix4x4.CreateWorld(
                new Vector3(deltaX, deltaY, deltaZ),
                new Vector3(0, 0, -1),
                new Vector3(0, -1, 0));
        }
        
        public static Matrix4x4 GetViewMatrix(Vector3 camera, Vector3 target, Vector3 up)
        {
            return Matrix4x4.CreateLookAt(
                camera,
                target,
                up);
        }


        public static Matrix4x4 GetPerspectiveMatrix(float fieldOfView, float aspectRatio,
            float nearPlaneDistance, float farPlaneDistance)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(
                fieldOfView,
                aspectRatio,
                nearPlaneDistance,
                farPlaneDistance);
        }
        
        public static Matrix4x4 GetViewPortMatrix(float width, float height,
            float Xmin = 0, float Ymin = 0)
        {
            return new Matrix4x4(
                width / 2, 0, 0, 0,
                0, -height / 2, 0, 0,
                0, 0, 1, 0,
                Xmin + width / 2, Ymin + height / 2, 0, 1
            );
        }

        

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
