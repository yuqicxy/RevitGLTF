using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGLTF
{
    public class ExportConfig
    {
        public string mOutPutPath    = "";
        public string mOutputFormat  = "";
        public string mTextureFolder = "";
        public float mScaleFactor = 1.0f;
        public bool  mOptimizeVertices = true;
    }
}
