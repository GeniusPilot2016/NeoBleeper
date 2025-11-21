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

namespace NeoBleeper
{
    public partial class LyricsOverlay : Form
    {
        // Lyrics in MIDI file player? yeah, why not :)
        // Also, this overlay can be used for karaoke.
        public LyricsOverlay()
        {
            InitializeComponent();
            var screen = Screen.PrimaryScreen;
            var workingArea = screen.WorkingArea; // Working area excludes taskbar
            this.Bounds = workingArea;
        }
        string lyrics = string.Empty;
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
                return cp;
            }
        }
        private string _currentLyrics = string.Empty;

        public void PrintLyrics(string lyrics)
        {
            _currentLyrics = lyrics ?? string.Empty;
            Invalidate(); // Trigger a repaint
        }
        public void ClearLyrics()
        {
            _currentLyrics = string.Empty;
            Invalidate(); // Trigger a repaint
        }
        UIFonts uiFonts = UIFonts.Instance;

        private void LyricsOverlay_Paint(object sender, PaintEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_currentLyrics))
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                using var font = uiFonts.SetUIFont(Settings1.Default.lyricsSize, FontStyle.Regular);
                Color textColor = SystemColors.ControlText;
                Color backColor = SystemColors.ControlLight;
                Color borderColor = Color.LightGray;

                switch (Settings1.Default.theme)
                {
                    case 0: // System
                        textColor = SystemThemeUtility.IsDarkTheme() == true ? Color.White : SystemColors.ControlText;
                        backColor = SystemThemeUtility.IsDarkTheme() == true ? Color.FromArgb(40, 40, 40) : SystemColors.ControlLight;
                        borderColor = SystemThemeUtility.IsDarkTheme() == true ? Color.Gray : Color.LightGray;
                        break;
                    case 1: // Light
                        textColor = SystemColors.ControlText;
                        backColor = SystemColors.ControlLight;
                        borderColor = Color.LightGray;
                        break;
                    case 2: // Dark
                        textColor = Color.White;
                        backColor = Color.FromArgb(40, 40, 40);
                        borderColor = Color.Gray;
                        break;
                }

                int padding = 15;
                int maxBoxWidth = (int)(ClientSize.Width * 0.8); // Width up to 80% of the screen
                int maxBoxHeight = (int)(ClientSize.Height * 0.25); // Height up to 25% of the screen.

                // Measurement of the full text
                SizeF fullSize = g.MeasureString(_currentLyrics, font, maxBoxWidth);

                string displayText = _currentLyrics;

                // If the full text exceeds the max height, trim it
                if (fullSize.Height + padding * 2 > maxBoxHeight)
                {
                    var lines = _currentLyrics.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    var visibleLines = new List<string>();

                    // Add lines from the end until it exceed max height
                    for (int i = lines.Length - 1; i >= 0; i--)
                    {
                        visibleLines.Insert(0, lines[i]);
                        string candidate = string.Join(Environment.NewLine, visibleLines);
                        SizeF candidateSize = g.MeasureString(candidate, font, maxBoxWidth);

                        if (candidateSize.Height + padding * 2 > maxBoxHeight)
                        {
                            // If adding this line exceeds max height, remove it and stop
                            visibleLines.RemoveAt(0);
                            break;
                        }
                    }

                    if (visibleLines.Count == 0)
                    {
                        // Find the longest suffix that fits if even a single line is too tall
                        string s = _currentLyrics;
                        int lo = 0, hi = s.Length;
                        while (lo < hi)
                        {
                            int mid = (lo + hi + 1) / 2;
                            string candidate = s.Substring(s.Length - mid);
                            SizeF candidateSize = g.MeasureString(candidate, font, maxBoxWidth);
                            if (candidateSize.Height + padding * 2 <= maxBoxHeight) lo = mid; else hi = mid - 1;
                        }
                        displayText = lo > 0 ? _currentLyrics.Substring(_currentLyrics.Length - lo) : string.Empty;
                    }
                    else
                    {
                        // Show the collected lines
                        displayText = string.Join(Environment.NewLine, visibleLines);

                        // Ensure it fits (in case of very long single lines)
                        while (true)
                        {
                            SizeF candidateSize = g.MeasureString(displayText, font, maxBoxWidth);
                            if (candidateSize.Height + padding * 2 <= maxBoxHeight) break;

                            var parts = displayText.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                            if (parts.Count <= 1)
                            {
                                displayText = string.Empty;
                                break;
                            }

                            // Remove the oldest line
                            parts.RemoveAt(0);
                            displayText = string.Join(Environment.NewLine, parts);
                        }
                    }
                }

                // Last measurement for the display text
                SizeF textSize = g.MeasureString(displayText, font, maxBoxWidth);

                Rectangle rect = new Rectangle(
                    (ClientSize.Width - (int)textSize.Width - padding * 2) / 2,
                    ClientSize.Height - (int)textSize.Height - padding * 2 - 100,
                    (int)textSize.Width + padding * 2,
                    (int)textSize.Height + padding * 2);

                // Background
                using (var backBrush = new SolidBrush(backColor))
                    g.FillRectangle(backBrush, rect);

                // Border
                using (var borderPen = new Pen(borderColor, 2))
                    g.DrawRectangle(borderPen, rect);

                // Text (end of line aligned)
                using (var textBrush = new SolidBrush(textColor))
                    g.DrawString(displayText, font, textBrush,
                        new RectangleF(rect.X + padding, rect.Y + padding, maxBoxWidth, textSize.Height));
            }
            else
            {
                e.Graphics.Clear(Color.Lime);
            }
        }
    }
}