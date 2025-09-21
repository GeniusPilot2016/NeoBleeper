namespace NeoBleeper
{
    partial class CreateMusicWithAI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateMusicWithAI));
            labelPrompt = new Label();
            textBoxPrompt = new TextBox();
            buttonCreate = new Button();
            images = new ImageList(components);
            labelPoweredByGemini = new Label();
            comboBox_ai_model = new ComboBox();
            label_ai_model = new Label();
            progressBarCreating = new ProgressBar();
            pictureBoxCreating = new PictureBox();
            labelCreating = new Label();
            connectionCheckTimer = new System.Windows.Forms.Timer(components);
            labelWarning = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCreating).BeginInit();
            SuspendLayout();
            // 
            // labelPrompt
            // 
            labelPrompt.Anchor = AnchorStyles.Top;
            labelPrompt.AutoSize = true;
            labelPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPrompt.Location = new Point(31, 53);
            labelPrompt.Margin = new Padding(2, 0, 2, 0);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(46, 16);
            labelPrompt.TabIndex = 2;
            labelPrompt.Text = "Prompt";
            // 
            // textBoxPrompt
            // 
            textBoxPrompt.Anchor = AnchorStyles.Top;
            textBoxPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            textBoxPrompt.Location = new Point(85, 51);
            textBoxPrompt.Margin = new Padding(2);
            textBoxPrompt.Name = "textBoxPrompt";
            textBoxPrompt.Size = new Size(257, 23);
            textBoxPrompt.TabIndex = 3;
            // 
            // buttonCreate
            // 
            buttonCreate.Anchor = AnchorStyles.Top;
            buttonCreate.Font = new Font("HarmonyOS Sans", 8.999999F);
            buttonCreate.ImageIndex = 1;
            buttonCreate.ImageList = images;
            buttonCreate.Location = new Point(346, 49);
            buttonCreate.Margin = new Padding(2);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(102, 26);
            buttonCreate.TabIndex = 4;
            buttonCreate.Text = "Create";
            buttonCreate.TextAlign = ContentAlignment.MiddleRight;
            buttonCreate.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += buttonCreate_Click;
            // 
            // images
            // 
            images.ColorDepth = ColorDepth.Depth32Bit;
            images.ImageStream = (ImageListStreamer)resources.GetObject("images.ImageStream");
            images.TransparentColor = Color.Transparent;
            images.Images.SetKeyName(0, "icons8-bard-48.png");
            images.Images.SetKeyName(1, "icons8-create-48.png");
            images.Images.SetKeyName(2, "icons8-general-warning-sign-96.png");
            // 
            // labelPoweredByGemini
            // 
            labelPoweredByGemini.Anchor = AnchorStyles.Top;
            labelPoweredByGemini.AutoSize = true;
            labelPoweredByGemini.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPoweredByGemini.ImageAlign = ContentAlignment.MiddleLeft;
            labelPoweredByGemini.ImageIndex = 0;
            labelPoweredByGemini.ImageList = images;
            labelPoweredByGemini.Location = new Point(134, 140);
            labelPoweredByGemini.Margin = new Padding(2, 0, 2, 0);
            labelPoweredByGemini.Name = "labelPoweredByGemini";
            labelPoweredByGemini.Size = new Size(182, 16);
            labelPoweredByGemini.TabIndex = 3;
            labelPoweredByGemini.Text = "      Powered by Google Gemini™";
            labelPoweredByGemini.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboBox_ai_model
            // 
            comboBox_ai_model.Anchor = AnchorStyles.Top;
            comboBox_ai_model.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_ai_model.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBox_ai_model.FormattingEnabled = true;
            comboBox_ai_model.Items.AddRange(new object[] { "Gemini 2.5 Flash", "Gemini 2.5 Pro", "Gemini 2.0 Flash", "Gemini 2.0 Flash Lite" });
            comboBox_ai_model.Location = new Point(85, 17);
            comboBox_ai_model.Margin = new Padding(2);
            comboBox_ai_model.Name = "comboBox_ai_model";
            comboBox_ai_model.Size = new Size(257, 24);
            comboBox_ai_model.TabIndex = 1;
            comboBox_ai_model.SelectedIndexChanged += comboBox_ai_model_SelectedIndexChanged;
            // 
            // label_ai_model
            // 
            label_ai_model.Anchor = AnchorStyles.Top;
            label_ai_model.AutoSize = true;
            label_ai_model.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_ai_model.Location = new Point(24, 19);
            label_ai_model.Margin = new Padding(2, 0, 2, 0);
            label_ai_model.Name = "label_ai_model";
            label_ai_model.Size = new Size(55, 16);
            label_ai_model.TabIndex = 0;
            label_ai_model.Text = "AI Model";
            // 
            // progressBarCreating
            // 
            progressBarCreating.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarCreating.Location = new Point(11, 215);
            progressBarCreating.Margin = new Padding(2);
            progressBarCreating.MarqueeAnimationSpeed = 5;
            progressBarCreating.Name = "progressBarCreating";
            progressBarCreating.Size = new Size(448, 12);
            progressBarCreating.Style = ProgressBarStyle.Marquee;
            progressBarCreating.TabIndex = 7;
            progressBarCreating.Visible = false;
            // 
            // pictureBoxCreating
            // 
            pictureBoxCreating.Anchor = AnchorStyles.Top;
            pictureBoxCreating.Image = Properties.Resources.icons8_wait_96;
            pictureBoxCreating.Location = new Point(83, 173);
            pictureBoxCreating.Margin = new Padding(2);
            pictureBoxCreating.Name = "pictureBoxCreating";
            pictureBoxCreating.Size = new Size(38, 38);
            pictureBoxCreating.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCreating.TabIndex = 6;
            pictureBoxCreating.TabStop = false;
            pictureBoxCreating.Visible = false;
            // 
            // labelCreating
            // 
            labelCreating.Anchor = AnchorStyles.Top;
            labelCreating.AutoSize = true;
            labelCreating.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelCreating.Location = new Point(134, 182);
            labelCreating.Margin = new Padding(2, 0, 2, 0);
            labelCreating.Name = "labelCreating";
            labelCreating.Size = new Size(235, 16);
            labelCreating.TabIndex = 5;
            labelCreating.Text = "Music is being created by AI. Please wait...";
            labelCreating.TextAlign = ContentAlignment.MiddleCenter;
            labelCreating.Visible = false;
            // 
            // connectionCheckTimer
            // 
            connectionCheckTimer.Interval = 5000;
            connectionCheckTimer.Tick += connectionCheckTimer_Tick;
            // 
            // labelWarning
            // 
            labelWarning.AutoSize = true;
            labelWarning.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelWarning.ImageAlign = ContentAlignment.TopLeft;
            labelWarning.ImageIndex = 2;
            labelWarning.ImageList = images;
            labelWarning.Location = new Point(12, 91);
            labelWarning.MaximumSize = new Size(1024, 0);
            labelWarning.Name = "labelWarning";
            labelWarning.Size = new Size(446, 32);
            labelWarning.TabIndex = 8;
            labelWarning.Text = "      Warning: The AI generates inspirational suggestions, not exact reproductions. \r\nResults may be imperfect or contain mistakes.";
            labelWarning.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CreateMusicWithAI
            // 
            AcceptButton = buttonCreate;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(470, 171);
            Controls.Add(labelWarning);
            Controls.Add(progressBarCreating);
            Controls.Add(pictureBoxCreating);
            Controls.Add(labelCreating);
            Controls.Add(comboBox_ai_model);
            Controls.Add(labelPoweredByGemini);
            Controls.Add(buttonCreate);
            Controls.Add(textBoxPrompt);
            Controls.Add(label_ai_model);
            Controls.Add(labelPrompt);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateMusicWithAI";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Create Music with AI";
            FormClosed += CreateMusicWithAI_FormClosed;
            SystemColorsChanged += CreateMusicWithAI_SystemColorsChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBoxCreating).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPrompt;
        private TextBox textBoxPrompt;
        private Button buttonCreate;
        private Label labelPoweredByGemini;
        private ImageList images;
        private ComboBox comboBox_ai_model;
        private Label label_ai_model;
        private ProgressBar progressBarCreating;
        private PictureBox pictureBoxCreating;
        private Label labelCreating;
        private System.Windows.Forms.Timer connectionCheckTimer;
        private Label labelWarning;
    }
}