namespace BeepBridge
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelInstruction = new Label();
            buttonEnableBeepSlider = new Button();
            labelStatus = new Label();
            buttonRevertChanges = new Button();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // labelInstruction
            // 
            labelInstruction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelInstruction.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelInstruction.Location = new Point(12, 15);
            labelInstruction.Name = "labelInstruction";
            labelInstruction.Size = new Size(440, 55);
            labelInstruction.TabIndex = 0;
            labelInstruction.Text = "This utility unlocks the legacy beep volume slider (often named 'PC Beep', 'Beep', 'System Beep', or 'PC Speaker') in your sound mixer.";
            labelInstruction.TextAlign = ContentAlignment.TopCenter;
            // 
            // buttonEnableBeepSlider
            // 
            buttonEnableBeepSlider.Anchor = AnchorStyles.Top;
            buttonEnableBeepSlider.Font = new Font("HarmonyOS Sans", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonEnableBeepSlider.Location = new Point(122, 75);
            buttonEnableBeepSlider.Name = "buttonEnableBeepSlider";
            buttonEnableBeepSlider.Size = new Size(220, 50);
            buttonEnableBeepSlider.TabIndex = 1;
            buttonEnableBeepSlider.Text = "Enable Beep Slider";
            buttonEnableBeepSlider.UseVisualStyleBackColor = true;
            buttonEnableBeepSlider.Click += buttonEnableBeepSlider_Click;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelStatus.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelStatus.ForeColor = SystemColors.GrayText;
            labelStatus.Location = new Point(12, 173);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(440, 42);
            labelStatus.TabIndex = 3;
            labelStatus.Text = "Waiting for command...";
            labelStatus.TextAlign = ContentAlignment.BottomCenter;
            // 
            // buttonRevertChanges
            // 
            buttonRevertChanges.Anchor = AnchorStyles.Top;
            buttonRevertChanges.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonRevertChanges.Location = new Point(157, 131);
            buttonRevertChanges.Name = "buttonRevertChanges";
            buttonRevertChanges.Size = new Size(150, 30);
            buttonRevertChanges.TabIndex = 2;
            buttonRevertChanges.Text = "Revert Changes";
            buttonRevertChanges.UseVisualStyleBackColor = true;
            buttonRevertChanges.Click += buttonRevertChanges_Click;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 225);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(440, 15);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 4;
            progressBar1.Visible = false;
            // 
            // MainWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(464, 261);
            Controls.Add(progressBar1);
            Controls.Add(labelStatus);
            Controls.Add(buttonRevertChanges);
            Controls.Add(buttonEnableBeepSlider);
            Controls.Add(labelInstruction);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainWindow";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BeepBridge - PC Beep Enabler";
            ResumeLayout(false);
        }

        #endregion

        private Label labelInstruction;
        private Button buttonEnableBeepSlider;
        private Label labelStatus;
        private Button buttonRevertChanges;
        private ProgressBar progressBar1;
    }
}