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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PortamentoWindow));
            groupBox1 = new GroupBox();
            labelLength = new Label();
            trackBarLength = new TrackBar();
            radioButtonProduceSoundForManyMilliseconds = new RadioButton();
            radioButtonAlwaysProduceSound = new RadioButton();
            trackBarPitchChangeSpeed = new TrackBar();
            label2 = new Label();
            finishTimer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarLength).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitchChangeSpeed).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(labelLength);
            groupBox1.Controls.Add(trackBarLength);
            groupBox1.Controls.Add(radioButtonProduceSoundForManyMilliseconds);
            groupBox1.Controls.Add(radioButtonAlwaysProduceSound);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // labelLength
            // 
            resources.ApplyResources(labelLength, "labelLength");
            labelLength.Name = "labelLength";
            // 
            // trackBarLength
            // 
            resources.ApplyResources(trackBarLength, "trackBarLength");
            trackBarLength.Maximum = 1000;
            trackBarLength.Minimum = 50;
            trackBarLength.Name = "trackBarLength";
            trackBarLength.TickFrequency = 50;
            trackBarLength.Value = 250;
            trackBarLength.Scroll += trackBarLength_Scroll;
            // 
            // radioButtonProduceSoundForManyMilliseconds
            // 
            resources.ApplyResources(radioButtonProduceSoundForManyMilliseconds, "radioButtonProduceSoundForManyMilliseconds");
            radioButtonProduceSoundForManyMilliseconds.Checked = true;
            radioButtonProduceSoundForManyMilliseconds.Name = "radioButtonProduceSoundForManyMilliseconds";
            radioButtonProduceSoundForManyMilliseconds.TabStop = true;
            radioButtonProduceSoundForManyMilliseconds.UseVisualStyleBackColor = true;
            radioButtonProduceSoundForManyMilliseconds.CheckedChanged += radioButtons_Checked_Changed;
            // 
            // radioButtonAlwaysProduceSound
            // 
            resources.ApplyResources(radioButtonAlwaysProduceSound, "radioButtonAlwaysProduceSound");
            radioButtonAlwaysProduceSound.Name = "radioButtonAlwaysProduceSound";
            radioButtonAlwaysProduceSound.UseVisualStyleBackColor = true;
            radioButtonAlwaysProduceSound.CheckedChanged += radioButtons_Checked_Changed;
            // 
            // trackBarPitchChangeSpeed
            // 
            resources.ApplyResources(trackBarPitchChangeSpeed, "trackBarPitchChangeSpeed");
            trackBarPitchChangeSpeed.Maximum = 18000;
            trackBarPitchChangeSpeed.Minimum = 50;
            trackBarPitchChangeSpeed.Name = "trackBarPitchChangeSpeed";
            trackBarPitchChangeSpeed.TickFrequency = 400;
            trackBarPitchChangeSpeed.Value = 12000;
            trackBarPitchChangeSpeed.Scroll += trackBarPitchChangeSpeed_Scroll;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // finishTimer
            // 
            finishTimer.Enabled = true;
            finishTimer.Interval = 250;
            finishTimer.Tick += finishTimer_Tick;
            // 
            // PortamentoWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            ControlBox = false;
            Controls.Add(label2);
            Controls.Add(trackBarPitchChangeSpeed);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PortamentoWindow";
            ShowIcon = false;
            SystemColorsChanged += PortamentoWindow_SystemColorsChanged;
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
        public System.Windows.Forms.Timer finishTimer;
    }
}