namespace ScreenSoundSwitch.UI
{
    partial class DeviceControl
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
            button1 = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            comboBoxScreen = new ComboBox();
            labelScreen = new Label();
            selectButton = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            comBoxScreen = new ComboBox();
            label1 = new Label();
            comBoxAudio = new ComboBox();
            labelAudio = new Label();
            screenList = new ListBox();
            audioDeviceList = new ListBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(37, 3);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 8;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.943018F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.1369953F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.9008007F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.01919F));
            tableLayoutPanel1.Controls.Add(comboBoxScreen, 1, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(200, 100);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // comboBoxScreen
            // 
            comboBoxScreen.FormattingEnabled = true;
            comboBoxScreen.Location = new Point(28, 3);
            comboBoxScreen.Name = "comboBoxScreen";
            comboBoxScreen.Size = new Size(50, 25);
            comboBoxScreen.TabIndex = 0;
            // 
            // labelScreen
            // 
            labelScreen.AutoSize = true;
            labelScreen.Location = new Point(3, 0);
            labelScreen.Name = "labelScreen";
            labelScreen.Size = new Size(19, 85);
            labelScreen.TabIndex = 1;
            labelScreen.Text = "Screen";
            // 
            // selectButton
            // 
            selectButton.Location = new Point(236, 19);
            selectButton.Name = "selectButton";
            selectButton.Size = new Size(75, 23);
            selectButton.TabIndex = 8;
            selectButton.Text = "绑定设备";
            selectButton.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.943018F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28.1369953F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13.9008007F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45.01919F));
            tableLayoutPanel2.Controls.Add(comBoxScreen, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(comBoxAudio, 3, 0);
            tableLayoutPanel2.Controls.Add(labelAudio, 2, 0);
            tableLayoutPanel2.Location = new Point(46, 63);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 51F));
            tableLayoutPanel2.Size = new Size(456, 43);
            tableLayoutPanel2.TabIndex = 7;
            // 
            // comBoxScreen
            // 
            comBoxScreen.Anchor = AnchorStyles.Left;
            comBoxScreen.FormattingEnabled = true;
            comBoxScreen.Location = new Point(62, 9);
            comBoxScreen.Name = "comBoxScreen";
            comBoxScreen.Size = new Size(122, 25);
            comBoxScreen.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(9, 13);
            label1.Name = "label1";
            label1.Size = new Size(47, 17);
            label1.TabIndex = 1;
            label1.Text = "Screen";
            // 
            // comBoxAudio
            // 
            comBoxAudio.Anchor = AnchorStyles.None;
            comBoxAudio.FormattingEnabled = true;
            comBoxAudio.Location = new Point(253, 9);
            comBoxAudio.Name = "comBoxAudio";
            comBoxAudio.Size = new Size(200, 25);
            comBoxAudio.TabIndex = 3;
            // 
            // labelAudio
            // 
            labelAudio.Anchor = AnchorStyles.Right;
            labelAudio.AutoSize = true;
            labelAudio.Location = new Point(205, 13);
            labelAudio.Name = "labelAudio";
            labelAudio.Size = new Size(42, 17);
            labelAudio.TabIndex = 2;
            labelAudio.Text = "Audio";
            // 
            // screenList
            // 
            screenList.FormattingEnabled = true;
            screenList.ItemHeight = 17;
            screenList.Location = new Point(108, 136);
            screenList.Name = "screenList";
            screenList.Size = new Size(120, 157);
            screenList.TabIndex = 9;
            // 
            // audioDeviceList
            // 
            audioDeviceList.FormattingEnabled = true;
            audioDeviceList.ItemHeight = 17;
            audioDeviceList.Location = new Point(299, 136);
            audioDeviceList.Name = "audioDeviceList";
            audioDeviceList.Size = new Size(190, 157);
            audioDeviceList.TabIndex = 10;
            // 
            // DeviceControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(audioDeviceList);
            Controls.Add(screenList);
            Controls.Add(selectButton);
            Controls.Add(tableLayoutPanel2);
            Name = "DeviceControl";
            Size = new Size(547, 343);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox comboBoxScreen;
        private Label labelScreen;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private Label labelAudio;
        public ComboBox comBoxScreen;
        public ComboBox comBoxAudio;
        public Button selectButton;
        private ListBox screenList;
        private ListBox audioDeviceList;
    }
}
