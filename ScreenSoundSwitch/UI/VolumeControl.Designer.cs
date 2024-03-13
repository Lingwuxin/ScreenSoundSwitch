namespace ScreenSoundSwitch
{
    partial class VolumeControl
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
            volumeTraceBar = new TrackBar();
            deviceLable = new Label();
            ((System.ComponentModel.ISupportInitialize)volumeTraceBar).BeginInit();
            SuspendLayout();
            // 
            // volumeTraceBar
            // 
            volumeTraceBar.Location = new Point(39, 112);
            volumeTraceBar.Name = "volumeTraceBar";
            volumeTraceBar.Orientation = Orientation.Vertical;
            volumeTraceBar.Size = new Size(45, 104);
            volumeTraceBar.TabIndex = 0;
            // 
            // deviceLable
            // 
            deviceLable.AutoSize = true;
            deviceLable.Location = new Point(26, 64);
            deviceLable.Name = "deviceLable";
            deviceLable.Size = new Size(80, 17);
            deviceLable.TabIndex = 1;
            deviceLable.Text = "deviceName";
            // 
            // VolumeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(deviceLable);
            Controls.Add(volumeTraceBar);
            Name = "VolumeControl";
            Size = new Size(152, 240);
            Load += VolumeControl_Load;
            ((System.ComponentModel.ISupportInitialize)volumeTraceBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar volumeTraceBar;
        private Label deviceLable;
    }
}
