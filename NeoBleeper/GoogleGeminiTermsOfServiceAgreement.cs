using NeoBleeper.Properties;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static UIHelper;

namespace NeoBleeper
{
    public partial class GoogleGeminiTermsOfServiceAgreement : Form
    {
        private readonly List<(int Start, int Length, string Url)> _links = new();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref CHARFORMAT2 lParam);

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
        private void InsertLink(string text, string url)
        {
            int start = richTextBoxTerms.TextLength;
            richTextBoxTerms.AppendText(text);

            // Select and format the link
            richTextBoxTerms.Select(start, text.Length);
            richTextBoxTerms.SelectionColor = darkTheme != true ? Color.Blue : SystemColors.MenuHighlight;
            richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Underline);

            // Save link info for click handling
            _links.Add((start, text.Length, url));

            // Select end of text
            richTextBoxTerms.Select(richTextBoxTerms.TextLength, 0);
            richTextBoxTerms.SelectionColor = richTextBoxTerms.ForeColor;
            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
        }
        private void RichTextBoxTerms_MouseClick(object? sender, MouseEventArgs e)
        {
            try
            {
                // Get character index from mouse position
                int charIndex = richTextBoxTerms.GetCharIndexFromPosition(e.Location);

                // Check if the character index is within any link range
                var info = _links.FirstOrDefault(x => charIndex >= x.Start && charIndex < x.Start + x.Length);
                if (!string.IsNullOrEmpty(info.Url))
                {
                    Process.Start(new ProcessStartInfo(info.Url) { UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to open link (mouse click): " + ex.Message, Logger.LogTypes.Error);
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
            SendMessage(richTextBoxTerms.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

            _links.Clear();
            richTextBoxTerms.Clear();

            var lines = unformattedText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            int i = 0;

            while (i < lines.Length)
            {
                var line = lines[i];

                // Code blocks (```)
                if (line.TrimStart().StartsWith("```"))
                {
                    i++;
                    var codeLines = new List<string>();
                    while (i < lines.Length && !lines[i].TrimStart().StartsWith("```"))
                    {
                        codeLines.Add(lines[i]);
                        i++;
                    }

                    richTextBoxTerms.SelectionFont = new Font("Consolas", richTextBoxTerms.Font.Size, FontStyle.Regular);
                    richTextBoxTerms.SelectionBackColor = darkTheme ? Color.FromArgb(45, 45, 45) : Color.FromArgb(240, 240, 240);
                    richTextBoxTerms.AppendText(string.Join(Environment.NewLine, codeLines) + Environment.NewLine);
                    richTextBoxTerms.SelectionBackColor = richTextBoxTerms.BackColor;
                    i++;
                    continue;
                }

                // Block quotes (>)
                if (line.TrimStart().StartsWith(">"))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Italic);
                    richTextBoxTerms.SelectionColor = darkTheme ? Color.LightGray : Color.Gray;
                    ProcessInlineMarkdown(line.TrimStart().Substring(1).Trim());
                    richTextBoxTerms.AppendText(Environment.NewLine);
                    richTextBoxTerms.SelectionColor = richTextBoxTerms.ForeColor;
                    i++;
                    continue;
                }

                // Headers
                if (line.StartsWith("# "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font.FontFamily, 16, FontStyle.Bold);
                    ProcessInlineMarkdown(line.Substring(2));
                    richTextBoxTerms.AppendText(Environment.NewLine);
                    i++;
                    continue;
                }
                else if (line.StartsWith("## "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font.FontFamily, 14, FontStyle.Bold);
                    ProcessInlineMarkdown(line.Substring(3));
                    richTextBoxTerms.AppendText(Environment.NewLine);
                    i++;
                    continue;
                }
                else if (line.StartsWith("### "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font.FontFamily, 12, FontStyle.Bold);
                    ProcessInlineMarkdown(line.Substring(4));
                    richTextBoxTerms.AppendText(Environment.NewLine);
                    i++;
                    continue;
                }

                // Bullet lists
                var trimmed = line.TrimStart();
                if (trimmed.StartsWith("- ") || trimmed.StartsWith("* "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Regular);
                    richTextBoxTerms.AppendText("• ");
                    ProcessInlineMarkdown(trimmed.Substring(2));
                    richTextBoxTerms.AppendText(Environment.NewLine);
                    i++;
                    continue;
                }

                // Numbered lists
                if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^\d+\.\s"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(trimmed, @"^(\d+\.\s)(.*)");
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Regular);
                    richTextBoxTerms.AppendText(match.Groups[1].Value);
                    ProcessInlineMarkdown(match.Groups[2].Value);
                    richTextBoxTerms.AppendText(Environment.NewLine);
                    i++;
                    continue;
                }

                // Horizontal rule
                if (line.Trim() == "---" || line.Trim() == "***" || line.Trim() == "___")
                {
                    richTextBoxTerms.AppendText("─────────────────────────────────────" + Environment.NewLine);
                    i++;
                    continue;
                }

                // Regular text with inline formatting
                if (!string.IsNullOrWhiteSpace(line))
                {
                    richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                    ProcessInlineMarkdown(line);
                }

                richTextBoxTerms.AppendText(Environment.NewLine);
                i++;
            }

            richTextBoxTerms.SelectionStart = 0;
            richTextBoxTerms.SelectionLength = 0;
            SendMessage(richTextBoxTerms.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            richTextBoxTerms.Invalidate();
            richTextBoxTerms.BeginInvoke(new Action(() => richTextBoxTerms.ScrollToCaret()));
        }

        private void ProcessInlineMarkdown(string text)
        {
            // Parse inline elements: links, bold, italic, code, strikethrough
            var regex = new System.Text.RegularExpressions.Regex(
                @"(\[([^\]]+)\]\(([^)]+)\))" +  // [text](url)
                @"\*\*\*(.+?)\*\*\*|" +         // ***bold italic***
                @"\*\*(.+?)\*\*|" +              // **bold**
                @"__(.+?)__|" +                  // __underline__ (custom)
                @"\*(.+?)\*|" +                  // *italic*
                @"_(.+?)_|" +                    // _italic_
                @"~~(.+?)~~|" +                  // ~~strikethrough~~
                @"`([^`]+)`"                     // `code`
            );

            int lastIndex = 0;
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(text))
            {
                // Add text before match
                if (match.Index > lastIndex)
                {
                    richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                    richTextBoxTerms.SelectionColor = richTextBoxTerms.ForeColor;
                    richTextBoxTerms.AppendText(text.Substring(lastIndex, match.Index - lastIndex));
                }

                // Link [text](url)
                if (!string.IsNullOrEmpty(match.Groups[1].Value))
                {
                    if(richTextBoxTerms.TextLength > 0 && match.Groups[1].Value.StartsWith(" "))
                    {
                        richTextBoxTerms.AppendText(" ");
                    }
                    InsertLink(match.Groups[2].Value, match.Groups[3].Value);
                    if(richTextBoxTerms.TextLength > 0 && match.Groups[1].Value.EndsWith(" "))
                    {
                        richTextBoxTerms.AppendText(" ");
                    }
                }
                // ***bold italic***
                else if (!string.IsNullOrEmpty(match.Groups[3].Value))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Bold | FontStyle.Italic);
                    richTextBoxTerms.AppendText(match.Groups[3].Value);
                }
                // **bold**
                else if (!string.IsNullOrEmpty(match.Groups[4].Value))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Bold);
                    richTextBoxTerms.AppendText(match.Groups[4].Value);
                }
                // __underline__
                else if (!string.IsNullOrEmpty(match.Groups[5].Value))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Underline);
                    richTextBoxTerms.AppendText(match.Groups[5].Value);
                }
                // *italic* or _italic_
                else if (!string.IsNullOrEmpty(match.Groups[6].Value))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Italic);
                    richTextBoxTerms.AppendText(match.Groups[6].Value);
                }
                else if (!string.IsNullOrEmpty(match.Groups[7].Value))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Italic);
                    richTextBoxTerms.AppendText(match.Groups[7].Value);
                }
                // ~~strikethrough~~
                else if (!string.IsNullOrEmpty(match.Groups[8].Value))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Strikeout);
                    richTextBoxTerms.AppendText(match.Groups[8].Value);
                }
                // `code`
                else if (!string.IsNullOrEmpty(match.Groups[9].Value))
                {
                    var prevFont = richTextBoxTerms.SelectionFont;
                    richTextBoxTerms.SelectionFont = new Font("Consolas", richTextBoxTerms.Font.Size, FontStyle.Regular);
                    richTextBoxTerms.SelectionBackColor = darkTheme ? Color.FromArgb(60, 60, 60) : Color.FromArgb(245, 245, 245);
                    richTextBoxTerms.AppendText(match.Groups[9].Value);
                    richTextBoxTerms.SelectionBackColor = richTextBoxTerms.BackColor;
                }

                lastIndex = match.Index + match.Length;
            }

            // Add remaining text
            if (lastIndex < text.Length)
            {
                richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                richTextBoxTerms.SelectionColor = richTextBoxTerms.ForeColor;
                richTextBoxTerms.AppendText(text.Substring(lastIndex));
            }
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
            await DownloadActualTermsOfService();
            richTextBoxTerms.Text = unformattedText;
            FormatTerms();
        }

        private void GoogleGeminiTermsOfServiceAgreement_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void richTextBoxTerms_MouseMove(object sender, MouseEventArgs e)
        {
            int charIndex = richTextBoxTerms.GetCharIndexFromPosition(e.Location);
            var info = _links.FirstOrDefault(x => charIndex >= x.Start && charIndex < x.Start + x.Length);

            if (!string.IsNullOrEmpty(info.Url))
            {
                if (richTextBoxTerms.Cursor != Cursors.Hand)
                {
                    richTextBoxTerms.Cursor = Cursors.Hand;
                }
            }
            else
            {
                if (richTextBoxTerms.Cursor != Cursors.IBeam)
                {
                    richTextBoxTerms.Cursor = Cursors.IBeam;
                }
            }
        }

        private void richTextBoxTerms_MouseLeave(object sender, EventArgs e)
        {
            richTextBoxTerms.Cursor = Cursors.IBeam;
        }
    }
}
