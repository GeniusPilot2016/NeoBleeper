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
            playbackTimer = new System.Windows.Forms.Timer(components);
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
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = SystemColors.Window;
            textBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(9, 26);
            textBox1.Margin = new Padding(3, 2, 3, 2);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(217, 23);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(10, 7);
            label1.Name = "label1";
            label1.Size = new Size(151, 16);
            label1.TabIndex = 1;
            label1.Text = "Currently playing MIDI file:";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(flowLayoutPanel1);
            groupBox1.Controls.Add(checkBox_loop);
            groupBox1.Controls.Add(button_stop);
            groupBox1.Controls.Add(button_play);
            groupBox1.Controls.Add(button_rewind);
            groupBox1.Controls.Add(trackBar1);
            groupBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(10, 52);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(328, 103);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Playback Controls";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(label_position);
            flowLayoutPanel1.Controls.Add(label_percentage);
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(184, 55);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(137, 40);
            flowLayoutPanel1.TabIndex = 28;
            // 
            // label_position
            // 
            label_position.Anchor = AnchorStyles.Right;
            label_position.AutoSize = true;
            label_position.Font = new Font("HarmonyOS Sans", 9F);
            label_position.Location = new Point(30, 0);
            label_position.Name = "label_position";
            label_position.Size = new Size(104, 16);
            label_position.TabIndex = 3;
            label_position.Text = "Position: 00:00.00";
            // 
            // label_percentage
            // 
            label_percentage.Anchor = AnchorStyles.Right;
            label_percentage.AutoSize = true;
            label_percentage.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label_percentage.Location = new Point(94, 16);
            label_percentage.MaximumSize = new Size(68, 16);
            label_percentage.Name = "label_percentage";
            label_percentage.RightToLeft = RightToLeft.Yes;
            label_percentage.Size = new Size(40, 16);
            label_percentage.TabIndex = 4;
            label_percentage.Text = "0,00%";
            label_percentage.TextAlign = ContentAlignment.BottomRight;
            // 
            // checkBox_loop
            // 
            checkBox_loop.Anchor = AnchorStyles.Left;
            checkBox_loop.AutoSize = true;
            checkBox_loop.ImageIndex = 1;
            checkBox_loop.ImageList = icons2;
            checkBox_loop.Location = new Point(112, 56);
            checkBox_loop.Margin = new Padding(3, 2, 3, 2);
            checkBox_loop.Name = "checkBox_loop";
            checkBox_loop.Size = new Size(70, 20);
            checkBox_loop.TabIndex = 5;
            checkBox_loop.Text = "Loop";
            checkBox_loop.TextImageRelation = TextImageRelation.ImageBeforeText;
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
            button_stop.Anchor = AnchorStyles.Left;
            button_stop.Enabled = false;
            button_stop.FlatAppearance.BorderSize = 0;
            button_stop.FlatStyle = FlatStyle.Flat;
            button_stop.ImageIndex = 1;
            button_stop.ImageList = icons;
            button_stop.Location = new Point(71, 55);
            button_stop.Margin = new Padding(3, 2, 3, 2);
            button_stop.Name = "button_stop";
            button_stop.Size = new Size(24, 20);
            button_stop.TabIndex = 4;
            toolTip1.SetToolTip(button_stop, "Stop");
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
            button_play.Anchor = AnchorStyles.Left;
            button_play.FlatAppearance.BorderSize = 0;
            button_play.FlatStyle = FlatStyle.Flat;
            button_play.ImageIndex = 0;
            button_play.ImageList = icons;
            button_play.Location = new Point(42, 55);
            button_play.Margin = new Padding(3, 2, 3, 2);
            button_play.Name = "button_play";
            button_play.Size = new Size(24, 20);
            button_play.TabIndex = 3;
            toolTip1.SetToolTip(button_play, "Play from current position");
            button_play.UseVisualStyleBackColor = true;
            button_play.Click += button_play_Click;
            // 
            // button_rewind
            // 
            button_rewind.Anchor = AnchorStyles.Left;
            button_rewind.FlatAppearance.BorderSize = 0;
            button_rewind.FlatStyle = FlatStyle.Flat;
            button_rewind.ImageIndex = 2;
            button_rewind.ImageList = icons;
            button_rewind.Location = new Point(13, 55);
            button_rewind.Margin = new Padding(3, 2, 3, 2);
            button_rewind.Name = "button_rewind";
            button_rewind.Size = new Size(24, 20);
            button_rewind.TabIndex = 2;
            toolTip1.SetToolTip(button_rewind, "Rewind");
            button_rewind.UseVisualStyleBackColor = true;
            button_rewind.Click += button_rewind_Click;
            // 
            // trackBar1
            // 
            trackBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBar1.Location = new Point(5, 20);
            trackBar1.Margin = new Padding(3, 2, 3, 2);
            trackBar1.Maximum = 1000;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(317, 45);
            trackBar1.TabIndex = 1;
            trackBar1.TickFrequency = 10;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // label4
            // 
            label4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(10, 166);
            label4.MinimumSize = new Size(321, 32);
            label4.Name = "label4";
            label4.Size = new Size(321, 32);
            label4.TabIndex = 3;
            label4.Text = "Select the channel(s) that should be listened to. \r\nChannels that are not selected will be ignored.";
            // 
            // checkBox_channel_1
            // 
            checkBox_channel_1.AutoSize = true;
            checkBox_channel_1.Checked = true;
            checkBox_channel_1.CheckState = CheckState.Checked;
            checkBox_channel_1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_1.Location = new Point(13, 207);
            checkBox_channel_1.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_1.Name = "checkBox_channel_1";
            checkBox_channel_1.Size = new Size(33, 20);
            checkBox_channel_1.TabIndex = 6;
            checkBox_channel_1.Text = "1";
            checkBox_channel_1.UseVisualStyleBackColor = true;
            checkBox_channel_1.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_2
            // 
            checkBox_channel_2.AutoSize = true;
            checkBox_channel_2.Checked = true;
            checkBox_channel_2.CheckState = CheckState.Checked;
            checkBox_channel_2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_2.Location = new Point(61, 207);
            checkBox_channel_2.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_2.Name = "checkBox_channel_2";
            checkBox_channel_2.Size = new Size(33, 20);
            checkBox_channel_2.TabIndex = 7;
            checkBox_channel_2.Text = "2";
            checkBox_channel_2.UseVisualStyleBackColor = true;
            checkBox_channel_2.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_3
            // 
            checkBox_channel_3.AutoSize = true;
            checkBox_channel_3.Checked = true;
            checkBox_channel_3.CheckState = CheckState.Checked;
            checkBox_channel_3.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_3.Location = new Point(110, 207);
            checkBox_channel_3.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_3.Name = "checkBox_channel_3";
            checkBox_channel_3.Size = new Size(33, 20);
            checkBox_channel_3.TabIndex = 8;
            checkBox_channel_3.Text = "3";
            checkBox_channel_3.UseVisualStyleBackColor = true;
            checkBox_channel_3.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_4
            // 
            checkBox_channel_4.AutoSize = true;
            checkBox_channel_4.Checked = true;
            checkBox_channel_4.CheckState = CheckState.Checked;
            checkBox_channel_4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_4.Location = new Point(159, 207);
            checkBox_channel_4.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_4.Name = "checkBox_channel_4";
            checkBox_channel_4.Size = new Size(33, 20);
            checkBox_channel_4.TabIndex = 9;
            checkBox_channel_4.Text = "4";
            checkBox_channel_4.UseVisualStyleBackColor = true;
            checkBox_channel_4.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_5
            // 
            checkBox_channel_5.AutoSize = true;
            checkBox_channel_5.Checked = true;
            checkBox_channel_5.CheckState = CheckState.Checked;
            checkBox_channel_5.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_5.Location = new Point(13, 231);
            checkBox_channel_5.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_5.Name = "checkBox_channel_5";
            checkBox_channel_5.Size = new Size(33, 20);
            checkBox_channel_5.TabIndex = 10;
            checkBox_channel_5.Text = "5";
            checkBox_channel_5.UseVisualStyleBackColor = true;
            checkBox_channel_5.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_7
            // 
            checkBox_channel_7.AutoSize = true;
            checkBox_channel_7.Checked = true;
            checkBox_channel_7.CheckState = CheckState.Checked;
            checkBox_channel_7.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_7.Location = new Point(110, 231);
            checkBox_channel_7.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_7.Name = "checkBox_channel_7";
            checkBox_channel_7.Size = new Size(33, 20);
            checkBox_channel_7.TabIndex = 12;
            checkBox_channel_7.Text = "7";
            checkBox_channel_7.UseVisualStyleBackColor = true;
            checkBox_channel_7.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_6
            // 
            checkBox_channel_6.AutoSize = true;
            checkBox_channel_6.Checked = true;
            checkBox_channel_6.CheckState = CheckState.Checked;
            checkBox_channel_6.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_6.Location = new Point(61, 231);
            checkBox_channel_6.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_6.Name = "checkBox_channel_6";
            checkBox_channel_6.Size = new Size(33, 20);
            checkBox_channel_6.TabIndex = 11;
            checkBox_channel_6.Text = "6";
            checkBox_channel_6.UseVisualStyleBackColor = true;
            checkBox_channel_6.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_8
            // 
            checkBox_channel_8.AutoSize = true;
            checkBox_channel_8.Checked = true;
            checkBox_channel_8.CheckState = CheckState.Checked;
            checkBox_channel_8.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_8.Location = new Point(159, 231);
            checkBox_channel_8.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_8.Name = "checkBox_channel_8";
            checkBox_channel_8.Size = new Size(33, 20);
            checkBox_channel_8.TabIndex = 13;
            checkBox_channel_8.Text = "8";
            checkBox_channel_8.UseVisualStyleBackColor = true;
            checkBox_channel_8.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_9
            // 
            checkBox_channel_9.AutoSize = true;
            checkBox_channel_9.Checked = true;
            checkBox_channel_9.CheckState = CheckState.Checked;
            checkBox_channel_9.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_9.Location = new Point(13, 254);
            checkBox_channel_9.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_9.Name = "checkBox_channel_9";
            checkBox_channel_9.Size = new Size(33, 20);
            checkBox_channel_9.TabIndex = 14;
            checkBox_channel_9.Text = "9";
            checkBox_channel_9.UseVisualStyleBackColor = true;
            checkBox_channel_9.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_11
            // 
            checkBox_channel_11.AutoSize = true;
            checkBox_channel_11.Checked = true;
            checkBox_channel_11.CheckState = CheckState.Checked;
            checkBox_channel_11.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_11.Location = new Point(110, 254);
            checkBox_channel_11.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_11.Name = "checkBox_channel_11";
            checkBox_channel_11.Size = new Size(40, 20);
            checkBox_channel_11.TabIndex = 16;
            checkBox_channel_11.Text = "11";
            checkBox_channel_11.UseVisualStyleBackColor = true;
            checkBox_channel_11.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_10
            // 
            checkBox_channel_10.AutoSize = true;
            checkBox_channel_10.Checked = true;
            checkBox_channel_10.CheckState = CheckState.Checked;
            checkBox_channel_10.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_10.Location = new Point(61, 254);
            checkBox_channel_10.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_10.Name = "checkBox_channel_10";
            checkBox_channel_10.Size = new Size(40, 20);
            checkBox_channel_10.TabIndex = 15;
            checkBox_channel_10.Text = "10";
            checkBox_channel_10.UseVisualStyleBackColor = true;
            checkBox_channel_10.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_12
            // 
            checkBox_channel_12.AutoSize = true;
            checkBox_channel_12.Checked = true;
            checkBox_channel_12.CheckState = CheckState.Checked;
            checkBox_channel_12.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_12.Location = new Point(159, 254);
            checkBox_channel_12.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_12.Name = "checkBox_channel_12";
            checkBox_channel_12.Size = new Size(40, 20);
            checkBox_channel_12.TabIndex = 17;
            checkBox_channel_12.Text = "12";
            checkBox_channel_12.UseVisualStyleBackColor = true;
            checkBox_channel_12.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_13
            // 
            checkBox_channel_13.AutoSize = true;
            checkBox_channel_13.Checked = true;
            checkBox_channel_13.CheckState = CheckState.Checked;
            checkBox_channel_13.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_13.Location = new Point(13, 276);
            checkBox_channel_13.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_13.Name = "checkBox_channel_13";
            checkBox_channel_13.Size = new Size(40, 20);
            checkBox_channel_13.TabIndex = 18;
            checkBox_channel_13.Text = "13";
            checkBox_channel_13.UseVisualStyleBackColor = true;
            checkBox_channel_13.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_15
            // 
            checkBox_channel_15.AutoSize = true;
            checkBox_channel_15.Checked = true;
            checkBox_channel_15.CheckState = CheckState.Checked;
            checkBox_channel_15.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_15.Location = new Point(110, 276);
            checkBox_channel_15.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_15.Name = "checkBox_channel_15";
            checkBox_channel_15.Size = new Size(40, 20);
            checkBox_channel_15.TabIndex = 20;
            checkBox_channel_15.Text = "15";
            checkBox_channel_15.UseVisualStyleBackColor = true;
            checkBox_channel_15.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_14
            // 
            checkBox_channel_14.AutoSize = true;
            checkBox_channel_14.Checked = true;
            checkBox_channel_14.CheckState = CheckState.Checked;
            checkBox_channel_14.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_14.Location = new Point(61, 276);
            checkBox_channel_14.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_14.Name = "checkBox_channel_14";
            checkBox_channel_14.Size = new Size(40, 20);
            checkBox_channel_14.TabIndex = 19;
            checkBox_channel_14.Text = "14";
            checkBox_channel_14.UseVisualStyleBackColor = true;
            checkBox_channel_14.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // checkBox_channel_16
            // 
            checkBox_channel_16.AutoSize = true;
            checkBox_channel_16.Checked = true;
            checkBox_channel_16.CheckState = CheckState.Checked;
            checkBox_channel_16.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_channel_16.Location = new Point(159, 276);
            checkBox_channel_16.Margin = new Padding(3, 2, 3, 2);
            checkBox_channel_16.Name = "checkBox_channel_16";
            checkBox_channel_16.Size = new Size(40, 20);
            checkBox_channel_16.TabIndex = 21;
            checkBox_channel_16.Text = "16";
            checkBox_channel_16.UseVisualStyleBackColor = true;
            checkBox_channel_16.CheckedChanged += checkBox_channel_CheckedChanged;
            // 
            // holded_note_label
            // 
            holded_note_label.AutoSize = true;
            holded_note_label.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            holded_note_label.Location = new Point(9, 309);
            holded_note_label.Name = "holded_note_label";
            holded_note_label.Size = new Size(245, 16);
            holded_note_label.TabIndex = 3;
            holded_note_label.Text = "Notes which are currently being held on: (0)";
            // 
            // label_note1
            // 
            label_note1.Anchor = AnchorStyles.None;
            label_note1.AutoEllipsis = true;
            label_note1.AutoSize = true;
            label_note1.BackColor = Color.Red;
            label_note1.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note1.Location = new Point(6, 10);
            label_note1.MaximumSize = new Size(37, 18);
            label_note1.MinimumSize = new Size(37, 18);
            label_note1.Name = "label_note1";
            label_note1.Size = new Size(37, 18);
            label_note1.TabIndex = 3;
            label_note1.Text = "C#4";
            label_note1.TextAlign = ContentAlignment.MiddleCenter;
            label_note1.Visible = false;
            // 
            // label_note2
            // 
            label_note2.Anchor = AnchorStyles.None;
            label_note2.AutoEllipsis = true;
            label_note2.AutoSize = true;
            label_note2.BackColor = Color.FromArgb(176, 0, 0);
            label_note2.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note2.Location = new Point(47, 10);
            label_note2.MaximumSize = new Size(37, 18);
            label_note2.MinimumSize = new Size(37, 18);
            label_note2.Name = "label_note2";
            label_note2.Size = new Size(37, 18);
            label_note2.TabIndex = 3;
            label_note2.Text = "C#4";
            label_note2.TextAlign = ContentAlignment.MiddleCenter;
            label_note2.Visible = false;
            // 
            // label_note3
            // 
            label_note3.Anchor = AnchorStyles.None;
            label_note3.AutoEllipsis = true;
            label_note3.AutoSize = true;
            label_note3.BackColor = Color.FromArgb(176, 0, 0);
            label_note3.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note3.Location = new Point(88, 10);
            label_note3.MaximumSize = new Size(37, 18);
            label_note3.MinimumSize = new Size(37, 18);
            label_note3.Name = "label_note3";
            label_note3.Size = new Size(37, 18);
            label_note3.TabIndex = 3;
            label_note3.Text = "C#4";
            label_note3.TextAlign = ContentAlignment.MiddleCenter;
            label_note3.Visible = false;
            // 
            // label_note4
            // 
            label_note4.Anchor = AnchorStyles.None;
            label_note4.AutoEllipsis = true;
            label_note4.AutoSize = true;
            label_note4.BackColor = Color.FromArgb(176, 0, 0);
            label_note4.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note4.Location = new Point(129, 10);
            label_note4.MaximumSize = new Size(37, 18);
            label_note4.MinimumSize = new Size(37, 18);
            label_note4.Name = "label_note4";
            label_note4.Size = new Size(37, 18);
            label_note4.TabIndex = 3;
            label_note4.Text = "C#4";
            label_note4.TextAlign = ContentAlignment.MiddleCenter;
            label_note4.Visible = false;
            // 
            // label_note5
            // 
            label_note5.Anchor = AnchorStyles.None;
            label_note5.AutoEllipsis = true;
            label_note5.AutoSize = true;
            label_note5.BackColor = Color.FromArgb(176, 0, 0);
            label_note5.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note5.Location = new Point(170, 10);
            label_note5.MaximumSize = new Size(37, 18);
            label_note5.MinimumSize = new Size(37, 18);
            label_note5.Name = "label_note5";
            label_note5.Size = new Size(37, 18);
            label_note5.TabIndex = 3;
            label_note5.Text = "C#4";
            label_note5.TextAlign = ContentAlignment.MiddleCenter;
            label_note5.Visible = false;
            // 
            // label_note7
            // 
            label_note7.Anchor = AnchorStyles.None;
            label_note7.AutoEllipsis = true;
            label_note7.AutoSize = true;
            label_note7.BackColor = Color.FromArgb(176, 0, 0);
            label_note7.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note7.Location = new Point(252, 10);
            label_note7.MaximumSize = new Size(37, 18);
            label_note7.MinimumSize = new Size(37, 18);
            label_note7.Name = "label_note7";
            label_note7.Size = new Size(37, 18);
            label_note7.TabIndex = 3;
            label_note7.Text = "C#4";
            label_note7.TextAlign = ContentAlignment.MiddleCenter;
            label_note7.Visible = false;
            // 
            // label_note6
            // 
            label_note6.Anchor = AnchorStyles.None;
            label_note6.AutoEllipsis = true;
            label_note6.AutoSize = true;
            label_note6.BackColor = Color.FromArgb(176, 0, 0);
            label_note6.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note6.Location = new Point(211, 10);
            label_note6.MaximumSize = new Size(37, 18);
            label_note6.MinimumSize = new Size(37, 18);
            label_note6.Name = "label_note6";
            label_note6.Size = new Size(37, 18);
            label_note6.TabIndex = 3;
            label_note6.Text = "C#4";
            label_note6.TextAlign = ContentAlignment.MiddleCenter;
            label_note6.Visible = false;
            // 
            // label_note8
            // 
            label_note8.Anchor = AnchorStyles.None;
            label_note8.AutoEllipsis = true;
            label_note8.AutoSize = true;
            label_note8.BackColor = Color.FromArgb(176, 0, 0);
            label_note8.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note8.Location = new Point(293, 10);
            label_note8.MaximumSize = new Size(37, 18);
            label_note8.MinimumSize = new Size(37, 18);
            label_note8.Name = "label_note8";
            label_note8.Size = new Size(37, 18);
            label_note8.TabIndex = 3;
            label_note8.Text = "C#4";
            label_note8.TextAlign = ContentAlignment.MiddleCenter;
            label_note8.Visible = false;
            // 
            // label_note9
            // 
            label_note9.Anchor = AnchorStyles.None;
            label_note9.AutoEllipsis = true;
            label_note9.AutoSize = true;
            label_note9.BackColor = Color.FromArgb(176, 0, 0);
            label_note9.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note9.Location = new Point(6, 35);
            label_note9.MaximumSize = new Size(37, 18);
            label_note9.MinimumSize = new Size(37, 18);
            label_note9.Name = "label_note9";
            label_note9.Size = new Size(37, 18);
            label_note9.TabIndex = 3;
            label_note9.Text = "C#4";
            label_note9.TextAlign = ContentAlignment.MiddleCenter;
            label_note9.Visible = false;
            // 
            // label_note13
            // 
            label_note13.Anchor = AnchorStyles.None;
            label_note13.AutoEllipsis = true;
            label_note13.AutoSize = true;
            label_note13.BackColor = Color.FromArgb(176, 0, 0);
            label_note13.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note13.Location = new Point(170, 35);
            label_note13.MaximumSize = new Size(37, 18);
            label_note13.MinimumSize = new Size(37, 18);
            label_note13.Name = "label_note13";
            label_note13.Size = new Size(37, 18);
            label_note13.TabIndex = 3;
            label_note13.Text = "C#4";
            label_note13.TextAlign = ContentAlignment.MiddleCenter;
            label_note13.Visible = false;
            // 
            // label_note11
            // 
            label_note11.Anchor = AnchorStyles.None;
            label_note11.AutoEllipsis = true;
            label_note11.AutoSize = true;
            label_note11.BackColor = Color.FromArgb(176, 0, 0);
            label_note11.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note11.Location = new Point(88, 35);
            label_note11.MaximumSize = new Size(37, 18);
            label_note11.MinimumSize = new Size(37, 18);
            label_note11.Name = "label_note11";
            label_note11.Size = new Size(37, 18);
            label_note11.TabIndex = 3;
            label_note11.Text = "C#4";
            label_note11.TextAlign = ContentAlignment.MiddleCenter;
            label_note11.Visible = false;
            // 
            // label_note15
            // 
            label_note15.Anchor = AnchorStyles.None;
            label_note15.AutoEllipsis = true;
            label_note15.AutoSize = true;
            label_note15.BackColor = Color.FromArgb(176, 0, 0);
            label_note15.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note15.Location = new Point(252, 35);
            label_note15.MaximumSize = new Size(37, 18);
            label_note15.MinimumSize = new Size(37, 18);
            label_note15.Name = "label_note15";
            label_note15.Size = new Size(37, 18);
            label_note15.TabIndex = 3;
            label_note15.Text = "C#4";
            label_note15.TextAlign = ContentAlignment.MiddleCenter;
            label_note15.Visible = false;
            // 
            // label_note10
            // 
            label_note10.Anchor = AnchorStyles.None;
            label_note10.AutoEllipsis = true;
            label_note10.AutoSize = true;
            label_note10.BackColor = Color.FromArgb(176, 0, 0);
            label_note10.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note10.Location = new Point(47, 35);
            label_note10.MaximumSize = new Size(37, 18);
            label_note10.MinimumSize = new Size(37, 18);
            label_note10.Name = "label_note10";
            label_note10.Size = new Size(37, 18);
            label_note10.TabIndex = 3;
            label_note10.Text = "C#4";
            label_note10.TextAlign = ContentAlignment.MiddleCenter;
            label_note10.Visible = false;
            // 
            // label_note14
            // 
            label_note14.Anchor = AnchorStyles.None;
            label_note14.AutoEllipsis = true;
            label_note14.AutoSize = true;
            label_note14.BackColor = Color.FromArgb(176, 0, 0);
            label_note14.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note14.Location = new Point(211, 35);
            label_note14.MaximumSize = new Size(37, 18);
            label_note14.MinimumSize = new Size(37, 18);
            label_note14.Name = "label_note14";
            label_note14.Size = new Size(37, 18);
            label_note14.TabIndex = 3;
            label_note14.Text = "C#4";
            label_note14.TextAlign = ContentAlignment.MiddleCenter;
            label_note14.Visible = false;
            // 
            // label_note12
            // 
            label_note12.Anchor = AnchorStyles.None;
            label_note12.AutoEllipsis = true;
            label_note12.AutoSize = true;
            label_note12.BackColor = Color.FromArgb(176, 0, 0);
            label_note12.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note12.Location = new Point(129, 35);
            label_note12.MaximumSize = new Size(37, 18);
            label_note12.MinimumSize = new Size(37, 18);
            label_note12.Name = "label_note12";
            label_note12.Size = new Size(37, 18);
            label_note12.TabIndex = 3;
            label_note12.Text = "C#4";
            label_note12.TextAlign = ContentAlignment.MiddleCenter;
            label_note12.Visible = false;
            // 
            // label_note16
            // 
            label_note16.Anchor = AnchorStyles.None;
            label_note16.AutoEllipsis = true;
            label_note16.AutoSize = true;
            label_note16.BackColor = Color.FromArgb(176, 0, 0);
            label_note16.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note16.Location = new Point(293, 35);
            label_note16.MaximumSize = new Size(37, 18);
            label_note16.MinimumSize = new Size(37, 18);
            label_note16.Name = "label_note16";
            label_note16.Size = new Size(37, 18);
            label_note16.TabIndex = 3;
            label_note16.Text = "C#4";
            label_note16.TextAlign = ContentAlignment.MiddleCenter;
            label_note16.Visible = false;
            // 
            // label_note17
            // 
            label_note17.Anchor = AnchorStyles.None;
            label_note17.AutoEllipsis = true;
            label_note17.AutoSize = true;
            label_note17.BackColor = Color.FromArgb(176, 0, 0);
            label_note17.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note17.Location = new Point(6, 60);
            label_note17.MaximumSize = new Size(37, 18);
            label_note17.MinimumSize = new Size(37, 18);
            label_note17.Name = "label_note17";
            label_note17.Size = new Size(37, 18);
            label_note17.TabIndex = 3;
            label_note17.Text = "C#4";
            label_note17.TextAlign = ContentAlignment.MiddleCenter;
            label_note17.Visible = false;
            // 
            // label_note21
            // 
            label_note21.Anchor = AnchorStyles.None;
            label_note21.AutoEllipsis = true;
            label_note21.AutoSize = true;
            label_note21.BackColor = Color.FromArgb(176, 0, 0);
            label_note21.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note21.Location = new Point(170, 60);
            label_note21.MaximumSize = new Size(37, 18);
            label_note21.MinimumSize = new Size(37, 18);
            label_note21.Name = "label_note21";
            label_note21.Size = new Size(37, 18);
            label_note21.TabIndex = 3;
            label_note21.Text = "C#4";
            label_note21.TextAlign = ContentAlignment.MiddleCenter;
            label_note21.Visible = false;
            // 
            // label_note19
            // 
            label_note19.Anchor = AnchorStyles.None;
            label_note19.AutoEllipsis = true;
            label_note19.AutoSize = true;
            label_note19.BackColor = Color.FromArgb(176, 0, 0);
            label_note19.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note19.Location = new Point(88, 60);
            label_note19.MaximumSize = new Size(37, 18);
            label_note19.MinimumSize = new Size(37, 18);
            label_note19.Name = "label_note19";
            label_note19.Size = new Size(37, 18);
            label_note19.TabIndex = 3;
            label_note19.Text = "C#4";
            label_note19.TextAlign = ContentAlignment.MiddleCenter;
            label_note19.Visible = false;
            // 
            // label_note23
            // 
            label_note23.Anchor = AnchorStyles.None;
            label_note23.AutoEllipsis = true;
            label_note23.AutoSize = true;
            label_note23.BackColor = Color.FromArgb(176, 0, 0);
            label_note23.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note23.Location = new Point(252, 60);
            label_note23.MaximumSize = new Size(37, 18);
            label_note23.MinimumSize = new Size(37, 18);
            label_note23.Name = "label_note23";
            label_note23.Size = new Size(37, 18);
            label_note23.TabIndex = 3;
            label_note23.Text = "C#4";
            label_note23.TextAlign = ContentAlignment.MiddleCenter;
            label_note23.Visible = false;
            // 
            // label_note18
            // 
            label_note18.Anchor = AnchorStyles.None;
            label_note18.AutoEllipsis = true;
            label_note18.AutoSize = true;
            label_note18.BackColor = Color.FromArgb(176, 0, 0);
            label_note18.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note18.Location = new Point(47, 60);
            label_note18.MaximumSize = new Size(37, 18);
            label_note18.MinimumSize = new Size(37, 18);
            label_note18.Name = "label_note18";
            label_note18.Size = new Size(37, 18);
            label_note18.TabIndex = 3;
            label_note18.Text = "C#4";
            label_note18.TextAlign = ContentAlignment.MiddleCenter;
            label_note18.Visible = false;
            // 
            // label_note22
            // 
            label_note22.Anchor = AnchorStyles.None;
            label_note22.AutoEllipsis = true;
            label_note22.AutoSize = true;
            label_note22.BackColor = Color.FromArgb(176, 0, 0);
            label_note22.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note22.Location = new Point(211, 60);
            label_note22.MaximumSize = new Size(37, 18);
            label_note22.MinimumSize = new Size(37, 18);
            label_note22.Name = "label_note22";
            label_note22.Size = new Size(37, 18);
            label_note22.TabIndex = 3;
            label_note22.Text = "C#4";
            label_note22.TextAlign = ContentAlignment.MiddleCenter;
            label_note22.Visible = false;
            // 
            // label_note20
            // 
            label_note20.Anchor = AnchorStyles.None;
            label_note20.AutoEllipsis = true;
            label_note20.AutoSize = true;
            label_note20.BackColor = Color.FromArgb(176, 0, 0);
            label_note20.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note20.Location = new Point(129, 60);
            label_note20.MaximumSize = new Size(37, 18);
            label_note20.MinimumSize = new Size(37, 18);
            label_note20.Name = "label_note20";
            label_note20.Size = new Size(37, 18);
            label_note20.TabIndex = 3;
            label_note20.Text = "C#4";
            label_note20.TextAlign = ContentAlignment.MiddleCenter;
            label_note20.Visible = false;
            // 
            // label_note24
            // 
            label_note24.Anchor = AnchorStyles.None;
            label_note24.AutoEllipsis = true;
            label_note24.AutoSize = true;
            label_note24.BackColor = Color.FromArgb(176, 0, 0);
            label_note24.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note24.Location = new Point(293, 60);
            label_note24.MaximumSize = new Size(37, 18);
            label_note24.MinimumSize = new Size(37, 18);
            label_note24.Name = "label_note24";
            label_note24.Size = new Size(37, 18);
            label_note24.TabIndex = 3;
            label_note24.Text = "C#4";
            label_note24.TextAlign = ContentAlignment.MiddleCenter;
            label_note24.Visible = false;
            // 
            // label_note25
            // 
            label_note25.Anchor = AnchorStyles.None;
            label_note25.AutoEllipsis = true;
            label_note25.AutoSize = true;
            label_note25.BackColor = Color.FromArgb(176, 0, 0);
            label_note25.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note25.Location = new Point(6, 85);
            label_note25.MaximumSize = new Size(37, 18);
            label_note25.MinimumSize = new Size(37, 18);
            label_note25.Name = "label_note25";
            label_note25.Size = new Size(37, 18);
            label_note25.TabIndex = 3;
            label_note25.Text = "C#4";
            label_note25.TextAlign = ContentAlignment.MiddleCenter;
            label_note25.Visible = false;
            // 
            // label_note29
            // 
            label_note29.Anchor = AnchorStyles.None;
            label_note29.AutoEllipsis = true;
            label_note29.AutoSize = true;
            label_note29.BackColor = Color.FromArgb(176, 0, 0);
            label_note29.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note29.Location = new Point(170, 85);
            label_note29.MaximumSize = new Size(37, 18);
            label_note29.MinimumSize = new Size(37, 18);
            label_note29.Name = "label_note29";
            label_note29.Size = new Size(37, 18);
            label_note29.TabIndex = 3;
            label_note29.Text = "C#4";
            label_note29.TextAlign = ContentAlignment.MiddleCenter;
            label_note29.Visible = false;
            // 
            // label_note27
            // 
            label_note27.Anchor = AnchorStyles.None;
            label_note27.AutoEllipsis = true;
            label_note27.AutoSize = true;
            label_note27.BackColor = Color.FromArgb(176, 0, 0);
            label_note27.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note27.Location = new Point(88, 85);
            label_note27.MaximumSize = new Size(37, 18);
            label_note27.MinimumSize = new Size(37, 18);
            label_note27.Name = "label_note27";
            label_note27.Size = new Size(37, 18);
            label_note27.TabIndex = 3;
            label_note27.Text = "C#4";
            label_note27.TextAlign = ContentAlignment.MiddleCenter;
            label_note27.Visible = false;
            // 
            // label_note31
            // 
            label_note31.Anchor = AnchorStyles.None;
            label_note31.AutoEllipsis = true;
            label_note31.AutoSize = true;
            label_note31.BackColor = Color.FromArgb(176, 0, 0);
            label_note31.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note31.Location = new Point(252, 85);
            label_note31.MaximumSize = new Size(37, 18);
            label_note31.MinimumSize = new Size(37, 18);
            label_note31.Name = "label_note31";
            label_note31.Size = new Size(37, 18);
            label_note31.TabIndex = 3;
            label_note31.Text = "C#4";
            label_note31.TextAlign = ContentAlignment.MiddleCenter;
            label_note31.Visible = false;
            // 
            // label_note26
            // 
            label_note26.Anchor = AnchorStyles.None;
            label_note26.AutoEllipsis = true;
            label_note26.AutoSize = true;
            label_note26.BackColor = Color.FromArgb(176, 0, 0);
            label_note26.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note26.Location = new Point(47, 85);
            label_note26.MaximumSize = new Size(37, 18);
            label_note26.MinimumSize = new Size(37, 18);
            label_note26.Name = "label_note26";
            label_note26.Size = new Size(37, 18);
            label_note26.TabIndex = 3;
            label_note26.Text = "C#4";
            label_note26.TextAlign = ContentAlignment.MiddleCenter;
            label_note26.Visible = false;
            // 
            // label_note30
            // 
            label_note30.Anchor = AnchorStyles.None;
            label_note30.AutoEllipsis = true;
            label_note30.AutoSize = true;
            label_note30.BackColor = Color.FromArgb(176, 0, 0);
            label_note30.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note30.Location = new Point(211, 85);
            label_note30.MaximumSize = new Size(37, 18);
            label_note30.MinimumSize = new Size(37, 18);
            label_note30.Name = "label_note30";
            label_note30.Size = new Size(37, 18);
            label_note30.TabIndex = 3;
            label_note30.Text = "C#4";
            label_note30.TextAlign = ContentAlignment.MiddleCenter;
            label_note30.Visible = false;
            // 
            // label_note28
            // 
            label_note28.Anchor = AnchorStyles.None;
            label_note28.AutoEllipsis = true;
            label_note28.AutoSize = true;
            label_note28.BackColor = Color.FromArgb(176, 0, 0);
            label_note28.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note28.Location = new Point(129, 85);
            label_note28.MaximumSize = new Size(37, 18);
            label_note28.MinimumSize = new Size(37, 18);
            label_note28.Name = "label_note28";
            label_note28.Size = new Size(37, 18);
            label_note28.TabIndex = 3;
            label_note28.Text = "C#4";
            label_note28.TextAlign = ContentAlignment.MiddleCenter;
            label_note28.Visible = false;
            // 
            // label_note32
            // 
            label_note32.Anchor = AnchorStyles.None;
            label_note32.AutoEllipsis = true;
            label_note32.AutoSize = true;
            label_note32.BackColor = Color.FromArgb(176, 0, 0);
            label_note32.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note32.Location = new Point(293, 85);
            label_note32.MaximumSize = new Size(37, 18);
            label_note32.MinimumSize = new Size(37, 18);
            label_note32.Name = "label_note32";
            label_note32.Size = new Size(37, 18);
            label_note32.TabIndex = 3;
            label_note32.Text = "C#4";
            label_note32.TextAlign = ContentAlignment.MiddleCenter;
            label_note32.Visible = false;
            // 
            // button_browse_file
            // 
            button_browse_file.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button_browse_file.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_browse_file.ImageIndex = 0;
            button_browse_file.ImageList = icons2;
            button_browse_file.Location = new Point(232, 24);
            button_browse_file.Margin = new Padding(3, 2, 3, 2);
            button_browse_file.Name = "button_browse_file";
            button_browse_file.Size = new Size(108, 27);
            button_browse_file.TabIndex = 0;
            button_browse_file.Text = "Browse File";
            button_browse_file.TextAlign = ContentAlignment.MiddleRight;
            button_browse_file.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_browse_file.UseVisualStyleBackColor = true;
            button_browse_file.Click += button4_Click;
            // 
            // checkBox_play_each_note
            // 
            checkBox_play_each_note.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBox_play_each_note.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_play_each_note.Location = new Point(9, 502);
            checkBox_play_each_note.Margin = new Padding(3, 2, 3, 2);
            checkBox_play_each_note.MinimumSize = new Size(321, 20);
            checkBox_play_each_note.Name = "checkBox_play_each_note";
            checkBox_play_each_note.Size = new Size(321, 20);
            checkBox_play_each_note.TabIndex = 22;
            checkBox_play_each_note.Text = "Play each note once at a time (don't keep alternating)";
            checkBox_play_each_note.UseVisualStyleBackColor = true;
            checkBox_play_each_note.CheckedChanged += checkBox_play_each_note_CheckedChanged;
            // 
            // checkBox_make_each_cycle_last_30ms
            // 
            checkBox_make_each_cycle_last_30ms.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBox_make_each_cycle_last_30ms.Checked = true;
            checkBox_make_each_cycle_last_30ms.CheckState = CheckState.Checked;
            checkBox_make_each_cycle_last_30ms.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_make_each_cycle_last_30ms.Location = new Point(9, 525);
            checkBox_make_each_cycle_last_30ms.Margin = new Padding(3, 2, 3, 2);
            checkBox_make_each_cycle_last_30ms.MaximumSize = new Size(321, 42);
            checkBox_make_each_cycle_last_30ms.MinimumSize = new Size(321, 42);
            checkBox_make_each_cycle_last_30ms.Name = "checkBox_make_each_cycle_last_30ms";
            checkBox_make_each_cycle_last_30ms.Size = new Size(321, 42);
            checkBox_make_each_cycle_last_30ms.TabIndex = 23;
            checkBox_make_each_cycle_last_30ms.Text = "Try making each cycle last 30mS (with maximium alternating time capped to 15mS per note)";
            checkBox_make_each_cycle_last_30ms.UseVisualStyleBackColor = true;
            checkBox_make_each_cycle_last_30ms.CheckedChanged += disable_alternating_notes_panel;
            // 
            // checkBox_dont_update_grid
            // 
            checkBox_dont_update_grid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkBox_dont_update_grid.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_dont_update_grid.Location = new Point(9, 571);
            checkBox_dont_update_grid.Margin = new Padding(3, 2, 3, 2);
            checkBox_dont_update_grid.MaximumSize = new Size(321, 20);
            checkBox_dont_update_grid.MinimumSize = new Size(159, 20);
            checkBox_dont_update_grid.Name = "checkBox_dont_update_grid";
            checkBox_dont_update_grid.Size = new Size(159, 20);
            checkBox_dont_update_grid.TabIndex = 24;
            checkBox_dont_update_grid.Text = "Don't update grid above";
            checkBox_dont_update_grid.UseVisualStyleBackColor = true;
            checkBox_dont_update_grid.CheckedChanged += checkBox_dont_update_grid_CheckedChanged;
            // 
            // playbackTimer
            // 
            playbackTimer.Interval = 1;
            playbackTimer.Tick += playbackTimer_Tick;
            // 
            // label_alternating_note
            // 
            label_alternating_note.Anchor = AnchorStyles.Left;
            label_alternating_note.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label_alternating_note.Location = new Point(1, 3);
            label_alternating_note.Name = "label_alternating_note";
            label_alternating_note.Size = new Size(231, 16);
            label_alternating_note.TabIndex = 25;
            label_alternating_note.Text = "Switch between alternating notes every:";
            // 
            // numericUpDown_alternating_note
            // 
            numericUpDown_alternating_note.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown_alternating_note.AutoSize = true;
            numericUpDown_alternating_note.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numericUpDown_alternating_note.Location = new Point(233, 0);
            numericUpDown_alternating_note.Margin = new Padding(3, 2, 3, 2);
            numericUpDown_alternating_note.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            numericUpDown_alternating_note.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numericUpDown_alternating_note.Name = "numericUpDown_alternating_note";
            numericUpDown_alternating_note.Size = new Size(56, 23);
            numericUpDown_alternating_note.TabIndex = 26;
            numericUpDown_alternating_note.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numericUpDown_alternating_note.ValueChanged += numericUpDown_alternating_note_ValueChanged;
            // 
            // label_ms
            // 
            label_ms.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label_ms.AutoSize = true;
            label_ms.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label_ms.Location = new Point(293, 3);
            label_ms.Name = "label_ms";
            label_ms.Size = new Size(24, 16);
            label_ms.TabIndex = 25;
            label_ms.Text = "mS";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panel1.AutoSize = true;
            panel1.Controls.Add(label_alternating_note);
            panel1.Controls.Add(label_ms);
            panel1.Controls.Add(numericUpDown_alternating_note);
            panel1.Enabled = false;
            panel1.Location = new Point(9, 473);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(322, 25);
            panel1.TabIndex = 27;
            // 
            // label_more_notes
            // 
            label_more_notes.Anchor = AnchorStyles.Right;
            label_more_notes.AutoSize = true;
            label_more_notes.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_more_notes.Location = new Point(295, 116);
            label_more_notes.Margin = new Padding(2, 0, 2, 0);
            label_more_notes.Name = "label_more_notes";
            label_more_notes.Size = new Size(47, 16);
            label_more_notes.TabIndex = 28;
            label_more_notes.Text = "(More)";
            label_more_notes.Visible = false;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Right;
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
            panel2.Location = new Point(3, 2);
            panel2.Margin = new Padding(2);
            panel2.Name = "panel2";
            panel2.Size = new Size(339, 112);
            panel2.TabIndex = 30;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(panel2);
            flowLayoutPanel2.Controls.Add(label_more_notes);
            flowLayoutPanel2.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel2.Location = new Point(2, 329);
            flowLayoutPanel2.Margin = new Padding(2);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(344, 140);
            flowLayoutPanel2.TabIndex = 31;
            // 
            // panelLoading
            // 
            panelLoading.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelLoading.Controls.Add(progressBar1);
            panelLoading.Controls.Add(pictureBoxIcon);
            panelLoading.Controls.Add(labelStatus);
            panelLoading.Location = new Point(9, 56);
            panelLoading.Name = "panelLoading";
            panelLoading.Size = new Size(329, 107);
            panelLoading.TabIndex = 29;
            panelLoading.Visible = false;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.None;
            progressBar1.Location = new Point(14, 60);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(303, 23);
            progressBar1.TabIndex = 5;
            // 
            // pictureBoxIcon
            // 
            pictureBoxIcon.Anchor = AnchorStyles.Top;
            pictureBoxIcon.Image = Properties.Resources.icons8_wait_96;
            pictureBoxIcon.Location = new Point(14, 9);
            pictureBoxIcon.Margin = new Padding(2);
            pictureBoxIcon.Name = "pictureBoxIcon";
            pictureBoxIcon.Size = new Size(38, 38);
            pictureBoxIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxIcon.TabIndex = 4;
            pictureBoxIcon.TabStop = false;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Top;
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelStatus.Location = new Point(67, 20);
            labelStatus.Margin = new Padding(2, 0, 2, 0);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(177, 16);
            labelStatus.TabIndex = 3;
            labelStatus.Text = "Loading MIDI file. Please wait...";
            labelStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MIDI_file_player
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(348, 598);
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
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MIDI_file_player";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Play MIDI File";
            FormClosing += MIDI_file_player_FormClosing;
            Load += MIDI_file_player_Load;
            DragDrop += MIDI_file_player_DragDrop;
            DragEnter += MIDI_file_player_DragEnter;
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
            panelLoading.PerformLayout();
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
    }
}