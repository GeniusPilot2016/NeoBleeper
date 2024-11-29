namespace NeoBleeper
{
    partial class synchronized_play_window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(synchronized_play_window));
            groupBox1 = new GroupBox();
            lbl_current_system_time = new Label();
            lbl_current_time = new Label();
            label2 = new Label();
            lbl_hour_minute_second = new Label();
            dateTimePicker1 = new DateTimePicker();
            groupBox2 = new GroupBox();
            radioButton2 = new RadioButton();
            imageList_synchronized_play = new ImageList(components);
            radioButton1 = new RadioButton();
            button1 = new Button();
            lbl_waiting = new Label();
            label6 = new Label();
            timer_time = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(lbl_current_system_time);
            groupBox1.Controls.Add(lbl_current_time);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(lbl_hour_minute_second);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
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
            // 
            // groupBox2
            // 
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Controls.Add(radioButton2);
            groupBox2.Controls.Add(radioButton1);
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // radioButton2
            // 
            resources.ApplyResources(radioButton2, "radioButton2");
            radioButton2.ImageList = imageList_synchronized_play;
            radioButton2.Name = "radioButton2";
            radioButton2.UseVisualStyleBackColor = true;
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
            // radioButton1
            // 
            resources.ApplyResources(radioButton1, "radioButton1");
            radioButton1.Checked = true;
            radioButton1.ImageList = imageList_synchronized_play;
            radioButton1.Name = "radioButton1";
            radioButton1.TabStop = true;
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.ImageList = imageList_synchronized_play;
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // lbl_waiting
            // 
            resources.ApplyResources(lbl_waiting, "lbl_waiting");
            lbl_waiting.BackColor = Color.Red;
            lbl_waiting.Name = "lbl_waiting";
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
            // synchronized_play_window
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            ControlBox = false;
            Controls.Add(pictureBox1);
            Controls.Add(label6);
            Controls.Add(lbl_waiting);
            Controls.Add(button1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "synchronized_play_window";
            ShowIcon = false;
            FormClosing += synchronized_play_window_FormClosing;
            Load += synchronized_play_window_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private Label lbl_hour_minute_second;
        private DateTimePicker dateTimePicker1;
        private Label lbl_current_system_time;
        private Label lbl_current_time;
        private Label label2;
        private GroupBox groupBox2;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Button button1;
        private Label lbl_waiting;
        private Label label6;
        private System.Windows.Forms.Timer timer_time;
        private ImageList imageList_synchronized_play;
        private PictureBox pictureBox1;
    }
}