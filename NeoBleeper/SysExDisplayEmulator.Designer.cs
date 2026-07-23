namespace NeoBleeper
{
    partial class SysExDisplayEmulator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SysExDisplayEmulator));
            sysexEmulatorBase = new TableLayoutPanel();
            textBoxSysExText = new TextBox();
            SuspendLayout();
            // 
            // sysexEmulatorBase
            // 
            resources.ApplyResources(sysexEmulatorBase, "sysexEmulatorBase");
            sysexEmulatorBase.BackColor = Color.FromArgb(173, 216, 23);
            sysexEmulatorBase.ForeColor = Color.DimGray;
            sysexEmulatorBase.Name = "sysexEmulatorBase";
            // 
            // textBoxSysExText
            // 
            resources.ApplyResources(textBoxSysExText, "textBoxSysExText");
            textBoxSysExText.BackColor = Color.Black;
            textBoxSysExText.ForeColor = Color.White;
            textBoxSysExText.Name = "textBoxSysExText";
            textBoxSysExText.ReadOnly = true;
            // 
            // SysExDisplayEmulator
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = false;
            Controls.Add(textBoxSysExText);
            Controls.Add(sysexEmulatorBase);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SysExDisplayEmulator";
            ShowIcon = false;
            FormClosing += SysExDisplayEmulator_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel sysexEmulatorBase;
        private TextBox textBoxSysExText;
    }
}