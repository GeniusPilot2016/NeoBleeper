using Sanford.Multimedia;
using System.Drawing.Text;
using System.Windows.Forms;
using static NeoBleeper.base_note_length_timer;
using static NeoBleeper.main_window;
using NeoBleeper.Properties;
using System.Runtime.InteropServices;
using Windows.Devices.Usb;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.Design;
using static NeoBleeper.RenderBeep;
using NUnit.Framework;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace NeoBleeper
{
    public partial class main_window : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        Stopwatch stopwatch = new Stopwatch();
        public Boolean is_note_playing = false;
        public static class Variables
        {
            public static int octave;
            public static int bpm;
            public static int miliseconds_per_beat;
            public static int alternating_note_length;
            public static double note_silence_ratio;
        }
        double note_length;
        double total_note_length;
        int note_count;
        double final_note_length;
        int final_note_count;
        public main_window()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            foreach (Control ctrl in Controls)
            {
                ctrl.Font = new Font(fonts.Families[0], 9);
            }
            lbl_alternating_note_options.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            label_mods.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            lbl_note_silence_ratio.Font = new Font(fonts.Families[0], 11, FontStyle.Bold);
            lbl_time_signature.Font = new Font(fonts.Families[0], 11, FontStyle.Bold);
            label_beep.Font = new Font(fonts.Families[0], 14, FontStyle.Bold);
            lbl_c3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_d3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_e3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_f3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_g3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_a3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_b3.Font = new Font(fonts.Families[0], 9.75F);
            lbl_c4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_d4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_e4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_f4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_g4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_a4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_b4.Font = new Font(fonts.Families[0], 9.75F);
            lbl_c5.Font = new Font(fonts.Families[0], 9.75F);
            lbl_d5.Font = new Font(fonts.Families[0], 9.75F);
            lbl_e5.Font = new Font(fonts.Families[0], 9.75F);
            lbl_f5.Font = new Font(fonts.Families[0], 9.75F);
            lbl_g5.Font = new Font(fonts.Families[0], 9.75F);
            lbl_a5.Font = new Font(fonts.Families[0], 9.75F);
            lbl_b5.Font = new Font(fonts.Families[0], 9.75F);
            btn_octave_decrease.Font = new Font(fonts.Families[0], 12);
            btn_octave_increase.Font = new Font(fonts.Families[0], 12);
            menuStrip1.Font = new Font(fonts.Families[0], 9);
            listViewNotes.Font = new Font(fonts.Families[0], 9);
            numericUpDown_bpm.Font = new Font(fonts.Families[0], 9);
            numericUpDown_alternating_notes.Font = new Font(fonts.Families[0], 9);
            label_note_length.Font = new Font(fonts.Families[0], 9);
            comboBox_note_length.Font = new Font(fonts.Families[0], 9);
            label_alternating_notes_switch.Font = new Font(fonts.Families[0], 9);
            lbl_ms.Font = new Font(fonts.Families[0], 9);
            label_bpm.Font = new Font(fonts.Families[0], 9);
            position_table.Font = new Font(fonts.Families[0], 9);
            newToolStripMenuItem1.Font = new Font(fonts.Families[0], 9);
            openToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            saveToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            saveAsToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            playMIDIFileToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            undoToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            redoToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            rewindToSavedVersionToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            fileToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            editToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            settingsToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            aboutNeoBleeperToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            helpToolStripMenuItem.Font = new Font(fonts.Families[0], 9);
            lbl_measure.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            lbl_measure_value.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            lbl_beat.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            lbl_beat_value.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            lbl_beat_traditional.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            lbl_beat_traditional_value.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            button_c3.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            button_c_s3.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_d3.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_d_s3.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_e3.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_f3.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_f_s3.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_g3.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_g_s3.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_a3.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_a_s3.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_b3.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_c4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_c_s4.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_d4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_d_s4.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_e4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_f4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_f_s4.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_g4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_g_s4.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_a4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_a_s4.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_b4.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_c5.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
            button_c_s5.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_d5.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_d_s5.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_e5.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_f5.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_f_s5.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_g5.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_g_s5.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_a5.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            button_a_s5.Font = new Font(fonts.Families[0], 8.249999F, FontStyle.Bold);
            button_b5.Font = new Font(fonts.Families[0], 11.249998F, FontStyle.Bold);
            notes_list_right_click.Font = new Font(fonts.Families[0], 9);
            listViewNotes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == false)
            {
                checkBox_mute_system_speaker.Checked = true;
                checkBox_mute_system_speaker.Enabled = false;
            }
            set_keyboard_colors();
            set_buttons_colors();
            set_beep_label_color();
            comboBox_note_length.SelectedItem = comboBox_note_length.Items[3];
            comboBox_note_length.SelectedValue = comboBox_note_length.Items[3];
            Variables.octave = 4;
            Variables.bpm = 140;
            Variables.alternating_note_length = 30;
            Variables.note_silence_ratio = 0.5;
        }

        private void set_keyboard_colors()
        {
            lbl_c3.BackColor = Settings1.Default.first_octave_color;
            lbl_d3.BackColor = Settings1.Default.first_octave_color;
            lbl_e3.BackColor = Settings1.Default.first_octave_color;
            lbl_f3.BackColor = Settings1.Default.first_octave_color;
            lbl_g3.BackColor = Settings1.Default.first_octave_color;
            lbl_a3.BackColor = Settings1.Default.first_octave_color;
            lbl_b3.BackColor = Settings1.Default.first_octave_color;
            lbl_c4.BackColor = Settings1.Default.second_octave_color;
            lbl_d4.BackColor = Settings1.Default.second_octave_color;
            lbl_e4.BackColor = Settings1.Default.second_octave_color;
            lbl_f4.BackColor = Settings1.Default.second_octave_color;
            lbl_g4.BackColor = Settings1.Default.second_octave_color;
            lbl_a4.BackColor = Settings1.Default.second_octave_color;
            lbl_b4.BackColor = Settings1.Default.second_octave_color;
            lbl_c5.BackColor = Settings1.Default.third_octave_color;
            lbl_d5.BackColor = Settings1.Default.third_octave_color;
            lbl_e5.BackColor = Settings1.Default.third_octave_color;
            lbl_f5.BackColor = Settings1.Default.third_octave_color;
            lbl_g5.BackColor = Settings1.Default.third_octave_color;
            lbl_a5.BackColor = Settings1.Default.third_octave_color;
            lbl_b5.BackColor = Settings1.Default.third_octave_color;
            lbl_c3.ForeColor = set_text_color.GetTextColor(lbl_c3.BackColor);
            lbl_d3.ForeColor = set_text_color.GetTextColor(lbl_d3.BackColor);
            lbl_e3.ForeColor = set_text_color.GetTextColor(lbl_e3.BackColor);
            lbl_f3.ForeColor = set_text_color.GetTextColor(lbl_f3.BackColor);
            lbl_g3.ForeColor = set_text_color.GetTextColor(lbl_g3.BackColor);
            lbl_a3.ForeColor = set_text_color.GetTextColor(lbl_a3.BackColor);
            lbl_b3.ForeColor = set_text_color.GetTextColor(lbl_b3.BackColor);
            lbl_c4.ForeColor = set_text_color.GetTextColor(lbl_c4.BackColor);
            lbl_d4.ForeColor = set_text_color.GetTextColor(lbl_d4.BackColor);
            lbl_e4.ForeColor = set_text_color.GetTextColor(lbl_e4.BackColor);
            lbl_f4.ForeColor = set_text_color.GetTextColor(lbl_f4.BackColor);
            lbl_g4.ForeColor = set_text_color.GetTextColor(lbl_g4.BackColor);
            lbl_a4.ForeColor = set_text_color.GetTextColor(lbl_a4.BackColor);
            lbl_b4.ForeColor = set_text_color.GetTextColor(lbl_b4.BackColor);
            lbl_c5.ForeColor = set_text_color.GetTextColor(lbl_c5.BackColor);
            lbl_d5.ForeColor = set_text_color.GetTextColor(lbl_d5.BackColor);
            lbl_e5.ForeColor = set_text_color.GetTextColor(lbl_e5.BackColor);
            lbl_f5.ForeColor = set_text_color.GetTextColor(lbl_f5.BackColor);
            lbl_g5.ForeColor = set_text_color.GetTextColor(lbl_g5.BackColor);
            lbl_a5.ForeColor = set_text_color.GetTextColor(lbl_a5.BackColor);
            lbl_b5.ForeColor = set_text_color.GetTextColor(lbl_b5.BackColor);
        }
        private void set_buttons_colors()
        {
            button_blank_line.BackColor = Settings1.Default.blank_line_color;
            button_clear_note1.BackColor = Settings1.Default.clear_notes_color;
            button_clear_note2.BackColor = Settings1.Default.clear_notes_color;
            button_clear_note3.BackColor = Settings1.Default.clear_notes_color;
            button_clear_note4.BackColor = Settings1.Default.clear_notes_color;
            button_unselect.BackColor = Settings1.Default.unselect_line_color;
            button_erase_line.BackColor = Settings1.Default.erase_whole_line_color;
            button_play_all.BackColor = Settings1.Default.playback_buttons_color;
            button_play_from_selected_line.BackColor = Settings1.Default.playback_buttons_color;
            button_stop_playing.BackColor = Settings1.Default.playback_buttons_color;
            button_blank_line.ForeColor = set_text_color.GetTextColor(button_blank_line.BackColor);
            button_clear_note1.ForeColor = set_text_color.GetTextColor(button_clear_note1.BackColor);
            button_clear_note2.ForeColor = set_text_color.GetTextColor(button_clear_note2.BackColor);
            button_clear_note3.ForeColor = set_text_color.GetTextColor(button_clear_note3.BackColor);
            button_clear_note4.ForeColor = set_text_color.GetTextColor(button_clear_note4.BackColor);
            button_unselect.ForeColor = set_text_color.GetTextColor(button_unselect.BackColor);
            button_erase_line.ForeColor = set_text_color.GetTextColor(button_erase_line.BackColor);
            button_play_all.ForeColor = set_text_color.GetTextColor(button_play_all.BackColor);
            button_play_from_selected_line.ForeColor = set_text_color.GetTextColor(button_play_from_selected_line.BackColor);
            button_stop_playing.ForeColor = set_text_color.GetTextColor(button_stop_playing.BackColor);
        }
        private void set_beep_label_color()
        {
            label_beep.BackColor = Settings1.Default.beep_indicator_color;
            label_beep.ForeColor = set_text_color.GetTextColor(label_beep.BackColor);

        }
        private void main_window_refresh()
        {
            set_keyboard_colors();
            set_buttons_colors();
            set_beep_label_color();
        }
        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        private void label_time_signature_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox_replace.Checked == true)
            {
                checkBox_replace_length.Enabled = true;
            }
            else if (checkBox_replace.Checked == false)
            {
                checkBox_replace_length.Enabled = false;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void group_key_is_clicked_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }



        private void label_alternating_notes_switch_Click(object sender, EventArgs e)
        {

        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void �mportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutNeoBleeperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about_neobleeper about = new about_neobleeper();
            about.ShowDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings_window settings = new settings_window();
            settings.ShowDialog();
            main_window_refresh();
        }

        private void main_window_Load(object sender, EventArgs e)
        {

        }
        public static class Line
        {
            public static string length;
            public static string note1;
            public static string note2;
            public static string note3;
            public static string note4;
            public static string mod;
            public static string art;
        }
        public static class Mods
        {
            public static bool synchronized_play = false;
        }
        private void add_note()
        {
            if (comboBox_note_length.SelectedIndex == 3 || comboBox_note_length.SelectedIndex == 4 || comboBox_note_length.SelectedIndex == 5)
            {
                Line.length = comboBox_note_length.SelectedItem.ToString();
            }
            if (comboBox_note_length.SelectedIndex == 2)
            {
                Line.length = "Quarter";
            }
            if (comboBox_note_length.SelectedIndex == 1)
            {
                Line.length = "Half";
            }
            if (comboBox_note_length.SelectedIndex == 0)
            {
                Line.length = "Whole";
            }
            string[] note_line = { Line.length, Line.note1, Line.note2, Line.note3, Line.note4, Line.mod, Line.art };
            var listViewItem = new ListViewItem(note_line);
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selectedLine = listViewNotes.SelectedIndices[0];
                listViewNotes.Items[selectedLine].Selected = true;
                listViewNotes.Items.Insert(selectedLine, listViewItem);
            }
            else
            {
                listViewNotes.Items.Add(listViewItem);
            }
        }
        private void add_notes_to_column(string note)
        {
            if (checkBox_add_note_to_list.Checked == true)
            {
                if (add_as_note1.Checked == true)
                {
                    Line.note1 = note;
                    Line.note2 = string.Empty;
                    Line.note3 = string.Empty;
                    Line.note4 = string.Empty;
                }
                if (add_as_note2.Checked == true)
                {
                    Line.note1 = string.Empty;
                    Line.note2 = note;
                    Line.note3 = string.Empty;
                    Line.note4 = string.Empty;
                }
                if (add_as_note3.Checked == true)
                {
                    Line.note1 = string.Empty;
                    Line.note2 = string.Empty;
                    Line.note3 = note;
                    Line.note4 = string.Empty;
                }
                if (add_as_note4.Checked == true)
                {
                    Line.note1 = string.Empty;
                    Line.note2 = string.Empty;
                    Line.note3 = string.Empty;
                    Line.note4 = note;
                }
                add_note();
            }
        }
        private void select_next_line(string note)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selectedLine = listViewNotes.SelectedIndices[0];
                if (selectedLine < listViewNotes.Items.Count - 1)
                {
                    selectedLine++;
                    listViewNotes.Items[selectedLine].Selected = true;
                }
                else if (selectedLine == (listViewNotes.Items.Count - 1))
                {
                    listViewNotes.Items[selectedLine].Selected = false;
                }
            }
            else
            {
                add_notes_to_column(note);
            }
        }
        private void replace_length_in_line()
        {
            if (checkBox_replace_length.Checked == true)
            {
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[3] || comboBox_note_length.SelectedItem == comboBox_note_length.Items[4] || comboBox_note_length.SelectedItem == comboBox_note_length.Items[5])
                {
                    Line.length = comboBox_note_length.SelectedItem.ToString();
                }
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[2])
                {
                    Line.length = "Quarter";
                }
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[1])
                {
                    Line.length = "Half";
                }
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[0])
                {
                    Line.length = "Whole";
                }
                for (int i = 0; i < listViewNotes.Items.Count; i++)
                {
                    if (listViewNotes.Items[i].Selected)
                    {
                        listViewNotes.Items[i].SubItems[0].Text = Line.length;
                    }
                }
            }
        }
        private void replace_note_in_line(string note)
        {
            if (checkBox_add_note_to_list.Checked == true)
            {
                if (add_as_note1.Checked == true)
                {
                    Line.note1 = note;
                    for (int i = 0; i < listViewNotes.Items.Count; i++)
                    {
                        if (listViewNotes.Items[i].Selected)
                        {
                            listViewNotes.Items[i].SubItems[1].Text = note;
                        }
                    }
                }
                if (add_as_note2.Checked == true)
                {
                    Line.note2 = note;
                    for (int i = 0; i < listViewNotes.Items.Count; i++)
                    {
                        if (listViewNotes.Items[i].Selected)
                        {
                            listViewNotes.Items[i].SubItems[2].Text = note;
                        }
                    }
                }
                if (add_as_note3.Checked == true)
                {
                    Line.note3 = note;
                    for (int i = 0; i < listViewNotes.Items.Count; i++)
                    {
                        if (listViewNotes.Items[i].Selected)
                        {
                            listViewNotes.Items[i].SubItems[3].Text = note;
                        }
                    }
                }
                if (add_as_note4.Checked == true)
                {
                    Line.note4 = note;
                    for (int i = 0; i < listViewNotes.Items.Count; i++)
                    {
                        if (listViewNotes.Items[i].Selected)
                        {
                            listViewNotes.Items[i].SubItems[4].Text = note;
                        }
                    }
                }
            }
        }
        int note_frequency;
        private void play_note_when_key_is_clicked(int frequency)
        {
            if (Program.creating_sounds.create_beep_with_soundcard == false)
            {
                if (checkBox_mute_system_speaker.Checked == false)
                {
                    if (frequency >= 37 && frequency <= 32767)
                    {
                        RenderBeep.BeepClass.Beep(frequency, 100);
                    }
                }
            }
        }
        private void button_c3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_c3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_c3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_c3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.C * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_d3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_d3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_d3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_d3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.D * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }
        private void button_e3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_e3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_e3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_e3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.E * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_f3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_f3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_f3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_f3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.F * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_g3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_g3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_g3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_g3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.G * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_a3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_a3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_a3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_a3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.A * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_b3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_b3.Text);
                    replace_length_in_line();
                    select_next_line(lbl_b3.Text);
                }
                else
                {
                    add_notes_to_column(lbl_b3.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.B * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }
        private void button_c4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_c4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_c4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_c4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.C * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_d4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_d4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_d4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_d4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.D * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_e4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_e4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_e4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_e4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.E * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_f4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_f4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_f4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_f4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.F * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_g4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_g4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_f4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_g4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.G * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_a4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_a4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_a4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_a4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.A * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_b4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_b4.Text);
                    replace_length_in_line();
                    select_next_line(lbl_b4.Text);
                }
                else
                {
                    add_notes_to_column(lbl_b4.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.B * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }
        private void button_c5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_c5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_c5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_c5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.C * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_d5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_d5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_d5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_d5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.D * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_e5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_e5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_e5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_e5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.E * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_f5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_f5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_f5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_f5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.F * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_g5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_g5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_g5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_g5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.G * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_a5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_a5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_a5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_a5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.A * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_b5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line(lbl_b5.Text);
                    replace_length_in_line();
                    select_next_line(lbl_b5.Text);
                }
                else
                {
                    add_notes_to_column(lbl_b5.Text);
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.B * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }
        private void button_c_s3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("C#" + (Variables.octave - 1).ToString());
                    replace_length_in_line();
                    select_next_line("C#" + (Variables.octave - 1).ToString());
                }
                else
                {
                    add_notes_to_column("C#" + (Variables.octave - 1).ToString());
                }
            }
            if (checkbox_play_note.Checked == true)
            {
                note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.CS * (Math.Pow(2, (Variables.octave - 5))));
                play_note_when_key_is_clicked(note_frequency);
            }
        }
        private void button_d_s3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("D#" + (Variables.octave - 1).ToString());
                    replace_length_in_line();
                    select_next_line("D#" + (Variables.octave - 1).ToString());
                }
                else
                {
                    add_notes_to_column("D#" + (Variables.octave - 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.DS * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }


        private void button_f_s3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("F#" + (Variables.octave - 1).ToString());
                    replace_length_in_line();
                    select_next_line("F#" + (Variables.octave - 1).ToString());
                }
                else
                {
                    add_notes_to_column("F#" + (Variables.octave - 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.FS * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_g_s3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("G#" + (Variables.octave - 1).ToString());
                    replace_length_in_line();
                    select_next_line("G#" + (Variables.octave - 1).ToString());
                }
                else
                {
                    add_notes_to_column("G#" + (Variables.octave - 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.GS * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_a_s3_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("A#" + (Variables.octave - 1).ToString());
                    replace_length_in_line();
                    select_next_line("A#" + (Variables.octave - 1).ToString());
                }
                else
                {
                    add_notes_to_column("A#" + (Variables.octave - 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.AS * (Math.Pow(2, (Variables.octave - 5))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_c_s4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("C#" + Variables.octave.ToString());
                    replace_length_in_line();
                    select_next_line("C#" + Variables.octave.ToString());
                }
                else
                {
                    add_notes_to_column("C#" + Variables.octave.ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.CS * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_d_s4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("D#" + Variables.octave.ToString());
                    replace_length_in_line();
                    select_next_line("D#" + Variables.octave.ToString());
                }
                else
                {
                    add_notes_to_column("D#" + Variables.octave.ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.DS * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_f_s4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("F#" + Variables.octave.ToString());
                    replace_length_in_line();
                    select_next_line("F#" + Variables.octave.ToString());
                }
                else
                {
                    add_notes_to_column("F#" + Variables.octave.ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.FS * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_g_s4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("G#" + Variables.octave.ToString());
                    replace_length_in_line();
                    select_next_line("G#" + Variables.octave.ToString());
                }
                else
                {
                    add_notes_to_column("G#" + Variables.octave.ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.GS * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_a_s4_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("A#" + Variables.octave.ToString());
                    replace_length_in_line();
                    select_next_line("A#" + Variables.octave.ToString());
                }
                else
                {
                    add_notes_to_column("A#" + Variables.octave.ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.AS * (Math.Pow(2, (Variables.octave - 4))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_c_s5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("C#" + (Variables.octave + 1).ToString());
                    replace_length_in_line();
                    select_next_line("C#" + (Variables.octave + 1).ToString());
                }
                else
                {
                    add_notes_to_column("C#" + (Variables.octave + 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.CS * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_d_s5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("D#" + (Variables.octave + 1).ToString());
                    replace_length_in_line();
                    select_next_line("D#" + (Variables.octave + 1).ToString());

                }
                else
                {
                    add_notes_to_column("D#" + (Variables.octave + 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.DS * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_f_s5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("F#" + (Variables.octave + 1).ToString());
                    replace_length_in_line();
                    select_next_line("F#" + (Variables.octave + 1).ToString());
                }
                else
                {
                    add_notes_to_column("F#" + (Variables.octave + 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.FS * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_g_s5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("G#" + (Variables.octave + 1).ToString());
                    replace_length_in_line();
                    select_next_line("G#" + (Variables.octave + 1).ToString());
                }
                else
                {
                    add_notes_to_column("G#" + (Variables.octave + 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.GS * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }

        private void button_a_s5_Click(object sender, EventArgs e)
        {
            if (is_note_playing == false)
            {
                if (checkBox_replace.Checked == true)
                {
                    replace_note_in_line("A#" + (Variables.octave + 1).ToString());
                    replace_length_in_line();
                    select_next_line("A#" + (Variables.octave + 1).ToString());
                }
                else
                {
                    add_notes_to_column("A#" + (Variables.octave + 1).ToString());
                }
                if (checkbox_play_note.Checked == true)
                {
                    note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.AS * (Math.Pow(2, (Variables.octave - 3))));
                    play_note_when_key_is_clicked(note_frequency);
                }
            }
        }
        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void FileParser(string filename)
        {
            string first_line = File.ReadLines(filename).First();
            if (first_line == "Bleeper Music Maker by Robbi-985 file format")
            {
                try
                {
                    string[] lines = File.ReadAllLines(filename);

                    int noteListStartIndex = Array.IndexOf(lines, "MUSICLISTSTART") + 1;

                    for (int i = 1; i < noteListStartIndex; i++)
                    {
                        if (lines[i].StartsWith("//") || lines[i] == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            string[] parts = lines[i].Split(' ');
                            switch (parts[0])
                            {
                                case "KeyboardOctave":
                                    Variables.octave = Convert.ToInt32(parts[1]);
                                    break;
                                case "BPM":
                                    Variables.bpm = Convert.ToInt32(parts[1]);
                                    numericUpDown_bpm.Value = Convert.ToDecimal(parts[1]);
                                    break;
                                case "TimeSig":
                                    trackBar_time_signature.Value = Convert.ToInt32(parts[1]);
                                    lbl_time_signature.Text = parts[1].ToString();
                                    break;
                                case "NoteSilenceRatio":
                                    Variables.note_silence_ratio = Convert.ToDouble(Convert.ToDouble(parts[1]) / 100);
                                    trackBar_note_silence_ratio.Value = Convert.ToInt32(parts[1]);
                                    lbl_note_silence_ratio.Text = parts[1].ToString() + "%";
                                    break;
                                case "NoteLength":
                                    comboBox_note_length.SelectedIndex = Convert.ToInt32(parts[1]);
                                    break;
                                case "AlternateTime":
                                    Variables.alternating_note_length = Convert.ToInt32(parts[1]);
                                    numericUpDown_alternating_notes.Value = Convert.ToDecimal(parts[1]);
                                    break;
                                case "NoteClickPlay":
                                    checkbox_play_note.Checked = parts[1] == "1";
                                    break;
                                case "NoteClickAdd":
                                    checkBox_add_note_to_list.Checked = parts[1] == "1";
                                    break;
                                case "AddNote1":
                                    add_as_note1.Checked = parts[1] == "True";
                                    break;
                                case "AddNote2":
                                    add_as_note2.Checked = parts[1] == "False";
                                    break;
                                case "NoteReplace":
                                    checkBox_replace.Checked = parts[1] == "1";
                                    break;
                                case "NoteLengthReplace":
                                    checkBox_replace_length.Checked = parts[1] == "1";
                                    break;
                                case "ClickPlayNote1":
                                    checkBox_play_note1_clicked.Checked = parts[1] == "1";
                                    break;
                                case "ClickPlayNote2":
                                    checkBox_play_note2_clicked.Checked = parts[1] == "1";
                                    break;
                                case "PlayNote1":
                                    checkBox_play_note1_played.Checked = parts[1] == "1";
                                    break;
                                case "PlayNote2":
                                    checkBox_play_note2_played.Checked = parts[1] == "1";
                                    break;
                                default:
                                    continue;
                            }
                        }
                    }
                    if (add_as_note1.Checked != true && add_as_note2.Checked != true && add_as_note3.Checked != true && add_as_note4.Checked != true)
                    {
                        add_as_note1.Checked = true;
                    }
                    noteLabelsUpdate();
                    if (Variables.octave == 9)
                    {
                        octave10NoteLabelShiftToRight();
                    }
                    this.Text = System.AppDomain.CurrentDomain.FriendlyName + " - " + filename;
                    listViewNotes.Items.Clear();

                    for (int i = noteListStartIndex; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith("//") || lines[i] == string.Empty)
                        {
                            continue;
                        }
                        else
                        {
                            string[] noteData = lines[i].Split(' ');

                            ListViewItem item = new ListViewItem(noteData[0]); // Note length
                            if (noteData[1] != "-")
                            {
                                item.SubItems.Add(noteData[1]); // Note 1
                            }
                            else
                            {
                                item.SubItems.Add(string.Empty);
                            }
                            if (noteData[2] != "-")
                            {
                                item.SubItems.Add(noteData[2]); // Note 2
                            }
                            else
                            {
                                item.SubItems.Add(string.Empty);
                            }
                            if (noteData.Length == 4)
                            {
                                if (noteData[3] != "-")
                                {
                                    for (int j = 0; j < 2; j++)
                                    {
                                        item.SubItems.Add(string.Empty);
                                    }
                                    item.SubItems.Add(noteData[3]);
                                }
                                else
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        item.SubItems.Add(string.Empty);
                                    }
                                }
                                item.SubItems.Add(string.Empty);
                            }
                            else
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    item.SubItems.Add(string.Empty);
                                }
                            }
                            listViewNotes.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DialogResult dialogResult = MessageBox.Show("This Bleeper Music Maker file contains invalid elements that do not comply with the syntax of Bleeper Music Maker file format. \n\n" +
                        "Do you want to open this file anyway?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult != DialogResult.Yes)
                    {
                        createNewFile();
                    }
                }
            }
            else if (first_line == "<NeoBleeperProjectFile>")
            {
                try
                {
                    NBPML_File.NeoBleeperProjectFile projectFile = DeserializeXML(filename); if (projectFile != null)
                    {

                        Variables.octave = Convert.ToInt32(projectFile.Settings.RandomSettings.KeyboardOctave);
                        Variables.bpm = Convert.ToInt32(projectFile.Settings.RandomSettings.BPM);
                        trackBar_time_signature.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                        lbl_time_signature.Text = projectFile.Settings.RandomSettings.KeyboardOctave;
                        Variables.note_silence_ratio = Convert.ToDouble(Convert.ToDouble(Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio)) / 100);
                        trackBar_note_silence_ratio.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                        lbl_note_silence_ratio.Text = projectFile.Settings.RandomSettings.NoteSilenceRatio + "%";
                        comboBox_note_length.SelectedIndex = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteLength);
                        Variables.alternating_note_length = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
                        numericUpDown_alternating_notes.Value = Convert.ToDecimal(projectFile.Settings.RandomSettings.AlternateTime);
                        checkbox_play_note.Checked = projectFile.Settings.PlaybackSettings.NoteClickPlay == "True";
                        checkBox_add_note_to_list.Checked = projectFile.Settings.PlaybackSettings.NoteClickAdd == "True";
                        add_as_note1.Checked = projectFile.Settings.PlayNotes.PlayNote1 == "True";
                        add_as_note2.Checked = projectFile.Settings.PlayNotes.PlayNote2 == "True";
                        add_as_note3.Checked = projectFile.Settings.PlayNotes.PlayNote3 == "True";
                        add_as_note4.Checked = projectFile.Settings.PlayNotes.PlayNote4 == "True";
                        checkBox_replace.Checked = projectFile.Settings.PlaybackSettings.NoteReplace == "True";
                        checkBox_replace_length.Checked = projectFile.Settings.PlaybackSettings.NoteLengthReplace == "True";
                        checkBox_play_note1_clicked.Checked = projectFile.Settings.ClickPlayNotes.ClickPlayNote1 == "True";
                        checkBox_play_note2_clicked.Checked = projectFile.Settings.ClickPlayNotes.ClickPlayNote2 == "True";
                        checkBox_play_note3_clicked.Checked = projectFile.Settings.ClickPlayNotes.ClickPlayNote3 == "True";
                        checkBox_play_note4_clicked.Checked = projectFile.Settings.ClickPlayNotes.ClickPlayNote4 == "True";
                        checkBox_play_note1_played.Checked = projectFile.Settings.PlayNotes.PlayNote1 == "True";
                        checkBox_play_note2_played.Checked = projectFile.Settings.PlayNotes.PlayNote2 == "True";
                        checkBox_play_note3_played.Checked = projectFile.Settings.PlayNotes.PlayNote3 == "True";
                        checkBox_play_note4_played.Checked = projectFile.Settings.PlayNotes.PlayNote4 == "True";
                        if (add_as_note1.Checked != true && add_as_note2.Checked != true && add_as_note3.Checked != true && add_as_note4.Checked != true)
                        {
                            add_as_note1.Checked = true;
                        }
                        noteLabelsUpdate();
                        if (Variables.octave == 9)
                        {
                            octave10NoteLabelShiftToRight();
                        }
                        this.Text = System.AppDomain.CurrentDomain.FriendlyName + " - " + filename;
                        listViewNotes.Items.Clear();

                        foreach (var line in projectFile.LineList.Lines)
                        {
                            ListViewItem item = new ListViewItem(line.Length);
                            item.SubItems.Add(line.Note1);
                            item.SubItems.Add(line.Note2);
                            item.SubItems.Add(line.Note3);
                            item.SubItems.Add(line.Note4);
                            item.SubItems.Add(line.Mod);
                            item.SubItems.Add(line.Art);
                            listViewNotes.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DialogResult dialogResult = MessageBox.Show("This NeoBleeper file contains invalid elements that do not comply with the syntax of NeoBleeper file format. \n\n" +
                        "Do you want to open this file anyway?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult != DialogResult.Yes)
                    {
                        createNewFile();
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid file", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static NBPML_File.NeoBleeperProjectFile DeserializeXML(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (NBPML_File.NeoBleeperProjectFile)serializer.Deserialize(reader);
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "NeoBleeper Project Markup Language Files|*.NBPML|Bleeper Music Maker Files|*.BMM|All Files|*.*";
            openFileDialog.ShowDialog(this);
            if (openFileDialog.FileName != string.Empty)
            {
                FileParser(openFileDialog.FileName);
            }
        }

        private void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "NeoBleeper Project Markup Language Files|*.NBPML|All Files|*.*"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveToNBPML(saveFileDialog.FileName);
            }
        }

        private void SaveToNBPML(string filename)
        {
            NBPML_File.NeoBleeperProjectFile projectFile = new NBPML_File.NeoBleeperProjectFile
            {
                Settings = new NBPML_File.Settings
                {
                    RandomSettings = new NBPML_File.RandomSettings
                    {
                        KeyboardOctave = Variables.octave.ToString(),
                        BPM = Variables.bpm.ToString(),
                        TimeSignature = trackBar_time_signature.Value.ToString(),
                        NoteSilenceRatio = (Variables.note_silence_ratio * 100).ToString(),
                        NoteLength = comboBox_note_length.SelectedIndex.ToString(),
                        AlternateTime = Variables.alternating_note_length.ToString()
                    },
                    PlaybackSettings = new NBPML_File.PlaybackSettings
                    {
                        NoteClickPlay = checkbox_play_note.Checked.ToString(),
                        NoteClickAdd = checkBox_add_note_to_list.Checked.ToString(),
                        NoteReplace = checkBox_replace.Checked.ToString(),
                        NoteLengthReplace = checkBox_replace_length.Checked.ToString()
                    },
                    PlayNotes = new NBPML_File.PlayNotes
                    {
                        PlayNote1 = add_as_note1.Checked.ToString(),
                        PlayNote2 = add_as_note2.Checked.ToString(),
                        PlayNote3 = add_as_note3.Checked.ToString(),
                        PlayNote4 = add_as_note4.Checked.ToString()
                    },
                    ClickPlayNotes = new NBPML_File.ClickPlayNotes
                    {
                        ClickPlayNote1 = checkBox_play_note1_clicked.Checked.ToString(),
                        ClickPlayNote2 = checkBox_play_note2_clicked.Checked.ToString(),
                        ClickPlayNote3 = checkBox_play_note3_clicked.Checked.ToString(),
                        ClickPlayNote4 = checkBox_play_note4_clicked.Checked.ToString()
                    }
                },
                LineList = new NBPML_File.List
                {
                    Lines = listViewNotes.Items.Cast<ListViewItem>().Select(item => new NBPML_File.Line
                    {
                        Length = item.SubItems[0].Text,
                        Note1 = item.SubItems[1].Text,
                        Note2 = item.SubItems[2].Text,
                        Note3 = item.SubItems[3].Text,
                        Note4 = item.SubItems[4].Text,
                        Mod = item.SubItems[5].Text,
                        Art = item.SubItems[6].Text
                    }).ToArray()
                }
            };

            SerializeXML(filename, projectFile);
        }

        private void trackBar_note_silence_ratio_Scroll(object sender, EventArgs e)
        {
            Variables.note_silence_ratio = (Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100);
            lbl_note_silence_ratio.Text = trackBar_note_silence_ratio.Value.ToString() + "%";
        }

        private void trackBar_time_signature_Scroll(object sender, EventArgs e)
        {
            lbl_time_signature.Text = trackBar_time_signature.Value.ToString();
        }

        private void noteLabelsUpdate()
        {
            lbl_c3.Text = "C" + (Variables.octave - 1).ToString();
            lbl_d3.Text = "D" + (Variables.octave - 1).ToString();
            lbl_e3.Text = "E" + (Variables.octave - 1).ToString();
            lbl_f3.Text = "F" + (Variables.octave - 1).ToString();
            lbl_g3.Text = "G" + (Variables.octave - 1).ToString();
            lbl_a3.Text = "A" + (Variables.octave - 1).ToString();
            lbl_b3.Text = "B" + (Variables.octave - 1).ToString();
            lbl_c4.Text = "C" + Variables.octave.ToString();
            lbl_d4.Text = "D" + Variables.octave.ToString();
            lbl_e4.Text = "E" + Variables.octave.ToString();
            lbl_f4.Text = "F" + Variables.octave.ToString();
            lbl_g4.Text = "G" + Variables.octave.ToString();
            lbl_a4.Text = "A" + Variables.octave.ToString();
            lbl_b4.Text = "B" + Variables.octave.ToString();
            lbl_c5.Text = "C" + (Variables.octave + 1).ToString();
            lbl_d5.Text = "D" + (Variables.octave + 1).ToString();
            lbl_e5.Text = "E" + (Variables.octave + 1).ToString();
            lbl_f5.Text = "F" + (Variables.octave + 1).ToString();
            lbl_g5.Text = "G" + (Variables.octave + 1).ToString();
            lbl_a5.Text = "A" + (Variables.octave + 1).ToString();
            lbl_b5.Text = "B" + (Variables.octave + 1).ToString();
        }
        private void octave10NoteLabelShiftToRight()
        {
            float dX;
            Graphics g = this.CreateGraphics();
            try
            {
                dX = g.DpiX;
            }
            finally
            {
                g.Dispose();
            }
            lbl_c5.Location = new Point(lbl_c5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_c5.Location.Y);
            lbl_d5.Location = new Point(lbl_d5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_d5.Location.Y);
            lbl_e5.Location = new Point(lbl_e5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_e5.Location.Y);
            lbl_f5.Location = new Point(lbl_f5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_f5.Location.Y);
            lbl_g5.Location = new Point(lbl_g5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_g5.Location.Y);
            lbl_a5.Location = new Point(lbl_a5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_a5.Location.Y);
            lbl_b5.Location = new Point(lbl_b5.Location.X + Convert.ToInt32(3 * (dX / 96)), lbl_b5.Location.Y);
        }
        private void octave10NoteLabelShiftToLeft()
        {
            float dX;
            Graphics g = this.CreateGraphics();
            try
            {
                dX = g.DpiX;
            }
            finally
            {
                g.Dispose();
            }
            lbl_c5.Location = new Point(lbl_c5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_c5.Location.Y);
            lbl_d5.Location = new Point(lbl_d5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_d5.Location.Y);
            lbl_e5.Location = new Point(lbl_e5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_e5.Location.Y);
            lbl_f5.Location = new Point(lbl_f5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_f5.Location.Y);
            lbl_g5.Location = new Point(lbl_g5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_g5.Location.Y);
            lbl_a5.Location = new Point(lbl_a5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_a5.Location.Y);
            lbl_b5.Location = new Point(lbl_b5.Location.X - Convert.ToInt32(3 * (dX / 96)), lbl_b5.Location.Y);
        }
        private void btn_octave_decrease_Click(object sender, EventArgs e)
        {
            Variables.octave--;
            noteLabelsUpdate();
            if (Variables.octave == 2)
            {
                btn_octave_decrease.Enabled = false;
            }
            else if (Variables.octave == 8)
            {
                btn_octave_increase.Enabled = true;
            }
            if (Variables.octave == 8)
            {
                octave10NoteLabelShiftToRight();
            }
        }

        private void btn_octave_increase_Click(object sender, EventArgs e)
        {
            Variables.octave++;
            noteLabelsUpdate();
            if (Variables.octave == 9)
            {
                btn_octave_increase.Enabled = false;
            }
            else if (Variables.octave == 3)
            {
                btn_octave_decrease.Enabled = true;
            }
            if (Variables.octave == 9)
            {
                octave10NoteLabelShiftToLeft();

            }
        }


        private void checkBox_dotted_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_triplet.Checked == checkBox_dotted.Checked)
            {
                checkBox_triplet.Checked = false;
            }
            if (checkBox_dotted.Checked == true)
            {
                Line.mod = "Dot";
            }
            else
            {
                Line.mod = string.Empty;

            }
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[5].Text = Line.mod;
                }
            }
        }

        private void checkBox_triplet_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_dotted.Checked == checkBox_triplet.Checked)
            {
                checkBox_dotted.Checked = false;
            }
            if (checkBox_triplet.Checked == true)
            {
                Line.mod = "Tri";
            }
            else
            {
                Line.mod = string.Empty;
            }
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[5].Text = Line.mod;
                }
            }
        }

        private void checkBox_staccato_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_spiccato.Checked == checkBox_staccato.Checked)
            {
                checkBox_spiccato.Checked = false;
            }
            if (checkBox_fermata.Checked == checkBox_staccato.Checked)
            {
                checkBox_fermata.Checked = false;
            }
            if (checkBox_staccato.Checked == true)
            {
                Line.art = "Sta";
            }
            else
            {
                Line.art = string.Empty;
            }
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[6].Text = Line.art;
                }
            }
        }

        private void checkBox_fermata_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_spiccato.Checked == checkBox_fermata.Checked)
            {
                checkBox_spiccato.Checked = false;
            }
            if (checkBox_staccato.Checked == checkBox_fermata.Checked)
            {
                checkBox_staccato.Checked = false;
            }
            if (checkBox_fermata.Checked == true)
            {
                Line.art = "Fer";
            }
            else
            {
                Line.art = string.Empty;
            }
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[6].Text = Line.art;
                }
            }
        }
        private void checkBox_spiccato_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox_staccato.Checked == checkBox_spiccato.Checked)
            {
                checkBox_staccato.Checked = false;
            }
            if (checkBox_fermata.Checked == checkBox_spiccato.Checked)
            {
                checkBox_fermata.Checked = false;
            }
            if (checkBox_spiccato.Checked == true)
            {
                Line.art = "Spi";
            }
            else
            {
                Line.art = string.Empty;
            }
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[6].Text = Line.art;
                }
            }
        }

        private void erase_line()
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].Remove();

                    if (listViewNotes.Items.Count > 0 && i >= 0)
                    {
                        if (i == listViewNotes.Items.Count)
                        {
                            i--;
                        }
                        listViewNotes.Items[i].Selected = true;
                    }
                }
            }
        }
        private void button_erase_line_Click(object sender, EventArgs e)
        {
            erase_line();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createNewFile();
        }
        private void createNewFile()
        {
            this.Text = System.AppDomain.CurrentDomain.FriendlyName;
            if (Variables.octave == 9)
            {
                octave10NoteLabelShiftToRight();
            }
            Variables.octave = 4;
            Variables.bpm = 140;
            Variables.alternating_note_length = 30;
            Variables.note_silence_ratio = 0.5;
            noteLabelsUpdate();
            listViewNotes.Items.Clear();
            if (checkbox_play_note.Checked == false)
            {
                checkbox_play_note.Checked = true;
            }
            if (checkBox_add_note_to_list.Checked == false)
            {
                checkBox_add_note_to_list.Checked = true;
            }
            if (checkBox_replace.Checked == true)
            {
                checkBox_replace.Checked = false;
            }
            if (checkBox_replace_length.Checked == true)
            {
                checkBox_replace_length.Checked = false;
            }
            if (checkBox_mute_system_speaker.Checked == true)
            {
                checkBox_mute_system_speaker.Checked = false;
            }
            if (add_as_note2.Checked == true)
            {
                add_as_note2.Checked = false;
                add_as_note1.Checked = true;
            }
            if (add_as_note3.Checked == true)
            {
                add_as_note3.Checked = false;
                add_as_note1.Checked = true;
            }
            if (add_as_note4.Checked == true)
            {
                add_as_note4.Checked = false;
                add_as_note1.Checked = true;
            }
            if (checkBox_play_note1_clicked.Checked == false)
            {
                checkBox_play_note1_clicked.Checked = true;
            }
            if (checkBox_play_note2_clicked.Checked == false)
            {
                checkBox_play_note2_clicked.Checked = true;
            }
            if (checkBox_play_note3_clicked.Checked == false)
            {
                checkBox_play_note3_clicked.Checked = true;
            }
            if (checkBox_play_note4_clicked.Checked == false)
            {
                checkBox_play_note4_clicked.Checked = true;
            }
            if (checkBox_play_note1_played.Checked == false)
            {
                checkBox_play_note1_played.Checked = true;
            }
            if (checkBox_play_note2_played.Checked == false)
            {
                checkBox_play_note2_played.Checked = true;
            }
            if (checkBox_play_note3_played.Checked == false)
            {
                checkBox_play_note3_played.Checked = true;
            }
            if (checkBox_play_note4_played.Checked == false)
            {
                checkBox_play_note4_played.Checked = true;
            }
            if (radioButtonPlay_alternating_notes1.Checked == false)
            {
                radioButtonPlay_alternating_notes1.Checked = true;
            }
            if (radioButtonPlay_alternating_notes2.Checked == true)
            {
                radioButtonPlay_alternating_notes2.Checked = false;
            }
            numericUpDown_alternating_notes.Value = 30;
            numericUpDown_bpm.Value = 140;
            trackBar_time_signature.Value = 4;
            lbl_time_signature.Text = trackBar_time_signature.Value.ToString();
            lbl_note_silence_ratio.Text = 50 + "%";
            trackBar_note_silence_ratio.Value = 50;
            comboBox_note_length.Text = "1/8";
            if (checkBox_dotted.Checked == true)
            {
                checkBox_dotted.Checked = false;
            }
            if (checkBox_triplet.Checked == true)
            {
                checkBox_triplet.Checked = false;
            }
            if (checkBox_staccato.Checked == true)
            {
                checkBox_staccato.Checked = false;
            }
            if (checkBox_fermata.Checked == true)
            {
                checkBox_fermata.Checked = false;
            }
            if (checkBox_metronome.Checked == true)
            {
                checkBox_metronome.Checked = false;
            }
            if (checkBox_loop.Checked == false)
            {
                checkBox_loop.Checked = true;
            }
            if (checkBox_bleeper_portamento.Checked == true)
            {
                checkBox_bleeper_portamento.Checked = false;
            }
        }
        private void button_play_all_Click(object sender, EventArgs e)
        {

        }

        private void button_play_from_selected_line_Click(object sender, EventArgs e)
        {

        }
        private void button_stop_playing_Click(object sender, EventArgs e)
        {

        }
        private void metronome()
        {
            if (Program.creating_sounds.create_beep_with_soundcard == false && checkBox_mute_system_speaker.Checked == false)
            {
                RenderBeep.BeepClass.Beep(498, 15);
            }
            else
            {
                Thread.Sleep(15);
            }
            while (checkBox_metronome.Checked == true)
            {
                int i = 1;
                UpdateLabelVisible(true);
                if (Program.creating_sounds.create_beep_with_soundcard == false && checkBox_mute_system_speaker.Checked == false)
                {
                    RenderBeep.BeepClass.Beep(1000, 60);
                }
                else
                {
                    Thread.Sleep(60);
                }
                UpdateLabelVisible(false);
                Thread.Sleep(Convert.ToInt32(60000 / Variables.bpm) - 60);
                while (i < trackBar_time_signature.Value && checkBox_metronome.Checked == true)
                {
                    UpdateLabelVisible(true);
                    if (Program.creating_sounds.create_beep_with_soundcard == false && checkBox_mute_system_speaker.Checked == false)
                    {
                        RenderBeep.BeepClass.Beep(498, 60);
                    }
                    else
                    {
                        Thread.Sleep(60);
                    }
                    UpdateLabelVisible(false);
                    Thread.Sleep(Convert.ToInt32(60000 / Variables.bpm) - 60);
                    i++;
                }
            }
        }
        private void UpdateLabelVisible(bool visible)
        {
            if (label_beep.InvokeRequired)
            {
                label_beep.Invoke(new Action(() =>
                label_beep.Visible = visible));
            }
            else
            {
                label_beep.Visible = visible;
            }
        }
        private void checkBox_metronome_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_metronome.Checked == true)
            {
                checkBox_metronome.BackColor = Color.FromArgb(192, 255, 192);
                new Thread(() =>
                {
                    metronome();
                }).Start();
            }
            else
            {
                checkBox_metronome.BackColor = System.Drawing.Color.Transparent;
            }
        }

        private void add_blank_line()
        {
            if (checkBox_replace.Checked == true && checkBox_replace_length.Checked == true)
            {
                replace_length_in_line();
                select_next_line(String.Empty);

            }
            else if (checkBox_replace.Checked)
            {
                select_next_line(String.Empty);
            }
            else
            {
                add_notes_to_column(String.Empty);
            }
        }
        private void clear_note_1()
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[1].Text = string.Empty;
                }
            }
        }
        private void clear_note_2()
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[2].Text = string.Empty;
                }
            }
        }
        private void clear_note_3()
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[3].Text = string.Empty;
                }
            }
        }
        private void clear_note_4()
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].SubItems[4].Text = string.Empty;
                }
            }
        }
        private void unselect_line()
        {
            for (int i = 0; i < listViewNotes.Items.Count; i++)
            {
                if (listViewNotes.Items[i].Selected)
                {
                    listViewNotes.Items[i].Selected = false;
                    i--;
                }
            }
        }
        private void button_blank_line_Click(object sender, EventArgs e)
        {
            add_blank_line();
        }

        private void button_clear_note1_Click(object sender, EventArgs e)
        {
            clear_note_1();
        }

        private void button_clear_note2_Click(object sender, EventArgs e)
        {
            clear_note_2();
        }

        private void button_clear_note3_Click(object sender, EventArgs e)
        {
            clear_note_3();
        }

        private void button_clear_note4_Click(object sender, EventArgs e)
        {
            clear_note_4();
        }

        private void button_unselect_Click(object sender, EventArgs e)
        {
            unselect_line();
        }
        private void listViewNotes_Click(object sender, EventArgs e)
        {
            if (listViewNotes.FocusedItem != null)
            {
                Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                beep_label_appear();
                new Thread(() =>
                {
                    play_note_in_line(Convert.ToInt32(final_note_length));
                }).Start();
            }
        }

        private void numericUpDown_bpm_ValueChanged(object sender, EventArgs e)
        {
            Variables.bpm = Convert.ToInt32(numericUpDown_bpm.Value);
        }

        private void numericUpDown_alternating_notes_ValueChanged(object sender, EventArgs e)
        {
            Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
        }

        /*private void disable_keys()
        {
            if (button_b5.Enabled == button_a5.Enabled == button_g5.Enabled == button_f5.Enabled == button_e5.Enabled == button_d5.Enabled == button_c5.Enabled == true)
            {
                button_b5.Enabled = button_a5.Enabled = button_g5.Enabled = button_f5.Enabled = button_e5.Enabled = button_d5.Enabled = button_c5.Enabled = false;
            }
            if (button_b4.Enabled == button_a4.Enabled == button_g4.Enabled == button_f4.Enabled == button_e4.Enabled == button_d4.Enabled == button_c4.Enabled == true)
            {
                button_b4.Enabled = button_a4.Enabled = button_g4.Enabled = button_f4.Enabled = button_e4.Enabled = button_d4.Enabled = button_c4.Enabled = false;
            }
            if (button_b3.Enabled == button_a3.Enabled == button_g3.Enabled == button_f3.Enabled == button_e3.Enabled == button_d3.Enabled == button_c3.Enabled == true)
            {
                button_b3.Enabled = button_a3.Enabled = button_g3.Enabled = button_f3.Enabled = button_e3.Enabled = button_d3.Enabled = button_c3.Enabled = false;
            }
            if (button_a_s5.Enabled == button_g_s5.Enabled == button_f_s5.Enabled == button_d_s5.Enabled == button_c_s5.Enabled == true)
            {
                button_a_s5.Enabled = button_g_s5.Enabled = button_f_s5.Enabled = button_d_s5.Enabled = button_c_s5.Enabled = false;
            }
            if (button_a_s4.Enabled == button_g_s4.Enabled == button_f_s4.Enabled == button_d_s4.Enabled == button_c_s4.Enabled == true)
            {
                button_a_s4.Enabled = button_g_s4.Enabled = button_f_s4.Enabled = button_d_s4.Enabled = button_c_s4.Enabled = false;
            }
            if (button_a_s3.Enabled == button_g_s3.Enabled == button_f_s3.Enabled == button_d_s3.Enabled == button_c_s3.Enabled == true)
            {
                button_a_s3.Enabled = button_g_s3.Enabled = button_f_s3.Enabled = button_d_s3.Enabled = button_c_s3.Enabled = false;
            }
            if (lbl_b5.Enabled == lbl_a5.Enabled == lbl_g5.Enabled == lbl_f5.Enabled == lbl_e5.Enabled == lbl_d5.Enabled == lbl_c5.Enabled == true)
            {
                lbl_b5.Enabled = lbl_a5.Enabled = lbl_g5.Enabled = lbl_f5.Enabled = lbl_e5.Enabled = lbl_d5.Enabled = lbl_c5.Enabled = false;
            }
            if (lbl_b4.Enabled == lbl_a4.Enabled == lbl_g4.Enabled == lbl_f4.Enabled == lbl_e4.Enabled == lbl_d4.Enabled == lbl_c4.Enabled == true)
            {
                lbl_b4.Enabled = lbl_a4.Enabled = lbl_g4.Enabled = lbl_f4.Enabled = lbl_e4.Enabled = lbl_d4.Enabled = lbl_c4.Enabled = false;
            }
            if (lbl_b3.Enabled == lbl_a3.Enabled == lbl_g3.Enabled == lbl_f3.Enabled == lbl_e3.Enabled == lbl_d3.Enabled == lbl_c3.Enabled == true)
            {
                lbl_b3.Enabled = lbl_a3.Enabled = lbl_g3.Enabled = lbl_f3.Enabled = lbl_e3.Enabled = lbl_d3.Enabled = lbl_c3.Enabled = false;
            }
            if (checkBox_do_not_update.Enabled == true)
            {
                checkBox_do_not_update.Enabled = false;
            }
            if (listViewNotes.Enabled == true)
            {
                listViewNotes.Enabled = false;
            }
        }
        private void enable_keys()
        {
            if (button_b5.Enabled == button_a5.Enabled == button_g5.Enabled == button_f5.Enabled == button_e5.Enabled == button_d5.Enabled == button_c5.Enabled == false)
            {
                button_b5.Enabled = button_a5.Enabled = button_g5.Enabled = button_f5.Enabled = button_e5.Enabled = button_d5.Enabled = button_c5.Enabled = true;
            }
            if (button_b4.Enabled == button_a4.Enabled == button_g4.Enabled == button_f4.Enabled == button_e4.Enabled == button_d4.Enabled == button_c4.Enabled == false)
            {
                button_b4.Enabled = button_a4.Enabled = button_g4.Enabled = button_f4.Enabled = button_e4.Enabled = button_d4.Enabled = button_c4.Enabled = true;
            }
            if (button_b3.Enabled == button_a3.Enabled == button_g3.Enabled == button_f3.Enabled == button_e3.Enabled == button_d3.Enabled == button_c3.Enabled == false)
            {
                button_b3.Enabled = button_a3.Enabled = button_g3.Enabled = button_f3.Enabled = button_e3.Enabled = button_d3.Enabled = button_c3.Enabled = true;
            }
            if (button_a_s5.Enabled == button_g_s5.Enabled == button_f_s5.Enabled == button_d_s5.Enabled == button_c_s5.Enabled == false)
            {
                button_a_s5.Enabled = button_g_s5.Enabled = button_f_s5.Enabled = button_d_s5.Enabled = button_c_s5.Enabled = true;
            }
            if (button_a_s4.Enabled == button_g_s4.Enabled == button_f_s4.Enabled == button_d_s4.Enabled == button_c_s4.Enabled == false)
            {
                button_a_s4.Enabled = button_g_s4.Enabled = button_f_s4.Enabled = button_d_s4.Enabled = button_c_s4.Enabled = true;
            }
            if (button_a_s3.Enabled == button_g_s3.Enabled == button_f_s3.Enabled == button_d_s3.Enabled == button_c_s3.Enabled == false)
            {
                button_a_s3.Enabled = button_g_s3.Enabled = button_f_s3.Enabled = button_d_s3.Enabled = button_c_s3.Enabled = true;
            }
            if (lbl_b5.Enabled == lbl_a5.Enabled == lbl_g5.Enabled == lbl_f5.Enabled == lbl_e5.Enabled == lbl_d5.Enabled == lbl_c5.Enabled == false)
            {
                lbl_b5.Enabled = lbl_a5.Enabled = lbl_g5.Enabled = lbl_f5.Enabled = lbl_e5.Enabled = lbl_d5.Enabled = lbl_c5.Enabled = true;
            }
            if (lbl_b4.Enabled == lbl_a4.Enabled == lbl_g4.Enabled == lbl_f4.Enabled == lbl_e4.Enabled == lbl_d4.Enabled == lbl_c4.Enabled == false)
            {
                lbl_b4.Enabled = lbl_a4.Enabled = lbl_g4.Enabled = lbl_f4.Enabled = lbl_e4.Enabled = lbl_d4.Enabled = lbl_c4.Enabled = true;
            }
            if (lbl_b3.Enabled == lbl_a3.Enabled == lbl_g3.Enabled == lbl_f3.Enabled == lbl_e3.Enabled == lbl_d3.Enabled == lbl_c3.Enabled == false)
            {
                lbl_b3.Enabled = lbl_a3.Enabled = lbl_g3.Enabled = lbl_f3.Enabled = lbl_e3.Enabled = lbl_d3.Enabled = lbl_c3.Enabled = true;
            }
            if (checkBox_do_not_update.Enabled == false)
            {
                checkBox_do_not_update.Enabled = true;
            }
            if (listViewNotes.Enabled == false)
            {
                listViewNotes.Enabled = true;
            }
        }*/
        private async void beep_label_appear()
        {
            if (listViewNotes.FocusedItem != null)
            {
                Variables.miliseconds_per_beat = Convert.ToInt32(60000 / Variables.bpm);
                int selected_line = listViewNotes.Items.IndexOf(listViewNotes.SelectedItems[0]);
                if ((checkBox_play_note1_clicked.Checked == true && listViewNotes.Items[selected_line].SubItems[1].Text != string.Empty) ||
                    (checkBox_play_note2_clicked.Checked == true && listViewNotes.Items[selected_line].SubItems[2].Text != string.Empty) ||
                    (checkBox_play_note3_clicked.Checked == true && listViewNotes.Items[selected_line].SubItems[3].Text != string.Empty) ||
                    (checkBox_play_note4_clicked.Checked == true && listViewNotes.Items[selected_line].SubItems[4].Text != string.Empty))
                {
                    if (listViewNotes.Items[selected_line].SubItems[0].Text == "Whole")
                    {
                        note_length = Convert.ToInt32(Variables.miliseconds_per_beat) * 4;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[0].Text == "Half")
                    {
                        note_length = Convert.ToInt32(Variables.miliseconds_per_beat) * 2;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[0].Text == "Quarter")
                    {
                        note_length = Convert.ToInt32(Variables.miliseconds_per_beat);
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/8")
                    {
                        note_length = Convert.ToInt32(Variables.miliseconds_per_beat) / 2;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/16")
                    {
                        note_length = Convert.ToInt32(Variables.miliseconds_per_beat) / 4;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/32")
                    {
                        note_length = Convert.ToInt32(Variables.miliseconds_per_beat) / 8;
                    }
                    if (listViewNotes.Items[selected_line].SubItems[5].Text == "Dot")
                    {
                        note_length *= 1.5;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[5].Text == "Tri")
                    {
                        note_length /= 3;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Sta")
                    {
                        note_length /= 2;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Spi")
                    {
                        note_length /= 4;
                    }
                    else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Fer")
                    {
                        note_length *= 2;
                    }
                    final_note_length = note_length * Variables.note_silence_ratio;
                    final_note_count = Convert.ToInt32(final_note_length);
                    listViewNotes.Enabled = false;
                    //stopwatch.Restart();
                    checkBox_do_not_update.Enabled = false;
                    numericUpDown_bpm.Enabled = false;
                    numericUpDown_alternating_notes.Enabled = false;
                    is_note_playing = true;
                    label_beep.Visible = true;
                    duration(Convert.ToInt32(final_note_length));
                    async Task duration(int length)
                    {
                        await Task.Delay(length);
                        label_beep.Visible = false;
                        is_note_playing = false;
                        total_note_length = 0;
                        note_count = 0;
                        listViewNotes.Enabled = true;
                        checkBox_do_not_update.Enabled = true;
                        numericUpDown_bpm.Enabled = true;
                        numericUpDown_alternating_notes.Enabled = true;

                    }
                    /*if (Convert.ToInt32(stopwatch.ElapsedMilliseconds / 2) > Convert.ToInt32(final_note_length))
                    {
                        label_beep.Visible = false;
                        total_note_length = 0;
                        note_count = 0;
                        listViewNotes.Enabled = true;
                        checkBox_do_not_update.Enabled = true;
                        numericUpDown_bpm.Enabled = true;
                        numericUpDown_alternating_notes.Enabled = true;
                        stopwatch.Stop();
                    }*/
                    //disable_keys();
                    //enable_keys();
                    /*if (note_count < final_note_count)
                    {
                        note_count++;
                        Task.Delay(1);
                    }
                    else if ((note_count == final_note_count) && label_beep.Visible == true)
                    {
                        
                    }*/
                }
            }
        }
        private void play_note_when_line_is_clicked(int frequency, int length)
        {
            if (Program.creating_sounds.create_beep_with_soundcard == false)
            {
                if (checkBox_mute_system_speaker.Checked == false)
                {
                    if (frequency >= 37 && frequency <= 32767)
                    {
                        RenderBeep.BeepClass.Beep(frequency, length);
                    }
                }
            }
        }
        private void play_note_in_line(int length)
        {
            string note1;
            string note2;
            string note3;
            string note4;
            double note1_base_frequency = 0;
            int note1_octave = 0;
            double note1_frequency = 0;
            double note2_base_frequency = 0;
            int note2_octave = 0;
            double note2_frequency = 0;
            double note3_base_frequency = 0;
            int note3_octave = 0;
            double note3_frequency = 0;
            double note4_base_frequency = 0;
            int note4_octave = 0;
            double note4_frequency = 0;
            if (listViewNotes.FocusedItem != null)
            {
                int selected_line = listViewNotes.Items.IndexOf(listViewNotes.SelectedItems[0]);
                note1 = listViewNotes.Items[selected_line].SubItems[1].Text;
                note2 = listViewNotes.Items[selected_line].SubItems[2].Text;
                note3 = listViewNotes.Items[selected_line].SubItems[3].Text;
                note4 = listViewNotes.Items[selected_line].SubItems[4].Text;
                if (note1 != string.Empty)
                {
                    if (note1.Contains("#"))
                    {
                        if (note1.Length == 3)
                        {
                            note1_octave = Convert.ToInt32((note1.Substring(2, 1)));
                        }
                        else if (note1.Length == 4)
                        {
                            note1_octave = Convert.ToInt32((note1.Substring(2, 2)));
                        }
                        switch (note1.Substring(0, 2))
                        {
                            case "C#":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.CS;
                                break;
                            case "D#":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.DS;
                                break;
                            case "F#":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.FS;
                                break;
                            case "G#":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.GS;
                                break;
                            case "A#":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.AS;
                                break;
                        }
                    }
                    else
                    {
                        if (note1.Length == 2)
                        {
                            note1_octave = Convert.ToInt32((note1.Substring(1, 1)));
                        }
                        else if (note1.Length == 3)
                        {
                            note1_octave = Convert.ToInt32((note1.Substring(1, 2)));
                        }
                        switch (note1.Substring(0, 1))
                        {
                            case "C":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.C;
                                break;
                            case "D":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.D;
                                break;
                            case "E":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.E;
                                break;
                            case "F":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.F;
                                break;
                            case "G":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.G;
                                break;
                            case "A":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.A;
                                break;
                            case "B":
                                note1_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.B;
                                break;
                        }
                    }
                    note1_frequency = note1_base_frequency * Math.Pow(2, (note1_octave - 4));
                }
                if (note2 != string.Empty)
                {
                    if (note2.Contains("#"))
                    {
                        if (note2.Length == 3)
                        {
                            note2_octave = Convert.ToInt32((note2.Substring(2, 1)));
                        }
                        else if (note2.Length == 4)
                        {
                            note2_octave = Convert.ToInt32((note2.Substring(2, 2)));
                        }
                        switch (note2.Substring(0, 2))
                        {
                            case "C#":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.CS;
                                break;
                            case "D#":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.DS;
                                break;
                            case "F#":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.FS;
                                break;
                            case "G#":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.GS;
                                break;
                            case "A#":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.AS;
                                break;
                        }
                    }
                    else
                    {
                        if (note2.Length == 2)
                        {
                            note2_octave = Convert.ToInt32((note2.Substring(1, 1)));
                        }
                        else if (note2.Length == 3)
                        {
                            note2_octave = Convert.ToInt32((note2.Substring(1, 2)));
                        }
                        switch (note2.Substring(0, 1))
                        {
                            case "C":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.C;
                                break;
                            case "D":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.D;
                                break;
                            case "E":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.E;
                                break;
                            case "F":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.F;
                                break;
                            case "G":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.G;
                                break;
                            case "A":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.A;
                                break;
                            case "B":
                                note2_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.B;
                                break;
                        }
                    }
                    note2_frequency = note2_base_frequency * Math.Pow(2, (note2_octave - 4));
                }
                if (note3 != string.Empty)
                {
                    if (note3.Contains("#"))
                    {
                        if (note3.Length == 3)
                        {
                            note3_octave = Convert.ToInt32((note3.Substring(2, 1)));
                        }
                        else if (note3.Length == 4)
                        {
                            note3_octave = Convert.ToInt32((note3.Substring(2, 2)));
                        }
                        switch (note3.Substring(0, 2))
                        {
                            case "C#":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.CS;
                                break;
                            case "D#":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.DS;
                                break;
                            case "F#":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.FS;
                                break;
                            case "G#":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.GS;
                                break;
                            case "A#":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.AS;
                                break;
                        }
                    }
                    else
                    {
                        if (note3.Length == 2)
                        {
                            note3_octave = Convert.ToInt32((note3.Substring(1, 1)));
                        }
                        else if (note3.Length == 3)
                        {
                            note3_octave = Convert.ToInt32((note3.Substring(1, 2)));
                        }
                        switch (note3.Substring(0, 1))
                        {
                            case "C":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.C;
                                break;
                            case "D":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.D;
                                break;
                            case "E":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.E;
                                break;
                            case "F":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.F;
                                break;
                            case "G":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.G;
                                break;
                            case "A":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.A;
                                break;
                            case "B":
                                note3_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.B;
                                break;
                        }
                    }
                    note3_frequency = note3_base_frequency * Math.Pow(2, (note3_octave - 4));
                }
                if (note4 != string.Empty)
                {
                    if (note4.Contains("#"))
                    {
                        if (note4.Length == 3)
                        {
                            note4_octave = Convert.ToInt32((note4.Substring(2, 1)));
                        }
                        else if (note3.Length == 4)
                        {
                            note4_octave = Convert.ToInt32((note4.Substring(2, 2)));
                        }
                        switch (note4.Substring(0, 2))
                        {
                            case "C#":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.CS;
                                break;
                            case "D#":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.DS;
                                break;
                            case "F#":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.FS;
                                break;
                            case "G#":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.GS;
                                break;
                            case "A#":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.AS;
                                break;
                        }
                    }
                    else
                    {
                        if (note4.Length == 2)
                        {
                            note4_octave = Convert.ToInt32((note4.Substring(1, 1)));
                        }
                        else if (note4.Length == 3)
                        {
                            note4_octave = Convert.ToInt32((note4.Substring(1, 2)));
                        }
                        switch (note4.Substring(0, 1))
                        {
                            case "C":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.C;
                                break;
                            case "D":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.D;
                                break;
                            case "E":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.E;
                                break;
                            case "F":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.F;
                                break;
                            case "G":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.G;
                                break;
                            case "A":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.A;
                                break;
                            case "B":
                                note4_base_frequency = base_note_frequency.base_note_frequency_in_4th_octave.B;
                                break;
                        }
                    }
                    note4_frequency = note4_base_frequency * Math.Pow(2, (note4_octave - 4));
                }

                if (radioButtonPlay_alternating_notes1.Checked == true) // Combine same notes in a row and play them as one note with a length of the sum of their lengths
                {
                    if (note1 == note2 && note2 == note3 && note3 == note4)
                    {
                        Variables.alternating_note_length *= 4;
                        note2 = note3 = note4 = string.Empty;
                    }
                    else if (note1 == note2 && note2 == note3)
                    {
                        Variables.alternating_note_length *= 2;
                        note2 = note3 = string.Empty;
                    }

                    else if (note1 == note2)
                    {
                        if (note1 == note2 && note3 == note4)
                        {
                            Variables.alternating_note_length *= 4;
                            note2 = string.Empty;
                            note4 = string.Empty;
                        }
                        else
                        {
                            Variables.alternating_note_length *= 2;
                            note2 = string.Empty;
                        }
                    }
                    else if (note1 == note3)
                    {
                        Variables.alternating_note_length *= 2;
                        note3 = string.Empty;
                    }
                    else if (note2 == note3)
                    {
                        Variables.alternating_note_length *= 4;
                        note3 = string.Empty;
                    }
                    else if (note2 == note4)
                    {
                        Variables.alternating_note_length *= 2;
                        note4 = string.Empty;
                    }
                    else if (note3 == note4)
                    {
                        Variables.alternating_note_length *= 2;
                        note4 = string.Empty;
                    }
                }
                else if (radioButtonPlay_alternating_notes2.Checked == true)
                {
                    if (note1 == note3 && note3 == note2 && note2 == note4)
                    {
                        Variables.alternating_note_length *= 4;
                        note3 = note2 = note4 = string.Empty;
                    }
                    else if (note1 == note3 && note3 == note2)
                    {
                        Variables.alternating_note_length *= 2;
                        note3 = note2 = string.Empty;
                    }

                    else if (note1 == note2)
                    {
                        if (note1 == note3 && note2 == note4)
                        {
                            Variables.alternating_note_length *= 4;
                            note3 = string.Empty;
                            note4 = string.Empty;
                        }
                        else
                        {
                            Variables.alternating_note_length *= 2;
                            note3 = string.Empty;
                        }
                    }
                    else if (note1 == note2)
                    {
                        Variables.alternating_note_length *= 2;
                        note2 = string.Empty;
                    }
                    else if (note3 == note2)
                    {
                        Variables.alternating_note_length *= 4;
                        note2 = string.Empty;
                    }
                    else if (note3 == note4)
                    {
                        Variables.alternating_note_length *= 2;
                        note4 = string.Empty;
                    }
                    else if (note2 == note4)
                    {
                        Variables.alternating_note_length *= 2;
                        note4 = string.Empty;
                    }
                }
                if ((note1 != string.Empty || note1 != null) && (note2 == string.Empty || note2 == null) && (note3 == string.Empty || note3 == null) && (note4 == string.Empty || note4 == null))
                {
                    if (checkBox_play_note1_clicked.Checked == true)
                    {
                        play_note_when_line_is_clicked(Convert.ToInt32(note1_frequency), length);
                    }
                }
                else if ((note1 == string.Empty || note1 == null) && (note2 != string.Empty || note2 != null) && (note3 == string.Empty || note3 == null) && (note4 == string.Empty || note4 == null))
                {
                    if (checkBox_play_note2_clicked.Checked == true)
                    {
                        play_note_when_line_is_clicked(Convert.ToInt32(note2_frequency), length);
                    }
                }

                else if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 != string.Empty || note3 != null) && (note4 == string.Empty || note4 == null))
                {
                    if (checkBox_play_note3_clicked.Checked == true)
                    {
                        play_note_when_line_is_clicked(Convert.ToInt32(note3_frequency), length);
                    }
                }
                else if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 == string.Empty || note3 == null) && (note4 != string.Empty || note4 != null))
                {
                    if (checkBox_play_note4_clicked.Checked == true)
                    {
                        play_note_when_line_is_clicked(Convert.ToInt32(note4_frequency), length);
                    }
                }
                else
                {
                    int note_order = 1;
                    int last_note_order = length / Variables.alternating_note_length;
                    if (radioButtonPlay_alternating_notes1.Checked == true)
                    {
                        string[] note_series = { note1, note2, note3, note4 };
                        do
                        {
                            int column = 0;
                            while (column < 4)
                            {
                                if (note_series[column] != string.Empty)
                                {
                                    if (column == 0)
                                    {
                                        play_note_when_line_is_clicked(Convert.ToInt32(note1_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    }
                                    else if (column == 1)
                                    {
                                        play_note_when_line_is_clicked(Convert.ToInt32(note2_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    }
                                    else if (column == 2)
                                    {
                                        play_note_when_line_is_clicked(Convert.ToInt32(note3_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    }
                                    else if (column == 3)
                                    {
                                        play_note_when_line_is_clicked(Convert.ToInt32(note4_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    }
                                    note_order++;
                                }
                                column++;
                            }
                        }
                        while (note_order < last_note_order);
                    }
                    else if (radioButtonPlay_alternating_notes2.Checked == true)
                    {
                        string[] note_series = { note1, note2, note3, note4 };
                        do
                        {
                            // �nce tek say�l� s�tunlar (Nota1 ve Nota3)
                            for (int i = 0; i < 4; i += 2)
                            {
                                if (note_series[i] != string.Empty)
                                {
                                    play_note_when_line_is_clicked(Convert.ToInt32(note_series[i] == note1 ? note1_frequency : note3_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    note_order++;
                                }
                            }
                            // Sonra �ift say�l� s�tunlar (Nota2 ve Nota4)
                            for (int i = 1; i < 4; i += 2)
                            {
                                if (note_series[i] != string.Empty)
                                {
                                    play_note_when_line_is_clicked(Convert.ToInt32(note_series[i] == note2 ? note2_frequency : note4_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    note_order++;
                                }
                            }
                        }
                        while (note_order < last_note_order);
                    }
                }
            }
        }
        private void listViewNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selectedLine = listViewNotes.SelectedIndices[0];
                if (listViewNotes.Items[selectedLine].SubItems[5].Text == "Dot" && checkBox_staccato.Checked == false)
                {
                    checkBox_dotted.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[5].Text == "Tri" && checkBox_fermata.Checked == false)
                {
                    checkBox_triplet.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[5].Text == String.Empty)
                {
                    if (checkBox_dotted.Checked == true)
                    {
                        checkBox_dotted.Checked = false;
                    }
                    if (checkBox_triplet.Checked == true)
                    {
                        checkBox_triplet.Checked = false;
                    }
                }
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == "Sta" && checkBox_staccato.Checked == false)
                {
                    checkBox_staccato.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == "Fer" && checkBox_fermata.Checked == false)
                {
                    checkBox_fermata.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == "Spi" && checkBox_spiccato.Checked == false)
                {
                    checkBox_spiccato.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == String.Empty)
                {
                    if (checkBox_staccato.Checked == true)
                    {
                        checkBox_staccato.Checked = false;
                    }
                    if (checkBox_fermata.Checked == true)
                    {
                        checkBox_fermata.Checked = false;
                    }
                    if (checkBox_spiccato.Checked == true)
                    {
                        checkBox_spiccato.Checked = false;
                    }
                }
            }
        }

        private void checkBox_synchronized_play_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_synchronized_play.Checked == true)
            {
                synchronized_play_window synchronized_play = new synchronized_play_window();
                synchronized_play.Show();
                checkBox_synchronized_play.Tag = synchronized_play;
            }
            else if (checkBox_synchronized_play.Checked == false)
            {
                synchronized_play_window synchronized_play = checkBox_synchronized_play.Tag as synchronized_play_window;
                synchronized_play.Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int recalculated_final_note_count;
            recalculated_final_note_count = final_note_count / 20;

        }

        private void trackBar_note_silence_ratio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void listViewNotes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {

        }

        private void listViewNotes_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void trackBar_time_signature_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label_note_Click(object sender, EventArgs e)
        {

        }

        private void main_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                RenderBeep.BeepClass.StopBeep();
            }
            this.Dispose();
        }

        private void main_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                RenderBeep.BeepClass.StopBeep();
            }
            this.Dispose();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

        }

        private void lbl_f3_Click(object sender, EventArgs e)
        {

        }

        private void lbl_g3_Click(object sender, EventArgs e)
        {
        }

        private void lbl_a3_Click(object sender, EventArgs e)
        {
        }

        private void lbl_b3_Click(object sender, EventArgs e)
        {
        }

        private void group_adding_note_Enter(object sender, EventArgs e)
        {

        }

        private void rewindToSavedVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkBox_mute_system_speaker_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_mute_system_speaker.Checked == false)
            {
                if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
                {
                    RenderBeep.BeepClass.StopBeep();
                }
            }
        }
        private void show_keyboard_keys_shortcut()
        {
            button_c3.Text = "Tab";
            button_c_s3.Text = "`";
            button_d3.Text = "Q";
            button_d_s3.Text = "1";
            button_e3.Text = "W";
            button_f3.Text = "E";
            button_f_s3.Text = "3";
            button_g3.Text = "R";
            button_g_s3.Text = "4";
            button_a3.Text = "T";
            button_a_s3.Text = "5";
            button_b3.Text = "Y";
            button_c4.Text = "U";
            button_c_s4.Text = "7";
            button_d4.Text = "I";
            button_d_s4.Text = "8";
            button_e4.Text = "O";
            button_f4.Text = "P";
            button_f_s4.Text = "0";
            button_g4.Text = "[";
            button_g_s4.Text = "-";
            button_a4.Text = "]";
            button_a_s4.Text = "+";
            button_b4.Text = "|";
            button_c5.Text = "Shift";
            button_c_s5.Text = "A";
            button_d5.Text = "Z";
            button_d_s5.Text = "S";
            button_e5.Text = "X";
            button_f5.Text = "C";
            button_f_s5.Text = "F";
            button_g5.Text = "V";
            button_g_s5.Text = "G";
            button_a5.Text = "B";
            button_a_s5.Text = "H";
            button_b5.Text = "N";
        }
        private void hide_keyboard_keys_shortcut()
        {
            button_c3.Text = string.Empty;
            button_c_s3.Text = string.Empty;
            button_d3.Text = string.Empty;
            button_d_s3.Text = string.Empty;
            button_e3.Text = string.Empty;
            button_f3.Text = string.Empty;
            button_f_s3.Text = string.Empty;
            button_g3.Text = string.Empty;
            button_g_s3.Text = string.Empty;
            button_a3.Text = string.Empty;
            button_a_s3.Text = string.Empty;
            button_b3.Text = string.Empty;
            button_c4.Text = string.Empty;
            button_c_s4.Text = string.Empty;
            button_d4.Text = string.Empty;
            button_d_s4.Text = string.Empty;
            button_e4.Text = string.Empty;
            button_f4.Text = string.Empty;
            button_f_s4.Text = string.Empty;
            button_g4.Text = string.Empty;
            button_g_s4.Text = string.Empty;
            button_a4.Text = string.Empty;
            button_a_s4.Text = string.Empty;
            button_b4.Text = string.Empty;
            button_c5.Text = string.Empty;
            button_c_s5.Text = string.Empty;
            button_d5.Text = string.Empty;
            button_d_s5.Text = string.Empty;
            button_e5.Text = string.Empty;
            button_f5.Text = string.Empty;
            button_f_s5.Text = string.Empty;
            button_g5.Text = string.Empty; ;
            button_g_s5.Text = string.Empty;
            button_a5.Text = string.Empty;
            button_a_s5.Text = string.Empty;
            button_b5.Text = string.Empty;
        }
        private void checkBox_use_keyboard_as_piano_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_use_keyboard_as_piano.Checked == true)
            {
                show_keyboard_keys_shortcut();
            }
            else if (checkBox_use_keyboard_as_piano.Checked == false)
            {
                hide_keyboard_keys_shortcut();
            }
        }

        private void main_window_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void main_window_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = files[0];
                string first_line = File.ReadLines(fileName).First();
                if (IsMidiFile(fileName))
                {
                    MIDI_file_player midi_file_player = new MIDI_file_player(fileName);
                    midi_file_player.ShowDialog();
                }
                else if (first_line == "Bleeper Music Maker by Robbi-985 file format" ||
                    first_line == "<NeoBleeperProjectFile>")
                {
                    FileParser(fileName);
                }
                else
                {
                    MessageBox.Show("The file you dragged is not supported by NeoBleeper or is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private static void SerializeXML(string filePath, NBPML_File.NeoBleeperProjectFile projectFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
            XmlDocument xmlDoc = new XmlDocument();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty); // Namespace'leri kald�rmak i�in

                using (XmlWriter writer = XmlWriter.Create(memoryStream, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    serializer.Serialize(writer, projectFile, namespaces);
                }
                memoryStream.Position = 0;
                xmlDoc.Load(memoryStream);
            }

            xmlDoc.Save(filePath);
        }

        private void playMIDIFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "MIDI Files|*.mid|All Files|*.*";
            openFileDialog.ShowDialog(this);
            if (openFileDialog.FileName != string.Empty)
            {
                if (IsMidiFile(openFileDialog.FileName))
                {
                    MIDI_file_player midi_file_player = new MIDI_file_player(openFileDialog.FileName);
                    midi_file_player.ShowDialog();
                }
                else
                {
                    MessageBox.Show("This file is not a valid MIDI file or the file is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsMidiFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            byte[] header = new byte[4];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header, 0, 4);
            }

            // MIDI dosyalar� "MThd" ile ba�lar
            return header[0] == 'M' && header[1] == 'T' && header[2] == 'h' && header[3] == 'd';
        }

        private void blankLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            add_blank_line();
        }

        private void clearNote1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clear_note_1();
        }

        private void unselectLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            unselect_line();
        }

        private void eraseWholeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            erase_line();
        }

        private void playAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void playFromSelectedLineToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void stopPlayingToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}