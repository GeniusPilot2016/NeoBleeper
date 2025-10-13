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
            checkBoxDontShowAgain = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // button_yes
            // 
            resources.ApplyResources(button_yes, "button_yes");
            button_yes.AccessibleRole = AccessibleRole.None;
            button_yes.DialogResult = DialogResult.Yes;
            button_yes.ImageList = imageList_compact_computer_warning;
            button_yes.Name = "button_yes";
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
            resources.ApplyResources(label_compact_computer_warning, "label_compact_computer_warning");
            label_compact_computer_warning.Name = "label_compact_computer_warning";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Image = Properties.Resources.icons8_laptop_48;
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // label_compact_computer_warning_result
            // 
            resources.ApplyResources(label_compact_computer_warning_result, "label_compact_computer_warning_result");
            label_compact_computer_warning_result.Name = "label_compact_computer_warning_result";
            // 
            // button_no
            // 
            resources.ApplyResources(button_no, "button_no");
            button_no.DialogResult = DialogResult.No;
            button_no.ImageList = imageList_compact_computer_warning;
            button_no.Name = "button_no";
            button_no.UseVisualStyleBackColor = true;
            button_no.Click += button_no_Click;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // checkBoxDontShowAgain
            // 
            resources.ApplyResources(checkBoxDontShowAgain, "checkBoxDontShowAgain");
            checkBoxDontShowAgain.Name = "checkBoxDontShowAgain";
            checkBoxDontShowAgain.UseVisualStyleBackColor = true;
            // 
            // neobleeper_init_compact_computer_warning
            // 
            AcceptButton = button_yes;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = button_no;
            Controls.Add(checkBoxDontShowAgain);
            Controls.Add(button_yes);
            Controls.Add(label_compact_computer_warning);
            Controls.Add(pictureBox1);
            Controls.Add(label_compact_computer_warning_result);
            Controls.Add(button_no);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "neobleeper_init_compact_computer_warning";
            ShowIcon = false;
            Shown += neobleeper_init_compact_computer_warning_Shown;
            SystemColorsChanged += neobleeper_init_compact_computer_warning_SystemColorsChanged;
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
        private CheckBox checkBoxDontShowAgain;
    }
}