using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitGLTF
{
    public class ExportManager
    {
        public ExportManager()
        {
            mExportConfig = new ExportConfig();
        }

        public ExportManager(ExportConfig config)
        {
            mExportConfig = config;
        }

        public void Export(Autodesk.Revit.DB.View exportableView)
        {
            IModelExportContext context = (RevitGLTF.GLTF.GLTFExportContext)new RevitGLTF.GLTF.GLTFExportContext(mExportConfig,exportableView.Document);
            //IModelExportContext context = (RevitGLTF.Tile3D.Tile3DExportContext)new RevitGLTF.Tile3D.Tile3DExportContext(mExportConfig, exportableView.Document);
            CustomExporter exporter = new CustomExporter(exportableView.Document, context);
            exporter.IncludeGeometricObjects = true;
            exporter.ShouldStopOnError = true;
            try 
            {
                exporter.Export(exportableView);
            }
            catch(System.Exception exception)
            {
                TaskDialog dialog = new TaskDialog("exception");
                dialog.MainContent = exception.Message;
            }

        }

        private ExportConfig mExportConfig;
    }
}
