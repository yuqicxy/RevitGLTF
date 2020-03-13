using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitGLTF
{
    public partial class ExportDialog : Form
    {
        private Autodesk.Revit.DB.Document mRevitDocument = null;
        //private Autodesk.Revit.DB.ViewSet mAllViews = null;
        private Autodesk.Revit.DB.View mSelectView = null;
        private List<Autodesk.Revit.DB.View3D> mViewList = null;

        UI.GLTFConfigUI mGltfConfigWidget;
        UI.Tile3DConfigUI mTile3dConfigWiget;
        public ExportDialog(Autodesk.Revit.DB.Document document)
        {
            InitializeComponent();
            mRevitDocument = document;

            mGltfConfigWidget = new UI.GLTFConfigUI();
            this.mGLTFPage.Controls.Add(mGltfConfigWidget);

            mTile3dConfigWiget = new UI.Tile3DConfigUI();
            this.mTile3DPage.Controls.Add(mTile3dConfigWiget);

            //mAllViews = new Autodesk.Revit.DB.ViewSet();
            mViewList = new List<Autodesk.Revit.DB.View3D>();

            GetAllView();
        }

        private void GetAllView()
        {
            Autodesk.Revit.DB.FilteredElementCollector collector 
                = new Autodesk.Revit.DB.FilteredElementCollector(mRevitDocument);
            Autodesk.Revit.DB.FilteredElementIterator itor
                = collector.OfClass(typeof(Autodesk.Revit.DB.View3D)).GetElementIterator();
            itor.Reset();
            while (itor.MoveNext())
            {
                Autodesk.Revit.DB.View3D view3d= itor.Current as Autodesk.Revit.DB.View3D;
                if (view3d != null && !view3d.IsTemplate)
                {
                    //mAllViews.Insert(view3d);
                    mViewList.Add(view3d);
                }
            }
            AddViewToViewCombo();
        }

        private void AddViewToViewCombo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Text", Type.GetType("System.String"));
            dt.Columns.Add("Value", Type.GetType("System.UInt32"));

            List<string> list = new List<string>();
            uint index = 0;
            foreach (Autodesk.Revit.DB.View view in mViewList)
            {
                if (mSelectView == null)
                    mSelectView = view;
                dt.Rows.Add(view.Name,index);
                ++index;
            }
            view3dComboBox.Items.Clear();
            view3dComboBox.DataSource = dt;
            view3dComboBox.DisplayMember = "Text";   // Text，即显式的文本
            view3dComboBox.ValueMember = "Value";    // Value，即实际的值
        }

        private bool checkConfig(RevitGLTF.ExportConfig config)
        {
            if (config.mOutPutPath == null || !Directory.Exists(config.mOutPutPath))
            {
                MessageBox.Show("无效导出路径");
                return false;
            }
            if(config.mExportMode == ExportConfig.ExportMode.GLTF)
            {
                if (config.mOutputFormat == null)
                {
                    MessageBox.Show("GLTF导出：无效扩展名");
                    return false;
                }
                if (config.mOutputFilename == null)
                {
                    MessageBox.Show("GLTF导出：无效文件名");
                    return false;
                }
            }
            return true;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            RevitGLTF.ExportConfig config = null;
            if (this.ModeControl.SelectedIndex == 0)
                config = this.mGltfConfigWidget.Config;
            else
                config = this.mTile3dConfigWiget.Config;

            if (!checkConfig(config))
                return;

            ExportManager manager = new ExportManager(config);
            if (mSelectView != null)
            {
                this.Hide();
                manager.Export(mSelectView);
                this.Show();
            }
            else
            {
                MessageBox.Show("无效导出,未选择视图");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewComboBoxSelectChanged(object sender,EventArgs e)
        {
            int index = this.view3dComboBox.SelectedIndex;
            mSelectView = mViewList[index];
            //Autodesk.Revit.DB.ViewSetIterator itor = mAllViews.ForwardIterator();
            //for (int i = 0;i <= index;++i)
            //{
            //    itor.MoveNext();
            //}
            //mSelectView = itor.Current as Autodesk.Revit.DB.View3D;
        }
    }
}
