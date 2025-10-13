namespace NeoBleeper
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
            resources.ApplyResources(label_system_speaker_warning, "label_system_speaker_warning");
            label_system_speaker_warning.Name = "label_system_speaker_warning";
            // 
            // button_yes
            // 
            resources.ApplyResources(button_yes, "button_yes");
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.ImageList = imageList_system_speaker_warning;
            button_yes.Name = "button_yes";
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
            resources.ApplyResources(button_no, "button_no");
            button_no.DialogResult = DialogResult.No;
            button_no.ImageList = imageList_system_speaker_warning;
            button_no.Name = "button_no";
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // label_system_speaker_warning_result
            // 
            resources.ApplyResources(label_system_speaker_warning_result, "label_system_speaker_warning_result");
            label_system_speaker_warning_result.Name = "label_system_speaker_warning_result";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_mute_48__1_;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // checkBoxDontShowAgain
            // 
            resources.ApplyResources(checkBoxDontShowAgain, "checkBoxDontShowAgain");
            checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
            checkBoxDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // neobleeper_init_system_speaker_warning
            // 
            AcceptButton = button_yes;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = button_no;
            Controls.Add(checkBoxDontShowAgain);
            Controls.Add(pictureBox1);
            Controls.Add(label_system_speaker_warning_result);
            Controls.Add(button_no);
            Controls.Add(button_yes);
            Controls.Add(label1);
            Controls.Add(label_system_speaker_warning);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_system_speaker_warning";
            ShowIcon = false;
            Shown += neobleeper_init_system_speaker_warning_Shown;
            SystemColorsChanged += neobleeper_init_system_speaker_warning_SystemColorsChanged;
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