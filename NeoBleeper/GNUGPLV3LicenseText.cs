// NeoBleeper - // NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
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

using static UIHelper;

namespace NeoBleeper
{
    public partial class GNUGPLV3LicenseText : Form
    {
        bool darkTheme = false;
        public GNUGPLV3LicenseText()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
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
        /// <remarks>This method selects and applies a light or dark theme according to the user's theme
        /// preference. If the theme is set to follow the system, the method detects the system's current theme and
        /// applies the corresponding style. The method also ensures that UI updates are performed efficiently by
        /// suspending and resuming layout during the operation.</remarks>
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
            this.ForeColor = Color.White;
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ForeColor = Color.White;
            close_button.BackColor = Color.FromArgb(32, 32, 32);
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }

        private void LightTheme()
        {
            darkTheme = false;
            foreach (Control ctrl in Controls)
            {
                ctrl.BackColor = SystemColors.Control;
                ctrl.ForeColor = SystemColors.ControlText;
                richTextBox1.BackColor = SystemColors.Window;
                richTextBox1.ForeColor = SystemColors.WindowText;
                close_button.BackColor = Color.Transparent;
            }
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.LinkText) { UseShellExecute = true });
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GNU_GPL_v3_license_text_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }
    }
}
