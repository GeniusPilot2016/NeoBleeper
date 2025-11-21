using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            if (CreateMusicWithAI.IsInternetAvailable())
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

            _links.Clear(); // Clear previous links

            var lines = richTextBoxTerms.Lines;
            richTextBoxTerms.Clear();

            foreach (var line in lines)
            {
                int start = richTextBoxTerms.TextLength;

                // Titles
                if (line.StartsWith("# "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font.FontFamily, 16, FontStyle.Bold);
                    richTextBoxTerms.AppendText(line.Substring(2) + Environment.NewLine);
                }
                else if (line.StartsWith("## "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font.FontFamily, 14, FontStyle.Bold);
                    richTextBoxTerms.AppendText(line.Substring(3) + Environment.NewLine);
                }
                else if (line.StartsWith("### "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font.FontFamily, 12, FontStyle.Bold);
                    richTextBoxTerms.AppendText(line.Substring(4) + Environment.NewLine);
                }
                // Bullet list
                else if (line.TrimStart().StartsWith("- ") || line.TrimStart().StartsWith("* "))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Regular);
                    richTextBoxTerms.AppendText("• " + line.TrimStart().Substring(2) + Environment.NewLine);
                }
                // Numbered list
                else if (System.Text.RegularExpressions.Regex.IsMatch(line.TrimStart(), @"^\d+\.\s"))
                {
                    richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Regular);
                    richTextBoxTerms.AppendText(line.TrimStart() + Environment.NewLine);
                }
                // Inline link markdown: [title](url)
                else if (line.Contains("[") && line.Contains("]("))
                {
                    int idx = 0;
                    var regex = new System.Text.RegularExpressions.Regex(@"\[([^\]]+)\]\(([^)]+)\)");
                    var matches = regex.Matches(line);
                    if (matches.Count == 0)
                    {
                        richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                        richTextBoxTerms.AppendText(line + Environment.NewLine);
                    }
                    else
                    {
                        foreach (System.Text.RegularExpressions.Match m in matches)
                        {
                            if (m.Index > idx)
                            {
                                richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                                richTextBoxTerms.AppendText(line.Substring(idx, m.Index - idx));
                            }
                            string title = m.Groups[1].Value;
                            string url = m.Groups[2].Value;
                            InsertLink(title, url);
                            idx = m.Index + m.Length;
                        }
                        if (idx < line.Length)
                        {
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(idx));
                        }
                        richTextBoxTerms.AppendText(Environment.NewLine);
                    }
                }
                // Inline code
                else if (line.Contains("`"))
                {
                    int idx = 0;
                    while (idx < line.Length)
                    {
                        int codeStart = line.IndexOf('`', idx);
                        if (codeStart == -1)
                        {
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(idx));
                            break;
                        }
                        richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                        richTextBoxTerms.AppendText(line.Substring(idx, codeStart - idx));
                        int codeEnd = line.IndexOf('`', codeStart + 1);
                        if (codeEnd == -1)
                        {
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(codeStart));
                            break;
                        }
                        richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Italic);
                        richTextBoxTerms.AppendText(line.Substring(codeStart + 1, codeEnd - codeStart - 1));
                        idx = codeEnd + 1;
                    }
                    richTextBoxTerms.AppendText(Environment.NewLine);
                }
                // Bold and Italic
                else if (line.Contains("**") || line.Contains("_"))
                {
                    int idx = 0;
                    while (idx < line.Length)
                    {
                        int boldStart = line.IndexOf("**", idx);
                        int italicStart = line.IndexOf("_", idx);

                        if ((boldStart == -1) && (italicStart == -1))
                        {
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(idx));
                            break;
                        }

                        if (boldStart != -1 && (italicStart == -1 || boldStart < italicStart))
                        {
                            // Bold
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(idx, boldStart - idx));
                            int boldEnd = line.IndexOf("**", boldStart + 2);
                            if (boldEnd == -1)
                            {
                                richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                                richTextBoxTerms.AppendText(line.Substring(boldStart));
                                break;
                            }
                            richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Bold);
                            richTextBoxTerms.AppendText(line.Substring(boldStart + 2, boldEnd - boldStart - 2));
                            idx = boldEnd + 2;
                        }
                        else
                        {
                            // Italic
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(idx, italicStart - idx));
                            int italicEnd = line.IndexOf("_", italicStart + 1);
                            if (italicEnd == -1)
                            {
                                richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                                richTextBoxTerms.AppendText(line.Substring(italicStart));
                                break;
                            }
                            richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Italic);
                            richTextBoxTerms.AppendText(line.Substring(italicStart + 1, italicEnd - italicStart - 1));
                            idx = italicEnd + 1;
                        }
                    }
                    richTextBoxTerms.AppendText(Environment.NewLine);
                }
                // Underline (custom: __text__)
                else if (line.Contains("__"))
                {
                    int idx = 0;
                    while (idx < line.Length)
                    {
                        int ulStart = line.IndexOf("__", idx);
                        if (ulStart == -1)
                        {
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(idx));
                            break;
                        }
                        richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                        richTextBoxTerms.AppendText(line.Substring(idx, ulStart - idx));
                        int ulEnd = line.IndexOf("__", ulStart + 2);
                        if (ulEnd == -1)
                        {
                            richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                            richTextBoxTerms.AppendText(line.Substring(ulStart));
                            break;
                        }
                        richTextBoxTerms.SelectionFont = new Font(richTextBoxTerms.Font, FontStyle.Underline);
                        richTextBoxTerms.AppendText(line.Substring(ulStart + 2, ulEnd - ulStart - 2));
                        idx = ulEnd + 2;
                    }
                    richTextBoxTerms.AppendText(Environment.NewLine);
                }
                // Regular text
                else
                {
                    richTextBoxTerms.SelectionFont = richTextBoxTerms.Font;
                    richTextBoxTerms.AppendText(line + Environment.NewLine);
                }
            }
            // Scroll to caret
            richTextBoxTerms.SelectionStart = 0;
            richTextBoxTerms.SelectionLength = 0;
            SendMessage(richTextBoxTerms.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            richTextBoxTerms.Invalidate();
            richTextBoxTerms.BeginInvoke(new Action(() => richTextBoxTerms.ScrollToCaret()));
        }
        public static void AskToAgreeTermsAndDoAction(Action action, Action rejectAction)
        {
            GoogleGeminiTermsOfServiceAgreement agreement = new GoogleGeminiTermsOfServiceAgreement();
            agreement.ShowDialog();
            if (agreement.agreedTermsOfServiceAgreement)
            {
                if(agreement.DateOfBirth.AddYears(18) > DateTime.Now)
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
