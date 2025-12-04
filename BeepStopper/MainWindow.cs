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

using BeepStopper.Properties;
using NeoBleeper;
using System.Diagnostics;
using System.Drawing.Text;
using static UIHelper;

namespace BeepStopper
{
    public partial class MainWindow : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public MainWindow()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
            UIHelper.ApplyCustomTitleBar(this, Color.White);
            SetTheme();
        }

        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                SetTheme();
            }
        }

        int themeIndex = 0;

        private void SetTheme()
        {
            switch (Program.themeIndex)
            {
                case 0:
                    switch (SystemThemeUtility.IsDarkTheme())
                    {
                        case true:
                            DarkTheme();
                            break;
                        case false:
                            LightTheme();
                            break;
                    }
                    break;
                case 1:
                    LightTheme();
                    break;
                case 2:
                    DarkTheme();
                    break;
            }
            themeIndex = Program.themeIndex;
        }
        private void LightTheme()
        {
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            stopBeepButton.BackColor = Color.Transparent;
            stopBeepButton.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, false);
        }
        private void DarkTheme()
        {
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            stopBeepButton.BackColor = Color.FromArgb(32, 32, 32);
            stopBeepButton.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, true);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Stop the stuck system speaker beep without force-shutdown the computer by manipulating the 0x61 port (system speaker control port).
            try
            {
                if (SoundRenderingEngine.SystemSpeakerBeepEngine.IsSystemSpeakerBeepStuck())
                {
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                    Debug.WriteLine("Stuck system speaker beep is stopped.");
                    MessageForm.Show(Program.themeIndex, Resources.BeepStoppedMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    Debug.WriteLine("System speaker is not stuck, nothing to stop.");
                    MessageForm.Show(Program.themeIndex, Resources.SystemSpeakerIsNotStuckMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Show the error message
                Debug.WriteLine("Error stopping the beep: " + ex.Message);
                MessageForm.Show(Program.themeIndex, Resources.AnErrorOccurredMessage + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settingsChangeTimer_Tick(object sender, EventArgs e)
        {
            if (Program.themeIndex != themeIndex)
            {
                SetTheme(); // Apply theme if changed
            }
        }

        private void main_window_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme(); // Set theme again when system colors changed
        }

        private void main_window_Load(object sender, EventArgs e)
        {
            NotificationUtils.SetPrimaryNotifyIcon(this, notifyIconBeepStopper);
        }
    }
}
