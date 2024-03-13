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
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            button1 = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            comboBoxScreen = new ComboBox();
            labelScreen = new Label();
            labelAudio = new Label();
            comboBoxAudio = new ComboBox();
            tabControl.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPage1);
            tabControl.Controls.Add(tabPage2);
            tabControl.Location = new Point(24, 12);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(732, 426);
            tabControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(tableLayoutPanel1);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(724, 396);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "设备选择";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(724, 396);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "音量控制";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(324, 31);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 6;
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
            tableLayoutPanel1.Controls.Add(labelScreen, 0, 0);
            tableLayoutPanel1.Controls.Add(labelAudio, 2, 0);
            tableLayoutPanel1.Controls.Add(comboBoxAudio, 3, 0);
            tableLayoutPanel1.Location = new Point(134, 75);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 51F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 49F));
            tableLayoutPanel1.Size = new Size(456, 100);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // comboBoxScreen
            // 
            comboBoxScreen.FormattingEnabled = true;
            comboBoxScreen.Location = new Point(62, 3);
            comboBoxScreen.Name = "comboBoxScreen";
            comboBoxScreen.Size = new Size(122, 25);
            comboBoxScreen.TabIndex = 0;
            // 
            // labelScreen
            // 
            labelScreen.AutoSize = true;
            labelScreen.Location = new Point(3, 0);
            labelScreen.Name = "labelScreen";
            labelScreen.Size = new Size(47, 17);
            labelScreen.TabIndex = 1;
            labelScreen.Text = "Screen";
            // 
            // labelAudio
            // 
            labelAudio.AutoSize = true;
            labelAudio.Location = new Point(190, 0);
            labelAudio.Name = "labelAudio";
            labelAudio.Size = new Size(42, 17);
            labelAudio.TabIndex = 2;
            labelAudio.Text = "Audio";
            // 
            // comboBoxAudio
            // 
            comboBoxAudio.FormattingEnabled = true;
            comboBoxAudio.Location = new Point(253, 3);
            comboBoxAudio.Name = "comboBoxAudio";
            comboBoxAudio.Size = new Size(200, 25);
            comboBoxAudio.TabIndex = 3;
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
            tabPage1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage tabPage1;
        private Button button1;
        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox comboBoxScreen;
        private Label labelScreen;
        private Label labelAudio;
        private ComboBox comboBoxAudio;
        private TabPage tabPage2;
    }
}
