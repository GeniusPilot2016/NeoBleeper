﻿namespace NeoBleeper
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
            label3 = new Label();
            label2 = new Label();
            checkBox1 = new CheckBox();
            icons2 = new ImageList(components);
            button3 = new Button();
            icons = new ImageList(components);
            button2 = new Button();
            button1 = new Button();
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
            button4 = new Button();
            checkBox18 = new CheckBox();
            checkBox19 = new CheckBox();
            checkBox20 = new CheckBox();
            openFileDialog = new OpenFileDialog();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(10, 34);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(300, 27);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(195, 20);
            label1.TabIndex = 1;
            label1.Text = "Currently playing MIDI file:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Controls.Add(trackBar1);
            groupBox1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(12, 67);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(427, 109);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Playback Controls";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(369, 77);
            label3.Name = "label3";
            label3.RightToLeft = RightToLeft.Yes;
            label3.Size = new Size(52, 20);
            label3.TabIndex = 3;
            label3.Text = "0,00%";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(286, 57);
            label2.Name = "label2";
            label2.Size = new Size(135, 20);
            label2.TabIndex = 3;
            label2.Text = "Position: 00:00.00";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.ImageIndex = 1;
            checkBox1.ImageList = icons2;
            checkBox1.Location = new Point(123, 60);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(82, 24);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "Loop";
            checkBox1.TextImageRelation = TextImageRelation.ImageBeforeText;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // icons2
            // 
            icons2.ColorDepth = ColorDepth.Depth32Bit;
            icons2.ImageStream = (ImageListStreamer)resources.GetObject("icons2.ImageStream");
            icons2.TransparentColor = Color.Transparent;
            icons2.Images.SetKeyName(0, "icons8-browse-folder-48.png");
            icons2.Images.SetKeyName(1, "icons8-loop-48.png");
            // 
            // button3
            // 
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.ImageIndex = 1;
            button3.ImageList = icons;
            button3.Location = new Point(82, 57);
            button3.Name = "button3";
            button3.Size = new Size(27, 27);
            button3.TabIndex = 4;
            button3.UseVisualStyleBackColor = true;
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
            // button2
            // 
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.ImageIndex = 0;
            button2.ImageList = icons;
            button2.Location = new Point(49, 57);
            button2.Name = "button2";
            button2.Size = new Size(27, 27);
            button2.TabIndex = 3;
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ImageIndex = 2;
            button1.ImageList = icons;
            button1.Location = new Point(16, 57);
            button1.Name = "button1";
            button1.Size = new Size(27, 27);
            button1.TabIndex = 2;
            button1.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(6, 26);
            trackBar1.Maximum = 255;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(415, 56);
            trackBar1.TabIndex = 1;
            trackBar1.TickStyle = TickStyle.None;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(12, 192);
            label4.Name = "label4";
            label4.Size = new Size(343, 40);
            label4.TabIndex = 3;
            label4.Text = "Select the channel(s) that should be listened to. \r\nChannels that are not selected will be ignored.";
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox2.Location = new Point(18, 244);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(40, 24);
            checkBox2.TabIndex = 6;
            checkBox2.Text = "1";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox3.Location = new Point(72, 244);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(40, 24);
            checkBox3.TabIndex = 7;
            checkBox3.Text = "2";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Checked = true;
            checkBox4.CheckState = CheckState.Checked;
            checkBox4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox4.Location = new Point(129, 244);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(40, 24);
            checkBox4.TabIndex = 8;
            checkBox4.Text = "3";
            checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Checked = true;
            checkBox5.CheckState = CheckState.Checked;
            checkBox5.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox5.Location = new Point(185, 244);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(40, 24);
            checkBox5.TabIndex = 9;
            checkBox5.Text = "4";
            checkBox5.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Checked = true;
            checkBox6.CheckState = CheckState.Checked;
            checkBox6.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox6.Location = new Point(18, 274);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(40, 24);
            checkBox6.TabIndex = 10;
            checkBox6.Text = "5";
            checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            checkBox7.AutoSize = true;
            checkBox7.Checked = true;
            checkBox7.CheckState = CheckState.Checked;
            checkBox7.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox7.Location = new Point(129, 274);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(40, 24);
            checkBox7.TabIndex = 12;
            checkBox7.Text = "7";
            checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            checkBox8.AutoSize = true;
            checkBox8.Checked = true;
            checkBox8.CheckState = CheckState.Checked;
            checkBox8.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox8.Location = new Point(72, 274);
            checkBox8.Name = "checkBox8";
            checkBox8.Size = new Size(40, 24);
            checkBox8.TabIndex = 11;
            checkBox8.Text = "6";
            checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            checkBox9.AutoSize = true;
            checkBox9.Checked = true;
            checkBox9.CheckState = CheckState.Checked;
            checkBox9.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox9.Location = new Point(185, 274);
            checkBox9.Name = "checkBox9";
            checkBox9.Size = new Size(40, 24);
            checkBox9.TabIndex = 13;
            checkBox9.Text = "8";
            checkBox9.UseVisualStyleBackColor = true;
            // 
            // checkBox10
            // 
            checkBox10.AutoSize = true;
            checkBox10.Checked = true;
            checkBox10.CheckState = CheckState.Checked;
            checkBox10.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox10.Location = new Point(18, 304);
            checkBox10.Name = "checkBox10";
            checkBox10.Size = new Size(40, 24);
            checkBox10.TabIndex = 14;
            checkBox10.Text = "9";
            checkBox10.UseVisualStyleBackColor = true;
            // 
            // checkBox11
            // 
            checkBox11.AutoSize = true;
            checkBox11.Checked = true;
            checkBox11.CheckState = CheckState.Checked;
            checkBox11.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox11.Location = new Point(129, 304);
            checkBox11.Name = "checkBox11";
            checkBox11.Size = new Size(49, 24);
            checkBox11.TabIndex = 16;
            checkBox11.Text = "11";
            checkBox11.UseVisualStyleBackColor = true;
            // 
            // checkBox12
            // 
            checkBox12.AutoSize = true;
            checkBox12.Checked = true;
            checkBox12.CheckState = CheckState.Checked;
            checkBox12.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox12.Location = new Point(72, 304);
            checkBox12.Name = "checkBox12";
            checkBox12.Size = new Size(49, 24);
            checkBox12.TabIndex = 15;
            checkBox12.Text = "10";
            checkBox12.UseVisualStyleBackColor = true;
            // 
            // checkBox13
            // 
            checkBox13.AutoSize = true;
            checkBox13.Checked = true;
            checkBox13.CheckState = CheckState.Checked;
            checkBox13.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox13.Location = new Point(185, 304);
            checkBox13.Name = "checkBox13";
            checkBox13.Size = new Size(49, 24);
            checkBox13.TabIndex = 17;
            checkBox13.Text = "12";
            checkBox13.UseVisualStyleBackColor = true;
            // 
            // checkBox14
            // 
            checkBox14.AutoSize = true;
            checkBox14.Checked = true;
            checkBox14.CheckState = CheckState.Checked;
            checkBox14.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox14.Location = new Point(18, 334);
            checkBox14.Name = "checkBox14";
            checkBox14.Size = new Size(49, 24);
            checkBox14.TabIndex = 18;
            checkBox14.Text = "13";
            checkBox14.UseVisualStyleBackColor = true;
            // 
            // checkBox15
            // 
            checkBox15.AutoSize = true;
            checkBox15.Checked = true;
            checkBox15.CheckState = CheckState.Checked;
            checkBox15.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox15.Location = new Point(129, 334);
            checkBox15.Name = "checkBox15";
            checkBox15.Size = new Size(49, 24);
            checkBox15.TabIndex = 20;
            checkBox15.Text = "15";
            checkBox15.UseVisualStyleBackColor = true;
            // 
            // checkBox16
            // 
            checkBox16.AutoSize = true;
            checkBox16.Checked = true;
            checkBox16.CheckState = CheckState.Checked;
            checkBox16.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox16.Location = new Point(72, 334);
            checkBox16.Name = "checkBox16";
            checkBox16.Size = new Size(49, 24);
            checkBox16.TabIndex = 19;
            checkBox16.Text = "14";
            checkBox16.UseVisualStyleBackColor = true;
            // 
            // checkBox17
            // 
            checkBox17.AutoSize = true;
            checkBox17.Checked = true;
            checkBox17.CheckState = CheckState.Checked;
            checkBox17.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox17.Location = new Point(185, 334);
            checkBox17.Name = "checkBox17";
            checkBox17.Size = new Size(49, 24);
            checkBox17.TabIndex = 21;
            checkBox17.Text = "16";
            checkBox17.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(12, 372);
            label5.Name = "label5";
            label5.Size = new Size(314, 20);
            label5.TabIndex = 3;
            label5.Text = "Notes which are currently being held on: (0)";
            // 
            // label_note1
            // 
            label_note1.AutoSize = true;
            label_note1.BackColor = Color.Red;
            label_note1.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note1.Location = new Point(14, 401);
            label_note1.Name = "label_note1";
            label_note1.Size = new Size(44, 23);
            label_note1.TabIndex = 3;
            label_note1.Text = "C#4";
            label_note1.Visible = false;
            // 
            // label_note2
            // 
            label_note2.AutoSize = true;
            label_note2.BackColor = Color.FromArgb(192, 0, 0);
            label_note2.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note2.Location = new Point(68, 401);
            label_note2.Name = "label_note2";
            label_note2.Size = new Size(44, 23);
            label_note2.TabIndex = 3;
            label_note2.Text = "C#4";
            label_note2.Visible = false;
            // 
            // label_note3
            // 
            label_note3.AutoSize = true;
            label_note3.BackColor = Color.FromArgb(192, 0, 0);
            label_note3.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note3.Location = new Point(122, 401);
            label_note3.Name = "label_note3";
            label_note3.Size = new Size(44, 23);
            label_note3.TabIndex = 3;
            label_note3.Text = "C#4";
            label_note3.Visible = false;
            // 
            // label_note4
            // 
            label_note4.AutoSize = true;
            label_note4.BackColor = Color.FromArgb(192, 0, 0);
            label_note4.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note4.Location = new Point(176, 401);
            label_note4.Name = "label_note4";
            label_note4.Size = new Size(44, 23);
            label_note4.TabIndex = 3;
            label_note4.Text = "C#4";
            label_note4.Visible = false;
            // 
            // label_note5
            // 
            label_note5.AutoSize = true;
            label_note5.BackColor = Color.FromArgb(192, 0, 0);
            label_note5.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note5.Location = new Point(230, 401);
            label_note5.Name = "label_note5";
            label_note5.Size = new Size(44, 23);
            label_note5.TabIndex = 3;
            label_note5.Text = "C#4";
            label_note5.Visible = false;
            // 
            // label_note7
            // 
            label_note7.AutoSize = true;
            label_note7.BackColor = Color.FromArgb(192, 0, 0);
            label_note7.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note7.Location = new Point(338, 401);
            label_note7.Name = "label_note7";
            label_note7.Size = new Size(44, 23);
            label_note7.TabIndex = 3;
            label_note7.Text = "C#4";
            label_note7.Visible = false;
            // 
            // label_note6
            // 
            label_note6.AutoSize = true;
            label_note6.BackColor = Color.FromArgb(192, 0, 0);
            label_note6.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note6.Location = new Point(284, 401);
            label_note6.Name = "label_note6";
            label_note6.Size = new Size(44, 23);
            label_note6.TabIndex = 3;
            label_note6.Text = "C#4";
            label_note6.Visible = false;
            // 
            // label_note8
            // 
            label_note8.AutoSize = true;
            label_note8.BackColor = Color.FromArgb(192, 0, 0);
            label_note8.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note8.Location = new Point(392, 401);
            label_note8.Name = "label_note8";
            label_note8.Size = new Size(44, 23);
            label_note8.TabIndex = 3;
            label_note8.Text = "C#4";
            label_note8.Visible = false;
            // 
            // label_note9
            // 
            label_note9.AutoSize = true;
            label_note9.BackColor = Color.FromArgb(192, 0, 0);
            label_note9.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note9.Location = new Point(14, 436);
            label_note9.Name = "label_note9";
            label_note9.Size = new Size(44, 23);
            label_note9.TabIndex = 3;
            label_note9.Text = "C#4";
            label_note9.Visible = false;
            // 
            // label_note13
            // 
            label_note13.AutoSize = true;
            label_note13.BackColor = Color.FromArgb(192, 0, 0);
            label_note13.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note13.Location = new Point(230, 436);
            label_note13.Name = "label_note13";
            label_note13.Size = new Size(44, 23);
            label_note13.TabIndex = 3;
            label_note13.Text = "C#4";
            label_note13.Visible = false;
            // 
            // label_note11
            // 
            label_note11.AutoSize = true;
            label_note11.BackColor = Color.FromArgb(192, 0, 0);
            label_note11.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note11.Location = new Point(122, 436);
            label_note11.Name = "label_note11";
            label_note11.Size = new Size(44, 23);
            label_note11.TabIndex = 3;
            label_note11.Text = "C#4";
            label_note11.Visible = false;
            // 
            // label_note15
            // 
            label_note15.AutoSize = true;
            label_note15.BackColor = Color.FromArgb(192, 0, 0);
            label_note15.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note15.Location = new Point(338, 436);
            label_note15.Name = "label_note15";
            label_note15.Size = new Size(44, 23);
            label_note15.TabIndex = 3;
            label_note15.Text = "C#4";
            label_note15.Visible = false;
            // 
            // label_note10
            // 
            label_note10.AutoSize = true;
            label_note10.BackColor = Color.FromArgb(192, 0, 0);
            label_note10.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note10.Location = new Point(68, 436);
            label_note10.Name = "label_note10";
            label_note10.Size = new Size(44, 23);
            label_note10.TabIndex = 3;
            label_note10.Text = "C#4";
            label_note10.Visible = false;
            // 
            // label_note14
            // 
            label_note14.AutoSize = true;
            label_note14.BackColor = Color.FromArgb(192, 0, 0);
            label_note14.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note14.Location = new Point(284, 436);
            label_note14.Name = "label_note14";
            label_note14.Size = new Size(44, 23);
            label_note14.TabIndex = 3;
            label_note14.Text = "C#4";
            label_note14.Visible = false;
            // 
            // label_note12
            // 
            label_note12.AutoSize = true;
            label_note12.BackColor = Color.FromArgb(192, 0, 0);
            label_note12.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note12.Location = new Point(176, 436);
            label_note12.Name = "label_note12";
            label_note12.Size = new Size(44, 23);
            label_note12.TabIndex = 3;
            label_note12.Text = "C#4";
            label_note12.Visible = false;
            // 
            // label_note16
            // 
            label_note16.AutoSize = true;
            label_note16.BackColor = Color.FromArgb(192, 0, 0);
            label_note16.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note16.Location = new Point(392, 436);
            label_note16.Name = "label_note16";
            label_note16.Size = new Size(44, 23);
            label_note16.TabIndex = 3;
            label_note16.Text = "C#4";
            label_note16.Visible = false;
            // 
            // label_note17
            // 
            label_note17.AutoSize = true;
            label_note17.BackColor = Color.FromArgb(192, 0, 0);
            label_note17.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note17.Location = new Point(14, 469);
            label_note17.Name = "label_note17";
            label_note17.Size = new Size(44, 23);
            label_note17.TabIndex = 3;
            label_note17.Text = "C#4";
            label_note17.Visible = false;
            // 
            // label_note21
            // 
            label_note21.AutoSize = true;
            label_note21.BackColor = Color.FromArgb(192, 0, 0);
            label_note21.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note21.Location = new Point(230, 469);
            label_note21.Name = "label_note21";
            label_note21.Size = new Size(44, 23);
            label_note21.TabIndex = 3;
            label_note21.Text = "C#4";
            label_note21.Visible = false;
            // 
            // label_note19
            // 
            label_note19.AutoSize = true;
            label_note19.BackColor = Color.FromArgb(192, 0, 0);
            label_note19.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note19.Location = new Point(122, 469);
            label_note19.Name = "label_note19";
            label_note19.Size = new Size(44, 23);
            label_note19.TabIndex = 3;
            label_note19.Text = "C#4";
            label_note19.Visible = false;
            // 
            // label_note23
            // 
            label_note23.AutoSize = true;
            label_note23.BackColor = Color.FromArgb(192, 0, 0);
            label_note23.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note23.Location = new Point(338, 469);
            label_note23.Name = "label_note23";
            label_note23.Size = new Size(44, 23);
            label_note23.TabIndex = 3;
            label_note23.Text = "C#4";
            label_note23.Visible = false;
            // 
            // label_note18
            // 
            label_note18.AutoSize = true;
            label_note18.BackColor = Color.FromArgb(192, 0, 0);
            label_note18.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note18.Location = new Point(68, 469);
            label_note18.Name = "label_note18";
            label_note18.Size = new Size(44, 23);
            label_note18.TabIndex = 3;
            label_note18.Text = "C#4";
            label_note18.Visible = false;
            // 
            // label_note22
            // 
            label_note22.AutoSize = true;
            label_note22.BackColor = Color.FromArgb(192, 0, 0);
            label_note22.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note22.Location = new Point(284, 469);
            label_note22.Name = "label_note22";
            label_note22.Size = new Size(44, 23);
            label_note22.TabIndex = 3;
            label_note22.Text = "C#4";
            label_note22.Visible = false;
            // 
            // label_note20
            // 
            label_note20.AutoSize = true;
            label_note20.BackColor = Color.FromArgb(192, 0, 0);
            label_note20.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note20.Location = new Point(176, 469);
            label_note20.Name = "label_note20";
            label_note20.Size = new Size(44, 23);
            label_note20.TabIndex = 3;
            label_note20.Text = "C#4";
            label_note20.Visible = false;
            // 
            // label_note24
            // 
            label_note24.AutoSize = true;
            label_note24.BackColor = Color.FromArgb(192, 0, 0);
            label_note24.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note24.Location = new Point(392, 469);
            label_note24.Name = "label_note24";
            label_note24.Size = new Size(44, 23);
            label_note24.TabIndex = 3;
            label_note24.Text = "C#4";
            label_note24.Visible = false;
            // 
            // label_note25
            // 
            label_note25.AutoSize = true;
            label_note25.BackColor = Color.FromArgb(192, 0, 0);
            label_note25.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note25.Location = new Point(14, 503);
            label_note25.Name = "label_note25";
            label_note25.Size = new Size(44, 23);
            label_note25.TabIndex = 3;
            label_note25.Text = "C#4";
            label_note25.Visible = false;
            // 
            // label_note29
            // 
            label_note29.AutoSize = true;
            label_note29.BackColor = Color.FromArgb(192, 0, 0);
            label_note29.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note29.Location = new Point(230, 503);
            label_note29.Name = "label_note29";
            label_note29.Size = new Size(44, 23);
            label_note29.TabIndex = 3;
            label_note29.Text = "C#4";
            label_note29.Visible = false;
            // 
            // label_note27
            // 
            label_note27.AutoSize = true;
            label_note27.BackColor = Color.FromArgb(192, 0, 0);
            label_note27.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note27.Location = new Point(122, 503);
            label_note27.Name = "label_note27";
            label_note27.Size = new Size(44, 23);
            label_note27.TabIndex = 3;
            label_note27.Text = "C#4";
            label_note27.Visible = false;
            // 
            // label_note31
            // 
            label_note31.AutoSize = true;
            label_note31.BackColor = Color.FromArgb(192, 0, 0);
            label_note31.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note31.Location = new Point(338, 503);
            label_note31.Name = "label_note31";
            label_note31.Size = new Size(44, 23);
            label_note31.TabIndex = 3;
            label_note31.Text = "C#4";
            label_note31.Visible = false;
            // 
            // label_note26
            // 
            label_note26.AutoSize = true;
            label_note26.BackColor = Color.FromArgb(192, 0, 0);
            label_note26.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note26.Location = new Point(68, 503);
            label_note26.Name = "label_note26";
            label_note26.Size = new Size(44, 23);
            label_note26.TabIndex = 3;
            label_note26.Text = "C#4";
            label_note26.Visible = false;
            // 
            // label_note30
            // 
            label_note30.AutoSize = true;
            label_note30.BackColor = Color.FromArgb(192, 0, 0);
            label_note30.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note30.Location = new Point(284, 503);
            label_note30.Name = "label_note30";
            label_note30.Size = new Size(44, 23);
            label_note30.TabIndex = 3;
            label_note30.Text = "C#4";
            label_note30.Visible = false;
            // 
            // label_note28
            // 
            label_note28.AutoSize = true;
            label_note28.BackColor = Color.FromArgb(192, 0, 0);
            label_note28.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note28.Location = new Point(176, 503);
            label_note28.Name = "label_note28";
            label_note28.Size = new Size(44, 23);
            label_note28.TabIndex = 3;
            label_note28.Text = "C#4";
            label_note28.Visible = false;
            // 
            // label_note32
            // 
            label_note32.AutoSize = true;
            label_note32.BackColor = Color.FromArgb(192, 0, 0);
            label_note32.Font = new Font("HarmonyOS Sans", 10.1999989F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_note32.Location = new Point(392, 503);
            label_note32.Name = "label_note32";
            label_note32.Size = new Size(44, 23);
            label_note32.TabIndex = 3;
            label_note32.Text = "C#4";
            label_note32.Visible = false;
            // 
            // button4
            // 
            button4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button4.ImageIndex = 0;
            button4.ImageList = icons2;
            button4.Location = new Point(316, 32);
            button4.Name = "button4";
            button4.Size = new Size(123, 31);
            button4.TabIndex = 0;
            button4.Text = "Browse File";
            button4.TextAlign = ContentAlignment.MiddleRight;
            button4.TextImageRelation = TextImageRelation.ImageBeforeText;
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // checkBox18
            // 
            checkBox18.AutoSize = true;
            checkBox18.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox18.Location = new Point(18, 540);
            checkBox18.Name = "checkBox18";
            checkBox18.Size = new Size(401, 24);
            checkBox18.TabIndex = 22;
            checkBox18.Text = "Play each note once at a time (don't keep alternating)";
            checkBox18.UseVisualStyleBackColor = true;
            // 
            // checkBox19
            // 
            checkBox19.AutoSize = true;
            checkBox19.Checked = true;
            checkBox19.CheckState = CheckState.Checked;
            checkBox19.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox19.Location = new Point(18, 570);
            checkBox19.Name = "checkBox19";
            checkBox19.Size = new Size(373, 44);
            checkBox19.TabIndex = 23;
            checkBox19.Text = "Try making each cycle last 30mS (with maximium \r\nalternating time capped to 15mS per note)";
            checkBox19.UseVisualStyleBackColor = true;
            // 
            // checkBox20
            // 
            checkBox20.AutoSize = true;
            checkBox20.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox20.Location = new Point(18, 620);
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
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(451, 655);
            Controls.Add(button4);
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
        private Button button3;
        private Button button2;
        private ImageList icons;
        private Button button1;
        private CheckBox checkBox1;
        private Label label3;
        private Label label2;
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
        private Button button4;
        private ImageList icons2;
        private CheckBox checkBox18;
        private CheckBox checkBox19;
        private CheckBox checkBox20;
        private OpenFileDialog openFileDialog;
    }
}