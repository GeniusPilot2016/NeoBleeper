namespace NeoBleeper
{
    partial class InitUnknownTypeOfComputerWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitUnknownTypeOfComputerWarning));
            checkBoxDontShowAgain = new CheckBox();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            label_unknown_type_of_computer_warning = new Label();
            button_no = new Button();
            imageList_unknown_type_of_computer_warning = new ImageList(components);
            button_yes = new Button();
            label_unknown_type_of_computer_result = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // checkBoxDontShowAgain
            // 
            resources.ApplyResources(checkBoxDontShowAgain, "checkBoxDontShowAgain");
            checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
            checkBoxDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_question_mark_48;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // label_unknown_type_of_computer_warning
            // 
            resources.ApplyResources(label_unknown_type_of_computer_warning, "label_unknown_type_of_computer_warning");
            label_unknown_type_of_computer_warning.Name = "label_unknown_type_of_computer_warning";
            // 
            // button_no
            // 
            resources.ApplyResources(button_no, "button_no");
            button_no.DialogResult = DialogResult.No;
            button_no.ImageList = imageList_unknown_type_of_computer_warning;
            button_no.Name = "button_no";
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // imageList_unknown_type_of_computer_warning
            // 
            imageList_unknown_type_of_computer_warning.ColorDepth = ColorDepth.Depth32Bit;
            imageList_unknown_type_of_computer_warning.ImageStream = (ImageListStreamer)resources.GetObject("imageList_unknown_type_of_computer_warning.ImageStream");
            imageList_unknown_type_of_computer_warning.TransparentColor = Color.Transparent;
            imageList_unknown_type_of_computer_warning.Images.SetKeyName(0, "icons8-no-48.png");
            imageList_unknown_type_of_computer_warning.Images.SetKeyName(1, "icons8-yes-48.png");
            // 
            // button_yes
            // 
            resources.ApplyResources(button_yes, "button_yes");
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.ImageList = imageList_unknown_type_of_computer_warning;
            button_yes.Name = "button_yes";
            button_yes.UseVisualStyleBackColor = true;
            button_yes.Click += button_yes_Click;
            // 
            // label_unknown_type_of_computer_result
            // 
            resources.ApplyResources(label_unknown_type_of_computer_result, "label_unknown_type_of_computer_result");
            label_unknown_type_of_computer_result.Name = "label_unknown_type_of_computer_result";
            // 
            // neobleeper_init_unknown_type_of_computer_warning
            // 
            AcceptButton = button_yes;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = button_no;
            Controls.Add(checkBoxDontShowAgain);
            Controls.Add(button_yes);
            Controls.Add(button_no);
            Controls.Add(label_unknown_type_of_computer_warning);
            Controls.Add(pictureBox1);
            Controls.Add(label_unknown_type_of_computer_result);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_unknown_type_of_computer_warning";
            ShowIcon = false;
            Shown += neobleeper_init_unknown_type_of_computer_warning_Shown;
            SystemColorsChanged += neobleeper_init_unknown_type_of_computer_warning_SystemColorsChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBoxDontShowAgain;
        private Label label1;
        private PictureBox pictureBox1;
        private Label label_unknown_type_of_computer_warning;
        private Button button_no;
        private ImageList imageList_unknown_type_of_computer_warning;
        private Button button_yes;
        private Label label_unknown_type_of_computer_result;
    }
}