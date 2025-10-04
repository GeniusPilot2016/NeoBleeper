using NAudio.Midi;
using NeoBleeper.Properties;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public partial class MIDI_file_player : Form
    {
        private Dictionary<long, List<MetaEvent>> _metaEventsByTime = new Dictionary<long, List<MetaEvent>>();
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
            UIFonts.setFonts(this);
            set_theme();
            _playbackStopwatch = new Stopwatch();
            textBox1.Text = filename;
            LoadMIDI(filename);
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
        private void dark_theme()
        {
            darkTheme = true;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            textBox1.BackColor = Color.Black;
            textBox1.ForeColor = Color.White;
            groupBox1.ForeColor = Color.White;
            button_browse_file.BackColor = Color.FromArgb(32, 32, 32);
            numericUpDown_alternating_note.BackColor = Color.Black;
            numericUpDown_alternating_note.ForeColor = Color.White;
        }

        private void light_theme()
        {
            darkTheme = false;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            textBox1.BackColor = SystemColors.Window;
            textBox1.ForeColor = SystemColors.WindowText;
            groupBox1.ForeColor = SystemColors.ControlText;
            button_browse_file.BackColor = Color.Transparent;
            numericUpDown_alternating_note.BackColor = SystemColors.Window;
            numericUpDown_alternating_note.ForeColor = SystemColors.WindowText;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            Stop();
            openFileDialog.Filter = "MIDI Files|*.mid";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (MIDIFileValidator.IsMidiFile(openFileDialog.FileName))
                {
                    textBox1.Text = openFileDialog.FileName;
                    await LoadMIDI(openFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show(Resources.MessageNonValidMIDIFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        MessageBox.Show(Resources.MessageMIDIFilePlayerNonSupportedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    Logger.Log("The file you dragged is corrupted or the file is in use by another process.", Logger.LogTypes.Error);
                    MessageBox.Show(Resources.MessageCorruptedOrCurrentlyUsedFile, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                // Offload heavy processing to a background thread
                await Task.Run(() =>
                {
                    var midiFile = new MidiFile(filename, false);
                    _midiFile = midiFile;
                    _ticksPerQuarterNote = midiFile.DeltaTicksPerQuarterNote;
                    _tempoEvents = new List<(long time, int tempo)>();

                    // Extract tempo information
                    int microsecondsPerQuarterNote = 500000; // Default 120 BPM
                    int trackCount = midiFile.Events.Tracks;
                    int trackIndex = 0;

                    foreach (var track in midiFile.Events)
                    {
                        foreach (var midiEvent in track)
                        {
                            if (midiEvent is TempoEvent tempoEvent)
                            {
                                _tempoEvents.Add((tempoEvent.AbsoluteTime, tempoEvent.MicrosecondsPerQuarterNote));
                            }
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
                                // Collect lyric/text meta events so we can create frames at their times
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
                        int percent = 30 + (int)(20.0 * processedTracks / totalTracks); // Between %30 and %50
                        UpdateProgressBar(percent, $"{Resources.TextEventsAreBeingCollected} ({processedTracks}/{totalTracks})");
                    }

                    // Sort events by time
                    UpdateProgressBar(55, Resources.TextEventsAreBeingSorted);
                    allEvents = allEvents.OrderBy(e => e.Time).ToList();

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
                            int percent = 60 + (int)(35.0 * i / totalTimePoints); // Between %60 and %95
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
            }
            catch (Exception ex)
            {
                labelStatus.Text = Resources.TextMIDIFileLoadingError;
                progressBar1.Visible = false;
                progressBar1.Value = 0;
                MessageBox.Show($"{Resources.MessageMIDIFileLoadingError} {ex.Message}");
                _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
                Logger.Log($"Error loading MIDI file: {ex.Message}", Logger.LogTypes.Error);
            }
            finally
            {
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
        public void Play()
        {
            Logger.Log($"Play called. IsPlaying: {_isPlaying}, Frames count: {_frames?.Count ?? 0}", Logger.LogTypes.Info);

            if (_isPlaying || _frames == null || _frames.Count == 0)
                return;

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
                MessageBox.Show($"{Resources.MessagePlaybackStartingError} {ex.Message}");
                _isPlaying = false;
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
                holded_note_label.Text = $"{Properties.Resources.TextHoldedNotes} (0)";
                label_more_notes.Visible = false;
                ClearLyrics();
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
            if (!_wasPlayingBeforeScroll) // Only store if not already stored
            {
                _wasPlayingBeforeScroll = _isPlaying;
            }

            // Stop current playback if any
            if (_isPlaying)
            {
                Stop();

                // Wait for the previous task to truly complete
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

            // Clamp the index to valid range
            _currentFrameIndex = Math.Max(0, Math.Min(_currentFrameIndex, _frames.Count - 1));

            Logger.Log($"Position set to {positionPercent}% (frame {_currentFrameIndex} of {_frames.Count})", Logger.LogTypes.Info);

            // Don't restart playback immediately if trackbar is being dragged
            // The timer in trackBar1_Scroll will handle restart after user stops dragging
        }
        // Update note labels with synchronization
        private HashSet<int> _lastDrawnNotes = new HashSet<int>();
        private void UpdateNoteLabelsSync(HashSet<int> activeNotes)
        {
            if (_lastDrawnNotes.SetEquals(activeNotes))
                return;

            _lastDrawnNotes = new HashSet<int>(activeNotes);
            panel1.SuspendLayout();

            // Update only the labels that are currently active
            var sortedNotes = activeNotes.OrderBy(note => note).ToList();
            int i = 0;
            for (; i < Math.Min(sortedNotes.Count, _noteLabels.Length); i++)
            {
                int noteNumber = sortedNotes[i];
                Label label = _noteLabels[i];
                string noteName = MidiNoteToName(noteNumber);

                if (!label.Visible) label.Visible = true;
                if (label.Text != noteName) label.Text = noteName;
                if (label.BackColor != _highlightColor) label.BackColor = _highlightColor;
            }
            // Reset remaining labels if any
            for (; i < _noteLabels.Length; i++)
            {
                Label label = _noteLabels[i];
                if (label.Visible) label.Visible = false;
                if (label.BackColor != _originalLabelColors[label]) label.BackColor = _originalLabelColors[label];
                if (!string.IsNullOrEmpty(label.Text)) label.Text = "";
            }

            // If there's more notes
            if (sortedNotes.Count > _noteLabels.Length)
            {
                if (!label_more_notes.Visible) label_more_notes.Visible = true;
                int extraNotes = sortedNotes.Count - _noteLabels.Length;
                string localizedMoreText = Resources.MoreText; // ({number} More)
                localizedMoreText = localizedMoreText.Replace("{number}", extraNotes.ToString());
                string moreText = localizedMoreText;
                if (label_more_notes.Text != moreText) label_more_notes.Text = moreText;
            }
            else
            {
                if (label_more_notes.Visible) label_more_notes.Visible = false;
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
            switch (checkBox_play_each_note.Checked)
            {
                case true:
                    {
                        switch (checkBox_make_each_cycle_last_30ms.Checked)
                        {
                            case true:
                                {
                                    int interval = 30;

                                    if (frequencies.Length >= (duration / interval))
                                    {
                                        // More notes than cycles - play each note once in each cycle
                                        int noteIndex = 0;
                                        int notesPerCycle = frequencies.Length;
                                        double timePerNote = (double)interval / notesPerCycle;

                                        while (totalStopwatch.ElapsedMilliseconds < duration)
                                        {
                                            long currentTotalElapsed = totalStopwatch.ElapsedMilliseconds;
                                            double expectedNoteStartTime = noteIndex * timePerNote;
                                            double absoluteNoteStartTime = expectedNoteStartTime;

                                            if (absoluteNoteStartTime > duration) break;

                                            int alternatingTime = Math.Min(15, Math.Max(1, (int)Math.Round((double)interval / frequencies.Length, MidpointRounding.ToEven)));

                                            // Play the note
                                            Stopwatch noteStopwatch = new Stopwatch();
                                            noteStopwatch.Start();

                                            HighlightNoteLabel(noteIndex);
                                            NotePlayer.play_note(frequencies[noteIndex], alternatingTime);
                                            UnHighlightNoteLabel(noteIndex);
                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            // Calculate the time to sleep to maintain the rhythm
                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                HighPrecisionSleep.Sleep((int)timeToNextNote);
                                            }

                                            noteIndex = (noteIndex + 1) % frequencies.Length;
                                        }
                                    }
                                    else
                                    {
                                        // Fewer notes than cycles - play each note once then wait
                                        double timePerNote = (double)duration / frequencies.Length;

                                        for (int i = 0; i < frequencies.Length; i++)
                                        {
                                            long currentTotalElapsed = totalStopwatch.ElapsedMilliseconds;
                                            double expectedNoteStartTime = i * timePerNote;
                                            double absoluteNoteStartTime = expectedNoteStartTime;

                                            if (absoluteNoteStartTime > duration) break;

                                            int alternatingTime = Math.Min(15, Math.Max(1, (int)Math.Round((double)interval / frequencies.Length, MidpointRounding.ToEven)));

                                            Stopwatch noteStopwatch = new Stopwatch();
                                            noteStopwatch.Start();

                                            HighlightNoteLabel(i);
                                            NotePlayer.play_note(frequencies[i], alternatingTime);
                                            UnHighlightNoteLabel(i);

                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                HighPrecisionSleep.Sleep((int)timeToNextNote);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case false:
                                {
                                    int interval = Convert.ToInt32(numericUpDown_alternating_note.Value);

                                    // Ensure interval is at least 1 ms
                                    interval = Math.Max(1, interval);

                                    if (frequencies.Length >= (duration / interval))
                                    {
                                        // More notes than cycles - cycle through notes
                                        int noteIndex = 0;
                                        int notesPerCycle = frequencies.Length;
                                        double timePerNote = (double)interval / notesPerCycle;

                                        while (totalStopwatch.ElapsedMilliseconds < duration)
                                        {
                                            long currentTotalElapsed = totalStopwatch.ElapsedMilliseconds;
                                            double expectedNoteStartTime = noteIndex * timePerNote;
                                            double absoluteNoteStartTime = expectedNoteStartTime;

                                            if (absoluteNoteStartTime > duration) break;

                                            Stopwatch noteStopwatch = new Stopwatch();
                                            noteStopwatch.Start();

                                            HighlightNoteLabel(noteIndex);
                                            NotePlayer.play_note(frequencies[noteIndex], interval);
                                            UnHighlightNoteLabel(noteIndex);
                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            // Calculate the time to sleep to maintain the rhythm
                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                HighPrecisionSleep.Sleep((int)timeToNextNote);
                                            }

                                            noteIndex = (noteIndex + 1) % frequencies.Length;
                                        }
                                    }
                                    else
                                    {
                                        // Fewer notes than cycles - play each note once then wait
                                        double timePerNote = (double)duration / frequencies.Length;

                                        for (int i = 0; i < frequencies.Length; i++)
                                        {
                                            long currentTotalElapsed = totalStopwatch.ElapsedMilliseconds;
                                            double expectedNoteStartTime = i * timePerNote;
                                            double absoluteNoteStartTime = expectedNoteStartTime;

                                            if (absoluteNoteStartTime > duration) break;

                                            Stopwatch noteStopwatch = new Stopwatch();
                                            noteStopwatch.Start();

                                            HighlightNoteLabel(i);
                                            NotePlayer.play_note(frequencies[i], interval);
                                            UnHighlightNoteLabel(i);

                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                HighPrecisionSleep.Sleep((int)timeToNextNote);
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case false:
                    {
                        switch (checkBox_make_each_cycle_last_30ms.Checked)
                        {
                            case true:
                                {
                                    int interval = 30;
                                    int notesPerCycle = frequencies.Length;
                                    double timePerNote = (double)interval / notesPerCycle;

                                    int noteIndex = 0;
                                    while (totalStopwatch.ElapsedMilliseconds < duration)
                                    {
                                        long currentTotalElapsed = totalStopwatch.ElapsedMilliseconds;
                                        double expectedNoteStartTime = noteIndex * timePerNote;
                                        double absoluteNoteStartTime = expectedNoteStartTime;

                                        if (absoluteNoteStartTime > duration) break;

                                        int alternatingTime = Math.Min(15, Math.Max(1, (int)Math.Round((double)interval / frequencies.Length, MidpointRounding.ToEven)));

                                        Stopwatch noteStopwatch = new Stopwatch();
                                        noteStopwatch.Start();

                                        HighlightNoteLabel(noteIndex);
                                        NotePlayer.play_note(frequencies[noteIndex], alternatingTime);
                                        UnHighlightNoteLabel(noteIndex);

                                        noteStopwatch.Stop();
                                        long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                        // Calculate the time to sleep to maintain the rhythm
                                        double timeToNextNote = timePerNote - noteElapsed;
                                        if (timeToNextNote > 0)
                                        {
                                            HighPrecisionSleep.Sleep((int)timeToNextNote);
                                        }

                                        noteIndex = (noteIndex + 1) % frequencies.Length;
                                    }
                                    break;
                                }
                            case false:
                                {
                                    // Alternating notes with a custom interval
                                    int interval = Convert.ToInt32(numericUpDown_alternating_note.Value);

                                    // Ensure interval is at least 1 ms
                                    interval = Math.Max(1, interval);
                                    int notesPerCycle = frequencies.Length;
                                    double timePerNote = (double)interval / notesPerCycle;

                                    int noteIndex = 0;
                                    while (totalStopwatch.ElapsedMilliseconds < duration)
                                    {
                                        long currentTotalElapsed = totalStopwatch.ElapsedMilliseconds;
                                        double expectedNoteStartTime = noteIndex * timePerNote;
                                        double absoluteNoteStartTime = expectedNoteStartTime;

                                        if (absoluteNoteStartTime > duration) break;

                                        Stopwatch noteStopwatch = new Stopwatch();
                                        noteStopwatch.Start();

                                        HighlightNoteLabel(noteIndex);
                                        NotePlayer.play_note(frequencies[noteIndex], interval);
                                        UnHighlightNoteLabel(noteIndex);

                                        noteStopwatch.Stop();
                                        long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                        // Calculate the time to sleep to maintain the rhythm
                                        double timeToNextNote = timePerNote - noteElapsed;
                                        if (timeToNextNote > 0)
                                        {
                                            HighPrecisionSleep.Sleep((int)timeToNextNote);
                                        }

                                        noteIndex = (noteIndex + 1) % frequencies.Length;
                                    }
                                }
                                break;
                        }
                        break;
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
            double percent = (currentTimeMs / totalDurationMs) * 100.0;

            string timeStr = TimeSpan.FromMilliseconds(currentTimeMs).ToString(@"mm\:ss\.ff");

            string percentagestr = Resources.TextPercent.Replace("{number}", percent.ToString("0.00"));
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
        private async void Rewind()
        {
            // Reset lyric state
            _lastLyricTime = DateTime.MinValue;
            _isInLyricSection = false;
            ClearLyrics();
            trackBar1.Value = 0;
            int positionPercent = trackBar1.Value / 10;
            await SetPosition(positionPercent);
            UpdateTimeAndPercentPosition(positionPercent);

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
        }

        private void button_rewind_Click(object sender, EventArgs e)
        {
            Rewind();
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
                MessageBox.Show($"{Resources.MessageErrorClosingForm} {ex.Message}");
            }
        }

        private void disable_alternating_notes_panel(object sender, EventArgs e)
        {
            if (checkBox_play_each_note.Checked == true || checkBox_make_each_cycle_last_30ms.Checked == true)
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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // It's not user scroll if updating by the program
            if (!_isUserScrolling)
            {
                _isUserScrolling = true;
                ClearLyrics();
                return;
            }

            _lastTrackBarScrollTime = DateTime.Now;
            _isTrackBarBeingDragged = true;

            // Save state in first scroll
            if (!_wasPlayingBeforeScroll && _isPlaying)
            {
                _wasPlayingBeforeScroll = true;
            }

            int positionPercent = trackBar1.Value / 10;

            // Stop current restart timer
            _playbackRestartTimer?.Stop();
            _playbackRestartTimer?.Dispose();

            // Set position, but don't restart
            _ = Task.Run(async () =>
            {
                await SetPosition(positionPercent);

                // Update the UI in main thread
                this.BeginInvoke(new Action(() =>
                {
                    UpdateTimeAndPercentPosition(positionPercent);
                }));
            });

            // Timer for restarting after scrolling by user
            _playbackRestartTimer = new System.Timers.Timer(300); // 300ms latency
            _playbackRestartTimer.Elapsed += OnPlaybackRestartTimer;
            _playbackRestartTimer.AutoReset = false;
            _playbackRestartTimer.Start();
        }

        private void OnPlaybackRestartTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            _playbackRestartTimer?.Stop();
            _playbackRestartTimer?.Dispose();
            _playbackRestartTimer = null;

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
            // Find the last tempo event that occurred before or at the given ticks
            int index = _precomputedTempoTimes.BinarySearch((ticks, 0), Comparer<(long, double)>.Create((x, y) => x.Item1.CompareTo(y.Item1)));
            if (index < 0)
            {
                index = ~index - 1;
            }

            // Use the default tempo (120 BPM) if no tempo events are found
            if (index < 0)
            {
                return (double)ticks * 500000 / _ticksPerQuarterNote / 1000.0;
            }

            var lastTempoEvent = _precomputedTempoTimes[index];
            double cumulativeMs = lastTempoEvent.cumulativeMs;
            long lastTicks = lastTempoEvent.time;
            int lastTempo = _tempoEvents.FirstOrDefault(e => e.time == lastTicks).tempo;

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

            if (_currentFrameIndex >= _frames.Count - 1)
            {
                HandlePlaybackComplete();
                return;
            }

            // Prevent re-entrancy. If the last task is still running, skip this tick.
            if (!_playbackTask.IsCompleted)
            {
                return;
            }

            // More robust null check for _cancellationTokenSource
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                Logger.Log("CancellationTokenSource is null or canceled, stopping playback", Logger.LogTypes.Info);
                Stop();
                return;
            }

            // Start a new task to process all due frames.
            _playbackTask = Task.Run(async () =>
            {
                try
                {
                    var token = _cancellationTokenSource?.Token ?? CancellationToken.None;
                    if (_cancellationTokenSource != null)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    var elapsedMs = _playbackStopwatch.ElapsedMilliseconds;
                    var songTimeMs = _playbackStartOffsetMs + elapsedMs;

                    // Loop through all frames that should have been played by now
                    while (_currentFrameIndex < _frames.Count)
                    {
                        // Check if we're still playing and token is valid
                        if (!_isPlaying || _cancellationTokenSource == null)
                        {
                            Logger.Log("Playback stopped or token cancelled during frame processing", Logger.LogTypes.Info);
                            break;
                        }

                        var currentFrame = _frames[_currentFrameIndex];
                        var targetTimeMs = TicksToMilliseconds(currentFrame.Time);

                        if (targetTimeMs <= songTimeMs)
                        {
                            // This frame is due. Process it.
                            await ProcessCurrentFrame();
                            _currentFrameIndex++;
                        }
                        else
                        {
                            // This frame is in the future. We've caught up.
                            break;
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
            }, _cancellationTokenSource?.Token ?? CancellationToken.None);
        }
        private DateTime _lastLyricTime = DateTime.MinValue;
        private bool _isInLyricSection = false;

        private async Task ProcessCurrentFrame()
        {
            var token = _cancellationTokenSource?.Token ?? CancellationToken.None;

            // Cancellation check with null safety
            if (_cancellationTokenSource != null)
            {
                token.ThrowIfCancellationRequested();
            }

            var currentFrame = _frames[_currentFrameIndex];

            // Filter active notes based on enabled channels
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
                // Find greatest tick in MIDI file
                long lastTick = _midiFile.Events
                    .Select(track => track.LastOrDefault(ev => ev.CommandCode == MidiCommandCode.MetaEvent && ((MetaEvent)ev).MetaEventType == MetaEventType.EndTrack)?.AbsoluteTime ?? 0)
                    .Max();
                double totalDurationMs = TicksToMilliseconds(lastTick);
                durationMs = totalDurationMs - TicksToMilliseconds(currentFrame.Time);
            }

            int durationMsInt = Math.Max(1, (int)Math.Round(durationMs)); // Minimum 1ms

            // Update UI
            UpdateAllUISync(_currentFrameIndex, filteredNotes);

            // Handle silent frames with better cancellation handling
            if (filteredNotes.Count == 0)
            {
                // Use a more robust waiting mechanism for silent frames
                await WaitPreciseWithCancellation(durationMsInt, token);
                return; // Skip note playback for this frame
            }

            // Show lyrics if enabled — use pre-collected meta events for fast lookup
            if (checkBox_show_lyrics_or_text_events.Checked)
            {
                HandleLyricsDisplay(currentFrame.Time);
            }

            // Play notes
            var frequencies = filteredNotes.Select(note => NoteToFrequency(note)).ToArray();
            if (TemporarySettings.MIDIDevices.useMIDIoutput)
            {
                foreach (var noteNumber in filteredNotes)
                {
                    int instrument = 0;
                    _noteInstruments.TryGetValue((noteNumber, currentFrame.Time), out instrument);
                    if (_noteChannels.TryGetValue(noteNumber, out int channel) && channel != 10)
                    {
                        MIDIIOUtils.PlayMidiNoteAsync(noteNumber, durationMsInt, instrument);
                    }
                    else
                    {
                        MIDIIOUtils.PlayMidiNoteAsync(noteNumber, durationMsInt, -1, false, 9); // Channel 10
                    }
                }
            }
            if (frequencies.Length == 1)
            {
                var noteNumber = filteredNotes.First();
                int noteIndex = _noteToLabelMap[noteNumber];
                HighlightNoteLabel(noteIndex);
                if (checkBox_play_each_note.Checked)
                {
                    if (checkBox_make_each_cycle_last_30ms.Checked)
                    {
                        int length = Math.Min(15, durationMsInt);
                        int remainingTime = durationMsInt - length;
                        await Task.Run(() =>
                        {
                            NotePlayer.play_note(frequencies[0], length);
                            UnHighlightNoteLabel(noteIndex);
                            if (remainingTime > 0)
                            {
                                HighPrecisionSleep.Sleep(remainingTime);
                            }
                        }, token);
                    }
                    else
                    {
                        int length = Math.Min((int)numericUpDown_alternating_note.Value, durationMsInt);
                        int remainingTime = durationMsInt - length;
                        await Task.Run(() =>
                        {
                            NotePlayer.play_note(frequencies[0], length);
                            UnHighlightNoteLabel(noteIndex);
                            if (remainingTime > 0)
                            {
                                HighPrecisionSleep.Sleep(remainingTime);
                            }
                        }, token);
                    }
                }
                else
                {
                    await Task.Run(() =>
                    {
                        NotePlayer.play_note(frequencies[0], durationMsInt);
                    }, token);
                    UnHighlightNoteLabel(noteIndex);
                }
            }
            else
            {
                await Task.Run(() => PlayMultipleNotes(frequencies, durationMsInt), token);
            }
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
            lyricGapThreshold = Math.Max(500, Math.Min(5000, lyricGapThreshold));    // 0.5-5 saniye arası
            melodySectionThreshold = Math.Max(1000, Math.Min(10000, melodySectionThreshold)); // 1-10 saniye arası

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
            string percentagestr = Resources.TextPercent.Replace("{number}", ((double)frameIndex / _frames.Count * 100).ToString("0.00"));
            label_percentage.Text = percentagestr;

            // Update the position label
            UpdatePositionLabel();

            if (!checkBox_dont_update_grid.Checked)
            {
                // Not labellarını güncelle
                UpdateNoteLabelsSync(filteredNotes);
            }

            // Update the label of notes that being held on
            holded_note_label.Text = $"{Properties.Resources.TextHoldedNotes} ({filteredNotes.Count})";
        }
        private void UpdatePositionLabel()
        {
            if (!_isPlaying) return;

            // Actual playing duration (starting offset + elapsed time)
            double songTimeMs = _playbackStartOffsetMs + _playbackStopwatch.ElapsedMilliseconds;

            // Show the time as minute:seconds:split seconds format
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
        private void HandlePlaybackComplete()
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
                    Rewind();
                    // Restart playback if looping is enabled
                    Play();
                }
                else
                {
                    Logger.Log("Playback finished.", Logger.LogTypes.Info);
                    Stop();
                    Rewind();
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