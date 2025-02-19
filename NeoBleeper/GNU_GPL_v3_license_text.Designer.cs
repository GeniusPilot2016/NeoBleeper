namespace NeoBleeper
{
    partial class GNU_GPL_v3_license_text
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GNU_GPL_v3_license_text));
            richTextBox1 = new RichTextBox();
            close_button = new Button();
            icons = new ImageList(components);
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.BackColor = SystemColors.Window;
            richTextBox1.Font = new Font("HarmonyOS Sans", 8.95F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox1.Location = new Point(12, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(545, 577);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = resources.GetString("richTextBox1.Text");
            richTextBox1.LinkClicked += richTextBox1_LinkClicked;
            // 
            // close_button
            // 
            close_button.Anchor = AnchorStyles.Bottom;
            close_button.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            close_button.ImageIndex = 0;
            close_button.ImageList = icons;
            close_button.Location = new Point(229, 604);
            close_button.Name = "close_button";
            close_button.Size = new Size(94, 29);
            close_button.TabIndex = 1;
            close_button.Text = "Close";
            close_button.TextAlign = ContentAlignment.MiddleRight;
            close_button.TextImageRelation = TextImageRelation.ImageBeforeText;
            close_button.UseVisualStyleBackColor = true;
            close_button.Click += close_button_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-close-48 (1).png");
            // 
            // GNU_GPL_v3_license_text
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(569, 645);
            Controls.Add(close_button);
            Controls.Add(richTextBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GNU_GPL_v3_license_text";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "License";
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button close_button;
        private ImageList icons;
    }
}