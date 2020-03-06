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
        public bool Start()
        {
            throw new NotImplementedException();
        }

        public void Finish()
        {
            throw new NotImplementedException();
        }

        public bool IsCanceled()
        {
            throw new NotImplementedException();
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            throw new NotImplementedException();
        }

        public void OnViewEnd(ElementId elementId)
        {
            throw new NotImplementedException();
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            throw new NotImplementedException();
        }

        public void OnElementEnd(ElementId elementId)
        {
            throw new NotImplementedException();
        }
    }
}
