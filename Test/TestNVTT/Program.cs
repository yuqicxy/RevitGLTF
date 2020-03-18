using System;
using Nvidia.TextureTools;

namespace TestNVTT
{
    class Program
    {
        static void Main(string[] args)
        {
            //ToNormal
            String input = "E:\\RevitPlugin\\RevitGLTF\\Test\\TestNVTT\\bin\\Release\\ceiling_tile_dots_narrow_gap_taper_600x600mm_bump.jpg";
            String output = "E:\\RevitPlugin\\RevitGLTF\\Test\\TestNVTT\\bin\\Release\\ceiling_tile_dots_narrow_gap_taper_600x600mm_bump.png";
            Compressor nvttCompressor = new Compressor();
            nvttCompressor.BumpMapToNormalMap(input, output);
        }
    }
}
