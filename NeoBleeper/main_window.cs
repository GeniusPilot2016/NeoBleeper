using GenerativeAI.Types;
using NAudio.Midi;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing.Text;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace NeoBleeper
{
    public partial class main_window : Form
    {
        private play_beat_window play_Beat_Window;
        private PortamentoWindow portamentoWindow;
        private bool isModified = false;
        private CommandManager commandManager;
        private Originator originator;
        private Memento initialMemento;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        PrivateFontCollection fonts = new PrivateFontCollection();
        public event EventHandler MusicStopped;
        private bool KeyPressed = false;
        int[] keyCharNum;
        public static class Variables
        {
            public static int octave;
            public static int bpm;
            public static int alternating_note_length;
            public static double note_silence_ratio;
        }
        private bool isClosing = false;
        string currentFilePath;
        public Boolean is_music_playing = false;
        Boolean is_file_valid = false;
        public main_window()
        {
            CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            InitializeButtonShortcuts();
            set_default_font();
            originator = new Originator(listViewNotes);
            commandManager = new CommandManager(originator);
            commandManager.StateChanged += CommandManager_StateChanged;
            listViewNotes.DoubleBuffering(true);
            UpdateUndoRedoButtons();
            listViewNotes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (listViewNotes.Columns.Count > 0)
            {
                listViewNotes.Columns[listViewNotes.Columns.Count - 1].Width = 45;
            }
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
            Program.MIDIDevices.MidiStatusChanged += MidiDevices_StatusChanged;

            // Initialize MIDI input if it's enabled
            if (Program.MIDIDevices.useMIDIinput)
            {
                InitializeMidiInput();
            }
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
                UIFonts uiFonts = UIFonts.Instance;
                foreach (Control ctrl in Controls)
                {
                    if (ctrl is Panel panel)
                    {
                        foreach (Control panelCtrl in panel.Controls)
                        {
                            if (panelCtrl is Panel childPanel)
                            {
                                foreach (Control childControl in childPanel.Controls)
                                {
                                    childControl.Font = uiFonts.SetUIFont(childControl.Font.Size, childControl.Font.Style);
                                }
                            }
                            else
                            {
                                panelCtrl.Font = uiFonts.SetUIFont(panelCtrl.Font.Size, panelCtrl.Font.Style);
                            }
                        }
                    }
                    else if (ctrl is GroupBox groupBox)
                    {
                        ctrl.Font = uiFonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
                        foreach (Control groupBoxCtrl in groupBox.Controls)
                        {
                            if (groupBoxCtrl is GroupBox childGroupBox)
                            {
                                childGroupBox.Font = uiFonts.SetUIFont(childGroupBox.Font.Size, childGroupBox.Font.Style);
                                foreach (Control childControl in childGroupBox.Controls)
                                {
                                    childControl.Font = uiFonts.SetUIFont(childControl.Font.Size, childControl.Font.Style);
                                }
                            }
                            else
                            {
                                groupBoxCtrl.Font = uiFonts.SetUIFont(groupBoxCtrl.Font.Size, groupBoxCtrl.Font.Style);
                            }
                        }
                    }
                    else if (ctrl is MenuStrip menuStrip)
                    {
                        ctrl.Font = uiFonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
                        foreach (ToolStripItem item in menuStrip.Items) // Corrected from MenuStripItem to ToolStripItem  
                        {
                            item.Font = uiFonts.SetUIFont(item.Font.Size, item.Font.Style);
                            if (item is ToolStripMenuItem menuItem)
                            {
                                foreach (ToolStripItem subItem in menuItem.DropDownItems)
                                {
                                    subItem.Font = uiFonts.SetUIFont(subItem.Font.Size, subItem.Font.Style);
                                }
                            }
                        }
                    }
                    else
                    {
                        ctrl.Font = uiFonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
                    }
                }
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
            OpenSaveAsDialog();
        }

        private void aboutNeoBleeperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            about_neobleeper about = new about_neobleeper();
            about.ShowDialog();
            Debug.WriteLine("About window is opened");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            if (checkBox_synchronized_play.Checked == true)
            {
                checkBox_synchronized_play.Checked = false;
            }
            if (checkBox_play_beat_sound.Checked == true)
            {
                checkBox_play_beat_sound.Checked = false;
            }
            settings_window settings = new settings_window(this); // Pass reference to main_window
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
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
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

                                if (projectFile.LineList?.Lines != null && projectFile.LineList.Lines.Length > 0)
                                {
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
                                // Leave empty if no lines are found
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
        public static NBPML_File.NeoBleeperProjectFile DeserializeXML(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (NBPML_File.NeoBleeperProjectFile)serializer.Deserialize(reader);
            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
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
                OpenSaveAsDialog(); // Open Save As dialog if no file path is set or if the file is not a NBPML file
            }
        }
        private void OpenSaveAsDialog()
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
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
                if (Variables.octave == 8)
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
        private void stopPlayingAllSounds()
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            KeyPressed = false;
            NotePlayer.StopAllNotes(); // Stop all notes
            RemoveUnpressedKeys();
            singleNote = 0; // Reset the single note variable
            UnmarkAllButtons(); // Unmark all buttons
            isAlternatingPlayingRegularKeyboard = false; // Reset the alternating playing state
            Debug.WriteLine("All sounds stopped");
        }
        private void createNewFile()
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
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
        private (int noteSoundDuration, int silenceDuration) CalculateNoteDurations(double baseLength)
        {
            // Compute raw double values
            double noteSound_double = note_length_calculator(baseLength);
            double totalRhythm_double = line_length_calculator(baseLength);

            // Snap to nearest value with relative epsilon
            noteSound_double = FixRoundingErrors(noteSound_double);
            totalRhythm_double = FixRoundingErrors(totalRhythm_double);

            int noteSound_int = (int)Math.Floor(noteSound_double);
            int totalRhythm_int = (int)Math.Floor(totalRhythm_double);
            int silence_int = Math.Max(0, totalRhythm_int - noteSound_int);

            return (noteSound_int, silence_int);
        }

        public static double FixRoundingErrors(double value)
        {
            // Epsilon for checking if difference is negligible
            const double EPSILON_NEGLIGIBLE = 0.0001;
                                                      // Epsilon to add if not negligible
            const double EPSILON_ADD = 0.00001;

            double flooredValue = Math.Floor(value);

            // If the value is very close to its floored integer (e.g., 5.00001 -> 5.0)
            if (Math.Abs(value - flooredValue) < EPSILON_NEGLIGIBLE)
            {
                return flooredValue; // Round down to the floored integer
            }
            // If the value is very close to its ceiled integer (e.g., 5.99991 -> 6.0)
            double truncatedValue = Math.Truncate(value); // Truncate the value to its integer part

            // If the truncated value is greater than a small threshold, add a small epsilon
            if (truncatedValue > 0.0001) 
            {
                return truncatedValue + EPSILON_ADD;
            }

            return value; // If no conditions matched, return the original value
        }

        private void HandleMidiOutput(int noteSoundDuration)
        {
            if (Program.MIDIDevices.useMIDIoutput && listViewNotes.SelectedIndices.Count > 0)
            {
                Task.Run(() =>
                {
                    play_note_in_line_from_MIDIOutput(
                    listViewNotes.SelectedIndices[0],
                    checkBox_play_note1_played.Checked,
                    checkBox_play_note2_played.Checked,
                    checkBox_play_note3_played.Checked,
                    checkBox_play_note4_played.Checked,
                    noteSoundDuration
                );
                }); 
            }
        }

        private void HandleStandardNotePlayback(int noteSoundDuration, bool nonStopping = false)
        {
            if (listViewNotes.SelectedIndices.Count > 0)
            {
                play_note_in_line(
                    checkBox_play_note1_played.Checked,
                    checkBox_play_note2_played.Checked,
                    checkBox_play_note3_played.Checked,
                    checkBox_play_note4_played.Checked,
                    noteSoundDuration,
                    nonStopping
                );
            }
        }
        private void play_music(int startIndex)
        {
            bool nonStopping = false;
            EnableDisableCommonControls(false);
            nonStopping = trackBar_note_silence_ratio.Value == 100;
            int baseLength = 0;
            if (Variables.bpm > 0)
            {
                baseLength = Math.Max(1, (int)Math.Floor(60000.0 / (double)Variables.bpm));
            }
            while (listViewNotes.SelectedItems.Count > 0 && is_music_playing)
            {
                var (noteSound_int, silence_int) = CalculateNoteDurations(baseLength);

                HandleMidiOutput(noteSound_int);
                HandleStandardNotePlayback(noteSound_int, nonStopping);

                if (!nonStopping && silence_int > 0)
                {
                    UpdateLabelVisible(false);
                    NonBlockingSleep.Sleep(silence_int);
                }
                UpdateListViewSelection(startIndex);
            }

            if (nonStopping)
            {
                stopAllNotesAfterPlaying();
            }

            EnableDisableCommonControls(true);
        }

        // Sync ListView update method

        private void UpdateListViewSelection(int startIndex)
        {
            if (listViewNotes.Items.Count == 0) return;

            if (listViewNotes.SelectedItems.Count > 0)
            {
                int currentIndex = listViewNotes.SelectedIndices[0];
                int nextIndex = currentIndex + 1;

                if (nextIndex < listViewNotes.Items.Count)
                {
                    listViewNotes.Items[nextIndex].Selected = true;
                    EnsureSpecificIndexVisible(nextIndex);
                }
                else if (checkBox_loop.Checked)
                {
                    listViewNotes.Items[startIndex].Selected = true;
                    EnsureSpecificIndexVisible(startIndex);
                }
                else
                {
                    stop_playing();
                }
            }
        }
        private void EnsureSpecificIndexVisible(int index)
        {
            Task.Run(() =>
            {
                try
                {
                    if (listViewNotes.InvokeRequired)
                    {
                        SuspendLayout();
                        listViewNotes.Invoke(new Action(() => listViewNotes.EnsureVisible(index)));
                        ResumeLayout(true);
                    }
                    else
                    {
                        SuspendLayout();
                        listViewNotes.EnsureVisible(index);
                        ResumeLayout(true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error ensuring index visible: " + ex.Message);
                }
            });
        }
        public void play_all()
        {
            if (listViewNotes.Items.Count > 0)
            {
                is_music_playing = true;
                listViewNotes.Items[0].Selected = true;
                EnsureSpecificIndexVisible(0);
                Debug.WriteLine("Music is playing");
                play_music(0);
            }
        }
        public void play_from_selected_line()
        {
            if (listViewNotes.Items.Count > 0)
            {
                is_music_playing = true;
                if (listViewNotes.SelectedItems.Count < 1)
                {
                    listViewNotes.Items[0].Selected = true;
                    EnsureSpecificIndexVisible(0);
                    Debug.WriteLine("Music is playing");
                    play_music(0);
                }
                else
                {
                    int index = listViewNotes.SelectedItems[0].Index;
                    EnsureSpecificIndexVisible(index);
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
        private void EnableDisableCommonControls(bool enable)
        {
            keyboard_panel.Enabled = enable;
            checkBox_do_not_update.Enabled = enable;
            numericUpDown_bpm.Enabled = enable;
            numericUpDown_alternating_notes.Enabled = enable;
            button_play_all.Enabled = enable;
            playAllToolStripMenuItem.Enabled = enable;
            button_play_from_selected_line.Enabled = enable;
            playFromSelectedLineToolStripMenuItem.Enabled = enable;
            button_stop_playing.Enabled = !enable;
            stopPlayingToolStripMenuItem.Enabled = !enable;
            switch (enable)
            {
                case true:
                    Debug.WriteLine("Controls enabled");
                    break;
                case false:
                    Debug.WriteLine("Controls disabled");
                    break;
            }
        }
        public void stop_playing()
        {
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

        private async void play_metronome_sound_from_midi_output(int frequency, int length)
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
                PlayMetronomeBeat(beatCount == 0);

                // Then update the UI (which is less time-critical)
                ShowMetronomeBeatLabel();

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

        private void PlayMetronomeBeat(bool isAccent)
        {
            // Option 1: Use pre-loaded SoundPlayer if implemented
            // (isAccent ? accentBeatSound : normalBeatSound).Play();

            // Option 2: Use your NotePlayer but optimize for immediate playback
            int frequency = isAccent ? 1000 : 500;

            // Important: Play sound on high-priority thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                play_metronome_sound_from_midi_output(frequency, 20);
                NotePlayer.play_note(frequency, 20);
            });
        }

        private void ShowMetronomeBeatLabel()
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
                SuspendLayout();
                if (label_beep.InvokeRequired)
                {
                    label_beep.Invoke(new Action(() => label_beep.Visible = visible));
                }
                else
                {
                    label_beep.Visible = visible;
                }
                ResumeLayout(performLayout: true);
                return;
            });
        }


        private void StartMetronome()
        {
            beatCount = 0;
            double interval = Math.Max(1, Math.Truncate(FixRoundingErrors(60000.0 / (double)Variables.bpm)));
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
                int baseLength = 0;
                Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                if (Variables.bpm != 0)
                {
                    baseLength = Math.Max(1, (int)Math.Truncate(FixRoundingErrors(60000.0 / (double)Variables.bpm)));
                }
                if (listViewNotes.SelectedItems.Count > 0)
                {
                    updateIndicators(listViewNotes.SelectedIndices[0]);
                }
                var (noteSound_int, silence_int) = CalculateNoteDurations(baseLength);
                EnableDisableCommonControls(false);
                HandleMidiOutput(noteSound_int);
                bool nonStopping;
                if (trackBar_note_silence_ratio.Value == 100)
                {
                    nonStopping = true;
                }
                else
                {
                    nonStopping = false;
                }
                HandleStandardNotePlayback(noteSound_int, nonStopping);
                if (nonStopping == true)
                {
                    stopAllNotesAfterPlaying();
                }
                Debug.WriteLine($"Selected line: {listViewNotes.FocusedItem.Index} Length: " +
                     $"{listViewNotes.FocusedItem.SubItems[0].Text.ToString()} Note 1: {GetOrDefault(listViewNotes.FocusedItem.SubItems[1].Text.ToString())}" +
                     $" Note 2: {GetOrDefault(listViewNotes.FocusedItem.SubItems[2].Text.ToString())} Note 3: {GetOrDefault(listViewNotes.FocusedItem.SubItems[3].Text.ToString())}" +
                     $" Note 4: {GetOrDefault(listViewNotes.FocusedItem.SubItems[4].Text.ToString())} Modifier: {GetOrDefault(listViewNotes.FocusedItem.SubItems[5].Text.ToString())}" +
                     $" Articulation: {GetOrDefault(listViewNotes.FocusedItem.SubItems[6].Text.ToString())}");
                EnableDisableCommonControls(true);
            }
        }
        private string GetOrDefault(string value)
        {
            return string.IsNullOrEmpty(value) ? "None" : value;
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
        private double note_length_calculator(double baseLength)
        {
            if (listViewNotes.SelectedItems == null || listViewNotes.SelectedItems.Count == 0 ||
                listViewNotes.Items == null || listViewNotes.Items.Count == 0)
            {
                return 0;
            }

            int selectedLine = listViewNotes.SelectedIndices[0];
            string noteType = listViewNotes.Items[selectedLine].SubItems[0].Text;
            string modifier = listViewNotes.Items[selectedLine].SubItems[5].Text;
            string articulation = listViewNotes.Items[selectedLine].SubItems[6].Text;

            // Articulation factor
            double articulationFactor = 1.0;
            if (!string.IsNullOrEmpty(articulation))
            {
                if (articulation.ToLowerInvariant().Contains("sta"))
                    articulationFactor = 0.5;    // Staccato: 0.5x length
                else if (articulation.ToLowerInvariant().Contains("spi"))
                    articulationFactor = 0.25;   // Spiccato: 0.25x length
                else if (articulation.ToLowerInvariant().Contains("fer"))
                    articulationFactor = 2.0;    // Fermata: 2x length
            }

            // Note-silence ratio (from trackBar)
            double silenceRatio = (double)trackBar_note_silence_ratio.Value / 100.0;

            // Calculate the total note length - use precise calculations without truncation
            double result = getNoteLength(baseLength, noteType);
            result = getModifiedNoteLength(result, modifier);
            result = result * articulationFactor; 
            result = result * silenceRatio;       

            // Only round at the very end when converting to integer milliseconds
            return Math.Max(1, result);
        }

        private double line_length_calculator(double quarterNoteMs)
        {
            if (listViewNotes.SelectedItems == null || listViewNotes.SelectedItems.Count == 0 ||
                listViewNotes.Items == null || listViewNotes.Items.Count == 0)
            {
                return 0;
            }

            int selectedLine = listViewNotes.SelectedIndices[0];
            string noteType = listViewNotes.Items[selectedLine].SubItems[0].Text;
            string modifier = listViewNotes.Items[selectedLine].SubItems[5].Text;
            string articulation = listViewNotes.Items[selectedLine].SubItems[6].Text;

            // Articulation factor - only fermata affects line length
            double articulationFactor = 1.0;
            if (!string.IsNullOrEmpty(articulation) && articulation.ToLowerInvariant().Contains("fer"))
            {
                articulationFactor = 2.0; // Fermata: 2x length
            }
            // Staccato and Spiccato do not affect line length

            // Calculate the total line length (without note-silence ratio)
            double result = getNoteLength(quarterNoteMs, noteType);
            result = getModifiedNoteLength(result, modifier);
            result = result * articulationFactor; 

            // Should be at least 1 ms
            return Math.Max(1, result);
        }
        private double getNoteLength(double rawNoteLength, String lengthName)
        {
            double noteFraction = lengthName switch
            {
                "Whole" => 4.0,      // Whole note = 4 quarter notes
                "Half" => 2.0,       // Half note = 2 quarter notes
                "Quarter" => 1.0,    // Quarter note = 1 quarter note
                "1/8" => 0.5,        // 1/8 note = 1/2 quarter note
                "1/16" => 0.25,      // 1/16 note = 1/4 quarter note
                "1/32" => 0.125,     // 1/32 note = 1/8 quarter note
                _ => 1.0,            // Default: Quarter note
            };
            return rawNoteLength * noteFraction; // Remove truncation here
        }
        private double getModifiedNoteLength(double noteLength, String modifier)
        {
            double modifierFactor = 1.0;
            if (!string.IsNullOrEmpty(modifier))
            {
                if (modifier.ToLowerInvariant().Contains("dot"))
                    modifierFactor = 1.5; // Dotted: 1.5x length
                else if (modifier.ToLowerInvariant().Contains("tri"))
                    modifierFactor = 1.0 / 3.0; // Triplet: 1/3x length
            }
            return noteLength * modifierFactor; // Remove truncation
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
                    note1_frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);

                if (!string.IsNullOrEmpty(note2))
                    note2_frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);

                if (!string.IsNullOrEmpty(note3))
                    note3_frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);

                if (!string.IsNullOrEmpty(note4))
                    note4_frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
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
                NonBlockingSleep.Sleep(Math.Max(1, length));
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
                return;
            }
            else if ((note1 == string.Empty || note1 == null) && (note2 != string.Empty || note2 != null) && (note3 == string.Empty || note3 == null) && (note4 == string.Empty || note4 == null))
            {
                UpdateLabelVisible(true);
                NotePlayer.play_note(Convert.ToInt32(note2_frequency), length, nonStopping);
                if (nonStopping == false)
                {
                    UpdateLabelVisible(false);
                }
                return;
            }

            else if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 != string.Empty || note3 != null) && (note4 == string.Empty || note4 == null))
            {
                UpdateLabelVisible(true);
                NotePlayer.play_note(Convert.ToInt32(note3_frequency), length, nonStopping);
                if (nonStopping == false)
                {
                    UpdateLabelVisible(false);
                }
                return;
            }
            else if ((note1 == string.Empty || note1 == null) && (note2 == string.Empty || note2 == null) && (note3 == string.Empty || note3 == null) && (note4 != string.Empty || note4 != null))
            {
                UpdateLabelVisible(true);
                NotePlayer.play_note(Convert.ToInt32(note4_frequency), length, nonStopping);
                if (nonStopping == false)
                {
                    UpdateLabelVisible(false);
                }
                return;
            }
            else
            {
                if (play_note1 == true || play_note2 == true || play_note3 == true || play_note4 == true)
                {
                    UpdateLabelVisible(true);
                }
                Stopwatch stopwatch = new Stopwatch();
                double totalDuration = length; // Total playing duration of loop

                if (radioButtonPlay_alternating_notes1.Checked == true)
                {
                    string[] note_series = { note1, note2, note3, note4 };
                    stopwatch.Start();
                    do
                    {
                        foreach (string note in note_series)
                        {
                            if (!string.IsNullOrEmpty(note))
                            {
                                // Check elapsed time
                                if (stopwatch.ElapsedMilliseconds >= totalDuration)
                                {
                                    stopwatch.Stop();
                                    break;
                                }
                                double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);
                                NotePlayer.play_note(Convert.ToInt32(frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                            }
                        }
                    }
                    while (stopwatch.ElapsedMilliseconds < totalDuration);
                    stopwatch.Stop(); // Stop the stopwatch after the loop ends
                }
                else if (radioButtonPlay_alternating_notes2.Checked == true)
                {
                    string[] note_series = { note1, note2, note3, note4 };
                    stopwatch.Start(); 
                    do
                    {
                        // Odd numbered columns (Note1 and Note3)
                        for (int i = 0; i < 4; i += 2)
                        {
                            if (!string.IsNullOrEmpty(note_series[i]))
                            {
                                // Check elapsed time
                                if (stopwatch.ElapsedMilliseconds >= totalDuration)
                                {
                                    stopwatch.Stop();
                                    break;
                                }
                                double frequency = (i == 0) ? note1_frequency : note3_frequency;
                                NotePlayer.play_note(Convert.ToInt32(frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));
                            }
                        }

                        // Even numbered columns (Note2 and Note4)
                        for (int i = 1; i < 4; i += 2)
                        {
                            if (!string.IsNullOrEmpty(note_series[i]))
                            {
                                double frequency = (i == 1) ? note2_frequency : note4_frequency;
                                NotePlayer.play_note(Convert.ToInt32(frequency), Convert.ToInt32(numericUpDown_alternating_notes.Value));

                                // Check elapsed time
                                if (stopwatch.ElapsedMilliseconds >= totalDuration)
                                {
                                    stopwatch.Stop();
                                    break;
                                }
                            }
                        }
                    }
                    while (stopwatch.ElapsedMilliseconds < totalDuration);
                    stopwatch.Stop(); // Stop the stopwatch after the loop ends
                }

                if (cancellationTokenSource.Token.IsCancellationRequested) return;
                if (play_note1 == true || play_note2 == true || play_note3 == true || play_note4 == true)
                {
                    if (nonStopping == false)
                    {
                        UpdateLabelVisible(false);
                        //NonBlockingSleep.Sleep(10);
                    }
                }
                return;
            }
        }
        public static bool IsWholeNumber(double value)
        {
            // Compare the value with its truncated version
            return value == Math.Truncate(value);
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
                if (checkBox_play_beat_sound.Checked == true && clicked == false && IsWholeNumber(beat_number))
                {
                    switch (Program.BeatTypes.beat_type)
                    {
                        case 0:
                            play_beat_sound();
                            break;
                        case 1:
                            if (beat_number % 2 != 0)
                            {
                                play_beat_sound();
                            }
                            break;
                        case 2:
                            if (beat_number % 2 == 0)
                            {
                                play_beat_sound();
                            }
                            break;
                    }
                }
            }
        }
        private void play_beat_sound()
        {
            // Basic frequencies
            int snareFrequency = Convert.ToInt32(NoteFrequencies.GetFrequencyFromNoteName("D2"));
            int kickFrequency = Convert.ToInt32(NoteFrequencies.GetFrequencyFromNoteName("E2"));
            int hiHatFrequency = Convert.ToInt32(NoteFrequencies.GetFrequencyFromNoteName("F#2"));

            // Calculate length based on BPM
            double calculatedLengthFactor = 0.1; // Factor to adjust the length of the sound
            int miliseconds_per_whole_note = 0;
            if (Variables.bpm != 0)
            {
                miliseconds_per_whole_note = (int)Math.Truncate(240000.0 / Variables.bpm);
            }

            int length = Math.Max(1, (int)Math.Truncate((miliseconds_per_whole_note / 15.0) * calculatedLengthFactor));

            // Create a percussion sound
            for (int i = 0; i < 2; i++) // 2 beats
            {
                if (i % 2 == 0) // Kick 
                {
                    NotePlayer.play_note(kickFrequency, length, true);
                }
                else // Snare
                {
                    NotePlayer.play_note(snareFrequency, length, true);
                }
            }
            // Add hi-hat sound
            NotePlayer.play_note(hiHatFrequency, length / 2, true);

            // Percussion sound for MIDI output
            if (Program.MIDIDevices.useMIDIoutput)
            {
                Task.Run(async () =>
                {
                    int midiSnare = MIDIIOUtils.FrequencyToMidiNote(snareFrequency); // Snare Drum
                    int midiKick = MIDIIOUtils.FrequencyToMidiNote(kickFrequency);  // Bass Drum
                    int midiHiHat = MIDIIOUtils.FrequencyToMidiNote(hiHatFrequency); // Closed Hi-Hat

                    await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, midiKick, length);
                    await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, midiSnare, length);
                    await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, midiHiHat, length / 2);
                });
            }
            NotePlayer.StopAllNotes();
        }
        public static string ConvertDecimalBeatToTraditional(double decimalBeat)
        {
            double fractionOfWhole = decimalBeat / 4;
            double thirtyseconds = Math.Round(fractionOfWhole * 32);

            if (Math.Abs((decimalBeat / 4) - (thirtyseconds / 32)) > 0.00001)
            {
                if (thirtyseconds > 16)
                {
                    int wholeNumber = (int)Math.Truncate(thirtyseconds / 16);
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
                int wholeNumber = (int)Math.Truncate(thirtyseconds / 32);
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
            decimal length = 0m;
            decimal modifier = 1m;
            decimal articulation = 1m;

            // Use the first subitem to determine the length
            switch (listViewItem.SubItems[0].Text)
            {
                case "Whole":
                    length = 4m;
                    break;
                case "Half":
                    length = 2m;
                    break;
                case "Quarter":
                    length = 1m;
                    break;
                case "1/8":
                    length = 0.5m;
                    break;
                case "1/16":
                    length = 0.25m;
                    break;
                case "1/32":
                    length = 0.125m;
                    break;
                default:
                    length = 1m; // Default: Quarter
                    break;
            }

            // Apply modifiers
            switch (listViewItem.SubItems[5].Text)
            {
                case "Dot":
                    modifier = 1.5m;
                    break;
                case "Tri":
                    modifier = 1m / 3m; // Precise triplet calculation
                    break;
                default:
                    modifier = 1m;
                    break;
            }

            // Use the articulation subitem to determine the articulation effect
            switch (listViewItem.SubItems[6].Text)
            {
                case "Sta":
                    articulation = 0.5m;
                    break;
                case "Spi":
                    articulation = 0.25m;
                    break;
                case "Fer":
                    articulation = 2m;
                    break;
                default:
                    articulation = 1m;
                    break;
            }

            // Convert to double and round to 8 decimal places
            decimal result = length * modifier * articulation;
            return Convert.ToDouble(Math.Round(result, 8));
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
            stop_playing();
            checkBox_metronome.Checked = false;
            cancellationTokenSource.Cancel();
            isClosing = true;
            stop_system_speaker_beep();
        }

        private void main_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            stop_playing();
            checkBox_metronome.Checked = false;
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
        private void show_keyboard_keys_shortcut()
        {
            Task.Run(() =>
            {
                foreach (var entry in buttonShortcuts)
                {
                    entry.Key.Text = entry.Value; // Set the shortcut text
                }
            });
        }
        private void hide_keyboard_keys_shortcut()
        {
            Task.Run(() =>
            {
                foreach (var entry in buttonShortcuts)
                {
                    entry.Key.Text = string.Empty; // Clear the shortcut text
                }
            });
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
                KeyPressed = false; // Reset the KeyPressed flag when the checkbox is unchecked
            }
            enableDisableTabStop(this, !checkBox_use_keyboard_as_piano.Checked);
        }
        private void enableDisableTabStop(Control parent, bool enabled)
        {
            // Corrected the type check to ensure it checks for Control, not TabStop
            if (parent is Control control)
            {
                control.TabStop = enabled;
            }

            foreach (Control ctrl in parent.Controls)
            {
                enableDisableTabStop(ctrl, enabled);
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
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
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
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show("This feature allows you to synchronize the playback of multiple instances of NeoBleeper. \n" +
                "It can also run on multiple computers and still start playing at the same time, as long as all of their clocks are set correctly. Therefore, it is recommended to synchronize your clock before using this feature.\n" +
                "You can synchronize the clocks of multiple computers using the Set time zone automatically, Set time automatically, and Sync now buttons, which are available in Settings > Time & Language > Date & Time. \n\n" +
                "When you enable this feature, a new window will open, showing the synchronized playback controls. \n" +
                "You can then use this window to control the playback of all instances of NeoBleeper on your computer.", "Synchronized Play", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_play_beat_sound_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show("This feature allows you to play beat like sounds from system speaker/sound device \n\n" +
                "You can choose the sound to play by clicking the 'Change Beat Sound' button.", "Play a Beat Sound", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_bleeper_portamento_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show("Only for the case where music is played in real time on the keyboard. \n\n" +
                "This feature makes the system speaker/sound device increase or decrease in pitch to the note that you clicked.", "Bleeper Portamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_use_keyboard_as_piano_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show("This feature allows you to use your computer keyboard as a piano keyboard. \n" +
                "When enabled, you can play notes by pressing the corresponding keys on your keyboard without any MIDI devices. \n\n" +
                "You can see the key mappings in the buttons on the right side of the window.", "Use Keyboard As Piano", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_do_not_update_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
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
        private void UpdateRecentFilesMenu()
        {
            openRecentToolStripMenuItem.DropDownItems.Clear();
            var recentFiles = Settings1.Default.RecentFiles;
            if (recentFiles != null && recentFiles.Count > 0)
            {
                foreach (string filePath in recentFiles)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(filePath);
                    item.Click += (sender, e) => OpenRecentFile(filePath);

                    if (!File.Exists(filePath))
                    {
                        item.Text += " (Not Found)"; // Add "(Not Found)" if the file does not exist
                        item.Enabled = false; // Disable the item if the file is not found
                    }

                    openRecentToolStripMenuItem.DropDownItems.Add(item);
                }
                openRecentToolStripMenuItem.Enabled = true;
            }
            else
            {
                openRecentToolStripMenuItem.Enabled = false;
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
            try
            {
                stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
                createMusicWithAI.ShowDialog();
                if (createMusicWithAI.output != string.Empty)
                {
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

        private void openPlayBeatSoundWindow()
        {
            if (play_Beat_Window == null || play_Beat_Window.IsDisposed)
            {
                play_Beat_Window = new play_beat_window(this);
            }
            play_Beat_Window.Show();
            checkBox_play_beat_sound.Tag = play_Beat_Window;
        }
        private void openBleeperPortamentoWindow()
        {
            if (portamentoWindow == null || portamentoWindow.IsDisposed)
            {
                portamentoWindow = new PortamentoWindow(this);
            }
            portamentoWindow.Show();
            checkBox_bleeper_portamento.Tag = portamentoWindow;
        }
        private void closePlayBeatSoundWindow()
        {
            play_beat_window play_Beat_Window = checkBox_play_beat_sound.Tag as play_beat_window;
            play_Beat_Window.Close();
        }
        private void closePortamentoWindow()
        {
            PortamentoWindow portamentoWindow = checkBox_bleeper_portamento.Tag as PortamentoWindow;
            portamentoWindow.Close();
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
        private String ConvertToNBPMLString()
        {
            try
            {
                // Create NBPML file object
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

                // Serialize to string into XML format and remove namespace
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty); // Namespace'i kaldýr
                    serializer.Serialize(stringWriter, projectFile, namespaces);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error converting to NBPML string: " + ex.Message);
                return string.Empty; // Return empty string in case of error
            }
        }

        private void convertToGCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
                if (checkBox_synchronized_play.Checked == true)
                {
                    checkBox_synchronized_play.Checked = false;
                }
                ConvertToGCode convertToGCode = new ConvertToGCode(ConvertToNBPMLString());
                convertToGCode.ShowDialog();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error converting to GCode: " + ex.Message);
                MessageBox.Show("Error converting to GCode: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox_bleeper_portamento_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_bleeper_portamento.Checked == true)
            {
                openBleeperPortamentoWindow();
                Debug.WriteLine("Bleeper portamento window is opened.");
            }
            else if (checkBox_play_beat_sound.Checked == false)
            {
                closePortamentoWindow();
                Debug.WriteLine("Bleeper portamento window is closed.");
            }
        }
        private readonly Dictionary<Button, string> buttonShortcuts = new Dictionary<Button, string>();

        private void InitializeButtonShortcuts()
        {
            buttonShortcuts.Add(button_c3, "Tab");
            buttonShortcuts.Add(button_c_s3, "`");
            buttonShortcuts.Add(button_d3, "Q");
            buttonShortcuts.Add(button_d_s3, "1");
            buttonShortcuts.Add(button_e3, "W");
            buttonShortcuts.Add(button_f3, "E");
            buttonShortcuts.Add(button_f_s3, "3");
            buttonShortcuts.Add(button_g3, "R");
            buttonShortcuts.Add(button_g_s3, "4");
            buttonShortcuts.Add(button_a3, "T");
            buttonShortcuts.Add(button_a_s3, "5");
            buttonShortcuts.Add(button_b3, "Y");
            buttonShortcuts.Add(button_c4, "U");
            buttonShortcuts.Add(button_c_s4, "7");
            buttonShortcuts.Add(button_d4, "I");
            buttonShortcuts.Add(button_d_s4, "8");
            buttonShortcuts.Add(button_e4, "O");
            buttonShortcuts.Add(button_f4, "P");
            buttonShortcuts.Add(button_f_s4, "0");
            buttonShortcuts.Add(button_g4, "[");
            buttonShortcuts.Add(button_g_s4, "-");
            buttonShortcuts.Add(button_a4, "]");
            buttonShortcuts.Add(button_a_s4, "+");
            buttonShortcuts.Add(button_b4, "|");
            buttonShortcuts.Add(button_c5, "Shift");
            buttonShortcuts.Add(button_c_s5, "A");
            buttonShortcuts.Add(button_d5, "Z");
            buttonShortcuts.Add(button_d_s5, "S");
            buttonShortcuts.Add(button_e5, "X");
            buttonShortcuts.Add(button_f5, "C");
            buttonShortcuts.Add(button_f_s5, "F");
            buttonShortcuts.Add(button_g5, "V");
            buttonShortcuts.Add(button_g_s5, "G");
            buttonShortcuts.Add(button_a5, "B");
            buttonShortcuts.Add(button_a_s5, "H");
            buttonShortcuts.Add(button_b5, "N");
        }
        private HashSet<int> pressedKeys = new HashSet<int>();
        private void main_window_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the key is one we want to use for piano playing
            if (IsKeyboardPianoKey(e.KeyCode))
            {
                HashSet<int> currentlyPressedKeys = new HashSet<int>();
                currentlyPressedKeys.Add((int)e.KeyCode);
                if(currentlyPressedKeys == pressedKeys)
                {
                    // If the key is already pressed, do nothing
                    e.Handled = true;
                    return;
                }
                KeyPressed = true; // Set KeyPressed to true when a key is pressed
                pressedKeys.Add((int)e.KeyCode);
                keyCharNum = pressedKeys.ToArray();
                MarkupTheKeyWhenKeyIsPressed(e.KeyValue);
                playWithRegularKeyboard();
            }
            // Allow regular keyboard shortcuts to work
            else
            {
                // Let other key presses pass through (like Ctrl+S, Ctrl+Z, etc.)
                e.Handled = false;
            }
        }

        private void main_window_KeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove((int)e.KeyCode);
            keyCharNum = pressedKeys.ToArray();

            // Stop alternating playback and reset flags
            isAlternatingPlayingRegularKeyboard = false;
            UnmarkAllButtons(); // Unmark all buttons when a key is released
            singleNote = 0; // Reset singleNote to ensure no lingering playback
            stopAllNotesAfterPlaying(); // Stop all notes only when no keys remain
            // Stop all notes if no keys are pressed
            if (pressedKeys.Count == 0)
            {
                KeyPressed = false;
            }
            else
            {
                // Continue playing notes for remaining pressed keys
                foreach (int key in keyCharNum)
                {
                    MarkupTheKeyWhenKeyIsPressed(key);
                }
                playWithRegularKeyboard(); // Trigger playback for remaining notes
            }
        }
        private int GetFrequencyFromKeyCode(int keyCode)
        {
            // Key and octave offset mapping
            Dictionary<int, (double baseFreq, int octaveOffset)> keyValuePairs = new()
            {
                { (int)Keys.Tab, (base_note_frequency.base_note_frequency_in_4th_octave.C, -1) }, // C3
                { (int)Keys.Oemtilde, (base_note_frequency.base_note_frequency_in_4th_octave.CS, -1) }, // C#3
                { (int)Keys.Q, (base_note_frequency.base_note_frequency_in_4th_octave.D, -1) }, // D3
                { (int)Keys.D1, (base_note_frequency.base_note_frequency_in_4th_octave.DS, -1) }, // D#3
                { (int)Keys.W, (base_note_frequency.base_note_frequency_in_4th_octave.E, -1) }, // E3
                { (int)Keys.E, (base_note_frequency.base_note_frequency_in_4th_octave.F, -1) }, // F3
                { (int)Keys.D3, (base_note_frequency.base_note_frequency_in_4th_octave.FS, -1) }, // F#3
                { (int)Keys.R, (base_note_frequency.base_note_frequency_in_4th_octave.G, -1) }, // G3
                { (int)Keys.D4, (base_note_frequency.base_note_frequency_in_4th_octave.GS, -1) }, // G#3
                { (int)Keys.T, (base_note_frequency.base_note_frequency_in_4th_octave.A, -1) }, // A3
                { (int)Keys.D5, (base_note_frequency.base_note_frequency_in_4th_octave.AS, -1) }, // A#3
                { (int)Keys.Y, (base_note_frequency.base_note_frequency_in_4th_octave.B, -1) }, // B3
                { (int)Keys.U, (base_note_frequency.base_note_frequency_in_4th_octave.C, 0) }, // C4
                { (int)Keys.D6, (base_note_frequency.base_note_frequency_in_4th_octave.CS, 0) }, // C#4
                { (int)Keys.I, (base_note_frequency.base_note_frequency_in_4th_octave.D, 0) }, // D4
                { (int)Keys.D7, (base_note_frequency.base_note_frequency_in_4th_octave.DS, 0) }, // D#4
                { (int)Keys.O, (base_note_frequency.base_note_frequency_in_4th_octave.E, 0) }, // E4
                { (int)Keys.P, (base_note_frequency.base_note_frequency_in_4th_octave.F, 0) }, // F4
                { (int)Keys.D8, (base_note_frequency.base_note_frequency_in_4th_octave.FS, 0) }, // F#4
                { (int)Keys.OemOpenBrackets, (base_note_frequency.base_note_frequency_in_4th_octave.G, 0) }, // G4
                { (int)Keys.OemMinus, (base_note_frequency.base_note_frequency_in_4th_octave.GS, 0) }, // G#4
                { (int)Keys.OemCloseBrackets, (base_note_frequency.base_note_frequency_in_4th_octave.A, 0) }, // A4
                { (int)Keys.Oemplus, (base_note_frequency.base_note_frequency_in_4th_octave.AS, 0) }, // A#4
                { (int)Keys.OemPipe, (base_note_frequency.base_note_frequency_in_4th_octave.B, 0) }, // B4
                { (int)Keys.ShiftKey, (base_note_frequency.base_note_frequency_in_4th_octave.C, 1) }, // C5
                { (int)Keys.A, (base_note_frequency.base_note_frequency_in_4th_octave.CS, 1) }, // C#5
                { (int)Keys.Z, (base_note_frequency.base_note_frequency_in_4th_octave.D, 1) }, // D5
                { (int)Keys.S, (base_note_frequency.base_note_frequency_in_4th_octave.DS, 1) }, // D#5
                { (int)Keys.X, (base_note_frequency.base_note_frequency_in_4th_octave.E, 1) }, // E5
                { (int)Keys.C, (base_note_frequency.base_note_frequency_in_4th_octave.F, 1) }, // F5
                { (int)Keys.F, (base_note_frequency.base_note_frequency_in_4th_octave.FS, 1) }, // F#5
                { (int)Keys.V, (base_note_frequency.base_note_frequency_in_4th_octave.G, 1) }, // G5
                { (int)Keys.G, (base_note_frequency.base_note_frequency_in_4th_octave.GS, 1) }, // G#5
                { (int)Keys.B, (base_note_frequency.base_note_frequency_in_4th_octave.A, 1) }, // A5
                { (int)Keys.H, (base_note_frequency.base_note_frequency_in_4th_octave.AS, 1) }, // A#5
                { (int)Keys.N, (base_note_frequency.base_note_frequency_in_4th_octave.B, 1) }, // B5
            };

            if (keyValuePairs.TryGetValue(keyCode, out var noteInfo))
            {
                // Calculate the frequency based on the base frequency and octave offset
                int octave = Variables.octave + noteInfo.octaveOffset;
                double frequency = noteInfo.baseFreq * Math.Pow(2, octave - 4);
                return (int)frequency;
            }
            return 0;
        }
        private void RemoveUnpressedKeys()
        {
            if (keyCharNum != null)
            {
                // Remove keys that are no longer pressed
                var currentMidiNotes = keyCharNum.Select(k => MIDIIOUtils.FrequencyToMidiNote(GetFrequencyFromKeyCode(k))).ToHashSet();
                var notesToRemove = activeMidiNotes.Except(currentMidiNotes).ToList();
                foreach (var note in notesToRemove)
                {
                    MIDIIOUtils.StopMidiNote(note);
                    activeMidiNotes.Remove(note);
                }
            }
        }
        private HashSet<int> activeMidiInNotes = new HashSet<int>();
        private int singleNote = 0; // Variable to store the single note being played
        private bool isAlternatingPlayingRegularKeyboard = false;
        private void playWithRegularKeyboard() // Play notes with regular keyboard (the keyboard of the computer, not MIDI keyboard)
        {
            if (!checkBox_use_keyboard_as_piano.Checked)
                return;

            UpdateLabelVisible(true);

            if (Program.MIDIDevices.useMIDIoutput)
            {
                foreach (int key in keyCharNum)
                {
                    int midiNote = MIDIIOUtils.FrequencyToMidiNote(GetFrequencyFromKeyCode(key));
                    // Don't play the same note again if it's already active
                    if (!activeMidiNotes.Contains(midiNote))
                    {
                        activeMidiNotes.Add(midiNote);
                        Task.Run(() =>
                        {
                            MIDIIOUtils.PlayMidiNote(midiNote, 1, true);
                        });
                    }
                }
                RemoveUnpressedKeys();
            }

            if (keyCharNum.Length > 1)
            {
                if (!isAlternatingPlayingRegularKeyboard)
                {
                    isAlternatingPlayingRegularKeyboard = true;
                    while (KeyPressed && isAlternatingPlayingRegularKeyboard)
                    {
                        foreach (int key in keyCharNum)
                        {
                            NotePlayer.play_note(GetFrequencyFromKeyCode(key), Variables.alternating_note_length);
                        }
                    }
                }
            }
            else if (keyCharNum.Length == 1)
            {
                isAlternatingPlayingRegularKeyboard = false;
                int midiNote = MIDIIOUtils.FrequencyToMidiNote(GetFrequencyFromKeyCode(keyCharNum[0]));
                if (singleNote != midiNote)
                {
                    singleNote = midiNote;
                    NotePlayer.play_note(MIDIIOUtils.MidiNoteToFrequency(midiNote), 1, true);
                }
            }
        }
        private void MarkupTheKeyWhenKeyIsPressed(int keyCode)
        {
            if(!checkBox_use_keyboard_as_piano.Checked)
                return;
            Color markdownColor = Settings1.Default.markup_color; // Get the markdown color from settings

            if (buttonShortcuts.TryGetValue(button_c3, out string shortcut))
            {
                if (keyCode == (int)Keys.Tab)
                {
                    button_c3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_c_s3, out shortcut))
            {
                if (keyCode == (int)Keys.Oemtilde)
                {
                    button_c_s3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_d3, out shortcut))
            {
                if (keyCode == (int)Keys.Q)
                {
                    button_d3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_d_s3, out shortcut))
            {
                if (keyCode == (int)Keys.D1)
                {
                    button_d_s3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_e3, out shortcut))
            {
                if (keyCode == (int)Keys.W)
                {
                    button_e3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_f3, out shortcut))
            {
                if (keyCode == (int)Keys.E)
                {
                    button_f3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_f_s3, out shortcut))
            {
                if (keyCode == (int)Keys.D3)
                {
                    button_f_s3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_g3, out shortcut))
            {
                if (keyCode == (int)Keys.R)
                {
                    button_g3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_g_s3, out shortcut))
            {
                if (keyCode == (int)Keys.D4)
                {
                    button_g_s3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_a3, out shortcut))
            {
                if (keyCode == (int)Keys.T)
                {
                    button_a3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_a_s3, out shortcut))
            {
                if (keyCode == (int)Keys.D5)
                {
                    button_a_s3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_b3, out shortcut))
            {
                if (keyCode == (int)Keys.Y)
                {
                    button_b3.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_c4, out shortcut))
            {
                if (keyCode == (int)Keys.U)
                {
                    button_c4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_c_s4, out shortcut))
            {
                if (keyCode == (int)Keys.D6)
                {
                    button_c_s4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_d4, out shortcut))
            {
                if (keyCode == (int)Keys.I)
                {
                    button_d4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_d_s4, out shortcut))
            {
                if (keyCode == (int)Keys.D7)
                {
                    button_d_s4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_e4, out shortcut))
            {
                if (keyCode == (int)Keys.O)
                {
                    button_e4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_f4, out shortcut))
            {
                if (keyCode == (int)Keys.P)
                {
                    button_f4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_f_s4, out shortcut))
            {
                if (keyCode == (int)Keys.D8)
                {
                    button_f_s4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_g4, out shortcut))
            {
                if (keyCode == (int)Keys.OemOpenBrackets)
                {
                    button_g4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_g_s4, out shortcut))
            {
                if (keyCode == (int)Keys.OemMinus)
                {
                    button_g_s4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_a4, out shortcut))
            {
                if (keyCode == (int)Keys.OemCloseBrackets)
                {
                    button_a4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_a_s4, out shortcut))
            {
                if (keyCode == (int)Keys.Oemplus)
                {
                    button_a_s4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_b4, out shortcut))
            {
                if (keyCode == (int)Keys.OemPipe)
                {
                    button_b4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_c5, out shortcut))
            {
                if (keyCode == (int)Keys.ShiftKey)
                {
                    button_c5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_c_s5, out shortcut))
            {
                if (keyCode == (int)Keys.A)
                {
                    button_c_s5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_d5, out shortcut))
            {
                if (keyCode == (int)Keys.Z)
                {
                    button_d5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_d_s5, out shortcut))
            {
                if (keyCode == (int)Keys.S)
                {
                    button_d_s5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_e5, out shortcut))
            {
                if (keyCode == (int)Keys.X)
                {
                    button_e5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_f5, out shortcut))
            {
                if (keyCode == (int)Keys.C)
                {
                    button_f5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_f_s5, out shortcut))
            {
                if (keyCode == (int)Keys.F)
                {
                    button_f_s5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_g5, out shortcut))
            {
                if (keyCode == (int)Keys.V)
                {
                    button_g5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_g_s5, out shortcut))
            {
                if (keyCode == (int)Keys.G)
                {
                    button_g_s5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_a5, out shortcut))
            {
                if (keyCode == (int)Keys.B)
                {
                    button_a5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_a_s5, out shortcut))
            {
                if (keyCode == (int)Keys.H)
                {
                    button_a_s5.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_b5, out shortcut))
            {
                if (keyCode == (int)Keys.N)
                {
                    button_b5.BackColor = markdownColor;
                }
            }
        }
        private void UnmarkAllButtons()
        {
            Color whiteKeyColor = Color.White; // Define the color to reset the button
            Color blackKeyColor = Color.Black; // Define the color to reset the button
            foreach (Control ctrl in keyboard_panel.Controls)
            {
                if (ctrl is Button)
                {
                    if (ctrl.Name.Contains("s") || ctrl.Name.Contains("S")) // Black keys
                    {
                        ctrl.BackColor = blackKeyColor;
                    }
                    else // White keys
                    {
                        ctrl.BackColor = whiteKeyColor;
                    }
                }
            }
        }
        // This method checks if a key is part of the piano keyboard
        private bool IsKeyboardPianoKey(Keys keyCode)
        {
            // Define all the keys that should trigger the piano functionality
            HashSet<Keys> pianoKeys = new HashSet<Keys>
        {
            // White keys
            Keys.Tab, Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y,
            Keys.U, Keys.I, Keys.O, Keys.P, Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.OemPipe,
            Keys.ShiftKey, Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N,
        
            // Black keys
            Keys.Oemtilde, Keys.D1, Keys.D3, Keys.D4, Keys.D5,
            Keys.D6, Keys.D7, Keys.D8, Keys.OemMinus, Keys.Oemplus,
            Keys.A, Keys.S, Keys.F, Keys.G, Keys.H
        };

            return pianoKeys.Contains(keyCode);
        }
        private void InitializeMidiInput()
        {
            if (!Program.MIDIDevices.useMIDIinput || MIDIIOUtils._midiIn == null)
                return;

            // Set up event handler for MIDI input
            MIDIIOUtils._midiIn.MessageReceived += MidiIn_MessageReceived;
            MIDIIOUtils._midiIn.Start(); // Start listening for MIDI input

            Debug.WriteLine("MIDI input initialized and listening");
        }
        // Handle MIDI device status changes
        private void MidiDevices_StatusChanged(object sender, EventArgs e)
        {
            try
            {
                // First, safely dispose the current MIDI input if it exists
                if (MIDIIOUtils._midiIn != null)
                {
                    try
                    {
                        MIDIIOUtils._midiIn.Stop();
                        MIDIIOUtils._midiIn.MessageReceived -= MidiIn_MessageReceived;
                    }
                    catch (NAudio.MmException)
                    {
                        // Device might already be disconnected, ignore this error
                        Debug.WriteLine("MIDI input device already disconnected");
                    }
                }

                // If MIDI input is enabled, try to re-initialize it with a fresh device instance
                if (Program.MIDIDevices.useMIDIinput)
                {
                    // Force reinitialize the MIDI device with the current device index
                    MIDIIOUtils.ChangeInputDevice(Program.MIDIDevices.MIDIInputDevice);

                    // Only proceed if we successfully created a new device instance
                    if (MIDIIOUtils._midiIn != null)
                    {
                        MIDIIOUtils._midiIn.MessageReceived += MidiIn_MessageReceived;
                        try
                        {
                            MIDIIOUtils._midiIn.Start();
                            Debug.WriteLine("MIDI input reinitialized and listening");
                        }
                        catch (NAudio.MmException ex)
                        {
                            Debug.WriteLine($"Failed to start MIDI input: {ex.Message}");
                            // Update UI or show a message to the user that MIDI input is unavailable
                            MessageBox.Show($"Cannot start MIDI input device: {ex.Message}\nTry refreshing the device list.",
                                "MIDI Device Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // Reset the MIDI device reference
                            MIDIIOUtils._midiIn = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error handling MIDI status change: {ex.Message}");
            }
        }

        // Store active MIDI notes for alternating playback
        private List<int> activeMidiNotes = new List<int>();
        private bool isAlternatingPlaying = false;

        private void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            // Process only if appropriate (not playing other music, etc.)
            if (is_music_playing || isClosing)
                return;

            int command = (int)e.MidiEvent.CommandCode;

            // Handle note on/off events
            if (command is (int)MidiCommandCode.NoteOn or (int)MidiCommandCode.NoteOff)
            {
                var noteEvent = e.MidiEvent as NoteEvent;
                if (noteEvent != null)
                {
                    int noteNumber = noteEvent.NoteNumber;
                    int velocity = noteEvent.Velocity;

                    // Note on with velocity > 0
                    if (command == (int)MidiCommandCode.NoteOn && velocity > 0)
                    {
                        // Add to active notes
                        if (!activeMidiNotes.Contains(noteNumber))
                        {
                            activeMidiNotes.Add(noteNumber);
                            UpdateLabelVisible(true);
                            if(Program.MIDIDevices.useMIDIoutput)
                            {
                                foreach(int note in activeMidiNotes)
                                {
                                    MIDIIOUtils.PlayMidiNote(note, 1, true); // Play with sustain
                                }
                            }
                            // If we already have alternating playback, but now only have one note
                            // we should stop alternating and play the single note directly
                            if (activeMidiNotes.Count == 1 && isAlternatingPlaying)
                            {
                                isAlternatingPlaying = false;
                            }

                            // Handle based on number of active notes
                            if (activeMidiNotes.Count == 1)
                            {
                                // Single note mode - play directly without alternating
                                int frequency = MIDIIOUtils.MidiNoteToFrequency(noteNumber);
                                NotePlayer.play_note(frequency, 0, true); // Continue until note off
                            }
                            else if (activeMidiNotes.Count > 1 && !isAlternatingPlaying)
                            {
                                // Multiple notes - start alternating
                                isAlternatingPlaying = true;
                                playAlternatingNotes();
                            }
                        }
                    }
                    // Note off or Note on with velocity 0 (equivalent to note off)
                    else
                    {
                        // Remove from active notes
                        if (activeMidiNotes.Contains(noteNumber))
                        {
                            activeMidiNotes.Remove(noteNumber);

                            // If no more notes, stop all playback
                            if (activeMidiNotes.Count == 0)
                            {
                                isAlternatingPlaying = false;
                                NotePlayer.StopAllNotes();
                                MIDIIOUtils.StopAllNotes(); // Stop all MIDI notes
                                UpdateLabelVisible(false);
                            }
                            // If we now have exactly one note and were in alternating mode
                            else if (activeMidiNotes.Count == 1 && isAlternatingPlaying)
                            {
                                // Switch to single note mode
                                isAlternatingPlaying = false;
                                int remainingNote = activeMidiNotes[0];
                                int frequency = MIDIIOUtils.MidiNoteToFrequency(remainingNote);
                                NotePlayer.StopAllNotes();
                            }
                        }
                    }
                }
            }
        }
        private void playAlternatingNotes()
        {
            Task.Run(() =>
            {
                do
                {
                    List<int> notesCopy;
                    lock (activeMidiNotes)
                    {
                        notesCopy = new List<int>(activeMidiNotes); // Create a copy
                    }

                    foreach (int note in notesCopy)
                    {
                        NotePlayer.play_note(MIDIIOUtils.MidiNoteToFrequency(note), Variables.alternating_note_length);
                    }
                }
                while (isAlternatingPlaying == true);
            });
        }
        public void UpdateMidiInputDevice(int deviceNumber)
        {
            if (MIDIIOUtils._midiIn != null)
            {
                // Stop and clean up the current MIDI input connection
                MIDIIOUtils._midiIn.Stop();
                MIDIIOUtils._midiIn.MessageReceived -= MidiIn_MessageReceived;
            }

            // Change the device using the MIDIIOUtils helper
            MIDIIOUtils.ChangeInputDevice(deviceNumber);

            // Set up the new connection if MIDI input is enabled
            if (Program.MIDIDevices.useMIDIinput && MIDIIOUtils._midiIn != null)
            {
                MIDIIOUtils._midiIn.MessageReceived += MidiIn_MessageReceived;
                MIDIIOUtils._midiIn.Start();
                Debug.WriteLine($"MIDI input device changed to device #{deviceNumber}");
            }

            // Stop any active alternating notes playback
            if (isAlternatingPlaying)
            {
                isAlternatingPlaying = false;
            }
            activeMidiInNotes.Clear();
        }
    }
}