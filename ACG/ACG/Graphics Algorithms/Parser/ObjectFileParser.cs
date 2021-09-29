using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using GraphicsModeler.Shapes;

namespace GraphicsModeler.Parser
{
    public class ObjectFileParser
    {
        private string _content;
        private string[] _lines;
        private Vector4[] _vectors;
        public ObjectFileParser(string fileName)
        {
            if (fileName is null)
                throw new ArgumentNullException(nameof(fileName));

            _content = File.ReadAllText(fileName);
            _lines = _content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();
            _vectors = _lines.Where(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("v", StringComparison.OrdinalIgnoreCase))
                                .Select(x => new Vector4(float.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1]),
                                                          float.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2]),
                                                          float.Parse(x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[3]),
                                                          1))
                                .ToArray();
        }


        public List<Polygon> GetPolygons()
        {
            List<Polygon> polygonList = new List<Polygon>();
            var faces = _lines.Where(x => x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Equals("f", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (string face in faces)
            {
                var coords = face.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1).ToArray();
                int i0 = int.Parse(coords[0].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0]) - 1;
                int i1 = int.Parse(coords[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0]) - 1;
                int i2 = int.Parse(coords[2].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[0]) - 1;
                polygonList.Add(new Polygon(new List<Vector4>()
                {
                    new Vector4(_vectors[i0].X, _vectors[i0].Y, _vectors[i0].Z, 1),
                    new Vector4(_vectors[i1].X, _vectors[i1].Y, _vectors[i1].Z, 1),
                    new Vector4(_vectors[i2].X, _vectors[i2].Y, _vectors[i2].Z, 1)
                }));
            }

            return polygonList;
        }

    }
}
