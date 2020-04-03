using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

using BabylonExport.Entities;
using Utilities;
using KdTree;

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

            //RootNode
            List<BabylonMesh> babylonRootNodes = babylonScene.MeshesList.FindAll(node => node.parentId == null);
            if(!(babylonRootNodes.Count > 0))
                return;
            BabylonMesh root = babylonRootNodes[0];
            //Find all Element Node
            List<BabylonMesh> elementMeshes = GetDescendants(babylonScene, root);

            List<BabylonScene> scenes = new List<BabylonScene>();
            foreach(var elementMesh in elementMeshes)
            {
                List<BabylonMesh> elementMeshChilds = TraverseGetDescendants(babylonScene, elementMesh);
                List<BabylonMesh> instanceMeshes = GetInstance(babylonScene, elementMeshChilds);

                List<BabylonMesh> tmp = new List<BabylonMesh>();
                tmp.Add(root);
                tmp.Add(elementMesh);
                tmp.AddRange(elementMeshChilds);
                tmp.AddRange(instanceMeshes);
                List<BabylonMultiMaterial> multiMaterials = GetMultiMateial(babylonScene, tmp);

                var materialList = GetMaterials(babylonScene, multiMaterials);

                List<BabylonMesh> meshes = new List<BabylonMesh>();
                meshes.Add(root);
                meshes.Add(elementMesh);
                meshes.AddRange(elementMeshChilds);

                var scene = ConstuctScene(meshes, instanceMeshes, multiMaterials, materialList, exportParameters);

                scene.id = elementMesh.id;

                if (elementMesh.boundingVolume != null)
                    scene.boundingVolume = elementMesh.boundingVolume;
                else
                    scene.boundingVolume = BabylonExport.Entities.BoundingVolume.EmptyBounds;

                scenes.Add(scene);
            }

            scenes.Sort(delegate (BabylonScene left, BabylonScene right)
                        {
                            return right.boundingVolume.Volume.CompareTo(left.boundingVolume.Volume);
                        });

            KdTree.KdTree<double, BabylonScene> tree = new KdTree.KdTree<double, BabylonScene>(3, new KdTree.Math.DoubleMath());
            foreach (var scene in scenes)
            {
                List<double> keys = new List<double>();
                var bounding = scene.boundingVolume;
                keys.Add(bounding.CenterX);
                keys.Add(bounding.CenterY);
                keys.Add(bounding.CenterZ);
                tree.Add(keys.ToArray(), scene);
            }
            
            foreach(var scene in scenes)
            {
                scene.Prepare(false, false);

                Babylon2GLTF.GLTFExporter gltfExporter = new Babylon2GLTF.GLTFExporter();
                var gltfexportParameters = new BabylonExport.Entities.ExportParameters();
                gltfexportParameters.dracoCompression = exportParameters.DracoCompress;
                gltfexportParameters.optimizeVertices = true;
                gltfExporter.ExportGltf(gltfexportParameters, scene, exportParameters.TilePath, scene.id + ".gltf", false, logger);
            }

            logger.ReportProgressChanged((int)progression);
            logger.RaiseMessage("3DTileExporter | Exportation ended");
        }

        //获取babylonNode所有子节点
        private List<BabylonMesh> GetDescendants(BabylonScene babylonScene, BabylonMesh babylonNode)
        {
            List<BabylonMesh> nodelist = new List<BabylonMesh>();
            nodelist.AddRange(babylonScene.MeshesList.FindAll(node => node.parentId == babylonNode.id));
            return nodelist;
        }

        //递归遍历获取babylonNode所有子节点
        private List<BabylonMesh> TraverseGetDescendants(BabylonScene babylonScene, BabylonMesh babylonNode)
        {
            List<BabylonMesh> nodelist = GetDescendants(babylonScene, babylonNode);
            List<BabylonMesh> list = new List<BabylonMesh>();
            list.AddRange(nodelist);
            foreach (var node in nodelist)
            {
                list.AddRange(TraverseGetDescendants(babylonScene, node));
            }
            return list;
        }
        
        //获取mesh所有instance        
        private HashSet<string> GetInstance(BabylonScene scene,BabylonMesh mesh)
        {
            if (mesh.instances != null)
            {
                HashSet<string> set = new HashSet<string>();
                foreach (var instance in mesh.instances)
                {
                    set.Add(instance.id);
                }
                return set;
            }
            return null;
        }

        //递归遍历获取babylonNode所有的instance
        private List<BabylonMesh> GetInstance(BabylonScene scene, List<BabylonMesh> babylonNodes)
        {
            HashSet<string> set = new HashSet<string>();
            foreach(var node in babylonNodes)
            {
                HashSet<string> instanceIDs = GetInstance(scene, node);
                if(instanceIDs != null)
                    set.UnionWith(instanceIDs);
            }

            List<BabylonMesh> instanceList = new List<BabylonMesh>();
            instanceList = scene.InstancesList.FindAll(mesh => set.Contains(mesh.id));
            return instanceList;
        }

        private List<BabylonMultiMaterial> GetMultiMateial(BabylonScene scene,List<BabylonMesh> babylonNodes)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (var node in babylonNodes)
            {
                if(node.materialId != null)
                   set.Add(node.materialId);
            }

            var multiMaterials = scene.MultiMaterialsList.FindAll(material => set.Contains(material.id));
            return multiMaterials;
        }
        
        private List<BabylonMaterial> GetMaterials(BabylonScene scene,List<BabylonMultiMaterial> multiMaterials)
        {
            HashSet<string> materialNames = new HashSet<string>();
            foreach (var multiMaterial in multiMaterials)
            {
                foreach (var name in multiMaterial.materials)
                {
                    if(name != null)
                        materialNames.Add(name);
                }
            }
            var materials = babylonScene.MaterialsList.FindAll(mat => materialNames.Contains(mat.id));
            return materials;
        }

        public BabylonScene ConstuctScene(List<BabylonMesh> meshes, List<BabylonMesh> instanceMeshes, 
            List<BabylonMultiMaterial> multiMaterials, List<BabylonMaterial> materialList, 
            Tile3DExportParameter exportParameters)
        {
            var producer = new BabylonProducer { name = "浙江科澜信息技术有限公司——Revit导出插件", version = "1.0" };
            BabylonScene scene = new BabylonScene(exportParameters.TilePath);
            scene.producer = producer;
            scene.MeshesList.AddRange(meshes);
            scene.InstancesList.AddRange(instanceMeshes);
            scene.MultiMaterialsList.AddRange(multiMaterials);
            scene.MaterialsList.AddRange(materialList);
            return scene;
        }

        public void ExportMesh(BabylonMesh mesh,Tile3DExportParameter exportParameters, BabylonScene babylonScene, ILoggingProvider logger)
        {
            var producer = new BabylonProducer { name = "浙江科澜信息技术有限公司——Revit导出插件", version = "1.0" };
            BabylonMesh RootNode = new BabylonMesh { name = "root", id = "rootTrans" };
            RootNode.isDummy = true;
            //GLTFUtil.ExportTransform(mRootNode, Transform.Identity);
            RootNode.rotation = new float[] { (float)-Math.PI / 2, 0, 0 };

            //构造临时Scene用于导出GLTF
            BabylonScene scene = new BabylonScene(exportParameters.TilePath);

            var materialID = mesh.materialId;
            BabylonMultiMaterial multimaterial = babylonScene.MultiMaterialsList.Find(mat => mat.id == materialID);
            scene.MultiMaterialsList.Add(multimaterial);

            foreach (var name in multimaterial.materials)
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
            gltfExporter.ExportGltf(gltfexportParameters, scene, exportParameters.TilePath, mesh.id + ".gltf", false, logger);
        }
    }
}
