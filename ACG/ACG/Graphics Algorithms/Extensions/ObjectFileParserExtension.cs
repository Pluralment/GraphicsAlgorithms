using System.Collections.Generic;
using System.Linq;
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
            parser.Vectors = new List<Vector3>();
            foreach (var vertex in obj.VertexList)
            {
                parser.Vectors.Add(new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z));
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
            
            // List<Face> -> List<Polygon>
            foreach (var face in obj.FaceList)
            {
                /*var verticesList = new List<int>();
                for (var i = 0; i < face.VertexIndexList.Length; i++)
                {
                    verticesList.Add(face.VertexIndexList[i] - 1);
                }
                parser.Polygons.Add(verticesList);*/
                var verticesList = new List<int>();
                for (var i = 0; i < face.VertexIndexList.Length; i++)
                {
                    verticesList.Add(face.VertexIndexList[i] - 1);
                }
                
                var normalsList = new List<int>();
                for (var i = 0; i < face.NormalVertexIndexList.Length; i++)
                {
                    normalsList.Add(face.NormalVertexIndexList[i] - 1);
                }
                
                var texturesList = new List<int>();
                for (var i = 0; i < face.TextureVertexIndexList.Length; i++)
                {
                    texturesList.Add(face.TextureVertexIndexList[i] - 1);
                }
                
                Polygon polygon = new Polygon()
                {
                    VerticesIndexes = verticesList,
                    NormalsIndexes = normalsList,
                    TexturesIndexes = texturesList
                };
                parser.Polygons.Add(polygon);
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