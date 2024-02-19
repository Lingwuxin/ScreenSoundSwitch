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
            tableLayoutPanel1 = new TableLayoutPanel();
            comboBoxScreen = new ComboBox();
            labelScreen = new Label();
            labelAudio = new Label();
            comboBoxAudio = new ComboBox();
            textBox1 = new TextBox();
            button1 = new Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
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
            tableLayoutPanel1.Location = new Point(188, 148);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 51F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 49F));
            tableLayoutPanel1.Size = new Size(456, 100);
            tableLayoutPanel1.TabIndex = 1;
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
            // textBox1
            // 
            textBox1.Location = new Point(0, 268);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Horizontal;
            textBox1.Size = new Size(765, 185);
            textBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(378, 104);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(768, 450);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(tableLayoutPanel1);
            Name = "MainForm";
            Text = "Main";
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox comboBoxScreen;
        private Label labelScreen;
        private Label labelAudio;
        private ComboBox comboBoxAudio;
        private TextBox textBox1;
        private Button button1;
    }
}
