namespace NeoBleeper
{
    partial class AIModelManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AIModelManager));
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            label3 = new Label();
            button4 = new Button();
            icons = new ImageList(components);
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            label2 = new Label();
            checkedListBox1 = new CheckedListBox();
            textBox1 = new TextBox();
            label1 = new Label();
            groupBoxCreateMusicWithAI = new GroupBox();
            buttonResetAPIKey = new Button();
            buttonUpdateAPIKey = new Button();
            buttonShowHide = new Button();
            labelAPIKeyWarning = new Label();
            labelAPIKey = new Label();
            textBoxAPIKey = new TextBox();
            labelGoogleGeminiAPIWarning = new Label();
            modelsRightClickMenu = new ContextMenuStrip(components);
            removeModelToolStripMenuItem = new ToolStripMenuItem();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBoxCreateMusicWithAI.SuspendLayout();
            modelsRightClickMenu.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBoxCreateMusicWithAI);
            flowLayoutPanel1.Controls.Add(labelGoogleGeminiAPIWarning);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Padding = new Padding(5);
            flowLayoutPanel1.Size = new Size(423, 556);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.BackColor = Color.Transparent;
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(checkedListBox1);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(8, 8);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(407, 305);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ollama Settings (Experimental)";
            // 
            // label3
            // 
            label3.BackColor = SystemColors.Window;
            label3.Enabled = false;
            label3.Location = new Point(21, 161);
            label3.Name = "label3";
            label3.Size = new Size(360, 67);
            label3.TabIndex = 8;
            label3.Text = "No installed local AI model is found";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom;
            button4.ImageIndex = 2;
            button4.ImageList = icons;
            button4.Location = new Point(42, 271);
            button4.Name = "button4";
            button4.Size = new Size(162, 25);
            button4.TabIndex = 7;
            button4.Text = "Download New Model";
            button4.TextAlign = ContentAlignment.MiddleRight;
            button4.TextImageRelation = TextImageRelation.ImageBeforeText;
            button4.UseVisualStyleBackColor = true;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-url-48.png");
            icons.Images.SetKeyName(1, "icons8-reset-48.png");
            icons.Images.SetKeyName(2, "icons8-software-installer-48.png");
            icons.Images.SetKeyName(3, "icons8-refresh-48.png");
            icons.Images.SetKeyName(4, "icons8-mark-view-as-non-hidden-48.png");
            icons.Images.SetKeyName(5, "icons8-mark-view-as-hidden-48.png");
            icons.Images.SetKeyName(6, "icons8-warning-48.png");
            icons.Images.SetKeyName(7, "icons8-update-48.png");
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top;
            button3.ImageIndex = 1;
            button3.ImageList = icons;
            button3.Location = new Point(199, 76);
            button3.Name = "button3";
            button3.Size = new Size(140, 25);
            button3.TabIndex = 6;
            button3.Text = "Reset Client URL";
            button3.TextAlign = ContentAlignment.MiddleRight;
            button3.TextImageRelation = TextImageRelation.ImageBeforeText;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom;
            button2.ImageIndex = 3;
            button2.ImageList = icons;
            button2.Location = new Point(210, 271);
            button2.Name = "button2";
            button2.Size = new Size(149, 25);
            button2.TabIndex = 5;
            button2.Text = "Refresh Models List";
            button2.TextAlign = ContentAlignment.MiddleRight;
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top;
            button1.ImageIndex = 0;
            button1.ImageList = icons;
            button1.Location = new Point(53, 76);
            button1.Name = "button1";
            button1.Size = new Size(140, 25);
            button1.TabIndex = 4;
            button1.Text = "Update Client URL";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 110);
            label2.Name = "label2";
            label2.Size = new Size(88, 16);
            label2.TabIndex = 3;
            label2.Text = "Ollama Models";
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            checkedListBox1.Enabled = false;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(10, 129);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(384, 130);
            checkedListBox1.TabIndex = 2;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(10, 47);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(384, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "http://localhost:11434";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 28);
            label1.Name = "label1";
            label1.Size = new Size(108, 16);
            label1.TabIndex = 0;
            label1.Text = "Ollama Client URL";
            // 
            // groupBoxCreateMusicWithAI
            // 
            groupBoxCreateMusicWithAI.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            groupBoxCreateMusicWithAI.Controls.Add(buttonResetAPIKey);
            groupBoxCreateMusicWithAI.Controls.Add(buttonUpdateAPIKey);
            groupBoxCreateMusicWithAI.Controls.Add(buttonShowHide);
            groupBoxCreateMusicWithAI.Controls.Add(labelAPIKeyWarning);
            groupBoxCreateMusicWithAI.Controls.Add(labelAPIKey);
            groupBoxCreateMusicWithAI.Controls.Add(textBoxAPIKey);
            groupBoxCreateMusicWithAI.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBoxCreateMusicWithAI.Location = new Point(8, 319);
            groupBoxCreateMusicWithAI.Name = "groupBoxCreateMusicWithAI";
            groupBoxCreateMusicWithAI.Size = new Size(407, 179);
            groupBoxCreateMusicWithAI.TabIndex = 12;
            groupBoxCreateMusicWithAI.TabStop = false;
            groupBoxCreateMusicWithAI.Text = "Google Gemini™ Settings";
            // 
            // buttonResetAPIKey
            // 
            buttonResetAPIKey.Anchor = AnchorStyles.Bottom;
            buttonResetAPIKey.Enabled = false;
            buttonResetAPIKey.ImageIndex = 1;
            buttonResetAPIKey.ImageList = icons;
            buttonResetAPIKey.ImeMode = ImeMode.NoControl;
            buttonResetAPIKey.Location = new Point(98, 144);
            buttonResetAPIKey.Margin = new Padding(2);
            buttonResetAPIKey.Name = "buttonResetAPIKey";
            buttonResetAPIKey.Size = new Size(211, 26);
            buttonResetAPIKey.TabIndex = 4;
            buttonResetAPIKey.Text = "Reset Google Gemini™ API Key";
            buttonResetAPIKey.TextAlign = ContentAlignment.MiddleRight;
            buttonResetAPIKey.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonResetAPIKey.UseVisualStyleBackColor = true;
            // 
            // buttonUpdateAPIKey
            // 
            buttonUpdateAPIKey.Anchor = AnchorStyles.Bottom;
            buttonUpdateAPIKey.Enabled = false;
            buttonUpdateAPIKey.ImageIndex = 7;
            buttonUpdateAPIKey.ImageList = icons;
            buttonUpdateAPIKey.ImeMode = ImeMode.NoControl;
            buttonUpdateAPIKey.Location = new Point(89, 115);
            buttonUpdateAPIKey.Margin = new Padding(2);
            buttonUpdateAPIKey.Name = "buttonUpdateAPIKey";
            buttonUpdateAPIKey.Size = new Size(228, 26);
            buttonUpdateAPIKey.TabIndex = 4;
            buttonUpdateAPIKey.Text = "Update Google Gemini™ API Key";
            buttonUpdateAPIKey.TextAlign = ContentAlignment.MiddleRight;
            buttonUpdateAPIKey.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonUpdateAPIKey.UseVisualStyleBackColor = true;
            // 
            // buttonShowHide
            // 
            buttonShowHide.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonShowHide.ImageIndex = 4;
            buttonShowHide.ImageList = icons;
            buttonShowHide.ImeMode = ImeMode.NoControl;
            buttonShowHide.Location = new Point(324, 43);
            buttonShowHide.Margin = new Padding(2);
            buttonShowHide.Name = "buttonShowHide";
            buttonShowHide.Size = new Size(70, 28);
            buttonShowHide.TabIndex = 3;
            buttonShowHide.Text = "Show";
            buttonShowHide.TextAlign = ContentAlignment.MiddleRight;
            buttonShowHide.TextImageRelation = TextImageRelation.ImageBeforeText;
            buttonShowHide.UseVisualStyleBackColor = true;
            // 
            // labelAPIKeyWarning
            // 
            labelAPIKeyWarning.Anchor = AnchorStyles.None;
            labelAPIKeyWarning.ImageAlign = ContentAlignment.TopLeft;
            labelAPIKeyWarning.ImageIndex = 6;
            labelAPIKeyWarning.ImageList = icons;
            labelAPIKeyWarning.ImeMode = ImeMode.NoControl;
            labelAPIKeyWarning.Location = new Point(5, 75);
            labelAPIKeyWarning.Margin = new Padding(2, 0, 2, 0);
            labelAPIKeyWarning.Name = "labelAPIKeyWarning";
            labelAPIKeyWarning.Size = new Size(397, 38);
            labelAPIKeyWarning.TabIndex = 2;
            labelAPIKeyWarning.Text = "     Warning: For your security, do not share your Google Gemini™ API key with anyone else.";
            labelAPIKeyWarning.TextAlign = ContentAlignment.TopCenter;
            // 
            // labelAPIKey
            // 
            labelAPIKey.AutoSize = true;
            labelAPIKey.ImeMode = ImeMode.NoControl;
            labelAPIKey.Location = new Point(10, 27);
            labelAPIKey.Margin = new Padding(2, 0, 2, 0);
            labelAPIKey.Name = "labelAPIKey";
            labelAPIKey.Size = new Size(142, 16);
            labelAPIKey.TabIndex = 1;
            labelAPIKey.Text = "Google Gemini™ API Key";
            // 
            // textBoxAPIKey
            // 
            textBoxAPIKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxAPIKey.Location = new Point(10, 46);
            textBoxAPIKey.Margin = new Padding(2);
            textBoxAPIKey.Name = "textBoxAPIKey";
            textBoxAPIKey.PlaceholderText = "Enter the Google Gemini™ API key to here...";
            textBoxAPIKey.Size = new Size(310, 23);
            textBoxAPIKey.TabIndex = 0;
            textBoxAPIKey.UseSystemPasswordChar = true;
            // 
            // labelGoogleGeminiAPIWarning
            // 
            labelGoogleGeminiAPIWarning.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelGoogleGeminiAPIWarning.ImageAlign = ContentAlignment.TopLeft;
            labelGoogleGeminiAPIWarning.ImageIndex = 6;
            labelGoogleGeminiAPIWarning.ImageList = icons;
            labelGoogleGeminiAPIWarning.ImeMode = ImeMode.NoControl;
            labelGoogleGeminiAPIWarning.Location = new Point(8, 501);
            labelGoogleGeminiAPIWarning.Name = "labelGoogleGeminiAPIWarning";
            labelGoogleGeminiAPIWarning.Size = new Size(407, 51);
            labelGoogleGeminiAPIWarning.TabIndex = 14;
            labelGoogleGeminiAPIWarning.Text = "    In your region (European Economic Area, Switzerland, or United Kingdom), the Google Gemini™ API may only be available as a \r\npaid service. Please ensure your API key is for a paid account.";
            labelGoogleGeminiAPIWarning.TextAlign = ContentAlignment.TopCenter;
            labelGoogleGeminiAPIWarning.Visible = false;
            // 
            // modelsRightClickMenu
            // 
            modelsRightClickMenu.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            modelsRightClickMenu.Items.AddRange(new ToolStripItem[] { removeModelToolStripMenuItem });
            modelsRightClickMenu.Name = "modelsRightClickMenu";
            modelsRightClickMenu.Size = new Size(157, 26);
            // 
            // removeModelToolStripMenuItem
            // 
            removeModelToolStripMenuItem.Image = Properties.Resources.icons8_delete_48;
            removeModelToolStripMenuItem.Name = "removeModelToolStripMenuItem";
            removeModelToolStripMenuItem.Size = new Size(156, 22);
            removeModelToolStripMenuItem.Text = "Remove Model";
            // 
            // AIModelManager
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(423, 556);
            Controls.Add(flowLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AIModelManager";
            ShowIcon = false;
            Text = "AI Model Manager";
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBoxCreateMusicWithAI.ResumeLayout(false);
            groupBoxCreateMusicWithAI.PerformLayout();
            modelsRightClickMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox groupBox1;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private CheckedListBox checkedListBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private ImageList icons;
        private GroupBox groupBoxCreateMusicWithAI;
        private Button buttonResetAPIKey;
        private Button buttonUpdateAPIKey;
        private Button buttonShowHide;
        private Label labelAPIKeyWarning;
        private Label labelAPIKey;
        private TextBox textBoxAPIKey;
        private Label labelGoogleGeminiAPIWarning;
        private Button button4;
        private ContextMenuStrip modelsRightClickMenu;
        private ToolStripMenuItem removeModelToolStripMenuItem;
        private Label label3;
    }
}