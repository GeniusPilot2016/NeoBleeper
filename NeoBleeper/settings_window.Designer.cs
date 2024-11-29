namespace NeoBleeper
{
    partial class settings_window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(settings_window));
            tabControl_settings = new TabControl();
            general_settings = new TabPage();
            groupBox_system_speaker_test = new GroupBox();
            btn_test_system_speaker = new Button();
            imageList_settings = new ImageList(components);
            label_test_system_speaker_message = new Label();
            groupBox_appearance = new GroupBox();
            lbl_theme = new Label();
            comboBox_theme = new ComboBox();
            label_test_system_speaker_message_3 = new Label();
            label_test_system_speaker_message_2 = new Label();
            creating_sound_settings = new TabPage();
            button_show_reason = new Button();
            label_create_beep_from_soundcard_automatically_activated_message_2 = new Label();
            label_create_beep_from_soundcard_automatically_activated_message_1 = new Label();
            group_fmod_settings = new GroupBox();
            checkBox1 = new CheckBox();
            group_fmod_waveform = new GroupBox();
            radioButton_noise = new RadioButton();
            radioButton_triangle = new RadioButton();
            radioButton_sine = new RadioButton();
            radioButton_square = new RadioButton();
            checkBox_enable_create_beep_from_soundcard = new CheckBox();
            devices_settings = new TabPage();
            groupBox1 = new GroupBox();
            label_motor_speed_mod = new Label();
            checkBox_use_motor_speed_mod = new CheckBox();
            trackBar_motor_octave = new TrackBar();
            group_midi_output_devices = new GroupBox();
            comboBox_midi_output = new ComboBox();
            label1 = new Label();
            checkBox_midi_output = new CheckBox();
            group_midi_input_devices = new GroupBox();
            comboBox_midi_input_devices = new ComboBox();
            label_midi_input_device = new Label();
            checkBox_use_midi_input = new CheckBox();
            toolTip1 = new ToolTip(components);
            tabControl_settings.SuspendLayout();
            general_settings.SuspendLayout();
            groupBox_system_speaker_test.SuspendLayout();
            groupBox_appearance.SuspendLayout();
            creating_sound_settings.SuspendLayout();
            group_fmod_settings.SuspendLayout();
            group_fmod_waveform.SuspendLayout();
            devices_settings.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_motor_octave).BeginInit();
            group_midi_output_devices.SuspendLayout();
            group_midi_input_devices.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl_settings
            // 
            tabControl_settings.Controls.Add(general_settings);
            tabControl_settings.Controls.Add(creating_sound_settings);
            tabControl_settings.Controls.Add(devices_settings);
            resources.ApplyResources(tabControl_settings, "tabControl_settings");
            tabControl_settings.ImageList = imageList_settings;
            tabControl_settings.Name = "tabControl_settings";
            tabControl_settings.SelectedIndex = 0;
            tabControl_settings.SelectedIndexChanged += tabControl_settings_SelectedIndexChanged;
            // 
            // general_settings
            // 
            general_settings.Controls.Add(groupBox_system_speaker_test);
            general_settings.Controls.Add(groupBox_appearance);
            general_settings.Controls.Add(label_test_system_speaker_message_3);
            general_settings.Controls.Add(label_test_system_speaker_message_2);
            resources.ApplyResources(general_settings, "general_settings");
            general_settings.Name = "general_settings";
            general_settings.UseVisualStyleBackColor = true;
            general_settings.Click += general_settings_Click;
            // 
            // groupBox_system_speaker_test
            // 
            resources.ApplyResources(groupBox_system_speaker_test, "groupBox_system_speaker_test");
            groupBox_system_speaker_test.Controls.Add(btn_test_system_speaker);
            groupBox_system_speaker_test.Controls.Add(label_test_system_speaker_message);
            groupBox_system_speaker_test.Name = "groupBox_system_speaker_test";
            groupBox_system_speaker_test.TabStop = false;
            groupBox_system_speaker_test.Enter += groupBox_system_speaker_test_Enter;
            // 
            // btn_test_system_speaker
            // 
            resources.ApplyResources(btn_test_system_speaker, "btn_test_system_speaker");
            btn_test_system_speaker.ImageList = imageList_settings;
            btn_test_system_speaker.Name = "btn_test_system_speaker";
            toolTip1.SetToolTip(btn_test_system_speaker, resources.GetString("btn_test_system_speaker.ToolTip"));
            btn_test_system_speaker.UseVisualStyleBackColor = true;
            btn_test_system_speaker.Click += btn_test_system_speaker_Click;
            // 
            // imageList_settings
            // 
            imageList_settings.ColorDepth = ColorDepth.Depth32Bit;
            imageList_settings.ImageStream = (ImageListStreamer)resources.GetObject("imageList_settings.ImageStream");
            imageList_settings.TransparentColor = Color.Transparent;
            imageList_settings.Images.SetKeyName(0, "icons8-wrench-48.png");
            imageList_settings.Images.SetKeyName(1, "icons8-usb-connected-48.png");
            imageList_settings.Images.SetKeyName(2, "icons8-wave-48.png");
            imageList_settings.Images.SetKeyName(3, "icons8-test-48.png");
            imageList_settings.Images.SetKeyName(4, "icons8-square-wave-48.png");
            imageList_settings.Images.SetKeyName(5, "icons8-motor-48.png");
            imageList_settings.Images.SetKeyName(6, "icons8-piano-48.png");
            imageList_settings.Images.SetKeyName(7, "icons8-motor-48.png");
            // 
            // label_test_system_speaker_message
            // 
            resources.ApplyResources(label_test_system_speaker_message, "label_test_system_speaker_message");
            label_test_system_speaker_message.Name = "label_test_system_speaker_message";
            // 
            // groupBox_appearance
            // 
            resources.ApplyResources(groupBox_appearance, "groupBox_appearance");
            groupBox_appearance.Controls.Add(lbl_theme);
            groupBox_appearance.Controls.Add(comboBox_theme);
            groupBox_appearance.Name = "groupBox_appearance";
            groupBox_appearance.TabStop = false;
            // 
            // lbl_theme
            // 
            resources.ApplyResources(lbl_theme, "lbl_theme");
            lbl_theme.Name = "lbl_theme";
            lbl_theme.Click += lbl_theme_Click;
            // 
            // comboBox_theme
            // 
            resources.ApplyResources(comboBox_theme, "comboBox_theme");
            comboBox_theme.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_theme.FormattingEnabled = true;
            comboBox_theme.Items.AddRange(new object[] { resources.GetString("comboBox_theme.Items"), resources.GetString("comboBox_theme.Items1"), resources.GetString("comboBox_theme.Items2") });
            comboBox_theme.Name = "comboBox_theme";
            toolTip1.SetToolTip(comboBox_theme, resources.GetString("comboBox_theme.ToolTip"));
            comboBox_theme.SelectedIndexChanged += comboBox_theme_SelectedIndexChanged;
            // 
            // label_test_system_speaker_message_3
            // 
            resources.ApplyResources(label_test_system_speaker_message_3, "label_test_system_speaker_message_3");
            label_test_system_speaker_message_3.ForeColor = Color.FromArgb(255, 128, 0);
            label_test_system_speaker_message_3.Name = "label_test_system_speaker_message_3";
            label_test_system_speaker_message_3.Click += label_test_system_speaker_message_2_Click;
            // 
            // label_test_system_speaker_message_2
            // 
            resources.ApplyResources(label_test_system_speaker_message_2, "label_test_system_speaker_message_2");
            label_test_system_speaker_message_2.ForeColor = Color.FromArgb(192, 0, 0);
            label_test_system_speaker_message_2.Name = "label_test_system_speaker_message_2";
            label_test_system_speaker_message_2.Click += label_test_system_speaker_message_2_Click;
            // 
            // creating_sound_settings
            // 
            creating_sound_settings.Controls.Add(button_show_reason);
            creating_sound_settings.Controls.Add(label_create_beep_from_soundcard_automatically_activated_message_2);
            creating_sound_settings.Controls.Add(label_create_beep_from_soundcard_automatically_activated_message_1);
            creating_sound_settings.Controls.Add(group_fmod_settings);
            creating_sound_settings.Controls.Add(checkBox_enable_create_beep_from_soundcard);
            resources.ApplyResources(creating_sound_settings, "creating_sound_settings");
            creating_sound_settings.Name = "creating_sound_settings";
            creating_sound_settings.UseVisualStyleBackColor = true;
            // 
            // button_show_reason
            // 
            resources.ApplyResources(button_show_reason, "button_show_reason");
            button_show_reason.Name = "button_show_reason";
            button_show_reason.UseVisualStyleBackColor = true;
            button_show_reason.Click += button1_Click;
            // 
            // label_create_beep_from_soundcard_automatically_activated_message_2
            // 
            resources.ApplyResources(label_create_beep_from_soundcard_automatically_activated_message_2, "label_create_beep_from_soundcard_automatically_activated_message_2");
            label_create_beep_from_soundcard_automatically_activated_message_2.Name = "label_create_beep_from_soundcard_automatically_activated_message_2";
            // 
            // label_create_beep_from_soundcard_automatically_activated_message_1
            // 
            resources.ApplyResources(label_create_beep_from_soundcard_automatically_activated_message_1, "label_create_beep_from_soundcard_automatically_activated_message_1");
            label_create_beep_from_soundcard_automatically_activated_message_1.Name = "label_create_beep_from_soundcard_automatically_activated_message_1";
            // 
            // group_fmod_settings
            // 
            resources.ApplyResources(group_fmod_settings, "group_fmod_settings");
            group_fmod_settings.Controls.Add(checkBox1);
            group_fmod_settings.Controls.Add(group_fmod_waveform);
            group_fmod_settings.Name = "group_fmod_settings";
            group_fmod_settings.TabStop = false;
            // 
            // checkBox1
            // 
            resources.ApplyResources(checkBox1, "checkBox1");
            checkBox1.Name = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // group_fmod_waveform
            // 
            resources.ApplyResources(group_fmod_waveform, "group_fmod_waveform");
            group_fmod_waveform.Controls.Add(radioButton_noise);
            group_fmod_waveform.Controls.Add(radioButton_triangle);
            group_fmod_waveform.Controls.Add(radioButton_sine);
            group_fmod_waveform.Controls.Add(radioButton_square);
            group_fmod_waveform.Name = "group_fmod_waveform";
            group_fmod_waveform.TabStop = false;
            // 
            // radioButton_noise
            // 
            resources.ApplyResources(radioButton_noise, "radioButton_noise");
            radioButton_noise.Name = "radioButton_noise";
            radioButton_noise.UseVisualStyleBackColor = true;
            // 
            // radioButton_triangle
            // 
            resources.ApplyResources(radioButton_triangle, "radioButton_triangle");
            radioButton_triangle.Name = "radioButton_triangle";
            radioButton_triangle.UseVisualStyleBackColor = true;
            // 
            // radioButton_sine
            // 
            resources.ApplyResources(radioButton_sine, "radioButton_sine");
            radioButton_sine.Name = "radioButton_sine";
            radioButton_sine.UseVisualStyleBackColor = true;
            // 
            // radioButton_square
            // 
            resources.ApplyResources(radioButton_square, "radioButton_square");
            radioButton_square.Checked = true;
            radioButton_square.Name = "radioButton_square";
            radioButton_square.TabStop = true;
            radioButton_square.UseVisualStyleBackColor = true;
            radioButton_square.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // checkBox_enable_create_beep_from_soundcard
            // 
            resources.ApplyResources(checkBox_enable_create_beep_from_soundcard, "checkBox_enable_create_beep_from_soundcard");
            checkBox_enable_create_beep_from_soundcard.ImageList = imageList_settings;
            checkBox_enable_create_beep_from_soundcard.Name = "checkBox_enable_create_beep_from_soundcard";
            toolTip1.SetToolTip(checkBox_enable_create_beep_from_soundcard, resources.GetString("checkBox_enable_create_beep_from_soundcard.ToolTip"));
            checkBox_enable_create_beep_from_soundcard.UseVisualStyleBackColor = true;
            checkBox_enable_create_beep_from_soundcard.CheckedChanged += checkBox_enable_create_beep_from_soundcard_CheckedChanged;
            // 
            // devices_settings
            // 
            devices_settings.Controls.Add(groupBox1);
            devices_settings.Controls.Add(group_midi_output_devices);
            devices_settings.Controls.Add(checkBox_midi_output);
            devices_settings.Controls.Add(group_midi_input_devices);
            devices_settings.Controls.Add(checkBox_use_midi_input);
            resources.ApplyResources(devices_settings, "devices_settings");
            devices_settings.Name = "devices_settings";
            devices_settings.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(label_motor_speed_mod);
            groupBox1.Controls.Add(checkBox_use_motor_speed_mod);
            groupBox1.Controls.Add(trackBar_motor_octave);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // label_motor_speed_mod
            // 
            resources.ApplyResources(label_motor_speed_mod, "label_motor_speed_mod");
            label_motor_speed_mod.Name = "label_motor_speed_mod";
            // 
            // checkBox_use_motor_speed_mod
            // 
            resources.ApplyResources(checkBox_use_motor_speed_mod, "checkBox_use_motor_speed_mod");
            checkBox_use_motor_speed_mod.ImageList = imageList_settings;
            checkBox_use_motor_speed_mod.Name = "checkBox_use_motor_speed_mod";
            checkBox_use_motor_speed_mod.UseVisualStyleBackColor = true;
            // 
            // trackBar_motor_octave
            // 
            resources.ApplyResources(trackBar_motor_octave, "trackBar_motor_octave");
            trackBar_motor_octave.BackColor = Color.FromArgb(249, 248, 249);
            trackBar_motor_octave.Maximum = 5;
            trackBar_motor_octave.Name = "trackBar_motor_octave";
            trackBar_motor_octave.Value = 2;
            // 
            // group_midi_output_devices
            // 
            resources.ApplyResources(group_midi_output_devices, "group_midi_output_devices");
            group_midi_output_devices.Controls.Add(comboBox_midi_output);
            group_midi_output_devices.Controls.Add(label1);
            group_midi_output_devices.Name = "group_midi_output_devices";
            group_midi_output_devices.TabStop = false;
            // 
            // comboBox_midi_output
            // 
            resources.ApplyResources(comboBox_midi_output, "comboBox_midi_output");
            comboBox_midi_output.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_output.FormattingEnabled = true;
            comboBox_midi_output.Name = "comboBox_midi_output";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // checkBox_midi_output
            // 
            resources.ApplyResources(checkBox_midi_output, "checkBox_midi_output");
            checkBox_midi_output.ImageList = imageList_settings;
            checkBox_midi_output.Name = "checkBox_midi_output";
            checkBox_midi_output.UseVisualStyleBackColor = true;
            // 
            // group_midi_input_devices
            // 
            resources.ApplyResources(group_midi_input_devices, "group_midi_input_devices");
            group_midi_input_devices.Controls.Add(comboBox_midi_input_devices);
            group_midi_input_devices.Controls.Add(label_midi_input_device);
            group_midi_input_devices.Name = "group_midi_input_devices";
            group_midi_input_devices.TabStop = false;
            // 
            // comboBox_midi_input_devices
            // 
            resources.ApplyResources(comboBox_midi_input_devices, "comboBox_midi_input_devices");
            comboBox_midi_input_devices.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_input_devices.FormattingEnabled = true;
            comboBox_midi_input_devices.Name = "comboBox_midi_input_devices";
            // 
            // label_midi_input_device
            // 
            resources.ApplyResources(label_midi_input_device, "label_midi_input_device");
            label_midi_input_device.Name = "label_midi_input_device";
            // 
            // checkBox_use_midi_input
            // 
            resources.ApplyResources(checkBox_use_midi_input, "checkBox_use_midi_input");
            checkBox_use_midi_input.ImageList = imageList_settings;
            checkBox_use_midi_input.Name = "checkBox_use_midi_input";
            checkBox_use_midi_input.UseVisualStyleBackColor = true;
            // 
            // settings_window
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(tabControl_settings);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "settings_window";
            ShowIcon = false;
            ShowInTaskbar = false;
            FormClosing += settings_window_FormClosing;
            FormClosed += settings_window_FormClosed;
            Load += settings_window_Load;
            tabControl_settings.ResumeLayout(false);
            general_settings.ResumeLayout(false);
            general_settings.PerformLayout();
            groupBox_system_speaker_test.ResumeLayout(false);
            groupBox_system_speaker_test.PerformLayout();
            groupBox_appearance.ResumeLayout(false);
            groupBox_appearance.PerformLayout();
            creating_sound_settings.ResumeLayout(false);
            creating_sound_settings.PerformLayout();
            group_fmod_settings.ResumeLayout(false);
            group_fmod_settings.PerformLayout();
            group_fmod_waveform.ResumeLayout(false);
            group_fmod_waveform.PerformLayout();
            devices_settings.ResumeLayout(false);
            devices_settings.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_motor_octave).EndInit();
            group_midi_output_devices.ResumeLayout(false);
            group_midi_output_devices.PerformLayout();
            group_midi_input_devices.ResumeLayout(false);
            group_midi_input_devices.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl_settings;
        private TabPage general_settings;
        private TabPage creating_sound_settings;
        private TabPage devices_settings;
        private ComboBox comboBox_theme;
        private Label lbl_theme;
        private CheckBox checkBox_use_midi_input;
        private GroupBox group_fmod_settings;
        private GroupBox group_fmod_waveform;
        private RadioButton radioButton_square;
        private RadioButton radioButton_noise;
        private RadioButton radioButton_triangle;
        private RadioButton radioButton_sine;
        private CheckBox checkBox1;
        private GroupBox group_midi_input_devices;
        private ComboBox comboBox_midi_input_devices;
        private Label label_midi_input_device;
        private CheckBox checkBox_midi_output;
        private GroupBox group_midi_output_devices;
        private ComboBox comboBox_midi_output;
        private Label label1;
        private ToolTip toolTip1;
        private GroupBox groupBox_appearance;
        public CheckBox checkBox_enable_create_beep_from_soundcard;
        public Label label_test_system_speaker_message;
        public Label label_test_system_speaker_message_2;
        public GroupBox groupBox_system_speaker_test;
        public Label label_create_beep_from_soundcard_automatically_activated_message_1;
        public Button button_show_reason;
        private Button btn_test_system_speaker;
        private TrackBar trackBar_motor_octave;
        private Label label_motor_speed_mod;
        private CheckBox checkBox_use_motor_speed_mod;
        private GroupBox groupBox1;
        private ImageList imageList_settings;
        public Label label_create_beep_from_soundcard_automatically_activated_message_2;
        public Label label_test_system_speaker_message_3;
    }
}