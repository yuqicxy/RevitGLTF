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
        private Autodesk.Revit.DB.Document mRevitDocument;
        private Autodesk.Revit.DB.ViewSet mAllViews = new Autodesk.Revit.DB.ViewSet();
        private Autodesk.Revit.DB.View3D mSelectView;
        private ExportConfig mConfig = new ExportConfig();
        public ExportConfig Config
        {
            get { return mConfig; }
            set { mConfig = value; }
        }

        public ExportDialog(Autodesk.Revit.DB.Document document)
        {
            InitializeComponent();
            mRevitDocument = document;
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
                if(view3d != null && !view3d.IsTemplate)
                    mAllViews.Insert(view3d);
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
            foreach (Autodesk.Revit.DB.View view in mAllViews)
            {
                dt.Rows.Add(view.Name,index);
                ++index;
            }
            view3dComboBox.Items.Clear();
            view3dComboBox.DataSource = dt;
            view3dComboBox.DisplayMember = "Text";   // Text，即显式的文本
            view3dComboBox.ValueMember = "Value";    // Value，即实际的值
        }

        private void OpenPathClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fileDialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                string dir = fileDialog.SelectedPath;
                this.pathTextBox.Text = dir;
                mConfig.mOutPutPath = dir;
            }
        }
        
        private void pathTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = this.pathTextBox.Text;
            if(!Directory.Exists(path))
            {
                mConfig.mOutPutPath = path;
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            ExportManager manager = new ExportManager(mConfig);
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
            Autodesk.Revit.DB.ViewSetIterator itor = mAllViews.ForwardIterator();
            for (int i = 0;i <= index;++i)
            {
                itor.MoveNext();
            }
            mSelectView = itor.Current as Autodesk.Revit.DB.View3D;
        }
    }
}
