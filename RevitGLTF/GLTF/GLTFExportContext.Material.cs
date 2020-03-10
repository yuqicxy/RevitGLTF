using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

using BabylonExport.Entities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RevitGLTF
{
    public partial class GLTFExportContext : IModelExportContext
    {
        private int mCurrentMaterialIndex = -1;
        
        //private Dictionary<int, BabylonMaterial> mMaterialTable = new Dictionary<int, BabylonMaterial>();

        private int mCurrentMaterialID = -1;

        public void OnMaterial(MaterialNode node)
        {
            CreateMaterial(node);

            if(mCurrentMaterialID != node.MaterialId.IntegerValue)
            {
                mCurrentMaterialID = node.MaterialId.IntegerValue;
                if (mCurrentSubMesh != null)
                    OnSubmeshEnd();
                OnSubmeshStart(mCurrentMaterialID);
                mCurrentMaterialID = node.MaterialId.IntegerValue;
            }
        }

        public void CreateMaterial(MaterialNode node)
        {
            Asset asset = null;
            if (node.HasOverriddenAppearance)
            {
                asset = node.GetAppearanceOverride();
            }
            else
            {
                asset = node.GetAppearance();
            }

            var revitMaterial = mRevitDocument.GetElement(node.MaterialId) as Material;

            if (!MaterialFactory.Instance.HasCreatedMaterial(node.MaterialId))
            {
                var babylonmaterial = MaterialFactory.Instance.CreateMaterial(asset, revitMaterial);
                mScene.MaterialsList.Add(babylonmaterial);

                if (mCurrentMutiMaterial.materials == null)
                {
                    List<String> materials = new List<String>();
                    materials.Add(node.MaterialId.ToString());
                    mCurrentMaterialIndex = materials.Count() - 1;
                    mCurrentMutiMaterial.materials = materials.ToArray();
                }
                else
                {
                    var materials = mCurrentMutiMaterial.materials.ToList();
                    materials.Add(node.MaterialId.ToString());
                    mCurrentMaterialIndex = materials.Count() - 1;
                    mCurrentMutiMaterial.materials = materials.ToArray();
                }
            }
            else
            {
                if (mCurrentMutiMaterial.materials == null)
                {
                    List<String> materials = new List<String>();
                    materials.Add(node.MaterialId.ToString());
                    mCurrentMaterialIndex = materials.Count() - 1;
                    mCurrentMutiMaterial.materials = materials.ToArray();
                }
                else
                {
                    var materials = mCurrentMutiMaterial.materials.ToList();
                    if (!materials.Contains(node.MaterialId.ToString()))
                    {
                        materials.Add(node.MaterialId.ToString());
                        mCurrentMaterialIndex = materials.Count() - 1;
                        mCurrentMutiMaterial.materials = materials.ToArray();
                    }
                    else
                    {
                        mCurrentMaterialIndex = materials.FindIndex(_str => _str == node.MaterialId.ToString());
                    }
                }
            }

            //if (!mMaterialTable.ContainsKey(node.MaterialId.IntegerValue))
            //{
            //    var revitMaterial = mRevitDocument.GetElement(node.MaterialId) as Material;
            //    try
            //    {
            //        var material = MaterialFactory.CreateMaterial(asset, revitMaterial);

            //        if (material == null)
            //        {
            //            material = new BabylonStandardMaterial(node.MaterialId.ToString());
            //            material.name = node.NodeName;
            //        }

            //        mScene.MaterialsList.Add(material);
            //        mMaterialTable.Add(node.MaterialId.IntegerValue, material);
            //    }
            //    catch (Exception e)
            //    {
            //        log.Error(e.Message);
            //        log.Error(e.StackTrace);
            //        return;
            //    }

            //    if (mCurrentMutiMaterial.materials == null)
            //    {
            //        List<String> materials = new List<String>();
            //        materials.Add(node.MaterialId.ToString());
            //        mCurrentMaterialIndex = materials.Count() - 1;
            //        mCurrentMutiMaterial.materials = materials.ToArray();
            //    }
            //    else
            //    {
            //        var materials = mCurrentMutiMaterial.materials.ToList();
            //        materials.Add(node.MaterialId.ToString());
            //        mCurrentMaterialIndex = materials.Count() - 1;
            //        mCurrentMutiMaterial.materials = materials.ToArray();
            //    }
            //}
            //else
            //{
            //    if (mCurrentMutiMaterial.materials == null)
            //    {
            //        List<String> materials = new List<String>();
            //        materials.Add(node.MaterialId.ToString());
            //        mCurrentMaterialIndex = materials.Count() - 1;
            //        mCurrentMutiMaterial.materials = materials.ToArray();
            //    }
            //    else
            //    {                    
            //        var materials = mCurrentMutiMaterial.materials.ToList();
            //        if (!materials.Contains(node.MaterialId.ToString()))
            //        {
            //            materials.Add(node.MaterialId.ToString());
            //            mCurrentMaterialIndex = materials.Count() - 1;
            //            mCurrentMutiMaterial.materials = materials.ToArray();
            //        }
            //        else
            //        {
            //            mCurrentMaterialIndex = materials.FindIndex(_str => _str == node.MaterialId.ToString());
            //        }
            //    }
            //}
        }
    }
}