namespace NeoBleeper
{
    partial class play_beat_window
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
            radioButton_play_sound_on_all_beats = new RadioButton();
            radioButton_play_sound_on_odd_beats = new RadioButton();
            radioButton_play_sound_on_even_beats = new RadioButton();
            label_uncheck_do_not_update = new Label();
            panel1 = new Panel();
            SuspendLayout();
            // 
            // radioButton_play_sound_on_all_beats
            // 
            radioButton_play_sound_on_all_beats.AutoSize = true;
            radioButton_play_sound_on_all_beats.Checked = true;
            radioButton_play_sound_on_all_beats.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            radioButton_play_sound_on_all_beats.Location = new Point(11, 10);
            radioButton_play_sound_on_all_beats.Margin = new Padding(2);
            radioButton_play_sound_on_all_beats.Name = "radioButton_play_sound_on_all_beats";
            radioButton_play_sound_on_all_beats.Size = new Size(329, 20);
            radioButton_play_sound_on_all_beats.TabIndex = 0;
            radioButton_play_sound_on_all_beats.TabStop = true;
            radioButton_play_sound_on_all_beats.Text = "Play beat sound on all beats (for music with slow tempo)\r\n";
            radioButton_play_sound_on_all_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_all_beats.Click += beat_types_click;
            // 
            // radioButton_play_sound_on_odd_beats
            // 
            radioButton_play_sound_on_odd_beats.AutoSize = true;
            radioButton_play_sound_on_odd_beats.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            radioButton_play_sound_on_odd_beats.Location = new Point(11, 34);
            radioButton_play_sound_on_odd_beats.Margin = new Padding(2);
            radioButton_play_sound_on_odd_beats.Name = "radioButton_play_sound_on_odd_beats";
            radioButton_play_sound_on_odd_beats.Size = new Size(395, 20);
            radioButton_play_sound_on_odd_beats.TabIndex = 0;
            radioButton_play_sound_on_odd_beats.Text = "Play beat sound on odd-numbered beats (for music with fast tempo)\r\n";
            radioButton_play_sound_on_odd_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_odd_beats.Click += beat_types_click;
            // 
            // radioButton_play_sound_on_even_beats
            // 
            radioButton_play_sound_on_even_beats.AutoSize = true;
            radioButton_play_sound_on_even_beats.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            radioButton_play_sound_on_even_beats.Location = new Point(11, 58);
            radioButton_play_sound_on_even_beats.Margin = new Padding(2);
            radioButton_play_sound_on_even_beats.Name = "radioButton_play_sound_on_even_beats";
            radioButton_play_sound_on_even_beats.Size = new Size(254, 20);
            radioButton_play_sound_on_even_beats.TabIndex = 0;
            radioButton_play_sound_on_even_beats.Text = "Play beat sound on even-numbered beats";
            radioButton_play_sound_on_even_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_even_beats.Click += beat_types_click;
            // 
            // label_uncheck_do_not_update
            // 
            label_uncheck_do_not_update.AutoSize = true;
            label_uncheck_do_not_update.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_uncheck_do_not_update.Location = new Point(11, 87);
            label_uncheck_do_not_update.Margin = new Padding(2, 0, 2, 0);
            label_uncheck_do_not_update.MaximumSize = new Size(410, 0);
            label_uncheck_do_not_update.Name = "label_uncheck_do_not_update";
            label_uncheck_do_not_update.Size = new Size(401, 32);
            label_uncheck_do_not_update.TabIndex = 1;
            label_uncheck_do_not_update.Text = "Please uncheck the \"Do not update\" checkbox in the main window to ensure that the beat sounds can be played correctly.";
            label_uncheck_do_not_update.Visible = false;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            panel1.Location = new Point(406, 104);
            panel1.Name = "panel1";
            panel1.Size = new Size(20, 20);
            panel1.TabIndex = 2;
            // 
            // play_beat_window
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(428, 127);
            ControlBox = false;
            Controls.Add(label_uncheck_do_not_update);
            Controls.Add(radioButton_play_sound_on_even_beats);
            Controls.Add(radioButton_play_sound_on_odd_beats);
            Controls.Add(radioButton_play_sound_on_all_beats);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "play_beat_window";
            ShowIcon = false;
            Text = "Beat Settings";
            FormClosed += play_beat_window_FormClosed;
            Load += play_beat_window_Load;
            SystemColorsChanged += play_beat_window_SystemColorsChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButton_play_sound_on_all_beats;
        private RadioButton radioButton_play_sound_on_odd_beats;
        private RadioButton radioButton_play_sound_on_even_beats;
        public Label label_uncheck_do_not_update;
        private Panel panel1;
    }
}