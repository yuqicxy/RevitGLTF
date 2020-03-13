using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;

namespace RevitGLTF.GLTF
{
    public partial class GLTFExportContext : IModelExportContext
    {
        public void OnLight(LightNode node)
        {
            return;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("Light:{0}  => Start", node.NodeName));
        }
        public RenderNodeAction OnPoint(PointNode node)
        {
            return RenderNodeAction.Skip;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("PointNode:{0}  => Start", node.NodeName));
            //return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnCurve(CurveNode node)
        {
            return RenderNodeAction.Skip;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("CurveNode:{0}  => Start", node.NodeName));
            //return RenderNodeAction.Proceed;
        }

        public void OnLineSegment(LineSegment segment)
        {
            return;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("LineSegment:{0}  => Start", segment.ToString()));
        }

        public RenderNodeAction OnPolyline(PolylineNode node)
        {
            return RenderNodeAction.Skip;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("PolylineNode:{0}  => Start", node.NodeName));
            //return RenderNodeAction.Proceed;
        }

        public void OnText(TextNode node)
        {
            return;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("TextNode:{0}  => Start", node.Text));
        }

        public void OnPolylineSegments(PolylineSegments segments)
        {
            return;
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info(String.Format("PolylineSegments:{0}  => Start", segments.ToString()));
        }
    }
}
