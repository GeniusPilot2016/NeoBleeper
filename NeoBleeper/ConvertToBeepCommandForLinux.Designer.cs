namespace NeoBleeper
{
    partial class ConvertToBeepCommandForLinux
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertToBeepCommandForLinux));
            richTextBoxBeepCommand = new RichTextBox();
            buttonCopyBeepCommandToClipboard = new Button();
            icons = new ImageList(components);
            buttonSaveAsShFile = new Button();
            saveFileDialog1 = new SaveFileDialog();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // richTextBoxBeepCommand
            // 
            resources.ApplyResources(richTextBoxBeepCommand, "richTextBoxBeepCommand");
            richTextBoxBeepCommand.BackColor = Color.FromArgb(16, 16, 16);
            richTextBoxBeepCommand.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxBeepCommand.ForeColor = Color.White;
            richTextBoxBeepCommand.Name = "richTextBoxBeepCommand";
            richTextBoxBeepCommand.ReadOnly = true;
            // 
            // buttonCopyBeepCommandToClipboard
            // 
            resources.ApplyResources(buttonCopyBeepCommandToClipboard, "buttonCopyBeepCommandToClipboard");
            buttonCopyBeepCommandToClipboard.ImageList = icons;
            buttonCopyBeepCommandToClipboard.Name = "buttonCopyBeepCommandToClipboard";
            buttonCopyBeepCommandToClipboard.UseVisualStyleBackColor = true;
            buttonCopyBeepCommandToClipboard.Click += buttonCopyBeepCommandToClipboard_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-copy-to-clipboard-48.png");
            icons.Images.SetKeyName(1, "icons8-save-48 (1).png");
            // 
            // buttonSaveAsShFile
            // 
            resources.ApplyResources(buttonSaveAsShFile, "buttonSaveAsShFile");
            buttonSaveAsShFile.ImageList = icons;
            buttonSaveAsShFile.Name = "buttonSaveAsShFile";
            buttonSaveAsShFile.UseVisualStyleBackColor = true;
            buttonSaveAsShFile.Click += buttonSaveAsShFile_Click;
            // 
            // saveFileDialog1
            // 
            resources.ApplyResources(saveFileDialog1, "saveFileDialog1");
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // ConvertToBeepCommandForLinux
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(panel1);
            Controls.Add(buttonSaveAsShFile);
            Controls.Add(buttonCopyBeepCommandToClipboard);
            Controls.Add(richTextBoxBeepCommand);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConvertToBeepCommandForLinux";
            ShowIcon = false;
            ShowInTaskbar = false;
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBoxBeepCommand;
        private Button buttonCopyBeepCommandToClipboard;
        private Button buttonSaveAsShFile;
        private ImageList icons;
        private SaveFileDialog saveFileDialog1;
        private Panel panel1;
    }
}