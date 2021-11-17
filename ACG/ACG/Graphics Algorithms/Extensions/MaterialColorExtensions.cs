using ObjParser.Types;
using static GraphicsModeler.Scene.MaterialData;

namespace GraphicsModeler.Extensions
{
    public static class MaterialColorExtensions
    {
        public static RgbaColor ToRgba(this MaterialColor materialColor)
        {
            return new RgbaColor{ R = materialColor.r, G = materialColor.g, B = materialColor.b };
        }
    }
}
