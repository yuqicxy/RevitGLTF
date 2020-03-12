using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitGLTF.UI
{
    public partial class Tile3DConfigUI : UserControl
    {
        public RevitGLTF.ExportConfig Config { get; set; }
        
        public Tile3DConfigUI()
        {
            InitializeComponent();

            Config = new RevitGLTF.ExportConfig();
            Config.mExportMode = RevitGLTF.ExportConfig.ExportMode.TILE3D;
        }

        public Tile3DConfigUI(RevitGLTF.ExportConfig config)
        {
            Config = config;
        }

        private void OpenPathClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();
            fileDialog.Description = $"选择3DTile导出目录";
            fileDialog.ShowNewFolderButton = true;
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string dir = fileDialog.SelectedPath;
                this.pathTextBox.Text = dir;
                Config.mOutPutPath = dir;
            }
        }

        private void CheckDracoChanged(object sender, EventArgs e)
        {
            Config.mDracoCompress = this.checkDraco.Checked;
        }
    }
}
