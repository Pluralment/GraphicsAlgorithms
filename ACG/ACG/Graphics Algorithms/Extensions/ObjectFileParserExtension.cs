using System.Collections.Generic;
using System.Numerics;
using GraphicsModeler.Helper;
using GraphicsModeler.Parser;
using ObjParser;

namespace GraphicsModeler.MainWindow.Extensions
{
    public static class ObjectFileParserExtension
    {
        public static Model CreateModel(this ObjectFileParser parser, string fileName)
        {
            var obj = new Obj();
            obj.LoadObj(@"C:\Users\KIRILL\Desktop\bag_low.OBJ");

            parser.Vectors = new List<Vector4>();
            // List<Vertex> -> List<Vector4>.
            foreach (var vertex in obj.VertexList)
            {
                parser.Vectors.Add(new Vector4((float)vertex.X, (float)vertex.Y, (float)vertex.Z, 1));
            }
            
            // List<Face> -> List<List<int>>
            foreach (var face in obj.FaceList)
            {
                var vertList = new List<int>();
                for (var i = 0; i < face.VertexIndexList.Length; i++)
                {
                    vertList.Add(face.VertexIndexList[i] - 1);
                }
                parser.Polygons.Add(vertList);
            }

            return new Model(parser.Polygons, parser.Vectors);
        }
    }
}