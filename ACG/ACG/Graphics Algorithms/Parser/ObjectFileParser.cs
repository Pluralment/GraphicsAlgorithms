using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using GraphicsModeler.Helper;

namespace GraphicsModeler.Parser
{
    public class ObjectFileParser
    {
        private string _content;
        private string[] _lines;
        private List<Vector4> _vectors;
        private List<List<int>> _polygons = new List<List<int>>();
        

        public List<Vector4> Vectors
        {
            get => _vectors;
            set => _vectors = value;
        }
        
        public List<List<int>> Polygons
        {
            get => _polygons;
            set => _polygons = value;
        }

        public ObjectFileParser()
        {
            
        }

        
        public ObjectFileParser(string fileName)
        {
            if (fileName is null)
                throw new ArgumentNullException(nameof(fileName));

            _content = File.ReadAllText(fileName);
            _lines = _content.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Trim() != string.Empty)
                .Select(x => x.Trim()).ToArray();
            _vectors = _lines.Where(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("v", StringComparison.OrdinalIgnoreCase))
                                .Select(x => new Vector4(float.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                                          float.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2]),
                                                          float.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[3]),
                                                          1))
                                .ToList();
        }
        


        public Model GetModel()
        {
            var faces = _lines.Where(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("f", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (string face in faces)
            {
                var polygonVertexes = face.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).ToArray();
                var polygonList = new List<int>();
                for (int i = 0; i < polygonVertexes.Length; i++)
                {
                    polygonList.Add(int.Parse(polygonVertexes[i].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0]) - 1);
                }
                
                _polygons.Add(polygonList);
            }

            return new Model(_polygons, _vectors);
        }
    }
}
