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
            label_test_system_speaker_message_2 = new Label();
            label_test_system_speaker_message_3 = new Label();
            creating_sound_settings = new TabPage();
            group_fmod_settings = new GroupBox();
            label_create_beep_from_soundcard_automatically_activated_message_2 = new Label();
            label_create_beep_from_soundcard_automatically_activated_message_1 = new Label();
            button_show_reason = new Button();
            group_fmod_waveform = new GroupBox();
            radioButton_noise = new RadioButton();
            checkBox1 = new CheckBox();
            radioButton_triangle = new RadioButton();
            radioButton_sine = new RadioButton();
            radioButton_square = new RadioButton();
            checkBox_enable_create_beep_from_soundcard = new CheckBox();
            devices_settings = new TabPage();
            groupBox1 = new GroupBox();
            groupBox5 = new GroupBox();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            label_motor_speed_mod = new Label();
            checkBox_use_motor_speed_mod = new CheckBox();
            trackBar_motor_octave = new TrackBar();
            group_midi_output_devices = new GroupBox();
            comboBox_midi_output_instrument = new ComboBox();
            comboBox_midi_output_channel = new ComboBox();
            label_instrument = new Label();
            label_channel = new Label();
            refresh_midi_output_button = new Button();
            comboBox_midi_output_devices = new ComboBox();
            label_midi_output_device = new Label();
            checkBox_use_midi_output = new CheckBox();
            group_midi_input_devices = new GroupBox();
            refresh_midi_input_button = new Button();
            comboBox_midi_input_devices = new ComboBox();
            label_midi_input_device = new Label();
            checkBox_use_midi_input = new CheckBox();
            appearance = new TabPage();
            reset_colors = new Button();
            groupBox4 = new GroupBox();
            note_indicator_color_change = new Button();
            label11 = new Label();
            beep_indicator_color_change = new Button();
            label12 = new Label();
            beep_indicator_color = new Panel();
            note_indicator_color = new Panel();
            groupBox3 = new GroupBox();
            metronome_color_change = new Button();
            playback_buttons_color_change = new Button();
            erase_whole_line_color_change = new Button();
            unseelct_line_color_change = new Button();
            clear_notes_color_change = new Button();
            blank_line_color_change = new Button();
            metronome_color = new Panel();
            playback_buttons_color = new Panel();
            erase_whole_line_color = new Panel();
            unselect_line_color = new Panel();
            clear_notes_color = new Panel();
            blank_line_color = new Panel();
            label10 = new Label();
            label8 = new Label();
            label9 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            groupBox2 = new GroupBox();
            third_octave_color_change = new Button();
            second_octave_color_change = new Button();
            first_octave_color_change = new Button();
            third_octave_color = new Panel();
            second_octave_color = new Panel();
            first_octave_color = new Panel();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            toolTip1 = new ToolTip(components);
            colorDialog1 = new ColorDialog();
            tabControl_settings.SuspendLayout();
            general_settings.SuspendLayout();
            groupBox_system_speaker_test.SuspendLayout();
            groupBox_appearance.SuspendLayout();
            creating_sound_settings.SuspendLayout();
            group_fmod_settings.SuspendLayout();
            group_fmod_waveform.SuspendLayout();
            devices_settings.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_motor_octave).BeginInit();
            group_midi_output_devices.SuspendLayout();
            group_midi_input_devices.SuspendLayout();
            appearance.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl_settings
            // 
            tabControl_settings.Controls.Add(general_settings);
            tabControl_settings.Controls.Add(creating_sound_settings);
            tabControl_settings.Controls.Add(devices_settings);
            tabControl_settings.Controls.Add(appearance);
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
            general_settings.Controls.Add(label_test_system_speaker_message_2);
            general_settings.Controls.Add(label_test_system_speaker_message_3);
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
            imageList_settings.Images.SetKeyName(8, "icons8-brush-48.png");
            imageList_settings.Images.SetKeyName(9, "icons8-reset-48.png");
            imageList_settings.Images.SetKeyName(10, "icons8-refresh-48 (1).png");
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
            // label_test_system_speaker_message_2
            // 
            resources.ApplyResources(label_test_system_speaker_message_2, "label_test_system_speaker_message_2");
            label_test_system_speaker_message_2.ForeColor = Color.FromArgb(192, 0, 0);
            label_test_system_speaker_message_2.Name = "label_test_system_speaker_message_2";
            // 
            // label_test_system_speaker_message_3
            // 
            resources.ApplyResources(label_test_system_speaker_message_3, "label_test_system_speaker_message_3");
            label_test_system_speaker_message_3.ForeColor = Color.FromArgb(255, 128, 0);
            label_test_system_speaker_message_3.Name = "label_test_system_speaker_message_3";
            // 
            // creating_sound_settings
            // 
            creating_sound_settings.Controls.Add(group_fmod_settings);
            creating_sound_settings.Controls.Add(checkBox_enable_create_beep_from_soundcard);
            resources.ApplyResources(creating_sound_settings, "creating_sound_settings");
            creating_sound_settings.Name = "creating_sound_settings";
            creating_sound_settings.UseVisualStyleBackColor = true;
            // 
            // group_fmod_settings
            // 
            resources.ApplyResources(group_fmod_settings, "group_fmod_settings");
            group_fmod_settings.Controls.Add(label_create_beep_from_soundcard_automatically_activated_message_2);
            group_fmod_settings.Controls.Add(label_create_beep_from_soundcard_automatically_activated_message_1);
            group_fmod_settings.Controls.Add(button_show_reason);
            group_fmod_settings.Controls.Add(group_fmod_waveform);
            group_fmod_settings.Name = "group_fmod_settings";
            group_fmod_settings.TabStop = false;
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
            // button_show_reason
            // 
            resources.ApplyResources(button_show_reason, "button_show_reason");
            button_show_reason.Name = "button_show_reason";
            button_show_reason.UseVisualStyleBackColor = true;
            button_show_reason.Click += button1_Click;
            // 
            // group_fmod_waveform
            // 
            resources.ApplyResources(group_fmod_waveform, "group_fmod_waveform");
            group_fmod_waveform.Controls.Add(radioButton_noise);
            group_fmod_waveform.Controls.Add(checkBox1);
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
            // checkBox1
            // 
            resources.ApplyResources(checkBox1, "checkBox1");
            checkBox1.Name = "checkBox1";
            checkBox1.UseVisualStyleBackColor = true;
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
            devices_settings.Controls.Add(checkBox_use_midi_output);
            devices_settings.Controls.Add(group_midi_input_devices);
            devices_settings.Controls.Add(checkBox_use_midi_input);
            resources.ApplyResources(devices_settings, "devices_settings");
            devices_settings.Name = "devices_settings";
            devices_settings.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(groupBox1, "groupBox1");
            groupBox1.Controls.Add(groupBox5);
            groupBox1.Controls.Add(label_motor_speed_mod);
            groupBox1.Controls.Add(checkBox_use_motor_speed_mod);
            groupBox1.Controls.Add(trackBar_motor_octave);
            groupBox1.Name = "groupBox1";
            groupBox1.TabStop = false;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(radioButton2);
            groupBox5.Controls.Add(radioButton1);
            resources.ApplyResources(groupBox5, "groupBox5");
            groupBox5.Name = "groupBox5";
            groupBox5.TabStop = false;
            // 
            // radioButton2
            // 
            resources.ApplyResources(radioButton2, "radioButton2");
            radioButton2.Name = "radioButton2";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            resources.ApplyResources(radioButton1, "radioButton1");
            radioButton1.Checked = true;
            radioButton1.Name = "radioButton1";
            radioButton1.TabStop = true;
            radioButton1.UseVisualStyleBackColor = true;
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
            group_midi_output_devices.Controls.Add(comboBox_midi_output_instrument);
            group_midi_output_devices.Controls.Add(comboBox_midi_output_channel);
            group_midi_output_devices.Controls.Add(label_instrument);
            group_midi_output_devices.Controls.Add(label_channel);
            group_midi_output_devices.Controls.Add(refresh_midi_output_button);
            group_midi_output_devices.Controls.Add(comboBox_midi_output_devices);
            group_midi_output_devices.Controls.Add(label_midi_output_device);
            group_midi_output_devices.Name = "group_midi_output_devices";
            group_midi_output_devices.TabStop = false;
            // 
            // comboBox_midi_output_instrument
            // 
            comboBox_midi_output_instrument.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBox_midi_output_instrument, "comboBox_midi_output_instrument");
            comboBox_midi_output_instrument.FormattingEnabled = true;
            comboBox_midi_output_instrument.Name = "comboBox_midi_output_instrument";
            // 
            // comboBox_midi_output_channel
            // 
            comboBox_midi_output_channel.DropDownStyle = ComboBoxStyle.DropDownList;
            resources.ApplyResources(comboBox_midi_output_channel, "comboBox_midi_output_channel");
            comboBox_midi_output_channel.FormattingEnabled = true;
            comboBox_midi_output_channel.Name = "comboBox_midi_output_channel";
            // 
            // label_instrument
            // 
            resources.ApplyResources(label_instrument, "label_instrument");
            label_instrument.Name = "label_instrument";
            // 
            // label_channel
            // 
            resources.ApplyResources(label_channel, "label_channel");
            label_channel.Name = "label_channel";
            // 
            // refresh_midi_output_button
            // 
            resources.ApplyResources(refresh_midi_output_button, "refresh_midi_output_button");
            refresh_midi_output_button.ImageList = imageList_settings;
            refresh_midi_output_button.Name = "refresh_midi_output_button";
            refresh_midi_output_button.UseVisualStyleBackColor = true;
            refresh_midi_output_button.Click += refresh_midi_output_button_Click;
            // 
            // comboBox_midi_output_devices
            // 
            resources.ApplyResources(comboBox_midi_output_devices, "comboBox_midi_output_devices");
            comboBox_midi_output_devices.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_output_devices.FormattingEnabled = true;
            comboBox_midi_output_devices.Name = "comboBox_midi_output_devices";
            // 
            // label_midi_output_device
            // 
            resources.ApplyResources(label_midi_output_device, "label_midi_output_device");
            label_midi_output_device.Name = "label_midi_output_device";
            // 
            // checkBox_use_midi_output
            // 
            resources.ApplyResources(checkBox_use_midi_output, "checkBox_use_midi_output");
            checkBox_use_midi_output.ImageList = imageList_settings;
            checkBox_use_midi_output.Name = "checkBox_use_midi_output";
            checkBox_use_midi_output.UseVisualStyleBackColor = true;
            // 
            // group_midi_input_devices
            // 
            resources.ApplyResources(group_midi_input_devices, "group_midi_input_devices");
            group_midi_input_devices.Controls.Add(refresh_midi_input_button);
            group_midi_input_devices.Controls.Add(comboBox_midi_input_devices);
            group_midi_input_devices.Controls.Add(label_midi_input_device);
            group_midi_input_devices.Name = "group_midi_input_devices";
            group_midi_input_devices.TabStop = false;
            // 
            // refresh_midi_input_button
            // 
            resources.ApplyResources(refresh_midi_input_button, "refresh_midi_input_button");
            refresh_midi_input_button.ImageList = imageList_settings;
            refresh_midi_input_button.Name = "refresh_midi_input_button";
            refresh_midi_input_button.UseVisualStyleBackColor = true;
            refresh_midi_input_button.Click += refresh_midi_input_button_Click;
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
            // appearance
            // 
            appearance.Controls.Add(reset_colors);
            appearance.Controls.Add(groupBox4);
            appearance.Controls.Add(groupBox3);
            appearance.Controls.Add(groupBox2);
            resources.ApplyResources(appearance, "appearance");
            appearance.Name = "appearance";
            appearance.UseVisualStyleBackColor = true;
            // 
            // reset_colors
            // 
            resources.ApplyResources(reset_colors, "reset_colors");
            reset_colors.ImageList = imageList_settings;
            reset_colors.Name = "reset_colors";
            reset_colors.UseVisualStyleBackColor = true;
            reset_colors.Click += reset_colors_Click;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(note_indicator_color_change);
            groupBox4.Controls.Add(label11);
            groupBox4.Controls.Add(beep_indicator_color_change);
            groupBox4.Controls.Add(label12);
            groupBox4.Controls.Add(beep_indicator_color);
            groupBox4.Controls.Add(note_indicator_color);
            resources.ApplyResources(groupBox4, "groupBox4");
            groupBox4.Name = "groupBox4";
            groupBox4.TabStop = false;
            // 
            // note_indicator_color_change
            // 
            resources.ApplyResources(note_indicator_color_change, "note_indicator_color_change");
            note_indicator_color_change.Name = "note_indicator_color_change";
            note_indicator_color_change.UseVisualStyleBackColor = true;
            note_indicator_color_change.Click += note_indicator_color_change_Click;
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.Name = "label11";
            // 
            // beep_indicator_color_change
            // 
            resources.ApplyResources(beep_indicator_color_change, "beep_indicator_color_change");
            beep_indicator_color_change.Name = "beep_indicator_color_change";
            beep_indicator_color_change.UseVisualStyleBackColor = true;
            beep_indicator_color_change.Click += beep_indicator_color_change_Click;
            // 
            // label12
            // 
            resources.ApplyResources(label12, "label12");
            label12.Name = "label12";
            // 
            // beep_indicator_color
            // 
            beep_indicator_color.BackColor = Color.Red;
            beep_indicator_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(beep_indicator_color, "beep_indicator_color");
            beep_indicator_color.Name = "beep_indicator_color";
            // 
            // note_indicator_color
            // 
            note_indicator_color.BackColor = Color.Red;
            note_indicator_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(note_indicator_color, "note_indicator_color");
            note_indicator_color.Name = "note_indicator_color";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(metronome_color_change);
            groupBox3.Controls.Add(playback_buttons_color_change);
            groupBox3.Controls.Add(erase_whole_line_color_change);
            groupBox3.Controls.Add(unseelct_line_color_change);
            groupBox3.Controls.Add(clear_notes_color_change);
            groupBox3.Controls.Add(blank_line_color_change);
            groupBox3.Controls.Add(metronome_color);
            groupBox3.Controls.Add(playback_buttons_color);
            groupBox3.Controls.Add(erase_whole_line_color);
            groupBox3.Controls.Add(unselect_line_color);
            groupBox3.Controls.Add(clear_notes_color);
            groupBox3.Controls.Add(blank_line_color);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(label7);
            resources.ApplyResources(groupBox3, "groupBox3");
            groupBox3.Name = "groupBox3";
            groupBox3.TabStop = false;
            // 
            // metronome_color_change
            // 
            resources.ApplyResources(metronome_color_change, "metronome_color_change");
            metronome_color_change.Name = "metronome_color_change";
            metronome_color_change.UseVisualStyleBackColor = true;
            metronome_color_change.Click += metronome_color_change_Click;
            // 
            // playback_buttons_color_change
            // 
            resources.ApplyResources(playback_buttons_color_change, "playback_buttons_color_change");
            playback_buttons_color_change.Name = "playback_buttons_color_change";
            playback_buttons_color_change.UseVisualStyleBackColor = true;
            playback_buttons_color_change.Click += playback_buttons_color_change_Click;
            // 
            // erase_whole_line_color_change
            // 
            resources.ApplyResources(erase_whole_line_color_change, "erase_whole_line_color_change");
            erase_whole_line_color_change.Name = "erase_whole_line_color_change";
            erase_whole_line_color_change.UseVisualStyleBackColor = true;
            erase_whole_line_color_change.Click += erase_whole_line_color_change_Click;
            // 
            // unseelct_line_color_change
            // 
            resources.ApplyResources(unseelct_line_color_change, "unseelct_line_color_change");
            unseelct_line_color_change.Name = "unseelct_line_color_change";
            unseelct_line_color_change.UseVisualStyleBackColor = true;
            unseelct_line_color_change.Click += unseelct_line_color_change_Click;
            // 
            // clear_notes_color_change
            // 
            resources.ApplyResources(clear_notes_color_change, "clear_notes_color_change");
            clear_notes_color_change.Name = "clear_notes_color_change";
            clear_notes_color_change.UseVisualStyleBackColor = true;
            clear_notes_color_change.Click += clear_notes_color_change_Click;
            // 
            // blank_line_color_change
            // 
            resources.ApplyResources(blank_line_color_change, "blank_line_color_change");
            blank_line_color_change.Name = "blank_line_color_change";
            blank_line_color_change.UseVisualStyleBackColor = true;
            blank_line_color_change.Click += blank_line_color_change_Click;
            // 
            // metronome_color
            // 
            metronome_color.BackColor = Color.FromArgb(192, 255, 192);
            metronome_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(metronome_color, "metronome_color");
            metronome_color.Name = "metronome_color";
            // 
            // playback_buttons_color
            // 
            playback_buttons_color.BackColor = Color.FromArgb(128, 255, 128);
            playback_buttons_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(playback_buttons_color, "playback_buttons_color");
            playback_buttons_color.Name = "playback_buttons_color";
            // 
            // erase_whole_line_color
            // 
            erase_whole_line_color.BackColor = Color.FromArgb(255, 128, 128);
            erase_whole_line_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(erase_whole_line_color, "erase_whole_line_color");
            erase_whole_line_color.Name = "erase_whole_line_color";
            // 
            // unselect_line_color
            // 
            unselect_line_color.BackColor = Color.FromArgb(128, 255, 255);
            unselect_line_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(unselect_line_color, "unselect_line_color");
            unselect_line_color.Name = "unselect_line_color";
            // 
            // clear_notes_color
            // 
            clear_notes_color.BackColor = Color.FromArgb(128, 128, 255);
            clear_notes_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(clear_notes_color, "clear_notes_color");
            clear_notes_color.Name = "clear_notes_color";
            // 
            // blank_line_color
            // 
            blank_line_color.BackColor = Color.FromArgb(255, 224, 192);
            blank_line_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(blank_line_color, "blank_line_color");
            blank_line_color.Name = "blank_line_color";
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(third_octave_color_change);
            groupBox2.Controls.Add(second_octave_color_change);
            groupBox2.Controls.Add(first_octave_color_change);
            groupBox2.Controls.Add(third_octave_color);
            groupBox2.Controls.Add(second_octave_color);
            groupBox2.Controls.Add(first_octave_color);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            resources.ApplyResources(groupBox2, "groupBox2");
            groupBox2.Name = "groupBox2";
            groupBox2.TabStop = false;
            // 
            // third_octave_color_change
            // 
            resources.ApplyResources(third_octave_color_change, "third_octave_color_change");
            third_octave_color_change.Name = "third_octave_color_change";
            third_octave_color_change.UseVisualStyleBackColor = true;
            third_octave_color_change.Click += third_octave_color_change_Click;
            // 
            // second_octave_color_change
            // 
            resources.ApplyResources(second_octave_color_change, "second_octave_color_change");
            second_octave_color_change.Name = "second_octave_color_change";
            second_octave_color_change.UseVisualStyleBackColor = true;
            second_octave_color_change.Click += second_octave_color_change_Click;
            // 
            // first_octave_color_change
            // 
            resources.ApplyResources(first_octave_color_change, "first_octave_color_change");
            first_octave_color_change.Name = "first_octave_color_change";
            first_octave_color_change.UseVisualStyleBackColor = true;
            first_octave_color_change.Click += first_octave_color_change_Click;
            // 
            // third_octave_color
            // 
            third_octave_color.BackColor = Color.FromArgb(192, 255, 192);
            third_octave_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(third_octave_color, "third_octave_color");
            third_octave_color.Name = "third_octave_color";
            // 
            // second_octave_color
            // 
            second_octave_color.BackColor = Color.FromArgb(192, 192, 255);
            second_octave_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(second_octave_color, "second_octave_color");
            second_octave_color.Name = "second_octave_color";
            // 
            // first_octave_color
            // 
            first_octave_color.BackColor = Color.FromArgb(255, 224, 192);
            first_octave_color.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(first_octave_color, "first_octave_color");
            first_octave_color.Name = "first_octave_color";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
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
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_motor_octave).EndInit();
            group_midi_output_devices.ResumeLayout(false);
            group_midi_output_devices.PerformLayout();
            group_midi_input_devices.ResumeLayout(false);
            group_midi_input_devices.PerformLayout();
            appearance.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
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
        private CheckBox checkBox_use_midi_output;
        private GroupBox group_midi_output_devices;
        private ComboBox comboBox_midi_output_devices;
        private Label label_midi_output_device;
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
        private TabPage appearance;
        private GroupBox groupBox2;
        private Panel first_octave_color;
        private Label label2;
        private Button second_octave_color_change;
        private Button first_octave_color_change;
        private Panel second_octave_color;
        private Label label3;
        private Button third_octave_color_change;
        private Panel third_octave_color;
        private Label label4;
        private GroupBox groupBox3;
        private Button unseelct_line_color_change;
        private Button clear_notes_color_change;
        private Button blank_line_color_change;
        private Panel unselect_line_color;
        private Panel clear_notes_color;
        private Panel blank_line_color;
        private Label label5;
        private Label label6;
        private Label label7;
        private Button erase_whole_line_color_change;
        private Panel erase_whole_line_color;
        private Label label9;
        private Button playback_buttons_color_change;
        private Panel playback_buttons_color;
        private Label label8;
        private Button metronome_color_change;
        private Panel metronome_color;
        private Label label10;
        private GroupBox groupBox4;
        private Button note_indicator_color_change;
        private Label label11;
        private Button beep_indicator_color_change;
        private Label label12;
        private Panel beep_indicator_color;
        private Panel note_indicator_color;
        private GroupBox groupBox5;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private ColorDialog colorDialog1;
        private Button reset_colors;
        private Button refresh_midi_input_button;
        private Button refresh_midi_output_button;
        private ComboBox comboBox_midi_output_channel;
        private Label label_channel;
        private ComboBox comboBox_midi_output_instrument;
        private Label label_instrument;
    }
}