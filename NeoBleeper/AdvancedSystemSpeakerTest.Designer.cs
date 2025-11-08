namespace NeoBleeper
{
    partial class AdvancedSystemSpeakerTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdvancedSystemSpeakerTest));
            icons = new ImageList(components);
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-wait-48.png");
            icons.Images.SetKeyName(1, "icons8-check-48.png");
            icons.Images.SetKeyName(2, "icons8-cross-48.png");
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.ImageList = icons;
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.ImageList = icons;
            label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.ImageList = icons;
            label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.ImageList = icons;
            label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.ImageList = icons;
            label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // AdvancedSystemSpeakerTest
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AdvancedSystemSpeakerTest";
            ShowIcon = false;
            ShowInTaskbar = false;
            FormClosing += AdvancedSystemSpeakerTest_FormClosing;
            SystemColorsChanged += AdvancedSystemSpeakerTest_SystemColorsChanged;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ImageList icons;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}