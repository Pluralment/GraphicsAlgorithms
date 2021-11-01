using System.Collections.Generic;
using System.Numerics;
using GraphicsModeler.Scene;

namespace GraphicsModeler.Helper
{
    public class Model
    {
        public Mesh Mesh { get; private set; }
        public List<Vector3> Textures { get; set; } 
        public List<Vector3> Normals { get; set; } 
        
        public List<Vector3> WorldVertices { get; set; } 
        
        public Vector3 Position { get; set; }
        public float Scale { get; set; }
        public Vector3 Rotation { get; set; }
        
        public Model() {}

        public Model(Mesh mesh)
        {
            Mesh = mesh;
        }
    }
}
