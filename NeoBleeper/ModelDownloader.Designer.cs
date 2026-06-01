namespace NeoBleeper
{
    partial class ModelDownloader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelDownloader));
            AIModelDownloadWorker = new System.ComponentModel.BackgroundWorker();
            progressBar1 = new ProgressBar();
            label1 = new Label();
            button1 = new Button();
            icons = new ImageList(components);
            label2 = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // AIModelDownloadWorker
            // 
            AIModelDownloadWorker.WorkerSupportsCancellation = true;
            AIModelDownloadWorker.DoWork += AIModelDownloadWorker_DoWork;
            AIModelDownloadWorker.RunWorkerCompleted += AIModelDownloadWorker_RunWorkerCompleted;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 65);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(545, 23);
            progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(60, 9);
            label1.Name = "label1";
            label1.Size = new Size(454, 44);
            label1.TabIndex = 1;
            label1.Text = "{modelName} is being downloaded... Please wait.";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom;
            button1.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.ImageIndex = 0;
            button1.ImageList = icons;
            button1.Location = new Point(244, 103);
            button1.Name = "button1";
            button1.Size = new Size(80, 25);
            button1.TabIndex = 2;
            button1.Text = "Cancel";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-cancel-48.png");
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label2.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(353, 96);
            label2.Name = "label2";
            label2.Size = new Size(204, 23);
            label2.TabIndex = 3;
            label2.Text = "Remaining time is being calculated";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_wait_96;
            pictureBox1.Location = new Point(12, 9);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(44, 44);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // ModelDownloader
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(569, 140);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(progressBar1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ModelDownloader";
            ShowIcon = false;
            Text = "Download AI Model";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker AIModelDownloadWorker;
        private ProgressBar progressBar1;
        private Label label1;
        private Button button1;
        private Label label2;
        private ImageList icons;
        private PictureBox pictureBox1;
    }
}