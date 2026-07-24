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
            labelSysExText = new Label();
            SuspendLayout();
            // 
            // sysexEmulatorBase
            // 
            resources.ApplyResources(sysexEmulatorBase, "sysexEmulatorBase");
            sysexEmulatorBase.BackColor = Color.FromArgb(173, 216, 23);
            sysexEmulatorBase.ForeColor = Color.DimGray;
            sysexEmulatorBase.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            sysexEmulatorBase.Name = "sysexEmulatorBase";
            // 
            // labelSysExText
            // 
            labelSysExText.BackColor = Color.Black;
            labelSysExText.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(labelSysExText, "labelSysExText");
            labelSysExText.ForeColor = Color.White;
            labelSysExText.Name = "labelSysExText";
            // 
            // SysExDisplayEmulator
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            ControlBox = false;
            Controls.Add(labelSysExText);
            Controls.Add(sysexEmulatorBase);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SysExDisplayEmulator";
            ShowIcon = false;
            FormClosing += SysExDisplayEmulator_FormClosing;
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel sysexEmulatorBase;
        private Label labelSysExText;
    }
}