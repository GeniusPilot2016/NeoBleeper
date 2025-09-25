namespace NeoBleeper
{
    partial class Toast
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
            ShowTimer = new System.Windows.Forms.Timer(components);
            labelMessage = new Label();
            SuspendLayout();
            // 
            // ShowTimer
            // 
            ShowTimer.Enabled = true;
            ShowTimer.Tick += ShowTimer_Tick;
            // 
            // labelMessage
            // 
            labelMessage.AutoSize = true;
            labelMessage.Font = new Font("HarmonyOS Sans", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelMessage.Location = new Point(12, 9);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(69, 20);
            labelMessage.TabIndex = 0;
            labelMessage.Text = "Message";
            // 
            // Toast
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            BackColor = SystemColors.ControlLight;
            ClientSize = new Size(92, 38);
            Controls.Add(labelMessage);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Toast";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Toast";
            Paint += Toast_Paint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Timer ShowTimer;
        private Label labelMessage;
    }
}