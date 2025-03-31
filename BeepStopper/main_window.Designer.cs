namespace BeepStopper
{
    partial class main_window
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main_window));
            warningLabel = new Label();
            pictureBox1 = new PictureBox();
            instructionLabel = new Label();
            stopBeepButton = new Button();
            icons = new ImageList(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // warningLabel
            // 
            warningLabel.Anchor = AnchorStyles.Top;
            warningLabel.AutoSize = true;
            warningLabel.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            warningLabel.Location = new Point(99, 29);
            warningLabel.MaximumSize = new Size(512, 0);
            warningLabel.Name = "warningLabel";
            warningLabel.Size = new Size(503, 60);
            warningLabel.TabIndex = 0;
            warningLabel.Text = "Warning: NeoBleeper Beep Stopper should only be used in cases where the beep does not stop due to the system losing control of the system speaker, such as when the program crashes or is force-quitted.";
            warningLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top;
            pictureBox1.Image = Properties.Resources.icons8_warning_48;
            pictureBox1.Location = new Point(45, 29);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // instructionLabel
            // 
            instructionLabel.Anchor = AnchorStyles.Top;
            instructionLabel.AutoSize = true;
            instructionLabel.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            instructionLabel.Location = new Point(124, 126);
            instructionLabel.MaximumSize = new Size(512, 0);
            instructionLabel.Name = "instructionLabel";
            instructionLabel.Size = new Size(431, 20);
            instructionLabel.TabIndex = 0;
            instructionLabel.Text = "Press \"Stop Beep\" button to stop beep from system speaker.";
            instructionLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // stopBeepButton
            // 
            stopBeepButton.Anchor = AnchorStyles.Bottom;
            stopBeepButton.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            stopBeepButton.ImageIndex = 0;
            stopBeepButton.ImageList = icons;
            stopBeepButton.Location = new Point(263, 173);
            stopBeepButton.Name = "stopBeepButton";
            stopBeepButton.Size = new Size(141, 46);
            stopBeepButton.TabIndex = 2;
            stopBeepButton.Text = "Stop Beep";
            stopBeepButton.TextAlign = ContentAlignment.MiddleRight;
            stopBeepButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            stopBeepButton.UseVisualStyleBackColor = true;
            stopBeepButton.Click += button1_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-mute-48.png");
            // 
            // main_window
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(669, 231);
            Controls.Add(stopBeepButton);
            Controls.Add(pictureBox1);
            Controls.Add(instructionLabel);
            Controls.Add(warningLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "main_window";
            Text = "NeoBleeper Beep Stopper";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label warningLabel;
        private PictureBox pictureBox1;
        private Label instructionLabel;
        private Button stopBeepButton;
        private ImageList icons;
    }
}
