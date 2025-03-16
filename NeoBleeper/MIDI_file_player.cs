using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            openFileDialog.Filter = "MIDI Files|*.mid";
            openFileDialog.ShowDialog(this);
            if (openFileDialog.FileName != string.Empty)
            {
                if (IsMidiFile(openFileDialog.FileName))
                {
                    textBox1.Text = openFileDialog.FileName;
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
        // Static variables
        private static bool isPlaying = false;
        private static CancellationTokenSource cancellationTokenSource;
        private static long totalMidiDuration;
        private static Label staticLabelPosition;
        private static Label staticLabelPercentage;

        // Play MIDI file method
        public void play_MIDI_file(string filename)
        {
            staticLabelPosition = label_position;
            staticLabelPercentage = label_percentage;

            // Stop current playback if playing
            StopPlayback();

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            isPlaying = true;

            Task.Run(() =>
            {
                try
                {
                    PlayMidiFileInternal(filename, token);
                }
                catch (OperationCanceledException)
                {
                    UpdatePositionLabels(0, 0); // Reset if playback is canceled
                }
                finally
                {
                    isPlaying = false;
                }
            }, token);
        }

        // Update UI labels
        private static void UpdatePositionLabels(long currentPosition, long totalDuration)
        {
            if (staticLabelPosition != null && staticLabelPercentage != null)
            {
                if (staticLabelPosition.InvokeRequired)
                {
                    staticLabelPosition.Invoke(new Action(() => UpdatePositionLabels(currentPosition, totalDuration)));
                    return;
                }

                TimeSpan position = TimeSpan.FromMilliseconds(currentPosition);
                staticLabelPosition.Text = string.Format("Position: {0:00}:{1:00}.{2:00}",
                    position.Minutes, position.Seconds, Math.Truncate(Convert.ToDouble(position.Milliseconds / 100)));

                double percentage = (totalDuration > 0) ? (currentPosition * 100.0 / totalDuration) : 0;
                staticLabelPercentage.Text = string.Format("%{0:00.00}", percentage);
            }
        }

        // Main playback logic
        private static void PlayMidiFileInternal(string filename, CancellationToken token)
        {
            var midiFile = new MidiFile(filename, false);
            var noteEvents = new List<NoteEvent>();

            // Default tempo (120 BPM)
            double currentTempo = 500000; // Microseconds per quarter note
            double microsecondsPerTick = currentTempo / midiFile.DeltaTicksPerQuarterNote;
            totalMidiDuration = CalculateTotalMidiDuration(midiFile);

            // Get the first tempo event or use default tempo
            var initialTempo = midiFile.Events
                                        .SelectMany(track => track)
                                        .OfType<TempoEvent>()
                                        .FirstOrDefault();
            currentTempo = initialTempo != null
                ? initialTempo.MicrosecondsPerQuarterNote
                : 500000; // Default to 120 BPM
            microsecondsPerTick = currentTempo / midiFile.DeltaTicksPerQuarterNote;

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent is MetaEvent metaEvent && metaEvent.MetaEventType == MetaEventType.SetTempo)
                    {
                        if (metaEvent is TempoEvent tempoEvent)
                        {
                            currentTempo = tempoEvent.MicrosecondsPerQuarterNote;
                            microsecondsPerTick = currentTempo / midiFile.DeltaTicksPerQuarterNote;
                        }
                    }

                    if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        var noteEvent = midiEvent as NoteEvent;
                        if (noteEvent != null)
                        {
                            noteEvents.Add(noteEvent);
                        }
                    }
                }
            }

            var groupedNotes = noteEvents.GroupBy(e => e.AbsoluteTime)
                                         .OrderBy(g => g.Key)
                                         .ToList();

            if (groupedNotes.Count == 0) return;

            long previousTime = 0;
            long currentPosition = 0;
            UpdatePositionLabels(0, totalMidiDuration);

            foreach (var group in groupedNotes)
            {
                token.ThrowIfCancellationRequested();

                long currentTime = group.Key;
                var notes = group.ToList();

                long timeDifference = currentTime - previousTime;
                double waitTime = timeDifference * microsecondsPerTick / 1000.0;

                if (waitTime > 0)
                {
                    currentPosition += timeDifference;
                    UpdatePositionLabels(currentPosition, totalMidiDuration);
                    WaitWithCancellation((int)waitTime, token);
                }

                int duration = notes.Max(n => n.DeltaTime);

                if (notes.Count == 1)
                {
                    var note = notes.First();
                    int frequency = NoteToFrequency(note.NoteNumber);
                    NotePlayer.play_note(frequency, duration);
                }
                else
                {
                    var frequencies = notes.Select(note => NoteToFrequency(note.NoteNumber)).ToArray();
                    PlayMultipleNotes(frequencies, duration, token);
                }

                currentPosition += duration;
                UpdatePositionLabels(currentPosition, totalMidiDuration);
                WaitWithCancellation(duration, token);

                previousTime = currentTime + duration;
            }

            UpdatePositionLabels(0, totalMidiDuration);
        }

        // Calculate total duration of the MIDI file
        private static long CalculateTotalMidiDuration(MidiFile midiFile)
        {
            long maxTime = 0;

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    long endTime = midiEvent.AbsoluteTime;

                    if (midiEvent is NoteEvent noteEvent)
                    {
                        endTime += noteEvent.DeltaTime;
                    }

                    if (endTime > maxTime)
                    {
                        maxTime = endTime;
                    }
                }
            }

            return maxTime;
        }

        // Stop MIDI playback
        public static void StopPlayback()
        {
            if (isPlaying && cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                isPlaying = false;

                if (staticLabelPosition != null && staticLabelPercentage != null)
                {
                    UpdatePositionLabels(0, totalMidiDuration);
                }
            }
        }

        // Wait with cancellation
        private static void WaitWithCancellation(int milliseconds, CancellationToken token)
        {
            try
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds < milliseconds)
                {
                    if (token.IsCancellationRequested)
                        throw new OperationCanceledException();
                }
                stopwatch.Stop();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }

        // Play multiple notes with intervals
        private static void PlayMultipleNotes(int[] frequencies, int duration, CancellationToken token)
        {
            int interval = 30; // Switch notes every 30 ms
            int steps = duration / interval;

            for (int i = 0; i < steps; i++)
            {
                if (token.IsCancellationRequested) return;

                foreach (var frequency in frequencies)
                {
                    NotePlayer.play_note(frequency, interval);
                }
            }
        }

        // Convert MIDI note number to frequency
        private static int NoteToFrequency(int noteNumber)
        {
            return (int)(880.0 * Math.Pow(2.0, (noteNumber - 69) / 12.0));
        }

        private void button_play_Click(object sender, EventArgs e)
        {
            button_play.Enabled = false;
            button_stop.Enabled = true;
            play_MIDI_file(textBox1.Text);
        }
        private void button_stop_Click(object sender, EventArgs e)
        {
            button_play.Enabled = true;
            button_stop.Enabled = false;
            StopPlayback();
        }

        private void button_rewind_Click(object sender, EventArgs e)
        {

        }

        private void MIDI_file_player_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopPlayback();
        }
    }
}
