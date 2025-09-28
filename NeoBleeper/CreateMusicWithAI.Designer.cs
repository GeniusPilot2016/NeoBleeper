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
            resources.ApplyResources(labelPrompt, "labelPrompt");
            labelPrompt.Name = "labelPrompt";
            // 
            // textBoxPrompt
            // 
            resources.ApplyResources(textBoxPrompt, "textBoxPrompt");
            textBoxPrompt.Name = "textBoxPrompt";
            // 
            // buttonCreate
            // 
            resources.ApplyResources(buttonCreate, "buttonCreate");
            buttonCreate.ImageList = images;
            buttonCreate.Name = "buttonCreate";
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
            resources.ApplyResources(labelPoweredByGemini, "labelPoweredByGemini");
            labelPoweredByGemini.ImageList = images;
            labelPoweredByGemini.Name = "labelPoweredByGemini";
            // 
            // comboBox_ai_model
            // 
            resources.ApplyResources(comboBox_ai_model, "comboBox_ai_model");
            comboBox_ai_model.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_ai_model.FormattingEnabled = true;
            comboBox_ai_model.Items.AddRange(new object[] { resources.GetString("comboBox_ai_model.Items"), resources.GetString("comboBox_ai_model.Items1"), resources.GetString("comboBox_ai_model.Items2"), resources.GetString("comboBox_ai_model.Items3") });
            comboBox_ai_model.Name = "comboBox_ai_model";
            comboBox_ai_model.SelectedIndexChanged += comboBox_ai_model_SelectedIndexChanged;
            // 
            // label_ai_model
            // 
            resources.ApplyResources(label_ai_model, "label_ai_model");
            label_ai_model.Name = "label_ai_model";
            // 
            // progressBarCreating
            // 
            resources.ApplyResources(progressBarCreating, "progressBarCreating");
            progressBarCreating.MarqueeAnimationSpeed = 5;
            progressBarCreating.Name = "progressBarCreating";
            progressBarCreating.Style = ProgressBarStyle.Marquee;
            // 
            // pictureBoxCreating
            // 
            resources.ApplyResources(pictureBoxCreating, "pictureBoxCreating");
            pictureBoxCreating.Image = Properties.Resources.icons8_wait_96;
            pictureBoxCreating.Name = "pictureBoxCreating";
            pictureBoxCreating.TabStop = false;
            // 
            // labelCreating
            // 
            resources.ApplyResources(labelCreating, "labelCreating");
            labelCreating.Name = "labelCreating";
            // 
            // connectionCheckTimer
            // 
            connectionCheckTimer.Interval = 5000;
            connectionCheckTimer.Tick += connectionCheckTimer_Tick;
            // 
            // labelWarning
            // 
            resources.ApplyResources(labelWarning, "labelWarning");
            labelWarning.ImageList = images;
            labelWarning.Name = "labelWarning";
            // 
            // CreateMusicWithAI
            // 
            AcceptButton = buttonCreate;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
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
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateMusicWithAI";
            ShowIcon = false;
            ShowInTaskbar = false;
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