using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NeoBleeper.ConvertToGCode;

namespace NeoBleeper
{
    public partial class ConvertToBeepCommandForLinux : Form
    {
        // The class that I added for my old laptop, which doesn't support Windows 11, until I migrated to Avalonia UI to make NeoBleeper cross-platform.
        int alternate_length = 30;
        int bpm = 120; // Default BPM
        int note_silence_ratio = 50;
        main_window main_Window;
        bool nonStopping = false;
        public ConvertToBeepCommandForLinux(string musicFile, main_window main_Window)
        {
            this.main_Window = main_Window;
            InitializeComponent();
            UIFonts.setFonts(this);
            richTextBoxBeepCommand.Font = new Font("Consolas", richTextBoxBeepCommand.Font.Size); // Set a monospaced font for better readability
            set_theme();
            String notes = ExtractNotes(musicFile);
            richTextBoxBeepCommand.Text = notes;
        }
        private void set_theme()
        {
            try
            {
                this.SuspendLayout();
                switch (Settings1.Default.theme)
                {
                    case 0: // System theme
                        switch (check_system_theme.IsDarkTheme())
                        {
                            case true:
                                dark_theme();
                                break;
                            case false:
                                light_theme();
                                break;
                        }
                        break;
                    case 1: // Light theme
                        light_theme();
                        break;
                    case 2: // Dark theme
                        dark_theme();
                        break;
                }
                this.ResumeLayout();
            }
            finally
            {
                UIHelper.ForceUpdateUI(this);
            }

        }
        private void dark_theme()
        {
            UIHelper.ApplyCustomTitleBar(this, Color.Black, true);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            buttonCopyBeepCommandToClipboard.BackColor = Color.FromArgb(32, 32, 32);
            buttonSaveAsShFile.BackColor = Color.FromArgb(32, 32, 32);
            richTextBoxBeepCommand.ForeColor = Color.White;
        }
        private void light_theme()
        {
            UIHelper.ApplyCustomTitleBar(this, Color.Black, false);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            buttonCopyBeepCommandToClipboard.BackColor = Color.Transparent;
            buttonSaveAsShFile.BackColor = Color.Transparent;
            richTextBoxBeepCommand.ForeColor = Color.White;
        }
        private void buttonCopyBeepCommandToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxBeepCommand.Text);
            Toast toast = new Toast(this, Resources.MessageConvertedBeepCommandCopied, 2000);
            toast.Show();
        }

        StringBuilder beepCommandBuilder = new StringBuilder();
        int elapsedElementTime = 0; // Equivalent of Stopwatch.ElapsedMilliseconds for text based timing
        private String ExtractNotes(string musicString)
        {
            List<NoteInfo> notes = new List<NoteInfo>();
            beepCommandBuilder.Clear();
            using (StringReader stringReader = new StringReader(musicString))
            {
                var projectFile = DeserializeXMLFromString(stringReader);
                if (projectFile != null)
                {
                    bpm = Convert.ToInt32(projectFile.Settings.RandomSettings.BPM);
                    note_silence_ratio = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                    alternate_length = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
                    nonStopping = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio) == 100;
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                        note_silence_ratio = 50;
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.AlternateTime))
                        alternate_length = 30;

                    notes.Clear();
                    if (projectFile.LineList?.Lines != null)
                    {
                        foreach (var line in projectFile.LineList.Lines)
                        {
                            notes.Add(new NoteInfo(
                                line.Length,
                                line.Note1,
                                line.Note2,
                                line.Note3,
                                line.Note4,
                                line.Mod,
                                line.Art));
                        }
                    }
                }
            }

            if (notes.Count == 0)
                return string.Empty;
            beepCommandBuilder.Append("beep"); // Start the beep command
            foreach (var note in notes)
            {
                bool endOfLine = notes.IndexOf(note) == notes.Count - 1;
                double noteDuration = NoteLengths.CalculateLineLength(bpm, note.Length, note.Mod, note.Art);
                double rawNoteLength = NoteLengths.CalculateNoteLength(bpm, note.Length, note.Mod, note.Art) * (note_silence_ratio / 100.0);
                int note_length = (int)Math.Truncate(rawNoteLength);
                int silence = (int)Math.Truncate(noteDuration - rawNoteLength);
                int drift = 0;
                if (drift > 0)
                {
                    if (drift < note_length)
                    {
                        note_length -= drift; // Reduce note length by drift amount
                    }
                    else
                    {
                        drift -= note_length; // Skip note length if drift is larger
                        continue;
                    }
                }
                // Insert elements of Beep command
                elapsedElementTime = insert_note_to_beep_command(note.Note1, note.Note2, note.Note3, note.Note4,
                    true, true, true, true, note_length, endOfLine);
                if (drift < 0)
                {
                    silence -= drift; // Add drift to silence if drift is negative
                }
                if (silence > 0)
                {
                    beepCommandBuilder.Append(createDelay(silence, false).duration);
                }

            }

            return beepCommandBuilder.ToString();
        }
        private int insert_note_to_beep_command(String note1, String note2, String note3, String note4,
    bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length, bool endOfLine = false) // Play note in a line
        {
            int elapsedTime = 0;
            double note1_frequency = 0, note2_frequency = 0, note3_frequency = 0, note4_frequency = 0;
            String[] notes = new string[4];
            string Note1 = string.Empty, Note2 = string.Empty, Note3 = string.Empty, Note4 = string.Empty;
            if (play_note1)
            {
                Note1 = note1;
            }
            if (play_note2)
            {
                Note2 = note2;
            }
            if (play_note3)
            {
                Note3 = note3;
            }
            if (play_note4)
            {
                Note4 = note4;
            }
            if (main_Window.radioButtonPlay_alternating_notes1.Checked)
            {
                notes = new string[] { Note1, Note2, Note3, Note4 };
            }
            else if (main_Window.radioButtonPlay_alternating_notes2.Checked)
            {
                notes = new string[] { Note1, Note3, Note2, Note4 };
            }
            notes = notes.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray(); // Remove empty notes and duplicates
            // Calculate frequencies from note names
            if (notes.Contains(note1) && !string.IsNullOrWhiteSpace(note1))
                note1_frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);

            if (notes.Contains(note2) && !string.IsNullOrWhiteSpace(note2))
                note2_frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);

            if (notes.Contains(note3) && !string.IsNullOrWhiteSpace(note3))
                note3_frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);

            if (notes.Contains(note4) && !string.IsNullOrWhiteSpace(note4))
                note4_frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
            if (notes.Length == 1)
            {
                if (notes[0] == note1)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note1_frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
                else if (notes[0] == note2)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note2_frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
                else if (notes[0] == note3)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note3_frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
                else if (notes[0] == note4)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note4_frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
            }
            else if (notes.Length > 1)
            {
                string generatedBeepCommand = string.Empty;
                int note_order = 1;
                int remainingLength = length;
                bool willAnyNoteBeWritten = false;
                do
                {
                    foreach (string note in notes)
                    {
                        if (remainingLength >= alternate_length)
                        {
                            willAnyNoteBeWritten = true;
                        }
                        bool isLastAlternatingNoteOfLastLine = false;
                        if (remainingLength >= alternate_length || willAnyNoteBeWritten == false)
                        {
                            int alternate_length_to_write = willAnyNoteBeWritten ? alternate_length : remainingLength;
                            double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);
                            switch (note_order)
                            {
                                case 1: // Note 1
                                    var(frequencyAndLength1, totalDuration1) = CreateFrequencyAndDurationDuo((int)note1_frequency, alternate_length_to_write, false, isLastAlternatingNoteOfLastLine);
                                    generatedBeepCommand = frequencyAndLength1;
                                    elapsedTime += totalDuration1;
                                    break;

                                case 2: // Note 2
                                    var (frequencyAndLength2, totalDuration2) = CreateFrequencyAndDurationDuo((int)note2_frequency, alternate_length_to_write, false, isLastAlternatingNoteOfLastLine);
                                    generatedBeepCommand = frequencyAndLength2;
                                    elapsedTime += totalDuration2;
                                    break;

                                case 3: // Note 3
                                    var (frequencyAndLength3, totalDuration3) = CreateFrequencyAndDurationDuo((int)note3_frequency, alternate_length_to_write, false, isLastAlternatingNoteOfLastLine);
                                    generatedBeepCommand = frequencyAndLength3;
                                    elapsedTime += totalDuration3;
                                    break;

                                case 4: // Note 4
                                    var (frequencyAndLength4, totalDuration4) = CreateFrequencyAndDurationDuo((int)note4_frequency, alternate_length_to_write, false, isLastAlternatingNoteOfLastLine);
                                    generatedBeepCommand = frequencyAndLength4;
                                    elapsedTime += totalDuration4;
                                    break;
                            }
                            isLastAlternatingNoteOfLastLine = endOfLine && (remainingLength <= alternate_length);
                            note_order++;
                        }
                        else
                        {
                            generatedBeepCommand = createDelay(remainingLength, endOfLine).duration;
                            beepCommandBuilder.Append(generatedBeepCommand + Environment.NewLine);
                            remainingLength -= remainingLength;
                            break;
                        }
                        beepCommandBuilder.Append(generatedBeepCommand);
                        remainingLength -= elapsedTime; // Subtract the length of the note and the delay
                    }
                }
                while (remainingLength > 0);
                return elapsedTime;
            }
            else
            {
                beepCommandBuilder.Append(createDelay(length, endOfLine).duration);
                return length;
            }
            return elapsedTime;
        }
        private (string frequencyAndLength, int totalDuration) CreateFrequencyAndDurationDuo(int frequency, int duration, bool endOfLine, bool nonStopping = false)
        {
            string result = string.Empty;
            if (!nonStopping)
            {
                result += " -d 5 -n"; // Add a small delay before the beep to ensure it plays correctly
            }
            result += $" -f {frequency} -l {duration}"; // Add frequency and duration
            if (!endOfLine)
            {
                result += " -n"; // Add -n to start new beep if not end of line
            }
            return (result, nonStopping ? duration : duration + 5);
        }
        private (string duration, int totalDuration) createDelay(int duration, bool endOfLine)
        {
            string result = $" -d {duration}"; // Add delay
            if (!endOfLine)
            {
                result += " -n"; // Add -n to start new beep if not end of line
            }
            return (result, duration);
        }
        private NBPML_File.NeoBleeperProjectFile? DeserializeXMLFromString(StringReader stringReader)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
            return (NBPML_File.NeoBleeperProjectFile)serializer.Deserialize(stringReader);
        }

        private void buttonSaveAsShFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Resources.FilterBeepCommandFileFormats;
            saveFileDialog.Title = Resources.SaveBeepCommandTitle;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, richTextBoxBeepCommand.Text);
                    Logger.Log("Beep command saved as shell script: " + saveFileDialog.FileName, Logger.LogTypes.Info);
                    MessageBox.Show(Resources.MessageBeepCommandSaved, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Logger.Log("Error saving beep command as shell script: " + ex.Message, Logger.LogTypes.Error);
                    MessageBox.Show(Resources.MessageBeepCommandSavingError + ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
