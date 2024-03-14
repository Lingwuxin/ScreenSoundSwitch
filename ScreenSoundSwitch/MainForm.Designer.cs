namespace ScreenSoundSwitch
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControl = new TabControl();
            selectDevicePage = new TabPage();
            manageVolumePage = new TabPage();
            volumepPageTableLayout = new TableLayoutPanel();
            tabControl.SuspendLayout();
            manageVolumePage.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(selectDevicePage);
            tabControl.Controls.Add(manageVolumePage);
            tabControl.Location = new Point(12, 12);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(744, 426);
            tabControl.TabIndex = 0;
            // 
            // selectDevicePage
            // 
            selectDevicePage.Location = new Point(4, 26);
            selectDevicePage.Name = "selectDevicePage";
            selectDevicePage.Padding = new Padding(3);
            selectDevicePage.Size = new Size(736, 396);
            selectDevicePage.TabIndex = 0;
            selectDevicePage.Text = "选择设备";
            selectDevicePage.UseVisualStyleBackColor = true;
            // 
            // manageVolumePage
            // 
            manageVolumePage.Controls.Add(volumepPageTableLayout);
            manageVolumePage.Location = new Point(4, 26);
            manageVolumePage.Name = "manageVolumePage";
            manageVolumePage.Padding = new Padding(3);
            manageVolumePage.Size = new Size(736, 396);
            manageVolumePage.TabIndex = 1;
            manageVolumePage.Text = "管理音量";
            manageVolumePage.UseVisualStyleBackColor = true;
            // 
            // volumepPageTableLayout
            // 
            volumepPageTableLayout.AutoSize = true;
            volumepPageTableLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            volumepPageTableLayout.ColumnCount = 1;
            volumepPageTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            volumepPageTableLayout.Dock = DockStyle.Left;
            volumepPageTableLayout.Location = new Point(3, 3);
            volumepPageTableLayout.Name = "volumepPageTableLayout";
            volumepPageTableLayout.RowCount = 1;
            volumepPageTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            volumepPageTableLayout.Size = new Size(0, 390);
            volumepPageTableLayout.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(768, 450);
            Controls.Add(tabControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Main";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            tabControl.ResumeLayout(false);
            manageVolumePage.ResumeLayout(false);
            manageVolumePage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage selectDevicePage;
        private TabPage manageVolumePage;
        public TableLayoutPanel volumepPageTableLayout;
    }
}
