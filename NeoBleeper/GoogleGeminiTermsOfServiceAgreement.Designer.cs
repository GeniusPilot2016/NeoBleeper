namespace NeoBleeper
{
    partial class GoogleGeminiTermsOfServiceAgreement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoogleGeminiTermsOfServiceAgreement));
            richTextBoxTerms = new RichTextBox();
            checkBoxAccept = new CheckBox();
            buttonClose = new Button();
            icons = new ImageList(components);
            panel1 = new Panel();
            label1 = new Label();
            label2 = new Label();
            dateTimePickerDateOfBirth = new DateTimePicker();
            SuspendLayout();
            // 
            // richTextBoxTerms
            // 
            resources.ApplyResources(richTextBoxTerms, "richTextBoxTerms");
            richTextBoxTerms.BackColor = SystemColors.Window;
            richTextBoxTerms.DetectUrls = false;
            richTextBoxTerms.Name = "richTextBoxTerms";
            richTextBoxTerms.ReadOnly = true;
            richTextBoxTerms.MouseClick += RichTextBoxTerms_MouseClick;
            richTextBoxTerms.MouseLeave += richTextBoxTerms_MouseLeave;
            richTextBoxTerms.MouseMove += richTextBoxTerms_MouseMove;
            // 
            // checkBoxAccept
            // 
            resources.ApplyResources(checkBoxAccept, "checkBoxAccept");
            checkBoxAccept.Name = "checkBoxAccept";
            checkBoxAccept.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            resources.ApplyResources(buttonClose, "buttonClose");
            buttonClose.ImageList = icons;
            buttonClose.Name = "buttonClose";
            buttonClose.UseVisualStyleBackColor = true;
            buttonClose.Click += buttonClose_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-close-48.png");
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // dateTimePickerDateOfBirth
            // 
            resources.ApplyResources(dateTimePickerDateOfBirth, "dateTimePickerDateOfBirth");
            dateTimePickerDateOfBirth.MaxDate = new DateTime(2025, 12, 31, 0, 0, 0, 0);
            dateTimePickerDateOfBirth.MinDate = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            dateTimePickerDateOfBirth.Name = "dateTimePickerDateOfBirth";
            // 
            // GoogleGeminiTermsOfServiceAgreement
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(dateTimePickerDateOfBirth);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(buttonClose);
            Controls.Add(checkBoxAccept);
            Controls.Add(richTextBoxTerms);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GoogleGeminiTermsOfServiceAgreement";
            ShowIcon = false;
            ShowInTaskbar = false;
            Load += GoogleGeminiTermsOfServiceAgreement_Load;
            SystemColorsChanged += GoogleGeminiTermsOfServiceAgreement_SystemColorsChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBoxTerms;
        private CheckBox checkBoxAccept;
        private Button buttonClose;
        private ImageList icons;
        private Panel panel1;
        private Label label1;
        private Label label2;
        private DateTimePicker dateTimePickerDateOfBirth;
    }
}