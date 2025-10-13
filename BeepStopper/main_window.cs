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
using System.Drawing.Text;

namespace BeepStopper
{
    public partial class main_window : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public main_window()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            UIHelper.ApplyCustomTitleBar(this, Color.White);
            set_theme();
        }
        private SynchronizedSettings synchronizedSettings = SynchronizedSettings.Load();
        int themeIndex = 0;
        private void set_theme()
        {
            switch (synchronizedSettings.Theme)
            {
                case 0:
                    switch (check_system_theme.IsDarkTheme())
                    {
                        case true:
                            dark_theme();
                            break;
                        case false:
                            light_theme();
                            break;
                    }
                    break;
                case 1:
                    light_theme();
                    break;
                case 2:
                    dark_theme();
                    break;
            }
            themeIndex = synchronizedSettings.Theme;
        }
        private void light_theme()
        {
            UIHelper.ApplyCustomTitleBar(this, Color.White, false);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            stopBeepButton.BackColor = Color.Transparent;
            stopBeepButton.ForeColor = SystemColors.ControlText;
        }
        private void dark_theme()
        {
            UIHelper.ApplyCustomTitleBar(this, Color.Black, true);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            stopBeepButton.BackColor = Color.FromArgb(64, 64, 64);
            stopBeepButton.ForeColor = Color.White;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Stop the stuck system speaker beep without force-shutdown the computer by manipulating the 0x61 port (system speaker control port).
            try
            {
                if (SoundRenderingEngine.SystemSpeakerBeepEngine.isSystemSpeakerBeepStuck())
                {
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                    Logger.Log("Stuck system speaker beep is stopped.", Logger.LogTypes.Info);
                    MessageBox.Show(Resources.BeepStoppedMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    Logger.Log("System speaker is not stuck, nothing to stop.", Logger.LogTypes.Warning);
                    MessageBox.Show(Resources.SystemSpeakerIsNotStuckMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Show the error message
                Logger.Log("Error stopping the beep: " + ex.Message, Logger.LogTypes.Error);
                MessageBox.Show(Resources.AnErrorOccurredMessage + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settingsChangeTimer_Tick(object sender, EventArgs e)
        {
            if(synchronizedSettings.Theme != themeIndex)
            {
                set_theme(); // Apply theme if changed
            }
        }

        private void main_window_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme(); // Set theme again when system colors changed
        }
    }
}
