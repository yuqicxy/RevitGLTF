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
        public void OnLight(LightNode node)
        {
           // throw new NotImplementedException();
        }
        public RenderNodeAction OnPoint(PointNode node)
        {
            return RenderNodeAction.Skip;
           // throw new NotImplementedException();
        }

        public RenderNodeAction OnCurve(CurveNode node)
        {
            return RenderNodeAction.Skip;
//            throw new NotImplementedException();
        }

        public void OnLineSegment(LineSegment segment)
        {
         //   throw new NotImplementedException();
        }

        public RenderNodeAction OnPolyline(PolylineNode node)
        {
            return RenderNodeAction.Skip;
            //throw new NotImplementedException();
        }

        public void OnText(TextNode node)
        {
            //throw new NotImplementedException();
        }

        public void OnPolylineSegments(PolylineSegments segments)
        {
            //throw new NotImplementedException();
        }
    }
}
