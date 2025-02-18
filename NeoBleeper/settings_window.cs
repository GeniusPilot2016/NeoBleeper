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
            comboBox_theme.SelectedItem = comboBox_theme.Items[0];
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
            switch (comboBox_theme.SelectedIndex)
            {
                case 0:
                    Settings1.Default.theme = "system";
                    Settings1.Default.Save();
                    break;
                case 1:
                    Settings1.Default.theme = "light";
                    Settings1.Default.Save();
                    break;
                case 2:
                    Settings1.Default.theme = "dark";
                    Settings1.Default.Save();
                    break;
            }
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

        private void label_test_system_speaker_message_2_Click(object sender, EventArgs e)
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
        }
    }
}
