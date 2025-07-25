﻿namespace NeoBleeper
{
    partial class neobleeper_init_system_speaker_warning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(neobleeper_init_system_speaker_warning));
            label_system_speaker_warning = new Label();
            button_yes = new Button();
            imageList_system_speaker_warning = new ImageList(components);
            button_no = new Button();
            label_system_speaker_warning_result = new Label();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            checkBoxDontShowAgain = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label_system_speaker_warning
            // 
            label_system_speaker_warning.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_system_speaker_warning.Font = new Font("HarmonyOS Sans", 9F);
            label_system_speaker_warning.Location = new Point(68, 10);
            label_system_speaker_warning.MinimumSize = new Size(280, 64);
            label_system_speaker_warning.Name = "label_system_speaker_warning";
            label_system_speaker_warning.Size = new Size(280, 64);
            label_system_speaker_warning.TabIndex = 0;
            label_system_speaker_warning.Text = "The NeoBleeper Smart System Speaker Sensor has detected that your computer's motherboard either does not have a system speaker output or has a non-standard system speaker output.\r\n";
            // 
            // button_yes
            // 
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button_yes.AutoSize = true;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.Font = new Font("HarmonyOS Sans", 9F);
            button_yes.ImageIndex = 1;
            button_yes.ImageList = imageList_system_speaker_warning;
            button_yes.Location = new Point(71, 221);
            button_yes.Name = "button_yes";
            button_yes.Size = new Size(75, 26);
            button_yes.TabIndex = 1;
            button_yes.Text = "&Yes";
            button_yes.TextAlign = ContentAlignment.MiddleRight;
            button_yes.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_yes_Click;
            // 
            // imageList_system_speaker_warning
            // 
            imageList_system_speaker_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_system_speaker_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_system_speaker_warning.ImageStream");
            imageList_system_speaker_warning.TransparentColor = Color.Transparent;
            imageList_system_speaker_warning.Images.SetKeyName(0, "icons8-no-48.png");
            imageList_system_speaker_warning.Images.SetKeyName(1, "icons8-yes-48.png");
            // 
            // button_no
            // 
            button_no.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_no.AutoSize = true;
            button_no.DialogResult = DialogResult.No;
            button_no.Font = new Font("HarmonyOS Sans", 9F);
            button_no.ImageIndex = 0;
            button_no.ImageList = imageList_system_speaker_warning;
            button_no.Location = new Point(201, 221);
            button_no.Name = "button_no";
            button_no.Size = new Size(75, 26);
            button_no.TabIndex = 2;
            button_no.Text = "&No";
            button_no.TextAlign = ContentAlignment.MiddleRight;
            button_no.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // label_system_speaker_warning_result
            // 
            label_system_speaker_warning_result.Anchor = AnchorStyles.Top;
            label_system_speaker_warning_result.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Bold);
            label_system_speaker_warning_result.Location = new Point(9, 128);
            label_system_speaker_warning_result.MinimumSize = new Size(340, 64);
            label_system_speaker_warning_result.Name = "label_system_speaker_warning_result";
            label_system_speaker_warning_result.Size = new Size(340, 64);
            label_system_speaker_warning_result.TabIndex = 3;
            label_system_speaker_warning_result.Text = "If you want to continue using the application, you can only use the \"Use sound device to generate beeps\" setting to generate beeps and this setting cannot be disabled in your computer.\r\n";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.Font = new Font("HarmonyOS Sans", 9F);
            label1.Location = new Point(8, 86);
            label1.MinimumSize = new Size(340, 32);
            label1.Name = "label1";
            label1.Size = new Size(340, 32);
            label1.TabIndex = 0;
            label1.Text = "Despite this, do you want to continue using the NeoBleeper application?\r\n";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_mute_48__1_;
            pictureBox1.Location = new Point(9, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(48, 48);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // checkBoxDontShowAgain
            // 
            checkBoxDontShowAgain.Anchor = AnchorStyles.Bottom;
            checkBoxDontShowAgain.AutoSize = true;
            checkBoxDontShowAgain.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBoxDontShowAgain.Location = new Point(115, 195);
            checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
            checkBoxDontShowAgain.Size = new Size(119, 20);
            checkBoxDontShowAgain.TabIndex = 13;
            checkBoxDontShowAgain.Text = "Don't show again";
            checkBoxDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // neobleeper_init_system_speaker_warning
            // 
            AcceptButton = button_yes;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = button_no;
            ClientSize = new Size(358, 251);
            Controls.Add(checkBoxDontShowAgain);
            Controls.Add(pictureBox1);
            Controls.Add(label_system_speaker_warning_result);
            Controls.Add(button_no);
            Controls.Add(button_yes);
            Controls.Add(label1);
            Controls.Add(label_system_speaker_warning);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_system_speaker_warning";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_system_speaker_warning;
        private Button button_yes;
        private Button button_no;
        private Label label_system_speaker_warning_result;
        private Label label1;
        private PictureBox pictureBox1;
        private ImageList imageList_system_speaker_warning;
        private CheckBox checkBoxDontShowAgain;
    }
}