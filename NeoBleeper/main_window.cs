using System.Drawing.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Text;
using System.Media;
using System.Threading.Tasks;
using System.Data.Common;

namespace NeoBleeper
{
    public partial class main_window : Form
    {
        private play_beat_window play_Beat_Window;
        private bool isModified = false;
        private CommandManager commandManager;
        private Originator originator;
        private Memento initialMemento;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        PrivateFontCollection fonts = new PrivateFontCollection();
        public event EventHandler MusicStopped;
        public static class Variables
        {
            public static int octave;
            public static int bpm;
            public static int miliseconds_per_beat;
            public static int alternating_note_length;
            public static double note_silence_ratio;
        }
        private bool isClosing = false;
        int note_length;
        int final_note_length;
        string currentFilePath;
        public Boolean is_music_playing = false;
        Boolean is_file_valid = false;
        public main_window()
        {
            CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            originator = new Originator(listViewNotes);
            commandManager = new CommandManager(originator);
            commandManager.StateChanged += CommandManager_StateChanged;
            listViewNotes.DoubleBuffering(true);
            label_beep.DoubleBuffering(true);
            UpdateUndoRedoButtons();
            set_default_font();
            listViewNotes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == false)
            {
                checkBox_mute_system_speaker.Checked = Program.creating_sounds.is_system_speaker_muted;
                checkBox_mute_system_speaker.Enabled = false;
            }
            main_window_refresh();
            comboBox_note_length.SelectedItem = comboBox_note_length.Items[3];
            comboBox_note_length.SelectedValue = comboBox_note_length.Items[3];
            Variables.octave = 4;
            Variables.bpm = 140;
            Variables.alternating_note_length = 30;
            Variables.note_silence_ratio = 0.5;
            initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternating_note_length);
        }
        private async void CommandManager_StateChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateUndoRedoButtons();
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void UpdateUndoRedoButtons()
        {
            try
            {
                undoToolStripMenuItem.Enabled = commandManager.CanUndo;
                redoToolStripMenuItem.Enabled = commandManager.CanRedo;
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void set_default_font()
        {
            try
            {
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
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void dark_theme()
        {
            try
            {
                menuStrip1.BackColor = Color.Black;
                menuStrip1.ForeColor = Color.White;
                listViewNotes.BackColor = Color.Black;
                listViewNotes.ForeColor = Color.White;
                foreach (ListViewItem item in listViewNotes.Items)
                {
                    item.BackColor = Color.Black;
                    item.ForeColor = Color.White;
                }
                this.BackColor = Color.FromArgb(32, 32, 32);
                this.ForeColor = Color.White;
                numericUpDown_alternating_notes.BackColor = Color.Black;
                numericUpDown_alternating_notes.ForeColor = Color.White;
                numericUpDown_bpm.BackColor = Color.Black;
                numericUpDown_bpm.ForeColor = Color.White;
                comboBox_note_length.BackColor = Color.Black;
                comboBox_note_length.ForeColor = Color.White;
                group_key_is_clicked.ForeColor = Color.White;
                group_adding_note.ForeColor = Color.White;
                group_notes.ForeColor = Color.White;
                group_line_clicked.ForeColor = Color.White;
                group_music_played.ForeColor = Color.White;
                btn_octave_increase.BackColor = Color.FromArgb(32, 32, 32);
                btn_octave_decrease.BackColor = Color.FromArgb(32, 32, 32);
                btn_octave_decrease.ForeColor = Color.White;
                btn_octave_increase.ForeColor = Color.White;
                checkBox_metronome.BackColor = Color.FromArgb(32, 32, 32);
                checkBox_dotted.BackColor = Color.FromArgb(32, 32, 32);
                checkBox_triplet.BackColor = Color.FromArgb(32, 32, 32);
                checkBox_staccato.BackColor = Color.FromArgb(32, 32, 32);
                checkBox_spiccato.BackColor = Color.FromArgb(32, 32, 32);
                checkBox_fermata.BackColor = Color.FromArgb(32, 32, 32);
                checkBox_metronome.ForeColor = Color.White;
                checkBox_dotted.ForeColor = Color.White;
                checkBox_triplet.ForeColor = Color.White;
                checkBox_staccato.ForeColor = Color.White;
                checkBox_spiccato.ForeColor = Color.White;
                checkBox_fermata.ForeColor = Color.White;
                checkBox_mute_system_speaker.ForeColor = Color.White;
                notes_list_right_click.BackColor = Color.Black;
                notes_list_right_click.ForeColor = Color.White;
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }


        private async void light_theme()
        {
            try
            {
                menuStrip1.BackColor = SystemColors.ControlLightLight;
                menuStrip1.ForeColor = SystemColors.ControlText;
                listViewNotes.BackColor = SystemColors.Window;
                listViewNotes.ForeColor = SystemColors.WindowText;
                foreach (ListViewItem item in listViewNotes.Items)
                {
                    item.BackColor = SystemColors.Window;
                    item.ForeColor = SystemColors.WindowText;
                }
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
                numericUpDown_alternating_notes.BackColor = SystemColors.Window;
                numericUpDown_alternating_notes.ForeColor = SystemColors.WindowText;
                numericUpDown_bpm.BackColor = SystemColors.Window;
                numericUpDown_bpm.ForeColor = SystemColors.WindowText;
                comboBox_note_length.BackColor = SystemColors.Window;
                comboBox_note_length.ForeColor = SystemColors.WindowText;
                group_key_is_clicked.ForeColor = SystemColors.ControlText;
                group_adding_note.ForeColor = SystemColors.ControlText;
                group_notes.ForeColor = SystemColors.ControlText;
                group_line_clicked.ForeColor = SystemColors.ControlText;
                group_music_played.ForeColor = SystemColors.ControlText;
                btn_octave_increase.BackColor = Color.Transparent;
                btn_octave_decrease.BackColor = Color.Transparent;
                btn_octave_decrease.ForeColor = SystemColors.ControlText;
                btn_octave_increase.ForeColor = SystemColors.ControlText;
                checkBox_metronome.BackColor = Color.Transparent;
                checkBox_dotted.BackColor = Color.Transparent;
                checkBox_triplet.BackColor = Color.Transparent;
                checkBox_staccato.BackColor = Color.Transparent;
                checkBox_spiccato.BackColor = Color.Transparent;
                checkBox_fermata.BackColor = Color.Transparent;
                checkBox_metronome.ForeColor = SystemColors.ControlText;
                checkBox_dotted.ForeColor = SystemColors.ControlText;
                checkBox_triplet.ForeColor = SystemColors.ControlText;
                checkBox_staccato.ForeColor = SystemColors.ControlText;
                checkBox_spiccato.ForeColor = SystemColors.ControlText;
                checkBox_fermata.ForeColor = SystemColors.ControlText;
                checkBox_mute_system_speaker.ForeColor = SystemColors.ControlText;
                notes_list_right_click.BackColor = SystemColors.Window;
                notes_list_right_click.ForeColor = SystemColors.WindowText;
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
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
        private async void set_keyboard_colors()
        {
            try
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
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void set_buttons_colors()
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
        private async void set_beep_label_color()
        {
            label_beep.BackColor = Settings1.Default.beep_indicator_color;
            label_beep.ForeColor = set_text_color.GetTextColor(label_beep.BackColor);

        }
        private async void main_window_refresh()
        {
            Application.DoEvents();
            set_theme();
            set_keyboard_colors();
            set_buttons_colors();
            set_beep_label_color();
            this.Refresh();
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
            Debug.WriteLine($"Checked state of play note when key is clicked is changed to: {checkbox_play_note.Checked}");
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
            Debug.WriteLine($"Checked state of replace is changed to: {checkBox_replace.Checked}");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] note_lengths = { "Whole", "Half", "Quarter", "1/8", "1/16", "1/32" };
            Debug.WriteLine($"Selected note length is changed to: {note_lengths[comboBox_note_length.SelectedIndex]}");
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

        private void ýmportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "NeoBleeper Project Markup Language Files|*.NBPML|All Files|*.*"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveToNBPML(saveFileDialog.FileName);
                initialMemento = originator.CreateMemento(); // Save the current state of the notes list
            }
        }

        private void aboutNeoBleeperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            about_neobleeper about = new about_neobleeper();
            about.ShowDialog();
            Debug.WriteLine("About window is opened");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            if (checkBox_synchronized_play.Checked == true)
            {
                checkBox_synchronized_play.Checked = false;
            }
            if(checkBox_play_beat_sound.Checked == true)
            {
                checkBox_play_beat_sound.Checked = false;
            }
            settings_window settings = new settings_window();
            settings.ColorsAndThemeChanged += refresh_main_window_elements_color;
            settings.ColorsAndThemeChanged += (s, args) =>
            {
                synchronized_play_window synchronizedPlayWindow = checkBox_synchronized_play.Tag as synchronized_play_window;
                if (synchronizedPlayWindow != null)
                {
                    synchronizedPlayWindow.set_theme();
                }
                play_beat_window playBeatWindow = checkBox_play_beat_sound.Tag as play_beat_window;
                if (playBeatWindow != null)
                {
                    playBeatWindow.set_theme();
                }
            };
            settings.ShowDialog();
            Debug.WriteLine("Settings window is opened");
        }

        private void refresh_main_window_elements_color(object sender, EventArgs e)
        {
            main_window_refresh();
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
                int index = listViewNotes.SelectedIndices[0];
                var insertNoteCommand = new InsertNoteCommand(listViewNotes, listViewItem, index);
                commandManager.ExecuteCommand(insertNoteCommand);
            }
            else
            {
                var addNoteCommand = new AddNoteCommand(listViewNotes, listViewItem);
                commandManager.ExecuteCommand(addNoteCommand);
            }
            isModified = true;
            UpdateFormTitle();
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
                var replaceLengthCommand = new ReplaceLengthCommand(listViewNotes, Line.length);
                commandManager.ExecuteCommand(replaceLengthCommand);
                isModified = true;
                UpdateFormTitle();
            }
        }
        private void replace_note_in_line(string note)
        {
            if (checkBox_add_note_to_list.Checked == true)
            {
                if (add_as_note1.Checked == true)
                {
                    Line.note1 = note;
                    var replaceNoteCommand = new ReplaceNoteCommand(listViewNotes, 1, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                if (add_as_note2.Checked == true)
                {
                    Line.note2 = note;
                    var replaceNoteCommand = new ReplaceNoteCommand(listViewNotes, 2, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                if (add_as_note3.Checked == true)
                {
                    Line.note3 = note;
                    var replaceNoteCommand = new ReplaceNoteCommand(listViewNotes, 3, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                if (add_as_note4.Checked == true)
                {
                    Line.note4 = note;
                    var replaceNoteCommand = new ReplaceNoteCommand(listViewNotes, 4, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                isModified = true;
                UpdateFormTitle();
            }
        }
        int note_frequency;
        private async void play_note_when_key_is_clicked(int frequency)
        {
            if (MIDIIOUtils._midiOut != null && Program.MIDIDevices.useMIDIoutput == true)
            {
                MIDIIOUtils.ChangeInstrument(MIDIIOUtils._midiOut, Program.MIDIDevices.MIDIOutputInstrument,
                            Program.MIDIDevices.MIDIOutputDeviceChannel);
                await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, frequency, 100);
            }
            NotePlayer.play_note(frequency, 100);
        }
        private async void update_indicator_when_key_is_clicked()
        {
            if (checkBox_add_note_to_list.Checked == true)
            {
                updateIndicators(listViewNotes.Items.Count - 1, true);
            }
        }
        private void button_c3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key C{Variables.octave - 1} is clicked");
        }

        private void button_d3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key D{Variables.octave - 1} is clicked");
        }
        private void button_e3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key E{Variables.octave - 1} is clicked");
        }

        private void button_f3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key F{Variables.octave - 1} is clicked");
        }

        private void button_g3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key G{Variables.octave - 1} is clicked");
        }

        private void button_a3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key A{Variables.octave - 1} is clicked");
        }

        private void button_b3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key B{Variables.octave - 1} is clicked");
        }
        private void button_c4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key C{Variables.octave} is clicked");
        }

        private void button_d4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key D{Variables.octave} is clicked");
        }

        private void button_e4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key E{Variables.octave} is clicked");
        }

        private void button_f4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key F{Variables.octave} is clicked");
        }

        private void button_g4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key G{Variables.octave} is clicked");
        }

        private void button_a4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key A{Variables.octave} is clicked");
        }

        private void button_b4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key B{Variables.octave} is clicked");
        }
        private void button_c5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key C{Variables.octave + 1} is clicked");
        }

        private void button_d5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key D{Variables.octave + 1} is clicked");
        }

        private void button_e5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key E{Variables.octave + 1} is clicked");
        }

        private void button_f5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key F{Variables.octave + 1} is clicked"); ;
        }

        private void button_g5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key G{Variables.octave + 1} is clicked");
        }

        private void button_a5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key A{Variables.octave + 1} is clicked");
        }

        private void button_b5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key B{Variables.octave + 1} is clicked");
        }
        private void button_c_s3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            if (checkbox_play_note.Checked == true)
            {
                note_frequency = Convert.ToInt16(base_note_frequency.base_note_frequency_in_4th_octave.CS * (Math.Pow(2, (Variables.octave - 5))));
                play_note_when_key_is_clicked(note_frequency);
            }
            Debug.WriteLine($"Key C#{Variables.octave - 1} is clicked");
        }
        private void button_d_s3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key D#{Variables.octave - 1} is clicked");
        }


        private void button_f_s3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key F#{Variables.octave - 1} is clicked");
        }

        private void button_g_s3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key G#{Variables.octave - 1} is clicked");
        }

        private void button_a_s3_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key A#{Variables.octave - 1} is clicked");
        }

        private void button_c_s4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key C#{Variables.octave} is clicked");
        }

        private void button_d_s4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key D#{Variables.octave} is clicked");
        }

        private void button_f_s4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key F#{Variables.octave} is clicked");
        }

        private void button_g_s4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key G#{Variables.octave} is clicked");
        }

        private void button_a_s4_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key A#{Variables.octave} is clicked");
        }

        private void button_c_s5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key C#{Variables.octave + 1} is clicked");
        }

        private void button_d_s5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key D#{Variables.octave + 1} is clicked");
        }

        private void button_f_s5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key F#{Variables.octave + 1} is clicked");
        }

        private void button_g_s5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key G#{Variables.octave + 1} is clicked");
        }

        private void button_a_s5_Click(object sender, EventArgs e)
        {
            update_indicator_when_key_is_clicked();
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
            Debug.WriteLine($"Key A#{Variables.octave + 1} is clicked");
        }
        private int note_name_to_MIDI_number(string noteName)
        {
            // Define the base MIDI numbers for each note
            Dictionary<string, int> baseMidiNumbers = new Dictionary<string, int>
    {
        { "C", 0 }, { "C#", 1 }, { "D", 2 }, { "D#", 3 },
        { "E", 4 }, { "F", 5 }, { "F#", 6 }, { "G", 7 },
        { "G#", 8 }, { "A", 9 }, { "A#", 10 }, { "B", 11 }
    };

            if (string.IsNullOrEmpty(noteName) || noteName.Length < 2)
            {
                return -1;
            }

            string note = noteName.Substring(0, noteName.Length - 1).ToUpper();
            string octaveString = noteName.Substring(noteName.Length - 1);
            int octave;

            if (!int.TryParse(octaveString, out octave))
            {
                return -1;
            }

            if (!baseMidiNumbers.ContainsKey(note))
            {
                return -1;
            }

            int baseMidiNumber = baseMidiNumbers[note];
            int midiNumber = (octave + 1) * 12 + baseMidiNumber;

            return midiNumber;
        }


        private void play_note_in_line_from_MIDIOutput(int index, bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length)
        {

            String note1 = listViewNotes.Items[index].SubItems[1].Text;
            String note2 = listViewNotes.Items[index].SubItems[2].Text;
            String note3 = listViewNotes.Items[index].SubItems[3].Text;
            String note4 = listViewNotes.Items[index].SubItems[4].Text;

            int[] notes = {
        note_name_to_MIDI_number(note1),
        note_name_to_MIDI_number(note2),
        note_name_to_MIDI_number(note3),
        note_name_to_MIDI_number(note4)
    };

            if (MIDIIOUtils._midiOut != null) // Check if initialized
            {
                if (play_note1 && !string.IsNullOrEmpty(note1) && notes[0] != -1) Task.Run(() => MIDIIOUtils.PlayMidiNote(notes[0], length));
                if (play_note2 && !string.IsNullOrEmpty(note2) && notes[1] != -1) Task.Run(() => MIDIIOUtils.PlayMidiNote(notes[1], length));
                if (play_note3 && !string.IsNullOrEmpty(note3) && notes[2] != -1) Task.Run(() => MIDIIOUtils.PlayMidiNote(notes[2], length));
                if (play_note4 && !string.IsNullOrEmpty(note4) && notes[3] != -1) Task.Run(() => MIDIIOUtils.PlayMidiNote(notes[3], length));
            }
        }



        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        private static NBPML_File.NeoBleeperProjectFile DeserializeXMLFromString(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (NBPML_File.NeoBleeperProjectFile)serializer.Deserialize(reader);
            }
        }

        private void createMusicWithAIResponse(string createdMusic)
        {
            lbl_measure_value.Text = "1";
            lbl_beat_value.Text = "0.0";
            lbl_beat_traditional_value.Text = "1";
            lbl_beat_traditional_value.ForeColor = Color.Green;
            string first_line = createdMusic.First().ToString().Trim();
            try
            {
                saveToolStripMenuItem.Enabled = true;
                NBPML_File.NeoBleeperProjectFile projectFile = DeserializeXMLFromString(createdMusic);
                if (projectFile != null)
                {
                    Variables.octave = Convert.ToInt32(projectFile.Settings.RandomSettings.KeyboardOctave);
                    Variables.bpm = Convert.ToInt32(projectFile.Settings.RandomSettings.BPM);
                    numericUpDown_bpm.Value = Convert.ToDecimal(projectFile.Settings.RandomSettings.BPM);
                    trackBar_time_signature.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                    lbl_time_signature.Text = projectFile.Settings.RandomSettings.TimeSignature;
                    Variables.note_silence_ratio = Convert.ToDouble(Convert.ToDouble(Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio)) / 100);
                    trackBar_note_silence_ratio.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                    lbl_note_silence_ratio.Text = projectFile.Settings.RandomSettings.NoteSilenceRatio + "%";
                    comboBox_note_length.SelectedIndex = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteLength);
                    Variables.alternating_note_length = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
                    numericUpDown_alternating_notes.Value = Convert.ToDecimal(projectFile.Settings.RandomSettings.AlternateTime);
                    checkbox_play_note.Checked = projectFile.Settings.PlaybackSettings.NoteClickPlay == "True";
                    checkBox_add_note_to_list.Checked = projectFile.Settings.PlaybackSettings.NoteClickAdd == "True";
                    add_as_note1.Checked = projectFile.Settings.PlaybackSettings.AddNote1 == "True";
                    add_as_note2.Checked = projectFile.Settings.PlaybackSettings.AddNote2 == "True";
                    add_as_note3.Checked = projectFile.Settings.PlaybackSettings.AddNote3 == "True";
                    add_as_note4.Checked = projectFile.Settings.PlaybackSettings.AddNote4 == "True";
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
                    // Assign default values if none of the radiobuttons are checked
                    if (add_as_note1.Checked != true && add_as_note2.Checked != true && add_as_note3.Checked != true && add_as_note4.Checked != true)
                    {
                        add_as_note1.Checked = true;
                    }
                    // Assign default values if no time signature is found
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.TimeSignature))
                    {
                        trackBar_time_signature.Value = 4; // Default value
                        lbl_time_signature.Text = "4";
                        Debug.WriteLine("Time signature not found, defaulting to 4");
                    }
                    // Assign default values if no note silence ratio is found
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                    {
                        Variables.note_silence_ratio = 0.5; // Default value
                        lbl_note_silence_ratio.Text = "50%";
                        Debug.WriteLine("Note silence ratio not found, defaulting to 50%");
                    }
                    // Assign default values if no note length is found
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteLength))
                    {
                        comboBox_note_length.SelectedIndex = 0; // Default value
                        Debug.WriteLine("Note length not found, defaulting to Whole");
                    }
                    // Assign default values if no alternating note length is found
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.AlternateTime))
                    {
                        Variables.alternating_note_length = 30; // Default value
                        numericUpDown_alternating_notes.Value = 30;
                        Debug.WriteLine("Alternating note length not found, defaulting to 30 ms");
                    }
                    // Assign default values if no note click play is found
                    if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteClickPlay))
                    {
                        checkbox_play_note.Checked = true; // Default value
                        Debug.WriteLine("Note click play not found, defaulting to true");
                    }
                    // Assign default values if no note click add is found
                    if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteClickAdd))
                    {
                        checkBox_add_note_to_list.Checked = true; // Default value
                        Debug.WriteLine("Note click add not found, defaulting to true");
                    }
                    // Assign default values if no note replace is found
                    if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteReplace))
                    {
                        checkBox_replace.Checked = false; // Default value
                        Debug.WriteLine("Note replace not found, defaulting to false");
                    }
                    // Assign default values if no note length replace is found
                    if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteLengthReplace))
                    {
                        checkBox_replace_length.Checked = false; // Default value
                        Debug.WriteLine("Note length replace not found, defaulting to false");
                    }
                    // Assign default values if no click play note is found
                    if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote1))
                    {
                        checkBox_play_note1_clicked.Checked = true; // Default value
                        Debug.WriteLine("Click play note 1 not found, defaulting to true");
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote2))
                    {
                        checkBox_play_note2_clicked.Checked = true; // Default value
                        Debug.WriteLine("Click play note 2 not found, defaulting to true");
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote3))
                    {
                        checkBox_play_note3_clicked.Checked = true; // Default value
                        Debug.WriteLine("Click play note 3 not found, defaulting to true");
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote4))
                    {
                        checkBox_play_note3_clicked.Checked = true; // Default value
                        Debug.WriteLine("Click play note 4 not found, defaulting to true");
                    }
                    // Assign default values if no play note is found
                    if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote1))
                    {
                        checkBox_play_note1_played.Checked = true; // Default value
                        Debug.WriteLine("Play note 1 not found, defaulting to true");
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote2))
                    {
                        checkBox_play_note2_played.Checked = true; // Default value
                        Debug.WriteLine("Play note 2 not found, defaulting to true");
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote3))
                    {
                        checkBox_play_note3_played.Checked = true; // Default value
                        Debug.WriteLine("Play note 3 not found, defaulting to true");
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote4))
                    {
                        checkBox_play_note4_played.Checked = true; // Default value
                        Debug.WriteLine("Play note 4 not found, defaulting to true");
                    }
                    noteLabelsUpdate();
                    if (Variables.octave == 9)
                    {
                        octave10NoteLabelShiftToRight();
                    }
                    this.Text = System.AppDomain.CurrentDomain.FriendlyName + " - " + "AI Generated Music";
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
                    isModified = false;
                    UpdateFormTitle();
                }
                Debug.WriteLine("File is succesfully created by AI");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AI music creation failed: {ex.Message}");
                MessageBox.Show("AI music creation failed: " + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FileParser(string filename)
        {
            lbl_measure_value.Text = "1";
            lbl_beat_value.Text = "0.0";
            lbl_beat_traditional_value.Text = "1";
            lbl_beat_traditional_value.ForeColor = Color.Green;
            string first_line = File.ReadLines(filename).First().Trim();
            switch (first_line)
            {
                case "Bleeper Music Maker by Robbi-985 file format":
                    {
                        try
                        {
                            is_file_valid = true;
                            saveToolStripMenuItem.Enabled = true;
                            saveAsToolStripMenuItem.Enabled = false;
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
                            // Assign default values if none of the radiobuttons are checked
                            if (add_as_note1.Checked != true && add_as_note2.Checked != true && add_as_note3.Checked != true && add_as_note4.Checked != true)
                            {
                                add_as_note1.Checked = true;
                                Debug.WriteLine("No note type selected, defaulting to Note 1");
                            }
                            // Assign default values if no time signature is found
                            if (!lines.Any(line => line.StartsWith("TimeSig")))
                            {
                                trackBar_time_signature.Value = 4; // Default value
                                lbl_time_signature.Text = "4";
                                Debug.WriteLine("Time signature not found, defaulting to 4");
                            }
                            // Assign default values if no note silence ratio is found
                            if (!lines.Any(line => line.StartsWith("NoteSilenceRatio")))
                            {
                                Variables.note_silence_ratio = 0.5; // Default value
                                lbl_note_silence_ratio.Text = "50%";
                                Debug.WriteLine("Note silence ratio not found, defaulting to 50%");
                            }
                            // Assign default values if no note length is found
                            if (!lines.Any(line => line.StartsWith("NoteLength")))
                            {
                                comboBox_note_length.SelectedIndex = 0; // Default value
                                Debug.WriteLine("Note length not found, defaulting to Whole");
                            }
                            // Assign default values if no alternating note length is found
                            if (!lines.Any(line => line.StartsWith("AlternateTime")))
                            {
                                Variables.alternating_note_length = 30; // Default value
                                numericUpDown_alternating_notes.Value = 30;
                                Debug.WriteLine("Alternating note length not found, defaulting to 30 ms");
                            }
                            // Assign default values if no note click play is found
                            if (!lines.Any(line => line.StartsWith("NoteClickPlay")))
                            {
                                checkbox_play_note.Checked = true; // Default value
                                Debug.WriteLine("Note click play not found, defaulting to true");
                            }
                            // Assign default values if no note click add is found
                            if (!lines.Any(line => line.StartsWith("NoteClickAdd")))
                            {
                                checkBox_add_note_to_list.Checked = true; // Default value
                                Debug.WriteLine("Note click add not found, defaulting to true");
                            }
                            // Assign default values if no note replace is found
                            if (!lines.Any(line => line.StartsWith("NoteReplace")))
                            {
                                checkBox_replace.Checked = false; // Default value
                                Debug.WriteLine("Note replace not found, defaulting to false");
                            }
                            // Assign default values if no note length replace is found
                            if (!lines.Any(line => line.StartsWith("NoteLengthReplace")))
                            {
                                checkBox_replace_length.Checked = false; // Default value
                                Debug.WriteLine("Note length replace not found, defaulting to false");
                            }
                            // Assign default values if no click play note is found
                            if (!lines.Any(line => line.StartsWith("ClickPlayNote1")))
                            {
                                checkBox_play_note1_clicked.Checked = true; // Default value
                                Debug.WriteLine("Click play note 1 not found, defaulting to true");
                            }
                            if (!lines.Any(line => line.StartsWith("ClickPlayNote2")))
                            {
                                checkBox_play_note2_clicked.Checked = true; // Default value
                                Debug.WriteLine("Click play note 2 not found, defaulting to true");
                            }
                            // Assign default values if no play note is found
                            if (!lines.Any(line => line.StartsWith("PlayNote1")))
                            {
                                checkBox_play_note1_played.Checked = true; // Default value
                                Debug.WriteLine("Play note 1 not found, defaulting to true");
                            }
                            if (!lines.Any(line => line.StartsWith("PlayNote2")))
                            {
                                checkBox_play_note2_played.Checked = true; // Default value
                                Debug.WriteLine("Play note 2 not found, defaulting to true");
                            }
                            noteLabelsUpdate();
                            if (Variables.octave == 9)
                            {
                                octave10NoteLabelShiftToRight();
                            }
                            currentFilePath = filename;
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
                            Debug.WriteLine("Bleeper Music Maker file opened successfully");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error opening Bleeper Music Maker file: " + ex.Message);
                            DialogResult dialogResult = MessageBox.Show("This Bleeper Music Maker file contains invalid elements that do not comply with the syntax of Bleeper Music Maker file format. \n\n" +
                                "Do you want to open this file anyway?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult != DialogResult.Yes)
                            {
                                Debug.WriteLine("User chose not to open the file");
                                createNewFile();
                            }
                            else
                            {
                                Debug.WriteLine("User chose to open the file anyway");
                            }
                        }
                        break;
                    }
                case "<NeoBleeperProjectFile>":
                    {
                        try
                        {
                            is_file_valid = true;
                            saveToolStripMenuItem.Enabled = true;
                            saveAsToolStripMenuItem.Enabled = true;
                            NBPML_File.NeoBleeperProjectFile projectFile = DeserializeXML(filename); if (projectFile != null)
                            {
                                Variables.octave = Convert.ToInt32(projectFile.Settings.RandomSettings.KeyboardOctave);
                                Variables.bpm = Convert.ToInt32(projectFile.Settings.RandomSettings.BPM);
                                numericUpDown_bpm.Value = Convert.ToDecimal(projectFile.Settings.RandomSettings.BPM);
                                trackBar_time_signature.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                                lbl_time_signature.Text = projectFile.Settings.RandomSettings.TimeSignature;
                                Variables.note_silence_ratio = Convert.ToDouble(Convert.ToDouble(Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio)) / 100);
                                trackBar_note_silence_ratio.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                                lbl_note_silence_ratio.Text = projectFile.Settings.RandomSettings.NoteSilenceRatio + "%";
                                comboBox_note_length.SelectedIndex = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteLength);
                                Variables.alternating_note_length = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
                                numericUpDown_alternating_notes.Value = Convert.ToDecimal(projectFile.Settings.RandomSettings.AlternateTime);
                                checkbox_play_note.Checked = projectFile.Settings.PlaybackSettings.NoteClickPlay == "True";
                                checkBox_add_note_to_list.Checked = projectFile.Settings.PlaybackSettings.NoteClickAdd == "True";
                                add_as_note1.Checked = projectFile.Settings.PlaybackSettings.AddNote1 == "True";
                                add_as_note2.Checked = projectFile.Settings.PlaybackSettings.AddNote2 == "True";
                                add_as_note3.Checked = projectFile.Settings.PlaybackSettings.AddNote3 == "True";
                                add_as_note4.Checked = projectFile.Settings.PlaybackSettings.AddNote4 == "True";
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
                                // Assign default values if none of the radiobuttons are checked
                                if (add_as_note1.Checked != true && add_as_note2.Checked != true && add_as_note3.Checked != true && add_as_note4.Checked != true)
                                {
                                    add_as_note1.Checked = true;
                                }
                                // Assign default values if no time signature is found
                                if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.TimeSignature))
                                {
                                    trackBar_time_signature.Value = 4; // Default value
                                    lbl_time_signature.Text = "4";
                                    Debug.WriteLine("Time signature not found, defaulting to 4");
                                }
                                // Assign default values if no note silence ratio is found
                                if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                                {
                                    Variables.note_silence_ratio = 0.5; // Default value
                                    lbl_note_silence_ratio.Text = "50%";
                                    Debug.WriteLine("Note silence ratio not found, defaulting to 50%");
                                }
                                // Assign default values if no note length is found
                                if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteLength))
                                {
                                    comboBox_note_length.SelectedIndex = 0; // Default value
                                    Debug.WriteLine("Note length not found, defaulting to Whole");
                                }
                                // Assign default values if no alternating note length is found
                                if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.AlternateTime))
                                {
                                    Variables.alternating_note_length = 30; // Default value
                                    numericUpDown_alternating_notes.Value = 30;
                                    Debug.WriteLine("Alternating note length not found, defaulting to 30 ms");
                                }
                                // Assign default values if no note click play is found
                                if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteClickPlay))
                                {
                                    checkbox_play_note.Checked = true; // Default value
                                    Debug.WriteLine("Note click play not found, defaulting to true");
                                }
                                // Assign default values if no note click add is found
                                if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteClickAdd))
                                {
                                    checkBox_add_note_to_list.Checked = true; // Default value
                                    Debug.WriteLine("Note click add not found, defaulting to true");
                                }
                                // Assign default values if no note replace is found
                                if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteReplace))
                                {
                                    checkBox_replace.Checked = false; // Default value
                                    Debug.WriteLine("Note replace not found, defaulting to false");
                                }
                                // Assign default values if no note length replace is found
                                if (string.IsNullOrEmpty(projectFile.Settings.PlaybackSettings.NoteLengthReplace))
                                {
                                    checkBox_replace_length.Checked = false; // Default value
                                    Debug.WriteLine("Note length replace not found, defaulting to false");
                                }
                                // Assign default values if no click play note is found
                                if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote1))
                                {
                                    checkBox_play_note1_clicked.Checked = true; // Default value
                                    Debug.WriteLine("Click play note 1 not found, defaulting to true");
                                }
                                if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote2))
                                {
                                    checkBox_play_note2_clicked.Checked = true; // Default value
                                    Debug.WriteLine("Click play note 2 not found, defaulting to true");
                                }
                                if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote3))
                                {
                                    checkBox_play_note3_clicked.Checked = true; // Default value
                                    Debug.WriteLine("Click play note 3 not found, defaulting to true");
                                }
                                if (string.IsNullOrEmpty(projectFile.Settings.ClickPlayNotes.ClickPlayNote4))
                                {
                                    checkBox_play_note3_clicked.Checked = true; // Default value
                                    Debug.WriteLine("Click play note 4 not found, defaulting to true");
                                }
                                // Assign default values if no play note is found
                                if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote1))
                                {
                                    checkBox_play_note1_played.Checked = true; // Default value
                                    Debug.WriteLine("Play note 1 not found, defaulting to true");
                                }
                                if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote2))
                                {
                                    checkBox_play_note2_played.Checked = true; // Default value
                                    Debug.WriteLine("Play note 2 not found, defaulting to true");
                                }
                                if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote3))
                                {
                                    checkBox_play_note3_played.Checked = true; // Default value
                                    Debug.WriteLine("Play note 3 not found, defaulting to true");
                                }
                                if (string.IsNullOrEmpty(projectFile.Settings.PlayNotes.PlayNote4))
                                {
                                    checkBox_play_note4_played.Checked = true; // Default value
                                    Debug.WriteLine("Play note 4 not found, defaulting to true");
                                }
                                noteLabelsUpdate();
                                if (Variables.octave == 9)
                                {
                                    octave10NoteLabelShiftToRight();
                                }
                                currentFilePath = filename;
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
                            Debug.WriteLine("NeoBleeper file opened successfully");
                            isModified = false;
                            UpdateFormTitle();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error opening NeoBleeper file: " + ex.Message);
                            DialogResult dialogResult = MessageBox.Show("This NeoBleeper file contains invalid elements that do not comply with the syntax of NeoBleeper file format. \n\n" +
                                "Do you want to open this file anyway?", string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult != DialogResult.Yes)
                            {
                                Debug.WriteLine("User chose not to open the file");
                                createNewFile();
                            }
                            else
                            {
                                isModified = false;
                                UpdateFormTitle();
                                Debug.WriteLine("User chose to open the file anyway");
                            }
                        }
                        break;
                    }
                default:
                    {
                        is_file_valid = false;
                        Debug.WriteLine("Invalid or corrupted music file");
                        MessageBox.Show("Invalid or corrupted music file", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
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
            if (is_music_playing == true)
            {
                stop_playing();
            }
            openFileDialog.Filter = "NeoBleeper Project Markup Language Files|*.NBPML|Bleeper Music Maker Files|*.BMM|All Files|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                FileParser(filePath);
                initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternating_note_length); // Save the initial state
                commandManager.ClearHistory(); // Reset the history
                // Add the file to the recent files list
                if (is_file_valid == true)
                {
                    if (is_file_valid)
                    {
                        initialMemento = originator.CreateSavedStateMemento(
                            Variables.bpm,
                            Variables.alternating_note_length);
                        isModified = false;
                        UpdateFormTitle();
                    }
                    if (is_file_valid)
                    {
                        initialMemento = originator.CreateSavedStateMemento(
                            Variables.bpm,
                            Variables.alternating_note_length);
                        isModified = false;
                        UpdateFormTitle();
                    }
                    if (Settings1.Default.RecentFiles == null)
                    {
                        Settings1.Default.RecentFiles = new System.Collections.Specialized.StringCollection();
                    }

                    if (!Settings1.Default.RecentFiles.Contains(filePath))
                    {
                        Settings1.Default.RecentFiles.Add(filePath);
                        Settings1.Default.Save();
                    }
                    UpdateRecentFilesMenu();
                }
            }
        }

        private void openFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath) && currentFilePath.ToUpper().EndsWith(".NBPML"))
            {
                SaveToNBPML(currentFilePath);

                // Save current state as SavedStateMemento
                List<ListViewItem> items = new List<ListViewItem>();
                foreach (ListViewItem item in listViewNotes.Items)
                {
                    items.Add((ListViewItem)item.Clone());
                }

                initialMemento = new SavedStateMemento(
                    items,
                    Convert.ToInt32(numericUpDown_bpm.Value),
                    Convert.ToInt32(numericUpDown_alternating_notes.Value));
                isModified = false;
                UpdateFormTitle();
            }
            else
            {
                if (is_music_playing == true)
                {
                    stop_playing();
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "NeoBleeper Project Markup Language Files|*.NBPML|All Files|*.*"
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SaveToNBPML(saveFileDialog.FileName);
                    currentFilePath = saveFileDialog.FileName;
                    this.Text = System.AppDomain.CurrentDomain.FriendlyName + " - " + currentFilePath;
                    isModified = false;
                    UpdateFormTitle();
                    initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternating_note_length);
                }
            }
        }

        private void SaveToNBPML(string filename)
        {
            try
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
                            AlternateTime = numericUpDown_alternating_notes.Value.ToString()
                        },
                        PlaybackSettings = new NBPML_File.PlaybackSettings
                        {
                            NoteClickPlay = checkbox_play_note.Checked.ToString(),
                            NoteClickAdd = checkBox_add_note_to_list.Checked.ToString(),
                            AddNote1 = add_as_note1.Checked.ToString(),
                            AddNote2 = add_as_note2.Checked.ToString(),
                            AddNote3 = add_as_note3.Checked.ToString(),
                            AddNote4 = add_as_note4.Checked.ToString(),
                            NoteReplace = checkBox_replace.Checked.ToString(),
                            NoteLengthReplace = checkBox_replace_length.Checked.ToString()
                        },
                        PlayNotes = new NBPML_File.PlayNotes
                        {
                            PlayNote1 = checkBox_play_note1_played.Checked.ToString(),
                            PlayNote2 = checkBox_play_note2_played.Checked.ToString(),
                            PlayNote3 = checkBox_play_note3_played.Checked.ToString(),
                            PlayNote4 = checkBox_play_note4_played.Checked.ToString()
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

                SerializeXML(filename, projectFile); isModified = false;
                currentFilePath = filename;
                UpdateFormTitle();
                Debug.WriteLine("NeoBleeper file saved successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving NeoBleeper file: " + ex.Message);
                MessageBox.Show("Error saving NeoBleeper file: " + ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trackBar_note_silence_ratio_Scroll(object sender, EventArgs e)
        {
            Variables.note_silence_ratio = (Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100);
            lbl_note_silence_ratio.Text = trackBar_note_silence_ratio.Value.ToString() + "%";
            Debug.WriteLine($"Note silence ratio is set to {trackBar_note_silence_ratio.Value}%");
        }

        private void trackBar_time_signature_Scroll(object sender, EventArgs e)
        {
            lbl_time_signature.Text = trackBar_time_signature.Value.ToString();
            Debug.WriteLine($"Time signature is set to {trackBar_time_signature.Value}");
        }

        private async void noteLabelsUpdate()
        {
            try
            {
                Application.DoEvents();
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
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void octave10NoteLabelShiftToRight()
        {
            try
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
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private void octave10NoteLabelShiftToLeft()
        {
            try
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
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private bool isOctaveChanging = false; // Flag for debouncing

        private async void btn_octave_decrease_Click(object sender, EventArgs e)
        {
            if (isOctaveChanging) return; // Don't allow multiple clicks
            isOctaveChanging = true;

            if (Variables.octave > 2) // Minimum limit check
            {
                Variables.octave--;
                noteLabelsUpdate();
                if(Variables.octave == 8)
                {
                    octave10NoteLabelShiftToRight(); // Shift labels to the right
                }
                if (Variables.octave == 2)
                {
                    btn_octave_decrease.Enabled = false; // Disable button at minimum limit
                }
                btn_octave_increase.Enabled = true; // Enable the other button
            }

            isOctaveChanging = false; // The operation is complete
        }

        private async void btn_octave_increase_Click(object sender, EventArgs e)
        {
            if (isOctaveChanging) return; // Don't allow multiple clicks
            isOctaveChanging = true;

            if (Variables.octave < 9) // Maximum limit check
            {
                Variables.octave++;
                noteLabelsUpdate();

                if (Variables.octave == 9)
                {
                    btn_octave_increase.Enabled = false; // Disable button at maximum limit
                    octave10NoteLabelShiftToLeft(); // Shift labels to the left
                }
                btn_octave_decrease.Enabled = true; // Enable the other button
            }

            isOctaveChanging = false; // The operation is complete
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
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int index = listViewNotes.SelectedIndices[0];
                var selectedItem = listViewNotes.SelectedItems[0];
                var removeNoteCommand = new RemoveNoteCommand(listViewNotes, selectedItem);
                if (index < listViewNotes.Items.Count - 1)
                {
                    listViewNotes.Items[index + 1].Selected = true;
                }
                commandManager.ExecuteCommand(removeNoteCommand);
                isModified = true;
                UpdateFormTitle();
                Debug.WriteLine("Line erased");
            }
        }
        private void button_erase_line_Click(object sender, EventArgs e)
        {
            erase_line();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createNewFile();
            initialMemento = originator.CreateMemento();
            commandManager.ClearHistory(); // Reset the history
            isModified = true;
            UpdateFormTitle();
            Debug.WriteLine("New file created");
        }
        private void createNewFile()
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            this.Text = System.AppDomain.CurrentDomain.FriendlyName;
            currentFilePath = String.Empty;
            if (Variables.octave == 9)
            {
                octave10NoteLabelShiftToRight();
            }
            Variables.octave = 4;
            Variables.bpm = 140;
            Variables.alternating_note_length = 30;
            Variables.note_silence_ratio = 0.5;
            lbl_measure_value.Text = "1";
            lbl_beat_value.Text = "0.0";
            lbl_beat_traditional_value.Text = "1";
            lbl_beat_traditional_value.ForeColor = Color.Green;
            saveAsToolStripMenuItem.Enabled = false;
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
            isModified = false;
            UpdateFormTitle();
        }

        private async Task dummy_play_note(int length) // Dummy play note function for testing purposes
        {
            String note1, note2, note3, note4;
            note1 = listViewNotes.SelectedItems[0].SubItems[1].Text;
            note2 = listViewNotes.SelectedItems[0].SubItems[2].Text;
            note3 = listViewNotes.SelectedItems[0].SubItems[3].Text;
            note4 = listViewNotes.SelectedItems[0].SubItems[4].Text;
            if (note1 != String.Empty || note2 != String.Empty || note3 != String.Empty || note4 != String.Empty)
            {
                UpdateLabelVisible(true);
                await Task.Delay(length);
                UpdateLabelVisible(false);
            }
            else
            {
                await Task.Delay(length);
            }
        }
        private async Task play_music(int index)
        {
            bool nonStopping = false;
            while (listViewNotes.SelectedItems.Count > 0 && is_music_playing)
            {
                if (trackBar_note_silence_ratio.Value == 100)
                {
                    nonStopping = true;
                }
                else
                {
                    nonStopping = false;
                }
                Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                if (Variables.bpm != 0)
                {
                    Variables.miliseconds_per_beat = Convert.ToInt32(Math.Truncate((float)60000 / Variables.bpm));
                }
                line_length_calculator();
                note_length_calculator();
                // Calculate full duration before playing
                int noteDuration = final_note_length;
                int waitDuration = line_length;

                // Play note and continue waiting
                if (Program.MIDIDevices.useMIDIoutput == true)
                {
                    MIDIIOUtils.ChangeInstrument(MIDIIOUtils._midiOut, Program.MIDIDevices.MIDIOutputInstrument,
                            Program.MIDIDevices.MIDIOutputDeviceChannel);
                    play_note_in_line_from_MIDIOutput(listViewNotes.SelectedIndices[0],
                    checkBox_play_note1_played.Checked,
                    checkBox_play_note2_played.Checked,
                    checkBox_play_note3_played.Checked,
                    checkBox_play_note4_played.Checked, noteDuration);
                }
                play_note_in_line(
                checkBox_play_note1_played.Checked,
                checkBox_play_note2_played.Checked,
                checkBox_play_note3_played.Checked,
                checkBox_play_note4_played.Checked,
                    noteDuration, nonStopping);
                // Wait between each note
                if (waitDuration - noteDuration > 0) 
                { 
                    await Task.Delay(waitDuration - noteDuration); 
                }
                    // Do ListView update in UI thread
                    UpdateListViewSelectionSync(index);
            }
            if (trackBar_note_silence_ratio.Value == 100)
            {
                stopAllNotesAfterPlaying();
            }
        }

        // Sync ListView update method
        private void UpdateListViewSelectionSync(int startIndex)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int currentIndex = listViewNotes.SelectedIndices[0];
                int nextIndex = currentIndex + 1;
                if (nextIndex < listViewNotes.Items.Count)
                {
                    // Select next note 
                    listViewNotes.Items[nextIndex].Selected = true;
                    listViewNotes.EnsureVisible(nextIndex);
                }
                else if (checkBox_loop.Checked == true)
                {
                    // Loop back to the beginning if looping is enabled
                    if (listViewNotes.Items.Count > 0)
                    {
                        listViewNotes.Items[startIndex].Selected = true;
                        listViewNotes.EnsureVisible(startIndex);
                    }
                }
                else if (checkBox_loop.Checked == false)
                {
                    // Stop playing if no more notes
                    stop_playing();
                }
            }
        }
        public void play_all()
        {
            if (listViewNotes.Items.Count > 0)
            {
                checkBox_do_not_update.Enabled = false;
                keyboard_panel.Enabled = false;
                numericUpDown_bpm.Enabled = false;
                numericUpDown_alternating_notes.Enabled = false;
                button_play_all.Enabled = false;
                playAllToolStripMenuItem.Enabled = false;
                button_play_from_selected_line.Enabled = false;
                playFromSelectedLineToolStripMenuItem.Enabled = false;
                button_stop_playing.Enabled = true;
                stopPlayingToolStripMenuItem.Enabled = true;
                is_music_playing = true;
                listViewNotes.Items[0].Selected = true;
                listViewNotes.EnsureVisible(0);
                Debug.WriteLine("Music is playing");
                play_music(0);
            }
        }
        public void play_from_selected_line()
        {
            if (listViewNotes.Items.Count > 0)
            {
                checkBox_do_not_update.Enabled = false;
                keyboard_panel.Enabled = false;
                numericUpDown_bpm.Enabled = false;
                numericUpDown_alternating_notes.Enabled = false;
                button_play_all.Enabled = false;
                playAllToolStripMenuItem.Enabled = false;
                button_play_from_selected_line.Enabled = false;
                playFromSelectedLineToolStripMenuItem.Enabled = false;
                button_stop_playing.Enabled = true;
                stopPlayingToolStripMenuItem.Enabled = true;
                is_music_playing = true;
                if (listViewNotes.SelectedItems.Count < 1)
                {
                    listViewNotes.Items[0].Selected = true;
                    listViewNotes.EnsureVisible(0);
                    Debug.WriteLine("Music is playing");
                    play_music(0);
                }
                else
                {
                    int index = listViewNotes.SelectedItems[0].Index;
                    listViewNotes.EnsureVisible(index);
                    Debug.WriteLine("Music is playing");
                    play_music(index);
                }
            }
        }
        private void button_play_all_Click(object sender, EventArgs e)
        {
            play_all();
        }

        private void button_play_from_selected_line_Click(object sender, EventArgs e)
        {
            play_from_selected_line();
        }
        private void button_stop_playing_Click(object sender, EventArgs e)
        {
            stop_playing();
        }
        public void stop_playing()
        {
            keyboard_panel.Enabled = true;
            checkBox_do_not_update.Enabled = true;
            numericUpDown_bpm.Enabled = true;
            numericUpDown_alternating_notes.Enabled = true;
            button_play_all.Enabled = true;
            playAllToolStripMenuItem.Enabled = true;
            button_play_from_selected_line.Enabled = true;
            playFromSelectedLineToolStripMenuItem.Enabled = true;
            button_stop_playing.Enabled = false;
            stopPlayingToolStripMenuItem.Enabled = false;
            is_music_playing = false;
            Debug.WriteLine("Music stopped");
            OnMusicStopped(EventArgs.Empty);
        }
        private System.Timers.Timer metronomeTimer;
        private int beatCount = 0;
        private readonly object syncLock = new object();
        private volatile bool isLabelVisible = false;

        // Prepare sound buffers in advance
        private SoundPlayer accentBeatSound; // For first beat
        private SoundPlayer normalBeatSound; // For other beats
        protected virtual void OnMusicStopped(EventArgs e)
        {
            MusicStopped?.Invoke(this, e);
        }

        private async void play_metronome_sound(int frequency, int length)
        {
            if (MIDIIOUtils._midiOut != null && Program.MIDIDevices.useMIDIoutput == true)
            {
                await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, frequency, length);
            }
        }
        private void InitializeMetronome()
        {
            // Pre-load sounds to eliminate initialization delay
            try
            {
                // If NotePlayer uses SoundPlayer internally, consider replacing with direct SoundPlayer usage
                // Or implement a mechanism to pre-buffer the sounds
                PreloadSounds();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error initializing sounds: " + ex.Message);
            }

            metronomeTimer = new System.Timers.Timer();
            metronomeTimer.Elapsed += MetronomeTimer_Elapsed;
        }

        private void PreloadSounds()
        {
            // Option 1: If you can create WAV files for your metronome sounds
            // accentBeatSound = new SoundPlayer(Properties.Resources.AccentBeat);
            // normalBeatSound = new SoundPlayer(Properties.Resources.NormalBeat);
            // accentBeatSound.LoadAsync();
            // normalBeatSound.LoadAsync();

            // Option 2: If using a different audio system, implement appropriate preloading
        }

        private void MetronomeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            metronomeTimer.Stop(); // Temporarily stop to prevent overlapping

            try
            {
                if (!checkBox_metronome.Checked)
                    return;

                // Play the appropriate sound first for minimal latency
                PlayBeatSound(beatCount == 0);

                // Then update the UI (which is less time-critical)
                ShowBeatLabel();

                // Update beat counter
                beatCount = (beatCount + 1) % trackBar_time_signature.Value;

                // Schedule the next beat
                metronomeTimer.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Metronome error: " + ex.Message);
            }
        }

        private void PlayBeatSound(bool isAccent)
        {
            // Option 1: Use pre-loaded SoundPlayer if implemented
            // (isAccent ? accentBeatSound : normalBeatSound).Play();

            // Option 2: Use your NotePlayer but optimize for immediate playback
            int frequency = isAccent ? 1000 : 498;

            // Important: Play sound on high-priority thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                play_metronome_sound(frequency, 30);
                NotePlayer.play_note(frequency, 30);
            });
        }

        private void ShowBeatLabel()
        {
            if (isLabelVisible) return;
            isLabelVisible = true;

            UpdateLabelVisible(true);

            // Schedule hiding the label
            ThreadPool.QueueUserWorkItem(async state =>
            {
                await Task.Delay(75);
                UpdateLabelVisible(false);
                isLabelVisible = false;
            });
        }

        private async void UpdateLabelVisible(bool visible)
        {
            Task.Run(() =>
            {
                if (label_beep.InvokeRequired)
                {
                    label_beep.BeginInvoke(new Action(() =>
                    {
                        if (!label_beep.IsDisposed)
                        {
                            label_beep.Visible = visible;
                        }
                    }));
                }
                else if (!label_beep.IsDisposed)
                {
                    label_beep.Visible = visible;
                }
            });
        }

        private void StartMetronome()
        {
            beatCount = 0;
            double interval = 60000.0 / Variables.bpm;
            metronomeTimer.Interval = interval;
            metronomeTimer.Start();
        }

        private void StopMetronome()
        {
            metronomeTimer.Stop();
        }

        private void checkBox_metronome_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_metronome.Checked == true)
            {
                checkBox_metronome.BackColor = Settings1.Default.metronome_color;
                switch (Settings1.Default.theme)
                {
                    case 0:
                        {
                            if (check_system_theme.IsDarkTheme() == true)
                            {
                                checkBox_metronome.ForeColor = SystemColors.ControlText;
                            }
                            break;
                        }
                    case 2:
                        {
                            checkBox_metronome.ForeColor = SystemColors.ControlText;
                            break;
                        }
                }
                StartMetronome();
                Debug.WriteLine("Metronome started");
            }
            else
            {
                switch (Settings1.Default.theme)
                {
                    case 0:
                        {
                            if (check_system_theme.IsDarkTheme() == true)
                            {
                                checkBox_metronome.BackColor = Color.FromArgb(32, 32, 32);
                                checkBox_metronome.ForeColor = Color.White;
                            }
                            else
                            {
                                checkBox_metronome.BackColor = System.Drawing.Color.Transparent;
                            }
                            break;
                        }
                    case 1:
                        {
                            checkBox_metronome.BackColor = System.Drawing.Color.Transparent;
                            break;
                        }
                    case 2:
                        {
                            checkBox_metronome.BackColor = Color.FromArgb(32, 32, 32);
                            checkBox_metronome.ForeColor = Color.White;
                            break;
                        }
                }
                StopMetronome();
                Debug.WriteLine("Metronome stopped");
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
            Debug.WriteLine("Blank line added");
        }
        private void clear_note_1()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 1);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Debug.WriteLine("Note 1 cleared");
        }
        private void clear_note_2()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 2);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Debug.WriteLine("Note 2 cleared");
        }
        private void clear_note_3()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 3);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Debug.WriteLine("Note 3 cleared");
        }
        private void clear_note_4()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 4);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Debug.WriteLine("Note 4 cleared");
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
            Debug.WriteLine("Line unselected");
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
        bool is_clicked = false;
        private void listViewNotes_Click(object sender, EventArgs e) // Stop music and play clicked note
        {
            stop_playing();
            if (listViewNotes.FocusedItem != null && listViewNotes.SelectedItems.Count > 0)
            {
                Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                if (Variables.bpm != 0)
                {
                    Variables.miliseconds_per_beat = Convert.ToInt32(Math.Truncate((double)(60000 / Variables.bpm)));
                }
                if (listViewNotes.SelectedItems.Count > 0)
                {
                    updateIndicators(listViewNotes.SelectedIndices[0]);
                }
                note_length_calculator();
                keyboard_panel.Enabled = false;
                numericUpDown_alternating_notes.Enabled = false;
                numericUpDown_bpm.Enabled = false;
                checkBox_do_not_update.Enabled = false;
                if (Program.MIDIDevices.useMIDIoutput == true)
                {
                    Task.Run(() =>
                    {
                        play_note_in_line_from_MIDIOutput(listViewNotes.SelectedIndices[0],
                        checkBox_play_note1_played.Checked,
                        checkBox_play_note2_played.Checked,
                        checkBox_play_note3_played.Checked,
                        checkBox_play_note4_played.Checked, final_note_length);
                    });
                }
                bool nonStopping;
                if (trackBar_note_silence_ratio.Value == 100)
                {
                    nonStopping = true;
                }
                else
                {
                    nonStopping = false;
                }
                play_note_in_line(checkBox_play_note1_clicked.Checked, checkBox_play_note2_clicked.Checked,
                checkBox_play_note3_clicked.Checked, checkBox_play_note4_clicked.Checked,
                final_note_length, nonStopping);
                if (nonStopping == true)
                {
                    stopAllNotesAfterPlaying();
                }
                keyboard_panel.Enabled = true;
                numericUpDown_alternating_notes.Enabled = true;
                numericUpDown_bpm.Enabled = true;
                checkBox_do_not_update.Enabled = true;
                Debug.WriteLine("Selected line: " + listViewNotes.FocusedItem.Index);
            }
        }

        private void numericUpDown_bpm_ValueChanged(object sender, EventArgs e)
        {
            // Skip the process if triggered by command
            if (numericUpDown_bpm.Tag as string == "SkipValueChanged")
                return;

            // Codes of normal value change rendering
            int oldValue = Variables.bpm;
            int newValue = Convert.ToInt32(numericUpDown_bpm.Value);

            if (oldValue != newValue)
            {
                var command = new ValueChangeCommand(
                    "bpm",
                    oldValue,
                    newValue,
                    numericUpDown_bpm,
                    true);

                commandManager.ExecuteCommand(command);
                isModified = true;
                UpdateFormTitle();
            }

            Debug.WriteLine($"BPM: {Variables.bpm}");
        }

        private void numericUpDown_alternating_notes_ValueChanged(object sender, EventArgs e)
        {
            // Skip the process if triggered by command
            if (numericUpDown_alternating_notes.Tag as string == "SkipValueChanged")
                return;

            // Codes of normal value change rendering
            int oldValue = Variables.alternating_note_length;
            int newValue = Convert.ToInt32(numericUpDown_alternating_notes.Value);

            if (oldValue != newValue)
            {
                var command = new ValueChangeCommand(
                    "alternating_note_length",
                    oldValue,
                    newValue,
                    numericUpDown_alternating_notes,
                    false);

                commandManager.ExecuteCommand(command);
                isModified = true;
                UpdateFormTitle();
            }

            Debug.WriteLine($"Alternating note length: {Variables.alternating_note_length}");
        }

        private void note_length_calculator()
        {
            if (listViewNotes.SelectedItems != null && listViewNotes.SelectedItems.Count > 0 &&
                listViewNotes.Items != null && listViewNotes.Items.Count > 0)
            {
                if (Variables.bpm != 0)
                {
                    int selected_line = listViewNotes.SelectedIndices[0];
                    if (selected_line >= 0)
                    {
                        if (listViewNotes.Items[selected_line].SubItems[0].Text == "Whole")
                        {
                            note_length = Variables.miliseconds_per_beat * 4;
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "Half")
                        {
                            note_length = Variables.miliseconds_per_beat * 2;
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "Quarter")
                        {
                            note_length = Variables.miliseconds_per_beat;
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/8")
                        {
                            note_length = Convert.ToInt32(Variables.miliseconds_per_beat * 0.5);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/16")
                        {
                            note_length = Convert.ToInt32(Variables.miliseconds_per_beat * 0.25);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/32")
                        {
                            note_length = Convert.ToInt32(Variables.miliseconds_per_beat * 0.125);
                        }
                        if (listViewNotes.Items[selected_line].SubItems[5].Text == "Dot")
                        {
                            note_length = Convert.ToInt32(note_length * 1.5);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[5].Text == "Tri")
                        {
                            note_length = Convert.ToInt32(note_length * 0.333);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Sta")
                        {
                            note_length = Convert.ToInt32(note_length * 0.5);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Spi")
                        {
                            note_length = Convert.ToInt32(note_length * 0.25);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Fer")
                        {
                            note_length = note_length * 2;
                        }
                        final_note_length = Convert.ToInt32(Math.Truncate(note_length * Variables.note_silence_ratio));
                    }
                }
            }
        }
        int line_length = 0;
        private void line_length_calculator()
        {
            if (listViewNotes.SelectedItems != null && listViewNotes.SelectedItems.Count > 0 &&
                listViewNotes.Items != null && listViewNotes.Items.Count > 0)
            {
                if (Variables.bpm != 0)
                {
                    int selected_line = listViewNotes.SelectedIndices[0];
                    if (selected_line >= 0)
                    {
                        if (listViewNotes.Items[selected_line].SubItems[0].Text == "Whole")
                        {
                            line_length = Variables.miliseconds_per_beat * 4;
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "Half")
                        {
                            line_length = Variables.miliseconds_per_beat * 2;
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "Quarter")
                        {
                            line_length = Variables.miliseconds_per_beat;
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/8")
                        {
                            line_length = Convert.ToInt32(Variables.miliseconds_per_beat * 0.5);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/16")
                        {
                            line_length = Convert.ToInt32(Variables.miliseconds_per_beat * 0.25);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[0].Text == "1/32")
                        {
                            line_length = Convert.ToInt32(Variables.miliseconds_per_beat * 0.125);
                        }
                        if (listViewNotes.Items[selected_line].SubItems[5].Text == "Dot")
                        {
                            line_length = Convert.ToInt32(line_length * 1.5);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[5].Text == "Tri")
                        {
                            line_length = Convert.ToInt32(line_length * 0.333);
                        }
                        else if (listViewNotes.Items[selected_line].SubItems[6].Text == "Fer")
                        {
                            line_length *= 2;
                        }
                    }
                }
            }
        }
        private void stopAllNotesAfterPlaying()
        {
            NotePlayer.StopAllNotes();
            UpdateLabelVisible(false);
        }
        private void play_note_in_line(bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length, bool nonStopping = false) // Play note in a line
        {
            if (cancellationTokenSource.Token.IsCancellationRequested) return;
            Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);

            string note1 = string.Empty, note2 = string.Empty, note3 = string.Empty, note4 = string.Empty;
            double note1_frequency = 0, note2_frequency = 0, note3_frequency = 0, note4_frequency = 0;

            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selected_line = listViewNotes.SelectedIndices[0];

                // Take music note names from the selected line
                note1 = play_note1 ? listViewNotes.Items[selected_line].SubItems[1].Text : string.Empty;
                note2 = play_note2 ? listViewNotes.Items[selected_line].SubItems[2].Text : string.Empty;
                note3 = play_note3 ? listViewNotes.Items[selected_line].SubItems[3].Text : string.Empty;
                note4 = play_note4 ? listViewNotes.Items[selected_line].SubItems[4].Text : string.Empty;

                // Calculate frequencies from note names
                if (!string.IsNullOrEmpty(note1))
                    note1_frequency = GetFrequencyFromNoteName(note1);

                if (!string.IsNullOrEmpty(note2))
                    note2_frequency = GetFrequencyFromNoteName(note2);

                if (!string.IsNullOrEmpty(note3))
                    note3_frequency = GetFrequencyFromNoteName(note3);

                if (!string.IsNullOrEmpty(note4))
                    note4_frequency = GetFrequencyFromNoteName(note4);
            }


            if (radioButtonPlay_alternating_notes1.Checked == true) // Odd column mode
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
                else if (note1 == note4)
                {
                    Variables.alternating_note_length *= 2;
                    note4 = string.Empty;
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
            else if (radioButtonPlay_alternating_notes2.Checked == true) // Even column mode
            {
                if (note1 == note3 && note3 == note2 && note2 == note4)
                {
                    Variables.alternating_note_length *= 4;
                    note2 = note3 = note4 = string.Empty;
                }
                else if (note1 == note3 && note3 == note2)
                {
                    Variables.alternating_note_length *= 2;
                    note2 = note3 = string.Empty;
                }
                else if (note1 == note2)
                {
                    if (note1 == note3 && note2 == note4)
                    {
                        Variables.alternating_note_length *= 4;
                        note3 = note4 = string.Empty;
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
                    Variables.alternating_note_length *= 2;
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
            if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 == string.Empty || note3 == null) && (note4 == string.Empty || note4 == null))
            {
                if (nonStopping == true)
                {
                    stopAllNotesAfterPlaying();
                }
                NonBlockingSleep.Sleep(length);
                return;
            }
            if ((note1 != string.Empty || note1 != null) && (note2 == string.Empty || note2 == null) && (note3 == string.Empty || note3 == null) && (note4 == string.Empty || note4 == null))
            {
                UpdateLabelVisible(true);
                NotePlayer.play_note(Convert.ToInt32(note1_frequency), length, nonStopping);
                if (nonStopping == false)
                {
                    UpdateLabelVisible(false);
                }
            }
            else if ((note1 == string.Empty || note1 == null) && (note2 != string.Empty || note2 != null) && (note3 == string.Empty || note3 == null) && (note4 == string.Empty || note4 == null))
            {
                UpdateLabelVisible(true);
                NotePlayer.play_note(Convert.ToInt32(note2_frequency), length, nonStopping);
                if (nonStopping == false)
                {
                    UpdateLabelVisible(false);
                }
            }

                else if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 != string.Empty || note3 != null) && (note4 == string.Empty || note4 == null))
                {
                    UpdateLabelVisible(true);
                    NotePlayer.play_note(Convert.ToInt32(note3_frequency), length, nonStopping);
                    if (nonStopping == false)
                    {
                        UpdateLabelVisible(false);
                    }
                }
                else if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 == string.Empty || note3 == null) && (note4 != string.Empty || note4 != null))
                {
                    UpdateLabelVisible(true);
                    NotePlayer.play_note(Convert.ToInt32(note4_frequency), length, nonStopping);
                    if (nonStopping == false)
                    {
                        UpdateLabelVisible(false);
                    }
                }
                else
                {
                    if (play_note1 == true || play_note2 == true || play_note3 == true || play_note4 == true)
                    {
                        UpdateLabelVisible(true);
                    }
                    int note_order = 1;
                    int last_note_order = Convert.ToInt32(Math.Truncate((double)length / Variables.alternating_note_length));
                    if (radioButtonPlay_alternating_notes1.Checked == true)
                    {
                        string[] note_series = { note1, note2, note3, note4 };
                        do
                        {
                            foreach (string note in note_series)
                            {
                                if (!string.IsNullOrEmpty(note))
                                {
                                    double frequency = GetFrequencyFromNoteName(note);
                                    NotePlayer.play_note(Convert.ToInt32(frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                    note_order++;
                                }
                            }
                        }
                        while (note_order <= last_note_order);
                    }

                else if (radioButtonPlay_alternating_notes2.Checked == true)
                {
                    string[] note_series = { note1, note2, note3, note4 };
                    do
                    {
                        // Odd number columns first (Note1 and Note3)
                        for (int i = 0; i < 4; i += 2)
                        {
                            if (note_series[i] != string.Empty)
                            {
                                if (i == 0)
                                {
                                    NotePlayer.play_note(Convert.ToInt32(note1_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                }
                                else if (i == 2)
                                {
                                    NotePlayer.play_note(Convert.ToInt32(note3_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                }
                                note_order++;
                            }
                        }
                        // Even number columns then (Note2 and Note4)
                        for (int i = 1; i < 4; i += 2)
                        {
                            if (note_series[i] != string.Empty)
                            {
                                if (i == 1)
                                {
                                    NotePlayer.play_note(Convert.ToInt32(note2_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                }
                                else if (i == 3)
                                {
                                    NotePlayer.play_note(Convert.ToInt32(note4_frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                                }
                                note_order++;
                            }
                        }
                    }
                    while (note_order <= last_note_order);
                }
                if (cancellationTokenSource.Token.IsCancellationRequested) return;
                if (play_note1 == true || play_note2 == true || play_note3 == true || play_note4 == true)
                {
                    if (nonStopping == false)
                    {
                        UpdateLabelVisible(false);
                    }
                }
            }
        }

        private void listViewNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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
                    if (checkBox_do_not_update.Checked == false && is_music_playing == true)
                    {
                        updateIndicators(selectedLine);
                    }
                }
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private void updateIndicators(int Line, bool clicked = false)
        {
            if (listViewNotes.Items.Count > 0)
            {
                int measure = 1;
                double beat = 0;
                double beat_number = 1;
                if (listViewNotes.SelectedItems.Count > 0)
                {
                    for (int i = 1; i <= Line; i++)
                    {
                        beat += Convert.ToDouble(NoteLengthToBeats(listViewNotes.Items[i]));
                        if (beat >= trackBar_time_signature.Value)
                        {
                            measure++;
                            beat = 0;

                        }
                    }
                }
                beat_number = beat + 1;
                Task.Run(() =>
                {
                    lbl_measure_value.Text = measure.ToString();
                    lbl_beat_value.Text = FormatNumber(beat_number);
                    lbl_beat_traditional_value.Text = ConvertDecimalBeatToTraditional(beat);
                    lbl_beat_traditional_value.ForeColor = set_traditional_beat_color(lbl_beat_traditional_value.Text);
                });
                if (checkBox_play_beat_sound.Checked==true && clicked == false && beat_number - Math.Truncate(beat_number) == 0)
                {
                    switch(Program.BeatTypes.beat_type)
                    {
                        case 0:
                            play_beat();
                            break;
                        case 1:
                            if(beat_number % 2 != 0)
                            {
                                play_beat();
                            }
                            break;
                        case 2:
                            if (beat_number % 2 == 0)
                            {
                                play_beat();
                            }
                            break;
                    }
                    
                }
            }
        }
        private void play_beat()
        {
            // Basic frequencies
            int snareFrequency = Convert.ToInt32(GetFrequencyFromNoteName("D2"));
            int kickFrequency = Convert.ToInt32(GetFrequencyFromNoteName("E2"));
            int hiHatFrequency = Convert.ToInt32(GetFrequencyFromNoteName("F#2"));

            // Calculate length based on BPM
            double calculatedLengthFactor = 0.1; // Factor to adjust the length of the sound
            int length = Math.Max(1, Convert.ToInt32(Math.Truncate((double)(Variables.miliseconds_per_beat / 8) * calculatedLengthFactor)));

            // Perkusif desen oluþturma
            for (int i = 0; i < 2; i++) // 2 beats
            {
                if (i % 2 == 0) // Kick 
                {
                    NotePlayer.play_note(kickFrequency, length);
                }
                else // Snare
                {
                    NotePlayer.play_note(snareFrequency, length);
                }

                // Her vuruþ arasýnda kýsa bir duraklama
            }
            NonBlockingSleep.Sleep(length / 2);
            // Add hi-hat sound
            NotePlayer.play_note(hiHatFrequency, length / 2);

            // Percussion sound for MIDI output
            if (Program.MIDIDevices.useMIDIoutput)
            {
                Task.Run(async () =>
                {
                    int midiSnare = 38; // Snare Drum
                    int midiKick = 35;  // Bass Drum
                    int midiHiHat = 42; // Closed Hi-Hat

                    await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, midiKick, length);
                    await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, midiSnare, length);
                    await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, midiHiHat, length / 2);
                });
            }
        }
        public static string ConvertDecimalBeatToTraditional(double decimalBeat)
        {
            double fractionOfWhole = decimalBeat / 4;
            double thirtyseconds = Math.Round(fractionOfWhole * 32);

            if (Math.Abs((decimalBeat / 4) - (thirtyseconds / 32)) > 0.00001)
            {
                if (thirtyseconds > 16)
                {
                    int wholeNumber = (int)Math.Floor(thirtyseconds / 16);
                    return (wholeNumber + 1) + " (Error)";
                }
                else
                {
                    return "1 (Error)";
                }
            }

            int numerator = (int)thirtyseconds;
            int denominator = 32;
            int gcd = GCD(numerator, denominator);

            numerator /= gcd;
            denominator /= gcd;

            if (numerator == denominator)
            {
                return "1";
            }
            else if (numerator == 0)
            {
                return "1"; // Return "1" only
            }
            else if (numerator > denominator) // For fractions that greater than 1
            {
                int wholeNumber = numerator / denominator;
                int remaining = numerator % denominator;

                if (remaining == 0)
                {
                    return (wholeNumber + 1).ToString(); // Whole number
                }
                else
                {
                    return (wholeNumber + 1) + " " + remaining + "/" + denominator; // Whole number and remaining fraction
                }
            }
            else if (denominator == 1)
            {
                return numerator.ToString();
            }
            else if (denominator == 2)
            {
                return "1 " + numerator + "/2"; // "1 fraction" format for fractions that smaller than 1
            }
            else if (denominator == 4)
            {
                return "1 " + numerator + "/4"; // "1 fraction" format for fractions that smaller than 1
            }
            else if (denominator == 8)
            {
                return "1 " + numerator + "/8"; // "1 fraction" format for fractions that smaller than 1
            }
            else if (denominator == 16)
            {
                return "1 " + numerator + "/16"; // "1 fraction" format for fractions that smaller than 1
            }
            else if (denominator == 32)
            {
                return "1 " + numerator + "/32"; // "1 fraction" format for fractions that smaller than 1
            }
            else
            {
                int wholeNumber = (int)Math.Floor(thirtyseconds / 32);
                return (wholeNumber + 1) + " (Error)";
            }
        }

        public static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
        private Color set_traditional_beat_color(string text)
        {
            Color text_color;
            if (text.Contains("Error"))
            {
                text_color = Color.Red;
            }
            else
            {
                text_color = Color.Green;
            }
            return text_color;
        }
        private double NoteLengthToBeats(ListViewItem listViewItem)
        {
            double length, mod, art;
            switch (listViewItem.SubItems[0].Text)
            {
                case "Whole":
                    length = 4;
                    break;
                case "Half":
                    length = 2;
                    break;
                case "Quarter":
                    length = 1;
                    break;
                case "1/8":
                    length = 1.0 / 2.0;
                    break;
                case "1/16":
                    length = 1.0 / 4.0;
                    break;
                case "1/32":
                    length = 1.0 / 8.0;
                    break;
                default:
                    length = 1;
                    break;
            }
            switch (listViewItem.SubItems[5].Text)
            {
                case "Dot":
                    mod = 1.5;
                    break;
                case "Tri":
                    mod = 1.0 / 3.0;
                    break;
                default:
                    mod = 1;
                    break;
            }
            switch (listViewItem.SubItems[6].Text)
            {
                case "Sta":
                    art = 1.0 / 2.0;
                    break;
                case "Spi":
                    art = 1.0 / 4.0;
                    break;
                case "Fer":
                    art = 2;
                    break;
                default:
                    art = 1;
                    break;
            }
            return length * mod * art;
        }
        public static string FormatNumber(double number)
        {
            string numberString = number.ToString("G17", System.Globalization.CultureInfo.InvariantCulture); // Maksimum hassasiyetle string'e çevir
            int decimalIndex = numberString.IndexOf('.');

            if (decimalIndex == -1)
            {
                // If number is an integer, return it as an integer
                return number.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                int decimalLength = numberString.Length - decimalIndex - 1;

                // Determine the number of decimal places to display
                if (decimalLength > 4)
                {
                    return number.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    return number.ToString($"F{decimalLength}", System.Globalization.CultureInfo.InvariantCulture);
                }
            }
        }
        private void openSynchronizedPlayWindow()
        {
            synchronized_play_window syncPlayWindow = new synchronized_play_window(this);
            syncPlayWindow.Show();
            checkBox_synchronized_play.Tag = syncPlayWindow;
        }
        private void closeSynchronizedPlayWindow()
        {
            synchronized_play_window syncPlayWindow = checkBox_synchronized_play.Tag as synchronized_play_window;
            syncPlayWindow.Close();
        }
        private void checkBox_synchronized_play_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_synchronized_play.Checked == true)
            {
                openSynchronizedPlayWindow();
                Debug.WriteLine("Synchronized play window is opened.");
            }
            else if (checkBox_synchronized_play.Checked == false)
            {
                closeSynchronizedPlayWindow();
                Debug.WriteLine("Synchronized play window is closed.");
            }
        }

        private void trackBar_note_silence_ratio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void trackBar_time_signature_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label_note_Click(object sender, EventArgs e)
        {

        }

        private void stop_system_speaker_beep()
        {
            if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true && Program.creating_sounds.is_system_speaker_muted == false &&
                Program.creating_sounds.create_beep_with_soundcard == false)
            {
                RenderBeep.BeepClass.StopBeep();
            }
        }

        private void main_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            checkBox_metronome.Checked = false;
            stop_playing();
            cancellationTokenSource.Cancel();
            isClosing = true;
            stop_system_speaker_beep();
        }

        private void main_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            checkBox_metronome.Checked = false;
            stop_playing();
            cancellationTokenSource.Cancel();
            isClosing = true;
            stop_system_speaker_beep();
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
            if (is_music_playing == true)
            {
                stop_playing();
            }
            if (initialMemento == null)
            {
                MessageBox.Show("No saved version to rewind to.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                // Create new command and run
                var rewindCommand = new RewindCommand(originator, initialMemento);
                commandManager.ExecuteCommand(rewindCommand);

                // Clear history
                commandManager.ClearHistory();

                // Reset changes
                isModified = false;
                UpdateFormTitle();

                // Log states of variables
                Debug.WriteLine($"Rewind to saved version - BPM: {Variables.bpm}, Alt Notes: {Variables.alternating_note_length}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error rewinding: {ex.Message}");
                MessageBox.Show($"Error rewinding to saved version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox_mute_system_speaker_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.creating_sounds.is_system_speaker_muted == false && checkBox_mute_system_speaker.Checked == false)
            {
                if (Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
                {
                    RenderBeep.BeepClass.StopBeep();
                }
            }
            Program.creating_sounds.is_system_speaker_muted = checkBox_mute_system_speaker.Checked;
            Debug.WriteLine("System speaker is muted: " + Program.creating_sounds.is_system_speaker_muted);
        }
        private async void show_keyboard_keys_shortcut()
        {
            try
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
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void hide_keyboard_keys_shortcut()
        {
            try
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
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
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
                try
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
                        Debug.WriteLine("The file you dragged is not supported by NeoBleeper or is corrupted.");
                        MessageBox.Show("The file you dragged is not supported by NeoBleeper or is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("The file you dragged is corrupted or the file is in use by another process.");
                    MessageBox.Show("The file you dragged is corrupted or the file is in use by another process.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                namespaces.Add(string.Empty, string.Empty); // to remove namespaces

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
            if (is_music_playing == true)
            {
                stop_playing();
            }
            if (checkBox_synchronized_play.Checked == true)
            {
                checkBox_synchronized_play.Checked = false;
            }
            openFileDialog.Filter = "MIDI Files|*.mid";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (IsMidiFile(openFileDialog.FileName))
                {
                    MIDI_file_player midi_file_player = new MIDI_file_player(openFileDialog.FileName);
                    midi_file_player.ShowDialog();
                    Debug.WriteLine("MIDI file is opened.");
                }
                else
                {
                    MessageBox.Show("This file is not a valid MIDI file, or it is corrupted or is being used by another process.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine("This file is not a valid MIDI file, or it is corrupted or is being used by another process.");
                }
            }
        }

        private bool IsMidiFile(string filePath)
        {
            try
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

                // MIDI files start with the header "MThd"
                return header[0] == 'M' && header[1] == 'T' && header[2] == 'h' && header[3] == 'd';
            }
            catch (Exception)
            {
                return false;
            }
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
            play_all();
        }

        private void playFromSelectedLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            play_from_selected_line();
        }

        private void stopPlayingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stop_playing();
        }

        private void checkBox_play_note1_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 1 when clicked is changed to: {checkBox_play_note1_clicked.Checked}");
        }

        private void checkBox_play_note2_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 2 when clicked is changed to: {checkBox_play_note2_clicked.Checked}");
        }

        private void checkBox_play_note3_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 3 when clicked is changed to: {checkBox_play_note3_clicked.Checked}");
        }

        private void checkBox_play_note4_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 4 when clicked is changed to: {checkBox_play_note4_clicked.Checked}");
        }

        private void checkBox_play_note1_played_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 1 when played is changed to: {checkBox_play_note1_played.Checked}");
        }
        private void checkBox_play_note2_played_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 2 when played is changed to: {checkBox_play_note2_played.Checked}");
        }
        private void checkBox_play_note3_played_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 3 when played is changed to: {checkBox_play_note3_played.Checked}");
        }

        private void checkBox_play_note4_played_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of play note 4 when played is changed to: {checkBox_play_note4_played.Checked}");
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listViewNotes.SelectedItems[0];
                StringBuilder clipboardText = new StringBuilder();

                foreach (ListViewItem.ListViewSubItem subItem in selectedItem.SubItems)
                {
                    clipboardText.Append(subItem.Text + "\t"); // Combine the text with a tab character
                }

                // Remove the last tab character
                if (clipboardText.Length > 0)
                {
                    clipboardText.Length--;
                }

                // Copy to clipboard
                Clipboard.SetText(clipboardText.ToString());
                Debug.WriteLine("Copy to clipboard is executed.");
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                string[] subItems = clipboardText.Split('\t');

                if (subItems.Length >= 7) // Ensure there are enough subitems
                {
                    ListViewItem newItem = new ListViewItem(subItems[0]); // First subitem is the main item
                    for (int i = 1; i < subItems.Length; i++)
                    {
                        newItem.SubItems.Add(subItems[i]);
                    }

                    int insertIndex = -1;
                    if (listViewNotes.SelectedItems.Count > 0)
                    {
                        insertIndex = listViewNotes.SelectedIndices[0];
                    }

                    var pasteCommand = new PasteCommand(listViewNotes, newItem, insertIndex);
                    commandManager.ExecuteCommand(pasteCommand);
                    isModified = true;
                    UpdateFormTitle();
                    Debug.WriteLine("Paste is executed.");
                }
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            commandManager.Undo();
            Debug.WriteLine("Undo is executed.");
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            commandManager.Redo();
            Debug.WriteLine("Redo is executed.");
        }

        private void button_synchronized_play_help_Click(object sender, EventArgs e)
        {
            stop_playing();
            MessageBox.Show("This feature allows you to synchronize the playback of multiple instances of NeoBleeper. \n" +
                "It can also run on multiple computers and still start playing at the same time, as long as all of their clocks are set correctly. Therefore, it is recommended to synchronize your clock before using this feature.\n" +
                "You can synchronize the clocks of multiple computers using the Set time zone automatically, Set time automatically, and Sync now buttons, which are available in Settings > Time & Language > Date & Time. \n\n" +
                "When you enable this feature, a new window will open, showing the synchronized playback controls. \n" +
                "You can then use this window to control the playback of all instances of NeoBleeper on your computer.", "Synchronized Play", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_play_beat_sound_help_Click(object sender, EventArgs e)
        {
            stop_playing();
            MessageBox.Show("This feature allows you to play beat like sounds from system speaker/sound device \n\n" +
                "You can choose the sound to play by clicking the 'Change Beat Sound' button.", "Play a Beat Sound", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_bleeper_portamento_help_Click(object sender, EventArgs e)
        {
            stop_playing();
            MessageBox.Show("Only for the case where music is played in real time on the keyboard. \n\n" +
                "This feature makes the system speaker/sound device increase or decrease in pitch to the note that you clicked.", "Bleeper Portamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_use_keyboard_as_piano_help_Click(object sender, EventArgs e)
        {
            stop_playing();
            MessageBox.Show("This feature allows you to use your computer keyboard as a piano keyboard. \n" +
                "When enabled, you can play notes by pressing the corresponding keys on your keyboard without any MIDI devices. \n\n" +
                "You can see the key mappings in the buttons on the right side of the window.", "Use Keyboard As Piano", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_do_not_update_help_Click(object sender, EventArgs e)
        {
            stop_playing();
            MessageBox.Show("This feature disables the automatic updating of the measure and beat indicators when selecting notes. However, it will continue to update during editing. \n" +
                "When enabled, the indicators will not update when you select notes, allowing you to make changes without affecting the playback position. \n\n" +
                "If you are experiencing problems with fluidity or skipping while playing the music in the list, it is recommended that you disable this option.", "Do Not Update Beat Indicators", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listViewNotes.SelectedItems[0];
                StringBuilder clipboardText = new StringBuilder();

                foreach (ListViewItem.ListViewSubItem subItem in selectedItem.SubItems)
                {
                    clipboardText.Append(subItem.Text + "\t"); // Combine the text with a tab character
                }

                // Remove the last tab character
                if (clipboardText.Length > 0)
                {
                    clipboardText.Length--;
                }

                // Copy to clipboard
                Clipboard.SetText(clipboardText.ToString());
                erase_line();
                Debug.WriteLine("Cut is executed.");
            }
        }
        private async void UpdateRecentFilesMenu()
        {
            try
            {
                openRecentToolStripMenuItem.DropDownItems.Clear();
                var recentFiles = Settings1.Default.RecentFiles;
                if (recentFiles != null && recentFiles.Count > 0)
                {
                    foreach (string filePath in recentFiles)
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(filePath);
                        item.Click += (sender, e) => OpenRecentFile(filePath);
                        item.Enabled = File.Exists(filePath); // Check if the file still exists
                        openRecentToolStripMenuItem.DropDownItems.Add(item);
                        openRecentToolStripMenuItem.Enabled = true;
                    }
                }
                else
                {
                    openRecentToolStripMenuItem.Enabled = false;
                }
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
        }
        private async void OpenRecentFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileParser(filePath);

                    // Create initialMemento with current values after file is opened.
                    initialMemento = originator.CreateSavedStateMemento(
                        Variables.bpm,
                        Variables.alternating_note_length);

                    commandManager.ClearHistory(); // Reset the history
                }
                else
                {
                    MessageBox.Show("The file is not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Settings1.Default.RecentFiles.Remove(filePath);
                    Settings1.Default.Save();
                    UpdateRecentFilesMenu();
                }
            }
            catch (InvalidAsynchronousStateException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening recent file: {ex.Message}");
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void main_window_Load(object sender, EventArgs e)
        {
            InitializeMetronome();
            UpdateRecentFilesMenu();
        }

        private void checkBox_add_note_to_list_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of add note to list is changed to: {checkBox_add_note_to_list.Checked}");
        }

        private void add_as_note1_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note1.Checked == true)
            {
                Debug.WriteLine("Add as note 1 is checked");
            }
        }

        private void add_as_note2_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note2.Checked == true)
            {
                Debug.WriteLine("Add as note 2 is checked");
            }
        }

        private void add_as_note3_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note3.Checked == true)
            {
                Debug.WriteLine("Add as note 3 is checked");
            }
        }

        private void add_as_note4_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note4.Checked == true)
            {
                Debug.WriteLine("Add as note 4 is checked");
            }
        }

        private void checkBox_replace_length_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of replace length is changed to: {checkBox_replace_length.Checked}");
        }

        private void radioButtonPlay_alternating_notes1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPlay_alternating_notes1.Checked == true)
            {
                Debug.WriteLine("Play alternating notes in order is checked");
            }
        }

        private void radioButtonPlay_alternating_notes2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPlay_alternating_notes2.Checked == true)
            {
                Debug.WriteLine("Play alternating notes in odd-even order is checked");
            }
        }

        private void checkBox_loop_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of loop is changed to: {checkBox_loop.Checked}");
        }

        private void checkBox_do_not_update_CheckedChanged(object sender, EventArgs e)
        {
            var play_Beat_Window = checkBox_play_beat_sound.Tag as play_beat_window;
            if (play_Beat_Window != null)
            {
                if (checkBox_do_not_update.Checked && checkBox_play_beat_sound.Checked)
                {
                    play_Beat_Window.label_uncheck_do_not_update.Visible = true;
                }
                else
                {
                    play_Beat_Window.label_uncheck_do_not_update.Visible = false;
                }
            }
            Debug.WriteLine($"Checked state of do not update is changed to: {checkBox_do_not_update.Checked}");
        }

        private void checkBox_dotted_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of dotted is changed to: {checkBox_dotted.Checked}");
        }

        private void checkBox_triplet_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of triplet is changed to: {checkBox_triplet.Checked}");
        }

        private void checkBox_staccato_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of staccato is changed to: {checkBox_staccato.Checked}");
        }

        private void checkBox_spiccato_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of spiccato is changed to: {checkBox_spiccato.Checked}");
        }

        private void checkBox_fermata_Click(object sender, EventArgs e)
        {
            Debug.WriteLine($"Checked state of fermata is changed to: {checkBox_fermata.Checked}");
        }

        private void createMusicWithAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateMusicWithAI createMusicWithAI = new CreateMusicWithAI();
            createMusicWithAI.ShowDialog();
            try
            {
                if (createMusicWithAI.output != string.Empty)
                {
                    if (is_music_playing == true)
                    {
                        stop_playing();
                    }
                    if (checkBox_synchronized_play.Checked == true)
                    {
                        checkBox_synchronized_play.Checked = false;
                    }
                    createNewFile();
                    createMusicWithAIResponse(createMusicWithAI.output);
                    saveAsToolStripMenuItem.Enabled = false;
                    initialMemento = originator.CreateMemento(); // Save the initial state
                    commandManager.ClearHistory(); // Reset the history
                }
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }
        // This method is used to update the form title with the current file path and modification status.
        private void UpdateFormTitle()
        {
            string title = System.AppDomain.CurrentDomain.FriendlyName;

            // Add the current file path if it exists
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                title += " - " + currentFilePath;
            }
            else if (this.Text.Contains("AI Generated Music"))
            {
                title += " - AI Generated Music";
            }

            // Add an asterisk if the file is modified
            if (isModified && !string.IsNullOrEmpty(currentFilePath))
            {
                title += "*";
            }

            this.Text = title;
        }
        public void RestoreVariableValues(int bpmValue, int alternatingNoteLength)
        {
            try
            {
                // Prevent triggering ValueChanged event by setting tag
                numericUpDown_bpm.Tag = "SkipValueChanged";
                numericUpDown_alternating_notes.Tag = "SkipValueChanged";

                // Update values
                numericUpDown_bpm.Value = bpmValue;
                Variables.bpm = bpmValue;

                numericUpDown_alternating_notes.Value = alternatingNoteLength;
                Variables.alternating_note_length = alternatingNoteLength;

                Debug.WriteLine($"Values restored: BPM={bpmValue}, Alt Notes={alternatingNoteLength}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error restoring values: {ex.Message}");
            }
            finally
            {
                // Clear tag
                numericUpDown_bpm.Tag = null;
                numericUpDown_alternating_notes.Tag = null;
            }
        }
        private double GetFrequencyFromNoteName(string noteName)
        {
            if (string.IsNullOrEmpty(noteName))
                return 0;

            // Disassemble note name into note and octave
            string note = noteName.Substring(0, noteName.Length - 1); // "C", "D#", vb.
            int octave = int.Parse(noteName.Substring(noteName.Length - 1)); // Octave number
            // Basic frequency for the note in the 4th octave
            double baseFrequency = note switch
            {
                "C" => base_note_frequency.base_note_frequency_in_4th_octave.C,
                "C#" => base_note_frequency.base_note_frequency_in_4th_octave.CS,
                "D" => base_note_frequency.base_note_frequency_in_4th_octave.D,
                "D#" => base_note_frequency.base_note_frequency_in_4th_octave.DS,
                "E" => base_note_frequency.base_note_frequency_in_4th_octave.E,
                "F" => base_note_frequency.base_note_frequency_in_4th_octave.F,
                "F#" => base_note_frequency.base_note_frequency_in_4th_octave.FS,
                "G" => base_note_frequency.base_note_frequency_in_4th_octave.G,
                "G#" => base_note_frequency.base_note_frequency_in_4th_octave.GS,
                "A" => base_note_frequency.base_note_frequency_in_4th_octave.A,
                "A#" => base_note_frequency.base_note_frequency_in_4th_octave.AS,
                "B" => base_note_frequency.base_note_frequency_in_4th_octave.B,
                _ => 0 // Invalid note
            };

            if (baseFrequency == 0)
                return 0;

            // Oktav farkýný hesaplama
            int octaveDifference = octave - 4; // 4th octave is the reference octave
            return baseFrequency * Math.Pow(2, octaveDifference);
        }
        private void openPlayBeatSoundWindow()
        {
            if (play_Beat_Window == null || play_Beat_Window.IsDisposed)
            {
                play_Beat_Window = new play_beat_window(this);
            }
            play_Beat_Window.Show();
            checkBox_play_beat_sound.Tag = play_Beat_Window;
        }
        private void closePlayBeatSoundWindow()
        {
            play_beat_window play_Beat_Window = checkBox_play_beat_sound.Tag as play_beat_window;
            play_Beat_Window.Close();
        }
        private void checkBox_play_beat_sound_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_play_beat_sound.Checked == true)
            {
                if (checkBox_do_not_update.Checked == true)
                {
                    checkBox_do_not_update.Checked = false;
                }
                openPlayBeatSoundWindow();
                Debug.WriteLine("Play a beat sound window is opened."); 
            }
            else if (checkBox_play_beat_sound.Checked == false)
            {
                closePlayBeatSoundWindow();
                Debug.WriteLine("Play a beat sound window is closed.");
            }
        }
    }
}