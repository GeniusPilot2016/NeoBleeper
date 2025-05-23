﻿namespace NeoBleeper
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
            SuspendLayout();
            // 
            // radioButton_play_sound_on_all_beats
            // 
            radioButton_play_sound_on_all_beats.Anchor = AnchorStyles.None;
            radioButton_play_sound_on_all_beats.AutoSize = true;
            radioButton_play_sound_on_all_beats.Checked = true;
            radioButton_play_sound_on_all_beats.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            radioButton_play_sound_on_all_beats.Location = new Point(22, 27);
            radioButton_play_sound_on_all_beats.Name = "radioButton_play_sound_on_all_beats";
            radioButton_play_sound_on_all_beats.Size = new Size(423, 24);
            radioButton_play_sound_on_all_beats.TabIndex = 0;
            radioButton_play_sound_on_all_beats.TabStop = true;
            radioButton_play_sound_on_all_beats.Text = "Play beat sound on all beats (for music with slow tempo)\r\n";
            radioButton_play_sound_on_all_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_all_beats.Click += beat_types_click;
            // 
            // radioButton_play_sound_on_odd_beats
            // 
            radioButton_play_sound_on_odd_beats.Anchor = AnchorStyles.None;
            radioButton_play_sound_on_odd_beats.AutoSize = true;
            radioButton_play_sound_on_odd_beats.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            radioButton_play_sound_on_odd_beats.Location = new Point(22, 57);
            radioButton_play_sound_on_odd_beats.Name = "radioButton_play_sound_on_odd_beats";
            radioButton_play_sound_on_odd_beats.Size = new Size(506, 24);
            radioButton_play_sound_on_odd_beats.TabIndex = 0;
            radioButton_play_sound_on_odd_beats.Text = "Play beat sound on odd-numbered beats (for music with fast tempo)\r\n";
            radioButton_play_sound_on_odd_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_odd_beats.Click += beat_types_click;
            // 
            // radioButton_play_sound_on_even_beats
            // 
            radioButton_play_sound_on_even_beats.Anchor = AnchorStyles.None;
            radioButton_play_sound_on_even_beats.AutoSize = true;
            radioButton_play_sound_on_even_beats.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            radioButton_play_sound_on_even_beats.Location = new Point(22, 87);
            radioButton_play_sound_on_even_beats.Name = "radioButton_play_sound_on_even_beats";
            radioButton_play_sound_on_even_beats.Size = new Size(320, 24);
            radioButton_play_sound_on_even_beats.TabIndex = 0;
            radioButton_play_sound_on_even_beats.Text = "Play beat sound on even-numbered beats";
            radioButton_play_sound_on_even_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_even_beats.Click += beat_types_click;
            // 
            // label_uncheck_do_not_update
            // 
            label_uncheck_do_not_update.Anchor = AnchorStyles.None;
            label_uncheck_do_not_update.AutoSize = true;
            label_uncheck_do_not_update.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_uncheck_do_not_update.Location = new Point(22, 119);
            label_uncheck_do_not_update.MaximumSize = new Size(512, 0);
            label_uncheck_do_not_update.Name = "label_uncheck_do_not_update";
            label_uncheck_do_not_update.Size = new Size(500, 40);
            label_uncheck_do_not_update.TabIndex = 1;
            label_uncheck_do_not_update.Text = "Please uncheck the \"Do not update\" checkbox in the main window to ensure that the beat sounds can be played correctly.";
            label_uncheck_do_not_update.Visible = false;
            // 
            // play_beat_window
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(546, 200);
            ControlBox = false;
            Controls.Add(label_uncheck_do_not_update);
            Controls.Add(radioButton_play_sound_on_even_beats);
            Controls.Add(radioButton_play_sound_on_odd_beats);
            Controls.Add(radioButton_play_sound_on_all_beats);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "play_beat_window";
            ShowIcon = false;
            Text = "Beat Settings";
            Load += play_beat_window_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButton_play_sound_on_all_beats;
        private RadioButton radioButton_play_sound_on_odd_beats;
        private RadioButton radioButton_play_sound_on_even_beats;
        public Label label_uncheck_do_not_update;
    }
}