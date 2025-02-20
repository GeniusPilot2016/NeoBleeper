﻿using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NeoBleeper.Program;

namespace NeoBleeper
{
    public partial class settings_window : Form
    {
        public delegate void ColorsAndThemeChangedEventHandler(object sender, EventArgs e);
        public event ColorsAndThemeChangedEventHandler ColorsAndThemeChanged;

        PrivateFontCollection fonts = new PrivateFontCollection();
        public settings_window()
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular_Italic.ttf");
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                }
            }
            checkBox_use_motor_speed_mod.Font = new Font(fonts.Families[0], 9);
            label_test_system_speaker_message_2.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            label_test_system_speaker_message_3.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            label_create_beep_from_soundcard_automatically_activated_message_1.Font = new Font(fonts.Families[0], 8, FontStyle.Bold);
            label_create_beep_from_soundcard_automatically_activated_message_2.Font = new Font(fonts.Families[0], 8, FontStyle.Bold);
            label_motor_speed_mod.Font = new Font(fonts.Families[0], 9, FontStyle.Italic);
            if (Program.creating_sounds.create_beep_with_soundcard == true)
            {
                checkBox_enable_create_beep_from_soundcard.Checked = true;
            }
            else if (Program.creating_sounds.create_beep_with_soundcard == false)
            {
                checkBox_enable_create_beep_from_soundcard.Checked = false;
            }
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == false)
            {
                label_test_system_speaker_message_2.Visible = true;
                label_create_beep_from_soundcard_automatically_activated_message_1.Visible = true;
                button_show_reason.Visible = true;
            }
            else
            {
                if (Program.eligability_of_create_beep_from_system_speaker.device_type == 0 ||
                    Program.eligability_of_create_beep_from_system_speaker.device_type == 2)
                {
                    label_test_system_speaker_message_3.Visible = true;
                    label_create_beep_from_soundcard_automatically_activated_message_2.Visible = true;
                    button_show_reason.Visible = true;
                }
            }
            if (Program.creating_sounds.permanently_enabled == true)
            {
                checkBox_enable_create_beep_from_soundcard.Enabled = false;
                groupBox_system_speaker_test.Enabled = false;
            }
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
            set_theme();
            refresh_midi_input();
            refresh_midi_output();
        }
        private void dark_theme()
        {
            Application.DoEvents();
            this.BackColor = Color.Black;
            this.ForeColor = Color.White;
            foreach (TabPage tabPage in tabControl_settings.TabPages)
            {
                tabPage.BackColor = Color.FromArgb(32, 32, 32);
                tabPage.ForeColor = Color.White;
            }
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
            trackBar_motor_octave.BackColor = Color.FromArgb(32, 32, 32);
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
            reset_colors.BackColor = Color.FromArgb(32, 32, 32);
            this.Refresh();
        }
        private void light_theme()
        {
            Application.DoEvents();
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            foreach (TabPage tabPage in tabControl_settings.TabPages)
            {
                tabPage.BackColor = SystemColors.Window;
                tabPage.ForeColor = SystemColors.ControlText;
            }
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
            trackBar_motor_octave.BackColor = SystemColors.Window;
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
            reset_colors.BackColor = Color.Transparent;
            this.Refresh();
        }

        private void set_theme()
        {
            switch (Settings1.Default.theme)
            {
                case 0:
                    {
                        if (check_system_theme.IsDarkTheme() == true)
                        {
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;
                    }
                case 1:
                    {
                        light_theme();
                        break;
                    }
                case 2:
                    {
                        dark_theme();
                        break;
                    }
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox_test_system_speaker_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void settings_window_Load(object sender, EventArgs e)
        {

        }
        private void system_speaker_test_tune()
        {
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                Random rnd = new Random();
                int tune_number = rnd.Next(1, 15); // Choose a random tune between 1 and 15

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
                }
            }
        }

        private void tabControl_settings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                RenderBeep.BeepClass.StopBeep();
            }
        }

        private void btn_test_system_speaker_Click(object sender, EventArgs e)
        {
            system_speaker_test_tune();
        }

        private void comboBox_theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings1.Default.theme = comboBox_theme.SelectedIndex;
            Settings1.Default.Save();
            set_theme();
            ColorsAndThemeChanged?.Invoke(this, new EventArgs());
        }

        private void general_settings_Click(object sender, EventArgs e)
        {
        }

        private void lbl_theme_Click(object sender, EventArgs e)
        {
        }

        private void groupBox_system_speaker_test_Enter(object sender, EventArgs e)
        {
        }

        private void checkBox_enable_create_beep_from_soundcard_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.eligability_of_create_beep_from_system_speaker.device_type == 0 ||
                Program.eligability_of_create_beep_from_system_speaker.device_type == 2)
            {
                if (checkBox_enable_create_beep_from_soundcard.Checked == false)
                {
                    disable_create_beep_from_sound_card_warning disable_Create_Beep_From_Sound_Card_Warning = new disable_create_beep_from_sound_card_warning();
                    DialogResult result = disable_Create_Beep_From_Sound_Card_Warning.ShowDialog();
                    switch (result)
                    {
                        case DialogResult.Yes:
                            Program.creating_sounds.create_beep_with_soundcard = false;
                            break;
                        case DialogResult.No:
                            checkBox_enable_create_beep_from_soundcard.Checked = true;
                            Program.creating_sounds.create_beep_with_soundcard = true;
                            break;
                    }
                }
                else if (checkBox_enable_create_beep_from_soundcard.Checked == true)
                {
                    Program.creating_sounds.create_beep_with_soundcard = true;
                }
            }
            else
            {
                switch (checkBox_enable_create_beep_from_soundcard.Checked)
                {
                    case true:
                        Program.creating_sounds.create_beep_with_soundcard = true;
                        break;
                    case false:
                        Program.creating_sounds.create_beep_with_soundcard = false;
                        break;
                }
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
            }
        }
        private void PlayMinorChord()
        {
            int[] frequencies = { 261, 311, 392 }; // C4, D#4, G4
            foreach (int freq in frequencies)
            {
                RenderBeep.BeepClass.Beep(freq, 500); // 500 ms for each note
            }
        }
        private void PlayMajorChord()
        {
            int[] frequencies = { 261, 329, 392 }; // C4, E4, G4
            foreach (int freq in frequencies)
            {
                RenderBeep.BeepClass.Beep(freq, 500); // 500 ms for each note
            }
        }
        private void PlayDescendingBeeps()
        {
            for (int freq = 2000; freq >= 200; freq -= 200)
            {
                RenderBeep.BeepClass.Beep(freq, 500); // 500 ms for each frequency
            }
        }
        private void PlayAscendingBeeps()
        {
            for (int freq = 200; freq <= 2000; freq += 200)
            {
                RenderBeep.BeepClass.Beep(freq, 500); // 500 ms for each frequency
            }
        }
        private void PlayRandomBeeps()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                int frequency = rnd.Next(200, 2000); // Random frequency between 200 Hz and 2000 Hz
                int duration = rnd.Next(100, 1000); // Random duration between 100 ms and 1000 ms
                RenderBeep.BeepClass.Beep(frequency, duration);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
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
                RenderBeep.BeepClass.Beep(note[0], note[1]);
            }
        }
        private void PlaySimpleBeepSequence()
        {
            RenderBeep.BeepClass.Beep(1000, 500); // Frequency: 1000 Hz, Duration: 500 ms
            RenderBeep.BeepClass.Beep(1500, 500); // Frequency: 1500 Hz, Duration: 500 ms
            RenderBeep.BeepClass.Beep(2000, 500); // Frequency: 2000 Hz, Duration: 500 ms
        }
        private void PlayScale()
        {
            int[] frequencies = { 261, 293, 329, 349, 392, 440, 493, 523 }; // C4 to C5
            foreach (int freq in frequencies)
            {
                RenderBeep.BeepClass.Beep(freq, 500); // 500ms for each note
            }
        }

        private void first_octave_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                first_octave_color.BackColor = colorDialog1.Color;
                Settings1.Default.first_octave_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void second_octave_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                second_octave_color.BackColor = colorDialog1.Color;
                Settings1.Default.second_octave_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void third_octave_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                third_octave_color.BackColor = colorDialog1.Color;
                Settings1.Default.third_octave_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void blank_line_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                blank_line_color.BackColor = colorDialog1.Color;
                Settings1.Default.blank_line_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void clear_notes_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                clear_notes_color.BackColor = colorDialog1.Color;
                Settings1.Default.clear_notes_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void unseelct_line_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                unselect_line_color.BackColor = colorDialog1.Color;
                Settings1.Default.unselect_line_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void erase_whole_line_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                erase_whole_line_color.BackColor = colorDialog1.Color;
                Settings1.Default.erase_whole_line_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void playback_buttons_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                playback_buttons_color.BackColor = colorDialog1.Color;
                Settings1.Default.playback_buttons_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void metronome_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                metronome_color.BackColor = colorDialog1.Color;
                Settings1.Default.metronome_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void beep_indicator_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                beep_indicator_color.BackColor = colorDialog1.Color;
                Settings1.Default.beep_indicator_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void note_indicator_color_change_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            if (colorDialog1.Color != null && result == DialogResult.OK)
            {
                note_indicator_color.BackColor = colorDialog1.Color;
                Settings1.Default.note_indicator_color = colorDialog1.Color;
                Settings1.Default.Save();
                ColorsAndThemeChanged?.Invoke(this, new EventArgs());
            }
        }

        private void reset_colors_Click(object sender, EventArgs e)
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
            Settings1.Default.Save();
            ColorsAndThemeChanged?.Invoke(this, new EventArgs());
        }

        private void refresh_midi_input()
        {
            comboBox_midi_input_devices.Items.Clear();
            for (int device = 0; device < MidiIn.NumberOfDevices; device++)
            {
                string deviceName = MidiIn.DeviceInfo(device).ProductName;
                comboBox_midi_input_devices.Items.Add(deviceName);
            }
            if(comboBox_midi_input_devices.Items.Count > 0)
            {
                label_midi_input_device.Enabled = true;
                comboBox_midi_input_devices.SelectedIndex = 0;
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
                comboBox_midi_output_devices.SelectedIndex = 0;
                comboBox_midi_output_devices.Enabled = true;
                checkBox_use_midi_output.Enabled = true;
            }
            else
            {
                label_midi_output_device.Enabled = false;
                comboBox_midi_output_devices.Enabled = false;
                checkBox_use_midi_output.Enabled = false;
                checkBox_use_midi_output.Checked = false;
            }

            // List of MIDI channels
            comboBox_midi_output_channel.Items.Clear();
            for (int channel = 1; channel <= 16; channel++)
            {
                comboBox_midi_output_channel.Items.Add("Channel " + channel);
            }
            if (comboBox_midi_output_channel.Items.Count > 0)
            {
                label_channel.Enabled = true;
                comboBox_midi_output_channel.SelectedIndex = 0;
                comboBox_midi_output_channel.Enabled = true;
            }
            else
            {
                label_channel.Enabled = false;
                comboBox_midi_output_channel.Enabled = false;
            }

            // List of MIDI instruments
            comboBox_midi_output_instrument.Items.Clear();
            string[] instruments = new string[]
            {
                "Acoustic Grand Piano", "Bright Acoustic Piano", "Electric Grand Piano", "Honky-tonk Piano",
                "Electric Piano 1", "Electric Piano 2", "Harpsichord", "Clavinet", "Celesta", "Glockenspiel",
                "Music Box", "Vibraphone", "Marimba", "Xylophone", "Tubular Bells", "Dulcimer", "Drawbar Organ",
                "Percussive Organ", "Rock Organ", "Church Organ", "Reed Organ", "Accordion", "Harmonica",
                "Tango Accordion", "Acoustic Guitar (nylon)", "Acoustic Guitar (steel)", "Electric Guitar (jazz)",
                "Electric Guitar (clean)", "Electric Guitar (muted)", "Overdriven Guitar", "Distortion Guitar",
                "Guitar harmonics", "Acoustic Bass", "Electric Bass (finger)", "Electric Bass (pick)",
                "Fretless Bass", "Slap Bass 1", "Slap Bass 2", "Synth Bass 1", "Synth Bass 2", "Violin", "Viola",
                "Cello", "Contrabass", "Tremolo Strings", "Pizzicato Strings", "Orchestral Harp", "Timpani",
                "String Ensemble 1", "String Ensemble 2", "SynthStrings 1", "SynthStrings 2", "Choir Aahs",
                "Voice Oohs", "Synth Voice", "Orchestra Hit", "Trumpet", "Trombone", "Tuba", "Muted Trumpet",
                "French Horn", "Brass Section", "SynthBrass 1", "SynthBrass 2", "Soprano Sax", "Alto Sax",
                "Tenor Sax", "Baritone Sax", "Oboe", "English Horn", "Bassoon", "Clarinet", "Piccolo", "Flute",
                "Recorder", "Pan Flute", "Blown Bottle", "Shakuhachi", "Whistle", "Ocarina", "Lead 1 (square)",
                "Lead 2 (sawtooth)", "Lead 3 (calliope)", "Lead 4 (chiff)", "Lead 5 (charang)", "Lead 6 (voice)",
                "Lead 7 (fifths)", "Lead 8 (bass + lead)", "Pad 1 (new age)", "Pad 2 (warm)", "Pad 3 (polysynth)",
                "Pad 4 (choir)", "Pad 5 (bowed)", "Pad 6 (metallic)", "Pad 7 (halo)", "Pad 8 (sweep)",
                "FX 1 (rain)", "FX 2 (soundtrack)", "FX 3 (crystal)", "FX 4 (atmosphere)", "FX 5 (brightness)",
                "FX 6 (goblins)", "FX 7 (echoes)", "FX 8 (sci-fi)", "Sitar", "Banjo", "Shamisen", "Koto",
                "Kalimba", "Bag pipe", "Fiddle", "Shanai", "Tinkle Bell", "Agogo", "Steel Drums", "Woodblock",
                "Taiko Drum", "Melodic Tom", "Synth Drum", "Reverse Cymbal", "Guitar Fret Noise", "Breath Noise",
                "Seashore", "Bird Tweet", "Telephone Ring", "Helicopter", "Applause", "Gunshot"
            };
            foreach (string instrument in instruments)
            {
                comboBox_midi_output_instrument.Items.Add(instrument);
            }
            if (comboBox_midi_output_instrument.Items.Count > 0)
            {
                label_instrument.Enabled = true;
                comboBox_midi_output_instrument.SelectedIndex = 0;
                comboBox_midi_output_instrument.Enabled = true;
            }
            else
            {
                label_instrument.Enabled = false;
                comboBox_midi_output_instrument.Enabled = false;
            }
        }

        private void refresh_midi_input_button_Click(object sender, EventArgs e)
        {
            refresh_midi_input();
        }

        private void refresh_midi_output_button_Click(object sender, EventArgs e)
        {
            refresh_midi_output();
        }
    }
}
