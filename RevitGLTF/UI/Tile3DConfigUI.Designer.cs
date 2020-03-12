namespace RevitGLTF.UI
{
    partial class Tile3DConfigUI
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkWebp = new System.Windows.Forms.CheckBox();
            this.checkDraco = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(99, 21);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.ReadOnly = true;
            this.pathTextBox.Size = new System.Drawing.Size(380, 25);
            this.pathTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "输出路径:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(493, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OpenPathClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkWebp);
            this.groupBox1.Controls.Add(this.checkDraco);
            this.groupBox1.Location = new System.Drawing.Point(15, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(563, 157);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "导出选项";
            // 
            // checkWebp
            // 
            this.checkWebp.AutoSize = true;
            this.checkWebp.Location = new System.Drawing.Point(37, 82);
            this.checkWebp.Name = "checkWebp";
            this.checkWebp.Size = new System.Drawing.Size(196, 19);
            this.checkWebp.TabIndex = 3;
            this.checkWebp.Text = "WebP纹理压缩（未实现）";
            this.checkWebp.UseVisualStyleBackColor = true;
            // 
            // checkDraco
            // 
            this.checkDraco.AutoSize = true;
            this.checkDraco.Location = new System.Drawing.Point(37, 44);
            this.checkDraco.Name = "checkDraco";
            this.checkDraco.Size = new System.Drawing.Size(119, 19);
            this.checkDraco.TabIndex = 2;
            this.checkDraco.Text = "顶点属性压缩";
            this.checkDraco.UseVisualStyleBackColor = true;
            this.checkDraco.CheckStateChanged += new System.EventHandler(this.CheckDracoChanged);
            // 
            // Tile3DConfigUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pathTextBox);
            this.Name = "Tile3DConfigUI";
            this.Size = new System.Drawing.Size(600, 250);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkWebp;
        private System.Windows.Forms.CheckBox checkDraco;
    }
}
