﻿using GenerativeAI.Types;
using Microsoft.VisualBasic.Logging;
using NAudio.Midi;
using NeoBleeper.Properties;
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
using static NBPML_File;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace NeoBleeper
{
    public partial class main_window : Form
    {
        bool darkTheme = false;
        private play_beat_window play_Beat_Window;
        private PortamentoWindow portamentoWindow;
        private VoiceInternalsWindow voiceInternalsWindow;
        private bool isModified = false;
        private CommandManager commandManager;
        private Originator originator;
        private Memento initialMemento;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public event EventHandler MusicStopped;
        public event EventHandler NotesChanged;
        private int lastNotesCount = 0;
        protected virtual void OnNotesChanged(EventArgs e)
        {
            NotesChanged?.Invoke(this, e);
        }
        private string lastListHash = string.Empty;

        private string ComputeListHash()
        {
            if (listViewNotes?.Items == null || listViewNotes.Items.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder(listViewNotes.Items.Count * 32);
            foreach (ListViewItem item in listViewNotes.Items)
            {
                // Her satırın tüm subitem metinlerini ekle
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    sb.Append(item.SubItems[i].Text);
                    sb.Append('\u001F'); // saha ayırıcı
                }
                sb.Append('\u001E'); // satır ayırıcı
            }
            return sb.ToString();
        }
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
            UIFonts.setFonts(this);
            originator = new Originator(listViewNotes);
            commandManager = new CommandManager(originator);
            commandManager.StateChanged += CommandManager_StateChanged;
            lastNotesCount = listViewNotes.Items.Count;
            lastListHash = ComputeListHash();
            listViewNotes.DoubleBuffering(true);
            label_beep.DoubleBuffering(true);
            UpdateUndoRedoButtons();
            resizeColumn();
            main_window_refresh();
            comboBox_note_length.SelectedItem = comboBox_note_length.Items[3];
            comboBox_note_length.SelectedValue = comboBox_note_length.Items[3];
            Variables.octave = 4;
            Variables.bpm = 140;
            Variables.alternating_note_length = 30;
            Variables.note_silence_ratio = 0.5;
            initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternating_note_length);
            TemporarySettings.MIDIDevices.MidiStatusChanged += MidiDevices_StatusChanged;

            // Initialize MIDI input if it's enabled
            if (TemporarySettings.MIDIDevices.useMIDIinput)
            {
                InitializeMidiInput();
            }
        }
        private void resizeColumn() 
        {
            listViewNotes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (listViewNotes.Columns.Count > 0)
            {
                listViewNotes.Columns[listViewNotes.Columns.Count - 1].Width = 45;
            }
        }
        private async void CommandManager_StateChanged(object sender, EventArgs e)
        {
            UpdateUndoRedoButtons();

            try
            {
                // Execute on the UI thread if necessary
                if (listViewNotes != null)
                {
                    if (listViewNotes.InvokeRequired)
                        listViewNotes.BeginInvoke(new Action(resizeColumn));
                    else
                        resizeColumn();
                }

                int currentCount = listViewNotes?.Items?.Count ?? 0;
                string currentHash = ComputeListHash();

                // Trigger NotesChanged event only if there's an actual change
                if (currentCount != lastNotesCount || currentHash != lastListHash)
                {
                    lastNotesCount = currentCount;
                    lastListHash = currentHash;
                    OnNotesChanged(EventArgs.Empty);
                }
            }
            catch
            {
                // Ignore exceptions during UI updates
            }
        }
        private async void UpdateUndoRedoButtons()
        {
            undoToolStripMenuItem.Enabled = commandManager.CanUndo;
            redoToolStripMenuItem.Enabled = commandManager.CanRedo;
        }

        private void dark_theme()
        {
            darkTheme = true;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
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
            this.BackgroundImage = Resources.neobleeper_background_dark;
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
            checkBox_mute_playback.ForeColor = Color.White;
            notes_list_right_click.BackColor = Color.Black;
            notes_list_right_click.ForeColor = Color.White;
        }


        private void light_theme()
        {
            darkTheme = false;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
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
            this.BackgroundImage = Resources.neobleeper_background_light;
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
            checkBox_mute_playback.ForeColor = SystemColors.ControlText;
            notes_list_right_click.BackColor = SystemColors.Window;
            notes_list_right_click.ForeColor = SystemColors.WindowText;
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
                        if (check_system_theme.IsDarkTheme())
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
            set_theme();
            set_keyboard_colors();
            set_buttons_colors();
            set_beep_label_color();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note when key is clicked is changed to: {checkbox_play_note.Checked}", Logger.LogTypes.Info);
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
            Logger.Log($"Checked state of replace is changed to: {checkBox_replace.Checked}", Logger.LogTypes.Info);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] note_lengths = { "Whole", "Half", "Quarter", "1/8", "1/16", "1/32" };
            Logger.Log($"Selected note length is changed to: {note_lengths[comboBox_note_length.SelectedIndex]}", Logger.LogTypes.Info);
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSaveAsDialog();
        }

        private void aboutNeoBleeperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            closeAllOpenWindows();
            about_neobleeper about = new about_neobleeper();
            about.ShowDialog();
            Logger.Log("About window is opened", Logger.LogTypes.Info);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            closeAllOpenWindows();
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
            Logger.Log("Settings window is opened", Logger.LogTypes.Info);
        }
        private void closeAllOpenWindows()
        {
            if (checkBox_synchronized_play.Checked == true)
            {
                checkBox_synchronized_play.Checked = false;
            }
            if (checkBox_play_beat_sound.Checked == true)
            {
                checkBox_play_beat_sound.Checked = false;
            }
            if (checkBox_bleeper_portamento.Checked == true)
            {
                checkBox_bleeper_portamento.Checked = false;
            }
            if (checkBox_use_voice_system.Checked == true)
            {
                checkBox_use_voice_system.Checked = false;
            }
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
            if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[5])
            {
                Line.length = Resources.ThirtySecondNote;
            }
            if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[4])
            {
                Line.length = Resources.SixteenthNote;
            }
            if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[3])
            {
                Line.length = Resources.EighthNote;
            }
            if (comboBox_note_length.SelectedIndex == 2)
            {
                Line.length = Resources.QuarterNote;
            }
            if (comboBox_note_length.SelectedIndex == 1)
            {
                Line.length = Resources.HalfNote;
            }
            if (comboBox_note_length.SelectedIndex == 0)
            {
                Line.length = Resources.WholeNote;
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
            listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
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
                listViewNotes.EnsureVisible(selectedLine);
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
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[5])
                {
                    Line.length = Resources.ThirtySecondNote;
                }
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[4])
                {
                    Line.length = Resources.SixteenthNote;
                }
                if (comboBox_note_length.SelectedItem == comboBox_note_length.Items[3])
                {
                    Line.length = Resources.EighthNote;
                }
                if (comboBox_note_length.SelectedIndex == 2)
                {
                    Line.length = Resources.QuarterNote;
                }
                if (comboBox_note_length.SelectedIndex == 1)
                {
                    Line.length = Resources.HalfNote;
                }
                if (comboBox_note_length.SelectedIndex == 0)
                {
                    Line.length = Resources.WholeNote;
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
            if (MIDIIOUtils._midiOut != null && TemporarySettings.MIDIDevices.useMIDIoutput == true)
            {
                MIDIIOUtils.ChangeInstrument(MIDIIOUtils._midiOut, TemporarySettings.MIDIDevices.MIDIOutputInstrument,
                            TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel);
                await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, frequency, 100);
            }
            NotePlayer.play_note(frequency, 100);
        }
        private async void update_indicator_when_key_is_clicked()
        {
            if (checkBox_add_note_to_list.Checked == true)
            {
                updateDisplays(listViewNotes.Items.Count - 1, true);
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
            Logger.Log($"Key C{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key D{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key E{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key F{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key G{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key A{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key B{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key C{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key D{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key E{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key F{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key G{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key A{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key B{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key C{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key D{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key E{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key F{Variables.octave + 1} is clicked", Logger.LogTypes.Info); ;
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
            Logger.Log($"Key G{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key A{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key B{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key C#{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key D#{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key F#{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key G#{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key A#{Variables.octave - 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key C#{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key D#{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key F#{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key G#{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key A#{Variables.octave} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key C#{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key D#{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key F#{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key G#{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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
            Logger.Log($"Key A#{Variables.octave + 1} is clicked", Logger.LogTypes.Info);
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

            if (string.IsNullOrWhiteSpace(noteName) || noteName.Length < 2)
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


        private async void play_note_in_line_from_MIDIOutput(int index, bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length)
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
                if (play_note1 && !string.IsNullOrWhiteSpace(note1) && notes[0] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[0], length);
                if (play_note2 && !string.IsNullOrWhiteSpace(note2) && notes[1] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[1], length);
                if (play_note3 && !string.IsNullOrWhiteSpace(note3) && notes[2] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[2], length);
                if (play_note4 && !string.IsNullOrWhiteSpace(note4) && notes[3] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[3], length);
            }
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
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.KeyboardOctave))
                    {
                        Variables.octave = 4; // Default value
                        Logger.Log("Keyboard octave not found, defaulting to 4", Logger.LogTypes.Info);
                    }
                    else
                    {
                        Variables.octave = Convert.ToInt32(projectFile.Settings.RandomSettings.KeyboardOctave);
                    }
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
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.TimeSignature))
                    {
                        trackBar_time_signature.Value = 4; // Default value
                        lbl_time_signature.Text = "4";
                        Logger.Log("Time signature not found, defaulting to 4", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note silence ratio is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                    {
                        Variables.note_silence_ratio = 0.5; // Default value
                        lbl_note_silence_ratio.Text = "50%";
                        Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note length is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteLength))
                    {
                        comboBox_note_length.SelectedIndex = 0; // Default value
                        Logger.Log("Note length not found, defaulting to Whole", Logger.LogTypes.Info);
                    }
                    // Assign default values if no alternating note length is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.AlternateTime))
                    {
                        Variables.alternating_note_length = 30; // Default value
                        numericUpDown_alternating_notes.Value = 30;
                        Logger.Log("Alternating note length not found, defaulting to 30 ms", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note click play is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteClickPlay))
                    {
                        checkbox_play_note.Checked = true; // Default value
                        Logger.Log("Note click play not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note click add is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteClickAdd))
                    {
                        checkBox_add_note_to_list.Checked = true; // Default value
                        Logger.Log("Note click add not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note replace is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteReplace))
                    {
                        checkBox_replace.Checked = false; // Default value
                        Logger.Log("Note replace not found, defaulting to false", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note length replace is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteLengthReplace))
                    {
                        checkBox_replace_length.Checked = false; // Default value
                        Logger.Log("Note length replace not found, defaulting to false", Logger.LogTypes.Info);
                    }
                    // Assign default values if no click play note is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote1))
                    {
                        checkBox_play_note1_clicked.Checked = true; // Default value
                        Logger.Log("Click play note 1 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote2))
                    {
                        checkBox_play_note2_clicked.Checked = true; // Default value
                        Logger.Log("Click play note 2 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote3))
                    {
                        checkBox_play_note3_clicked.Checked = true; // Default value
                        Logger.Log("Click play note 3 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote4))
                    {
                        checkBox_play_note3_clicked.Checked = true; // Default value
                        Logger.Log("Click play note 4 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    // Assign default values if no play note is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote1))
                    {
                        checkBox_play_note1_played.Checked = true; // Default value
                        Logger.Log("Play note 1 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote2))
                    {
                        checkBox_play_note2_played.Checked = true; // Default value
                        Logger.Log("Play note 2 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote3))
                    {
                        checkBox_play_note3_played.Checked = true; // Default value
                        Logger.Log("Play note 3 not found, defaulting to true", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote4))
                    {
                        checkBox_play_note4_played.Checked = true; // Default value
                        Logger.Log("Play note 4 not found, defaulting to true", Logger.LogTypes.Info);
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
                        ListViewItem item = new ListViewItem(convertNoteLengthIntoLocalized(line.Length));
                        item.SubItems.Add(line.Note1);
                        item.SubItems.Add(line.Note2);
                        item.SubItems.Add(line.Note3);
                        item.SubItems.Add(line.Note4);
                        item.SubItems.Add(convertModifiersIntoLocalized(line.Mod));
                        item.SubItems.Add(convertArticulationsIntoLocalized(line.Art));
                        listViewNotes.Items.Add(item);
                    }
                    isModified = false;
                    UpdateFormTitle();
                }
                Logger.Log("File is succesfully created by AI", Logger.LogTypes.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"AI music creation failed: {ex.Message}", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageAIMusicCreationFailed + " " + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string convertNoteLengthIntoLocalized(string noteLength)
        {
            switch (noteLength)
            {
                case "Whole":
                    return Resources.WholeNote;
                case "Half":
                    return Resources.HalfNote;
                case "Quarter":
                    return Resources.QuarterNote;
                case "1/8":
                    return Resources.EighthNote;
                case "1/16":
                    return Resources.SixteenthNote;
                case "1/32":
                    return Resources.ThirtySecondNote;
                default:
                    return Resources.WholeNote;
            }
        }
        private string convertModifiersIntoLocalized(string modifier)
        {
            switch(modifier)
            {
                case "Dot":
                    return Resources.DottedModifier;
                case "Tri":
                    return Resources.TripletModifier;
                default:
                    return string.Empty;
            }
        }
        private string convertArticulationsIntoLocalized(string articulation)
        {
            switch(articulation)
            {
                case "Sta":
                    return Resources.StaccatoArticulation;
                case "Spi":
                    return Resources.SpiccatoArticulation;
                case "Fer":
                    return Resources.FermataArticulation;
                default:
                    return string.Empty;
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
                case "Bleeper Music Maker by Robbi-985 file format": // Legacy Bleeper Music Maker file format by Robbi-985
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
                            if (!lines.Any(line => line.StartsWith("KeyboardOctave")))
                            {
                                Variables.octave = 4; // Default value
                                Logger.Log("Keyboard octave not found, defaulting to 4", Logger.LogTypes.Info);
                            }
                            // Assign default values if none of the radiobuttons are checked
                            if (add_as_note1.Checked != true && add_as_note2.Checked != true && add_as_note3.Checked != true && add_as_note4.Checked != true)
                            {
                                add_as_note1.Checked = true;
                                Logger.Log("No note type selected, defaulting to Note 1", Logger.LogTypes.Info);
                            }
                            // Assign default values if no time signature is found
                            if (!lines.Any(line => line.StartsWith("TimeSig")))
                            {
                                trackBar_time_signature.Value = 4; // Default value
                                lbl_time_signature.Text = "4";
                                Logger.Log("Time signature not found, defaulting to 4", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note silence ratio is found
                            if (!lines.Any(line => line.StartsWith("NoteSilenceRatio")))
                            {
                                Variables.note_silence_ratio = 0.5; // Default value
                                lbl_note_silence_ratio.Text = "50%";
                                Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note length is found
                            if (!lines.Any(line => line.StartsWith("NoteLength")))
                            {
                                comboBox_note_length.SelectedIndex = 0; // Default value
                                Logger.Log("Note length not found, defaulting to Whole", Logger.LogTypes.Info);
                            }
                            // Assign default values if no alternating note length is found
                            if (!lines.Any(line => line.StartsWith("AlternateTime")))
                            {
                                Variables.alternating_note_length = 30; // Default value
                                numericUpDown_alternating_notes.Value = 30;
                                Logger.Log("Alternating note length not found, defaulting to 30 ms", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note click play is found
                            if (!lines.Any(line => line.StartsWith("NoteClickPlay")))
                            {
                                checkbox_play_note.Checked = true; // Default value
                                Logger.Log("Note click play not found, defaulting to true", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note click add is found
                            if (!lines.Any(line => line.StartsWith("NoteClickAdd")))
                            {
                                checkBox_add_note_to_list.Checked = true; // Default value
                                Logger.Log("Note click add not found, defaulting to true", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note replace is found
                            if (!lines.Any(line => line.StartsWith("NoteReplace")))
                            {
                                checkBox_replace.Checked = false; // Default value
                                Logger.Log("Note replace not found, defaulting to false", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note length replace is found
                            if (!lines.Any(line => line.StartsWith("NoteLengthReplace")))
                            {
                                checkBox_replace_length.Checked = false; // Default value
                                Logger.Log("Note length replace not found, defaulting to false", Logger.LogTypes.Info);
                            }
                            // Assign default values if no click play note is found
                            if (!lines.Any(line => line.StartsWith("ClickPlayNote1")))
                            {
                                checkBox_play_note1_clicked.Checked = true; // Default value
                                Logger.Log("Click play note 1 not found, defaulting to true", Logger.LogTypes.Info);
                            }
                            if (!lines.Any(line => line.StartsWith("ClickPlayNote2")))
                            {
                                checkBox_play_note2_clicked.Checked = true; // Default value
                                Logger.Log("Click play note 2 not found, defaulting to true", Logger.LogTypes.Info);
                            }
                            // Assign default values if no play note is found
                            if (!lines.Any(line => line.StartsWith("PlayNote1")))
                            {
                                checkBox_play_note1_played.Checked = true; // Default value
                                Logger.Log("Play note 1 not found, defaulting to true", Logger.LogTypes.Info);
                            }
                            if (!lines.Any(line => line.StartsWith("PlayNote2")))
                            {
                                checkBox_play_note2_played.Checked = true; // Default value
                                Logger.Log("Play note 2 not found, defaulting to true", Logger.LogTypes.Info);
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

                                    ListViewItem item = new ListViewItem(convertNoteLengthIntoLocalized(noteData[0])); // Note length
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
                                        if (noteData[3] != "-") // Modifier
                                        {
                                            for (int j = 0; j < 2; j++)
                                            {
                                                item.SubItems.Add(string.Empty);
                                            }
                                            item.SubItems.Add(convertModifiersIntoLocalized(noteData[3])); 
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
                            Logger.Log("Bleeper Music Maker file opened successfully", Logger.LogTypes.Info);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Error opening Bleeper Music Maker file: " + ex.Message, Logger.LogTypes.Error);
                            DialogResult dialogResult = MessageBox.Show(Resources.MessageNonStandardBleeperMusicMakerFile, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult != DialogResult.Yes)
                            {
                                Logger.Log("User chose not to open the file", Logger.LogTypes.Info);
                                createNewFile();
                            }
                            else
                            {
                                Logger.Log("User chose to open the file anyway", Logger.LogTypes.Info);
                            }
                        }
                        break;
                    }
                case "<NeoBleeperProjectFile>": // Modern NeoBleeper Project File format
                    {
                        try
                        {
                            is_file_valid = true;
                            saveToolStripMenuItem.Enabled = true;
                            saveAsToolStripMenuItem.Enabled = true;
                            NBPML_File.NeoBleeperProjectFile projectFile = DeserializeXML(filename); if (projectFile != null)
                            {
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.KeyboardOctave))
                                {
                                    Variables.octave = 4; // Default value
                                    Logger.Log("Keyboard octave not found, defaulting to 4", Logger.LogTypes.Info);
                                }
                                else
                                {
                                    Variables.octave = Convert.ToInt32(projectFile.Settings.RandomSettings.KeyboardOctave); Variables.octave = Convert.ToInt32(projectFile.Settings.RandomSettings.KeyboardOctave);
                                }
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
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.TimeSignature))
                                {
                                    trackBar_time_signature.Value = 4; // Default value
                                    lbl_time_signature.Text = "4";
                                    Logger.Log("Time signature not found, defaulting to 4", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note silence ratio is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                                {
                                    Variables.note_silence_ratio = 0.5; // Default value
                                    lbl_note_silence_ratio.Text = "50%";
                                    Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note length is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteLength))
                                {
                                    comboBox_note_length.SelectedIndex = 0; // Default value
                                    Logger.Log("Note length not found, defaulting to Whole", Logger.LogTypes.Info);
                                }
                                // Assign default values if no alternating note length is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.AlternateTime))
                                {
                                    Variables.alternating_note_length = 30; // Default value
                                    numericUpDown_alternating_notes.Value = 30;
                                    Logger.Log("Alternating note length not found, defaulting to 30 ms", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note click play is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteClickPlay))
                                {
                                    checkbox_play_note.Checked = true; // Default value
                                    Logger.Log("Note click play not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note click add is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteClickAdd))
                                {
                                    checkBox_add_note_to_list.Checked = true; // Default value
                                    Logger.Log("Note click add not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note replace is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteReplace))
                                {
                                    checkBox_replace.Checked = false; // Default value
                                    Logger.Log("Note replace not found, defaulting to false", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note length replace is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlaybackSettings.NoteLengthReplace))
                                {
                                    checkBox_replace_length.Checked = false; // Default value
                                    Logger.Log("Note length replace not found, defaulting to false", Logger.LogTypes.Info);
                                }
                                // Assign default values if no click play note is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote1))
                                {
                                    checkBox_play_note1_clicked.Checked = true; // Default value
                                    Logger.Log("Click play note 1 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote2))
                                {
                                    checkBox_play_note2_clicked.Checked = true; // Default value
                                    Logger.Log("Click play note 2 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote3))
                                {
                                    checkBox_play_note3_clicked.Checked = true; // Default value
                                    Logger.Log("Click play note 3 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.ClickPlayNotes.ClickPlayNote4))
                                {
                                    checkBox_play_note3_clicked.Checked = true; // Default value
                                    Logger.Log("Click play note 4 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                // Assign default values if no play note is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote1))
                                {
                                    checkBox_play_note1_played.Checked = true; // Default value
                                    Logger.Log("Play note 1 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote2))
                                {
                                    checkBox_play_note2_played.Checked = true; // Default value
                                    Logger.Log("Play note 2 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote3))
                                {
                                    checkBox_play_note3_played.Checked = true; // Default value
                                    Logger.Log("Play note 3 not found, defaulting to true", Logger.LogTypes.Info);
                                }
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.PlayNotes.PlayNote4))
                                {
                                    checkBox_play_note4_played.Checked = true; // Default value
                                    Logger.Log("Play note 4 not found, defaulting to true", Logger.LogTypes.Info);
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
                                        ListViewItem item = new ListViewItem(convertNoteLengthIntoLocalized(line.Length));
                                        item.SubItems.Add(line.Note1);
                                        item.SubItems.Add(line.Note2);
                                        item.SubItems.Add(line.Note3);
                                        item.SubItems.Add(line.Note4);
                                        item.SubItems.Add(convertModifiersIntoLocalized(line.Mod));
                                        item.SubItems.Add(convertArticulationsIntoLocalized(line.Art));
                                        listViewNotes.Items.Add(item);
                                    }
                                }
                                // Leave empty if no lines are found
                            }
                            Logger.Log("NeoBleeper file opened successfully", Logger.LogTypes.Info);
                            isModified = false;
                            UpdateFormTitle();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Error opening NeoBleeper file: " + ex.Message, Logger.LogTypes.Error);
                            DialogResult dialogResult = MessageBox.Show(Resources.MessageNonStandardNeoBleeperFile, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult != DialogResult.Yes)
                            {
                                Logger.Log("User chose not to open the file", Logger.LogTypes.Info);
                                createNewFile();
                            }
                            else
                            {
                                isModified = false;
                                UpdateFormTitle();
                                Logger.Log("User chose to open the file anyway", Logger.LogTypes.Info);
                            }
                        }
                        break;
                    }
                default:
                    {
                        is_file_valid = false;
                        Logger.Log("Invalid or corrupted music file", Logger.LogTypes.Error);
                        MessageBox.Show(Resources.MessageInvalidOrCorruptedMusicFile, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentFilePath) && currentFilePath.ToUpper().EndsWith(".NBPML"))
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
                            Length = convertLocalizedNoteLengthIntoUnlocalized(item.SubItems[0].Text),
                            Note1 = item.SubItems[1].Text,
                            Note2 = item.SubItems[2].Text,
                            Note3 = item.SubItems[3].Text,
                            Note4 = item.SubItems[4].Text,
                            Mod = convertLocalizedModifiersIntoUnlocalized(item.SubItems[5].Text),
                            Art = convertLocalizedArticulationsIntoUnlocalized(item.SubItems[6].Text)
                        }).ToArray()
                    }
                };

                SerializeXML(filename, projectFile); isModified = false;
                currentFilePath = filename;
                UpdateFormTitle();
                Logger.Log("NeoBleeper file saved successfully", Logger.LogTypes.Info);
            }
            catch (Exception ex)
            {
                Logger.Log("Error saving NeoBleeper file: " + ex.Message, Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageErrorSavingFile + " " + ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trackBar_note_silence_ratio_Scroll(object sender, EventArgs e)
        {
            Variables.note_silence_ratio = (Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100);
            lbl_note_silence_ratio.Text = trackBar_note_silence_ratio.Value.ToString() + "%";
            Logger.Log($"Note silence ratio is set to {trackBar_note_silence_ratio.Value}%", Logger.LogTypes.Info);
        }

        private void trackBar_time_signature_Scroll(object sender, EventArgs e)
        {
            lbl_time_signature.Text = trackBar_time_signature.Value.ToString();
            Logger.Log($"Time signature is set to {trackBar_time_signature.Value}", Logger.LogTypes.Info);
        }

        private async void noteLabelsUpdate()
        {
            try
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
                Line.mod = Resources.DottedModifier;
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
                Line.mod = Resources.TripletModifier;
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
                Line.art = Resources.StaccatoArticulation;
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
                Line.art = Resources.FermataArticulation;
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
                Line.art = Resources.SpiccatoArticulation;
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
            // If there's any checked item, remove all checked items
            if (listViewNotes.CheckedItems.Count > 0)
            {
                // Store the index of the first checked item
                int minCheckedIndex = listViewNotes.CheckedItems.Cast<ListViewItem>().Min(i => i.Index);

                var removeNoteCommand = new RemoveNoteCommand(listViewNotes, Target.Checked);
                commandManager.ExecuteCommand(removeNoteCommand);

                // Select the item that was below the last removed item, or the last item if it removed the last one
                if (listViewNotes.Items.Count > 0)
                {
                    int sel = Math.Min(minCheckedIndex, listViewNotes.Items.Count - 1);
                    listViewNotes.SelectedItems.Clear();
                    listViewNotes.Items[sel].Selected = true;
                    listViewNotes.EnsureVisible(sel);
                }
                isModified = true;
                UpdateFormTitle();
                Logger.Log("Checked lines erased", Logger.LogTypes.Info);
                return;
            }

            // Erase the selected item if no checked items are found
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int index = listViewNotes.SelectedIndices[0];
                var selectedItem = listViewNotes.SelectedItems[0];
                var removeNoteCommand = new RemoveNoteCommand(listViewNotes, selectedItem);

                // If there's an item below the selected item, select it after removal
                if (index < listViewNotes.Items.Count - 1)
                {
                    listViewNotes.Items[index + 1].Selected = true;
                }

                commandManager.ExecuteCommand(removeNoteCommand);

                isModified = true;
                UpdateFormTitle();
                Logger.Log("Selected line erased", Logger.LogTypes.Info);
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
            Logger.Log("New file created", Logger.LogTypes.Info);
        }
        private async void stopPlayingAllSounds()
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            KeyPressed = false;
            NotePlayer.StopAllNotes(); // Stop all notes
            await StopAllVoices(); // Stop all voices if using voice system
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller)
            {
                NotePlayer.StopMicrocontrollerSound(); // Stop the sound from the microcontroller
            }
            RemoveUnpressedKeys();
            singleNote = 0; // Reset the single note variable
            UnmarkAllButtons(); // Unmark all buttons
            isAlternatingPlayingRegularKeyboard = false; // Reset the alternating playing state
            Logger.Log("All sounds stopped", Logger.LogTypes.Info);
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
        private (int noteSound_int, int silence_int) CalculateNoteDurations(double baseLength)
        {
            // Compute raw double values
            double noteSound_double = FixRoundingErrors(note_length_calculator(baseLength));
            double totalRhythm_double = FixRoundingErrors(line_length_calculator(baseLength));

            // Convert to integers
            int totalRhythm_int = (int)Math.Truncate(totalRhythm_double);
            int noteSound_int = Math.Min((int)Math.Truncate(noteSound_double), totalRhythm_int);
            int silence_int = totalRhythm_int - noteSound_int;

            return (noteSound_int, silence_int);
        }
        public static double FixRoundingErrors(double inputValue)
        {
            // Define the threshold and adjustment values based on the assembly constants
            const double threshold = 1e-7;
            const double adjustment = 1e-10;

            // Check if the input value exceeds the threshold
            if (inputValue >= 0)
            {
                if (inputValue > threshold)
                {
                    inputValue += adjustment;
                }
            }
            else
            {
                if (inputValue < (threshold * -1))
                {
                    inputValue -= adjustment;
                }
            }
            // Return the corrected value
            return inputValue;
        }
        private void HandleMidiOutput(int noteSoundDuration)
        {
            if (TemporarySettings.MIDIDevices.useMIDIoutput && listViewNotes.SelectedIndices.Count > 0)
            {
                play_note_in_line_from_MIDIOutput(
                    listViewNotes.SelectedIndices[0],
                    checkBox_play_note1_played.Checked,
                    checkBox_play_note2_played.Checked,
                    checkBox_play_note3_played.Checked,
                    checkBox_play_note4_played.Checked,
                    noteSoundDuration);
            }
        }

        private async Task HandleStandardNotePlayback(int noteSoundDuration, int rawNoteDuration, bool nonStopping = false)
        {
            if (listViewNotes.SelectedIndices.Count > 0)
            {
                if (checkBox_use_voice_system.Checked)
                {
                    await play_note_in_line_with_voice(
                        checkBox_play_note1_played.Checked,
                        checkBox_play_note2_played.Checked,
                        checkBox_play_note3_played.Checked,
                        checkBox_play_note4_played.Checked,
                        noteSoundDuration, rawNoteDuration,
                        nonStopping
                    );
                }
                else
                {
                    await play_note_in_line(
                        checkBox_play_note1_played.Checked,
                        checkBox_play_note2_played.Checked,
                        checkBox_play_note3_played.Checked,
                        checkBox_play_note4_played.Checked,
                        noteSoundDuration,
                        nonStopping);
                }
            }
        }
        private bool IsSelectedNoteChecked()
        {
            return listViewNotes.SelectedIndices.Count > 0 && listViewNotes.SelectedItems[0].Checked;
        }
        private bool IsPlayVoiceOnCheckedLineEnabled()
        {
            return TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions == TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnCheckedLines;
        }
        private async Task play_note_in_line_with_voice(bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length, int rawLength, bool nonStopping = false) // Play note with voice in a line
        {
            // System speaker notes
            bool systemSpeakerNote1 = (TemporarySettings.VoiceInternalSettings.Note1OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && play_note1;
            bool systemSpeakerNote2 = (TemporarySettings.VoiceInternalSettings.Note2OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && play_note2;
            bool systemSpeakerNote3 = (TemporarySettings.VoiceInternalSettings.Note3OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && play_note3;
            bool systemSpeakerNote4 = (TemporarySettings.VoiceInternalSettings.Note4OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && play_note4;

            // Voice system notes
            bool voiceSystemNote1 = TemporarySettings.VoiceInternalSettings.Note1OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && play_note1;
            bool voiceSystemNote2 = TemporarySettings.VoiceInternalSettings.Note2OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && play_note2;
            bool voiceSystemNote3 = TemporarySettings.VoiceInternalSettings.Note3OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && play_note3;
            bool voiceSystemNote4 = TemporarySettings.VoiceInternalSettings.Note4OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && play_note4;

            // Play voice
            StartVoice(voiceSystemNote1, voiceSystemNote2, voiceSystemNote3, voiceSystemNote4, length, nonStopping);

            // Play system speaker notes
            await play_note_in_line(
                 systemSpeakerNote1,
                 systemSpeakerNote2,
                 systemSpeakerNote3,
                 systemSpeakerNote4,
                 rawLength,
                 nonStopping
                );
        }
        private async Task StartVoice(bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length, bool nonStopping = false)
        {
            string note1 = string.Empty, note2 = string.Empty, note3 = string.Empty, note4 = string.Empty;
            double note1_frequency = 0, note2_frequency = 0, note3_frequency = 0, note4_frequency = 0;
            String[] notes = new string[4];
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selected_line = listViewNotes.SelectedIndices[0];

                // Take music note names from the selected line
                note1 = play_note1 ? listViewNotes.Items[selected_line].SubItems[1].Text : string.Empty;
                note2 = play_note2 ? listViewNotes.Items[selected_line].SubItems[2].Text : string.Empty;
                note3 = play_note3 ? listViewNotes.Items[selected_line].SubItems[3].Text : string.Empty;
                note4 = play_note4 ? listViewNotes.Items[selected_line].SubItems[4].Text : string.Empty;
                // Calculate frequencies from note names
                await StopSelectedVoicesThatEmpty(note1, note2, note3, note4);
                if (!string.IsNullOrWhiteSpace(note1))
                {
                    note1_frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(0, (int)note1_frequency);
                }
                if (!string.IsNullOrWhiteSpace(note2))
                {
                    note2_frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(1, (int)note2_frequency);
                }
                if (!string.IsNullOrWhiteSpace(note3))
                {
                    note3_frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(2, (int)note3_frequency);
                }
                if (!string.IsNullOrWhiteSpace(note4))
                {
                    note4_frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(3, (int)note4_frequency);
                }
            }
        }
        public async Task StopAllVoices()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(i);
                }
            });
        }
        private async Task StopSelectedVoicesThatEmpty(string note1, string note2, string note3, string note4)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(note1))
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(0);
                }
                if (string.IsNullOrWhiteSpace(note2))
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(1);
                }
                if (string.IsNullOrWhiteSpace(note3))
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(2);
                }
                if (string.IsNullOrWhiteSpace(note4))
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(3);
                }
            });
        }
        public static double RemoveWholeNumber(double number)
        {
            return number - Math.Truncate(number);
        }
        private async void play_music(int startIndex)
        {
            bool nonStopping = false;
            int currentNoteIndex = startIndex;
            int baseLength = 0;

            EnableDisableCommonControls(false);

            // Use a single, high-resolution timer for the entire playback duration
            await HighPrecisionSleep.SleepAsync(1); // Sleep briefly to ensure accurate timing
            Stopwatch globalStopwatch = new Stopwatch();
            globalStopwatch.Start();

            // Store the total elapsed time of all previous notes
            double totalElapsedNoteDuration = 0.0;

            while (listViewNotes.SelectedItems.Count > 0 && is_music_playing)
            {
                if (Variables.bpm > 0)
                {
                    baseLength = Math.Max(1, (int)(60000.0 / (double)Variables.bpm));
                }

                nonStopping = trackBar_note_silence_ratio.Value == 100;
                var (noteSound_int, silence_int) = CalculateNoteDurations(baseLength);
                double noteDuration = line_length_calculator(baseLength); // + beat_length;
                int rawNoteDuration = (int)Math.Truncate(FixRoundingErrors(raw_note_length_calculator(baseLength))); // Note length calculator without note-silence ratio

                // Calculate the expected end time for the current note
                double expectedEndTime = totalElapsedNoteDuration + noteDuration;

                // Get the current elapsed time from the global stopwatch
                double currentTime = globalStopwatch.Elapsed.TotalMilliseconds;

                // Calculate the drift
                double drift = currentTime - totalElapsedNoteDuration;

                if (drift > 0) // Handle positive drift
                {
                    rawNoteDuration = (int)Math.Max(0, rawNoteDuration - drift);
                    if (drift < noteDuration) // If drift is less than the note duration, adjust the note sound duration
                    {
                        int cachedNoteDuration = noteSound_int;
                        // Adjust the note sound duration to compensate for drift
                        noteSound_int = Math.Max(1, noteSound_int - (int)drift);
                        if (drift - cachedNoteDuration > 0) // If drift exceeds the original note duration, adjust silence accordingly
                        {
                            silence_int = (Math.Max(0, (int)(drift - cachedNoteDuration)));
                        }
                        drift -= drift; // Reset drift after adjustment
                    }
                    else // If drift exceeds the note duration, skip to the next note
                    {
                        currentNoteIndex++;
                        if (currentNoteIndex > (listViewNotes.Items.Count - 1))
                        {
                            if (listViewNotes.Items.Count == 0)
                            {
                                EnableDisableCommonControls(true);
                                return;
                            }
                            int totalIndexOverflow = currentNoteIndex - (listViewNotes.Items.Count - 1); // Calculate how many indices we've gone past the end
                            int indexOverflow = totalIndexOverflow % listViewNotes.Items.Count; // Calculate the overflow within the bounds of the list
                            if (checkBox_loop.Checked)
                            {
                                // Looping enabled - wrap around to the start
                                currentNoteIndex = startIndex + indexOverflow;
                            }
                            else
                            {
                                // End of list reached and not looping - stop playback
                                listViewNotes.SelectedItems.Clear();
                                is_music_playing = false;
                                break;
                            }
                        }
                        UpdateListViewSelection(currentNoteIndex);
                        drift -= noteDuration;
                        totalElapsedNoteDuration += noteDuration;
                        continue; // Skip to the next note if drift exceeds note duration
                    }
                }
                // Normal playing flow
                HandleMidiOutput(noteSound_int);
                await HandleStandardNotePlayback(noteSound_int, rawNoteDuration, nonStopping);

                if (!nonStopping && silence_int > 0) // Only sleep if there's silence to wait for
                {
                    UpdateLabelVisible(false);
                    await HighPrecisionSleep.SleepAsync(silence_int);
                }
                if (drift < 0) // Handle negative drift
                {
                    await HighPrecisionSleep.SleepAsync(Math.Abs((int)drift));
                    drift -= drift;
                }
                // Update the total elapsed note duration for the next loop iteration
                totalElapsedNoteDuration += noteDuration;

                currentNoteIndex++;

                // Check if it's reached the end and handle looping
                if (currentNoteIndex >= listViewNotes.Items.Count)
                {
                    if (checkBox_loop.Checked)
                    {
                        UpdateListViewSelection(startIndex);
                        currentNoteIndex = startIndex;
                    }
                    else
                    {
                        // End of list reached and not looping - stop playback
                        listViewNotes.SelectedItems.Clear();
                        is_music_playing = false;
                        break;
                    }
                }
                else
                {
                    // Normal progression - update selection to current note
                    UpdateListViewSelection(currentNoteIndex);
                }
            }

            if (nonStopping)
            {
                stopAllNotesAfterPlaying();
            }
            await StopAllVoices();
            EnableDisableCommonControls(true);
        }

        // ListView update method
        private void UpdateListViewSelection(int index)
        {
            if (listViewNotes.InvokeRequired)
            {
                listViewNotes.Invoke(new Action(() =>
                {
                    if (index >= 0 && index < listViewNotes.Items.Count)
                    {
                        listViewNotes.SelectedItems.Clear();
                        listViewNotes.Items[index].Selected = true;
                    }
                }));
            }
            else
            {
                ;
                if (index >= 0 && index < listViewNotes.Items.Count)
                {
                    listViewNotes.SelectedItems.Clear();
                    listViewNotes.Items[index].Selected = true;
                }
            }
            if (index >= 0 && index < listViewNotes.Items.Count)
            {
                EnsureSpecificIndexVisible(index);
            }
        }
        private void EnsureSpecificIndexVisible(int index)
        {
            if (listViewNotes.InvokeRequired)
            {
                listViewNotes.Invoke(new Action(() =>
                {
                    listViewNotes.EnsureVisible(index);
                }));
            }
            else
            {
                listViewNotes.EnsureVisible(index);
            }
        }
        public void play_all()
        {
            if (listViewNotes.Items.Count > 0)
            {
                is_music_playing = true;
                listViewNotes.Items[0].Selected = true;
                EnsureSpecificIndexVisible(0);
                Logger.Log("Music is playing", Logger.LogTypes.Info);
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
                    Logger.Log("Music is playing", Logger.LogTypes.Info);
                    play_music(0);
                }
                else
                {
                    int index = listViewNotes.SelectedItems[0].Index;
                    EnsureSpecificIndexVisible(index);
                    Logger.Log("Music is playing", Logger.LogTypes.Info);
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
                    Logger.Log("Controls enabled", Logger.LogTypes.Info);
                    break;
                case false:
                    Logger.Log("Controls disabled", Logger.LogTypes.Info);
                    break;
            }
        }
        public void stop_playing()
        {
            is_music_playing = false;
            Logger.Log("Music stopped", Logger.LogTypes.Info);
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
            if (MIDIIOUtils._midiOut != null && TemporarySettings.MIDIDevices.useMIDIoutput == true)
            {
                MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, frequency, length);
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
                Logger.Log("Error initializing sounds: " + ex.Message, Logger.LogTypes.Error);
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
            try
            {
                metronomeTimer.Stop(); // Temporarily stop to prevent overlapping
                if (!checkBox_metronome.Checked)
                    return;

                // Play the appropriate sound first for minimal latency
                PlayMetronomeBeat(beatCount == 0);

                // Then update the UI (which is less time-critical)
                ShowMetronomeBeatLabel();

                // Get time signature value safely from UI thread
                int timeSignatureValue;
                if (trackBar_time_signature.InvokeRequired)
                {
                    timeSignatureValue = (int)trackBar_time_signature.Invoke(
                        new Func<int>(() => trackBar_time_signature.Value));
                }
                else
                {
                    timeSignatureValue = trackBar_time_signature.Value;
                }
                // Update beat counter
                beatCount = (beatCount + 1) % timeSignatureValue;

                // Schedule the next beat
                metronomeTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.Log("Metronome error: " + ex.Message, Logger.LogTypes.Error);
            }
        }

        private void PlayMetronomeBeat(bool isAccent)
        {
            int frequency = isAccent ? 1000 : 500;

            // Important: Play sound on high-priority thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                play_metronome_sound_from_midi_output(frequency, 15);
                NotePlayer.play_note(frequency, 15);
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


        private void UpdateLabelVisible(bool visible)
        {
            try
            {
                if (label_beep.InvokeRequired)
                {
                    label_beep.BeginInvoke(() =>
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
                else
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
                }
            }
            catch
            {
                return;
            }
        }


        private void StartMetronome()
        {
            beatCount = 0;
            double interval = Math.Max(1, 60000.0 / (double)Variables.bpm);
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
                Logger.Log("Metronome started", Logger.LogTypes.Info);
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
                Logger.Log("Metronome stopped", Logger.LogTypes.Info);
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
            Logger.Log("Blank line added", Logger.LogTypes.Info);
        }
        private void clear_note_1()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 1);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 1 cleared", Logger.LogTypes.Info);
        }
        private void clear_note_2()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 2);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 2 cleared", Logger.LogTypes.Info);
        }
        private void clear_note_3()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 3);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 3 cleared", Logger.LogTypes.Info);
        }
        private void clear_note_4()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 4);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 4 cleared", Logger.LogTypes.Info);
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
            Logger.Log("Line unselected", Logger.LogTypes.Info);
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
        private async void listViewNotes_Click(object sender, EventArgs e) // Stop music and play clicked note
        {
            var mousePoint = listViewNotes.PointToClient(Control.MousePosition);
            var hit = listViewNotes.HitTest(mousePoint);
            if (hit.Location == ListViewHitTestLocations.StateImage)
            {
                return;
            }

            stop_playing();
            EnableDisableCommonControls(false);
            if (listViewNotes.FocusedItem != null && listViewNotes.SelectedItems.Count > 0)
            {
                int baseLength = 0;
                Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                if (Variables.bpm != 0)
                {
                    baseLength = Math.Max(1, (int)(60000.0 / (double)Variables.bpm));
                }
                if (listViewNotes.SelectedItems.Count > 0)
                {
                    updateDisplays(listViewNotes.SelectedIndices[0]);
                }
                HighPrecisionSleep.Sleep(1);
                var (noteSound_int, silence_int) = CalculateNoteDurations(baseLength);
                int rawNoteDuration = (int)Math.Truncate(FixRoundingErrors(raw_note_length_calculator(baseLength))); // Note length calculator without note-silence ratio
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
                await HandleStandardNotePlayback(noteSound_int, rawNoteDuration, nonStopping);
                await StopAllVoices(); // Stop all voices if using voice system
                if (nonStopping == true)
                {
                    stopAllNotesAfterPlaying();
                }
                EnableDisableCommonControls(true);
                if (!(listViewNotes.SelectedItems == null) && listViewNotes.SelectedItems.Count > 0)
                {
                    Logger.Log($"Selected line: {listViewNotes.SelectedItems[0].Index} Length: " +
                     $"{listViewNotes.SelectedItems[0].SubItems[0].Text.ToString()} Note 1: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[1].Text.ToString())}" +
                     $" Note 2: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[2].Text.ToString())} Note 3: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[3].Text.ToString())}" +
                     $" Note 4: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[4].Text.ToString())} Modifier: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[5].Text.ToString())}" +
                     $" Articulation: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[6].Text.ToString())}", Logger.LogTypes.Info);
                }

            }
        }
        private string GetOrDefault(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "None" : value;
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

            Logger.Log($"BPM: {Variables.bpm}", Logger.LogTypes.Info);
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

            Logger.Log($"Alternating note length: {Variables.alternating_note_length}", Logger.LogTypes.Info);
        }
        private double raw_note_length_calculator(double baseLength)
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
            if (!string.IsNullOrWhiteSpace(articulation))
            {
                if (articulation.ToLowerInvariant().Contains(Resources.StaccatoArticulation.ToLower()))
                    articulationFactor = 0.5;    // Staccato: 0.5x length
                else if (articulation.ToLowerInvariant().Contains(Resources.SpiccatoArticulation.ToLower()))
                    articulationFactor = 0.25;   // Spiccato: 0.25x length
                else if (articulation.ToLowerInvariant().Contains(Resources.FermataArticulation.ToLower()))
                    articulationFactor = 2.0;    // Fermata: 2x length
            }

            // Note-silence ratio (from trackBar)
            double silenceRatio = (double)trackBar_note_silence_ratio.Value / 100.0;

            // Calculate the total note length - use precise calculations without truncation
            double result = getNoteLength(baseLength, noteType);
            result = getModifiedNoteLength(result, modifier);
            result = result * articulationFactor;

            // Only round at the very end when converting to integer milliseconds
            return Math.Max(1, result);
        }
        private double note_length_calculator(double baseLength)
        {
            if (listViewNotes.SelectedItems == null || listViewNotes.SelectedItems.Count == 0 ||
                listViewNotes.Items == null || listViewNotes.Items.Count == 0)
            {
                return 0;
            }

            // Note-silence ratio (from trackBar)
            double silenceRatio = (double)trackBar_note_silence_ratio.Value / 100.0;

            // Calculate the total note length - use precise calculations without truncation
            double result = raw_note_length_calculator(baseLength);
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
            if (!string.IsNullOrWhiteSpace(articulation) && articulation.ToLowerInvariant().Contains(Resources.FermataArticulation.ToLower()))
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
        public static double getNoteLength(double rawNoteLength, String lengthName)
        {
            // Yerelleştirilmiş kaynaklardan alınan değerlerle karşılaştırma
            if (lengthName == Resources.WholeNote)
                return rawNoteLength * 4.0;
            else if (lengthName == Resources.HalfNote)
                return rawNoteLength * 2.0;
            else if (lengthName == Resources.QuarterNote)
                return rawNoteLength * 1.0;
            else if (lengthName == Resources.EighthNote)
                return rawNoteLength * 0.5;
            else if (lengthName == Resources.SixteenthNote)
                return rawNoteLength * 0.25;
            else if (lengthName == Resources.ThirtySecondNote)
                return rawNoteLength * 0.125;
            else
                return rawNoteLength * 1.0; // Default to quarter note if unrecognized
        }
        public static double getModifiedNoteLength(double noteLength, String modifier)
        {
            double modifierFactor = 1.0;
            if (!string.IsNullOrWhiteSpace(modifier))
            {
                if (modifier.ToLowerInvariant().Contains(Resources.DottedModifier.ToLower()))
                    modifierFactor = 1.5; // Dotted: 1.5x length
                else if (modifier.ToLowerInvariant().Contains(Resources.TripletModifier.ToLower()))
                    modifierFactor = 1.0 / 3.0; // Triplet: 1/3x length
            }
            return noteLength * modifierFactor; // Remove truncation
        }
        private async void stopAllNotesAfterPlaying()
        {
            NotePlayer.StopAllNotes();
            await StopAllVoices(); // Stop all voices if using voice system
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller)
            {
                NotePlayer.StopMicrocontrollerSound();
            }
            UpdateLabelVisible(false);
        }
        private async Task play_note_in_line(bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length, bool nonStopping = false) // Play note in a line
        {
            Variables.alternating_note_length = Convert.ToInt32(numericUpDown_alternating_notes.Value);
            string note1 = string.Empty, note2 = string.Empty, note3 = string.Empty, note4 = string.Empty;
            double note1_frequency = 0, note2_frequency = 0, note3_frequency = 0, note4_frequency = 0;
            String[] notes = new string[4];
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selected_line = listViewNotes.SelectedIndices[0];

                // Take music note names from the selected line
                note1 = play_note1 ? listViewNotes.Items[selected_line].SubItems[1].Text : string.Empty;
                note2 = play_note2 ? listViewNotes.Items[selected_line].SubItems[2].Text : string.Empty;
                note3 = play_note3 ? listViewNotes.Items[selected_line].SubItems[3].Text : string.Empty;
                note4 = play_note4 ? listViewNotes.Items[selected_line].SubItems[4].Text : string.Empty;
                // Calculate frequencies from note names
                if (!string.IsNullOrWhiteSpace(note1))
                    note1_frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);

                if (!string.IsNullOrWhiteSpace(note2))
                    note2_frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);

                if (!string.IsNullOrWhiteSpace(note3))
                    note3_frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);

                if (!string.IsNullOrWhiteSpace(note4))
                    note4_frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
            }
            if (radioButtonPlay_alternating_notes1.Checked == true) // Odd column mode
            {
                notes = new string[] { note1, note2, note3, note4 };
            }
            else if (radioButtonPlay_alternating_notes2.Checked == true) // Even column mode
            {
                notes = new string[] { note1, note3, note2, note4 };
            }
            notes = notes.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray(); // Remove empty notes and duplicates
            if (notes.Length == 1)
            {
                if (notes[0].Contains(note1) && !string.IsNullOrWhiteSpace(note1))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note1_frequency), length, nonStopping);
                }
                else if (notes[0].Contains(note2) && !string.IsNullOrWhiteSpace(note2))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note2_frequency), length, nonStopping);
                }
                else if (notes[0].Contains(note3) && !string.IsNullOrWhiteSpace(note3))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note3_frequency), length, nonStopping);
                }
                else if (notes[0].Contains(note4) && (!string.IsNullOrWhiteSpace(note4)))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note4_frequency), length, nonStopping);
                }
            }
            else if (notes.Length > 1)
            {
                Stopwatch stopwatch = new Stopwatch();
                double totalDuration = length; // Total playing duration of loop
                bool isAnyNotePlayed = false;
                UpdateLabelVisible(true);
                stopwatch.Start();
                do
                {
                    long remainingTime = (long)totalDuration - stopwatch.ElapsedMilliseconds;
                    if (remainingTime <= 0)
                    {
                        break;
                    }

                    foreach (string note in notes)
                    {
                        remainingTime = (long)totalDuration - stopwatch.ElapsedMilliseconds;
                        if (remainingTime <= 0)
                        {
                            break;
                        }

                        double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);
                        int alternatingNoteDuration = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                        if (remainingTime >= alternatingNoteDuration)
                        {
                            await Task.Run(() => { NotePlayer.play_note(Convert.ToInt32(frequency), alternatingNoteDuration); });
                            isAnyNotePlayed = true;
                        }
                        else
                        {
                            if (isAnyNotePlayed)
                            {
                                UpdateLabelVisible(false);
                                HighPrecisionSleep.Sleep((int)remainingTime);
                            }
                            else
                            {
                                await Task.Run(() => { NotePlayer.play_note(Convert.ToInt32(frequency), (int)remainingTime); });
                            }
                        }
                    }
                }
                while (stopwatch.ElapsedMilliseconds < totalDuration);
                stopwatch.Stop(); // Stop the stopwatch after the loop ends
                UpdateLabelVisible(false);
            }
            else
            {
                if (!is_music_playing)
                {
                    EnableDisableCommonControls(false);
                }
                if (nonStopping == true)
                {
                    stopAllNotesAfterPlaying();
                }
                if (!is_music_playing)
                {
                    EnableDisableCommonControls(true);
                }
                await HighPrecisionSleep.SleepAsync(Math.Max(1, length));
            }
        }
        private void PlayBeepWithLabel(int frequency, int length, bool nonStopping = false)
        {
            UpdateLabelVisible(true);
            NotePlayer.play_note(frequency, length, nonStopping);
            if (!nonStopping)
            {
                UpdateLabelVisible(false);
            }
        }
        public static bool IsWholeNumber(double value)
        {
            // Compare the value with its truncated version
            return value == Math.Truncate(value);
        }

        private void listViewNotes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selectedLine = listViewNotes.SelectedIndices[0];
                if (listViewNotes.Items[selectedLine].SubItems[5].Text == Resources.DottedModifier && checkBox_staccato.Checked == false)
                {
                    checkBox_dotted.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[5].Text == Resources.TripletModifier && checkBox_fermata.Checked == false)
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
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == Resources.StaccatoArticulation && checkBox_staccato.Checked == false)
                {
                    checkBox_staccato.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == Resources.FermataArticulation && checkBox_fermata.Checked == false)
                {
                    checkBox_fermata.Checked = true;
                }
                if (listViewNotes.Items[selectedLine].SubItems[6].Text == Resources.SpiccatoArticulation && checkBox_spiccato.Checked == false)
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
                    updateDisplays(selectedLine);
                }
            }
        }
        int beat_length = 0; // Length of the beat sound in milliseconds for adding corrected note length to prevent irregularities
        private void updateDisplays(int Line, bool clicked = false)
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
                    lbl_beat_value.Text = Math.Round(beat_number, 4).ToString();
                    lbl_beat_traditional_value.Text = ConvertDecimalBeatToTraditional(beat);
                    lbl_beat_traditional_value.ForeColor = set_traditional_beat_color(lbl_beat_traditional_value.Text);
                });
                if (checkBox_play_beat_sound.Checked == true && clicked == false)
                {
                    switch (TemporarySettings.BeatTypes.beatType)
                    {
                        case TemporarySettings.BeatTypes.BeatType.PlayOnAllBeats:
                            if (IsWholeNumber(beat_number))
                            {
                                beat_length = play_beat_sound();
                            }
                            break;
                        case TemporarySettings.BeatTypes.BeatType.PlayOnOddBeats:
                            if (beat_number % 2 != 0 && IsWholeNumber(beat_number))
                            {
                                beat_length = play_beat_sound();
                            }
                            else
                            {
                                beat_length = 0; // Reset beat length if not playing on odd beats
                            }
                            break;
                        case TemporarySettings.BeatTypes.BeatType.PlayOnEvenBeats:
                            if (beat_number % 2 == 0 && IsWholeNumber(beat_number))
                            {
                                beat_length = play_beat_sound();
                            }
                            else
                            {
                                beat_length = 0; // Reset beat length if not playing on odd beats
                            }
                            break;
                        case TemporarySettings.BeatTypes.BeatType.PlayOnCheckedLines:
                            if (listViewNotes.Items[Line].Checked == true)
                            {
                                beat_length = play_beat_sound();
                            }
                            else
                            {
                                beat_length = 0; // Reset beat length if not playing on checked lines
                            }
                            break;
                    }
                }
                else
                {
                    beat_length = 0; // Reset beat length if not playing beat sound
                }
            }
        }
        private int play_beat_sound()
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
            if (TemporarySettings.MIDIDevices.useMIDIoutput)
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
            return (length * 2) + (length / 2); // Total length of the beat sound
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
                    return (wholeNumber + 1) + " " + Properties.Resources.TextBeatError;
                }
                else
                {
                    return $"1 {Properties.Resources.TextBeatError}";
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
                return (wholeNumber + 1) + " " + Properties.Resources.TextBeatError;
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
                case var s when s == Resources.WholeNote:
                    length = 4m;
                    break;
                case var s when s == Resources.HalfNote:
                    length = 2m;
                    break;
                case var s when s == Resources.QuarterNote:
                    length = 1m;
                    break;
                case var s when s == Resources.EighthNote:
                    length = 0.5m;
                    break;
                case var s when s == Resources.SixteenthNote:
                    length = 0.25m;
                    break;
                case var s when s == Resources.ThirtySecondNote:
                    length = 0.125m;
                    break;
                default:
                    length = 1m; // Default: Quarter
                    break;
            }


            // Apply modifiers
            switch (listViewItem.SubItems[5].Text)
            {
                case var s when s == Resources.DottedModifier:
                    modifier = 1.5m;
                    break;
                case var s when s == Resources.TripletModifier:
                    modifier = 1m / 3m; // Precise triplet calculation
                    break;
                default:
                    modifier = 1m;
                    break;
            }

            // Use the articulation subitem to determine the articulation effect
            switch (listViewItem.SubItems[6].Text)
            {
                case var s when s == Resources.StaccatoArticulation:
                    articulation = 0.5m;
                    break;
                case var s when s == Resources.SpiccatoArticulation:
                    articulation = 0.25m;
                    break;
                case var s when s == Resources.FermataArticulation:
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
                Logger.Log("Synchronized play window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_synchronized_play.Checked == false)
            {
                closeSynchronizedPlayWindow();
                Logger.Log("Synchronized play window is closed.", Logger.LogTypes.Info);
            }
        }
        private async void stop_all_sounds_before_closing()
        {
            NotePlayer.StopAllNotes();
            await StopAllVoices(); // Stop all voices if using voice system
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller)
            {
                NotePlayer.StopMicrocontrollerSound();
            }
        }

        private void main_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop_playing();
            checkBox_metronome.Checked = false;
            cancellationTokenSource.Cancel();
            isClosing = true;
            stop_all_sounds_before_closing();
        }

        private void main_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            stop_playing();
            checkBox_metronome.Checked = false;
            cancellationTokenSource.Cancel();
            isClosing = true;
            stop_all_sounds_before_closing();
        }
        private void rewindToSavedVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            int selectedLine = listViewNotes.SelectedItems.Count > 0 ? listViewNotes.SelectedIndices[0] : -1;
            if (initialMemento == null)
            {
                MessageBox.Show(Resources.MessageNoSavedVersion, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                setThemeOfListViewItems(); // Set theme of list view items after rewinding if theme when rewinding is different
                if (listViewNotes.Items.Count > 0)
                {
                    if (selectedLine != -1 && selectedLine < listViewNotes.Items.Count)
                    {
                        listViewNotes.EnsureVisible(selectedLine);
                    }
                    else
                    {
                        listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                    }
                }
                // Log states of variables
                Logger.Log($"Rewind to saved version - BPM: {Variables.bpm}, Alt Notes: {Variables.alternating_note_length}", Logger.LogTypes.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error rewinding: {ex.Message}", Logger.LogTypes.Error);
                MessageBox.Show($"{Resources.MessageErrorRewinding} {ex.Message}", Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox_mute_system_speaker_CheckedChanged(object sender, EventArgs e)
        {
            if (TemporarySettings.creating_sounds.is_playback_muted == false && checkBox_mute_playback.Checked == false)
            {
                NotePlayer.StopAllNotes(); // Stop all sounds if playback is not muted
            }
            TemporarySettings.creating_sounds.is_playback_muted = checkBox_mute_playback.Checked;
            Logger.Log($"Playback muted: {TemporarySettings.creating_sounds.is_playback_muted}", Logger.LogTypes.Info);
        }
        private void show_keyboard_keys_shortcut()
        {
            foreach (var entry in buttonShortcuts)
            {
                if (entry.Key.InvokeRequired)
                {
                    entry.Key.Invoke(new Action(() => entry.Key.Text = entry.Value));
                }
                else
                {
                    entry.Key.Text = entry.Value;
                }
            }
        }
        private void hide_keyboard_keys_shortcut()
        {
            Task.Run(() =>
            {
                foreach (var entry in buttonShortcuts)
                {
                    if (entry.Key.InvokeRequired)
                    {
                        entry.Key.Invoke(new Action(() => entry.Key.Text = string.Empty));
                    }
                    else
                    {
                        entry.Key.Text = string.Empty;
                    }
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
                StopAllSounds();
            }
            foreach (Control control in this.Controls)
            {
                enableDisableTabStop(control, !checkBox_use_keyboard_as_piano.Checked);
            }
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
                    if (MIDIFileValidator.IsMidiFile(fileName))
                    {
                        openMIDIFilePlayer(fileName);
                    }
                    else if (first_line == "Bleeper Music Maker by Robbi-985 file format" ||
                        first_line == "<NeoBleeperProjectFile>")
                    {
                        FileParser(fileName);
                    }
                    else
                    {
                        Logger.Log("The file you dragged is not supported by NeoBleeper or is corrupted.", Logger.LogTypes.Error);
                        MessageBox.Show(Resources.MessageNonSupportedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    Logger.Log("The file you dragged is corrupted or the file is in use by another process.", Logger.LogTypes.Error);
                    MessageBox.Show(Resources.MessageCorruptedOrCurrentlyUsedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void openMIDIFilePlayer(string fileName)
        {
            if (!(checkBox_mute_playback.Checked && !TemporarySettings.MIDIDevices.useMIDIoutput))
            {
                if (MIDIFileValidator.IsMidiFile(openFileDialog.FileName))
                {
                    MIDI_file_player midi_file_player = new MIDI_file_player(openFileDialog.FileName, this);
                    midi_file_player.ShowDialog();
                    Logger.Log("MIDI file is opened.", Logger.LogTypes.Info);
                }
                else
                {
                    MessageBox.Show(Resources.MessageNonValidMIDIFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("This file is not a valid MIDI file, or it is corrupted or is being used by another process.", Logger.LogTypes.Error);
                }
            }
            else
            {
                Logger.Log("\"Mute playback\" is checked and \"Use MIDI output\" checkbox is unchecked, so it cannot be opened.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MIDIFilePlayerMutedError, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (!(checkBox_mute_playback.Checked && !TemporarySettings.MIDIDevices.useMIDIoutput))
            {
                openFileDialog.Filter = "MIDI Files|*.mid";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    openMIDIFilePlayer(openFileDialog.FileName);
                }
            }
            else
            {
                Logger.Log("\"Mute playback\" is checked and \"Use MIDI output\" checkbox is unchecked, so it cannot be opened.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MIDIFilePlayerMutedError, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Logger.Log($"Checked state of play note 1 when clicked is changed to: {checkBox_play_note1_clicked.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_play_note2_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 2 when clicked is changed to: {checkBox_play_note2_clicked.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_play_note3_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 3 when clicked is changed to: {checkBox_play_note3_clicked.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_play_note4_clicked_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 4 when clicked is changed to: {checkBox_play_note4_clicked.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_play_note1_played_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 1 when played is changed to: {checkBox_play_note1_played.Checked}", Logger.LogTypes.Info);
        }
        private void checkBox_play_note2_played_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 2 when played is changed to: {checkBox_play_note2_played.Checked}", Logger.LogTypes.Info);
        }
        private void checkBox_play_note3_played_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 3 when played is changed to: {checkBox_play_note3_played.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_play_note4_played_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of play note 4 when played is changed to: {checkBox_play_note4_played.Checked}", Logger.LogTypes.Info);
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }
        private void CopyToClipboard()
        {
            // Combine selected and checked items into a single collection
            var itemsToCopy = listViewNotes.SelectedItems.Cast<ListViewItem>()
                .Union(listViewNotes.CheckedItems.Cast<ListViewItem>())
                .Distinct();

            if (itemsToCopy.Any())
            {
                StringBuilder clipboardText = new StringBuilder();

                foreach (ListViewItem item in itemsToCopy)
                {
                    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                    {
                        clipboardText.Append(subItem.Text + "\t"); // Combine the text with a tab character
                    }
                    clipboardText.Length--; // Remove the last tab character
                    clipboardText.AppendLine(); // Add a newline for the next item
                }

                // Remove the last newline character
                if (clipboardText.Length > 0)
                {
                    clipboardText.Length--;
                }

                // Copy to clipboard
                Clipboard.SetText(clipboardText.ToString());
                Toast toast = new Toast(this, Resources.ToastMessageNotesCopy, 2000);
                toast.Show();
                Logger.Log("Copy to clipboard is executed.", Logger.LogTypes.Info);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }
        
        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsText()) return;

            string clipboardText = Clipboard.GetText();
            string[] lines = clipboardText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            int insertIndex = -1;
            if (listViewNotes.SelectedItems.Count > 0)
            {
                insertIndex = listViewNotes.SelectedIndices[0];
            }

            List<ListViewItem> itemsToAdd = new List<ListViewItem>();

            foreach (string line in lines)
            {
                string[] subItems = line.Split('\t');

                if (subItems.Length >= 7)
                {
                    ListViewItem newItem = new ListViewItem(subItems[0]);
                    for (int i = 1; i < subItems.Length; i++)
                    {
                        newItem.SubItems.Add(subItems[i]);
                    }
                    itemsToAdd.Add(newItem);
                }
            }

            if (itemsToAdd.Any())
            {
                var addNotesCommand = new AddNoteCommand(listViewNotes, itemsToAdd, insertIndex);
                commandManager.ExecuteCommand(addNotesCommand);

                isModified = true;
                UpdateFormTitle();
                if (insertIndex != -1) // If inserted at a specific index, ensure that index is visible
                {
                    listViewNotes.EnsureVisible(insertIndex);
                }
                else
                {
                    listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                }

                Logger.Log("Paste is executed.", Logger.LogTypes.Info);
            }
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ;
            if (is_music_playing == true)
            {
                stop_playing();
            }
            int selectedLine = listViewNotes.SelectedItems.Count > 0 ? listViewNotes.SelectedIndices[0] : -1;
            commandManager.Undo();
            setThemeOfListViewItems(); // Set theme of list view items after undoing if theme when undoing is different 
            if (listViewNotes.Items.Count > 0)
            {
                if (selectedLine != -1 && selectedLine < listViewNotes.Items.Count)
                {
                    listViewNotes.EnsureVisible(selectedLine);
                }
                else
                {
                    listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                }
            }
            Logger.Log("Undo is executed.", Logger.LogTypes.Info);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_music_playing == true)
            {
                stop_playing();
            }
            int selectedLine = listViewNotes.SelectedItems.Count > 0 ? listViewNotes.SelectedIndices[0] : -1;
            commandManager.Redo();
            setThemeOfListViewItems(); // Set theme of list view items after redoing if theme when redoing is different
            if (listViewNotes.Items.Count > 0)
            {
                if (selectedLine != -1 && selectedLine < listViewNotes.Items.Count)
                {
                    listViewNotes.EnsureVisible(selectedLine);
                }
                else
                {
                    listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                }
            }
            Logger.Log("Redo is executed.", Logger.LogTypes.Info);
        }
        private void setThemeOfListViewItems()
        {
            if (listViewNotes.Items.Count > 0)
            {
                switch (darkTheme)
                {
                    case true:
                        if (listViewNotes.Items[0].BackColor != Color.Black)
                        {
                            foreach (ListViewItem items in listViewNotes.Items)
                            {
                                items.BackColor = Color.Black;
                                items.ForeColor = Color.White;
                                foreach (ListViewItem.ListViewSubItem subItem in items.SubItems)
                                {
                                    subItem.BackColor = Color.Black;
                                    subItem.ForeColor = Color.White;
                                }
                            }
                        }
                        break;
                    case false:
                        if (listViewNotes.Items[0].BackColor != SystemColors.Window)
                        {
                            foreach (ListViewItem items in listViewNotes.Items)
                            {
                                items.BackColor = SystemColors.Window;
                                items.ForeColor = SystemColors.WindowText;
                                foreach (ListViewItem.ListViewSubItem subItem in items.SubItems)
                                {
                                    subItem.BackColor = SystemColors.Window;
                                    subItem.ForeColor = SystemColors.WindowText;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void button_synchronized_play_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show(Resources.SynchronizedPlayHelp, Resources.SynchronizedPlayHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_play_beat_sound_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show(Resources.PlayBeatSoundHelp, Resources.PlayBeatSoundHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_bleeper_portamento_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show(Resources.PortamentoHelp, Resources.PortamentoHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_use_keyboard_as_piano_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show(Resources.UseKeyboardAsPianoHelp, Resources.UseKeyboardAsPianoHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_do_not_update_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show(Resources.DoNotUpdateHelp, Resources.DoNotUpdateHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutToClipboard();
        }
        private void CutToClipboard()
        {
            var itemsToCut = listViewNotes.SelectedItems.Cast<ListViewItem>()
                .Union(listViewNotes.CheckedItems.Cast<ListViewItem>())
                .Distinct()
                .ToList();

            if (itemsToCut.Any())
            {
                StringBuilder clipboardText = new StringBuilder();

                foreach (ListViewItem item in itemsToCut)
                {
                    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                    {
                        clipboardText.Append(subItem.Text + "\t");
                    }
                    clipboardText.Length--;
                    clipboardText.AppendLine();
                }

                if (clipboardText.Length > 0)
                {
                    clipboardText.Length--;
                }

                Clipboard.SetText(clipboardText.ToString());

                var removeCommand = new RemoveNoteCommand(listViewNotes, Target.Both);
                commandManager.ExecuteCommand(removeCommand);

                isModified = true;
                UpdateFormTitle();
                Toast toast = new Toast(this, Resources. ToastMessageNotesCut, 2000);
                toast.Show();
                Logger.Log("Cut is executed.", Logger.LogTypes.Info);
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
                        item.Text += " " + Resources.TextNotFound; // Add "(Not Found)" if the file does not exist
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
                    Logger.Log("The recent file you are trying to open is not found.", Logger.LogTypes.Error);
                    MessageBox.Show(Resources.MessageFileNotFoundError + " " + filePath, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Logger.Log($"Error opening recent file: {ex.Message}", Logger.LogTypes.Error);
                MessageBox.Show($"{Resources.MessageErrorFileOpening} {ex.Message}", Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void main_window_Load(object sender, EventArgs e)
        {
            InitializeMetronome();
            UpdateRecentFilesMenu();
        }

        private void checkBox_add_note_to_list_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of add note to list is changed to: {checkBox_add_note_to_list.Checked}", Logger.LogTypes.Info);
        }

        private void add_as_note1_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note1.Checked == true)
            {
                Logger.Log("Add as note 1 is checked", Logger.LogTypes.Info);
            }
        }

        private void add_as_note2_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note2.Checked == true)
            {
                Logger.Log("Add as note 2 is checked", Logger.LogTypes.Info);
            }
        }

        private void add_as_note3_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note3.Checked == true)
            {
                Logger.Log("Add as note 3 is checked", Logger.LogTypes.Info);
            }
        }

        private void add_as_note4_CheckedChanged(object sender, EventArgs e)
        {
            if (add_as_note4.Checked == true)
            {
                Logger.Log("Add as note 4 is checked", Logger.LogTypes.Info);
            }
        }

        private void checkBox_replace_length_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of replace length is changed to: {checkBox_replace_length.Checked}", Logger.LogTypes.Info);
        }

        private void radioButtonPlay_alternating_notes1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPlay_alternating_notes1.Checked == true)
            {
                Logger.Log("Play alternating notes in order is checked", Logger.LogTypes.Info);
            }
        }

        private void radioButtonPlay_alternating_notes2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPlay_alternating_notes2.Checked == true)
            {
                Logger.Log("Play alternating notes in odd-even order is checked", Logger.LogTypes.Info);
            }
        }

        private void checkBox_loop_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of loop is changed to: {checkBox_loop.Checked}", Logger.LogTypes.Info);
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
            Logger.Log($"Checked state of do not update is changed to: {checkBox_do_not_update.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_dotted_Click(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of dotted is changed to: {checkBox_dotted.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_triplet_Click(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of triplet is changed to: {checkBox_triplet.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_staccato_Click(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of staccato is changed to: {checkBox_staccato.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_spiccato_Click(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of spiccato is changed to: {checkBox_spiccato.Checked}", Logger.LogTypes.Info);
        }

        private void checkBox_fermata_Click(object sender, EventArgs e)
        {
            Logger.Log($"Checked state of fermata is changed to: {checkBox_fermata.Checked}", Logger.LogTypes.Info);
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
            if (!string.IsNullOrWhiteSpace(currentFilePath))
            {
                title += " - " + currentFilePath;
            }
            else if (this.Text.Contains("AI Generated Music"))
            {
                title += " - AI Generated Music";
            }

            // Add an asterisk if the file is modified
            if (isModified && !string.IsNullOrWhiteSpace(currentFilePath))
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

                Logger.Log($"Values restored: BPM={bpmValue}, Alt Notes={alternatingNoteLength}", Logger.LogTypes.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error restoring values: {ex.Message}", Logger.LogTypes.Error);
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
        private void openVoiceInternalsWindow()
        {
            if (voiceInternalsWindow == null || voiceInternalsWindow.IsDisposed)
            {
                voiceInternalsWindow = new VoiceInternalsWindow(this);
            }
            voiceInternalsWindow.Show();
            checkBox_use_voice_system.Tag = voiceInternalsWindow;
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
        private void closeVoiceInternalsWindow()
        {
            VoiceInternalsWindow voiceInternalsWindow = checkBox_use_voice_system.Tag as VoiceInternalsWindow;
            voiceInternalsWindow.Close();
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
                Logger.Log("Play a beat sound window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_play_beat_sound.Checked == false)
            {
                closePlayBeatSoundWindow();
                Logger.Log("Play a beat sound window is closed.", Logger.LogTypes.Info);
            }
        }
        private string convertLocalizedNoteLengthIntoUnlocalized(string noteLength)
        {
            switch (noteLength)
            {
                case var s when s == Resources.WholeNote:
                    return "Whole";
                case var s when s == Resources.HalfNote:
                    return "Half";
                case var s when s == Resources.QuarterNote:
                    return "Quarter";
                case var s when s == Resources.EighthNote:
                    return "1/8";
                case var s when s == Resources.SixteenthNote:
                    return "1/16";
                case var s when s == Resources.ThirtySecondNote:
                    return "1/32";
                default:
                    return Resources.WholeNote;
            }
        }
        private string convertLocalizedModifiersIntoUnlocalized(string modifier)
        {
            switch (modifier)
            {
                case var s when s == Resources.DottedModifier:
                    return "Dot";
                case var s when s == Resources.TripletModifier:
                    return "Tri";
                default:
                    return string.Empty;
            }
        }
        private string convertLocalizedArticulationsIntoUnlocalized(string articulation)
        {
            switch (articulation)
            {
                case var s when s == Resources.StaccatoArticulation:
                    return "Sta";
                case var s when s == Resources.SpiccatoArticulation:
                    return "Spi";
                case var s when s == Resources.FermataArticulation:
                    return "Fer";
                default:
                    return string.Empty;
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
                            Length = convertLocalizedNoteLengthIntoUnlocalized(item.SubItems[0].Text),
                            Note1 = item.SubItems[1].Text,
                            Note2 = item.SubItems[2].Text,
                            Note3 = item.SubItems[3].Text,
                            Note4 = item.SubItems[4].Text,
                            Mod = convertLocalizedModifiersIntoUnlocalized(item.SubItems[5].Text),
                            Art = convertLocalizedArticulationsIntoUnlocalized(item.SubItems[6].Text)
                        }).ToArray()
                    }
                };

                // Serialize to string into XML format and remove namespace
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty); // Namespace'i kaldır
                    serializer.Serialize(stringWriter, projectFile, namespaces);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error converting to NBPML string: " + ex.Message, Logger.LogTypes.Error);
                return string.Empty; // Return empty string in case of error
            }
        }

        private void convertToGCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
                closeAllOpenWindows();
                if (checkBox_synchronized_play.Checked == true)
                {
                    checkBox_synchronized_play.Checked = false;
                }
                ConvertToGCode convertToGCode = new ConvertToGCode(ConvertToNBPMLString());
                convertToGCode.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Log("Error converting to GCode: " + ex.Message, Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageConvertToGCodeError + " " + ex.Message, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox_bleeper_portamento_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_bleeper_portamento.Checked == true)
            {
                openBleeperPortamentoWindow();
                Logger.Log("Bleeper portamento window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_play_beat_sound.Checked == false)
            {
                closePortamentoWindow();
                Logger.Log("Bleeper portamento window is closed.", Logger.LogTypes.Info);
                NotePlayer.StopAllNotes();
                UpdateLabelVisible(false);
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
                e.Handled = true; // Prevent the key press from being processed further
                HashSet<int> currentlyPressedKeys = new HashSet<int>();
                currentlyPressedKeys.Add((int)e.KeyCode);
                if (currentlyPressedKeys == pressedKeys)
                {
                    // If the key is already pressed, do nothing
                    e.Handled = true;
                    return;
                }
                KeyPressed = true; // Set KeyPressed to true when a key is pressed
                pressedKeys.Add((int)e.KeyCode);
                keyCharNum = pressedKeys.Distinct().ToArray();
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
            keyCharNum = keyCharNum.Distinct().ToArray();

            isAlternatingPlayingRegularKeyboard = false;
            UnmarkAllButtons();

            // Don't stop all notes if portamento is enabled and set to always produce sound
            if (!(checkBox_bleeper_portamento.Checked &&
                  TemporarySettings.PortamentoSettings.portamentoType == TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound))
            {
                stopAllNotesAfterPlaying();
            }

            if (pressedKeys.Count == 0)
            {
                KeyPressed = false;
                singleNote = 0;
            }
            else
            {
                if (pressedKeys.Count != 1)
                {
                    singleNote = 0;
                }
                foreach (int key in keyCharNum)
                {
                    MarkupTheKeyWhenKeyIsPressed(key);
                }
                playWithRegularKeyboard();
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
        private async void playWithRegularKeyboard() // Play notes with regular keyboard (the keyboard of the computer, not MIDI keyboard)
        {
            if (!checkBox_use_keyboard_as_piano.Checked)
                return;
            keyCharNum = pressedKeys.Distinct().ToArray();
            if (TemporarySettings.MIDIDevices.useMIDIoutput)
            {
                foreach (int key in keyCharNum)
                {
                    int midiNote = MIDIIOUtils.FrequencyToMidiNote(GetFrequencyFromKeyCode(key));
                    // Don't play the same note again if it's already active
                    if (!activeMidiNotes.Contains(midiNote))
                    {
                        activeMidiNotes.Add(midiNote);
                        MIDIIOUtils.PlayMidiNoteAsync(midiNote, 1, true);
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
                            int frequency = GetFrequencyFromKeyCode(key);
                            if (checkBox_bleeper_portamento.Checked)
                            {
                                PlayPortamento(frequency);
                                await HighPrecisionSleep.SleepAsync(TemporarySettings.PortamentoSettings.length);
                            }
                            else
                            {
                                await PlayBeepWithLabelAsync(frequency, Variables.alternating_note_length);
                            }

                            if (!isAlternatingPlayingRegularKeyboard && checkBox_use_keyboard_as_piano.Checked)
                            {
                                if (keyCharNum.Length == 0)
                                    return;
                                int midiNote = MIDIIOUtils.FrequencyToMidiNote(GetFrequencyFromKeyCode(keyCharNum[0]));
                                singleNote = midiNote;
                                if (checkBox_bleeper_portamento.Checked)
                                {
                                    PlayPortamento(MIDIIOUtils.MidiNoteToFrequency(midiNote));
                                }
                                else
                                {
                                    await PlayBeepWithLabelAsync(MIDIIOUtils.MidiNoteToFrequency(midiNote), 1, true);
                                }
                            }
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
                    int frequency = MIDIIOUtils.MidiNoteToFrequency(midiNote);
                    if (checkBox_bleeper_portamento.Checked)
                    {
                        PlayPortamento(frequency);
                    }
                    else
                    {
                        await PlayBeepWithLabelAsync(frequency, 1, true);
                    }
                }
            }
        }
        private async Task PlayBeepWithLabelAsync(int frequency, int duration, bool nonStopping = false)
        {
            await Task.Run(() => PlayBeepWithLabel(frequency, duration, nonStopping));
        }
        private void MarkupTheKeyWhenKeyIsPressed(int keyCode)
        {
            if (!checkBox_use_keyboard_as_piano.Checked)
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
            if (!TemporarySettings.MIDIDevices.useMIDIinput || MIDIIOUtils._midiIn == null)
                return;

            // Set up event handler for MIDI input
            MIDIIOUtils._midiIn.MessageReceived += MidiIn_MessageReceived;
            MIDIIOUtils._midiIn.Start(); // Start listening for MIDI input

            Logger.Log("MIDI input initialized and listening", Logger.LogTypes.Info);
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
                        Logger.Log("MIDI input device already disconnected", Logger.LogTypes.Error);
                    }
                }

                // If MIDI input is enabled, try to re-initialize it with a fresh device instance
                if (TemporarySettings.MIDIDevices.useMIDIinput)
                {
                    // Force reinitialize the MIDI device with the current device index
                    MIDIIOUtils.ChangeInputDevice(TemporarySettings.MIDIDevices.MIDIInputDevice);

                    // Only proceed if we successfully created a new device instance
                    if (MIDIIOUtils._midiIn != null)
                    {
                        MIDIIOUtils._midiIn.MessageReceived += MidiIn_MessageReceived;
                        try
                        {
                            MIDIIOUtils._midiIn.Start();
                            Logger.Log("MIDI input reinitialized and listening", Logger.LogTypes.Info);
                        }
                        catch (NAudio.MmException ex)
                        {
                            Logger.Log($"Failed to start MIDI input: {ex.Message}", Logger.LogTypes.Error);
                            // Update UI or show a message to the user that MIDI input is unavailable
                            MessageBox.Show($"{Resources.MessageStartMIDIInputDeviceError} {ex.Message}\n {Resources.MessageStartMIDIInputDeviceErrorPart2}",
                                Resources.MIDIDeviceErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            // Reset the MIDI device reference
                            MIDIIOUtils._midiIn = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error handling MIDI status change: {ex.Message}", Logger.LogTypes.Error);
            }
        }

        // Store active MIDI notes for alternating playback
        private List<int> activeMidiNotes = new List<int>();
        private bool isAlternatingPlaying = false;
        private int lastFrequency = 0;
        private CancellationTokenSource portamentoCts = new CancellationTokenSource();

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
                            if (TemporarySettings.MIDIDevices.useMIDIoutput)
                            {
                                foreach (int note in activeMidiNotes)
                                {
                                    MIDIIOUtils.PlayMidiNote(note, 1, true); // Play with sustain
                                }
                            }
                            // If the note is already active, it does not need to be played again
                            // it should be stopped and restarted if necessary.
                            if (activeMidiNotes.Count == 1 && isAlternatingPlaying)
                            {
                                isAlternatingPlaying = false;
                            }

                            // Handle based on number of active notes
                            if (activeMidiNotes.Count == 1)
                            {
                                // Single note mode - play directly without alternating
                                int frequency = MIDIIOUtils.MidiNoteToFrequency(noteNumber);
                                if (checkBox_bleeper_portamento.Checked)
                                {
                                    PlayPortamento(frequency);
                                }
                                else
                                {
                                    PlayBeepWithLabel(frequency, 0, true); // Continue until note off
                                }
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
                                portamentoCts.Cancel(); // Stop any running portamento
                                MIDIIOUtils.StopAllNotes(); // Stop all MIDI notes

                                // Only stop the sound if not in "Always Produce Sound" mode
                                if (!checkBox_bleeper_portamento.Checked ||
                                    TemporarySettings.PortamentoSettings.portamentoType != TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound)
                                {
                                    NotePlayer.StopAllNotes();
                                    UpdateLabelVisible(false);
                                }
                                // In "Always Produce Sound" mode, it should not stop the sound
                                // but the last frequency should be reset
                                lastFrequency = 0;
                            }
                            // If it was the last note in alternating mode, switch to single note mode
                            else if (activeMidiNotes.Count == 1 && isAlternatingPlaying)
                            {
                                // Switch to single note mode
                                isAlternatingPlaying = false;
                                int remainingNote = activeMidiNotes[0];
                                int frequency = MIDIIOUtils.MidiNoteToFrequency(remainingNote);
                                if (checkBox_bleeper_portamento.Checked)
                                {
                                    PlayPortamento(frequency);
                                }
                                else
                                {
                                    PlayBeepWithLabel(frequency, 0, true);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void PlayPortamento(int targetFrequency)
        {
            // Cancel any ongoing portamento
            portamentoCts.Cancel();
            portamentoCts = new CancellationTokenSource();
            var token = portamentoCts.Token;

            Task.Run(async () =>
            {
                int startFrequency = (lastFrequency == 0) ? targetFrequency : lastFrequency;
                lastFrequency = targetFrequency;

                int steps = TemporarySettings.PortamentoSettings.pitchChangeSpeed / 100;
                if (steps == 0) steps = 1;
                int totalDuration = TemporarySettings.PortamentoSettings.length;
                int stepDuration = Math.Max(1, totalDuration / steps);

                for (int i = 0; i <= steps; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    double progress = (double)i / steps;
                    int currentFrequency = (int)(startFrequency + (targetFrequency - startFrequency) * progress);
                    UpdateLabelVisible(true);
                    NotePlayer.play_note(currentFrequency, stepDuration, true);
                    await Task.Delay(stepDuration, token);
                }

                if (TemporarySettings.PortamentoSettings.portamentoType == TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound)
                {
                    UpdateLabelVisible(true);
                    NotePlayer.play_note(targetFrequency, 1, true);
                }
                else
                {
                    if (!token.IsCancellationRequested)
                    {
                        NotePlayer.StopAllNotes();
                        UpdateLabelVisible(false);
                    }
                }
            }, token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                {
                    UpdateLabelVisible(false);
                }
            });
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
                        PlayBeepWithLabel(MIDIIOUtils.MidiNoteToFrequency(note), Variables.alternating_note_length);
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
            if (TemporarySettings.MIDIDevices.useMIDIinput && MIDIIOUtils._midiIn != null)
            {
                MIDIIOUtils._midiIn.MessageReceived += MidiIn_MessageReceived;
                MIDIIOUtils._midiIn.Start();
                Logger.Log($"MIDI input device changed to device #{deviceNumber}", Logger.LogTypes.Info);
            }

            // Stop any active alternating notes playback
            if (isAlternatingPlaying)
            {
                isAlternatingPlaying = false;
            }
            activeMidiInNotes.Clear();
        }
        private void StopAllSounds()
        {
            // Stop alternating playback and reset flags
            isAlternatingPlayingRegularKeyboard = false;
            KeyPressed = false;
            singleNote = 0; // Reset singleNote to ensure no lingering playback
            UnmarkAllButtons(); // Unmark all buttons when a key is released
            stopAllNotesAfterPlaying(); // Stop all notes only when no keys remain
        }
        private void main_window_Deactivate(object sender, EventArgs e)
        {
            if (checkBox_use_keyboard_as_piano.Checked)
            {
                StopAllSounds();
            }
        }

        private void main_window_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL.md") { UseShellExecute = true });
        }

        private void button_use_voice_system_help_Click(object sender, EventArgs e)
        {
            stopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageBox.Show(Resources.UseVoiceSystemHelp, Resources.UseVoiceSystemHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void checkBox_use_voice_system_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_use_voice_system.Checked == true)
            {
                openVoiceInternalsWindow();
                Logger.Log("Voice internals window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_use_voice_system.Checked == false)
            {
                await StopAllVoices();
                closeVoiceInternalsWindow();
                Logger.Log("Voice internals window is closed", Logger.LogTypes.Info);
            }
        }

        private void checkAllNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewNotes.Items)
            {
                item.Checked = true;
            }
        }

        private void uncheckAllNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewNotes.Items)
            {
                item.Checked = false;
            }
        }
    }
}