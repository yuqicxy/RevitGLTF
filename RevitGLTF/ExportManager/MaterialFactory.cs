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

                string schemaname = GetAssetSchemaName(asset);
                if(schemaname != null)
                {
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
                    else if (schemaname == "masonrycmu")
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
                        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                        log.Error(schemaname + " is not handled");
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

        private static string GetAssetSchemaName(Asset asset)
        {
            string typeName = "Autodesk.Revit.DB.Visual." + ((AssetProperty)asset).Name.Replace("Schema", "") + ",RevitAPI";
            Type type = Type.GetType(typeName);
            if(type != null)
            {
                string schemaname = type.Name.ToLower();
                return schemaname;
            }

            //对于Revit老版本生产的bim，存在上述方式分类方式不起效的情况，
            //此时则需要通过各Schema类型特有字段来识别
            if (HasProperty(asset, Ceramic.CeramicColor))
                return "ceramic";
            if(HasProperty(asset, Concrete.ConcreteColor))
                return "concrete";
            if (HasProperty(asset, Glazing.GlazingTransmittanceColor))
                return "glazing";
            if (HasProperty(asset, Hardwood.HardwoodColor))
                return "hardwood";
            if (HasProperty(asset, MasonryCMU.MasonryCMUColor))
                return "masonrycmu";
            if (HasProperty(asset, Metal.MetalColor))
                return "metal";
            if (HasProperty(asset, MetallicPaint.MetallicpaintBaseColor))
                return "metallicpaint";
            if (HasProperty(asset, Mirror.MirrorColorByObject))
                return "mirror";
            if (HasProperty(asset, PlasticVinyl.PlasticvinylColor))
                return "plasticvinyl";
            if (HasProperty(asset, SolidGlass.SolidglassReflectance))
                return "solidglass";
            if (HasProperty(asset, WallPaint.WallpaintColor))
                return "wallpaint";
            if (HasProperty(asset, Water.WaterTintColor))
                return "water";
            if (HasProperty(asset, Stone.StoneColor))
                return "stone";
            if (HasProperty(asset, Generic.GenericDiffuse))
                return "generic";
            return null;
        }

        private static bool HasProperty(Asset in_asset,string in_proprtyName)
        {
            AssetProperty byName = in_asset.FindByName(in_proprtyName);
            if (byName != null)
                return true;
            return false;
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

        private static string GetStringPropertyValue(Asset in_asset,string in_propertyName,string in_defaultValue = "")
        {
            AssetProperty byName = ((AssetProperties)in_asset).FindByName(in_propertyName);
            if (byName == null)
                return in_defaultValue;
            return byName.Type == AssetPropertyType.String ? (byName as AssetPropertyString).Value : (string)(byName as AssetPropertyString).Value;
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

        private static BabylonTexture GetBumpTexture(Asset in_asset, Material material, string in_propertyName)
        {
            BabylonTexture texture = null;
            string texturePath = GetTexturePropertyPath(in_asset, in_propertyName);
            if (!string.IsNullOrEmpty(texturePath) && File.Exists(texturePath))
            {
                float uOffset = GetTexturePropertyDistance(in_asset, in_propertyName, UnifiedBitmap.TextureRealWorldOffsetX, 0.0f);
                float vOffset = GetTexturePropertyDistance(in_asset, in_propertyName, UnifiedBitmap.TextureRealWorldOffsetY, 0.0f);
                float uScale = 1f / GetTexturePropertyDistance(in_asset, in_propertyName, UnifiedBitmap.TextureRealWorldScaleX, 1f);
                float vScale = 1f / GetTexturePropertyDistance(in_asset, in_propertyName, UnifiedBitmap.TextureRealWorldScaleY, 1f);
                float angle = GetTexturePropertyAngle(in_asset, in_propertyName, UnifiedBitmap.TextureWAngle);

                texture = new BabylonTexture(material.Id.ToString() + "bumpMap");
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

        private static BabylonTexture CreateUnifiedBitmapTexture(Asset asset,String propertyName,bool isBump = false)
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

                string name;
                if (isBump)
                {
                    name = Path.GetFileNameWithoutExtension(texturePath) + "_bump" + Path.GetExtension(texturePath);
                    name = name.ToLower();
                }
                else
                {
                    name = Path.GetFileName(texturePath).ToLower();
                }
                texture = new BabylonTexture(name);
                texture.name = name;
                texture.originalPath = texturePath;
                texture.uOffset = uOffset;
                texture.vOffset = vOffset;
                texture.uScale = uScale;
                texture.vScale = vScale;
                texture.uAng = angle;
            }
            return texture;
        }

        //一般材质
        private static BabylonMaterial CreateGenericMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);

            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            var babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, Generic.CommonTintToggle, false);
            if(commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Generic.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Generic.GenericDiffuse, defaultVal).ToList();
            babylonMaterial.baseColor = defaultVal.ToArray();

            var isMetal = GetBooleanPropertyValue(asset, Generic.GenericIsMetal, false);
            if (isMetal)
                babylonMaterial.metallic = 0.6f;
            
            var GenericDiffuseTexture = CreateUnifiedBitmapTexture(asset,Generic.GenericDiffuse);
            if (GenericDiffuseTexture != null)
            {
                babylonMaterial.baseTexture = GenericDiffuseTexture;
                babylonMaterial.hasTexture = true;
            }
            
            var BumpTexture = CreateUnifiedBitmapTexture(asset, Generic.GenericBumpMap,true);
            if (BumpTexture != null)
            {
                BumpTexture.IsBump = true;
                babylonMaterial.normalTexture = BumpTexture;
                babylonMaterial.hasTexture = true;
            }

            //var glossiness = GetFloatPropertyValue(asset, Generic.GenericGlossiness, 0.0f);
            //babylonMaterial.roughness = glossiness;

            var transparency = 1-GetFloatPropertyValue(asset, Generic.GenericTransparency, 1.0f);
            if (Math.Abs(transparency - 1) > 1e-2)
            {
                babylonMaterial.alpha = transparency;
                babylonMaterial.transparencyMode = (int)BabylonPBRMetallicRoughnessMaterial.TransparencyMode.ALPHABLEND;
            }
            return babylonMaterial;
        }

        //陶瓷
        private static BabylonMaterial CreateCeramicMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, Ceramic.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Ceramic.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Ceramic.CeramicColor, defaultVal).ToList();
            
            babylonMaterial.baseColor = defaultVal.ToArray();

            var CeramicColorTexture = CreateUnifiedBitmapTexture(asset,Ceramic.CeramicColor);
            if (CeramicColorTexture != null)
            {
                //CeramicColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = CeramicColorTexture;
                babylonMaterial.hasTexture = true;
            }

            int ceramicType = GetIntegerPropertyValue(asset, Ceramic.CeramicType,0);
            switch(ceramicType)
            {
                //陶瓷
                case (int)CeramicType.Ceramic:
                    {
                        babylonMaterial.roughness = 0.5f;
                        break;
                    }
                //瓦罐
                case (int)CeramicType.Porcelain:
                    {
                        break;
                    }
            }

            var BumpTexture = CreateUnifiedBitmapTexture(asset, Ceramic.CeramicBumpMap,true);
            if (BumpTexture != null)
            {
                //BumpTexture.name = material.Id.ToString() + "_bump";
                BumpTexture.IsBump = true;
                babylonMaterial.normalTexture = BumpTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        //混凝土
        private static BabylonMaterial CreateConcreteMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };
            
            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, Concrete.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Concrete.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Concrete.ConcreteColor, defaultVal).ToList();
            babylonMaterial.baseColor = defaultVal.ToArray();

            var ConcreteColorTexture = CreateUnifiedBitmapTexture(asset, Concrete.ConcreteColor);
            if (ConcreteColorTexture != null)
            {
                //ConcreteColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = ConcreteColorTexture;
                babylonMaterial.hasTexture = true;
            }

            var BumpTexture = CreateUnifiedBitmapTexture(asset, Concrete.ConcreteBumpMap,true);
            if (BumpTexture != null)
            {
                //BumpTexture.name = material.Id.ToString() + "_bump";
                BumpTexture.IsBump = true;
                babylonMaterial.normalTexture = BumpTexture;
                babylonMaterial.hasTexture = true;
            }

            int concreteSealantType = GetIntegerPropertyValue(asset, Concrete.ConcreteSealant, 0);
            switch(concreteSealantType)
            {
                case (int)ConcreteSealantType.None:
                    {
                        break;
                    }
                case (int)ConcreteSealantType.Epoxy:
                case (int)ConcreteSealantType.Acrylic:
                    {
                        babylonMaterial.roughness = 0.8f;
                        break;
                    }
            }

            return babylonMaterial;
        }

        //玻璃
        private static BabylonMaterial CreateGlazingMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, Glazing.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Glazing.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Glazing.GlazingTransmittanceMap, defaultVal).ToList();
            
            babylonMaterial.baseColor = defaultVal.ToArray();

            var GlazingTransmittanceMapTexture = CreateUnifiedBitmapTexture(asset, Glazing.GlazingTransmittanceMap);
            if (GlazingTransmittanceMapTexture != null)
            {
               // GlazingTransmittanceMapTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = GlazingTransmittanceMapTexture;
                babylonMaterial.hasTexture = true;
            }

            var relfectivity = GetFloatPropertyValue(asset, Glazing.GlazingReflectance, 0.5f);
            babylonMaterial.alpha = relfectivity;
            if(relfectivity<1.0f)
                babylonMaterial.transparencyMode = (int)BabylonPBRMetallicRoughnessMaterial.TransparencyMode.ALPHABLEND;
            
            return babylonMaterial;
        }

        //木质
        private static BabylonMaterial CreateHardwoodMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, Hardwood.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Hardwood.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Hardwood.HardwoodColor, defaultVal).ToList();
            babylonMaterial.baseColor = defaultVal.ToArray();

            var HardwoodColorTexture = CreateUnifiedBitmapTexture(asset, Hardwood.HardwoodColor);
            if (HardwoodColorTexture != null)
            {
               // HardwoodColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = HardwoodColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        //墙体
        private static BabylonMaterial CreateMansoryCMUMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, MasonryCMU.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, MasonryCMU.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, MasonryCMU.MasonryCMUColor, defaultVal).ToList();
            babylonMaterial.baseColor = defaultVal.ToArray();

            var MasonryCMUColorTexture = CreateUnifiedBitmapTexture(asset,MasonryCMU.MasonryCMUColor);
            if (MasonryCMUColorTexture != null)
            {
                //MasonryCMUColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = MasonryCMUColorTexture;
                babylonMaterial.hasTexture = true;
            }

            var MasonryCMUBumpTexture = CreateUnifiedBitmapTexture(asset,MasonryCMU.MasonryCMUPatternMap,true);
            if(MasonryCMUBumpTexture != null)
            {
                //MasonryCMUBumpTexture.name = material.Id.ToString() + "_bump";
                MasonryCMUBumpTexture.IsBump = true;
                babylonMaterial.normalTexture = MasonryCMUBumpTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        //金属
        private static BabylonMaterial CreateMetalMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());
            bool commonTintToggle = GetBooleanPropertyValue(asset, MasonryCMU.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, MasonryCMU.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Metal.MetalColor, defaultVal).ToList();

            babylonMaterial.baseColor = defaultVal.ToArray();

            var MetalColorTexture = CreateUnifiedBitmapTexture(asset, Metal.MetalColor);
            if (MetalColorTexture != null)
            {
                //MetalColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = MetalColorTexture;
                babylonMaterial.hasTexture = true;
            }

            BabylonTexture metalBumpTexture = null;
            //浮雕图案
            var metalPatternType = GetIntegerPropertyValue(asset, Metal.MetalPattern, 0);
            switch(metalPatternType)
            {
                //滚花
                case (int)MetalPatternType.Knurl:
                    break;
                //花纹板
                case (int)MetalPatternType.DiamondPlate:
                    break;
                //方格板
                case (int)MetalPatternType.CheckerPlate:
                    break;
                //自定义
                case (int)MetalPatternType.Custom:
                    metalBumpTexture = CreateUnifiedBitmapTexture(asset, Metal.MetalPatternShader, true);
                    break;
                //无
                case (int)MetalPatternType.None:
                    break;
            }

            if(metalBumpTexture!=null)
            {
                metalBumpTexture.IsBump = true;
                babylonMaterial.normalTexture = metalBumpTexture;
                babylonMaterial.hasTexture = true;
            }

            babylonMaterial.metallic = 1.0f;
            babylonMaterial.roughness = 0.3f;
            return babylonMaterial;
        }

        private static BabylonMaterial CreateMetallicPaintMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, MetallicPaint.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, MetallicPaint.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, MetallicPaint.MetallicpaintBaseColor, defaultVal).ToList();
            
            babylonMaterial.diffuse = defaultVal.ToArray();

            var MetallicpaintBaseColorTexture = CreateUnifiedBitmapTexture(asset, MetallicPaint.MetallicpaintBaseColor);
            if (MetallicpaintBaseColorTexture != null)
            {
               // MetallicpaintBaseColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.diffuseTexture = MetallicpaintBaseColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        //镜子
        private static BabylonMaterial CreateMirrorMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, Mirror.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Mirror.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Mirror.MirrorTintcolor, defaultVal).ToList();
            babylonMaterial.baseColor = defaultVal.ToArray();

            var MirrorTintcolorTexture = CreateUnifiedBitmapTexture(asset, Mirror.MirrorTintcolor);
            if (MirrorTintcolorTexture != null)
            {
               // MirrorTintcolorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = MirrorTintcolorTexture;
                babylonMaterial.hasTexture = true;
            }

            babylonMaterial.roughness = 0.0f;
            babylonMaterial.metallic = 0.0f;
            return babylonMaterial;
        }

        //塑料
        private static BabylonMaterial CreatePlasticVinylMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, PlasticVinyl.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, PlasticVinyl.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, PlasticVinyl.PlasticvinylColor, defaultVal).ToList();
            babylonMaterial.diffuse = defaultVal.ToArray();

            var PlasticvinylColorTexture = CreateUnifiedBitmapTexture(asset, PlasticVinyl.PlasticvinylColor);
            if (PlasticvinylColorTexture != null)
            {
               // PlasticvinylColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.diffuseTexture = PlasticvinylColorTexture;
                babylonMaterial.hasTexture = true;
            }

            int type = GetIntegerPropertyValue(asset, PlasticVinyl.PlasticvinylType, 0);
            switch(type)
            {
                case (int)PlasticvinylType.Plasticsolid:
                    {
                        break;
                    }
                case (int)PlasticvinylType.Plastictransparent:
                    {
                        babylonMaterial.alpha = 0.8f;
                        babylonMaterial.transparencyMode = (int)BabylonPBRMetallicRoughnessMaterial.TransparencyMode.ALPHABLEND;
                        break;
                    }
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateSolidGlassMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                    new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());

            var type = GetIntegerPropertyValue(asset, SolidGlass.SolidglassTransmittance, 0);
            switch(type)
            {
                //透明
                case (int)SolidglassTransmittanceType.Clear:
                    defaultVal  = new List<float> { 1.0f,1.0f,1.0f};
                    break;
                //绿色
                case (int)SolidglassTransmittanceType.Green:
                    defaultVal = new List<float> { 0.0f, 1.0f, 127.0f/255.0f };
                    break;
                //灰色
                case (int)SolidglassTransmittanceType.Gray:
                    defaultVal = new List<float> { 192.0f/ 255.0f, 192.0f / 255.0f, 192.0f / 255.0f };
                    break;
                //蓝色
                case (int)SolidglassTransmittanceType.Blue:
                    defaultVal = new List<float> { 0.0f, 0.0f, 1.0f };
                    break;
                //青绿色
                case (int)SolidglassTransmittanceType.Bluegreen:
                    defaultVal = new List<float> { 64.0f/255.0f, 224.0f/255.0f, 208.0f / 255.0f };
                    break;
               //青铜色
                case (int)SolidglassTransmittanceType.Bronze:
                    defaultVal = new List<float> { 61.0f / 255.0f, 145.0f / 255.0f, 64.0f / 255.0f };
                    break;
                //自定义色
                case (int)SolidglassTransmittanceType.CustomColor:
                    defaultVal = GetColorPropertyValue(asset, SolidGlass.SolidglassTransmittanceCustomColor, defaultVal).ToList();
                    var SolidglassTransmittanceCustomColorTexture = CreateUnifiedBitmapTexture(asset,SolidGlass.SolidglassTransmittanceCustomColor);
                    if (SolidglassTransmittanceCustomColorTexture != null)
                    {
                        //SolidglassTransmittanceCustomColorTexture.name = material.Id.ToString() + "_color";

                        babylonMaterial.baseTexture = SolidglassTransmittanceCustomColorTexture;
                        babylonMaterial.hasTexture = true;
                    }
                    break;
            }

            bool commonTintToggle = GetBooleanPropertyValue(asset, SolidGlass.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, SolidGlass.CommonTintColor, defaultVal).ToList();

            babylonMaterial.alpha = 0.5f;
            babylonMaterial.transparencyMode = (int)BabylonPBRMetallicRoughnessMaterial.TransparencyMode.ALPHABLEND;

            babylonMaterial.baseColor = defaultVal.ToArray();
            babylonMaterial.roughness = GetFloatPropertyValue(asset, SolidGlass.SolidglassGlossiness, 0.0f);
            return babylonMaterial;
        }

        private static BabylonMaterial CreateWallPaintMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());

            bool commonTintToggle = GetBooleanPropertyValue(asset, WallPaint.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, WallPaint.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, WallPaint.WallpaintColor, defaultVal).ToList();
            babylonMaterial.diffuse = defaultVal.ToArray();

            var WallpaintColorTexture = CreateUnifiedBitmapTexture(asset, WallPaint.WallpaintColor);
            if (WallpaintColorTexture != null)
            {
                //WallpaintColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.diffuseTexture = WallpaintColorTexture;
                babylonMaterial.hasTexture = true;
            }
            return babylonMaterial;
        }

        private static BabylonMaterial CreateWaterMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            bool commonTintToggle = GetBooleanPropertyValue(asset, Water.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Water.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Water.WaterTintColor, defaultVal).ToList();

            BabylonStandardMaterial babylonMaterial = new BabylonStandardMaterial(material.Id.ToString());

            babylonMaterial.diffuse = defaultVal.ToArray();

            var WaterTintColorTexture = CreateUnifiedBitmapTexture(asset, Water.WaterTintColor);
            if (WaterTintColorTexture != null)
            {
               // WaterTintColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.diffuseTexture = WaterTintColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }

        private static BabylonMaterial CreateStoneMaterial(Asset asset, Material material)
        {
            Color defaultColor = material.Color.IsValid ? material.Color :
                new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            List<float> defaultVal = new List<float> { defaultColor.Red / 255.0f, defaultColor.Green / 255.0f, defaultColor.Blue / 255.0f };

            BabylonPBRMetallicRoughnessMaterial babylonMaterial = new BabylonPBRMetallicRoughnessMaterial(material.Id.ToString());
            bool commonTintToggle = GetBooleanPropertyValue(asset, Stone.CommonTintToggle, false);
            if (commonTintToggle)
                defaultVal = GetColorPropertyValue(asset, Stone.CommonTintColor, defaultVal).ToList();
            else
                defaultVal = GetColorPropertyValue(asset, Stone.StoneColor, defaultVal).ToList();
            babylonMaterial.baseColor = defaultVal.ToArray();

            var StoneColorTexture = CreateUnifiedBitmapTexture(asset,Stone.StoneColor);
            if (StoneColorTexture != null)
            {
               // StoneColorTexture.name = material.Id.ToString() + "_color";
                babylonMaterial.baseTexture = StoneColorTexture;
                babylonMaterial.hasTexture = true;
            }

            var StonebumpTexture = CreateUnifiedBitmapTexture(asset,Stone.StoneBumpMap,false);
            if (StonebumpTexture != null)
            {
                //StonebumpTexture.name = material.Id.ToString() + "_bump";
                StonebumpTexture.IsBump = true;
                babylonMaterial.normalTexture = StoneColorTexture;
                babylonMaterial.hasTexture = true;
            }

            return babylonMaterial;
        }
    }
}
