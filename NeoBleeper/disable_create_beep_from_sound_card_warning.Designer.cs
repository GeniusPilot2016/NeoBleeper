namespace NeoBleeper
{
    partial class disable_create_beep_from_sound_card_warning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(disable_create_beep_from_sound_card_warning));
            button_yes = new Button();
            imageList_disable_create_beep_from_sound_card_warning = new ImageList(components);
            button_no = new Button();
            label_unknown_type_of_computer_warning = new Label();
            pictureBox1 = new PictureBox();
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
            button_yes.ImageList = imageList_disable_create_beep_from_sound_card_warning;
            button_yes.Location = new Point(69, 186);
            button_yes.Name = "button_yes";
            button_yes.Size = new Size(75, 26);
            button_yes.TabIndex = 19;
            button_yes.Text = "&Yes";
            button_yes.TextAlign = ContentAlignment.MiddleRight;
            button_yes.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_yes_Click;
            // 
            // imageList_disable_create_beep_from_sound_card_warning
            // 
            imageList_disable_create_beep_from_sound_card_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_disable_create_beep_from_sound_card_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_disable_create_beep_from_sound_card_warning.ImageStream");
            imageList_disable_create_beep_from_sound_card_warning.TransparentColor = Color.Transparent;
            imageList_disable_create_beep_from_sound_card_warning.Images.SetKeyName(0, "icons8-no-48.png");
            imageList_disable_create_beep_from_sound_card_warning.Images.SetKeyName(1, "icons8-yes-48.png");
            // 
            // button_no
            // 
            button_no.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_no.AutoSize = true;
            button_no.DialogResult = DialogResult.No;
            button_no.Font = new Font("HarmonyOS Sans", 9F);
            button_no.ImageIndex = 0;
            button_no.ImageList = imageList_disable_create_beep_from_sound_card_warning;
            button_no.Location = new Point(197, 186);
            button_no.Name = "button_no";
            button_no.Size = new Size(75, 26);
            button_no.TabIndex = 20;
            button_no.Text = "&No";
            button_no.TextAlign = ContentAlignment.MiddleRight;
            button_no.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // label_unknown_type_of_computer_warning
            // 
            label_unknown_type_of_computer_warning.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_unknown_type_of_computer_warning.AutoSize = true;
            label_unknown_type_of_computer_warning.Font = new Font("HarmonyOS Sans", 9F);
            label_unknown_type_of_computer_warning.Location = new Point(66, 8);
            label_unknown_type_of_computer_warning.Name = "label_unknown_type_of_computer_warning";
            label_unknown_type_of_computer_warning.Size = new Size(283, 128);
            label_unknown_type_of_computer_warning.TabIndex = 17;
            label_unknown_type_of_computer_warning.Text = resources.GetString("label_unknown_type_of_computer_warning.Text");
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_warning_48;
            pictureBox1.Location = new Point(12, 8);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.TabIndex = 22;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 9F);
            label1.Location = new Point(12, 147);
            label1.Name = "label1";
            label1.Size = new Size(308, 32);
            label1.TabIndex = 18;
            label1.Text = "Despite this, do you want to disable \"Use sound card to \r\ncreate beeps\" option?";
            // 
            // disable_create_beep_from_sound_card_warning
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(357, 218);
            Controls.Add(button_yes);
            Controls.Add(button_no);
            Controls.Add(label_unknown_type_of_computer_warning);
            Controls.Add(pictureBox1);
            Controls.Add(label1);
            Font = new Font("HarmonyOS Sans", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "disable_create_beep_from_sound_card_warning";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_yes;
        private ImageList imageList_disable_create_beep_from_sound_card_warning;
        private Button button_no;
        private Label label_unknown_type_of_computer_warning;
        private PictureBox pictureBox1;
        private Label label1;
    }
}