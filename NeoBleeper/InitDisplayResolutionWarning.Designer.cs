namespace NeoBleeper
{
    partial class InitDisplayResolutionWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitDisplayResolutionWarning));
            button_close = new Button();
            imageList_display_resolution_warning = new ImageList(components);
            label_display_resolution_warning = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button_close
            // 
            resources.ApplyResources(button_close, "button_close");
            button_close.AccessibleRole = AccessibleRole.None;
            button_close.DialogResult = DialogResult.Abort;
            button_close.ImageList = imageList_display_resolution_warning;
            button_close.Name = "button_close";
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
            resources.ApplyResources(label_display_resolution_warning, "label_display_resolution_warning");
            label_display_resolution_warning.Name = "label_display_resolution_warning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_resolution_48;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // neobleeper_init_display_resolution_warning
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = button_close;
            Controls.Add(pictureBox1);
            Controls.Add(button_close);
            Controls.Add(label_display_resolution_warning);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_display_resolution_warning";
            ShowIcon = false;
            Shown += neobleeper_init_display_resolution_warning_Shown;
            SystemColorsChanged += neobleeper_init_display_resolution_warning_SystemColorsChanged;
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