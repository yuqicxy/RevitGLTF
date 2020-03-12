namespace RevitGLTF
{
    partial class ExportDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDialog));
            this.exportButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ModeControl = new System.Windows.Forms.TabControl();
            this.mGLTFPage = new System.Windows.Forms.TabPage();
            this.mTile3DPage = new System.Windows.Forms.TabPage();
            this.view3dComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ModeControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            resources.ApplyResources(this.exportButton, "exportButton");
            this.exportButton.Name = "exportButton";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ModeControl
            // 
            this.ModeControl.Controls.Add(this.mGLTFPage);
            this.ModeControl.Controls.Add(this.mTile3DPage);
            this.ModeControl.ImageList = this.imageList1;
            resources.ApplyResources(this.ModeControl, "ModeControl");
            this.ModeControl.Name = "ModeControl";
            this.ModeControl.SelectedIndex = 0;
            // 
            // mGLTFPage
            // 
            resources.ApplyResources(this.mGLTFPage, "mGLTFPage");
            this.mGLTFPage.Name = "mGLTFPage";
            this.mGLTFPage.UseVisualStyleBackColor = true;
            // 
            // mTile3DPage
            // 
            resources.ApplyResources(this.mTile3DPage, "mTile3DPage");
            this.mTile3DPage.Name = "mTile3DPage";
            this.mTile3DPage.UseVisualStyleBackColor = true;
            // 
            // view3dComboBox
            // 
            this.view3dComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.view3dComboBox, "view3dComboBox");
            this.view3dComboBox.Name = "view3dComboBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "favicon-32x32.png");
            this.imageList1.Images.SetKeyName(1, "favicon-32x32.png");
            // 
            // ExportDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ModeControl);
            this.Controls.Add(this.view3dComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.exportButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportDialog";
            this.ModeControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl ModeControl;
        private System.Windows.Forms.TabPage mGLTFPage;
        private System.Windows.Forms.TabPage mTile3DPage;
        private System.Windows.Forms.ComboBox view3dComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ImageList imageList1;
    }
}