using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;
namespace RevitGLTF.Tile3D
{
    public partial class Tile3DExportContext : IModelExportContext
    {
        public RenderNodeAction OnFaceBegin(FaceNode node)
        {
            throw new NotImplementedException();
        }

        public void OnFaceEnd(FaceNode node)
        {
            throw new NotImplementedException();
        }

        public void OnPolymesh(PolymeshTopology node)
        {
            throw new NotImplementedException();
        }
    }
}
