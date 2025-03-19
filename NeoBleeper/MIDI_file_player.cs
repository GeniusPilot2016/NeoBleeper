using NAudio.Midi;
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
            set_theme();
            label_note1.BackColor = Settings1.Default.note_indicator_color;
            label_note2.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note3.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note4.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note5.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note6.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note7.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note8.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note9.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note10.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note11.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note12.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note13.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note14.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note15.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note16.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note17.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note18.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note19.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note20.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note21.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note22.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note23.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note24.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note25.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note26.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note27.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note28.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note29.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note30.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note31.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note32.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note1.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note2.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note3.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note4.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note5.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note6.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note7.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note8.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note9.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note10.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note11.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note12.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note13.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note14.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note15.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note16.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note17.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note18.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note19.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note20.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note21.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note22.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note23.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note24.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note25.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note26.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note27.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note28.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note29.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note30.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note31.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note32.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
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

        public void LoadMIDI(string filename)
        {
            try
            {
                _currentFileName = filename;
                // Stop any current playback
                Stop();

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
                var allEvents = new List<(long Time, int NoteNumber, bool IsNoteOn)>();
                foreach (var track in midiFile.Events)
                {
                    foreach (var midiEvent in track)
                    {
                        if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                        {
                            var noteEvent = (NoteOnEvent)midiEvent;
                            allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, noteEvent.Velocity > 0));
                        }
                        else if (midiEvent.CommandCode == MidiCommandCode.NoteOff)
                        {
                            var noteEvent = (NoteEvent)midiEvent;
                            allEvents.Add((noteEvent.AbsoluteTime, noteEvent.NoteNumber, false));
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
                    holded_note_label.BeginInvoke(new Action(() => {
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

            try
            {
                // Start playing from the specified index
                for (int i = startIndex; i < _frames.Count; i++)
                {
                    // Check for cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("Playback cancelled");
                        return;
                    }

                    // Update the current position
                    _currentFrameIndex = i;
                    UpdateTrackBarPosition(i);
                    var currentFrame = _frames[i];
                    int notesCount = currentFrame.ActiveNotes.Count;

                    // Update label on UI thread
                    try
                    {
                        if (holded_note_label.InvokeRequired)
                        {
                            holded_note_label.BeginInvoke(new Action(() => {
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

                    // Play active notes or silence
                    if (notesCount > 0)
                    {
                        var frequencies = currentFrame.ActiveNotes.Select(note => NoteToFrequency(note)).ToArray();

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
                }

                // Finished playing
                if (checkBox_loop.Checked == true)
                {
                    Rewind();
                }
                else
                {
                    Stop();
                    trackBar1.Value = 0;
                    SetPosition(trackBar1.Value/10);
                }
                Debug.WriteLine("Playback completed successfully");

                // Reset label
                if (holded_note_label.InvokeRequired)
                {
                    holded_note_label.BeginInvoke(new Action(() => {
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
                // Task was canceled, do nothing
                Debug.WriteLine("Task was canceled");
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Debug.WriteLine($"Error during playback: {ex.Message}");
                MessageBox.Show($"Error during playback: {ex.Message}");
            }
            finally
            {
                _isPlaying = false;
            }
        }

        // Original PlayMultipleNotes method
        private void PlayMultipleNotes(int[] frequencies, int duration)
        {
            if (!(checkBox_play_each_note.Checked == true || checkBox_make_each_cycle_last_30ms.Checked == true))
            {
                int interval = 30; // 30 ms aralıklarla frekans değiştir
                int steps = duration / interval;
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
                while (i * interval < duration);
            }
            else
            {
                int interval = Convert.ToInt32(numericUpDown_alternating_note.Value);
                int steps = duration / interval;
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
                while (i * interval < duration);
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
        private void Rewind()
        {
            trackBar1.Value = 0;
            int positionPercent = trackBar1.Value / 10;
            SetPosition(positionPercent);
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
            int positionPercent = trackBar1.Value/10;
            SetPosition(positionPercent);
        }
    }
}
