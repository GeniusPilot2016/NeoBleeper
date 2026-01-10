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
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using NAudio.Midi;
using NeoBleeper.Properties;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using static UIHelper;

namespace NeoBleeper
{
    // The main window of the application, which inspired from Bleeper Music Maker by Robbi-985 (aka SomethingUnreal)
    public partial class MainWindow : Form
    {
        bool darkTheme = false;
        private PlayBeatWindow playBeatWindow;
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
        string lastOpenedProjectFileName = string.Empty;
        public static string lastOpenedMIDIFileName = string.Empty;
        private string keyToolTip = string.Empty;
        public static DateTime lastCreateTime = DateTime.MinValue;
        private readonly TimeSpan createCooldown = TimeSpan.FromSeconds(10); // 10 seconds cooldown to prevent out-of-RPM (Requests Per Minute) quota errors
        protected virtual void OnNotesChanged(EventArgs e)
        {
            NotesChanged?.Invoke(this, e);
        }
        private string lastListHash = string.Empty;

        /// <summary>
        /// Generates a hash string representing the current contents of the notes list view.
        /// </summary>
        /// <remarks>The returned string can be used to detect changes in the list view's contents, such
        /// as for caching or change tracking purposes. The format uses non-printable Unicode characters as separators
        /// between subitems and items.</remarks>
        /// <returns>A string that uniquely represents the concatenated text of all items and subitems in the list view. Returns
        /// an empty string if the list is empty.</returns>
        private string ComputeListHash()
        {
            if (listViewNotes?.Items == null || listViewNotes.Items.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder(listViewNotes.Items.Count * 32);
            foreach (ListViewItem item in listViewNotes.Items)
            {
                // Append each subitem's text to the string builder
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    sb.Append(item.SubItems[i].Text);
                    sb.Append('\u001F'); // Seperator for subitems
                }
                sb.Append('\u001E'); // Line separator
            }
            return sb.ToString();
        }
        private bool keyPressed = false;
        int[] keyCharNum;
        public static class Variables
        {
            public static int octave = 4;
            public static int bpm = 140;
            public static int alternatingNoteLength = 30;
            public static double noteSilenceRatio = 0.5;
            public static int timeSignature = 4;
        }
        private bool isClosing = false;
        string currentFilePath;
        public Boolean isMusicPlaying = false;
        Boolean isFileValid = false;
        public MainWindow()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.SuspendLayout();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            PowerManager.SystemSleeping += PowerManager_SystemSleeping;
            PowerManager.PreparingToShutdown += PowerManager_PreparingToShutdown;
            PowerManager.PreparingToLogoff += PowerManager_PreparingToLogoff;
            PowerManager.SystemHibernating += PowerManager_SystemHibernating;
            PowerManager.Logoff += PowerManager_Logoff;
            PowerManager.Shutdown += PowerManager_Shutdown;
            InputLanguageManager.InputLanguageChanged += InputLanguageManager_InputLanguageChanged;
            keyToolTip = toolTip1.GetToolTip(button_c3) != null ? toolTip1.GetToolTip(button_c3) : string.Empty;
            InitializeButtonShortcuts();
            UIFonts.SetFonts(this);
            originator = new Originator(listViewNotes);
            commandManager = new CommandManager(originator);
            commandManager.StateChanged += CommandManager_StateChanged;
            lastNotesCount = listViewNotes.Items.Count;
            lastListHash = ComputeListHash();
            listViewNotes.DoubleBuffering(true);
            label_beep.DoubleBuffering(true);
            trackBar_note_silence_ratio.DoubleBuffering(true);
            trackBar_time_signature.DoubleBuffering(true);
            UpdateUndoRedoButtons();
            ResizeColumn();
            RefreshMainWindow();
            comboBox_note_length.SelectedIndex = 3; // Default to "1/8" note length
            initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternatingNoteLength,
                Variables.noteSilenceRatio, Variables.timeSignature);
            TemporarySettings.MIDIDevices.MidiStatusChanged += MidiDevices_StatusChanged;

