namespace NeoBleeper
{
    partial class neobleeper_init_compact_computer_warning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(neobleeper_init_compact_computer_warning));
            button_yes = new Button();
            imageList_compact_computer_warning = new ImageList(components);
            label_compact_computer_warning = new Label();
            pictureBox1 = new PictureBox();
            label_compact_computer_warning_result = new Label();
            button_no = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button_yes
            // 
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button_yes.AutoSize = true;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.Font = new Font("HarmonyOS Sans", 9F);
            button_yes.ImageIndex = 1;
            button_yes.ImageList = imageList_compact_computer_warning;
            button_yes.Location = new Point(66, 327);
            button_yes.Name = "button_yes";
            button_yes.Size = new Size(94, 32);
            button_yes.TabIndex = 7;
            button_yes.Text = "&Yes";
            button_yes.TextAlign = ContentAlignment.MiddleRight;
            button_yes.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_yes_Click;
            // 
            // imageList_compact_computer_warning
            // 
            imageList_compact_computer_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_compact_computer_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_compact_computer_warning.ImageStream");
            imageList_compact_computer_warning.TransparentColor = Color.Transparent;
            imageList_compact_computer_warning.Images.SetKeyName(0, "icons8-no-48.png");
            imageList_compact_computer_warning.Images.SetKeyName(1, "icons8-yes-48.png");
            // 
            // label_compact_computer_warning
            // 
            label_compact_computer_warning.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_compact_computer_warning.AutoSize = true;
            label_compact_computer_warning.Font = new Font("HarmonyOS Sans", 9F);
            label_compact_computer_warning.Location = new Point(66, 12);
            label_compact_computer_warning.Name = "label_compact_computer_warning";
            label_compact_computer_warning.Size = new Size(357, 100);
            label_compact_computer_warning.TabIndex = 5;
            label_compact_computer_warning.Text = resources.GetString("label_compact_computer_warning.Text");
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_laptop_48;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            // 
            // label_compact_computer_warning_result
            // 
            label_compact_computer_warning_result.Anchor = AnchorStyles.Bottom;
            label_compact_computer_warning_result.AutoSize = true;
            label_compact_computer_warning_result.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Bold);
            label_compact_computer_warning_result.Location = new Point(5, 175);
            label_compact_computer_warning_result.Name = "label_compact_computer_warning_result";
            label_compact_computer_warning_result.Size = new Size(424, 140);
            label_compact_computer_warning_result.TabIndex = 9;
            label_compact_computer_warning_result.Text = resources.GetString("label_compact_computer_warning_result.Text");
            // 
            // button_no
            // 
            button_no.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_no.AutoSize = true;
            button_no.DialogResult = DialogResult.No;
            button_no.Font = new Font("HarmonyOS Sans", 9F);
            button_no.ImageIndex = 0;
            button_no.ImageList = imageList_compact_computer_warning;
            button_no.Location = new Point(255, 327);
            button_no.Name = "button_no";
            button_no.Size = new Size(94, 32);
            button_no.TabIndex = 8;
            button_no.Text = "&No";
            button_no.TextAlign = ContentAlignment.MiddleRight;
            button_no.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 9F);
            label1.Location = new Point(5, 122);
            label1.Name = "label1";
            label1.Size = new Size(429, 40);
            label1.TabIndex = 6;
            label1.Text = "Despite this, do you want to continue using the NeoBleeper \r\napplication?\r\n";
            // 
            // neobleeper_init_compact_computer_warning
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(438, 367);
            Controls.Add(button_yes);
            Controls.Add(label_compact_computer_warning);
            Controls.Add(pictureBox1);
            Controls.Add(label_compact_computer_warning_result);
            Controls.Add(button_no);
            Controls.Add(label1);
            Font = new Font("HarmonyOS Sans", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_compact_computer_warning";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_yes;
        private ImageList imageList_compact_computer_warning;
        private Label label_compact_computer_warning;
        private PictureBox pictureBox1;
        private Label label_compact_computer_warning_result;
        private Button button_no;
        private Label label1;
    }
}