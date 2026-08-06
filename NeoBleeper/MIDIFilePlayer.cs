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
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static UIHelper;

namespace NeoBleeper
{
    /* The MIDI file player of NeoBleeper application, which plays MIDI files using system speaker
        and can show lyrics overlay if the MIDI file contains lyrics.*/

    /* Note: Notes are alternated like telephone ringing or playing one of notes if multiple notes are held
    and it may cause crackling sound in some systems that uses piezo buzzer for system speaker.
    This is because the system speaker can only play one note at a time.
    Also, percussions are playing as PWM-like noise to simulate the sound of percussions [inspired from BaWaMI by Robbi-985 (aka SomethingUnreal)].*/
    public partial class MIDIFilePlayer : Form
    {
        bool darkTheme = false;
        private bool _isAlternatingPlayback = false;
        bool isPlaying = false;
        private List<int> _displayOrder = new List<int>();
        private List<(long time, double cumulativeMs)> _precomputedTempoTimes;
        private long _playbackStartTime;
        private long _nextFrameTime;
        private bool _isStopping = false;
        private readonly object _stopTaskLock = new object();
        private Task _activeStopTask = Task.CompletedTask;
        private bool _isCompletingPlayback = false;
        private bool _playRequestedAfterCompletion = false;
        private bool _isUpdatingLabels = false;
        private MidiFile _midiFile;
        private Stopwatch _playbackStopwatch;
        private LyricsOverlay lyricsOverlay;
        private SysExDisplayEmulator sysExDisplayEmulator;
        private readonly object _playbackRestartTimerLock = new object();
        public MIDIFilePlayer(string filename, Form owner)
        {
            InitializeComponent();
            this.Owner = owner;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            PowerManager.SystemSleeping += PowerManager_SystemSleeping;
            PowerManager.PreparingToShutdown += PowerManager_PreparingToShutdown;
            PowerManager.PreparingToLogoff += PowerManager_PreparingToLogoff;
            PowerManager.SystemHibernating += PowerManager_SystemHibernating;
            PowerManager.Logoff += PowerManager_Logoff;
            PowerManager.Shutdown += PowerManager_Shutdown;
            typeof(Panel).InvokeMember("DoubleBuffered",
        BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
        null, panel1, new object[] { true });
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            UIFonts.SetFonts(this);
            SetTheme();
            _playbackStopwatch = new Stopwatch();
            textBox1.Text = filename;
            LoadMIDI(filename);
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
        private void PowerManager_Shutdown(object? sender, EventArgs e)
        {
            // Handle actual shutdown
            StopImmediately(); // Stop playing MIDI file and notes immediately
            Application.Exit();
        }

        private void PowerManager_Logoff(object? sender, EventArgs e)
        {
            // Handle actual logoff
            StopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_SystemHibernating(object? sender, EventArgs e)
        {
            // Handle system sleep/hibernate
            StopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_PreparingToLogoff(object? sender, EventArgs e)
        {
            // Handle logoff preparation
            StopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_SystemSleeping(object? sender, EventArgs e)
        {
            // Handle system sleep/hibernate
            StopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_PreparingToShutdown(object? sender, EventArgs e)
        {
            // Handle shutdown preparation
            StopImmediately(); // Stop playing MIDI file and notes immediately
            Application.Exit();
        }

        private void StopNotesImmediately()
        {
            NotePlayer.StopAllNotes();
            NotePlayer.StopMicrocontrollerSound();
            MIDIIOUtils.StopAllNotes();
        }

        /// <summary>
        /// Stops all processing and halts any ongoing operations immediately.
        /// </summary>
        private void StopImmediately()
        {
            Stop();
            StopNotesImmediately();
        }

        /// <summary>
        /// Applies the current application theme to the user interface based on user or system settings.
        /// </summary>
        /// <remarks>This method updates the UI to reflect the selected theme, such as light or dark mode,
        /// according to application settings or the system's theme preference. It should be called when the theme needs
        /// to be refreshed, such as after a settings change.</remarks>
        private void SetTheme()
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
                this.ResumeLayout();
            }
            UIHelper.SetFormBackgroundFluent(this, darkTheme);
        }
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            textBox1.BackColor = Color.Black;
            textBox1.ForeColor = Color.White;
            groupBox1.ForeColor = Color.White;
            button_browse_file.BackColor = Color.FromArgb(32, 32, 32);
            numericUpDown_alternating_note.BackColor = Color.Black;
            numericUpDown_alternating_note.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }

        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            textBox1.BackColor = SystemColors.Window;
            textBox1.ForeColor = SystemColors.WindowText;
            groupBox1.ForeColor = SystemColors.ControlText;
            button_browse_file.BackColor = Color.Transparent;
            numericUpDown_alternating_note.BackColor = SystemColors.Window;
            numericUpDown_alternating_note.ForeColor = SystemColors.WindowText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Stop();
            openFileDialog.FileName = MainWindow.lastOpenedMIDIFileName;
            MainWindow.SetFallbackInitialFolderForOpenFileDialog(openFileDialog);
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (MIDIFileValidator.IsMidiFile(openFileDialog.FileName))
                {
                    Action action = async () =>
                    {
                        MainWindow.lastOpenedMIDIFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        textBox1.Text = openFileDialog.FileName;
                        await LoadMIDI(openFileDialog.FileName);
                    };
                    MainWindow.DoActionIfFileIsExist(openFileDialog.FileName, this, action);
                }
                else
                {
                    MessageForm.Show(this, Resources.MessageNonValidMIDIFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logger.Log("This file is not a valid MIDI file or the file is corrupted.", Logger.LogTypes.Error);
                }
            }
        }

        private void MIDI_file_player_DragEnter(object sender, DragEventArgs e)
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

        private async void MIDI_file_player_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    string fileName = files[0];
                    if (MIDIFileValidator.IsMidiFile(fileName))
                    {
                        textBox1.Text = fileName;
                        await LoadMIDI(fileName);
                    }
                    else
                    {
                        Logger.Log("The file you dragged is not supported by NeoBleeper MIDI player or is corrupted.", Logger.LogTypes.Error);
                        MessageForm.Show(this, Resources.MessageMIDIFilePlayerNonSupportedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    Logger.Log("The file you dragged is corrupted or the file is in use by another process.", Logger.LogTypes.Error);
                    MessageForm.Show(this, Resources.MessageCorruptedOrCurrentlyUsedDraggedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Extracts and renders raw lyric texts from hardware streams while stripping hardware control commands,
        /// formatting double quotes, and preserving syllable/word boundaries cleanly.
        /// </summary>
        /// <param name="lyricChunk">The raw text string directly from the MIDI parser.</param>
        /// <returns>A beautifully formatted lyric string free of hardware commands.</returns>
        private string SanitizeLyricQuotes(string lyricChunk)
        {
            if (string.IsNullOrEmpty(lyricChunk)) return string.Empty;

            string result = lyricChunk;

            // ==========================================
            // 0. CONTROL CHARACTER & HARDWARE NOISE CLEANUP
            // ==========================================
            // Strip non-printable control characters (\x00-\x1F, \x7F-\x9F) except standard whitespace
            result = Regex.Replace(result, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F]", "");

            // ==========================================
            // 1. HARDWARE STREAM COMMAND EXTRACTION
            // ==========================================
            // Strip "Display page [n]", "Display page 1", "Display page n", etc.
            result = Regex.Replace(result, @"Display\s+page\s*\[?[\dn]+\]?", "", RegexOptions.IgnoreCase);

            // Strips out "FD:", "FD: Page X", and "FD: Page X:" patterns wherever they occur
            result = Regex.Replace(result, @"FD:\s*(Page\s*\d+\s*:?)?", "", RegexOptions.IgnoreCase);

            // Clean out hardware display style bracket toggles (e.g., "(WOB)", "(BOW)", "(Ch01)")
            result = Regex.Replace(result, @"\([A-Z0-9]{3,4}\)", "", RegexOptions.IgnoreCase);

            // Strip hardware display status tags/indices (e.g., "<P05>", "<02>")
            result = Regex.Replace(result, @"<[^>]+>", "");

            // Strip residual hardware store instructions (e.g., "P1: Store+Display", "P2: Store")
            result = Regex.Replace(result, @"P\d+\s*:?\s*Store(\+Display)?", "", RegexOptions.IgnoreCase);

            // Strip channel program change telemetry (e.g., "Update prog of ch 10 (drum 65)")
            result = Regex.Replace(result, @"Update\s+prog\s+of\s+ch\s*\d+(\s*\([^)]*\))?", "", RegexOptions.IgnoreCase);

            // Strip Sequencer setup markers and FX cues (e.g., "Live-recorded setup...", "OD 2: Level - > 90")
            result = Regex.Replace(result, @"Live-recorded\s+setup[^\n\r]*", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\bOD\s*\d+\s*:\s*Level\s*-\s*>\s*\d+", "", RegexOptions.IgnoreCase);

            // Remove residual hardware flags (e.g. standalone "UF" flags from FD displays)
            result = Regex.Replace(result, @"\bUF\b", "", RegexOptions.IgnoreCase);

            // ==========================================
            // 2. UNIFIED QUOTES & WHITESPACE NORMALIZATION
            // ==========================================

            // Remove structural double quote characters
            result = result.Replace("\"", "");

            // Collapse excessive internal tab/space sequences while preserving boundary spacing
            result = Regex.Replace(result, @"[ \t]+", " ");

            return result;
        }

        // Class-level variables for controlling playback
        private CancellationTokenSource _cancellationTokenSource;
        private List<(long Time, HashSet<int> ActiveNotes)> _frames;
        private double _ticksToMs;
        private int _currentFrameIndex = 0;
        private bool _isPlaying = false;
        private string _currentFileName;
        private HashSet<int> _enabledChannels = new HashSet<int>();
        private HashSet<(int NoteNumber, long Time)> _rearticulatedNotes = new HashSet<(int, long)>();

        // Method to update enabled channels based on checkbox states

        /// <summary>
        /// Updates the collection of enabled channels based on the current state of channel checkboxes in the user
        /// interface.
        /// </summary>
        /// <remarks>This method clears the existing list of enabled channels and repopulates it by
        /// checking which channel checkboxes are selected. It is typically called after the user modifies channel
        /// selections in the UI. The method also logs the updated list of enabled channels for informational
        /// purposes.</remarks>
        private void UpdateEnabledChannels()
        {
            _enabledChannels.Clear();

            // Check each channel checkbox
            for (int i = 1; i <= 16; i++)
            {
                var checkBox = Controls.Find($"checkBox_channel_{i}", true).FirstOrDefault() as CheckBox;
                if (checkBox != null && checkBox.Checked)
                {
                    _enabledChannels.Add(i);
                }
            }
            if (isDeciding)
            {
                return; // Don't log in deciding process
            }
            Logger.Log($"Enabled channels: {string.Join(", ", _enabledChannels)}", Logger.LogTypes.Info);
        }

        private int lyricsChunkCount = 0; // Count of lyric chunks processed for display
        private int sysExEventCount = 0; // Count of SysEx events processed for display
        /// <summary>
        /// Determines whether a given text chunk from a MIDI file is considered "junk" or hardware noise and should be filtered out.
        /// Evaluates hardware commands, telemetry data, display events, and non-lyric control parameters.
        /// </summary>
        /// <param name="textChunk">The raw text event string extracted from the MIDI file.</param>
        /// <returns>True if the text chunk is considered junk or a hardware command; otherwise, false.</returns>
        private bool IsTextEventJunk(string textChunk)
        {
            if (string.IsNullOrWhiteSpace(textChunk)) return true;

            string trimmed = textChunk.Trim();

            // ==========================================
            // EVALUATE DISPLAY / FD LINES FOR EMPTY FIELDS & HARDWARE COMMANDS
            // Handles: "Display page [n]", "Display page 1", "Display page", "page [n]", "page 1", etc.
            // ==========================================
            if (Regex.IsMatch(trimmed, @"^(Display\s+)?page(\s*\[?[\dn]+\]?)?$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"^FD:\s*Page\s*\d+\s*:\s*UF$", RegexOptions.IgnoreCase) ||
                trimmed.Equals("FD: Page 5: UF", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // FD is a hardware frame-display channel, not a lyric channel.
            // Some files mirror the text written by SysEx as an FD meta event;
            // allowing its payload through makes display text leak into lyrics.
            if (trimmed.StartsWith("FD:", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // ==========================================
            // 1. PREFIX & SYMBOL CONTROL TAGS
            // ==========================================
            // Per the Soft Karaoke (.kar) text-event convention, a leading "@" always introduces
            // file/header metadata (e.g. "@KMIDI KARAOKE FILE", "@T title", "@LENGL" language) -
            // never a sung syllable - so these are always junk.
            if (trimmed.StartsWith("@"))
                return true;

            // A leading "\" or "/" denotes screen/line breaks; keep these events as valid structural lyric markers
            if (trimmed.StartsWith("\\") || trimmed.StartsWith("/"))
            {
                return false;
            }

            // A line that is *entirely* a bracketed stage direction (e.g. "[Chorus]", "[Instrumental]") is an annotation
            if (Regex.IsMatch(trimmed, @"^\[[^\]]*\]$"))
                return true;


            // ==========================================
            // 2. DYNAMIC TELEMETRY, HARDWARE & MIXER REGEX FILTERS
            // ==========================================

            // Pattern A: Filters MIDI Multi-Channel parameter updates (e.g., "(Ch03) G L Muted: IFX Off")
            if (Regex.IsMatch(trimmed, @"^\(Ch\d+\)", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern B: Filters CC / NRPN / Port / Controller setups
            if (Regex.IsMatch(trimmed, @"\b(CC|CC#|NRPN|RPN|Port|MidiOut|MidiIn)\s*\d+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern C: Filters dynamic hardware delay assignments and signal levels (e.g., "Alt V Shouts L: Delay 127")
            if (Regex.IsMatch(trimmed, @"\bDelay\s*\d+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern D: Filters hardware memory store allocations (e.g., "Store  2: kisushisu", "Store  6 Music")
            if (Regex.IsMatch(trimmed, @"^Store\s*\d+.*", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern E: Filters hardware display directories (e.g., "Display FD3", "Display no FD")
            if (Regex.IsMatch(trimmed, @"^Display\s+no\s+FD$", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"^Display\s+FD\d+$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern F: Filters real-time envelope release parameter updates (e.g., "Release: Momentarily drop to 64")
            if (trimmed.StartsWith("Release:", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(trimmed, @"^Release\s*:\s*.+\d+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern G: Filters instrument receiver state logs (e.g., "Crash2 - Rx.Note Off = 0")
            if (Regex.IsMatch(trimmed, @"-\s*Rx\.Note\s*Off\s*=\s*\d+", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"-\s*(Rx|Tx)\.\w+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern H: Filters controller layout reset events (e.g., "Spread reset: Atk, Rel, Pan, Exp")
            if (Regex.IsMatch(trimmed, @"^Spread\s+reset\s*:", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern I: General variants of "Display page [n]", "Display page 1", "Display no page"
            if (Regex.IsMatch(trimmed, @"\b(Display|display)\s+(page|no\s+page)(\s*\[?[\dn]+\]?)?", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern J: Target hybrid graphics pipeline and storage modifications
            if (Regex.IsMatch(trimmed, @"Store\s*\+?\s*display\s+graphic\s+page\s*\d*", RegexOptions.IgnoreCase) ||
                trimmed.Contains("display graphic", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Pattern K: Step/Tracking parameters for pads (e.g., "Shoe 1/4 <P05>")
            if (Regex.IsMatch(trimmed, @"^(Shoe|Hand|Foot|Leg|Arm)\s+\d+/\d+\s*<[^>]+>", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern L: Hardware mapping assignments (e.g., "D1 P1 1/2 <02>")
            if (Regex.IsMatch(trimmed, @"^D\d+\s+P\d+.*<\d+>", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"^D\d+\s+P\d+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern M: Initialization Routines and Module Tags (e.g., "Init Bell Synth")
            if (trimmed.StartsWith("Init", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(trimmed, @"^Init\s*.+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern N: Parameter data-binding streams and channel mod routes (e.g., "Drum 1 Note 27 Pan -> 64")
            if (Regex.IsMatch(trimmed, @"Drum\s*\d+\s*Note\s*\d+.+->\s*\d+", RegexOptions.IgnoreCase) ||
                trimmed.Contains("->"))
            {
                return true;
            }

            // Pattern O: Target standalone visual instruction fallbacks
            if (trimmed.Equals("No Display", StringComparison.OrdinalIgnoreCase) ||
                Regex.IsMatch(trimmed, @"\bNo\s+Display\b", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern P: Continuous Visual/Layout Loops (e.g., "P2 DisplayP3 Display")
            if (Regex.IsMatch(trimmed, @"(isplay|display|\bno\b|\bdisp\b)?\s*parts?\s*\d*", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"(P\d+\s*Display)+", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"\bNo\s*DisplayPart\b", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern Q: Custom Hardware Patch & Storing states (e.g. "P1: Store+Display", "P2 Store:")
            if (Regex.IsMatch(trimmed, @"P\d+\s*:?\s*Store(\+Display)?", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"\bStore\+Display\b", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"\bStore\s*:\s*\w+", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern R: Sequencer Directive Strings
            if (Regex.IsMatch(trimmed, @"-\s*Skip\s*!", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern S: Automatic Unlabeled Track Titles or Blank Placeholders
            if (Regex.IsMatch(trimmed, @"^(Track|Trk|Channel|Chan|Inst|Instrument|Midi\s*Track|Drum|Percussion|Synth|Vocal|Melody)\s*\d*$", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern T: Raw Hex blocks or isolated numeric hardware register indicators
            if (Regex.IsMatch(trimmed, @"^[0-9A-Fa-f\s\-\:\#]+$") && trimmed.Length > 3)
                return true;

            // Pattern U: Program / Channel Change Hardware Telemetry (e.g., "Update prog of ch 10 (drum 65)")
            if (Regex.IsMatch(trimmed, @"\bUpdate\s+prog(\s+of\s+ch\s*\d+)?\b", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern V: Live-recorded setup blocks (e.g. "Live-recorded setup (minus reset) - START")
            if (Regex.IsMatch(trimmed, @"\bLive-recorded\s+setup\b", RegexOptions.IgnoreCase))
            {
                return true;
            }

            // Pattern W: Patch/FX Initializations & State Post-Markers (e.g. "Solo init (Main)", "Glockenspiel AFTER", "Solo AFTER (Harm)")
            if (Regex.IsMatch(trimmed, @"\b(init|AFTER)\s*(\([^)]*\))?$", RegexOptions.None) ||
                Regex.IsMatch(trimmed, @"\b(init|AFTER)\b", RegexOptions.IgnoreCase) && (trimmed.Contains("Solo") || trimmed.Contains("Glockenspiel") || trimmed.Contains("overdrive")))
            {
                return true;
            }

            // Pattern X: Overdrive SFX & Level Telemetry (e.g. "OD 2: Level - > 90", "Overdrive SFX - Right (lower)")
            if (Regex.IsMatch(trimmed, @"\b(OD|Overdrive)\s*(SFX|\d+)?\b", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"\bLevel\s*-\s*>\s*\d+\b", RegexOptions.IgnoreCase))
            {
                return true;
            }


            // ==========================================
            // 3. TECHNICAL & SYNTHESIZER SETUP KEYWORDS
            // ==========================================
            string[] strictJunkKeywords = {
        "Microsoft Wavetable", "GS Reset", "XG System On", "GM System On", "GM2 System On",
        "Roland GS", "Yamaha XG", "SoundBlaster", "AWE32", "AWE64", "Creative Labs",
        "QuickTime Music", "CoreAudio", "DirectMusic", "HyperSound", "SC-55", "SC-88",
        "Start of Setup", "End of Setup", "SysEx", "System Exclusive", "NRPN", "RPN",
        "Control Change", "Parameter", "LCD:", "GSAE", "Bank Select", "Program Change",
        "Channel Volume", "Velocity", "Pitch Bend", "Aftertouch", "Modulation Wheel",
        "Expression", "Sustain", "Panpot", "Reverb Send", "Chorus Send",
        "screen layout", "view mode", "window status", "resolution:", "tempo map",
        "time sig", "key sig", "marker:", "cue:", "frame:", "smpte", "samplerate",
        "Cakewalk", "Sonar", "Cubase", "Nuendo", "Logic Audio", "Anvil Studio",
        "Pro Tools", "Ableton", "FL Studio", "FruityLoops", "Guitar Pro", "TuxGuitar",
        "Rosegarden", "MuseScore", "Sibelius", "Finale", "REAPER", "Studio One",
        "pan left", "pan right", "pan centre", "no chorus", "no resonance", "White noise",
        "Normal attack", "Slow attack", "Enable EFX", "EFX Level", "Big Shot",
        "Random pan", "Non-random pan", "Update prog", "Stereo overdrive"
    };

            foreach (var keyword in strictJunkKeywords)
            {
                if (trimmed.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }


            // ==========================================
            // 4. METADATA, CREDITS, & FILE ATTRIBUTES
            // ==========================================
            string[] metadataPrefixes = {
        "Sequenced by", "Sequenced", "Arranged by", "Arranged", "Composed by", "Composer",
        "Copyright", "(c)", "Author", "Written by", "Lyrics by", "Performer", "Artist",
        "SoundFont", "Soundfont Bank", "Patch Name", "Instrument Name", "Program Name",
        "File:", "Path:", "URL:", "http:", "https:", "www.", "Email:", "Mail:",
        "Created with", "Generated by", "Converted by", "Encoded by", "Downloaded from"
    };

            foreach (var prefix in metadataPrefixes)
            {
                if (trimmed.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes MIDI lyric/text events that duplicate Roland display-letter
        /// SysEx. The matching is deliberately narrow: recognized model-45
        /// text writes only. Dot graphics and unrelated text remain untouched.
        /// </summary>
        private void RemoveSysExDisplayTextFromLyrics(
            Dictionary<long, List<MetaEvent>> metaEvents,
            Dictionary<long, List<byte[]>> sysExEvents)
        {
            if (metaEvents == null || metaEvents.Count == 0 ||
                sysExEvents == null || sysExEvents.Count == 0)
            {
                return;
            }

            var signaturesByTick =
                new Dictionary<long, HashSet<string>>();
            var allDisplayTextSignatures =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var decoder = new RolandGSStyleDisplayDecoder();

            foreach (long tick in sysExEvents.Keys.OrderBy(value => value))
            {
                if (!sysExEvents.TryGetValue(tick, out var messages))
                {
                    continue;
                }

                foreach (byte[] message in messages)
                {
                    // Keep both the bytes written by this packet and the complete
                    // display buffer after applying it. Roland files often split a
                    // single caption over several address writes.
                    if (RolandGSStyleDisplayDecoder.TryGetDisplayTextWrite(
                            message,
                            out _,
                            out string writtenText))
                    {
                        AddSysExTextSignature(
                            signaturesByTick,
                            allDisplayTextSignatures,
                            tick,
                            writtenText);
                    }

                    if (decoder.Apply(
                            message,
                            out _,
                            out _,
                            out bool textChanged) &&
                        textChanged)
                    {
                        AddSysExTextSignature(
                            signaturesByTick,
                            allDisplayTextSignatures,
                            tick,
                            decoder.DisplayedText);
                    }
                }
            }

            if (signaturesByTick.Count == 0)
            {
                return;
            }

            // Duplicate meta events are not always stamped at the exact SysEx
            // tick. Permit a small sequencer-quantization window, while keeping
            // matching local enough not to remove genuine lyrics elsewhere.
            long nearbyTickTolerance = Math.Max(
                1L,
                _ticksPerQuarterNote > 0
                    ? _ticksPerQuarterNote / 16L
                    : 1L);

            long[] displayTextTicks =
                signaturesByTick.Keys.OrderBy(value => value).ToArray();

            foreach (long tick in metaEvents.Keys.ToList())
            {
                List<MetaEvent> eventsAtTick = metaEvents[tick];
                HashSet<string> nearbySignatures =
                    GetNearbySysExTextSignatures(
                        signaturesByTick,
                        displayTextTicks,
                        tick,
                        nearbyTickTolerance);

                eventsAtTick.RemoveAll(metaEvent =>
                {
                    string rawText = ExtractLyricsFromMetaEvent(metaEvent);
                    string normalizedText =
                        NormalizeSysExComparableText(rawText);
                    string normalizedDisplayedText =
                        NormalizeSysExComparableText(
                            SanitizeLyricQuotes(rawText));

                    // Nearby display writes are authoritative for both Lyric and
                    // TextEvent entries. Use containment as well as equality:
                    // display text is frequently padded, split into chunks, or
                    // represented as the complete 32-character LCD buffer.
                    if (ContainsSysExTextSignature(
                            nearbySignatures,
                            normalizedText,
                            normalizedDisplayedText,
                            allowPartialMatch: true))
                    {
                        return true;
                    }

                    // Away from the display-write tick, suppress only an exact
                    // generic TextEvent duplicate. This preserves genuine lyric
                    // events that happen to repeat a word shown on the display.
                    return metaEvent.MetaEventType ==
                               MetaEventType.TextEvent &&
                           ContainsSysExTextSignature(
                               allDisplayTextSignatures,
                               normalizedText,
                               normalizedDisplayedText,
                               allowPartialMatch: false);
                });

                if (eventsAtTick.Count == 0)
                {
                    metaEvents.Remove(tick);
                }
            }
        }

        private static HashSet<string> GetNearbySysExTextSignatures(
            Dictionary<long, HashSet<string>> signaturesByTick,
            long[] orderedTicks,
            long targetTick,
            long tolerance)
        {
            var result = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (long displayTick in orderedTicks)
            {
                if (displayTick < targetTick - tolerance)
                {
                    continue;
                }

                if (displayTick > targetTick + tolerance)
                {
                    break;
                }

                if (signaturesByTick.TryGetValue(
                        displayTick,
                        out HashSet<string> signatures))
                {
                    result.UnionWith(signatures);
                }
            }

            return result;
        }

        private static bool ContainsSysExTextSignature(
            HashSet<string> signatures,
            string rawText,
            string displayedText,
            bool allowPartialMatch)
        {
            if (signatures == null || signatures.Count == 0)
            {
                return false;
            }

            foreach (string candidate in new[] { rawText, displayedText })
            {
                if (string.IsNullOrEmpty(candidate))
                {
                    continue;
                }

                if (signatures.Contains(candidate))
                {
                    return true;
                }

                if (!allowPartialMatch)
                {
                    continue;
                }

                foreach (string signature in signatures)
                {
                    if (AreSysExTextVariants(candidate, signature))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool AreSysExTextVariants(
            string metaText,
            string sysExText)
        {
            if (string.IsNullOrEmpty(metaText) ||
                string.IsNullOrEmpty(sysExText))
            {
                return false;
            }

            string compactMeta = CompactSysExComparableText(metaText);
            string compactSysEx = CompactSysExComparableText(sysExText);

            // Avoid treating tiny/common lyric fragments as display duplicates.
            if (compactMeta.Length < 4 || compactSysEx.Length < 4)
            {
                return false;
            }

            return compactMeta.Contains(
                       compactSysEx,
                       StringComparison.OrdinalIgnoreCase) ||
                   compactSysEx.Contains(
                       compactMeta,
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string CompactSysExComparableText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            StringBuilder compact = new StringBuilder(text.Length);
            foreach (char character in text)
            {
                if (char.IsLetterOrDigit(character))
                {
                    compact.Append(char.ToUpperInvariant(character));
                }
            }

            return compact.ToString();
        }

        private static void AddSysExTextSignature(
            Dictionary<long, HashSet<string>> signaturesByTick,
            HashSet<string> allSignatures,
            long tick,
            string text)
        {
            string normalizedText = NormalizeSysExComparableText(text);
            if (normalizedText.Length == 0)
            {
                return;
            }

            if (!signaturesByTick.TryGetValue(
                    tick,
                    out HashSet<string> signatures))
            {
                signatures = new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);
                signaturesByTick[tick] = signatures;
            }

            signatures.Add(normalizedText);
            allSignatures.Add(normalizedText);
        }

        private static string NormalizeSysExComparableText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            StringBuilder normalized = new StringBuilder(text.Length);
            bool previousWasWhitespace = false;

            foreach (char character in text)
            {
                char value = char.IsControl(character) ? ' ' : character;

                if (char.IsWhiteSpace(value))
                {
                    if (!previousWasWhitespace)
                    {
                        normalized.Append(' ');
                        previousWasWhitespace = true;
                    }
                }
                else
                {
                    normalized.Append(value);
                    previousWasWhitespace = false;
                }
            }

            return normalized.ToString().Trim();
        }

        private bool lyricsEnabled = false;
        private bool sysExEmulatorEnabled = false;
        private Dictionary<int, int> _noteChannels = new Dictionary<int, int>();
        private List<(long time, int tempo)> _tempoEvents;
        private int _ticksPerQuarterNote;
        private Dictionary<long, List<MetaEvent>> _metaEventsByTime = new Dictionary<long, List<MetaEvent>>();
        private Dictionary<long, List<MidiEvent>> _eventsByTime = new Dictionary<long, List<MidiEvent>>();
        private Dictionary<long, List<byte[]>> _sysExEventsByTime = new Dictionary<long, List<byte[]>>();
        private List<long> _sysExDisplayEventTimes = new List<long>();
        private int _nextSysExDisplayEventIndex = 0;
        private readonly RolandGSStyleDisplayDecoder _sysExDisplayDecoder = new RolandGSStyleDisplayDecoder();
        private readonly object _sysExDisplayLock = new object();
        private double? _sysExDisplayClearAtMs;
        private bool _hasAppliedSysExDisplayState;

        private static readonly PropertyInfo SysExDataProperty =
            typeof(SysexEvent).GetProperty("Data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ??
            typeof(SysexEvent).GetProperty("Buffer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private static readonly FieldInfo SysExDataField =
            typeof(SysexEvent).GetField("data", BindingFlags.Instance | BindingFlags.NonPublic) ??
            typeof(SysexEvent).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);

        CancellationTokenSource midiFileLoadCts = new CancellationTokenSource();

        /// <summary>
        /// Safely executes UI updates checking for handle availability and form disposal.
        /// </summary>
        private void SafeInvoke(Action action)
        {
            if (IsDisposed || Disposing || !IsHandleCreated) return;
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
            {
                // Form or control was disposed while marshaling to the UI thread
            }
        }

        private HashSet<int> _channelsWithNotes = new HashSet<int>();

        /// <summary>
        /// Asynchronously loads a MIDI file and prepares it for playback and analysis.
        /// </summary>
        /// <remarks>This method resets the current playback state and updates the user interface to
        /// reflect the loading progress. After loading, the MIDI data is parsed and internal structures are initialized
        /// for playback, event analysis, and lyric display. If an error occurs during loading, the playback state is
        /// reset and an error notification is shown. This method is not thread-safe and should be called from the UI
        /// thread.</remarks>
        /// <param name="filename">The path to the MIDI file to load. Must refer to a valid, accessible MIDI file.</param>
        /// <returns>A task that represents the asynchronous load operation.</returns>

        private async Task LoadMIDI(string filename)
        {
            // 1. Cancel previous load operation safely
            try
            {
                midiFileLoadCts?.Cancel();
            }
            catch (ObjectDisposedException) { }

            var loadCts = new CancellationTokenSource();
            midiFileLoadCts = loadCts;
            CancellationToken loadToken = loadCts.Token;

            try
            {
                if (loadToken.IsCancellationRequested) return;

                // 2. Stop active playback before state reset
                await StopAsync();

                if (loadToken.IsCancellationRequested) return;

                // Reset UI indicators safely
                SafeInvoke(() =>
                {
                    sysExEventCount = 0;
                    lyricsChunkCount = 0;
                    DecideCheckboxesThatWillBeEnabled(0, 0, new HashSet<int>(), lyricsEnabled);
                    panelLoading.Visible = true;
                    labelStatus.Text = Resources.TextTheMIDIFileIsBeingLoaded;
                    progressBar1.Value = 0;
                    progressBar1.Maximum = 100;
                    progressBar1.Visible = true;
                });

                _currentFileName = filename;

                // 3. Thread-local isolated staging variables
                MidiFile localMidiFile = null;
                int localTicksPerQuarterNote = 500;
                var localTempoEvents = new List<(long time, int tempo)>();
                var localNoteChannels = new Dictionary<int, int>();
                var localChannelsWithNotes = new HashSet<int>();
                var localEventsByTime = new Dictionary<long, List<MidiEvent>>();
                var localMetaDict = new Dictionary<long, List<MetaEvent>>();
                var localSysExDict = new Dictionary<long, List<byte[]>>();
                var localFrames = new List<(long Time, HashSet<int> ActiveNotes)>();
                var localRearticulatedNotes = new HashSet<(int NoteNumber, long Time)>();
                int localLyricsCount = 0;
                int localSysExCount = 0;

                // 4. Background parsing task with boolean cancellation checks
                await Task.Run(() =>
                {
                    if (loadToken.IsCancellationRequested) return;
                    localMidiFile = new MidiFile(filename, false);
                    localTicksPerQuarterNote = localMidiFile.DeltaTicksPerQuarterNote;

                    foreach (var track in localMidiFile.Events)
                    {
                        if (loadToken.IsCancellationRequested) return;
                        foreach (var midiEvent in track)
                        {
                            if (midiEvent is TempoEvent tempoEvent)
                            {
                                localTempoEvents.Add((tempoEvent.AbsoluteTime, tempoEvent.MicrosecondsPerQuarterNote));
                            }

                            if (!localEventsByTime.TryGetValue(midiEvent.AbsoluteTime, out var eventList))
                            {
                                eventList = new List<MidiEvent>();
                                localEventsByTime[midiEvent.AbsoluteTime] = eventList;
                            }
                            eventList.Add(midiEvent);
                        }
                    }

                    if (!localTempoEvents.Any())
                    {
                        localTempoEvents.Add((0, 500000));
                    }
                    else
                    {
                        localTempoEvents = localTempoEvents.OrderBy(t => t.time).ToList();
                    }

                    SafeUpdateProgressBar(30, Resources.TextMIDIEventsAreBeingCollected, loadToken);
                    var allEvents = new List<(long Time, int NoteNumber, bool IsNoteOn, int Channel)>();
                    int totalTracks = localMidiFile.Events.Tracks;
                    int processedTracks = 0;

                    foreach (var track in localMidiFile.Events)
                    {
                        if (loadToken.IsCancellationRequested) return;

                        List<byte> pendingSysEx = null;
                        long pendingSysExStartTick = 0;
                        long pendingSysExLastTick = 0;

                        foreach (var midiEvent in track)
                        {
                            if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                            {
                                var noteEvent = (NoteOnEvent)midiEvent;
                                allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, noteEvent.Velocity > 0, noteEvent.Channel));
                                localNoteChannels[noteEvent.NoteNumber] = noteEvent.Channel;
                                localChannelsWithNotes.Add(noteEvent.Channel);
                            }
                            else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                            {
                                var noteEvent = (NoteEvent)midiEvent;
                                allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, false, noteEvent.Channel));
                                if (!localNoteChannels.ContainsKey(noteEvent.NoteNumber))
                                {
                                    localNoteChannels[noteEvent.NoteNumber] = noteEvent.Channel;
                                }
                                localChannelsWithNotes.Add(noteEvent.Channel);
                            }
                            else if (midiEvent is SysexEvent sysexEvent)
                            {
                                if (TryExtractSysExData(sysexEvent, out byte[] sysExData))
                                {
                                    CollectSysExFragment(
                                        sysexEvent,
                                        sysExData,
                                        localSysExDict,
                                        ref pendingSysEx,
                                        ref pendingSysExStartTick,
                                        ref pendingSysExLastTick);
                                }
                            }
                            else if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                            {
                                var meta = (MetaEvent)midiEvent;
                                if (meta.MetaEventType == MetaEventType.Lyric || meta.MetaEventType == MetaEventType.TextEvent)
                                {
                                    string rawText = ExtractLyricsFromMetaEvent(meta);
                                    if (!IsTextEventJunk(rawText))
                                    {
                                        if (!localMetaDict.TryGetValue(meta.AbsoluteTime, out var list))
                                        {
                                            list = new List<MetaEvent>();
                                            localMetaDict[meta.AbsoluteTime] = list;
                                        }
                                        list.Add(meta);
                                    }
                                }
                            }
                        }

                        FlushPendingSysExFragment(
                            localSysExDict,
                            ref pendingSysEx,
                            ref pendingSysExStartTick,
                            ref pendingSysExLastTick);

                        processedTracks++;
                        int percent = 30 + (int)(20.0 * processedTracks / Math.Max(1, totalTracks));
                        SafeUpdateProgressBar(percent, $"{Resources.TextEventsAreBeingCollected} ({processedTracks}/{totalTracks})", loadToken);
                    }

                    RemoveSysExDisplayTextFromLyrics(localMetaDict, localSysExDict);
                    localLyricsCount = localMetaDict.Values.Sum(events => events.Count);

                    SafeUpdateProgressBar(55, Resources.TextEventsAreBeingSorted, loadToken);
                    allEvents = allEvents.OrderBy(e => e.Time).ToList();

                    var noteEventsByTime = allEvents.GroupBy(e => e.Time);
                    foreach (var timeGroup in noteEventsByTime)
                    {
                        if (loadToken.IsCancellationRequested) return;
                        var noteOffs = timeGroup.Where(e => !e.IsNoteOn).Select(e => e.NoteNumber).ToHashSet();
                        var noteOns = timeGroup.Where(e => e.IsNoteOn).Select(e => e.NoteNumber).ToHashSet();
                        foreach (var note in noteOffs.Intersect(noteOns))
                        {
                            localRearticulatedNotes.Add((note, timeGroup.Key));
                        }
                    }

                    var timePoints = allEvents.Select(e => e.Time)
                                              .Concat(localMetaDict.Keys)
                                              .Distinct()
                                              .OrderBy(t => t)
                                              .ToList();

                    if (timePoints.Count == 0 &&
                        localSysExDict.Values.SelectMany(events => events).Any(RolandGSStyleDisplayDecoder.AffectsDisplayState))
                    {
                        timePoints.Add(0);
                    }

                    SafeUpdateProgressBar(60, Resources.TextFramesAreBeingCreated, loadToken);
                    HashSet<int> currentlyActiveNotes = new HashSet<int>();
                    int totalTimePoints = timePoints.Count;

                    // Notice the check here: gracefully return without throwing an exception!
                    for (int i = 0; i < totalTimePoints; i++)
                    {
                        if (i % 256 == 0 && loadToken.IsCancellationRequested) return;

                        var time = timePoints[i];
                        foreach (var evt in allEvents.Where(e => e.Time == time))
                        {
                            if (evt.IsNoteOn)
                                currentlyActiveNotes.Add(evt.NoteNumber);
                            else
                                currentlyActiveNotes.Remove(evt.NoteNumber);
                        }
                        localFrames.Add((time, new HashSet<int>(currentlyActiveNotes)));

                        if (i % Math.Max(1, totalTimePoints / 20) == 0)
                        {
                            int percent = 60 + (int)(35.0 * i / totalTimePoints);
                            SafeUpdateProgressBar(percent, $"{Resources.TextFramesAreBeingCreated} ({i + 1}/{totalTimePoints})", loadToken);
                        }
                    }

                    foreach (var key in localSysExDict.Keys.ToList())
                    {
                        if (loadToken.IsCancellationRequested) return;
                        List<byte[]> displayMessages = localSysExDict[key]
                            .Where(RolandGSStyleDisplayDecoder.AffectsDisplayState)
                            .ToList();

                        if (displayMessages.Count == 0)
                            localSysExDict.Remove(key);
                        else
                            localSysExDict[key] = displayMessages;
                    }

                    HashSet<int> dotGraphicsPages = localSysExDict.Values
                        .SelectMany(events => events)
                        .Where(RolandGSStyleDisplayDecoder.ContainsDotGraphics)
                        .SelectMany(RolandGSStyleDisplayDecoder.GetDotGraphicsPagesTouched)
                        .ToHashSet();

                    localSysExCount = dotGraphicsPages.Count;
                });

                // If the task returned early due to cancellation, abort committing state
                if (loadToken.IsCancellationRequested)
                {
                    Logger.Log($"Loading of '{filename}' was safely canceled.", Logger.LogTypes.Info);
                    return;
                }

                // 5. Commit state atomically on UI thread
                _midiFile = localMidiFile;
                _ticksPerQuarterNote = localTicksPerQuarterNote;
                _tempoEvents = localTempoEvents;
                _noteChannels = localNoteChannels;
                _channelsWithNotes = localChannelsWithNotes;
                _eventsByTime = localEventsByTime;
                _metaEventsByTime = localMetaDict;
                _sysExEventsByTime = localSysExDict;
                _sysExDisplayEventTimes = localSysExDict.Keys.OrderBy(t => t).ToList();
                _frames = localFrames;
                _rearticulatedNotes = localRearticulatedNotes;
                lyricsChunkCount = localLyricsCount;
                sysExEventCount = localSysExCount;

                _currentFrameIndex = 0;
                _isPlaying = false;

                SafeInvoke(() =>
                {
                    UpdateEnabledChannels();
                    progressBar1.Value = 100;
                    labelStatus.Text = Resources.TextMIDIFileLoaded;
                    progressBar1.Visible = false;

                    PrecomputeTempoTimes();
                    AssignInstrumentsToNotes(_midiFile);
                    groupBox1.Enabled = true;

                    if (checkBoxShowSysExDisplayEmulator.Checked && sysExDisplayEmulator != null && !sysExDisplayEmulator.IsDisposed)
                    {
                        long displayTick = _frames.Count > 0 ? _frames[0].Time : 0;
                        RebuildSysExDisplayAtTick(displayTick);
                    }
                    else
                    {
                        ClearSysExDisplay();
                    }

                    NotificationUtils.CreateAndShowNotificationIfObscured(this, Resources.NotificationTitleMIDIFileLoaded, Resources.NotificationMessageMIDIFileLoaded, ToolTipIcon.Info, 3000);
                    DecideCheckboxesThatWillBeEnabled(lyricsChunkCount, sysExEventCount, _channelsWithNotes, lyricsEnabled);
                    ResetLabelsAndTrackBar();
                    panelLoading.Visible = false;
                });
            }
            catch (Exception ex)
            {
                // Handling actual unexpected file parsing errors
                SafeInvoke(() =>
                {
                    DecideCheckboxesThatWillBeEnabled(0, 0, new HashSet<int>(), lyricsEnabled);
                    labelStatus.Text = Resources.TextMIDIFileLoadingError;
                    progressBar1.Visible = false;
                    progressBar1.Value = 0;
                    groupBox1.Enabled = false;
                    panelLoading.Visible = false;
                    MessageForm.Show(this, $"{Resources.MessageMIDIFileLoadingError} {ex.Message}");
                });
                _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
                Logger.Log($"Error loading MIDI file: {ex.Message}", Logger.LogTypes.Error);
            }
        }
        private void SafeUpdateProgressBar(int percent, string text, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;
            SafeInvoke(() =>
            {
                if (progressBar1 != null && !progressBar1.IsDisposed)
                {
                    progressBar1.Value = Math.Clamp(percent, progressBar1.Minimum, progressBar1.Maximum);
                }
                if (labelStatus != null && !labelStatus.IsDisposed)
                {
                    labelStatus.Text = text;
                }
            });
        }

        /// <summary>
        /// Determines which checkboxes in the user interface should be enabled based on the presence of lyrics and note channels in the loaded MIDI file.
        /// </summary>
        private void DecideCheckboxesThatWillBeEnabled(int textChunkCount, int sysExEventCount, HashSet<int> channelsWithNotes, bool lyricsEnabled)
        {
            isDeciding = true;
            checkBox_show_lyrics_or_text_events.Enabled = textChunkCount > 0;
            checkBox_show_lyrics_or_text_events.Checked = textChunkCount > 0 && lyricsEnabled;
            checkBoxShowSysExDisplayEmulator.Enabled = sysExEventCount > 0;
            checkBoxShowSysExDisplayEmulator.Checked = sysExEventCount > 0 && sysExEmulatorEnabled;

            foreach (var checkBox in Controls.OfType<CheckBox>().Where(cb => cb.Name.StartsWith("checkBox_channel_")))
            {
                int channelNumber = int.Parse(checkBox.Name.Split('_').Last());
                bool hasNotes = channelsWithNotes.Contains(channelNumber);
                checkBox.Enabled = hasNotes;
                checkBox.Checked = hasNotes;
            }
            isDeciding = false;
        }

        /// <summary>
        /// Updates the progress bar to the specified value and displays the provided status message.
        /// </summary>
        /// <remarks>If called from a thread other than the UI thread, the update is marshaled to the UI
        /// thread automatically.</remarks>
        /// <param name="value">The new value to set for the progress bar. Values greater than 100 are capped at 100.</param>
        /// <param name="status">The status message to display alongside the progress bar.</param>
        private void UpdateProgressBar(int value, string status)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new Action(() =>
                {
                    progressBar1.Value = Math.Min(value, 100);
                    labelStatus.Text = status;
                }));
            }
            else
            {
                progressBar1.Value = Math.Min(value, 100);
                labelStatus.Text = status;
            }
        }

        private double _playbackStartOffsetMs = 0;

        /// <summary>
        /// Begins playback of the loaded frames if playback is not already in progress and frames are available.
        /// </summary>
        /// <remarks>If playback is already active or no frames are loaded, this method has no effect.
        /// Playback is started asynchronously, and any previous playback task is given up to five seconds to complete
        /// before starting a new one. This method enables the stop button and disables the play button upon successful
        /// start. If an error occurs while starting playback, an error message is displayed and playback does not
        /// begin.</remarks>
        public async void Play()
        {
            Logger.Log($"Play called. IsPlaying: {_isPlaying}, Frames count: {_frames?.Count ?? 0}", Logger.LogTypes.Info);

            if (_frames == null || _frames.Count == 0)
                return;

            // Completion cleanup is asynchronous. Do not lose a Play click that
            // arrives after the music has audibly ended but before Stop/Rewind has
            // finished normalizing the state.
            if (_isCompletingPlayback || _isStopping)
            {
                _playRequestedAfterCompletion = true;
                Logger.Log("Play was requested during completion cleanup; restart queued.", Logger.LogTypes.Info);
                return;
            }

            if (_isPlaying)
                return;

            // Be defensive when an earlier completion path left the frame cursor
            // at EOF. A fresh Play click must always start from the beginning.
            if (_currentFrameIndex >= _frames.Count)
            {
                await SetPosition(0.0);
            }

            // Wait for any previous playback task to complete, with a timeout to avoid blocking indefinitely
            if (_playbackTask != null && !_playbackTask.IsCompleted)
            {
                try
                {
                    await Task.WhenAny(_playbackTask, Task.Delay(5000)); // 5-second timeout
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error waiting for previous playback task: {ex.Message}", Logger.LogTypes.Error);
                }
            }

            try
            {
                _isPlaying = true;

                // Improve CTS management: cancel/wait/dispose existing before creating new one
                if (_cancellationTokenSource != null)
                {
                    try
                    {
                        _cancellationTokenSource.Cancel();
                        if (_playbackTask != null && !_playbackTask.IsCompleted)
                        {
                            HandleLyricsDisplay(_frames[_currentFrameIndex].Time); // Update lyrics immediately for current frame
                            await Task.WhenAny(_playbackTask, Task.Delay(1000));
                        }
                    }
                    catch { }
                    finally
                    {
                        try { _cancellationTokenSource.Dispose(); } catch { }
                        _cancellationTokenSource = null;
                    }
                }

                // Reinitialize the cancellation token source
                _cancellationTokenSource = new CancellationTokenSource();

                if (_currentFrameIndex < _frames.Count)
                {
                    _playbackStartOffsetMs = TicksToMilliseconds(_frames[_currentFrameIndex].Time);
                }
                else
                {
                    _playbackStartOffsetMs = 0;
                }
                _playbackStopwatch.Restart();

                playbackTimer.Start();

                Logger.Log("Timer-based playback started successfully", Logger.LogTypes.Info);
                SetPlaybackButtonState(isPlaying: true);
            }
            catch (Exception ex)
            {
                MessageForm.Show(this, $"{Resources.MessagePlaybackStartingError} {ex.Message}");
                _isPlaying = false;
                playbackTimer.Stop();
                _playbackStopwatch?.Stop();
                SetPlaybackButtonState(isPlaying: false);
            }
            finally
            {
                driftMs = 0; // Reset drifts
            }
        }

        /// <summary>
        /// Stops playback and resets the playback state asynchronously.
        /// </summary>
        /// <remarks>If playback is not currently active, this method has no effect. Calling this method
        /// will reset playback-related UI elements, clear held notes and lyrics, and release any resources associated
        /// with playback. This method is asynchronous but returns void; any exceptions that occur during the stop
        /// process are logged and not propagated to the caller.</remarks>
        public async void Stop()
        {
            await StopAsync();
        }

        /// <summary>
        /// Performs the complete stop operation and does not return until the
        /// playback state and controls have been normalized.
        /// </summary>
        private Task StopAsync()
        {
            Logger.Log($"Stop called. IsPlaying: {_isPlaying}", Logger.LogTypes.Info);

            // Every caller must observe the same stop transaction. Returning early
            // while another StopAsync is running lets Rewind/Play mutate the cursor
            // and token source before cleanup has finished, leaving the controls and
            // playback state stuck.
            lock (_stopTaskLock)
            {
                if (!_activeStopTask.IsCompleted)
                {
                    SetPlaybackButtonState(isPlaying: false);
                    return _activeStopTask;
                }

                _activeStopTask = StopCoreAsync();
                return _activeStopTask;
            }
        }

        private async Task StopCoreAsync()
        {
            _isStopping = true;
            try
            {
                playbackTimer.Stop();
                _playbackStopwatch?.Stop();

                _isPlaying = false;
                _isAlternatingPlayback = false;

                // Restore the buttons immediately. Lengthy note cancellation or
                // device cleanup must not leave the form looking as if it plays.
                SetPlaybackButtonState(isPlaying: false);

                _lastLyricTime = DateTime.MinValue;
                _isInLyricSection = false;

                CancellationTokenSource cancellation = _cancellationTokenSource;
                try
                {
                    cancellation?.Cancel();
                }
                catch (ObjectDisposedException)
                {
                    // A concurrent completion path may already have disposed it.
                }

                Task playbackTask = _playbackTask;
                if (playbackTask != null && !playbackTask.IsCompleted)
                {
                    try
                    {
                        await playbackTask;
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected while stopping.
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Playback task ended while stopping: {ex.Message}", Logger.LogTypes.Error);
                    }
                }

                UpdateNoteLabels(new HashSet<int>());
                holded_note_label.Text = $"{Properties.Resources.TextHeldNotes} (0)";
                label_more_notes.Visible = false;
                ClearLyrics();
                ResetSysExDisplayState();
                driftMs = 0;
                MIDIIOUtils.SendNoteOffToAllNotes();
            }
            catch (Exception ex)
            {
                Logger.Log($"Error stopping playback: {ex.Message}", Logger.LogTypes.Error);
            }
            finally
            {
                _isPlaying = false;
                _isAlternatingPlayback = false;
                SetPlaybackButtonState(isPlaying: false);

                try
                {
                    _cancellationTokenSource?.Dispose();
                }
                catch
                {
                }

                _cancellationTokenSource = null;
                _isStopping = false;
            }
        }

        /// <summary>
        /// Keeps the Play and Stop buttons synchronized with the real playback
        /// state. It is safe to call from timer/task completion paths.
        /// </summary>
        private void SetPlaybackButtonState(bool isPlaying)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action<bool>(SetPlaybackButtonState), isPlaying);
                }
                catch (InvalidOperationException)
                {
                    // The form handle was destroyed while playback was ending.
                }

                return;
            }

            button_play.Enabled = !isPlaying;
            button_stop.Enabled = isPlaying;
        }

        // Add this field to track the current playback task
        private Task _playbackTask = Task.CompletedTask;
        private bool _wasPlayingBeforeScroll = false;

        /// <summary>
        /// Sets the current playback position to the specified percentage of the total duration.
        /// </summary>
        /// <remarks>If playback is in progress, it is temporarily stopped while the position is updated.
        /// The method does not resume playback automatically after setting the position. If the specified percentage is
        /// outside the valid range, it is clamped to the nearest valid value.</remarks>
        /// <param name="positionPercent">The desired playback position as a percentage of the total duration. Must be between 0.0 and 100.0, where
        /// 0.0 represents the start and 100.0 represents the end.</param>
        /// <returns>A task that represents the asynchronous operation of setting the playback position.</returns>
        public async Task SetPosition(double positionPercent)
        {
            if (_frames == null || _frames.Count == 0)
                return;

            if (positionPercent < 0.0) positionPercent = 0.0;
            if (positionPercent > 100.0) positionPercent = 100.0;

            // Store playing state
            if (!_wasPlayingBeforeScroll)
                _wasPlayingBeforeScroll = _isPlaying;

            // Stop current playback if any
            if (_isPlaying || _isStopping)
            {
                await StopAsync();
            }

            // Map percent [0..100] -> frame index [0..count-1] with rounding
            int maxIndex = Math.Max(0, _frames.Count - 1);
            _currentFrameIndex = (int)Math.Round((positionPercent / 100.0) * maxIndex);
            _currentFrameIndex = Math.Max(0, Math.Min(_currentFrameIndex, _frames.Count - 1));

            // Reset the playback offset to exact frame time
            _playbackStartOffsetMs = (_currentFrameIndex < _frames.Count)
                ? TicksToMilliseconds(_frames[_currentFrameIndex].Time)
                : 0;

            _playbackStopwatch?.Reset();

            long displayTick = _currentFrameIndex < _frames.Count
                ? _frames[_currentFrameIndex].Time
                : 0;
            RebuildSysExDisplayAtTick(displayTick);

            Logger.Log($"Position set to {positionPercent:0.00}% (frame {_currentFrameIndex} of {_frames.Count}, offset: {_playbackStartOffsetMs:0.00}ms)", Logger.LogTypes.Info);
        }
        // Update note labels with synchronization
        private HashSet<int> _lastDrawnNotes = new HashSet<int>();

        /// <summary>
        /// Updates the note labels displayed in the UI to reflect the specified set of active MIDI notes.
        /// </summary>
        /// <remarks>If the number of active notes exceeds the number of available labels, an additional
        /// indicator is shown to represent the extra notes. The labels are updated only if the set of active notes has
        /// changed since the last update.</remarks>
        /// <param name="activeNotes">A set of MIDI note numbers that are currently active. Each integer represents a MIDI note to be displayed.</param>
        private void UpdateNoteLabelsSync(HashSet<int> activeNotes)
        {
            // The first file can be opened before the form Load event has created
            // the note-label cache. Playback cleanup must therefore be harmless
            // while the controls are still uninitialized.
            if (_noteLabels == null || _noteLabels.Length == 0)
            {
                _lastDrawnNotes = new HashSet<int>(activeNotes ?? new HashSet<int>());
                return;
            }

            activeNotes ??= new HashSet<int>();
            if (_lastDrawnNotes.SetEquals(activeNotes))
                return;

            _lastDrawnNotes = new HashSet<int>(activeNotes);
            panel1.SuspendLayout();

            var sortedNotes = activeNotes.OrderBy(note => note).ToList();
            for (int i = 0; i < _noteLabels.Length; i++)
            {
                Label label = _noteLabels[i];
                if (label == null) continue;

                if (i < sortedNotes.Count)
                {
                    int noteNumber = sortedNotes[i];
                    string noteName = MidiNoteToName(noteNumber);

                    if (!label.Visible) label.Visible = true;
                    if (label.Text != noteName) label.Text = noteName;
                    if (label.BackColor != _highlightColor) label.BackColor = _highlightColor;
                }
                else
                {
                    if (label.Visible) label.Visible = false;
                    if (_originalLabelColors.TryGetValue(label, out var orig))
                    {
                        if (label.BackColor != orig) label.BackColor = orig;
                    }
                    else
                    {
                        // fallback: don't throw
                    }
                    if (!string.IsNullOrEmpty(label.Text)) label.Text = "";
                }
                if (sortedNotes.Count > _noteLabels.Length)
                {
                    label_more_notes.Visible = true;
                    int extraNotes = sortedNotes.Count - _noteLabels.Length;
                    string localizedMoreText = Resources.MoreText.Replace("{number}", extraNotes.ToString());
                    label_more_notes.Text = localizedMoreText;
                }
                else
                {
                    label_more_notes.Visible = false;
                }
            }
            panel1.ResumeLayout();
        }
        bool isDeciding = false;
        private async void checkBox_channel_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledChannels();
            if (isDeciding)
            {
                return; // Don't log in deciding process
            }
            if (_isPlaying)
            {
                if (_frames == null || _frames.Count == 0)
                {
                    Logger.Log("No frames available while changing channels.", Logger.LogTypes.Warning);
                }
                else
                {
                    // Use 0-based denominator so last frame maps to 100%
                    int denom = Math.Max(1, _frames.Count - 1);
                    double currentPositionPercent = ((double)_currentFrameIndex / denom) * 100.0;
                    bool wasPlaying = _isPlaying;
                    await SetPosition(currentPositionPercent);
                    if (wasPlaying && !_isPlaying)
                    {
                        Play();
                    }
                }
            }
            Logger.Log("Channel checkboxes changed", Logger.LogTypes.Info);
        }

        // Play multiple notes alternating

        /// <summary>
        /// Plays multiple musical notes in an alternating sequence for the specified duration, supporting cancellation
        /// via a token.
        /// </summary>
        /// <remarks>If the cancellation token is triggered during playback, the operation stops
        /// immediately and no further notes are played. The playback sequence and timing may be affected by user
        /// interface settings or controls. This method does not throw exceptions for invalid frequencies; ensure all
        /// values are valid before calling.</remarks>
        /// <param name="frequencies">An array of frequencies, in hertz, representing the notes to be played in the alternating sequence. Each
        /// value must be a positive integer.</param>
        /// <param name="duration">The total duration, in milliseconds, for which the notes should be played. Must be a positive integer.</param>
        /// <param name="token">A cancellation token that can be used to cancel the playback operation before completion.</param>
        /// <returns>A task that represents the asynchronous operation of playing the notes. The task completes when playback
        /// finishes or is canceled.</returns>
        private async Task PlayMultipleNotesAsync(int[] frequencies, int duration, CancellationToken token)
        {
            _isAlternatingPlayback = true;
            var noteNumbers = frequencies.Select(freq => FrequencyToNoteNumber(freq)).ToArray();
            var totalStopwatch = Stopwatch.StartNew();

            int interval = checkBox_make_each_cycle_last_30ms.Checked ? 30 : Convert.ToInt32(numericUpDown_alternating_note.Value);
            interval = Math.Max(1, interval);

            try
            {
                if (checkBox_play_each_note.Checked)
                {
                    int cycleDuration = checkBox_make_each_cycle_last_30ms.Checked ? 30 : interval;

                    if (totalStopwatch.ElapsedMilliseconds + cycleDuration > duration)
                        return;

                    int timePerNote = Math.Max(1, cycleDuration / Math.Max(1, frequencies.Length));

                    for (int i = 0; i < frequencies.Length; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        if (totalStopwatch.ElapsedMilliseconds >= duration) break;

                        int notePlayDuration;
                        if (checkBox_make_each_cycle_last_30ms.Checked)
                            notePlayDuration = Math.Min(15, timePerNote);
                        else
                            notePlayDuration = Math.Min(timePerNote, (int)(duration - totalStopwatch.ElapsedMilliseconds));

                        notePlayDuration = Math.Max(1, notePlayDuration);

                        int currentNoteIndex = i;
                        HighlightNoteLabel(currentNoteIndex);
                        // Run the actual sound generation off the calling context but avoid spinning extra Task.Run loops.
                        await Task.Run(() => NotePlayer.PlayNoteWithoutGap(frequencies[currentNoteIndex], notePlayDuration), token);
                        UnHighlightNoteLabel(currentNoteIndex);

                        int gap = Math.Max(0, timePerNote - notePlayDuration);
                        if (gap > 0)
                            await HighPrecisionSleep.SleepAsync(gap);
                    }

                    // Ensure the cycle completes with remaining silence if needed
                    int remainingSilence = Math.Max(0, duration - (int)totalStopwatch.ElapsedMilliseconds);
                    UpdateNoteLabels(new HashSet<int>());
                    if (remainingSilence > 0)
                        await HighPrecisionSleep.SleepAsync(remainingSilence);
                }
                else
                {
                    int notesPerCycle = frequencies.Length;
                    double timePerNote = (double)interval / Math.Max(1, notesPerCycle);
                    int noteIndex = 0;

                    while (totalStopwatch.ElapsedMilliseconds < duration)
                    {
                        token.ThrowIfCancellationRequested();

                        int notePlayDuration;
                        if (checkBox_make_each_cycle_last_30ms.Checked)
                            notePlayDuration = Math.Min(15, Math.Max(1, (int)Math.Round((double)interval / frequencies.Length, MidpointRounding.ToZero)));
                        else
                            notePlayDuration = interval;

                        HighlightNoteLabel(noteIndex);
                        await Task.Run(() => NotePlayer.PlayNoteWithoutGap(frequencies[noteIndex], notePlayDuration), token);
                        UnHighlightNoteLabel(noteIndex);

                        // Wait until next note time (approximate)
                        int wait = Math.Max(0, (int)Math.Round(timePerNote) - notePlayDuration);
                        if (wait > 0)
                            await HighPrecisionSleep.SleepAsync(wait);

                        noteIndex = (noteIndex + 1) % frequencies.Length;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // playback was canceled — let caller handle cleanup
            }
            finally
            {
                totalStopwatch.Stop();
                _isAlternatingPlayback = false;
            }
        }
        private bool _alternatePercussionTurn = false;
        /// <summary>
        /// Alternates the single-voice speaker output between a percussion hit and one or more held
        /// melody notes so that both remain audible within the same frame slot, instead of a drum hit
        /// silencing whatever melody notes are currently held (or vice versa).
        /// </summary>
        /// <param name="frequencies">Frequencies, in hertz, of the currently held melody notes. May be empty if only percussion is due.</param>
        /// <param name="percussion">The percussion instrument to play, or null if there is no drum hit this frame.</param>
        /// <param name="duration">The total duration, in milliseconds, of the frame slot to fill.</param>
        /// <param name="token">A cancellation token that aborts playback early.</param>
        /// <returns>A task that completes when the full duration has been filled.</returns>
        private static PercussionSounds.MidiPercussion? _lastPercussion;
        private static long _lastPercussionTick = 0;
        private static readonly TimeSpan PercussionCooldown = TimeSpan.FromMilliseconds(80); // Prevents re-triggering the same hit across rapid frames

        private async Task PlayNotesAndPercussionAlternatingAsync(
            int[] frequencies,
            PercussionSounds.MidiPercussion? percussion,
            int duration,
            CancellationToken token)
        {
            if (duration <= 0) return;
            frequencies ??= Array.Empty<int>();

            // 1. If no percussion or playback is muted, play melody exclusively.
            if (!percussion.HasValue || TemporarySettings.CreatingSounds.isPlaybackMuted)
            {
                await PlayMelodySliceAsync(frequencies, duration, token).ConfigureAwait(false);
                return;
            }

            // 2. If no melody notes are held, play percussion exclusively.
            if (frequencies.Length == 0)
            {
                await PlayOnlyPercussionAsync(percussion.Value, duration, token).ConfigureAwait(false);
                return;
            }

            // 3. Both melody and percussion are active. 
            // Balance time proportionally so percussion sounds natural (not too short) 
            // and melody never receives a swallowed micro-slice (< 10ms) that causes interruptions.
            int percWindow = Math.Clamp(duration / 2, 20, 35); // Target 20ms to 35ms for a natural percussion hit
            int melodyWindow = duration - percWindow;

            // If the frame is too tight, split 50/50 to guarantee safe minimum slices for both
            if (melodyWindow < 10)
            {
                melodyWindow = duration / 2;
                percWindow = duration - melodyWindow;
            }

            int percussionLabelIndex = -1;
            if (_noteToLabelMap.TryGetValue((int)percussion.Value, out int foundLabel))
            {
                percussionLabelIndex = foundLabel;
                HighlightNoteLabel(percussionLabelIndex);
            }

            try
            {
                if (percWindow > 0)
                {
                    await PercussionSounds.PlayPercussionSliceAsync(percussion.Value, percWindow, token).ConfigureAwait(false);
                }

                if (melodyWindow > 0)
                {
                    await PlayMelodySliceAsync(frequencies, melodyWindow, token).ConfigureAwait(false);
                }
            }
            finally
            {
                if (percussionLabelIndex >= 0)
                {
                    UnHighlightNoteLabel(percussionLabelIndex);
                }
            }
        }

        private async Task PlayOnlyPercussionAsync(PercussionSounds.MidiPercussion percussion, int duration, CancellationToken token)
        {
            int naturalDuration = PercussionSounds.GetNaturalDurationMs(percussion);
            int playDuration = Math.Min(duration, naturalDuration);

            int labelIndex = -1;
            if (_noteToLabelMap.TryGetValue((int)percussion, out int found))
            {
                labelIndex = found;
                HighlightNoteLabel(labelIndex);
            }

            try
            {
                if (playDuration > 0)
                {
                    await PercussionSounds.PlayPercussionSliceAsync(percussion, playDuration, token).ConfigureAwait(false);
                }

                int silence = Math.Max(0, duration - playDuration);
                if (silence > 0)
                {
                    await HighPrecisionSleep.SleepAsync(silence).ConfigureAwait(false);
                }
            }
            finally
            {
                if (labelIndex >= 0)
                    UnHighlightNoteLabel(labelIndex);
            }
        }

        private static int GetSharedPercussionSliceMs(
            PercussionSounds.MidiPercussion percussion,
            int availableFrameMs,
            bool melodyAlsoPlaying)
        {
            if (availableFrameMs <= 0)
                return 0;

            if (!melodyAlsoPlaying)
                return Math.Min(availableFrameMs,
                    PercussionSounds.GetNaturalDurationMs(percussion));

            // Noise cymbals already have a richer wash, while other percussion hits need a
            // slightly longer slice to avoid turning into a barely audible click when melody
            // is also playing. Their sound-device tails continue separately.
            bool cymbal = percussion is
                PercussionSounds.MidiPercussion.HiHatClosed or
                PercussionSounds.MidiPercussion.HiHatFoot or
                PercussionSounds.MidiPercussion.HiHatOpen or
                PercussionSounds.MidiPercussion.CrashCymbal or
                PercussionSounds.MidiPercussion.CrashCymbal2 or
                PercussionSounds.MidiPercussion.RideCymbal or
                PercussionSounds.MidiPercussion.RideCymbal2 or
                PercussionSounds.MidiPercussion.ChinaCymbal or
                PercussionSounds.MidiPercussion.SplashCymbal or
                PercussionSounds.MidiPercussion.RideBell;

            int desired = cymbal ? 12 : 14;
            int melodyReserve = availableFrameMs >= 8 ? 4 : 1;
            return Math.Clamp(
                Math.Min(desired, availableFrameMs - melodyReserve),
                1, availableFrameMs);
        }

        private async Task PlayMelodySliceAsync(
            int[] frequencies,
            int duration,
            CancellationToken token)
        {
            if (duration <= 0)
                return;

            if (frequencies.Length == 0)
            {
                await WaitPreciseWithCancellation(duration, token).ConfigureAwait(false);
            }
            else if (frequencies.Length == 1)
            {
                await Task.Run(
                    () => NotePlayer.PlayNoteWithoutGap(frequencies[0], duration),
                    token).ConfigureAwait(false);
            }
            else
            {
                await PlayMultipleNotesAsync(frequencies, duration, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Converts a frequency in hertz to the corresponding MIDI note number using A4 (440 Hz) as the reference
        /// pitch.
        /// </summary>
        /// <param name="frequency">The frequency, in hertz, to convert. Must be a positive value.</param>
        /// <returns>The MIDI note number that most closely corresponds to the specified frequency.</returns>
        private int FrequencyToNoteNumber(int frequency)
        {
            // Convert frequency to MIDI note number using A4 = 440Hz as reference (MIDI note 69)
            return (int)Math.Round(69 + 12 * Math.Log2(frequency / 440.0));
        }

        /// <summary>
        /// Highlights the label corresponding to the specified note index by changing its background color.
        /// </summary>
        /// <remarks>If called from a thread other than the UI thread, the method automatically marshals
        /// the call to the UI thread. No action is taken if the specified index is out of range.</remarks>
        /// <param name="noteIndex">The zero-based index of the note label to highlight. Must be within the valid range of note labels.</param>
        private void HighlightNoteLabel(int noteIndex)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HighlightNoteLabel(noteIndex)));
                return;
            }

            if (_noteLabels != null && noteIndex >= 0 && noteIndex < _noteLabels.Length)
            {
                Label label = _noteLabels[noteIndex];
                label.BackColor = _highlightColor;
            }
        }

        /// <summary>
        /// Removes the highlight from the note label at the specified index, restoring its original background color.
        /// </summary>
        /// <remarks>If called from a thread other than the UI thread, the operation is marshaled to the
        /// UI thread automatically.</remarks>
        /// <param name="noteIndex">The zero-based index of the note label to unhighlight. Must be within the valid range of note labels.</param>
        private void UnHighlightNoteLabel(int noteIndex)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UnHighlightNoteLabel(noteIndex)));
                return;
            }

            if (_noteLabels != null && noteIndex >= 0 && noteIndex < _noteLabels.Length)
            {
                Label label = _noteLabels[noteIndex];
                if (_originalLabelColors.TryGetValue(label, out var orig))
                {
                    label.BackColor = orig;
                }
            }
        }

        /// <summary>
        /// Converts a MIDI note number to its corresponding frequency in hertz.
        /// </summary>
        /// <remarks>This method assumes equal temperament tuning with A4 set to 440 Hz. Values outside
        /// the typical MIDI note range may produce frequencies outside the standard audible range.</remarks>
        /// <param name="noteNumber">The MIDI note number to convert. Typically ranges from 0 to 127, where 69 represents the standard A4 (440
        /// Hz).</param>
        /// <returns>The frequency in hertz corresponding to the specified MIDI note number, rounded to the nearest integer.</returns>
        private int NoteToFrequency(int noteNumber)
        {
            // MIDI note number to frequency conversion (intentional for system speaker)
            return (int)(880.0 * Math.Pow(2.0, (noteNumber - 69) / 12.0));
        }

        /// <summary>
        /// Updates the displayed time and percentage position labels based on the specified frame index.
        /// </summary>
        /// <remarks>This method updates UI labels to reflect the current playback position in both time
        /// and percentage. If called from a non-UI thread, the update is marshaled to the UI thread. No action is taken
        /// if there are no frames available.</remarks>
        /// <param name="frameIndex">The zero-based index of the frame for which to update the time and percentage position labels. Must be
        /// within the bounds of the available frames.</param>
        private void UpdateTimeAndPercentPosition(int frameIndex)
        {
            if (_frames == null || _frames.Count == 0)
                return;

            long lastTick = _midiFile.Events
                .Select(track => track.LastOrDefault(ev => ev.CommandCode == MidiCommandCode.MetaEvent && ((MetaEvent)ev).MetaEventType == MetaEventType.EndTrack)?.AbsoluteTime ?? 0)
                .Max();

            double currentTimeMs = TicksToMilliseconds(_frames[frameIndex].Time);

            // Use consistent denominator for percent display
            int denom = Math.Max(1, _frames.Count - 1);
            double percent = ((double)frameIndex / denom) * 100.0;

            string timeStr = TimeSpan.FromMilliseconds(currentTimeMs).ToString(@"mm\:ss\.ff", CultureInfo.CurrentCulture);
            string percentagestr = Resources.TextPercent.Replace("{number}", percent.ToString("0.00", CultureInfo.CurrentCulture));

            if (label_percentage.InvokeRequired)
            {
                label_percentage.BeginInvoke(new Action(() =>
                {
                    label_percentage.Text = percentagestr;
                    label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
                }));
            }
            else
            {
                label_percentage.Text = percentagestr;
                label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
            }
        }

        /// <summary>
        /// Resets the playback state and lyric display to the beginning of the track asynchronously.
        /// </summary>
        /// <remarks>If playback was active before rewinding, playback resumes automatically after the
        /// operation completes. All lyric timing and progress indicators are reset to their initial states.</remarks>
        /// <returns>A task that represents the asynchronous rewind operation.</returns>
        private async Task Rewind(bool resumePreviousPlayback = true)
        {
            // Reset lyric state
            _lastLyricTime = DateTime.MinValue;
            _isInLyricSection = false;
            ClearLyrics();
            trackBar1.Value = 0;
            int positionPercent = trackBar1.Value / 10;
            await SetPosition(positionPercent);
            UpdateTimeAndPercentPosition(_currentFrameIndex);
            driftMs = 0; // Reset drifts
            ResetLabelsAndTrackBar();
            bool shouldResume = resumePreviousPlayback && _wasPlayingBeforeScroll;
            _wasPlayingBeforeScroll = false;

            if (shouldResume)
            {
                Play();
            }
            Logger.Log("Rewind completed", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Converts a MIDI note number to its corresponding note name with octave notation.
        /// </summary>
        /// <remarks>The returned note name uses standard Western notation with sharps (e.g., "C#").
        /// Octave numbers follow the convention where MIDI note 60 is C4 (middle C).</remarks>
        /// <param name="noteNumber">The MIDI note number to convert. Must be in the range 0 to 127, where 60 represents middle C (C4).</param>
        /// <returns>A string representing the note name and octave (for example, "C4" or "A#3").</returns>
        private string MidiNoteToName(int noteNumber)
        {
            // Define note names (C, C#, D, D#, E, F, F#, G, G#, A, A#, B)
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            // Calculate the octave (MIDI note 60 is middle C, which is C4)
            int octave = (noteNumber / 12) - 1;

            // Calculate the note name index (0-11) within the octave
            int noteIndex = ((noteNumber % 12) + 12) % 12;

            // Format the note name with its octave
            return $"{noteNames[noteIndex]}{octave + 1}";
        }
        private Dictionary<int, Color> _activeNoteColors = new Dictionary<int, Color>();
        private Color _highlightColor = Settings1.Default.note_indicator_color; // You can choose any color
        private HashSet<int> _previousActiveNotes = new HashSet<int>();

        /// <summary>
        /// Updates the UI labels to reflect the currently active MIDI notes.
        /// </summary>
        /// <remarks>This method updates label visibility, text, and highlighting to match the provided
        /// set of active notes. If the number of active notes exceeds the available labels, an additional label is
        /// shown to indicate the number of extra notes. The update is performed on the UI thread if required.</remarks>
        /// <param name="activeNotes">A set of MIDI note numbers representing the notes that are currently active. Each note number should
        /// correspond to a valid MIDI note.</param>
        private void UpdateNoteLabels(HashSet<int> activeNotes)
        {
            // Opening the first MIDI file can trigger Stop/Reset before
            // InitializeNoteLabels runs in the Load event. Nothing needs to be
            // painted until the label cache exists.
            if (_noteLabels == null || _noteLabels.Length == 0)
            {
                _previousActiveNotes = new HashSet<int>(activeNotes ?? new HashSet<int>());
                return;
            }

            activeNotes ??= new HashSet<int>();
            if (_isUpdatingLabels) return; _isUpdatingLabels = true; try
            { // Sort notes once, outside the UI update action
                var sortedNotes = activeNotes.OrderBy(note => note).ToList();
                Action updateAction = () =>
                { // Reset all labels
                    if (_noteLabels != null)
                    {
                        foreach (var label in _noteLabels)
                        {
                            if (label == null) continue;
                            label.Visible = false;
                            if (_originalLabelColors.TryGetValue(label, out var c))
                                label.BackColor = c;
                        }
                    }
                    // Process active notes with better mapping
                    for (int i = 0; i < Math.Min(sortedNotes.Count, _noteLabels.Length); i++)
                    {
                        int noteNumber = sortedNotes[i];
                        Label label = _noteLabels[i];
                        if (label == null) continue;

                        // Convert note number to note name
                        string noteName = MidiNoteToName(noteNumber);

                        label.Visible = true;
                        label.Text = noteName;
                        label.BackColor = _highlightColor;
                    }

                    // Update more notes label if necessary
                    if (sortedNotes.Count > _noteLabels.Length)
                    {
                        label_more_notes.Visible = true;
                        int extraNotes = sortedNotes.Count - _noteLabels.Length;
                        string localizedMoreText = Resources.MoreText; // ({number} More)
                        localizedMoreText = localizedMoreText.Replace("{number}", extraNotes.ToString());
                        label_more_notes.Text = localizedMoreText;
                    }
                    else
                    {
                        label_more_notes.Visible = false;
                    }
                };

                // Ensure UI update happens on the UI thread
                if (_noteLabels != null && _noteLabels.Length > 0 && _noteLabels[0].InvokeRequired)
                    _noteLabels[0].BeginInvoke(updateAction);
                else
                    updateAction();
            }
            catch (Exception ex)
            {
                Logger.Log($"Error updating note labels: {ex.Message}", Logger.LogTypes.Error);
            }
            finally
            {
                _isUpdatingLabels = false;
            }
        }

        private void button_play_Click(object sender, EventArgs e)
        {
            Play();
        }
        private void button_stop_Click(object sender, EventArgs e)
        {
            Stop();
            _wasPlayingBeforeScroll = false;
        }

        private async void button_rewind_Click(object sender, EventArgs e)
        {
            await Rewind();
        }

        private void MIDI_file_player_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Stop();
                lock (_playbackRestartTimerLock)
                {
                    _playbackRestartTimer?.Stop();
                    _playbackRestartTimer?.Dispose();
                    _playbackRestartTimer = null;
                }
                try { lyricsOverlay?.Dispose(); } catch { }
            }
            catch (Exception ex)
            {
                MessageForm.Show(this, $"{Resources.MessageErrorClosingForm} {ex.Message}");
            }
        }

        private void disable_alternating_notes_panel(object sender, EventArgs e)
        {
            if (checkBox_make_each_cycle_last_30ms.Checked == true)
            {
                panel1.Enabled = false;
                Logger.Log("Play each note or make each cycle last 30 ms checkbox is checked. Disabling the panel.", Logger.LogTypes.Info);
            }
            else
            {
                panel1.Enabled = true;
                Logger.Log("Play each note or make each cycle last 30 ms checkbox is not checked. Enabling the panel.", Logger.LogTypes.Info);
            }
        }

        private bool _isTrackBarBeingDragged = false;
        private DateTime _lastTrackBarScrollTime = DateTime.MinValue;
        private System.Timers.Timer _playbackRestartTimer;

        private bool _isUserScrolling = false; // To seperate user scroll from program scroll

        private CancellationTokenSource _scrollDebounceCts;
        private double _pendingPositionPercent;

        private async void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (!_isUserScrolling)
            {
                _isUserScrolling = true;
            }

            ClearLyrics();
            _lastTrackBarScrollTime = DateTime.Now;
            _isTrackBarBeingDragged = true;

            // Save playback state
            if (!_wasPlayingBeforeScroll && _isPlaying)
            {
                _wasPlayingBeforeScroll = true;
            }

            double positionPercent = (double)trackBar1.Value / trackBar1.Maximum * 100.0;

            // Cancel any existing debounce task
            try { _scrollDebounceCts?.Cancel(); } catch { }
            _scrollDebounceCts = new CancellationTokenSource();
            _pendingPositionPercent = positionPercent;

            var token = _scrollDebounceCts.Token;

            // Start debounce task
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100, token); // Debounce delay

                    // Invoke on UI thread - perform synchronous UI updates there and start SetPosition task safely
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // Start SetPosition but do not block UI; observe errors
                            var setTask = SetPosition(_pendingPositionPercent);
                            setTask.ContinueWith(t =>
                            {
                                if (t.IsFaulted)
                                {
                                    Logger.Log($"Error in SetPosition (debounced): {t.Exception?.GetBaseException().Message}", Logger.LogTypes.Error);
                                }
                            }, TaskScheduler.Default);

                            // Update the UI labels (guard frames)
                            if (_frames != null && _frames.Count > 0)
                            {
                                // Map percent -> frame index using 0-based denominator so last frame corresponds to 100%
                                int denomForIndex = Math.Max(1, _frames.Count - 1);
                                int frameIndex = (int)Math.Round((_pendingPositionPercent / 100.0) * denomForIndex);
                                frameIndex = Math.Max(0, Math.Min(frameIndex, _frames.Count - 1));

                                long frameTick = _frames[frameIndex].Time;
                                double currentTimeMs = TicksToMilliseconds(frameTick);
                                string timeStr = TimeSpan.FromMilliseconds(currentTimeMs).ToString(@"mm\:ss\.ff", CultureInfo.CurrentCulture);
                                string percentagestr = Resources.TextPercent.Replace("{number}", _pendingPositionPercent.ToString("0.00", CultureInfo.CurrentCulture));

                                label_percentage.Text = percentagestr;
                                label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log($"Error in debounced trackBar1_Scroll UI update: {ex.Message}", Logger.LogTypes.Error);
                        }

                        // Start the playback restart timer in a thread-safe manner
                        lock (_playbackRestartTimerLock)
                        {
                            try
                            {
                                _playbackRestartTimer?.Stop();
                                _playbackRestartTimer?.Dispose();
                            }
                            catch { }
                            _playbackRestartTimer = new System.Timers.Timer(300);
                            _playbackRestartTimer.Elapsed += OnPlaybackRestartTimer;
                            _playbackRestartTimer.AutoReset = false;
                            _playbackRestartTimer.Start();
                        }

                        _isUserScrolling = false;
                    }));
                }
                catch (OperationCanceledException)
                {
                    // Debounce was cancelled, do nothing
                }
            }, token);
        }

        /// <summary>
        /// Handles the timer event that triggers playback restart after user interaction with the playback controls.
        /// </summary>
        /// <remarks>This method is intended to be used as an event handler for a System.Timers.Timer. It
        /// resets relevant playback state and resumes playback if it was previously interrupted by user actions such as
        /// scrolling or dragging the track bar.</remarks>
        /// <param name="sender">The source of the event, typically the timer that initiated the callback.</param>
        /// <param name="e">An ElapsedEventArgs object that contains the event data.</param>
        private async void OnPlaybackRestartTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_playbackRestartTimerLock)
            {
                try
                {
                    _playbackRestartTimer?.Stop();
                    _playbackRestartTimer?.Dispose();
                }
                catch { }
                _playbackRestartTimer = null;
            }

            this.BeginInvoke(new Action(() =>
            {
                _isTrackBarBeingDragged = false;
                _isUserScrolling = false;

                // Restart if was playing before scrolling
                if (_wasPlayingBeforeScroll && !_isPlaying)
                {
                    _wasPlayingBeforeScroll = false; // Reset the flag
                    Play();
                }
                else
                {
                    _wasPlayingBeforeScroll = false; // Reset in any condition
                }
            }));
        }

        private Label[] _noteLabels;
        private Dictionary<int, int> _noteToLabelMap;
        private Dictionary<Label, Color> _originalLabelColors = new Dictionary<Label, Color>();

        // Initializes the note labels and their properties

        /// <summary>
        /// Initializes the note label controls and sets their default properties for use in the user interface.
        /// </summary>
        /// <remarks>This method prepares the note labels by assigning them to an internal array,
        /// configuring their appearance, and mapping MIDI note numbers to label indices. It should be called before any
        /// operations that depend on the note labels being initialized.</remarks>
        private void InitializeNoteLabels()
        {
            // Collect all labels
            _noteLabels = new Label[32];
            for (int i = 1; i <= 32; i++)
            {
                var found = this.Controls.Find($"label_note{i}", true).FirstOrDefault() as Label;
                if (found == null)
                {
                    Logger.Log($"Label 'label_note{i}' not found during initialization.", Logger.LogTypes.Warning);
                    found = new Label() { Visible = false, BackColor = SetInactiveNoteColor.GetInactiveNoteColor(Settings1.Default.note_indicator_color) };
                    this.Controls.Add(found);
                }
                _noteLabels[i - 1] = found;
                _noteLabels[i - 1].BackColor = SetInactiveNoteColor.GetInactiveNoteColor(Settings1.Default.note_indicator_color);
                _noteLabels[i - 1].ForeColor = SetTextColor.GetTextColor(Settings1.Default.note_indicator_color);
                _noteLabels[i - 1].Visible = false; // Initially hide all labels

                // Store the original color safely
                if (!_originalLabelColors.ContainsKey(_noteLabels[i - 1]))
                    _originalLabelColors[_noteLabels[i - 1]] = _noteLabels[i - 1].BackColor;
            }
            foreach (var label in _noteLabels)
            {
                if (label == null) continue;
                typeof(Label).InvokeMember("DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, label, new object[] { true });
            }
            // Create a mapping from MIDI note numbers to label indices
            // Map notes centered so middle of grid corresponds to a reasonable range
            _noteToLabelMap = new Dictionary<int, int>();
            for (int i = 0; i <= 128; i++)
            {
                int mapped = i - 36; // shift so middle C near center
                mapped = Math.Clamp(mapped, 0, _noteLabels.Length - 1);
                _noteToLabelMap[i] = mapped;
            }
        }
        private void MIDI_file_player_Load(object sender, EventArgs e)
        {
            InitializeNoteLabels();
        }

        private void checkBox_dont_update_grid_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_dont_update_grid.Checked == true)
            {
                UpdateNoteLabels(new HashSet<int>());
                Logger.Log("Don't update grid checkbox is checked. Hiding all labels.", Logger.LogTypes.Info);
            }
            else
            {
                Logger.Log("Don't update grid checkbox is not checked. Showing all labels.", Logger.LogTypes.Info);
            }
        }

        private void checkBox_loop_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_loop.Checked == true)
            {
                Logger.Log("Loop is enabled.", Logger.LogTypes.Info);
            }
            else
            {
                Logger.Log("Loop is disabled.", Logger.LogTypes.Info);
            }
        }

        private void numericUpDown_alternating_note_ValueChanged(object sender, EventArgs e)
        {
            Logger.Log($"Alternating note duration changed to {numericUpDown_alternating_note.Value} ms.", Logger.LogTypes.Info);
        }

        private void checkBox_play_each_note_CheckedChanged(object sender, EventArgs e)
        {
            Logger.Log("Play each note checkbox changed.", Logger.LogTypes.Info);
        }

        /// <summary>
        /// Converts the specified number of MIDI ticks to the corresponding duration in milliseconds, taking into
        /// account tempo changes.
        /// </summary>
        /// <remarks>If no tempo events have occurred before the specified tick position, the conversion
        /// uses the default tempo of 120 BPM. The calculation accounts for all tempo changes up to and including the
        /// specified tick.</remarks>
        /// <param name="ticks">The number of MIDI ticks to convert. Must be greater than or equal to zero.</param>
        /// <returns>The duration, in milliseconds, that corresponds to the specified number of ticks. The value reflects tempo
        /// changes that may have occurred up to the given tick position.</returns>
        private double TicksToMilliseconds(long ticks)
        {
            if (_precomputedTempoTimes == null || _precomputedTempoTimes.Count == 0)
            {
                // No precomputed tempos - use default tempo
                return (double)ticks * 500000 / _ticksPerQuarterNote / 1000.0;
            }

            // Find tempo events in right order
            int index = _precomputedTempoTimes.BinarySearch((ticks, 0), Comparer<(long, double)>.Create((x, y) => x.Item1.CompareTo(y.Item1)));
            if (index < 0)
            {
                index = ~index - 1;
            }

            // Use default tempo (120 BPM) if before first
            if (index < 0)
            {
                return (double)ticks * 500000 / _ticksPerQuarterNote / 1000.0;
            }

            var lastTempoEvent = _precomputedTempoTimes[index];
            double cumulativeMs = lastTempoEvent.cumulativeMs;
            long lastTicks = lastTempoEvent.time;
            int lastTempo = _tempoEvents.FirstOrDefault(e => e.time == lastTicks).tempo;
            if (lastTempo == 0) lastTempo = 500000;

            // More precise calculation
            cumulativeMs += (double)(ticks - lastTicks) * lastTempo / _ticksPerQuarterNote / 1000.0;
            return cumulativeMs;
        }

        /// <summary>
        /// Precomputes the cumulative elapsed time in milliseconds for each tempo event in the sequence.
        /// </summary>
        /// <remarks>This method prepares a lookup table mapping MIDI tick positions to their
        /// corresponding elapsed time in milliseconds, based on the current tempo map. This enables efficient
        /// conversion from MIDI ticks to real time for subsequent operations. If no tempo events are present, the
        /// method does not perform any computation.</remarks>
        private void PrecomputeTempoTimes()
        {
            _precomputedTempoTimes = new List<(long time, double cumulativeMs)>();
            if (_tempoEvents == null || !_tempoEvents.Any())
            {
                return;
            }

            double cumulativeMs = 0;
            long lastTicks = 0;

            // Calculate the cumulative milliseconds for each tempo event
            var firstTempoEvent = _tempoEvents[0];
            if (firstTempoEvent.time > 0)
            {
                int defaultTempo = 500000; // Default tempo (120 BPM) in microseconds per quarter note
                cumulativeMs += (double)(firstTempoEvent.time - lastTicks) * defaultTempo / _ticksPerQuarterNote / 1000.0;
            }

            _precomputedTempoTimes.Add((firstTempoEvent.time, cumulativeMs));
            lastTicks = firstTempoEvent.time;

            // Calculate cumulative milliseconds for each segment between tempo events
            for (int i = 0; i < _tempoEvents.Count - 1; i++)
            {
                var currentTempoEvent = _tempoEvents[i];
                var nextTempoEvent = _tempoEvents[i + 1];
                int tempoForSegment = currentTempoEvent.tempo;
                if (tempoForSegment == 0) tempoForSegment = 500000;

                cumulativeMs += (double)(nextTempoEvent.time - lastTicks) * tempoForSegment / _ticksPerQuarterNote / 1000.0;
                _precomputedTempoTimes.Add((nextTempoEvent.time, cumulativeMs));
                lastTicks = nextTempoEvent.time;
            }
        }
        string lyricRow = string.Empty;
        private async void playbackTimer_Tick(object sender, EventArgs e)
        {
            if (_isStopping || !_isPlaying || _frames == null)
                return;

            // SysEx uses the same stopwatch, but it is not part of the audio frames.
            double currentSongTimeMs =
                _playbackStartOffsetMs + _playbackStopwatch.Elapsed.TotalMilliseconds;

            // --- UI Update Block ---
            if (IsHandleCreated && Visible)
            {
                var currentFrameIndexForUI = _currentFrameIndex;
                if (currentFrameIndexForUI < _frames.Count)
                {
                    var currentFrameForUI = _frames[currentFrameIndexForUI];
                    HashSet<int> filteredNotes = new HashSet<int>();
                    foreach (var note in currentFrameForUI.ActiveNotes)
                    {
                        if (_noteChannels.TryGetValue(note, out int channel) && _enabledChannels.Contains(channel))
                            filteredNotes.Add(note);
                    }
                    UpdateAllUISync(currentFrameIndexForUI, filteredNotes);
                }
            }

            // Playback is complete after every audio/lyric frame is processed.
            if (_currentFrameIndex >= _frames.Count)
            {
                // Apply anything already due, but never keep the musical playback
                // state alive for a future SysEx event or a display timeout. Some
                // files contain late/malformed display timestamps, which previously
                // left _isPlaying true forever and made Stop/Rewind/Play appear dead.
                ProcessPendingSysExDisplayEvents(currentSongTimeMs);
                HandlePlaybackComplete();
                return;
            }

            // --- Sound Processing Block ---
            // While sound is already playing, use this timer tick for the display.
            if (!_playbackTask.IsCompleted)
            {
                ProcessPendingSysExDisplayEvents(currentSongTimeMs);
                return;
            }

            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                Logger.Log("CancellationTokenSource is null or canceled, stopping playback", Logger.LogTypes.Info);
                Stop();
                return;
            }

            // Start a new playback task
            _playbackTask = Task.Run(async () =>
            {
                try
                {
                    var token = _cancellationTokenSource.Token;
                    token.ThrowIfCancellationRequested();

                    var elapsedMs = _playbackStopwatch.ElapsedMilliseconds;
                    var songTimeMs = _playbackStartOffsetMs + elapsedMs;

                    while (_currentFrameIndex < _frames.Count)
                    {
                        if (!_isPlaying || token.IsCancellationRequested)
                        {
                            Logger.Log("Playback stopped or token cancelled during frame processing", Logger.LogTypes.Info);
                            break;
                        }

                        var currentFrame = _frames[_currentFrameIndex];
                        var targetTimeMs = TicksToMilliseconds(currentFrame.Time);

                        if (targetTimeMs <= songTimeMs)
                        {
                            await ProcessCurrentFrame(); // This method handles note playback and UI updates
                            _currentFrameIndex++;
                        }
                        else
                        {
                            break; // Next frame is in the future, exit loop
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.Log("Playback task was canceled.", Logger.LogTypes.Info);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error in playback task: {ex.Message}", Logger.LogTypes.Error);
                    if (IsHandleCreated)
                    {
                        this.BeginInvoke((Action)Stop);
                    }
                }
            }, _cancellationTokenSource.Token);

            // The next audio task has already been scheduled, so updating the
            // display here cannot delay the start of the next sound segment.
            ProcessPendingSysExDisplayEvents(currentSongTimeMs);
        }
        private DateTime _lastLyricTime = DateTime.MinValue;
        private bool _isInLyricSection = false;
        double driftMs = 0;
        private HashSet<int> _previousMidiOutputNotes = new();

        /// <summary>
        /// Processes the current MIDI frame, handling MIDI output events and system speaker playback as appropriate.
        /// Advances playback by sending note events and managing timing for the current frame.
        /// </summary>
        /// <remarks>This method should be called as part of a MIDI playback loop to process each frame in
        /// sequence. It respects cancellation requests via the associated cancellation token. MIDI output and system
        /// speaker playback are performed based on user settings and enabled channels.</remarks>
        /// <returns>A task that represents the asynchronous operation of processing the current frame.</returns>
        private async Task ProcessCurrentFrame()
        {
            var token = _cancellationTokenSource?.Token ?? CancellationToken.None;
            token.ThrowIfCancellationRequested();

            Stopwatch driftStopwatch = Stopwatch.StartNew();
            var currentFrame = _frames[_currentFrameIndex];
            var currentTime = currentFrame.Time;

            _eventsByTime.TryGetValue(currentTime, out var eventsAtThisTime);

            // --- Event-based MIDI output logic ---
            if (TemporarySettings.MIDIDevices.useMIDIoutput && eventsAtThisTime != null)
            {
                // 1. Process Note Off events
                var noteOffEvents = eventsAtThisTime.OfType<NoteEvent>().Where(n => !MidiEvent.IsNoteOn(n));
                foreach (var noteOff in noteOffEvents)
                {
                    if (_enabledChannels.Contains(noteOff.Channel))
                    {
                        MIDIIOUtils.SendNoteOff(noteOff.NoteNumber, noteOff.Channel - 1);
                    }
                }

                // 2. Process all Note On events
                var noteOnEvents = eventsAtThisTime.OfType<NoteOnEvent>().Where(n => n.Velocity > 0);
                foreach (var noteOn in noteOnEvents)
                {
                    if (_enabledChannels.Contains(noteOn.Channel))
                    {
                        _noteInstruments.TryGetValue((noteOn.Channel, noteOn.NoteNumber, currentTime), out int instrument);
                        MIDIIOUtils.SendNoteOn(noteOn.NoteNumber, instrument, noteOn.Channel - 1);
                    }
                }
            }

            // --- Playing with system speaker (aka PC speaker) logic ---
            HashSet<int> filteredNotes = new HashSet<int>();
            foreach (var note in currentFrame.ActiveNotes)
            {
                // Check all events occurring at this time to see if any correspond to this note
                bool isPercussion = false;

                if (eventsAtThisTime != null)
                {
                    // Check if this note number is being triggered on Channel 10 at this exact time
                    isPercussion = eventsAtThisTime.OfType<NoteOnEvent>()
                        .Any(n => n.NoteNumber == note && n.Channel == 10);
                }

                if (!isPercussion)
                {
                    // Only add to melodic notes if it's not a Channel 10 percussion hit
                    if (_noteChannels.TryGetValue(note, out int channel) && _enabledChannels.Contains(channel))
                    {
                        filteredNotes.Add(note);
                    }
                }
            }

            // Pick the drum hit from the actual events at this tick, not from ActiveNotes.
            NoteOnEvent drumEvent = null;
            if (eventsAtThisTime != null && _enabledChannels.Contains(10))
            {
                drumEvent = PickBestDrumEvent(
                    eventsAtThisTime.OfType<NoteOnEvent>()
                        .Where(n => n.Velocity > 0 && n.Channel == 10));
            }

            // Calculate duration
            double durationMs;
            if (_currentFrameIndex < _frames.Count - 1)
            {
                var nextFrame = _frames[_currentFrameIndex + 1];
                durationMs = TicksToMilliseconds(nextFrame.Time) - TicksToMilliseconds(currentFrame.Time);
            }
            else
            {
                long lastTick = _midiFile.Events.SelectMany(t => t).Max(e => e.AbsoluteTime);
                double totalDurationMs = TicksToMilliseconds(lastTick);
                durationMs = totalDurationMs - TicksToMilliseconds(currentTime);
            }

            // --- Robust drift compensation (preserve fractional ms) ---
            double originalDuration = Math.Max(0.0, durationMs);
            double adjustedDuration;

            if (driftMs > 0.0)
            {
                double consume = Math.Min(driftMs, originalDuration);
                adjustedDuration = Math.Max(0.0, originalDuration - driftMs);
                driftMs -= consume;
            }
            else if (driftMs < 0.0)
            {
                adjustedDuration = originalDuration + (-driftMs);
                driftMs = 0.0;
            }
            else
            {
                adjustedDuration = originalDuration;
            }

            int durationMsInt = (int)Math.Max(0, Math.Round(adjustedDuration));
            if (durationMsInt <= 0 && filteredNotes.Count == 0 && drumEvent == null)
            {
                driftMs += (driftStopwatch.Elapsed.TotalMilliseconds - adjustedDuration);
                return;
            }

            if (checkBox_show_lyrics_or_text_events.Checked)
            {
                HandleLyricsDisplay(currentTime);
            }

            // --- Melody + percussion, alternated on the single-voice speaker output ---
            int totalFrameDuration = durationMsInt;

            PercussionSounds.MidiPercussion? percussionToPlay = drumEvent != null
                ? (PercussionSounds.MidiPercussion)drumEvent.NoteNumber
                : (PercussionSounds.MidiPercussion?)null;

            int[] frequenciesToPlay = filteredNotes
                .Select(note => NoteToFrequency(note))
                .ToArray();

            await PlayNotesAndPercussionAlternatingAsync(
                frequenciesToPlay, percussionToPlay, totalFrameDuration, token);

            // Accumulate the real elapsed difference (can be positive or negative)
            driftMs += (driftStopwatch.Elapsed.TotalMilliseconds - adjustedDuration);
        }


        private static void CollectSysExFragment(
            SysexEvent sysexEvent,
            byte[] fragment,
            Dictionary<long, List<byte[]>> destination,
            ref List<byte> pending,
            ref long pendingStartTick,
            ref long pendingLastTick)
        {
            int status = ((int)sysexEvent.CommandCode) & 0xFF;
            bool isStart = status == 0xF0;
            bool isContinuation = status == 0xF7;
            bool fragmentStartsWithF0 =
                fragment != null &&
                fragment.Length > 0 &&
                fragment[0] == 0xF0;
            bool fragmentStartsWithF7 =
                fragment != null &&
                fragment.Length > 0 &&
                fragment[0] == 0xF7;
            bool fragmentStartsRolandDt1 =
                RolandGSStyleDisplayDecoder
                    .LooksLikeRolandDt1PacketStart(fragment);

            // NAudio's SysEx payload omits F0/F7 and some versions expose
            // both SMF F0 and F7 events with the same Sysex command code.
            // A fragment without a new Roland header therefore continues the
            // pending packet instead of incorrectly flushing it as a new one.
            bool continuesPendingPacket = pending != null &&
                (isContinuation ||
                 fragmentStartsWithF7 ||
                 (!fragmentStartsWithF0 &&
                  !fragmentStartsRolandDt1));

            if (continuesPendingPacket)
            {
                pendingLastTick = sysexEvent.AbsoluteTime;
                AppendSysExFragment(
                    pending,
                    fragment,
                    stripLeadingStatusByte: fragmentStartsWithF7);
            }
            else if (isStart ||
                     fragmentStartsWithF0 ||
                     fragmentStartsRolandDt1)
            {
                // A new F0 starts a new packet. Preserve a previous malformed or
                // checksum-tolerant packet rather than silently dropping it.
                FlushPendingSysExFragment(
                    destination,
                    ref pending,
                    ref pendingStartTick,
                    ref pendingLastTick);

                pending = new List<byte>();
                pendingStartTick = sysexEvent.AbsoluteTime;
                pendingLastTick = sysexEvent.AbsoluteTime;
                AppendSysExFragment(
                    pending,
                    fragment,
                    stripLeadingStatusByte: fragmentStartsWithF0);
            }
            else
            {
                // Some NAudio versions expose a complete F7 event without a prior
                // F0 object. Keep it as a standalone packet and let the decoder filter it.
                AddSysExMessage(
                    destination,
                    sysexEvent.AbsoluteTime,
                    fragment);
                return;
            }

            byte[] combined = pending.ToArray();
            if (RolandGSStyleDisplayDecoder.IsCompleteDisplayPacket(combined))
            {
                // The command takes effect when its last fragment arrives.
                AddSysExMessage(destination, pendingLastTick, combined);
                pending = null;
                pendingStartTick = 0;
                pendingLastTick = 0;
            }
        }

        private static void AppendSysExFragment(
            List<byte> destination,
            byte[] fragment,
            bool stripLeadingStatusByte)
        {
            if (fragment == null || fragment.Length == 0)
            {
                return;
            }

            int startIndex = 0;
            if (stripLeadingStatusByte &&
                fragment.Length > 1 &&
                (fragment[0] == 0xF0 || fragment[0] == 0xF7))
            {
                startIndex = 1;
            }

            for (int i = startIndex; i < fragment.Length; i++)
            {
                destination.Add(fragment[i]);
            }
        }

        private static void FlushPendingSysExFragment(
            Dictionary<long, List<byte[]>> destination,
            ref List<byte> pending,
            ref long pendingStartTick,
            ref long pendingLastTick)
        {
            if (pending == null)
            {
                return;
            }

            long tick = pendingLastTick != 0
                ? pendingLastTick
                : pendingStartTick;
            AddSysExMessage(destination, tick, pending.ToArray());
            pending = null;
            pendingStartTick = 0;
            pendingLastTick = 0;
        }

        private static void AddSysExMessage(
            Dictionary<long, List<byte[]>> destination,
            long tick,
            byte[] message)
        {
            if (!destination.TryGetValue(tick, out var messages))
            {
                messages = new List<byte[]>();
                destination[tick] = messages;
            }

            messages.Add(message == null
                ? Array.Empty<byte>()
                : (byte[])message.Clone());
        }

        private static bool TryExtractSysExData(
            SysexEvent sysexEvent,
            out byte[] data)
        {
            data = Array.Empty<byte>();
            if (sysexEvent == null)
            {
                return false;
            }

            try
            {
                // A present byte-array member is a successful extraction even when
                // its length is zero. That distinguishes a real empty SysEx packet
                // from a reflection/parsing failure.
                if (SysExDataProperty?.GetValue(sysexEvent) is byte[] propertyData)
                {
                    data = (byte[])propertyData.Clone();
                    return true;
                }

                // NAudio 2.x stores MIDI-file SysEx bytes in a private field named "data".
                if (SysExDataField?.GetValue(sysexEvent) is byte[] fieldData)
                {
                    data = (byte[])fieldData.Clone();
                    return true;
                }

                // Last-resort compatibility path for a future NAudio build that changes
                // the backing member but keeps the hexadecimal ToString representation.
                string eventText = sysexEvent.ToString();
                int bytesMarker = eventText.IndexOf("bytes", StringComparison.OrdinalIgnoreCase);
                if (bytesMarker >= 0)
                {
                    string hexadecimalText = eventText.Substring(bytesMarker + 5);
                    MatchCollection matches = Regex.Matches(
                        hexadecimalText,
                        @"(?<![0-9A-Fa-f])[0-9A-Fa-f]{2}(?![0-9A-Fa-f])");

                    data = new byte[matches.Count];
                    for (int i = 0; i < matches.Count; i++)
                    {
                        data[i] = Convert.ToByte(matches[i].Value, 16);
                    }

                    // "0 bytes" is a valid empty packet and therefore succeeds.
                    return matches.Count > 0 ||
                           Regex.IsMatch(eventText, @"\b0\s+bytes\b", RegexOptions.IgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Unable to read SysEx data: {ex.Message}", Logger.LogTypes.Warning);
            }

            return false;
        }

        /// <summary>
        /// Applies every supported display SysEx event whose MIDI time has
        /// been reached by the playback stopwatch. This does not add frames
        /// or otherwise participate in audio timing.
        /// </summary>
        private void ProcessPendingSysExDisplayEvents(double songTimeMs)
        {
            bool repaint = false;
            bool publishSysExText = false;
            bool[,] pixels = null;
            string sysExText = null;

            lock (_sysExDisplayLock)
            {
                while (_nextSysExDisplayEventIndex <
                       _sysExDisplayEventTimes.Count)
                {
                    long eventTick =
                        _sysExDisplayEventTimes[
                            _nextSysExDisplayEventIndex];
                    double eventTimeMs = TicksToMilliseconds(eventTick);
                    if (eventTimeMs > songTimeMs)
                    {
                        break;
                    }

                    repaint |= ExpireSysExDisplayIfDue(
                        eventTimeMs,
                        out bool textExpiredBeforeEvent);
                    if (textExpiredBeforeEvent)
                    {
                        publishSysExText = true;
                        sysExText = string.Empty;
                    }

                    if (_sysExEventsByTime.TryGetValue(
                            eventTick,
                            out var messages))
                    {
                        foreach (byte[] message in messages)
                        {
                            if (_sysExDisplayDecoder.Apply(
                                    message,
                                    out bool visibleChanged,
                                    out bool restartDisplayTimeout,
                                    out bool textChanged))
                            {
                                _hasAppliedSysExDisplayState = true;
                                repaint |= visibleChanged;

                                if (textChanged)
                                {
                                    publishSysExText = true;
                                    sysExText = _sysExDisplayDecoder.DisplayedText;
                                }

                                UpdateSysExDisplayTimeout(
                                    eventTimeMs,
                                    restartDisplayTimeout);
                            }
                        }
                    }

                    _nextSysExDisplayEventIndex++;
                }

                repaint |= ExpireSysExDisplayIfDue(
                    songTimeMs,
                    out bool textExpiredAfterEvents);
                if (textExpiredAfterEvents)
                {
                    publishSysExText = true;
                    sysExText = string.Empty;
                }

                if (repaint)
                {
                    // Always publish a complete frame, including an all-off dot
                    // page or page zero. This makes blank commands authoritative.
                    pixels = _sysExDisplayDecoder.GetPixels();
                }
            }

            if (repaint)
            {
                RenderSysExDisplay(pixels);
            }

            if (publishSysExText)
            {
                RenderSysExText(sysExText);
            }
        }

        private bool HasPendingSysExDisplayWork(double songTimeMs)
        {
            lock (_sysExDisplayLock)
            {
                return _nextSysExDisplayEventIndex < _sysExDisplayEventTimes.Count ||
                       (_sysExDisplayClearAtMs.HasValue &&
                        songTimeMs < _sysExDisplayClearAtMs.Value);
            }
        }

        private void UpdateSysExDisplayTimeout(
            double eventTimeMs,
            bool restartDisplayTimeout)
        {
            if (_sysExDisplayDecoder.CurrentPage == 0)
            {
                _sysExDisplayClearAtMs = null;
                return;
            }

            if (!restartDisplayTimeout)
            {
                return;
            }

            // Display Time 00 means zero seconds (immediate return to bar
            // display), not an unlimited display. The default value is 06.
            double timeoutMs = Math.Max(
                0.0,
                _sysExDisplayDecoder.DisplayTimeoutMilliseconds);
            _sysExDisplayClearAtMs = eventTimeMs + timeoutMs;
        }

        private bool ExpireSysExDisplayIfDue(
            double songTimeMs,
            out bool textChanged)
        {
            textChanged = false;

            if (!_sysExDisplayClearAtMs.HasValue ||
                songTimeMs < _sysExDisplayClearAtMs.Value)
            {
                return false;
            }

            _sysExDisplayClearAtMs = null;
            return _sysExDisplayDecoder.ExpireDisplay(out textChanged);
        }

        private void RebuildSysExDisplayAtTick(long targetTick)
        {
            bool[,] pixels = null;
            bool hasAppliedState;
            string sysExText;
            double targetTimeMs = TicksToMilliseconds(targetTick);

            lock (_sysExDisplayLock)
            {
                _sysExDisplayDecoder.Reset();
                _nextSysExDisplayEventIndex = 0;
                _sysExDisplayClearAtMs = null;
                _hasAppliedSysExDisplayState = false;

                while (_nextSysExDisplayEventIndex <
                       _sysExDisplayEventTimes.Count)
                {
                    long eventTick =
                        _sysExDisplayEventTimes[
                            _nextSysExDisplayEventIndex];
                    if (eventTick > targetTick)
                    {
                        break;
                    }

                    double eventTimeMs = TicksToMilliseconds(eventTick);
                    ExpireSysExDisplayIfDue(eventTimeMs, out _);

                    if (_sysExEventsByTime.TryGetValue(
                            eventTick,
                            out var messages))
                    {
                        foreach (byte[] message in messages)
                        {
                            if (_sysExDisplayDecoder.Apply(
                                    message,
                                    out _,
                                    out bool restartDisplayTimeout,
                                    out _))
                            {
                                _hasAppliedSysExDisplayState = true;
                                UpdateSysExDisplayTimeout(
                                    eventTimeMs,
                                    restartDisplayTimeout);
                            }
                        }
                    }

                    _nextSysExDisplayEventIndex++;
                }

                ExpireSysExDisplayIfDue(targetTimeMs, out _);
                hasAppliedState = _hasAppliedSysExDisplayState;
                sysExText = _sysExDisplayDecoder.DisplayedText;

                if (hasAppliedState)
                {
                    pixels = _sysExDisplayDecoder.GetPixels();
                }
            }

            // Seeking/rebuilding publishes the decoder's text buffer directly.
            // This is independent from lyrics and MIDI meta text processing.
            RenderSysExText(sysExText);

            if (hasAppliedState)
            {
                RenderSysExDisplay(pixels);
            }
            else
            {
                ClearSysExDisplay();
            }
        }

        private void ResetSysExDisplayState()
        {
            lock (_sysExDisplayLock)
            {
                _sysExDisplayDecoder.Reset();
                _nextSysExDisplayEventIndex = 0;
                _sysExDisplayClearAtMs = null;
                _hasAppliedSysExDisplayState = false;
            }

            ClearSysExDisplay();
            ClearSysExText();
        }

        /// <summary>
        /// Writes only decoded Roland display-letter SysEx data to the emulator's
        /// marquee-capable label. MIDI lyrics and meta text use separate code
        /// paths and never call this method.
        /// </summary>
        private void RenderSysExText(string text)
        {
            if (IsDisposed || Disposing)
            {
                return;
            }

            string normalizedText = text ?? string.Empty;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(RenderSysExText), normalizedText);
                return;
            }

            // Route directly through the emulator's own SetSysExText entry
            // point instead of searching Controls for a named TextBox. That
            // previous Controls.Find("textBoxSysExText", ...) lookup targeted
            // a control name that does not match labelSysExText, so updates
            // (including clears) were silently dropped.
            SysExDisplayEmulator emulator = sysExDisplayEmulator;
            if (emulator != null && !emulator.IsDisposed && !emulator.Disposing)
            {
                emulator.SetSysExText(normalizedText);
            }
        }

        private void ClearSysExText()
        {
            RenderSysExText(string.Empty);
        }

        private void RenderCurrentSysExDisplay()
        {
            bool[,] pixels;

            lock (_sysExDisplayLock)
            {
                pixels = _sysExDisplayDecoder.GetPixels();
            }

            RenderSysExDisplay(pixels);
        }

        private void RenderSysExDisplay(bool[,] pixels)
        {
            SysExDisplayEmulator emulator = sysExDisplayEmulator;
            if (emulator == null || emulator.IsDisposed || emulator.Disposing)
            {
                return;
            }

            // Preserve the original panel-grid renderer. This call replaces the
            // complete 16x16 frame, and its existing all-off path clears it.
            emulator.SetDisplayContent(pixels);
        }

        /// <summary>
        /// Explicitly clears the emulator display rather than repainting it with
        /// an all-off pixel frame. This is used whenever the decoded content has
        /// no lit pixels at all — for example right after a GS Reset, or when the
        /// currently selected page was never written to by the MIDI file.
        /// </summary>
        private void ClearSysExDisplay()
        {
            SysExDisplayEmulator emulator = sysExDisplayEmulator;
            if (emulator == null || emulator.IsDisposed || emulator.Disposing)
            {
                return;
            }

            emulator.ClearDisplayContent();
        }


        private static NoteOnEvent PickBestDrumEvent(IEnumerable<NoteOnEvent> drumEvents)
        {
            return drumEvents
                .OrderByDescending(e => GetPercussionPriority(e.NoteNumber))
                .ThenByDescending(e => e.Velocity)
                .FirstOrDefault();
        }

        private static int GetPercussionPriority(int noteNumber)
        {
            // PC speaker is monophonic, so simultaneous drum hits need a deterministic choice.
            switch (noteNumber)
            {
                case 35: // Acoustic Bass Drum
                case 36: // Bass Drum 1
                    return 100;
                case 38: // Acoustic Snare
                case 40: // Electric Snare
                case 37: // Side Stick
                case 39: // Hand Clap
                    return 90;
                case 41:
                case 43:
                case 45:
                case 47:
                case 48:
                case 50:
                    return 80;
                case 42: // Closed Hi-Hat
                case 44: // Pedal Hi-Hat
                case 46: // Open Hi-Hat
                    return 70;
                case 49:
                case 51:
                case 52:
                case 53:
                case 55:
                case 57:
                case 59:
                    return 60;
                default:
                    return 40;
            }
        }

        /// <summary>
        /// Retrieves the tempo, in microseconds per quarter note, that is in effect at the specified time.
        /// </summary>
        /// <param name="currentTime">The current time, in ticks, for which to determine the active tempo. Must be greater than or equal to zero.</param>
        /// <returns>The tempo in microseconds per quarter note that applies at the specified time.</returns>
        private int GetCurrentTempo(long currentTime)
        {
            // Last tempo event before or at currentTime
            var lastTempoEvent = _tempoEvents
                .Where(t => t.time <= currentTime)
                .LastOrDefault();

            return lastTempoEvent.tempo != 0 ? lastTempoEvent.tempo : 500000; // Default 120 BPM
        }

        /// <summary>
        /// Calculates adaptive threshold values for lyric gaps and melody sections based on the current tempo at the
        /// specified time.
        /// </summary>
        /// <param name="currentTime">The current time, in ticks, used to determine the tempo for threshold calculation.</param>
        /// <returns>A tuple containing the lyric gap threshold and the melody section threshold, both in milliseconds.</returns>
        private (int lyricGapThreshold, int melodySectionThreshold) CalculateDynamicThresholds(long currentTime)
        {
            int currentTempo = GetCurrentTempo(currentTime);

            // Calculate BPM from microseconds per quarter note
            double bpm = 60000000.0 / currentTempo;

            // Base threshold values at 120 BPM
            const double baseBpm = 120.0;
            const int baseLyricGap = 1250;      // 1.25 seconds
            const int baseMelodySection = 2000; // 2 seconds

            // Set thresholds based on tempo ratio
            double tempoRatio = baseBpm / bpm;

            int lyricGapThreshold = (int)(baseLyricGap * tempoRatio);
            int melodySectionThreshold = (int)(baseMelodySection * tempoRatio);

            // Add bounds to thresholds
            lyricGapThreshold = Math.Max(500, Math.Min(5000, lyricGapThreshold));    // Between 0.5-5 seconds
            melodySectionThreshold = Math.Max(1000, Math.Min(10000, melodySectionThreshold)); // Between 1-10 seconds

            return (lyricGapThreshold, melodySectionThreshold);
        }

        /// <summary>
        /// Handles the display and clearing of lyrics based on the current playback time.
        /// </summary>
        /// <param name="currentTime">The current playback time, in ticks or milliseconds, used to determine which lyrics to display or clear.</param>
        private void HandleLyricsDisplay(long currentTime)
        {
            bool hasLyrics = false;
            DateTime currentDateTime = DateTime.Now;

            // Calculate dynamic thresholds based on current tempo
            var (lyricGapThreshold, melodySectionThreshold) = CalculateDynamicThresholds(currentTime);

            // Check if that frame has lyric
            if (_metaEventsByTime != null && _metaEventsByTime.TryGetValue(currentTime, out var metas))
            {
                foreach (var metaEvent in metas)
                {
                    string lyrics = ExtractLyricsFromMetaEvent(metaEvent);
                    if (!string.IsNullOrEmpty(lyrics))
                    {
                        hasLyrics = true;
                        _lastLyricTime = currentDateTime;
                        _isInLyricSection = true;

                        // Process lyric
                        ProcessLyricText(lyrics);
                        break;
                    }
                }
            }

            // Specify type of delay if it hasn't any lyric
            if (!hasLyrics)
            {
                double timeSinceLastLyric = (currentDateTime - _lastLyricTime).TotalMilliseconds;

                if (_isInLyricSection)
                {
                    // Use dynamic threshold
                    if (timeSinceLastLyric > lyricGapThreshold)
                    {
                        // Longer delay 
                        _isInLyricSection = false;
                        ClearLyrics();
                    }
                }
                else
                {
                    if (timeSinceLastLyric > melodySectionThreshold)
                    {
                        // Do nothing, if there isn't any lyric
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the lyrics text from the specified MIDI meta event, if available.
        /// </summary>
        /// <param name="metaEvent">The meta event from which to extract lyrics.</param>
        /// <returns>A string containing the lyrics text if present in the meta event; otherwise, null.</returns>
        private string ExtractLyricsFromMetaEvent(MetaEvent metaEvent)
        {
            string lyrics = null;
            if (metaEvent is TextEvent textEvent)
            {
                lyrics = textEvent.Text;
            }
            else
            {
                var prop = metaEvent.GetType().GetProperty("Text");
                if (prop != null)
                {
                    lyrics = prop.GetValue(metaEvent)?.ToString();
                }
            }
            return lyrics;
        }

        // Process and display lyric text

        /// <summary>
        /// Processes the specified lyric text by removing control characters and formatting it for display.
        /// </summary>
        /// <param name="lyrics">The lyric text to process.</param>
        private void ProcessLyricText(string lyrics)
        {
            if (string.IsNullOrEmpty(lyrics)) return;

            // Clear previous line buffer if a line or screen break marker is present
            if (lyrics.Contains("\n") || lyrics.Contains("\\") || lyrics.Contains("/") ||
                lyrics.Contains("\r") || lyrics.Contains("\t") || lyrics.Contains("\0") ||
                lyrics.Contains("\f") || lyrics.Contains("\v") || lyrics.Contains("|"))
            {
                lyricRow = string.Empty;
            }

            // Sanitize individual lyric chunk before appending to preserve syllable boundaries
            string sanitizedChunk = SanitizeLyricQuotes(lyrics);

            // Strip line break and control symbols from the printable text
            sanitizedChunk = sanitizedChunk
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("/", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\0", string.Empty)
                .Replace("\f", string.Empty)
                .Replace("\v", string.Empty)
                .Replace("|", string.Empty);

            lyricRow += sanitizedChunk;
            PrintLyrics(lyricRow.Trim());
        }

        /// <summary>
        /// Asynchronously waits for the specified number of milliseconds or until the operation is canceled.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to wait before completing the task.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the wait operation.</param>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
        private async Task WaitPreciseWithCancellation(int milliseconds, CancellationToken cancellationToken)
        {
            if (milliseconds <= 0)
                return;

            // Silent frames do not need high-precision speaker timing. Use the
            // framework delay here because it guarantees that Stop/Rewind can
            // interrupt a long rest immediately through the cancellation token.
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await Task.Delay(milliseconds, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                Logger.Log($"Silent frame wait was canceled before {milliseconds}ms elapsed", Logger.LogTypes.Info);
                throw;
            }
        }

        /// <summary>
        /// Resets the track bar and associated labels to their initial state.
        /// </summary>
        private void ResetLabelsAndTrackBar()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ResetLabelsAndTrackBar()));
                return;
            }

            // Prevent the conflict during trackBar update
            trackBar1.Value = 0;

            // Update the percentage label
            string percentagestr = Resources.TextPercent.Replace("{number}", (0.ToString("0.00", CultureInfo.CurrentCulture)));
            label_percentage.Text = percentagestr;

            // Update the position label
            string timeStr = $"{0:D2}:{0:D2}.{0:D2}";

            if (label_position.InvokeRequired)
            {
                label_position.BeginInvoke(new Action(() =>
                {
                    label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
                }));
            }
            else
            {
                label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
            }
        }

        /// <summary>
        /// Synchronizes all relevant UI elements to reflect the specified frame index and filtered notes.
        /// </summary>
        /// <param name="frameIndex">The zero-based index of the frame to display.</param>
        /// <param name="filteredNotes">A set of note identifiers to be displayed or highlighted in the UI.</param>
        private void UpdateAllUISync(int frameIndex, HashSet<int> filteredNotes)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateAllUISync(frameIndex, filteredNotes)));
                return;
            }

            if (_frames == null || _frames.Count == 0)
                return;

            // Prevent the conflict during trackBar update
            if (!_isTrackBarBeingDragged)
            {
                int denom = Math.Max(1, _frames.Count - 1);
                int trackbarValue = (int)Math.Round((double)trackBar1.Maximum * frameIndex / denom);
                trackbarValue = Math.Clamp(trackbarValue, 0, trackBar1.Maximum);

                if (trackbarValue != trackBar1.Value)
                {
                    _isUserScrolling = false; // State that it's a program update
                    trackBar1.Value = trackbarValue;
                }
            }

            // Update the percentage label using consistent denominator
            int denomForPercent = Math.Max(1, _frames.Count - 1);
            double percent = ((double)frameIndex / denomForPercent) * 100.0;
            string percentagestr = Resources.TextPercent.Replace("{number}", percent.ToString("0.00", CultureInfo.CurrentCulture));
            label_percentage.Text = percentagestr;

            // Update the position label
            UpdatePositionLabel();

            if (!checkBox_dont_update_grid.Checked)
            {
                UpdateNoteLabelsSync(filteredNotes);
            }

            holded_note_label.Text = $"{Properties.Resources.TextHeldNotes} ({filteredNotes.Count})";
        }

        /// <summary>
        /// Updates the position label to display the current playback time in minutes, seconds, and hundredths of a second.
        /// </summary>
        private void UpdatePositionLabel()
        {
            if (!_isPlaying) return;

            // Prefer stopwatch-based song time as authoritative for playback
            double songTimeMs = _playbackStartOffsetMs + _playbackStopwatch.ElapsedMilliseconds;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(songTimeMs);
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            int milliseconds = timeSpan.Milliseconds / 10;

            string timeStr = $"{minutes:D2}:{seconds:D2}.{milliseconds:D2}";

            if (label_position.InvokeRequired)
            {
                label_position.BeginInvoke(new Action(() =>
                {
                    label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
                }));
            }
            else
            {
                label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
            }
        }

        // Playback complete handler

        /// <summary>
        /// Handles the completion of audio playback, performing necessary cleanup and optionally restarting playback if
        /// looping is enabled.
        /// </summary>
        private async void HandlePlaybackComplete()
        {
            // Timer ticks can observe EOF more than once before this async handler
            // reaches its first awaited cleanup operation. Only one completion
            // transaction may run at a time.
            if (_isCompletingPlayback)
            {
                return;
            }

            _isCompletingPlayback = true;
            try
            {
                if (!_isPlaying) return;

                // Atomically leave the playing state before any asynchronous cleanup.
                // This prevents additional timer ticks and makes Stop/Rewind safe even
                // when clicked at the exact end of the file. Play clicks are queued by
                // Play() while _isCompletingPlayback remains true.
                _isPlaying = false;
                playbackTimer.Stop();
                _playbackStopwatch?.Stop();
                SetPlaybackButtonState(isPlaying: false);

                // Ensure UI shows final position (100%)
                try
                {
                    if (this.InvokeRequired)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                trackBar1.Value = trackBar1.Maximum;
                                label_percentage.Text = Resources.TextPercent.Replace("{number}", (100.0).ToString("0.00", CultureInfo.CurrentCulture));
                                long lastTick = _midiFile.Events.SelectMany(t => t).Max(ev => ev.AbsoluteTime);
                                double totalMs = TicksToMilliseconds(lastTick);
                                string timeStr = TimeSpan.FromMilliseconds(totalMs).ToString(@"mm\:ss\.ff", CultureInfo.CurrentCulture);
                                label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
                            }
                            catch { }
                        }));
                    }
                    else
                    {
                        try
                        {
                            trackBar1.Value = trackBar1.Maximum;
                            label_percentage.Text = Resources.TextPercent.Replace("{number}", (100.0).ToString("0.00", CultureInfo.CurrentCulture));
                            long lastTick = _midiFile.Events.SelectMany(t => t).Max(ev => ev.AbsoluteTime);
                            double totalMs = TicksToMilliseconds(lastTick);
                            string timeStr = TimeSpan.FromMilliseconds(totalMs).ToString(@"mm\:ss\.ff", CultureInfo.CurrentCulture);
                            label_position.Text = $"{Properties.Resources.TextPosition} {timeStr}";
                        }
                        catch { }
                    }
                }
                catch { /* UI best-effort, ignore UI exceptions */ }

                playbackTimer.Stop();
                _wasPlayingBeforeScroll = false;
                if (checkBox_loop.Checked)
                {
                    Logger.Log("Playback loop enabled. Rewinding.", Logger.LogTypes.Info);
                    await Rewind(resumePreviousPlayback: false);
                    Play();
                }
                else
                {
                    Logger.Log("Playback finished.", Logger.LogTypes.Info);
                    // Natural EOF must never inherit a scroll-resume request.
                    // Otherwise Rewind() can restart playback even when Loop is off.
                    _wasPlayingBeforeScroll = false;
                    await StopAsync();
                    await Rewind(resumePreviousPlayback: false);
                    ResetSysExDisplayState();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"An error occurred in HandlePlaybackComplete: {ex.Message}", Logger.LogTypes.Error);
                await StopAsync();
            }
            finally
            {
                _isCompletingPlayback = false;
                Logger.Log("Timer-based playback completed", Logger.LogTypes.Info);

                // Honor a click made while StopAsync/Rewind was still running.
                // Clear the flag before calling Play so another genuine click can
                // be queued independently if startup encounters cleanup again.
                if (_playRequestedAfterCompletion &&
                    !_isPlaying &&
                    !_isStopping &&
                    !IsDisposed &&
                    !Disposing)
                {
                    _playRequestedAfterCompletion = false;
                    Play();
                }
                else if (!_isStopping)
                {
                    _playRequestedAfterCompletion = false;
                }
            }
        }

        private void MIDI_file_player_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }
        private Dictionary<int, int> _channelInstruments = new();
        // Added Channel to the dictionary key tuple to prevent collisions
        private Dictionary<(int Channel, int NoteNumber, long Time), int> _noteInstruments = new();

        /// <summary>
        /// Assigns instrument identifiers to each note event in the specified MIDI file based on the most recent
        /// program change for each channel.
        /// </summary>
        /// <param name="midiFile">The MIDI file whose note events will be analyzed and assigned instrument identifiers.</param>
        private void AssignInstrumentsToNotes(MidiFile midiFile)
        {
            _noteInstruments.Clear();
            if (midiFile == null) return;

            var channelPrograms = new Dictionary<int, int>();

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent is PatchChangeEvent patchChange)
                    {
                        if (patchChange.Channel != 9)
                        {
                            channelPrograms[patchChange.Channel] = patchChange.Patch;
                        }
                    }
                    else if (midiEvent is NoteOnEvent noteOn && noteOn.Velocity > 0)
                    {
                        // Channel 10 (0-indexed as 9) is percussion -> assign -1
                        int program = (noteOn.Channel == 9)
                            ? -1
                            : (channelPrograms.TryGetValue(noteOn.Channel, out int p) ? p : 0);

                        // Include noteOn.Channel in the dictionary key
                        _noteInstruments[(noteOn.Channel, noteOn.NoteNumber, noteOn.AbsoluteTime)] = program;
                    }
                }
            }
        }
        private void checkBoxShowSysExDisplayEmulator_CheckedChanged(object sender, EventArgs e)
        {
            bool logging = !isDeciding;
            if (!isDeciding)
            {
                sysExEmulatorEnabled = checkBoxShowSysExDisplayEmulator.Checked;
            }

            if (checkBoxShowSysExDisplayEmulator.Checked)
            {
                if (sysExDisplayEmulator == null || sysExDisplayEmulator.IsDisposed)
                {
                    sysExDisplayEmulator = new SysExDisplayEmulator(this);
                }

                if (!sysExDisplayEmulator.Visible)
                {
                    sysExDisplayEmulator.Show(this);
                }

                long displayTick = _frames != null && _frames.Count > 0
                    ? _frames[Math.Max(0, Math.Min(_currentFrameIndex, _frames.Count - 1))].Time
                    : (_sysExDisplayEventTimes.Count > 0 ? _sysExDisplayEventTimes[0] : 0);

                RebuildSysExDisplayAtTick(displayTick);

                if (logging)
                {
                    Logger.Log("SysEx display emulator enabled.", Logger.LogTypes.Info);
                }
            }
            else
            {
                if (sysExDisplayEmulator != null && !sysExDisplayEmulator.IsDisposed)
                {
                    sysExDisplayEmulator.Hide();
                }

                if (logging)
                {
                    Logger.Log("SysEx display emulator disabled.", Logger.LogTypes.Info);
                }
            }
        }

        /// <summary>
        /// Displays the specified lyrics using the lyrics overlay.
        /// </summary>
        /// <param name="lyrics">The lyrics text to display.</param>
        private void PrintLyrics(string lyrics)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => PrintLyrics(lyrics)));
                return;
            }

            if (lyricsOverlay == null || lyricsOverlay.IsDisposed)
            {
                if (checkBox_show_lyrics_or_text_events != null && checkBox_show_lyrics_or_text_events.Checked)
                {
                    lyricsOverlay = new LyricsOverlay();
                    lyricsOverlay.Owner = this;
                }
                else
                {
                    return;
                }
            }

            if (lyricsOverlay != null && !lyricsOverlay.IsDisposed && !lyricsOverlay.Disposing)
            {
                try { lyricsOverlay.PrintLyrics(lyrics); } catch (Exception ex) { Logger.Log($"PrintLyrics error: {ex.Message}", Logger.LogTypes.Error); }
            }
        }

        /// <summary>
        /// Clears the current lyrics from both the internal state and any associated lyrics overlay.
        /// </summary>
        private void ClearLyrics()
        {
            lyricRow = string.Empty;
            if (lyricsOverlay != null && !lyricsOverlay.IsDisposed && !lyricsOverlay.Disposing)
            {
                try { lyricsOverlay.ClearLyrics(); } catch { }
            }
        }

        /// <summary>
        /// Displays the lyrics overlay window if it is not already visible.
        /// </summary>
        private void ShowLyricsOverlay()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ShowLyricsOverlay()));
                return;
            }

            if (lyricsOverlay == null || lyricsOverlay.IsDisposed)
            {
                lyricsOverlay = new LyricsOverlay();
                lyricsOverlay.Owner = this;
            }

            if (!lyricsOverlay.Visible)
            {
                lyricsOverlay.Show(this);
            }

            try { BeginInvoke((Action)(() => this.Activate())); } catch { }
        }

        /// <summary>
        /// Hides the lyrics overlay if it is currently visible and not disposed.
        /// </summary>
        private void HideLyricsOverlay()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HideLyricsOverlay()));
                return;
            }

            if (lyricsOverlay != null && !lyricsOverlay.IsDisposed)
            {
                lyricsOverlay.Hide();
            }
        }

        private void checkBox_show_lyrics_or_text_events_CheckedChanged(object sender, EventArgs e)
        {
            bool logging = !isDeciding;

            if (!isDeciding)
            {
                lyricsEnabled = checkBox_show_lyrics_or_text_events.Checked;
            }
            if (checkBox_show_lyrics_or_text_events.Checked)
            {
                if (logging)
                {
                    Logger.Log("Show lyrics is enabled.", Logger.LogTypes.Info);
                }
                ShowLyricsOverlay();
            }
            else
            {
                if (logging)
                {
                    Logger.Log("Show lyrics is disabled.", Logger.LogTypes.Info);
                }
                HideLyricsOverlay();
            }
        }

    }
}