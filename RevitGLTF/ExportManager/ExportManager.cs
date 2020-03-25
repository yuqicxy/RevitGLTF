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
            BabylonExportManager babylonExporterManager = new BabylonExportManager(mExportConfig);

            IModelExportContext context = (RevitGLTF.GLTF.GLTFExportContext)new RevitGLTF.GLTF.GLTFExportContext(babylonExporterManager, exportableView/*.Document*/);
            CustomExporter exporter = new CustomExporter(exportableView.Document, context);
            exporter.IncludeGeometricObjects = true;
            exporter.ShouldStopOnError = true;
            try 
            {
                //Revit Export Pipeline
                exporter.Export(exportableView);

                //Export to GLTF or 3DTile
                babylonExporterManager.Export();
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
