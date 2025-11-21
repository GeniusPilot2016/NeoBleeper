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
using static UIHelper;

namespace NeoBleeper
{
    // When your CNC machine or 3D printer meets the world of music: Converting melodies to GCode for a symphony of mechanical sounds!
    public partial class ConvertToGCode : Form
    {
        bool darkTheme = false;
        int note1_component = 0;
        int note2_component = 0;
        int note3_component = 0;
        int note4_component = 0;
        String MusicString;
        int alternate_length = 30;
        int bpm = 120; // Default BPM
        int note_silence_ratio = 50;
        bool nonStopping = false;
        public ConvertToGCode(String musicFile)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.setFonts(this);
            set_theme();
            MusicString = musicFile;
            comboBox_component_note1.SelectedIndex = 0;
            comboBox_component_note2.SelectedIndex = 0;
            comboBox_component_note3.SelectedIndex = 0;
            comboBox_component_note4.SelectedIndex = 0;
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

        public class NoteInfo
        {
            public String Length { get; set; }
            public String Note1 { get; set; }
            public String Note2 { get; set; }
            public String Note3 { get; set; }
            public String Note4 { get; set; }
            public String Mod { get; set; }
            public String Art { get; set; }
            public NoteInfo(string length, string note1, string note2, string note3, string note4,
                string mod, string art)
            {
                Length = length;
                Note1 = note1;
                Note2 = note2;
                Note3 = note3;
                Note4 = note4;
                Mod = mod;
                Art = art;
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            label_note1.ForeColor = Color.White;
            label_note2.ForeColor = Color.White;
            label_note3.ForeColor = Color.White;
            label_note4.ForeColor = Color.White;
            button_export_as_gcode.BackColor = Color.FromArgb(32, 32, 32);
            comboBox_component_note1.BackColor = Color.Black;
            comboBox_component_note2.BackColor = Color.Black;
            comboBox_component_note3.BackColor = Color.Black;
            comboBox_component_note4.BackColor = Color.Black;
            comboBox_component_note1.ForeColor = Color.White;
            comboBox_component_note2.ForeColor = Color.White;
            comboBox_component_note3.ForeColor = Color.White;
            comboBox_component_note4.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            label_note1.ForeColor = SystemColors.ControlText;
            label_note2.ForeColor = SystemColors.ControlText;
            label_note3.ForeColor = SystemColors.ControlText;
            label_note4.ForeColor = SystemColors.ControlText;
            button_export_as_gcode.BackColor = Color.Transparent;
            comboBox_component_note1.BackColor = SystemColors.Window;
            comboBox_component_note2.BackColor = SystemColors.Window;
            comboBox_component_note3.BackColor = SystemColors.Window;
            comboBox_component_note4.BackColor = SystemColors.Window;
            comboBox_component_note1.ForeColor = SystemColors.WindowText;
            comboBox_component_note2.ForeColor = SystemColors.WindowText;
            comboBox_component_note3.ForeColor = SystemColors.WindowText;
            comboBox_component_note4.ForeColor = SystemColors.WindowText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
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
        StringBuilder gcodeBuilder = new StringBuilder();
        int elapsedLineTime = 0; // Equivalent of Stopwatch.ElapsedMilliseconds for text based timing
        private String ExtractNotes(string musicString)
        {
            List<NoteInfo> notes = new List<NoteInfo>();
            gcodeBuilder.Clear();
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

            foreach (var note in notes)
            {
                double noteDuration = NoteLengths.CalculateLineLength(bpm, note.Length, note.Mod, note.Art);
                double rawNoteLength = NoteLengths.CalculateNoteLength(bpm, note.Length, note.Mod, note.Art) * (note_silence_ratio / 100.0);
                int note_length = (int)Math.Truncate(rawNoteLength);
                int silence = (int)Math.Truncate(noteDuration - rawNoteLength);
                int drift = 0;
                if(drift > 0)
                {
                    if(drift < note_length)
                    {
                        note_length -= drift; // Reduce note length by drift amount
                    }
                    else
                    {
                        drift -= note_length; // Skip note length if drift is larger
                        continue;
                    }
                }
                // Add GCode line
                elapsedLineTime = insert_note_to_gcode(note.Note1, note.Note2, note.Note3, note.Note4,
                    true, true, true, true, note_length);
                if(drift < 0)
                {
                    silence -= drift; // Add drift to silence if drift is negative
                }
                if (silence > 0)
                {
                    gcodeBuilder.AppendLine($"G4 P{silence}");
                }
            }

            return gcodeBuilder.ToString();
        }
        private NBPML_File.NeoBleeperProjectFile? DeserializeXMLFromString(StringReader stringReader)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NBPML_File.NeoBleeperProjectFile));
            return (NBPML_File.NeoBleeperProjectFile)serializer.Deserialize(stringReader);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            note1_component = comboBox_component_note1.SelectedIndex;
        }
        private void EnableDisableExportAsGCodeButton()
        {
            if (checkBox_play_note1.Checked || checkBox_play_note2.Checked || checkBox_play_note3.Checked || checkBox_play_note4.Checked)
            {
                button_export_as_gcode.Enabled = true;
            }
            else
            {
                button_export_as_gcode.Enabled = false;
            }
        }
        private void checkBoxes_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableExportAsGCodeButton();
            if (checkBox_play_note1.Checked)
            {
                label_note1.Enabled = true;
                comboBox_component_note1.Enabled = true;
            }
            else
            {
                label_note1.Enabled = false;
                comboBox_component_note1.Enabled = false;
            }
            if (checkBox_play_note2.Checked)
            {
                label_note2.Enabled = true;
                comboBox_component_note2.Enabled = true;
            }
            else
            {
                label_note2.Enabled = false;
                comboBox_component_note2.Enabled = false;
            }
            if (checkBox_play_note3.Checked)
            {
                label_note3.Enabled = true;
                comboBox_component_note3.Enabled = true;
            }
            else
            {
                label_note3.Enabled = false;
                comboBox_component_note3.Enabled = false;
            }
            if (checkBox_play_note4.Checked)
            {
                label_note4.Enabled = true;
                comboBox_component_note4.Enabled = true;
            }
            else
            {
                label_note4.Enabled = false;
                comboBox_component_note4.Enabled = false;
            }
        }

        private void button_export_as_gcode_Click(object sender, EventArgs e)
        {
            String notes = ExtractNotes(MusicString);
            if (!string.IsNullOrEmpty(notes))
            {
                DialogResult result = exportGCodeFile.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    string filePath = exportGCodeFile.FileName;
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(notes);
                        writer.Close();
                    }
                    Logger.Log("GCode exported to " + filePath, Logger.LogTypes.Info);
                    MessageForm.Show(Resources.MessageGCodeExported, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                Logger.Log("No notes found to convert to GCode.", Logger.LogTypes.Error);
                MessageForm.Show(Resources.MessageGCodeEmptyNoteList, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox_component_note2_SelectedIndexChanged(object sender, EventArgs e)
        {
            note2_component = comboBox_component_note2.SelectedIndex;
        }

        private void comboBox_component_note3_SelectedIndexChanged(object sender, EventArgs e)
        {
            note3_component = comboBox_component_note3.SelectedIndex;
        }

        private void comboBox_component_note4_SelectedIndexChanged(object sender, EventArgs e)
        {
            note4_component = comboBox_component_note4.SelectedIndex;
        }
        private (string output, int length) GenerateGCodeForBuzzerNote(double frequency, int length, bool nonStopping = false)
        {
            string delay = nonStopping ? "G4 P5\n" : string.Empty;
            return ($"G4 P5\nM300 S{frequency} P{length}", nonStopping ? length : length + 5);
        }
        private (string output, int length) GenerateGCodeForMotorNote(int frequency, int length, bool nonStopping = false)
        {
            // Convert frequency to RPM
            int rpm = FrequencyToRPM(frequency);
            string delay = nonStopping ? "G4 P5\n" : string.Empty;
            // Start the motor, then wait for the specified length, then stop the motor
            return (delay + $"G4 P5\nM3 S{rpm}\nG4 P{length}\nM5", nonStopping? length : length + 5); 
        }
        private int FrequencyToRPM(double frequency)
        {
            return (int)(frequency * 60); // Convert frequency to RPM
        }
        private int insert_note_to_gcode(String note1, String note2, String note3, String note4,
    bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length) // Play note in a line
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
            if (radioButtonPlay_alternating_notes1.Checked)
            {
                notes = new string[] { Note1, Note2, Note3, Note4 };
            }
            else if (radioButtonPlay_alternating_notes2.Checked)
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
                    switch (comboBox_component_note1.SelectedIndex)
                    {
                        case 0:
                            var motorNote = GenerateGCodeForMotorNote((int)note1_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForMotorNote((int)note1_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(buzzerNote.output);
                            elapsedTime = buzzerNote.length;
                            break;
                    }
                    return elapsedTime;
                }
                else if (notes[0] == note2)
                {
                    switch (comboBox_component_note2.SelectedIndex)
                    {
                        case 0:
                            var motorNote = GenerateGCodeForMotorNote((int)note2_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForMotorNote((int)note2_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(buzzerNote.output);
                            elapsedTime = buzzerNote.length;
                            break;
                    }
                    return elapsedTime;
                }
                else if (notes[0] == note3)
                {
                    switch (comboBox_component_note3.SelectedIndex)
                    {
                        case 0:
                            var motorNote = GenerateGCodeForMotorNote((int)note3_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForMotorNote((int)note3_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(buzzerNote.output);
                            elapsedTime = buzzerNote.length;
                            break;
                    }
                    return elapsedTime;
                }
                else if (notes[0] == note4)
                {
                    switch (comboBox_component_note4.SelectedIndex)
                    {
                        case 0:
                            var motorNote = GenerateGCodeForMotorNote((int)note4_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForMotorNote((int)note4_frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(buzzerNote.output);
                            elapsedTime = buzzerNote.length;
                            break;
                    }
                    return elapsedTime;
                }
            }
            else if (notes.Length > 1)
            {
                int note_order = 1;
                int remainingLength = length;
                bool willAnyNoteBeWritten = false;
                do
                {
                    note_order = 1; // Reset order at each alternation cycle
                    foreach (string note in notes)
                    {
                        if (remainingLength <= 0)
                            break;

                        if (remainingLength >= alternate_length)
                        {
                            willAnyNoteBeWritten = true;
                        }

                        if (remainingLength >= alternate_length || willAnyNoteBeWritten == false)
                        {
                            int alternate_length_to_write = willAnyNoteBeWritten ? alternate_length : remainingLength;
                            double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);

                            int currentDuration = 0;
                            string generatedGCode = string.Empty;

                            switch (note_order)
                            {
                                case 1: // Note 1
                                    if (comboBox_component_note1.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note1_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note1_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                case 2: // Note 2
                                    if (comboBox_component_note2.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note2_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note2_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                case 3: // Note 3
                                    if (comboBox_component_note3.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note3_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note3_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                case 4: // Note 4
                                    if (comboBox_component_note4.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note4_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note4_frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                default:
                                    // Safety fallback
                                    var fallback = GenerateGCodeForBuzzerNote((int)frequency, alternate_length_to_write, nonStopping);
                                    generatedGCode = fallback.output;
                                    currentDuration = fallback.length;
                                    break;
                            }

                            // Append and account only for this chunk's duration
                            if (!string.IsNullOrEmpty(generatedGCode))
                                gcodeBuilder.AppendLine(generatedGCode);

                            elapsedTime += currentDuration;
                            remainingLength -= currentDuration;
                            note_order++;
                        }
                        else
                        {
                            // Remaining less than alternate chunk => insert delay and finish
                            gcodeBuilder.AppendLine($"G4 P{remainingLength}");
                            remainingLength = 0;
                            break;
                        }
                    }
                }
                while (remainingLength > 0);
                return elapsedTime;
            }
            else // No notes to play
            {
                gcodeBuilder.AppendLine($"G4 P{length}");
                return length;
            }
            return elapsedTime;
        }
        private void ConvertToGCode_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}
