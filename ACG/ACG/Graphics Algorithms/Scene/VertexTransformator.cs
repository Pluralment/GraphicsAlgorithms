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
                0.1f,
                100f
            );
            var transformedMatrix = worldMatrix * viewMatrix * perspectiveMatrix;
            return transformedMatrix;
        }
        
        private static Matrix4x4 GetNormalMatrix(Vector3 rotation)
        {
            return Matrix4x4.CreateRotationX(rotation.X)
                               * Matrix4x4.CreateRotationY(rotation.Y) 
                               * Matrix4x4.CreateRotationZ(rotation.Z);
        }

        private static List<Vector3> TransformNormals(List<Vector3> normals, Vector3 rotation)
        {
            var worldNormals = new Vector3[normals.Count];
            var normalMatrix = GetNormalMatrix(rotation);
            Parallel.For(0, normals.Count, i =>
            {
                var normal = Vector3.TransformNormal(normals[i], normalMatrix);
                worldNormals[i] = normal;
            });
            return worldNormals.ToList();
        }

        /*private static List<Vector4> TransformVertices(Model model)
        {
            
        }*/
        
        public static Model Transform(Model model, Camera camera)
        {
            var points = new List<Vector4>();
            foreach (var p in model.Mesh.Vertices)
            {
                points.Add(new Vector4(p.X, p.Y, p.Z, 1));
            }
            var normals = model.Normals;
            var worldNormals = TransformNormals(normals, model.Rotation);
            
            
            var transformedMatrix = GetTransformMatrix(model, camera);
            var viewportMatrix = GetViewportMatrix(camera.Width, camera.Height);
            var worldMatrix = GetWorldMatrix(model.Scale, model.Rotation, model.Position);
            
            var transformedPoints = new Vector3[points.Count];
            var worldPoints = new Vector3[points.Count];
            Parallel.For(0, points.Count, i =>
            {
                var point = points[i];
                
                var worldPoint = Vector3.Transform(new Vector3(point.X, point.Y, point.Z),
                    worldMatrix);
                
                var transformedPoint = Vector4.Transform(point, transformedMatrix);
                
                // Implement Sutherland-Hodgman clipping //
                
                // Perspective divide to get NDC.
                if (transformedPoint.W != 0.0f)
                {
                    transformedPoint /= transformedPoint.W;  
                }
                
                transformedPoint = Vector4.Transform(transformedPoint, viewportMatrix);

                worldPoints[i] = worldPoint;
                transformedPoints[i] = new Vector3(transformedPoint.X, transformedPoint.Y, transformedPoint.Z);
            });

            var transformedMesh = new Mesh()
            {
                Vertices = transformedPoints.ToList(),
                Polygons = model.Mesh.Polygons
            };
            
            return new Model(transformedMesh)
            {
                WorldVertices = worldPoints.ToList(),
                Normals = worldNormals,
                Position = model.Position,
                Materials = model.Materials,
                Textures = model.Textures
            };
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
            var scaleMatrix = Matrix4x4.CreateScale(scaleFactor);
            var translateMatrix = Matrix4x4.CreateTranslation(translation);
            var rotateMatrix = Matrix4x4.CreateRotationX(rotation.X)
                               * Matrix4x4.CreateRotationY(rotation.Y) 
                               * Matrix4x4.CreateRotationZ(rotation.Z);
            return scaleMatrix * rotateMatrix * translateMatrix;
        }
    }
}