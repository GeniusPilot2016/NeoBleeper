﻿using GenerativeAI.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GenerativeAI;
using System.Text.RegularExpressions;
using System.Drawing.Text;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace NeoBleeper
{
    public partial class CreateMusicWithAI : Form
    {
        public string output = "";
        String AIModel = "models/gemini-2.5-flash";
        Size NormalWindowSize;
        double scaleFraction = 0.425; // Scale factor for the window size
        Size LoadingWindowSize;
        public CreateMusicWithAI()
        {
            InitializeComponent();
            NormalWindowSize = this.Size;
            LoadingWindowSize = new Size(NormalWindowSize.Width, (int)(NormalWindowSize.Height+(NormalWindowSize.Height * scaleFraction)));
            UIFonts.setFonts(this);
            set_theme();
            comboBox_ai_model.SelectedIndex = Settings1.Default.preferredAIModel;
            ApplyAIModelChanges();
            if (!IsInternetAvailable())
            {
                Debug.WriteLine("Internet connection is not available. Please check your connection.");
                MessageBox.Show("Internet connection is not available. Please check your connection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else if (string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
            {
                Debug.WriteLine("Google Gemini™ API key is not set. Please set the API key in the \"General\" tab in settings.");
                MessageBox.Show("Google Gemini™ API key is not set. Please set the API key in the \"General\" tab in settings.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
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
            this.BackColor = Color.FromArgb(32, 32, 32);
            buttonCreate.BackColor = Color.FromArgb(32, 32, 32);
            buttonCreate.ForeColor = Color.White;
            textBoxPrompt.BackColor = Color.Black;
            textBoxPrompt.ForeColor = Color.White;
            comboBox_ai_model.BackColor = Color.Black;
            comboBox_ai_model.ForeColor = Color.White;
            this.ForeColor = Color.White;
        }
        private void light_theme()
        {
            this.BackColor = SystemColors.Control;
            buttonCreate.BackColor = Color.Transparent;
            buttonCreate.ForeColor = SystemColors.ControlText;
            textBoxPrompt.BackColor = SystemColors.Window;
            textBoxPrompt.ForeColor = SystemColors.WindowText;
            comboBox_ai_model.BackColor = SystemColors.Window;
            comboBox_ai_model.ForeColor = SystemColors.WindowText;
            this.ForeColor = SystemColors.ControlText;
        }
        private bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("www.google.com");
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SetControlsEnabledAndMakeLoadingVisible(false);
                var apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                var googleAI = new GoogleAi(apiKey);
                var googleModel = googleAI.CreateGenerativeModel(AIModel);
                var googleResponse = await googleModel.GenerateContentAsync($"**User Prompt:\r\n[{textBoxPrompt.Text}]**\r\n\r\n" +
                    $"--- AI Instructions ---\r\n" +
                    $"Based on the User Prompt above, generate a sequence of <Line> elements to replace the placeholder section within the <LineList> below.\r\n" +
                    $"- PRIORITY 1: Ensure the output is a complete and valid <NeoBleeperProjectFile> XML structure. Do not include any additional text, code block markers (```xml), or explanations. Use UTF-8 encoding.\r\n" +
                    $"- Adhere to the XML structure provided.\r\n" +
                    $"- Each <Line> represents a musical event or rest.\r\n" +
                    $"- <Length> defines the duration. Use a variety of durations from the following: Whole, Half, Quarter, 1/8, 1/16, and 1/32. Vary the frequency of each duration\r\n" +
                    $"- <Note1>, <Note2>, <Note3>, <Note4> hold the notes (e.g., C4, G#5, F6). Use empty tags (e.g., <Note3 />) only when creating intentional musical rests. Prioritize filling at least one note channel per line to create musical content.\r\n" +
                    $"- <Mod /> defines the modulation (e.g Dot, Tri). Use empty tags (e.g., <Mod />) if there's no modulation in that voice/slot.\r\n" +
                    $"- <Art /> defines the articulation(e.g Sta, Spi, Fer). Use empty tags (e.g., <Art />) if there's no articulation in that voice/slot.\r\n" +
                    $"- Use the <Settings> provided as context for parameters like BPM and Time Signature, unless the prompt overrides them.\r\n" +
                    $"- Use only valid XML characters and escape special characters (&lt;, &gt;, &amp;, &apos;, &quot;) correctly.\r\n" +
                    $"- Generate music with a duration between 10 and 180 seconds.\r\n" +
                    $"- Introduce more variations in melody, harmony, and rhythm.\r\n" +
                    $"- Vary the BPM between 40 and 600.\r\n" +
                    $"- Use different time signatures (e.g., 3/4, 6/8, 4/4) randomly.\r\n" +
                    $"- **Note Silence Ratio:**\r\n" +
                    $"- For normal music generation, use NoteSilenceRatio, which is ratio of note and silence in each line, between 5-100 to ensure adequate musical content.\r\n" +
                    $"- If the user's prompt contains keywords like 'sparse', 'minimal', 'quiet', or 'ambient', increase the `NoteSilenceRatio` towards 70.\r\n" +
                    $"- If the user's prompt contains keywords like 'dense', 'full', 'rich', or 'continuous', decrease the `NoteSilenceRatio` towards 10.\r\n" +
                    $"- If the user prompt contains a number, use that number to set the `<NoteSilenceRatio>` value.\r\n" +
                    $"- If no number is given, use a default value of 95 for the NoteSilenceRatio.\r\n" +
                    $"- The NoteSilenceRatio value represents the percentage of time that silence occurs, versus a note being played.\r\n" +
                    $"- Create musical rests (lines with all empty note tags) sparingly, only when musically appropriate.\r\n" +
                    $"- Ensure most lines contain notes in at least one voice channel to maintain musical flow.\r\n" +
                    $"- Randomly vary the `NoteLength` between 1 and 5.\r\n" +
                    $"- Include single-voice sections in the generated music.\r\n" +
                    $"- Distribute notes randomly across all four voices.\r\n" +
                    $"- Vary the combinations of voices used in chords.\r\n" +
                    $"- Use Note 1, Note 2, Note 3 and Note 4 channels randomly.\r\n" +
                    $"- Limit the range of note durations to avoid extreme variations.\r\n" +
                    $"- Control the randomness of note selection to create more coherent melodies.\r\n" +
                    $"- **Ensure the generated XML includes a randomly chosen <AlternateTime> value (e.g., between 5 and 200) within the <RandomSettings> section for alternating notes that switches " +
                    $"between Note 1, Note 2, Note 3 and Note 4, and " +
                    $"<AlternateTime> should be shorter as possible to achieve polyphony in system speaker by fitting the context of the music " +
                    $"[because the system speaker (PC Speaker) located on the motherboard of the computer and used to make the computer only beep can only play one note at a time.].**\r\n" +
                    $"--- NeoBleeperProjectFile Template ---" +
                    $"\r\n<NeoBleeperProjectFile>\r\n    <Settings>\r\n        <RandomSettings>\r\n            <KeyboardOctave>5</KeyboardOctave>\r\n            <BPM>120</BPM>\r\n            " +
                    $"<TimeSignature>4</TimeSignature>\r\n            <NoteSilenceRatio>95</NoteSilenceRatio>\r\n            <NoteLength>3</NoteLength>\r\n            " +
                    $"<AlternateTime>5</AlternateTime>\r\n        </RandomSettings>\r\n        <PlaybackSettings>\r\n            <NoteClickPlay>True</NoteClickPlay>\r\n            " +
                    $"<NoteClickAdd>True</NoteClickAdd>\r\n            <AddNote1>False</AddNote1>\r\n            <AddNote2>False</AddNote2>\r\n            <AddNote3>True</AddNote3>\r\n            " +
                    $"<AddNote4>False</AddNote4>\r\n            <NoteReplace>True</NoteReplace>\r\n            <NoteLengthReplace>False</NoteLengthReplace>\r\n        </PlaybackSettings>\r\n        " +
                    $"<ClickPlayNotes>\r\n            <ClickPlayNote1>True</ClickPlayNote1>\r\n            <ClickPlayNote2>True</ClickPlayNote2>\r\n            <ClickPlayNote3>True</ClickPlayNote3>\r\n            " +
                    $"<ClickPlayNote4>True</ClickPlayNote4>\r\n        </ClickPlayNotes>\r\n        <PlayNotes>\r\n            <PlayNote1>True</PlayNote1>\r\n            <PlayNote2>True</PlayNote2>\r\n            " +
                    $"<PlayNote3>True</PlayNote3>\r\n            <PlayNote4>True</PlayNote4>\r\n        </PlayNotes>\r\n    </Settings>\r\n    <LineList>\r\n    </LineList>\r\n</NeoBleeperProjectFile>\r\n\r\n" +
                    $"--- END OF MUSIC GENERATION TEMPLATE ---");
                if (googleResponse != null)
                {
                    output = googleResponse.Text();

                    // Remove ```xml and any surrounding text
                    output = Regex.Replace(output, @"^\s*```xml\s*", String.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*```\s*$", String.Empty);
                    output = Regex.Replace(output, @"\s*1\s*$", "Whole", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*1/2\s*$", "Half", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*1/4\s*$", "Quarter", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Eighth\s*$", "1/8", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Sixteenth\s*$", "1/16", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Thirty-second\s*$", "1/32", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Thirty Second\s*$", "1/32", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Db\s*$", "C#", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Eb\s*$", "D#", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Gb\s*$", "F#", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Ab\s*$", "G#", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Bb\s*$", "A#", RegexOptions.IgnoreCase);
                    // Trim leading/trailing whitespace
                    output = output.Trim();
                    output = RewriteOutput(output).Trim();
                }
                else
                {
                    Debug.WriteLine("AI response is null or empty.");
                    MessageBox.Show("AI response is null or empty.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {
                SetControlsEnabledAndMakeLoadingVisible(true);
                if(checkIfOutputIsValidNBPML(output))
                {
                    this.Close();
                }
                else
                {
                    // If the output is not valid, show an error message
                    AIGeneratedNBPMLError errorForm = new AIGeneratedNBPMLError(output);
                    errorForm.ShowDialog();
                    output = String.Empty; // Clear the output if it's invalid
                }
            }
        }
        private bool checkIfOutputIsValidNBPML(String output)
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                return false;
            }
            bool isValidXml = false;
            bool isValidNBPML = false;
            // Check if the output is valid XML
            try
            {
                var xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(output);
                isValidXml = true;
            }
            catch (System.Xml.XmlException)
            {
                isValidXml = false;
            }
            // Check if the output starts with <NeoBleeperProjectFile> and ends with </NeoBleeperProjectFile>
            if (!output.StartsWith("<NeoBleeperProjectFile>") ||
                !output.EndsWith("</NeoBleeperProjectFile>"))
            {
                isValidNBPML = false;
            }
            else
            {
                isValidNBPML = true;
            }
            // Check for the presence of <LineList> and <Line> elements
            if (!output.Contains("<LineList>") || !output.Contains("<Line>"))
            {
                isValidNBPML = false;
            }
            else
            {
                isValidNBPML = true;
            }
            // Additional checks can be added here as needed
            return isValidNBPML && isValidXml;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxPrompt.Text))
            {
                buttonCreate.Enabled = true;
            }
            else
            {
                buttonCreate.Enabled = false;
            }
        }
        private string RewriteOutput(string output)
        {
            // Apply additional transformations to ensure NBPM format compliance
            output = Regex.Replace(output, @"<AlternateTime>\d+</AlternateTime>", "<AlternateTime>5</AlternateTime>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"<NoteSilenceRatio>\d+</NoteSilenceRatio>", "<NoteSilenceRatio>95</NoteSilenceRatio>", RegexOptions.IgnoreCase);

            // Ensure <NeoBleeperProjectFile> starts and ends correctly
            if (!output.StartsWith("<NeoBleeperProjectFile>"))
            {
                output = "<NeoBleeperProjectFile>\r\n" + output;
            }
            if (!output.EndsWith("</NeoBleeperProjectFile>"))
            {
                output += "\r\n</NeoBleeperProjectFile>";
            }

            // Ensure <LineList> section exists
            if (!output.Contains("<LineList>"))
            {
                output = output.Replace("</Settings>", "</Settings>\r\n<LineList>\r\n</LineList>");
            }

            // Trim leading/trailing whitespace
            output = FixParameterNames(output);
            output = SynchronizeLengths(output);
            // Remove XML declaration
            output = Regex.Replace(output, @"<\?xml.*?\?>", String.Empty, RegexOptions.IgnoreCase);
            output = output.Trim();

            return output;
        }
        private string FixParameterNames(string xmlContent)
        {
            // Fix all parameter names according to Clementi Sonatina No. 3, Op 36.NBPML syntax
            xmlContent = Regex.Replace(xmlContent, @"<length>", "<Length>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</length>", "</Length>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<note1>", "<Note1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</note1>", "</Note1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<note2>", "<Note2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</note2>", "</Note2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<note3>", "<Note3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</note3>", "</Note3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<note4>", "<Note4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</note4>", "</Note4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<mod>", "<Mod>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</mod>", "</Mod>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<art>", "<Art>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</art>", "</Art>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<keyboardoctave>", "<KeyboardOctave>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</keyboardoctave>", "</KeyboardOctave>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<bpm>", "<BPM>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</bpm>", "</BPM>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<timesignature>", "<TimeSignature>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</timesignature>", "</TimeSignature>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<notesilenceratio>", "<NoteSilenceRatio>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</notesilenceratio>", "</NoteSilenceRatio>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<notelength>", "<NoteLength>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</notelength>", "</NoteLength>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<alternatetime>", "<AlternateTime>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</alternatetime>", "</AlternateTime>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<noteclickplay>", "<NoteClickPlay>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</noteclickplay>", "</NoteClickPlay>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<noteclickadd>", "<NoteClickAdd>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</noteclickadd>", "</NoteClickAdd>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<addnote1>", "<AddNote1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</addnote1>", "</AddNote1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<addnote2>", "<AddNote2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</addnote2>", "</AddNote2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<addnote3>", "<AddNote3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</addnote3>", "</AddNote3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<addnote4>", "<AddNote4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</addnote4>", "</AddNote4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<notereplace>", "<NoteReplace>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</notereplace>", "</NoteReplace>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<notelengthreplace>", "<NoteLengthReplace>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</notelengthreplace>", "</NoteLengthReplace>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<clickplaynote1>", "<ClickPlayNote1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</clickplaynote1>", "</ClickPlayNote1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<clickplaynote2>", "<ClickPlayNote2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</clickplaynote2>", "</ClickPlayNote2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<clickplaynote3>", "<ClickPlayNote3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</clickplaynote3>", "</ClickPlayNote3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<clickplaynote4>", "<ClickPlayNote4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</clickplaynote4>", "</ClickPlayNote4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<playnote1>", "<PlayNote1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</playnote1>", "</PlayNote1>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<playnote2>", "<PlayNote2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</playnote2>", "</PlayNote2>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<playnote3>", "<PlayNote3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</playnote3>", "</PlayNote3>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"<playnote4>", "<PlayNote4>", RegexOptions.IgnoreCase);
            xmlContent = Regex.Replace(xmlContent, @"</playnote4>", "</PlayNote4>", RegexOptions.IgnoreCase);

            return xmlContent;
        }
        private string SynchronizeLengths(string xmlContent)
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var lineNodes = xmlDoc.SelectNodes("//Line[@Length]");
            foreach (System.Xml.XmlNode lineNode in lineNodes)
            {
                var lengthAttribute = lineNode.Attributes["Length"];
                if (lengthAttribute != null)
                {
                    var lengthValue = lengthAttribute.Value;
                    var lengthElement = lineNode.SelectSingleNode("Length");

                    if (lengthElement != null)
                    {
                        // Update the <Length> tag to match the Length attribute
                        lengthElement.InnerText = lengthValue;
                    }
                    else
                    {
                        // Create a new <Length> tag if it doesn't exist
                        var newLengthElement = xmlDoc.CreateElement("Length");
                        newLengthElement.InnerText = lengthValue;
                        lineNode.AppendChild(newLengthElement);
                    }
                }
            }

            using (var stringWriter = new System.IO.StringWriter())
            {
                xmlDoc.Save(stringWriter);
                return stringWriter.ToString();
            }
        }
        private void SetControlsEnabledAndMakeLoadingVisible(bool enabled)
        {
            pictureBoxCreating.Visible = !enabled;
            labelCreating.Visible = !enabled;
            progressBarCreating.Visible = !enabled;
            if (enabled)
            {
                this.Size = NormalWindowSize;
            }
            else
            {
                this.Size = LoadingWindowSize;
            }
            foreach (Control ctrl in Controls)
            {
                if (ctrl == labelCreating || ctrl == pictureBoxCreating || ctrl == progressBarCreating || ctrl == labelPoweredByGemini)
                    continue;

                ctrl.Enabled = enabled;
            }
            if (enabled == true)
            {
                if (textBoxPrompt.Text != String.Empty)
                {
                    buttonCreate.Enabled = true;
                }
                else
                {
                    buttonCreate.Enabled = false;
                }
            }
        }
        private void ApplyAIModelChanges()
        {
            switch (comboBox_ai_model.SelectedIndex)
            {
                case 0:
                    AIModel = "models/gemini-2.5-flash";
                    break;
                case 1:
                    AIModel = "models/gemini-2.5-pro";
                    break;
                case 2:
                    AIModel = "models/gemini-2.0-flash";
                    break;
                case 3:
                    AIModel = "models/gemini-2.0-flash-lite";
                    break;
                default:
                    AIModel = "models/gemini-2.5-flash";
                    break;
            }
        }

        private void comboBox_ai_model_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox_ai_model.SelectedIndex != Settings1.Default.preferredAIModel)
            {
                Settings1.Default.preferredAIModel = comboBox_ai_model.SelectedIndex;
                Settings1.Default.Save();
                ApplyAIModelChanges();
            }
        }
    }
}