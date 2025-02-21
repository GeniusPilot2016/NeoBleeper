namespace NeoBleeper
{
    partial class neobleeper_init_display_resolution_warning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(neobleeper_init_display_resolution_warning));
            button_close = new Button();
            imageList_display_resolution_warning = new ImageList(components);
            label_display_resolution_warning = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button_close
            // 
            button_close.AccessibleRole = AccessibleRole.None;
            button_close.Anchor = AnchorStyles.Bottom;
            button_close.AutoSize = true;
            button_close.DialogResult = DialogResult.Abort;
            button_close.Font = new Font("HarmonyOS Sans", 9F);
            button_close.ImageIndex = 0;
            button_close.ImageList = imageList_display_resolution_warning;
            button_close.Location = new Point(178, 65);
            button_close.Margin = new Padding(4);
            button_close.Name = "button_close";
            button_close.Size = new Size(165, 32);
            button_close.TabIndex = 5;
            button_close.Text = "&Close the program";
            button_close.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_close.UseVisualStyleBackColor = true;
            button_close.Click += button_close_the_program_Click;
            // 
            // imageList_display_resolution_warning
            // 
            imageList_display_resolution_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_display_resolution_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_display_resolution_warning.ImageStream");
            imageList_display_resolution_warning.TransparentColor = Color.Transparent;
            imageList_display_resolution_warning.Images.SetKeyName(0, "icons8-close-48.png");
            // 
            // label_display_resolution_warning
            // 
            label_display_resolution_warning.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_display_resolution_warning.AutoSize = true;
            label_display_resolution_warning.Font = new Font("HarmonyOS Sans", 9F);
            label_display_resolution_warning.Location = new Point(86, 15);
            label_display_resolution_warning.Margin = new Padding(4, 0, 4, 0);
            label_display_resolution_warning.Name = "label_display_resolution_warning";
            label_display_resolution_warning.Size = new Size(418, 40);
            label_display_resolution_warning.TabIndex = 4;
            label_display_resolution_warning.Text = "The screen resolution does not meet the screen resolution \r\nrequirements for the NeoBleeper program interface.";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_resolution_48;
            pictureBox1.Location = new Point(15, 15);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(60, 60);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // neobleeper_init_display_resolution_warning
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(510, 105);
            Controls.Add(pictureBox1);
            Controls.Add(button_close);
            Controls.Add(label_display_resolution_warning);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_display_resolution_warning";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button_close;
        private Label label_display_resolution_warning;
        private ImageList imageList_display_resolution_warning;
        private PictureBox pictureBox1;
    }
}