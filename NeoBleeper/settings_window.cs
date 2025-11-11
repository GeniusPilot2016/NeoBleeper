// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using NAudio.Midi;
using NeoBleeper.Properties;
using System.Media;
using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public partial class settings_window : Form
    {
        bool darkTheme = false;
        private main_window mainWindow;
        public delegate void ColorsAndThemeChangedEventHandler(object sender, EventArgs e);
        public event ColorsAndThemeChangedEventHandler ColorsAndThemeChanged;
        LyricsOverlay lyricsOverlay = new LyricsOverlay();
        public settings_window(main_window mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            checkBox_use_microcontroller.Enabled = SerialPortHelper.IsAnyPortThatIsMicrocontrollerAvailable(); // Enable or disable the microcontroller checkbox based on availability
            if (!checkBox_use_microcontroller.Enabled)
            {
                checkBox_use_microcontroller.Checked = false;
                TemporarySettings.MicrocontrollerSettings.useMicrocontroller = checkBox_use_microcontroller.Checked;
            }
            switch (TemporarySettings.creating_sounds.soundDeviceBeepWaveform)
            {
                case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Square:
                    radioButton_square.Checked = true;
                    break;
                case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Sine:
                    radioButton_sine.Checked = true;
                    break;
                case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Triangle:
                    radioButton_triangle.Checked = true;
                    break;
                case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Noise:
                    radioButton_noise.Checked = true;
                    break;
                default:
                    radioButton_square.Checked = true;
                    break;
            }

            if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
                checkBox_enable_create_beep_from_soundcard.Checked = TemporarySettings.creating_sounds.create_beep_with_soundcard;
                if (TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present == false)
                {
                    label_test_system_speaker_message_2.Visible = true;
                    label_create_beep_from_soundcard_automatically_activated_message_1.Visible = true;
                    button_show_reason.Visible = true;
                }
                else
                {
                    if (TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType == TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.Unknown ||
                        TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType == TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.CompactComputers)
                    {
                        label_test_system_speaker_message_3.Visible = true;
                        label_create_beep_from_soundcard_automatically_activated_message_2.Visible = true;
                        button_show_reason.Visible = true;
                    }
                    else
                    {
                        flowLayoutPanelGeneralSettings.Controls.RemoveAt(flowLayoutPanelGeneralSettings.Controls.IndexOf(panelSystemSpeakerWarnings));
                        group_beep_creation_from_sound_card_settings.Controls.Remove(flowLayoutPanelSoundDeviceBeepEnabledInfo);
                        group_beep_creation_from_sound_card_settings.Size = new Size(group_beep_creation_from_sound_card_settings.Size.Width, group_beep_creation_from_sound_card_settings.Size.Height - flowLayoutPanelSoundDeviceBeepEnabledInfo.Height);
                    }
                }
            }
            else
            {
                // Hide system speaker related settings on ARM64 systems such as most of Copilot+ devices due to lack of support for system speaker
                groupBox_system_speaker_test.Visible = false;
                checkBox_enable_create_beep_from_soundcard.Visible = false;
            }
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller)
            {
                checkBox_use_microcontroller.Checked = true;
            }
            else
            {
                checkBox_use_microcontroller.Checked = false;
            }
            switch (TemporarySettings.MicrocontrollerSettings.deviceType)
            {
                case TemporarySettings.MicrocontrollerSettings.DeviceType.StepperMotor:
                    radioButtonStepperMotor.Checked = true;
                    break;
                case TemporarySettings.MicrocontrollerSettings.DeviceType.DCMotorOrBuzzer:
                    radioButtonDCMotorOrBuzzer.Checked = true;
                    break;
            }
            trackBar_stepper_motor_octave.Value = TemporarySettings.MicrocontrollerSettings.stepperMotorOctave;
            first_octave_color.BackColor = Settings1.Default.first_octave_color;
            second_octave_color.BackColor = Settings1.Default.second_octave_color;
            third_octave_color.BackColor = Settings1.Default.third_octave_color;
            blank_line_color.BackColor = Settings1.Default.blank_line_color;
            clear_notes_color.BackColor = Settings1.Default.clear_notes_color;
            unselect_line_color.BackColor = Settings1.Default.unselect_line_color;
            erase_whole_line_color.BackColor = Settings1.Default.erase_whole_line_color;
            playback_buttons_color.BackColor = Settings1.Default.playback_buttons_color;
            metronome_color.BackColor = Settings1.Default.metronome_color;
            beep_indicator_color.BackColor = Settings1.Default.beep_indicator_color;
            note_indicator_color.BackColor = Settings1.Default.note_indicator_color;
            comboBox_theme.SelectedIndex = Settings1.Default.theme;
            checkBox_use_midi_input.Checked = TemporarySettings.MIDIDevices.useMIDIinput;
            checkBox_use_midi_output.Checked = TemporarySettings.MIDIDevices.useMIDIoutput;
            checkBoxClassicBleeperMode.Checked = Settings1.Default.ClassicBleeperMode;
            comboBoxLanguage.SelectedItem = Settings1.Default.preferredLanguage;
            UIFonts.setFonts(this);
            set_theme();
            refresh_midi_input();
            refresh_midi_output();
            if (!CreateMusicWithAI.IsAvailableInCountry())
            {
                groupBoxCreateMusicWithAI.Visible = false; // Hide the group box if the feature is not available in the user's country
                Logger.Log("Create Music with AI feature is not available in this country. Hiding the settings.", Logger.LogTypes.Info);
            }
            else
            {
                if (IsPotentiallyPaidApiCountry())
                {
                    labelGoogleGeminiAPIWarning.Visible = true;
                }
            }
            textBoxAPIKey.Text = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
            if (Settings1.Default.geminiAPIKey != String.Empty)
            {
                buttonResetAPIKey.Enabled = true;
            }
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SETTINGCHANGE = 0x001A;
            base.WndProc(ref m);

            if (m.Msg == WM_SETTINGCHANGE)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme();
                }
            }
        }
        private static readonly HashSet<string> PotentiallyPaidApiCountries = new()
        {
            // European Economic Area (EEA) countries (ISO 3166-1 alpha-2 codes)
            "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR", "DE", "GR", "HU",
            "IS", "IE", "IT", "LV", "LI", "LT", "LU", "MT", "NL", "NO", "PL", "PT", "RO",
            "SK", "SI", "ES", "SE",
            // Additionally Switzerland and the United Kingdom
            "CH", "GB"
        };

        public static bool IsPotentiallyPaidApiCountry()
        {
            try
            {
                string countryCode = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;
                return PotentiallyPaidApiCountries.Contains(countryCode);
            }
            catch (Exception ex)
            {
                Logger.Log("Error determining user's country for API pricing: " + ex.Message, Logger.LogTypes.Error);
                return false; // Default to false if there's an error
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
            foreach (TabPage tabPage in tabControl_settings.TabPages)
            {
                tabPage.BackColor = Color.FromArgb(32, 32, 32);
                tabPage.ForeColor = Color.White;
            }
            groupBoxLanguageSettings.ForeColor = Color.White;
            comboBoxLanguage.BackColor = Color.Black;
            comboBoxLanguage.ForeColor = Color.White;
            groupBox_appearance.ForeColor = Color.White;
            btn_test_system_speaker.BackColor = Color.FromArgb(32, 32, 32);
            btn_test_system_speaker.ForeColor = Color.White;
            groupBox_system_speaker_test.ForeColor = Color.White;
            comboBox_theme.BackColor = Color.Black;
            comboBox_theme.ForeColor = Color.White;
            group_tone_waveform.ForeColor = Color.White;
            group_beep_creation_from_sound_card_settings.ForeColor = Color.White;
            button_show_reason.BackColor = Color.FromArgb(32, 32, 32);
            button_show_reason.ForeColor = Color.White;
            button_show_reason.BackColor = Color.FromArgb(32, 32, 32);
            button_show_reason.ForeColor = Color.White;
            group_midi_input_devices.ForeColor = Color.White;
            comboBox_midi_input_devices.BackColor = Color.Black;
            comboBox_midi_input_devices.ForeColor = Color.White;
            refresh_midi_input_button.BackColor = Color.FromArgb(32, 32, 32);
            group_midi_output_devices.ForeColor = Color.White;
            comboBox_midi_output_devices.BackColor = Color.Black;
            comboBox_midi_output_devices.ForeColor = Color.White;
            refresh_midi_input_button.BackColor = Color.FromArgb(32, 32, 32);
            comboBox_midi_output_channel.BackColor = Color.Black;
            comboBox_midi_output_channel.ForeColor = Color.White;
            comboBox_midi_output_instrument.BackColor = Color.Black;
            comboBox_midi_output_instrument.ForeColor = Color.White;
            refresh_midi_output_button.BackColor = Color.FromArgb(32, 32, 32);
            groupBox_other_devices.ForeColor = Color.White;
            groupBox_type_of_device.ForeColor = Color.White;
            trackBar_stepper_motor_octave.BackColor = Color.FromArgb(32, 32, 32);
            group_keyboard_colors.ForeColor = Color.White;
            group_buttons_and_controls_colors.ForeColor = Color.White;
            group_indicator_colors.ForeColor = Color.White;
            first_octave_color_change.BackColor = Color.FromArgb(32, 32, 32);
            second_octave_color_change.BackColor = Color.FromArgb(32, 32, 32);
            third_octave_color_change.BackColor = Color.FromArgb(32, 32, 32);
            blank_line_color_change.BackColor = Color.FromArgb(32, 32, 32);
            clear_notes_color_change.BackColor = Color.FromArgb(32, 32, 32);
            unselect_line_color_change.BackColor = Color.FromArgb(32, 32, 32);
            erase_whole_line_color_change.BackColor = Color.FromArgb(32, 32, 32);
            playback_buttons_color_change.BackColor = Color.FromArgb(32, 32, 32);
            metronome_color_change.BackColor = Color.FromArgb(32, 32, 32);
            beep_indicator_color_change.BackColor = Color.FromArgb(32, 32, 32);
            note_indicator_color_change.BackColor = Color.FromArgb(32, 32, 32);
            reset_appearance_settings.BackColor = Color.FromArgb(32, 32, 32);
            buttonUpdateAPIKey.BackColor = Color.FromArgb(32, 32, 32);
            buttonUpdateAPIKey.ForeColor = Color.White;
            textBoxAPIKey.BackColor = Color.Black;
            textBoxAPIKey.ForeColor = Color.White;
            buttonShowHide.BackColor = Color.FromArgb(32, 32, 32);
            buttonShowHide.ForeColor = Color.White;
            buttonResetAPIKey.BackColor = Color.FromArgb(32, 32, 32);
            buttonResetAPIKey.ForeColor = Color.White;
            groupBoxCreateMusicWithAI.ForeColor = Color.White;
            markup_color_change.BackColor = Color.FromArgb(32, 32, 32);
            group_lyrics_size_settings.ForeColor = Color.White;
            numericUpDownLyricsSize.BackColor = Color.Black;
            numericUpDownLyricsSize.ForeColor = Color.White;
            buttonPreviewLyrics.BackColor = Color.FromArgb(32, 32, 32);
            buttonPreviewLyrics.ForeColor = Color.White;
            button_get_firmware.BackColor = Color.FromArgb(32, 32, 32);
            contextMenuStripSystemSpeakerTests.BackColor = Color.Black;
            contextMenuStripSystemSpeakerTests.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            foreach (TabPage tabPage in tabControl_settings.TabPages)
            {
                tabPage.BackColor = SystemColors.Window;
                tabPage.ForeColor = SystemColors.ControlText;
            }
            groupBoxLanguageSettings.ForeColor = SystemColors.ControlText;
            comboBoxLanguage.BackColor = SystemColors.Window;
            comboBoxLanguage.ForeColor = SystemColors.WindowText;
            groupBox_appearance.ForeColor = SystemColors.ControlText;
            btn_test_system_speaker.BackColor = Color.Transparent;
            btn_test_system_speaker.ForeColor = SystemColors.ControlText;
            comboBox_theme.BackColor = SystemColors.Window;
            comboBox_theme.ForeColor = SystemColors.WindowText;
            groupBox_system_speaker_test.ForeColor = SystemColors.ControlText;
            group_beep_creation_from_sound_card_settings.ForeColor = SystemColors.ControlText;
            group_tone_waveform.ForeColor = SystemColors.ControlText;
            button_show_reason.BackColor = Color.Transparent;
            button_show_reason.ForeColor = SystemColors.ControlText;
            group_midi_input_devices.ForeColor = SystemColors.ControlText;
            comboBox_midi_input_devices.BackColor = SystemColors.Window;
            comboBox_midi_input_devices.ForeColor = SystemColors.WindowText;
            refresh_midi_input_button.BackColor = Color.Transparent;
            group_midi_output_devices.ForeColor = SystemColors.ControlText;
            comboBox_midi_output_devices.BackColor = SystemColors.Window;
            comboBox_midi_output_devices.ForeColor = SystemColors.WindowText;
            refresh_midi_input_button.BackColor = Color.Transparent;
            comboBox_midi_output_channel.BackColor = SystemColors.Window;
            comboBox_midi_output_channel.ForeColor = SystemColors.WindowText;
            comboBox_midi_output_instrument.BackColor = SystemColors.Window;
            comboBox_midi_output_instrument.ForeColor = SystemColors.WindowText;
            refresh_midi_output_button.BackColor = Color.Transparent;
            groupBox_other_devices.ForeColor = SystemColors.ControlText;
            groupBox_type_of_device.ForeColor = SystemColors.ControlText;
            trackBar_stepper_motor_octave.BackColor = SystemColors.Window;
            group_keyboard_colors.ForeColor = SystemColors.ControlText;
            group_buttons_and_controls_colors.ForeColor = SystemColors.ControlText;
            group_indicator_colors.ForeColor = SystemColors.ControlText;
            first_octave_color_change.BackColor = Color.Transparent;
            second_octave_color_change.BackColor = Color.Transparent;
            third_octave_color_change.BackColor = Color.Transparent;
            blank_line_color_change.BackColor = Color.Transparent;
            clear_notes_color_change.BackColor = Color.Transparent;
            unselect_line_color_change.BackColor = Color.Transparent;
            erase_whole_line_color_change.BackColor = Color.Transparent;
            playback_buttons_color_change.BackColor = Color.Transparent;
            metronome_color_change.BackColor = Color.Transparent;
            beep_indicator_color_change.BackColor = Color.Transparent;
            note_indicator_color_change.BackColor = Color.Transparent;
            reset_appearance_settings.BackColor = Color.Transparent;
            buttonUpdateAPIKey.BackColor = Color.Transparent;
            buttonUpdateAPIKey.ForeColor = SystemColors.ControlText;
            textBoxAPIKey.BackColor = SystemColors.Window;
            textBoxAPIKey.ForeColor = SystemColors.WindowText;
            buttonShowHide.BackColor = Color.Transparent;
            buttonShowHide.ForeColor = SystemColors.ControlText;
            buttonResetAPIKey.BackColor = Color.Transparent;
            buttonResetAPIKey.ForeColor = SystemColors.ControlText;
            groupBoxCreateMusicWithAI.ForeColor = SystemColors.ControlText;
            markup_color_change.BackColor = Color.Transparent;
            group_lyrics_size_settings.ForeColor = SystemColors.ControlText;
            numericUpDownLyricsSize.BackColor = SystemColors.Window;
            numericUpDownLyricsSize.ForeColor = SystemColors.WindowText;
            buttonPreviewLyrics.BackColor = Color.Transparent;
            buttonPreviewLyrics.ForeColor = SystemColors.ControlText;
            button_get_firmware.BackColor = Color.Transparent;
            contextMenuStripSystemSpeakerTests.BackColor = SystemColors.Window;
            contextMenuStripSystemSpeakerTests.ForeColor = SystemColors.WindowText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void set_theme()
        {
            this.SuspendLayout(); // Suspend layout to batch updates
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering

            try
            {
                switch (Settings1.Default.theme)
                {
                    case 0:
                        if (SystemThemeUtility.IsDarkTheme())
                        {
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;

                    case 1:
                        light_theme();
                        break;

                    case 2:
                        dark_theme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
            }
        }
        private async Task system_speaker_test_tune()
        {
            if (!isTestingSystemSpeaker)
            // Debounce check to prevent multiple clicks to test the system speaker
            {
                isTestingSystemSpeaker = true; // Set the flag to true to indicate that the system speaker is being tested
                Random rnd = new Random();
                int tune_number = rnd.Next(1, 24); // Random number between 1 and 23
                await Task.Run(() =>
                {
                    switch (tune_number)
                    {
                        case 1:
                            PlaySimpleBeepSequence();
                            break;
                        case 2:
                            PlayScale();
                            break;
                        case 3:
                            PlayTwinkleTwinkle();
                            break;
                        case 4:
                            PlayBeethovenFifth();
                            break;
                        case 5:
                            PlayHappyBirthday();
                            break;
                        case 6:
                            PlayRandomBeeps();
                            break;
                        case 7:
                            PlayAscendingBeeps();
                            break;
                        case 8:
                            PlayDescendingBeeps();
                            break;
                        case 9:
                            PlayMajorChord();
                            break;
                        case 10:
                            PlayMinorChord();
                            break;
                        case 11:
                            PlayFrereJacques();
                            break;
                        case 12:
                            PlayYankeeDoodle();
                            break;
                        case 13:
                            PlayOdeToJoy();
                            break;
                        case 14:
                            PlayMaryHadALittleLamb();
                            break;
                        case 15:
                            PlayJingleBells();
                            break;
                        case 16:
                            PlayFurElise();
                            break;
                        case 17:
                            PlayTetrisTheme();
                            break;
                        case 18:
                            PlayCanonInD();
                            break;
                        case 19:
                            PlaySuperMarioTheme();
                            break;
                        case 20:
                            PlayTheEntertainer();
                            break;
                        case 21:
                            PlayGreensleeves();
                            break;
                        case 22:
                            PlayRowRowRowYourBoat();
                            break;
                        case 23:
                            PlayMinuetInG();
                            break;
                    }
                });
                isTestingSystemSpeaker = false; // Reset the flag after the tune is played
            }
            else
            {
                SystemSounds.Beep.Play(); // Play a simple beep sound to indicate that a test is already in progress
                Logger.Log("System speaker test is already in progress. Ignoring additional test request.", Logger.LogTypes.Warning);
            }
        }
        private void tabControl_settings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
            }
        }
        bool isTestingSystemSpeaker = false;
        private void btn_test_system_speaker_Click(object sender, EventArgs e)
        {
            contextMenuStripSystemSpeakerTests.Show(btn_test_system_speaker, new Point(0, btn_test_system_speaker.Height));
        }

        private void comboBox_theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_theme.SelectedIndex != Settings1.Default.theme)
            {
                var synchronizedSettings = SynchronizedSettings.Load();
                string[] theme_names = { "System Theme", "Light Theme", "Dark Theme" };
                Settings1.Default.theme = comboBox_theme.SelectedIndex;
                Settings1.Default.Save();
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    // Update synchronized settings for Beep Stopper if not running on ARM64 architecture
                    synchronizedSettings.Theme = Settings1.Default.theme;
                }
                set_theme();
                Logger.Log("Theme changed to: " + theme_names[comboBox_theme.SelectedIndex], Logger.LogTypes.Info);
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void checkBox_enable_create_beep_from_soundcard_CheckedChanged(object sender, EventArgs e)
        {
            if ((TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType == TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.Unknown ||
                TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType == TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.CompactComputers) &&
                TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                if (checkBox_enable_create_beep_from_soundcard.Checked == false)
                {
                    disable_create_beep_from_sound_device_warning_on_certain_types warningForm = new disable_create_beep_from_sound_device_warning_on_certain_types();
                    DisableSoundDeviceBeepWithWarning(warningForm);
                }
                else if (checkBox_enable_create_beep_from_soundcard.Checked == true)
                {
                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                    Logger.Log("Beep creation from sound card enabled.", Logger.LogTypes.Info);
                }
            }
            else if (TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_manufacturer_of_motherboard_affecting_system_speaker_issues)
            {
                if (checkBox_enable_create_beep_from_soundcard.Checked == false)
                {
                    disable_create_beep_from_sound_device_warning_on_affected_motherboards warningForm = new disable_create_beep_from_sound_device_warning_on_affected_motherboards();
                    DisableSoundDeviceBeepWithWarning(warningForm);
                }
                else if (checkBox_enable_create_beep_from_soundcard.Checked == true)
                {
                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                    Logger.Log("Beep creation from sound card enabled.", Logger.LogTypes.Info);
                }
            }
            else if (TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present == false)
            {
                if (checkBox_enable_create_beep_from_soundcard.Checked == false)
                {
                    disable_create_beep_from_sound_device_warning_on_computers_without_system_speaker_output warningForm = new disable_create_beep_from_sound_device_warning_on_computers_without_system_speaker_output();
                    DisableSoundDeviceBeepWithWarning(warningForm);
                }
                else if (checkBox_enable_create_beep_from_soundcard.Checked == true)
                {
                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                    Logger.Log("Beep creation from sound card enabled.", Logger.LogTypes.Info);
                }
            }
            else
            {
                switch (checkBox_enable_create_beep_from_soundcard.Checked)
                {
                    case true:
                        TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                        Logger.Log("Beep creation from sound card enabled.", Logger.LogTypes.Info);
                        break;
                    case false:
                        TemporarySettings.creating_sounds.create_beep_with_soundcard = false;
                        Logger.Log("Beep creation from sound card disabled.", Logger.LogTypes.Info);
                        break;
                }
            }
        }
        private void DisableSoundDeviceBeepWithWarning(Form form)
        {
            if (!Settings1.Default.dont_show_disable_create_beep_from_soundcard_warnings_again)
            {
                Logger.Log("User is trying to disable \"Use Sound Device to Create Beep\" feature in system that's a compact computer or unknown type of computer.", Logger.LogTypes.Warning);
                DialogResult result = form.ShowDialog();
                switch (result)
                {
                    case DialogResult.Yes:
                        TemporarySettings.creating_sounds.create_beep_with_soundcard = false;
                        Logger.Log("User chose to disable beep creation from sound card.", Logger.LogTypes.Info);
                        Logger.Log("Beep creation from sound card disabled.", Logger.LogTypes.Info);
                        break;
                    case DialogResult.No:
                        checkBox_enable_create_beep_from_soundcard.Checked = true;
                        TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                        Logger.Log("User chose to keep beep creation from sound card enabled.", Logger.LogTypes.Info);
                        Logger.Log("Beep creation from sound card enabled.", Logger.LogTypes.Info);
                        break;
                }
            }
            else
            {
                TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                Logger.Log("Beep creation from sound card enabled.", Logger.LogTypes.Info);
            }
        }
        private void PlayFurElise()
        {
            int[][] melody = new int[][]
            {
                new int[] { 659, 300 }, // E5
                new int[] { 622, 300 }, // D#5
                new int[] { 659, 300 }, // E5
                new int[] { 622, 300 }, // D#5
                new int[] { 659, 300 }, // E5
                new int[] { 494, 300 }, // B4
                new int[] { 587, 300 }, // D5
                new int[] { 523, 300 }, // C5
                new int[] { 440, 600 }, // A4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayTetrisTheme()
        {
            int[][] melody = new int[][]
            {
                new int[] { 659, 300 }, // E5
                new int[] { 494, 300 }, // B4
                new int[] { 523, 300 }, // C5
                new int[] { 587, 300 }, // D5
                new int[] { 523, 300 }, // C5
                new int[] { 494, 300 }, // B4
                new int[] { 440, 300 }, // A4
                new int[] { 440, 300 }, // A4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlaySuperMarioTheme()
        {
            int[][] melody = new int[][]
            {
                new int[] { 660, 100 }, // E5
                new int[] { 660, 100 }, // E5
                new int[] { 660, 100 }, // E5
                new int[] { 510, 100 }, // B4
                new int[] { 660, 100 }, // E5
                new int[] { 770, 100 }, // G5
                new int[] { 380, 100 }, // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayCanonInD()
        {
            int[][] melody = new int[][]
            {
                new int[] { 293, 500 }, // D4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }  // E4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayTheEntertainer()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 300 }, // G4
                new int[] { 440, 300 }, // A4
                new int[] { 392, 300 }, // G4
                new int[] { 349, 300 }, // F4
                new int[] { 329, 300 }, // E4
                new int[] { 349, 300 }, // F4
                new int[] { 392, 600 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayMinuetInG()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 1000 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayGreensleeves()
        {
            int[][] melody = new int[][]
            {
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 1000 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayRowRowRowYourBoat()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 1000 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }


        private void PlayJingleBells()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 1000 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 1000 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 494, 500 }, // B4
                new int[] { 261, 500 }, // C5
                new int[] { 329, 500 }, // E5
                new int[] { 392, 1000 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayMaryHadALittleLamb()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 500 }, // G4
                new int[] { 329, 500 }, // E4
                new int[] { 293, 500 }, // D4
                new int[] { 329, 500 }, // E4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 1000 }, // G4
                new int[] { 329, 500 }, // E4
                new int[] { 329, 500 }, // E4
                new int[] { 329, 1000 }, // E4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 440, 1000 }  // A4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayOdeToJoy()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 293, 500 }, // D4
                new int[] { 261, 500 }, // C4
                new int[] { 293, 500 }, // D4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayYankeeDoodle()
        {
            int[][] melody = new int[][]
            {
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 293, 500 }, // D4
                new int[] { 261, 500 }, // C4
                new int[] { 293, 500 }, // D4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }  // G4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayFrereJacques()
        {
            int[][] melody = new int[][]
            {
                new int[] { 261, 500 }, // C4
                new int[] { 293, 500 }, // D4
                new int[] { 329, 500 }, // E4
                new int[] { 261, 500 }, // C4
                new int[] { 261, 500 }, // C4
                new int[] { 293, 500 }, // D4
                new int[] { 329, 500 }, // E4
                new int[] { 261, 500 }, // C4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 500 }, // G4
                new int[] { 329, 500 }, // E4
                new int[] { 349, 500 }, // F4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 261, 500 }, // C4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 500 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 261, 500 }  // C4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayMinorChord()
        {
            int[] frequencies = { 261, 311, 392 }; // C4, D#4, G4
            foreach (int freq in frequencies)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(freq, 500); // 500 ms for each note
            }
        }
        private void PlayMajorChord()
        {
            int[] frequencies = { 261, 329, 392 }; // C4, E4, G4
            foreach (int freq in frequencies)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(freq, 500); // 500 ms for each note
            }
        }
        private void PlayDescendingBeeps()
        {
            for (int freq = 2000; freq >= 200; freq -= 200)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(freq, 500); // 500 ms for each frequency
            }
        }
        private void PlayAscendingBeeps()
        {
            for (int freq = 200; freq <= 2000; freq += 200)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(freq, 500); // 500 ms for each frequency
            }
        }
        private void PlayRandomBeeps()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                int frequency = rnd.Next(200, 2000); // Random frequency between 200 Hz and 2000 Hz
                int duration = rnd.Next(100, 1000); // Random duration between 100 ms and 1000 ms
                NotePlayer.PlayOnlySystemSpeakerBeep(frequency, duration);
            }
        }
        private void PlayHappyBirthday()
        {
            int[][] melody = new int[][]
            {
                new int[] { 264, 125 }, // C4
                new int[] { 264, 125 }, // C4
                new int[] { 297, 250 }, // D4
                new int[] { 264, 250 }, // C4
                new int[] { 352, 250 }, // F4
                new int[] { 330, 500 }, // E4
                new int[] { 264, 125 }, // C4
                new int[] { 264, 125 }, // C4
                new int[] { 297, 250 }, // D4
                new int[] { 264, 250 }, // C4
                new int[] { 396, 250 }, // G4
                new int[] { 352, 500 }  // F4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayBeethovenFifth()
        {
            int[][] melody = new int[][]
            {
                new int[] { 523, 250 }, // C5
                new int[] { 523, 250 }, // C5
                new int[] { 523, 250 }, // C5
                new int[] { 415, 1000 }, // G#4
                new int[] { 466, 250 }, // A#4
                new int[] { 466, 250 }, // A#4
                new int[] { 466, 250 }, // A#4
                new int[] { 349, 1000 }  // F4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlayTwinkleTwinkle()
        {
            int[][] melody = new int[][]
            {
                new int[] { 261, 500 }, // C4
                new int[] { 261, 500 }, // C4
                new int[] { 392, 500 }, // G4
                new int[] { 392, 500 }, // G4
                new int[] { 440, 500 }, // A4
                new int[] { 440, 500 }, // A4
                new int[] { 392, 1000 }, // G4
                new int[] { 349, 500 }, // F4
                new int[] { 349, 500 }, // F4
                new int[] { 329, 500 }, // E4
                new int[] { 329, 500 }, // E4
                new int[] { 293, 500 }, // D4
                new int[] { 293, 500 }, // D4
                new int[] { 261, 1000 }  // C4
            };

            foreach (int[] note in melody)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(note[0], note[1]);
            }
        }
        private void PlaySimpleBeepSequence()
        {
            NotePlayer.PlayOnlySystemSpeakerBeep(1000, 500); // Frequency: 1000 Hz, Duration: 500 ms
            NotePlayer.PlayOnlySystemSpeakerBeep(1500, 500); // Frequency: 1500 Hz, Duration: 500 ms
            NotePlayer.PlayOnlySystemSpeakerBeep(2000, 500); // Frequency: 2000 Hz, Duration: 500 ms
        }
        private void PlayScale()
        {
            int[] frequencies = { 261, 293, 329, 349, 392, 440, 493, 523 }; // C4 to C5
            foreach (int freq in frequencies)
            {
                NotePlayer.PlayOnlySystemSpeakerBeep(freq, 500); // 500ms for each note
            }
        }

        private void first_octave_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.first_octave_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                first_octave_color.BackColor = colorDialog1.Color;
                Settings1.Default.first_octave_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("First octave color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void second_octave_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.second_octave_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                second_octave_color.BackColor = colorDialog1.Color;
                Settings1.Default.second_octave_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Second octave color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void third_octave_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.third_octave_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                third_octave_color.BackColor = colorDialog1.Color;
                Settings1.Default.third_octave_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Third octave color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void blank_line_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.blank_line_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                blank_line_color.BackColor = colorDialog1.Color;
                Settings1.Default.blank_line_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Blank line color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void clear_notes_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.clear_notes_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                clear_notes_color.BackColor = colorDialog1.Color;
                Settings1.Default.clear_notes_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Clear notes color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void unseelct_line_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.unselect_line_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                unselect_line_color.BackColor = colorDialog1.Color;
                Settings1.Default.unselect_line_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Unselect line color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void erase_whole_line_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.erase_whole_line_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                erase_whole_line_color.BackColor = colorDialog1.Color;
                Settings1.Default.erase_whole_line_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Erase whole line color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void playback_buttons_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.playback_buttons_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                playback_buttons_color.BackColor = colorDialog1.Color;
                Settings1.Default.playback_buttons_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Playback buttons color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void metronome_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.metronome_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                metronome_color.BackColor = colorDialog1.Color;
                Settings1.Default.metronome_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Metronome color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void beep_indicator_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.beep_indicator_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                beep_indicator_color.BackColor = colorDialog1.Color;
                Settings1.Default.beep_indicator_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Beep indicator color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void note_indicator_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.note_indicator_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                note_indicator_color.BackColor = colorDialog1.Color;
                Settings1.Default.note_indicator_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Note indicator color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void reset_appearance_settings_Click(object sender, EventArgs e) // Reset to default values
        {
            first_octave_color.BackColor = Settings1.Default.first_octave_color = Color.FromArgb(255, 224, 192);
            second_octave_color.BackColor = Settings1.Default.second_octave_color = Color.FromArgb(192, 192, 255);
            third_octave_color.BackColor = Settings1.Default.third_octave_color = Color.FromArgb(192, 255, 192);
            blank_line_color.BackColor = Settings1.Default.blank_line_color = Color.FromArgb(255, 224, 192);
            clear_notes_color.BackColor = Settings1.Default.clear_notes_color = Color.FromArgb(128, 128, 255);
            unselect_line_color.BackColor = Settings1.Default.unselect_line_color = Color.FromArgb(128, 255, 255);
            erase_whole_line_color.BackColor = Settings1.Default.erase_whole_line_color = Color.FromArgb(255, 128, 128);
            playback_buttons_color.BackColor = Settings1.Default.playback_buttons_color = Color.FromArgb(128, 255, 128);
            metronome_color.BackColor = Settings1.Default.metronome_color = Color.FromArgb(192, 255, 192);
            beep_indicator_color.BackColor = Settings1.Default.beep_indicator_color = Color.Red;
            note_indicator_color.BackColor = Settings1.Default.note_indicator_color = Color.Red;
            markup_color.BackColor = Settings1.Default.markup_color = Color.LightBlue;
            Settings1.Default.lyricsSize = 32;
            numericUpDownLyricsSize.Value = 32;
            Settings1.Default.Save();
            ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            Logger.Log("Colors reset to default.", Logger.LogTypes.Info);
        }

        private void refresh_midi_input()
        {
            comboBox_midi_input_devices.Items.Clear();
            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                string deviceName = MidiIn.DeviceInfo(device).ProductName;
                comboBox_midi_input_devices.Items.Add(deviceName);
            }
            if (comboBox_midi_input_devices.Items.Count > 0)
            {
                label_midi_input_device.Enabled = true;
                comboBox_midi_input_devices.SelectedIndex = TemporarySettings.MIDIDevices.MIDIInputDevice;
                comboBox_midi_input_devices.Enabled = true;
                checkBox_use_midi_input.Enabled = true;
            }
            else
            {
                label_midi_input_device.Enabled = false;
                comboBox_midi_input_devices.Enabled = false;
                checkBox_use_midi_input.Enabled = false;
                checkBox_use_midi_input.Checked = false;
            }
            Logger.Log("MIDI input devices refreshed.", Logger.LogTypes.Info);
        }

        private void refresh_midi_output()
        {
            comboBox_midi_output_devices.Items.Clear();
            for (int device = 0; device < MidiOut.NumberOfDevices; device++)
            {
                string deviceName = MidiOut.DeviceInfo(device).ProductName;
                comboBox_midi_output_devices.Items.Add(deviceName);
            }
            if (comboBox_midi_output_devices.Items.Count > 0)
            {
                label_midi_output_device.Enabled = true;
                if (TemporarySettings.MIDIDevices.MIDIOutputDevice >= 0 && TemporarySettings.MIDIDevices.MIDIOutputDevice < comboBox_midi_output_devices.Items.Count)
                {
                    comboBox_midi_output_devices.SelectedIndex = TemporarySettings.MIDIDevices.MIDIOutputDevice;
                    comboBox_midi_output_devices.Enabled = true;
                    checkBox_use_midi_output.Enabled = true;
                }
                else if (TemporarySettings.MIDIDevices.MIDIOutputDevice >= 0)
                {
                    comboBox_midi_output_devices.SelectedIndex = 0;
                    comboBox_midi_output_devices.Enabled = true;
                    checkBox_use_midi_output.Enabled = true;
                }
                else
                {
                    comboBox_midi_output_devices.SelectedIndex = -1; // Or handle the invalid case appropriately
                }
                label_channel.Enabled = true;
                comboBox_midi_output_channel.SelectedIndex = TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel;
                comboBox_midi_output_channel.Enabled = true;
                label_instrument.Enabled = true;
                comboBox_midi_output_instrument.SelectedIndex = TemporarySettings.MIDIDevices.MIDIOutputInstrument;
                comboBox_midi_output_instrument.Enabled = true;
            }
            else
            {
                comboBox_midi_output_channel.SelectedIndex = -1;
                comboBox_midi_output_instrument.SelectedIndex = -1;
                label_midi_output_device.Enabled = false;
                comboBox_midi_output_devices.Enabled = false;
                checkBox_use_midi_output.Enabled = false;
                checkBox_use_midi_output.Checked = false;
                label_channel.Enabled = false;
                comboBox_midi_output_channel.Enabled = false;
                label_instrument.Enabled = false;
                comboBox_midi_output_instrument.Enabled = false;
            }
            Logger.Log("MIDI output devices refreshed.", Logger.LogTypes.Info);
        }

        private void refresh_midi_input_button_Click(object sender, EventArgs e)
        {
            refresh_midi_input();
        }

        private void refresh_midi_output_button_Click(object sender, EventArgs e)
        {
            refresh_midi_output();
        }

        private void soundcard_beep_waveform_selection(object sender, EventArgs e)
        {
            if (radioButton_square.Checked == true)
            {
                TemporarySettings.creating_sounds.soundDeviceBeepWaveform = TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Square;
                SoundRenderingEngine.WaveSynthEngine.SquareWave(0, 0, false); // Dummy beep for prevent unintended delay just before playing the beep
                Logger.Log("Square waveform selected.", Logger.LogTypes.Info);
            }
            else if (radioButton_sine.Checked == true)
            {
                TemporarySettings.creating_sounds.soundDeviceBeepWaveform = TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Sine;
                SoundRenderingEngine.WaveSynthEngine.SineWave(0, 0, false); // Dummy beep for prevent unintended delay just before playing the beep
                Logger.Log("Sine waveform selected.", Logger.LogTypes.Info);
            }
            else if (radioButton_triangle.Checked == true)
            {
                TemporarySettings.creating_sounds.soundDeviceBeepWaveform = TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Triangle;
                SoundRenderingEngine.WaveSynthEngine.TriangleWave(0, 0, false); // Dummy beep for prevent unintended delay just before playing the beep
                Logger.Log("Triangle waveform selected.", Logger.LogTypes.Info);
            }
            else if (radioButton_noise.Checked == true)
            {
                TemporarySettings.creating_sounds.soundDeviceBeepWaveform = TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Noise;
                SoundRenderingEngine.WaveSynthEngine.PlayFilteredNoise(0, 0, false); // Dummy beep for prevent unintended delay just before playing the beep
                Logger.Log("Noise waveform selected.", Logger.LogTypes.Info);
            }
        }

        private void checkBox_use_midi_output_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_use_midi_output.Checked == true)
            {
                TemporarySettings.MIDIDevices.useMIDIoutput = true;
                Logger.Log("MIDI output device enabled", Logger.LogTypes.Info);
            }
            else
            {
                TemporarySettings.MIDIDevices.useMIDIoutput = false;
                Logger.Log("MIDI output device disabled", Logger.LogTypes.Info);
            }
        }

        private void comboBox_midi_output_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            TemporarySettings.MIDIDevices.MIDIOutputDevice = comboBox_midi_output_devices.SelectedIndex;
            MIDIIOUtils.ChangeOutputDevice(TemporarySettings.MIDIDevices.MIDIOutputDevice);
            Logger.Log("MIDI output device selected: " + comboBox_midi_output_devices.SelectedItem.ToString(), Logger.LogTypes.Info);
        }

        private void comboBox_midi_output_channel_SelectedIndexChanged(object sender, EventArgs e)
        {
            TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel = comboBox_midi_output_channel.SelectedIndex;
            Logger.Log("MIDI output channel selected: " + comboBox_midi_output_channel.SelectedItem.ToString(), Logger.LogTypes.Info);
        }

        private void comboBox_midi_output_instrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            TemporarySettings.MIDIDevices.MIDIOutputInstrument = comboBox_midi_output_instrument.SelectedIndex;
            MIDIIOUtils.ChangeInstrument(MIDIIOUtils._midiOut, TemporarySettings.MIDIDevices.MIDIOutputInstrument,
            TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel);
            Logger.Log("MIDI output instrument selected: " + comboBox_midi_output_instrument.SelectedItem.ToString(), Logger.LogTypes.Info);
        }

        private void checkBox_use_midi_input_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_use_midi_input.Checked == true)
            {
                TemporarySettings.MIDIDevices.useMIDIinput = true;
                Logger.Log("MIDI input device enabled", Logger.LogTypes.Info);
            }
            else
            {
                TemporarySettings.MIDIDevices.useMIDIinput = false;
                Logger.Log("MIDI input device disabled", Logger.LogTypes.Info);
            }

            // Notify that MIDI status has changed
            TemporarySettings.MIDIDevices.NotifyMidiStatusChanged();
        }

        private void comboBox_midi_input_devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedDeviceIndex = comboBox_midi_input_devices.SelectedIndex;
            TemporarySettings.MIDIDevices.MIDIInputDevice = selectedDeviceIndex;

            // Update the MIDI input device in the main window
            if (mainWindow != null)
            {
                mainWindow.UpdateMidiInputDevice(TemporarySettings.MIDIDevices.MIDIInputDevice);
            }

            // Notify other components about the MIDI device change
            TemporarySettings.MIDIDevices.NotifyMidiStatusChanged();
        }
        private void checkBox_use_motor_speed_mod_CheckedChanged(object sender, EventArgs e)
        {
            TemporarySettings.MicrocontrollerSettings.useMicrocontroller = checkBox_use_microcontroller.Checked;
            if (checkBox_use_microcontroller.Checked == true)
            {
                Logger.Log("Microcontroller modulation is enabled.", Logger.LogTypes.Info);
            }
            else
            {
                Logger.Log("Microcontroller modulation is disabled.", Logger.LogTypes.Info);
            }
        }

        private void trackBar_motor_octave_Scroll(object sender, EventArgs e)
        {
            TemporarySettings.MicrocontrollerSettings.stepperMotorOctave = trackBar_stepper_motor_octave.Value;
            Logger.Log("Stepper motor octave: " + trackBar_stepper_motor_octave.Value, Logger.LogTypes.Info);
        }

        private void buttonShowHide_Click(object sender, EventArgs e)
        {
            if (buttonShowHide.Text == Properties.Resources.TextShow)
            {
                buttonShowHide.Text = Properties.Resources.TextHide;
                toolTip1.SetToolTip(buttonShowHide, Properties.Resources.HideAPIKeyToolTip);
                buttonShowHide.ImageIndex = 13;
            }
            else
            {
                buttonShowHide.Text = Properties.Resources.TextShow;
                toolTip1.SetToolTip(buttonShowHide, Properties.Resources.ShowAPIKeyToolTip);
                buttonShowHide.ImageIndex = 12;
            }
            if (textBoxAPIKey.UseSystemPasswordChar == false)
            {
                textBoxAPIKey.UseSystemPasswordChar = true;
            }
            else
            {
                textBoxAPIKey.UseSystemPasswordChar = false;
            }
        }

        // Replace the buttonUpdateAPIKey_Click method
        private void buttonUpdateAPIKey_Click(object sender, EventArgs e)
        {
            try
            {
                if (CreateMusicWithAI.isAPIKeyValidFormat(textBoxAPIKey.Text))
                {
                    Action acceptAction = () =>
                    { // Generate new encryption keys first
                        EncryptionHelper.ChangeKeyAndIV();

                        // Now encrypt and save the API key with the new keys
                        Settings1.Default.geminiAPIKey = EncryptionHelper.EncryptString(textBoxAPIKey.Text);
                        Settings1.Default.Save();

                        buttonUpdateAPIKey.Enabled = false;
                        buttonResetAPIKey.Enabled = true;
                        MessageForm.Show(Resources.GoogleGeminiAPIKeySaved, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Logger.Log("API key saved successfully with new encryption keys", Logger.LogTypes.Info);
                    };
                    Action rejectAction = () =>
                    {
                        textBoxAPIKey.Text = string.Empty;
                        Logger.Log("User rejected the Google Gemini™ Terms of Service. API key not saved.", Logger.LogTypes.Info);
                    };
                    if (!Settings1.Default.googleGeminiTermsOfServiceAccepted)
                    {
                        GoogleGeminiTermsOfServiceAgreement.AskToAgreeTermsAndDoAction(acceptAction, rejectAction);
                    }
                    else
                    {
                        acceptAction();
                    }
                }
                else
                {
                    Logger.Log("Attempted to save an invalid API key format", Logger.LogTypes.Error);
                    MessageForm.Show(Resources.GoogleGeminiAPIKeyFormatInvalid, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error saving API key: " + ex.Message, Logger.LogTypes.Error);
                MessageForm.Show(Resources.ErrorSavingAPIKey + ex.Message, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void textBoxAPIKey_TextChanged(object sender, EventArgs e)
        {
            if (textBoxAPIKey.Text != string.Empty && textBoxAPIKey.Text != EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey))
            {
                buttonUpdateAPIKey.Enabled = true;
            }
            else
            {
                buttonUpdateAPIKey.Enabled = false;
            }
        }

        private void buttonResetAPIKey_Click(object sender, EventArgs e)
        {
            try
            {
                Settings1.Default.geminiAPIKey = string.Empty;
                Settings1.Default.Save();
                EncryptionHelper.ChangeKeyAndIV();
                textBoxAPIKey.Text = string.Empty;
                buttonUpdateAPIKey.Enabled = false;
                buttonResetAPIKey.Enabled = false;
                Logger.Log("Google Gemini™ API key reset successfully.", Logger.LogTypes.Info);
                MessageForm.Show(Resources.GoogleGeminiAPIKeyReset, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.Log("Error resetting API key: " + ex.Message, Logger.LogTypes.Error);
                MessageForm.Show(Resources.ErrorResettingAPIKey + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxClassicBleeperMode_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.ClassicBleeperMode = checkBoxClassicBleeperMode.Checked;
            Settings1.Default.Save();
            if (Settings1.Default.ClassicBleeperMode == true)
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
                Logger.Log("Classic Bleeper Mode enabled.", Logger.LogTypes.Info);
            }
            else
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
                Logger.Log("Classic Bleeper Mode disabled.", Logger.LogTypes.Info);
            }
        }

        private void markup_color_change_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Settings1.Default.markup_color;
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                markup_color.BackColor = colorDialog1.Color;
                Settings1.Default.markup_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
                Logger.Log("Keyboard markup color changed to: " + colorDialog1.Color.ToString(), Logger.LogTypes.Info);
            }
        }

        private void deviceTypeRadioButtons_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb == radioButtonStepperMotor)
                {
                    TemporarySettings.MicrocontrollerSettings.deviceType = TemporarySettings.MicrocontrollerSettings.DeviceType.StepperMotor;
                    Logger.Log("Device type set to Stepper Motor.", Logger.LogTypes.Info);
                }
                else if (rb == radioButtonDCMotorOrBuzzer)
                {
                    TemporarySettings.MicrocontrollerSettings.deviceType = TemporarySettings.MicrocontrollerSettings.DeviceType.DCMotorOrBuzzer;
                    Logger.Log("Device type set to DC Motor or Buzzer.", Logger.LogTypes.Info);
                }
            }
        }

        private void settings_window_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void settings_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isTestingSystemSpeaker)
            {
                e.Cancel = true; // Prevent closing while testing system speaker
            }
            if (lyricsOverlay != null && !lyricsOverlay.IsDisposed && !lyricsOverlay.Disposing)
            {
                lyricsOverlay.Close();
                lyricsOverlay.Dispose();
            }
        }

        private void button_show_reason_Click(object sender, EventArgs e)
        {
            string language = Settings1.Default.preferredLanguage;
            string link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING.md#2-system-speaker-detection--compatibility";
            switch (language)
            {
                case "English":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING.md#2-system-speaker-detection--compatibility";
                    break;
                case "Deutsch":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-de.md#2-systemlautsprechererkennung-und--kompatibilit%C3%A4t";
                    break;
                case "Français":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-fr.md#2-d%C3%A9tection-et-compatibilit%C3%A9-des-haut-parleurs-syst%C3%A8me";
                    break;
                case "Español":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-es.md#2-detecci%C3%B3n-y-compatibilidad-de-altavoces-del-sistema";
                    break;
                case "Italiano":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-it.md#2-rilevamento-e-compatibilit%C3%A0-degli-altoparlanti-di-sistema";
                    break;
                case "Türkçe":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-tr.md#2-sistem-hoparl%C3%B6r%C3%BC-alg%C4%B1lama-ve-uyumluluk";
                    break;
                case "Русский":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-ru.md#2-%D0%BE%D0%B1%D0%BD%D0%B0%D1%80%D1%83%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5-%D0%B8-%D1%81%D0%BE%D0%B2%D0%BC%D0%B5%D1%81%D1%82%D0%B8%D0%BC%D0%BE%D1%81%D1%82%D1%8C-%D1%81%D0%B8%D1%81%D1%82%D0%B5%D0%BC%D0%BD%D1%8B%D1%85-%D0%B4%D0%B8%D0%BD%D0%B0%D0%BC%D0%B8%D0%BA%D0%BE%D0%B2";
                    break;
                case "українська":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-ukr.md#2-%D0%B2%D0%B8%D1%8F%D0%B2%D0%BB%D0%B5%D0%BD%D0%BD%D1%8F-%D1%82%D0%B0-%D1%81%D1%83%D0%BC%D1%96%D1%81%D0%BD%D1%96%D1%81%D1%82%D1%8C-%D1%81%D0%B8%D1%81%D1%82%D0%B5%D0%BC%D0%BD%D0%B8%D1%85-%D0%B4%D0%B8%D0%BD%D0%B0%D0%BC%D1%96%D0%BA%D1%96%D0%B2";
                    break;
                case "Tiếng Việt":
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING-vn.md#2-ph%C3%A1t-hi%E1%BB%87n-v%C3%A0-t%C6%B0%C6%A1ng-th%C3%ADch-loa-h%E1%BB%87-th%E1%BB%91ng";
                    break;
                default:
                    link = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/TROUBLESHOOTING.md#2-system-speaker-detection--compatibility";
                    break;
            }
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(link) { UseShellExecute = true });
        }

        private void numericUpDownLyricsSize_ValueChanged(object sender, EventArgs e)
        {
            if ((float)numericUpDownLyricsSize.Value != Settings1.Default.lyricsSize)
            {
                try
                {
                    Settings1.Default.lyricsSize = (float)numericUpDownLyricsSize.Value;
                    Settings1.Default.Save();
                    Logger.Log($"Lyrics/Text events size is changed to {Settings1.Default.lyricsSize} pt", Logger.LogTypes.Info);
                }
                catch (Exception ex)
                {
                    Logger.Log($"An error occurred while lyrics/text event size is changing: {ex.Message}", Logger.LogTypes.Error);
                    MessageForm.Show($"{Resources.MessageAnErrorOccurred} {ex.Message}", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void buttonPreviewLyrics_Click(object sender, EventArgs e) // Preview lyrics/text events
        {
            try
            {
                Logger.Log("Previewing lyrics/text events...", Logger.LogTypes.Info);
                if (lyricsOverlay != null && !lyricsOverlay.IsDisposed && !lyricsOverlay.Disposing)
                {
                    lyricsOverlay.Show();
                    this.BringToFront();
                    lyricsOverlay.PrintLyrics(Resources.LyricsExampleText);
                    await HighPrecisionSleep.SleepAsync(1000);
                    lyricsOverlay.Hide();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"An error occurred while showing lyrics: {ex.Message}", Logger.LogTypes.Error);
                MessageForm.Show($"{Resources.MessageAnErrorOccurred} {ex.Message}", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static bool willRestartForChanges = false; // Flag to indicate if the application will restart for changes to take effect
        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguage.SelectedItem.ToString() != Settings1.Default.preferredLanguage)
            {
                var synchronizedSettings = SynchronizedSettings.Load();
                Settings1.Default.preferredLanguage = comboBoxLanguage.SelectedItem.ToString();
                Settings1.Default.Save();
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    // Also update synchronized settings for Beep Stopper if not on ARM64 architecture
                    synchronizedSettings.Language = Settings1.Default.preferredLanguage;
                }
                willRestartForChanges = true; // Set the flag to true to indicate restart is needed
                MessageForm.Show(Resources.MessageLanguageChanged, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Restart(); // Restart the application to apply the new language
            }
        }

        private void button_get_firmware_Click(object sender, EventArgs e)
        {
            GetFirmwareWindow getFirmwareWindow = new GetFirmwareWindow();
            getFirmwareWindow.ShowDialog();
        }

        private async void standardTuneTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Logger.Log("Testing system speaker with standard tune test...", Logger.LogTypes.Info);
            await system_speaker_test_tune();
        }

        private void advancedSystemSpeakerTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!isTestingSystemSpeaker)
            // Debounce check to prevent multiple clicks to test the system speaker
            {
                isTestingSystemSpeaker = true; // Set the flag to true to indicate that the system speaker is being tested
                Logger.Log("Opening advanced system speaker test window...", Logger.LogTypes.Info);
                isTestingSystemSpeaker = true; // Set the flag to true to indicate that the system speaker is being tested
                AdvancedSystemSpeakerTest advancedSystemSpeakerTest = new AdvancedSystemSpeakerTest();
                advancedSystemSpeakerTest.ShowDialog();
                isTestingSystemSpeaker = false; // Reset the flag after the tune is played
                isTestingSystemSpeaker = false; // Reset the flag after the tune is played
            }
            else
            {
                SystemSounds.Beep.Play(); // Play a simple beep sound to indicate that a test is already in progress
                Logger.Log("System speaker test is already in progress. Ignoring additional test request.", Logger.LogTypes.Warning);
            }
        }
    }
}