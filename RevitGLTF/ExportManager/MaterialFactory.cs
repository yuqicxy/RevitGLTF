using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

using BabylonExport.Entities;

namespace RevitGLTF
{
    public sealed class MaterialFactory
    {
        private static readonly Lazy<MaterialFactory> lazy = new Lazy<MaterialFactory>(() => new MaterialFactory());

        public static MaterialFactory Instance
        {
            get { return lazy.Value; }
        }

        private Dictionary<ElementId, BabylonMaterial> mMaterialTable = null;

        private Dictionary<ElementId, bool> mMaterialNeedUV = null;

        private MaterialFactory()
        {
            mMaterialTable = new Dictionary<ElementId, BabylonMaterial>();
            mMaterialNeedUV = new Dictionary<ElementId, bool>();
        }

        public void Clear() { mMaterialTable.Clear(); mMaterialNeedUV.Clear(); }

        //检测材质是否以创建
        public bool HasCreatedMaterial(ElementId MaterialId)
        {
            return mMaterialTable.ContainsKey(MaterialId);
        }

        //检测材质是否有纹理以及是否需要UV坐标
        public bool NeedUVCoord(ElementId MaterialId)
        {
            if(mMaterialNeedUV.ContainsKey(MaterialId))
                return mMaterialNeedUV[MaterialId];
            //默认返回True，也即需要UV坐标
            return true;
        }

        //创建或获取材质
        public BabylonMaterial CreateMaterial(Asset asset, Material revitMaterial, ElementId materialID = null)
        {

            BabylonMaterial material = null;
            ElementId id = null;
            if (revitMaterial != null)
            {
                id = revitMaterial.Id;
                if (HasCreatedMaterial(id))
                {
                    return mMaterialTable[id];
                }

                string typeName = "Autodesk.Revit.DB.Visual." + ((AssetProperty)asset).Name.Replace("Schema", "") + ",RevitAPI";
                Type type = Type.GetType(typeName);
                if (type != null)
                {
                    string schemaname = type.Name.ToLower();
                    if (schemaname == "generic")
                    {
                        material = CreateGenericMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "ceramic")
                    {
                        material = CreateCeramicMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "concrete")
                    {
                        material = CreateConcreteMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "glazing")
                    {
                        material = CreateGlazingMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "hardwood")
                    {
                        material = CreateHardwoodMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "mansorycmu")
                    {
                        material = CreateMansoryCMUMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "metal")
                    {
                        material = CreateMetalMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "metallicpaint")
                    {
                        material = CreateMetallicPaintMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "mirror")
                    {
                        material = CreateMirrorMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "plasticvinyl")
                    {
                        material = CreatePlasticVinylMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "solidglass")
                    {
                        material = CreateSolidGlassMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "wallpaint")
                    {
                        material = CreateWallPaintMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "water")
                    {
                        material = CreateWaterMaterial(asset, revitMaterial);
                    }
                    else if (schemaname == "stone")
                    {
                        material = CreateStoneMaterial(asset, revitMaterial);
                    }
                    else
                    {
                    #if DEBUG
                        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                        log.Warn(schemaname + " is not handled");
                    #endif
                    }
                }
            }

            if(material == null)
            {
                id = materialID;
                material = new BabylonStandardMaterial(id.ToString());
                //默认材质，关闭背面裁剪
                material.backFaceCulling = false;
            }

            material.name = id.ToString();
            bool needUV = material.hasTexture;
            mMaterialTable.Add(id, material);
            mMaterialNeedUV.Add(id, needUV);
            return material;
        }

        private static bool GetBooleanPropertyValue(Asset in_asset, string in_propertyName, bool in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            return (byName == null || (byName as AssetPropertyBoolean == null)) ? in_defaultValue : (byName as AssetPropertyBoolean).Value;
        }

        private static int GetIntegerPropertyValue(Asset in_asset, string in_propertyName, int in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName == null)
                return in_defaultValue;
            return byName.Type == AssetPropertyType.Enumeration ? (byName as AssetPropertyEnum).Value : (byName as AssetPropertyInteger).Value;
        }

