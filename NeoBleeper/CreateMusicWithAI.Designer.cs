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
            SuspendLayout();
            // 
            // labelPrompt
            // 
            labelPrompt.Anchor = AnchorStyles.None;
            labelPrompt.AutoSize = true;
            labelPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPrompt.Location = new Point(20, 54);
            labelPrompt.Margin = new Padding(2, 0, 2, 0);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(46, 16);
            labelPrompt.TabIndex = 0;
            labelPrompt.Text = "Prompt";
            // 
            // textBoxPrompt
            // 
            textBoxPrompt.Anchor = AnchorStyles.None;
            textBoxPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            textBoxPrompt.Location = new Point(74, 52);
            textBoxPrompt.Margin = new Padding(2);
            textBoxPrompt.Name = "textBoxPrompt";
            textBoxPrompt.Size = new Size(231, 23);
            textBoxPrompt.TabIndex = 1;
            textBoxPrompt.TextChanged += textBox1_TextChanged;
            // 
            // buttonCreate
            // 
            buttonCreate.Anchor = AnchorStyles.None;
            buttonCreate.Enabled = false;
            buttonCreate.Font = new Font("HarmonyOS Sans", 8.999999F);
            buttonCreate.ImageIndex = 1;
            buttonCreate.ImageList = images;
            buttonCreate.Location = new Point(309, 50);
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
            labelPoweredByGemini.Anchor = AnchorStyles.None;
            labelPoweredByGemini.AutoSize = true;
            labelPoweredByGemini.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPoweredByGemini.ImageAlign = ContentAlignment.MiddleLeft;
            labelPoweredByGemini.ImageIndex = 0;
            labelPoweredByGemini.ImageList = images;
            labelPoweredByGemini.Location = new Point(112, 93);
            labelPoweredByGemini.Margin = new Padding(2, 0, 2, 0);
            labelPoweredByGemini.Name = "labelPoweredByGemini";
            labelPoweredByGemini.Size = new Size(182, 16);
            labelPoweredByGemini.TabIndex = 3;
            labelPoweredByGemini.Text = "      Powered by Google Gemini™";
            labelPoweredByGemini.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboBox_ai_model
            // 
            comboBox_ai_model.Anchor = AnchorStyles.None;
            comboBox_ai_model.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_ai_model.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            comboBox_ai_model.FormattingEnabled = true;
            comboBox_ai_model.Items.AddRange(new object[] { "Gemini 2.5 Flash", "Gemini 2.5 Pro", "Gemini 2.0 Flash", "Gemini 2.0 Flash Lite" });
            comboBox_ai_model.Location = new Point(74, 18);
            comboBox_ai_model.Margin = new Padding(2, 2, 2, 2);
            comboBox_ai_model.Name = "comboBox_ai_model";
            comboBox_ai_model.Size = new Size(231, 24);
            comboBox_ai_model.TabIndex = 4;
            comboBox_ai_model.SelectedIndexChanged += comboBox_ai_model_SelectedIndexChanged;
            // 
            // label_ai_model
            // 
            label_ai_model.Anchor = AnchorStyles.None;
            label_ai_model.AutoSize = true;
            label_ai_model.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_ai_model.Location = new Point(13, 20);
            label_ai_model.Margin = new Padding(2, 0, 2, 0);
            label_ai_model.Name = "label_ai_model";
            label_ai_model.Size = new Size(55, 16);
            label_ai_model.TabIndex = 0;
            label_ai_model.Text = "AI Model";
            // 
            // CreateMusicWithAI
            // 
            AcceptButton = buttonCreate;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(431, 126);
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
            Load += CreateMusicWithAI_Load;
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
    }
}