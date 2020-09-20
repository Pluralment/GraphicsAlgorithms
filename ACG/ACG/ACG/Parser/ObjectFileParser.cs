using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ACG.Extensions;
using ACG.Parser;
using ACG.Shapes;
using ACG.Vectors;

namespace ACG.Parser
{
    internal class ObjectFileParser
    {
        private string content;
        private string[] lines;
        private Vector3D[] vectors;
        public ObjectFileParser(string fileName)
        {
            if (fileName is null)
                throw new ArgumentNullException(nameof(fileName));

            content = File.ReadAllText(fileName);
            lines = content.Split('\n').Select(x => x.Trim()).ToArray();
            vectors = lines.Where(x => x.Split(' ')[0] == "v")
                                .Select(x => new Vector3D(float.Parse(x.Split(' ')[1]),
                                                          float.Parse(x.Split(' ')[2]),
                                                          float.Parse(x.Split(' ')[3])))
                                .ToArray();
        }


        public Polygon[] GetPolygons()
        {
            List<Polygon> polygonList = new List<Polygon>();
            var faces = lines.Where(x => x.Split(' ')[0] == "f").ToArray();
            foreach (string face in faces)
            {
                var coords = face.Split(' ').Skip(1).ToArray();
                int i0 = int.Parse(coords[0].Split('/')[0]) - 1;
                int i1 = int.Parse(coords[1].Split('/')[0]) - 1;
                int i2 = int.Parse(coords[2].Split('/')[0]) - 1;
                polygonList.Add(new Polygon(new Vector3D(vectors[i0].X, vectors[i0].Y, vectors[i0].Z),
                                            new Vector3D(vectors[i1].X, vectors[i1].Y, vectors[i1].Z),
                                            new Vector3D(vectors[i2].X, vectors[i2].Y, vectors[i2].Z)));
            }

            return polygonList.ToArray();

        }

    }
}
