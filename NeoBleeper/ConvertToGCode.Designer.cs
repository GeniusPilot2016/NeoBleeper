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
            SuspendLayout();
            // 
            // label_note1
            // 
            label_note1.Anchor = AnchorStyles.None;
            label_note1.AutoSize = true;
            label_note1.Font = new Font("HarmonyOS Sans", 8.999999F);
            label_note1.Location = new Point(28, 89);
            label_note1.Name = "label_note1";
            label_note1.Size = new Size(56, 20);
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
            comboBox_component_note1.Location = new Point(90, 86);
            comboBox_component_note1.Name = "comboBox_component_note1";
            comboBox_component_note1.Size = new Size(162, 28);
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
            checkBox_play_note1.Location = new Point(271, 90);
            checkBox_play_note1.Name = "checkBox_play_note1";
            checkBox_play_note1.Size = new Size(111, 24);
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
            label_note2.Location = new Point(28, 128);
            label_note2.Name = "label_note2";
            label_note2.Size = new Size(56, 20);
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
            comboBox_component_note2.Location = new Point(90, 125);
            comboBox_component_note2.Name = "comboBox_component_note2";
            comboBox_component_note2.Size = new Size(162, 28);
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
            checkBox_play_note2.Location = new Point(271, 129);
            checkBox_play_note2.Name = "checkBox_play_note2";
            checkBox_play_note2.Size = new Size(111, 24);
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
            label_note3.Location = new Point(28, 167);
            label_note3.Name = "label_note3";
            label_note3.Size = new Size(56, 20);
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
            comboBox_component_note3.Location = new Point(90, 164);
            comboBox_component_note3.Name = "comboBox_component_note3";
            comboBox_component_note3.Size = new Size(162, 28);
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
            checkBox_play_note3.Location = new Point(271, 168);
            checkBox_play_note3.Name = "checkBox_play_note3";
            checkBox_play_note3.Size = new Size(111, 24);
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
            label_note4.Location = new Point(28, 208);
            label_note4.Name = "label_note4";
            label_note4.Size = new Size(56, 20);
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
            comboBox_component_note4.Location = new Point(90, 205);
            comboBox_component_note4.Name = "comboBox_component_note4";
            comboBox_component_note4.Size = new Size(162, 28);
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
            checkBox_play_note4.Location = new Point(271, 209);
            checkBox_play_note4.Name = "checkBox_play_note4";
            checkBox_play_note4.Size = new Size(111, 24);
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
            button_export_as_gcode.Location = new Point(123, 248);
            button_export_as_gcode.Name = "button_export_as_gcode";
            button_export_as_gcode.Size = new Size(164, 39);
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
            label5.Location = new Point(28, 28);
            label5.MaximumSize = new Size(356, 0);
            label5.Name = "label5";
            label5.Size = new Size(338, 40);
            label5.TabIndex = 4;
            label5.Text = "Select the notes and component type you want to play on the 3D printer/CNC machine.";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ConvertToGCode
            // 
            AutoScaleDimensions = new SizeF(120F, 120F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(411, 299);
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
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConvertToGCode";
            ShowIcon = false;
            Text = "Convert File to GCode";
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
    }
}