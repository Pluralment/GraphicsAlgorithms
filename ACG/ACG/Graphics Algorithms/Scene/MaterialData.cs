using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsModeler.Scene
{
    public class MaterialData
    {
        public string Name { get; set; }
        public RgbaColor AmbientReflectivity { get; set; }
        public RgbaColor DiffuseReflectivity { get; set; }
        public RgbaColor SpecularReflectivity { get; set; }
        public RgbaColor TransmissionFilter { get; set; }
        public RgbaColor EmissiveCoefficient { get; set; }
        public float SpecularExponent { get; set; }
        public float OpticalDensity { get; set; }
        public float Dissolve { get; set; }
        public float IlluminationModel { get; set; }

        //public List<Texture> Textures { get; set; }
        public Texture BumpMap { get; set; }
        public Texture DiffuseMap { get; set; }
        public Texture SpecularMap { get; set; }

        public MaterialData()
        {
            Name = "DefaultMaterial";
            AmbientReflectivity = new RgbaColor();
            DiffuseReflectivity = new RgbaColor();
            SpecularReflectivity = new RgbaColor();
            TransmissionFilter = new RgbaColor();
            EmissiveCoefficient = new RgbaColor();
            SpecularExponent = 0;
            OpticalDensity = 1.0f;
            Dissolve = 1.0f;
            IlluminationModel = 0;
            BumpMap = new Texture();
            DiffuseMap = new Texture();
            SpecularMap = new Texture();
        }

        public struct RgbaColor
        {
            public float R;
            public float G;
            public float B;
            public float A;

            public static RgbaColor operator *(RgbaColor c1, RgbaColor c2)
            {
                return new RgbaColor { R = c1.R * c2.R, G = c1.G * c2.G, B = c1.B * c2.B, A = c1.A * c2.A };
            }

            public static RgbaColor operator *(RgbaColor c1, float grad)
            {
                return new RgbaColor { R = c1.R * grad, G = c1.G * grad, B = c1.B * grad, A = c1.A };
            }

            public static RgbaColor operator +(RgbaColor c1, RgbaColor c2)
            {
                return new RgbaColor { R = c1.R + c2.R, G = c1.G + c2.G, B = c1.B + c2.B, A = 1.0f };
            }
        }
    }
}
