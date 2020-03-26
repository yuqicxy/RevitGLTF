using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

using BabylonExport.Entities;
using Utilities;

namespace Tile3DExport.Entities
{
    public class Tile3DExportParameter
    {
        public string TilePath { get; set; }
        public bool UseGLTFBinary { get; set; } = false;

        public bool DracoCompress { get; set; } = false;

        public bool WebpCompress { get; set; } = false;
    }

    public class Tile3DExporter
    {
        private ILoggingProvider logger;
        private Tile3DExportParameter exportParameters;
        private BabylonScene babylonScene;
        public void ExportTile3D(Tile3DExportParameter exportParameters, BabylonScene babylonScene,ILoggingProvider logger)
        {
            this.exportParameters = exportParameters;
            this.logger = logger;
            this.babylonScene = babylonScene;

            logger.RaiseMessage("3DTileExporter | Exportation started");

            float progressionStep;
            var progression = 0.0f;

            //BabylonMesh rootNode = babylonScene.MeshesList.Find(mesh => mesh.parentId == null);
            //List<BabylonMesh> elements =  babylonScene.MeshesList.FindAll(mesh => mesh.parentId == rootNode.id);



            logger.ReportProgressChanged((int)progression);
            logger.RaiseMessage("3DTileExporter | Exportation ended");
        }

    }
}
