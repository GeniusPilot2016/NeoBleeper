// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
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
        ComponentTypes note1Component = 0;
        ComponentTypes note2Component = 0;
        ComponentTypes note3Component = 0;
        ComponentTypes note4Component = 0;
        String musicString;
        int alternateLength = 30;
        int bpm = 120; // Default BPM
        int noteSilenceRatio = 50;
        bool nonStopping = false;

        // Enum for different firmware types
        private enum FirmwareTypes
        {
            Marlin,
            GRBL,
            Repetier,
            Smoothieware,
            Klipper,
            RepRapFirmware,
            TinyG,
            LinuxCNC,
            Mach3,
            Mach4,
            FlashForge,
            Sailfish,
            Duet,
            PathPilot,
            FluidNC,
            GrblHAL,
            Smoothieboard,
            MakerBot,
            Snapmaker,
            OctoPrint,
            PrusaFirmware,
            Creality,
            Other
        }

        // Enum for different component types
        private enum ComponentTypes
        {
            Motor,
            Buzzer
        }

        private enum SelectedAxis
        {
            X,
            Y,
            Z
        }

        private SelectedAxis selectedAxis = SelectedAxis.X;

        // Constant values for unit conversions
        public const int MillimetersPerMinuteToMillimetersPerSecond = 60; // 1 mm/s = 60 mm/minute
        public const double InchesPerMinuteToMillimetersPerMinute = 25.4; // 1 inch/minute = 25.4 mm/minute
        public const int SecondsPerMinute = 60;

        public ConvertToGCode(String musicFile, Form owner)
        {
            InitializeComponent();
            this.Owner = owner;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
            SetTheme();
            musicString = musicFile;
            comboBoxFirmware.SelectedIndex = (int)FirmwareTypes.Marlin;
            comboBox_component_note1.SelectedIndex = (int)ComponentTypes.Motor;
            comboBox_component_note2.SelectedIndex = (int)ComponentTypes.Motor;
            comboBox_component_note3.SelectedIndex = (int)ComponentTypes.Motor;
            comboBox_component_note4.SelectedIndex = (int)ComponentTypes.Motor;
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
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            label_note1.ForeColor = Color.White;
            label_note2.ForeColor = Color.White;
            label_note3.ForeColor = Color.White;
            label_note4.ForeColor = Color.White;
            button_export_as_gcode.BackColor = Color.FromArgb(32, 32, 32);
            comboBoxFirmware.BackColor = Color.Black;
            comboBoxFirmware.ForeColor = Color.White;
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
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            label_note1.ForeColor = SystemColors.ControlText;
            label_note2.ForeColor = SystemColors.ControlText;
            label_note3.ForeColor = SystemColors.ControlText;
            label_note4.ForeColor = SystemColors.ControlText;
            button_export_as_gcode.BackColor = Color.Transparent;
            comboBoxFirmware.BackColor = SystemColors.Window;
            comboBoxFirmware.ForeColor = SystemColors.WindowText;
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

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method selects and applies a light or dark theme according to the user's theme
        /// preference or the system's theme setting. It also enables double buffering to improve rendering performance
        /// and ensures that all UI changes are applied immediately. This method should be called when the theme needs
        /// to be updated, such as after a settings change.</remarks>
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
        StringBuilder gcodeBuilder = new StringBuilder();
        int elapsedLineTime = 0; // Equivalent of Stopwatch.ElapsedMilliseconds for text based timing

        /// <summary>
        /// Parses a music project string and generates a G-code representation of its note sequence.
        /// </summary>
        /// <remarks>The input string is expected to be in a specific XML format representing a music
        /// project. The generated G-code encodes note timing and silence based on project settings such as BPM and note
        /// silence ratio. If the input does not contain any valid notes, the result will be an empty string.</remarks>
        /// <param name="musicString">A string containing the serialized music project data to be parsed. Must not be null or empty.</param>
        /// <returns>A string containing the generated G-code for the notes in the music project. Returns an empty string if no
        /// notes are found or if the input is invalid.</returns>
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
                    noteSilenceRatio = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio);
                    alternateLength = Convert.ToInt32(projectFile.Settings.RandomSettings.AlternateTime);
                    nonStopping = Convert.ToInt32(projectFile.Settings.RandomSettings.NoteSilenceRatio) == 100;
                    if (string.IsNullOrEmpty(projectFile.Settings.RandomSettings.NoteSilenceRatio))
                        noteSilenceRatio = 50;
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

            foreach (var note in notes)
            {
                double noteDuration = NoteLengths.CalculateLineLength(bpm, note.Length, note.Mod, note.Art);
                double rawNoteLength = NoteLengths.CalculateNoteLength(bpm, note.Length, note.Mod, note.Art) * (noteSilenceRatio / 100.0);
                int noteLength = (int)Math.Truncate(rawNoteLength);
                int silence = (int)Math.Truncate(noteDuration - rawNoteLength);
                int drift = 0;
                if (drift > 0)
                {
                    if (drift < noteLength)
                    {
                        noteLength -= drift; // Reduce note length by drift amount
                    }
                    else
                    {
                        drift -= noteLength; // Skip note length if drift is larger
                        continue;
                    }
                }
                // Add GCode line
                elapsedLineTime = InsertNoteToGCode(note.Note1, note.Note2, note.Note3, note.Note4,
                    true, true, true, true, noteLength);
                if (drift < 0)
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

        /// <summary>
        /// Deserializes an XML representation of a NeoBleeper project file from the specified string reader.
        /// </summary>
        /// <param name="stringReader">A StringReader containing the XML data to deserialize. Cannot be null.</param>
        /// <returns>A NeoBleeperProjectFile object deserialized from the XML data, or null if the XML does not represent a valid
        /// NeoBleeper project file.</returns>
        private NBPMLFile.NeoBleeperProjectFile? DeserializeXMLFromString(StringReader stringReader)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NBPMLFile.NeoBleeperProjectFile));
            return (NBPMLFile.NeoBleeperProjectFile)serializer.Deserialize(stringReader);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            note1Component = (ComponentTypes)comboBox_component_note1.SelectedIndex;
        }

        /// <summary>
        /// Enables or disables the Export as GCode button based on the selection state of note checkboxes.
        /// </summary>
        /// <remarks>The Export as GCode button is enabled only when at least one note checkbox is
        /// selected. This method should be called whenever the selection state of the note checkboxes changes to ensure
        /// the button's enabled state reflects the current selection.</remarks>
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
            String notes = ExtractNotes(musicString);
            if (!string.IsNullOrEmpty(notes))
            {
                MainWindow.SetFallbackInitialFolderForSaveFileDialog(exportGCodeFile);
                if (exportGCodeFile.ShowDialog(this) == DialogResult.OK)
                {
                    string filePath = exportGCodeFile.FileName;
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine(notes);
                        writer.Close();
                    }
                    Logger.Log("GCode exported to " + filePath, Logger.LogTypes.Info);
                    MessageForm.Show(this, Resources.MessageGCodeExported, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                Logger.Log("No notes found to convert to GCode.", Logger.LogTypes.Error);
                MessageForm.Show(this, Resources.MessageGCodeEmptyNoteList, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox_component_note2_SelectedIndexChanged(object sender, EventArgs e)
        {
            note2Component = (ComponentTypes)comboBox_component_note2.SelectedIndex;
        }

        private void comboBox_component_note3_SelectedIndexChanged(object sender, EventArgs e)
        {
            note3Component = (ComponentTypes)comboBox_component_note3.SelectedIndex;
        }

        private void comboBox_component_note4_SelectedIndexChanged(object sender, EventArgs e)
        {
            note4Component = (ComponentTypes)comboBox_component_note4.SelectedIndex;
        }

        // List of firmware types that support the M300 command
        private FirmwareTypes[] M300SupportedFirmwares = {
                FirmwareTypes.Marlin,
                FirmwareTypes.Repetier,
                FirmwareTypes.Smoothieware,
                FirmwareTypes.Klipper,
                FirmwareTypes.RepRapFirmware
            };

        /// <summary>
        /// Generates a G-code command sequence to play a buzzer note at the specified frequency and duration.
        /// </summary>
        /// <param name="frequency">The frequency of the buzzer note, in hertz. Must be a positive value.</param>
        /// <param name="length">The duration of the note, in milliseconds. Must be a non-negative integer.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, omits the additional delay after the note; otherwise, includes a 5 ms
        /// delay to ensure proper timing.</param>
        /// <returns>A tuple containing the generated G-code command string and the total duration in milliseconds. The duration
        /// includes an additional 5 ms if <paramref name="nonStopping"/> is <see langword="false"/>.</returns>
        private (string output, int length) GenerateGCodeForBuzzerNote(double frequency, int length, bool nonStopping = false)
        {
            if (M300SupportedFirmwares.Contains((FirmwareTypes)comboBoxFirmware.SelectedIndex))
            {
                string delay = nonStopping ? "G4 P5\n" : string.Empty;
                return ($"G4 P5\nM300 S{frequency} P{length}", nonStopping ? length : length + 5);
            }
            else
            {
                // Fallback with silence if M300 is not supported by the selected firmware
                return ($"G4 P{length}", length);
            }
        }

        /// <summary>
        /// Generates a G-code command sequence to control a motor for a specified duration and frequency using G0/G1 movements.
        /// </summary>
        /// <param name="frequency">The frequency, in hertz, at which the motor should operate. Must be a positive integer.</param>
        /// <param name="length">The duration, in milliseconds, for which the motor should run. Must be a non-negative integer.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, the generated G-code will not include a stop command after the specified
        /// duration; otherwise, the motor will be stopped at the end of the sequence. The default is <see langword="false"/>.</param>
        /// <returns>A tuple containing the generated G-code command sequence as a string and the total duration in milliseconds.</returns>
        private (string output, int length) GenerateGCodeForMotorNote(int frequency, int length, bool nonStopping = false)
        {
            // Validate input parameters
            if (frequency <= 0 || length <= 0)
            {
                return ($"G4 P{length}", length);
            }

            // Get axis letter based on selected axis
            string axisLetter = "X";
            switch(selectedAxis)
            {
                case SelectedAxis.X:
                    axisLetter = "X";
                    break;
                case SelectedAxis.Y:
                    axisLetter = "Y";
                    break;
                case SelectedAxis.Z:
                    axisLetter = "Z";
                    break;
                default:
                    axisLetter = "X";
                    break;
            }

            // Get selected firmware type
            FirmwareTypes selectedFirmware = (FirmwareTypes)comboBoxFirmware.SelectedIndex;

            // Determine if firmware uses inches (G20) or millimeters (G21)
            bool usesInches = IsInchBasedFirmware(selectedFirmware);

            // Parameters
            double targetFrequency = Math.Max(1, frequency); // Hz
            int durationMs = Math.Max(0, length);

            // Calculate number of cycles needed
            double periodSeconds = 1.0 / targetFrequency; // one complete cycle (seconds)
            double totalSeconds = durationMs / 1000.0;
            int cycles = (int)Math.Floor(totalSeconds / periodSeconds);

            if (cycles <= 0)
            {
                // Duration is shorter than one period, just add a delay
                return ($"G4 P{length}", length);
            }

            // Base travel distance in mm
            double travelMm = 0.1; // 0.1 mm starting value

            // Convert to inches if needed
            double travelDistance = usesInches ? (travelMm / InchesPerMinuteToMillimetersPerMinute) : travelMm;

            // Calculate time per movement (two movements = one cycle: forward + backward)
            double secondsPerMove = periodSeconds / 2.0; // seconds
            double minutesPerMove = secondsPerMove / 60.0; // minutes

            // Calculate feed rate (distance/time)
            double feed = travelDistance / minutesPerMove;

            // Apply safety limits based on firmware and units
            double maxFeed = usesInches ? 2000.0 : 60000.0; // inches/min or mm/min
            if (feed > maxFeed)
            {
                // Reduce travel distance to bring feed within limits
                travelDistance = Math.Max(usesInches ? 0.0004 : 0.01, (maxFeed * minutesPerMove));
                feed = travelDistance / minutesPerMove;
            }

            // Build G-code sequence
            var sb = new StringBuilder();

            // Set unit mode based on firmware
            if (usesInches)
            {
                sb.AppendLine("G20"); // Set units to inches
            }
            else
            {
                sb.AppendLine("G21"); // Set units to millimeters
            }

            // Switch to incremental positioning for easier +/- movements
            sb.AppendLine("G91");

            // Optional initial delay for compatibility
            if (!nonStopping)
                sb.AppendLine("G4 P5");

            // Generate movement cycles
            for (int c = 0; c < cycles; c++)
            {
                // Forward movement
                sb.AppendLine($"G1 {axisLetter}{travelDistance.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)} F{Math.Round(feed)}");
                // Backward movement
                sb.AppendLine($"G1 {axisLetter}-{travelDistance.ToString("F4", System.Globalization.CultureInfo.InvariantCulture)} F{Math.Round(feed)}");
            }

            // Return to absolute positioning
            sb.AppendLine("G90");

            // Final delay for compatibility
            if (!nonStopping)
                sb.AppendLine("G4 P5");

            int totalLengthReported = nonStopping ? durationMs : durationMs + 5;
            return (sb.ToString(), totalLengthReported);
        }

        /// <summary>
        /// Determines if the specified firmware type uses inches as default units instead of millimeters.
        /// </summary>
        /// <param name="firmware">The firmware type to check.</param>
        /// <returns>True if the firmware typically uses inches; false if it uses millimeters.</returns>
        private bool IsInchBasedFirmware(FirmwareTypes firmware)
        {
            // Firmware types that commonly use or support inches
            return firmware switch
            {
                FirmwareTypes.Mach3 => true,
                FirmwareTypes.Mach4 => true,
                FirmwareTypes.LinuxCNC => false, // LinuxCNC genellikle mm kullanır, ama her ikisini de destekler
                FirmwareTypes.TinyG => false, // TinyG varsayılan mm'dir
                FirmwareTypes.GRBL => false, // GRBL varsayılan mm'dir
                _ => false // Diğer tüm firmware'ler mm kullanır
            };
        }

        /// <summary>
        /// Generates and inserts G-code instructions to play up to four musical notes, using the specified note values,
        /// play flags, and duration.
        /// </summary>
        /// <remarks>If multiple notes are selected to play, the method alternates between them according
        /// to the current alternation mode. Notes that are not selected (with their play flag set to false) or are
        /// empty are ignored. If no notes are selected, a G-code delay command is inserted for the specified
        /// duration.</remarks>
        /// <param name="note1">The name of the first note to play. This should be a valid note name recognized by the system (e.g., "C4",
        /// "A#3").</param>
        /// <param name="note2">The name of the second note to play. This should be a valid note name recognized by the system.</param>
        /// <param name="note3">The name of the third note to play. This should be a valid note name recognized by the system.</param>
        /// <param name="note4">The name of the fourth note to play. This should be a valid note name recognized by the system.</param>
        /// <param name="playNote1">true to play the first note; otherwise, false.</param>
        /// <param name="playNote2">true to play the second note; otherwise, false.</param>
        /// <param name="playNote3">true to play the third note; otherwise, false.</param>
        /// <param name="playNote4">true to play the fourth note; otherwise, false.</param>
        /// <param name="length">The total duration, in milliseconds, for which the notes should be played. Must be a non-negative integer.</param>
        /// <returns>The total elapsed time, in milliseconds, corresponding to the generated G-code instructions for the
        /// specified notes and duration.</returns>
        private int InsertNoteToGCode(String note1, String note2, String note3, String note4,
        bool playNote1, bool playNote2, bool playNote3, bool playNote4, int length) // Play note in a line
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
                    switch (comboBox_component_note1.SelectedIndex)
                    {
                        case 0:
                            var motorNote = GenerateGCodeForMotorNote((int)note1Frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForBuzzerNote((int)note1Frequency, length, nonStopping);
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
                            var motorNote = GenerateGCodeForMotorNote((int)note2Frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForBuzzerNote((int)note2Frequency, length, nonStopping);
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
                            var motorNote = GenerateGCodeForMotorNote((int)note3Frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForBuzzerNote((int)note3Frequency, length, nonStopping);
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
                            var motorNote = GenerateGCodeForMotorNote((int)note4Frequency, length, nonStopping);
                            gcodeBuilder.AppendLine(motorNote.output);
                            elapsedTime = motorNote.length;
                            break;
                        case 1:
                            var buzzerNote = GenerateGCodeForBuzzerNote((int)note4Frequency, length, nonStopping);
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

                        if (remainingLength >= alternateLength)
                        {
                            willAnyNoteBeWritten = true;
                        }

                        if (remainingLength >= alternateLength || willAnyNoteBeWritten == false)
                        {
                            int alternate_length_to_write = willAnyNoteBeWritten ? alternateLength : remainingLength;
                            double frequency = NoteFrequencies.GetFrequencyFromNoteName(note);

                            int currentDuration = 0;
                            string generatedGCode = string.Empty;

                            switch (note_order)
                            {
                                case 1: // Note 1
                                    if (comboBox_component_note1.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note1Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note1Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                case 2: // Note 2
                                    if (comboBox_component_note2.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note2Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note2Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                case 3: // Note 3
                                    if (comboBox_component_note3.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note3Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note3Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = buz.output;
                                        currentDuration = buz.length;
                                    }
                                    break;

                                case 4: // Note 4
                                    if (comboBox_component_note4.SelectedIndex == 0)
                                    {
                                        var motor = GenerateGCodeForMotorNote((int)note4Frequency, alternate_length_to_write, nonStopping);
                                        generatedGCode = motor.output;
                                        currentDuration = motor.length;
                                    }
                                    else
                                    {
                                        var buz = GenerateGCodeForBuzzerNote((int)note4Frequency, alternate_length_to_write, nonStopping);
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
            SetTheme();
        }

        private void comboBoxFirmware_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Disable component selection if firmware does not support it
            FirmwareTypes selectedFirmware = (FirmwareTypes)comboBoxFirmware.SelectedIndex;
            // Check if selected firmware supports M300 command
            if (!M300SupportedFirmwares.Contains(selectedFirmware))
            {
                // Set all component selections to Motor and disable them
                comboBox_component_note1.SelectedIndex = (int)ComponentTypes.Motor;
                comboBox_component_note2.SelectedIndex = (int)ComponentTypes.Motor;
                comboBox_component_note3.SelectedIndex = (int)ComponentTypes.Motor;
                comboBox_component_note4.SelectedIndex = (int)ComponentTypes.Motor;
                comboBox_component_note1.Enabled = false;
                comboBox_component_note2.Enabled = false;
                comboBox_component_note3.Enabled = false;
                comboBox_component_note4.Enabled = false;
            }
            else
            {
                // Enable component selection
                comboBox_component_note1.Enabled = true;
                comboBox_component_note2.Enabled = true;
                comboBox_component_note3.Enabled = true;
                comboBox_component_note4.Enabled = true;
            }
        }

        private void AxisRadioButtonsCheckedChanged(object sender, EventArgs e)
        {
            if (sender == radioButtonX)
            {
                selectedAxis = SelectedAxis.X;
            }
            else if (sender == radioButtonY)
            {
                selectedAxis = SelectedAxis.Y;
            }
            else if (sender == radioButtonZ)
            {
                selectedAxis = SelectedAxis.Z;
            }
            else
            {
                selectedAxis = SelectedAxis.X;
            }
        }
    }
}
