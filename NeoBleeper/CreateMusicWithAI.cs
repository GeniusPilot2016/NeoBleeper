using GenerativeAI.Types;
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
using NeoBleeper.Properties;

namespace NeoBleeper
{
    public partial class CreateMusicWithAI : Form
    {
        bool darkTheme = false;
        public string output = "";
        String AIModel = "models/gemini-2.5-flash";
        Size NormalWindowSize;
        double scaleFraction = 0.425; // Scale factor for the window size
        Size LoadingWindowSize;
        public CreateMusicWithAI()
        {
            InitializeComponent();
            NormalWindowSize = this.Size;
            LoadingWindowSize = new Size(NormalWindowSize.Width, (int)(NormalWindowSize.Height + (NormalWindowSize.Height * scaleFraction)));
            UIFonts.setFonts(this);
            set_theme();
            comboBox_ai_model.SelectedIndex = Settings1.Default.preferredAIModel;
            ApplyAIModelChanges();
            if (!IsInternetAvailable())
            {
                Logger.Log("Internet connection is not available. Please check your connection.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageNoInternet, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else if (string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
            {
                Logger.Log("Google Gemini™ API key is not set. Please set the API key in the \"General\" tab in settings.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageAPIKeyIsNotSet, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            buttonCreate.BackColor = Color.FromArgb(32, 32, 32);
            buttonCreate.ForeColor = Color.White;
            textBoxPrompt.BackColor = Color.Black;
            textBoxPrompt.ForeColor = Color.White;
            comboBox_ai_model.BackColor = Color.Black;
            comboBox_ai_model.ForeColor = Color.White;
            this.ForeColor = Color.White;
            TitleBarHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            buttonCreate.BackColor = Color.Transparent;
            buttonCreate.ForeColor = SystemColors.ControlText;
            textBoxPrompt.BackColor = SystemColors.Window;
            textBoxPrompt.ForeColor = SystemColors.WindowText;
            comboBox_ai_model.BackColor = SystemColors.Window;
            comboBox_ai_model.ForeColor = SystemColors.WindowText;
            this.ForeColor = SystemColors.ControlText;
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
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
                var googleResponse = await googleModel.GenerateContentAsync(
                    $"**User Prompt:**\r\n[{textBoxPrompt.Text}]\r\n\r\n" +
                    $"--- AI Instructions ---\r\n" +
                    $"Generate a valid XML structure for a NeoBleeper project file based on the user prompt. Follow these rules strictly:\r\n" +
                    $"- The output must be a complete and valid XML document starting with <NeoBleeperProjectFile> and ending with </NeoBleeperProjectFile>.\r\n" +
                    $"- Do not include any text, comments, or markers outside the XML structure.\r\n" +
                    $"- Use UTF-8 encoding and escape special characters (&lt;, &gt;, &amp;, &apos;, &quot;) correctly.\r\n" +
                    $"- Ensure all tags are properly closed and formatted.\r\n" +
                    $"- Use the provided <Settings> as context for parameters like BPM and Time Signature unless overridden by the user prompt.\r\n" +
                    $"- Populate the <LineList> section with <Line> elements representing musical events or rests.\r\n" +
                    $"- Each <Line> must include:\r\n" +
                    $"  - A <Length> tag with one of the following values: Whole, Half, Quarter, 1/8, 1/16, or 1/32.\r\n" +
                    $"  - A single <Mod /> tag with values \"Dot\" or \"Tri\" (use empty tags if no modulation).\r\n" +
                    $"  - A single <Art /> tag with articulation values (e.g., Sta, Spi, Fer) or empty tags if none.\r\n" +
                    $"- Notes must follow these rules:\r\n" +
                    $"  - Represent notes as letters (A-G).\r\n" +
                    $"  - Include sharps (#) if applicable (e.g., C#, F#).\r\n" +
                    $"  - Specify the octave number (1-10) after the note (e.g., A4, C#5).\r\n" +
                    $"  - Do not use flat notes (e.g., Db). Convert them to their sharp equivalents (e.g., C#).\r\n" +
                    $"- For rests, leave all <Note1>, <Note2>, <Note3>, and <Note4> tags blank (e.g., <Note1></Note1>). Do not write 'rest' or any other text inside the tags.\r\n" +
                    $"- Distribute notes randomly across <Note1>, <Note2>, <Note3>, and <Note4> channels.\r\n" +
                    $"- Distribute notes randomly across <Note1>, <Note2>, <Note3>, and <Note4> channels.\r\n" +
                    $"- Use <PlayNote1>, <PlayNote2>, <PlayNote3>, and <PlayNote4> tags in the <PlayNotes> section.\r\n" +
                    $"- Ensure the <RandomSettings> section includes chosen <AlternateTime> value (5-200) by context of music to enable pseudo-polyphony on system speakers (aka PC speaker) (5-30 is for better pseudo-polyphony effect).\r\n" +
                    $"- Generate music with a BPM of its context, typically between 40 and 120, unless specified otherwise in the user prompt.\r\n" +
                    $"- Vary time signatures (e.g., 3/4, 6/8, 4/4).\r\n" +
                    $"- Maintain a NoteSilenceRatio between 40-95 to balance notes and rests.\r\n" +
                    $"- Avoid extreme variations in note durations and ensure coherent melodies.\r\n"+
                    $"- Do not use numbered tags (e.g., <Mod1>, <Art2>) or unsupported values (e.g., Vib, Arp, Gliss).\r\n" +
                    $"- Ensure the output adheres to the NeoBleeper XML structure template below:\r\n\r\n" +
                    $"<NeoBleeperProjectFile>\r\n" +
                    $"    <Settings>\r\n" +
                    $"        <RandomSettings>\r\n" +
                    $"            <KeyboardOctave>5</KeyboardOctave>\r\n" +
                    $"            <BPM>120</BPM>\r\n" +
                    $"            <TimeSignature>4</TimeSignature>\r\n" +
                    $"            <NoteSilenceRatio>95</NoteSilenceRatio>\r\n" +
                    $"            <NoteLength>3</NoteLength>\r\n" +
                    $"            <AlternateTime>5</AlternateTime>\r\n" +
                    $"        </RandomSettings>\r\n" +
                    $"        <PlaybackSettings>\r\n" +
                    $"            <NoteClickPlay>True</NoteClickPlay>\r\n" +
                    $"            <NoteClickAdd>True</NoteClickAdd>\r\n" +
                    $"            <AddNote1>False</AddNote1>\r\n" +
                    $"            <AddNote2>False</AddNote2>\r\n" +
                    $"            <AddNote3>True</AddNote3>\r\n" +
                    $"            <AddNote4>False</AddNote4>\r\n" +
                    $"            <NoteReplace>True</NoteReplace>\r\n" +
                    $"            <NoteLengthReplace>False</NoteLengthReplace>\r\n" +
                    $"        </PlaybackSettings>\r\n" +
                    $"        <ClickPlayNotes>\r\n" +
                    $"            <ClickPlayNote1>True</ClickPlayNote1>\r\n" +
                    $"            <ClickPlayNote2>True</ClickPlayNote2>\r\n" +
                    $"            <ClickPlayNote3>True</ClickPlayNote3>\r\n" +
                    $"            <ClickPlayNote4>True</ClickPlayNote4>\r\n" +
                    $"        </ClickPlayNotes>\r\n" +
                    $"        <PlayNotes>\r\n" +
                    $"            <PlayNote1>True</PlayNote1>\r\n" +
                    $"            <PlayNote2>True</PlayNote2>\r\n" +
                    $"            <PlayNote3>True</PlayNote3>\r\n" +
                    $"            <PlayNote4>True</PlayNote4>\r\n" +
                    $"        </PlayNotes>\r\n" +
                    $"    </Settings>\r\n" +
                    $"    <LineList>\r\n" +
                    $"    </LineList>\r\n" +
                    $"</NeoBleeperProjectFile>\r\n"
                );
                if (googleResponse != null || !string.IsNullOrWhiteSpace(googleResponse.Text))
                {
                    output = googleResponse.Text();

                    // Remove ```xml and any surrounding text
                    output = Regex.Replace(output, @"<\?xml.*?\?>", String.Empty, RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"^\s*```xml\s*", String.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*```\s*$", String.Empty);
                    output = Regex.Replace(output, @"\s*1\s*$", "Whole", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*1/2\s*$", "Half", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*1/4\s*$", "Quarter", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Eighth\s*$", "1/8", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Sixteenth\s*$", "1/16", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Thirty-second\s*$", "1/32", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"\s*Thirty Second\s*$", "1/32", RegexOptions.IgnoreCase);
                    output = Regex.Replace(output, @"N(\d)([A-G])", "$2$1");
                    output = Regex.Replace(output, @"N(\d)([A-G]#?)", "$2$1");
                    output = Regex.Replace(output, @"N(\d)([A-G][#b]?)", "$2$1");
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
                    Logger.Log("AI response is null or empty.", Logger.LogTypes.Error);
                    MessageBox.Show(Resources.MessageAIResponseNullOrEmpty);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error: {ex.Message}", Logger.LogTypes.Error);
                MessageBox.Show($"{Resources.MessageAnErrorOccured} {ex.Message}");
            }
            finally
            {
                SetControlsEnabledAndMakeLoadingVisible(true);
                if (checkIfOutputIsValidNBPML(output))
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
            // Ensure all tags are properly closed and formatted
            output = Regex.Replace(output, @"(?<!<)(NeoBleeperProjectFile>.*?</NeoBleeperProjectFile>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Settings>.*?</Settings>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(RandomSettings>.*?</RandomSettings>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(KeyboardOctave>.*?</KeyboardOctave>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(BPM>.*?</BPM>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(TimeSignature>.*?</TimeSignature>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(NoteSilenceRatio>.*?</NoteSilenceRatio>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(AlternateTime>.*?</AlternateTime>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(PlaybackSettings>.*?</PlaybackSettings>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(NoteClickPlay>.*?</NoteClickPlay>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(NoteClickAdd>.*?</NoteClickAdd>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(AddNote[1-4]>.*?</AddNote[1-4]>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(NoteReplace>.*?</NoteReplace>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(NoteLengthReplace>.*?</NoteLengthReplace>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(ClickPlayNotes>.*?</ClickPlayNotes>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(ClickPlayNote[1-4]>.*?</ClickPlayNote[1-4]>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(PlayNotes>.*?</PlayNotes>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Note[1-4]>.*?</Note[1-4]>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Mod>.*?</Mod>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Art>.*?</Art>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Length>.*?</Length>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(PlayNote[1-4]>.*?</PlayNote[1-4]>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(LineList>.*?</LineList>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Line>.*?</Line>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Mod>.*?</Mod>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"(?<!<)(Art>.*?</Art>)", "<$1", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</NeoBleeperProjectFile\s*</NeoBleeperProjectFile>", "</NeoBleeperProjectFile>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Settings\s*</Settings>", "</Settings>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</RandomSettings\s*</RandomSettings>", "</RandomSettings>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</KeyboardOctave\s*</KeyboardOctave>", "</KeyboardOctave>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</BPM\s*</BPM>", "</BPM>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</TimeSignature\s*</TimeSignature>", "</TimeSignature>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</NoteSilenceRatio\s*</NoteSilenceRatio>", "</NoteSilenceRatio>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</AlternateTime\s*</AlternateTime>", "</AlternateTime>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</PlaybackSettings\s*</PlaybackSettings>", "</PlaybackSettings>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</NoteClickPlay\s*</NoteClickPlay>", "</NoteClickPlay>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</NoteClickAdd\s*</NoteClickAdd>", "</NoteClickAdd>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</AddNote1\s*</AddNote1>", "</AddNote1>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</AddNote2\s*</AddNote2>", "</AddNote2>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</AddNote3\s*</AddNote3>", "</AddNote3>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</AddNote4\s*</AddNote4>", "</AddNote4>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</NoteReplace\s*</NoteReplace>", "</NoteReplace>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</NoteLengthReplace\s*</NoteLengthReplace>", "</NoteLengthReplace>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</ClickPlayNotes\s*</ClickPlayNotes>", "</ClickPlayNotes>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</ClickPlayNote1\s*</ClickPlayNote1>", "</ClickPlayNote1>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</ClickPlayNote2\s*</ClickPlayNote2>", "</ClickPlayNote2>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</ClickPlayNote3\s*</C3lickPlayNote3>", "</ClickPlayNote3>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</ClickPlayNote4\s*</ClickPlayNote4>", "</ClickPlayNote4>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</PlayNotes\s*</PlayNotes>", "</PlayNotes>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</PlayNote1\s*</PlayNote1>", "</PlayNote1>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</PlayNote2\s*</PlayNote2>", "</PlayNote2>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</PlayNote3\s*</PlayNote3>", "</PlayNote3>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</PlayNote4\s*</PlayNote4>", "</PlayNote4>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</LineList\s*</LineList>", "</LineList>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Line\s*</Line>", "</Line>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Length\s*</Length>", "</Length>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Mod\s*</Mod>", "</Mod>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Art\s*</Art>", "</Art>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Note4\s*</Note4>", "</Note4>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Note3\s*</Note3>", "</Note3>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Note2\s*</Note2>", "</Note2>", RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"</Note1\s*</Note1>", "</Note1>", RegexOptions.IgnoreCase);
            // Apply additional transformations to ensure NBPML format compliance
            output = Regex.Replace(output, @"<\s*/?\s*alternatetime\s*>", m =>
            {
                // Check if the match contains a number
                if (m.Value.Contains("/"))
                    return "</AlternateTime>";
                else
                    return "<AlternateTime>";
            }, RegexOptions.IgnoreCase);
            output = Regex.Replace(output, @"<\s*/?\s*notesilenceratio\s*>", m =>
            {
                // Check if the match contains a number
                if (m.Value.Contains("/"))
                    return "</NoteSilenceRatio>";
                else
                    return "<NoteSilenceRatio>";
            }, RegexOptions.IgnoreCase);
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

            // Ensure all tags are properly closed and formatted
            output = Regex.Replace(output, @"(?<!<)(NeoBleeperProjectFile>.*?</NeoBleeperProjectFile>)", "<$1", RegexOptions.IgnoreCase);

            // Remove any characters before the root element
            output = Regex.Replace(output, @"^[^\S\r\n]*", ""); // Remove leading whitespace

            // Trim leading/trailing whitespace
            output = output.Trim();

            // Ensure <NeoBleeperProjectFile> starts and ends correctly
            if (!output.StartsWith("<NeoBleeperProjectFile>"))
            {
                throw new Exception("Invalid XML: Root element is missing or incorrect.");
            }

            // Additional transformations for NBPML compliance
            output = FixParameterNames(output);
            output = output.Trim();
            output = SynchronizeLengths(output);
            output = output.Trim();

            // Remove XML declaration if present
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
            xmlContent = xmlContent.TrimStart();
            xmlContent = Regex.Replace(xmlContent, @"</<(\w+)>", @"</$1>");
            xmlContent = System.Text.RegularExpressions.Regex.Replace(
                xmlContent, @"<\?xml.*?\?>", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

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
                        lengthElement.InnerText = lengthValue;
                    }
                    else
                    {
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
            if (comboBox_ai_model.SelectedIndex != Settings1.Default.preferredAIModel)
            {
                Settings1.Default.preferredAIModel = comboBox_ai_model.SelectedIndex;
                Settings1.Default.Save();
                ApplyAIModelChanges();
            }
        }

        private void CreateMusicWithAI_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}