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

using NeoBleeper.Properties;
using System.Data;
using System.Text;
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
        int[] probableResonantFrequencies = new int[] { 45, 50, 60, 100, 120 }; // Common resonant frequencies to avoid because storage type can't be determined in Linux's beep command
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
            UIHelper.ApplyCustomTitleBar(this, Color.White, false);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            buttonCopyBeepCommandToClipboard.BackColor = Color.Transparent;
            buttonSaveAsShFile.BackColor = Color.Transparent;
            richTextBoxBeepCommand.ForeColor = Color.White;
        }
        private void buttonCopyBeepCommandToClipboard_Click(object sender, EventArgs e)
        {
            string textToCopy = richTextBoxBeepCommand.Text.Trim();
            textToCopy = textToCopy.Replace("\n", string.Empty);
            Clipboard.SetText(textToCopy);
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
                    // Pass endOfLine so that last element doesn't get a trailing -n
                    beepCommandBuilder.Append(createDelay(silence, endOfLine).duration);
                }

            }
            string rawOutput = beepCommandBuilder.ToString();
            string trimmedOutput = rawOutput.TrimEnd();
            return trimmedOutput;
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
                    note_order = 1; // Reset order at each alternation cycle
                    foreach (string note in notes)
                    {
                        if (remainingLength >= alternate_length)
                        {
                            willAnyNoteBeWritten = true;
                        }
                        if (remainingLength >= alternate_length || willAnyNoteBeWritten == false)
                        {
                            int alternate_length_to_write = willAnyNoteBeWritten ? alternate_length : remainingLength;
                            // Determine if this alternating chunk will be the last chunk of the last line.
                            bool isLastAlternatingNoteOfLastLine = endOfLine && (alternate_length_to_write == remainingLength);
                            double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);

                            int currentDuration = 0; // duration for this single chunk
                            switch (note_order)
                            {
                                case 1: // Note 1
                                    {
                                        var (frequencyAndLength1, totalDuration1) = CreateFrequencyAndDurationDuo((int)frequency, alternate_length_to_write, isLastAlternatingNoteOfLastLine, nonStopping);
                                        generatedBeepCommand = frequencyAndLength1;
                                        currentDuration = totalDuration1;
                                        break;
                                    }
                                case 2: // Note 2
                                    {
                                        var (frequencyAndLength2, totalDuration2) = CreateFrequencyAndDurationDuo((int)frequency, alternate_length_to_write, isLastAlternatingNoteOfLastLine, nonStopping);
                                        generatedBeepCommand = frequencyAndLength2;
                                        currentDuration = totalDuration2;
                                        break;
                                    }
                                case 3: // Note 3
                                    {
                                        var (frequencyAndLength3, totalDuration3) = CreateFrequencyAndDurationDuo((int)frequency, alternate_length_to_write, isLastAlternatingNoteOfLastLine, nonStopping);
                                        generatedBeepCommand = frequencyAndLength3;
                                        currentDuration = totalDuration3;
                                        break;
                                    }
                                case 4: // Note 4
                                    {
                                        var (frequencyAndLength4, totalDuration4) = CreateFrequencyAndDurationDuo((int)frequency, alternate_length_to_write, isLastAlternatingNoteOfLastLine, nonStopping);
                                        generatedBeepCommand = frequencyAndLength4;
                                        currentDuration = totalDuration4;
                                        break;
                                    }
                                default:
                                    {
                                        // Safety: if note_order goes out of expected range, treat as a simple chunk
                                        var (frequencyAndLengthX, totalDurationX) = CreateFrequencyAndDurationDuo((int)frequency, alternate_length_to_write, isLastAlternatingNoteOfLastLine, nonStopping);
                                        generatedBeepCommand = frequencyAndLengthX;
                                        currentDuration = totalDurationX;
                                        break;
                                    }
                            }

                            elapsedTime += currentDuration;
                            note_order++;
                            beepCommandBuilder.Append(generatedBeepCommand);
                            remainingLength -= currentDuration; // Subtract only this chunk's duration
                        }
                        else
                        {
                            generatedBeepCommand = createDelay(remainingLength, endOfLine).duration;
                            beepCommandBuilder.Append(generatedBeepCommand + Environment.NewLine);
                            remainingLength = 0;
                            break;
                        }

                        if (remainingLength <= 0)
                            break;
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
            if(probableResonantFrequencies.Contains(frequency))
            {
                frequency += 1; // Shift frequency by 1 Hz to avoid resonant frequency issues in computer that used with beep command
            }
            string result = string.Empty;
            result += $" -f {frequency} -l {duration}"; // Add frequency and duration
            if (!nonStopping)
            {
                result += " -n -f 0 -l 0 -D 5"; // Add a small delay before the beep to ensure it plays correctly
            }
            if (!endOfLine)
            {
                result += " -n"; // Add -n to start new beep if not end of line
            }
            return (result, nonStopping ? duration : duration + 5);
        }
        private (string duration, int totalDuration) createDelay(int duration, bool endOfLine)
        {
            string result = $" -f 0 -l 0 -D {duration}"; // Add delay
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
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string textToSave = richTextBoxBeepCommand.Text.Trim();
                    textToSave = textToSave.Replace("\n", string.Empty);
                    System.IO.File.WriteAllText(saveFileDialog1.FileName, textToSave);
                    Logger.Log("Beep command saved as shell script: " + saveFileDialog1.FileName, Logger.LogTypes.Info);
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