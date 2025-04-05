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

namespace NeoBleeper
{
    public partial class CreateMusicWithAI : Form
    {
        public string output = "";
        PrivateFontCollection fonts = new PrivateFontCollection();
        public CreateMusicWithAI()
        {
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            foreach (Control ctrl in Controls)
            {
                ctrl.Font = new Font(fonts.Families[0], 9);
            }
            InitializeComponent();
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
            MusicCreating musicCreating = new MusicCreating();
            try
            {
                musicCreating.Show();
                SetControlsEnabled(false);
                var apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                var googleAI = new GoogleAi(apiKey);
                var googleModel = googleAI.CreateGenerativeModel("models/gemini-2.0-flash");
                var googleResponse = await googleModel.GenerateContentAsync($"**User Prompt:\r\n[{textBoxPrompt.Text}]**\r\n\r\n" +
                    $"--- AI Instructions ---\r\n" +
                    $"Based on the User Prompt above, generate a sequence of <Line> elements to replace the placeholder section within the <LineList> below.\r\n" +
                    $"- PRIORITY 1: Ensure the output is a complete and valid <NeoBleeperProjectFile> XML structure. Do not include any additional text, code block markers (```xml), or explanations. Use UTF-8 encoding.\r\n" +
                    $"- Adhere to the XML structure provided.\r\n" +
                    $"- Each <Line> represents a musical event or rest.\r\n" +
                    $"- <Length> defines the duration. Use a variety of durations from the following: Whole, Half, Quarter, 1/8, 1/16, and 1/32. Vary the frequency of each duration\r\n" +
                    $"- <Note1>, <Note2>, <Note3>, <Note4> hold the notes (e.g., C4, G#5, F6). Use empty tags (e.g., <Note3 />) if no note is played in that voice/slot for that duration. Mix single notes and chords.\r\n" +
                    $"- <Mod /> defines the modulation (e.g Dot, Tri). Use empty tags (e.g., <Mod />) if there's no modulation in that voice/slot.\r\n" +
                    $"- <Art /> defines the articulation(e.g Sta, Spi, Fer). Use empty tags (e.g., <Art />) if there's no articulation in that voice/slot.\r\n" +
                    $"- Use the <Settings> provided as context for parameters like BPM and Time Signature, unless the prompt overrides them.\r\n" +
                    $"- Use only valid XML characters and escape special characters (&lt;, &gt;, &amp;, &apos;, &quot;) correctly.\r\n" +
                    $"- Generate music with a duration between 10 and 180 seconds.\r\n" +
                    $"- Introduce more variations in melody, harmony, and rhythm.\r\n" +
                    $"- Vary the BPM between 40 and 600.\r\n" +
                    $"- Use different time signatures (e.g., 3/4, 6/8, 4/4) randomly.\r\n" +
                    $"- **Note Silence Ratio:**\r\n" +
                    $"- Randomly vary the `NoteSilenceRatio` between 1 and 100, favoring higher values (more silence).\r\n" +
                    $"- If the user's prompt contains keywords like 'sparse', 'minimal', 'quiet', or 'ambient', increase the `NoteSilenceRatio` towards 99.\r\n" +
                    $"- If the user prompt contains a number, use that number to set the `<NoteSilenceRatio>` value.\r\n" +
                    $"- If no number is given, use a default value of 95 for the NoteSilenceRatio.\r\n" +
                    $"- The NoteSilenceRatio value represents the percentage of time that silence occurs, versus a note being played.\r\n" +
                    $"- Randomly vary the `NoteLength` between 1 and 5.\r\n" +
                    $"- Include single-voice sections in the generated music.\r\n" +
                    $"- Distribute notes randomly across all four voices.\r\n" +
                    $"- Vary the combinations of voices used in chords.\r\n" +
                    $"- Use Note 1, Note 2, Note 3 and Note 4 channels randomly.\r\n" +
                    $"- Limit the range of note durations to avoid extreme variations.\r\n" +
                    $"- Control the randomness of note selection to create more coherent melodies.\r\n" +
                    $"- **Ensure the generated XML includes a randomly chosen <AlternateTime> value (e.g., between 5 and 200) within the <RandomSettings> section for arpeggiated notes (because system speaker can only play single note at a time).**\r\n" +
                    $"--- NeoBleeperProjectFile Template ---" +
                    $"\r\n<NeoBleeperProjectFile>\r\n    <Settings>\r\n        <RandomSettings>\r\n            <KeyboardOctave>5</KeyboardOctave>\r\n            <BPM>120</BPM>\r\n            " +
                    $"<TimeSignature>4</TimeSignature>\r\n            <NoteSilenceRatio>98</NoteSilenceRatio>\r\n            <NoteLength>3</NoteLength>\r\n            " +
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
                    output = Regex.Replace(output, @"^\s*```xml\s*", String.Empty, RegexOptions.Multiline);
                    output = Regex.Replace(output, @"\s*```\s*$", String.Empty);

                    // Trim leading/trailing whitespace
                    output = output.Trim();
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
                musicCreating.Close();
                SetControlsEnabled(true);
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
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
        private void SetControlsEnabled(bool enabled)
        {
            foreach (Control ctrl in Controls)
            {
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

        private void CreateMusicWithAI_Load(object sender, EventArgs e)
        {

        }
    }
}