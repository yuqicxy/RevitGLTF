using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGLTF
{
    public class ExportConfig
    {
        public string mOutputFilename = "defaultName";
        public string mOutPutPath    = "E:\\RevitPlugin\\RevitGLTF\\RevitGLTF\\bin\\x64\\Log";
        public string mOutputFormat  = ".gltf";
        public string mTextureFolder = "";
        public float mScaleFactor = 1.0f;
        public bool  mOptimizeVertices = true;
    }
}
