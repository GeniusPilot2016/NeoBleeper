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
using static UIHelper;

namespace NeoBleeper
{
    public partial class MIDI_file_player : Form
    {
        bool darkTheme = false;
        private bool _isAlternatingPlayback = false;
        bool is_playing = false;
        private List<int> _displayOrder = new List<int>();
        private List<(long time, double cumulativeMs)> _precomputedTempoTimes;
        private long _playbackStartTime;
        private long _nextFrameTime;
        private bool _isStopping = false;
        private bool _isUpdatingLabels = false;
        private MidiFile _midiFile;
        private Stopwatch _playbackStopwatch;
        private main_window mainWindow;
        private LyricsOverlay lyricsOverlay;
        public MIDI_file_player(string filename, main_window mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
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
            UIFonts.setFonts(this);
            set_theme();
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
                    set_theme();
                }
            }
        }
        private void PowerManager_Shutdown(object? sender, EventArgs e)
        {
            // Handle actual shutdown
            stopImmediately(); // Stop playing MIDI file and notes immediately
            Application.Exit();
        }

        private void PowerManager_Logoff(object? sender, EventArgs e)
        {
            // Handle actual logoff
            stopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_SystemHibernating(object? sender, EventArgs e)
        {
            // Handle system sleep/hibernate
            stopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_PreparingToLogoff(object? sender, EventArgs e)
        {
            // Handle logoff preparation
            stopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_SystemSleeping(object? sender, EventArgs e)
        {
            // Handle system sleep/hibernate
            stopImmediately(); // Stop playing MIDI file and notes immediately
        }

        private void PowerManager_PreparingToShutdown(object? sender, EventArgs e)
        {
            // Handle shutdown preparation
            stopImmediately(); // Stop playing MIDI file and notes immediately
            Application.Exit();
        }

        private void stopNotesImmediately()
        {
            NotePlayer.StopAllNotes();
            NotePlayer.StopMicrocontrollerSound();
            MIDIIOUtils.StopAllNotes();
        }
        private void stopImmediately()
        {
            Stop();
            stopNotesImmediately();
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
                this.ResumeLayout();
            }
        }
        private void dark_theme()
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

        private void light_theme()
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

        private async void button4_Click(object sender, EventArgs e)
        {
            Stop();
            openFileDialog.FileName = main_window.lastOpenedMIDIFileName;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (MIDIFileValidator.IsMidiFile(openFileDialog.FileName))
                {
                    main_window.lastOpenedMIDIFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                    textBox1.Text = openFileDialog.FileName;
                    await LoadMIDI(openFileDialog.FileName);
                }
                else
                {
                    MessageForm.Show(Resources.MessageNonValidMIDIFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string first_line = File.ReadLines(fileName).First();
                    if (MIDIFileValidator.IsMidiFile(fileName))
                    {
                        textBox1.Text = fileName;
                        await LoadMIDI(fileName);
                    }
                    else
                    {
                        Logger.Log("The file you dragged is not supported by NeoBleeper MIDI player or is corrupted.", Logger.LogTypes.Error);
                        MessageForm.Show(Resources.MessageMIDIFilePlayerNonSupportedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    Logger.Log("The file you dragged is corrupted or the file is in use by another process.", Logger.LogTypes.Error);
                    MessageForm.Show(Resources.MessageCorruptedOrCurrentlyUsedDraggedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

            Logger.Log($"Enabled channels: {string.Join(", ", _enabledChannels)}", Logger.LogTypes.Info);
        }

        private Dictionary<int, int> _noteChannels = new Dictionary<int, int>();
        private List<(long time, int tempo)> _tempoEvents;
        private int _ticksPerQuarterNote;
        private Dictionary<long, List<MetaEvent>> _metaEventsByTime = new Dictionary<long, List<MetaEvent>>();
        private Dictionary<long, List<MidiEvent>> _eventsByTime = new Dictionary<long, List<MidiEvent>>();
        private async Task LoadMIDI(string filename)
        {
            try
            {
                panelLoading.Visible = true;
                labelStatus.Text = Resources.TextTheMIDIFileIsBeingLoaded;
                progressBar1.Value = 0;
                progressBar1.Maximum = 100;
                progressBar1.Visible = true;

                _currentFileName = filename;
                Stop();
                _noteChannels.Clear();
                _metaEventsByTime.Clear();
                _eventsByTime.Clear();

                // Offload heavy processing to a background thread
                await Task.Run(() =>
                {
                    var midiFile = new MidiFile(filename, false);
                    _midiFile = midiFile;
                    _ticksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
                    _tempoEvents = new List<(long time, int tempo)>();

                    // Extract tempo information
                    int trackCount = midiFile.Events.Tracks;

                    foreach (var track in midiFile.Events)
                    {
                        foreach (var midiEvent in track)
                        {
                            if (midiEvent is TempoEvent tempoEvent)
                            {
                                _tempoEvents.Add((tempoEvent.AbsoluteTime, tempoEvent.MicrosecondsPerQuarterNote));
                            }

                            // Add events to dictionary by time
                            if (!_eventsByTime.TryGetValue(midiEvent.AbsoluteTime, out var eventList))
                            {
                                eventList = new List<MidiEvent>();
                                _eventsByTime[midiEvent.AbsoluteTime] = eventList;
                            }
                            eventList.Add(midiEvent);
                        }
                    }
                    // Sort tempo events by time and add a default if none are found
                    if (!_tempoEvents.Any())
                    {
                        _tempoEvents.Add((0, 500000)); // Default to 120 BPM
                    }
                    else
                    {
                        _tempoEvents = _tempoEvents.OrderBy(t => t.time).ToList();
                    }


                    // Collect MIDI events
                    UpdateProgressBar(30, Resources.TextMIDIEventsAreBeingCollected);
                    var allEvents = new List<(long Time, int NoteNumber, bool IsNoteOn, int Channel)>();
                    int totalTracks = midiFile.Events.Tracks;
                    int processedTracks = 0;

                    // Local dictionary to collect lyric/text meta events by absolute time
                    var metaDict = new Dictionary<long, List<MetaEvent>>();

                    foreach (var track in midiFile.Events)
                    {
                        foreach (var midiEvent in track)
                        {
                            if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                            {
                                var noteEvent = (NoteOnEvent)midiEvent;
                                allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, noteEvent.Velocity > 0, noteEvent.Channel));
                                _noteChannels[noteEvent.NoteNumber] = noteEvent.Channel;
                            }
                            else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                            {
                                var noteEvent = (NoteEvent)midiEvent;
                                allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, false, noteEvent.Channel));
                                if (!_noteChannels.ContainsKey(noteEvent.NoteNumber))
                                {
                                    _noteChannels[noteEvent.NoteNumber] = noteEvent.Channel; // or 0
                                }
                            }
                            else if (midiEvent.CommandCode == MidiCommandCode.MetaEvent)
                            {
                                // Collect lyric/text meta events so it can create frames at their times
                                var meta = (MetaEvent)midiEvent;
                                if (meta.MetaEventType == MetaEventType.Lyric || meta.MetaEventType == MetaEventType.TextEvent)
                                {
                                    if (!metaDict.TryGetValue(meta.AbsoluteTime, out var list))
                                    {
                                        list = new List<MetaEvent>();
                                        metaDict[meta.AbsoluteTime] = list;
                                    }
                                    list.Add(meta);
                                }
                            }
                        }

                        processedTracks++;
                        int percent = 30 + (int)(20.0 * processedTracks / totalTracks); // Between 30% and 50%
                        UpdateProgressBar(percent, $"{Resources.TextEventsAreBeingCollected} ({processedTracks}/{totalTracks})");
                    }

                    // Sort events by time
                    UpdateProgressBar(55, Resources.TextEventsAreBeingSorted);
                    allEvents = allEvents.OrderBy(e => e.Time).ToList();

                    // --- Detect rearticulated notes ---
                    _rearticulatedNotes.Clear();
                    var noteEventsByTime = allEvents.GroupBy(e => e.Time);
                    foreach (var timeGroup in noteEventsByTime)
                    {
                        var noteOffs = timeGroup.Where(e => !e.IsNoteOn).Select(e => e.NoteNumber).ToHashSet();
                        var noteOns = timeGroup.Where(e => e.IsNoteOn).Select(e => e.NoteNumber).ToHashSet();
                        var rearticulated = noteOffs.Intersect(noteOns);
                        foreach (var note in rearticulated)
                        {
                            _rearticulatedNotes.Add((note, timeGroup.Key));
                        }
                    }

                    // Take distinct time points; include meta event times so lyrics have frames
                    var timePoints = allEvents.Select(e => e.Time)
                                              .Concat(metaDict.Keys)
                                              .Distinct()
                                              .OrderBy(t => t)
                                              .ToList();

                    // Create frames
                    UpdateProgressBar(60, Resources.TextFramesAreBeingCreated);
                    _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
                    HashSet<int> currentlyActiveNotes = new HashSet<int>();
                    int totalTimePoints = timePoints.Count;

                    for (int i = 0; i < totalTimePoints; i++)
                    {
                        var time = timePoints[i];
                        foreach (var evt in allEvents.Where(e => e.Time == time))
                        {
                            if (evt.IsNoteOn)
                                currentlyActiveNotes.Add(evt.NoteNumber);
                            else
                                currentlyActiveNotes.Remove(evt.NoteNumber);
                        }
                        _frames.Add((time, new HashSet<int>(currentlyActiveNotes)));

                        // Update progress bar every 5% of frames processed
                        if (i % Math.Max(1, totalTimePoints / 20) == 0)
                        {
                            int percent = 60 + (int)(35.0 * i / totalTimePoints); // Between 60% and 95%
                            UpdateProgressBar(percent, $"{Resources.TextFramesAreBeingCreated} ({i + 1}/{totalTimePoints})");
                        }
                    }

                    // Assign collected meta events to the field for fast lookup later
                    _metaEventsByTime = metaDict;
                });

                _currentFrameIndex = 0;
                _isPlaying = false;

                UpdateEnabledChannels();

                progressBar1.Value = 100;
                labelStatus.Text = Resources.TextMIDIFileLoaded;
                await Task.Delay(300);
                progressBar1.Visible = false;
                PrecomputeTempoTimes();
                AssignInstrumentsToNotes(_midiFile);
                groupBox1.Enabled = true; // Enable controls after successful load
            }
            catch (Exception ex)
            {
                labelStatus.Text = Resources.TextMIDIFileLoadingError;
                progressBar1.Visible = false;
                progressBar1.Value = 0;
                groupBox1.Enabled = false; // Disable controls on error
                MessageForm.Show($"{Resources.MessageMIDIFileLoadingError} {ex.Message}");
                _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
                Logger.Log($"Error loading MIDI file: {ex.Message}", Logger.LogTypes.Error);
            }
            finally
            {
                _currentFrameIndex = 0; // Reset frame index
                ResetLabelsAndTrackBar(); // Reset UI elements
                panelLoading.Visible = false;
            }
        }

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
        public async void Play()
        {
            Logger.Log($"Play called. IsPlaying: {_isPlaying}, Frames count: {_frames?.Count ?? 0}", Logger.LogTypes.Info);

            if (_isPlaying || _frames == null || _frames.Count == 0)
                return;

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
                // Reinitialize the cancellation token source
                _cancellationTokenSource?.Dispose();
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
                button_play.Enabled = false;
                button_stop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageForm.Show($"{Resources.MessagePlaybackStartingError} {ex.Message}");
                _isPlaying = false;
            }
            finally
            {
                driftMs = 0; // Reset drifts
            }
        }
        public async void Stop()
        {
            Logger.Log($"Stop called. IsPlaying: {_isPlaying}", Logger.LogTypes.Info);

            if (!_isPlaying)
                return;

            _isStopping = true;
            try
            {
                playbackTimer.Stop();
                _playbackStopwatch?.Stop();
                _isPlaying = false;
                _isAlternatingPlayback = false;

                // Reset lyric state
                _lastLyricTime = DateTime.MinValue;
                _isInLyricSection = false;

                // Cancel the playback task
                _cancellationTokenSource?.Cancel();

                // Wait for the playback task to complete asynchronously
                if (_playbackTask != null && !_playbackTask.IsCompleted)
                {
                    await _playbackTask;
                }

                // Reset UI elements
                button_play.Enabled = true;
                button_stop.Enabled = false;
                UpdateNoteLabels(new HashSet<int>());
                holded_note_label.Text = $"{Properties.Resources.TextHeldNotes} (0)";
                label_more_notes.Visible = false;
                ClearLyrics();
                // Reset drifts 
                driftMs = 0;
                MIDIIOUtils.SendNoteOffToAllNotes();
            }
            catch (Exception ex)
            {
                Logger.Log($"Error stopping playback: {ex.Message}", Logger.LogTypes.Error);
            }
            finally
            {
                _isStopping = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }
        // Add this field to track the current playback task
        private Task _playbackTask = Task.CompletedTask;
        private bool _wasPlayingBeforeScroll = false;

        public async Task SetPosition(double positionPercent)
        {
            if (_frames == null || _frames.Count == 0)
                return;

            // Store playing state before any changes
            if (!_wasPlayingBeforeScroll)
            {
                _wasPlayingBeforeScroll = _isPlaying;
            }

            // Stop current playback if any
            if (_isPlaying)
            {
                Stop();

                // Wait for the previous task to complete
                if (_playbackTask != null && !_playbackTask.IsCompleted)
                {
                    try
                    {
                        await Task.WhenAny(_playbackTask, Task.Delay(1000));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Error waiting for playback task: {ex.Message}", Logger.LogTypes.Error);
                    }
                }
            }

            // Calculate new position
            _currentFrameIndex = (int)(positionPercent * _frames.Count / 100.0);
            _currentFrameIndex = Math.Max(0, Math.Min(_currentFrameIndex, _frames.Count - 1));

            // Reset the playback offset
            if (_currentFrameIndex < _frames.Count)
            {
                _playbackStartOffsetMs = TicksToMilliseconds(_frames[_currentFrameIndex].Time);
            }
            else
            {
                _playbackStartOffsetMs = 0;
            }

            // Reset the stopwatch
            _playbackStopwatch?.Reset();

            Logger.Log($"Position set to {positionPercent:0.00}% (frame {_currentFrameIndex} of {_frames.Count}, offset: {_playbackStartOffsetMs:0.00}ms)", Logger.LogTypes.Info);
        }
        // Update note labels with synchronization
        private HashSet<int> _lastDrawnNotes = new HashSet<int>();
        private void UpdateNoteLabelsSync(HashSet<int> activeNotes)
        {
            if (_lastDrawnNotes.SetEquals(activeNotes))
                return;

            _lastDrawnNotes = new HashSet<int>(activeNotes);
            panel1.SuspendLayout();

            var sortedNotes = activeNotes.OrderBy(note => note).ToList();
            for (int i = 0; i < _noteLabels.Length; i++)
            {
                Label label = _noteLabels[i];
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
                    if (label.BackColor != _originalLabelColors[label]) label.BackColor = _originalLabelColors[label];
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
        private async void checkBox_channel_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledChannels();

            if (_isPlaying)
            {
                double currentPositionPercent = (double)_currentFrameIndex / _frames.Count * 100;
                bool wasPlaying = _isPlaying;
                await SetPosition(currentPositionPercent);
                if (wasPlaying && !_isPlaying)
                {
                    Play();
                }
            }
            Logger.Log("Channel checkboxes changed", Logger.LogTypes.Info);
        }

        // Play multiple notes alternating
        private void PlayMultipleNotes(int[] frequencies, int duration)
        {
            _isAlternatingPlayback = true;
            // Convert frequencies to note numbers for highlighting
            var noteNumbers = frequencies.Select(freq => FrequencyToNoteNumber(freq)).ToArray();
            Stopwatch totalStopwatch = new Stopwatch();
            totalStopwatch.Start();

            // Determine interval based on UI settings first
            int interval;
            if (checkBox_make_each_cycle_last_30ms.Checked)
            {
                interval = 30;
            }
            else
            {
                interval = Convert.ToInt32(numericUpDown_alternating_note.Value);
            }
            interval = Math.Max(1, interval); // Ensure interval is at least 1 ms

            if (checkBox_play_each_note.Checked)
            {
                // --- Play each note once per cycle ---

                // Calculate cycle duration
                int cycleDuration;
                if (checkBox_make_each_cycle_last_30ms.Checked)
                {
                    cycleDuration = 30; // 30ms cycle duration
                }
                else
                {
                    cycleDuration = interval; // User-defined interval
                }

                // Check if there's enough time left for another cycle
                if (totalStopwatch.ElapsedMilliseconds + cycleDuration > duration)
                {
                    // Skip the cycle if not enough time remains
                    return;
                }

                Stopwatch cycleStopwatch = Stopwatch.StartNew();

                // Time per note in the cycle
                int timePerNote = Math.Max(1, cycleDuration / frequencies.Length);

                // Play all notes in the cycle
                for (int i = 0; i < frequencies.Length; i++)
                {
                    // Check if total duration exceeded
                    if (totalStopwatch.ElapsedMilliseconds >= duration) break;

                    int notePlayDuration;
                    if (checkBox_make_each_cycle_last_30ms.Checked)
                    {
                        // Every note lasts 15ms or less
                        notePlayDuration = Math.Min(15, timePerNote);
                    }
                    else
                    {
                        // User-defined interval
                        notePlayDuration = Math.Min(timePerNote, (int)(duration - totalStopwatch.ElapsedMilliseconds));
                    }

                    notePlayDuration = Math.Max(1, notePlayDuration);

                    Stopwatch noteStopwatch = Stopwatch.StartNew();

                    // Play note and highlight
                    int currentNoteIndex = i;
                    HighlightNoteLabel(currentNoteIndex);
                    NotePlayer.play_note(frequencies[currentNoteIndex], notePlayDuration);
                    UnHighlightNoteLabel(currentNoteIndex);

                    noteStopwatch.Stop();

                    // Gap between notes
                    int noteGap = Math.Max(0, timePerNote - (int)noteStopwatch.ElapsedMilliseconds);
                    if (noteGap > 0 && i < frequencies.Length - 1)
                    {
                        HighPrecisionSleep.Sleep(noteGap);
                    }
                }

                cycleStopwatch.Stop();

                // Silence until the cycle duration is complete
                int cycleElapsed = (int)cycleStopwatch.ElapsedMilliseconds;
                int remainingSilence = Math.Max(0, duration - (int)totalStopwatch.ElapsedMilliseconds);

                // Clean all note labels
                UpdateNoteLabels(new HashSet<int>());

                // Add remaining silence to complete the cycle duration
                if (remainingSilence > 0)
                {
                    HighPrecisionSleep.Sleep(remainingSilence);
                }
            }
            else
            {
                // --- Alternate notes for the whole duration (because the system speaker can't play multiple notes same time)---
                int notesPerCycle = frequencies.Length;
                double timePerNote = (double)interval / notesPerCycle;
                int noteIndex = 0;

                while (totalStopwatch.ElapsedMilliseconds < duration)
                {
                    int notePlayDuration;
                    if (checkBox_make_each_cycle_last_30ms.Checked)
                    {
                        notePlayDuration = Math.Min(15, Math.Max(1, (int)Math.Round((double)interval / frequencies.Length, MidpointRounding.ToZero)));
                    }
                    else
                    {
                        notePlayDuration = interval;
                    }

                    Stopwatch noteStopwatch = new Stopwatch();
                    noteStopwatch.Start();

                    HighlightNoteLabel(noteIndex);
                    NotePlayer.play_note(frequencies[noteIndex], notePlayDuration);
                    UnHighlightNoteLabel(noteIndex);

                    noteStopwatch.Stop();
                    long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                    double timeToNextNote = timePerNote - noteElapsed;
                    if (timeToNextNote > 0)
                    {
                        HighPrecisionSleep.Sleep((int)timeToNextNote);
                    }

                    noteIndex = (noteIndex + 1) % frequencies.Length;
                }
            }

            totalStopwatch.Stop();
            _isAlternatingPlayback = false;
        }

        private int FrequencyToNoteNumber(int frequency)
        {
            // Convert frequency to MIDI note number using A4 = 440Hz as reference (MIDI note 69)
            return (int)Math.Round(69 + 12 * Math.Log2(frequency / 440.0));
        }

        private void HighlightNoteLabel(int noteIndex)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HighlightNoteLabel(noteIndex)));
                return;
            }

            if (noteIndex >= 0 && noteIndex < _noteLabels.Length)
            {
                Label label = _noteLabels[noteIndex];
                label.BackColor = _highlightColor;
            }
        }

        private void UnHighlightNoteLabel(int noteIndex)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UnHighlightNoteLabel(noteIndex)));
                return;
            }

            if (noteIndex >= 0 && noteIndex < _noteLabels.Length)
            {
                Label label = _noteLabels[noteIndex];
                label.BackColor = _originalLabelColors[label];
            }
        }
        private int NoteToFrequency(int noteNumber)
        {
            // MIDI note number to frequency conversion
            return (int)(880.0 * Math.Pow(2.0, (noteNumber - 69) / 12.0));
        }
        private void UpdateTimeAndPercentPosition(int frameIndex)
        {
            if (_frames == null || _frames.Count == 0)
                return;

            long lastTick = _midiFile.Events
                .Select(track => track.LastOrDefault(ev => ev.CommandCode == MidiCommandCode.MetaEvent && ((MetaEvent)ev).MetaEventType == MetaEventType.EndTrack)?.AbsoluteTime ?? 0)
                .Max();
            double totalDurationMs = TicksToMilliseconds(lastTick);

            double currentTimeMs = TicksToMilliseconds(_frames[frameIndex].Time);
            double percent = ((double)_currentFrameIndex + 1 / _frames.Count) * 100.0;

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
        private async Task Rewind()
        {
            // Reset lyric state
            _lastLyricTime = DateTime.MinValue;
            _isInLyricSection = false;
            ClearLyrics();
            trackBar1.Value = 0;
            int positionPercent = trackBar1.Value / 10;
            await SetPosition(positionPercent);
            UpdateTimeAndPercentPosition(positionPercent);
            driftMs = 0; // Reset drifts
            ResetLabelsAndTrackBar();
            if (_wasPlayingBeforeScroll)
            {
                Play();
                _wasPlayingBeforeScroll = false;
            }
            Logger.Log("Rewind completed", Logger.LogTypes.Info);
        }
        private string MidiNoteToName(int noteNumber)
        {
            // Define note names (C, C#, D, D#, E, F, F#, G, G#, A, A#, B)
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            // Calculate the octave (MIDI note 60 is middle C, which is C4)
            int octave = (noteNumber / 12) - 1;

            // Calculate the note name index (0-11) within the octave
            int noteIndex = noteNumber % 12;

            // Format the note name with its octave
            return $"{noteNames[noteIndex]}{octave + 1}";
        }
        private Dictionary<int, Color> _activeNoteColors = new Dictionary<int, Color>();
        private Color _highlightColor = Settings1.Default.note_indicator_color; // You can choose any color
        private HashSet<int> _previousActiveNotes = new HashSet<int>();

        private void UpdateNoteLabels(HashSet<int> activeNotes)
        {
            if (_isUpdatingLabels) return; _isUpdatingLabels = true; try
            { // Sort notes once, outside the UI update action
                var sortedNotes = activeNotes.OrderBy(note => note).ToList();
                Action updateAction = () =>
                { // Reset all labels
                    foreach (var label in _noteLabels)
                    { label.Visible = false; label.BackColor = _originalLabelColors[label]; }
                    // Process active notes with better mapping
                    for (int i = 0; i < Math.Min(sortedNotes.Count, _noteLabels.Length); i++)
                    {
                        int noteNumber = sortedNotes[i];
                        Label label = _noteLabels[i];

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
                if (_noteLabels.Length > 0 && _noteLabels[0].InvokeRequired)
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
                _playbackRestartTimer?.Stop();
                _playbackRestartTimer?.Dispose();
                lyricsOverlay?.Dispose();
            }
            catch (Exception ex)
            {
                MessageForm.Show($"{Resources.MessageErrorClosingForm} {ex.Message}");
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
            _scrollDebounceCts?.Cancel();
            _scrollDebounceCts = new CancellationTokenSource();
            _pendingPositionPercent = positionPercent;

            var token = _scrollDebounceCts.Token;

            // Start debounce task
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(100, token); // Debounce delay

                    // Invoke on UI thread
                    this.BeginInvoke(async () =>
                    {
                        try
                        {
                            // Set new position and wait for it to complete
                            await SetPosition(_pendingPositionPercent);

                            // Update the UI labels
                            int frameIndex = (int)(_pendingPositionPercent * _frames.Count / 100.0);
                            frameIndex = Math.Max(0, Math.Min(frameIndex, _frames.Count - 1));

                            if (_frames.Count > 0)
                            {
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
                            Logger.Log($"Error in debounced trackBar1_Scroll: {ex.Message}", Logger.LogTypes.Error);
                        }

                        // Start the playback restart timer
                        _playbackRestartTimer?.Stop();
                        _playbackRestartTimer?.Dispose();
                        _playbackRestartTimer = new System.Timers.Timer(300);
                        _playbackRestartTimer.Elapsed += OnPlaybackRestartTimer;
                        _playbackRestartTimer.AutoReset = false;
                        _playbackRestartTimer.Start();

                        _isUserScrolling = false;
                    });
                }
                catch (OperationCanceledException)
                {
                    // Debounce was cancelled, do nothing
                }
            }, token);
        }
        private async void OnPlaybackRestartTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            _playbackRestartTimer?.Stop();
            _playbackRestartTimer?.Dispose();
            _playbackRestartTimer = null;

            this.BeginInvoke(async () =>
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
            });
        }

        private Label[] _noteLabels;
        private Dictionary<int, int> _noteToLabelMap;
        private Dictionary<Label, Color> _originalLabelColors = new Dictionary<Label, Color>();

        // Initializes the note labels and their properties
        private void InitializeNoteLabels()
        {
            // Collect all labels
            _noteLabels = new Label[32];
            for (int i = 1; i <= 32; i++)
            {
                _noteLabels[i - 1] = (Label)this.Controls.Find($"label_note{i}", true)[0];
                _noteLabels[i - 1].BackColor = set_inactive_note_color.GetInactiveNoteColor(Settings1.Default.note_indicator_color);
                _noteLabels[i - 1].ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
                _noteLabels[i - 1].Visible = false; // Initially hide all labels

                // Store the original color
                _originalLabelColors[_noteLabels[i - 1]] = _noteLabels[i - 1].BackColor;
            }
            foreach (var label in _noteLabels)
            {
                typeof(Label).InvokeMember("DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, label, new object[] { true });
            }
            // Create a mapping from MIDI note numbers to label indices
            // This mapping assumes MIDI notes 0-128 correspond to labels 1-32
            _noteToLabelMap = new Dictionary<int, int>();
            // Maps MIDI notes 0-128 to labels 1-32
            for (int i = 0; i <= 128; i++)
            {
                _noteToLabelMap[i] = i - 60;
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
        private int _highlightDuration;

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

        private double TicksToMilliseconds(long ticks)
        {
            // Find tempo events in right order
            int index = _precomputedTempoTimes.BinarySearch((ticks, 0), Comparer<(long, double)>.Create((x, y) => x.Item1.CompareTo(y.Item1)));
            if (index < 0)
            {
                index = ~index - 1;
            }

            // Use default tempo (120 BPM)
            if (index < 0)
            {
                return (double)ticks * 500000 / _ticksPerQuarterNote / 1000.0;
            }

            var lastTempoEvent = _precomputedTempoTimes[index];
            double cumulativeMs = lastTempoEvent.cumulativeMs;
            long lastTicks = lastTempoEvent.time;
            int lastTempo = _tempoEvents.FirstOrDefault(e => e.time == lastTicks).tempo;

            // More precise calculation
            cumulativeMs += (double)(ticks - lastTicks) * lastTempo / _ticksPerQuarterNote / 1000.0;
            return cumulativeMs;
        }
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

            // --- UI Update Block ---
            // Update the UI periodically on the UI thread if needed
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

            if (_currentFrameIndex >= _frames.Count - 1)
            {
                HandlePlaybackComplete();
                return;
            }

            // --- Sound Processing Block ---
            // Skip if previous playback task is still running
            if (!_playbackTask.IsCompleted)
            {
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
        }
        private DateTime _lastLyricTime = DateTime.MinValue;
        private bool _isInLyricSection = false;
        int driftMs = 0;
        private HashSet<int> _previousMidiOutputNotes = new();
        private async Task ProcessCurrentFrame()
        {
            var token = _cancellationTokenSource?.Token ?? CancellationToken.None;
            token.ThrowIfCancellationRequested();

            Stopwatch driftStopwatch = Stopwatch.StartNew();
            var currentFrame = _frames[_currentFrameIndex];
            var currentTime = currentFrame.Time;

            // --- Event-based MIDI output logic ---
            if (TemporarySettings.MIDIDevices.useMIDIoutput)
            {
                // Take events from preprocessed dictionary
                if (_eventsByTime.TryGetValue(currentTime, out var eventsAtThisTime))
                {
                    // 1. Process Note Off events
                    var noteOffEvents = eventsAtThisTime.OfType<NoteEvent>().Where(n => !MidiEvent.IsNoteOn(n));
                    foreach (var noteOff in noteOffEvents)
                    {
                        // Check if this channel enabled
                        if (_enabledChannels.Contains(noteOff.Channel))
                        {
                            MIDIIOUtils.SendNoteOff(noteOff.NoteNumber, noteOff.Channel - 1);
                        }
                    }

                    // 2. Process all Note On events
                    var noteOnEvents = eventsAtThisTime.OfType<NoteOnEvent>().Where(n => n.Velocity > 0);
                    foreach (var noteOn in noteOnEvents)
                    {
                        // Check if this channel enabled
                        if (_enabledChannels.Contains(noteOn.Channel))
                        {
                            _noteInstruments.TryGetValue((noteOn.NoteNumber, currentTime), out int instrument);
                            MIDIIOUtils.SendNoteOn(noteOn.NoteNumber, instrument, noteOn.Channel - 1);
                        }
                    }
                }
            }

            // --- Playing with system speaker (aka PC speaker) logic ---
            HashSet<int> filteredNotes = new HashSet<int>();
            foreach (var note in currentFrame.ActiveNotes)
            {
                if (_noteChannels.TryGetValue(note, out int channel) && _enabledChannels.Contains(channel))
                    filteredNotes.Add(note);
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

            int durationMsInt = Math.Max(0, (int)Math.Floor(durationMs));
            if (driftMs > 0)
            {
                durationMsInt = Math.Max(0, durationMsInt - driftMs);
                driftMs = Math.Max(0, driftMs - (int)durationMs);
            }
            else if (driftMs < 0)
            {
                durationMsInt -= driftMs;
                driftMs = 0;
            }

            if (durationMsInt <= 0 && filteredNotes.Count == 0)
            {
                driftMs = (int)(driftStopwatch.ElapsedMilliseconds - durationMsInt);
                return;
            }

            if (checkBox_show_lyrics_or_text_events.Checked)
            {
                HandleLyricsDisplay(currentTime);
            }

            if (filteredNotes.Count == 0)
            {
                await WaitPreciseWithCancellation(durationMsInt, token);
            }
            else
            {
                var frequencies = filteredNotes.Select(note => NoteToFrequency(note)).ToArray();
                if (frequencies.Length == 1)
                {
                    await Task.Run(() => NotePlayer.play_note(frequencies[0], durationMsInt), token);
                }
                else
                {
                    await Task.Run(() => PlayMultipleNotes(frequencies, durationMsInt), token);
                }
            }
            driftMs = (int)(driftStopwatch.ElapsedMilliseconds - durationMsInt);
        }
        private int GetCurrentTempo(long currentTime)
        {
            // Last tempo event before or at currentTime
            var lastTempoEvent = _tempoEvents
                .Where(t => t.time <= currentTime)
                .LastOrDefault();

            return lastTempoEvent.tempo != 0 ? lastTempoEvent.tempo : 500000; // Default 120 BPM
        }

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
            // If the tempo is higher, thresholds decrease, and vice versa
            double tempoRatio = baseBpm / bpm;

            int lyricGapThreshold = (int)(baseLyricGap * tempoRatio);
            int melodySectionThreshold = (int)(baseMelodySection * tempoRatio);

            // Add bounds to thresholds
            lyricGapThreshold = Math.Max(500, Math.Min(5000, lyricGapThreshold));    // Between 0.5-5 seconds
            melodySectionThreshold = Math.Max(1000, Math.Min(10000, melodySectionThreshold)); // Between 1-10 seconds

            return (lyricGapThreshold, melodySectionThreshold);
        }

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
                    // Shorter delay
                }
                else
                {
                    // The lyric is already cleaned
                    if (timeSinceLastLyric > melodySectionThreshold)
                    {
                        // Do nothing, if there isn't any lyric
                    }
                }
            }
        }
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
        private void ProcessLyricText(string lyrics)
        {
            if (lyrics.Contains("\n") || lyrics.Contains("\\") || lyrics.Contains("/") ||
                lyrics.Contains("\r") || lyrics.Contains("\t") || lyrics.Contains("\0") ||
                lyrics.Contains("\f") || lyrics.Contains("\v") || lyrics.Contains("|"))
            {
                lyricRow = string.Empty; // Clear previous lyrics if there's a newline
            }

            lyricRow += lyrics;
            lyricRow = lyricRow
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("/", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\0", string.Empty)
                .Replace("\f", string.Empty)
                .Replace("\v", string.Empty)
                .Replace("|", string.Empty);

            PrintLyrics(lyricRow.Trim());
        }
        private async Task WaitPreciseWithCancellation(int milliseconds, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(milliseconds, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Logger.Log($"Silent frame wait was canceled after {milliseconds}ms", Logger.LogTypes.Info);
                throw;
            }
        }
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
        private void UpdateAllUISync(int frameIndex, HashSet<int> filteredNotes)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateAllUISync(frameIndex, filteredNotes)));
                return;
            }

            // Prevent the conflict during trackBar update
            if (!_isTrackBarBeingDragged)
            {
                // Update the position of trackBar
                if (_frames.Count > 0)
                {
                    int trackbarValue = (int)(1000.0 * frameIndex / _frames.Count);
                    if (trackbarValue <= trackBar1.Maximum && trackbarValue != trackBar1.Value)
                    {
                        _isUserScrolling = false; // State that it's a program update
                        trackBar1.Value = trackbarValue;
                    }
                }
            }

            // Update the percentage label
            string percentagestr = Resources.TextPercent.Replace("{number}", ((double)frameIndex / _frames.Count * 100).ToString("0.00", CultureInfo.CurrentCulture));
            label_percentage.Text = percentagestr;

            // Update the position label
            UpdatePositionLabel();

            if (!checkBox_dont_update_grid.Checked)
            {
                // Update note labels
                UpdateNoteLabelsSync(filteredNotes);
            }

            // Update the label of notes that being held on
            holded_note_label.Text = $"{Properties.Resources.TextHeldNotes} ({filteredNotes.Count})";
        }
        private void UpdatePositionLabel()
        {
            if (!_isPlaying) return;

            // Playback time based on stopwatch
            double songTimeMs = _playbackStartOffsetMs + _playbackStopwatch.ElapsedMilliseconds;

            // Frame time based on current frame
            double frameTimeMs = 0;
            if (_frames != null && _frames.Count > 0 && _currentFrameIndex < _frames.Count)
            {
                frameTimeMs = TicksToMilliseconds(_frames[_currentFrameIndex].Time);
            }
            else
            {
                frameTimeMs = songTimeMs;
            }

            // Use average or frame time for more accuracy
            double accurateMs = (songTimeMs + frameTimeMs) / 2.0;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(accurateMs);
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
        private async void HandlePlaybackComplete()
        {
            try
            {
                // Process the completion of playback if the player is still playing
                if (!_isPlaying) return;

                playbackTimer.Stop();
                _wasPlayingBeforeScroll = false;
                if (checkBox_loop.Checked)
                {
                    Logger.Log("Playback loop enabled. Rewinding.", Logger.LogTypes.Info);
                    await Rewind();
                    // Restart playback if looping is enabled
                    Play();
                }
                else
                {
                    Logger.Log("Playback finished.", Logger.LogTypes.Info);
                    Stop();
                    await Rewind();
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"An error occurred in HandlePlaybackComplete: {ex.Message}", Logger.LogTypes.Error);
                // Stop playback if an error occurs
                Stop();
            }
            finally
            {
                Logger.Log("Timer-based playback completed", Logger.LogTypes.Info);
            }
        }

        private void MIDI_file_player_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
        private Dictionary<int, int> _channelInstruments = new();
        private Dictionary<(int note, long time), int> _noteInstruments = new();

        private void AssignInstrumentsToNotes(MidiFile midiFile)
        {
            var lastPatchPerChannel = new Dictionary<int, int>();

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.PatchChange)
                    {
                        var patch = (PatchChangeEvent)midiEvent;
                        lastPatchPerChannel[patch.Channel] = patch.Patch;
                    }
                    else if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        var noteEvent = (NoteOnEvent)midiEvent;
                        int instrument;
                        if (noteEvent.Channel == 9) // Channel 10 (percussion)
                            instrument = -1;
                        else
                            instrument = lastPatchPerChannel.TryGetValue(noteEvent.Channel, out var patch) ? patch : 0;
                        _noteInstruments[(noteEvent.NoteNumber, noteEvent.AbsoluteTime)] = instrument;
                    }
                }
            }
        }
        private void PrintLyrics(string lyrics)
        {
            lyricsOverlay.PrintLyrics(lyrics);
        }
        private void ClearLyrics()
        {
            lyricRow = string.Empty;
            if (lyricsOverlay != null && !lyricsOverlay.IsDisposed && !lyricsOverlay.Disposing)
            {
                lyricsOverlay.ClearLyrics();
            }
        }
        private void ShowLyricsOverlay()
        {
            if (lyricsOverlay == null || lyricsOverlay.IsDisposed)
            {
                lyricsOverlay = new LyricsOverlay();
                lyricsOverlay.Owner = this;
            }

            if (!lyricsOverlay.Visible)
            {
                lyricsOverlay.Show(this);
            }

            BeginInvoke((Action)(() => this.Activate()));
        }
        private void HideLyricsOverlay()
        {
            if (lyricsOverlay != null && !lyricsOverlay.IsDisposed)
            {
                lyricsOverlay.Hide();
            }
        }
        private void checkBox_show_lyrics_or_text_events_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_show_lyrics_or_text_events.Checked)
            {
                Logger.Log("Show lyrics is enabled.", Logger.LogTypes.Info);
                ShowLyricsOverlay();
            }
            else
            {
                Logger.Log("Show lyrics is disabled.", Logger.LogTypes.Info);
                HideLyricsOverlay();
            }
        }
    }
}