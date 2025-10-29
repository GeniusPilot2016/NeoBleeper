﻿// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
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

using GenerativeAI;
using NeoBleeper.Properties;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace NeoBleeper
{
    public partial class CreateMusicWithAI : Form
    {
        // Created as byproduct of my old school project to create chaotic music with AI, which is the AI-powered Paint-like program called "ArtFusion" (however, our projects were prohibited to exhibit in school exhibition until final exam points are given, so I exhibited this program instead, instead of exhibiting "ugly" automation projects like "Hotel reservation system" and "Library management system" which are boring and useless for normal users)
        string[] examplePrompts =         {
            Resources.ExamplePrompt1,
            Resources.ExamplePrompt2,
            Resources.ExamplePrompt3,
            Resources.ExamplePrompt4,
            Resources.ExamplePrompt5,
            Resources.ExamplePrompt6,
            Resources.ExamplePrompt7,
            Resources.ExamplePrompt8,
            Resources.ExamplePrompt9,
            Resources.ExamplePrompt10,
            Resources.ExamplePrompt11,
            Resources.ExamplePrompt12,
            Resources.ExamplePrompt13,
            Resources.ExamplePrompt14,
            Resources.ExamplePrompt15
        };
        string examplePrompt = "";
        bool isCreatedAnything = false; // Flag to indicate if anything was created by AI
        bool darkTheme = false;
        public string output = "";
        public string generatedFilename = "";   
        String AIModel = Settings1.Default.preferredAIModel;
        Size NormalWindowSize;
        double scaleFraction = 0.325; // Scale factor for the window size
        Size LoadingWindowSize;
        string selectedLanguage = Settings1.Default.preferredLanguage; // Get the preferred language from settings
        CancellationTokenSource cts = new CancellationTokenSource(); // CancellationTokenSource for cancelling requests when internet is lost or server is down
        bool isErrorMessageShown = false; // Flag to prevent invalid NBPML error message when JSON error message is shown
        Dictionary<string, string> aiModelMapping = new Dictionary<string, string>
        {
        };
        public CreateMusicWithAI()
        {
            InitializeComponent();
            NormalWindowSize = this.Size;
            LoadingWindowSize = new Size(NormalWindowSize.Width, (int)(NormalWindowSize.Height + (NormalWindowSize.Height * scaleFraction)));
            UIFonts.setFonts(this);
            set_theme();
            examplePrompt = examplePrompts[new Random().Next(examplePrompts.Length)];
            textBoxPrompt.PlaceholderText = examplePrompt;
            if (!IsAvailableInCountry())
            {
                Logger.Log("Google Gemini™ API is not available in your country. Please check the list of supported countries.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.GoogleGeminiAPIIsNotSupportedInYourCountry, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else if (!IsInternetAvailable())
            {
                ShowNoInternetMessage();
                this.Close();
            }
            else if (!IsServerUp())
            {
                ShowServerDownMessage();
                this.Close();
            }
            else if (string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
            {
                Logger.Log("Google Gemini™ API key is not set. Please set the API key in the \"General\" tab in settings.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageAPIKeyIsNotSet, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else if (!isAPIKeyValidFormat(EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey)))
            {
                Logger.Log("Google Gemini™ API key format is invalid. Please re-enter the API key in the \"General\" tab in settings.", Logger.LogTypes.Error);
                MessageBox.Show(Resources.MessageGoogleGeminiAPIKeyFormatIsInvalid, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            listAndSelectAIModels();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SETTINGCHANGE = 0x001A;
            base.WndProc(ref m);

            if (m.Msg == WM_SETTINGCHANGE)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme();
                }
            }
        }
        string[] wantedFeatures =
        {
            "generateContent", // Metin oluşturma yeteneği
            "countTokens"      // Token sayma yeteneği
        };

        string[] unwantedFeatures =
        {
            "predict",         // Görüntü oluşturma yeteneği
            "embedContent",    // Metin gömme yeteneği
            "embedText",       // Metin gömme yeteneği
            "asyncBatchEmbedContent", // Asenkron gömme yeteneği
            "generateAnswer"   // Soru yanıtlama yeteneği
        };
        private async void listAndSelectAIModels()
        {
            buttonCreate.Enabled = false; // Disable the create button while loading models
            var generativeAI = new GenerativeAI.GoogleAi(EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey));
            var models = await generativeAI.ListModelsAsync();

            List<string> filteredDisplayNames = new List<string>();

            foreach (var model in models.Models)
            {
                // Skip models that don't contain "gemini" in the name (case-insensitive)
                if (string.IsNullOrEmpty(model.Name) || !model.Name.Contains("gemini", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Skip specific preview, experimental and special purpose models
                if (model.Name.Contains("image", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("computer-use", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("robotics", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("code", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("experimental", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("preview", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("latest", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("001", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("tts", StringComparison.OrdinalIgnoreCase) ||
                model.Name.Contains("audio", StringComparison.OrdinalIgnoreCase) || 
                model.DisplayName.Contains("image", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("computer-use", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("robotics", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("code", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("experimental", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("preview", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("latest", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("001", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("tts", StringComparison.OrdinalIgnoreCase) ||
                model.DisplayName.Contains("audio", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // Check for required features and absence of unwanted features
                if (model.SupportedGenerationMethods != null &&
                    wantedFeatures.All(feature => model.SupportedGenerationMethods.Contains(feature)) &&
                    unwantedFeatures.All(feature => !model.SupportedGenerationMethods.Contains(feature)))
                {
                    aiModelMapping[model.Name] = model.DisplayName;
                    aiModelMapping[model.DisplayName] = model.Name;
                    filteredDisplayNames.Add(model.DisplayName);
                }
            }

            Logger.Log($"Total filtered models: {filteredDisplayNames.Count}", Logger.LogTypes.Info);

            comboBox_ai_model.Items.AddRange(filteredDisplayNames.ToArray());

            if (filteredDisplayNames.Count == 0)
            {
                Logger.Log("WARNING: No valid Gemini models found!", Logger.LogTypes.Warning);
            }

            if (aiModelMapping.ContainsKey(Settings1.Default.preferredAIModel))
            {
                AIModel = Settings1.Default.preferredAIModel;
                comboBox_ai_model.SelectedItem = aiModelMapping[Settings1.Default.preferredAIModel];
                Logger.Log($"Using preferred model: {AIModel}", Logger.LogTypes.Info);
            }
            else
            {
                if (filteredDisplayNames.Count > 0)
                {
                    AIModel = aiModelMapping[filteredDisplayNames[0]];
                    comboBox_ai_model.SelectedItem = filteredDisplayNames[0];
                    Logger.Log($"Using first available model: {AIModel}", Logger.LogTypes.Info);
                }
                else
                {
                    Logger.Log("ERROR: No valid models available!", Logger.LogTypes.Error);
                    MessageBox.Show(Resources.MessageNoAvailableGeminiModel, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            buttonCreate.Enabled = true; // Enable the create button after loading models
        }
        private string selectedLanguageToLanguageName(string languageName)
        {
            string language = "English";
            switch (languageName)
            {
                case "English":
                    language = "English";
                    break;
                case "Deutsch":
                    language = "German";
                    break;
                case "Español":
                    language = "Spanish";
                    break;
                case "Français":
                    language = "French";
                    break;
                case "Italiano":
                    language = "Italian";
                    break;
                case "Türkçe":
                    language = "Turkish";
                    break;
                case "Русский":
                    language = "Russian";
                    break;
                case "українська":
                    language = "Ukrainian";
                    break;
                case "Tiếng Việt":
                    language = "Vietnamese";
                    break;
                default:
                    Logger.Log("Selected language is not supported. Defaulting to English.", Logger.LogTypes.Warning);
                    language = "English";
                    break;
            }
            return language;
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
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
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
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        // Check internet connectivity and server status
        private bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("info.cern.ch"); // Pinging the first website ever
                    // Fun fact: info.cern.ch is the first website ever created, launched on August 6, 1991, by Tim Berners-Lee at CERN.
                    // It was originally used to provide information about the World Wide Web project.
                    // Today, it serves as a historical site and a tribute to the origins of the web.
                    // Pinging this site is a nod to the history of the internet.
                    if (reply.Status == IPStatus.Success)
                    {
                        if (reply.RoundtripTime > 500) // 500 ms and above is considered slow
                        {
                            Logger.Log($"Internet connection is slow: {reply.RoundtripTime} ms.", Logger.LogTypes.Warning);
                        }
                        return true;
                    }
                    else
                    {
                        Logger.Log("Internet connection failed.", Logger.LogTypes.Error);
                        return true; // Return true in all cases
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Internet connection check error: {ex.Message}", Logger.LogTypes.Error);
                return false; // Return false
            }
        }

        private bool IsServerUp()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("generativelanguage.googleapis.com");
                    if (reply.Status == IPStatus.Success)
                    {
                        if (reply.RoundtripTime > 500)
                        {
                            Logger.Log($"Access to server is slow: {reply.RoundtripTime} ms.", Logger.LogTypes.Warning);
                        }
                        return true;
                    }
                    else
                    {
                        Logger.Log("Access to server failed.", Logger.LogTypes.Error);
                        return true; // Return true in all cases
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error checking server status: {ex.Message}", Logger.LogTypes.Error);
                return false; // Return false
            }
        }
        public static bool isAPIKeyValidFormat(string APIKey)
        {
            // Google API keys typically start with "AIzaSy" followed by 33 alphanumeric characters, underscores, or hyphens
            if (string.IsNullOrWhiteSpace(APIKey))
                return false;

            // Regex pattern to match the Google API key format
            var regex = new Regex(@"AIzaSy[A-Za-z0-9_\-]{33}$");
            return regex.IsMatch(APIKey);
        }

        // Check country availability for Google Gemini™ API (according to https://ai.google.dev/gemini-api/docs/available-regions)
        public static bool IsAvailableInCountry()
        {
            String[] supportedCountries =
            {
              "AL",
              "DZ",
              "AS",
              "AO",
              "AI",
              "AQ",
              "AG",
              "AR",
              "AM",
              "AW",
              "AU",
              "AT",
              "AZ",
              "BS",
              "BH",
              "BD",
              "BB",
              "BE",
              "BZ",
              "BJ",
              "BM",
              "BT",
              "BO",
              "BA",
              "BW",
              "BR",
              "IO",
              "VG",
              "BN",
              "BG",
              "BF",
              "BI",
              "CV",
              "KH",
              "CM",
              "CA",
              "BQ",
              "KY",
              "CF",
              "TD",
              "CL",
              "CX",
              "CC",
              "CO",
              "KM",
              "CK",
              "CI",
              "CR",
              "HR",
              "CW",
              "CZ",
              "CD",
              "DK",
              "DJ",
              "DM",
              "DO",
              "EC",
              "EG",
              "SV",
              "GQ",
              "ER",
              "EE",
              "SZ",
              "ET",
              "FK",
              "FO",
              "FJ",
              "FI",
              "FR",
              "GA",
              "GM",
              "GE",
              "DE",
              "GH",
              "GI",
              "GR",
              "GL",
              "GD",
              "GU",
              "GT",
              "GG",
              "GN",
              "GW",
              "GY",
              "HT",
              "HM",
              "BA",
              "HN",
              "HU",
              "IS",
              "IN",
              "ID",
              "IQ",
              "IE",
              "IM",
              "IL",
              "IT",
              "JM",
              "JP",
              "JE",
              "JO",
              "KZ",
              "KE",
              "KI",
              "XK",
              "KG",
              "KW",
              "LA",
              "LV",
              "LB",
              "LS",
              "LR",
              "LY",
              "LI",
              "LT",
              "LU",
              "MG",
              "MW",
              "MY",
              "MV",
              "ML",
              "MT",
              "MH",
              "MR",
              "MU",
              "MX",
              "FM",
              "MN",
              "ME",
              "MS",
              "MA",
              "MZ",
              "NA",
              "NR",
              "NP",
              "NL",
              "NC",
              "NZ",
              "NI",
              "NE",
              "NG",
              "NU",
              "NF",
              "MK",
              "MP",
              "NO",
              "OM",
              "PK",
              "PW",
              "PS",
              "PA",
              "PG",
              "PY",
              "PE",
              "PH",
              "PN",
              "PL",
              "PT",
              "PR",
              "QA",
              "CY",
              "CG",
              "RO",
              "RW",
              "BL",
              "KN",
              "LC",
              "PM",
              "VC",
              "SH",
              "WS",
              "ST",
              "SA",
              "SN",
              "RS",
              "SC",
              "SL",
              "SG",
              "SK",
              "SI",
              "SB",
              "SO",
              "ZA",
              "GS",
              "KR",
              "SS",
              "ES",
              "LK",
              "SD",
              "SR",
              "SE",
              "CH",
              "TW",
              "TJ",
              "TZ",
              "TH",
              "TL",
              "TG",
              "TK",
              "TO",
              "TT",
              "TN",
              "TR",
              "TM",
              "TC",
              "TV",
              "UG",
              "UA",
              "GB",
              "AE",
              "US",
              "UM",
              "VI",
              "UY",
              "UZ",
              "VU",
              "VE",
              "VN",
              "WF",
              "EH",
              "YE",
              "ZM",
              "ZW"
            };
            var countryCode = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;
            return supportedCountries.Contains(countryCode);
        }
        private async void buttonCreate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(AIModel))
            {
                try
                {
                    // Create music with AI like it's 2007 again using Google Gemini™ API, which is 2020's technology
                    string prompt = !string.IsNullOrWhiteSpace(textBoxPrompt.Text) ? textBoxPrompt.Text.Trim() : textBoxPrompt.PlaceholderText.Trim(); // Use placeholder if textbox is empty
                    connectionCheckTimer.Start();
                    SetControlsEnabledAndMakeLoadingVisible(false);
                    var apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                    var googleAI = new GoogleAi(apiKey);
                    var googleModel = googleAI.CreateGenerativeModel(AIModel);
                    // System prompt and user prompt for generating NBPML content with strict rules and format
                    var googleResponse = await googleModel.GenerateContentAsync(
                        $"**User Prompt:**\r\n[{prompt}]\r\n\r\n" +
                        $"--- AI Instructions ---\r\n" +
                        $"You are an expert music composition AI. " +
                        $"If the user prompt is a song name, artist name, composer name, or ANY music-related term (even a single word), treat it as a music composition request. " +
                        $"If the user prompt contains words like 'create', 'generate', 'compose', 'make', or 'write' followed by music-related content, treat it as a music composition request. " +
                        $"If the user prompt is clearly NOT about music (weather, mathematics, cooking, etc.), or if the prompt contains offensive, inappropriate, violent, explosive, weapon-related, harmful, political, controversial, or religious content (hate speech, violence, adult content, discrimination, political topics, religious topics, controversial issues), " +
                        $"return ONLY the JSON error (no XML) in {selectedLanguageToLanguageName(selectedLanguage)}. The error message must include:\r\n" +
                        $"- A specific reason for the error (e.g., \"Profanity detected\", \"Non-music prompt detected\").\r\n" +
                        $"- Suggestions for valid prompts (e.g., \"Try asking for a song composition or artist-related music\")." +
                        $"ADDITIONAL SAFETY RULES:\r\n" +
                        $"- Treat as OFFENSIVE any prompt that uses local / phonetic word games, spoonerisms, spaced syllables, intentional misspellings, digit/asterisk substitutions (e.g. f*ck, f#ck, f@ck, f-ck) that conceal profanity, sexual, violent, or extremist terms.\r\n" +
                        $"- Examples (DO NOT OUTPUT THEM): \"Fenasi Kerim\" (a spaced phonetic construction forming a vulgar phrase phonetically). If such detected: respond ONLY with JSON error (no XML).\r\n" +
                        $"- To decide, internally normalize the user prompt by: lowercasing, removing diacritics, removing spaces and punctuation; compare against known offensive phonetic composites. If matched → JSON error.\r\n" +
                        $"- If the prompt includes or disguises violent / weapon / explosive terms (e.g., bomb, b*mb, b0mb, b o m b, grenade, explosive, terror...), produce ONLY JSON error.\r\n" +
                        $"- If the prompt contains political, controversial, or religious topics (e.g., political figures, parties, ideologies, religious beliefs, rituals, or controversial social issues), respond ONLY with JSON error (no XML).\r\n" +
                        $"- Normalize internally: lowercase, strip diacritics, remove spaces/punctuation to detect hidden forms.\r\n" +
                        $"- If such content detected → respond ONLY with JSON error (no explanation, no XML).\r\n" +
                        $"Examples of VALID music requests that should generate XML:\r\n" +
                        $"- \"Yesterday\" → generate music\r\n" +
                        $"- \"Beatles\" → generate music\r\n" +
                        $"- \"Beethoven\" → generate music\r\n" +
                        $"- \"classical\" → generate music\r\n" +
                        $"- \"rock song\" → generate music\r\n" +
                        $"- \"create Yesterday\" → generate music\r\n" +
                        $"- Country / nationality names in MUSIC context (in non-political context) → ALLOW\r\n" +
                        $"Examples of NON-music or disallowed requests that should return error:\r\n" +
                        $"- \"What is the weather?\" → error\r\n" +
                        $"- \"How to cook pasta?\" → error\r\n" +
                        $"- \"Calculate 2+2\" → error\r\n" +
                        $"- \"Who is the president of Turkey?\" → error\r\n" +
                        $"- \"Write a song about Islam\" → error\r\n" +
                        $"- \"Discuss climate change\" → error\r\n" +
                        $"- \"Make a song about elections\" → error\r\n" +
                        $"- Any violent / weapon / explosive / phonetic disguised vulgar request → error\r\n" +
                        $"- Only return a JSON error. The error message must be impersonal, direct, and must not contain any personal pronouns (I, we, you) or apologies (sorry, unfortunately, etc.) in any language.\r\n" +
                        $"- When returning a JSON error, always include both \"title\" and \"errorMessage\" fields, even if the title is generic. and When returning a JSON error, always use a specific, direct, and impersonal error message describing the reason (e.g., \"Non-music prompt detected\", \"Inappropriate content detected\"). Do not use ambiguous phrases like \"the prompt can't be processed\".\r\n" +
                        $"- Don't create JSON error if the prompt is a valid music request.\r\n" +
                        $"- If the user prompt specifies a song or artist name, generate music that closely resembles the style, melody, harmony, and structure of that song or artist. \r\n" +
                        $"- Try to capture the main melodic motifs, rhythm, and overall feel, but do not copy the original exactly. \r\n" +
                        $"- The output should be a new composition inspired by the specified song. \r\n" +
                        $"- The output should contain generated file name that each words are seperated with spaces in language of user prompt, without any extension (such as .BMM, .NBPML, .XML, etc.), then a separator line made of dashes, followed by the complete NeoBleeper XML content.\r\n" +
                        $"- The output must be a complete and valid XML document starting with <NeoBleeperProjectFile> and ending with </NeoBleeperProjectFile> when generating music.\r\n" +
                        $"- Do not include text outside XML. Escape special characters properly.\r\n" +
                        $"- Do not include any text, comments, or markers outside the XML structure.\r\n" +
                        $"- Use UTF-8 encoding and escape special characters (&lt;, &gt;, &amp;, &apos;, &quot;) correctly.\r\n" +
                        $"- Ensure all tags are properly closed and formatted.\r\n" +
                        $"- Use the provided <Settings> as context for parameters like BPM and Time Signature unless overridden by the user prompt.\r\n" +
                        $"- Populate the <LineList> section with <Line> elements representing musical events or rests.\r\n" +
                        $"- Each <Line> must include:\r\n" +
                        $"  - A <Length> tag with one of the following values: Whole, Half, Quarter, 1/8, 1/16, or 1/32.\r\n" +
                        $"  - A single <Mod /> tag with values \"Dot\" or \"Tri\" (use empty tags if no modulation).\r\n" +
                        $"  - A single <Art /> tag with articulation values (e.g., Sta, Spi, Fer) or empty tags if none.\r\n" +
                        $"  - Notes must follow these rules:\r\n" +
                        $"  - Represent notes as letters (A-G).\r\n" +
                        $"  - Include sharps (#) if applicable (e.g., C#, F#).\r\n" +
                        $"  - Specify the octave number (1-10) after the note (e.g., A4, C#5).\r\n" +
                        $"  - Always convert flat notes (e.g., Db) to their sharp equivalents (e.g., C#).\r\n" +
                        $"  - For rests, leave all <Note1>, <Note2>, <Note3>, and <Note4> tags blank (e.g., <Note1></Note1>). Do not write 'rest' or any other text inside the tags.\r\n" +
                        $"  - Distribute notes randomly across <Note1>, <Note2>, <Note3>, and <Note4> channels.\r\n" +
                        $"  - Use <PlayNote1>, <PlayNote2>, <PlayNote3>, and <PlayNote4> tags in the <PlayNotes> section.\r\n" +
                        $"  - Ensure the <RandomSettings> section includes chosen <AlternateTime> value (5-200) by context of music to enable pseudo-polyphony on system speakers (aka PC speaker) (5-30 is for better pseudo-polyphony effect).\r\n" +
                        $"  - Generate music with a BPM of its context, typically between 40 and 120, unless specified otherwise in the user prompt.\r\n" +
                        $"  - Vary time signatures (e.g., 3/4, 6/8, 4/4).\r\n" +
                        $"  - Maintain a NoteSilenceRatio between 40-95 to balance notes and rests.\r\n" +
                        $"  - Avoid extreme variations in note durations and ensure coherent melodies.\r\n" +
                        $"  - Do not use numbered tags (e.g., <Mod1>, <Art2>) or unsupported values (e.g., Vib, Arp, Gliss).\r\n" +
                        $"  - Do not include any explanations, apologies, or disclaimers in the output.\r\n" +
                        $"- Ensure the output adheres to the NeoBleeper XML structure template below:\r\n\r\n" +
                        $"[Generated file name without extension]\r\n" +
                        $"-----------------------------------------------------------\r\n" +
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
                    , cts.Token);
                    if (googleResponse != null || !string.IsNullOrWhiteSpace(googleResponse.Text))
                    {
                        // Clean and process the AI response from invalid or unwanted text or characters to extract valid NBPML content
                        string rawOutput = googleResponse.Text();
                        output = output.Trim();
                        string JSONText = string.Empty;
                        // Parse JSON blocks
                        var jsonMatch = Regex.Match(rawOutput, @"\{[\s\S]*?\}");
                        if (jsonMatch.Success)
                        {
                            JSONText = jsonMatch.Value;
                        }
                        if (checkIfOutputIsJSONErrorMessage(JSONText))
                        {
                            TurnJSONErrorIntoMessageBoxAndLog(JSONText);
                            generatedFilename = string.Empty; // Clear the filename if it's an error message
                            output = String.Empty; // Clear the output if it's an error message
                            this.Close(); // Close the form after handling the error message
                        }
                        splitFileNameAndOutput(rawOutput); ;
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
                        output = Regex.Replace(output, @"\bDb(\d+)\b", "C#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bEb(\d+)\b", "D#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bGb(\d+)\b", "F#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bAb(\d+)\b", "G#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bBb(\d+)\b", "A#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bD♭(\d+)\b", "C#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bE♭(\d+)\b", "D#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bG♭(\d+)\b", "F#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bA♭(\d+)\b", "G#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bB♭(\d+)\b", "A#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bC♯(\d+)\b", "C#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bD♯(\d+)\b", "D#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bF♯(\d+)\b", "F#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bG♯(\d+)\b", "G#$1", RegexOptions.IgnoreCase);
                        output = Regex.Replace(output, @"\bA♯(\d+)\b", "A#$1", RegexOptions.IgnoreCase);
                        // Trim leading/trailing whitespace
                        output = output.Trim();
                        output = RewriteOutput(output).Trim();
                        isCreatedAnything = true; // Set the flag to true
                    }
                    else
                    {
                        // AI response is null or empty - show an error message and log the error
                        Logger.Log("AI response is null or empty.", Logger.LogTypes.Error);
                        isCreatedAnything = false; // Set the flag to false
                        generatedFilename = string.Empty; // Clear the filename
                        output = String.Empty; // Clear the output
                        MessageBox.Show(Resources.MessageAIResponseNullOrEmpty, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Show a generic error message and log the exception details
                    Logger.Log($"Error: {ex.Message}", Logger.LogTypes.Error);
                    isCreatedAnything = false;
                    generatedFilename = string.Empty; // Clear the filename
                    output = String.Empty; // Clear the output
                    MessageBox.Show($"{Resources.MessageAnErrorOccurred} {ex.Message}", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Stop the timer, re-enable controls, and handle the output
                    connectionCheckTimer.Stop();
                    SetControlsEnabledAndMakeLoadingVisible(true);
                    if (checkIfOutputIsValidNBPML(output))
                    {
                        this.Close();
                    }
                    else
                    {
                        // If the output is not valid NBPML, show an error form and log the error
                        if (!string.IsNullOrWhiteSpace(output) && !isErrorMessageShown)
                        {
                            Logger.Log("Generated output is not valid NBPML.", Logger.LogTypes.Error);
                            AIGeneratedNBPMLError errorForm = new AIGeneratedNBPMLError(output);
                            errorForm.ShowDialog();
                        }
                        generatedFilename = string.Empty; // Clear the filename if it's invalid
                        output = String.Empty; // Clear the output if it's invalid
                        this.Close(); // Close the form after handling the invalid output
                    }
                }
            }
        }
        private void splitFileNameAndOutput(string rawOutput)
        {
            if (string.IsNullOrWhiteSpace(rawOutput))
            {
                generatedFilename = string.Empty;
                output = string.Empty;
                return;
            }

            // Find the separator line made of dashes (at least 3 dashes)
            var separatorMatch = Regex.Match(rawOutput, @"^-{3,}$", RegexOptions.Multiline);

            if (separatorMatch.Success)
            {
                // Find position of the separator
                int separatorIndex = separatorMatch.Index;

                // Take the part before the separator as filename
                string filenamePart = rawOutput.Substring(0, separatorIndex).Trim();

                // Length of the separator line
                int separatorLength = separatorMatch.Length;

                // Take the part after the separator as XML content
                int xmlStartIndex = separatorIndex + separatorLength;
                string xmlPart = rawOutput.Substring(xmlStartIndex).Trim();

                // Extract the last line from the filename part as the actual filename
                generatedFilename = filenamePart.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                               .LastOrDefault() ?? string.Empty;
                output = xmlPart;
            }
            else
            {
                // No separator found; treat entire output as XML content
                output = rawOutput.Trim();
                generatedFilename = GenerateFilenameFromPromptOrXml(textBoxPrompt.Text);
            }
        }

        private string GenerateFilenameFromPromptOrXml(string userPrompt)
        {
            if (!string.IsNullOrWhiteSpace(userPrompt))
            {
                // Use user prompt to create filename
                // Remove special characters and take first few words
                string sanitized = Regex.Replace(userPrompt.Trim(), @"[^a-zA-Z0-9\s]", "");
                string[] words = sanitized.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 0)
                {
                    // Take first 2-3 words, max 40 characters
                    string filename = string.Join("", words.Take(3));
                    if (filename.Length > 40)
                        filename = filename.Substring(0, 40);

                    return filename;
                }
            }

            // Fallback: generate timestamp-based filename
            return $"NeoBleeperMusic_{DateTime.Now:yyyyMMdd_HHmmss}";
        }
        private bool checkIfOutputIsJSONErrorMessage(String output)
        {
            if (string.IsNullOrWhiteSpace(output))
            {
                return false;
            }
            bool isValidJson = false;
            // Check if the output is valid JSON
            try
            {
                var jsonObj = System.Text.Json.JsonDocument.Parse(output);
                isValidJson = true;
            }
            catch (System.Text.Json.JsonException)
            {
                isValidJson = false;
            }
            // Check for the presence of error-related properties
            if (isValidJson)
            {
                var jsonDoc = System.Text.Json.JsonDocument.Parse(output);
                // Check for both formats: new format (title + errorMessage) and old format (error)
                if ((jsonDoc.RootElement.TryGetProperty("title", out var titleProp) &&
                     jsonDoc.RootElement.TryGetProperty("errorMessage", out var errorMessageProp)) ||
                    jsonDoc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    return true;
                }
            }
            return false;
        }
        private void TurnJSONErrorIntoMessageBoxAndLog(String output)
        {
            if (checkIfOutputIsJSONErrorMessage(output))
            {
                isErrorMessageShown = true; // Set the flag to true to indicate an error message has been shown
                var jsonDoc = System.Text.Json.JsonDocument.Parse(output);

                string title = "Error";
                string errorMessage = "";

                // Check for new format first (title + errorMessage)
                if (jsonDoc.RootElement.TryGetProperty("title", out var titleProp) &&
                    jsonDoc.RootElement.TryGetProperty("errorMessage", out var errorMessageProp))
                {
                    title = titleProp.GetString();
                    errorMessage = errorMessageProp.GetString();
                }
                // Check for old format (error)
                else if (jsonDoc.RootElement.TryGetProperty("error", out var errorProp))
                {
                    errorMessage = errorProp.GetString();
                }

                Logger.Log($"AI Error - {errorMessage}", Logger.LogTypes.Error);
                MessageBox.Show(errorMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private string RewriteOutput(string output)
        {
            // Regex spaghetti to fix common issues in the AI-generated XML output
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
            output = Regex.Replace(output, @"</ClickPlayNote3\s*</ClickPlayNote3>", "</ClickPlayNote3>", RegexOptions.IgnoreCase);
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
            // Another batch of regex spaghetti to fix parameter names
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
                if (ctrl == labelCreating || ctrl == pictureBoxCreating || ctrl == progressBarCreating || 
                    ctrl == labelPoweredByGemini || ctrl == labelWarning)
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

        private void comboBox_ai_model_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedDisplayName = comboBox_ai_model.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedDisplayName) && aiModelMapping.ContainsKey(selectedDisplayName) &&
                (aiModelMapping[selectedDisplayName] != Settings1.Default.preferredAIModel))
            {
                AIModel = aiModelMapping[selectedDisplayName];
                Logger.Log($"AI Model changed to: {selectedDisplayName}", Logger.LogTypes.Info);
                Settings1.Default.preferredAIModel = AIModel;
                Settings1.Default.Save();
            }
        }

        private void CreateMusicWithAI_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void connectionCheckTimer_Tick(object sender, EventArgs e)
        {
            // No connection, no AI music generation
            if (!IsInternetAvailable())
            {
                cts.Cancel();
                generatedFilename = string.Empty; // Clear filename on internet failure
                output = String.Empty; // Clear output on internet failure
                connectionCheckTimer.Stop();
                SetControlsEnabledAndMakeLoadingVisible(true);
                ShowNoInternetMessage();
                this.Close();
            }
            else if (!IsServerUp())
            {
                cts.Cancel();
                generatedFilename = string.Empty; // Clear filename on server failure
                output = String.Empty; // Clear output on server failure
                connectionCheckTimer.Stop();
                SetControlsEnabledAndMakeLoadingVisible(true);
                ShowServerDownMessage();
                this.Close();
            }
        }
        private void ShowNoInternetMessage()
        {
            Logger.Log("Internet connection is not available. Please check your connection.", Logger.LogTypes.Error);
            MessageBox.Show(Resources.MessageNoInternet, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ShowServerDownMessage()
        {
            Logger.Log("Google Gemini server is not reachable. Please try again later.", Logger.LogTypes.Error);
            MessageBox.Show(Resources.GoogleGeminiServerDown, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CreateMusicWithAI_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!isCreatedAnything)
            {
                if (!cts.IsCancellationRequested)
                { 
                    cts.Cancel(); // Cancel any ongoing AI requests
                }
                generatedFilename = string.Empty; // Clear filename if the form is closed if anything aren't created properly
                output = String.Empty; // Clear output if the form is closed if anything aren't created properly
            }
        }
    }
}