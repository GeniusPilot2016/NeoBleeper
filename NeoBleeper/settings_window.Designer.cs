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
            flowLayoutPanelGeneralSettings = new FlowLayoutPanel();
            groupBoxLanguageSettings = new GroupBox();
            comboBoxLanguage = new ComboBox();
            labelLanguage = new Label();
            groupBox_appearance = new GroupBox();
            checkBoxClassicBleeperMode = new CheckBox();
            lbl_theme = new Label();
            comboBox_theme = new ComboBox();
            groupBoxCreateMusicWithAI = new GroupBox();
            buttonResetAPIKey = new Button();
            imageList_settings = new ImageList(components);
            buttonUpdateAPIKey = new Button();
            buttonShowHide = new Button();
            labelAPIKeyWarning = new Label();
            labelAPIKey = new Label();
            textBoxAPIKey = new TextBox();
            groupBox_system_speaker_test = new GroupBox();
            btn_test_system_speaker = new Button();
            label_test_system_speaker_message = new Label();
            panelSystemSpeakerWarnings = new Panel();
            label_test_system_speaker_message_2 = new Label();
            label_test_system_speaker_message_3 = new Label();
            creating_sound_settings = new TabPage();
            flowLayoutPanelCreatingSoundSettings = new FlowLayoutPanel();
            checkBox_enable_create_beep_from_soundcard = new CheckBox();
            group_beep_creation_from_sound_card_settings = new GroupBox();
            flowLayoutPanelSoundDeviceBeepEnabledInfo = new FlowLayoutPanel();
            panel1 = new Panel();
            label_create_beep_from_soundcard_automatically_activated_message_1 = new Label();
            label_create_beep_from_soundcard_automatically_activated_message_2 = new Label();
            button_show_reason = new Button();
            group_tone_waveform = new GroupBox();
            radioButton_noise = new RadioButton();
            radioButton_triangle = new RadioButton();
            radioButton_sine = new RadioButton();
            radioButton_square = new RadioButton();
            devices_settings = new TabPage();
            flowLayoutPanelDevicesSettings = new FlowLayoutPanel();
            checkBox_use_midi_input = new CheckBox();
            group_midi_input_devices = new GroupBox();
            refresh_midi_input_button = new Button();
            comboBox_midi_input_devices = new ComboBox();
            label_midi_input_device = new Label();
            checkBox_use_midi_output = new CheckBox();
            group_midi_output_devices = new GroupBox();
            comboBox_midi_output_instrument = new ComboBox();
            comboBox_midi_output_channel = new ComboBox();
            label_instrument = new Label();
            label_channel = new Label();
            refresh_midi_output_button = new Button();
            comboBox_midi_output_devices = new ComboBox();
            label_midi_output_device = new Label();
            groupBox_other_devices = new GroupBox();
            button_get_firmware = new Button();
            label_firmware_warning = new Label();
            trackBar_stepper_motor_octave = new TrackBar();
            label_stepper_motor_octave = new Label();
            groupBox_type_of_device = new GroupBox();
            radioButtonDCMotorOrBuzzer = new RadioButton();
            radioButtonStepperMotor = new RadioButton();
            checkBox_use_microcontroller = new CheckBox();
            appearance = new TabPage();
            flowLayoutPanelAppearanceSettings = new FlowLayoutPanel();
            group_keyboard_colors = new GroupBox();
            third_octave_color_change = new Button();
            second_octave_color_change = new Button();
            first_octave_color_change = new Button();
            third_octave_color = new Panel();
            second_octave_color = new Panel();
            first_octave_color = new Panel();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            group_buttons_and_controls_colors = new GroupBox();
            markup_color_change = new Button();
            metronome_color_change = new Button();
            playback_buttons_color_change = new Button();
            erase_whole_line_color_change = new Button();
            unselect_line_color_change = new Button();
            clear_notes_color_change = new Button();
            blank_line_color_change = new Button();
            markup_color = new Panel();
            metronome_color = new Panel();
            playback_buttons_color = new Panel();
            erase_whole_line_color = new Panel();
            unselect_line_color = new Panel();
            clear_notes_color = new Panel();
            blank_line_color = new Panel();
            label1 = new Label();
            label10 = new Label();
            label8 = new Label();
            label9 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            group_indicator_colors = new GroupBox();
            note_indicator_color_change = new Button();
            beep_indicator_color_change = new Button();
            label12 = new Label();
            beep_indicator_color = new Panel();
            note_indicator_color = new Panel();
            label11 = new Label();
            group_lyrics_size_settings = new GroupBox();
            buttonPreviewLyrics = new Button();
            labelPt = new Label();
            numericUpDownLyricsSize = new NumericUpDown();
            label13 = new Label();
            panel2 = new Panel();
            reset_appearance_settings = new Button();
            toolTip1 = new ToolTip(components);
            colorDialog1 = new ColorDialog();
            tabControl_settings.SuspendLayout();
            general_settings.SuspendLayout();
            flowLayoutPanelGeneralSettings.SuspendLayout();
            groupBoxLanguageSettings.SuspendLayout();
            groupBox_appearance.SuspendLayout();
            groupBoxCreateMusicWithAI.SuspendLayout();
            groupBox_system_speaker_test.SuspendLayout();
            panelSystemSpeakerWarnings.SuspendLayout();
            creating_sound_settings.SuspendLayout();
            flowLayoutPanelCreatingSoundSettings.SuspendLayout();
            group_beep_creation_from_sound_card_settings.SuspendLayout();
            flowLayoutPanelSoundDeviceBeepEnabledInfo.SuspendLayout();
            panel1.SuspendLayout();
            group_tone_waveform.SuspendLayout();
            devices_settings.SuspendLayout();
            flowLayoutPanelDevicesSettings.SuspendLayout();
            group_midi_input_devices.SuspendLayout();
            group_midi_output_devices.SuspendLayout();
            groupBox_other_devices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_stepper_motor_octave).BeginInit();
            groupBox_type_of_device.SuspendLayout();
            appearance.SuspendLayout();
            flowLayoutPanelAppearanceSettings.SuspendLayout();
            group_keyboard_colors.SuspendLayout();
            group_buttons_and_controls_colors.SuspendLayout();
            group_indicator_colors.SuspendLayout();
            group_lyrics_size_settings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownLyricsSize).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl_settings
            // 
            resources.ApplyResources(tabControl_settings, "tabControl_settings");
            tabControl_settings.Controls.Add(general_settings);
            tabControl_settings.Controls.Add(creating_sound_settings);
            tabControl_settings.Controls.Add(devices_settings);
            tabControl_settings.Controls.Add(appearance);
            tabControl_settings.ImageList = imageList_settings;
            tabControl_settings.Name = "tabControl_settings";
            tabControl_settings.SelectedIndex = 0;
            toolTip1.SetToolTip(tabControl_settings, resources.GetString("tabControl_settings.ToolTip"));
            tabControl_settings.SelectedIndexChanged += tabControl_settings_SelectedIndexChanged;
            // 
            // general_settings
            // 
            resources.ApplyResources(general_settings, "general_settings");
            general_settings.Controls.Add(flowLayoutPanelGeneralSettings);
            general_settings.Name = "general_settings";
            toolTip1.SetToolTip(general_settings, resources.GetString("general_settings.ToolTip"));
            general_settings.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelGeneralSettings
            // 
            resources.ApplyResources(flowLayoutPanelGeneralSettings, "flowLayoutPanelGeneralSettings");
            flowLayoutPanelGeneralSettings.Controls.Add(groupBoxLanguageSettings);
            flowLayoutPanelGeneralSettings.Controls.Add(groupBox_appearance);
            flowLayoutPanelGeneralSettings.Controls.Add(groupBoxCreateMusicWithAI);
            flowLayoutPanelGeneralSettings.Controls.Add(groupBox_system_speaker_test);
            flowLayoutPanelGeneralSettings.Controls.Add(panelSystemSpeakerWarnings);
            flowLayoutPanelGeneralSettings.Name = "flowLayoutPanelGeneralSettings";
            toolTip1.SetToolTip(flowLayoutPanelGeneralSettings, resources.GetString("flowLayoutPanelGeneralSettings.ToolTip"));
            // 
            // groupBoxLanguageSettings
            // 
            resources.ApplyResources(groupBoxLanguageSettings, "groupBoxLanguageSettings");
            groupBoxLanguageSettings.Controls.Add(comboBoxLanguage);
            groupBoxLanguageSettings.Controls.Add(labelLanguage);
            groupBoxLanguageSettings.Name = "groupBoxLanguageSettings";
            groupBoxLanguageSettings.TabStop = false;
            toolTip1.SetToolTip(groupBoxLanguageSettings, resources.GetString("groupBoxLanguageSettings.ToolTip"));
            // 
            // comboBoxLanguage
            // 
            resources.ApplyResources(comboBoxLanguage, "comboBoxLanguage");
            comboBoxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Items.AddRange(new object[] { resources.GetString("comboBoxLanguage.Items"), resources.GetString("comboBoxLanguage.Items1"), resources.GetString("comboBoxLanguage.Items2"), resources.GetString("comboBoxLanguage.Items3"), resources.GetString("comboBoxLanguage.Items4"), resources.GetString("comboBoxLanguage.Items5"), resources.GetString("comboBoxLanguage.Items6"), resources.GetString("comboBoxLanguage.Items7"), resources.GetString("comboBoxLanguage.Items8") });
            comboBoxLanguage.Name = "comboBoxLanguage";
            toolTip1.SetToolTip(comboBoxLanguage, resources.GetString("comboBoxLanguage.ToolTip"));
            comboBoxLanguage.SelectedIndexChanged += comboBoxLanguage_SelectedIndexChanged;
            // 
            // labelLanguage
            // 
            resources.ApplyResources(labelLanguage, "labelLanguage");
            labelLanguage.Name = "labelLanguage";
            toolTip1.SetToolTip(labelLanguage, resources.GetString("labelLanguage.ToolTip"));
            // 
            // groupBox_appearance
            // 
            resources.ApplyResources(groupBox_appearance, "groupBox_appearance");
            groupBox_appearance.Controls.Add(checkBoxClassicBleeperMode);
            groupBox_appearance.Controls.Add(lbl_theme);
            groupBox_appearance.Controls.Add(comboBox_theme);
            groupBox_appearance.Name = "groupBox_appearance";
            groupBox_appearance.TabStop = false;
            toolTip1.SetToolTip(groupBox_appearance, resources.GetString("groupBox_appearance.ToolTip"));
            // 
            // checkBoxClassicBleeperMode
            // 
            resources.ApplyResources(checkBoxClassicBleeperMode, "checkBoxClassicBleeperMode");
            checkBoxClassicBleeperMode.Name = "checkBoxClassicBleeperMode";
            toolTip1.SetToolTip(checkBoxClassicBleeperMode, resources.GetString("checkBoxClassicBleeperMode.ToolTip"));
            checkBoxClassicBleeperMode.UseVisualStyleBackColor = true;
            checkBoxClassicBleeperMode.CheckedChanged += checkBoxClassicBleeperMode_CheckedChanged;
            // 
            // lbl_theme
            // 
            resources.ApplyResources(lbl_theme, "lbl_theme");
            lbl_theme.Name = "lbl_theme";
            toolTip1.SetToolTip(lbl_theme, resources.GetString("lbl_theme.ToolTip"));
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
            // groupBoxCreateMusicWithAI
            // 
            resources.ApplyResources(groupBoxCreateMusicWithAI, "groupBoxCreateMusicWithAI");
            groupBoxCreateMusicWithAI.Controls.Add(buttonResetAPIKey);
            groupBoxCreateMusicWithAI.Controls.Add(buttonUpdateAPIKey);
            groupBoxCreateMusicWithAI.Controls.Add(buttonShowHide);
            groupBoxCreateMusicWithAI.Controls.Add(labelAPIKeyWarning);
            groupBoxCreateMusicWithAI.Controls.Add(labelAPIKey);
            groupBoxCreateMusicWithAI.Controls.Add(textBoxAPIKey);
            groupBoxCreateMusicWithAI.Name = "groupBoxCreateMusicWithAI";
            groupBoxCreateMusicWithAI.TabStop = false;
            toolTip1.SetToolTip(groupBoxCreateMusicWithAI, resources.GetString("groupBoxCreateMusicWithAI.ToolTip"));
            // 
            // buttonResetAPIKey
            // 
            resources.ApplyResources(buttonResetAPIKey, "buttonResetAPIKey");
            buttonResetAPIKey.ImageList = imageList_settings;
            buttonResetAPIKey.Name = "buttonResetAPIKey";
            toolTip1.SetToolTip(buttonResetAPIKey, resources.GetString("buttonResetAPIKey.ToolTip"));
            buttonResetAPIKey.UseVisualStyleBackColor = true;
            buttonResetAPIKey.Click += buttonResetAPIKey_Click;
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
            imageList_settings.Images.SetKeyName(11, "icons8-warning-48.png");
            imageList_settings.Images.SetKeyName(12, "icons8-mark-view-as-non-hidden-48.png");
            imageList_settings.Images.SetKeyName(13, "icons8-mark-view-as-hidden-48.png");
            imageList_settings.Images.SetKeyName(14, "icons8-update-48.png");
            imageList_settings.Images.SetKeyName(15, "icons8-preview-48.png");
            imageList_settings.Images.SetKeyName(16, "icons8-manual-48.png");
            imageList_settings.Images.SetKeyName(17, "icons8-bios-48.png");
            // 
            // buttonUpdateAPIKey
            // 
            resources.ApplyResources(buttonUpdateAPIKey, "buttonUpdateAPIKey");
            buttonUpdateAPIKey.ImageList = imageList_settings;
            buttonUpdateAPIKey.Name = "buttonUpdateAPIKey";
            toolTip1.SetToolTip(buttonUpdateAPIKey, resources.GetString("buttonUpdateAPIKey.ToolTip"));
            buttonUpdateAPIKey.UseVisualStyleBackColor = true;
            buttonUpdateAPIKey.Click += buttonUpdateAPIKey_Click;
            // 
            // buttonShowHide
            // 
            resources.ApplyResources(buttonShowHide, "buttonShowHide");
            buttonShowHide.ImageList = imageList_settings;
            buttonShowHide.Name = "buttonShowHide";
            toolTip1.SetToolTip(buttonShowHide, resources.GetString("buttonShowHide.ToolTip"));
            buttonShowHide.UseVisualStyleBackColor = true;
            buttonShowHide.Click += buttonShowHide_Click;
            // 
            // labelAPIKeyWarning
            // 
            resources.ApplyResources(labelAPIKeyWarning, "labelAPIKeyWarning");
            labelAPIKeyWarning.ImageList = imageList_settings;
            labelAPIKeyWarning.Name = "labelAPIKeyWarning";
            toolTip1.SetToolTip(labelAPIKeyWarning, resources.GetString("labelAPIKeyWarning.ToolTip"));
            // 
            // labelAPIKey
            // 
            resources.ApplyResources(labelAPIKey, "labelAPIKey");
            labelAPIKey.Name = "labelAPIKey";
            toolTip1.SetToolTip(labelAPIKey, resources.GetString("labelAPIKey.ToolTip"));
            // 
            // textBoxAPIKey
            // 
            resources.ApplyResources(textBoxAPIKey, "textBoxAPIKey");
            textBoxAPIKey.Name = "textBoxAPIKey";
            toolTip1.SetToolTip(textBoxAPIKey, resources.GetString("textBoxAPIKey.ToolTip"));
            textBoxAPIKey.UseSystemPasswordChar = true;
            textBoxAPIKey.TextChanged += textBoxAPIKey_TextChanged;
            // 
            // groupBox_system_speaker_test
            // 
            resources.ApplyResources(groupBox_system_speaker_test, "groupBox_system_speaker_test");
            groupBox_system_speaker_test.Controls.Add(btn_test_system_speaker);
            groupBox_system_speaker_test.Controls.Add(label_test_system_speaker_message);
            groupBox_system_speaker_test.Name = "groupBox_system_speaker_test";
            groupBox_system_speaker_test.TabStop = false;
            toolTip1.SetToolTip(groupBox_system_speaker_test, resources.GetString("groupBox_system_speaker_test.ToolTip"));
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
            // label_test_system_speaker_message
            // 
            resources.ApplyResources(label_test_system_speaker_message, "label_test_system_speaker_message");
            label_test_system_speaker_message.Name = "label_test_system_speaker_message";
            toolTip1.SetToolTip(label_test_system_speaker_message, resources.GetString("label_test_system_speaker_message.ToolTip"));
            // 
            // panelSystemSpeakerWarnings
            // 
            resources.ApplyResources(panelSystemSpeakerWarnings, "panelSystemSpeakerWarnings");
            panelSystemSpeakerWarnings.Controls.Add(label_test_system_speaker_message_2);
            panelSystemSpeakerWarnings.Controls.Add(label_test_system_speaker_message_3);
            panelSystemSpeakerWarnings.Name = "panelSystemSpeakerWarnings";
            toolTip1.SetToolTip(panelSystemSpeakerWarnings, resources.GetString("panelSystemSpeakerWarnings.ToolTip"));
            // 
            // label_test_system_speaker_message_2
            // 
            resources.ApplyResources(label_test_system_speaker_message_2, "label_test_system_speaker_message_2");
            label_test_system_speaker_message_2.ForeColor = Color.FromArgb(192, 0, 0);
            label_test_system_speaker_message_2.Name = "label_test_system_speaker_message_2";
            toolTip1.SetToolTip(label_test_system_speaker_message_2, resources.GetString("label_test_system_speaker_message_2.ToolTip"));
            // 
            // label_test_system_speaker_message_3
            // 
            resources.ApplyResources(label_test_system_speaker_message_3, "label_test_system_speaker_message_3");
            label_test_system_speaker_message_3.ForeColor = Color.FromArgb(255, 128, 0);
            label_test_system_speaker_message_3.Name = "label_test_system_speaker_message_3";
            toolTip1.SetToolTip(label_test_system_speaker_message_3, resources.GetString("label_test_system_speaker_message_3.ToolTip"));
            // 
            // creating_sound_settings
            // 
            resources.ApplyResources(creating_sound_settings, "creating_sound_settings");
            creating_sound_settings.Controls.Add(flowLayoutPanelCreatingSoundSettings);
            creating_sound_settings.Name = "creating_sound_settings";
            toolTip1.SetToolTip(creating_sound_settings, resources.GetString("creating_sound_settings.ToolTip"));
            creating_sound_settings.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelCreatingSoundSettings
            // 
            resources.ApplyResources(flowLayoutPanelCreatingSoundSettings, "flowLayoutPanelCreatingSoundSettings");
            flowLayoutPanelCreatingSoundSettings.Controls.Add(checkBox_enable_create_beep_from_soundcard);
            flowLayoutPanelCreatingSoundSettings.Controls.Add(group_beep_creation_from_sound_card_settings);
            flowLayoutPanelCreatingSoundSettings.Name = "flowLayoutPanelCreatingSoundSettings";
            toolTip1.SetToolTip(flowLayoutPanelCreatingSoundSettings, resources.GetString("flowLayoutPanelCreatingSoundSettings.ToolTip"));
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
            // group_beep_creation_from_sound_card_settings
            // 
            resources.ApplyResources(group_beep_creation_from_sound_card_settings, "group_beep_creation_from_sound_card_settings");
            group_beep_creation_from_sound_card_settings.Controls.Add(flowLayoutPanelSoundDeviceBeepEnabledInfo);
            group_beep_creation_from_sound_card_settings.Controls.Add(group_tone_waveform);
            group_beep_creation_from_sound_card_settings.Name = "group_beep_creation_from_sound_card_settings";
            group_beep_creation_from_sound_card_settings.TabStop = false;
            toolTip1.SetToolTip(group_beep_creation_from_sound_card_settings, resources.GetString("group_beep_creation_from_sound_card_settings.ToolTip"));
            // 
            // flowLayoutPanelSoundDeviceBeepEnabledInfo
            // 
            resources.ApplyResources(flowLayoutPanelSoundDeviceBeepEnabledInfo, "flowLayoutPanelSoundDeviceBeepEnabledInfo");
            flowLayoutPanelSoundDeviceBeepEnabledInfo.Controls.Add(panel1);
            flowLayoutPanelSoundDeviceBeepEnabledInfo.Controls.Add(button_show_reason);
            flowLayoutPanelSoundDeviceBeepEnabledInfo.Name = "flowLayoutPanelSoundDeviceBeepEnabledInfo";
            toolTip1.SetToolTip(flowLayoutPanelSoundDeviceBeepEnabledInfo, resources.GetString("flowLayoutPanelSoundDeviceBeepEnabledInfo.ToolTip"));
            // 
            // panel1
            // 
            resources.ApplyResources(panel1, "panel1");
            panel1.Controls.Add(label_create_beep_from_soundcard_automatically_activated_message_1);
            panel1.Controls.Add(label_create_beep_from_soundcard_automatically_activated_message_2);
            panel1.Name = "panel1";
            toolTip1.SetToolTip(panel1, resources.GetString("panel1.ToolTip"));
            // 
            // label_create_beep_from_soundcard_automatically_activated_message_1
            // 
            resources.ApplyResources(label_create_beep_from_soundcard_automatically_activated_message_1, "label_create_beep_from_soundcard_automatically_activated_message_1");
            label_create_beep_from_soundcard_automatically_activated_message_1.Name = "label_create_beep_from_soundcard_automatically_activated_message_1";
            toolTip1.SetToolTip(label_create_beep_from_soundcard_automatically_activated_message_1, resources.GetString("label_create_beep_from_soundcard_automatically_activated_message_1.ToolTip"));
            // 
            // label_create_beep_from_soundcard_automatically_activated_message_2
            // 
            resources.ApplyResources(label_create_beep_from_soundcard_automatically_activated_message_2, "label_create_beep_from_soundcard_automatically_activated_message_2");
            label_create_beep_from_soundcard_automatically_activated_message_2.Name = "label_create_beep_from_soundcard_automatically_activated_message_2";
            toolTip1.SetToolTip(label_create_beep_from_soundcard_automatically_activated_message_2, resources.GetString("label_create_beep_from_soundcard_automatically_activated_message_2.ToolTip"));
            // 
            // button_show_reason
            // 
            resources.ApplyResources(button_show_reason, "button_show_reason");
            button_show_reason.ImageList = imageList_settings;
            button_show_reason.Name = "button_show_reason";
            toolTip1.SetToolTip(button_show_reason, resources.GetString("button_show_reason.ToolTip"));
            button_show_reason.UseVisualStyleBackColor = true;
            button_show_reason.Click += button_show_reason_Click;
            // 
            // group_tone_waveform
            // 
            resources.ApplyResources(group_tone_waveform, "group_tone_waveform");
            group_tone_waveform.Controls.Add(radioButton_noise);
            group_tone_waveform.Controls.Add(radioButton_triangle);
            group_tone_waveform.Controls.Add(radioButton_sine);
            group_tone_waveform.Controls.Add(radioButton_square);
            group_tone_waveform.Name = "group_tone_waveform";
            group_tone_waveform.TabStop = false;
            toolTip1.SetToolTip(group_tone_waveform, resources.GetString("group_tone_waveform.ToolTip"));
            // 
            // radioButton_noise
            // 
            resources.ApplyResources(radioButton_noise, "radioButton_noise");
            radioButton_noise.Name = "radioButton_noise";
            toolTip1.SetToolTip(radioButton_noise, resources.GetString("radioButton_noise.ToolTip"));
            radioButton_noise.UseVisualStyleBackColor = true;
            radioButton_noise.CheckedChanged += soundcard_beep_waveform_selection;
            // 
            // radioButton_triangle
            // 
            resources.ApplyResources(radioButton_triangle, "radioButton_triangle");
            radioButton_triangle.Name = "radioButton_triangle";
            toolTip1.SetToolTip(radioButton_triangle, resources.GetString("radioButton_triangle.ToolTip"));
            radioButton_triangle.UseVisualStyleBackColor = true;
            radioButton_triangle.CheckedChanged += soundcard_beep_waveform_selection;
            // 
            // radioButton_sine
            // 
            resources.ApplyResources(radioButton_sine, "radioButton_sine");
            radioButton_sine.Name = "radioButton_sine";
            toolTip1.SetToolTip(radioButton_sine, resources.GetString("radioButton_sine.ToolTip"));
            radioButton_sine.UseVisualStyleBackColor = true;
            radioButton_sine.CheckedChanged += soundcard_beep_waveform_selection;
            // 
            // radioButton_square
            // 
            resources.ApplyResources(radioButton_square, "radioButton_square");
            radioButton_square.Checked = true;
            radioButton_square.Name = "radioButton_square";
            radioButton_square.TabStop = true;
            toolTip1.SetToolTip(radioButton_square, resources.GetString("radioButton_square.ToolTip"));
            radioButton_square.UseVisualStyleBackColor = true;
            radioButton_square.CheckedChanged += soundcard_beep_waveform_selection;
            // 
            // devices_settings
            // 
            resources.ApplyResources(devices_settings, "devices_settings");
            devices_settings.Controls.Add(flowLayoutPanelDevicesSettings);
            devices_settings.Name = "devices_settings";
            toolTip1.SetToolTip(devices_settings, resources.GetString("devices_settings.ToolTip"));
            devices_settings.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelDevicesSettings
            // 
            resources.ApplyResources(flowLayoutPanelDevicesSettings, "flowLayoutPanelDevicesSettings");
            flowLayoutPanelDevicesSettings.Controls.Add(checkBox_use_midi_input);
            flowLayoutPanelDevicesSettings.Controls.Add(group_midi_input_devices);
            flowLayoutPanelDevicesSettings.Controls.Add(checkBox_use_midi_output);
            flowLayoutPanelDevicesSettings.Controls.Add(group_midi_output_devices);
            flowLayoutPanelDevicesSettings.Controls.Add(groupBox_other_devices);
            flowLayoutPanelDevicesSettings.Name = "flowLayoutPanelDevicesSettings";
            toolTip1.SetToolTip(flowLayoutPanelDevicesSettings, resources.GetString("flowLayoutPanelDevicesSettings.ToolTip"));
            // 
            // checkBox_use_midi_input
            // 
            resources.ApplyResources(checkBox_use_midi_input, "checkBox_use_midi_input");
            checkBox_use_midi_input.ImageList = imageList_settings;
            checkBox_use_midi_input.Name = "checkBox_use_midi_input";
            toolTip1.SetToolTip(checkBox_use_midi_input, resources.GetString("checkBox_use_midi_input.ToolTip"));
            checkBox_use_midi_input.UseVisualStyleBackColor = true;
            checkBox_use_midi_input.CheckedChanged += checkBox_use_midi_input_CheckedChanged;
            // 
            // group_midi_input_devices
            // 
            resources.ApplyResources(group_midi_input_devices, "group_midi_input_devices");
            group_midi_input_devices.Controls.Add(refresh_midi_input_button);
            group_midi_input_devices.Controls.Add(comboBox_midi_input_devices);
            group_midi_input_devices.Controls.Add(label_midi_input_device);
            group_midi_input_devices.Name = "group_midi_input_devices";
            group_midi_input_devices.TabStop = false;
            toolTip1.SetToolTip(group_midi_input_devices, resources.GetString("group_midi_input_devices.ToolTip"));
            // 
            // refresh_midi_input_button
            // 
            resources.ApplyResources(refresh_midi_input_button, "refresh_midi_input_button");
            refresh_midi_input_button.ImageList = imageList_settings;
            refresh_midi_input_button.Name = "refresh_midi_input_button";
            toolTip1.SetToolTip(refresh_midi_input_button, resources.GetString("refresh_midi_input_button.ToolTip"));
            refresh_midi_input_button.UseVisualStyleBackColor = true;
            refresh_midi_input_button.Click += refresh_midi_input_button_Click;
            // 
            // comboBox_midi_input_devices
            // 
            resources.ApplyResources(comboBox_midi_input_devices, "comboBox_midi_input_devices");
            comboBox_midi_input_devices.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_input_devices.FormattingEnabled = true;
            comboBox_midi_input_devices.Name = "comboBox_midi_input_devices";
            toolTip1.SetToolTip(comboBox_midi_input_devices, resources.GetString("comboBox_midi_input_devices.ToolTip"));
            comboBox_midi_input_devices.SelectedIndexChanged += comboBox_midi_input_devices_SelectedIndexChanged;
            // 
            // label_midi_input_device
            // 
            resources.ApplyResources(label_midi_input_device, "label_midi_input_device");
            label_midi_input_device.Name = "label_midi_input_device";
            toolTip1.SetToolTip(label_midi_input_device, resources.GetString("label_midi_input_device.ToolTip"));
            // 
            // checkBox_use_midi_output
            // 
            resources.ApplyResources(checkBox_use_midi_output, "checkBox_use_midi_output");
            checkBox_use_midi_output.ImageList = imageList_settings;
            checkBox_use_midi_output.Name = "checkBox_use_midi_output";
            toolTip1.SetToolTip(checkBox_use_midi_output, resources.GetString("checkBox_use_midi_output.ToolTip"));
            checkBox_use_midi_output.UseVisualStyleBackColor = true;
            checkBox_use_midi_output.CheckedChanged += checkBox_use_midi_output_CheckedChanged;
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
            toolTip1.SetToolTip(group_midi_output_devices, resources.GetString("group_midi_output_devices.ToolTip"));
            // 
            // comboBox_midi_output_instrument
            // 
            resources.ApplyResources(comboBox_midi_output_instrument, "comboBox_midi_output_instrument");
            comboBox_midi_output_instrument.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_output_instrument.FormattingEnabled = true;
            comboBox_midi_output_instrument.Name = "comboBox_midi_output_instrument";
            toolTip1.SetToolTip(comboBox_midi_output_instrument, resources.GetString("comboBox_midi_output_instrument.ToolTip"));
            comboBox_midi_output_instrument.SelectedIndexChanged += comboBox_midi_output_instrument_SelectedIndexChanged;
            // 
            // comboBox_midi_output_channel
            // 
            resources.ApplyResources(comboBox_midi_output_channel, "comboBox_midi_output_channel");
            comboBox_midi_output_channel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_output_channel.FormattingEnabled = true;
            comboBox_midi_output_channel.Name = "comboBox_midi_output_channel";
            toolTip1.SetToolTip(comboBox_midi_output_channel, resources.GetString("comboBox_midi_output_channel.ToolTip"));
            comboBox_midi_output_channel.SelectedIndexChanged += comboBox_midi_output_channel_SelectedIndexChanged;
            // 
            // label_instrument
            // 
            resources.ApplyResources(label_instrument, "label_instrument");
            label_instrument.Name = "label_instrument";
            toolTip1.SetToolTip(label_instrument, resources.GetString("label_instrument.ToolTip"));
            // 
            // label_channel
            // 
            resources.ApplyResources(label_channel, "label_channel");
            label_channel.Name = "label_channel";
            toolTip1.SetToolTip(label_channel, resources.GetString("label_channel.ToolTip"));
            // 
            // refresh_midi_output_button
            // 
            resources.ApplyResources(refresh_midi_output_button, "refresh_midi_output_button");
            refresh_midi_output_button.ImageList = imageList_settings;
            refresh_midi_output_button.Name = "refresh_midi_output_button";
            toolTip1.SetToolTip(refresh_midi_output_button, resources.GetString("refresh_midi_output_button.ToolTip"));
            refresh_midi_output_button.UseVisualStyleBackColor = true;
            refresh_midi_output_button.Click += refresh_midi_output_button_Click;
            // 
            // comboBox_midi_output_devices
            // 
            resources.ApplyResources(comboBox_midi_output_devices, "comboBox_midi_output_devices");
            comboBox_midi_output_devices.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_midi_output_devices.FormattingEnabled = true;
            comboBox_midi_output_devices.Name = "comboBox_midi_output_devices";
            toolTip1.SetToolTip(comboBox_midi_output_devices, resources.GetString("comboBox_midi_output_devices.ToolTip"));
            comboBox_midi_output_devices.SelectedIndexChanged += comboBox_midi_output_devices_SelectedIndexChanged;
            // 
            // label_midi_output_device
            // 
            resources.ApplyResources(label_midi_output_device, "label_midi_output_device");
            label_midi_output_device.Name = "label_midi_output_device";
            toolTip1.SetToolTip(label_midi_output_device, resources.GetString("label_midi_output_device.ToolTip"));
            // 
            // groupBox_other_devices
            // 
            resources.ApplyResources(groupBox_other_devices, "groupBox_other_devices");
            groupBox_other_devices.Controls.Add(button_get_firmware);
            groupBox_other_devices.Controls.Add(label_firmware_warning);
            groupBox_other_devices.Controls.Add(trackBar_stepper_motor_octave);
            groupBox_other_devices.Controls.Add(label_stepper_motor_octave);
            groupBox_other_devices.Controls.Add(groupBox_type_of_device);
            groupBox_other_devices.Controls.Add(checkBox_use_microcontroller);
            groupBox_other_devices.Name = "groupBox_other_devices";
            groupBox_other_devices.TabStop = false;
            toolTip1.SetToolTip(groupBox_other_devices, resources.GetString("groupBox_other_devices.ToolTip"));
            // 
            // button_get_firmware
            // 
            resources.ApplyResources(button_get_firmware, "button_get_firmware");
            button_get_firmware.ImageList = imageList_settings;
            button_get_firmware.Name = "button_get_firmware";
            toolTip1.SetToolTip(button_get_firmware, resources.GetString("button_get_firmware.ToolTip"));
            button_get_firmware.UseVisualStyleBackColor = true;
            button_get_firmware.Click += button_get_firmware_Click;
            // 
            // label_firmware_warning
            // 
            resources.ApplyResources(label_firmware_warning, "label_firmware_warning");
            label_firmware_warning.ImageList = imageList_settings;
            label_firmware_warning.Name = "label_firmware_warning";
            toolTip1.SetToolTip(label_firmware_warning, resources.GetString("label_firmware_warning.ToolTip"));
            // 
            // trackBar_stepper_motor_octave
            // 
            resources.ApplyResources(trackBar_stepper_motor_octave, "trackBar_stepper_motor_octave");
            trackBar_stepper_motor_octave.BackColor = Color.FromArgb(249, 248, 249);
            trackBar_stepper_motor_octave.Maximum = 5;
            trackBar_stepper_motor_octave.Name = "trackBar_stepper_motor_octave";
            toolTip1.SetToolTip(trackBar_stepper_motor_octave, resources.GetString("trackBar_stepper_motor_octave.ToolTip"));
            trackBar_stepper_motor_octave.Value = 2;
            trackBar_stepper_motor_octave.Scroll += trackBar_motor_octave_Scroll;
            // 
            // label_stepper_motor_octave
            // 
            resources.ApplyResources(label_stepper_motor_octave, "label_stepper_motor_octave");
            label_stepper_motor_octave.Name = "label_stepper_motor_octave";
            toolTip1.SetToolTip(label_stepper_motor_octave, resources.GetString("label_stepper_motor_octave.ToolTip"));
            // 
            // groupBox_type_of_device
            // 
            resources.ApplyResources(groupBox_type_of_device, "groupBox_type_of_device");
            groupBox_type_of_device.Controls.Add(radioButtonDCMotorOrBuzzer);
            groupBox_type_of_device.Controls.Add(radioButtonStepperMotor);
            groupBox_type_of_device.Name = "groupBox_type_of_device";
            groupBox_type_of_device.TabStop = false;
            toolTip1.SetToolTip(groupBox_type_of_device, resources.GetString("groupBox_type_of_device.ToolTip"));
            // 
            // radioButtonDCMotorOrBuzzer
            // 
            resources.ApplyResources(radioButtonDCMotorOrBuzzer, "radioButtonDCMotorOrBuzzer");
            radioButtonDCMotorOrBuzzer.Name = "radioButtonDCMotorOrBuzzer";
            toolTip1.SetToolTip(radioButtonDCMotorOrBuzzer, resources.GetString("radioButtonDCMotorOrBuzzer.ToolTip"));
            radioButtonDCMotorOrBuzzer.UseVisualStyleBackColor = true;
            radioButtonDCMotorOrBuzzer.CheckedChanged += deviceTypeRadioButtons_CheckedChanged;
            // 
            // radioButtonStepperMotor
            // 
            resources.ApplyResources(radioButtonStepperMotor, "radioButtonStepperMotor");
            radioButtonStepperMotor.Checked = true;
            radioButtonStepperMotor.Name = "radioButtonStepperMotor";
            radioButtonStepperMotor.TabStop = true;
            toolTip1.SetToolTip(radioButtonStepperMotor, resources.GetString("radioButtonStepperMotor.ToolTip"));
            radioButtonStepperMotor.UseVisualStyleBackColor = true;
            radioButtonStepperMotor.CheckedChanged += deviceTypeRadioButtons_CheckedChanged;
            // 
            // checkBox_use_microcontroller
            // 
            resources.ApplyResources(checkBox_use_microcontroller, "checkBox_use_microcontroller");
            checkBox_use_microcontroller.ImageList = imageList_settings;
            checkBox_use_microcontroller.Name = "checkBox_use_microcontroller";
            toolTip1.SetToolTip(checkBox_use_microcontroller, resources.GetString("checkBox_use_microcontroller.ToolTip"));
            checkBox_use_microcontroller.UseVisualStyleBackColor = true;
            checkBox_use_microcontroller.CheckedChanged += checkBox_use_motor_speed_mod_CheckedChanged;
            // 
            // appearance
            // 
            resources.ApplyResources(appearance, "appearance");
            appearance.Controls.Add(flowLayoutPanelAppearanceSettings);
            appearance.Name = "appearance";
            toolTip1.SetToolTip(appearance, resources.GetString("appearance.ToolTip"));
            appearance.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanelAppearanceSettings
            // 
            resources.ApplyResources(flowLayoutPanelAppearanceSettings, "flowLayoutPanelAppearanceSettings");
            flowLayoutPanelAppearanceSettings.Controls.Add(group_keyboard_colors);
            flowLayoutPanelAppearanceSettings.Controls.Add(group_buttons_and_controls_colors);
            flowLayoutPanelAppearanceSettings.Controls.Add(group_indicator_colors);
            flowLayoutPanelAppearanceSettings.Controls.Add(group_lyrics_size_settings);
            flowLayoutPanelAppearanceSettings.Controls.Add(panel2);
            flowLayoutPanelAppearanceSettings.Name = "flowLayoutPanelAppearanceSettings";
            toolTip1.SetToolTip(flowLayoutPanelAppearanceSettings, resources.GetString("flowLayoutPanelAppearanceSettings.ToolTip"));
            // 
            // group_keyboard_colors
            // 
            resources.ApplyResources(group_keyboard_colors, "group_keyboard_colors");
            group_keyboard_colors.Controls.Add(third_octave_color_change);
            group_keyboard_colors.Controls.Add(second_octave_color_change);
            group_keyboard_colors.Controls.Add(first_octave_color_change);
            group_keyboard_colors.Controls.Add(third_octave_color);
            group_keyboard_colors.Controls.Add(second_octave_color);
            group_keyboard_colors.Controls.Add(first_octave_color);
            group_keyboard_colors.Controls.Add(label4);
            group_keyboard_colors.Controls.Add(label3);
            group_keyboard_colors.Controls.Add(label2);
            group_keyboard_colors.Name = "group_keyboard_colors";
            group_keyboard_colors.TabStop = false;
            toolTip1.SetToolTip(group_keyboard_colors, resources.GetString("group_keyboard_colors.ToolTip"));
            // 
            // third_octave_color_change
            // 
            resources.ApplyResources(third_octave_color_change, "third_octave_color_change");
            third_octave_color_change.Name = "third_octave_color_change";
            toolTip1.SetToolTip(third_octave_color_change, resources.GetString("third_octave_color_change.ToolTip"));
            third_octave_color_change.UseVisualStyleBackColor = true;
            third_octave_color_change.Click += third_octave_color_change_Click;
            // 
            // second_octave_color_change
            // 
            resources.ApplyResources(second_octave_color_change, "second_octave_color_change");
            second_octave_color_change.Name = "second_octave_color_change";
            toolTip1.SetToolTip(second_octave_color_change, resources.GetString("second_octave_color_change.ToolTip"));
            second_octave_color_change.UseVisualStyleBackColor = true;
            second_octave_color_change.Click += second_octave_color_change_Click;
            // 
            // first_octave_color_change
            // 
            resources.ApplyResources(first_octave_color_change, "first_octave_color_change");
            first_octave_color_change.Name = "first_octave_color_change";
            toolTip1.SetToolTip(first_octave_color_change, resources.GetString("first_octave_color_change.ToolTip"));
            first_octave_color_change.UseVisualStyleBackColor = true;
            first_octave_color_change.Click += first_octave_color_change_Click;
            // 
            // third_octave_color
            // 
            resources.ApplyResources(third_octave_color, "third_octave_color");
            third_octave_color.BackColor = Color.FromArgb(192, 255, 192);
            third_octave_color.BorderStyle = BorderStyle.FixedSingle;
            third_octave_color.Name = "third_octave_color";
            toolTip1.SetToolTip(third_octave_color, resources.GetString("third_octave_color.ToolTip"));
            // 
            // second_octave_color
            // 
            resources.ApplyResources(second_octave_color, "second_octave_color");
            second_octave_color.BackColor = Color.FromArgb(192, 192, 255);
            second_octave_color.BorderStyle = BorderStyle.FixedSingle;
            second_octave_color.Name = "second_octave_color";
            toolTip1.SetToolTip(second_octave_color, resources.GetString("second_octave_color.ToolTip"));
            // 
            // first_octave_color
            // 
            resources.ApplyResources(first_octave_color, "first_octave_color");
            first_octave_color.BackColor = Color.FromArgb(255, 224, 192);
            first_octave_color.BorderStyle = BorderStyle.FixedSingle;
            first_octave_color.Name = "first_octave_color";
            toolTip1.SetToolTip(first_octave_color, resources.GetString("first_octave_color.ToolTip"));
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            toolTip1.SetToolTip(label4, resources.GetString("label4.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            toolTip1.SetToolTip(label3, resources.GetString("label3.ToolTip"));
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            toolTip1.SetToolTip(label2, resources.GetString("label2.ToolTip"));
            // 
            // group_buttons_and_controls_colors
            // 
            resources.ApplyResources(group_buttons_and_controls_colors, "group_buttons_and_controls_colors");
            group_buttons_and_controls_colors.Controls.Add(markup_color_change);
            group_buttons_and_controls_colors.Controls.Add(metronome_color_change);
            group_buttons_and_controls_colors.Controls.Add(playback_buttons_color_change);
            group_buttons_and_controls_colors.Controls.Add(erase_whole_line_color_change);
            group_buttons_and_controls_colors.Controls.Add(unselect_line_color_change);
            group_buttons_and_controls_colors.Controls.Add(clear_notes_color_change);
            group_buttons_and_controls_colors.Controls.Add(blank_line_color_change);
            group_buttons_and_controls_colors.Controls.Add(markup_color);
            group_buttons_and_controls_colors.Controls.Add(metronome_color);
            group_buttons_and_controls_colors.Controls.Add(playback_buttons_color);
            group_buttons_and_controls_colors.Controls.Add(erase_whole_line_color);
            group_buttons_and_controls_colors.Controls.Add(unselect_line_color);
            group_buttons_and_controls_colors.Controls.Add(clear_notes_color);
            group_buttons_and_controls_colors.Controls.Add(blank_line_color);
            group_buttons_and_controls_colors.Controls.Add(label1);
            group_buttons_and_controls_colors.Controls.Add(label10);
            group_buttons_and_controls_colors.Controls.Add(label8);
            group_buttons_and_controls_colors.Controls.Add(label9);
            group_buttons_and_controls_colors.Controls.Add(label5);
            group_buttons_and_controls_colors.Controls.Add(label6);
            group_buttons_and_controls_colors.Controls.Add(label7);
            group_buttons_and_controls_colors.Name = "group_buttons_and_controls_colors";
            group_buttons_and_controls_colors.TabStop = false;
            toolTip1.SetToolTip(group_buttons_and_controls_colors, resources.GetString("group_buttons_and_controls_colors.ToolTip"));
            // 
            // markup_color_change
            // 
            resources.ApplyResources(markup_color_change, "markup_color_change");
            markup_color_change.Name = "markup_color_change";
            toolTip1.SetToolTip(markup_color_change, resources.GetString("markup_color_change.ToolTip"));
            markup_color_change.UseVisualStyleBackColor = true;
            markup_color_change.Click += markup_color_change_Click;
            // 
            // metronome_color_change
            // 
            resources.ApplyResources(metronome_color_change, "metronome_color_change");
            metronome_color_change.Name = "metronome_color_change";
            toolTip1.SetToolTip(metronome_color_change, resources.GetString("metronome_color_change.ToolTip"));
            metronome_color_change.UseVisualStyleBackColor = true;
            metronome_color_change.Click += metronome_color_change_Click;
            // 
            // playback_buttons_color_change
            // 
            resources.ApplyResources(playback_buttons_color_change, "playback_buttons_color_change");
            playback_buttons_color_change.Name = "playback_buttons_color_change";
            toolTip1.SetToolTip(playback_buttons_color_change, resources.GetString("playback_buttons_color_change.ToolTip"));
            playback_buttons_color_change.UseVisualStyleBackColor = true;
            playback_buttons_color_change.Click += playback_buttons_color_change_Click;
            // 
            // erase_whole_line_color_change
            // 
            resources.ApplyResources(erase_whole_line_color_change, "erase_whole_line_color_change");
            erase_whole_line_color_change.Name = "erase_whole_line_color_change";
            toolTip1.SetToolTip(erase_whole_line_color_change, resources.GetString("erase_whole_line_color_change.ToolTip"));
            erase_whole_line_color_change.UseVisualStyleBackColor = true;
            erase_whole_line_color_change.Click += erase_whole_line_color_change_Click;
            // 
            // unselect_line_color_change
            // 
            resources.ApplyResources(unselect_line_color_change, "unselect_line_color_change");
            unselect_line_color_change.Name = "unselect_line_color_change";
            toolTip1.SetToolTip(unselect_line_color_change, resources.GetString("unselect_line_color_change.ToolTip"));
            unselect_line_color_change.UseVisualStyleBackColor = true;
            unselect_line_color_change.Click += unseelct_line_color_change_Click;
            // 
            // clear_notes_color_change
            // 
            resources.ApplyResources(clear_notes_color_change, "clear_notes_color_change");
            clear_notes_color_change.Name = "clear_notes_color_change";
            toolTip1.SetToolTip(clear_notes_color_change, resources.GetString("clear_notes_color_change.ToolTip"));
            clear_notes_color_change.UseVisualStyleBackColor = true;
            clear_notes_color_change.Click += clear_notes_color_change_Click;
            // 
            // blank_line_color_change
            // 
            resources.ApplyResources(blank_line_color_change, "blank_line_color_change");
            blank_line_color_change.Name = "blank_line_color_change";
            toolTip1.SetToolTip(blank_line_color_change, resources.GetString("blank_line_color_change.ToolTip"));
            blank_line_color_change.UseVisualStyleBackColor = true;
            blank_line_color_change.Click += blank_line_color_change_Click;
            // 
            // markup_color
            // 
            resources.ApplyResources(markup_color, "markup_color");
            markup_color.BackColor = Color.LightBlue;
            markup_color.BorderStyle = BorderStyle.FixedSingle;
            markup_color.Name = "markup_color";
            toolTip1.SetToolTip(markup_color, resources.GetString("markup_color.ToolTip"));
            // 
            // metronome_color
            // 
            resources.ApplyResources(metronome_color, "metronome_color");
            metronome_color.BackColor = Color.FromArgb(192, 255, 192);
            metronome_color.BorderStyle = BorderStyle.FixedSingle;
            metronome_color.Name = "metronome_color";
            toolTip1.SetToolTip(metronome_color, resources.GetString("metronome_color.ToolTip"));
            // 
            // playback_buttons_color
            // 
            resources.ApplyResources(playback_buttons_color, "playback_buttons_color");
            playback_buttons_color.BackColor = Color.FromArgb(128, 255, 128);
            playback_buttons_color.BorderStyle = BorderStyle.FixedSingle;
            playback_buttons_color.Name = "playback_buttons_color";
            toolTip1.SetToolTip(playback_buttons_color, resources.GetString("playback_buttons_color.ToolTip"));
            // 
            // erase_whole_line_color
            // 
            resources.ApplyResources(erase_whole_line_color, "erase_whole_line_color");
            erase_whole_line_color.BackColor = Color.FromArgb(255, 128, 128);
            erase_whole_line_color.BorderStyle = BorderStyle.FixedSingle;
            erase_whole_line_color.Name = "erase_whole_line_color";
            toolTip1.SetToolTip(erase_whole_line_color, resources.GetString("erase_whole_line_color.ToolTip"));
            // 
            // unselect_line_color
            // 
            resources.ApplyResources(unselect_line_color, "unselect_line_color");
            unselect_line_color.BackColor = Color.FromArgb(128, 255, 255);
            unselect_line_color.BorderStyle = BorderStyle.FixedSingle;
            unselect_line_color.Name = "unselect_line_color";
            toolTip1.SetToolTip(unselect_line_color, resources.GetString("unselect_line_color.ToolTip"));
            // 
            // clear_notes_color
            // 
            resources.ApplyResources(clear_notes_color, "clear_notes_color");
            clear_notes_color.BackColor = Color.FromArgb(128, 128, 255);
            clear_notes_color.BorderStyle = BorderStyle.FixedSingle;
            clear_notes_color.Name = "clear_notes_color";
            toolTip1.SetToolTip(clear_notes_color, resources.GetString("clear_notes_color.ToolTip"));
            // 
            // blank_line_color
            // 
            resources.ApplyResources(blank_line_color, "blank_line_color");
            blank_line_color.BackColor = Color.FromArgb(255, 224, 192);
            blank_line_color.BorderStyle = BorderStyle.FixedSingle;
            blank_line_color.Name = "blank_line_color";
            toolTip1.SetToolTip(blank_line_color, resources.GetString("blank_line_color.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            toolTip1.SetToolTip(label1, resources.GetString("label1.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(label10, "label10");
            label10.Name = "label10";
            toolTip1.SetToolTip(label10, resources.GetString("label10.ToolTip"));
            // 
            // label8
            // 
            resources.ApplyResources(label8, "label8");
            label8.Name = "label8";
            toolTip1.SetToolTip(label8, resources.GetString("label8.ToolTip"));
            // 
            // label9
            // 
            resources.ApplyResources(label9, "label9");
            label9.Name = "label9";
            toolTip1.SetToolTip(label9, resources.GetString("label9.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            toolTip1.SetToolTip(label5, resources.GetString("label5.ToolTip"));
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            toolTip1.SetToolTip(label6, resources.GetString("label6.ToolTip"));
            // 
            // label7
            // 
            resources.ApplyResources(label7, "label7");
            label7.Name = "label7";
            toolTip1.SetToolTip(label7, resources.GetString("label7.ToolTip"));
            // 
            // group_indicator_colors
            // 
            resources.ApplyResources(group_indicator_colors, "group_indicator_colors");
            group_indicator_colors.Controls.Add(note_indicator_color_change);
            group_indicator_colors.Controls.Add(beep_indicator_color_change);
            group_indicator_colors.Controls.Add(label12);
            group_indicator_colors.Controls.Add(beep_indicator_color);
            group_indicator_colors.Controls.Add(note_indicator_color);
            group_indicator_colors.Controls.Add(label11);
            group_indicator_colors.Name = "group_indicator_colors";
            group_indicator_colors.TabStop = false;
            toolTip1.SetToolTip(group_indicator_colors, resources.GetString("group_indicator_colors.ToolTip"));
            // 
            // note_indicator_color_change
            // 
            resources.ApplyResources(note_indicator_color_change, "note_indicator_color_change");
            note_indicator_color_change.Name = "note_indicator_color_change";
            toolTip1.SetToolTip(note_indicator_color_change, resources.GetString("note_indicator_color_change.ToolTip"));
            note_indicator_color_change.UseVisualStyleBackColor = true;
            note_indicator_color_change.Click += note_indicator_color_change_Click;
            // 
            // beep_indicator_color_change
            // 
            resources.ApplyResources(beep_indicator_color_change, "beep_indicator_color_change");
            beep_indicator_color_change.Name = "beep_indicator_color_change";
            toolTip1.SetToolTip(beep_indicator_color_change, resources.GetString("beep_indicator_color_change.ToolTip"));
            beep_indicator_color_change.UseVisualStyleBackColor = true;
            beep_indicator_color_change.Click += beep_indicator_color_change_Click;
            // 
            // label12
            // 
            resources.ApplyResources(label12, "label12");
            label12.Name = "label12";
            toolTip1.SetToolTip(label12, resources.GetString("label12.ToolTip"));
            // 
            // beep_indicator_color
            // 
            resources.ApplyResources(beep_indicator_color, "beep_indicator_color");
            beep_indicator_color.BackColor = Color.Red;
            beep_indicator_color.BorderStyle = BorderStyle.FixedSingle;
            beep_indicator_color.Name = "beep_indicator_color";
            toolTip1.SetToolTip(beep_indicator_color, resources.GetString("beep_indicator_color.ToolTip"));
            // 
            // note_indicator_color
            // 
            resources.ApplyResources(note_indicator_color, "note_indicator_color");
            note_indicator_color.BackColor = Color.Red;
            note_indicator_color.BorderStyle = BorderStyle.FixedSingle;
            note_indicator_color.Name = "note_indicator_color";
            toolTip1.SetToolTip(note_indicator_color, resources.GetString("note_indicator_color.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(label11, "label11");
            label11.Name = "label11";
            toolTip1.SetToolTip(label11, resources.GetString("label11.ToolTip"));
            // 
            // group_lyrics_size_settings
            // 
            resources.ApplyResources(group_lyrics_size_settings, "group_lyrics_size_settings");
            group_lyrics_size_settings.Controls.Add(buttonPreviewLyrics);
            group_lyrics_size_settings.Controls.Add(labelPt);
            group_lyrics_size_settings.Controls.Add(numericUpDownLyricsSize);
            group_lyrics_size_settings.Controls.Add(label13);
            group_lyrics_size_settings.Name = "group_lyrics_size_settings";
            group_lyrics_size_settings.TabStop = false;
            toolTip1.SetToolTip(group_lyrics_size_settings, resources.GetString("group_lyrics_size_settings.ToolTip"));
            // 
            // buttonPreviewLyrics
            // 
            resources.ApplyResources(buttonPreviewLyrics, "buttonPreviewLyrics");
            buttonPreviewLyrics.ImageList = imageList_settings;
            buttonPreviewLyrics.Name = "buttonPreviewLyrics";
            toolTip1.SetToolTip(buttonPreviewLyrics, resources.GetString("buttonPreviewLyrics.ToolTip"));
            buttonPreviewLyrics.UseVisualStyleBackColor = true;
            buttonPreviewLyrics.Click += buttonPreviewLyrics_Click;
            // 
            // labelPt
            // 
            resources.ApplyResources(labelPt, "labelPt");
            labelPt.Name = "labelPt";
            toolTip1.SetToolTip(labelPt, resources.GetString("labelPt.ToolTip"));
            // 
            // numericUpDownLyricsSize
            // 
            resources.ApplyResources(numericUpDownLyricsSize, "numericUpDownLyricsSize");
            numericUpDownLyricsSize.Increment = new decimal(new int[] { 2, 0, 0, 0 });
            numericUpDownLyricsSize.Maximum = new decimal(new int[] { 105, 0, 0, 0 });
            numericUpDownLyricsSize.Minimum = new decimal(new int[] { 8, 0, 0, 0 });
            numericUpDownLyricsSize.Name = "numericUpDownLyricsSize";
            toolTip1.SetToolTip(numericUpDownLyricsSize, resources.GetString("numericUpDownLyricsSize.ToolTip"));
            numericUpDownLyricsSize.Value = new decimal(new int[] { 32, 0, 0, 0 });
            numericUpDownLyricsSize.ValueChanged += numericUpDownLyricsSize_ValueChanged;
            // 
            // label13
            // 
            resources.ApplyResources(label13, "label13");
            label13.Name = "label13";
            toolTip1.SetToolTip(label13, resources.GetString("label13.ToolTip"));
            // 
            // panel2
            // 
            resources.ApplyResources(panel2, "panel2");
            panel2.Controls.Add(reset_appearance_settings);
            panel2.Name = "panel2";
            toolTip1.SetToolTip(panel2, resources.GetString("panel2.ToolTip"));
            // 
            // reset_appearance_settings
            // 
            resources.ApplyResources(reset_appearance_settings, "reset_appearance_settings");
            reset_appearance_settings.ImageList = imageList_settings;
            reset_appearance_settings.Name = "reset_appearance_settings";
            toolTip1.SetToolTip(reset_appearance_settings, resources.GetString("reset_appearance_settings.ToolTip"));
            reset_appearance_settings.UseVisualStyleBackColor = true;
            reset_appearance_settings.Click += reset_appearance_settings_Click;
            // 
            // colorDialog1
            // 
            colorDialog1.AnyColor = true;
            colorDialog1.FullOpen = true;
            // 
            // settings_window
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(tabControl_settings);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "settings_window";
            ShowIcon = false;
            ShowInTaskbar = false;
            toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            FormClosing += settings_window_FormClosing;
            SystemColorsChanged += settings_window_SystemColorsChanged;
            tabControl_settings.ResumeLayout(false);
            general_settings.ResumeLayout(false);
            flowLayoutPanelGeneralSettings.ResumeLayout(false);
            groupBoxLanguageSettings.ResumeLayout(false);
            groupBoxLanguageSettings.PerformLayout();
            groupBox_appearance.ResumeLayout(false);
            groupBox_appearance.PerformLayout();
            groupBoxCreateMusicWithAI.ResumeLayout(false);
            groupBoxCreateMusicWithAI.PerformLayout();
            groupBox_system_speaker_test.ResumeLayout(false);
            groupBox_system_speaker_test.PerformLayout();
            panelSystemSpeakerWarnings.ResumeLayout(false);
            creating_sound_settings.ResumeLayout(false);
            flowLayoutPanelCreatingSoundSettings.ResumeLayout(false);
            flowLayoutPanelCreatingSoundSettings.PerformLayout();
            group_beep_creation_from_sound_card_settings.ResumeLayout(false);
            flowLayoutPanelSoundDeviceBeepEnabledInfo.ResumeLayout(false);
            panel1.ResumeLayout(false);
            group_tone_waveform.ResumeLayout(false);
            group_tone_waveform.PerformLayout();
            devices_settings.ResumeLayout(false);
            flowLayoutPanelDevicesSettings.ResumeLayout(false);
            flowLayoutPanelDevicesSettings.PerformLayout();
            group_midi_input_devices.ResumeLayout(false);
            group_midi_input_devices.PerformLayout();
            group_midi_output_devices.ResumeLayout(false);
            group_midi_output_devices.PerformLayout();
            groupBox_other_devices.ResumeLayout(false);
            groupBox_other_devices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar_stepper_motor_octave).EndInit();
            groupBox_type_of_device.ResumeLayout(false);
            groupBox_type_of_device.PerformLayout();
            appearance.ResumeLayout(false);
            flowLayoutPanelAppearanceSettings.ResumeLayout(false);
            group_keyboard_colors.ResumeLayout(false);
            group_keyboard_colors.PerformLayout();
            group_buttons_and_controls_colors.ResumeLayout(false);
            group_buttons_and_controls_colors.PerformLayout();
            group_indicator_colors.ResumeLayout(false);
            group_indicator_colors.PerformLayout();
            group_lyrics_size_settings.ResumeLayout(false);
            group_lyrics_size_settings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownLyricsSize).EndInit();
            panel2.ResumeLayout(false);
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
        private GroupBox group_midi_input_devices;
        private ComboBox comboBox_midi_input_devices;
        private Label label_midi_input_device;
        private CheckBox checkBox_use_midi_output;
        private GroupBox group_midi_output_devices;
        private ComboBox comboBox_midi_output_devices;
        private Label label_midi_output_device;
        private ToolTip toolTip1;
        private GroupBox groupBox_appearance;
        public Label label_test_system_speaker_message;
        public Label label_test_system_speaker_message_2;
        public GroupBox groupBox_system_speaker_test;
        private Button btn_test_system_speaker;
        private ImageList imageList_settings;
        public Label label_test_system_speaker_message_3;
        private TabPage appearance;
        private GroupBox group_keyboard_colors;
        private Label label2;
        private Button second_octave_color_change;
        private Button first_octave_color_change;
        private Label label3;
        private Button third_octave_color_change;
        private Label label4;
        private GroupBox group_buttons_and_controls_colors;
        private Button unselect_line_color_change;
        private Button clear_notes_color_change;
        private Button blank_line_color_change;
        private Label label5;
        private Label label6;
        private Label label7;
        private Button erase_whole_line_color_change;
        private Label label9;
        private Button playback_buttons_color_change;
        private Label label8;
        private Button metronome_color_change;
        private Label label10;
        private GroupBox group_indicator_colors;
        private Button note_indicator_color_change;
        private Label label11;
        private Button beep_indicator_color_change;
        private Label label12;
        private ColorDialog colorDialog1;
        private Button reset_appearance_settings;
        private Button refresh_midi_input_button;
        private Button refresh_midi_output_button;
        private ComboBox comboBox_midi_output_channel;
        private Label label_channel;
        private ComboBox comboBox_midi_output_instrument;
        private Label label_instrument;
        private GroupBox groupBox_other_devices;
        private Label label_stepper_motor_octave;
        private GroupBox groupBox_type_of_device;
        private RadioButton radioButtonDCMotorOrBuzzer;
        private RadioButton radioButtonStepperMotor;
        private CheckBox checkBox_use_microcontroller;
        private TrackBar trackBar_stepper_motor_octave;
        private CheckBox checkBoxClassicBleeperMode;
        private Button markup_color_change;
        private Label label1;
        private FlowLayoutPanel flowLayoutPanelGeneralSettings;
        private Panel panelSystemSpeakerWarnings;
        private GroupBox groupBoxCreateMusicWithAI;
        private Button buttonShowHide;
        private Label labelAPIKeyWarning;
        private Label labelAPIKey;
        private TextBox textBoxAPIKey;
        private FlowLayoutPanel flowLayoutPanelDevicesSettings;
        private FlowLayoutPanel flowLayoutPanelAppearanceSettings;
        private FlowLayoutPanel flowLayoutPanelCreatingSoundSettings;
        public CheckBox checkBox_enable_create_beep_from_soundcard;
        private GroupBox group_beep_creation_from_sound_card_settings;
        private FlowLayoutPanel flowLayoutPanelSoundDeviceBeepEnabledInfo;
        private Panel panel1;
        public Label label_create_beep_from_soundcard_automatically_activated_message_2;
        public Label label_create_beep_from_soundcard_automatically_activated_message_1;
        public Button button_show_reason;
        private GroupBox group_tone_waveform;
        private RadioButton radioButton_noise;
        private RadioButton radioButton_triangle;
        private RadioButton radioButton_sine;
        private RadioButton radioButton_square;
        private Panel panel2;
        private GroupBox group_lyrics_size_settings;
        private Label label13;
        private NumericUpDown numericUpDownLyricsSize;
        private Label labelPt;
        private Button buttonPreviewLyrics;
        private GroupBox groupBoxLanguageSettings;
        private ComboBox comboBoxLanguage;
        private Label labelLanguage;
        private Panel third_octave_color;
        private Panel second_octave_color;
        private Panel first_octave_color;
        private Panel markup_color;
        private Panel metronome_color;
        private Panel playback_buttons_color;
        private Panel erase_whole_line_color;
        private Panel unselect_line_color;
        private Panel clear_notes_color;
        private Panel blank_line_color;
        private Panel beep_indicator_color;
        private Panel note_indicator_color;
        private Button buttonUpdateAPIKey;
        private Button buttonResetAPIKey;
        private Label label_firmware_warning;
        private Button button_get_firmware;
    }
}