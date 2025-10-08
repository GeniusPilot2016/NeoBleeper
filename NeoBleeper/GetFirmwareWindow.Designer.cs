namespace NeoBleeper
{
    partial class GetFirmwareWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetFirmwareWindow));
            label1 = new Label();
            icons = new ImageList(components);
            comboBoxMicrocontroller = new ComboBox();
            richTextBoxFirmware = new RichTextBox();
            buttonCopyFirmwareToClipboard = new Button();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ImageList = icons;
            label1.Name = "label1";
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-motherboard-48.png");
            icons.Images.SetKeyName(1, "icons8-copy-to-clipboard-48.png");
            // 
            // comboBoxMicrocontroller
            // 
            resources.ApplyResources(comboBoxMicrocontroller, "comboBoxMicrocontroller");
            comboBoxMicrocontroller.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxMicrocontroller.FormattingEnabled = true;
            comboBoxMicrocontroller.Items.AddRange(new object[] { resources.GetString("comboBoxMicrocontroller.Items"), resources.GetString("comboBoxMicrocontroller.Items1"), resources.GetString("comboBoxMicrocontroller.Items2") });
            comboBoxMicrocontroller.Name = "comboBoxMicrocontroller";
            comboBoxMicrocontroller.SelectedIndexChanged += comboBoxMicrocontroller_SelectedIndexChanged;
            // 
            // richTextBoxFirmware
            // 
            resources.ApplyResources(richTextBoxFirmware, "richTextBoxFirmware");
            richTextBoxFirmware.BackColor = SystemColors.Window;
            richTextBoxFirmware.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxFirmware.Name = "richTextBoxFirmware";
            richTextBoxFirmware.ReadOnly = true;
            // 
            // buttonCopyFirmwareToClipboard
            // 
            resources.ApplyResources(buttonCopyFirmwareToClipboard, "buttonCopyFirmwareToClipboard");
            buttonCopyFirmwareToClipboard.ImageList = icons;
            buttonCopyFirmwareToClipboard.Name = "buttonCopyFirmwareToClipboard";
            buttonCopyFirmwareToClipboard.UseVisualStyleBackColor = true;
            buttonCopyFirmwareToClipboard.Click += buttonCopyFirmwareToClipboard_Click;
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // GetFirmwareWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel1);
            Controls.Add(buttonCopyFirmwareToClipboard);
            Controls.Add(richTextBoxFirmware);
            Controls.Add(comboBoxMicrocontroller);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GetFirmwareWindow";
            ShowIcon = false;
            ShowInTaskbar = false;
            SystemColorsChanged += GetFirmwareWindow_SystemColorsChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox comboBoxMicrocontroller;
        private RichTextBox richTextBoxFirmware;
        private Button buttonCopyFirmwareToClipboard;
        private ImageList icons;
        private Panel panel1;
    }
}