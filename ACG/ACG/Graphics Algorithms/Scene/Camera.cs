using System;
using System.Numerics;

namespace GraphicsModeler.Scene
{
    public class Camera
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector3 Target { get; set; } = Vector3.Zero;
        public Vector3 Up { get; set; } = new Vector3(0 ,1, 0);
        public float Fov { get; set; } = (float)(Math.PI * 0.5);
        public int Width { get; set; }
        public int Height { get; set; }
        
        public Camera() {}

        public Camera(Vector3 position, Vector3 target, Vector3 up)
        {
            Position = position;
            Target = target;
            Up = up;
        }
    }
}