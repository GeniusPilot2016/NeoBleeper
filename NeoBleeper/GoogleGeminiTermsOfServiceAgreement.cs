using NeoBleeper.Properties;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static UIHelper;

namespace NeoBleeper
{
    public partial class GoogleGeminiTermsOfServiceAgreement : Form
    {
        string unformattedText = string.Empty;
        private const int EM_SETCHARFORMAT = 1092;
        private const int SCF_SELECTION = 1;
        private const uint CFM_LINK = 0x00000020;
        private const uint CFE_LINK = 0x00000020;
        private DateTime DateOfBirth = DateTime.Now.AddYears(-13);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CHARFORMAT2
        {
            public int cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public uint crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szFaceName;
            public ushort wWeight;
            public ushort sSpacing;
            public int crBackColor;
            public int lcid;
            public uint dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }
        bool agreedTermsOfServiceAgreement = false;
        bool darkTheme = false;
        public GoogleGeminiTermsOfServiceAgreement()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.setFonts(this);
            dateTimePickerDateOfBirth.MaxDate = DateTime.Now.AddYears(-13);
            dateTimePickerDateOfBirth.Value = DateTime.Now.AddYears(-13).Date;
            if (!string.IsNullOrEmpty(Settings1.Default.cachedGoogleGeminiTermsOfService))
            {
                richTextBoxTerms.Text = Settings1.Default.cachedGoogleGeminiTermsOfService;
            }
            unformattedText = richTextBoxTerms.Text;
            FormatTerms();
            set_theme();
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

        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            buttonClose.BackColor = Color.FromArgb(32, 32, 32);
            richTextBoxTerms.BackColor = Color.Black;
            richTextBoxTerms.ForeColor = Color.White;
            this.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            buttonClose.BackColor = Color.Transparent;
            richTextBoxTerms.BackColor = SystemColors.Window;
            richTextBoxTerms.ForeColor = SystemColors.WindowText;
            this.ForeColor = SystemColors.ControlText;
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

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0x0B;
        private void FormatTerms()
        {
            richTextBoxTerms.Rtf = MarkdownToRichTextFile(unformattedText);
        }
        private string MarkdownToRichTextFile(string markdown)
        {
            // Beginning of RTF document
            var rtf = new System.Text.StringBuilder();
            rtf.Append(@"{\rtf1\ansi ");

            // Markkdown headers and formatting conversions
            // Headers
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^# (.*)$", @"{\b\fs40 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^## (.*)$", @"{\b\fs32 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^### (.*)$", @"{\b\fs24 $1}", System.Text.RegularExpressions.RegexOptions.Multiline);

            // Bold
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"\*\*(.+?)\*\*", @"{\b $1}");

            // Italic
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"_(.+?)_", @"{\i $1}");

            // Underline
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"__(.+?)__", @"{\ul $1}");

            // Code
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"`(.+?)`", @"{\f1 $1}");

            // Links 
            markdown = System.Text.RegularExpressions.Regex.Replace(
            markdown,
                @"\[(.+?)\]\((.+?)\)",
                @"{\field{\*\fldinst{HYPERLINK ""$2""}}{\fldrslt{$1}}}"
            );

            // Bullet list 
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^\s*[-*] (.*)$", @"• $1", System.Text.RegularExpressions.RegexOptions.Multiline);

            // Numbered list
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"^\s*(\d+)\. (.*)$", @"$1. $2", System.Text.RegularExpressions.RegexOptions.Multiline);

            // Convert new lines to RTF line breaks
            markdown = markdown.Replace("\r\n", @"\par ").Replace("\n", @"\par ");

            // HTML headers and formatting conversions that might be in the markdown
            
            // Headers
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<h1>(.*?)</h1>", @"{\b\fs40 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<h2>(.*?)</h2>", @"{\b\fs32 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<h3>(.*?)</h3>", @"{\b\fs24 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Bold
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<b>(.*?)</b>", @"{\b $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Italic
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<i>(.*?)</i>", @"{\i $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Underline
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<u>(.*?)</u>", @"{\ul $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Code
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<code>(.*?)</code>", @"{\f1 $1}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Links
            markdown = System.Text.RegularExpressions.Regex.Replace(
                markdown,
                @"<a\s+href=[""'](.*?)[""'].*?>(.*?)</a>",
                @"{\field{\*\fldinst{HYPERLINK ""$1""}}{\fldrslt{$2}}}",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            
            // Bullet list
            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"<li>(.*?)</li>", @"• $1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
            // Numbered list
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
        public static void AskToAgreeTermsAndDoAction(Action action, Action rejectAction)
        {
            GoogleGeminiTermsOfServiceAgreement agreement = new GoogleGeminiTermsOfServiceAgreement();
            agreement.ShowDialog();
            if (agreement.agreedTermsOfServiceAgreement)
            {
                if (agreement.DateOfBirth.AddYears(18) > DateTime.Now)
                {
                    MessageForm.Show(Resources.AISettingsAgeRestrictionWarning, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            DateOfBirth = dateTimePickerDateOfBirth.Value;
            Settings1.Default.googleGeminiTermsOfServiceAccepted = agreedTermsOfServiceAgreement && DateOfBirth.AddYears(18).Date < DateTime.Now;
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
            set_theme();
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
