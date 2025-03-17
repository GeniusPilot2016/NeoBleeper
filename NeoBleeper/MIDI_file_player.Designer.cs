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
            label_percentage = new Label();
            label_position = new Label();
            checkBox_loop = new CheckBox();
            icons2 = new ImageList(components);
            button_stop = new Button();
            icons = new ImageList(components);
            button_play = new Button();
            button_rewind = new Button();
            trackBar1 = new TrackBar();
            label4 = new Label();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            checkBox4 = new CheckBox();
            checkBox5 = new CheckBox();
            checkBox6 = new CheckBox();
            checkBox7 = new CheckBox();
            checkBox8 = new CheckBox();
            checkBox9 = new CheckBox();
            checkBox10 = new CheckBox();
            checkBox11 = new CheckBox();
            checkBox12 = new CheckBox();
            checkBox13 = new CheckBox();
            checkBox14 = new CheckBox();
            checkBox15 = new CheckBox();
            checkBox16 = new CheckBox();
            checkBox17 = new CheckBox();
            label5 = new Label();
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
            checkBox18 = new CheckBox();
            checkBox19 = new CheckBox();
            checkBox20 = new CheckBox();
            openFileDialog = new OpenFileDialog();
            toolTip1 = new ToolTip(components);
            update_timer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = SystemColors.Window;
            textBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(10, 35);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(264, 27);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(11, 9);
            label1.Name = "label1";
            label1.Size = new Size(195, 20);
            label1.TabIndex = 1;
            label1.Text = "Currently playing MIDI file:";
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(label_percentage);
            groupBox1.Controls.Add(label_position);
            groupBox1.Controls.Add(checkBox_loop);
            groupBox1.Controls.Add(button_stop);
            groupBox1.Controls.Add(button_play);
            groupBox1.Controls.Add(button_rewind);
            groupBox1.Controls.Add(trackBar1);
            groupBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(11, 67);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(392, 109);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Playback Controls";
            // 
            // label_percentage
            // 
            label_percentage.Anchor = AnchorStyles.Right;
            label_percentage.AutoSize = true;
            label_percentage.Location = new Point(334, 75);
            label_percentage.Name = "label_percentage";
            label_percentage.RightToLeft = RightToLeft.Yes;
            label_percentage.Size = new Size(52, 20);
            label_percentage.TabIndex = 3;
            label_percentage.Text = "0,00%";
            // 
            // label_position
            // 
            label_position.Anchor = AnchorStyles.Right;
            label_position.AutoSize = true;
            label_position.Location = new Point(251, 55);
            label_position.Name = "label_position";
            label_position.Size = new Size(135, 20);
            label_position.TabIndex = 3;
            label_position.Text = "Position: 00:00.00";
            // 
            // checkBox_loop
            // 
            checkBox_loop.Anchor = AnchorStyles.Left;
            checkBox_loop.AutoSize = true;
            checkBox_loop.ImageIndex = 1;
            checkBox_loop.ImageList = icons2;
            checkBox_loop.Location = new Point(123, 60);
            checkBox_loop.Name = "checkBox_loop";
            checkBox_loop.Size = new Size(82, 24);
            checkBox_loop.TabIndex = 5;
            checkBox_loop.Text = "Loop";
            checkBox_loop.TextImageRelation = TextImageRelation.ImageBeforeText;
            checkBox_loop.UseVisualStyleBackColor = true;
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
            button_stop.Location = new Point(82, 57);
            button_stop.Name = "button_stop";
            button_stop.Size = new Size(27, 27);
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
            button_play.Location = new Point(49, 57);
            button_play.Name = "button_play";
            button_play.Size = new Size(27, 27);
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
            button_rewind.Location = new Point(16, 57);
            button_rewind.Name = "button_rewind";
            button_rewind.Size = new Size(27, 27);
            button_rewind.TabIndex = 2;
            toolTip1.SetToolTip(button_rewind, "Rewind");
            button_rewind.UseVisualStyleBackColor = true;
            button_rewind.Click += button_rewind_Click;
            // 
            // trackBar1
            // 
            trackBar1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            trackBar1.Location = new Point(6, 26);
            trackBar1.Maximum = 1000;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(380, 56);
            trackBar1.TabIndex = 1;
            trackBar1.TickStyle = TickStyle.None;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(11, 191);
            label4.Name = "label4";
            label4.Size = new Size(343, 40);
            label4.TabIndex = 3;
            label4.Text = "Select the channel(s) that should be listened to. \r\nChannels that are not selected will be ignored.";
            // 
            // checkBox2
            // 
            checkBox2.Anchor = AnchorStyles.Left;
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox2.Location = new Point(14, 246);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(40, 24);
            checkBox2.TabIndex = 6;
            checkBox2.Text = "1";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.Anchor = AnchorStyles.Left;
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox3.Location = new Point(68, 246);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(40, 24);
            checkBox3.TabIndex = 7;
            checkBox3.Text = "2";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            checkBox4.Anchor = AnchorStyles.Left;
            checkBox4.AutoSize = true;
            checkBox4.Checked = true;
            checkBox4.CheckState = CheckState.Checked;
            checkBox4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox4.Location = new Point(125, 246);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(40, 24);
            checkBox4.TabIndex = 8;
            checkBox4.Text = "3";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            checkBox5.Anchor = AnchorStyles.Left;
            checkBox5.AutoSize = true;
            checkBox5.Checked = true;
            checkBox5.CheckState = CheckState.Checked;
            checkBox5.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox5.Location = new Point(181, 246);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(40, 24);
            checkBox5.TabIndex = 9;
            checkBox5.Text = "4";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.Anchor = AnchorStyles.Left;
            checkBox6.AutoSize = true;
            checkBox6.Checked = true;
            checkBox6.CheckState = CheckState.Checked;
            checkBox6.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox6.Location = new Point(14, 277);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(40, 24);
            checkBox6.TabIndex = 10;
            checkBox6.Text = "5";
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            checkBox7.Anchor = AnchorStyles.Left;
            checkBox7.AutoSize = true;
            checkBox7.Checked = true;
            checkBox7.CheckState = CheckState.Checked;
            checkBox7.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox7.Location = new Point(125, 277);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(40, 24);
            checkBox7.TabIndex = 12;
            checkBox7.Text = "7";
            checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            checkBox8.Anchor = AnchorStyles.Left;
            checkBox8.AutoSize = true;
            checkBox8.Checked = true;
            checkBox8.CheckState = CheckState.Checked;
            checkBox8.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox8.Location = new Point(68, 277);
            checkBox8.Name = "checkBox8";
            checkBox8.Size = new Size(40, 24);
            checkBox8.TabIndex = 11;
            checkBox8.Text = "6";
            checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            checkBox9.Anchor = AnchorStyles.Left;
            checkBox9.AutoSize = true;
            checkBox9.Checked = true;
            checkBox9.CheckState = CheckState.Checked;
            checkBox9.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox9.Location = new Point(181, 277);
            checkBox9.Name = "checkBox9";
            checkBox9.Size = new Size(40, 24);
            checkBox9.TabIndex = 13;
            checkBox9.Text = "8";
            checkBox9.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            checkBox10.Anchor = AnchorStyles.Left;
            checkBox10.AutoSize = true;
            checkBox10.Checked = true;
            checkBox10.CheckState = CheckState.Checked;
            checkBox10.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox10.Location = new Point(14, 306);
            checkBox10.Name = "checkBox10";
            checkBox10.Size = new Size(40, 24);
            checkBox10.TabIndex = 14;
            checkBox10.Text = "9";
            checkBox10.UseVisualStyleBackColor = true;
            // 
            // checkBox11
            // 
            checkBox11.Anchor = AnchorStyles.Left;
            checkBox11.AutoSize = true;
            checkBox11.Checked = true;
            checkBox11.CheckState = CheckState.Checked;
            checkBox11.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox11.Location = new Point(125, 306);
            checkBox11.Name = "checkBox11";
            checkBox11.Size = new Size(49, 24);
            checkBox11.TabIndex = 16;
            checkBox11.Text = "11";
            checkBox11.UseVisualStyleBackColor = true;
            // 
            // checkBox12
            // 
            checkBox12.Anchor = AnchorStyles.Left;
            checkBox12.AutoSize = true;
            checkBox12.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox12.Location = new Point(68, 306);
            checkBox12.Name = "checkBox12";
            checkBox12.Size = new Size(49, 24);
            checkBox12.TabIndex = 15;
            checkBox12.Text = "10";
            checkBox12.UseVisualStyleBackColor = true;
            // 
            // checkBox13
            // 
            checkBox13.Anchor = AnchorStyles.Left;
            checkBox13.AutoSize = true;
            checkBox13.Checked = true;
            checkBox13.CheckState = CheckState.Checked;
            checkBox13.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox13.Location = new Point(181, 306);
            checkBox13.Name = "checkBox13";
            checkBox13.Size = new Size(49, 24);
            checkBox13.TabIndex = 17;
            checkBox13.Text = "12";
            checkBox13.UseVisualStyleBackColor = true;
            // 
            // checkBox14
            // 
            checkBox14.Anchor = AnchorStyles.Left;
            checkBox14.AutoSize = true;
            checkBox14.Checked = true;
            checkBox14.CheckState = CheckState.Checked;
            checkBox14.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox14.Location = new Point(14, 335);
            checkBox14.Name = "checkBox14";
            checkBox14.Size = new Size(49, 24);
            checkBox14.TabIndex = 18;
            checkBox14.Text = "13";
            checkBox14.UseVisualStyleBackColor = true;
            // 
            // checkBox15
            // 
            checkBox15.Anchor = AnchorStyles.Left;
            checkBox15.AutoSize = true;
            checkBox15.Checked = true;
            checkBox15.CheckState = CheckState.Checked;
            checkBox15.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox15.Location = new Point(125, 335);
            checkBox15.Name = "checkBox15";
            checkBox15.Size = new Size(49, 24);
            checkBox15.TabIndex = 20;
            checkBox15.Text = "15";
            checkBox15.UseVisualStyleBackColor = true;
            // 
            // checkBox16
            // 
            checkBox16.Anchor = AnchorStyles.Left;
            checkBox16.AutoSize = true;
            checkBox16.Checked = true;
            checkBox16.CheckState = CheckState.Checked;
            checkBox16.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox16.Location = new Point(68, 335);
            checkBox16.Name = "checkBox16";
            checkBox16.Size = new Size(49, 24);
            checkBox16.TabIndex = 19;
            checkBox16.Text = "14";
            checkBox16.UseVisualStyleBackColor = true;
            // 
            // checkBox17
            // 
            checkBox17.Anchor = AnchorStyles.Left;
            checkBox17.AutoSize = true;
            checkBox17.Checked = true;
            checkBox17.CheckState = CheckState.Checked;
            checkBox17.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox17.Location = new Point(181, 335);
            checkBox17.Name = "checkBox17";
            checkBox17.Size = new Size(49, 24);
            checkBox17.TabIndex = 21;
            checkBox17.Text = "16";
            checkBox17.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(11, 371);
            label5.Name = "label5";
            label5.Size = new Size(314, 20);
            label5.TabIndex = 3;
            label5.Text = "Notes which are currently being held on: (0)";
            // 
            // label_note1
            // 
            label_note1.Anchor = AnchorStyles.None;
            label_note1.AutoSize = true;
            label_note1.BackColor = Color.Red;
            label_note1.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note1.Location = new Point(12, 401);
            label_note1.Name = "label_note1";
            label_note1.Size = new Size(44, 23);
            label_note1.TabIndex = 3;
            label_note1.Text = "C#4";
            label_note1.Visible = false;
            // 
            // label_note2
            // 
            label_note2.Anchor = AnchorStyles.None;
            label_note2.AutoSize = true;
            label_note2.BackColor = Color.FromArgb(192, 0, 0);
            label_note2.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note2.Location = new Point(61, 401);
            label_note2.Name = "label_note2";
            label_note2.Size = new Size(44, 23);
            label_note2.TabIndex = 3;
            label_note2.Text = "C#4";
            label_note2.Visible = false;
            // 
            // label_note3
            // 
            label_note3.Anchor = AnchorStyles.None;
            label_note3.AutoSize = true;
            label_note3.BackColor = Color.FromArgb(192, 0, 0);
            label_note3.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note3.Location = new Point(110, 401);
            label_note3.Name = "label_note3";
            label_note3.Size = new Size(44, 23);
            label_note3.TabIndex = 3;
            label_note3.Text = "C#4";
            label_note3.Visible = false;
            // 
            // label_note4
            // 
            label_note4.Anchor = AnchorStyles.None;
            label_note4.AutoSize = true;
            label_note4.BackColor = Color.FromArgb(192, 0, 0);
            label_note4.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note4.Location = new Point(159, 401);
            label_note4.Name = "label_note4";
            label_note4.Size = new Size(44, 23);
            label_note4.TabIndex = 3;
            label_note4.Text = "C#4";
            label_note4.Visible = false;
            // 
            // label_note5
            // 
            label_note5.Anchor = AnchorStyles.None;
            label_note5.AutoSize = true;
            label_note5.BackColor = Color.FromArgb(192, 0, 0);
            label_note5.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note5.Location = new Point(208, 401);
            label_note5.Name = "label_note5";
            label_note5.Size = new Size(44, 23);
            label_note5.TabIndex = 3;
            label_note5.Text = "C#4";
            label_note5.Visible = false;
            // 
            // label_note7
            // 
            label_note7.Anchor = AnchorStyles.None;
            label_note7.AutoSize = true;
            label_note7.BackColor = Color.FromArgb(192, 0, 0);
            label_note7.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note7.Location = new Point(307, 401);
            label_note7.Name = "label_note7";
            label_note7.Size = new Size(44, 23);
            label_note7.TabIndex = 3;
            label_note7.Text = "C#4";
            label_note7.Visible = false;
            // 
            // label_note6
            // 
            label_note6.Anchor = AnchorStyles.None;
            label_note6.AutoSize = true;
            label_note6.BackColor = Color.FromArgb(192, 0, 0);
            label_note6.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note6.Location = new Point(257, 401);
            label_note6.Name = "label_note6";
            label_note6.Size = new Size(44, 23);
            label_note6.TabIndex = 3;
            label_note6.Text = "C#4";
            label_note6.Visible = false;
            // 
            // label_note8
            // 
            label_note8.Anchor = AnchorStyles.None;
            label_note8.AutoSize = true;
            label_note8.BackColor = Color.FromArgb(192, 0, 0);
            label_note8.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note8.Location = new Point(356, 401);
            label_note8.Name = "label_note8";
            label_note8.Size = new Size(44, 23);
            label_note8.TabIndex = 3;
            label_note8.Text = "C#4";
            label_note8.Visible = false;
            // 
            // label_note9
            // 
            label_note9.Anchor = AnchorStyles.None;
            label_note9.AutoSize = true;
            label_note9.BackColor = Color.FromArgb(192, 0, 0);
            label_note9.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note9.Location = new Point(12, 436);
            label_note9.Name = "label_note9";
            label_note9.Size = new Size(44, 23);
            label_note9.TabIndex = 3;
            label_note9.Text = "C#4";
            label_note9.Visible = false;
            // 
            // label_note13
            // 
            label_note13.Anchor = AnchorStyles.None;
            label_note13.AutoSize = true;
            label_note13.BackColor = Color.FromArgb(192, 0, 0);
            label_note13.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note13.Location = new Point(208, 436);
            label_note13.Name = "label_note13";
            label_note13.Size = new Size(44, 23);
            label_note13.TabIndex = 3;
            label_note13.Text = "C#4";
            label_note13.Visible = false;
            // 
            // label_note11
            // 
            label_note11.Anchor = AnchorStyles.None;
            label_note11.AutoSize = true;
            label_note11.BackColor = Color.FromArgb(192, 0, 0);
            label_note11.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note11.Location = new Point(110, 436);
            label_note11.Name = "label_note11";
            label_note11.Size = new Size(44, 23);
            label_note11.TabIndex = 3;
            label_note11.Text = "C#4";
            label_note11.Visible = false;
            // 
            // label_note15
            // 
            label_note15.Anchor = AnchorStyles.None;
            label_note15.AutoSize = true;
            label_note15.BackColor = Color.FromArgb(192, 0, 0);
            label_note15.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note15.Location = new Point(307, 436);
            label_note15.Name = "label_note15";
            label_note15.Size = new Size(44, 23);
            label_note15.TabIndex = 3;
            label_note15.Text = "C#4";
            label_note15.Visible = false;
            // 
            // label_note10
            // 
            label_note10.Anchor = AnchorStyles.None;
            label_note10.AutoSize = true;
            label_note10.BackColor = Color.FromArgb(192, 0, 0);
            label_note10.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note10.Location = new Point(61, 436);
            label_note10.Name = "label_note10";
            label_note10.Size = new Size(44, 23);
            label_note10.TabIndex = 3;
            label_note10.Text = "C#4";
            label_note10.Visible = false;
            // 
            // label_note14
            // 
            label_note14.Anchor = AnchorStyles.None;
            label_note14.AutoSize = true;
            label_note14.BackColor = Color.FromArgb(192, 0, 0);
            label_note14.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note14.Location = new Point(257, 436);
            label_note14.Name = "label_note14";
            label_note14.Size = new Size(44, 23);
            label_note14.TabIndex = 3;
            label_note14.Text = "C#4";
            label_note14.Visible = false;
            // 
            // label_note12
            // 
            label_note12.Anchor = AnchorStyles.None;
            label_note12.AutoSize = true;
            label_note12.BackColor = Color.FromArgb(192, 0, 0);
            label_note12.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note12.Location = new Point(159, 436);
            label_note12.Name = "label_note12";
            label_note12.Size = new Size(44, 23);
            label_note12.TabIndex = 3;
            label_note12.Text = "C#4";
            label_note12.Visible = false;
            // 
            // label_note16
            // 
            label_note16.Anchor = AnchorStyles.None;
            label_note16.AutoSize = true;
            label_note16.BackColor = Color.FromArgb(192, 0, 0);
            label_note16.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note16.Location = new Point(356, 436);
            label_note16.Name = "label_note16";
            label_note16.Size = new Size(44, 23);
            label_note16.TabIndex = 3;
            label_note16.Text = "C#4";
            label_note16.Visible = false;
            // 
            // label_note17
            // 
            label_note17.Anchor = AnchorStyles.None;
            label_note17.AutoSize = true;
            label_note17.BackColor = Color.FromArgb(192, 0, 0);
            label_note17.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note17.Location = new Point(12, 469);
            label_note17.Name = "label_note17";
            label_note17.Size = new Size(44, 23);
            label_note17.TabIndex = 3;
            label_note17.Text = "C#4";
            label_note17.Visible = false;
            // 
            // label_note21
            // 
            label_note21.Anchor = AnchorStyles.None;
            label_note21.AutoSize = true;
            label_note21.BackColor = Color.FromArgb(192, 0, 0);
            label_note21.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note21.Location = new Point(208, 469);
            label_note21.Name = "label_note21";
            label_note21.Size = new Size(44, 23);
            label_note21.TabIndex = 3;
            label_note21.Text = "C#4";
            label_note21.Visible = false;
            // 
            // label_note19
            // 
            label_note19.Anchor = AnchorStyles.None;
            label_note19.AutoSize = true;
            label_note19.BackColor = Color.FromArgb(192, 0, 0);
            label_note19.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note19.Location = new Point(110, 469);
            label_note19.Name = "label_note19";
            label_note19.Size = new Size(44, 23);
            label_note19.TabIndex = 3;
            label_note19.Text = "C#4";
            label_note19.Visible = false;
            // 
            // label_note23
            // 
            label_note23.Anchor = AnchorStyles.None;
            label_note23.AutoSize = true;
            label_note23.BackColor = Color.FromArgb(192, 0, 0);
            label_note23.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note23.Location = new Point(307, 469);
            label_note23.Name = "label_note23";
            label_note23.Size = new Size(44, 23);
            label_note23.TabIndex = 3;
            label_note23.Text = "C#4";
            label_note23.Visible = false;
            // 
            // label_note18
            // 
            label_note18.Anchor = AnchorStyles.None;
            label_note18.AutoSize = true;
            label_note18.BackColor = Color.FromArgb(192, 0, 0);
            label_note18.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note18.Location = new Point(61, 469);
            label_note18.Name = "label_note18";
            label_note18.Size = new Size(44, 23);
            label_note18.TabIndex = 3;
            label_note18.Text = "C#4";
            label_note18.Visible = false;
            // 
            // label_note22
            // 
            label_note22.Anchor = AnchorStyles.None;
            label_note22.AutoSize = true;
            label_note22.BackColor = Color.FromArgb(192, 0, 0);
            label_note22.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note22.Location = new Point(257, 469);
            label_note22.Name = "label_note22";
            label_note22.Size = new Size(44, 23);
            label_note22.TabIndex = 3;
            label_note22.Text = "C#4";
            label_note22.Visible = false;
            // 
            // label_note20
            // 
            label_note20.Anchor = AnchorStyles.None;
            label_note20.AutoSize = true;
            label_note20.BackColor = Color.FromArgb(192, 0, 0);
            label_note20.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note20.Location = new Point(159, 469);
            label_note20.Name = "label_note20";
            label_note20.Size = new Size(44, 23);
            label_note20.TabIndex = 3;
            label_note20.Text = "C#4";
            label_note20.Visible = false;
            // 
            // label_note24
            // 
            label_note24.Anchor = AnchorStyles.None;
            label_note24.AutoSize = true;
            label_note24.BackColor = Color.FromArgb(192, 0, 0);
            label_note24.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note24.Location = new Point(356, 469);
            label_note24.Name = "label_note24";
            label_note24.Size = new Size(44, 23);
            label_note24.TabIndex = 3;
            label_note24.Text = "C#4";
            label_note24.Visible = false;
            // 
            // label_note25
            // 
            label_note25.Anchor = AnchorStyles.None;
            label_note25.AutoSize = true;
            label_note25.BackColor = Color.FromArgb(192, 0, 0);
            label_note25.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note25.Location = new Point(12, 503);
            label_note25.Name = "label_note25";
            label_note25.Size = new Size(44, 23);
            label_note25.TabIndex = 3;
            label_note25.Text = "C#4";
            label_note25.Visible = false;
            // 
            // label_note29
            // 
            label_note29.Anchor = AnchorStyles.None;
            label_note29.AutoSize = true;
            label_note29.BackColor = Color.FromArgb(192, 0, 0);
            label_note29.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note29.Location = new Point(208, 503);
            label_note29.Name = "label_note29";
            label_note29.Size = new Size(44, 23);
            label_note29.TabIndex = 3;
            label_note29.Text = "C#4";
            label_note29.Visible = false;
            // 
            // label_note27
            // 
            label_note27.Anchor = AnchorStyles.None;
            label_note27.AutoSize = true;
            label_note27.BackColor = Color.FromArgb(192, 0, 0);
            label_note27.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note27.Location = new Point(110, 503);
            label_note27.Name = "label_note27";
            label_note27.Size = new Size(44, 23);
            label_note27.TabIndex = 3;
            label_note27.Text = "C#4";
            label_note27.Visible = false;
            // 
            // label_note31
            // 
            label_note31.Anchor = AnchorStyles.None;
            label_note31.AutoSize = true;
            label_note31.BackColor = Color.FromArgb(192, 0, 0);
            label_note31.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note31.Location = new Point(307, 503);
            label_note31.Name = "label_note31";
            label_note31.Size = new Size(44, 23);
            label_note31.TabIndex = 3;
            label_note31.Text = "C#4";
            label_note31.Visible = false;
            // 
            // label_note26
            // 
            label_note26.Anchor = AnchorStyles.None;
            label_note26.AutoSize = true;
            label_note26.BackColor = Color.FromArgb(192, 0, 0);
            label_note26.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note26.Location = new Point(61, 503);
            label_note26.Name = "label_note26";
            label_note26.Size = new Size(44, 23);
            label_note26.TabIndex = 3;
            label_note26.Text = "C#4";
            label_note26.Visible = false;
            // 
            // label_note30
            // 
            label_note30.Anchor = AnchorStyles.None;
            label_note30.AutoSize = true;
            label_note30.BackColor = Color.FromArgb(192, 0, 0);
            label_note30.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note30.Location = new Point(257, 503);
            label_note30.Name = "label_note30";
            label_note30.Size = new Size(44, 23);
            label_note30.TabIndex = 3;
            label_note30.Text = "C#4";
            label_note30.Visible = false;
            // 
            // label_note28
            // 
            label_note28.Anchor = AnchorStyles.None;
            label_note28.AutoSize = true;
            label_note28.BackColor = Color.FromArgb(192, 0, 0);
            label_note28.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note28.Location = new Point(159, 503);
            label_note28.Name = "label_note28";
            label_note28.Size = new Size(44, 23);
            label_note28.TabIndex = 3;
            label_note28.Text = "C#4";
            label_note28.Visible = false;
            // 
            // label_note32
            // 
            label_note32.Anchor = AnchorStyles.None;
            label_note32.AutoSize = true;
            label_note32.BackColor = Color.FromArgb(192, 0, 0);
            label_note32.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note32.Location = new Point(356, 503);
            label_note32.Name = "label_note32";
            label_note32.Size = new Size(44, 23);
            label_note32.TabIndex = 3;
            label_note32.Text = "C#4";
            label_note32.Visible = false;
            // 
            // button_browse_file
            // 
            button_browse_file.Anchor = AnchorStyles.Right;
            button_browse_file.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_browse_file.ImageIndex = 0;
            button_browse_file.ImageList = icons2;
            button_browse_file.Location = new Point(280, 31);
            button_browse_file.Name = "button_browse_file";
            button_browse_file.Size = new Size(123, 35);
            button_browse_file.TabIndex = 0;
            button_browse_file.Text = "Browse File";
            button_browse_file.TextAlign = ContentAlignment.MiddleRight;
            button_browse_file.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_browse_file.UseVisualStyleBackColor = true;
            button_browse_file.Click += button4_Click;
            // 
            // checkBox18
            // 
            checkBox18.Anchor = AnchorStyles.Left;
            checkBox18.AutoSize = true;
            checkBox18.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox18.Location = new Point(11, 538);
            checkBox18.Name = "checkBox18";
            checkBox18.Size = new Size(401, 24);
            checkBox18.TabIndex = 22;
            checkBox18.Text = "Play each note once at a time (don't keep alternating)";
            checkBox18.UseVisualStyleBackColor = true;
            // 
            // checkBox19
            // 
            checkBox19.Anchor = AnchorStyles.Left;
            checkBox19.AutoSize = true;
            checkBox19.Checked = true;
            checkBox19.CheckState = CheckState.Checked;
            checkBox19.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox19.Location = new Point(11, 569);
            checkBox19.Name = "checkBox19";
            checkBox19.Size = new Size(373, 44);
            checkBox19.TabIndex = 23;
            checkBox19.Text = "Try making each cycle last 30mS (with maximium \r\nalternating time capped to 15mS per note)";
            checkBox19.UseVisualStyleBackColor = true;
            // 
            // checkBox20
            // 
            checkBox20.Anchor = AnchorStyles.Left;
            checkBox20.AutoSize = true;
            checkBox20.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox20.Location = new Point(11, 618);
            checkBox20.Name = "checkBox20";
            checkBox20.Size = new Size(199, 24);
            checkBox20.TabIndex = 24;
            checkBox20.Text = "Don't update grid above";
            checkBox20.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // MIDI_file_player
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 653);
            Controls.Add(button_browse_file);
            Controls.Add(checkBox17);
            Controls.Add(checkBox19);
            Controls.Add(checkBox20);
            Controls.Add(checkBox18);
            Controls.Add(checkBox13);
            Controls.Add(checkBox9);
            Controls.Add(checkBox5);
            Controls.Add(checkBox16);
            Controls.Add(checkBox12);
            Controls.Add(checkBox8);
            Controls.Add(checkBox3);
            Controls.Add(checkBox15);
            Controls.Add(checkBox11);
            Controls.Add(checkBox7);
            Controls.Add(checkBox4);
            Controls.Add(checkBox14);
            Controls.Add(checkBox10);
            Controls.Add(checkBox6);
            Controls.Add(checkBox2);
            Controls.Add(label_note32);
            Controls.Add(label_note24);
            Controls.Add(label_note16);
            Controls.Add(label_note8);
            Controls.Add(label_note28);
            Controls.Add(label_note20);
            Controls.Add(label_note12);
            Controls.Add(label_note4);
            Controls.Add(label_note30);
            Controls.Add(label_note22);
            Controls.Add(label_note14);
            Controls.Add(label_note6);
            Controls.Add(label_note26);
            Controls.Add(label_note18);
            Controls.Add(label_note10);
            Controls.Add(label_note2);
            Controls.Add(label_note31);
            Controls.Add(label_note23);
            Controls.Add(label_note15);
            Controls.Add(label_note7);
            Controls.Add(label_note27);
            Controls.Add(label_note19);
            Controls.Add(label_note11);
            Controls.Add(label_note3);
            Controls.Add(label_note29);
            Controls.Add(label_note21);
            Controls.Add(label_note13);
            Controls.Add(label_note5);
            Controls.Add(label_note25);
            Controls.Add(label_note17);
            Controls.Add(label_note9);
            Controls.Add(label_note1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MIDI_file_player";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Play MIDI File";
            FormClosing += MIDI_file_player_FormClosing;
            DragDrop += MIDI_file_player_DragDrop;
            DragEnter += MIDI_file_player_DragEnter;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
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
        private Label label_percentage;
        private Label label_position;
        private Label label4;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private CheckBox checkBox7;
        private CheckBox checkBox8;
        private CheckBox checkBox9;
        private CheckBox checkBox10;
        private CheckBox checkBox11;
        private CheckBox checkBox12;
        private CheckBox checkBox13;
        private CheckBox checkBox14;
        private CheckBox checkBox15;
        private CheckBox checkBox16;
        private CheckBox checkBox17;
        private Label label5;
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
        private CheckBox checkBox18;
        private CheckBox checkBox19;
        private CheckBox checkBox20;
        private OpenFileDialog openFileDialog;
        private ToolTip toolTip1;
        private System.Windows.Forms.Timer update_timer;
    }
}