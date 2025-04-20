using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class ConvertToGCode : Form
    {
        int note1_component = 0;
        int note2_component = 0;
        int note3_component = 0;
        int note4_component = 0;
        String MusicString = "";
        int milisecondsPerBeat = 0; 
        public ConvertToGCode(String musicFile)
        {
            InitializeComponent();
            comboBox_component_note1.SelectedIndex = 0;
            comboBox_component_note2.SelectedIndex = 0;
            comboBox_component_note3.SelectedIndex = 0;
            comboBox_component_note4.SelectedIndex = 0;
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
            exportGCodeFile.Filter = "GCode Files (*.gcode)|*.gcode";
            DialogResult result = exportGCodeFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                //Export the GCode to the file
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
        private string GenerateGCodeForNote(double frequency, int length)
        {
            return $"M300 S{frequency} P{length}"; // Sample GCode format
        }

        private void play_note_in_line_as_gcode(bool play_note1, bool play_note2, bool play_note3, bool play_note4, int length)
        {
            string gcode = string.Empty;

            if (play_note1)
            {
                gcode += GenerateGCodeForNote(440, length); // Sample frequency for note 1
            }
            if (play_note2)
            {
                gcode += GenerateGCodeForNote(494, length);
            }
            if (play_note3)
            {
                gcode += GenerateGCodeForNote(523, length);
            }
            if (play_note4)
            {
                gcode += GenerateGCodeForNote(587, length);
            }

            // Write the GCode to a file or debug output
            Console.WriteLine(gcode);
        }
        private int CalculateNoteLength(string noteType, string modifier)
        {
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

            if (modifier == "Dot")
            {
                baseLength = (int)(baseLength * 1.5);
            }
            else if (modifier == "Tri")
            {
                baseLength /= 3;
            }

            return baseLength;
        }
        private int CalculateLineLength(string noteType, string modifier)
        {
            return CalculateNoteLength(noteType, modifier); // Same logic for line length
        }
        private void ParseFileToGCode(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                // Parse the line to extract note type and modifier
                string[] parts = line.Split(' ');
                string noteType = parts[0];
                string modifier = parts.Length > 1 ? parts[1] : string.Empty;

                int length = CalculateNoteLength(noteType, modifier);
                string gcode = GenerateGCodeForNote(440, length); // Sample frequency

                // Write the GCode to a file or debug output
                File.AppendAllText("output.gcode", gcode + Environment.NewLine);
            }
        }

    }
}
