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
using static UIHelper;

namespace NeoBleeper
{
    public partial class ConvertToBeepCommandForLinux : Form
    {
        // The class that I added for my old laptop, which doesn't support Windows 11, until I migrated to Avalonia UI to make NeoBleeper cross-platform.
        int alternateLength = 30;
        int bpm = 120; // Default BPM
        int note_silence_ratio = 50;
        MainWindow mainWindow;
        bool nonStopping = false;
        bool darkTheme = false;
        int[] probableResonantFrequencies = new int[] { 45, 50, 60, 100, 120 }; // Common resonant frequencies to avoid because storage type can't be determined in Linux's beep command
        public ConvertToBeepCommandForLinux(string musicFile, MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
            richTextBoxBeepCommand.Font = new Font("Consolas", richTextBoxBeepCommand.Font.Size); // Set a monospaced font for better readability
            SetTheme();
            String notes = ExtractNotes(musicFile);
            richTextBoxBeepCommand.Text = notes;
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

        /// <summary>
        /// Applies the current application theme to the form based on user or system preferences.
        /// </summary>
        /// <remarks>This method selects and applies a light or dark theme according to the application's
        /// theme settings. If the theme is set to follow the system, the method detects the system's current theme and
        /// applies the corresponding style. The method also ensures that UI updates are performed efficiently and that
        /// the form is refreshed to reflect the new theme.</remarks>
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
        }
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            buttonCopyBeepCommandToClipboard.BackColor = Color.FromArgb(32, 32, 32);
            buttonSaveAsShFile.BackColor = Color.FromArgb(32, 32, 32);
            richTextBoxBeepCommand.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            buttonCopyBeepCommandToClipboard.BackColor = Color.Transparent;
            buttonSaveAsShFile.BackColor = Color.Transparent;
            richTextBoxBeepCommand.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private void buttonCopyBeepCommandToClipboard_Click(object sender, EventArgs e)
        {
            string textToCopy = richTextBoxBeepCommand.Text.Trim();
            textToCopy = textToCopy.Replace("\n", string.Empty);
            Clipboard.SetText(textToCopy);
            Toast.ShowToast(this, Resources.MessageConvertedBeepCommandCopied, 2000);
        }


        StringBuilder beepCommandBuilder = new StringBuilder();
        int elapsedElementTime = 0; // Equivalent of Stopwatch.ElapsedMilliseconds for text based timing

