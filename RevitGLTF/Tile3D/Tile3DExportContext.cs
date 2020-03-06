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
        public bool Cancel { get; set; } = false;
        private Document mRevitDocument = null;
        private ExportConfig mConfig = null;

        public Tile3DExportContext(ExportConfig config, Document document)
        {
            mConfig = config;
            mRevitDocument = document;
        }

        public bool Start()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("Exporting {0} => Start", mRevitDocument.Title));
            return true;
        }

        public void Finish()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("Exporting {0} => Finish", mRevitDocument.Title));
        }

        public bool IsCanceled()
        {
            if(Cancel)
                return true;
            return false;
        }

        public RenderNodeAction OnViewBegin(ViewNode node)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("View3D {0} => Start", node.NodeName));
            return RenderNodeAction.Proceed;
        }

        public void OnViewEnd(ElementId elementId)
        {
            var view = mRevitDocument.GetElement(elementId) as View;
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("View3D {0} => Finish", view.Name));
        }

        public RenderNodeAction OnElementBegin(ElementId elementId)
        {
            var element = mRevitDocument.GetElement(elementId);
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("Element:{0} ID:{1} => Start", element.Name,elementId.ToString()));
            return RenderNodeAction.Proceed;
        }

        public void OnElementEnd(ElementId elementId)
        {
            var element = mRevitDocument.GetElement(elementId);
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info(String.Format("Element:{0} ID:{1} => Finish", element.Name, elementId.ToString()));
        }
    }
}
