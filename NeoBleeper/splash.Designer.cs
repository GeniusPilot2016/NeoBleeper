namespace NeoBleeper
{
    partial class splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(splash));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            labelVersion = new Label();
            progressBar1 = new ProgressBar();
            progressTimer = new System.Windows.Forms.Timer(components);
            labelStatus = new Label();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.BackColor = Color.Transparent;
            label1.Name = "label1";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.neobleeper_icon;
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // labelVersion
            // 
            resources.ApplyResources(labelVersion, "labelVersion");
            labelVersion.BackColor = Color.Transparent;
            labelVersion.Name = "labelVersion";
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            // 
            // labelStatus
            // 
            resources.ApplyResources(labelStatus, "labelStatus");
            labelStatus.BackColor = Color.Transparent;
            labelStatus.Name = "labelStatus";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // splash
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            BackgroundImage = Properties.Resources.neobleeper_splash_background;
            ControlBox = false;
            Controls.Add(labelStatus);
            Controls.Add(progressBar1);
            Controls.Add(labelVersion);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(panel1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "splash";
            ShowIcon = false;
            ShowInTaskbar = false;
            TopMost = true;
            FormClosed += splash_FormClosed;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private PictureBox pictureBox1;
        private Label labelVersion;
        private ProgressBar progressBar1;
        private System.Windows.Forms.Timer progressTimer;
        private Label labelStatus;
        private Panel panel1;
    }
}