        /// <summary>
        /// Parses a music project string and generates a formatted beep command sequence representing the extracted
        /// notes.
        /// </summary>
        /// <remarks>The returned beep command sequence encodes the timing and pitch information for each
        /// note as specified in the input music project. The method applies default values for certain settings if they
        /// are missing from the input. The output can be used as input to a beep-compatible audio tool or command-line
        /// utility.</remarks>
        /// <param name="musicString">A string containing the serialized music project data to parse for note extraction. Cannot be null.</param>
        /// <returns>A string containing the formatted beep command sequence for the extracted notes. Returns an empty string if
        /// no notes are found or if the input is invalid.</returns>
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
                    alternateLength = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
                    nonStopping = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio) == 100;
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                        note_silence_ratio = 50;
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.AlternateTime))
                        alternateLength = 30;

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
                    beepCommandBuilder.Append(CreateDelay(silence, endOfLine).duration);
                }

            }
            string rawOutput = beepCommandBuilder.ToString();
            string trimmedOutput = rawOutput.TrimEnd();
            return trimmedOutput;
        }

        /// <summary>
        /// Inserts one or more musical notes into the beep command sequence, specifying which notes to play, their
        /// order, and duration.
        /// </summary>
        /// <remarks>The order and alternation of notes depend on the current playback mode. Only notes
        /// marked to be played and with non-empty names are included. If no notes are selected, a delay of the
        /// specified length is inserted instead.</remarks>
        /// <param name="note1">The name of the first note to include in the sequence. Can be null or empty if not used.</param>
        /// <param name="note2">The name of the second note to include in the sequence. Can be null or empty if not used.</param>
        /// <param name="note3">The name of the third note to include in the sequence. Can be null or empty if not used.</param>
        /// <param name="note4">The name of the fourth note to include in the sequence. Can be null or empty if not used.</param>
        /// <param name="playNote1">true to play the note specified by note1; otherwise, false.</param>
        /// <param name="playNote2">true to play the note specified by note2; otherwise, false.</param>
        /// <param name="playNote3">true to play the note specified by note3; otherwise, false.</param>
        /// <param name="playNote4">true to play the note specified by note4; otherwise, false.</param>
        /// <param name="length">The total duration, in milliseconds, for which the notes should be played. Must be a positive integer.</param>
        /// <param name="endOfLine">true to indicate that this is the last note or group of notes in the line; otherwise, false. The default is
        /// false.</param>
        /// <returns>The total elapsed time, in milliseconds, for the inserted notes or delay.</returns>
        private int insert_note_to_beep_command(String note1, String note2, String note3, String note4,
        bool playNote1, bool playNote2, bool playNote3, bool playNote4, int length, bool endOfLine = false) // Play note in a line
        {
            int elapsedTime = 0;
            double note1Frequency = 0, note2Frequency = 0, note3Frequency = 0, note4Frequency = 0;
            String[] notes = new string[4];
            string Note1 = string.Empty, Note2 = string.Empty, Note3 = string.Empty, Note4 = string.Empty;
            if (playNote1)
            {
                Note1 = note1;
            }
            if (playNote2)
            {
                Note2 = note2;
            }
            if (playNote3)
            {
                Note3 = note3;
            }
            if (playNote4)
            {
                Note4 = note4;
            }
            if (mainWindow.radioButtonPlay_alternating_notes1.Checked)
            {
                notes = new string[] { Note1, Note2, Note3, Note4 };
            }
            else if (mainWindow.radioButtonPlay_alternating_notes2.Checked)
            {
                notes = new string[] { Note1, Note3, Note2, Note4 };
            }
            notes = notes.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToArray(); // Remove empty notes and duplicates
                                                                                          // Calculate frequencies from note names
            if (notes.Contains(note1) && !string.IsNullOrWhiteSpace(note1))
                note1Frequency = NoteFrequencies.GetFrequencyFromNoteName(note1);

            if (notes.Contains(note2) && !string.IsNullOrWhiteSpace(note2))
                note2Frequency = NoteFrequencies.GetFrequencyFromNoteName(note2);

            if (notes.Contains(note3) && !string.IsNullOrWhiteSpace(note3))
                note3Frequency = NoteFrequencies.GetFrequencyFromNoteName(note3);

            if (notes.Contains(note4) && !string.IsNullOrWhiteSpace(note4))
                note4Frequency = NoteFrequencies.GetFrequencyFromNoteName(note4);
            if (notes.Length == 1)
            {
                if (notes[0] == note1)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note1Frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
                else if (notes[0] == note2)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note2Frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
                else if (notes[0] == note3)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note3Frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
                else if (notes[0] == note4)
                {
                    var (frequencyAndLength, totalDuration) = CreateFrequencyAndDurationDuo((int)note4Frequency, length, endOfLine, nonStopping);
                    beepCommandBuilder.Append(frequencyAndLength);
                    elapsedTime = totalDuration;
                    return elapsedTime;
                }
            }
            else if (notes.Length > 1)
            {
                string generatedBeepCommand = string.Empty;
                int noteOrder = 1;
                int remainingLength = length;
                bool willAnyNoteBeWritten = false;
                do
                {
                    noteOrder = 1; // Reset order at each alternation cycle
                    foreach (string note in notes)
                    {
                        if (remainingLength >= alternateLength)
                        {
                            willAnyNoteBeWritten = true;
                        }
                        if (remainingLength >= alternateLength || willAnyNoteBeWritten == false)
                        {
                            int alternate_length_to_write = willAnyNoteBeWritten ? alternateLength : remainingLength;
                            // Determine if this alternating chunk will be the last chunk of the last line.
                            bool isLastAlternatingNoteOfLastLine = endOfLine && (alternate_length_to_write == remainingLength);
                            double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);

                            int currentDuration = 0; // duration for this single chunk
                            switch (noteOrder)
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
                            noteOrder++;
                            beepCommandBuilder.Append(generatedBeepCommand);
                            remainingLength -= currentDuration; // Subtract only this chunk's duration
                        }
                        else
                        {
                            generatedBeepCommand = CreateDelay(remainingLength, endOfLine).duration;
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
                beepCommandBuilder.Append(CreateDelay(length, endOfLine).duration);
                return length;
            }
            return elapsedTime;
        }

        /// <summary>
        /// Creates a command-line argument string representing a beep with the specified frequency and duration, and
        /// calculates the total duration including any required delays.
        /// </summary>
        /// <remarks>If the specified frequency matches a known resonant frequency, it is incremented by 1
        /// Hz to prevent potential playback issues. When nonStopping is false, an extra delay is added to ensure the
        /// beep plays correctly.</remarks>
        /// <param name="frequency">The frequency of the beep, in hertz. If the value matches a known resonant frequency, it is automatically
        /// adjusted to avoid issues.</param>
        /// <param name="duration">The duration of the beep, in milliseconds. Must be a non-negative integer.</param>
        /// <param name="endOfLine">true if the beep is at the end of a line and should not be followed by a new beep; otherwise, false.</param>
        /// <param name="nonStopping">true to omit the additional delay and stop sequence after the beep; otherwise, false. The default is false.</param>
        /// <returns>A tuple containing the constructed command-line argument string for the beep and the total duration in
        /// milliseconds, including any additional delay if applicable.</returns>
        private (string frequencyAndLength, int totalDuration) CreateFrequencyAndDurationDuo(int frequency, int duration, bool endOfLine, bool nonStopping = false)
        {
            if (probableResonantFrequencies.Contains(frequency))
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

        /// <summary>
        /// Creates a delay command string and returns the corresponding duration information.
        /// </summary>
        /// <param name="duration">The length of the delay, in milliseconds. Must be a non-negative integer.</param>
        /// <param name="endOfLine">true to indicate the delay is at the end of a line; otherwise, false to start a new beep after the delay.</param>
        /// <returns>A tuple containing the delay command string and the total duration in milliseconds.</returns>
        private (string duration, int totalDuration) CreateDelay(int duration, bool endOfLine)
        {
            string result = $" -f 0 -l 0 -D {duration}"; // Add delay
            if (!endOfLine)
            {
                result += " -n"; // Add -n to start new beep if not end of line
            }
            return (result, duration);
        }

        /// <summary>
        /// Deserializes an XML representation of a NeoBleeper project file from the specified string reader.
        /// </summary>
        /// <param name="stringReader">A StringReader containing the XML data to deserialize. Must not be null and must contain a valid XML
        /// representation of a NeoBleeper project file.</param>
        /// <returns>A NeoBleeperProjectFile object deserialized from the XML data, or null if the XML does not represent a valid
        /// NeoBleeper project file.</returns>
        private NBPMLFile.NeoBleeperProjectFile? DeserializeXMLFromString(StringReader stringReader)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NBPMLFile.NeoBleeperProjectFile));
            return (NBPMLFile.NeoBleeperProjectFile)serializer.Deserialize(stringReader);
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
                    MessageForm.Show(Resources.MessageBeepCommandSaved, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Logger.Log("Error saving beep command as shell script: " + ex.Message, Logger.LogTypes.Error);
                    MessageForm.Show(Resources.MessageBeepCommandSavingError + ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}