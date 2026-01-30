namespace NeoBleeper
{
    partial class SynchronizedPlayWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SynchronizedPlayWindow));
            groupBox_time = new GroupBox();
            lbl_current_system_time = new Label();
            lbl_current_time = new Label();
            label2 = new Label();
            lbl_hour_minute_second = new Label();
            dateTimePicker1 = new DateTimePicker();
            groupBox_position = new GroupBox();
            radioButton_play_currently_selected_line = new RadioButton();
            imageList_synchronized_play = new ImageList(components);
            radioButton_play_beginning_of_music = new RadioButton();
            button_wait = new Button();
            label6 = new Label();
            timer_time = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            lbl_waiting = new Label();
            errorProviderWaiting = new ErrorProvider(components);
            groupBox_time.SuspendLayout();
            groupBox_position.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)errorProviderWaiting).BeginInit();
            SuspendLayout();
            // 
            // groupBox_time
            // 
            resources.ApplyResources(groupBox_time, "groupBox_time");
            groupBox_time.Controls.Add(lbl_current_system_time);
            groupBox_time.Controls.Add(lbl_current_time);
            groupBox_time.Controls.Add(label2);
            groupBox_time.Controls.Add(lbl_hour_minute_second);
            groupBox_time.Controls.Add(dateTimePicker1);
            groupBox_time.Name = "groupBox_time";
            groupBox_time.TabStop = false;
            // 
            // lbl_current_system_time
            // 
            resources.ApplyResources(lbl_current_system_time, "lbl_current_system_time");
            lbl_current_system_time.Name = "lbl_current_system_time";
            // 
            // lbl_current_time
            // 
            resources.ApplyResources(lbl_current_time, "lbl_current_time");
            lbl_current_time.Name = "lbl_current_time";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.BackColor = Color.Transparent;
            label2.Name = "label2";
            // 
            // lbl_hour_minute_second
            // 
            resources.ApplyResources(lbl_hour_minute_second, "lbl_hour_minute_second");
            lbl_hour_minute_second.Name = "lbl_hour_minute_second";
            // 
            // dateTimePicker1
            // 
            resources.ApplyResources(dateTimePicker1, "dateTimePicker1");
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
            // 
            // groupBox_position
            // 
            resources.ApplyResources(groupBox_position, "groupBox_position");
            groupBox_position.Controls.Add(radioButton_play_currently_selected_line);
            groupBox_position.Controls.Add(radioButton_play_beginning_of_music);
            groupBox_position.Name = "groupBox_position";
            groupBox_position.TabStop = false;
            // 
            // radioButton_play_currently_selected_line
            // 
            resources.ApplyResources(radioButton_play_currently_selected_line, "radioButton_play_currently_selected_line");
            radioButton_play_currently_selected_line.ImageList = imageList_synchronized_play;
            radioButton_play_currently_selected_line.Name = "radioButton_play_currently_selected_line";
            radioButton_play_currently_selected_line.UseVisualStyleBackColor = true;
            radioButton_play_currently_selected_line.CheckedChanged += radioButton_play_currently_selected_line_CheckedChanged;
            // 
            // imageList_synchronized_play
            // 
            imageList_synchronized_play.ColorDepth = ColorDepth.Depth32Bit;
            imageList_synchronized_play.ImageStream = (ImageListStreamer)resources.GetObject("imageList_synchronized_play.ImageStream");
            imageList_synchronized_play.TransparentColor = Color.Transparent;
            imageList_synchronized_play.Images.SetKeyName(0, "icons8-wait-48.png");
            imageList_synchronized_play.Images.SetKeyName(1, "icons8-play-48.png");
            imageList_synchronized_play.Images.SetKeyName(2, "icons8-music-notation-48.png");
            // 
            // radioButton_play_beginning_of_music
            // 
            resources.ApplyResources(radioButton_play_beginning_of_music, "radioButton_play_beginning_of_music");
            radioButton_play_beginning_of_music.Checked = true;
            radioButton_play_beginning_of_music.ImageList = imageList_synchronized_play;
            radioButton_play_beginning_of_music.Name = "radioButton_play_beginning_of_music";
            radioButton_play_beginning_of_music.TabStop = true;
            radioButton_play_beginning_of_music.UseVisualStyleBackColor = true;
            radioButton_play_beginning_of_music.CheckedChanged += radioButton_play_beginning_of_music_CheckedChanged;
            // 
            // button_wait
            // 
            resources.ApplyResources(button_wait, "button_wait");
            button_wait.ImageList = imageList_synchronized_play;
            button_wait.Name = "button_wait";
            button_wait.UseVisualStyleBackColor = true;
            button_wait.Click += button_wait_Click;
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // timer_time
            // 
            timer_time.Enabled = true;
            timer_time.Interval = 1;
            timer_time.Tick += timer1_Tick;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.icons8_clock_48;
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // lbl_waiting
            // 
            resources.ApplyResources(lbl_waiting, "lbl_waiting");
            lbl_waiting.BackColor = Color.Red;
            lbl_waiting.ForeColor = SystemColors.ControlText;
            lbl_waiting.Name = "lbl_waiting";
            // 
            // errorProviderWaiting
            // 
            errorProviderWaiting.ContainerControl = this;
            // 
            // SynchronizedPlayWindow
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            ControlBox = false;
            Controls.Add(pictureBox1);
            Controls.Add(label6);
            Controls.Add(lbl_waiting);
            Controls.Add(button_wait);
            Controls.Add(groupBox_position);
            Controls.Add(groupBox_time);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SynchronizedPlayWindow";
            ShowIcon = false;
            Deactivate += SynchronizedPlayWindow_Deactivate;
            FormClosing += synchronized_play_window_FormClosing;
            FormClosed += synchronized_play_window_FormClosed;
            Click += SynchronizedPlayWindow_Click;
            SystemColorsChanged += synchronized_play_window_SystemColorsChanged;
            groupBox_time.ResumeLayout(false);
            groupBox_time.PerformLayout();
            groupBox_position.ResumeLayout(false);
            groupBox_position.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)errorProviderWaiting).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox_time;
        private Label lbl_hour_minute_second;
        private DateTimePicker dateTimePicker1;
        private Label lbl_current_system_time;
        private Label lbl_current_time;
        private Label label2;
        private GroupBox groupBox_position;
        private RadioButton radioButton_play_currently_selected_line;
        private RadioButton radioButton_play_beginning_of_music;
        private Button button_wait;
        private Label label6;
        private System.Windows.Forms.Timer timer_time;
        private ImageList imageList_synchronized_play;
        private PictureBox pictureBox1;
        private Label lbl_waiting;
        private ErrorProvider errorProviderWaiting;
    }
}