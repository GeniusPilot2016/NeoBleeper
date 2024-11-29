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
            button_yes = new Button();
            imageList_display_resolution_warning = new ImageList(components);
            label_display_resolution_warning = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button_yes
            // 
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button_yes.AutoSize = true;
            button_yes.DialogResult = DialogResult.Abort;
            button_yes.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button_yes.ImageIndex = 0;
            button_yes.ImageList = imageList_display_resolution_warning;
            button_yes.Location = new Point(142, 52);
            button_yes.Name = "button_yes";
            button_yes.Size = new Size(132, 26);
            button_yes.TabIndex = 5;
            button_yes.Text = "&Close the program";
            button_yes.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_close_the_program_Click;
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
            label_display_resolution_warning.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label_display_resolution_warning.Location = new Point(69, 12);
            label_display_resolution_warning.Name = "label_display_resolution_warning";
            label_display_resolution_warning.Size = new Size(325, 32);
            label_display_resolution_warning.TabIndex = 4;
            label_display_resolution_warning.Text = "The screen resolution does not meet the screen resolution \r\nrequirements for the NeoBleeper program interface.";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_resolution_48;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // neobleeper_init_display_resolution_warning
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(408, 84);
            Controls.Add(pictureBox1);
            Controls.Add(button_yes);
            Controls.Add(label_display_resolution_warning);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_display_resolution_warning";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Load += neobleeper_init_resolution_warning_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button_yes;
        private Label label_display_resolution_warning;
        private ImageList imageList_display_resolution_warning;
        private PictureBox pictureBox1;
    }
}