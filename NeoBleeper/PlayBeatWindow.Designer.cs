namespace NeoBleeper
{
    partial class PlayBeatWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlayBeatWindow));
            radioButton_play_sound_on_all_beats = new RadioButton();
            radioButton_play_sound_on_odd_beats = new RadioButton();
            radioButton_play_sound_on_even_beats = new RadioButton();
            label_uncheck_do_not_update = new Label();
            panel1 = new Panel();
            radioButton_play_sound_on_checked_lines = new RadioButton();
            SuspendLayout();
            // 
            // radioButton_play_sound_on_all_beats
            // 
            resources.ApplyResources(radioButton_play_sound_on_all_beats, "radioButton_play_sound_on_all_beats");
            radioButton_play_sound_on_all_beats.Checked = true;
            radioButton_play_sound_on_all_beats.Name = "radioButton_play_sound_on_all_beats";
            radioButton_play_sound_on_all_beats.TabStop = true;
            radioButton_play_sound_on_all_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_all_beats.Click += beat_types_click;
            // 
            // radioButton_play_sound_on_odd_beats
            // 
            resources.ApplyResources(radioButton_play_sound_on_odd_beats, "radioButton_play_sound_on_odd_beats");
            radioButton_play_sound_on_odd_beats.Name = "radioButton_play_sound_on_odd_beats";
            radioButton_play_sound_on_odd_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_odd_beats.Click += beat_types_click;
            // 
            // radioButton_play_sound_on_even_beats
            // 
            resources.ApplyResources(radioButton_play_sound_on_even_beats, "radioButton_play_sound_on_even_beats");
            radioButton_play_sound_on_even_beats.Name = "radioButton_play_sound_on_even_beats";
            radioButton_play_sound_on_even_beats.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_even_beats.Click += beat_types_click;
            // 
            // label_uncheck_do_not_update
            // 
            resources.ApplyResources(label_uncheck_do_not_update, "label_uncheck_do_not_update");
            label_uncheck_do_not_update.Name = "label_uncheck_do_not_update";
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // radioButton_play_sound_on_checked_lines
            // 
            resources.ApplyResources(radioButton_play_sound_on_checked_lines, "radioButton_play_sound_on_checked_lines");
            radioButton_play_sound_on_checked_lines.Name = "radioButton_play_sound_on_checked_lines";
            radioButton_play_sound_on_checked_lines.UseVisualStyleBackColor = true;
            radioButton_play_sound_on_checked_lines.Click += beat_types_click;
            // 
            // play_beat_window
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            ControlBox = false;
            Controls.Add(radioButton_play_sound_on_checked_lines);
            Controls.Add(label_uncheck_do_not_update);
            Controls.Add(radioButton_play_sound_on_even_beats);
            Controls.Add(radioButton_play_sound_on_odd_beats);
            Controls.Add(radioButton_play_sound_on_all_beats);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "play_beat_window";
            ShowIcon = false;
            FormClosed += play_beat_window_FormClosed;
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
        private RadioButton radioButton_play_sound_on_checked_lines;
    }
}