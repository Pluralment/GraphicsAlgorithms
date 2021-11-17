using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjParser.Types
{
    public class MaterialColor : IType
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }

        public MaterialColor()
        {
            this.r = 1f;
            this.g = 1f;
            this.b = 1f;
        }

        public void LoadFromStringArray(string[] data)
        {
            if (data.Length != 4) return;
            r = float.Parse(data[1], NumberStyles.Any, CultureInfo.InvariantCulture);
            g = float.Parse(data[2], NumberStyles.Any, CultureInfo.InvariantCulture);
            b = float.Parse(data[3], NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", r, g, b);
        }
    }
}
