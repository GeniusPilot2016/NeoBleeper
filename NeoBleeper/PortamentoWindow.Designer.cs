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
            labelLength = new Label();
            trackBarLength = new TrackBar();
            radioButtonProduceSoundForManyMilliseconds = new RadioButton();
            radioButtonAlwaysProduceSound = new RadioButton();
            trackBarPitchChangeSpeed = new TrackBar();
            label2 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLength).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitchChangeSpeed).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(labelLength);
            groupBox1.Controls.Add(trackBarLength);
            groupBox1.Controls.Add(radioButtonProduceSoundForManyMilliseconds);
            groupBox1.Controls.Add(radioButtonAlwaysProduceSound);
            groupBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(601, 185);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Length of sound";
            // 
            // labelLength
            // 
            labelLength.AutoSize = true;
            labelLength.Font = new Font("HarmonyOS Sans", 11.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelLength.Location = new Point(263, 147);
            labelLength.Name = "labelLength";
            labelLength.Size = new Size(70, 26);
            labelLength.TabIndex = 2;
            labelLength.Text = "50 mS";
            // 
            // trackBarLength
            // 
            trackBarLength.Location = new Point(19, 110);
            trackBarLength.Maximum = 1000;
            trackBarLength.Minimum = 50;
            trackBarLength.Name = "trackBarLength";
            trackBarLength.Size = new Size(559, 56);
            trackBarLength.TabIndex = 1;
            trackBarLength.TickFrequency = 50;
            trackBarLength.Value = 50;
            trackBarLength.Scroll += trackBarLength_Scroll;
            // 
            // radioButtonProduceSoundForManyMilliseconds
            // 
            radioButtonProduceSoundForManyMilliseconds.AutoSize = true;
            radioButtonProduceSoundForManyMilliseconds.Location = new Point(19, 65);
            radioButtonProduceSoundForManyMilliseconds.Name = "radioButtonProduceSoundForManyMilliseconds";
            radioButtonProduceSoundForManyMilliseconds.Size = new Size(544, 24);
            radioButtonProduceSoundForManyMilliseconds.TabIndex = 0;
            radioButtonProduceSoundForManyMilliseconds.Text = "System speaker/sound produces sound for roughly this many milliseconds";
            radioButtonProduceSoundForManyMilliseconds.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlwaysProduceSound
            // 
            radioButtonAlwaysProduceSound.AutoSize = true;
            radioButtonAlwaysProduceSound.Checked = true;
            radioButtonAlwaysProduceSound.Location = new Point(19, 35);
            radioButtonAlwaysProduceSound.Name = "radioButtonAlwaysProduceSound";
            radioButtonAlwaysProduceSound.Size = new Size(400, 24);
            radioButtonAlwaysProduceSound.TabIndex = 0;
            radioButtonAlwaysProduceSound.TabStop = true;
            radioButtonAlwaysProduceSound.Text = "System speaker/sound device always produces sound";
            radioButtonAlwaysProduceSound.UseVisualStyleBackColor = true;
            // 
            // trackBarPitchChangeSpeed
            // 
            trackBarPitchChangeSpeed.Location = new Point(12, 242);
            trackBarPitchChangeSpeed.Maximum = 18000;
            trackBarPitchChangeSpeed.Minimum = 50;
            trackBarPitchChangeSpeed.Name = "trackBarPitchChangeSpeed";
            trackBarPitchChangeSpeed.Size = new Size(601, 56);
            trackBarPitchChangeSpeed.TabIndex = 1;
            trackBarPitchChangeSpeed.TickFrequency = 400;
            trackBarPitchChangeSpeed.Value = 50;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(228, 219);
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
            ClientSize = new Size(613, 299);
            ControlBox = false;
            Controls.Add(label2);
            Controls.Add(trackBarPitchChangeSpeed);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PortamentoWindow";
            ShowIcon = false;
            Text = "Bleeper Portamento";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLength).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitchChangeSpeed).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private RadioButton radioButtonProduceSoundForManyMilliseconds;
        private RadioButton radioButtonAlwaysProduceSound;
        private TrackBar trackBarLength;
        private Label labelLength;
        private TrackBar trackBarPitchChangeSpeed;
        private Label label2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
    }
}