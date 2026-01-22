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

using System.Xml;
using static UIHelper;

namespace NeoBleeper
{
    public partial class AIGeneratedNBPMLError : Form
    {
        bool darkTheme = false;
        public AIGeneratedNBPMLError(String text)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            WriteToRichTextBoxAndHighlightError(text);
            UIFonts.SetFonts(this);
            richTextBox1.Font = new Font("Consolas", richTextBox1.Font.Size);
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

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method updates the control's appearance to match the selected theme. If the
        /// theme is set to follow the system, the method detects the system's light or dark mode and applies the
        /// corresponding theme. The method also enables double buffering to improve rendering performance and
        /// temporarily suspends layout updates to prevent flickering during the theme change.</remarks>
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
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            label1.ForeColor = Color.White;
            button1.BackColor = Color.FromArgb(32, 32, 32);
            button1.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            label1.ForeColor = SystemColors.ControlText;
            button1.BackColor = Color.Transparent;
            button1.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AIGeneratedNBPMLError_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }

        /// <summary>
        /// Parses the specified XML string, displays it in a RichTextBox, and highlights the entire tag where a
        /// parsing error occurs.
        /// </summary>
        /// <remarks>This method visually distinguishes the first XML parsing error by highlighting the
        /// entire tag containing the error in the RichTextBox. Only the first error encountered is highlighted; subsequent
        /// errors are not marked. The method does not modify the input string.</remarks>
        /// <param name="brokenXML">The XML string to display and analyze for errors. If the string contains invalid XML, the entire tag with the first problematic
        /// character will be highlighted.</param>
        private void WriteToRichTextBoxAndHighlightError(string brokenXML)
        {
            List<(string text, bool isError)> parts = new();
            string xml = brokenXML;

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                parts.Add((xml, false));
            }
            catch (XmlException ex)
            {
                int line = ex.LineNumber;
                int pos = ex.LinePosition;

                int charIndex = 0;
                string[] lines = xml.Split('\n');
                for (int i = 0; i < line - 1 && i < lines.Length; i++)
                    charIndex += lines[i].Length + 1;
                charIndex += pos - 1;

                // Boundary checks
                if (charIndex < 0) charIndex = 0;
                if (charIndex > xml.Length) charIndex = xml.Length;

                int tagStart = xml.LastIndexOf('<', Math.Min(charIndex, xml.Length - 1));
                if (tagStart < 0 || tagStart > xml.Length - 1) tagStart = 0;

                int tagEnd = xml.IndexOf('>', charIndex);
                if (tagEnd < 0 || tagEnd > xml.Length) tagEnd = xml.Length;
                else tagEnd += 1;
                if (tagEnd < tagStart) tagEnd = xml.Length;

                // Last sanity checks
                if (tagStart < 0) tagStart = 0;
                if (tagStart > xml.Length) tagStart = xml.Length;
                if (tagEnd < tagStart) tagEnd = tagStart;
                if (tagEnd > xml.Length) tagEnd = xml.Length;

                string before = tagStart > 0 ? xml.Substring(0, tagStart) : "";
                string errorTag = (tagEnd > tagStart && tagStart < xml.Length) ? xml.Substring(tagStart, tagEnd - tagStart) : "";
                string after = (tagEnd < xml.Length) ? xml.Substring(tagEnd) : "";

                // If no valid tag found, treat entire NBPML (XML) as error
                if (string.IsNullOrEmpty(errorTag))
                {
                    parts.Add((xml, true));
                }
                else
                {
                    if (!string.IsNullOrEmpty(before))
                        parts.Add((before, false));
                    if (!string.IsNullOrEmpty(errorTag))
                        parts.Add((errorTag, true));
                    if (!string.IsNullOrEmpty(after))
                        parts.Add((after, false));
                }
            }

            string EscapeRtf(string s) => s.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}").Replace("\n", "\\line ");
            var rtf = @"{\rtf1\ansi{\colortbl ;\red255\green255\blue255;\red255\green0\blue0;}";
            foreach (var part in parts)
            {
                if (part.isError)
                    rtf += @"{\highlight2\cf2 " + EscapeRtf(part.text) + "}";
                else
                    rtf += EscapeRtf(part.text);
            }
            rtf += "}";

            richTextBox1.Rtf = rtf;
        }
    }
}
