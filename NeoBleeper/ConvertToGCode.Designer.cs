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
            resources.ApplyResources(label_note1, "label_note1");
            label_note1.Name = "label_note1";
            // 
            // comboBox_component_note1
            // 
            resources.ApplyResources(comboBox_component_note1, "comboBox_component_note1");
            comboBox_component_note1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note1.FormattingEnabled = true;
            comboBox_component_note1.Items.AddRange(new object[] { resources.GetString("comboBox_component_note1.Items"), resources.GetString("comboBox_component_note1.Items1") });
            comboBox_component_note1.Name = "comboBox_component_note1";
            comboBox_component_note1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // checkBox_play_note1
            // 
            resources.ApplyResources(checkBox_play_note1, "checkBox_play_note1");
            checkBox_play_note1.Checked = true;
            checkBox_play_note1.CheckState = CheckState.Checked;
            checkBox_play_note1.Name = "checkBox_play_note1";
            checkBox_play_note1.UseVisualStyleBackColor = true;
            checkBox_play_note1.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // label_note2
            // 
            resources.ApplyResources(label_note2, "label_note2");
            label_note2.Name = "label_note2";
            // 
            // comboBox_component_note2
            // 
            resources.ApplyResources(comboBox_component_note2, "comboBox_component_note2");
            comboBox_component_note2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note2.FormattingEnabled = true;
            comboBox_component_note2.Items.AddRange(new object[] { resources.GetString("comboBox_component_note2.Items"), resources.GetString("comboBox_component_note2.Items1") });
            comboBox_component_note2.Name = "comboBox_component_note2";
            comboBox_component_note2.SelectedIndexChanged += comboBox_component_note2_SelectedIndexChanged;
            // 
            // checkBox_play_note2
            // 
            resources.ApplyResources(checkBox_play_note2, "checkBox_play_note2");
            checkBox_play_note2.Checked = true;
            checkBox_play_note2.CheckState = CheckState.Checked;
            checkBox_play_note2.Name = "checkBox_play_note2";
            checkBox_play_note2.UseVisualStyleBackColor = true;
            checkBox_play_note2.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // label_note3
            // 
            resources.ApplyResources(label_note3, "label_note3");
            label_note3.Name = "label_note3";
            // 
            // comboBox_component_note3
            // 
            resources.ApplyResources(comboBox_component_note3, "comboBox_component_note3");
            comboBox_component_note3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note3.FormattingEnabled = true;
            comboBox_component_note3.Items.AddRange(new object[] { resources.GetString("comboBox_component_note3.Items"), resources.GetString("comboBox_component_note3.Items1") });
            comboBox_component_note3.Name = "comboBox_component_note3";
            comboBox_component_note3.SelectedIndexChanged += comboBox_component_note3_SelectedIndexChanged;
            // 
            // checkBox_play_note3
            // 
            resources.ApplyResources(checkBox_play_note3, "checkBox_play_note3");
            checkBox_play_note3.Checked = true;
            checkBox_play_note3.CheckState = CheckState.Checked;
            checkBox_play_note3.Name = "checkBox_play_note3";
            checkBox_play_note3.UseVisualStyleBackColor = true;
            checkBox_play_note3.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // label_note4
            // 
            resources.ApplyResources(label_note4, "label_note4");
            label_note4.Name = "label_note4";
            // 
            // comboBox_component_note4
            // 
            resources.ApplyResources(comboBox_component_note4, "comboBox_component_note4");
            comboBox_component_note4.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_component_note4.FormattingEnabled = true;
            comboBox_component_note4.Items.AddRange(new object[] { resources.GetString("comboBox_component_note4.Items"), resources.GetString("comboBox_component_note4.Items1") });
            comboBox_component_note4.Name = "comboBox_component_note4";
            comboBox_component_note4.SelectedIndexChanged += comboBox_component_note4_SelectedIndexChanged;
            // 
            // checkBox_play_note4
            // 
            resources.ApplyResources(checkBox_play_note4, "checkBox_play_note4");
            checkBox_play_note4.Checked = true;
            checkBox_play_note4.CheckState = CheckState.Checked;
            checkBox_play_note4.Name = "checkBox_play_note4";
            checkBox_play_note4.UseVisualStyleBackColor = true;
            checkBox_play_note4.CheckedChanged += checkBoxes_CheckedChanged;
            // 
            // button_export_as_gcode
            // 
            resources.ApplyResources(button_export_as_gcode, "button_export_as_gcode");
            button_export_as_gcode.ImageList = icons;
            button_export_as_gcode.Name = "button_export_as_gcode";
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
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // exportGCodeFile
            // 
            resources.ApplyResources(exportGCodeFile, "exportGCodeFile");
            // 
            // lbl_alternating_note_options
            // 
            resources.ApplyResources(lbl_alternating_note_options, "lbl_alternating_note_options");
            lbl_alternating_note_options.Name = "lbl_alternating_note_options";
            // 
            // radioButtonPlay_alternating_notes2
            // 
            resources.ApplyResources(radioButtonPlay_alternating_notes2, "radioButtonPlay_alternating_notes2");
            radioButtonPlay_alternating_notes2.Name = "radioButtonPlay_alternating_notes2";
            radioButtonPlay_alternating_notes2.UseVisualStyleBackColor = true;
            // 
            // radioButtonPlay_alternating_notes1
            // 
            resources.ApplyResources(radioButtonPlay_alternating_notes1, "radioButtonPlay_alternating_notes1");
            radioButtonPlay_alternating_notes1.Checked = true;
            radioButtonPlay_alternating_notes1.Name = "radioButtonPlay_alternating_notes1";
            radioButtonPlay_alternating_notes1.TabStop = true;
            radioButtonPlay_alternating_notes1.UseVisualStyleBackColor = true;
            // 
            // ConvertToGCode
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
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
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConvertToGCode";
            ShowIcon = false;
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