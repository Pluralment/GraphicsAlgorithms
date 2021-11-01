using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GraphicsModeler.Helper;

namespace GraphicsModeler.Scene
{
    public static class VertexTransformator
    {
        private static Matrix4x4 GetTransformMatrix(Model model, Camera camera)
        {
            var worldMatrix = GetWorldMatrix(model.Scale, model.Rotation, model.Position);
            var viewMatrix = GetViewMatrix(camera.Position, camera.Target, camera.Up);
            var perspectiveMatrix = GetPerspectiveMatrix(
                camera.Fov,
                (float)camera.Width / camera.Height,
                1f,
                100f
            );
            var transformedMatrix = worldMatrix * viewMatrix * perspectiveMatrix;
            return transformedMatrix;
        }
        
        public static List<Vector4> Transform(Model model, Camera camera)
        {
            var points = model.Mesh.Vertices;

            var transformedMatrix = GetTransformMatrix(model, camera);
            var viewportMatrix = GetViewportMatrix(camera.Width, camera.Height);
            
            
            
            
            
            var transformedPoints = new Vector4[points.Count];
            var worldPoints = new Vector3[points.Count];
            Parallel.For(0, points.Count, i =>
            {
                var point = points[i];
                var transformedPoint = Vector4.Transform(point, transformedMatrix);
                
                // Implement Sutherland-Hodgman clipping //
                
                // Perspective divide to get NDC.
                if (transformedPoint.W != 0.0f)
                {
                    transformedPoint /= transformedPoint.W;  
                }
                
                transformedPoint = Vector4.Transform(transformedPoint, viewportMatrix);
                
                transformedPoints[i] = transformedPoint;
            });
            return transformedPoints.ToList();
        }
        
        private static Matrix4x4 GetViewportMatrix(float width, float height,
            float Xmin = 0, float Ymin = 0)
        {
            return new Matrix4x4(
                width / 2,        0,                 0, 0,
                0,                -height / 2,       0, 0,
                0,                0,                 1, 0,
                Xmin + width / 2, Ymin + height / 2, 0, 1
            );
        }
        
        private static Matrix4x4 GetPerspectiveMatrix(float fieldOfView, float aspectRatio,
            float nearPlaneDistance, float farPlaneDistance)
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(
                fieldOfView,
                aspectRatio,
                nearPlaneDistance,
                farPlaneDistance);
        }

        private static Matrix4x4 GetViewMatrix(Vector3 cameraPosition, Vector3 target, Vector3 up)
        {
            return Matrix4x4.CreateLookAt(cameraPosition, target, up);
        }

        private static Matrix4x4 GetWorldMatrix(float scaleFactor, 
            Vector3 rotation, 
            Vector3 translation)
        {
            var initMatrix = Matrix4x4.CreateWorld(
              translation,
                             new Vector3(0, 0, -1),
                new Vector3(0, -1, 0));
            var scaleMatrix = Matrix4x4.CreateScale(scaleFactor);
            var rotateMatrix = Matrix4x4.CreateRotationX(rotation.X)
                               * Matrix4x4.CreateRotationY(rotation.Y) 
                               * Matrix4x4.CreateRotationZ(rotation.Z);
            return scaleMatrix * rotateMatrix * initMatrix;
            /*var scaleMatrix = Matrix4x4.CreateScale(scaleFactor);
            var rotateMatrix = Matrix4x4.CreateRotationX(rotation.X)
                               * Matrix4x4.CreateRotationY(rotation.Y) 
                               * Matrix4x4.CreateRotationZ(rotation.Z);
            var translateMatrix = Matrix4x4.CreateTranslation(translation);
            return scaleMatrix * rotateMatrix * translateMatrix;*/
        }
    }
}