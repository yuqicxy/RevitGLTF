using System;
using System.IO;
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
    public partial class GLTFConfigUI : UserControl
    {
        public RevitGLTF.ExportConfig Config { get; set; }

        public GLTFConfigUI()
        {
            InitializeComponent();

            Config = new RevitGLTF.ExportConfig();
        }

        public GLTFConfigUI(RevitGLTF.ExportConfig config)
        {
            InitializeComponent();

            Config = config;
            Config.mExportMode = RevitGLTF.ExportConfig.ExportMode.GLTF;
        }

        private void OnPathClick(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "GL Transmission Format(*.gltf)|*.gltf|" +
                "Binary glTF Layout(*.glb)|*.glb"; //设置要选择的文件的类型
            //保存对话框是否记忆上次打开的目录 
            fileDialog.RestoreDirectory = true;
            
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = fileDialog.FileName;
                Config.mOutputFilename = Path.GetFileNameWithoutExtension(filename);
                Config.mOutputFormat = Path.GetExtension(filename);
                Config.mOutPutPath = Path.GetDirectoryName(filename);
                this.pathTextBox.Text = filename;
            }
        }

        private void CheckDracoChanged(object sender, EventArgs e)
        {
            Config.mDracoCompress = this.checkDraco.Checked;
        }
    }
}
