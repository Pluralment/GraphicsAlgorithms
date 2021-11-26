using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using GraphicsModeler.Helper;
using GraphicsModeler.Parser;
using GraphicsModeler.Scene;
using ObjParser;

namespace GraphicsModeler.Extensions
{
    public static class ObjectFileParserExtension
    {
        public static Model CreateModel(this ObjectFileParser parser, string folderName, string fileName)
        {
            var obj = new Obj();
            fileName = folderName + "\\" + fileName;
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
                var verticesList = new List<int>();
                for (var i = 0; i < face.VertexIndexList.Length; i++)
                {
                    var vertexIndex = face.VertexIndexList[i] - 1;
                    if (vertexIndex < 0) vertexIndex = face.VertexIndexList.Length - vertexIndex;
                    verticesList.Add(vertexIndex);
                }
                
                var normalsList = new List<int>();
                for (var i = 0; i < face.NormalVertexIndexList.Length; i++)
                {
                    var normalIndex = face.NormalVertexIndexList[i] - 1;
                    if (normalIndex < 0) normalIndex = face.NormalVertexIndexList.Length - normalIndex;
                    normalsList.Add(normalIndex);
                }
                
                var texturesList = new List<int>();
                for (var i = 0; i < face.TextureVertexIndexList.Length; i++)
                {
                    var textureIndex = face.TextureVertexIndexList[i] - 1;
                    if (textureIndex < 0) textureIndex = face.TextureVertexIndexList.Length - textureIndex;
                    texturesList.Add(textureIndex);
                }
                
                Polygon polygon = new Polygon()
                {
                    VerticesIndexes = verticesList,
                    NormalsIndexes = normalsList,
                    TexturesIndexes = texturesList
                };
                parser.Polygons.Add(polygon);
            }




            var mtl = new Mtl();
            List<MaterialData> materials = new List<MaterialData>();
            if (obj.Mtl != null)
            {
                mtl.LoadMtl(folderName, obj.Mtl);

                foreach (var mtlMaterial in mtl.MaterialList)
                {
                    var material = new MaterialData
                    {
                        Name = mtlMaterial.Name,
                        AmbientReflectivity = mtlMaterial.AmbientReflectivity.ToRgba(),
                        DiffuseReflectivity = mtlMaterial.DiffuseReflectivity.ToRgba(),
                        SpecularReflectivity = mtlMaterial.SpecularReflectivity.ToRgba(),
                        TransmissionFilter = mtlMaterial.TransmissionFilter.ToRgba(),
                        EmissiveCoefficient = mtlMaterial.EmissiveCoefficient.ToRgba(),
                        SpecularExponent = mtlMaterial.SpecularExponent,
                        OpticalDensity = mtlMaterial.OpticalDensity,
                        Dissolve = mtlMaterial.Dissolve
                    };

                    if (mtlMaterial.KdFileName != null)
                    {
                        material.DiffuseMap = new Texture();
                        material.DiffuseMap.Load(folderName + "\\" + mtlMaterial.KdFileName);
                    }
                    if (mtlMaterial.KsFileName != null)
                    {
                        material.SpecularMap = new Texture();
                        material.SpecularMap.Load(folderName + "\\" + mtlMaterial.KsFileName);
                    }

                    materials.Add(material);
                }
            }

            if (materials.Count == 0)
            {
                var material = new MaterialData();
                material.DiffuseMap = new Texture();
                material.DiffuseMap.Load(folderName + "\\" + "ShadowMermaid_DIFF.png");
                materials.Add(material);
            }

            return new Model(new Mesh { Vertices = parser.Vectors,Polygons = parser.Polygons })
            {
                Textures = parser.Textures,
                Normals = parser.Normals,
                Materials = materials
            };
        }
    }
}