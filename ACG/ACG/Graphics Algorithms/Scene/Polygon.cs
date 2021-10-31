using System.Collections.Generic;

namespace GraphicsModeler.Scene
{
    public class Polygon
    {
        public List<int> VerticesIndexes { get; set; }
        public List<int> TexturesIndexes { get; set; }
        public List<int> NormalsIndexes { get; set; }
    }
}