using NAudio.Midi;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public partial class MIDI_file_player : Form
    {
        private bool _isAlternatingPlayback = false;
        bool is_playing = false;
        private List<int> _displayOrder = new List<int>();
        private long _playbackStartTime;
        private long _nextFrameTime;
        private bool _isStopping = false;
        private bool _isUpdatingLabels = false;
        private Stopwatch _playbackStopwatch;
        public MIDI_file_player(string filename)
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            set_theme();
            _playbackStopwatch = new Stopwatch();
            textBox1.Text = filename;
            LoadMIDI(filename);
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
        private void dark_theme()
        {
            Application.DoEvents();
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            textBox1.BackColor = Color.Black;
            textBox1.ForeColor = Color.White;
            groupBox1.ForeColor = Color.White;
            button_browse_file.BackColor = Color.FromArgb(32, 32, 32);
            numericUpDown_alternating_note.BackColor = Color.Black;
            numericUpDown_alternating_note.ForeColor = Color.White;
            this.Refresh();
        }

        private void light_theme()
        {
            Application.DoEvents();
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            textBox1.BackColor = SystemColors.Window;
            textBox1.ForeColor = SystemColors.WindowText;
            groupBox1.ForeColor = SystemColors.ControlText;
            button_browse_file.BackColor = Color.Transparent;
            numericUpDown_alternating_note.BackColor = SystemColors.Window;
            numericUpDown_alternating_note.ForeColor = SystemColors.WindowText;
            this.Refresh();
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

                // MIDI files always start with the header MThd
                return header[0] == 'M' && header[1] == 'T' && header[2] == 'h' && header[3] == 'd';
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            Stop();
            openFileDialog.Filter = "MIDI Files|*.mid";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (IsMidiFile(openFileDialog.FileName))
                {
                    textBox1.Text = openFileDialog.FileName;
                    await LoadMIDI(openFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("This file is not a valid MIDI file or the file is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine("This file is not a valid MIDI file or the file is corrupted.");
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
                    if (IsMidiFile(fileName))
                    {
                        textBox1.Text = fileName;
                        await LoadMIDI(fileName);
                    }
                    else
                    {
                        Debug.WriteLine("The file you dragged is not supported by NeoBleeper MIDI player or is corrupted.");
                        MessageBox.Show("The file you dragged is not supported by NeoBleeper MIDI player or is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("The file you dragged is corrupted or the file is in use by another process.");
                    MessageBox.Show("The file you dragged is corrupted or the file is in use by another process.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    // Channels in MIDI are 0-based, but our UI is 1-based
                    _enabledChannels.Add(i - 1);
                }
            }

            Debug.WriteLine($"Enabled channels: {string.Join(", ", _enabledChannels)}");
        }
        private Dictionary<int, int> _noteChannels = new Dictionary<int, int>();
        private List<(long time, int tempo)> _tempoEvents;
        private int _ticksPerQuarterNote;
        private async Task LoadMIDI(string filename)
        {
            try
            {
                panelLoading.Visible = true;
                labelStatus.Text = "The MIDI file is being loaded...";
                progressBar1.Value = 0;
                progressBar1.Maximum = 100;
                progressBar1.Visible = true;

                _currentFileName = filename;
                Stop();
                _noteChannels.Clear();

                // Offload heavy processing to a background thread
                await Task.Run(() =>
                {
                    var midiFile = new MidiFile(filename, false);
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
                    UpdateProgressBar(30, "MIDI events are being collected...");
                    var allEvents = new List<(long Time, int NoteNumber, bool IsNoteOn, int Channel)>();
                    int totalTracks = midiFile.Events.Tracks;
                    int processedTracks = 0;

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
                            }
                        }

                        processedTracks++;
                        int percent = 30 + (int)(20.0 * processedTracks / totalTracks); // Between %30 and %50
                        UpdateProgressBar(percent, $"Events are being collected... ({processedTracks}/{totalTracks})");
                    }

                    // Sort events by time
                    UpdateProgressBar(55, "Events are being sorted...");
                    allEvents = allEvents.OrderBy(e => e.Time).ToList();

                    // Take distinct time points
                    var timePoints = allEvents.Select(e => e.Time).Distinct().OrderBy(t => t).ToList();

                    // Create frames
                    UpdateProgressBar(60, "Frames are being created...");
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
                            UpdateProgressBar(percent, $"Frames are being created... ({i + 1}/{totalTimePoints})");
                        }
                    }
                });

                _currentFrameIndex = 0;
                _isPlaying = false;

                UpdateEnabledChannels();

                progressBar1.Value = 100;
                labelStatus.Text = "MIDI file loaded successfully.";
                await Task.Delay(300);
                progressBar1.Visible = false;
            }
            catch (Exception ex)
            {
                labelStatus.Text = "An error occurred while loading the MIDI file.";
                progressBar1.Visible = false;
                progressBar1.Value = 0;
                MessageBox.Show($"Error loading MIDI file: {ex.Message}");
                _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
                Debug.WriteLine($"Error loading MIDI file: {ex.Message}");
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
            Debug.WriteLine($"Play called. IsPlaying: {_isPlaying}, Frames count: {_frames?.Count ?? 0}");

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

                Debug.WriteLine("Timer-based playback started successfully");
                button_play.Enabled = false;
                button_stop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting playback: {ex.Message}");
                _isPlaying = false;
            }
        }
        public async void Stop()
        {
            Debug.WriteLine($"Stop called. IsPlaying: {_isPlaying}");

            if (!_isPlaying)
                return;

            _isStopping = true;
            try
            {
                playbackTimer.Stop();
                _playbackStopwatch?.Stop();
                _isPlaying = false;
                _isAlternatingPlayback = false;

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
                holded_note_label.Text = "Notes which are currently being held on: (0)";
                label_more_notes.Visible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping playback: {ex.Message}");
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

        public async Task SetPosition(double positionPercent)
        {
            if (_frames == null || _frames.Count == 0)
                return;

            bool wasPlaying = _isPlaying;

            // Stop current playback if any
            if (_isPlaying)
            {
                Stop();

                // Wait for the previous task to truly complete
                if (_playbackTask != null && !_playbackTask.IsCompleted)
                {
                    try
                    {
                        // Time out after 1 second to prevent hanging
                        await Task.WhenAny(_playbackTask, Task.Delay(1000));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error waiting for playback task: {ex.Message}");
                    }
                }
            }

            // Calculate new position
            _currentFrameIndex = (int)(positionPercent * _frames.Count / 100.0);

            // Clamp the index to valid range
            _currentFrameIndex = Math.Max(0, Math.Min(_currentFrameIndex, _frames.Count - 1));

            Debug.WriteLine($"Position set to {positionPercent}% (frame {_currentFrameIndex} of {_frames.Count})");

            // Resume playing if it was playing before
            if (wasPlaying)
            {
                Play();
            }
        }

        // Update note labels with synchronization
        private void UpdateNoteLabelsSync(HashSet<int> activeNotes)
        {
            // Tüm label'ları kesin olarak sıfırla
            for (int i = 0; i < _noteLabels.Length; i++)
            {
                _noteLabels[i].Visible = false;
                _noteLabels[i].BackColor = _originalLabelColors[_noteLabels[i]];
                _noteLabels[i].Text = "";
            }

            // Aktif notaları MIDI numarasına göre sırala ve soldan sağa ata
            var sortedNotes = activeNotes.OrderBy(note => note).ToList();
            for (int i = 0; i < Math.Min(sortedNotes.Count, _noteLabels.Length); i++)
            {
                int noteNumber = sortedNotes[i];
                Label label = _noteLabels[i];
                string noteName = MidiNoteToName(noteNumber);

                label.Visible = true;
                label.Text = noteName;
                label.BackColor = _highlightColor;
            }

            // Fazla nota varsa
            if (sortedNotes.Count > _noteLabels.Length)
            {
                label_more_notes.Visible = true;
                label_more_notes.Text = $"({sortedNotes.Count - _noteLabels.Length} More)";
            }
            else
            {
                label_more_notes.Visible = false;
            }
        }
        private void checkBox_channel_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledChannels();

            // If it is playing, update the position immediately
            if (_isPlaying)
            {
                double currentPositionPercent = (double)_currentFrameIndex / _frames.Count * 100;
                SetPosition(currentPositionPercent);
            }
            Debug.WriteLine("Channel checkboxes changed");
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

                                            HighlightNoteLabel(noteNumbers[noteIndex]);
                                            NotePlayer.play_note(frequencies[noteIndex], alternatingTime);
                                            UnHighlightNoteLabel(noteNumbers[noteIndex]);

                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            // Calculate the time to sleep to maintain the rhythm
                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                NonBlockingSleep.Sleep((int)timeToNextNote);
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

                                            HighlightNoteLabel(noteNumbers[i]);
                                            NotePlayer.play_note(frequencies[i], alternatingTime);
                                            UnHighlightNoteLabel(noteNumbers[i]);

                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                NonBlockingSleep.Sleep((int)timeToNextNote);
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

                                            HighlightNoteLabel(noteNumbers[noteIndex]);
                                            NotePlayer.play_note(frequencies[noteIndex], interval);
                                            UnHighlightNoteLabel(noteNumbers[noteIndex]);

                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            // Calculate the time to sleep to maintain the rhythm
                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                NonBlockingSleep.Sleep((int)timeToNextNote);
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

                                            HighlightNoteLabel(noteNumbers[i]);
                                            NotePlayer.play_note(frequencies[i], interval);
                                            UnHighlightNoteLabel(noteNumbers[i]);

                                            noteStopwatch.Stop();
                                            long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                            double timeToNextNote = timePerNote - noteElapsed;
                                            if (timeToNextNote > 0)
                                            {
                                                NonBlockingSleep.Sleep((int)timeToNextNote);
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

                                        HighlightNoteLabel(noteNumbers[noteIndex]);
                                        NotePlayer.play_note(frequencies[noteIndex], alternatingTime);
                                        UnHighlightNoteLabel(noteNumbers[noteIndex]);

                                        noteStopwatch.Stop();
                                        long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                        // Calculate the time to sleep to maintain the rhythm
                                        double timeToNextNote = timePerNote - noteElapsed;
                                        if (timeToNextNote > 0)
                                        {
                                            NonBlockingSleep.Sleep((int)timeToNextNote);
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

                                        HighlightNoteLabel(noteNumbers[noteIndex]);
                                        NotePlayer.play_note(frequencies[noteIndex], interval);
                                        UnHighlightNoteLabel(noteNumbers[noteIndex]);

                                        noteStopwatch.Stop();
                                        long noteElapsed = noteStopwatch.ElapsedMilliseconds;

                                        // Calculate the time to sleep to maintain the rhythm
                                        double timeToNextNote = timePerNote - noteElapsed;
                                        if (timeToNextNote > 0)
                                        {
                                            NonBlockingSleep.Sleep((int)timeToNextNote);
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

        private void HighlightNoteLabel(int noteNumber)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => HighlightNoteLabel(noteNumber)));
                return;
            }
            string noteName = MidiNoteToName(noteNumber);
            foreach (Label label in _noteLabels)
            {
                if (label.Text.Contains(noteName))
                {
                    label.BackColor = _highlightColor;
                    break;
                }
            }
        }

        private void UnHighlightNoteLabel(int noteNumber)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UnHighlightNoteLabel(noteNumber)));
                return;
            }
            string noteName = MidiNoteToName(noteNumber);
            foreach (Label label in _noteLabels)
            {
                if (label.Text.Contains(noteName))
                {
                    label.BackColor = _originalLabelColors[label];
                    break;
                }
            }
        }
        private int NoteToFrequency(int noteNumber)
        {
            // MIDI note number to frequency conversion
            return (int)(880.0 * Math.Pow(2.0, (noteNumber - 69) / 12.0));
        }
        private void UpdateTimeAndPercentPosition(int frameIndex)
        {
            if (label_percentage.InvokeRequired)
            {
                label_percentage.BeginInvoke(new Action(() =>
                {
                    label_percentage.Text = ((double)frameIndex / _frames.Count * 100).ToString("0.00") + "%";
                    label_position.Text = $"Position: {UpdateTimeLabel(frameIndex)}";
                }));
            }
            else
            {
                label_percentage.Text = ((double)frameIndex / _frames.Count * 100).ToString("0.00") + "%";
                label_position.Text = $"Position: {UpdateTimeLabel(frameIndex)}";
            }
        }

        private string UpdateTimeLabel(int frameIndex)
        {
            if (_frames == null || _frames.Count == 0)
                return "00:00.00";

            // Calculate current duration
            double currentTimeMs = TicksToMilliseconds(_frames[frameIndex].Time);

            // Convert time to minute:seconds.(miliseconds/10) format
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(currentTimeMs);
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;
            int milliseconds = timeSpan.Milliseconds / 10; // miliseconds/10

            // Update label_percentage
            return $"{minutes:D2}:{seconds:D2}.{milliseconds:D2}";
        }

        private void Rewind()
        {
            trackBar1.Value = 0;
            int positionPercent = trackBar1.Value / 10;
            SetPosition(positionPercent);
            UpdateTimeAndPercentPosition(positionPercent);
            Debug.WriteLine("Rewind completed");
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
            if (_isUpdatingLabels)
                return;
            _isUpdatingLabels = true;
            try
            {
                // Sort notes once, outside the UI update action
                var sortedNotes = activeNotes.OrderBy(note => note).ToList();
                Action updateAction = () =>
                {
                    // Reset all labels
                    foreach (var label in _noteLabels)
                    {
                        label.Visible = false;
                        label.BackColor = _originalLabelColors[label];
                    }

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
                        label_more_notes.Text = $"({sortedNotes.Count - _noteLabels.Length} More)";
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
                Debug.WriteLine($"Error updating note labels: {ex.Message}");
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing the form: {ex.Message}");
            }
        }

        private void disable_alternating_notes_panel(object sender, EventArgs e)
        {
            if (checkBox_play_each_note.Checked == true || checkBox_make_each_cycle_last_30ms.Checked == true)
            {
                panel1.Enabled = false;
                Debug.WriteLine("Play each note or make each cycle last 30 ms checkbox is checked. Disabling the panel.");
            }
            else
            {
                panel1.Enabled = true;
                Debug.WriteLine("Play each note or make each cycle last 30 ms checkbox is not checked. Enabling the panel.");
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int positionPercent = trackBar1.Value / 10;
            SetPosition(positionPercent);
            UpdateTimeAndPercentPosition(positionPercent);
        }

        private Label[] _noteLabels;
        private Dictionary<int, int> _noteToLabelMap;
        private Dictionary<Label, Color> _originalLabelColors = new Dictionary<Label, Color>();

        // Initialize in your constructor or Form_Load
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
            // This mapping assumes MIDI notes 60-91 (C1 to B10) correspond to labels 1-32
            _noteToLabelMap = new Dictionary<int, int>();
            // Maps MIDI notes 60-91 (C1 to B10) to labels 1-32
            for (int i = 24; i <= 128; i++)
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
                Debug.WriteLine("Don't update grid checkbox is checked. Hiding all labels.");
            }
            else
            {
                Debug.WriteLine("Don't update grid checkbox is not checked. Showing all labels.");
            }
        }
        private int _highlightDuration;

        private void checkBox_loop_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_loop.Checked == true)
            {
                Debug.WriteLine("Loop is enabled.");
            }
            else
            {
                Debug.WriteLine("Loop is disabled.");
            }
        }

        private void numericUpDown_alternating_note_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Alternating note duration changed to {numericUpDown_alternating_note.Value} ms.");
        }

        private void checkBox_play_each_note_CheckedChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("Play each note checkbox changed.");
        }

        private double TicksToMilliseconds(long ticks)
        {
            double milliseconds = 0;
            long lastTicks = 0;
            int lastTempo = _tempoEvents[0].tempo;

            foreach (var tempoEvent in _tempoEvents)
            {
                if (tempoEvent.time >= ticks)
                {
                    break;
                }
                milliseconds += (double)(tempoEvent.time - lastTicks) * lastTempo / (_ticksPerQuarterNote * 1000.0);
                lastTicks = tempoEvent.time;
                lastTempo = tempoEvent.tempo;
            }

            milliseconds += (double)(ticks - lastTicks) * lastTempo / (_ticksPerQuarterNote * 1000.0);
            return milliseconds;
        }
        private async void playbackTimer_Tick(object sender, EventArgs e)
        {
            if (_isStopping || !_isPlaying || _frames == null)
                return;

            if (_currentFrameIndex >= _frames.Count)
            {
                HandlePlaybackComplete();
                return;
            }

            // Prevent re-entrancy. If the last task is still running, skip this tick.
            if (!_playbackTask.IsCompleted)
            {
                return;
            }

            // Start a new task to process all due frames.
            _playbackTask = Task.Run(async () =>
            {
                try
                {
                    var token = _cancellationTokenSource.Token;
                    token.ThrowIfCancellationRequested();
                    var elapsedMs = _playbackStopwatch.ElapsedMilliseconds;
                    var songTimeMs = _playbackStartOffsetMs + elapsedMs;

                    // Loop through all frames that should have been played by now
                    while (_currentFrameIndex < _frames.Count)
                    {
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
                    Debug.WriteLine("Playback task was canceled.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in playback task: {ex.Message}");
                    if (IsHandleCreated)
                    {
                        this.BeginInvoke((Action)Stop);
                    }
                }
            }, _cancellationTokenSource.Token);
        }
        private async Task ProcessCurrentFrame()
        {
            var token = _cancellationTokenSource.Token;

            // Example of cancellation check
            token.ThrowIfCancellationRequested();

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
                durationMs = 500; // Default for the last frame
            }

            int durationMsInt = Math.Max(1, (int)Math.Round(durationMs)); // Minimum 1ms

            // Update UI
            if (!checkBox_dont_update_grid.Checked)
            {
                UpdateAllUISync(_currentFrameIndex, filteredNotes);
            }

            // Play notes
            if (filteredNotes.Count > 0)
            {
                var frequencies = filteredNotes.Select(note => NoteToFrequency(note)).ToArray();
                if (TemporarySettings.MIDIDevices.useMIDIoutput)
                {
                    foreach (int frequency in frequencies)
                    {
                        MIDIIOUtils.PlayMidiNoteAsync(MIDIIOUtils.FrequencyToMidiNote(frequency), durationMsInt);
                    }
                }
                if (frequencies.Length == 1)
                {
                    var noteNumber = filteredNotes.First();
                    HighlightNoteLabel(noteNumber);
                    await Task.Run(() => NotePlayer.play_note(frequencies[0], durationMsInt), token);
                    UnHighlightNoteLabel(noteNumber);
                }
                else
                {
                    await Task.Run(() => PlayMultipleNotes(frequencies, durationMsInt), token);
                }
            }
            else
            {
                // Silence playback
                await WaitPrecise(durationMsInt);
            }
        }
        private async Task WaitPrecise(int milliseconds)
        {
            await Task.Run(() => NonBlockingSleep.Sleep(milliseconds));
        }
        private void UpdateAllUISync(int frameIndex, HashSet<int> filteredNotes)
        {
            if (this.InvokeRequired)
            {
                // Use BeginInvoke for fire-and-forget to avoid blocking the playback thread.
                this.BeginInvoke(new Action(() => UpdateAllUISync(frameIndex, filteredNotes)));
                return;
            }

            // Update trackbar position
            if (_frames.Count > 0)
            {
                int trackbarValue = (int)(1000.0 * frameIndex / _frames.Count);
                if (trackbarValue <= trackBar1.Maximum)
                {
                    trackBar1.Value = trackbarValue;
                }
            }

            // Update percentage label
            label_percentage.Text = ((double)frameIndex / _frames.Count * 100).ToString("0.00") + "%";

            // Update position label
            label_position.Text = $"Position: {UpdateTimeLabel(frameIndex)}";

            // Update note labels
            UpdateNoteLabelsSync(filteredNotes);

            // Update holded note label
            holded_note_label.Text = $"Notes which are currently being held on: ({filteredNotes.Count})";
        }

        // Playback complete handler
        private void HandlePlaybackComplete()
        {
            if (!_isPlaying) return; // Don't process if not playing

            playbackTimer.Stop();

            if (checkBox_loop.Checked)
            {
                Debug.WriteLine("Playback loop enabled. Rewinding.");
                // Rewind 
                Rewind();
            }
            else
            {
                Debug.WriteLine("Playback finished.");
                // Stop playback
                Stop();
                Rewind();
            }

            Debug.WriteLine("Timer-based playback completed");
        }
    }
}