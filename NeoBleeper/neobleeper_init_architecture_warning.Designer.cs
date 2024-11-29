namespace NeoBleeper
{
    partial class neobleeper_init_architecture_warning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(neobleeper_init_architecture_warning));
            label_architecture_warning_result = new Label();
            button_no = new Button();
            imageList_architecuture_warning = new ImageList(components);
            button_yes = new Button();
            label_architecture_warning = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label_architecture_warning_result
            // 
            label_architecture_warning_result.Anchor = AnchorStyles.None;
            label_architecture_warning_result.AutoSize = true;
            label_architecture_warning_result.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label_architecture_warning_result.Location = new Point(3, 112);
            label_architecture_warning_result.Name = "label_architecture_warning_result";
            label_architecture_warning_result.Size = new Size(341, 64);
            label_architecture_warning_result.TabIndex = 11;
            label_architecture_warning_result.Text = "If you want to continue using the application, you can only \r\nuse the \"Use sound card to generate beeps\" setting to \r\ngenerate beeps and this setting cannot be disabled in \r\nyour computer.\r\n";
            // 
            // button_no
            // 
            button_no.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_no.AutoSize = true;
            button_no.DialogResult = DialogResult.No;
            button_no.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button_no.ImageIndex = 0;
            button_no.ImageList = imageList_architecuture_warning;
            button_no.Location = new Point(200, 189);
            button_no.Name = "button_no";
            button_no.Size = new Size(75, 26);
            button_no.TabIndex = 10;
            button_no.Text = "&No";
            button_no.TextAlign = ContentAlignment.MiddleRight;
            button_no.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // imageList_architecuture_warning
            // 
            imageList_architecuture_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_architecuture_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_architecuture_warning.ImageStream");
            imageList_architecuture_warning.TransparentColor = Color.Transparent;
            imageList_architecuture_warning.Images.SetKeyName(0, "icons8-no-48.png");
            imageList_architecuture_warning.Images.SetKeyName(1, "icons8-yes-48.png");
            // 
            // button_yes
            // 
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button_yes.AutoSize = true;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button_yes.ImageIndex = 1;
            button_yes.ImageList = imageList_architecuture_warning;
            button_yes.Location = new Point(70, 189);
            button_yes.Name = "button_yes";
            button_yes.Size = new Size(75, 26);
            button_yes.TabIndex = 9;
            button_yes.Text = "&Yes";
            button_yes.TextAlign = ContentAlignment.MiddleRight;
            button_yes.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_yes_Click;
            // 
            // label_architecture_warning
            // 
            label_architecture_warning.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_architecture_warning.AutoSize = true;
            label_architecture_warning.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label_architecture_warning.ImageList = imageList_architecuture_warning;
            label_architecture_warning.Location = new Point(69, 12);
            label_architecture_warning.Name = "label_architecture_warning";
            label_architecture_warning.Size = new Size(268, 48);
            label_architecture_warning.TabIndex = 8;
            label_architecture_warning.Text = "The NeoBleeper Smart Architecture Sensor has \r\ndetected that your computer is not X64 based.\r\n\r\n";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(3, 69);
            label1.Name = "label1";
            label1.Size = new Size(334, 32);
            label1.TabIndex = 8;
            label1.Text = "Despite this, do you want to continue using the NeoBleeper \r\napplication?\r\n";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_processor_48;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(49, 50);
            pictureBox1.TabIndex = 12;
            pictureBox1.TabStop = false;
            // 
            // neobleeper_init_architecture_warning
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(358, 221);
            Controls.Add(pictureBox1);
            Controls.Add(label_architecture_warning_result);
            Controls.Add(button_no);
            Controls.Add(button_yes);
            Controls.Add(label1);
            Controls.Add(label_architecture_warning);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_architecture_warning";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Load += neobleeper_init_architecture_warning_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_architecture_warning_result;
        private Button button_no;
        private Button button_yes;
        private Label label_architecture_warning;
        private ImageList imageList_architecuture_warning;
        private Label label1;
        private PictureBox pictureBox1;
    }
}