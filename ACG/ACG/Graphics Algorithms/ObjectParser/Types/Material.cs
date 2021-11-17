using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjParser.Types
{
    public class Material : IType
    {
        public string Name { get; set; }
        public MaterialColor AmbientReflectivity { get; set; }
        public MaterialColor DiffuseReflectivity { get; set; }
        public MaterialColor SpecularReflectivity { get; set; }
        public MaterialColor TransmissionFilter { get; set; }
        public MaterialColor EmissiveCoefficient { get; set; }
        public float SpecularExponent { get; set; }
        public float OpticalDensity { get; set; }
        public float Dissolve { get; set; }
        public float IlluminationModel { get; set; }


        public string BumpFileName { get; set; }
        public string KdFileName { get; set; }
        public string KsFileName { get; set; }

        public Material()
        {
            this.Name = "DefaultMaterial";
            this.AmbientReflectivity = new MaterialColor();
            this.DiffuseReflectivity = new MaterialColor();
            this.SpecularReflectivity = new MaterialColor();
            this.TransmissionFilter = new MaterialColor();
            this.EmissiveCoefficient = new MaterialColor();
            this.SpecularExponent = 0;
            this.OpticalDensity = 1.0f;
            this.Dissolve = 1.0f;
            this.IlluminationModel = 0;
        }

        public void LoadFromStringArray(string[] data)
        {
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("newmtl " + Name);

            b.AppendLine(string.Format("Ka {0}", AmbientReflectivity));
            b.AppendLine(string.Format("Kd {0}", DiffuseReflectivity));
            b.AppendLine(string.Format("Ks {0}", SpecularReflectivity));
            b.AppendLine(string.Format("Tf {0}", TransmissionFilter));
            b.AppendLine(string.Format("Ke {0}", EmissiveCoefficient));
            b.AppendLine(string.Format("Ns {0}", SpecularExponent));
            b.AppendLine(string.Format("Ni {0}", OpticalDensity));
            b.AppendLine(string.Format("d {0}", Dissolve));
            b.AppendLine(string.Format("illum {0}", IlluminationModel));

            return b.ToString();
        }
    }
}
