using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class LyricsOverlay : Form
    {
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
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!string.IsNullOrWhiteSpace(_currentLyrics))
            {
                var g = e.Graphics;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                using var font = uiFonts.SetUIFont(32f, FontStyle.Regular);
                Color textColor = SystemColors.ControlText;
                Color backColor = SystemColors.ControlLight;
                Color borderColor = Color.LightGray;

                switch (Settings1.Default.theme) 
                {
                    case 0: // System
                        textColor = check_system_theme.IsDarkTheme() == true ? Color.White : SystemColors.ControlText;
                        backColor = check_system_theme.IsDarkTheme() == true ? Color.FromArgb(40, 40, 40) : SystemColors.ControlLight;
                        borderColor = check_system_theme.IsDarkTheme() == true ? Color.Gray : Color.LightGray;
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
                SizeF textSize = g.MeasureString(_currentLyrics, font);
                int padding = 15;
                Rectangle rect = new Rectangle(
                    (ClientSize.Width - (int)textSize.Width - padding * 2) / 2,
                    ClientSize.Height - (int)textSize.Height - padding * 2 - 100, // Alt kısımda, yukarıdan 100px
                    (int)textSize.Width + padding * 2,
                    (int)textSize.Height + padding * 2);

                // Background
                using (var backBrush = new SolidBrush(backColor))
                    g.FillRectangle(backBrush, rect);

                // Border
                using (var borderPen = new Pen(borderColor, 2))
                    g.DrawRectangle(borderPen, rect);

                // Text
                using (var textBrush = new SolidBrush(textColor))
                    g.DrawString(_currentLyrics, font, textBrush, rect.X + padding, rect.Y + padding);
            }
            else
            {
                e.Graphics.Clear(Color.Lime);
            }
        }
    }
}
