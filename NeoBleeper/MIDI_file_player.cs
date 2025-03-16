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
        private static long totalMidiDuration; // Store total MIDI duration
        private static Label staticLabelPosition; // Static references
        private static Label staticLabelPercentage;

        // Play method
        public void play_MIDI_file(string filename)
        {
            // Assign labels into static references
            staticLabelPosition = label_position;
            staticLabelPercentage = label_percentage;

            // Stop current playback if playing
            StopPlayback();

            // Create new CancellationTokenSource
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            isPlaying = true;

            // Play in seperate thread
            Task.Run(() =>
            {
                try
                {
                    PlayMidiFileInternal(filename, token);
                }
                catch (OperationCanceledException)
                {
                    // Çalma iptal edildi
                    UpdatePositionLabels(0, 0); // Sıfırla
                }
                finally
                {
                    isPlaying = false;
                }
            }, token);
        }

        // Method of label updating
        private static void UpdatePositionLabels(long currentPosition, long totalDuration)
        {
            if (staticLabelPosition != null && staticLabelPercentage != null)
            {
                // Run in UI thread
                if (staticLabelPosition.InvokeRequired)
                {
                    staticLabelPosition.Invoke(new Action(() => UpdatePositionLabels(currentPosition, totalDuration)));
                    return;
                }

                    // Calculate position (minute:second:decimal)
                    TimeSpan position = TimeSpan.FromMilliseconds(currentPosition);
                staticLabelPosition.Text = string.Format("Position: {0:00}:{1:00}.{2:00}",
                    position.Minutes, position.Seconds, Math.Truncate(Convert.ToDouble(position.Milliseconds / 100)));

                // Calculate percentage
                double percentage = (totalDuration > 0) ? (currentPosition * 100.0 / totalDuration) : 0;
                staticLabelPercentage.Text = string.Format("%{0:00.00}", percentage);
            }
        }

        private static void PlayMidiFileInternal(string filename, CancellationToken token)
        {
            var midiFile = new MidiFile(filename, false);
            var noteEvents = new List<NoteEvent>();

            // Calculate total duration
            totalMidiDuration = CalculateTotalMidiDuration(midiFile);

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    if (midiEvent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        var noteEvent = (NoteEvent)midiEvent;
                        noteEvents.Add(noteEvent);
                    }
                }
            }

            // Group and list notes
            var groupedNotes = noteEvents.GroupBy(e => e.AbsoluteTime)
                                         .OrderBy(g => g.Key)
                                         .ToList();

            // if there's no note, return
            if (groupedNotes.Count == 0)
                return;

            long previousTime = 0;
            long currentPosition = 0;

            // Show start position
            UpdatePositionLabels(0, totalMidiDuration);

            // For each note group
            for (int i = 0; i < groupedNotes.Count; i++)
            {
                // Stop playback if stopped
                token.ThrowIfCancellationRequested();

                var group = groupedNotes[i];
                long currentTime = group.Key;
                var notes = group.ToList();

                // Calculate duration between previous and current times
                long timeDifference = currentTime - previousTime;

                // If time is elapsed, considered as silence
                if (timeDifference > 0)
                {
                    // Render - wait the silence
                    currentPosition += timeDifference;
                    UpdatePositionLabels(currentPosition, totalMidiDuration);
                    WaitWithCancellation((int)timeDifference, token);
                }

                // Calculate length of notes
                int duration = notes.Max(n => n.DeltaTime);

                if (notes.Count == 1)
                {
                    // Play single note
                    var note = notes.First();
                    int frequency = NoteToFrequency(note.NoteNumber);
                    NotePlayer.play_note(frequency, duration);
                }
                else
                {
                    // Play multiple notes in same time
                    var frequencies = notes.Select(note => NoteToFrequency(note.NoteNumber)).ToArray();
                    PlayMultipleNotes(frequencies, duration, token);
                }

                // Wait length of notes after playing
                currentPosition += duration;
                UpdatePositionLabels(currentPosition, totalMidiDuration);
                WaitWithCancellation(duration, token);

                // Save now for next iteration
                previousTime = currentTime + duration;
            }

            // Reset position after playing
            UpdatePositionLabels(0, totalMidiDuration);
        }

        // MIDI duration calculation method
        private static long CalculateTotalMidiDuration(MidiFile midiFile)
        {
            long maxTime = 0;

            foreach (var track in midiFile.Events)
            {
                foreach (var midiEvent in track)
                {
                    // Calculate starting time and duration
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

        // Public method for stop playback
        public static void StopPlayback()
        {
            if (isPlaying && cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                isPlaying = false;

                // Reset position when stopped
                if (staticLabelPosition != null && staticLabelPercentage != null)
                {
                    UpdatePositionLabels(0, totalMidiDuration);
                }
            }
        }
        private static void WaitWithCancellation(int milliseconds, CancellationToken token)
        {
            try
            {
                Task.Delay(milliseconds, token).Wait(token);
            }
            catch (OperationCanceledException)
            {
                // Cancelled
                throw;
            }
        }

        
        private static void PlayMultipleNotes(int[] frequencies, int duration, CancellationToken token)
        {
                            Application.DoEvents();
            int interval = 30; // Switch notes between 30 ms
            int steps = duration / interval;

            for (int i = 0; i < steps; i++)
            {
                // Return if cancelled
                if (token.IsCancellationRequested)
                    return;

                foreach (var frequency in frequencies)
                {
                    NotePlayer.play_note(frequency, interval);
                }
            }
        }


        private static int NoteToFrequency(int noteNumber)
        {
            // Frequency from MIDI number
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
