using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;

using Newtonsoft.Json;
using System.Globalization;
using System.IO;

namespace MaterialAssetViewer
{
    public partial class MaterialInfoDialog : System.Windows.Forms.Form
    {
        public static string ToJson(Object obj)
        {
            var jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
            var sb = new StringBuilder();
            var sw = new StringWriter(sb, CultureInfo.InvariantCulture);

            // Do not use the optimized writer because it's not necessary to truncate values
            // Use the bounded writer in case some values are infinity ()
            using (var jsonWriter = new JsonTextWriter(sw))
            {
#if DEBUG
                jsonWriter.Formatting = Formatting.Indented;
#else
                jsonWriter.Formatting = Formatting.None;
#endif
                jsonSerializer.Serialize(jsonWriter, obj);
            }
            return sb.ToString();
        }
        public MaterialInfoDialog(Element ele)
        {
            InitializeComponent();

            var str = GetMaterialInfo(ele);
            richTextBox1.Text = str;
        }

        private String GetMaterialInfo(Element ele)
        {
            String outString = "";

            var materialIds = ele.GetMaterialIds(false);
            Document doc = ele.Document;
            foreach (var id in materialIds)
            {
                var material = doc.GetElement(id) as Material;
                if (material == null)
                    continue;

                var appearanceAssetElement = doc.GetElement(material.AppearanceAssetId) as AppearanceAssetElement;
                if (appearanceAssetElement == null)
                    continue;

                outString += "material Name:" + appearanceAssetElement.Name + "\n";

                Asset asset = appearanceAssetElement.GetRenderingAsset();

                outString += asset.Stringify();

                //outString += "\n\n";

                //outString += "BabylonMaterial";
                //try 
                //{
                //    var babylonMaterial = RevitGLTF.MaterialFactory.Instance.CreateMaterial(asset, material);
                //    if (babylonMaterial != null)
                //        outString += ToJson(babylonMaterial);
                //}
                //catch (Exception exp)
                //{
                //    outString += exp.Source + "\n";
                //    outString += exp.Message + "\n";
                //    outString += exp.StackTrace + "\n";
                //}
                outString += "\n\n";
            }
            return outString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
        }
    }
}
