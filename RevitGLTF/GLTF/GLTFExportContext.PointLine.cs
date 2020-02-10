using Autodesk.Revit.DB;

namespace RevitGLTF
{
    public partial class GLTFExportContext:IModelExportContext
    {
        public RenderNodeAction OnCurve(CurveNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("CurveNode\t" + name);
            return RenderNodeAction.Proceed;
        }

        public void OnLineSegment(LineSegment segment)
        {
            string name = "\tid:" + segment.ToString();
            log.Info("LineSegment\t" + name);
        }

        public RenderNodeAction OnPoint(PointNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("PointNode\t" + name);
            return RenderNodeAction.Proceed;
        }

        public RenderNodeAction OnPolyline(PolylineNode node)
        {
            string name = node.NodeName + "\tid:" + node.ToString();
            log.Info("PolylineNode\t" + name);
            return RenderNodeAction.Proceed;
        }

        public void OnPolylineSegments(PolylineSegments segments)
        {
            string name = "\tid:" + segments.ToString();
            log.Info("PolylineSegments\t" + name);
        }
    }
}