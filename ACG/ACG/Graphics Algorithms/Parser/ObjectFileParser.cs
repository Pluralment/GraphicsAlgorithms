using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using GraphicsModeler.Helper;
using GraphicsModeler.Scene;

namespace GraphicsModeler.Parser
{
    public class ObjectFileParser
    {
        private string _content;
        private string[] _lines;
        private List<Vector3> _vectors;
        private List<Vector3> _textures;
        private List<Vector3> _normals;
        private List<Polygon> _polygons = new List<Polygon>();
        

        public List<Vector3> Vectors
        {
            get => _vectors;
            set => _vectors = value;
        }
        
        public List<Vector3> Textures
        {
            get => _textures;
            set => _textures = value;
        }
        
        public List<Vector3> Normals
        {
            get => _normals;
            set => _normals = value;
        }
        
        public List<Polygon> Polygons
        {
            get => _polygons;
            set => _polygons = value;
        }

        public ObjectFileParser()
        {
            
        }
    }
}
