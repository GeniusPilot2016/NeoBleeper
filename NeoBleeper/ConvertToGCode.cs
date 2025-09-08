using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NeoBleeper.main_window;

namespace NeoBleeper
{
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
        public ConvertToGCode(String musicFile)
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            set_theme();
            MusicString = musicFile;
            comboBox_component_note1.SelectedIndex = 0;
            comboBox_component_note2.SelectedIndex = 0;
            comboBox_component_note3.SelectedIndex = 0;
            comboBox_component_note4.SelectedIndex = 0;
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
            TitleBarHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
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
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
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
            this.Refresh();
        }
        StringBuilder gcodeBuilder = new StringBuilder();
        private String ExtractNotes(string musicString)
        {
            List<NoteInfo> notes = new List<NoteInfo>();
            gcodeBuilder.Clear();
            // Prepare the music string for parsing
            using (StringReader stringReader = new StringReader(musicString))
            {
                NBPML_File.NeoBleeperProjectFile? projectFile = DeserializeXMLFromString(stringReader);
                if (projectFile != null)
                {
                    bpm = Convert.ToInt32(projectFile.Settings.RandomSettings.BPM);
                    note_silence_ratio = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                    alternate_length = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);

                    // Assign default values if settings are not found
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                    {
                        note_silence_ratio = 50; // Default value
                        Logger.Log("Note silence ratio not found, defaulting to 50%", Logger.LogTypes.Info);
                    }
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.AlternateTime))
                    {
                        alternate_length = 30; // Default value
                        Logger.Log("Alternating note length not found, defaulting to 30 ms", Logger.LogTypes.Info);
                    }

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
            {
                return string.Empty; // Return empty string if no notes are found
            }
            double remainder = 0.0;
            foreach (var note in notes)
            {
                double raw_note_length = CalculateNoteLength(note.Length, note.Mod, note.Art) * (note_silence_ratio / 100.0);
                int note_length = (int)Math.Truncate(FixRoundingErrors(CalculateNoteLength(note.Length, note.Mod, note.Art) * (note_silence_ratio / 100.0)));
                int silence = (int)Math.Truncate(FixRoundingErrors(CalculateLineLength(note.Length, note.Mod) - (CalculateNoteLength(note.Length, note.Mod, note.Art) * (note_silence_ratio / 100))));
                double difference = RemoveWholeNumber(raw_note_length - (note_length + silence));
                remainder += difference;
                int roundedReminder = (int)Math.Round(remainder, MidpointRounding.ToEven);
                if (roundedReminder >= 1.0 || roundedReminder <= -1.0)
                {
                    note_length -= roundedReminder;
                    remainder -= remainder;
                }
                insert_note_to_gcode(note.Note1, note.Note2, note.Note3, note.Note4,
                    checkBox_play_note1.Checked, checkBox_play_note2.Checked,
                    checkBox_play_note3.Checked, checkBox_play_note4.Checked,
                    note_length);
                if (silence > 0)
                {
                    gcodeBuilder.AppendLine($"G4 P{silence}");
                }
            }
            string gcode = gcodeBuilder.ToString();
            return gcode;
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
                exportGCodeFile.Filter = "GCode Files (*.gcode)|*.gcode";
                DialogResult result = exportGCodeFile.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string filePath = exportGCodeFile.FileName;
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(notes);
                        writer.Close();
                    }
                    Logger.Log("GCode exported to " + filePath, Logger.LogTypes.Info);
                    MessageBox.Show(Resources.MessageGCodeExported, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                Logger.Log("No notes found to convert to GCode.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageGCodeEmptyNoteList, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private string GenerateGCodeForBuzzerNote(double frequency, int length)
        {
            return $"G4 P5\nM300 S{frequency} P{length}";
        }
        private string GenerateGCodeForMotorNote(int frequency, int length)
        {
            return $"G4 P5\nM3 S{FrequencyToRPM(frequency)} P{length}"; // Sample GCode format
        }
        private int FrequencyToRPM(double frequency)
        {
            return (int)(frequency * 60); // Convert frequency to RPM
        }
        private void insert_note_to_gcode(String note1, String note2, String note3, String note4,
            bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length) // Play note in a line
        {
            double note1_frequency = 0, note2_frequency = 0, note3_frequency = 0, note4_frequency = 0;
            String[] notes = new string[4];
            string Note1 = string.Empty, Note2 = string.Empty, Note3 = string.Empty, Note4 = string.Empty;
            if(play_note1)
            {
                Note1 = note1;
            }
            if(play_note2)
            {
                Note2 = note2;
            }
            if(play_note3)
            {
                Note3 = note3;
            }
            if(play_note4)
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
                            gcodeBuilder.AppendLine(GenerateGCodeForMotorNote((int)note1_frequency, length));
                            break;
                        case 1:
                            gcodeBuilder.AppendLine(GenerateGCodeForBuzzerNote((int)note1_frequency, length));
                            break;
                    }
                    return;
                }
                else if (notes[0] == note2)
                {
                    switch (comboBox_component_note1.SelectedIndex)
                    {
                        case 0:
                            gcodeBuilder.AppendLine(GenerateGCodeForMotorNote((int)note2_frequency, length));
                            break;
                        case 1:
                            gcodeBuilder.AppendLine(GenerateGCodeForBuzzerNote((int)note2_frequency, length));
                            break;
                    }
                    return;
                }
                else if (notes[0] == note3)
                {
                    switch (comboBox_component_note1.SelectedIndex)
                    {
                        case 0:
                            gcodeBuilder.AppendLine(GenerateGCodeForMotorNote((int)note3_frequency, length));
                            break;
                        case 1:
                            gcodeBuilder.AppendLine(GenerateGCodeForBuzzerNote((int)note3_frequency, length));
                            break;
                    }
                    return;
                }
                else if (notes[0] == note4)
                {
                    switch (comboBox_component_note1.SelectedIndex)
                    {
                        case 0:
                            gcodeBuilder.AppendLine(GenerateGCodeForMotorNote((int)note4_frequency, length));
                            break;
                        case 1:
                            gcodeBuilder.AppendLine(GenerateGCodeForBuzzerNote((int)note4_frequency, length));
                            break;
                    }
                    return;
                }
            }
            else if(notes.Length > 1)
            {
                string generatedGCode = string.Empty;
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
                        if (remainingLength >= alternate_length || willAnyNoteBeWritten == false)
                        {
                            int alternate_length_to_write = willAnyNoteBeWritten ? alternate_length : remainingLength;
                            double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);
                            switch (note_order)
                            {
                                case 1: // Note 1
                                    switch (comboBox_component_note1.SelectedIndex)
                                    {
                                        case 0: // Motor
                                            generatedGCode = GenerateGCodeForMotorNote((int)frequency, alternate_length_to_write);
                                            break;
                                        case 1: // Buzzer
                                            generatedGCode = GenerateGCodeForBuzzerNote((int)frequency, alternate_length_to_write);
                                            break;
                                    }
                                    break;

                                case 2: // Note 2
                                    switch (comboBox_component_note2.SelectedIndex)
                                    {
                                        case 0: // Motor
                                            generatedGCode = GenerateGCodeForMotorNote((int)frequency, alternate_length_to_write);
                                            break;
                                        case 1: // Buzzer
                                            generatedGCode = GenerateGCodeForBuzzerNote((int)frequency, alternate_length_to_write);
                                            break;
                                    }
                                    break;

                                case 3: // Note 3
                                    switch (comboBox_component_note3.SelectedIndex)
                                    {
                                        case 0: // Motor
                                            generatedGCode = GenerateGCodeForMotorNote((int)frequency, alternate_length_to_write);
                                            break;
                                        case 1: // Buzzer
                                            generatedGCode = GenerateGCodeForBuzzerNote((int)frequency, alternate_length_to_write);
                                            break;
                                    }
                                    break;

                                case 4: // Note 4
                                    switch (comboBox_component_note4.SelectedIndex)
                                    {
                                        case 0: // Motor
                                            generatedGCode = GenerateGCodeForMotorNote((int)frequency, alternate_length_to_write);
                                            break;
                                        case 1: // Buzzer
                                            generatedGCode = GenerateGCodeForBuzzerNote((int)frequency, alternate_length_to_write);
                                            break;
                                    }
                                    break;
                            }
                            note_order++;
                        }
                        else
                        {
                            generatedGCode = $"G4 P{remainingLength}";
                            gcodeBuilder.Append(generatedGCode + Environment.NewLine);
                            remainingLength -= remainingLength;
                            break;
                        }
                        gcodeBuilder.Append(generatedGCode + Environment.NewLine);
                        remainingLength -= (alternate_length + 5); // Subtract the length of the note and the delay
                    }
                }
                while (remainingLength > 0);
                return;
            }
            else
            {
                gcodeBuilder.AppendLine($"G4 P{length}");
                return;
            }
        }

        private double CalculateNoteLength(string noteType, string modifier = "", string articulation = "")
        {
            int milisecondsPerBeat = (int)Math.Floor(60000.0 / bpm);
            int baseLength = noteType switch
            {
                "Whole" => milisecondsPerBeat * 4,
                "Half" => milisecondsPerBeat * 2,
                "Quarter" => milisecondsPerBeat,
                "1/8" => milisecondsPerBeat / 2,
                "1/16" => milisecondsPerBeat / 4,
                "1/32" => milisecondsPerBeat / 8,
                _ => milisecondsPerBeat
            };
            switch (modifier)
            {
                case "Dot":
                    baseLength = (int)(baseLength * 1.5);
                    break;
                case "Tri":
                    baseLength /= 3;
                    break;
                default:
                    break;
            }
            switch (articulation)
            {
                case "Sta":
                    baseLength /= 2;
                    break;
                case "Spi":
                    baseLength /= 4;
                    break;
                case "Fer":
                    baseLength *= 2;
                    break;
                default:
                    break;
            }
            return baseLength;
        }
        private double CalculateLineLength(string noteType, string modifier = "", string articulation = "")
        {
            int milisecondsPerBeat = (int)Math.Floor(60000.0 / bpm);
            int baseLength = noteType switch
            {
                "Whole" => milisecondsPerBeat * 4,
                "Half" => milisecondsPerBeat * 2,
                "Quarter" => milisecondsPerBeat,
                "1/8" => milisecondsPerBeat / 2,
                "1/16" => milisecondsPerBeat / 4,
                "1/32" => milisecondsPerBeat / 8,
                _ => milisecondsPerBeat
            };
            switch (modifier)
            {
                case "Dot":
                    baseLength = (int)(baseLength * 1.5);
                    break;
                case "Tri":
                    baseLength /= 3;
                    break;
            }
            if (articulation == "Fer")
            {
                baseLength *= 2;
            }
            return baseLength;
        }

        private void ConvertToGCode_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}
