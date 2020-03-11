using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Babylon2GLTF;
using BabylonExport.Entities;
using Utilities;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

namespace RevitGLTF.GLTF
{
    public partial class GLTFExportContext : IModelExportContext
    {
        //记录上一个材质ID，用于检测材质是否变化
        private ElementId mLastMaterialID = null;

        //当前材质发生变化，在OnFaceStart之后调用
        public void OnMaterial(MaterialNode node)
        {
        #if DEBUG
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            string name = node.NodeName +"\t" + node.GetAppearance().Name  + "\tid:" + node.MaterialId.ToString() + "\tmaterial:" + node.ToString();
            log.Info("MaterialNode\t" + name);
        #endif

            var currentmesh = mMeshStack.Peek();
            var currentMyMesh = mMyMeshStack.Peek();
            if (currentmesh.materialId == null)
            {
                var currentMutiMaterial = currentMyMesh.MultiMaterial;
                //创建MultiMaterial与BabylonMesh绑定，并将其添加至Scene.MultiMaterial
                currentMutiMaterial.id = currentmesh.id;
                mExportManager.Scene.MultiMaterialsList.Add(currentMutiMaterial);
                currentmesh.materialId = currentMutiMaterial.id;
            }

            var materialId = node.MaterialId;
            //材质是否发生变化
            if (mLastMaterialID != materialId )
            {
                var materialSubMeshMap = currentMyMesh.MaterialSubMeshMap;
                var currentMaterialIndex = CreateMaterial(node);

                //根据当前材质id选择submesh
                if (materialSubMeshMap.ContainsKey(materialId))
                {
                    mCurrentSubMesh = materialSubMeshMap[materialId];
                }
                else
                {
                    //创建submesh,并添加至matrialSubMeshMap
                    mCurrentSubMesh = new MySubMesh();
                    mCurrentSubMesh.MaterialID = currentMaterialIndex;
                    //mCurrentSubMesh.NeedUV = MaterialFactory.Instance.NeedUVCoord(materialId);
                    materialSubMeshMap.Add(materialId, mCurrentSubMesh);
                }
                mLastMaterialID = materialId;
            }
        }

        //创建材质，添加至multiMaterial,并更新当前材质ID
        public int CreateMaterial(MaterialNode node)
        {
            var currentMutiMaterial = mMyMeshStack.Peek().MultiMaterial;

            //记录材质在MultiMaterial中的索引
            int currentMaterialIndex = -1;

            if (!MaterialFactory.Instance.HasCreatedMaterial(node.MaterialId))
            {
                Asset asset = null;
                if (node.HasOverriddenAppearance)
                    asset = node.GetAppearanceOverride();
                else
                    asset = node.GetAppearance();

                var revitMaterial = mRevitDocument.GetElement(node.MaterialId) as Material;
                //创建材质
                var babylonmaterial = MaterialFactory.Instance.CreateMaterial(asset, revitMaterial,node.MaterialId);
                //将材质添加至Scene
                mExportManager.Scene.MaterialsList.Add(babylonmaterial);
                
                //添加材质索引至MultiMaterial，并更新Multimaterial内当前局部材质索引
                var materials = currentMutiMaterial.materials.ToList();
                materials.Add(node.MaterialId.ToString());
                currentMaterialIndex = materials.Count() - 1;
                currentMutiMaterial.materials = materials.ToArray();
            }
            else
            {
                var materials = currentMutiMaterial.materials.ToList();
                //查看当前multimaterial是否包含该id
                if (!materials.Contains(node.MaterialId.ToString()))
                {
                    //添加材质索引至MultiMaterial，并更新Multimaterial内当前局部材质索引
                    materials.Add(node.MaterialId.ToString());
                    currentMaterialIndex = materials.Count() - 1;
                    currentMutiMaterial.materials = materials.ToArray();
                }
                else
                {
                    //更新Multimaterial内当前局部材质索引
                    currentMaterialIndex = materials.FindIndex(_str => _str == node.MaterialId.ToString());
                }
            }

            return currentMaterialIndex;
        }
    }
}
