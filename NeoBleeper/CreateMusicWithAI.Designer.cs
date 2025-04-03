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
            SuspendLayout();
            // 
            // labelPrompt
            // 
            labelPrompt.Anchor = AnchorStyles.None;
            labelPrompt.AutoSize = true;
            labelPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            labelPrompt.Location = new Point(24, 41);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(61, 20);
            labelPrompt.TabIndex = 0;
            labelPrompt.Text = "Prompt";
            // 
            // textBoxPrompt
            // 
            textBoxPrompt.Anchor = AnchorStyles.None;
            textBoxPrompt.Font = new Font("HarmonyOS Sans", 8.999999F);
            textBoxPrompt.Location = new Point(91, 38);
            textBoxPrompt.Name = "textBoxPrompt";
            textBoxPrompt.Size = new Size(288, 27);
            textBoxPrompt.TabIndex = 1;
            textBoxPrompt.TextChanged += textBox1_TextChanged;
            // 
            // buttonCreate
            // 
            buttonCreate.Anchor = AnchorStyles.None;
            buttonCreate.DialogResult = DialogResult.OK;
            buttonCreate.Enabled = false;
            buttonCreate.Font = new Font("HarmonyOS Sans", 8.999999F);
            buttonCreate.ImageIndex = 1;
            buttonCreate.ImageList = images;
            buttonCreate.Location = new Point(385, 35);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(128, 33);
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
            labelPoweredByGemini.Location = new Point(139, 89);
            labelPoweredByGemini.Name = "labelPoweredByGemini";
            labelPoweredByGemini.Size = new Size(224, 20);
            labelPoweredByGemini.TabIndex = 3;
            labelPoweredByGemini.Text = "    Powered by Google Gemini™";
            labelPoweredByGemini.TextAlign = ContentAlignment.MiddleRight;
            // 
            // CreateMusicWithAI
            // 
            AcceptButton = buttonCreate;
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(539, 135);
            Controls.Add(labelPoweredByGemini);
            Controls.Add(buttonCreate);
            Controls.Add(textBoxPrompt);
            Controls.Add(labelPrompt);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateMusicWithAI";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Create Music With AI";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelPrompt;
        private TextBox textBoxPrompt;
        private Button buttonCreate;
        private Label labelPoweredByGemini;
        private ImageList images;
    }
}