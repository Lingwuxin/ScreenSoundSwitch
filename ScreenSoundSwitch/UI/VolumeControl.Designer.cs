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
            volumeTrackBar = new TrackBar();
            deviceLable = new Label();
            leftTrackBar = new TrackBar();
            rightTrackBar = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)volumeTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)leftTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)rightTrackBar).BeginInit();
            SuspendLayout();
            // 
            // volumeTrackBar
            // 
            volumeTrackBar.LargeChange = 20;
            volumeTrackBar.Location = new Point(49, 43);
            volumeTrackBar.Maximum = 100;
            volumeTrackBar.Name = "volumeTrackBar";
            volumeTrackBar.Orientation = Orientation.Vertical;
            volumeTrackBar.Size = new Size(45, 104);
            volumeTrackBar.TabIndex = 0;
            // 
            // deviceLable
            // 
            deviceLable.AutoSize = true;
            deviceLable.Location = new Point(26, 150);
            deviceLable.Name = "deviceLable";
            deviceLable.Size = new Size(80, 17);
            deviceLable.TabIndex = 1;
            deviceLable.Text = "deviceName";
            // 
            // leftTrackBar
            // 
            leftTrackBar.LargeChange = 20;
            leftTrackBar.Location = new Point(26, 170);
            leftTrackBar.Maximum = 100;
            leftTrackBar.Name = "leftTrackBar";
            leftTrackBar.Size = new Size(104, 45);
            leftTrackBar.TabIndex = 2;
            // 
            // rightTrackBar
            // 
            rightTrackBar.LargeChange = 20;
            rightTrackBar.Location = new Point(26, 202);
            rightTrackBar.Maximum = 100;
            rightTrackBar.Name = "rightTrackBar";
            rightTrackBar.Size = new Size(104, 45);
            rightTrackBar.TabIndex = 3;
            // 
            // VolumeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(rightTrackBar);
            Controls.Add(leftTrackBar);
            Controls.Add(deviceLable);
            Controls.Add(volumeTrackBar);
            Name = "VolumeControl";
            Size = new Size(143, 249);
            Load += VolumeControl_Load;
            ((System.ComponentModel.ISupportInitialize)volumeTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)leftTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)rightTrackBar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label deviceLable;
        public TrackBar volumeTrackBar;
        private TrackBar leftTrackBar;
        private TrackBar rightTrackBar;
    }
}
