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
            ((System.ComponentModel.ISupportInitialize)pictureBoxCreating).BeginInit();
            SuspendLayout();
            // 
            // labelPrompt
            // 
            labelPrompt.Anchor = AnchorStyles.Top;
            labelPrompt.AutoSize = true;
            labelPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPrompt.Location = new Point(29, 57);
            labelPrompt.Margin = new Padding(2, 0, 2, 0);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(46, 16);
            labelPrompt.TabIndex = 0;
            labelPrompt.Text = "Prompt";
            // 
            // textBoxPrompt
            // 
            textBoxPrompt.Anchor = AnchorStyles.Top;
            textBoxPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            textBoxPrompt.Location = new Point(83, 55);
            textBoxPrompt.Margin = new Padding(2);
            textBoxPrompt.Name = "textBoxPrompt";
            textBoxPrompt.Size = new Size(231, 23);
            textBoxPrompt.TabIndex = 1;
            textBoxPrompt.TextChanged += textBox1_TextChanged;
            // 
            // buttonCreate
            // 
            buttonCreate.Anchor = AnchorStyles.Top;
            buttonCreate.Enabled = false;
            buttonCreate.Font = new Font("HarmonyOS Sans", 8.999999F);
            buttonCreate.ImageIndex = 1;
            buttonCreate.ImageList = images;
            buttonCreate.Location = new Point(318, 53);
            buttonCreate.Margin = new Padding(2);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(102, 26);
            buttonCreate.TabIndex = 2;
            buttonCreate.Text = "Create";
            buttonCreate.TextAlign = ContentAlignment.MiddleRight;
            buttonCreate.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += button1_Click;
            // 
            // images
            // 
            images.ColorDepth = ColorDepth.Depth32Bit;
            images.ImageStream = (ImageListStreamer)resources.GetObject("images.ImageStream");
            images.TransparentColor = Color.Transparent;
            images.Images.SetKeyName(0, "icons8-bard-48.png");
            images.Images.SetKeyName(1, "icons8-create-48.png");
            // 
            // labelPoweredByGemini
            // 
            labelPoweredByGemini.Anchor = AnchorStyles.Top;
            labelPoweredByGemini.AutoSize = true;
            labelPoweredByGemini.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPoweredByGemini.ImageAlign = ContentAlignment.MiddleLeft;
            labelPoweredByGemini.ImageIndex = 0;
            labelPoweredByGemini.ImageList = images;
            labelPoweredByGemini.Location = new Point(106, 96);
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
            comboBox_ai_model.Location = new Point(83, 21);
            comboBox_ai_model.Margin = new Padding(2);
            comboBox_ai_model.Name = "comboBox_ai_model";
            comboBox_ai_model.Size = new Size(231, 24);
            comboBox_ai_model.TabIndex = 4;
            comboBox_ai_model.SelectedIndexChanged += comboBox_ai_model_SelectedIndexChanged;
            // 
            // label_ai_model
            // 
            label_ai_model.Anchor = AnchorStyles.Top;
            label_ai_model.AutoSize = true;
            label_ai_model.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_ai_model.Location = new Point(22, 23);
            label_ai_model.Margin = new Padding(2, 0, 2, 0);
            label_ai_model.Name = "label_ai_model";
            label_ai_model.Size = new Size(55, 16);
            label_ai_model.TabIndex = 0;
            label_ai_model.Text = "AI Model";
            // 
            // progressBarCreating
            // 
            progressBarCreating.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            progressBarCreating.Location = new Point(11, 173);
            progressBarCreating.Margin = new Padding(2);
            progressBarCreating.MarqueeAnimationSpeed = 5;
            progressBarCreating.Name = "progressBarCreating";
            progressBarCreating.Size = new Size(409, 12);
            progressBarCreating.Style = ProgressBarStyle.Marquee;
            progressBarCreating.TabIndex = 7;
            progressBarCreating.Visible = false;
            // 
            // pictureBoxCreating
            // 
            pictureBoxCreating.Anchor = AnchorStyles.Top;
            pictureBoxCreating.Image = Properties.Resources.icons8_wait_96;
            pictureBoxCreating.Location = new Point(64, 127);
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
            labelCreating.Location = new Point(115, 139);
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
            // CreateMusicWithAI
            // 
            AcceptButton = buttonCreate;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(431, 126);
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
    }
}