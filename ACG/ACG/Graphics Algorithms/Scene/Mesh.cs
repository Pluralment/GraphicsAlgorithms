using System.Collections.Generic;
using System.Numerics;
using System.Windows.Forms.VisualStyles;

namespace GraphicsModeler.Scene
{
    public class Mesh
    {
        //public List<List<int>> Polygons { get; set; }
        public List<Polygon> Polygons { get; set; }
        public List<Vector3> Vertices { get; set; }
        public List<float> W { get; set; } = new List<float>();

        public Mesh() { }
    }
    
    public struct Vertex
    {
        public Vector3 Normal;
        public Vector3 Coordinates;
        public Vector3 WorldCoordinates;
        public Vector2 UV;
        public float W;
    }
}