﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

using BabylonExport.Entities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RevitGLTF
{
    public class MaterialFactory
    {
        public static BabylonMaterial CreateMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            string typeName = "Autodesk.Revit.DB.Visual." + ((AssetProperty)asset).Name.Replace("Schema", "") + ",RevitAPI";
            Type type = Type.GetType(typeName);
            if (type != null)
            {
                string schemaname = type.Name.ToLower();
                if (schemaname == "generic")
                {
                    return CreateGenericMaterial(asset, materialID);
                }
                else if(schemaname == "ceramic")
                {
                    return CreateCeramicMaterial(asset, materialID);
                }
                else if(schemaname == "concrete")
                {
                    return CreateConcreteMaterial(asset, materialID);
                }
                else if(schemaname == "glazing")
                {
                    return CreateGlazingMaterial(asset, materialID);
                }
                else if(schemaname == "hardwood")
                {
                    return CreateHardwoodMaterial(asset, materialID);
                }
                else if(schemaname == "mansorycmu")
                {
                    return CreateMansoryCMUMaterial(asset, materialID);
                }
                else if(schemaname == "metal")
                {
                    return CreateMetalMaterial(asset, materialID);
                }
                else if(schemaname == "metallicpaint")
                {
                    return CreateMetallicPaintMaterial(asset, materialID);
                }
                else if(schemaname == "mirror")
                {
                    return CreateMirrorMaterial(asset, materialID);
                }
                else if(schemaname == "plasticvinyl")
                {
                    return CreatePlasticVinylMaterial(asset, materialID);
                }
                else if(schemaname == "solidglass")
                {
                    return CreateSolidGlassMaterial(asset, materialID);
                }
                else if(schemaname == "wallpaint")
                {
                    return CreateWallPaintMaterial(asset, materialID);
                }
                else if(schemaname == "water")
                {
                    return CreateWaterMaterial(asset, materialID);
                }
                else if(schemaname == "stone")
                {
                    return CreateStoneMaterial(asset, materialID);
                }
                else
                {
                    log.Warn(schemaname + " is not handled");
                }
            }

            return null;
        }

        private static bool GetBooleanPropertyValue(Asset in_asset,string in_propertyName,bool in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            return byName == null ? in_defaultValue : (byName as AssetPropertyBoolean).Value;
        }

        private static int GetIntegerPropertyValue(Asset in_asset,string in_propertyName,int in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName == null)
                return in_defaultValue;
            return byName.Type == AssetPropertyType.Enumeration ? (byName as AssetPropertyEnum).Value : (byName as AssetPropertyInteger).Value;
        }

        private static float GetFloatPropertyValue(Asset in_asset,string in_propertyName,float in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName == null)
                return in_defaultValue;
            return byName.Type == AssetPropertyType.Float ? (byName as AssetPropertyFloat).Value : (float)(byName as AssetPropertyDouble).Value;
        }

        private static Color GetColorPropertyValue(Asset in_asset,string in_propertyName,Color in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName != null)
            {
                Color valueAsColor = (byName as AssetPropertyDoubleArray4d).GetValueAsColor();
                if (valueAsColor.IsValid)
                    return valueAsColor;
            }
            return in_defaultValue;
        }

        private static string GetTexturePropertyPath(Asset in_asset, string in_propertyName)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            AssetProperty byName1 = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName1 != null)
            {
                Asset singleConnectedAsset = byName1.GetSingleConnectedAsset();
                if (singleConnectedAsset != null)
                {
                    AssetProperty byName2 = ((AssetProperties)singleConnectedAsset).FindByName(UnifiedBitmap.UnifiedbitmapBitmap);
                    if (byName2 != null)
                    {
                        string str = (byName2 as AssetPropertyString).Value;
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str.Contains("|"))
                                str = str.Split('|')[0];
                            if (!string.IsNullOrEmpty(str))
                            {
                                if (str[0] == '1')
                                    str = "C:\\Program Files (x86)\\Common Files\\Autodesk Shared\\Materials\\Textures\\" + str;
                                if (str.Contains("Materials\\Generic\\Presets\\"))
                                    str = str.Replace("Materials\\Generic\\Presets\\", "C:\\Program Files (x86)\\Common Files\\Autodesk Shared\\Materials\\Textures\\1\\Mats\\").Replace(".jpg", ".png");
                                if (!Path.IsPathRooted(str))
                                {
                                    string path = Path.Combine("C:\\Program Files (x86)\\Common Files\\Autodesk Shared\\Materials\\Textures\\1\\Mats\\", str);
                                    if (File.Exists(path))
                                        str = path;
                                }
                                if (!Path.IsPathRooted(str))
                                {
                                    //foreach (string extraTexturePath in (IEnumerable<string>)this.ExtraTexturePaths)
                                    //{
                                    //    string path = Path.Combine(extraTexturePath, str);
                                    //    if (Path.IsPathRooted(path) && File.Exists(path))
                                    //    {
                                    //        str = path;
                                    //        break;
                                    //    }
                                    //}
                                }
                                if (!File.Exists(str))
                                    log.Warn("Warning - Material \"" + in_asset.Name + "\": Cannot find texture file " + str);
                                return str;
                            }
                        }
                    }
                }
            }
            return "";
        }

        private static float GetTexturePropertyDistance(Asset in_asset,string in_propertyName,string in_distanceName,float in_defaultValue)
        {
            AssetProperty byName1 = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName1 != null)
            {
                Asset singleConnectedAsset = byName1.GetSingleConnectedAsset();
                if (singleConnectedAsset != null)
                {
                    float num1 = 1f;
                    AssetProperty byName2 = ((AssetProperties)singleConnectedAsset).FindByName("version");
                    if (byName2 != null)
                        num1 = (byName2 as AssetPropertyInteger).Value == 4 ? 12f : 1f;
                    AssetProperty byName3 = ((AssetProperties)singleConnectedAsset).FindByName(in_distanceName);
                    if (byName3 != null)
                    {
                        double num2 = (byName3 as AssetPropertyDistance).Value;
                        return num2 == 0.0 ? in_defaultValue : (float)num2 / num1;
                    }
                }
            }
            return in_defaultValue;
        }

        private static float GetTexturePropertyAngle(Asset in_asset,string in_propertyName,string in_angleName)
        {
            AssetProperty byName1 = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName1 != null)
            {
                Asset singleConnectedAsset = byName1.GetSingleConnectedAsset();
                if (singleConnectedAsset != null)
                {
                    AssetProperty byName2 = ((AssetProperties)singleConnectedAsset).FindByName(in_angleName);
                    if (byName2 != null)
                        return (float)(byName2 as AssetPropertyDouble).Value / 360f;
                }
            }
            return 0.0f;
        }

        private static BabylonMaterial CreateGenericMaterial(Asset asset,Material material)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Generic start export");

            Color defaultColor = material.Color.IsValid ? material.Color: new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GLTFUtil.ToArray(GetColorPropertyValue(asset,
                Generic.GenericDiffuse, defaultColor));
            string diffuseTexturePath = GetTexturePropertyPath(asset, Autodesk.Revit.DB.Visual.Generic.GenericDiffuse);
            if(!string.IsNullOrEmpty(diffuseTexturePath) && File.Exists(diffuseTexturePath))
            {
                float imageFade  = GetFloatPropertyValue(asset,            Generic.GenericDiffuseImageFade, 0.0f);
                float uOffset    = GetTexturePropertyDistance(asset,       Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldOffsetX, 0.0f);
                float vOffset    = GetTexturePropertyDistance(asset,       Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldOffsetY, 0.0f);
                float uScale     = 1f / GetTexturePropertyDistance(asset,  Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldScaleX, 1f);
                float vScale     = 1f / GetTexturePropertyDistance(asset,  Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldScaleY, 1f);
                float angle      = GetTexturePropertyAngle(asset,          Generic.GenericDiffuse, UnifiedBitmap.TextureWAngle);

                BabylonTexture texture  = new BabylonTexture(material.Id.ToString() + "diffuseMap");
                texture.name = Path.GetFileNameWithoutExtension(diffuseTexturePath);
                texture.originalPath    = diffuseTexturePath;
                texture.uOffset         = uOffset;
                texture.vOffset         = vOffset;
                texture.uScale          = uScale;
                texture.vScale          = vScale;
                texture.uAng            = angle;
                
                babylonMaterial.diffuseTexture = texture;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateCeramicMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Ceramic start export");
            return null;
        }

        private static BabylonMaterial CreateConcreteMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Concrete start export");
            return null;
        }

        private static BabylonMaterial CreateGlazingMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Glazing start export");
            return null;
        }

        private static BabylonMaterial CreateHardwoodMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Hardwood start export");
            return null;
        }

        private static BabylonMaterial CreateMansoryCMUMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material MansoryCMU start export");
            return null;
        }

        private static BabylonMaterial CreateMetalMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Metal start export");
            return null;
        }

        private static BabylonMaterial CreateMetallicPaintMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material MetallicPaint start export");
            return null;
        }

        private static BabylonMaterial CreateMirrorMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Mirror start export");
            return null;
        }

        private static BabylonMaterial CreatePlasticVinylMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material PlasticVinyl start export");
            return null;
        }
        
        private static BabylonMaterial CreateSolidGlassMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material SolidGlass start export");
            return null;
        }

        private static BabylonMaterial CreateWallPaintMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material WallPaint start export");
            return null;
        }
        
        private static BabylonMaterial CreateWaterMaterial(Asset asset, Material materialID)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Water start export");
            return null;
        }

        private static BabylonMaterial CreateStoneMaterial(Asset asset, Material material)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Material Stone start export");

            Color defaultColor = material.Color.IsValid ? material.Color : new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);

            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GLTFUtil.ToArray(GetColorPropertyValue(asset,
                Generic.GenericDiffuse, defaultColor));
            string diffuseTexturePath = GetTexturePropertyPath(asset, Autodesk.Revit.DB.Visual.Generic.GenericDiffuse);
            if (!string.IsNullOrEmpty(diffuseTexturePath) && File.Exists(diffuseTexturePath))
            {
                float imageFade = GetFloatPropertyValue(asset, Generic.GenericDiffuseImageFade, 0.0f);
                float uOffset = GetTexturePropertyDistance(asset, Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldOffsetX, 0.0f);
                float vOffset = GetTexturePropertyDistance(asset, Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldOffsetY, 0.0f);
                float uScale = 1f / GetTexturePropertyDistance(asset, Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldScaleX, 1f);
                float vScale = 1f / GetTexturePropertyDistance(asset, Generic.GenericDiffuse, UnifiedBitmap.TextureRealWorldScaleY, 1f);
                float angle = GetTexturePropertyAngle(asset, Generic.GenericDiffuse, UnifiedBitmap.TextureWAngle);

                BabylonTexture texture = new BabylonTexture(material.Id.ToString() + "diffuseMap");
                texture.name = Path.GetFileNameWithoutExtension(diffuseTexturePath);
                texture.originalPath = diffuseTexturePath;
                texture.uOffset = uOffset;
                texture.vOffset = vOffset;
                texture.uScale = uScale;
                texture.vScale = vScale;
                texture.uAng = angle;

                babylonMaterial.diffuseTexture = texture;
            }

            return babylonMaterial;
        }
    }

    public partial class GLTFExportContext : IModelExportContext
    {
        private int mCurrentMaterialIndex = -1;
        
        private Dictionary<ElementId, int> mMaterialTable = new Dictionary<ElementId, int>();
        public void OnMaterial(MaterialNode node)
        {
            string name = node.NodeName + "\tid:" + node.MaterialId.ToString() + "\tmaterial:" + node.ToString();
            log.Info("MaterialNode\t" + name);

            Asset asset = null;
            if(node.HasOverriddenAppearance)
            {
                asset = node.GetAppearanceOverride();
            }
            else
            {
                asset = node.GetAppearance();
            }

            if (!mMaterialTable.ContainsKey(node.MaterialId))
            {
                var revitMaterial = mRevitDocument.GetElement(node.MaterialId) as Material;
                var material = MaterialFactory.CreateMaterial(asset, revitMaterial);
                
                mScene.MaterialsList.Add(material);

                //mCurrentMaterialIndex = mScene.MaterialsList.Count - 1;
                //mMaterialTable.Add(node.MaterialId, mCurrentMaterialIndex);
                mMaterialTable.Add(node.MaterialId, -1);
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
            else
            {
                //mCurrentMaterialIndex = mMaterialTable[node.MaterialId];
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
        }
    }
}