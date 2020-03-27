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

            //讲整体的Scene分解成小的Element，同时保持实例化

            //output all instance node
            var producer = new BabylonProducer { name = "浙江科澜信息技术有限公司——Revit导出插件", version = "1.0" };
            BabylonMesh RootNode = new BabylonMesh { name = "root", id = "rootTrans" };
            RootNode.isDummy = true;
            //GLTFUtil.ExportTransform(mRootNode, Transform.Identity);
            RootNode.rotation = new float[] { (float)-Math.PI / 2, 0, 0 };
            var instanceList = babylonScene.mInstanceInfo;
            foreach(var instance in instanceList)
            {
                BabylonMesh mesh;
                bool success = babylonScene.mInstanceTable.TryGetValue(instance.Key,out mesh);
                if (!success)
                    continue;

                //构造临时Scene用于导出GLTF
                BabylonScene scene = new BabylonScene(exportParameters.TilePath);

                var materialID = mesh.materialId;
                BabylonMultiMaterial multimaterial = babylonScene.MultiMaterialsList.Find(mat => mat.id == materialID);
                scene.MultiMaterialsList.Add(multimaterial);

                foreach(var name in multimaterial.materials)
                {
                    var material = babylonScene.MaterialsList.Find(mat => mat.id == name);
                    scene.MaterialsList.Add(material);
                }

                scene.producer = producer;
                scene.MeshesList.Add(RootNode);
                mesh.parentId = RootNode.id;
                scene.MeshesList.Add(mesh);

                scene.Prepare(false, false);
                Babylon2GLTF.GLTFExporter gltfExporter = new Babylon2GLTF.GLTFExporter();
                var gltfexportParameters = new BabylonExport.Entities.ExportParameters();
                gltfexportParameters.dracoCompression = exportParameters.DracoCompress;
                gltfexportParameters.optimizeVertices = true;
                gltfExporter.ExportGltf(gltfexportParameters, scene, exportParameters.TilePath,instance.Key + ".gltf" , false, logger);
                //scene.MaterialsList.Add(mesh)
            }

            logger.ReportProgressChanged((int)progression);
            logger.RaiseMessage("3DTileExporter | Exportation ended");
        }
    }
}
