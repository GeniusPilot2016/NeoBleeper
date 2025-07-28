namespace NeoBleeper
{
    partial class ConvertToGCode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertToGCode));
            label_note1 = new Label();
            comboBox_component_note1 = new ComboBox();
            checkBox_play_note1 = new CheckBox();
            label_note2 = new Label();
            comboBox_component_note2 = new ComboBox();
            checkBox_play_note2 = new CheckBox();
            label_note3 = new Label();
            comboBox_component_note3 = new ComboBox();
            checkBox_play_note3 = new CheckBox();
            label_note4 = new Label();
            comboBox_component_note4 = new ComboBox();
            checkBox_play_note4 = new CheckBox();
            button_export_as_gcode = new Button();
            icons = new ImageList(components);
            label5 = new Label();
            exportGCodeFile = new SaveFileDialog();
            lbl_alternating_note_options = new Label();
            radioButtonPlay_alternating_notes2 = new RadioButton();
            radioButtonPlay_alternating_notes1 = new RadioButton();
            SuspendLayout();
            // 
            // label_note1
            // 
            label_note1.Anchor = AnchorStyles.None;
            label_note1.AutoSize = true;
            label_note1.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_note1.Location = new Point(34, 72);
            label_note1.Margin = new Padding(2, 0, 2, 0);
            label_note1.Name = "label_note1";
            label_note1.Size = new Size(44, 16);
            label_note1.TabIndex = 0;
            label_note1.Text = "Note 1";
            // 
            // comboBox_component_note1
            // 
            comboBox_component_note1.Anchor = AnchorStyles.None;
            comboBox_component_note1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note1.Font = new Font("HarmonyOS Sans", 8.999999F);
            comboBox_component_note1.FormattingEnabled = true;
            comboBox_component_note1.Items.AddRange(new object[] { "M3/M4 - Motor", "M300 - Buzzer" });
            comboBox_component_note1.Location = new Point(83, 70);
            comboBox_component_note1.Margin = new Padding(2, 2, 2, 2);
            comboBox_component_note1.Name = "comboBox_component_note1";
            comboBox_component_note1.Size = new Size(130, 24);
            comboBox_component_note1.TabIndex = 1;
            comboBox_component_note1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // checkBox_play_note1
            // 
            checkBox_play_note1.Anchor = AnchorStyles.None;
            checkBox_play_note1.AutoSize = true;
            checkBox_play_note1.Checked = true;
            checkBox_play_note1.CheckState = CheckState.Checked;
            checkBox_play_note1.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_play_note1.Location = new Point(228, 73);
            checkBox_play_note1.Margin = new Padding(2, 2, 2, 2);
            checkBox_play_note1.Name = "checkBox_play_note1";
            checkBox_play_note1.Size = new Size(89, 20);
            checkBox_play_note1.TabIndex = 2;
            checkBox_play_note1.Text = "Play Note 1";
            checkBox_play_note1.UseVisualStyleBackColor = true;
            checkBox_play_note1.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // label_note2
            // 
            label_note2.Anchor = AnchorStyles.None;
            label_note2.AutoSize = true;
            label_note2.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_note2.Location = new Point(34, 103);
            label_note2.Margin = new Padding(2, 0, 2, 0);
            label_note2.Name = "label_note2";
            label_note2.Size = new Size(44, 16);
            label_note2.TabIndex = 0;
            label_note2.Text = "Note 2";
            // 
            // comboBox_component_note2
            // 
            comboBox_component_note2.Anchor = AnchorStyles.None;
            comboBox_component_note2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note2.Font = new Font("HarmonyOS Sans", 8.999999F);
            comboBox_component_note2.FormattingEnabled = true;
            comboBox_component_note2.Items.AddRange(new object[] { "M3/M4 - Motor", "M300 - Buzzer" });
            comboBox_component_note2.Location = new Point(83, 101);
            comboBox_component_note2.Margin = new Padding(2, 2, 2, 2);
            comboBox_component_note2.Name = "comboBox_component_note2";
            comboBox_component_note2.Size = new Size(130, 24);
            comboBox_component_note2.TabIndex = 1;
            comboBox_component_note2.SelectedIndexChanged += comboBox_component_note2_SelectedIndexChanged;
            // 
            // checkBox_play_note2
            // 
            checkBox_play_note2.Anchor = AnchorStyles.None;
            checkBox_play_note2.AutoSize = true;
            checkBox_play_note2.Checked = true;
            checkBox_play_note2.CheckState = CheckState.Checked;
            checkBox_play_note2.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_play_note2.Location = new Point(228, 104);
            checkBox_play_note2.Margin = new Padding(2, 2, 2, 2);
            checkBox_play_note2.Name = "checkBox_play_note2";
            checkBox_play_note2.Size = new Size(89, 20);
            checkBox_play_note2.TabIndex = 2;
            checkBox_play_note2.Text = "Play Note 2";
            checkBox_play_note2.UseVisualStyleBackColor = true;
            checkBox_play_note2.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // label_note3
            // 
            label_note3.Anchor = AnchorStyles.None;
            label_note3.AutoSize = true;
            label_note3.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_note3.Location = new Point(34, 134);
            label_note3.Margin = new Padding(2, 0, 2, 0);
            label_note3.Name = "label_note3";
            label_note3.Size = new Size(44, 16);
            label_note3.TabIndex = 0;
            label_note3.Text = "Note 3";
            // 
            // comboBox_component_note3
            // 
            comboBox_component_note3.Anchor = AnchorStyles.None;
            comboBox_component_note3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note3.Font = new Font("HarmonyOS Sans", 8.999999F);
            comboBox_component_note3.FormattingEnabled = true;
            comboBox_component_note3.Items.AddRange(new object[] { "M3/M4 - Motor", "M300 - Buzzer" });
            comboBox_component_note3.Location = new Point(83, 132);
            comboBox_component_note3.Margin = new Padding(2, 2, 2, 2);
            comboBox_component_note3.Name = "comboBox_component_note3";
            comboBox_component_note3.Size = new Size(130, 24);
            comboBox_component_note3.TabIndex = 1;
            comboBox_component_note3.SelectedIndexChanged += comboBox_component_note3_SelectedIndexChanged;
            // 
            // checkBox_play_note3
            // 
            checkBox_play_note3.Anchor = AnchorStyles.None;
            checkBox_play_note3.AutoSize = true;
            checkBox_play_note3.Checked = true;
            checkBox_play_note3.CheckState = CheckState.Checked;
            checkBox_play_note3.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_play_note3.Location = new Point(228, 135);
            checkBox_play_note3.Margin = new Padding(2, 2, 2, 2);
            checkBox_play_note3.Name = "checkBox_play_note3";
            checkBox_play_note3.Size = new Size(89, 20);
            checkBox_play_note3.TabIndex = 2;
            checkBox_play_note3.Text = "Play Note 3";
            checkBox_play_note3.UseVisualStyleBackColor = true;
            checkBox_play_note3.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // label_note4
            // 
            label_note4.Anchor = AnchorStyles.None;
            label_note4.AutoSize = true;
            label_note4.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_note4.Location = new Point(34, 167);
            label_note4.Margin = new Padding(2, 0, 2, 0);
            label_note4.Name = "label_note4";
            label_note4.Size = new Size(44, 16);
            label_note4.TabIndex = 0;
            label_note4.Text = "Note 4";
            // 
            // comboBox_component_note4
            // 
            comboBox_component_note4.Anchor = AnchorStyles.None;
            comboBox_component_note4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note4.Font = new Font("HarmonyOS Sans", 8.999999F);
            comboBox_component_note4.FormattingEnabled = true;
            comboBox_component_note4.Items.AddRange(new object[] { "M3/M4 - Motor", "M300 - Buzzer" });
            comboBox_component_note4.Location = new Point(83, 165);
            comboBox_component_note4.Margin = new Padding(2, 2, 2, 2);
            comboBox_component_note4.Name = "comboBox_component_note4";
            comboBox_component_note4.Size = new Size(130, 24);
            comboBox_component_note4.TabIndex = 1;
            comboBox_component_note4.SelectedIndexChanged += comboBox_component_note4_SelectedIndexChanged;
            // 
            // checkBox_play_note4
            // 
            checkBox_play_note4.Anchor = AnchorStyles.None;
            checkBox_play_note4.AutoSize = true;
            checkBox_play_note4.Checked = true;
            checkBox_play_note4.CheckState = CheckState.Checked;
            checkBox_play_note4.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox_play_note4.Location = new Point(228, 168);
            checkBox_play_note4.Margin = new Padding(2, 2, 2, 2);
            checkBox_play_note4.Name = "checkBox_play_note4";
            checkBox_play_note4.Size = new Size(89, 20);
            checkBox_play_note4.TabIndex = 2;
            checkBox_play_note4.Text = "Play Note 4";
            checkBox_play_note4.UseVisualStyleBackColor = true;
            checkBox_play_note4.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // button_export_as_gcode
            // 
            button_export_as_gcode.Anchor = AnchorStyles.Bottom;
            button_export_as_gcode.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button_export_as_gcode.ImageIndex = 0;
            button_export_as_gcode.ImageList = icons;
            button_export_as_gcode.Location = new Point(98, 325);
            button_export_as_gcode.Margin = new Padding(2, 2, 2, 2);
            button_export_as_gcode.Name = "button_export_as_gcode";
            button_export_as_gcode.Size = new Size(131, 31);
            button_export_as_gcode.TabIndex = 3;
            button_export_as_gcode.Text = "Export As GCode";
            button_export_as_gcode.TextAlign = ContentAlignment.MiddleRight;
            button_export_as_gcode.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_export_as_gcode.UseVisualStyleBackColor = true;
            button_export_as_gcode.Click += button_export_as_gcode_Click;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-3d-printer-48.png");
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top;
            label5.AutoSize = true;
            label5.Font = new Font("HarmonyOS Sans", 8.999999F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(26, 18);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.MaximumSize = new Size(285, 0);
            label5.Name = "label5";
            label5.Size = new Size(278, 32);
            label5.TabIndex = 4;
            label5.Text = "Select the notes and component type you want to play on the 3D printer/CNC machine.";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_alternating_note_options
            // 
            lbl_alternating_note_options.Anchor = AnchorStyles.None;
            lbl_alternating_note_options.AutoSize = true;
            lbl_alternating_note_options.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Bold);
            lbl_alternating_note_options.ImeMode = ImeMode.NoControl;
            lbl_alternating_note_options.Location = new Point(66, 199);
            lbl_alternating_note_options.Margin = new Padding(4, 0, 4, 0);
            lbl_alternating_note_options.MaximumSize = new Size(240, 0);
            lbl_alternating_note_options.Name = "lbl_alternating_note_options";
            lbl_alternating_note_options.Size = new Size(196, 16);
            lbl_alternating_note_options.TabIndex = 115;
            lbl_alternating_note_options.Text = "Playing Alternating Note Options";
            lbl_alternating_note_options.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // radioButtonPlay_alternating_notes2
            // 
            radioButtonPlay_alternating_notes2.Anchor = AnchorStyles.None;
            radioButtonPlay_alternating_notes2.AutoSize = true;
            radioButtonPlay_alternating_notes2.Font = new Font("HarmonyOS Sans", 9F);
            radioButtonPlay_alternating_notes2.ImeMode = ImeMode.NoControl;
            radioButtonPlay_alternating_notes2.Location = new Point(34, 257);
            radioButtonPlay_alternating_notes2.Margin = new Padding(4, 3, 4, 3);
            radioButtonPlay_alternating_notes2.MaximumSize = new Size(260, 80);
            radioButtonPlay_alternating_notes2.MinimumSize = new Size(0, 60);
            radioButtonPlay_alternating_notes2.Name = "radioButtonPlay_alternating_notes2";
            radioButtonPlay_alternating_notes2.Size = new Size(260, 60);
            radioButtonPlay_alternating_notes2.TabIndex = 117;
            radioButtonPlay_alternating_notes2.Text = "Play alternating notes in the odd numbered columns first, then in even numbered columns";
            radioButtonPlay_alternating_notes2.UseVisualStyleBackColor = true;
            // 
            // radioButtonPlay_alternating_notes1
            // 
            radioButtonPlay_alternating_notes1.Anchor = AnchorStyles.None;
            radioButtonPlay_alternating_notes1.AutoSize = true;
            radioButtonPlay_alternating_notes1.Checked = true;
            radioButtonPlay_alternating_notes1.Font = new Font("HarmonyOS Sans", 9F);
            radioButtonPlay_alternating_notes1.ImeMode = ImeMode.NoControl;
            radioButtonPlay_alternating_notes1.Location = new Point(34, 226);
            radioButtonPlay_alternating_notes1.Margin = new Padding(4, 3, 4, 3);
            radioButtonPlay_alternating_notes1.MaximumSize = new Size(260, 0);
            radioButtonPlay_alternating_notes1.MinimumSize = new Size(0, 25);
            radioButtonPlay_alternating_notes1.Name = "radioButtonPlay_alternating_notes1";
            radioButtonPlay_alternating_notes1.Size = new Size(189, 25);
            radioButtonPlay_alternating_notes1.TabIndex = 116;
            radioButtonPlay_alternating_notes1.TabStop = true;
            radioButtonPlay_alternating_notes1.Text = "Play alternating notes in order";
            radioButtonPlay_alternating_notes1.UseVisualStyleBackColor = true;
            // 
            // ConvertToGCode
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(329, 366);
            Controls.Add(lbl_alternating_note_options);
            Controls.Add(radioButtonPlay_alternating_notes2);
            Controls.Add(radioButtonPlay_alternating_notes1);
            Controls.Add(label5);
            Controls.Add(button_export_as_gcode);
            Controls.Add(checkBox_play_note4);
            Controls.Add(comboBox_component_note4);
            Controls.Add(checkBox_play_note3);
            Controls.Add(comboBox_component_note3);
            Controls.Add(checkBox_play_note2);
            Controls.Add(label_note4);
            Controls.Add(comboBox_component_note2);
            Controls.Add(label_note3);
            Controls.Add(checkBox_play_note1);
            Controls.Add(label_note2);
            Controls.Add(comboBox_component_note1);
            Controls.Add(label_note1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2, 2, 2, 2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConvertToGCode";
            ShowIcon = false;
            Text = "Convert File to GCode";
            SystemColorsChanged += ConvertToGCode_SystemColorsChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_note1;
        private ComboBox comboBox_component_note1;
        private CheckBox checkBox_play_note1;
        private Label label_note2;
        private ComboBox comboBox_component_note2;
        private CheckBox checkBox_play_note2;
        private Label label_note3;
        private ComboBox comboBox_component_note3;
        private CheckBox checkBox_play_note3;
        private Label label_note4;
        private ComboBox comboBox_component_note4;
        private CheckBox checkBox_play_note4;
        private Button button_export_as_gcode;
        private Label label5;
        private ImageList icons;
        private SaveFileDialog exportGCodeFile;
        private Label lbl_alternating_note_options;
        private RadioButton radioButtonPlay_alternating_notes2;
        private RadioButton radioButtonPlay_alternating_notes1;
    }
}