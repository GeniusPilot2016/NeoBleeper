namespace NeoBleeper
{
    partial class CrashReportingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrashReportingForm));
            richTextBoxCrashReport = new RichTextBox();
            buttonCopyCrashReport = new Button();
            icons = new ImageList(components);
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // richTextBoxCrashReport
            // 
            resources.ApplyResources(richTextBoxCrashReport, "richTextBoxCrashReport");
            richTextBoxCrashReport.BackColor = SystemColors.Window;
            richTextBoxCrashReport.Name = "richTextBoxCrashReport";
            richTextBoxCrashReport.ReadOnly = true;
            richTextBoxCrashReport.LinkClicked += richTextBoxCrashReport_LinkClicked;
            // 
            // buttonCopyCrashReport
            // 
            resources.ApplyResources(buttonCopyCrashReport, "buttonCopyCrashReport");
            buttonCopyCrashReport.ImageList = icons;
            buttonCopyCrashReport.Name = "buttonCopyCrashReport";
            buttonCopyCrashReport.UseVisualStyleBackColor = true;
            buttonCopyCrashReport.Click += buttonCopyCrashReport_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-copy-to-clipboard-48.png");
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_error_481;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // CrashReportingForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel1);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Controls.Add(buttonCopyCrashReport);
            Controls.Add(richTextBoxCrashReport);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CrashReportingForm";
            ShowIcon = false;
            SystemColorsChanged += CrashReportingForm_SystemColorsChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBoxCrashReport;
        private Button buttonCopyCrashReport;
        private Label label1;
        private PictureBox pictureBox1;
        private ImageList icons;
        private Panel panel1;
    }
}