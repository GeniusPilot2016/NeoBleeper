namespace NeoBleeper
{
    partial class PortamentoWindow
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
            components = new System.ComponentModel.Container();
            groupBox1 = new GroupBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            trackBar1 = new TrackBar();
            label1 = new Label();
            trackBar2 = new TrackBar();
            label2 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(trackBar1);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(601, 185);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Length of sound";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(19, 35);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(400, 24);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "System speaker/sound device always produces sound";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(19, 65);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(544, 24);
            radioButton2.TabIndex = 0;
            radioButton2.TabStop = true;
            radioButton2.Text = "System speaker/sound produces sound for roughly this many milliseconds";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(19, 110);
            trackBar1.Maximum = 1000;
            trackBar1.Minimum = 50;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(559, 56);
            trackBar1.TabIndex = 1;
            trackBar1.TickFrequency = 50;
            trackBar1.Value = 50;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 11.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(283, 150);
            label1.Name = "label1";
            label1.Size = new Size(36, 26);
            label1.TabIndex = 2;
            label1.Text = "50";
            // 
            // trackBar2
            // 
            trackBar2.Location = new Point(12, 242);
            trackBar2.Maximum = 18000;
            trackBar2.Minimum = 50;
            trackBar2.Name = "trackBar2";
            trackBar2.Size = new Size(601, 56);
            trackBar2.TabIndex = 1;
            trackBar2.TickFrequency = 400;
            trackBar2.Value = 50;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(233, 219);
            label2.Name = "label2";
            label2.Size = new Size(163, 20);
            label2.TabIndex = 2;
            label2.Text = "Speed of pitch change";
            // 
            // PortamentoWindow
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(625, 311);
            ControlBox = false;
            Controls.Add(label2);
            Controls.Add(trackBar2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PortamentoWindow";
            ShowIcon = false;
            Text = "Bleeper Portamento";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private TrackBar trackBar1;
        private Label label1;
        private TrackBar trackBar2;
        private Label label2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}