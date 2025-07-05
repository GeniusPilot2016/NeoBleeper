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
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(labelLength);
            groupBox1.Controls.Add(trackBarLength);
            groupBox1.Controls.Add(radioButtonProduceSoundForManyMilliseconds);
            groupBox1.Controls.Add(radioButtonAlwaysProduceSound);
            groupBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(12, 11);
            groupBox1.Margin = new Padding(2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(2);
            groupBox1.Size = new Size(457, 140);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Length of sound";
            // 
            // labelLength
            // 
            labelLength.Anchor = AnchorStyles.None;
            labelLength.AutoSize = true;
            labelLength.Font = new Font("HarmonyOS Sans", 11.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelLength.Location = new Point(204, 110);
            labelLength.Margin = new Padding(2, 0, 2, 0);
            labelLength.Name = "labelLength";
            labelLength.Size = new Size(67, 21);
            labelLength.TabIndex = 2;
            labelLength.Text = "250 mS";
            labelLength.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // trackBarLength
            // 
            trackBarLength.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            trackBarLength.Location = new Point(15, 76);
            trackBarLength.Margin = new Padding(2);
            trackBarLength.Maximum = 1000;
            trackBarLength.Minimum = 50;
            trackBarLength.Name = "trackBarLength";
            trackBarLength.Size = new Size(426, 45);
            trackBarLength.TabIndex = 1;
            trackBarLength.TickFrequency = 50;
            trackBarLength.Value = 250;
            trackBarLength.Scroll += trackBarLength_Scroll;
            // 
            // radioButtonProduceSoundForManyMilliseconds
            // 
            radioButtonProduceSoundForManyMilliseconds.Anchor = AnchorStyles.Left;
            radioButtonProduceSoundForManyMilliseconds.AutoSize = true;
            radioButtonProduceSoundForManyMilliseconds.Location = new Point(15, 49);
            radioButtonProduceSoundForManyMilliseconds.Margin = new Padding(2);
            radioButtonProduceSoundForManyMilliseconds.Name = "radioButtonProduceSoundForManyMilliseconds";
            radioButtonProduceSoundForManyMilliseconds.Size = new Size(422, 20);
            radioButtonProduceSoundForManyMilliseconds.TabIndex = 0;
            radioButtonProduceSoundForManyMilliseconds.Text = "System speaker/sound produces sound for roughly this many milliseconds";
            radioButtonProduceSoundForManyMilliseconds.UseVisualStyleBackColor = true;
            // 
            // radioButtonAlwaysProduceSound
            // 
            radioButtonAlwaysProduceSound.Anchor = AnchorStyles.Left;
            radioButtonAlwaysProduceSound.AutoSize = true;
            radioButtonAlwaysProduceSound.Checked = true;
            radioButtonAlwaysProduceSound.Location = new Point(15, 25);
            radioButtonAlwaysProduceSound.Margin = new Padding(2);
            radioButtonAlwaysProduceSound.Name = "radioButtonAlwaysProduceSound";
            radioButtonAlwaysProduceSound.Size = new Size(315, 20);
            radioButtonAlwaysProduceSound.TabIndex = 0;
            radioButtonAlwaysProduceSound.TabStop = true;
            radioButtonAlwaysProduceSound.Text = "System speaker/sound device always produces sound";
            radioButtonAlwaysProduceSound.UseVisualStyleBackColor = true;
            // 
            // trackBarPitchChangeSpeed
            // 
            trackBarPitchChangeSpeed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBarPitchChangeSpeed.Location = new Point(12, 181);
            trackBarPitchChangeSpeed.Margin = new Padding(2);
            trackBarPitchChangeSpeed.Maximum = 18000;
            trackBarPitchChangeSpeed.Minimum = 50;
            trackBarPitchChangeSpeed.Name = "trackBarPitchChangeSpeed";
            trackBarPitchChangeSpeed.Size = new Size(457, 45);
            trackBarPitchChangeSpeed.TabIndex = 1;
            trackBarPitchChangeSpeed.TickFrequency = 400;
            trackBarPitchChangeSpeed.Value = 12000;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(183, 160);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(130, 16);
            label2.TabIndex = 2;
            label2.Text = "Speed of pitch change";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PortamentoWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(480, 231);
            ControlBox = false;
            Controls.Add(label2);
            Controls.Add(trackBarPitchChangeSpeed);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
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