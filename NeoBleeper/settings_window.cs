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
                    checkBox_use_motor_speed_mod.Font = new Font(fonts.Families[0], 9);
                    label_test_system_speaker_message_2.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
                    label_test_system_speaker_message_3.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
                    label_create_beep_from_soundcard_automatically_activated_message_1.Font = new Font(fonts.Families[0], 8, FontStyle.Bold);
                    label_create_beep_from_soundcard_automatically_activated_message_2.Font = new Font(fonts.Families[0], 8, FontStyle.Bold);
                    label_motor_speed_mod.Font = new Font(fonts.Families[0], 9, FontStyle.Italic);
                }
            }
            if (Program.creating_sounds.create_beep_with_soundcard == true)
            {
                checkBox_enable_create_beep_from_soundcard.Checked = true;
            }
            else if (Program.creating_sounds.create_beep_with_soundcard == false)
            {
                checkBox_enable_create_beep_from_soundcard.Checked = false;
            }
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == false ||
                    Program.eligability_of_create_beep_from_system_speaker.is_x64_based == false)
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
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true && Program.eligability_of_create_beep_from_system_speaker.is_x64_based == true)
            {
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(1046, 428);
                RenderBeep.BeepClass.Beep(1174, 428);
                RenderBeep.BeepClass.Beep(1174, 428);
                RenderBeep.BeepClass.Beep(1046, 428);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(880, 428);
                RenderBeep.BeepClass.Beep(783, 428);
                RenderBeep.BeepClass.Beep(783, 428);
                RenderBeep.BeepClass.Beep(880, 428);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(987, 642);
                RenderBeep.BeepClass.Beep(880, 214);
                RenderBeep.BeepClass.Beep(880, 856);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(1046, 428);
                RenderBeep.BeepClass.Beep(1174, 428);
                RenderBeep.BeepClass.Beep(1174, 428);
                RenderBeep.BeepClass.Beep(1046, 428);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(880, 428);
                RenderBeep.BeepClass.Beep(783, 428);
                RenderBeep.BeepClass.Beep(783, 428);
                RenderBeep.BeepClass.Beep(880, 428);
                RenderBeep.BeepClass.Beep(987, 428);
                RenderBeep.BeepClass.Beep(880, 642);
                RenderBeep.BeepClass.Beep(783, 214);
                RenderBeep.BeepClass.Beep(783, 856);
            }
        }
        private void checkBox_test_system_speaker_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void tabControl_settings_SelectedIndexChanged(object sender, EventArgs e)
        {
            RenderBeep.BeepClass.StopBeep();
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

        private void settings_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true && Program.eligability_of_create_beep_from_system_speaker.is_x64_based == true)
            {
                RenderBeep.BeepClass.StopBeep();
            }
        }

        private void settings_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true && Program.eligability_of_create_beep_from_system_speaker.is_x64_based == true)
            {
                RenderBeep.BeepClass.StopBeep();
            }
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
    }
}
