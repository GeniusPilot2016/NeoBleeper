namespace NeoBleeper
{
    partial class Splash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            label1 = new Label();
            pictureBox1 = new PictureBox();
            labelVersion = new Label();
            progressBar1 = new ProgressBar();
            labelStatus = new Label();
            panel1 = new Panel();
            notifyIconNeoBleeper = new NotifyIcon(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.BackColor = Color.Transparent;
            label1.Name = "label1";
            label1.MouseDown += splash_MouseDown;
            label1.MouseMove += splash_MouseMove;
            label1.MouseUp += splash_MouseUp;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.neobleeper_icon;
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            pictureBox1.MouseDown += splash_MouseDown;
            pictureBox1.MouseMove += splash_MouseMove;
            pictureBox1.MouseUp += splash_MouseUp;
            // 
            // labelVersion
            // 
            resources.ApplyResources(labelVersion, "labelVersion");
            labelVersion.BackColor = Color.Transparent;
            labelVersion.Name = "labelVersion";
            labelVersion.MouseDown += splash_MouseDown;
            labelVersion.MouseMove += splash_MouseMove;
            labelVersion.MouseUp += splash_MouseUp;
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            progressBar1.MouseDown += splash_MouseDown;
            progressBar1.MouseMove += splash_MouseMove;
            progressBar1.MouseUp += splash_MouseUp;
            // 
            // labelStatus
            // 
            resources.ApplyResources(labelStatus, "labelStatus");
            labelStatus.BackColor = Color.Transparent;
            labelStatus.Name = "labelStatus";
            labelStatus.MouseDown += splash_MouseDown;
            labelStatus.MouseMove += splash_MouseMove;
            labelStatus.MouseUp += splash_MouseUp;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // notifyIconNeoBleeper
            // 
            resources.ApplyResources(notifyIconNeoBleeper, "notifyIconNeoBleeper");
            notifyIconNeoBleeper.BalloonTipClicked += notifyIconNeoBleeper_BalloonTipClicked;
            // 
            // Splash
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
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Splash";
            Deactivate += splash_Deactivate;
            FormClosed += splash_FormClosed;
            Load += splash_Load;
            MouseDown += splash_MouseDown;
            MouseMove += splash_MouseMove;
            MouseUp += splash_MouseUp;
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
        private NotifyIcon notifyIconNeoBleeper;
    }
}