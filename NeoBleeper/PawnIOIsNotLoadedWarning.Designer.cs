namespace NeoBleeper
{
    partial class PawnIOIsNotLoadedWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PawnIOIsNotLoadedWarning));
            button_yes = new Button();
            imageList_disable_create_beep_from_sound_device_warning = new ImageList(components);
            button_no = new Button();
            label_pawnio_is_not_loaded = new Label();
            pictureBox1 = new PictureBox();
            labelQuestionContinueWithoutPawnIO = new Label();
            checkBoxDontShowAgain = new CheckBox();
            linkLabelDownloadPawnIO = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button_yes
            // 
            resources.ApplyResources(button_yes, "button_yes");
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.ImageList = imageList_disable_create_beep_from_sound_device_warning;
            button_yes.Name = "button_yes";
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_yes_Click;
            // 
            // imageList_disable_create_beep_from_sound_device_warning
            // 
            imageList_disable_create_beep_from_sound_device_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_disable_create_beep_from_sound_device_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_disable_create_beep_from_sound_device_warning.ImageStream");
            imageList_disable_create_beep_from_sound_device_warning.TransparentColor = Color.Transparent;
            imageList_disable_create_beep_from_sound_device_warning.Images.SetKeyName(0, "icons8-no-48.png");
            imageList_disable_create_beep_from_sound_device_warning.Images.SetKeyName(1, "icons8-yes-48.png");
            // 
            // button_no
            // 
            resources.ApplyResources(button_no, "button_no");
            button_no.DialogResult = DialogResult.No;
            button_no.ImageList = imageList_disable_create_beep_from_sound_device_warning;
            button_no.Name = "button_no";
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // label_pawnio_is_not_loaded
            // 
            resources.ApplyResources(label_pawnio_is_not_loaded, "label_pawnio_is_not_loaded");
            label_pawnio_is_not_loaded.Name = "label_pawnio_is_not_loaded";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_pawn_48;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // labelQuestionContinueWithoutPawnIO
            // 
            resources.ApplyResources(labelQuestionContinueWithoutPawnIO, "labelQuestionContinueWithoutPawnIO");
            labelQuestionContinueWithoutPawnIO.Name = "labelQuestionContinueWithoutPawnIO";
            // 
            // checkBoxDontShowAgain
            // 
            resources.ApplyResources(checkBoxDontShowAgain, "checkBoxDontShowAgain");
            checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
            checkBoxDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // linkLabelDownloadPawnIO
            // 
            resources.ApplyResources(linkLabelDownloadPawnIO, "linkLabelDownloadPawnIO");
            linkLabelDownloadPawnIO.Name = "linkLabelDownloadPawnIO";
            linkLabelDownloadPawnIO.TabStop = true;
            linkLabelDownloadPawnIO.LinkClicked += linkLabelDownloadPawnIO_LinkClicked;
            // 
            // PawnIOIsNotLoadedWarning
            // 
            AcceptButton = button_yes;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = button_no;
            Controls.Add(linkLabelDownloadPawnIO);
            Controls.Add(checkBoxDontShowAgain);
            Controls.Add(button_yes);
            Controls.Add(button_no);
            Controls.Add(label_pawnio_is_not_loaded);
            Controls.Add(pictureBox1);
            Controls.Add(labelQuestionContinueWithoutPawnIO);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PawnIOIsNotLoadedWarning";
            Shown += disable_create_beep_from_sound_device_warning_on_computers_without_system_speaker_output_Shown;
            SystemColorsChanged += disable_create_beep_from_sound_card_warning_SystemColorsChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button_yes;
        private ImageList imageList_disable_create_beep_from_sound_device_warning;
        private Button button_no;
        private Label label_pawnio_is_not_loaded;
        private PictureBox pictureBox1;
        private Label labelQuestionContinueWithoutPawnIO;
        private CheckBox checkBoxDontShowAgain;
        private LinkLabel linkLabelDownloadPawnIO;
    }
}