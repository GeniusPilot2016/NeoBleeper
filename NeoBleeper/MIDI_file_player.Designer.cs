namespace NeoBleeper
{
    partial class MIDI_file_player
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MIDI_file_player));
            textBox1 = new TextBox();
            label1 = new Label();
            groupBox1 = new GroupBox();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label_position = new Label();
            label_percentage = new Label();
            checkBox_loop = new CheckBox();
            icons2 = new ImageList(components);
            button_stop = new Button();
            icons = new ImageList(components);
            button_play = new Button();
            button_rewind = new Button();
            trackBar1 = new TrackBar();
            label4 = new Label();
            checkBox_channel_1 = new CheckBox();
            checkBox_channel_2 = new CheckBox();
            checkBox_channel_3 = new CheckBox();
            checkBox_channel_4 = new CheckBox();
            checkBox_channel_5 = new CheckBox();
            checkBox_channel_7 = new CheckBox();
            checkBox_channel_6 = new CheckBox();
            checkBox_channel_8 = new CheckBox();
            checkBox_channel_9 = new CheckBox();
            checkBox_channel_11 = new CheckBox();
            checkBox_channel_10 = new CheckBox();
            checkBox_channel_12 = new CheckBox();
            checkBox_channel_13 = new CheckBox();
            checkBox_channel_15 = new CheckBox();
            checkBox_channel_14 = new CheckBox();
            checkBox_channel_16 = new CheckBox();
            holded_note_label = new Label();
            label_note1 = new Label();
            label_note2 = new Label();
            label_note3 = new Label();
            label_note4 = new Label();
            label_note5 = new Label();
            label_note7 = new Label();
            label_note6 = new Label();
            label_note8 = new Label();
            label_note9 = new Label();
            label_note13 = new Label();
            label_note11 = new Label();
            label_note15 = new Label();
            label_note10 = new Label();
            label_note14 = new Label();
            label_note12 = new Label();
            label_note16 = new Label();
            label_note17 = new Label();
            label_note21 = new Label();
            label_note19 = new Label();
            label_note23 = new Label();
            label_note18 = new Label();
            label_note22 = new Label();
            label_note20 = new Label();
            label_note24 = new Label();
            label_note25 = new Label();
            label_note29 = new Label();
            label_note27 = new Label();
            label_note31 = new Label();
            label_note26 = new Label();
            label_note30 = new Label();
            label_note28 = new Label();
            label_note32 = new Label();
            button_browse_file = new Button();
            checkBox_play_each_note = new CheckBox();
            checkBox_make_each_cycle_last_30ms = new CheckBox();
            checkBox_dont_update_grid = new CheckBox();
            openFileDialog = new OpenFileDialog();
            toolTip1 = new ToolTip(components);
            label_alternating_note = new Label();
            numericUpDown_alternating_note = new NumericUpDown();
            label_ms = new Label();
            panel1 = new Panel();
            label_more_notes = new Label();
            panel2 = new Panel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            panelLoading = new Panel();
            progressBar1 = new ProgressBar();
            pictureBoxIcon = new PictureBox();
            labelStatus = new Label();
            checkBox_show_lyrics_or_text_events = new CheckBox();
            playbackTimer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_alternating_note).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            panelLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.BackColor = SystemColors.Window;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            toolTip1.SetToolTip(textBox1, resources.GetString("textBox1.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            toolTip1.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(flowLayoutPanel1);
            groupBox1.Controls.Add(checkBox_loop);
            groupBox1.Controls.Add(button_stop);
            groupBox1.Controls.Add(button_play);
            groupBox1.Controls.Add(button_rewind);
            groupBox1.Controls.Add(trackBar1);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            toolTip1.SetToolTip(groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
            flowLayoutPanel1.Controls.Add(label_position);
            flowLayoutPanel1.Controls.Add(label_percentage);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            toolTip1.SetToolTip(flowLayoutPanel1, resources.GetString("flowLayoutPanel1.ToolTip"));
            // 
            // label_position
            // 
            resources.ApplyResources(label_position, "label_position");
            label_position.Name = "label_position";
            toolTip1.SetToolTip(label_position, resources.GetString("label_position.ToolTip"));
            // 
            // label_percentage
            // 
            resources.ApplyResources(label_percentage, "label_percentage");
            label_percentage.Name = "label_percentage";
            toolTip1.SetToolTip(label_percentage, resources.GetString("label_percentage.ToolTip"));
            // 
            // checkBox_loop
            // 
            resources.ApplyResources(checkBox_loop, "checkBox_loop");
            checkBox_loop.ImageList = icons2;
            checkBox_loop.Name = "checkBox_loop";
            toolTip1.SetToolTip(checkBox_loop, resources.GetString("checkBox_loop.ToolTip"));
            checkBox_loop.UseVisualStyleBackColor = true;
            checkBox_loop.CheckedChanged += checkBox_loop_CheckedChanged;
            // 
            // icons2
            // 
            icons2.ColorDepth = ColorDepth.Depth32Bit;
            icons2.ImageStream = (ImageListStreamer)resources.GetObject("icons2.ImageStream");
            icons2.TransparentColor = Color.Transparent;
            icons2.Images.SetKeyName(0, "icons8-browse-folder-48.png");
            icons2.Images.SetKeyName(1, "icons8-loop-48.png");
            // 
            // button_stop
            // 
            resources.ApplyResources(button_stop, "button_stop");
            button_stop.FlatAppearance.BorderSize = 0;
            button_stop.ImageList = icons;
            button_stop.Name = "button_stop";
            toolTip1.SetToolTip(button_stop, resources.GetString("button_stop.ToolTip"));
            button_stop.UseVisualStyleBackColor = true;
            button_stop.Click += button_stop_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-play-48.png");
            icons.Images.SetKeyName(1, "icons8-pause-48.png");
            icons.Images.SetKeyName(2, "icons8-rewind-48.png");
            // 
            // button_play
            // 
            resources.ApplyResources(button_play, "button_play");
            button_play.FlatAppearance.BorderSize = 0;
            button_play.ImageList = icons;
            button_play.Name = "button_play";
            toolTip1.SetToolTip(button_play, resources.GetString("button_play.ToolTip"));
            button_play.UseVisualStyleBackColor = true;
            button_play.Click += button_play_Click;
            // 
            // button_rewind
            // 
            resources.ApplyResources(button_rewind, "button_rewind");
            button_rewind.FlatAppearance.BorderSize = 0;
            button_rewind.ImageList = icons;
            button_rewind.Name = "button_rewind";
            toolTip1.SetToolTip(button_rewind, resources.GetString("button_rewind.ToolTip"));
            button_rewind.UseVisualStyleBackColor = true;
            button_rewind.Click += button_rewind_Click;
            // 
            // trackBar1
            // 
            resources.ApplyResources(trackBar1, "trackBar1");
            trackBar1.Maximum = 1000;
            trackBar1.Name = "trackBar1";
            trackBar1.TickFrequency = 10;
            toolTip1.SetToolTip(trackBar1, resources.GetString("trackBar1.ToolTip"));
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            toolTip1.SetToolTip(label4, resources.GetString("label4.ToolTip"));
            // 
            // checkBox_channel_1
            // 
            resources.ApplyResources(checkBox_channel_1, "checkBox_channel_1");
            checkBox_channel_1.Checked = true;
            checkBox_channel_1.CheckState = CheckState.Checked;
            checkBox_channel_1.Name = "checkBox_channel_1";
            toolTip1.SetToolTip(checkBox_channel_1, resources.GetString("checkBox_channel_1.ToolTip"));
            checkBox_channel_1.UseVisualStyleBackColor = true;
            checkBox_channel_1.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_2
            // 
            resources.ApplyResources(checkBox_channel_2, "checkBox_channel_2");
            checkBox_channel_2.Checked = true;
            checkBox_channel_2.CheckState = CheckState.Checked;
            checkBox_channel_2.Name = "checkBox_channel_2";
            toolTip1.SetToolTip(checkBox_channel_2, resources.GetString("checkBox_channel_2.ToolTip"));
            checkBox_channel_2.UseVisualStyleBackColor = true;
            checkBox_channel_2.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_3
            // 
            resources.ApplyResources(checkBox_channel_3, "checkBox_channel_3");
            checkBox_channel_3.Checked = true;
            checkBox_channel_3.CheckState = CheckState.Checked;
            checkBox_channel_3.Name = "checkBox_channel_3";
            toolTip1.SetToolTip(checkBox_channel_3, resources.GetString("checkBox_channel_3.ToolTip"));
            checkBox_channel_3.UseVisualStyleBackColor = true;
            checkBox_channel_3.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_4
            // 
            resources.ApplyResources(checkBox_channel_4, "checkBox_channel_4");
            checkBox_channel_4.Checked = true;
            checkBox_channel_4.CheckState = CheckState.Checked;
            checkBox_channel_4.Name = "checkBox_channel_4";
            toolTip1.SetToolTip(checkBox_channel_4, resources.GetString("checkBox_channel_4.ToolTip"));
            checkBox_channel_4.UseVisualStyleBackColor = true;
            checkBox_channel_4.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_5
            // 
            resources.ApplyResources(checkBox_channel_5, "checkBox_channel_5");
            checkBox_channel_5.Checked = true;
            checkBox_channel_5.CheckState = CheckState.Checked;
            checkBox_channel_5.Name = "checkBox_channel_5";
            toolTip1.SetToolTip(checkBox_channel_5, resources.GetString("checkBox_channel_5.ToolTip"));
            checkBox_channel_5.UseVisualStyleBackColor = true;
            checkBox_channel_5.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_7
            // 
            resources.ApplyResources(checkBox_channel_7, "checkBox_channel_7");
            checkBox_channel_7.Checked = true;
            checkBox_channel_7.CheckState = CheckState.Checked;
            checkBox_channel_7.Name = "checkBox_channel_7";
            toolTip1.SetToolTip(checkBox_channel_7, resources.GetString("checkBox_channel_7.ToolTip"));
            checkBox_channel_7.UseVisualStyleBackColor = true;
            checkBox_channel_7.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_6
            // 
            resources.ApplyResources(checkBox_channel_6, "checkBox_channel_6");
            checkBox_channel_6.Checked = true;
            checkBox_channel_6.CheckState = CheckState.Checked;
            checkBox_channel_6.Name = "checkBox_channel_6";
            toolTip1.SetToolTip(checkBox_channel_6, resources.GetString("checkBox_channel_6.ToolTip"));
            checkBox_channel_6.UseVisualStyleBackColor = true;
            checkBox_channel_6.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_8
            // 
            resources.ApplyResources(checkBox_channel_8, "checkBox_channel_8");
            checkBox_channel_8.Checked = true;
            checkBox_channel_8.CheckState = CheckState.Checked;
            checkBox_channel_8.Name = "checkBox_channel_8";
            toolTip1.SetToolTip(checkBox_channel_8, resources.GetString("checkBox_channel_8.ToolTip"));
            checkBox_channel_8.UseVisualStyleBackColor = true;
            checkBox_channel_8.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_9
            // 
            resources.ApplyResources(checkBox_channel_9, "checkBox_channel_9");
            checkBox_channel_9.Checked = true;
            checkBox_channel_9.CheckState = CheckState.Checked;
            checkBox_channel_9.Name = "checkBox_channel_9";
            toolTip1.SetToolTip(checkBox_channel_9, resources.GetString("checkBox_channel_9.ToolTip"));
            checkBox_channel_9.UseVisualStyleBackColor = true;
            checkBox_channel_9.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_11
            // 
            resources.ApplyResources(checkBox_channel_11, "checkBox_channel_11");
            checkBox_channel_11.Checked = true;
            checkBox_channel_11.CheckState = CheckState.Checked;
            checkBox_channel_11.Name = "checkBox_channel_11";
            toolTip1.SetToolTip(checkBox_channel_11, resources.GetString("checkBox_channel_11.ToolTip"));
            checkBox_channel_11.UseVisualStyleBackColor = true;
            checkBox_channel_11.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_10
            // 
            resources.ApplyResources(checkBox_channel_10, "checkBox_channel_10");
            checkBox_channel_10.Checked = true;
            checkBox_channel_10.CheckState = CheckState.Checked;
            checkBox_channel_10.Name = "checkBox_channel_10";
            toolTip1.SetToolTip(checkBox_channel_10, resources.GetString("checkBox_channel_10.ToolTip"));
            checkBox_channel_10.UseVisualStyleBackColor = true;
            checkBox_channel_10.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_12
            // 
            resources.ApplyResources(checkBox_channel_12, "checkBox_channel_12");
            checkBox_channel_12.Checked = true;
            checkBox_channel_12.CheckState = CheckState.Checked;
            checkBox_channel_12.Name = "checkBox_channel_12";
            toolTip1.SetToolTip(checkBox_channel_12, resources.GetString("checkBox_channel_12.ToolTip"));
            checkBox_channel_12.UseVisualStyleBackColor = true;
            checkBox_channel_12.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_13
            // 
            resources.ApplyResources(checkBox_channel_13, "checkBox_channel_13");
            checkBox_channel_13.Checked = true;
            checkBox_channel_13.CheckState = CheckState.Checked;
            checkBox_channel_13.Name = "checkBox_channel_13";
            toolTip1.SetToolTip(checkBox_channel_13, resources.GetString("checkBox_channel_13.ToolTip"));
            checkBox_channel_13.UseVisualStyleBackColor = true;
            checkBox_channel_13.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_15
            // 
            resources.ApplyResources(checkBox_channel_15, "checkBox_channel_15");
            checkBox_channel_15.Checked = true;
            checkBox_channel_15.CheckState = CheckState.Checked;
            checkBox_channel_15.Name = "checkBox_channel_15";
            toolTip1.SetToolTip(checkBox_channel_15, resources.GetString("checkBox_channel_15.ToolTip"));
            checkBox_channel_15.UseVisualStyleBackColor = true;
            checkBox_channel_15.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_14
            // 
            resources.ApplyResources(checkBox_channel_14, "checkBox_channel_14");
            checkBox_channel_14.Checked = true;
            checkBox_channel_14.CheckState = CheckState.Checked;
            checkBox_channel_14.Name = "checkBox_channel_14";
            toolTip1.SetToolTip(checkBox_channel_14, resources.GetString("checkBox_channel_14.ToolTip"));
            checkBox_channel_14.UseVisualStyleBackColor = true;
            checkBox_channel_14.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_16
            // 
            resources.ApplyResources(checkBox_channel_16, "checkBox_channel_16");
            checkBox_channel_16.Checked = true;
            checkBox_channel_16.CheckState = CheckState.Checked;
            checkBox_channel_16.Name = "checkBox_channel_16";
            toolTip1.SetToolTip(checkBox_channel_16, resources.GetString("checkBox_channel_16.ToolTip"));
            checkBox_channel_16.UseVisualStyleBackColor = true;
            checkBox_channel_16.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // holded_note_label
            // 
            resources.ApplyResources(holded_note_label, "holded_note_label");
            holded_note_label.Name = "holded_note_label";
            toolTip1.SetToolTip(holded_note_label, resources.GetString("holded_note_label.ToolTip"));
            // 
            // label_note1
            // 
            resources.ApplyResources(label_note1, "label_note1");
            label_note1.AutoEllipsis = true;
            label_note1.BackColor = Color.Red;
            label_note1.Name = "label_note1";
            toolTip1.SetToolTip(label_note1, resources.GetString("label_note1.ToolTip"));
            // 
            // label_note2
            // 
            resources.ApplyResources(label_note2, "label_note2");
            label_note2.AutoEllipsis = true;
            label_note2.BackColor = Color.FromArgb(176, 0, 0);
            label_note2.Name = "label_note2";
            toolTip1.SetToolTip(label_note2, resources.GetString("label_note2.ToolTip"));
            // 
            // label_note3
            // 
            resources.ApplyResources(label_note3, "label_note3");
            label_note3.AutoEllipsis = true;
            label_note3.BackColor = Color.FromArgb(176, 0, 0);
            label_note3.Name = "label_note3";
            toolTip1.SetToolTip(label_note3, resources.GetString("label_note3.ToolTip"));
            // 
            // label_note4
            // 
            resources.ApplyResources(label_note4, "label_note4");
            label_note4.AutoEllipsis = true;
            label_note4.BackColor = Color.FromArgb(176, 0, 0);
            label_note4.Name = "label_note4";
            toolTip1.SetToolTip(label_note4, resources.GetString("label_note4.ToolTip"));
            // 
            // label_note5
            // 
            resources.ApplyResources(label_note5, "label_note5");
            label_note5.AutoEllipsis = true;
            label_note5.BackColor = Color.FromArgb(176, 0, 0);
            label_note5.Name = "label_note5";
            toolTip1.SetToolTip(label_note5, resources.GetString("label_note5.ToolTip"));
            // 
            // label_note7
            // 
            resources.ApplyResources(label_note7, "label_note7");
            label_note7.AutoEllipsis = true;
            label_note7.BackColor = Color.FromArgb(176, 0, 0);
            label_note7.Name = "label_note7";
            toolTip1.SetToolTip(label_note7, resources.GetString("label_note7.ToolTip"));
            // 
            // label_note6
            // 
            resources.ApplyResources(label_note6, "label_note6");
            label_note6.AutoEllipsis = true;
            label_note6.BackColor = Color.FromArgb(176, 0, 0);
            label_note6.Name = "label_note6";
            toolTip1.SetToolTip(label_note6, resources.GetString("label_note6.ToolTip"));
            // 
            // label_note8
            // 
            resources.ApplyResources(label_note8, "label_note8");
            label_note8.AutoEllipsis = true;
            label_note8.BackColor = Color.FromArgb(176, 0, 0);
            label_note8.Name = "label_note8";
            toolTip1.SetToolTip(label_note8, resources.GetString("label_note8.ToolTip"));
            // 
            // label_note9
            // 
            resources.ApplyResources(label_note9, "label_note9");
            label_note9.AutoEllipsis = true;
            label_note9.BackColor = Color.FromArgb(176, 0, 0);
            label_note9.Name = "label_note9";
            toolTip1.SetToolTip(label_note9, resources.GetString("label_note9.ToolTip"));
            // 
            // label_note13
            // 
            resources.ApplyResources(label_note13, "label_note13");
            label_note13.AutoEllipsis = true;
            label_note13.BackColor = Color.FromArgb(176, 0, 0);
            label_note13.Name = "label_note13";
            toolTip1.SetToolTip(label_note13, resources.GetString("label_note13.ToolTip"));
            // 
            // label_note11
            // 
            resources.ApplyResources(label_note11, "label_note11");
            label_note11.AutoEllipsis = true;
            label_note11.BackColor = Color.FromArgb(176, 0, 0);
            label_note11.Name = "label_note11";
            toolTip1.SetToolTip(label_note11, resources.GetString("label_note11.ToolTip"));
            // 
            // label_note15
            // 
            resources.ApplyResources(label_note15, "label_note15");
            label_note15.AutoEllipsis = true;
            label_note15.BackColor = Color.FromArgb(176, 0, 0);
            label_note15.Name = "label_note15";
            toolTip1.SetToolTip(label_note15, resources.GetString("label_note15.ToolTip"));
            // 
            // label_note10
            // 
            resources.ApplyResources(label_note10, "label_note10");
            label_note10.AutoEllipsis = true;
            label_note10.BackColor = Color.FromArgb(176, 0, 0);
            label_note10.Name = "label_note10";
            toolTip1.SetToolTip(label_note10, resources.GetString("label_note10.ToolTip"));
            // 
            // label_note14
            // 
            resources.ApplyResources(label_note14, "label_note14");
            label_note14.AutoEllipsis = true;
            label_note14.BackColor = Color.FromArgb(176, 0, 0);
            label_note14.Name = "label_note14";
            toolTip1.SetToolTip(label_note14, resources.GetString("label_note14.ToolTip"));
            // 
            // label_note12
            // 
            resources.ApplyResources(label_note12, "label_note12");
            label_note12.AutoEllipsis = true;
            label_note12.BackColor = Color.FromArgb(176, 0, 0);
            label_note12.Name = "label_note12";
            toolTip1.SetToolTip(label_note12, resources.GetString("label_note12.ToolTip"));
            // 
            // label_note16
            // 
            resources.ApplyResources(label_note16, "label_note16");
            label_note16.AutoEllipsis = true;
            label_note16.BackColor = Color.FromArgb(176, 0, 0);
            label_note16.Name = "label_note16";
            toolTip1.SetToolTip(label_note16, resources.GetString("label_note16.ToolTip"));
            // 
            // label_note17
            // 
            resources.ApplyResources(label_note17, "label_note17");
            label_note17.AutoEllipsis = true;
            label_note17.BackColor = Color.FromArgb(176, 0, 0);
            label_note17.Name = "label_note17";
            toolTip1.SetToolTip(label_note17, resources.GetString("label_note17.ToolTip"));
            // 
            // label_note21
            // 
            resources.ApplyResources(label_note21, "label_note21");
            label_note21.AutoEllipsis = true;
            label_note21.BackColor = Color.FromArgb(176, 0, 0);
            label_note21.Name = "label_note21";
            toolTip1.SetToolTip(label_note21, resources.GetString("label_note21.ToolTip"));
            // 
            // label_note19
            // 
            resources.ApplyResources(label_note19, "label_note19");
            label_note19.AutoEllipsis = true;
            label_note19.BackColor = Color.FromArgb(176, 0, 0);
            label_note19.Name = "label_note19";
            toolTip1.SetToolTip(label_note19, resources.GetString("label_note19.ToolTip"));
            // 
            // label_note23
            // 
            resources.ApplyResources(label_note23, "label_note23");
            label_note23.AutoEllipsis = true;
            label_note23.BackColor = Color.FromArgb(176, 0, 0);
            label_note23.Name = "label_note23";
            toolTip1.SetToolTip(label_note23, resources.GetString("label_note23.ToolTip"));
            // 
            // label_note18
            // 
            resources.ApplyResources(label_note18, "label_note18");
            label_note18.AutoEllipsis = true;
            label_note18.BackColor = Color.FromArgb(176, 0, 0);
            label_note18.Name = "label_note18";
            toolTip1.SetToolTip(label_note18, resources.GetString("label_note18.ToolTip"));
            // 
            // label_note22
            // 
            resources.ApplyResources(label_note22, "label_note22");
            label_note22.AutoEllipsis = true;
            label_note22.BackColor = Color.FromArgb(176, 0, 0);
            label_note22.Name = "label_note22";
            toolTip1.SetToolTip(label_note22, resources.GetString("label_note22.ToolTip"));
            // 
            // label_note20
            // 
            resources.ApplyResources(label_note20, "label_note20");
            label_note20.AutoEllipsis = true;
            label_note20.BackColor = Color.FromArgb(176, 0, 0);
            label_note20.Name = "label_note20";
            toolTip1.SetToolTip(label_note20, resources.GetString("label_note20.ToolTip"));
            // 
            // label_note24
            // 
            resources.ApplyResources(label_note24, "label_note24");
            label_note24.AutoEllipsis = true;
            label_note24.BackColor = Color.FromArgb(176, 0, 0);
            label_note24.Name = "label_note24";
            toolTip1.SetToolTip(label_note24, resources.GetString("label_note24.ToolTip"));
            // 
            // label_note25
            // 
            resources.ApplyResources(label_note25, "label_note25");
            label_note25.AutoEllipsis = true;
            label_note25.BackColor = Color.FromArgb(176, 0, 0);
            label_note25.Name = "label_note25";
            toolTip1.SetToolTip(label_note25, resources.GetString("label_note25.ToolTip"));
            // 
            // label_note29
            // 
            resources.ApplyResources(label_note29, "label_note29");
            label_note29.AutoEllipsis = true;
            label_note29.BackColor = Color.FromArgb(176, 0, 0);
            label_note29.Name = "label_note29";
            toolTip1.SetToolTip(label_note29, resources.GetString("label_note29.ToolTip"));
            // 
            // label_note27
            // 
            resources.ApplyResources(label_note27, "label_note27");
            label_note27.AutoEllipsis = true;
            label_note27.BackColor = Color.FromArgb(176, 0, 0);
            label_note27.Name = "label_note27";
            toolTip1.SetToolTip(label_note27, resources.GetString("label_note27.ToolTip"));
            // 
            // label_note31
            // 
            resources.ApplyResources(label_note31, "label_note31");
            label_note31.AutoEllipsis = true;
            label_note31.BackColor = Color.FromArgb(176, 0, 0);
            label_note31.Name = "label_note31";
            toolTip1.SetToolTip(label_note31, resources.GetString("label_note31.ToolTip"));
            // 
            // label_note26
            // 
            resources.ApplyResources(label_note26, "label_note26");
            label_note26.AutoEllipsis = true;
            label_note26.BackColor = Color.FromArgb(176, 0, 0);
            label_note26.Name = "label_note26";
            toolTip1.SetToolTip(label_note26, resources.GetString("label_note26.ToolTip"));
            // 
            // label_note30
            // 
            resources.ApplyResources(label_note30, "label_note30");
            label_note30.AutoEllipsis = true;
            label_note30.BackColor = Color.FromArgb(176, 0, 0);
            label_note30.Name = "label_note30";
            toolTip1.SetToolTip(label_note30, resources.GetString("label_note30.ToolTip"));
            // 
            // label_note28
            // 
            resources.ApplyResources(label_note28, "label_note28");
            label_note28.AutoEllipsis = true;
            label_note28.BackColor = Color.FromArgb(176, 0, 0);
            label_note28.Name = "label_note28";
            toolTip1.SetToolTip(label_note28, resources.GetString("label_note28.ToolTip"));
            // 
            // label_note32
            // 
            resources.ApplyResources(label_note32, "label_note32");
            label_note32.AutoEllipsis = true;
            label_note32.BackColor = Color.FromArgb(176, 0, 0);
            label_note32.Name = "label_note32";
            toolTip1.SetToolTip(label_note32, resources.GetString("label_note32.ToolTip"));
            // 
            // button_browse_file
            // 
            resources.ApplyResources(button_browse_file, "button_browse_file");
            button_browse_file.ImageList = icons2;
            button_browse_file.Name = "button_browse_file";
            toolTip1.SetToolTip(button_browse_file, resources.GetString("button_browse_file.ToolTip"));
            button_browse_file.UseVisualStyleBackColor = true;
            button_browse_file.Click += button4_Click;
            // 
            // checkBox_play_each_note
            // 
            resources.ApplyResources(checkBox_play_each_note, "checkBox_play_each_note");
            checkBox_play_each_note.Name = "checkBox_play_each_note";
            toolTip1.SetToolTip(checkBox_play_each_note, resources.GetString("checkBox_play_each_note.ToolTip"));
            checkBox_play_each_note.UseVisualStyleBackColor = true;
            checkBox_play_each_note.CheckedChanged += checkBox_play_each_note_CheckedChanged;
            // 
            // checkBox_make_each_cycle_last_30ms
            // 
            resources.ApplyResources(checkBox_make_each_cycle_last_30ms, "checkBox_make_each_cycle_last_30ms");
            checkBox_make_each_cycle_last_30ms.Checked = true;
            checkBox_make_each_cycle_last_30ms.CheckState = CheckState.Checked;
            checkBox_make_each_cycle_last_30ms.Name = "checkBox_make_each_cycle_last_30ms";
            toolTip1.SetToolTip(checkBox_make_each_cycle_last_30ms, resources.GetString("checkBox_make_each_cycle_last_30ms.ToolTip"));
            checkBox_make_each_cycle_last_30ms.UseVisualStyleBackColor = true;
            checkBox_make_each_cycle_last_30ms.CheckedChanged += disable_alternating_notes_panel;
            // 
            // checkBox_dont_update_grid
            // 
            resources.ApplyResources(checkBox_dont_update_grid, "checkBox_dont_update_grid");
            checkBox_dont_update_grid.Name = "checkBox_dont_update_grid";
            toolTip1.SetToolTip(checkBox_dont_update_grid, resources.GetString("checkBox_dont_update_grid.ToolTip"));
            checkBox_dont_update_grid.UseVisualStyleBackColor = true;
            checkBox_dont_update_grid.CheckedChanged += checkBox_dont_update_grid_CheckedChanged;
            // 
            // openFileDialog
            // 
            resources.ApplyResources(openFileDialog, "openFileDialog");
            // 
            // label_alternating_note
            // 
            resources.ApplyResources(label_alternating_note, "label_alternating_note");
            label_alternating_note.Name = "label_alternating_note";
            toolTip1.SetToolTip(label_alternating_note, resources.GetString("label_alternating_note.ToolTip"));
            // 
            // numericUpDown_alternating_note
            // 
            resources.ApplyResources(numericUpDown_alternating_note, "numericUpDown_alternating_note");
            numericUpDown_alternating_note.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            numericUpDown_alternating_note.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numericUpDown_alternating_note.Name = "numericUpDown_alternating_note";
            toolTip1.SetToolTip(numericUpDown_alternating_note, resources.GetString("numericUpDown_alternating_note.ToolTip"));
            numericUpDown_alternating_note.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numericUpDown_alternating_note.ValueChanged += numericUpDown_alternating_note_ValueChanged;
            // 
            // label_ms
            // 
            resources.ApplyResources(label_ms, "label_ms");
            label_ms.Name = "label_ms";
            toolTip1.SetToolTip(label_ms, resources.GetString("label_ms.ToolTip"));
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(label_alternating_note);
            panel1.Controls.Add(label_ms);
            panel1.Controls.Add(numericUpDown_alternating_note);
            panel1.Name = "panel1";
            toolTip1.SetToolTip(panel1, resources.GetString("panel1.ToolTip"));
            // 
            // label_more_notes
            // 
            resources.ApplyResources(label_more_notes, "label_more_notes");
            label_more_notes.Name = "label_more_notes";
            toolTip1.SetToolTip(label_more_notes, resources.GetString("label_more_notes.ToolTip"));
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Controls.Add(label_note1);
            panel2.Controls.Add(label_note9);
            panel2.Controls.Add(label_note17);
            panel2.Controls.Add(label_note25);
            panel2.Controls.Add(label_note5);
            panel2.Controls.Add(label_note13);
            panel2.Controls.Add(label_note21);
            panel2.Controls.Add(label_note29);
            panel2.Controls.Add(label_note3);
            panel2.Controls.Add(label_note11);
            panel2.Controls.Add(label_note19);
            panel2.Controls.Add(label_note27);
            panel2.Controls.Add(label_note7);
            panel2.Controls.Add(label_note15);
            panel2.Controls.Add(label_note23);
            panel2.Controls.Add(label_note31);
            panel2.Controls.Add(label_note2);
            panel2.Controls.Add(label_note10);
            panel2.Controls.Add(label_note18);
            panel2.Controls.Add(label_note26);
            panel2.Controls.Add(label_note6);
            panel2.Controls.Add(label_note14);
            panel2.Controls.Add(label_note22);
            panel2.Controls.Add(label_note32);
            panel2.Controls.Add(label_note30);
            panel2.Controls.Add(label_note24);
            panel2.Controls.Add(label_note4);
            panel2.Controls.Add(label_note16);
            panel2.Controls.Add(label_note12);
            panel2.Controls.Add(label_note8);
            panel2.Controls.Add(label_note20);
            panel2.Controls.Add(label_note28);
            panel2.Name = "panel2";
            toolTip1.SetToolTip(panel2, resources.GetString("panel2.ToolTip"));
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(flowLayoutPanel2, "flowLayoutPanel2");
            flowLayoutPanel2.Controls.Add(panel2);
            flowLayoutPanel2.Controls.Add(label_more_notes);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            toolTip1.SetToolTip(flowLayoutPanel2, resources.GetString("flowLayoutPanel2.ToolTip"));
            // 
            // panelLoading
            // 
            resources.ApplyResources(panelLoading, "panelLoading");
            panelLoading.Controls.Add(progressBar1);
            panelLoading.Controls.Add(pictureBoxIcon);
            panelLoading.Controls.Add(labelStatus);
            panelLoading.Name = "panelLoading";
            toolTip1.SetToolTip(panelLoading, resources.GetString("panelLoading.ToolTip"));
            // 
            // progressBar1
            // 
            resources.ApplyResources(progressBar1, "progressBar1");
            progressBar1.Name = "progressBar1";
            toolTip1.SetToolTip(progressBar1, resources.GetString("progressBar1.ToolTip"));
            // 
            // pictureBoxIcon
            // 
            resources.ApplyResources(pictureBoxIcon, "pictureBoxIcon");
            pictureBoxIcon.Image = Properties.Resources.icons8_wait_96;
            pictureBoxIcon.Name = "pictureBoxIcon";
            pictureBoxIcon.TabStop = false;
            toolTip1.SetToolTip(pictureBoxIcon, resources.GetString("pictureBoxIcon.ToolTip"));
            // 
            // labelStatus
            // 
            resources.ApplyResources(labelStatus, "labelStatus");
            labelStatus.Name = "labelStatus";
            toolTip1.SetToolTip(labelStatus, resources.GetString("labelStatus.ToolTip"));
            // 
            // checkBox_show_lyrics_or_text_events
            // 
            resources.ApplyResources(checkBox_show_lyrics_or_text_events, "checkBox_show_lyrics_or_text_events");
            checkBox_show_lyrics_or_text_events.Name = "checkBox_show_lyrics_or_text_events";
            toolTip1.SetToolTip(checkBox_show_lyrics_or_text_events, resources.GetString("checkBox_show_lyrics_or_text_events.ToolTip"));
            checkBox_show_lyrics_or_text_events.UseVisualStyleBackColor = true;
            checkBox_show_lyrics_or_text_events.CheckedChanged += checkBox_show_lyrics_or_text_events_CheckedChanged;
            // 
            // playbackTimer
            // 
            playbackTimer.Interval = 1;
            playbackTimer.Tick += playbackTimer_Tick;
            // 
            // MIDI_file_player
            // 
            resources.ApplyResources(this, "$this");
            AllowDrop = true;
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(checkBox_show_lyrics_or_text_events);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(panel1);
            Controls.Add(button_browse_file);
            Controls.Add(checkBox_channel_16);
            Controls.Add(checkBox_make_each_cycle_last_30ms);
            Controls.Add(checkBox_dont_update_grid);
            Controls.Add(checkBox_play_each_note);
            Controls.Add(checkBox_channel_12);
            Controls.Add(checkBox_channel_8);
            Controls.Add(checkBox_channel_4);
            Controls.Add(checkBox_channel_14);
            Controls.Add(checkBox_channel_10);
            Controls.Add(checkBox_channel_6);
            Controls.Add(checkBox_channel_2);
            Controls.Add(checkBox_channel_15);
            Controls.Add(checkBox_channel_11);
            Controls.Add(checkBox_channel_7);
            Controls.Add(checkBox_channel_3);
            Controls.Add(checkBox_channel_13);
            Controls.Add(checkBox_channel_9);
            Controls.Add(checkBox_channel_5);
            Controls.Add(checkBox_channel_1);
            Controls.Add(holded_note_label);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(panelLoading);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MIDI_file_player";
            ShowIcon = false;
            ShowInTaskbar = false;
            toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            FormClosing += MIDI_file_player_FormClosing;
            Load += MIDI_file_player_Load;
            DragDrop += MIDI_file_player_DragDrop;
            DragEnter += MIDI_file_player_DragEnter;
            SystemColorsChanged += MIDI_file_player_SystemColorsChanged;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_alternating_note).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            panelLoading.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private GroupBox groupBox1;
        private TrackBar trackBar1;
        private Button button_stop;
        private Button button_play;
        private ImageList icons;
        private Button button_rewind;
        private CheckBox checkBox_loop;
        private Label label_position;
        private Label label4;
        private CheckBox checkBox_channel_1;
        private CheckBox checkBox_channel_2;
        private CheckBox checkBox_channel_3;
        private CheckBox checkBox_channel_4;
        private CheckBox checkBox_channel_5;
        private CheckBox checkBox_channel_7;
        private CheckBox checkBox_channel_6;
        private CheckBox checkBox_channel_8;
        private CheckBox checkBox_channel_9;
        private CheckBox checkBox_channel_11;
        private CheckBox checkBox_channel_10;
        private CheckBox checkBox_channel_12;
        private CheckBox checkBox_channel_13;
        private CheckBox checkBox_channel_15;
        private CheckBox checkBox_channel_14;
        private CheckBox checkBox_channel_16;
        private Label holded_note_label;
        private Label label_note1;
        private Label label_note2;
        private Label label_note3;
        private Label label_note4;
        private Label label_note5;
        private Label label_note7;
        private Label label_note6;
        private Label label_note8;
        private Label label_note9;
        private Label label_note13;
        private Label label_note11;
        private Label label_note15;
        private Label label_note10;
        private Label label_note14;
        private Label label_note12;
        private Label label_note16;
        private Label label_note17;
        private Label label_note21;
        private Label label_note19;
        private Label label_note23;
        private Label label_note18;
        private Label label_note22;
        private Label label_note20;
        private Label label_note24;
        private Label label_note25;
        private Label label_note29;
        private Label label_note27;
        private Label label_note31;
        private Label label_note26;
        private Label label_note30;
        private Label label_note28;
        private Label label_note32;
        private Button button_browse_file;
        private ImageList icons2;
        private CheckBox checkBox_play_each_note;
        private CheckBox checkBox_make_each_cycle_last_30ms;
        private CheckBox checkBox_dont_update_grid;
        private OpenFileDialog openFileDialog;
        private ToolTip toolTip1;
        private System.Windows.Forms.Timer resetHighlightTimer;
        private Label label_alternating_note;
        private NumericUpDown numericUpDown_alternating_note;
        private Label label_ms;
        private Panel panel1;
        private Label label_percentage;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label_more_notes;
        private Panel panel2;
        private FlowLayoutPanel flowLayoutPanel2;
        private Panel panelLoading;
        private PictureBox pictureBoxIcon;
        private Label labelStatus;
        private ProgressBar progressBar1;
        private System.Windows.Forms.Timer updatePlaybackPositionTimer;
        private System.Windows.Forms.Timer playbackTimer;
        private CheckBox checkBox_show_lyrics_or_text_events;
    }
}