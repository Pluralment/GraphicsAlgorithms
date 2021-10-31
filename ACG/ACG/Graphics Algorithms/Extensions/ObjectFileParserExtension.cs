using System.Collections.Generic;
using System.Numerics;
using GraphicsModeler.Helper;
using GraphicsModeler.Parser;
using GraphicsModeler.Scene;
using ObjParser;

namespace GraphicsModeler.Extensions
{
    public static class ObjectFileParserExtension
    {
        public static Model CreateModel(this ObjectFileParser parser, string fileName)
        {
            var obj = new Obj();
            obj.LoadObj(fileName);

            // List<Vertex> -> List<Vector4>.
            parser.Vectors = new List<Vector4>();
            foreach (var vertex in obj.VertexList)
            {
                parser.Vectors.Add(new Vector4((float)vertex.X, (float)vertex.Y, (float)vertex.Z, 1));
            }
            
            // List<Normal> -> List<Vector3>.
            parser.Normals = new List<Vector3>();
            foreach (var normal in obj.NormalList)
            {
                parser.Normals.Add(new Vector3((float)normal.X, (float)normal.Y, (float)normal.Z));
            }
            
            // List<Texture> -> List<Vector3>.
            parser.Textures = new List<Vector3>();
            foreach (var texture in obj.TextureList)
            {
                parser.Textures.Add(new Vector3((float)texture.X, (float)texture.Y, 0));
            }
            
            // List<Face> -> List<List<int>>
            foreach (var face in obj.FaceList)
            {
                var verticesList = new List<int>();
                for (var i = 0; i < face.VertexIndexList.Length; i++)
                {
                    verticesList.Add(face.VertexIndexList[i] - 1);
                }
                parser.Polygons.Add(verticesList);
            }

            return new Model(
                new Mesh
                {
                    Vertices = parser.Vectors,
                    Polygons = parser.Polygons
                })
            {
                Textures = parser.Textures,
                Normals = parser.Normals
            };
        }
    }
}