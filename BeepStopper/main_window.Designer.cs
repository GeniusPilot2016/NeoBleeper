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
            resources.ApplyResources(warningLabel, "warningLabel");
            warningLabel.Name = "warningLabel";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_warning_48;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // instructionLabel
            // 
            resources.ApplyResources(instructionLabel, "instructionLabel");
            instructionLabel.Name = "instructionLabel";
            // 
            // stopBeepButton
            // 
            resources.ApplyResources(stopBeepButton, "stopBeepButton");
            stopBeepButton.ImageList = icons;
            stopBeepButton.Name = "stopBeepButton";
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
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(stopBeepButton);
            Controls.Add(pictureBox1);
            Controls.Add(instructionLabel);
            Controls.Add(warningLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "main_window";
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
