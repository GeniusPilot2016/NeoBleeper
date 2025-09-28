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
            resources.ApplyResources(richTextBox1, "richTextBox1");
            richTextBox1.BackColor = SystemColors.Window;
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.LinkClicked += richTextBox1_LinkClicked;
            // 
            // close_button
            // 
            resources.ApplyResources(close_button, "close_button");
            close_button.ImageList = icons;
            close_button.Name = "close_button";
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
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(close_button);
            Controls.Add(richTextBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GNU_GPL_v3_license_text";
            ShowIcon = false;
            ShowInTaskbar = false;
            SystemColorsChanged += GNU_GPL_v3_license_text_SystemColorsChanged;
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private Button close_button;
        private ImageList icons;
    }
}