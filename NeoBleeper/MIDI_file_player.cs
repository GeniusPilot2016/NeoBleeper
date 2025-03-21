﻿using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class MIDI_file_player : Form
    {
        bool is_playing = false;
        PrivateFontCollection fonts = new PrivateFontCollection();
        private List<int> _displayOrder = new List<int>();
        public MIDI_file_player(string filename)
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                }
            }
            label_note1.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note2.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note3.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note4.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note5.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note6.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note7.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note8.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note9.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note10.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note11.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note12.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note13.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note14.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note15.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note16.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note17.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note18.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note19.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note20.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note21.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note22.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note23.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note24.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note25.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note26.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note27.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note28.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note29.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note30.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note31.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_note32.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
            label_alternating_note.Font = new Font(fonts.Families[0], 9);
            numericUpDown_alternating_note.Font = new Font(fonts.Families[0], 9);
            label_ms.Font = new Font(fonts.Families[0], 9);
            label_position.Font = new Font(fonts.Families[0], 9);
            label_percentage.Font = new Font(fonts.Families[0], 9);
            label_percentage.RightToLeft = RightToLeft.Yes;
            set_theme();
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

        private void button4_Click(object sender, EventArgs e)
        {
            Stop();
            openFileDialog.Filter = "MIDI Files|*.mid";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (IsMidiFile(openFileDialog.FileName))
                {
                    textBox1.Text = openFileDialog.FileName;
                    LoadMIDI(openFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("This file is not a valid MIDI file or the file is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void MIDI_file_player_DragDrop(object sender, DragEventArgs e)
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
                        LoadMIDI(fileName);
                    }
                    else
                    {
                        MessageBox.Show("The file you dragged is not supported by NeoBleeper MIDI player or is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
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
        public void LoadMIDI(string filename)
        {
            try
            {
                _currentFileName = filename;
                // Stop any current playback
                Stop();

                // Clear previous channel information
                _noteChannels.Clear();

                // Load the MIDI file using NAudio
                var midiFile = new MidiFile(filename, false);

                // Extract tempo information
                int microsecondsPerQuarterNote = 500000; // Default 120 BPM
                foreach (var midiEvent in midiFile.Events[0])
                {
                    if (midiEvent is TempoEvent tempoEvent)
                    {
                        microsecondsPerQuarterNote = tempoEvent.MicrosecondsPerQuarterNote;
                        break;
                    }
                }

                // Calculate timing conversion
                _ticksToMs = microsecondsPerQuarterNote / (midiFile.DeltaTicksPerQuarterNote * 1000.0);

                // Build a list of "frames" - snapshots of which notes are active at each time point
                _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
                HashSet<int> currentlyActiveNotes = new HashSet<int>();

                // Create a timeline of all note events sorted by time
                var allEvents = new List<(long Time, int NoteNumber, bool IsNoteOn, int Channel)>();
                foreach (var track in midiFile.Events)
                {
                    foreach (var midiEvent in track)
                    {
                        if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                        {
                            var noteEvent = (NoteOnEvent)midiEvent;
                            allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, noteEvent.Velocity > 0, noteEvent.Channel));

                            // Store the channel for this note
                            _noteChannels[noteEvent.NoteNumber] = noteEvent.Channel;
                        }
                        else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                        {
                            var noteEvent = (NoteEvent)midiEvent;
                            allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, false, noteEvent.Channel));
                        }
                    }
                }

                // Sort all events by time
                allEvents = allEvents.OrderBy(e => e.Time).ToList();

                // Get unique time points
                var timePoints = allEvents.Select(e => e.Time).Distinct().OrderBy(t => t).ToList();

                // Build frames
                foreach (var time in timePoints)
                {
                    // Process all events at this time point
                    foreach (var evt in allEvents.Where(e => e.Time == time))
                    {
                        if (evt.IsNoteOn)
                        {
                            currentlyActiveNotes.Add(evt.NoteNumber);
                        }
                        else
                        {
                            currentlyActiveNotes.Remove(evt.NoteNumber);
                        }
                    }

                    // Create a new frame with a copy of currently active notes
                    _frames.Add((time, new HashSet<int>(currentlyActiveNotes)));
                }

                // Reset the current frame index
                _currentFrameIndex = 0;
                _isPlaying = false;

                // Debug info
                Debug.WriteLine($"Loaded MIDI file: {filename}");
                Debug.WriteLine($"Total frames: {_frames.Count}");
                Debug.WriteLine($"Ticks to Ms: {_ticksToMs}");

                // Initialize the enabled channels based on checkboxes
                UpdateEnabledChannels();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading MIDI file: {ex.Message}");
                _frames = new List<(long Time, HashSet<int> ActiveNotes)>();
            }
        }
        public void Play()
        {
            // Debug check
            Debug.WriteLine($"Play called. IsPlaying: {_isPlaying}, Frames count: {_frames?.Count ?? 0}");

            if (_isPlaying || _frames == null || _frames.Count == 0)
                return;

            try
            {
                // Create a new cancellation token source
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;

                _isPlaying = true;

                // Start playing in a separate task and store the task
                _playbackTask = Task.Run(() => PlayFromPosition(_currentFrameIndex, token), token);

                Debug.WriteLine("Playback started successfully");
                button_play.Enabled = false;
                button_stop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting playback: {ex.Message}");
                _isPlaying = false;
            }
        }
        public void Stop()
        {
            Debug.WriteLine($"Stop called. IsPlaying: {_isPlaying}");

            if (!_isPlaying)
                return;

            try
            {
                // Cancel the playback
                _cancellationTokenSource?.Cancel();

                // Wait a moment for cancellation to take effect
                Thread.Sleep(50);

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _isPlaying = false;

                // Reset label safely using BeginInvoke
                if (holded_note_label.InvokeRequired)
                {
                    holded_note_label.BeginInvoke(new Action(() =>
                    {
                        holded_note_label.Text = "Notes which are currently being held on: (0)";
                    }));
                }
                else
                {
                    holded_note_label.Text = "Notes which are currently being held on: (0)";
                }
                button_play.Enabled = true;
                button_stop.Enabled = false;
                Debug.WriteLine("Playback stopped successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping playback: {ex.Message}");
            }
        }
        // Add this field to track the current playback task
        private Task _playbackTask = null;

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
        private async Task PlayFromPosition(int startIndex, CancellationToken cancellationToken)
        {
            Debug.WriteLine($"Starting playback from frame {startIndex} of {_frames.Count}");
            Stopwatch stopwatch = new Stopwatch();

            // Keep track of which notes were active in the previous frame
            HashSet<int> previousActiveNotes = new HashSet<int>();

            try
            {
                // Start playing from the specified index
                for (int i = startIndex; i < _frames.Count; i++)
                {
                    // Check for cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("Playback cancelled");
                        // Hide all labels when playback is cancelled
                        UpdateNoteLabels(new HashSet<int>());
                        return;
                    }

                    // Update the current position
                    _currentFrameIndex = i;
                    UpdateTrackBarPosition(i);
                    UpdateTimeAndPercentPosition(i);
                    var currentFrame = _frames[i];

                    // Filter notes by channel
                    HashSet<int> filteredNotes = new HashSet<int>();
                    foreach (var note in currentFrame.ActiveNotes)
                    {
                        // Only include notes from enabled channels
                        if (_noteChannels.TryGetValue(note, out int channel) && _enabledChannels.Contains(channel))
                        {
                            filteredNotes.Add(note);
                        }
                    }

                    // Update note labels based on filtered active notes
                    if (checkBox_dont_update_grid.Checked == false)
                    {
                        UpdateNoteLabels(filteredNotes);
                    }

                    // Remember current active notes for next iteration
                    previousActiveNotes = new HashSet<int>(filteredNotes);

                    int notesCount = filteredNotes.Count;

                    // Update label on UI thread
                    try
                    {
                        if (holded_note_label.InvokeRequired)
                        {
                            holded_note_label.BeginInvoke(new Action(() =>
                            {
                                holded_note_label.Text = $"Notes which are currently being held on: ({notesCount})";
                            }));
                        }
                        else
                        {
                            holded_note_label.Text = $"Notes which are currently being held on: ({notesCount})";
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error updating label: {ex.Message}");
                    }

                    // Calculate duration to next frame
                    int durationMs;
                    if (i < _frames.Count - 1)
                    {
                        durationMs = (int)((_frames[i + 1].Time - currentFrame.Time) * _ticksToMs);
                    }
                    else
                    {
                        durationMs = 500; // Default duration for last frame
                    }

                    // Ensure minimum duration
                    durationMs = Math.Max(10, durationMs);

                    // Debug every 100 frames
                    if (i % 100 == 0)
                    {
                        Debug.WriteLine($"Playing frame {i}, notes: {notesCount}, duration: {durationMs}ms");
                    }

                    stopwatch.Restart();
                    // Play active notes or silence
                    if (notesCount > 0)
                    {
                        var frequencies = filteredNotes.Select(note => NoteToFrequency(note)).ToArray();

                        if (frequencies.Length == 1)
                        {
                            // Single note - play directly
                            NotePlayer.play_note(frequencies[0], durationMs);
                        }
                        else
                        {
                            // Multiple notes
                            PlayMultipleNotes(frequencies, durationMs);
                        }
                    }
                    else
                    {
                        // Silence - just wait
                        await Task.Delay(durationMs, cancellationToken);
                    }

                    stopwatch.Stop();
                    int elapsedTime = (int)stopwatch.ElapsedMilliseconds;
                    int remainingTime = durationMs - elapsedTime;

                    if (remainingTime > 0)
                    {
                        await Task.Delay(remainingTime, cancellationToken);
                    }
                }

                // Finished playing
                if (checkBox_loop.Checked == true)
                {
                    UpdateTimeAndPercentPosition(100);
                    Rewind();
                }
                else
                {
                    Stop();
                    trackBar1.Value = 0;
                    SetPosition(trackBar1.Value / 10);
                    UpdateTimeAndPercentPosition(trackBar1.Value / 10);
                }

                // Hide all note labels when playback completes
                UpdateNoteLabels(new HashSet<int>());

                Debug.WriteLine("Playback completed successfully");

                // Reset label
                if (holded_note_label.InvokeRequired)
                {
                    holded_note_label.BeginInvoke(new Action(() =>
                    {
                        holded_note_label.Text = "Notes which are currently being held on: (0)";
                    }));
                }
                else
                {
                    holded_note_label.Text = "Notes which are currently being held on: (0)";
                }
            }
            catch (TaskCanceledException)
            {
                // Hide all labels when playback is cancelled
                UpdateNoteLabels(new HashSet<int>());
                // Task was canceled, do nothing
                Debug.WriteLine("Task was canceled");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Debug.WriteLine($"Error during playback: {ex.Message}");
                MessageBox.Show($"Error during playback: {ex.Message}");

                // Hide all labels in case of error
                UpdateNoteLabels(new HashSet<int>());
            }
            finally
            {
                _isPlaying = false;
            }
        }
        private void checkBox_channel_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledChannels();

            // If we're currently playing, restart from current position to apply the channel change
            if (_isPlaying)
            {
                double currentPositionPercent = (double)_currentFrameIndex / _frames.Count * 100;
                SetPosition(currentPositionPercent);
            }
        }

        // Play multiple notes alternating
        private void PlayMultipleNotes(int[] frequencies, int duration)
        {
            if (!(checkBox_play_each_note.Checked == true || checkBox_make_each_cycle_last_30ms.Checked == true))
            {
                if (checkBox_make_each_cycle_last_30ms.Checked == true)
                {
                    if (checkBox_play_each_note.Checked == true)
                    {
                        int interval = 30; // Switch between 30 ms
                        int minAlternatingTime = 3; // Minimum alternate time 3 ms
                        int maxAlternatingTime = 15; // Maximum alternate time 3 ms
                        int steps = Convert.ToInt32(Math.Truncate((double)duration / (double)interval));
                        Random random = new Random();
                        Stopwatch stopwatch = new Stopwatch();
                        if (frequencies.Length >= steps)
                        {
                            int i = 0;
                            do
                            {
                                foreach (var frequency in frequencies)
                                {
                                    int alternatingTime = random.Next(minAlternatingTime, maxAlternatingTime + 1); // Rastgele alternatif süre

                                    stopwatch.Restart();
                                    NotePlayer.play_note(frequency, alternatingTime);
                                    stopwatch.Stop();

                                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                                    if (elapsedMilliseconds < alternatingTime)
                                    {
                                        Thread.Sleep(alternatingTime - (int)elapsedMilliseconds);
                                    }

                                    int remainingTime = interval - alternatingTime;

                                    if (remainingTime > 0)
                                    {
                                        Thread.Sleep(remainingTime);
                                    }

                                    i++;
                                    if (i * interval >= duration)
                                        break;
                                }
                            }
                            while (i < steps);
                        }
                        else
                        {
                            int i = 0;
                            do
                            {
                                foreach (var frequency in frequencies)
                                {
                                    NotePlayer.play_note(frequency, interval);
                                    i++;
                                    if (i * interval >= frequencies.Length * interval)
                                        break;
                                }
                            }
                            while (i < frequencies.Length);
                            Thread.Sleep(duration - (interval * frequencies.Length));
                        }
                    }
                    else
                    {
                        int interval = 30; // Switch between 30 ms
                        int minAlternatingTime = 3; // Minimum alternate time 3 ms
                        int maxAlternatingTime = 15; // Maximum alternate time 3 ms
                        int steps = Convert.ToInt32(Math.Truncate((double)duration / (double)interval));
                        int i = 0;
                        Random random = new Random();
                        Stopwatch stopwatch = new Stopwatch();

                        do
                        {
                            foreach (var frequency in frequencies)
                            {
                                int alternatingTime = random.Next(minAlternatingTime, maxAlternatingTime + 1); // Rastgele alternatif süre

                                stopwatch.Restart();
                                NotePlayer.play_note(frequency, alternatingTime);
                                stopwatch.Stop();

                                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                                if (elapsedMilliseconds < alternatingTime)
                                {
                                    Thread.Sleep(alternatingTime - (int)elapsedMilliseconds);
                                    if (i * interval >= frequencies.Length * interval)
                                        break;
                                }

                                int remainingTime = interval - alternatingTime;

                                if (remainingTime > 0)
                                {
                                    Thread.Sleep(remainingTime);
                                }

                                i++;
                                if (i * interval >= duration)
                                    break;
                            }
                        }
                        while (i < steps);
                    }

                }
                else
                {
                    int interval = Convert.ToInt32(numericUpDown_alternating_note.Value);
                    int steps = Convert.ToInt32(Math.Truncate((double)duration / (double)interval));
                    if (frequencies.Length >= steps)
                    {
                        int i = 0;
                        do
                        {
                            foreach (var frequency in frequencies)
                            {
                                NotePlayer.play_note(frequency, interval);
                                i++;
                                // Check if we've gone past our duration
                                if (i * interval >= duration)
                                    break;
                            }
                        }
                        while (i < steps);
                    }
                    else
                    {
                        int i = 0;
                        do
                        {
                            foreach (var frequency in frequencies)
                            {
                                NotePlayer.play_note(frequency, interval);
                                i++;
                                if (i * interval >= frequencies.Length * interval)
                                    break;
                            }
                        }
                        while (i < frequencies.Length);
                        Thread.Sleep(duration - (interval * frequencies.Length));
                    }
                }

            }
            else
            {
                int interval = Convert.ToInt32(numericUpDown_alternating_note.Value);
                int steps = Convert.ToInt32(Math.Truncate(Math.Truncate((double)duration) / (double)interval));
                int i = 0;
                do
                {
                    foreach (var frequency in frequencies)
                    {
                        NotePlayer.play_note(frequency, interval);
                        i++;

                        // Check if we've gone past our duration
                        if (i * interval >= duration)
                            break;
                    }
                }
                while (i < steps);
            }
        }
        private int NoteToFrequency(int noteNumber)
        {
            // MIDI note number to frequency conversion
            return (int)(880.0 * Math.Pow(2.0, (noteNumber - 69) / 12.0));
        }
        private void UpdateTrackBarPosition(int frameIndex)
        {
            if (trackBar1.InvokeRequired)
            {
                trackBar1.Invoke(new Action(() =>
                {
                    trackBar1.Value = (int)(10 * (double)frameIndex / _frames.Count * 100);
                }));
            }
            else
            {
                trackBar1.Value = (int)(10 * (double)frameIndex / _frames.Count * 100);
            }
        }
        private void UpdateTimeAndPercentPosition(int frameIndex)
        {
            if (label_percentage.InvokeRequired)
            {
                label_percentage.Invoke(new Action(() =>
                {
                    label_percentage.Text = ((double)frameIndex / _frames.Count * 100).ToString("0.00") + "%";
                }));
            }
            else
            {
                label_percentage.Text = ((double)frameIndex / _frames.Count * 100).ToString("0.00") + "%";
            }
            CalculatePosition(frameIndex);
        }
        private void CalculatePosition(int frameIndex)
        {
            if (_frames == null || _frames.Count == 0)
                return;

            if (label_percentage.InvokeRequired)
            {
                label_percentage.Invoke(new Action(() =>
                {
                    label_position.Text = $"Position: {UpdateTimeLabel(frameIndex)}";
                }));
            }
            else
            {
                label_position.Text = $"Position: {UpdateTimeLabel(frameIndex)}";
            }
        }

        private string UpdateTimeLabel(int frameIndex)
        {
            if (_frames == null || _frames.Count == 0)
                return "00:00.00";

            // Calculate total duration
            double totalTimeMs = _frames[_frames.Count - 1].Time * _ticksToMs;

            // Calculate current duration
            double currentTimeMs = _frames[frameIndex].Time * _ticksToMs;

            // Convert time to minute:second.(miliseconds/10) format
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
        }
        private string MidiNoteToName(int noteNumber)
        {
            // Define note names (C, C#, D, etc.)
            string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

            // Calculate the octave (MIDI note 60 is middle C, which is C4)
            int octave = (noteNumber / 12) - 1;

            // Calculate the note name index (0-11) within the octave
            int noteIndex = noteNumber % 12;

            // Format the note name with its octave
            return $"{noteNames[noteIndex]}{octave+1}";
        }
        private Dictionary<int, Color> _activeNoteColors = new Dictionary<int, Color>();
        private Color _highlightColor = Settings1.Default.note_indicator_color; // You can choose any color
        private void UpdateNoteLabels(HashSet<int> activeNotes)
        {
            try
            {
                Action updateAction = () =>
                {
                    // 1. Reset all labels to their original color and hide them
                    foreach (var label in _noteLabels)
                    {
                        label.Visible = false;
                        label.BackColor = _originalLabelColors[label];
                    }

                    // 2. Process active notes
                    int labelIndex = 0;
                    foreach (int noteNumber in activeNotes)
                    {
                        // a) Add to display order if not already present
                        if (!_displayOrder.Contains(noteNumber))
                        {
                            _displayOrder.Add(noteNumber);
                        }

                        // b) Update label if in display order
                        int displayIndex = _displayOrder.IndexOf(noteNumber);
                        if (displayIndex >= 0 && displayIndex < _noteLabels.Length)
                        {
                            Label label = _noteLabels[labelIndex];
                            label.Visible = true;
                            label.Text = MidiNoteToName(noteNumber);

                            // Color Highlighting Logic
                            label.BackColor = _highlightColor; // Set highlight color

                            // We will revert the color later, so no toggling is needed here

                            labelIndex++; // Move to the next label
                        }
                    }

                    // 3. (Reverted) Colors for inactive notes are handled in the next step

                    // 4. Update the count display
                    holded_note_label.Text = $"Notes which are currently being held on: [{activeNotes.Count}]";
                };

                // Use BeginInvoke to update UI from background thread
                if (_noteLabels.Length > 0 && _noteLabels[0].InvokeRequired)
                    _noteLabels[0].BeginInvoke(updateAction);
                else
                    updateAction();

                // Revert highlight color after a short delay
                Task.Run(async () =>
                {
                    await Task.Delay(50); // Adjust the delay as needed (milliseconds)

                    Action revertColorAction = () =>
                    {
                        foreach (int noteNumber in activeNotes)
                        {
                            int displayIndex = _displayOrder.IndexOf(noteNumber);
                            if (displayIndex >= 0 && displayIndex < _noteLabels.Length)
                            {
                                Label label = _noteLabels[displayIndex];
                                if (label.Visible) // Only revert visible labels
                                {
                                    label.BackColor = _originalLabelColors[label];
                                }
                            }
                        }
                    };

                    // Use BeginInvoke to update UI from background thread
                    if (_noteLabels.Length > 0 && _noteLabels[0].InvokeRequired)
                        _noteLabels[0].BeginInvoke(revertColorAction);
                    else
                        revertColorAction();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating note labels: {ex.Message}");
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
            Stop();
        }

        private void disable_alternating_notes_panel(object sender, EventArgs e)
        {
            if (checkBox_play_each_note.Checked == true || checkBox_make_each_cycle_last_30ms.Checked == true)
            {
                panel1.Enabled = false;
            }
            else
            {
                panel1.Enabled = true;
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

            // Create a mapping from MIDI note numbers to label indices
            // This assumes you want to map certain MIDI notes to your 32 labels
            _noteToLabelMap = new Dictionary<int, int>();

            // Example mapping (you'll need to adjust this based on your needs)
            // Maps MIDI notes 60-91 (middle C to G6) to labels 1-32
            for (int i = 60; i < 92; i++)
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
            }
        }
    }
}
