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

namespace MaterialAssetViewer
{
    public partial class MaterialInfoDialog : System.Windows.Forms.Form
    {
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

                outString += "\n";
            }
            return outString;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
        }
    }
}
