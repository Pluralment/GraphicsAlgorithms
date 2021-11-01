using System.Collections.Generic;
using System.Numerics;

namespace GraphicsModeler.Scene
{
    public class Mesh
    {
        //public List<List<int>> Polygons { get; set; }
        public List<Polygon> Polygons { get; set; }
        public List<Vector4> Vertices { get; set; }

        public Mesh() { }
    }
}