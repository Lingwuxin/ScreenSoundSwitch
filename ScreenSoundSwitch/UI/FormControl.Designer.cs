namespace ScreenSoundSwitch.UI
{
    partial class FormControl
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
            tabControl = new TabControl();
            selectDevicePage = new TabPage();
            volumeCtrlPage = new TabPage();
            tabControl.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(selectDevicePage);
            tabControl.Controls.Add(volumeCtrlPage);
            tabControl.Location = new Point(7, 8);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(732, 426);
            tabControl.TabIndex = 1;
            // 
            // selectDevicePage
            // 
            selectDevicePage.Location = new Point(4, 26);
            selectDevicePage.Name = "selectDevicePage";
            selectDevicePage.Padding = new Padding(3);
            selectDevicePage.Size = new Size(724, 396);
            selectDevicePage.TabIndex = 0;
            selectDevicePage.Text = "设备选择";
            selectDevicePage.UseVisualStyleBackColor = true;
            // 
            // volumeCtrlPage
            // 
            volumeCtrlPage.Location = new Point(4, 26);
            volumeCtrlPage.Name = "volumeCtrlPage";
            volumeCtrlPage.Padding = new Padding(3);
            volumeCtrlPage.Size = new Size(724, 396);
            volumeCtrlPage.TabIndex = 1;
            volumeCtrlPage.Text = "音量控制";
            volumeCtrlPage.UseVisualStyleBackColor = true;
            // 
            // FormControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl);
            Name = "FormControl";
            Size = new Size(747, 442);
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        public TabPage selectDevicePage;
        public TabPage volumeCtrlPage;
    }
}
