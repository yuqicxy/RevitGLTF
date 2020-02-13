using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

namespace MaterialAssetViewer
{
    public static class AssetSerilizer
    {
        public static String Stringify(this Asset rootasset)
        {
            String str = String.Format("name:{0} Type:{1} Library:{2}\n", rootasset.Name, rootasset.Type.ToString(), rootasset.LibraryName);

            var properties = rootasset as AssetProperties;
            if (properties != null)
            {
                str += properties.Stringify();
            }
            return str;
        }

        private static String Serilize(AssetProperty asset)
        {
            var str = "";

            AssetPropertyType type = asset.Type;
            switch (type)
            {
                case AssetPropertyType.Boolean:
                    {
                        var property = asset as AssetPropertyBoolean;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Double1:
                    {
                        var property = asset as AssetPropertyDouble;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Double2:
                    {
                        var property = asset as AssetPropertyDoubleArray2d;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Double3:
                    {
                        var property = asset as AssetPropertyDoubleArray3d;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Double4:
                    {
                        var property = asset as AssetPropertyDoubleArray4d;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Double44:
                    {
                        var property = asset as AssetPropertyDoubleMatrix44;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Enumeration:
                    {
                        var property = asset as AssetPropertyEnum;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Float:
                    {
                        var property = asset as AssetPropertyFloat;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Float3:
                    {
                        var property = asset as AssetPropertyFloatArray;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Integer:
                    {
                        var property = asset as AssetPropertyInteger;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Longlong:
                    {
                        var property = asset as AssetPropertyInt64;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.List:
                    {
                        var property = asset as AssetPropertyList;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Properties:
                    {
                        var property = asset as AssetProperties;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Reference:
                    {
                        var property = asset as AssetPropertyReference;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.String:
                    {
                        var property = asset as AssetPropertyString;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Time:
                    {
                        var property = asset as AssetPropertyTime;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.ULonglong:
                    {
                        var property = asset as AssetPropertyUInt64;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Asset:
                    {
                        var property = asset as Asset;
                        str += property.Stringify();
                        break;
                    }
                case AssetPropertyType.Distance:
                    {
                        var property = asset as AssetPropertyDistance;
                        str += property.Stringify();
                        break;
                    }
                default:
                    {
                        str += String.Format("Name:{0} Type:{1}", asset.Name, asset.Type.ToString());
                        break;
                    }
            }

            return str;
        }

        private static String Serilize(IList<AssetProperty> properties)
        {
            var str = "";

            foreach (var asset in properties)
            {
                Serilize(asset);
            }

            return str;
        }

        public static String Stringify(this AssetProperty assetProperty)
        {
            String str = "";

            var propertyies = assetProperty.GetAllConnectedProperties();
            if (propertyies.Count > 0)
            {
                str += "AllConnectedProperties:\n";
                str += Serilize(propertyies);
            }
            

            if (assetProperty.GetSingleConnectedAsset() != null)
            {
                str += "SingleConnectedAsset:\n";
                str += assetProperty.GetSingleConnectedAsset().Stringify();
            }

            return str;
        }

        //AssetPropertyType.Boolean:
        public static String Stringify(this AssetPropertyBoolean asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n", asset.Name, asset.Type.ToString(), asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Double1:
        public static String Stringify(this AssetPropertyDouble asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n", asset.Name, asset.Type.ToString(), asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Double2:
        public static String Stringify(this AssetPropertyDoubleArray2d asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2} {3}\n", 
                asset.Name, asset.Type.ToString(), 
                asset.Value.get_Item(0), 
                asset.Value.get_Item(1));

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Double3:
        public static String Stringify(this AssetPropertyDoubleArray3d asset)
        {
            var str = String.Format("name:{0} Type:{1} Value:{2} {3} {4}\n",
                asset.Name, asset.Type.ToString(),
                asset.GetValueAsXYZ().X,
                asset.GetValueAsXYZ().Y,
                asset.GetValueAsXYZ().Z);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Double4:
        public static String Stringify(this AssetPropertyDoubleArray4d asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2} {3} {4} {5}\n",
                asset.Name, asset.Type.ToString(),
                asset.GetValueAsDoubles()[0],
                asset.GetValueAsDoubles()[1],
                asset.GetValueAsDoubles()[2],
                asset.GetValueAsDoubles()[3]);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Double44:
        public static String Stringify(this AssetPropertyDoubleMatrix44 asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:",
                asset.Name, asset.Type.ToString());
            for(int i =0;i< 16;++i)
            {
                str += asset.Value.get_Item(i).ToString();
            }
            str += "\n";
         
            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Enumeration:
        public static String Stringify(this AssetPropertyEnum asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Float:
        public static String Stringify(this AssetPropertyFloat asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Float3:
        public static String Stringify(this AssetPropertyFloatArray asset)
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.GetValue().ToString());

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Integer:
        public static String Stringify(this AssetPropertyInteger asset)
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Longlong:
        public static String Stringify(this AssetPropertyInt64 asset)
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.List:
        public static String Stringify(this AssetPropertyList assetProperty)
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                assetProperty.Name, assetProperty.Type.ToString(),
                assetProperty.ToString());

            var propertyies = assetProperty.GetValue();
            str +=Serilize(propertyies);

            var pro = assetProperty as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Properties:
        public static String Stringify(this AssetProperties rootasset) 
        {
            var str = "";
            for (int i = 0; i < rootasset.Size; ++i)
            {
                var asset = rootasset[i];
                str += Serilize(asset);
            }

            var pro = rootasset as AssetProperty;
            str += pro.Stringify();

            return str; 
        }

        //AssetPropertyType.Reference:
        public static String Stringify(this AssetPropertyReference asset) 
        {
            var str = String.Format("name:{0} Type:{1}\n",
                asset.Name, asset.Type.ToString());

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.String:
        public static String Stringify(this AssetPropertyString asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Time:
        public static String Stringify(this AssetPropertyTime asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.ULonglong:
        public static String Stringify(this AssetPropertyUInt64 asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value);

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }

        //AssetPropertyType.Distance:
        public static String Stringfy(this AssetPropertyDistance asset) 
        {
            var str = String.Format("name:{0} Type:{1} Value:{2} {3}\n",
                asset.Name, asset.Type.ToString(),
                asset.Value,asset.DisplayUnitType.ToString());

            var pro = asset as AssetProperty;
            str += pro.Stringify();

            return str;
        }
    }
}
