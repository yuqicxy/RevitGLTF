using System;
using Nvidia.TextureTools;

namespace TestNVTT
{
    class Program
    {
        static void Main(string[] args)
        {
            //ToNormal
            String input = "F:\\RevitPlugin\\RevitGLTF\\Test\\TestNVTT\\bin\\Release\\netcoreapp3.1\\brick_bump.jpg";
            String output = "F:\\RevitPlugin\\RevitGLTF\\Test\\TestNVTT\\bin\\Release\\netcoreapp3.1\\brick_bump.png";
            Compressor nvttCompressor = new Compressor();
            nvttCompressor.BumpMapToNormalMap(input, output);
        }
    }
}