            // Initialize MIDI input if it's enabled
            if (TemporarySettings.MIDIDevices.useMIDIinput)
            {
                InitializeMidiInput();
            }
            InitializePercussionNames();
            this.ResumeLayout();
        }

        private void InputLanguageManager_InputLanguageChanged(object? sender, EventArgs e)
        {
            InitializeButtonShortcuts();
        }

        private void PowerManager_Shutdown(object? sender, EventArgs e)
        {
            // Handle actual shutdown
            AskForSavingIfModified(() => { Application.Exit(); });
        }

        private void PowerManager_Logoff(object? sender, EventArgs e)
        {
            // Handle actual logoff
            StopPlaying(); // Stop playing sounds
            StopPlayingAllSounds(); // Stop all sounds
        }

        private void PowerManager_SystemHibernating(object? sender, EventArgs e)
        {
            // Handle system sleep/hibernate
            StopPlaying(); // Stop playing sounds
            StopPlayingAllSounds(); // Stop all sounds
        }

        private void PowerManager_PreparingToLogoff(object? sender, EventArgs e)
        {
            // Handle logoff preparation
            StopPlaying(); // Stop playing sounds
            StopPlayingAllSounds(); // Stop all sounds
        }

        private void PowerManager_SystemSleeping(object? sender, EventArgs e)
        {
            // Handle system sleep/hibernate
            StopPlaying(); // Stop playing sounds
            StopPlayingAllSounds(); // Stop all sounds
        }

        private void PowerManager_PreparingToShutdown(object? sender, EventArgs e)
        {
            // Handle shutdown preparation
            AskForSavingIfModified(() => { Application.Exit(); });
        }

        private void ResizeColumn()
        {
            listViewNotes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (listViewNotes.Columns.Count > 0)
            {
                listViewNotes.Columns[listViewNotes.Columns.Count - 1].Width = 45;
            }
        }
        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    SetTheme();
                }
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
                        listViewNotes.BeginInvoke(new Action(ResizeColumn));
                    else
                        ResizeColumn();
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

        /// <summary>
        /// Converts a raw key name string to a standard musical note name format.
        /// </summary>
        /// <param name="keyName">The raw key name to convert. Expected to be in the format "[note][octave]" (e.g., "c4") or
        /// "[note]_s[octave]" for sharps (e.g., "d4_s").</param>
        /// <returns>A string representing the note name in standard format (e.g., "C4" or "D#4"). If the input does not match
        /// the expected pattern, returns the original key name.</returns>
        private static string ConvertRawKeyNameToNoteName(string keyName)
        {
            // Example key names: "c4", "d4_s", "a5", "g3_s"
            var match = Regex.Match(keyName, @"^([a-gA-G])(_s)?(\d+)$");
            if (!match.Success)
                return keyName; // Return the original key name if it doesn't match the expected pattern

            string noteLetter = match.Groups[1].Value.ToUpper();
            bool isSharp = match.Groups[2].Success;
            string octave = match.Groups[3].Value;

            return isSharp ? $"{noteLetter}#{octave}" : $"{noteLetter}{octave}";
        }

        /// <summary>
        /// Converts a button name string to its corresponding raw musical note name.
        /// </summary>
        /// <param name="ButtonName">The button name to convert. The value should be in the format "button_<note>", such as "button_C3".</param>
        /// <returns>A string representing the raw musical note name corresponding to the specified button name.</returns>
        private string ConvertButtonNameToRawNote(string ButtonName)
        {
            string buttonName = ButtonName.ToLower(); // "button_c3"
            string keyName = buttonName.StartsWith("button_") ? buttonName.Substring(7) : buttonName; // "c3"
            string noteName = ConvertRawKeyNameToNoteName(keyName); // "C3"
            return noteName;
        }

        /// <summary>
        /// Updates the tooltips for all key buttons on the keyboard panel to reflect the current MIDI output channel
        /// and settings.
        /// </summary>
        /// <remarks>If the MIDI output channel is set to channel 10 (percussion) and MIDI output is
        /// enabled, the tooltips for keys corresponding to percussion notes are updated to display percussion
        /// instrument names. Otherwise, standard key tooltips are used. This method should be called whenever MIDI
        /// output settings change to ensure tooltips remain accurate.</remarks>
        private void SetToolTipForKeys()
        {
            string baseToolTip = keyToolTip;
            string concatenatingToolTipLine = Resources.TextPercussionTooltip;
            if ((TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel == 9) &&
                        TemporarySettings.MIDIDevices.useMIDIoutput)
            {
                Logger.Log("MIDI Output Channel is set to 10 (Percussion) - Updating key tooltips to show percussion names where applicable.", Logger.LogTypes.Info);
            }
            else
            {
                Logger.Log("MIDI Output Channel is not set to 10 (Percussion) - Using standard key tooltips.", Logger.LogTypes.Info);
            }
            foreach (Control ctrl in keyboard_panel.Controls)
            {
                if (ctrl is Button key)
                {
                    if ((TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel == 9) &&
                        TemporarySettings.MIDIDevices.useMIDIoutput) // Channel 9 is percussion in MIDI (0-based index)
                    {
                        int midiNoteNumber = CalculateMIDINumber(NoteNameToMIDINumber(ConvertButtonNameToRawNote(key.Name)));
                        if (midiNoteNumber >= 27 && midiNoteNumber <= 93)
                        {
                            if (PercussionKeyToolTipLabels.TryGetValue(midiNoteNumber, out string percussionLabel))
                            {
                                string replacedToolTip = concatenatingToolTipLine.Replace("{percussionName}", percussionLabel);
                                string finalToolTip = baseToolTip + replacedToolTip;
                                toolTip1.SetToolTip(key, finalToolTip);
                            }
                            else
                            {
                                toolTip1.SetToolTip(key, baseToolTip);
                            }
                        }
                        else
                        {
                            toolTip1.SetToolTip(key, baseToolTip);
                        }
                    }
                    else
                    {
                        toolTip1.SetToolTip(key, baseToolTip);
                    }
                }
            }
        }
        private static readonly Dictionary<int, string> PercussionKeyToolTipLabels = new Dictionary<int, string>
        {
        };

        /// <summary>
        /// Initializes the mapping of MIDI percussion note numbers to their corresponding instrument names.
        /// </summary>
        /// <remarks>This method populates the collection of percussion key tooltip labels using data from
        /// the application's resources. It is intended to be called during initialization to ensure that percussion
        /// instrument names are available for display in the user interface.</remarks>
        private void InitializePercussionNames()
        {
            string percussionNamesData = Resources.PercussionNames;
            // Support both ", " and "," as separators
            string[] lines = percussionNamesData
                .Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            // Start MIDI percussion notes from 27 (which corresponds to MIDI note number 27)
            for (int i = 0; i < lines.Length; i++)
            {
                int midiNote = 27 + i;
                if (midiNote <= 93) // MIDI percussion notes range from 27 to 93
                {
                    PercussionKeyToolTipLabels.Add(midiNote, lines[i]);
                }
            }
        }

        /// <summary>
        /// Updates the enabled state of the Undo and Redo menu items based on the current command history.
        /// </summary>
        /// <remarks>Call this method after actions that may affect the ability to undo or redo, such as
        /// executing, undoing, or redoing commands. This ensures that the user interface accurately reflects the
        /// available actions.</remarks>
        private async void UpdateUndoRedoButtons()
        {
            undoToolStripMenuItem.Enabled = commandManager.CanUndo;
            redoToolStripMenuItem.Enabled = commandManager.CanRedo;
        }

        private void DarkTheme()
        {
            darkTheme = true;
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
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }

        private void LightTheme()
        {
            darkTheme = false;
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
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        /// <summary>
        /// Applies the current application theme based on user settings and system preferences.
        /// </summary>
        /// <remarks>This method updates the visual appearance of the application by selecting either a
        /// light or dark theme. If the theme setting is set to automatic, the method determines the appropriate theme
        /// based on the system's current theme preference. The method suspends layout updates and enables double
        /// buffering to ensure a smooth transition. UI changes are forced to update immediately after the theme is
        /// applied.</remarks>
        private async void SetTheme()
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
                            DarkTheme();
                        }
                        else
                        {
                            LightTheme();
                        }
                        break;

                    case 1:
                        LightTheme();
                        break;

                    case 2:
                        DarkTheme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
            }
        }

        /// <summary>
        /// Sets the background and foreground colors of the keyboard key labels according to the configured octave
        /// color settings.
        /// </summary>
        /// <remarks>This method updates the appearance of the key labels to reflect the current color
        /// preferences for each octave. It should be called whenever the octave color settings change to ensure the
        /// keyboard display remains consistent with user preferences.</remarks>
        private void SetKeyboardKeyColors()
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
            lbl_c3.ForeColor = SetTextColor.GetTextColor(lbl_c3.BackColor);
            lbl_d3.ForeColor = SetTextColor.GetTextColor(lbl_d3.BackColor);
            lbl_e3.ForeColor = SetTextColor.GetTextColor(lbl_e3.BackColor);
            lbl_f3.ForeColor = SetTextColor.GetTextColor(lbl_f3.BackColor);
            lbl_g3.ForeColor = SetTextColor.GetTextColor(lbl_g3.BackColor);
            lbl_a3.ForeColor = SetTextColor.GetTextColor(lbl_a3.BackColor);
            lbl_b3.ForeColor = SetTextColor.GetTextColor(lbl_b3.BackColor);
            lbl_c4.ForeColor = SetTextColor.GetTextColor(lbl_c4.BackColor);
            lbl_d4.ForeColor = SetTextColor.GetTextColor(lbl_d4.BackColor);
            lbl_e4.ForeColor = SetTextColor.GetTextColor(lbl_e4.BackColor);
            lbl_f4.ForeColor = SetTextColor.GetTextColor(lbl_f4.BackColor);
            lbl_g4.ForeColor = SetTextColor.GetTextColor(lbl_g4.BackColor);
            lbl_a4.ForeColor = SetTextColor.GetTextColor(lbl_a4.BackColor);
            lbl_b4.ForeColor = SetTextColor.GetTextColor(lbl_b4.BackColor);
            lbl_c5.ForeColor = SetTextColor.GetTextColor(lbl_c5.BackColor);
            lbl_d5.ForeColor = SetTextColor.GetTextColor(lbl_d5.BackColor);
            lbl_e5.ForeColor = SetTextColor.GetTextColor(lbl_e5.BackColor);
            lbl_f5.ForeColor = SetTextColor.GetTextColor(lbl_f5.BackColor);
            lbl_g5.ForeColor = SetTextColor.GetTextColor(lbl_g5.BackColor);
            lbl_a5.ForeColor = SetTextColor.GetTextColor(lbl_a5.BackColor);
            lbl_b5.ForeColor = SetTextColor.GetTextColor(lbl_b5.BackColor);
        }

        /// <summary>
        /// Updates the background and foreground colors of all related buttons to match the current application
        /// settings.
        /// </summary>
        /// <remarks>This method should be called after changing color-related settings to ensure that
        /// button appearance reflects the latest configuration. It is typically used to synchronize the user interface
        /// with user preferences or theme changes.</remarks>
        private void SetButtonsColors()
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
            button_blank_line.ForeColor = SetTextColor.GetTextColor(button_blank_line.BackColor);
            button_clear_note1.ForeColor = SetTextColor.GetTextColor(button_clear_note1.BackColor);
            button_clear_note2.ForeColor = SetTextColor.GetTextColor(button_clear_note2.BackColor);
            button_clear_note3.ForeColor = SetTextColor.GetTextColor(button_clear_note3.BackColor);
            button_clear_note4.ForeColor = SetTextColor.GetTextColor(button_clear_note4.BackColor);
            button_unselect.ForeColor = SetTextColor.GetTextColor(button_unselect.BackColor);
            button_erase_line.ForeColor = SetTextColor.GetTextColor(button_erase_line.BackColor);
            button_play_all.ForeColor = SetTextColor.GetTextColor(button_play_all.BackColor);
            button_play_from_selected_line.ForeColor = SetTextColor.GetTextColor(button_play_from_selected_line.BackColor);
            button_stop_playing.ForeColor = SetTextColor.GetTextColor(button_stop_playing.BackColor);
        }

        /// <summary>
        /// Sets the background and foreground colors of the beep indicator label based on the current application
        /// settings.
        /// </summary>
        private void SetBeepLabelColor()
        {
            label_beep.BackColor = Settings1.Default.beep_indicator_color;
            label_beep.ForeColor = SetTextColor.GetTextColor(label_beep.BackColor);
        }

        /// <summary>
        /// Refreshes the appearance of the main window by updating theme and control colors.
        /// </summary>
        private void RefreshMainWindow()
        {
            SetTheme();
            SetKeyboardKeyColors();
            SetButtonsColors();
            SetBeepLabelColor();
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
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            CloseAllOpenWindows();
            AboutNeobleeper about = new AboutNeobleeper(this);
            about.ShowDialog();
            Logger.Log("About window is opened", Logger.LogTypes.Info);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            CloseAllOpenWindows();
            SettingsWindow settings = new SettingsWindow(this); // Pass reference to main_window
            settings.ColorsAndThemeChanged += RefreshMainWindowElementsColors;
            settings.ColorsAndThemeChanged += (s, args) =>
            {
                SynchronizedPlayWindow synchronizedPlayWindow = checkBox_synchronized_play.Tag as SynchronizedPlayWindow;
                if (synchronizedPlayWindow != null)
                {
                    synchronizedPlayWindow.SetTheme();
                }
                PlayBeatWindow playBeatWindow = checkBox_play_beat_sound.Tag as PlayBeatWindow;
                if (playBeatWindow != null)
                {
                    playBeatWindow.SetTheme();
                }
            };
            settings.ShowDialog();
            Logger.Log("Settings window is opened", Logger.LogTypes.Info);
            SetToolTipForKeys(); // Update tooltips based on the new MIDI channel
        }

        /// <summary>
        /// Closes all open windows and resets related user interface options to their default state.
        /// </summary>
        /// <remarks>This method unchecks all relevant option checkboxes to ensure that no auxiliary
        /// windows or features remain active. Call this method when a complete reset of the user interface state is
        /// required, such as during application shutdown or when switching modes.</remarks>
        private void CloseAllOpenWindows()
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
        private void RefreshMainWindowElementsColors(object sender, EventArgs e)
        {
            RefreshMainWindow();
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
        /// <summary>
        /// Adds a new note to the notes list based on the selected note length and current line data.
        /// </summary>
        /// <remarks>If a note is selected in the list, the new note is inserted at the selected position;
        /// otherwise, it is added to the end of the list. The method updates the modified state and refreshes the form
        /// title to reflect changes.</remarks>
        private void AddNote()
        {
            if (comboBox_note_length.SelectedIndex == 5)
            {
                Line.length = Resources.ThirtySecondNote;
            }
            if (comboBox_note_length.SelectedIndex == 4)
            {
                Line.length = Resources.SixteenthNote;
            }
            if (comboBox_note_length.SelectedIndex == 3)
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

        /// <summary>
        /// Adds the specified note to the selected note field of the current line if note addition is enabled.
        /// </summary>
        /// <remarks>The note is assigned to one of four possible note fields (note1, note2, note3, or
        /// note4) based on which option is selected. All other note fields are cleared. This method has no effect if
        /// note addition is not enabled.</remarks>
        /// <param name="note">The note text to add to the line. If null or empty, the note fields will be cleared.</param>
        private void AddNotesToList(string note)
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
                AddNote();
            }
        }

        /// <summary>
        /// Selects the next line in the notes list, or adds a new note if no item is currently selected.
        /// </summary>
        /// <remarks>If the last item in the list is selected, calling this method will deselect it. If no
        /// items are selected, the specified note will be added to the list and selected.</remarks>
        /// <param name="note">The note to add to the list if no item is currently selected. Cannot be null.</param>
        private void SelectNextLine(string note)
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
                AddNotesToList(note);
            }
        }

        /// <summary>
        /// Replaces the length of the selected note in the list with the value specified by the note length selection.
        /// </summary>
        /// <remarks>This method updates the note length only if the replace length option is enabled and
        /// at least one note is selected. The change is performed using a command pattern, which supports undo and redo
        /// operations if implemented by the command manager. The form's modified state and title are also updated to
        /// reflect the change.</remarks>
        private void ReplaceLengthOfLine()
        {
            if (checkBox_replace_length.Checked == true && listViewNotes.SelectedItems.Count > 0)
            {
                if (comboBox_note_length.SelectedIndex == 5)
                {
                    Line.length = Resources.ThirtySecondNote;
                }
                if (comboBox_note_length.SelectedIndex == 4)
                {
                    Line.length = Resources.SixteenthNote;
                }
                if (comboBox_note_length.SelectedIndex == 3)
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

        /// <summary>
        /// Updates the length of the selected note based on the current selection in the note length combo box, without
        /// saving the change to the undo history.
        /// </summary>
        /// <remarks>This method only updates the note length if the replace length option is checked and
        /// at least one note is selected. The change is not recorded in the application's memento or undo system, so it
        /// cannot be undone using standard undo operations.</remarks>
        private void ReplaceLengthWithoutSavingToMemento()
        {
            if (checkBox_replace_length.Checked == true && listViewNotes.SelectedItems.Count > 0)
            {
                if (comboBox_note_length.SelectedIndex == 5)
                {
                    Line.length = Resources.ThirtySecondNote;
                }
                if (comboBox_note_length.SelectedIndex == 4)
                {
                    Line.length = Resources.SixteenthNote;
                }
                if (comboBox_note_length.SelectedIndex == 3)
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
            }
        }

        /// <summary>
        /// Replaces the note and updates the length information for the selected line in the notes list, based on the
        /// specified note text and user selection.
        /// </summary>
        /// <remarks>This method only performs an update if a note is selected in the list and the option
        /// to add a note is enabled. The specific note field (note1, note2, note3, or note4) that is updated depends on
        /// which corresponding option is checked. After updating, the method marks the data as modified and updates the
        /// form title to reflect the change.</remarks>
        /// <param name="note">The text to set as the new note for the selected line. If multiple note fields are available, the field to
        /// update is determined by the user's selection.</param>
        private void ReplaceNoteAndLengthOfLine(string note)
        {
            if (checkBox_add_note_to_list.Checked == true && listViewNotes.SelectedItems.Count > 0)
            {
                if (add_as_note1.Checked == true)
                {
                    Line.note1 = note;
                    ReplaceLengthWithoutSavingToMemento();
                    var replaceNoteCommand = new ReplaceNoteAndLengthCommand(listViewNotes, listViewNotes.SelectedItems[0].SubItems[0].Text, 1, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                if (add_as_note2.Checked == true)
                {
                    Line.note2 = note;
                    ReplaceLengthWithoutSavingToMemento();
                    var replaceNoteCommand = new ReplaceNoteAndLengthCommand(listViewNotes, listViewNotes.SelectedItems[0].SubItems[0].Text, 2, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                if (add_as_note3.Checked == true)
                {
                    Line.note3 = note;
                    ReplaceLengthWithoutSavingToMemento();
                    var replaceNoteCommand = new ReplaceNoteAndLengthCommand(listViewNotes, listViewNotes.SelectedItems[0].SubItems[0].Text, 3, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                if (add_as_note4.Checked == true)
                {
                    Line.note4 = note;
                    ReplaceLengthWithoutSavingToMemento();
                    var replaceNoteCommand = new ReplaceNoteAndLengthCommand(listViewNotes, listViewNotes.SelectedItems[0].SubItems[0].Text, 4, note);
                    commandManager.ExecuteCommand(replaceNoteCommand);
                }
                isModified = true;
                UpdateFormTitle();
            }
        }
        int noteFrequency;

        /// <summary>
        /// Plays a musical note corresponding to the specified frequency when a key is clicked. The note is played
        /// using both MIDI output (if enabled) and the internal note player.
        /// </summary>
        /// <remarks>If MIDI output is enabled in the application settings and a MIDI output device is
        /// available, the note is played through the selected MIDI device and channel. The note is also played using
        /// the application's internal sound system regardless of MIDI settings.</remarks>
        /// <param name="frequency">The frequency of the note to play, in hertz. Must be a positive integer representing the desired pitch.</param>
        private async void PlayNoteWhenAKeyIsClicked(int frequency)
        {
            if (MIDIIOUtils._midiOut != null && TemporarySettings.MIDIDevices.useMIDIoutput == true)
            {
                MIDIIOUtils.ChangeInstrument(MIDIIOUtils._midiOut, TemporarySettings.MIDIDevices.MIDIOutputInstrument,
                            TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel);
                await MIDIIOUtils.PlayMidiNote(MIDIIOUtils._midiOut, frequency, 100);
            }
            NotePlayer.PlayNote(frequency, 100);
        }

        /// <summary>
        /// Updates the display to reflect changes when a key is clicked, such as adding a new note to the list.
        /// </summary>
        /// <remarks>This method should be called in response to a key click event when the option to add
        /// a note to the list is enabled. The display is updated only if the associated checkbox is checked.</remarks>
        private async void UpdateDisplayWhenKeyIsClicked()
        {
            if (checkBox_add_note_to_list.Checked == true)
            {
                UpdateDisplays(listViewNotes.Items.Count - 1, true);
            }
        }

        /// <summary>
        /// Handles the logic for when a virtual piano key is clicked, updating the note list, display, and optionally
        /// playing the corresponding note sound.
        /// </summary>
        /// <remarks>If the 'Replace' option is enabled, the method replaces the current note in the list;
        /// otherwise, it adds the note to the end. If the 'Play Note' option is checked, the method plays the
        /// corresponding note sound. The method also updates the display to reflect the key click and logs the action
        /// for diagnostic purposes.</remarks>
        /// <param name="keyName">The name of the key that was clicked. This should follow the expected naming convention for identifying the
        /// note and octave (e.g., "btn_C4" or "btn_DS5").</param>
        private void TriggerKeyClick(string keyName)
        {
            try
            {
                string input = keyName;
                int firstUnderscore = input.IndexOf('_');
                string afterFirstUnderscore = firstUnderscore >= 0 ? input.Substring(firstUnderscore + 1) : string.Empty;
                string labelName = string.Empty;
                string noteName = string.Empty;
                int extractedOctave = int.Parse(Regex.Match(afterFirstUnderscore, @"\d+").Value);
                int currentOctave = Variables.octave;
                switch (extractedOctave)
                {
                    case 3:
                        currentOctave = Variables.octave - 1;
                        break;
                    case 4:
                        currentOctave = Variables.octave;
                        break;
                    case 5:
                        currentOctave = Variables.octave + 1;
                        break;
                }
                if (!keyName.Contains("s")) // If the key is not a sharp note
                {
                    labelName = "lbl_" + afterFirstUnderscore;
                    Label label = this.Controls.Find(labelName, true).FirstOrDefault() as Label;
                    string labelText = label != null ? label.Text : string.Empty;
                    noteName = labelText;
                }
                else // If the key is a sharp note
                {
                    noteName = afterFirstUnderscore[0].ToString().ToUpper() + "#" + currentOctave;
                }
                UpdateDisplayWhenKeyIsClicked();
                if (checkBox_replace.Checked == true)
                {
                    ReplaceNoteAndLengthOfLine(noteName);
                    SelectNextLine(noteName);
                }
                else
                {
                    AddNotesToList(noteName);
                }
                if (checkbox_play_note.Checked == true)
                {
                    int rawFrequency = (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.C); // Default to C if no match
                    switch (noteName)
                    {
                        case var n when n.StartsWith("C"):
                            rawFrequency = n.Contains("#") ? (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.CS) :
                                (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.C);
                            break;
                        case var n when n.StartsWith("D"):
                            rawFrequency = n.Contains("#") ? (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.DS) :
                                (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.D);
                            break;
                        case var n when n.StartsWith("E"):
                            rawFrequency = (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.E);
                            break;
                        case var n when n.StartsWith("F"):
                            rawFrequency = n.Contains("#") ? (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.FS) :
                                (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.F);
                            break;
                        case var n when n.StartsWith("G"):
                            rawFrequency = n.Contains("#") ? (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.GS) :
                                (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.G);
                            break;
                        case var n when n.StartsWith("A"):
                            rawFrequency = n.Contains("#") ? (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.AS) :
                                (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.A);
                            break;
                        case var n when n.StartsWith("B"):
                            rawFrequency = (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.B);
                            break;
                        default:
                            rawFrequency = (int)(NoteUtility.BaseNoteFrequencyIn4thOctave.C);
                            break;
                    }
                    noteFrequency = Convert.ToInt16(rawFrequency * (Math.Pow(2, (currentOctave - 4))));
                    PlayNoteWhenAKeyIsClicked(noteFrequency);
                }
                Logger.Log($"Key {noteName} is clicked", Logger.LogTypes.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error in triggering key click: {ex.Message}", Logger.LogTypes.Error);
            }
        }
        private void piano_keys_Click(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                TriggerKeyClick(control.Name);
            }
        }

        /// <summary>
        /// Converts a musical note name (such as "C4" or "A#3") to its corresponding MIDI note number.
        /// </summary>
        /// <remarks>The method expects note names in the format of a note letter (A–G), an optional sharp
        /// sign (#), followed by a single-digit octave number. The calculation assumes that MIDI note 0 corresponds to
        /// C-1. Returns -1 if the input is null, empty, incorrectly formatted, or represents an invalid note.</remarks>
        /// <param name="noteName">The note name to convert, consisting of a note letter (A–G), an optional sharp sign (#), and a single-digit
        /// octave number. For example, "C4" or "F#2".</param>
        /// <returns>The MIDI note number corresponding to the specified note name, or -1 if the input is invalid.</returns>
        private int NoteNameToMIDINumber(string noteName)
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

        /// <summary>
        /// Calculates the MIDI note number adjusted for the current octave setting.
        /// </summary>
        /// <remarks>The calculation uses the current octave value from the Variables.octave property. The
        /// result increases or decreases by 12 for each octave above or below 4.</remarks>
        /// <param name="rawNumber">The base MIDI note number to adjust. Typically represents the note number before octave transposition.</param>
        /// <returns>The MIDI note number after applying the octave adjustment.</returns>
        private int CalculateMIDINumber(int rawNumber)
        {
            int multiplier = Variables.octave - 4;
            return rawNumber + (multiplier * 12);
        }

        /// <summary>
        /// Plays one or more MIDI notes from the specified line in the notes list asynchronously.
        /// </summary>
        /// <remarks>This method requires that the MIDI output device is initialized. Notes are only
        /// played if their corresponding play flag is set to true and the note value is valid. The method executes
        /// asynchronously and does not block the calling thread.</remarks>
        /// <param name="index">The zero-based index of the line in the notes list from which to retrieve note values.</param>
        /// <param name="playNote1">true to play the first note in the line; otherwise, false.</param>
        /// <param name="playNote2">true to play the second note in the line; otherwise, false.</param>
        /// <param name="playNote3">true to play the third note in the line; otherwise, false.</param>
        /// <param name="playNote4">true to play the fourth note in the line; otherwise, false.</param>
        /// <param name="length">The duration, in milliseconds, for which each note should be played.</param>
        private async void PlayMidiNotesFromLineAsync(int index, bool playNote1, bool playNote2, bool playNote3, bool playNote4, int length)
        {

            String note1 = listViewNotes.Items[index].SubItems[1].Text;
            String note2 = listViewNotes.Items[index].SubItems[2].Text;
            String note3 = listViewNotes.Items[index].SubItems[3].Text;
            String note4 = listViewNotes.Items[index].SubItems[4].Text;

            int[] notes = {
            NoteNameToMIDINumber(note1),
            NoteNameToMIDINumber(note2),
            NoteNameToMIDINumber(note3),
            NoteNameToMIDINumber(note4)
        };

            if (MIDIIOUtils._midiOut != null) // Check if initialized
            {
                if (playNote1 && !string.IsNullOrWhiteSpace(note1) && notes[0] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[0], length);
                if (playNote2 && !string.IsNullOrWhiteSpace(note2) && notes[1] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[1], length);
                if (playNote3 && !string.IsNullOrWhiteSpace(note3) && notes[2] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[2], length);
                if (playNote4 && !string.IsNullOrWhiteSpace(note4) && notes[3] != -1) MIDIIOUtils.PlayMidiNoteAsync(notes[3], length);
            }
        }

        /// <summary>
        /// Deserializes an XML string into a NeoBleeperProjectFile object.
        /// </summary>
        /// <param name="xmlContent">A string containing the XML representation of a NeoBleeperProjectFile. Cannot be null or empty.</param>
        /// <returns>A NeoBleeperProjectFile object deserialized from the specified XML string.</returns>
        private static NBPMLFile.NeoBleeperProjectFile DeserializeXMLFromString(string xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPMLFile.NeoBleeperProjectFile));
            using (StringReader reader = new StringReader(xmlContent))
            {
                return (NBPMLFile.NeoBleeperProjectFile)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Initializes a new music project in the editor using the specified AI-generated music data and file name.
        /// Updates the user interface and project state to reflect the loaded music.
        /// </summary>
        /// <remarks>If required project settings are missing from the AI-generated data, default values
        /// are applied to ensure the project loads correctly. The method resets the current project, updates all
        /// relevant controls, and notifies the user upon successful creation. If an error occurs during loading, an
        /// error message is displayed and the failure is logged.</remarks>
        /// <param name="createdMusic">A string containing the serialized music project data generated by AI, in the expected XML format. Must not
        /// be null or empty.</param>
        /// <param name="createdFileName">The file name to assign to the new project. Used to set the default file name in the save dialog. Must not
        /// be null or empty.</param>
        private void CreateMusicWithAIResponse(string createdMusic, string createdFileName)
        {
            ClearOpenedProject(); // Clear current project data
            saveFileDialog.FileName = createdFileName;
            lbl_measure_value.Text = "1";
            lbl_beat_value.Text = "0.0";
            lbl_beat_traditional_value.Text = "1";
            lbl_beat_traditional_value.ForeColor = Color.Green;
            string firstLine = createdMusic.First().ToString().Trim();
            try
            {
                saveToolStripMenuItem.Enabled = true;
                NBPMLFile.NeoBleeperProjectFile projectFile = DeserializeXMLFromString(createdMusic);
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
                    Variables.timeSignature = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                    trackBar_time_signature.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                    lbl_time_signature.Text = projectFile.Settings.RandomSettings.TimeSignature;
                    Variables.noteSilenceRatio = Convert.ToDouble(Convert.ToDouble(Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio)) / 100);
                    trackBar_note_silence_ratio.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                    lbl_note_silence_ratio.Text = Resources.TextPercent.Replace("{number}", projectFile.Settings.RandomSettings.NoteSilenceRatio);
                    comboBox_note_length.SelectedIndex = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteLength);
                    Variables.alternatingNoteLength = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
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
                        Variables.timeSignature = 4;
                        lbl_time_signature.Text = "4";
                        Logger.Log("Time signature not found, defaulting to 4", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note silence ratio is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                    {
                        Variables.noteSilenceRatio = 0.5; // Default value
                        lbl_note_silence_ratio.Text = "50%";
                        Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                    }
                    // Assign default values if no note length is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteLength))
                    {
                        comboBox_note_length.SelectedIndex = 3; // Default value
                        Logger.Log("Note length not found, defaulting to 1/8", Logger.LogTypes.Info);
                    }
                    // Assign default values if no alternating note length is found
                    if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.AlternateTime))
                    {
                        Variables.alternatingNoteLength = 30; // Default value
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
                        checkBox_play_note4_clicked.Checked = true; // Default value
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
                    UpdateNoteLabels();
                    if (Variables.octave == 9)
                    {
                        ShiftNoteLabelsToRightForOctave9();
                    }
                    this.Text = System.AppDomain.CurrentDomain.FriendlyName + " - " + Resources.TextAIGeneratedMusic;
                    listViewNotes.Items.Clear();

                    foreach (var line in projectFile.LineList.Lines)
                    {
                        ListViewItem item = new ListViewItem(ConvertNoteLengthIntoLocalized(line.Length));
                        item.SubItems.Add(line.Note1);
                        item.SubItems.Add(line.Note2);
                        item.SubItems.Add(line.Note3);
                        item.SubItems.Add(line.Note4);
                        item.SubItems.Add(ConvertModifiersIntoLocalized(line.Mod));
                        item.SubItems.Add(ConvertArticulationsIntoLocalized(line.Art));
                        listViewNotes.Items.Add(item);
                    }
                    isModified = false;
                    UpdateFormTitle();
                }
                saveAsToolStripMenuItem.Enabled = false;
                initialMemento = originator.CreateMemento(); // Save the initial state
                commandManager.ClearHistory(); // Reset the history
                Logger.Log("File is successfully created by AI", Logger.LogTypes.Info);
                NotificationUtils.CreateAndShowNotificationIfObscured(this, Resources.NotificationTitleAIMusicCreated, Resources.NotificationMessageAIMusicCreated, ToolTipIcon.Info, 3000); // Show notification if the window is obscured
            }
            catch (Exception ex)
            {
                Logger.Log($"AI music creation failed: {ex.Message}", Logger.LogTypes.Error);
                MessageForm.Show(this, Resources.MessageAIMusicCreationFailed + " " + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Converts a note length identifier to its localized display string.
        /// </summary>
        /// <param name="noteLength">The note length identifier to convert. Supported values include "Whole", "Half", "Quarter", "1/8", "1/16",
        /// and "1/32".</param>
        /// <returns>A localized string representing the specified note length. Returns the localized string for a whole note if
        /// the identifier is not recognized.</returns>
        private string ConvertNoteLengthIntoLocalized(string noteLength)
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

        /// <summary>
        /// Converts a modifier code into its corresponding localized string representation.
        /// </summary>
        /// <remarks>Use this method to obtain a user-friendly, localized description for known modifier
        /// codes. If the modifier is not recognized, the method returns an empty string.</remarks>
        /// <param name="modifier">The modifier code to convert. Supported values are "Dot" and "Tri".</param>
        /// <returns>A localized string representing the modifier if the code is recognized; otherwise, an empty string.</returns>
        private string ConvertModifiersIntoLocalized(string modifier)
        {
            switch (modifier)
            {
                case "Dot":
                    return Resources.DottedModifier;
                case "Tri":
                    return Resources.TripletModifier;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Converts an articulation code to its corresponding localized display string.
        /// </summary>
        /// <remarks>Use this method to obtain a user-friendly, localized name for a given articulation
        /// code. If the code is not recognized, the method returns an empty string.</remarks>
        /// <param name="articulation">The articulation code to convert. Supported values are "Sta", "Spi", and "Fer".</param>
        /// <returns>A localized string representing the articulation if the code is recognized; otherwise, an empty string.</returns>
        private string ConvertArticulationsIntoLocalized(string articulation)
        {
            switch (articulation)
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

        /// <summary>
        /// Parses and loads a music project file, initializing the application's state based on the file's contents.
        /// </summary>
        /// <remarks>This method supports both legacy Bleeper Music Maker and modern NeoBleeper project
        /// file formats. The application's settings and note list are updated to reflect the contents of the loaded
        /// file. If required data is missing from the file, default values are applied. If the file is invalid or
        /// corrupted, an error message is displayed and the project is not loaded. The method also updates the recent
        /// files list and resets the undo history upon successful loading.</remarks>
        /// <param name="filename">The path to the music project file to open. Must refer to a valid file in a supported format.</param>
        private void FileParser(string filename)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            ClearOpenedProject(); // Clear current project data
            saveFileDialog.FileName = string.Empty;
            lbl_measure_value.Text = "1";
            lbl_beat_value.Text = "0.0";
            lbl_beat_traditional_value.Text = "1";
            lbl_beat_traditional_value.ForeColor = Color.Green;
            string firstLine = File.ReadLines(filename).First().Trim();
            switch (firstLine)
            {
                case "Bleeper Music Maker by Robbi-985 file format": // Legacy Bleeper Music Maker file format by Robbi-985
                    {
                        try
                        {
                            isFileValid = true;
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
                                            Variables.timeSignature = Convert.ToInt32(parts[1]);
                                            lbl_time_signature.Text = parts[1].ToString();
                                            break;
                                        case "NoteSilenceRatio":
                                            Variables.noteSilenceRatio = Convert.ToDouble(Convert.ToDouble(parts[1]) / 100);
                                            trackBar_note_silence_ratio.Value = Convert.ToInt32(parts[1]);
                                            lbl_note_silence_ratio.Text = Resources.TextPercent.Replace("{number}", parts[1].ToString());
                                            break;
                                        case "NoteLength":
                                            comboBox_note_length.SelectedIndex = Convert.ToInt32(parts[1]);
                                            break;
                                        case "AlternateTime":
                                            Variables.alternatingNoteLength = Convert.ToInt32(parts[1]);
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
                                Variables.timeSignature = 4;
                                lbl_time_signature.Text = "4";
                                Logger.Log("Time signature not found, defaulting to 4", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note silence ratio is found
                            if (!lines.Any(line => line.StartsWith("NoteSilenceRatio")))
                            {
                                Variables.noteSilenceRatio = 0.5; // Default value
                                lbl_note_silence_ratio.Text = "50%";
                                Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                            }
                            // Assign default values if no note length is found
                            if (!lines.Any(line => line.StartsWith("NoteLength")))
                            {
                                comboBox_note_length.SelectedIndex = 3; // Default value
                                Logger.Log("Note length not found, defaulting to 1/8", Logger.LogTypes.Info);
                            }
                            // Assign default values if no alternating note length is found
                            if (!lines.Any(line => line.StartsWith("AlternateTime")))
                            {
                                Variables.alternatingNoteLength = 30; // Default value
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
                            UpdateNoteLabels();
                            if (Variables.octave == 9)
                            {
                                ShiftNoteLabelsToRightForOctave9();
                            }
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

                                    ListViewItem item = new ListViewItem(ConvertNoteLengthIntoLocalized(noteData[0])); // Note length
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
                                            item.SubItems.Add(ConvertModifiersIntoLocalized(noteData[3]));
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
                            DialogResult dialogResult = MessageForm.Show(this, Resources.MessageNonStandardBleeperMusicMakerFile, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult != DialogResult.Yes)
                            {
                                Logger.Log("User chose not to open the file", Logger.LogTypes.Info);
                                ClearOpenedProject(); // Clear current project data
                            }
                            else
                            {
                                Logger.Log("User chose to open the file anyway", Logger.LogTypes.Info);
                            }
                        }
                        finally
                        {
                            isFileValid = true;
                            currentFilePath = filename;
                        }
                        break;
                    }
                case "<NeoBleeperProjectFile>": // Modern NeoBleeper Project File format
                    {
                        try
                        {
                            isFileValid = true;
                            saveToolStripMenuItem.Enabled = true;
                            saveAsToolStripMenuItem.Enabled = true;
                            NBPMLFile.NeoBleeperProjectFile projectFile = DeserializeXML(filename); if (projectFile != null)
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
                                Variables.timeSignature = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                                trackBar_time_signature.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.TimeSignature);
                                lbl_time_signature.Text = projectFile.Settings.RandomSettings.TimeSignature;
                                Variables.noteSilenceRatio = Convert.ToDouble(Convert.ToDouble(Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio)) / 100);
                                trackBar_note_silence_ratio.Value = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                                lbl_note_silence_ratio.Text = Resources.TextPercent.Replace("{number}", projectFile.Settings.RandomSettings.NoteSilenceRatio);
                                comboBox_note_length.SelectedIndex = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteLength);
                                Variables.alternatingNoteLength = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
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
                                    Variables.timeSignature = 4;
                                    lbl_time_signature.Text = "4";
                                    Logger.Log("Time signature not found, defaulting to 4", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note silence ratio is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                                {
                                    Variables.noteSilenceRatio = 0.5; // Default value
                                    lbl_note_silence_ratio.Text = "50%";
                                    Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                                }
                                // Assign default values if no note length is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.NoteLength))
                                {
                                    comboBox_note_length.SelectedIndex = 3; // Default value
                                    Logger.Log("Note length not found, defaulting to 1/8", Logger.LogTypes.Info);
                                }
                                // Assign default values if no alternating note length is found
                                if (string.IsNullOrWhiteSpace(projectFile.Settings.RandomSettings.AlternateTime))
                                {
                                    Variables.alternatingNoteLength = 30; // Default value
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
                                    checkBox_play_note4_clicked.Checked = true; // Default value
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
                                UpdateNoteLabels();
                                if (Variables.octave == 9)
                                {
                                    ShiftNoteLabelsToRightForOctave9();
                                }
                                listViewNotes.Items.Clear();

                                if (projectFile.LineList?.Lines != null && projectFile.LineList.Lines.Length > 0)
                                {
                                    foreach (var line in projectFile.LineList.Lines)
                                    {
                                        ListViewItem item = new ListViewItem(ConvertNoteLengthIntoLocalized(line.Length));
                                        item.SubItems.Add(line.Note1);
                                        item.SubItems.Add(line.Note2);
                                        item.SubItems.Add(line.Note3);
                                        item.SubItems.Add(line.Note4);
                                        item.SubItems.Add(ConvertModifiersIntoLocalized(line.Mod));
                                        item.SubItems.Add(ConvertArticulationsIntoLocalized(line.Art));
                                        listViewNotes.Items.Add(item);
                                    }
                                }
                                // Leave empty if no lines are found
                            }
                            Logger.Log("NeoBleeper file opened successfully", Logger.LogTypes.Info);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Error opening NeoBleeper file: " + ex.Message, Logger.LogTypes.Error);
                            DialogResult dialogResult = MessageForm.Show(this, Resources.MessageNonStandardNeoBleeperFile, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult != DialogResult.Yes)
                            {
                                Logger.Log("User chose not to open the file", Logger.LogTypes.Info);
                                ClearOpenedProject();
                            }
                            else
                            {
                                isModified = false;
                                UpdateFormTitle();
                                Logger.Log("User chose to open the file anyway", Logger.LogTypes.Info);
                            }
                        }
                        finally
                        {
                            isFileValid = true;
                            currentFilePath = filename;
                        }
                        break;
                    }
                default:
                    {
                        isFileValid = false;
                        Logger.Log("Invalid or corrupted music file", Logger.LogTypes.Error);
                        MessageForm.Show(this, Resources.MessageInvalidOrCorruptedMusicFile, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
            }
            initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternatingNoteLength,
    Variables.noteSilenceRatio, Variables.timeSignature); // Save the initial state
            commandManager.ClearHistory(); // Reset the history
                                           // Add the file to the recent files list
            if (isFileValid == true)
            {
                initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternatingNoteLength,
    Variables.noteSilenceRatio, Variables.timeSignature);
                isModified = false;
                UpdateFormTitle();
                AddFileToRecentFilesMenu(filename);
            }
        }

        /// <summary>
        /// Adds the specified file to the recent files menu if it is not already present.
        /// </summary>
        /// <remarks>If the file is successfully added, the recent files list is updated and persisted.
        /// Duplicate entries are not added.</remarks>
        /// <param name="fileName">The full path of the file to add to the recent files menu. Cannot be null or empty.</param>
        private void AddFileToRecentFilesMenu(string fileName)
        {
            if (Settings1.Default.RecentFiles == null)
            {
                Settings1.Default.RecentFiles = new System.Collections.Specialized.StringCollection();
            }

            if (!Settings1.Default.RecentFiles.Contains(fileName))
            {
                Settings1.Default.RecentFiles.Add(fileName);
                Settings1.Default.Save();
            }
            UpdateRecentFilesMenu();
        }

        /// <summary>
        /// Deserializes an XML file into a NeoBleeperProjectFile object.
        /// </summary>
        /// <param name="filePath">The path to the XML file to deserialize. Cannot be null or empty.</param>
        /// <returns>A NeoBleeperProjectFile object representing the data contained in the specified XML file.</returns>
        public static NBPMLFile.NeoBleeperProjectFile DeserializeXML(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPMLFile.NeoBleeperProjectFile));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (NBPMLFile.NeoBleeperProjectFile)serializer.Deserialize(reader);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopPlaying();
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            CloseAllOpenWindows();
            AskForSavingIfModified(new Action(() => OpenAFileFromDialog()));
        }

        /// <summary>
        /// Displays a file open dialog to allow the user to select a project file and opens the selected file if
        /// confirmed.
        /// </summary>
        /// <remarks>The dialog uses the last opened project file name as the initial file name and
        /// applies a filter for supported project file formats. If the user cancels the dialog, no file is
        /// opened.</remarks>
        private void OpenAFileFromDialog()
        {
            openFileDialog.Title = Resources.TitleOpenProjectFile;
            openFileDialog.Filter = Resources.FilterProjectFileFormats;
            openFileDialog.FileName = lastOpenedProjectFileName;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                lastOpenedProjectFileName = System.IO.Path.GetFileName(filePath);
                FileParser(filePath);
            }
        }
        bool isSaved = false;
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveTheFile();
        }

        /// <summary>
        /// Saves the current file to the existing file path if it is set and has a valid ".NBPML" extension; otherwise,
        /// prompts the user to specify a file location.
        /// </summary>
        /// <remarks>If the file is successfully saved, the current state is recorded for undo or state
        /// management purposes, and the form title is updated to reflect the saved status. If the file path is not set
        /// or does not have a ".NBPML" extension, a Save As dialog is displayed to allow the user to choose a location
        /// and file name.</remarks>
        private void SaveTheFile()
        {
            if (!string.IsNullOrWhiteSpace(currentFilePath) && currentFilePath.ToUpper().EndsWith(".NBPML"))
            {
                try
                {
                    isSaved = false;
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
                        Convert.ToInt32(numericUpDown_alternating_notes.Value),
                        Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100,
                        Convert.ToInt32(trackBar_time_signature.Value));
                    isModified = false;
                    UpdateFormTitle();
                    isSaved = true;
                }
                catch
                {
                    isSaved = false;
                }
            }
            else
            {
                OpenSaveAsDialog(); // Open Save As dialog if no file path is set or if the file is not a NBPML file
            }
        }

        /// <summary>
        /// Displays a Save As dialog box, allowing the user to specify a file name and location for saving the current
        /// document.
        /// </summary>
        /// <remarks>If the user confirms the dialog, the current document is saved to the selected file,
        /// and the application's state is updated accordingly. All sounds are stopped and open windows are closed
        /// before displaying the dialog. The recent files list and form title are updated after a successful save. If
        /// the save operation fails, the document is not marked as saved.</remarks>
        private void OpenSaveAsDialog()
        {
            isSaved = false;
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            CloseAllOpenWindows(); // Close all open windows before opening the Save As dialog
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    isSaved = true;
                    SaveToNBPML(saveFileDialog.FileName);
                    AddFileToRecentFilesMenu(saveFileDialog.FileName);
                    if (saveAsToolStripMenuItem.Enabled == false)
                    {
                        saveAsToolStripMenuItem.Enabled = true;
                    }
                    currentFilePath = saveFileDialog.FileName;
                    this.Text = System.AppDomain.CurrentDomain.FriendlyName + " - " + currentFilePath;
                    isModified = false;
                    saveFileDialog.FileName = string.Empty;
                    UpdateFormTitle();
                    initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternatingNoteLength,
    Variables.noteSilenceRatio, Variables.timeSignature);
                }
                catch
                {
                    isSaved = false; // If saving failed, set isSaved to false
                }
            }
        }

        /// <summary>
        /// Saves the current project data to a NeoBleeper Project Markup Language (NBPML) file at the specified path.
        /// </summary>
        /// <remarks>If a file already exists at the specified path, it will be overwritten. After saving,
        /// the current file path and form title are updated. An error message is displayed if the save operation
        /// fails.</remarks>
        /// <param name="filename">The full file path where the NBPML project file will be saved. Must not be null or empty.</param>
        private void SaveToNBPML(string filename)
        {
            try
            {
                NBPMLFile.NeoBleeperProjectFile projectFile = new NBPMLFile.NeoBleeperProjectFile
                {
                    Settings = new NBPMLFile.Settings
                    {
                        RandomSettings = new NBPMLFile.RandomSettings
                        {
                            KeyboardOctave = Variables.octave.ToString(),
                            BPM = Variables.bpm.ToString(),
                            TimeSignature = trackBar_time_signature.Value.ToString(),
                            NoteSilenceRatio = (Variables.noteSilenceRatio * 100).ToString(),
                            NoteLength = comboBox_note_length.SelectedIndex.ToString(),
                            AlternateTime = numericUpDown_alternating_notes.Value.ToString()
                        },
                        PlaybackSettings = new NBPMLFile.PlaybackSettings
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
                        PlayNotes = new NBPMLFile.PlayNotes
                        {
                            PlayNote1 = checkBox_play_note1_played.Checked.ToString(),
                            PlayNote2 = checkBox_play_note2_played.Checked.ToString(),
                            PlayNote3 = checkBox_play_note3_played.Checked.ToString(),
                            PlayNote4 = checkBox_play_note4_played.Checked.ToString()
                        },
                        ClickPlayNotes = new NBPMLFile.ClickPlayNotes
                        {
                            ClickPlayNote1 = checkBox_play_note1_clicked.Checked.ToString(),
                            ClickPlayNote2 = checkBox_play_note2_clicked.Checked.ToString(),
                            ClickPlayNote3 = checkBox_play_note3_clicked.Checked.ToString(),
                            ClickPlayNote4 = checkBox_play_note4_clicked.Checked.ToString()
                        }
                    },
                    LineList = new NBPMLFile.List
                    {
                        Lines = listViewNotes.Items.Cast<ListViewItem>().Select(item => new NBPMLFile.Line
                        {
                            Length = ConvertLocalizedNoteLengthIntoUnlocalized(item.SubItems[0].Text),
                            Note1 = item.SubItems[1].Text,
                            Note2 = item.SubItems[2].Text,
                            Note3 = item.SubItems[3].Text,
                            Note4 = item.SubItems[4].Text,
                            Mod = ConvertLocalizedModifiersIntoUnlocalized(item.SubItems[5].Text),
                            Art = ConvertLocalizedArticulationsIntoUnlocalized(item.SubItems[6].Text)
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
                MessageForm.Show(this, Resources.MessageErrorSavingFile + " " + ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trackBar_note_silence_ratio_Scroll(object sender, EventArgs e)
        {
            double oldValue = Variables.noteSilenceRatio;
            Variables.noteSilenceRatio = (Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100);
            string percentText = Resources.TextPercent;
            percentText = percentText.Replace("{number}", trackBar_note_silence_ratio.Value.ToString());
            lbl_note_silence_ratio.Text = percentText;
            if (!variableIsChanging)
            {
                double newValue = Variables.noteSilenceRatio;
                if (newValue != oldValue)
                {
                    var command = new ValueChangeCommand(
                            "note_silence_ratio",
                            oldValue,
                            newValue,
                            trackBar_note_silence_ratio,
                            true,
                            lbl_note_silence_ratio);

                    commandManager.ExecuteCommand(command);
                    isModified = true;
                    UpdateFormTitle();
                }
            }
            Logger.Log($"Note silence ratio is set to {trackBar_note_silence_ratio.Value}%", Logger.LogTypes.Info);
        }

        private void trackBar_time_signature_Scroll(object sender, EventArgs e)
        {
            int oldValue = Variables.timeSignature;
            Variables.timeSignature = trackBar_time_signature.Value;
            lbl_time_signature.Text = trackBar_time_signature.Value.ToString();
            if (!variableIsChanging)
            {
                int newValue = Variables.timeSignature;
                if (newValue != oldValue)
                {
                    var command = new ValueChangeCommand(
                            "time_signature",
                            oldValue,
                            newValue,
                            trackBar_time_signature,
                            lbl_time_signature);

                    commandManager.ExecuteCommand(command);
                    isModified = true;
                    UpdateFormTitle();
                }
            }
            Logger.Log($"Time signature is set to {trackBar_time_signature.Value}", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Updates the text of note labels to reflect the current octave setting.
        /// </summary>
        /// <remarks>This method should be called whenever the octave value changes to ensure that the
        /// displayed note labels correspond to the correct octave range.</remarks>
        private void UpdateNoteLabels()
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

        /// <summary>
        /// Shifts the note label controls for octave 9 to the right to accommodate display scaling.
        /// </summary>
        /// <remarks>This method adjusts the horizontal position of the C5, D5, E5, F5, G5, A5, and B5
        /// label controls based on the current display DPI. Call this method when rendering octave 9 to ensure proper
        /// alignment of note labels on high-DPI displays.</remarks>
        private void ShiftNoteLabelsToRightForOctave9()
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

        /// <summary>
        /// Shifts the note label controls for octave 9 to the left to adjust their horizontal alignment on the form.
        /// </summary>
        /// <remarks>This method recalculates the X-coordinate of each note label based on the current
        /// display DPI to ensure consistent positioning across different screen resolutions. It is intended for
        /// internal use when rendering note labels for octave 9.</remarks>
        private void ShiftNoteLabelsToLeftForOctave9()
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
        private bool isOctaveChanging = false; // Flag for debouncing

        private async void btn_octave_decrease_Click(object sender, EventArgs e)
        {
            if (isOctaveChanging) return; // Don't allow multiple clicks
            isOctaveChanging = true;

            if (Variables.octave > 2) // Minimum limit check
            {
                Variables.octave--;
                UpdateNoteLabels();
                if (Variables.octave == 8)
                {
                    ShiftNoteLabelsToRightForOctave9(); // Shift labels to the right
                }
                if (Variables.octave == 2)
                {
                    btn_octave_decrease.Enabled = false; // Disable button at minimum limit
                }
                btn_octave_increase.Enabled = true; // Enable the other button
            }
            SetToolTipForKeys(); // Update tooltips for keys
            isOctaveChanging = false; // The operation is complete
        }

        private async void btn_octave_increase_Click(object sender, EventArgs e)
        {
            if (isOctaveChanging) return; // Don't allow multiple clicks
            isOctaveChanging = true;

            if (Variables.octave < 9) // Maximum limit check
            {
                Variables.octave++;
                UpdateNoteLabels();

                if (Variables.octave == 9)
                {
                    btn_octave_increase.Enabled = false; // Disable button at maximum limit
                    ShiftNoteLabelsToLeftForOctave9(); // Shift labels to the left
                }
                btn_octave_decrease.Enabled = true; // Enable the other button
            }
            SetToolTipForKeys(); // Update tooltips for keys

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

        /// <summary>
        /// Removes all checked items from the notes list if any are checked; otherwise, removes the currently selected
        /// item.
        /// </summary>
        /// <remarks>After removal, the method updates the selection to the next available item and marks
        /// the notes as modified. This method should be called when the user requests to erase lines from the notes
        /// list, such as via a delete action.</remarks>
        private void EraseLine()
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
            EraseLine();
        }

        private void newFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskForSavingIfModified(new Action(() => CreateNewFile()));
        }

        /// <summary>
        /// Stops all currently playing sounds, including music, notes, voices, and any sounds produced by a connected
        /// microcontroller.
        /// </summary>
        /// <remarks>This method resets the playback state and ensures that no sound continues to play
        /// from any source managed by the system. If a microcontroller is in use, its sound output is also stopped.
        /// This method is asynchronous but returns void; exceptions thrown during asynchronous operations may not be
        /// observed. Use with caution if you need to track completion or handle errors.</remarks>
        public async void StopPlayingAllSounds()
        {
            if (isMusicPlaying == true)
            {
                StopPlaying();
            }
            keyPressed = false;
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

        /// <summary>
        /// Resets the application state to its default values, clearing the currently opened project and restoring all
        /// controls and settings to their initial state.
        /// </summary>
        /// <remarks>Call this method to discard any unsaved changes and prepare the application for a new
        /// project or a clean start. This method stops all currently playing sounds, clears the note list, resets user
        /// interface controls, and reinitializes relevant project parameters. Any unsaved data will be lost after
        /// calling this method.</remarks>
        private void ClearOpenedProject()
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            this.Text = System.AppDomain.CurrentDomain.FriendlyName;
            saveFileDialog.FileName = string.Empty;
            currentFilePath = String.Empty;
            if (Variables.octave == 9)
            {
                ShiftNoteLabelsToRightForOctave9();
            }
            Variables.octave = 4;
            Variables.bpm = 140;
            Variables.alternatingNoteLength = 30;
            Variables.noteSilenceRatio = 0.5;
            Variables.timeSignature = 4;
            lbl_measure_value.Text = "1";
            lbl_beat_value.Text = "0.0";
            lbl_beat_traditional_value.Text = "1";
            lbl_beat_traditional_value.ForeColor = Color.Green;
            saveAsToolStripMenuItem.Enabled = false;
            UpdateNoteLabels();
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
            lbl_note_silence_ratio.Text = Resources.TextPercent.Replace("{number}", "50");
            trackBar_note_silence_ratio.Value = 50;
            comboBox_note_length.SelectedIndex = 3;
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
            initialMemento = originator.CreateMemento();
            commandManager.ClearHistory(); // Reset the history
            isModified = false;
            UpdateFormTitle();
        }

        /// <summary>
        /// Creates a new file by clearing the currently opened project and resetting the application state.
        /// </summary>
        /// <remarks>This method should be called when starting a new project to ensure that any existing
        /// project data is discarded. It also logs the creation of the new file for auditing purposes.</remarks>
        private void CreateNewFile()
        {
            ClearOpenedProject(); // Clear the current project
            Logger.Log("New file created", Logger.LogTypes.Info); // Log the creation of a new file
        }

        /// <summary>
        /// Calculates the durations of the note sound and the following silence based on the specified base note
        /// length.
        /// </summary>
        /// <param name="baseLength">The base length of the note, typically representing its duration in beats or time units. Must be a positive
        /// value.</param>
        /// <returns>A tuple containing the duration of the note sound and the duration of the silence, both as integers. The
        /// first item is the note sound duration; the second item is the silence duration. Both values are in the same
        /// units as the input.</returns>
        private (int noteSound_int, int silence_int) CalculateNoteDurations(double baseLength)
        {
            // Compute raw double values
            double noteSound_double = FixRoundingErrors(CalculateNoteLength(baseLength));
            double totalRhythm_double = FixRoundingErrors(CalculateLineLength(baseLength));

            // Convert to integers
            int totalRhythm_int = (int)Math.Truncate(totalRhythm_double);
            int noteSound_int = Math.Min((int)Math.Truncate(noteSound_double), totalRhythm_int);
            int silence_int = totalRhythm_int - noteSound_int;

            return (noteSound_int, silence_int);
        }

        /// <summary>
        /// Adjusts the specified floating-point value to reduce the impact of minor rounding errors near zero.
        /// </summary>
        /// <remarks>This method is useful when small floating-point inaccuracies could affect subsequent
        /// calculations or comparisons, particularly for values close to zero. The adjustment is only applied if the
        /// absolute value of the input exceeds a small threshold.</remarks>
        /// <param name="inputValue">The double-precision floating-point value to be corrected for potential rounding errors.</param>
        /// <returns>A double value with minor rounding errors adjusted. The returned value may be slightly increased or
        /// decreased if it is sufficiently far from zero; otherwise, it is returned unchanged.</returns>
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

        /// <summary>
        /// Handles MIDI output by playing the selected MIDI notes with the specified note duration, if MIDI output is
        /// enabled and a note is selected.
        /// </summary>
        /// <param name="noteSoundDuration">The duration, in milliseconds, for which each MIDI note should be played.</param>
        private void HandleMidiOutput(int noteSoundDuration)
        {
            if (TemporarySettings.MIDIDevices.useMIDIoutput && listViewNotes.SelectedIndices.Count > 0)
            {
                PlayMidiNotesFromLineAsync(
                    listViewNotes.SelectedIndices[0],
                    checkBox_play_note1_played.Checked,
                    checkBox_play_note2_played.Checked,
                    checkBox_play_note3_played.Checked,
                    checkBox_play_note4_played.Checked,
                    noteSoundDuration);
            }
        }

        /// <summary>
        /// Handles the playback of standard notes for the selected line, using either the voice system or standard
        /// playback based on the current settings.
        /// </summary>
        /// <remarks>Playback behavior depends on the current selection and user settings. If no notes are
        /// selected, this method performs no action.</remarks>
        /// <param name="noteSoundDuration">The duration, in milliseconds, for which each note sound should be played.</param>
        /// <param name="rawNoteDuration">The original duration, in milliseconds, of the note before any processing or adjustments.</param>
        /// <param name="nonStopping">true to play notes without stopping ongoing playback; otherwise, false. The default is false.</param>
        /// <returns>A task that represents the asynchronous operation of playing the notes.</returns>
        private async Task HandleStandardNotePlayback(int noteSoundDuration, int rawNoteDuration, bool nonStopping = false)
        {
            if (listViewNotes.SelectedIndices.Count > 0)
            {
                if (checkBox_use_voice_system.Checked)
                {
                    await PlayNotesOfLineWithVoice(
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
                    await PlayNotesOfLine(
                        checkBox_play_note1_played.Checked,
                        checkBox_play_note2_played.Checked,
                        checkBox_play_note3_played.Checked,
                        checkBox_play_note4_played.Checked,
                        noteSoundDuration,
                        nonStopping);
                }
            }
        }

        /// <summary>
        /// Determines whether the first selected note in the list is checked.
        /// </summary>
        /// <remarks>This method is typically used to check the state of the currently selected note in a
        /// list view. If no notes are selected, the method returns false.</remarks>
        /// <returns>true if at least one note is selected and the first selected note is checked; otherwise, false.</returns>
        private bool IsSelectedNoteChecked()
        {
            return listViewNotes.SelectedIndices.Count > 0 && listViewNotes.SelectedItems[0].Checked;
        }

        /// <summary>
        /// Determines whether playing voice on checked lines is enabled in the current voice settings.
        /// </summary>
        /// <returns>true if playing voice on checked lines is enabled; otherwise, false.</returns>
        private bool IsPlayVoiceOnCheckedLineEnabled()
        {
            return TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions == TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnCheckedLines;
        }

        /// <summary>
        /// Plays the specified notes of a line using both the voice system and the system speaker, according to the
        /// current output device settings.
        /// </summary>
        /// <remarks>The notes are routed to either the voice system or the system speaker based on the
        /// current output device settings for each note. The method plays the notes concurrently on both systems as
        /// appropriate. The actual behavior may depend on user or application settings such as selected notes and
        /// output device configuration.</remarks>
        /// <param name="playNote1">true to play the first note in the line; otherwise, false.</param>
        /// <param name="playNote2">true to play the second note in the line; otherwise, false.</param>
        /// <param name="playNote3">true to play the third note in the line; otherwise, false.</param>
        /// <param name="playNote4">true to play the fourth note in the line; otherwise, false.</param>
        /// <param name="length">The duration, in ticks or milliseconds depending on system configuration, for which the voice system should
        /// play the notes.</param>
        /// <param name="rawLength">The duration, in ticks or milliseconds depending on system configuration, for which the system speaker
        /// should play the notes.</param>
        /// <param name="nonStopping">true to prevent stopping currently playing notes before starting new ones; otherwise, false. Optional.</param>
        /// <returns>A task that represents the asynchronous operation of playing the notes.</returns>
        private async Task PlayNotesOfLineWithVoice(bool playNote1, bool playNote2, bool playNote3, bool playNote4, int length, int rawLength, bool nonStopping = false) // Play note with voice in a line
        {
            // System speaker notes
            bool systemSpeakerNote1 = (TemporarySettings.VoiceInternalSettings.note1OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && playNote1;
            bool systemSpeakerNote2 = (TemporarySettings.VoiceInternalSettings.note2OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && playNote2;
            bool systemSpeakerNote3 = (TemporarySettings.VoiceInternalSettings.note3OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && playNote3;
            bool systemSpeakerNote4 = (TemporarySettings.VoiceInternalSettings.note4OutputDeviceIndex == 1 || (!IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled())) && playNote4;

            // Voice system notes
            bool voiceSystemNote1 = TemporarySettings.VoiceInternalSettings.note1OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && playNote1;
            bool voiceSystemNote2 = TemporarySettings.VoiceInternalSettings.note2OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && playNote2;
            bool voiceSystemNote3 = TemporarySettings.VoiceInternalSettings.note3OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && playNote3;
            bool voiceSystemNote4 = TemporarySettings.VoiceInternalSettings.note4OutputDeviceIndex == 0 && ((IsSelectedNoteChecked() && IsPlayVoiceOnCheckedLineEnabled()) || !IsPlayVoiceOnCheckedLineEnabled()) && playNote4;

            // Play voice
            StartVoice(voiceSystemNote1, voiceSystemNote2, voiceSystemNote3, voiceSystemNote4, length, nonStopping);

            // Play system speaker notes
            await PlayNotesOfLine(
                 systemSpeakerNote1,
                 systemSpeakerNote2,
                 systemSpeakerNote3,
                 systemSpeakerNote4,
                 rawLength,
                 nonStopping
                );
        }

        /// <summary>
        /// Starts playback of up to four selected musical notes using the voice synthesis engine.
        /// </summary>
        /// <remarks>This method plays notes based on the currently selected item in the notes list. Only
        /// notes corresponding to parameters set to true will be played. If no item is selected, no notes will be
        /// played.</remarks>
        /// <param name="playNote1">true to play the first note from the selected line; otherwise, false.</param>
        /// <param name="playNote2">true to play the second note from the selected line; otherwise, false.</param>
        /// <param name="playNote3">true to play the third note from the selected line; otherwise, false.</param>
        /// <param name="playNote4">true to play the fourth note from the selected line; otherwise, false.</param>
        /// <param name="length">The duration, in milliseconds, for which the notes should be played.</param>
        /// <param name="nonStopping">true to prevent stopping currently playing voices before starting new ones; otherwise, false. The default is
        /// false.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task StartVoice(bool playNote1, bool playNote2, bool playNote3, bool playNote4, int length, bool nonStopping = false)
        {
            string note1 = string.Empty, note2 = string.Empty, note3 = string.Empty, note4 = string.Empty;
            double note1Frequency = 0, note2Frequency = 0, note3Frequency = 0, note4Frequency = 0;
            String[] notes = new string[4];
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selected_line = listViewNotes.SelectedIndices[0];

                // Take music note names from the selected line
                note1 = playNote1 ? listViewNotes.Items[selected_line].SubItems[1].Text : string.Empty;
                note2 = playNote2 ? listViewNotes.Items[selected_line].SubItems[2].Text : string.Empty;
                note3 = playNote3 ? listViewNotes.Items[selected_line].SubItems[3].Text : string.Empty;
                note4 = playNote4 ? listViewNotes.Items[selected_line].SubItems[4].Text : string.Empty;
                // Calculate frequencies from note names
                await StopSelectedVoicesThatEmpty(note1, note2, note3, note4);
                if (!string.IsNullOrWhiteSpace(note1))
                {
                    note1Frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(0, (int)note1Frequency);
                }
                if (!string.IsNullOrWhiteSpace(note2))
                {
                    note2Frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(1, (int)note2Frequency);
                }
                if (!string.IsNullOrWhiteSpace(note3))
                {
                    note3Frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(2, (int)note3Frequency);
                }
                if (!string.IsNullOrWhiteSpace(note4))
                {
                    note4Frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
                    SoundRenderingEngine.VoiceSynthesisEngine.StartVoice(3, (int)note4Frequency);
                }
            }
        }

        /// <summary>
        /// Stops all active voice synthesis operations asynchronously.
        /// </summary>
        /// <remarks>This method stops all voices currently managed by the voice synthesis engine. The
        /// operation is performed asynchronously and may take some time to complete, depending on the number of active
        /// voices.</remarks>
        /// <returns>A task that represents the asynchronous stop operation.</returns>
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

        /// <summary>
        /// Stops the voice synthesis for any of the four selected voices whose corresponding note parameter is null,
        /// empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="note1">The note assigned to the first voice. If null, empty, or white space, the first voice will be stopped.</param>
        /// <param name="note2">The note assigned to the second voice. If null, empty, or white space, the second voice will be stopped.</param>
        /// <param name="note3">The note assigned to the third voice. If null, empty, or white space, the third voice will be stopped.</param>
        /// <param name="note4">The note assigned to the fourth voice. If null, empty, or white space, the fourth voice will be stopped.</param>
        /// <returns>A task that represents the asynchronous stop operation.</returns>
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

        /// <summary>
        /// Returns the fractional part of a double-precision floating-point number by removing its whole number
        /// component.
        /// </summary>
        /// <remarks>The sign of the result matches the sign of the input number. For negative numbers,
        /// the fractional part is negative or zero.</remarks>
        /// <param name="number">The double-precision floating-point number from which to extract the fractional part.</param>
        /// <returns>A double-precision floating-point number representing the fractional part of the input. Returns 0 if the
        /// input is an integer value.</returns>
        public static double RemoveWholeNumber(double number)
        {
            return number - Math.Truncate(number);
        }

        /// <summary>
        /// Plays the sequence of musical notes starting from the specified index in the note list, handling timing,
        /// looping, and playback controls asynchronously.
        /// </summary>
        /// <remarks>Playback timing is managed to maintain accurate note durations, compensating for
        /// drift. If looping is enabled, playback will restart from the specified index upon reaching the end of the
        /// list. Playback can be stopped by user action or when the end of the list is reached and looping is disabled.
        /// This method disables certain UI controls during playback and restores them when playback
        /// completes.</remarks>
        /// <param name="startIndex">The zero-based index of the note in the list from which playback should begin. Must be within the bounds of
        /// the note list.</param>
        private async void PlayMusic(int startIndex)
        {
            bool nonStopping = false;
            int currentNoteIndex = startIndex;
            int baseLength = 0;

            EnableDisableCommonControls(false);
            try
            {
                // Use a single, high-resolution timer for the entire playback duration
                await HighPrecisionSleep.SleepAsync(1); // Sleep briefly to ensure accurate timing
                Stopwatch globalStopwatch = new Stopwatch();
                globalStopwatch.Start();

                // Store the total elapsed time of all previous notes
                double totalElapsedNoteDuration = 0.0;

                while (listViewNotes.SelectedItems.Count > 0 && isMusicPlaying)
                {
                    if (Variables.bpm > 0)
                    {
                        baseLength = Math.Max(1, (int)(60000.0 / (double)Variables.bpm));
                    }

                    nonStopping = trackBar_note_silence_ratio.Value == 100;
                    var (noteSound_int, silence_int) = CalculateNoteDurations(baseLength);
                    double noteDuration = CalculateLineLength(baseLength); // + beat_length;
                    int rawNoteDuration = (int)Math.Truncate(FixRoundingErrors(CalculateRawNoteLength(baseLength))); // Note length calculator without note-silence ratio

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
                                    StopPlaying();
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
                                    StopPlaying();
                                    listViewNotes.SelectedItems.Clear();
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
                            StopPlaying();
                            listViewNotes.SelectedItems.Clear();
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
                    StopAllNotesAfterPlaying();
                }
                await StopAllVoices();
            }
            finally
            {
                EnableDisableCommonControls(true);
            }
        }

        // ListView update method

        /// <summary>
        /// Updates the selection in the ListView to the item at the specified index.
        /// </summary>
        /// <remarks>If the specified index is valid, the method clears any existing selection and selects
        /// the item at the given index. The method is thread-safe and can be called from any thread; if called from a
        /// non-UI thread, the selection update is marshaled to the UI thread. If the index is out of range, no
        /// selection is made.</remarks>
        /// <param name="index">The zero-based index of the item to select. Must be greater than or equal to 0 and less than the total
        /// number of items in the ListView.</param>
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

        /// <summary>
        /// Ensures that the item at the specified index in the list view is visible to the user.
        /// </summary>
        /// <remarks>If called from a thread other than the UI thread, this method marshals the request to
        /// the UI thread to ensure thread safety.</remarks>
        /// <param name="index">The zero-based index of the item to make visible in the list view. Must be within the valid range of item
        /// indices.</param>
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

        /// <summary>
        /// Plays all notes in the list from the beginning, unless the keyboard is currently being used as a piano.
        /// </summary>
        /// <remarks>This method has no effect if the note list is empty or if the keyboard-as-piano mode
        /// is enabled. Playback starts from the first note in the list.</remarks>
        public void PlayAll()
        {
            if (listViewNotes.Items.Count > 0 && !checkBox_use_keyboard_as_piano.Checked) // Lock the play if using keyboard as piano
            {
                isMusicPlaying = true;
                listViewNotes.Items[0].Selected = true;
                EnsureSpecificIndexVisible(0);
                Logger.Log("Music is playing", Logger.LogTypes.Info);
                PlayMusic(0);
            }
        }

        /// <summary>
        /// Starts playback of the music sequence from the currently selected line in the notes list.
        /// </summary>
        /// <remarks>If no line is selected, playback starts from the first line. Playback is only
        /// initiated if there are notes in the list and the keyboard is not being used as a piano.</remarks>
        public void PlayFromSelectedLine()
        {
            if (listViewNotes.Items.Count > 0 && !checkBox_use_keyboard_as_piano.Checked) // Lock the play if using keyboard as piano
            {
                isMusicPlaying = true;
                if (listViewNotes.SelectedItems.Count < 1)
                {
                    listViewNotes.Items[0].Selected = true;
                    EnsureSpecificIndexVisible(0);
                    Logger.Log("Music is playing", Logger.LogTypes.Info);
                    PlayMusic(0);
                }
                else
                {
                    int index = listViewNotes.SelectedItems[0].Index;
                    EnsureSpecificIndexVisible(index);
                    Logger.Log("Music is playing", Logger.LogTypes.Info);
                    PlayMusic(index);
                }
            }
        }
        private void button_play_all_Click(object sender, EventArgs e)
        {
            PlayAll();
        }

        private void button_play_from_selected_line_Click(object sender, EventArgs e)
        {
            PlayFromSelectedLine();
        }
        private void button_stop_playing_Click(object sender, EventArgs e)
        {
            StopPlaying();
        }

        /// <summary>
        /// Enables or disables a set of common controls on the form based on the specified value.
        /// </summary>
        /// <remarks>This method ensures that control state changes are performed on the UI thread. When
        /// controls are enabled, the stop controls are disabled, and vice versa.</remarks>
        /// <param name="enable">true to enable the controls; false to disable them.</param>
        private void EnableDisableCommonControls(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => EnableDisableCommonControls(enable)));
                return;
            }
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

        /// <summary>
        /// Stops music playback and raises the MusicStopped event.
        /// </summary>
        /// <remarks>Call this method to halt any currently playing music. If no music is playing, this
        /// method has no effect. After stopping playback, the MusicStopped event is triggered to notify
        /// subscribers.</remarks>
        public void StopPlaying()
        {
            isMusicPlaying = false;
            Logger.Log("Music stopped", Logger.LogTypes.Info);
            OnMusicStopped(EventArgs.Empty);
        }
        private System.Timers.Timer metronomeTimer;
        private int beatCount = 0;
        private readonly object syncLock = new object();
        private volatile bool isLabelVisible = false;

        // Prepare sound buffers in advance
        protected virtual void OnMusicStopped(EventArgs e)
        {
            MusicStopped?.Invoke(this, e);
        }

        /// <summary>
        /// Plays a metronome sound using the configured MIDI output device.
        /// </summary>
        /// <remarks>The metronome sound is played only if a MIDI output device is available and MIDI
        /// output is enabled in the application settings. The frequency parameter is reserved for future use and does
        /// not affect the sound.</remarks>
        /// <param name="frequency">The frequency, in hertz, of the metronome sound. This parameter is currently not used.</param>
        /// <param name="length">The duration, in milliseconds, for which the metronome sound is played.</param>
        /// <param name="isAccent">A value indicating whether the metronome sound should be accented. Set to <see langword="true"/> to play an
        /// accented beat; otherwise, <see langword="false"/>.</param>
        private async void PlayMetronomeSoundFromMIDIOutput(int frequency, int length, bool isAccent)
        {
            if (MIDIIOUtils._midiOut != null && TemporarySettings.MIDIDevices.useMIDIoutput == true)
            {
                MIDIIOUtils.PlayMetronomeBeatOnMIDI(MIDIIOUtils._midiOut, isAccent, length);
            }
        }

        /// <summary>
        /// Initializes the metronome timer and configures its elapsed event handler.
        /// </summary>
        private void InitializeMetronome()
        {
            metronomeTimer = new System.Timers.Timer();
            metronomeTimer.Elapsed += MetronomeTimer_Elapsed;
        }

        private void MetronomeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                metronomeTimer.Stop(); // Temporarily stop to prevent overlapping
                if (!checkBox_metronome.Checked)
                    return;

                PlayMetronomeBeats();

                // Schedule the next beat
                metronomeTimer.Start();
            }
            catch (Exception ex)
            {
                Logger.Log("Metronome error: " + ex.Message, Logger.LogTypes.Error);
            }
        }

        /// <summary>
        /// Plays a metronome beat sound, using an accented or regular tone based on the specified parameter.
        /// </summary>
        /// <remarks>The metronome sound is played asynchronously on a high-priority thread to help ensure
        /// timing accuracy. This method is intended for internal use within timing-sensitive audio routines.</remarks>
        /// <param name="isAccent">true to play an accented beat; false to play a regular beat.</param>
        private void PlayMetronomeBeat(bool isAccent)
        {
            int frequency = isAccent ? 1000 : 500;

            // Important: Play sound on high-priority thread
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                PlayMetronomeSoundFromMIDIOutput(frequency, 15, isAccent);
                NotePlayer.PlayNote(frequency, 15);
            });
        }

        /// <summary>
        /// Displays the metronome beat label for a brief period to indicate the current beat visually.
        /// </summary>
        /// <remarks>If the label is already visible, this method does nothing. The label is shown for
        /// approximately 75 milliseconds before being hidden automatically. This method is intended to provide a visual
        /// cue synchronized with the metronome's beat.</remarks>
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

        /// <summary>
        /// Sets the visibility of the label that indicates a beep state on the UI thread.
        /// </summary>
        /// <remarks>If called from a non-UI thread, this method automatically marshals the call to the UI
        /// thread to ensure thread safety when updating the label's visibility.</remarks>
        /// <param name="visible">true to make the label visible; otherwise, false.</param>
        public void UpdateLabelVisible(bool visible)
        {
            try
            {
                if (label_beep.InvokeRequired)
                {
                    label_beep.Invoke(() =>
                    {
                        UpdateLabelVisible(visible);
                        return;
                    });
                }
                label_beep.SuspendLayout();
                label_beep.Visible = visible;
                label_beep.ResumeLayout(performLayout: true);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Starts the metronome and initializes the beat sequence based on the current tempo setting.
        /// </summary>
        /// <remarks>This method resets the internal beat count and configures the metronome timer
        /// interval according to the current beats per minute (BPM) value. It also plays an initial sound to indicate
        /// the start of the metronome. Call this method to begin metronome playback from the first beat.</remarks>
        private void StartMetronome()
        {
            beatCount = 0;
            double interval = Math.Max(1, 60000.0 / (double)Variables.bpm);
            metronomeTimer.Interval = interval;
            NotePlayer.PlayNote(500, 5);
            PlayMetronomeBeats();
            metronomeTimer.Start();
        }

        /// <summary>
        /// Advances the metronome by one beat, playing the appropriate sound and updating the user interface to reflect
        /// the current beat.
        /// </summary>
        /// <remarks>This method should be called once per metronome tick to ensure accurate audio and
        /// visual synchronization. It plays the metronome sound for the current beat and updates the beat display
        /// accordingly.</remarks>
        private void PlayMetronomeBeats()
        {
            // Play the appropriate sound first for minimal latency
            PlayMetronomeBeat(beatCount == 0);

            // Then update the UI (which is less time-critical)
            ShowMetronomeBeatLabel();

            // Update beat counter
            beatCount = (beatCount + 1) % Variables.timeSignature;
        }

        /// <summary>
        /// Stops the metronome if it is currently running.
        /// </summary>
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
                            if (SystemThemeUtility.IsDarkTheme() == true)
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
                            if (SystemThemeUtility.IsDarkTheme() == true)
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

        /// <summary>
        /// Adds a blank line to the current list or selection, applying replacement or length options as specified by
        /// the current settings.
        /// </summary>
        /// <remarks>The behavior of this method depends on the state of the associated checkboxes. If
        /// both replacement and length options are enabled, the method replaces the length of the current line before
        /// selecting the next line. If only the replacement option is enabled, it selects the next line without
        /// modifying its length. Otherwise, it simply adds a blank note to the list. This method also logs the action
        /// for informational purposes.</remarks>
        private void AddBlankLine()
        {
            if (checkBox_replace.Checked == true && checkBox_replace_length.Checked == true)
            {
                ReplaceLengthOfLine();
                SelectNextLine(String.Empty);

            }
            else if (checkBox_replace.Checked)
            {
                SelectNextLine(String.Empty);
            }
            else
            {
                AddNotesToList(String.Empty);
            }
            Logger.Log("Blank line added", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Clears the contents of the first note in the notes list and updates the application state accordingly.
        /// </summary>
        private void ClearNote1()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 1);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 1 cleared", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Clears the contents of the second note in the notes list and updates the application state accordingly.
        /// </summary>
        private void ClearNote2()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 2);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 2 cleared", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Clears the contents of the third note in the notes list and updates the application state accordingly.
        /// </summary>
        private void ClearNote3()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 3);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 3 cleared", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Clears the contents of the fourth note in the notes list and updates the application state accordingly.
        /// </summary>
        private void ClearNote4()
        {
            var clearNoteCommand = new ClearNoteCommand(listViewNotes, 4);
            commandManager.ExecuteCommand(clearNoteCommand);
            isModified = true;
            UpdateFormTitle();
            Logger.Log("Note 4 cleared", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Clears the selection from all selected items in the notes list view.
        /// </summary>
        /// <remarks>Call this method to ensure that no items remain selected in the list view. This can
        /// be useful before performing operations that require a clear selection state.</remarks>
        private void UnselectLine()
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
            AddBlankLine();
        }

        private void button_clear_note1_Click(object sender, EventArgs e)
        {
            ClearNote1();
        }

        private void button_clear_note2_Click(object sender, EventArgs e)
        {
            ClearNote2();
        }

        private void button_clear_note3_Click(object sender, EventArgs e)
        {
            ClearNote3();
        }

        private void button_clear_note4_Click(object sender, EventArgs e)
        {
            ClearNote4();
        }

        private void button_unselect_Click(object sender, EventArgs e)
        {
            UnselectLine();
        }
        bool rightClicked = false;
        private async void listViewNotes_Click(object sender, EventArgs e) // Stop music and play clicked note
        {
            if (rightClicked)
            {
                return; // Exit the method if it's a right-click
            }
            var mousePoint = listViewNotes.PointToClient(Control.MousePosition);
            var hit = listViewNotes.HitTest(mousePoint);
            if (hit.Location == ListViewHitTestLocations.StateImage)
            {
                return;
            }
            StopPlaying();
            EnableDisableCommonControls(false);
            if (listViewNotes.FocusedItem != null && listViewNotes.SelectedItems.Count > 0)
            {
                try
                {
                    int baseLength = 0;
                    Variables.alternatingNoteLength = Convert.ToInt32(numericUpDown_alternating_notes.Value);
                    if (Variables.bpm != 0)
                    {
                        baseLength = Math.Max(1, (int)(60000.0 / (double)Variables.bpm));
                    }
                    if (listViewNotes.SelectedItems.Count > 0)
                    {
                        UpdateDisplays(listViewNotes.SelectedIndices[0]);
                    }
                    HighPrecisionSleep.Sleep(1);
                    var (noteSound_int, silence_int) = CalculateNoteDurations(baseLength);
                    int rawNoteDuration = (int)Math.Truncate(FixRoundingErrors(CalculateRawNoteLength(baseLength))); // Note length calculator without note-silence ratio
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
                        StopAllNotesAfterPlaying();
                    }
                }
                finally
                {
                    EnableDisableCommonControls(true);
                }
                if (!(listViewNotes.SelectedItems == null) && listViewNotes.SelectedItems.Count > 0) // Multi-lingual compatible logging of selected line as English 
                {
                    string length = "Quarter"; // Default to quarter note
                    string modifier = string.Empty;
                    string articulation = string.Empty;
                    switch (listViewNotes.SelectedItems[0].SubItems[0].Text.ToString())
                    {
                        case var n when n == Resources.WholeNote:
                            length = "Whole";
                            break;
                        case var n when n == Resources.HalfNote:
                            length = "Half";
                            break;
                        case var n when n == Resources.QuarterNote:
                            length = "Quarter";
                            break;
                        case var n when n == Resources.EighthNote:
                            length = "1/8";
                            break;
                        case var n when n == Resources.SixteenthNote:
                            length = "1/16";
                            break;
                        case var n when n == Resources.ThirtySecondNote:
                            length = "1/32";
                            break;
                        default:
                            length = "Quarter";
                            break;
                    }
                    switch (listViewNotes.SelectedItems[0].SubItems[5].Text.ToString())
                    {
                        case var m when m == Resources.DottedModifier:
                            modifier = "Dotted";
                            break;
                        case var m when m == Resources.TripletModifier:
                            modifier = "Triplet";
                            break;
                        default:
                            modifier = string.Empty;
                            break;
                    }
                    switch (listViewNotes.SelectedItems[0].SubItems[6].Text.ToString())
                    {
                        case var a when a == Resources.StaccatoArticulation:
                            articulation = "Staccato";
                            break;
                        case var a when a == Resources.SpiccatoArticulation:
                            articulation = "Spiccato";
                            break;
                        case var a when a == Resources.FermataArticulation:
                            articulation = "Fermata";
                            break;
                        default:
                            articulation = string.Empty;
                            break;
                    }
                    Logger.Log($"Selected line: {listViewNotes.SelectedItems[0].Index} Length: " +
                     $"{length} Note 1: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[1].Text.ToString())}" +
                     $" Note 2: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[2].Text.ToString())} Note 3: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[3].Text.ToString())}" +
                     $" Note 4: {GetOrDefault(listViewNotes.SelectedItems[0].SubItems[4].Text.ToString())} Modifier: {GetOrDefault(modifier)}" +
                     $" Articulation: {GetOrDefault(articulation)}", Logger.LogTypes.Info);
                }

            }
        }

        /// <summary>
        /// Returns the specified string if it is not null, empty, or consists only of white-space characters;
        /// otherwise, returns a default value.
        /// </summary>
        /// <param name="value">The string to evaluate. Can be null, empty, or contain only white-space characters.</param>
        /// <returns>The original string if it contains non-white-space characters; otherwise, the string "None".</returns>
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
            int oldValue = Variables.alternatingNoteLength;
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

            Logger.Log($"Alternating note length: {Variables.alternatingNoteLength}", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Calculates the raw note length based on the specified base length and the currently selected note's
        /// properties.
        /// </summary>
        /// <remarks>The calculation takes into account the selected note's type, modifier, and
        /// articulation, as well as the current note-silence ratio. The result is always at least 1.</remarks>
        /// <param name="baseLength">The base length of the note, typically representing the unmodified duration before applying note-specific
        /// modifiers and articulations.</param>
        /// <returns>The calculated raw note length as a double. Returns 0 if no note is selected or if the note list is empty.</returns>
        private double CalculateRawNoteLength(double baseLength)
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
            double result = GetNoteLength(baseLength, noteType);
            result = GetModifiedNoteLength(result, modifier);
            result = result * articulationFactor;

            // Only round at the very end when converting to integer milliseconds
            return Math.Max(1, result);
        }

        /// <summary>
        /// Calculates the effective note length based on the specified base length and the current note-silence ratio
        /// settings.
        /// </summary>
        /// <remarks>The result is always at least 1. The calculation takes into account the current
        /// selection and the value of the note-silence ratio control. If no notes are selected or available, the method
        /// returns 0.</remarks>
        /// <param name="baseLength">The base length of the note, typically in milliseconds, before applying the note-silence ratio. Must be
        /// greater than zero.</param>
        /// <returns>The calculated note length after applying the note-silence ratio. Returns 0 if no notes are selected or
        /// available.</returns>
        private double CalculateNoteLength(double baseLength)
        {
            if (listViewNotes.SelectedItems == null || listViewNotes.SelectedItems.Count == 0 ||
                listViewNotes.Items == null || listViewNotes.Items.Count == 0)
            {
                return 0;
            }

            // Note-silence ratio (from trackBar)
            double silenceRatio = (double)trackBar_note_silence_ratio.Value / 100.0;

            // Calculate the total note length - use precise calculations without truncation
            double result = CalculateRawNoteLength(baseLength);
            result = result * silenceRatio;

            // Only round at the very end when converting to integer milliseconds
            return Math.Max(1, result);
        }

        /// <summary>
        /// Calculates the duration, in milliseconds, of the currently selected line in the notes list, taking into
        /// account note type, modifiers, and articulation.
        /// </summary>
        /// <remarks>A fermata articulation doubles the calculated line length. Staccato and spiccato
        /// articulations do not affect the duration.</remarks>
        /// <param name="quarterNoteMs">The duration of a quarter note, in milliseconds, used as the base for calculating the line length.</param>
        /// <returns>The calculated line length in milliseconds. Returns 0 if no line is selected or the notes list is empty. The
        /// value is always at least 1 millisecond.</returns>
        private double CalculateLineLength(double quarterNoteMs)
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
            double result = GetNoteLength(quarterNoteMs, noteType);
            result = GetModifiedNoteLength(result, modifier);
            result = result * articulationFactor;

            // Should be at least 1 ms
            return Math.Max(1, result);
        }

        /// <summary>
        /// Calculates the duration of a musical note based on its type and a base note length.
        /// </summary>
        /// <remarks>This method uses localized resource strings to identify note types. If an
        /// unrecognized note name is provided, the method defaults to treating it as a quarter note.</remarks>
        /// <param name="rawNoteLength">The base duration value representing a quarter note length, typically in beats or seconds.</param>
        /// <param name="lengthName">The name of the note length to calculate (for example, whole, half, quarter, eighth, sixteenth, or
        /// thirty-second note). Must match a supported note name from the localized resources.</param>
        /// <returns>The calculated duration of the specified note type. If the note type is not recognized, returns the duration
        /// for a quarter note.</returns>
        public static double GetNoteLength(double rawNoteLength, String lengthName)
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

        /// <summary>
        /// Calculates the effective note length after applying a musical modifier such as dotted or triplet.
        /// </summary>
        /// <remarks>A dotted modifier increases the note length by 50 percent, while a triplet modifier
        /// reduces it to one third of the original length. The method does not validate the format of the modifier
        /// string beyond checking for the presence of known modifier keywords.</remarks>
        /// <param name="noteLength">The original length of the note, typically expressed in beats or fractions of a whole note. Must be a
        /// non-negative value.</param>
        /// <param name="modifier">A string representing the note modifier to apply. Supported values include dotted and triplet modifiers. The
        /// comparison is case-insensitive. If null, empty, or unrecognized, no modification is applied.</param>
        /// <returns>The modified note length after applying the specified modifier. Returns the original note length if the
        /// modifier is null, empty, or unrecognized.</returns>
        public static double GetModifiedNoteLength(double noteLength, String modifier)
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

        /// <summary>
        /// Stops all currently playing notes and voices, and updates the user interface to reflect that playback has
        /// ended.
        /// </summary>
        /// <remarks>If microcontroller integration is enabled, this method also stops any sound output
        /// from the connected microcontroller. This method is intended to be called after playback is complete to
        /// ensure all audio sources are silenced and the UI is updated accordingly.</remarks>
        private async void StopAllNotesAfterPlaying()
        {
            NotePlayer.StopAllNotes();
            await StopAllVoices(); // Stop all voices if using voice system
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller)
            {
                NotePlayer.StopMicrocontrollerSound();
            }
            UpdateLabelVisible(false);
        }

        /// <summary>
        /// Plays the selected notes from a line in the note list according to the specified parameters and duration.
        /// </summary>
        /// <remarks>If multiple notes are selected, they are played in sequence within the specified
        /// duration. The method respects the current alternating note mode and skips empty or duplicate notes. If no
        /// notes are selected, the method waits for the specified duration without playing any sound.</remarks>
        /// <param name="playNote1">true to play the first note in the selected line; otherwise, false.</param>
        /// <param name="playNote2">true to play the second note in the selected line; otherwise, false.</param>
        /// <param name="playNote3">true to play the third note in the selected line; otherwise, false.</param>
        /// <param name="playNote4">true to play the fourth note in the selected line; otherwise, false.</param>
        /// <param name="length">The total duration, in milliseconds, for which the notes should be played. Must be greater than zero.</param>
        /// <param name="nonStopping">true to prevent stopping other notes after playback; otherwise, false. The default is false.</param>
        /// <returns>A task that represents the asynchronous operation of playing the specified notes.</returns>
        private async Task PlayNotesOfLine(bool playNote1, bool playNote2, bool playNote3, bool playNote4, int length, bool nonStopping = false) // Play note in a line
        {
            Variables.alternatingNoteLength = Convert.ToInt32(numericUpDown_alternating_notes.Value);
            string note1 = string.Empty, note2 = string.Empty, note3 = string.Empty, note4 = string.Empty;
            double note1Frequency = 0, note2Frequency = 0, note3Frequency = 0, note4Frequency = 0;
            String[] notes = new string[4];
            if (listViewNotes.SelectedItems.Count > 0)
            {
                int selectedLine = listViewNotes.SelectedIndices[0];

                // Take music note names from the selected line
                note1 = playNote1 ? listViewNotes.Items[selectedLine].SubItems[1].Text : string.Empty;
                note2 = playNote2 ? listViewNotes.Items[selectedLine].SubItems[2].Text : string.Empty;
                note3 = playNote3 ? listViewNotes.Items[selectedLine].SubItems[3].Text : string.Empty;
                note4 = playNote4 ? listViewNotes.Items[selectedLine].SubItems[4].Text : string.Empty;
                // Calculate frequencies from note names
                if (!string.IsNullOrWhiteSpace(note1))
                    note1Frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);

                if (!string.IsNullOrWhiteSpace(note2))
                    note2Frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);

                if (!string.IsNullOrWhiteSpace(note3))
                    note3Frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);

                if (!string.IsNullOrWhiteSpace(note4))
                    note4Frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
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
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note1Frequency), length, nonStopping);
                }
                else if (notes[0].Contains(note2) && !string.IsNullOrWhiteSpace(note2))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note2Frequency), length, nonStopping);
                }
                else if (notes[0].Contains(note3) && !string.IsNullOrWhiteSpace(note3))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note3Frequency), length, nonStopping);
                }
                else if (notes[0].Contains(note4) && (!string.IsNullOrWhiteSpace(note4)))
                {
                    await PlayBeepWithLabelAsync(Convert.ToInt32(note4Frequency), length, nonStopping);
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
                            await Task.Run(() => { NotePlayer.PlayNote(Convert.ToInt32(frequency), alternatingNoteDuration); });
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
                                await Task.Run(() => { NotePlayer.PlayNote(Convert.ToInt32(frequency), (int)remainingTime); });
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
                if (!isMusicPlaying)
                {
                    EnableDisableCommonControls(false);
                }
                if (nonStopping == true)
                {
                    StopAllNotesAfterPlaying();
                }
                if (!isMusicPlaying)
                {
                    EnableDisableCommonControls(true);
                }
                await HighPrecisionSleep.SleepAsync(Math.Max(1, length));
            }
        }

        /// <summary>
        /// Plays a beep sound at the specified frequency and duration, displaying a label while the sound is playing.
        /// </summary>
        /// <param name="frequency">The frequency of the beep sound, in hertz. Must be a positive integer.</param>
        /// <param name="length">The duration of the beep sound, in milliseconds. Must be a positive integer.</param>
        /// <param name="nonStopping">true to keep the label visible after the beep finishes; otherwise, false to hide the label when the beep
        /// completes. The default is false.</param>
        private void PlayBeepWithLabel(int frequency, int length, bool nonStopping = false)
        {
            UpdateLabelVisible(true);
            NotePlayer.PlayNote(frequency, length, nonStopping);
            if (!nonStopping)
            {
                UpdateLabelVisible(false);
            }
        }

        /// <summary>
        /// Determines whether the specified double-precision floating-point value represents a whole number.
        /// </summary>
        /// <remarks>This method returns true for both positive and negative whole numbers, including
        /// zero. It does not consider the sign of zero or special floating-point values such as NaN or infinity as
        /// whole numbers.</remarks>
        /// <param name="value">The double-precision floating-point value to evaluate.</param>
        /// <returns>true if the value is a whole number; otherwise, false.</returns>
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
                if (checkBox_do_not_update.Checked == false && isMusicPlaying == true)
                {
                    UpdateDisplays(selectedLine);
                }
            }
        }
        int beatLength = 0; // Length of the beat sound in milliseconds for adding corrected note length to prevent irregularities

        /// <summary>
        /// Updates the measure and beat display values based on the specified line index, and optionally plays a beat
        /// sound depending on user interaction and settings.
        /// </summary>
        /// <remarks>This method updates UI elements to reflect the current musical position and may play
        /// a beat sound depending on the application's beat playback settings. If the notes list is empty, no updates
        /// are performed.</remarks>
        /// <param name="Line">The zero-based index of the line in the notes list for which to update the display values.</param>
        /// <param name="clicked">Indicates whether the update was triggered by a user click. If set to <see langword="true"/>, the beat sound
        /// will not be played.</param>
        private void UpdateDisplays(int Line, bool clicked = false)
        {
            if (listViewNotes.Items.Count > 0)
            {
                int measure = 1;
                double beat = 0;
                double beatNumber = 1;
                if (listViewNotes.SelectedItems.Count > 0)
                {
                    for (int i = 1; i <= Line; i++)
                    {
                        beat += Convert.ToDouble(NoteLengthToBeats(listViewNotes.Items[i]));
                        if (beat >= Variables.timeSignature)
                        {
                            measure++;
                            beat = 0;

                        }
                    }
                }
                beatNumber = beat + 1;
                Task.Run(() =>
                {
                    position_table.SuspendLayout();
                    lbl_measure_value.Text = measure.ToString();
                    lbl_beat_value.Text = Math.Round(beatNumber, 4).ToString();
                    lbl_beat_traditional_value.Text = ConvertDecimalBeatToTraditional(beat);
                    lbl_beat_traditional_value.ForeColor = SetTraditionalBeatColor(lbl_beat_traditional_value.Text);
                    position_table.ResumeLayout();
                });
                if (checkBox_play_beat_sound.Checked == true && clicked == false)
                {
                    switch (TemporarySettings.BeatTypes.beatType)
                    {
                        case TemporarySettings.BeatTypes.BeatType.PlayOnAllBeats:
                            if (IsWholeNumber(beatNumber))
                            {
                                beatLength = PlayBeatSound();
                            }
                            break;
                        case TemporarySettings.BeatTypes.BeatType.PlayOnOddBeats:
                            if (beatNumber % 2 != 0 && IsWholeNumber(beatNumber))
                            {
                                beatLength = PlayBeatSound();
                            }
                            else
                            {
                                beatLength = 0; // Reset beat length if not playing on odd beats
                            }
                            break;
                        case TemporarySettings.BeatTypes.BeatType.PlayOnEvenBeats:
                            if (beatNumber % 2 == 0 && IsWholeNumber(beatNumber))
                            {
                                beatLength = PlayBeatSound();
                            }
                            else
                            {
                                beatLength = 0; // Reset beat length if not playing on odd beats
                            }
                            break;
                        case TemporarySettings.BeatTypes.BeatType.PlayOnCheckedLines:
                            if (listViewNotes.Items[Line].Checked == true)
                            {
                                beatLength = PlayBeatSound();
                            }
                            else
                            {
                                beatLength = 0; // Reset beat length if not playing on checked lines
                            }
                            break;
                    }
                }
                else
                {
                    beatLength = 0; // Reset beat length if not playing beat sound
                }
            }
        }

        /// <summary>
        /// Plays a short percussion beat using system audio and, if enabled, MIDI output.
        /// </summary>
        /// <remarks>The beat consists of a kick, snare, and hi-hat pattern. If MIDI output is enabled,
        /// the beat is also played on the configured MIDI device. The duration of the beat is determined by the current
        /// BPM setting.</remarks>
        /// <returns>The total duration of the beat sound, in milliseconds.</returns>
        private int PlayBeatSound()
        {
            // The original developer thought this "techno beat" was a masterpiece.
            // Spoiler alert: It's just a glorified system speaker sound. ;P

            // Basic frequencies
            int snareFrequency = Convert.ToInt32(NoteFrequencies.GetFrequencyFromNoteName("D2"));
            int kickFrequency = Convert.ToInt32(NoteFrequencies.GetFrequencyFromNoteName("E2"));
            int hiHatFrequency = Convert.ToInt32(NoteFrequencies.GetFrequencyFromNoteName("F#2"));

            // Calculate length based on BPM
            double calculatedLengthFactor = 0.1; // Factor to adjust the length of the sound
            int milisecondsPerWholeNote = 0;
            if (Variables.bpm != 0)
            {
                milisecondsPerWholeNote = (int)Math.Truncate(240000.0 / Variables.bpm);
            }

            int length = Math.Max(1, (int)Math.Truncate((milisecondsPerWholeNote / 15.0) * calculatedLengthFactor));

            // Create a percussion sound
            for (int i = 0; i < 2; i++) // 2 beats
            {
                if (i % 2 == 0) // Kick 
                {
                    NotePlayer.PlayNote(kickFrequency, length, true);
                }
                else // Snare
                {
                    NotePlayer.PlayNote(snareFrequency, length, true);
                }
            }
            // Add hi-hat sound
            NotePlayer.PlayNote(hiHatFrequency, length / 2, true);

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

        /// <summary>
        /// Converts a decimal beat value to its traditional fractional beat notation as a string.
        /// </summary>
        /// <remarks>The returned string uses common fractional beat formats (such as "1/2", "1/4", etc.)
        /// and may include a whole number if the value exceeds one beat. If the decimal value cannot be precisely
        /// represented as a standard fraction, an error message is included in the result.</remarks>
        /// <param name="decimalBeat">The beat value expressed as a decimal, where 4 represents a whole beat.</param>
        /// <returns>A string representing the traditional fractional notation of the beat. Returns an error message if the value
        /// cannot be accurately converted.</returns>
        private string ConvertDecimalBeatToTraditional(double decimalBeat)
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

        /// <summary>
        /// Calculates the greatest common divisor (GCD) of two integers using the Euclidean algorithm.
        /// </summary>
        /// <remarks>The result is always non-negative, regardless of the sign of the input
        /// values.</remarks>
        /// <param name="a">The first integer value. Can be positive, negative, or zero.</param>
        /// <param name="b">The second integer value. Can be positive, negative, or zero.</param>
        /// <returns>The greatest common divisor of the two specified integers. If both values are zero, returns zero.</returns>
        private int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// Determines the color to represent the beat status based on the specified text.
        /// </summary>
        /// <param name="text">The text to evaluate for determining the beat status color.</param>
        /// <returns>A Color value representing the beat status. Returns Color.Red if the text indicates an error; otherwise,
        /// returns Color.Green.</returns>
        private Color SetTraditionalBeatColor(string text)
        {
            Color textColor;
            if (text.Contains(Resources.TextBeatError))
            {
                textColor = Color.Red;
            }
            else
            {
                textColor = Color.Green;
            }
            return textColor;
        }

        /// <summary>
        /// Calculates the note duration in beats based on the specified ListViewItem's note type, modifier, and
        /// articulation.
        /// </summary>
        /// <remarks>The method interprets the ListViewItem's subitems according to expected musical
        /// notation conventions. If a subitem does not match a known value, default values are used (quarter note for
        /// note type, no modifier, and no articulation).</remarks>
        /// <param name="listViewItem">The ListViewItem containing subitems that specify the note type, modifier (such as dotted or triplet), and
        /// articulation (such as staccato or fermata). The subitems are expected to follow a specific order
        /// corresponding to these attributes.</param>
        /// <returns>A double representing the calculated duration of the note in beats, adjusted for any modifiers and
        /// articulations.</returns>
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

        /// <summary>
        /// Formats a double-precision floating-point number as a string with up to four decimal places, omitting
        /// unnecessary trailing zeros.
        /// </summary>
        /// <remarks>The formatted string uses a period (".") as the decimal separator, regardless of the
        /// current culture. This method is useful for generating culture-invariant numeric strings for display or
        /// serialization.</remarks>
        /// <param name="number">The number to format as a string.</param>
        /// <returns>A string representation of the number with up to four decimal places. If the number is an integer, the
        /// result includes one decimal place (e.g., "42.0").</returns>
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

        /// <summary>
        /// Opens the Synchronized Play window and associates it with the current instance.
        /// </summary>
        /// <remarks>This method creates a new Synchronized Play window, displays it to the user, and
        /// stores a reference to the window in the Tag property of the synchronized play checkbox. This allows for
        /// later retrieval or management of the window instance.</remarks>
        private void OpenSynchronizedPlayWindow()
        {
            SynchronizedPlayWindow syncPlayWindow = new SynchronizedPlayWindow(this);
            syncPlayWindow.Show();
            checkBox_synchronized_play.Tag = syncPlayWindow;
        }

        /// <summary>
        /// Closes the currently associated synchronized play window, if one is present.
        /// </summary>
        /// <remarks>This method attempts to close the window referenced by the Tag property of the
        /// checkBox_synchronized_play control. If no synchronized play window is associated, this method has no
        /// effect.</remarks>
        private void CloseSynchronizedPlayWindow()
        {
            SynchronizedPlayWindow syncPlayWindow = checkBox_synchronized_play.Tag as SynchronizedPlayWindow;
            syncPlayWindow.Close();
        }
        private void checkBox_synchronized_play_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_synchronized_play.Checked == true)
            {
                OpenSynchronizedPlayWindow();
                Logger.Log("Synchronized play window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_synchronized_play.Checked == false)
            {
                CloseSynchronizedPlayWindow();
                Logger.Log("Synchronized play window is closed.", Logger.LogTypes.Info);
            }
        }

        /// <summary>
        /// Stops all currently playing sounds and voices before the application closes.
        /// </summary>
        /// <remarks>This method ensures that all note playback, voice output, and
        /// microcontroller-generated sounds are stopped prior to application shutdown. It should be called as part of
        /// the application's closing sequence to prevent lingering audio.</remarks>
        private async void StopAllSoundsBeforeClosing()
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
            StopPlaying();
            StopAllSounds();
            if (isModified == true && !SettingsWindow.willRestartForChanges)  // Ask for saving only if there are unsaved changes and not restarting for settings change
            {
                var result = MessageForm.Show(this, Resources.MessageUnsavedChanges, Resources.TitleUnsavedChanges, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveTheFile();
                    if (!isSaved)
                    {
                        e.Cancel = true; // Cancel closing if save was not successful
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            checkBox_metronome.Checked = false;
            cancellationTokenSource.Cancel();
            isClosing = true;
            StopAllSoundsBeforeClosing();
        }

        private void main_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopPlaying();
            checkBox_metronome.Checked = false;
            cancellationTokenSource.Cancel();
            isClosing = true;
            StopAllSoundsBeforeClosing();
        }
        private void rewindToSavedVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isMusicPlaying == true)
            {
                StopPlaying();
            }
            int visibleLine = GetVisibleIndex();
            if (initialMemento == null)
            {
                MessageForm.Show(this, Resources.MessageNoSavedVersion, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                SetThemeOfListViewItems(); // Set theme of list view items after rewinding if theme when rewinding is different
                if (listViewNotes.Items.Count > 0)
                {
                    if (visibleLine != -1 && visibleLine < listViewNotes.Items.Count)
                    {
                        listViewNotes.EnsureVisible(visibleLine);
                    }
                    else
                    {
                        listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                    }
                }
                // Log states of variables
                Logger.Log($"Rewind to saved version - BPM: {Variables.bpm}, Alt Notes: {Variables.alternatingNoteLength}, Note Silence Ratio: {Variables.noteSilenceRatio}, Time Signature: {Variables.timeSignature}, Octave: {Variables.octave}", Logger.LogTypes.Info);
            }
            catch (Exception ex)
            {
                Logger.Log($"Error rewinding: {ex.Message}", Logger.LogTypes.Error);
                MessageForm.Show(this, $"{Resources.MessageErrorRewinding} {ex.Message}", Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox_mute_system_speaker_CheckedChanged(object sender, EventArgs e)
        {
            if (TemporarySettings.CreatingSounds.isPlaybackMuted == false && checkBox_mute_playback.Checked == false)
            {
                NotePlayer.StopAllNotes(); // Stop all sounds if playback is not muted
            }
            TemporarySettings.CreatingSounds.isPlaybackMuted = checkBox_mute_playback.Checked;
            Logger.Log($"Playback muted: {TemporarySettings.CreatingSounds.isPlaybackMuted}", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Displays the keyboard shortcut text on all associated buttons in the user interface.
        /// </summary>
        /// <remarks>This method updates the text of each button to show its corresponding keyboard
        /// shortcut. If called from a thread other than the UI thread, the update is marshaled to the UI thread to
        /// ensure thread safety.</remarks>
        private void ShowKeyboardKeysShortcuts()
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

        /// <summary>
        /// Clears the text of all keyboard key shortcut buttons asynchronously on the UI thread.
        /// </summary>
        /// <remarks>This method iterates through all registered shortcut buttons and sets their text to
        /// an empty string. If a button requires invocation on the UI thread, the update is performed using the
        /// appropriate thread marshaling. This operation is performed asynchronously and may not complete
        /// immediately.</remarks>
        private void HideKeyboardKeysShortcuts()
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
                StopPlaying();
                StopPlayingAllSounds(); // Stop all sounds before enabling keyboard as piano
                ShowKeyboardKeysShortcuts();
                this.ActiveControl = null; // Remove focus from any control to prevent accidental typing
            }
            else if (checkBox_use_keyboard_as_piano.Checked == false)
            {
                HideKeyboardKeysShortcuts();
                StopAllSounds();
            }
            EnableDisableTabStopOnEntireForm(this);
        }

        /// <summary>
        /// Enables or disables the TabStop property for all controls on the specified form based on the current
        /// keyboard mode setting.
        /// </summary>
        /// <remarks>This method updates the TabStop and CausesValidation properties for the form and all
        /// of its immediate child controls. Typically used to prevent or allow tab navigation when the keyboard is
        /// being used as a piano input device.</remarks>
        /// <param name="form">The form whose controls will have their TabStop property enabled or disabled.</param>
        private void EnableDisableTabStopOnEntireForm(Form form)
        {
            this.TabStop = !checkBox_use_keyboard_as_piano.Checked;
            this.CausesValidation = !checkBox_use_keyboard_as_piano.Checked;
            foreach (Control control in form.Controls)
            {
                EnableDisableTabStop(control);
            }
        }

        /// <summary>
        /// Enables or disables the TabStop and CausesValidation properties for the specified control and all of its
        /// child controls based on the current keyboard mode setting.
        /// </summary>
        /// <remarks>This method recursively updates all descendant controls. Controls will have TabStop
        /// and CausesValidation set to false when keyboard-as-piano mode is enabled, and true otherwise.</remarks>
        /// <param name="ctrl">The control whose TabStop and CausesValidation properties, as well as those of its child controls, will be
        /// updated.</param>
        private void EnableDisableTabStop(Control ctrl)
        {
            ctrl.TabStop = !checkBox_use_keyboard_as_piano.Checked;
            ctrl.CausesValidation = !checkBox_use_keyboard_as_piano.Checked;
            if (ctrl.HasChildren)
            {
                foreach (Control childCtrl in ctrl.Controls)
                {
                    EnableDisableTabStop(childCtrl);
                }
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

        // The feature that Robbi-985 (aka SomethingUnreal) planned add to his Bleeper Music Maker long time ago, but he abandoned his project and never added this feature.
        private void main_window_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fileName = files[0];
                OpenFiles(fileName, FileOpenMode.DragAndDrop);
            }
        }

        /// <summary>
        /// Specifies the method by which a file was opened within the application.
        /// </summary>
        /// <remarks>Use this enumeration to determine whether a file was opened via drag-and-drop or as a
        /// command-line argument. This can be useful for customizing application behavior based on how the file was
        /// provided by the user.</remarks>
        enum FileOpenMode
        {
            DragAndDrop,
            OpenedAsArg,
        }
        private FileOpenMode fileOpenMode;

        /// <summary>
        /// Opens the specified file using the provided file open mode, handling supported MIDI and Bleeper Music Maker
        /// project files.
        /// </summary>
        /// <remarks>If the file is not supported or is corrupted, an error message is displayed to the
        /// user. Supported files include standard MIDI files and Bleeper Music Maker project files. The method logs
        /// actions and errors according to the file open mode.</remarks>
        /// <param name="fileName">The path to the file to open. Must refer to a valid MIDI or Bleeper Music Maker project file.</param>
        /// <param name="fileOpenMode">Specifies how the file is being opened, such as via drag-and-drop or as a command-line argument. Determines
        /// how user prompts and error messages are displayed.</param>
        private void OpenFiles(string fileName, FileOpenMode fileOpenMode)
        {
            try
            {
                string firstLine = File.ReadLines(fileName).First();
                if (MIDIFileValidator.IsMidiFile(fileName))
                {
                    OpenMIDIFilePlayer(fileName);
                }
                else if (firstLine == "Bleeper Music Maker by Robbi-985 file format" ||
                    firstLine == "<NeoBleeperProjectFile>")
                {
                    switch (fileOpenMode)
                    {
                        case FileOpenMode.DragAndDrop:
                            Logger.Log($"Opening the file you dragged: {fileName}", Logger.LogTypes.Info);
                            AskForSavingIfModified(new Action(() => FileParser(fileName)));
                            break;
                        case FileOpenMode.OpenedAsArg:
                            Logger.Log($"Opening the file you opened: {fileName}", Logger.LogTypes.Info);
                            FileParser(fileName);
                            break;
                    }
                }
                else
                {
                    switch (fileOpenMode)
                    {
                        case FileOpenMode.DragAndDrop:
                            Logger.Log("The file you dragged is not supported by NeoBleeper or is corrupted.", Logger.LogTypes.Error);
                            MessageForm.Show(this, Resources.MessageNonSupportedDraggedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case FileOpenMode.OpenedAsArg:
                            Logger.Log("The file you opened is not supported by NeoBleeper or is corrupted.", Logger.LogTypes.Error);
                            MessageForm.Show(this, Resources.MessageNonSupportedOpenedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                switch (fileOpenMode)
                {
                    case FileOpenMode.DragAndDrop:
                        Logger.Log("The file you dragged is not supported by NeoBleeper or is corrupted.", Logger.LogTypes.Error);
                        MessageForm.Show(this, Resources.MessageCorruptedOrCurrentlyUsedDraggedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case FileOpenMode.OpenedAsArg:
                        Logger.Log("The file you opened is not supported by NeoBleeper or is corrupted.", Logger.LogTypes.Error);
                        MessageForm.Show(this, Resources.MessageCorruptedOrCurrentlyUsedOpenedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        /// <summary>
        /// Opens a MIDI file in the MIDI file player dialog if playback is not muted and MIDI output is enabled.
        /// </summary>
        /// <remarks>If the specified file is not a valid MIDI file or is inaccessible, an error message
        /// is displayed and the file is not opened. The method does not open the player if playback is muted and MIDI
        /// output is disabled.</remarks>
        /// <param name="fileName">The full path to the MIDI file to open. Must refer to a valid, accessible MIDI file.</param>
        private void OpenMIDIFilePlayer(string fileName)
        {
            if (!(checkBox_mute_playback.Checked && !TemporarySettings.MIDIDevices.useMIDIoutput))
            {
                if (MIDIFileValidator.IsMidiFile(fileName))
                {
                    lastOpenedMIDIFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                    MIDIFilePlayer MidiFilePlayer = new MIDIFilePlayer(fileName, this);
                    MidiFilePlayer.ShowDialog();
                    Logger.Log("MIDI file is opened.", Logger.LogTypes.Info);
                }
                else
                {
                    MessageForm.Show(this, Resources.MessageNonValidMIDIFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("This file is not a valid MIDI file, or it is corrupted or is being used by another process.", Logger.LogTypes.Error);
                }
            }
            else
            {
                Logger.Log("\"Mute playback\" is checked and \"Use MIDI output\" checkbox is unchecked, so it cannot be opened.", Logger.LogTypes.Error);
                MessageForm.Show(this, Resources.MIDIFilePlayerMutedError, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Serializes the specified NeoBleeper project file to XML and saves it to the given file path.
        /// </summary>
        /// <remarks>The resulting XML file will not include XML namespaces or an XML declaration. If a
        /// file already exists at the specified path, it will be overwritten.</remarks>
        /// <param name="filePath">The path to the file where the XML representation of the project will be saved. Must not be null or empty.</param>
        /// <param name="projectFile">The NeoBleeper project file to serialize. Must not be null.</param>
        private static void SerializeXML(string filePath, NBPMLFile.NeoBleeperProjectFile projectFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(NBPMLFile.NeoBleeperProjectFile));
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
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            CloseAllOpenWindows(); // Close all open windows before opening a new modal dialog
            if (!(checkBox_mute_playback.Checked && !TemporarySettings.MIDIDevices.useMIDIoutput && !TemporarySettings.MicrocontrollerSettings.useMicrocontroller))
            {
                openFileDialog.Filter = Resources.FilterMIDIFileFormat;
                openFileDialog.Title = Resources.TitleOpenMIDIFile;
                openFileDialog.FileName = lastOpenedMIDIFileName;
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    OpenMIDIFilePlayer(openFileDialog.FileName);
                }
            }
            else
            {
                Logger.Log("\"Mute playback\" is checkbox is checked, \"Use motor or buzzer (via Arduino, Raspberry Pi or ESP32)\" checkbox is unchecked and \"Use MIDI output\" checkbox is unchecked, so it cannot be opened.", Logger.LogTypes.Error);
                MessageForm.Show(this, Resources.MIDIFilePlayerMutedError, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void blankLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBlankLine();
        }

        private void clearNote1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearNote1();
        }

        private void unselectLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnselectLine();
        }

        private void eraseWholeLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EraseLine();
        }

        private void playAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayAll();
        }

        private void playFromSelectedLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayFromSelectedLine();
        }

        private void stopPlayingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopPlaying();
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

        /// <summary>
        /// Copies the text of all selected and checked notes in the list view to the system clipboard in a
        /// tab-delimited format.
        /// </summary>
        /// <remarks>If both selected and checked items overlap, each item's text is included only once.
        /// The copied text is standardized for localization and includes all subitem values separated by tabs, with
        /// each item on a new line. A notification is displayed upon successful copy.</remarks>
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
                string standardizedText = StandardizeLocalizedLengthModsAndArticulations(clipboardText.ToString());
                Clipboard.SetText(standardizedText);
                Toast.ShowToast(this, Resources.ToastMessageNotesCopy, 2000);
                Logger.Log("Copy to clipboard is executed.", Logger.LogTypes.Info);
            }
        }

        /// <summary>
        /// Converts localized musical note length modifiers and articulations in the specified text to their
        /// standardized English abbreviations.
        /// </summary>
        /// <remarks>This method is useful for normalizing musical notation text that may contain
        /// localized terms, ensuring consistent processing or display regardless of the original language. Only
        /// recognized terms are replaced; all other text remains unchanged.</remarks>
        /// <param name="rawClipboardText">The input text containing localized musical note length modifiers and articulations to be standardized.
        /// Cannot be null.</param>
        /// <returns>A string with all recognized localized note length modifiers and articulations replaced by their
        /// standardized English abbreviations.</returns>
        private string StandardizeLocalizedLengthModsAndArticulations(string rawClipboardText)
        {
            string standardizedText = rawClipboardText
                .Replace(Resources.WholeNote, "Whole")
                .Replace(Resources.HalfNote, "Half")
                .Replace(Resources.QuarterNote, "Quarter")
                .Replace(Resources.EighthNote, "1/8")
                .Replace(Resources.SixteenthNote, "1/16")
                .Replace(Resources.ThirtySecondNote, "1/32")
                .Replace(Resources.DottedModifier, "Dot")
                .Replace(Resources.TripletModifier, "Tri")
                .Replace(Resources.StaccatoArticulation, "Sta")
                .Replace(Resources.SpiccatoArticulation, "Spi")
                .Replace(Resources.FermataArticulation, "Fer");
            return standardizedText;
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteFromClipboard();
        }

        /// <summary>
        /// Pastes note data from the clipboard into the notes list at the selected position or at the end if no item is
        /// selected.
        /// </summary>
        /// <remarks>The method expects the clipboard to contain tab-delimited text with at least seven
        /// columns per line, corresponding to note fields. If one or more items are selected in the notes list, the
        /// pasted notes are inserted at the position of the first selected item; otherwise, they are appended to the
        /// end of the list. The method updates the modified state and ensures the newly inserted notes are visible in
        /// the list.</remarks>
        private void PasteFromClipboard()
        {
            if (!Clipboard.ContainsText()) return;

            string clipboardText = LocalizeLengthModsAndArticulations(Clipboard.GetText());
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

        /// <summary>
        /// Replaces standardized musical length modifiers and articulations in the specified text with their localized
        /// equivalents.
        /// </summary>
        /// <remarks>This method is intended to support localization of musical notation terms such as
        /// note lengths and articulations. Only recognized terms are replaced; all other text remains
        /// unchanged.</remarks>
        /// <param name="standardizedClipboardText">The text containing standardized musical terms to be localized. Cannot be null.</param>
        /// <returns>A string in which recognized musical length modifiers and articulations have been replaced with their
        /// localized representations.</returns>
        private string LocalizeLengthModsAndArticulations(string standardizedClipboardText)
        {
            string localizedText = standardizedClipboardText.Replace("Whole", Resources.WholeNote)
                .Replace("Half", Resources.HalfNote)
                .Replace("Quarter", Resources.QuarterNote)
                .Replace("1/8", Resources.EighthNote)
                .Replace("1/16", Resources.SixteenthNote)
                .Replace("1/32", Resources.ThirtySecondNote)
                .Replace("Dot", Resources.DottedModifier)
                .Replace("Tri", Resources.TripletModifier)
                .Replace("Sta", Resources.StaccatoArticulation)
                .Replace("Spi", Resources.SpiccatoArticulation)
                .Replace("Fer", Resources.FermataArticulation);
            return localizedText;
        }
        private int GetVisibleIndex()
        {
            if (listViewNotes.Items.Count == 0)
            {
                return -1;
            }

            // Check if there is a top visible item
            if (listViewNotes.TopItem != null)
            {
                // Get the index of the top visible item
                int topIndex = listViewNotes.TopItem.Index;
                int headerHeight = listViewNotes.Height - listViewNotes.ClientSize.Height;

                // Calculate the number of fully visible items
                int visibleItemsCount = listViewNotes.ClientSize.Height / listViewNotes.TopItem.Bounds.Height;

                // Calculate the index of the last visible item
                int lastVisibleIndex = Math.Min((topIndex + visibleItemsCount - 1) - 1, listViewNotes.Items.Count - 1);

                return lastVisibleIndex;
            }

            return -1; // Return -1 if no item is visible
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isMusicPlaying == true)
            {
                StopPlaying();
            }
            int visibleLine = GetVisibleIndex();
            commandManager.Undo();
            SetThemeOfListViewItems(); // Set theme of list view items after undoing if theme when undoing is different 
            if (listViewNotes.Items.Count > 0)
            {
                if (visibleLine != -1 && visibleLine < listViewNotes.Items.Count)
                {
                    listViewNotes.EnsureVisible(visibleLine);
                }
                else
                {
                    listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                }
            }
            if (commandManager.CanUndo == false)
            {
                isModified = false;
                UpdateFormTitle();
            }
            Logger.Log("Undo is executed.", Logger.LogTypes.Info);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isMusicPlaying == true)
            {
                StopPlaying();
            }
            int visibleIndex = GetVisibleIndex();
            commandManager.Redo();
            SetThemeOfListViewItems(); // Set theme of list view items after redoing if theme when redoing is different
            if (listViewNotes.Items.Count > 0)
            {
                if (visibleIndex != -1 && visibleIndex < listViewNotes.Items.Count)
                {
                    listViewNotes.EnsureVisible(visibleIndex);
                }
                else
                {
                    listViewNotes.EnsureVisible(listViewNotes.Items.Count - 1);
                }
            }
            if (commandManager.CanUndo == true)
            {
                isModified = true;
                UpdateFormTitle();
            }
            Logger.Log("Redo is executed.", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Applies the current theme colors to all items and subitems in the notes list view.
        /// </summary>
        /// <remarks>This method updates the background and foreground colors of each item and subitem in
        /// the list view to match the selected theme. It should be called whenever the theme changes to ensure that the
        /// list view items reflect the correct appearance.</remarks>
        private void SetThemeOfListViewItems()
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
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageForm.Show(this, Resources.SynchronizedPlayHelp, Resources.SynchronizedPlayHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_play_beat_sound_help_Click(object sender, EventArgs e)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageForm.Show(this, Resources.PlayBeatSoundHelp, Resources.PlayBeatSoundHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_bleeper_portamento_help_Click(object sender, EventArgs e)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageForm.Show(this, Resources.PortamentoHelp, Resources.PortamentoHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_use_keyboard_as_piano_help_Click(object sender, EventArgs e)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageForm.Show(this, Resources.UseKeyboardAsPianoHelp, Resources.UseKeyboardAsPianoHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_do_not_update_help_Click(object sender, EventArgs e)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageForm.Show(this, Resources.DoNotUpdateHelp, Resources.DoNotUpdateHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutToClipboard();
        }

        // The feature that Robbi-985 (aka SomethingUnreal) didn't thought of in Bleeper Music Maker :D

        /// <summary>
        /// Cuts the selected and checked notes from the list view, copies them to the clipboard in a tab-delimited
        /// format, and removes them from the list.
        /// </summary>
        /// <remarks>This method combines both selected and checked notes, ensuring each note is only
        /// included once. The copied text is formatted for easy pasting into spreadsheet applications. After cutting,
        /// the method updates the form state and displays a notification to the user.</remarks>
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
                string standardizedText = StandardizeLocalizedLengthModsAndArticulations(clipboardText.ToString());
                Clipboard.SetText(standardizedText);

                var removeCommand = new RemoveNoteCommand(listViewNotes, Target.Both);
                commandManager.ExecuteCommand(removeCommand);

                isModified = true;
                UpdateFormTitle();
                Toast.ShowToast(this, Resources.ToastMessageNotesCut, 2000);
                Logger.Log("Cut is executed.", Logger.LogTypes.Info);
            }
        }

        /// <summary>
        /// Updates the 'Open Recent' menu to reflect the current list of recently accessed files.
        /// </summary>
        /// <remarks>Enables or disables the 'Open Recent' menu based on whether any recent files are
        /// available. Files that no longer exist are shown as unavailable and cannot be selected.</remarks>
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

        /// <summary>
        /// Opens the specified recent file, prompting the user to save changes if the current document has been
        /// modified.
        /// </summary>
        /// <param name="filePath">The full path of the file to open. Cannot be null or empty.</param>
        private void OpenRecentFile(string filePath)
        {
            AskForSavingIfModified(new Action(() => OpenFileAndUpdateMenu(filePath)));
        }

        /// <summary>
        /// Prompts the user to save changes if there are unsaved modifications before executing the specified action.
        /// </summary>
        /// <remarks>If there are unsaved changes, the user is prompted to save, discard, or cancel. The
        /// specified action is executed only if the user chooses to save (and saving succeeds) or to discard changes.
        /// If the user cancels, the action is not executed.</remarks>
        /// <param name="action">The action to execute after handling any unsaved changes. This action is invoked only if the user chooses to
        /// proceed.</param>
        private void AskForSavingIfModified(Action action) // Action to execute after handling unsaved changes
        {
            if (isModified == true) // Ask for saving if there are unsaved changes
            {
                StopPlaying(); // Stop playing if music is playing
                StopAllNotesAfterPlaying(); // Stop all notes if any note is still playing to prevent stuck notes
                var result = MessageForm.Show(this, Resources.MessageUnsavedChanges, Resources.TitleUnsavedChanges, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveTheFile();
                    if (isSaved)
                    {
                        action(); // Execute the action if save was successful
                    }
                }
                else if (result == DialogResult.No) // Execute action without saving
                {
                    action();
                }
                else if (result == DialogResult.Cancel)
                {
                    return; // Do nothing if cancel is clicked
                }
            }
            else // No unsaved changes, execute action directly
            {
                action();
            }
        }

        /// <summary>
        /// Opens the specified file if it exists and updates the recent files menu accordingly.
        /// </summary>
        /// <remarks>If the file does not exist, an error message is displayed, the file is removed from
        /// the recent files list, and the menu is updated. Any errors encountered during the operation are logged and
        /// shown to the user.</remarks>
        /// <param name="filePath">The full path of the file to open. Cannot be null or empty.</param>
        private void OpenFileAndUpdateMenu(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    OpenAFile(filePath);
                }
                else
                {
                    Logger.Log("The recent file you are trying to open is not found.", Logger.LogTypes.Error);
                    MessageForm.Show(this, Resources.MessageFileNotFoundError + " " + filePath, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Settings1.Default.RecentFiles.Remove(filePath);
                    Settings1.Default.Save();
                    UpdateRecentFilesMenu();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error opening recent file: {ex.Message}", Logger.LogTypes.Error);
                MessageForm.Show(this, $"{Resources.MessageErrorFileOpening} {ex.Message}", Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens the specified file and initializes the application's state based on its contents.
        /// </summary>
        /// <remarks>After opening the file, the application's state is reset and the undo/redo history is
        /// cleared. Any unsaved changes will be lost.</remarks>
        /// <param name="filePath">The path to the file to open. Cannot be null or empty.</param>
        private void OpenAFile(string filePath)
        {
            FileParser(filePath);

            // Create initialMemento with current values after file is opened.
            initialMemento = originator.CreateSavedStateMemento(Variables.bpm, Variables.alternatingNoteLength,
    Variables.noteSilenceRatio, Variables.timeSignature);

            commandManager.ClearHistory(); // Reset the history
        }
        private void main_window_Load(object sender, EventArgs e)
        {
            NotificationUtils.SetPrimaryNotifyIcon(this, notifyIconNeoBleeper); // Set the primary notify icon for notifications
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
            var play_Beat_Window = checkBox_play_beat_sound.Tag as PlayBeatWindow;
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

        BusyFormHelper busyFormhelper = new BusyFormHelper();

        private async void createMusicWithAIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
                CloseAllOpenWindows(); // Close all open windows before opening a new modal dialog
                CreateMusicWithAI createMusicWithAI = new CreateMusicWithAI(this);
                if (!CreateMusicWithAI.IsAvailableInCountry())
                {
                    createMusicWithAI.Dispose();
                    Logger.Log("Google Gemini™ API is not available in your country. Please check the list of supported countries.", Logger.LogTypes.Error);
                    MessageForm.Show(this, Resources.GoogleGeminiAPIIsNotSupportedInYourCountry, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the method if not available
                }
                else
                {
                    if (DateTime.Now - lastCreateTime < createCooldown)
                    {
                        MessageForm.Show(this, Resources.MessageAICreationCooldown, Resources.TitlePleaseWait, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Exit the method if still in cooldown
                    }
                    busyFormhelper.SetFormBusy(this, true);
                    if (await createMusicWithAI.CheckWillItOpened())
                    {
                        busyFormhelper.SetFormBusy(this, false);
                        await Task.Delay(5);
                        createMusicWithAI.ShowDialog();
                        string output = createMusicWithAI.output;
                        string fileName = createMusicWithAI.generatedFilename;
                        if (createMusicWithAI.output != string.Empty)
                        {
                            if (isModified)
                            {
                                DialogResult result = MessageForm.Show(this, Resources.MessageUnsavedChanges, Resources.TitleUnsavedChanges, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                                switch (result)
                                {
                                    case DialogResult.Yes:
                                        SaveRunAndRetry(new Action(() => { CreateMusicWithAIResponse(createMusicWithAI.output, fileName); }));
                                        break;
                                    case DialogResult.No:
                                        CreateMusicWithAIResponse(createMusicWithAI.output, fileName);
                                        break;
                                }
                            }
                            else
                            {
                                CreateMusicWithAIResponse(createMusicWithAI.output, fileName);
                            }
                        }
                    }
                    else
                    {
                        busyFormhelper.SetFormBusy(this, false);
                        return;
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                busyFormhelper.SetFormBusy(this, false);
                return;
            }
        }
        int retryCount = 0;

        /// <summary>
        /// Attempts to save a file and, upon success, executes the specified action. Retries the save operation up to
        /// three times before executing the action regardless of save success.
        /// </summary>
        /// <remarks>If the save operation fails three times consecutively, the action is executed without
        /// saving. The retry count is reset after each sequence of attempts. This method is not thread-safe.</remarks>
        /// <param name="action">The action to execute after a successful save operation, or after three failed save attempts. Cannot be
        /// null.</param>
        private void SaveRunAndRetry(Action action)
        {
            SaveTheFile(); // Try to save file
            if (isSaved)
            {
                action(); // Run the action if the file is saved
                retryCount = 0; // Reset retry count after successful save and action execution
            }
            else
            {
                if (retryCount >= 3)
                {
                    Logger.Log("The action is executed without saving after 3 retries.", Logger.LogTypes.Warning);
                    retryCount = 0; // Reset retry count
                    action(); // Run the action without saving after 3 retries
                }
                else
                {
                    retryCount++;
                    SaveRunAndRetry(action); // Retry to save if not saved 
                }
            }
        }
        // This method is used to update the form title with the current file path and modification status.

        /// <summary>
        /// Updates the form's title to reflect the current file path and modification status.
        /// </summary>
        /// <remarks>The title is set to include the application's friendly name, the current file path if
        /// available, and an asterisk if the file has unsaved changes. If no file path is present and the form title
        /// contains the AI-generated music label, that label is appended instead.</remarks>
        private void UpdateFormTitle()
        {
            string title = System.AppDomain.CurrentDomain.FriendlyName;

            // Add the current file path if it exists
            if (!string.IsNullOrWhiteSpace(currentFilePath))
            {
                title += " - " + currentFilePath;
            }
            else if (this.Text.Contains(Resources.TextAIGeneratedMusic))
            {
                title += " - " + Resources.TextAIGeneratedMusic;
            }

            // Add an asterisk if the file is modified
            if (isModified && !string.IsNullOrWhiteSpace(currentFilePath))
            {
                title += "*";
            }

            this.Text = title;
        }

        /// <summary>
        /// Restores the user interface and internal variable values for tempo, alternating note length, time signature,
        /// and note silence ratio to the specified values.
        /// </summary>
        /// <remarks>This method updates both the UI controls and the corresponding internal variables.
        /// Value change events are temporarily suppressed during the update to prevent unintended side
        /// effects.</remarks>
        /// <param name="bpmValue">The beats per minute (BPM) value to set. Must be within the valid range supported by the UI control.</param>
        /// <param name="alternatingNoteLength">The length of the alternating note to set. Must be a valid value accepted by the UI control.</param>
        /// <param name="timeSignature">The time signature value to set. Must be within the supported range of the UI control.</param>
        /// <param name="noteSilenceRatio">The ratio of silence to note duration, as a double between 0.0 and 1.0, where 0.0 means no silence and 1.0
        /// means full silence.</param>
        public void RestoreVariableValues(int bpmValue, int alternatingNoteLength,
            int timeSignature, double noteSilenceRatio)
        {
            try
            {
                // Prevent triggering ValueChanged event by setting tag
                numericUpDown_bpm.Tag = "SkipValueChanged";
                numericUpDown_alternating_notes.Tag = "SkipValueChanged";
                trackBar_note_silence_ratio.Tag = "SkipValueChanged";
                trackBar_time_signature.Tag = "SkipValueChanged";

                // Update values
                numericUpDown_bpm.Value = bpmValue;
                Variables.bpm = bpmValue;

                numericUpDown_alternating_notes.Value = alternatingNoteLength;
                Variables.alternatingNoteLength = alternatingNoteLength;

                trackBar_time_signature.Value = timeSignature;
                Variables.timeSignature = timeSignature;
                lbl_time_signature.Text = timeSignature.ToString();

                trackBar_note_silence_ratio.Value = (int)(noteSilenceRatio * 100);
                Variables.noteSilenceRatio = noteSilenceRatio;
                lbl_note_silence_ratio.Text = Resources.TextPercent.Replace("{number}",
                    ((int)(noteSilenceRatio * 100)).ToString());

                Logger.Log($"Values restored: BPM={bpmValue}, Alt Notes={alternatingNoteLength}, Time Sig={timeSignature}, Silence Ratio={noteSilenceRatio}", Logger.LogTypes.Info);
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

        /// <summary>
        /// Opens the Play Beat Sound window, creating a new instance if necessary.
        /// </summary>
        /// <remarks>If the Play Beat Sound window is already open and not disposed, this method brings it
        /// to the foreground. Otherwise, it creates and displays a new window instance.</remarks>
        private void OpenPlayBeatSoundWindow()
        {
            if (playBeatWindow == null || playBeatWindow.IsDisposed)
            {
                playBeatWindow = new PlayBeatWindow(this);
            }
            playBeatWindow.Show();
            checkBox_play_beat_sound.Tag = playBeatWindow;
        }

        /// <summary>
        /// Opens the Bleeper portamento configuration window, creating a new instance if necessary.
        /// </summary>
        /// <remarks>If the portamento window is already open and not disposed, this method brings it to
        /// the foreground. Otherwise, it creates and displays a new window. The window instance is also associated with
        /// the related checkbox control for later reference.</remarks>
        private void OpenBleeperPortamentoWindow()
        {
            if (portamentoWindow == null || portamentoWindow.IsDisposed)
            {
                portamentoWindow = new PortamentoWindow(this);
            }
            portamentoWindow.Show();
            checkBox_bleeper_portamento.Tag = portamentoWindow;
        }

        /// <summary>
        /// Opens the Voice Internals window, creating a new instance if necessary.
        /// </summary>
        /// <remarks>If the Voice Internals window is already open and not disposed, this method brings it
        /// to the foreground. Otherwise, it creates and displays a new instance. This method associates the window with
        /// the related UI control for later reference.</remarks>
        private void OpenVoiceInternalsWindow()
        {
            if (voiceInternalsWindow == null || voiceInternalsWindow.IsDisposed)
            {
                voiceInternalsWindow = new VoiceInternalsWindow(this);
            }
            voiceInternalsWindow.Show();
            checkBox_use_voice_system.Tag = voiceInternalsWindow;
        }

        /// <summary>
        /// Closes the window used for playing beat sounds if it is currently open.
        /// </summary>
        private void ClosePlayBeatSoundWindow()
        {
            PlayBeatWindow playBeatWindow = checkBox_play_beat_sound.Tag as PlayBeatWindow;
            playBeatWindow.Close();
        }

        /// <summary>
        /// Closes the portamento window associated with the current control.
        /// </summary>
        /// <remarks>If no portamento window is associated, this method has no effect.</remarks>
        private void ClosePortamentoWindow()
        {
            PortamentoWindow portamentoWindow = checkBox_bleeper_portamento.Tag as PortamentoWindow;
            portamentoWindow.Close();
        }

        /// <summary>
        /// Closes the Voice Internals window if it is currently open.
        /// </summary>
        private void CloseVoiceInternalsWindow()
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
                OpenPlayBeatSoundWindow();
                Logger.Log("Play a beat sound window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_play_beat_sound.Checked == false)
            {
                ClosePlayBeatSoundWindow();
                Logger.Log("Play a beat sound window is closed.", Logger.LogTypes.Info);
            }
        }

        /// <summary>
        /// Converts a localized note length string to its corresponding unlocalized note length identifier.
        /// </summary>
        /// <param name="noteLength">The localized name of the note length to convert. This value is typically obtained from localized resources
        /// and should match one of the supported note length names.</param>
        /// <returns>A string representing the unlocalized note length identifier, such as "Whole", "Half", "Quarter", "1/8",
        /// "1/16", or "1/32". If the input does not match a known note length, returns the unlocalized identifier for a
        /// whole note.</returns>
        private string ConvertLocalizedNoteLengthIntoUnlocalized(string noteLength)
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

        /// <summary>
        /// Converts a localized modifier string to its corresponding unlocalized modifier code.
        /// </summary>
        /// <remarks>This method maps specific localized modifier strings, such as those for dotted or
        /// triplet notation, to their standard unlocalized codes (e.g., "Dot", "Tri"). If the input does not match a
        /// recognized modifier, the method returns an empty string.</remarks>
        /// <param name="modifier">The localized modifier string to convert. This value is typically obtained from localized resources.</param>
        /// <returns>A string containing the unlocalized modifier code if the input matches a known localized modifier;
        /// otherwise, an empty string.</returns>
        private string ConvertLocalizedModifiersIntoUnlocalized(string modifier)
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

        /// <summary>
        /// Converts a localized articulation name to its corresponding unlocalized abbreviation.
        /// </summary>
        /// <remarks>This method maps specific localized articulation names to their standard
        /// abbreviations. If the input does not match a known articulation, the method returns an empty
        /// string.</remarks>
        /// <param name="articulation">The localized name of the articulation to convert. This value is typically obtained from localized
        /// resources.</param>
        /// <returns>A string containing the unlocalized abbreviation for the specified articulation, or an empty string if the
        /// articulation is not recognized.</returns>
        private string ConvertLocalizedArticulationsIntoUnlocalized(string articulation)
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

        /// <summary>
        /// Converts the current project data and settings to an NBPML-formatted XML string.
        /// </summary>
        /// <remarks>The returned XML string does not include XML namespaces. This method captures the
        /// current state of all relevant project settings and note data for export or persistence in the NBPML
        /// format.</remarks>
        /// <returns>A string containing the serialized NBPML XML representation of the current project. Returns an empty string
        /// if the conversion fails.</returns>
        private String ConvertToNBPMLString()
        {
            try
            {
                // Create NBPML file object
                NBPMLFile.NeoBleeperProjectFile projectFile = new NBPMLFile.NeoBleeperProjectFile
                {
                    Settings = new NBPMLFile.Settings
                    {
                        RandomSettings = new NBPMLFile.RandomSettings
                        {
                            KeyboardOctave = Variables.octave.ToString(),
                            BPM = Variables.bpm.ToString(),
                            TimeSignature = Variables.timeSignature.ToString(),
                            NoteSilenceRatio = (Variables.noteSilenceRatio * 100).ToString(),
                            NoteLength = comboBox_note_length.SelectedIndex.ToString(),
                            AlternateTime = numericUpDown_alternating_notes.Value.ToString()
                        },
                        PlaybackSettings = new NBPMLFile.PlaybackSettings
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
                        PlayNotes = new NBPMLFile.PlayNotes
                        {
                            PlayNote1 = checkBox_play_note1_played.Checked.ToString(),
                            PlayNote2 = checkBox_play_note2_played.Checked.ToString(),
                            PlayNote3 = checkBox_play_note3_played.Checked.ToString(),
                            PlayNote4 = checkBox_play_note4_played.Checked.ToString()
                        },
                        ClickPlayNotes = new NBPMLFile.ClickPlayNotes
                        {
                            ClickPlayNote1 = checkBox_play_note1_clicked.Checked.ToString(),
                            ClickPlayNote2 = checkBox_play_note2_clicked.Checked.ToString(),
                            ClickPlayNote3 = checkBox_play_note3_clicked.Checked.ToString(),
                            ClickPlayNote4 = checkBox_play_note4_clicked.Checked.ToString()
                        }
                    },
                    LineList = new NBPMLFile.List
                    {
                        Lines = listViewNotes.Items.Cast<ListViewItem>().Select(item => new NBPMLFile.Line
                        {
                            Length = ConvertLocalizedNoteLengthIntoUnlocalized(item.SubItems[0].Text),
                            Note1 = item.SubItems[1].Text,
                            Note2 = item.SubItems[2].Text,
                            Note3 = item.SubItems[3].Text,
                            Note4 = item.SubItems[4].Text,
                            Mod = ConvertLocalizedModifiersIntoUnlocalized(item.SubItems[5].Text),
                            Art = ConvertLocalizedArticulationsIntoUnlocalized(item.SubItems[6].Text)
                        }).ToArray()
                    }
                };

                // Serialize to string into XML format and remove namespace
                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(NBPMLFile.NeoBleeperProjectFile));
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
                StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
                CloseAllOpenWindows();
                ConvertToGCode convertToGCode = new ConvertToGCode(ConvertToNBPMLString(), this);
                convertToGCode.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Log("Error converting to GCode: " + ex.Message, Logger.LogTypes.Error);
                MessageForm.Show(this, Resources.MessageConvertToGCodeError + " " + ex.Message, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBox_bleeper_portamento_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_bleeper_portamento.Checked == true)
            {
                OpenBleeperPortamentoWindow();
                Logger.Log("Bleeper portamento window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_play_beat_sound.Checked == false)
            {
                ClosePortamentoWindow();
                Logger.Log("Bleeper portamento window is closed.", Logger.LogTypes.Info);
                NotePlayer.StopAllNotes();
                UpdateLabelVisible(false);
            }
        }
        private readonly Dictionary<Button, string> buttonShortcuts = new Dictionary<Button, string>();
        [DllImport("user32.dll")]
        private static extern int ToUnicodeEx(
    uint wVirtKey, uint wScanCode, byte[] lpKeyState,
    [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff,
    int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        /// <summary>
        /// Returns a user-friendly display string for the specified key, suitable for use in UI labels or shortcut
        /// descriptions.
        /// </summary>
        /// <remarks>The returned string is localized for specialized keys if resources are available. For
        /// letter keys, the display text is capitalized. This method is intended to provide consistent and readable key
        /// labels for end users.</remarks>
        /// <param name="key">The key for which to retrieve the display text.</param>
        /// <returns>A string representing the display text for the specified key. Specialized keys such as Shift, Ctrl, Alt,
        /// Tab, Escape, and Space are returned as their common names; other keys are returned as their display
        /// character or name.</returns>
        private string GetKeyDisplayText(Keys key)
        {
            // Label specialized keys manually
            switch (key)
            {
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return Resources.ShiftKey;
                case Keys.Tab:
                    return Resources.TabKey;
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return Resources.CtrlKey;
                case Keys.Menu:
                case Keys.LMenu:
                case Keys.RMenu:
                    return Resources.AltKey;
                case Keys.Escape:
                    return Resources.EscKey;
                case Keys.Space:
                    return Resources.SpaceKey;
            }

            uint virtualKey = (uint)key;
            IntPtr keyboardLayout = GetKeyboardLayout(0);
            byte[] keyState = new byte[256];
            StringBuilder sb = new StringBuilder(5);

            int result = ToUnicodeEx(virtualKey, 0, keyState, sb, sb.Capacity, 0, keyboardLayout);

            if (result > 0)
            {
                string label = sb.ToString();
                // Only capitalize single letter keys
                if (label.Length == 1 && char.IsLetter(label[0]))
                    return label.ToUpper();
                return label;
            }
            else
            {
                // Capitalize single letter keys
                string fallback = key.ToString();
                if (fallback.Length == 1 && char.IsLetter(fallback[0]))
                    return fallback.ToUpper();
                return fallback;
            }
        }

        /// <summary>
        /// Initializes the mapping between keyboard shortcuts and their corresponding button controls.
        /// </summary>
        /// <remarks>This method associates specific keyboard keys with button controls to enable
        /// keyboard-based interaction. It should be called during form initialization to ensure that all button
        /// shortcuts are set up before user input is processed.</remarks>
        private void InitializeButtonShortcuts()
        {
            // Key code and button mapping
            var keyButtonMap = new Dictionary<Keys, Button>
    {
        { Keys.Tab, button_c3 },
        { Keys.Oemtilde, button_c_s3 },
        { Keys.Q, button_d3 },
        { Keys.D1, button_d_s3 },
        { Keys.W, button_e3 },
        { Keys.E, button_f3 },
        { Keys.D3, button_f_s3 },
        { Keys.R, button_g3 },
        { Keys.D4, button_g_s3 },
        { Keys.T, button_a3 },
        { Keys.D5, button_a_s3 },
        { Keys.Y, button_b3 },
        { Keys.U, button_c4 },
        { Keys.D7, button_c_s4 },
        { Keys.I, button_d4 },
        { Keys.D8, button_d_s4 },
        { Keys.O, button_e4 },
        { Keys.P, button_f4 },
        { Keys.D0, button_f_s4 },
        { Keys.OemOpenBrackets, button_g4 },
        { Keys.Oem8, button_g_s4 },
        { Keys.OemCloseBrackets, button_a4 },
        { Keys.OemMinus, button_a_s4 },
        { Keys.ShiftKey, button_b4 },
        { Keys.Oem102, button_c5 },
        { Keys.A, button_c_s5 },
        { Keys.Z, button_d5 },
        { Keys.S, button_d_s5 },
        { Keys.X, button_e5 },
        { Keys.C, button_f5 },
        { Keys.F, button_f_s5 },
        { Keys.V, button_g5 },
        { Keys.G, button_g_s5 },
        { Keys.B, button_a5 },
        { Keys.H, button_a_s5 },
        { Keys.N, button_b5 }
    };

            buttonShortcuts.Clear();

            foreach (var pair in keyButtonMap)
            {
                string keyLabel = GetKeyDisplayText(pair.Key);
                buttonShortcuts.Add(pair.Value, keyLabel);
            }
        }
        private HashSet<int> pressedKeys = new HashSet<int>();
        private void main_window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                trackBar_note_silence_ratio.Refresh();
                trackBar_time_signature.Refresh();
            }
            // Check if the key is one we want to use for piano playing
            if (IsKeyboardPianoKey(e.KeyCode))
            {
                e.Handled = true; // Prevent the key press from being processed further
                e.SuppressKeyPress = true; // Suppress the key press sound
                HashSet<int> currentlyPressedKeys = new HashSet<int>();
                currentlyPressedKeys.Add((int)e.KeyCode);
                if (!isAlternatingPlaying)
                {
                    if (checkBox_use_keyboard_as_piano.Checked)
                    {
                        RestartBeepIfMutedEarly(GetFrequencyFromKeyCode((int)e.KeyCode));
                    }
                    if (currentlyPressedKeys == pressedKeys)
                    {
                        // If the key is already pressed, do nothing
                        e.Handled = true;
                        return;
                    }
                    keyPressed = true; // Set KeyPressed to true when a key is pressed
                    pressedKeys.Add((int)e.KeyCode);
                    keyCharNum = pressedKeys.Distinct().ToArray();
                    MarkupTheKeyWhenKeyIsPressed(e.KeyValue);
                    PlayWithRegularKeyboard();
                }
                // Allow regular keyboard shortcuts to work
                else
                {
                    // Let other key presses pass through (like Ctrl+S, Ctrl+Z, etc.)
                    e.Handled = false;
                }
            }
        }

        /// <summary>
        /// Restarts the beep sound if it was muted early, based on the specified frequency and current portamento
        /// settings.
        /// </summary>
        /// <remarks>The beep is only restarted if portamento is disabled or configured to always produce
        /// sound. This method has no effect if the beep was not muted early.</remarks>
        /// <param name="frequency">The frequency, in hertz, at which to restart the beep if it was previously muted early.</param>
        private async void RestartBeepIfMutedEarly(int frequency)
        {
            if (IsBeepMutedEarly())
            {
                // Only restart beep if portamento is disabled or set to always produce sound
                Action restartBeepAction = async () =>
                {
                    await PlayBeepWithLabelAsync(frequency, 1, true);
                };
                if (checkBox_bleeper_portamento.Checked &&
                       TemporarySettings.PortamentoSettings.portamentoType == TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound)
                {
                    restartBeepAction(); // Play non-stopping beep to restart beep
                }
                else if (!checkBox_bleeper_portamento.Checked)
                {
                    restartBeepAction(); // Play non-stopping beep to restart beep
                }
            }
        }

        /// <summary>
        /// Determines whether the beep sound is muted at an early stage based on the current playback and sound device
        /// settings.
        /// </summary>
        /// <returns>true if the beep is considered muted early according to the playback and sound device configuration;
        /// otherwise, false.</returns>
        private bool IsBeepMutedEarly()
        {
            if (!TemporarySettings.CreatingSounds.isPlaybackMuted)
            {
                if (TemporarySettings.CreatingSounds.createBeepWithSoundDevice)
                {
                    return SoundRenderingEngine.WaveSynthEngine.AreWavesMutedEarly();
                }
                else
                {
                    return !SoundRenderingEngine.SystemSpeakerBeepEngine.IsSystemSpeakerBeepStuck();
                }
            }
            else
            {
                return false; // Not muted early if playback is muted
            }
        }
        private void main_window_KeyUp(object sender, KeyEventArgs e)
        {
            RemoveKey((int)e.KeyCode);
        }

        /// <summary>
        /// Removes the specified key from the set of currently pressed keys and updates the keyboard state accordingly.
        /// </summary>
        /// <remarks>If portamento is enabled and set to always produce sound, removing a key does not
        /// stop all notes. Otherwise, all notes are stopped after the key is removed. The method also updates the
        /// visual state of the keyboard to reflect the current set of pressed keys.</remarks>
        /// <param name="keyCode">The code of the key to remove from the set of pressed keys.</param>
        private void RemoveKey(int keyCode)
        {
            pressedKeys.Remove(keyCode);
            keyCharNum = pressedKeys.ToArray();
            keyCharNum = keyCharNum.Distinct().ToArray();

            isAlternatingPlayingRegularKeyboard = false;
            UnmarkAllButtons();

            // Don't stop all notes if portamento is enabled and set to always produce sound
            if (!(checkBox_bleeper_portamento.Checked &&
                  TemporarySettings.PortamentoSettings.portamentoType == TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound))
            {
                StopAllNotesAfterPlaying();
            }

            if (pressedKeys.Count == 0)
            {
                keyPressed = false;
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
                PlayWithRegularKeyboard();
            }
        }

        /// <summary>
        /// Removes all currently pressed keys from the collection.
        /// </summary>
        /// <remarks>This method clears the internal state of pressed keys by removing each key
        /// individually. After calling this method, the collection of pressed keys will be empty.</remarks>
        private void RemoveAllKeys()
        {
            foreach (int key in pressedKeys)
            {
                RemoveKey(key);
            }
        }

        /// <summary>
        /// Calculates the frequency, in hertz, corresponding to a given keyboard key code based on a predefined mapping
        /// of keys to musical notes and octaves.
        /// </summary>
        /// <remarks>The mapping associates specific keyboard keys with musical notes across several
        /// octaves. The resulting frequency depends on both the mapped note and the current octave setting. If the key
        /// code is not recognized, the method returns 0.</remarks>
        /// <param name="keyCode">The integer value representing the keyboard key code to map to a musical note frequency.</param>
        /// <returns>The frequency in hertz associated with the specified key code. Returns 0 if the key code does not correspond
        /// to a mapped note.</returns>
        private int GetFrequencyFromKeyCode(int keyCode)
        {
            // Key and octave offset mapping
            Dictionary<int, (double baseFreq, int octaveOffset)> keyValuePairs = new()
            {
                { (int)Keys.Tab, (NoteUtility.BaseNoteFrequencyIn4thOctave.C, -1) }, // C3
                { (int)Keys.Oemtilde, (NoteUtility.BaseNoteFrequencyIn4thOctave.CS, -1) }, // C#3
                { (int)Keys.Q, (NoteUtility.BaseNoteFrequencyIn4thOctave.D, -1) }, // D3
                { (int)Keys.D1, (NoteUtility.BaseNoteFrequencyIn4thOctave.DS, -1) }, // D#3
                { (int)Keys.W, (NoteUtility.BaseNoteFrequencyIn4thOctave.E, -1) }, // E3
                { (int)Keys.E, (NoteUtility.BaseNoteFrequencyIn4thOctave.F, -1) }, // F3
                { (int)Keys.D3, (NoteUtility.BaseNoteFrequencyIn4thOctave.FS, -1) }, // F#3
                { (int)Keys.R, (NoteUtility.BaseNoteFrequencyIn4thOctave.G, -1) }, // G3
                { (int)Keys.D4, (NoteUtility.BaseNoteFrequencyIn4thOctave.GS, -1) }, // G#3
                { (int)Keys.T, (NoteUtility.BaseNoteFrequencyIn4thOctave.A, -1) }, // A3
                { (int)Keys.D5, (NoteUtility.BaseNoteFrequencyIn4thOctave.AS, -1) }, // A#3
                { (int)Keys.Y, (NoteUtility.BaseNoteFrequencyIn4thOctave.B, -1) }, // B3
                { (int)Keys.U, (NoteUtility.BaseNoteFrequencyIn4thOctave.C, 0) }, // C4
                { (int)Keys.D7, (NoteUtility.BaseNoteFrequencyIn4thOctave.CS, 0) }, // C#4
                { (int)Keys.I, (NoteUtility.BaseNoteFrequencyIn4thOctave.D, 0) }, // D4
                { (int)Keys.D8, (NoteUtility.BaseNoteFrequencyIn4thOctave.DS, 0) }, // D#4
                { (int)Keys.O, (NoteUtility.BaseNoteFrequencyIn4thOctave.E, 0) }, // E4
                { (int)Keys.P, (NoteUtility.BaseNoteFrequencyIn4thOctave.F, 0) }, // F4
                { (int)Keys.D0, (NoteUtility.BaseNoteFrequencyIn4thOctave.FS, 0) }, // F#4
                { (int)Keys.OemOpenBrackets, (NoteUtility.BaseNoteFrequencyIn4thOctave.G, 0) }, // G4
                { (int)Keys.Oem8, (NoteUtility.BaseNoteFrequencyIn4thOctave.GS, 0) }, // G#4
                { (int)Keys.OemCloseBrackets, (NoteUtility.BaseNoteFrequencyIn4thOctave.A, 0) }, // A4
                { (int)Keys.OemMinus, (NoteUtility.BaseNoteFrequencyIn4thOctave.AS, 0) }, // A#4
                { (int)Keys.ShiftKey, (NoteUtility.BaseNoteFrequencyIn4thOctave.B, 0) }, // B4
                { (int)Keys.Oem102, (NoteUtility.BaseNoteFrequencyIn4thOctave.C, 1) }, // C5
                { (int)Keys.A, (NoteUtility.BaseNoteFrequencyIn4thOctave.CS, 1) }, // C#5
                { (int)Keys.Z, (NoteUtility.BaseNoteFrequencyIn4thOctave.D, 1) }, // D5
                { (int)Keys.S, (NoteUtility.BaseNoteFrequencyIn4thOctave.DS, 1) }, // D#5
                { (int)Keys.X, (NoteUtility.BaseNoteFrequencyIn4thOctave.E, 1) }, // E5
                { (int)Keys.C, (NoteUtility.BaseNoteFrequencyIn4thOctave.F, 1) }, // F5
                { (int)Keys.F, (NoteUtility.BaseNoteFrequencyIn4thOctave.FS, 1) }, // F#5
                { (int)Keys.V, (NoteUtility.BaseNoteFrequencyIn4thOctave.G, 1) }, // G5
                { (int)Keys.G, (NoteUtility.BaseNoteFrequencyIn4thOctave.GS, 1) }, // G#5
                { (int)Keys.B, (NoteUtility.BaseNoteFrequencyIn4thOctave.A, 1) }, // A5
                { (int)Keys.H, (NoteUtility.BaseNoteFrequencyIn4thOctave.AS, 1) }, // A#5
                { (int)Keys.N, (NoteUtility.BaseNoteFrequencyIn4thOctave.B, 1) }, // B5
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

        /// <summary>
        /// Removes MIDI notes from the active set that no longer correspond to currently pressed keys.
        /// </summary>
        /// <remarks>This method should be called to ensure that only notes corresponding to actively
        /// pressed keys remain active. It stops any MIDI notes that are no longer associated with a pressed key and
        /// updates the active note set accordingly.</remarks>
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

        /// <summary>
        /// Plays musical notes using the computer keyboard as input, emulating a piano or MIDI keyboard.
        /// </summary>
        /// <remarks>This method responds to key presses when the option to use the keyboard as a piano is
        /// enabled. It supports both single and multiple key presses, allowing for polyphonic playback. If MIDI output
        /// is enabled, notes are sent to the configured MIDI device; otherwise, notes are played using the system
        /// beeper. The method respects user settings such as portamento and note length. This method is intended to be
        /// called in response to keyboard events and is not thread-safe.</remarks>
        private async void PlayWithRegularKeyboard() // Play notes with regular keyboard (the keyboard of the computer, not MIDI keyboard)
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
                    while (keyPressed && isAlternatingPlayingRegularKeyboard)
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
                                await PlayBeepWithLabelAsync(frequency, Variables.alternatingNoteLength);
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

        /// <summary>
        /// Asynchronously plays a beep sound with the specified frequency and duration, and optionally prevents
        /// interruption by other sounds.
        /// </summary>
        /// <param name="frequency">The frequency of the beep, in hertz. Must be a positive integer.</param>
        /// <param name="duration">The duration of the beep, in milliseconds. Must be a positive integer.</param>
        /// <param name="nonStopping">true to prevent the beep from being interrupted by other sounds; otherwise, false.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task PlayBeepWithLabelAsync(int frequency, int duration, bool nonStopping = false)
        {
            await Task.Run(() => PlayBeepWithLabel(frequency, duration, nonStopping));
        }

        /// <summary>
        /// Marks up the corresponding piano key button with a highlight color when the specified keyboard key is
        /// pressed, if keyboard-to-piano mapping is enabled.
        /// </summary>
        /// <remarks>This method only applies the markup if the option to use the keyboard as a piano is
        /// enabled. The highlight color is determined by the current application settings. If the pressed key does not
        /// correspond to a mapped piano key, no action is taken.</remarks>
        /// <param name="keyCode">The key code of the keyboard key that was pressed. This value is typically obtained from a key event and
        /// determines which piano key button, if any, will be marked up.</param>
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
                if (keyCode == (int)Keys.D7)
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
                if (keyCode == (int)Keys.D8)
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
                if (keyCode == (int)Keys.D0)
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
                if (keyCode == (int)Keys.Oem8)
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
                if (keyCode == (int)Keys.OemMinus)
                {
                    button_a_s4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_b4, out shortcut))
            {
                if (keyCode == (int)Keys.ShiftKey)
                {
                    button_b4.BackColor = markdownColor;
                }
            }
            if (buttonShortcuts.TryGetValue(button_c5, out shortcut))
            {
                if (keyCode == (int)Keys.Oem102)
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

        /// <summary>
        /// Resets the background color of all button controls on the keyboard panel to their default key colors.
        /// </summary>
        /// <remarks>This method restores the appearance of both white and black keys by setting their
        /// background colors to standard values. It is typically used to clear any visual markings or highlights
        /// applied to the keys.</remarks>
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

        /// <summary>
        /// Determines whether the specified key corresponds to a piano key on the keyboard layout.
        /// </summary>
        /// <param name="keyCode">The key to evaluate as a member of the piano keyboard mapping.</param>
        /// <returns>true if the specified key is mapped to a piano key; otherwise, false.</returns>
        private bool IsKeyboardPianoKey(Keys keyCode)
        {
            // Define all the keys that should trigger the piano functionality
            HashSet<Keys> pianoKeys = new HashSet<Keys>
        {
            // White keys
            Keys.Tab, Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y,
            Keys.U, Keys.I, Keys.O, Keys.P, Keys.OemOpenBrackets, Keys.OemCloseBrackets, Keys.ShiftKey,
            Keys.Oem102, Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N,
        
            // Black keys
            Keys.Oemtilde, Keys.D1, Keys.D3, Keys.D4, Keys.D5,
            Keys.D7, Keys.D8, Keys.D0, Keys.Oem8, Keys.OemMinus,
            Keys.A, Keys.S, Keys.F, Keys.G, Keys.H
        };

            return pianoKeys.Contains(keyCode);
        }

        /// <summary>
        /// Initializes MIDI input and begins listening for incoming MIDI messages if MIDI input is enabled.
        /// </summary>
        /// <remarks>This method attaches the necessary event handler and starts the MIDI input device. It
        /// has no effect if MIDI input is disabled or the MIDI input device is not available. Call this method before
        /// attempting to receive MIDI input events.</remarks>
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
                            MessageForm.Show(this, $"{Resources.MessageStartMIDIInputDeviceError} {ex.Message}\n {Resources.MessageStartMIDIInputDeviceErrorPart2}",
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
            if (isMusicPlaying || isClosing)
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
                                PlayAlternatingNotes();
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

        /// <summary>
        /// Initiates a portamento effect, smoothly transitioning the currently playing note to the specified target
        /// frequency according to the configured portamento settings.
        /// </summary>
        /// <remarks>If a portamento is already in progress, it is canceled and replaced by the new
        /// transition. The behavior and speed of the portamento are determined by the current portamento settings. This
        /// method is asynchronous and returns immediately; the transition occurs in the background.</remarks>
        /// <param name="targetFrequency">The frequency, in hertz, to which the note should glide. Must be a positive integer representing the target
        /// pitch.</param>
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
                    NotePlayer.PlayNote(currentFrequency, stepDuration, true);
                    await Task.Delay(stepDuration, token);
                }

                if (TemporarySettings.PortamentoSettings.portamentoType == TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound)
                {
                    UpdateLabelVisible(true);
                    NotePlayer.PlayNote(targetFrequency, 1, true);
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

        /// <summary>
        /// Plays all currently active MIDI notes in an alternating sequence on a background thread.
        /// </summary>
        /// <remarks>This method starts playback asynchronously and continues to play the active notes
        /// until the alternating playback is stopped. It is intended to be called when alternating note playback is
        /// required, and should not be called multiple times concurrently. Thread safety is managed internally for the
        /// active MIDI notes collection.</remarks>
        private void PlayAlternatingNotes()
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
                        PlayBeepWithLabel(MIDIIOUtils.MidiNoteToFrequency(note), Variables.alternatingNoteLength);
                    }
                }
                while (isAlternatingPlaying == true);
            });
        }

        /// <summary>
        /// Updates the active MIDI input device to the specified device number, reconfiguring event handlers and input
        /// state as needed.
        /// </summary>
        /// <remarks>If a MIDI input device is already active, this method stops and detaches it before
        /// switching to the new device. If MIDI input is enabled in settings, the new device is started and event
        /// handlers are attached. Any active alternating notes playback is stopped, and the list of active MIDI input
        /// notes is cleared. This method should be called when the user selects a different MIDI input device at
        /// runtime.</remarks>
        /// <param name="deviceNumber">The zero-based index of the MIDI input device to activate. Must correspond to a valid device available on
        /// the system.</param>
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

        /// <summary>
        /// Stops all currently playing sounds and resets the playback state of the keyboard.
        /// </summary>
        /// <remarks>Call this method to ensure that all notes are silenced and any playback flags or key
        /// states are cleared. This is typically used when resetting the keyboard or when all keys have been
        /// released.</remarks>
        private void StopAllSounds()
        {
            // Stop alternating playback and reset flags
            isAlternatingPlayingRegularKeyboard = false;
            keyPressed = false;
            pressedKeys.Clear();
            singleNote = 0; // Reset singleNote to ensure no lingering playback
            UnmarkAllButtons(); // Unmark all buttons when a key is released
            StopAllNotesAfterPlaying(); // Stop all notes only when no keys remain
        }
        private void main_window_Deactivate(object sender, EventArgs e)
        {
            if (checkBox_use_keyboard_as_piano.Checked && pressedKeys.Count > 0) // If piano mode is enabled and there are pressed keys
            {
                RemoveAllKeys();
                StopAllSounds();
            }
        }

        private void main_window_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp(); // Open the help manual in the preferred language
        }

        /// <summary>
        /// Opens the user manual in the preferred language using the default web browser.
        /// </summary>
        /// <remarks>The manual is selected based on the application's current language preference. If a
        /// manual is not available for the selected language, the English version is opened by default. This method
        /// launches an external process to display the manual in a web browser.</remarks>
        private void ShowHelp() // Method to open the help manual in the preferred language
        {
            string language = Settings1.Default.preferredLanguage;
            string manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL.md";
            switch (language)
            {
                case "English":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL.md";
                    break;
                case "Deutsch":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-de.md";
                    break;
                case "Français":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-fr.md";
                    break;
                case "Español":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-es.md";
                    break;
                case "Italiano":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-it.md";
                    break;
                case "Türkçe":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-tr.md";
                    break;
                case "Русский":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-ru.md";
                    break;
                case "українська":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-ukr.md";
                    break;
                case "Tiếng Việt":
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL-vi.md";
                    break;
                default:
                    manualLink = "https://github.com/GeniusPilot2016/NeoBleeper/blob/master/docs/MANUAL.md";
                    break;
            }
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(manualLink) { UseShellExecute = true });
        }
        private void button_use_voice_system_help_Click(object sender, EventArgs e)
        {
            StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
            MessageForm.Show(this, Resources.UseVoiceSystemHelp, Resources.UseVoiceSystemHelpTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void checkBox_use_voice_system_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_use_voice_system.Checked == true)
            {
                OpenVoiceInternalsWindow();
                Logger.Log("Voice internals window is opened.", Logger.LogTypes.Info);
            }
            else if (checkBox_use_voice_system.Checked == false)
            {
                await StopAllVoices();
                CloseVoiceInternalsWindow();
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

        private void listViewNotes_MouseDown(object sender, MouseEventArgs e)
        {
            rightClicked = e.Button == MouseButtons.Right;
        }

        private void listViewNotes_MouseUp(object sender, MouseEventArgs e)
        {
            rightClicked = false;
        }

        private void main_window_Shown(object sender, EventArgs e)
        {
            if (Program.filePath != null)
            {
                OpenFiles(Program.filePath, FileOpenMode.OpenedAsArg);
            }
        }

        private void convertToBeepCommandForLinuxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewNotes.Items.Count == 0)
            {
                MessageForm.Show(this, Resources.MessageEmptyNoteListCannotBeExportedAsLinuxBeep, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                StopPlayingAllSounds(); // Stop all sounds before opening all modal dialogs or creating a new file
                CloseAllOpenWindows();
                ConvertToBeepCommandForLinux convertToBeepCommandForLinux = new ConvertToBeepCommandForLinux(ConvertToNBPMLString(), this);
                convertToBeepCommandForLinux.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Log("Error converting to Beep command for Linux: " + ex.Message, Logger.LogTypes.Error);
                MessageForm.Show(this, Resources.MessageLinuxBeepCommandConvertError + ex.Message, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        int previous_time_signature = Variables.timeSignature;

        /// <summary>
        /// Prepares the object for a time signature change by marking the value as being modified and storing the
        /// current time signature.
        /// </summary>
        private void SetTimeSignatureValueChanging()
        {
            variableIsChanging = true;
            previous_time_signature = Variables.timeSignature;
        }

        /// <summary>
        /// Handles changes to the time signature value and updates the application state accordingly.
        /// </summary>
        /// <remarks>This method should be called after the time signature value is modified to ensure
        /// that changes are tracked and the user interface reflects the current state. It records the change for
        /// undo/redo functionality and updates the form title to indicate unsaved changes.</remarks>
        private void SetTimeSignatureValueChanged()
        {
            variableIsChanging = false;
            int current_time_signature = Variables.timeSignature;
            if (current_time_signature != previous_time_signature)
            {
                var command = new ValueChangeCommand(
                        "time_signature",
                        previous_time_signature,
                        current_time_signature,
                        trackBar_time_signature,
                        lbl_time_signature);

                commandManager.ExecuteCommand(command);
                isModified = true;
                UpdateFormTitle();
            }
        }
        private void trackBar_time_signature_MouseDown(object sender, MouseEventArgs e)
        {
            SetTimeSignatureValueChanging();
        }

        private void trackBar_time_signature_MouseUp(object sender, MouseEventArgs e)
        {
            SetTimeSignatureValueChanged();
        }
        double previousNoteSilenceRatio = Variables.noteSilenceRatio;
        bool variableIsChanging = false;

        /// <summary>
        /// Marks the note silence ratio value as being in the process of changing.
        /// </summary>
        /// <remarks>Call this method before updating the note silence ratio to indicate that a change is
        /// in progress. This can be used to prevent triggering events or logic that should only occur after the value
        /// has been finalized.</remarks>
        private void SetNoteSilenceValueChanging()
        {
            variableIsChanging = true;
            previousNoteSilenceRatio = Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100;

        }

        /// <summary>
        /// Handles changes to the note silence ratio value and updates the application state accordingly.
        /// </summary>
        /// <remarks>This method should be called when the value of the note silence ratio track bar is
        /// modified by the user. It ensures that changes are tracked and the undo/redo command stack is updated if the
        /// value has changed.</remarks>
        private void SetNoteSilenceValueChanged()
        {
            variableIsChanging = false;
            double currentNoteSilenceRatio = (Convert.ToDouble(trackBar_note_silence_ratio.Value) / 100);
            if (currentNoteSilenceRatio != previousNoteSilenceRatio)
            {
                var command = new ValueChangeCommand(
                        "note_silence_ratio",
                        previousNoteSilenceRatio,
                        currentNoteSilenceRatio,
                        trackBar_note_silence_ratio,
                        true, lbl_note_silence_ratio);
                commandManager.ExecuteCommand(command);
                isModified = true;
                UpdateFormTitle();
            }
        }
        private void trackBar_note_silence_ratio_MouseDown(object sender, MouseEventArgs e)
        {
            SetNoteSilenceValueChanging();
        }

        private void trackBar_note_silence_ratio_MouseUp(object sender, MouseEventArgs e)
        {
            SetNoteSilenceValueChanged();
        }

        private void notifyIconNeoBleeper_BalloonTipClicked(object sender, EventArgs e)
        {
            NotificationUtils.ActivateWindowWhenShownIconIsClicked(); // Activate the main window when the notification is clicked
        }

        private void MIDIRestartBeepTimer_Tick(object sender, EventArgs e)
        {
            if (TemporarySettings.MIDIDevices.useMIDIinput && MIDIIOUtils._midiIn != null)
            {
                if (activeMidiNotes.Count == 1)
                {
                    int midiNote = activeMidiNotes[0];
                    int frequency = MIDIIOUtils.MidiNoteToFrequency(midiNote);
                    RestartBeepIfMutedEarly(GetFrequencyFromKeyCode(frequency));
                }
            }
        }
    }
}