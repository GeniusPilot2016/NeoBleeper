using NeoBleeper.Properties;
using System.Diagnostics;
using static UIHelper;

namespace NeoBleeper
{
    public partial class GoogleGeminiTermsOfServiceAgreement : Form
    {
        string unformattedText = string.Empty;
        private DateTime dateOfBirth = DateTime.Now.AddYears(-13);
        bool agreedTermsOfServiceAgreement = false;
        bool darkTheme = false;
        public GoogleGeminiTermsOfServiceAgreement(Form owner)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            this.Owner = owner;
            UIFonts.SetFonts(this);
            dateTimePickerDateOfBirth.MaxDate = DateTime.Now.AddYears(-13);
            dateTimePickerDateOfBirth.Value = DateTime.Now.AddYears(-13).Date;
            if (!string.IsNullOrEmpty(Settings1.Default.cachedGoogleGeminiTermsOfService))
            {
                richTextBoxTerms.Text = Settings1.Default.cachedGoogleGeminiTermsOfService;
            }
            unformattedText = richTextBoxTerms.Text;
            FormatTerms();
            SetTheme();
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

        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            buttonClose.BackColor = Color.FromArgb(32, 32, 32);
            richTextBoxTerms.BackColor = Color.Black;
            richTextBoxTerms.ForeColor = Color.White;
            this.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            buttonClose.BackColor = Color.Transparent;
            richTextBoxTerms.BackColor = SystemColors.Window;
            richTextBoxTerms.ForeColor = SystemColors.WindowText;
            this.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        /// <summary>
        /// Applies the current application theme to the control and updates the user interface accordingly.
        /// </summary>
        /// <remarks>This method selects and applies a light or dark theme based on user settings and
        /// system preferences. It also ensures that the control's layout and rendering are updated to reflect the new
        /// theme. This method should be called whenever the theme setting changes to ensure consistent
        /// appearance.</remarks>
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
                richTextBoxTerms.Text = unformattedText;
                FormatTerms();
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
        }
        private async Task DownloadActualTermsOfService()
        {
            string formerText = richTextBoxTerms.Text;
            if (await CreateMusicWithAI.IsInternetAvailable())
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        string url = "https://ai.google.dev/gemini-api/terms.md.txt";
                        string termsText = await httpClient.GetStringAsync(url);
                        unformattedText = termsText;
                        Settings1.Default.cachedGoogleGeminiTermsOfService = termsText;
                        Settings1.Default.Save();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("An error occurred while downloading actual terms of service text: " + ex.Message, Logger.LogTypes.Error);
                    unformattedText = formerText;
                    Settings1.Default.cachedGoogleGeminiTermsOfService = formerText;
                    Settings1.Default.Save();
                }
            }
            else
            {
                Logger.Log("No internet connection.", Logger.LogTypes.Error);
            }
        }

        /// <summary>
        /// Formats the unformatted terms text and updates the terms display with rich text formatting.
        /// </summary>
        /// <remarks>This method converts the current unformatted terms, assumed to be in Markdown format,
        /// into rich text and updates the associated display control. Call this method after modifying the unformatted
        /// terms to ensure the display reflects the latest content.</remarks>
        private void FormatTerms()
        {
            richTextBoxTerms.Rtf = MarkdownToRichTextFile(unformattedText);
        }

        /// <summary>
        /// Converts a Markdown-formatted string to a Rich Text Format (RTF) document string.
        /// </summary>
        /// <remarks>The conversion supports common Markdown features, including headings, bold, italics,
        /// strikethrough, lists, blockquotes, code, and links. Images are represented as text with their alt text and
        /// URL, as RTF does not support embedded images. Some HTML tags commonly found in Markdown output are also
        /// recognized and converted. The output is intended for basic RTF rendering and may not preserve all advanced
        /// Markdown or HTML features.</remarks>
        /// <param name="markdown">The Markdown-formatted text to convert. May include standard Markdown elements such as headings, lists,
        /// emphasis, links, and images.</param>
        /// <returns>A string containing the equivalent content in Rich Text Format (RTF). The returned string can be used with
        /// RTF-compatible controls or saved as an RTF file.</returns>
        private string MarkdownToRichTextFile(string markdown)
        {
            // Beginning of RTF document
            var rtf = new System.Text.StringBuilder();
            rtf.Append(@"{\rtf1\ansi ");

            // Blockquote
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"^> (.*)$",
                @"{\i\cf2 $1}",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            // Strikethrough
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"~~(.+?)~~",
                @"{\strike $1}");

            // Horizontal rule
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"^(\s*)(---|\*\*\*)\s*$",
                @"{\par\qr\brdrb\brdrs\brdrw10\par}",
                System.Text.RegularExpressions.RegexOptions.Multiline);

            // Inline image (shows alt text and URL, as RTF can't embed images directly)
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"!\[(.*?)\]\((.*?)\)",
                @"[Image: $1]($2)");

            // Nested bullet list (basic support)
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"^(\s*)[-*] (.*)$",
                m =>
                {
                    int indent = m.Groups[1].Value.Length;
                    return new string(' ', indent * 2) + "• " + m.Groups[2].Value;
                },
                System.Text.RegularExpressions.RegexOptions.Multiline);

            // Nested numbered list (basic support)
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"^(\s*)(\d+)\. (.*)$",
                m =>
                {
                    int indent = m.Groups[1].Value.Length;
                    return new string(' ', indent * 2) + $"{m.Groups[2].Value}. {m.Groups[3].Value}";
                },
                System.Text.RegularExpressions.RegexOptions.Multiline);

            // Escaped characters
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"\\([*_`~>])",
                "$1");

            // Markdown conversions
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^# (.*)$", @"{\b\fs40 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^## (.*)$", @"{\b\fs32 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^### (.*)$", @"{\b\fs24 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^#### (.*)$", @"{\b\fs20 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^##### (.*)$", @"{\b\fs16 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"\*\*(.+?)\*\*", @"{\b $1}");
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"_(.+?)_", @"{\i $1}");
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"__(.+?)__", @"{\ul $1}");
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"`(.+?)`", @"{\f1 $1}");
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"\[(.+?)\]\((.+?)\)",
                @"{\field{\*\fldinst{HYPERLINK ""$2""}}{\fldrslt{$1}}}");
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^\s*[-*] (.*)$", @"• $1", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^\s*(\d+)\. (.*)$", @"$1. $2", System.Text.RegularExpressions.RegexOptions.Multiline);

            // Convert new lines to RTF line breaks
            markdown = markdown.Replace("\r\n", @"\par ").Replace("\n", @"\par ");

            // HTML conversions 
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<h1>(.*?)</h1>", @"{\b\fs40 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<h2>(.*?)</h2>", @"{\b\fs32 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<h3>(.*?)</h3>", @"{\b\fs24 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<b>(.*?)</b>", @"{\b $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<i>(.*?)</i>", @"{\i $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<u>(.*?)</u>", @"{\ul $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<code>(.*?)</code>", @"{\f1 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"<a\s+href=[""'](.*?)[""'].*?>(.*?)</a>",
                @"{\field{\*\fldinst{HYPERLINK ""$1""}}{\fldrslt{$2}}}",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<li>(.*?)</li>", @"• $1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<ol>\s*(<li>.*?</li>\s*)+</ol>", match =>
            {
                var items = System.Text.RegularExpressions.Regex.Matches(match.Value, @"<li>(.*?)</li>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                var result = new System.Text.StringBuilder();
                for (int i = 0; i < items.Count; i++)
                {
                    result.AppendFormat("{0}. {1}\r\n", i + 1, items[i].Groups[1].Value);
                }
                return result.ToString();
            }, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Convert new lines to RTF line breaks for HTML
            markdown = markdown.Replace("\r\n", @"\par ").Replace("\n", @"\par ");
            markdown = markdown.Replace("\r", @"\par ");
            markdown = markdown.Replace("<br>", @"\par ").Replace("<br/>", @"\par ").Replace("<br />", @"\par ");
            markdown = markdown.Replace("<p>", @"\par ").Replace("</p>", @"\par ");

            // Trim extra breaks in beginning and end of rich text
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^(\\par\s*)+", string.Empty);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"(\\par\s*)+$", string.Empty);

            // End of RTF document
            rtf.Append(markdown);
            rtf.Append("}");
            return rtf.ToString();
        }

        /// <summary>
        /// Displays the Google Gemini Terms of Service agreement dialog and executes the specified action if the user
        /// agrees and meets the age requirement; otherwise, executes the rejection action.
        /// </summary>
        /// <remarks>This method prompts the user to accept the Google Gemini Terms of Service. If the
        /// user is under 18 years of age or declines the agreement, the specified rejection action is invoked. The
        /// method is synchronous and blocks until the user responds to the dialog.</remarks>
        /// <param name="action">The action to perform if the user agrees to the terms of service and is at least 18 years old.</param>
        /// <param name="rejectAction">The action to perform if the user does not agree to the terms of service or does not meet the age
        /// requirement.</param>
        public static void AskToAgreeTermsAndDoAction(Form owner, Action action, Action rejectAction)
        {
            GoogleGeminiTermsOfServiceAgreement agreement = new GoogleGeminiTermsOfServiceAgreement(owner);
            agreement.ShowDialog();
            if (agreement.agreedTermsOfServiceAgreement)
            {
                if (agreement.dateOfBirth.AddYears(18) > DateTime.Now)
                {
                    MessageForm.Show(owner, Resources.AISettingsAgeRestrictionWarning, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rejectAction();
                }
                else
                {
                    action(); // Run the action if agreed and age is above 18
                }
            }
            else
            {
                rejectAction(); // Run the reject action if not agreed
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            agreedTermsOfServiceAgreement = checkBoxAccept.Checked;
            dateOfBirth = dateTimePickerDateOfBirth.Value;
            Settings1.Default.googleGeminiTermsOfServiceAccepted = agreedTermsOfServiceAgreement && dateOfBirth.AddYears(18).Date < DateTime.Now;
            Settings1.Default.Save();
            this.Close();
        }

        private async void GoogleGeminiTermsOfServiceAgreement_Load(object sender, EventArgs e)
        {
            richTextBoxTerms.SuspendLayout();
            await DownloadActualTermsOfService();
            richTextBoxTerms.Text = unformattedText;
            FormatTerms();
            richTextBoxTerms.ResumeLayout();
        }

        private void GoogleGeminiTermsOfServiceAgreement_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }

        private void richTextBoxTerms_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.LinkText,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to open link: " + ex.Message, Logger.LogTypes.Error);
            }
        }
    }
}