        private static float GetFloatPropertyValue(Asset in_asset, string in_propertyName, float in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName == null)
                return in_defaultValue;
            return byName.Type == AssetPropertyType.Float ? (byName as AssetPropertyFloat).Value : (float)(byName as AssetPropertyDouble).Value;
        }

        private static IList<float> GetColorPropertyValue(Asset in_asset, string in_propertyName, IList<float> in_defaultValue)
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName as AssetPropertyDoubleArray4d != null)
            {
                var valueAsColor = (byName as AssetPropertyDoubleArray4d).GetValueAsDoubles();

                return valueAsColor.Select(x => (float)x).ToList();
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
                                    //外部纹理路径中查找纹理
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

        private static float GetTexturePropertyDistance(Asset in_asset, string in_propertyName, string in_distanceName, float in_defaultValue)
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

        private static float GetTexturePropertyAngle(Asset in_asset, string in_propertyName, string in_angleName)
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

        private static BabylonTexture CreateUnifiedBitmapTexture(Asset asset, Material material, String propertyName)
        {
            BabylonTexture texture = null;
            string texturePath = GetTexturePropertyPath(asset, propertyName);
            if (!string.IsNullOrEmpty(texturePath) && File.Exists(texturePath))
            {
                float uOffset = GetTexturePropertyDistance(asset, propertyName, UnifiedBitmap.TextureRealWorldOffsetX, 0.0f);
                float vOffset = GetTexturePropertyDistance(asset, propertyName, UnifiedBitmap.TextureRealWorldOffsetY, 0.0f);
                float uScale = 1f / GetTexturePropertyDistance(asset, propertyName, UnifiedBitmap.TextureRealWorldScaleX, 1f);
                float vScale = 1f / GetTexturePropertyDistance(asset, propertyName, UnifiedBitmap.TextureRealWorldScaleY, 1f);
                float angle = GetTexturePropertyAngle(asset, propertyName, UnifiedBitmap.TextureWAngle);

                texture = new BabylonTexture(material.Id.ToString() + "diffuseMap");
                texture.name = Path.GetFileNameWithoutExtension(texturePath);
                texture.originalPath = texturePath;
                texture.uOffset = uOffset;
                texture.vOffset = vOffset;
                texture.uScale = uScale;
                texture.vScale = vScale;
                texture.uAng = angle;
            }
            return texture;
        }

        private static BabylonMaterial CreateGenericMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Generic start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);

            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Generic.GenericDiffuse, defaultVal).ToArray();

            var GenericDiffuseTexture = CreateUnifiedBitmapTexture(asset, material, Generic.GenericDiffuse);
            if (GenericDiffuseTexture != null)
            {
                babylonMaterial.diffuseTexture = GenericDiffuseTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateCeramicMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Ceramic start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Ceramic.CeramicColor, defaultVal).ToArray();

            var CeramicColorTexture = CreateUnifiedBitmapTexture(asset, material, Ceramic.CeramicColor);
            if (CeramicColorTexture != null)
            {
                babylonMaterial.diffuseTexture = CeramicColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateConcreteMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Concrete start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Concrete.ConcreteColor, defaultVal).ToArray();

            var ConcreteColorTexture = CreateUnifiedBitmapTexture(asset, material, Concrete.ConcreteColor);
            if (ConcreteColorTexture != null)
            {
                babylonMaterial.diffuseTexture = ConcreteColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateGlazingMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Glazing start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Glazing.GlazingTransmittanceMap, defaultVal).ToArray();

            var GlazingTransmittanceMapTexture = CreateUnifiedBitmapTexture(asset, material, Glazing.GlazingTransmittanceMap);
            if (GlazingTransmittanceMapTexture != null)
            {
                babylonMaterial.diffuseTexture = GlazingTransmittanceMapTexture;
                babylonMaterial.hasTexture = true;
            }

            babylonMaterial.alpha = 0.5f;
            babylonMaterial.transparencyMode = (int)BabylonPBRMetallicRoughnessMaterial.TransparencyMode.ALPHABLEND;

            return babylonMaterial;
        }

        private static BabylonMaterial CreateHardwoodMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Hardwood start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Hardwood.HardwoodColor, defaultVal).ToArray();

            var HardwoodColorTexture = CreateUnifiedBitmapTexture(asset, material, Hardwood.HardwoodColor);
            if (HardwoodColorTexture != null)
            {
                babylonMaterial.diffuseTexture = HardwoodColorTexture;
                babylonMaterial.hasTexture = true;
            }

            //int tintEnable = GetIntegerPropertyValue(asset, Hardwood.HardwoodTintEnabled, 0);
            //if(tintEnable > 0)
            //{
            //    babylonMaterial.ambient = GLTFUtil.ToArray(GetColorPropertyValue(asset, Hardwood.HardwoodTintColor,defaultColor));
            //}
            int tintToggle = GetIntegerPropertyValue(asset, Hardwood.HardwoodTintEnabled, 0);
            if (tintToggle > 0)
            {
                var color = GetColorPropertyValue(asset, Hardwood.HardwoodTintColor, defaultVal);
                babylonMaterial.emissive = new float[] { color[0], color[1], color[2] };
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateMansoryCMUMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material MansoryCMU start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, MasonryCMU.MasonryCMUColor, defaultVal).ToArray();

            var MasonryCMUColorTexture = CreateUnifiedBitmapTexture(asset, material, MasonryCMU.MasonryCMUColor);
            if (MasonryCMUColorTexture != null)
            {
                babylonMaterial.diffuseTexture = MasonryCMUColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateMetalMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Metal start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());
            babylonMaterial.baseColor = GetColorPropertyValue(asset, Metal.MetalColor, defaultVal).ToArray();

            var MetalColorTexture = CreateUnifiedBitmapTexture(asset, material, Metal.MetalColor);
            if (MetalColorTexture != null)
            {
                babylonMaterial.baseTexture = MetalColorTexture;
                babylonMaterial.hasTexture = true;
            }

            babylonMaterial.metallic = 1.0f;

            return babylonMaterial;
        }

        private static BabylonMaterial CreateMetallicPaintMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material MetallicPaint start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, MetallicPaint.MetallicpaintBaseColor, defaultVal).ToArray();

            var MetallicpaintBaseColorTexture = CreateUnifiedBitmapTexture(asset, material, MetallicPaint.MetallicpaintBaseColor);
            if (MetallicpaintBaseColorTexture != null)
            {
                babylonMaterial.diffuseTexture = MetallicpaintBaseColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateMirrorMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Mirror start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Mirror.MirrorTintcolor, defaultVal).ToArray();

            var MirrorTintcolorTexture = CreateUnifiedBitmapTexture(asset, material, Mirror.MirrorTintcolor);
            if (MirrorTintcolorTexture != null)
            {
                babylonMaterial.diffuseTexture = MirrorTintcolorTexture;
                babylonMaterial.hasTexture = true;
            }
            return babylonMaterial;
        }

        private static BabylonMaterial CreatePlasticVinylMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material PlasticVinyl start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, PlasticVinyl.PlasticvinylColor, defaultVal).ToArray();

            var PlasticvinylColorTexture = CreateUnifiedBitmapTexture(asset, material, PlasticVinyl.PlasticvinylColor);
            if (PlasticvinylColorTexture != null)
            {
                babylonMaterial.diffuseTexture = PlasticvinylColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateSolidGlassMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material SolidGlass start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, SolidGlass.SolidglassTransmittanceCustomColor, defaultVal).ToArray();
            babylonMaterial.alpha = 0.5f;
            babylonMaterial.transparencyMode = (int)BabylonPBRMetallicRoughnessMaterial.TransparencyMode.ALPHABLEND;
            //babylonMaterial.specularPower = GetFloatPropertyValue(asset, SolidGlass.SolidglassGlossiness, 0) * 256.0f;

            var SolidglassTransmittanceCustomColorTexture = CreateUnifiedBitmapTexture(asset, material, SolidGlass.SolidglassTransmittanceCustomColor);
            if (SolidglassTransmittanceCustomColorTexture != null)
            {
                babylonMaterial.diffuseTexture = SolidglassTransmittanceCustomColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateWallPaintMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material WallPaint start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, WallPaint.WallpaintColor, defaultVal).ToArray();

            var WallpaintColorTexture = CreateUnifiedBitmapTexture(asset, material, WallPaint.WallpaintColor);
            if (WallpaintColorTexture != null)
            {
                babylonMaterial.diffuseTexture = WallpaintColorTexture;
                babylonMaterial.hasTexture = true;
            }
            return babylonMaterial;
        }

        private static BabylonMaterial CreateWaterMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Water start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Water.WaterTintColor, defaultVal).ToArray();

            var WaterTintColorTexture = CreateUnifiedBitmapTexture(asset, material, Water.WaterTintColor);
            if (WaterTintColorTexture != null)
            {
                babylonMaterial.diffuseTexture = WaterTintColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateStoneMaterial(Asset asset, Material material)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //log.Info("Material Stone start export");

            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());
            babylonMaterial.diffuse = GetColorPropertyValue(asset, Stone.StoneColor, defaultVal).ToArray();

            var StoneColorTexture = CreateUnifiedBitmapTexture(asset, material, Stone.StoneColor);
            if (StoneColorTexture != null)
            {
                babylonMaterial.diffuseTexture = StoneColorTexture;
                babylonMaterial.hasTexture = true;
            }
            return babylonMaterial;
        }
    }
}
