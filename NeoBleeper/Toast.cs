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
    public partial class Toast : Form
    {
        UIFonts uiFonts = UIFonts.Instance;
        Form parentForm;
        bool darkTheme = false;
        public Toast(Form parentForm, string Message, int duration)
        {
            this.parentForm = parentForm;
            InitializeComponent();
            set_theme();
            labelMessage.Font = uiFonts.SetUIFont(labelMessage.Font.Size, labelMessage.Font.Style);
            labelMessage.Text = Message;
            this.Size = new Size(labelMessage.Width + CalculatePaddingAfterText(20), this.Height);
            PositionToast();
            ShowTimer.Interval = duration;
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
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80; // WS_EX_TOOLWINDOW
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return cp;
            }
        }
        protected override bool ShowWithoutActivation => true;
        private void set_theme()
        {
            this.SuspendLayout();
            switch (Settings1.Default.theme)
            {
                case 0: // System theme
                    switch (SystemThemeUtility.IsDarkTheme())
                    {
                        case true:
                            dark_theme();
                            break;
                        case false:
                            light_theme();
                            break;
                    }
                    break;
                case 1: // Light theme
                    light_theme();
                    break;
                case 2: // Dark theme
                    dark_theme();
                    break;
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.ForeColor = Color.White;
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.ControlLight;
            this.ForeColor = SystemColors.ControlText;
        }
        private void PositionToast()
        {
            int semiWidthOfForm = this.parentForm.Width / 2;
            int semiWidthOfToast = this.Width / 2;
            int xPositionInForm = semiWidthOfForm - semiWidthOfToast;
            int heightOfParentForm = this.parentForm.Height;
            int xPositionOfParent = this.parentForm.Location.X;
            int yPositionOfParent = this.parentForm.Location.Y;
            int heightFromFloor = 50;
            this.Location = new Point(xPositionOfParent + xPositionInForm, yPositionOfParent + heightOfParentForm - heightFromFloor - this.Height);
        }

        private void ShowTimer_Tick(object sender, EventArgs e)
        {
            ShowTimer.Stop();
            Toast_disappear();
        }
        private async void Toast_disappear()
        {
            for (int i = 0; i <= 10; i++)
            {
                this.Opacity -= 0.1;
                await HighPrecisionSleep.SleepAsync(10);
            }
            this.Close();
            this.Dispose();
        }
        private int CalculatePaddingAfterText(int padding)
        {
            double dpi = 96; // Default DPI
            using (Graphics g = this.CreateGraphics())
            {
                dpi = g.DpiX; // Get the actual DPI
            }
            return (int)(padding * (dpi / 96.0));
        }

        private void Toast_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = darkTheme ? Color.Gray : Color.LightGray;
            using (Pen pen = new Pen(borderColor))
            {
                Rectangle rectangle = new Rectangle(1, 1, this.Size.Width - 2, this.Size.Height - 2);
                e.Graphics.DrawRectangle(pen, rectangle);
            }
        }
        public static void ShowToast(Form parentForm, string Message, int duration)
        {
            Toast toast = new Toast(parentForm, Message, duration);
            toast.Show();
        }
    }
}
