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

using NeoBleeper.Properties;
using static UIHelper;

namespace NeoBleeper
{
    public partial class GetFirmwareWindow : Form
    {
        bool darkTheme = false;
        public GetFirmwareWindow(Form owner)
        {
            InitializeComponent();
            this.Owner = owner;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
            richTextBoxFirmware.Font = new Font("Courier New", richTextBoxFirmware.Font.Size);
            SetTheme();
            comboBoxMicrocontroller.SelectedIndex = 0; // Default to Arduino 
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
        /// <remarks>This method selects and applies a light or dark theme according to the application's
        /// theme settings. If the theme is set to follow the system, the method detects the system's current theme and
        /// applies the corresponding style. The method also ensures that UI updates are performed efficiently and that
        /// the control is rendered smoothly.</remarks>
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
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            comboBoxMicrocontroller.BackColor = SystemColors.Window;
            comboBoxMicrocontroller.ForeColor = SystemColors.WindowText;
            richTextBoxFirmware.BackColor = SystemColors.Window;
            richTextBoxFirmware.ForeColor = SystemColors.WindowText;
            buttonCopyFirmwareToClipboard.BackColor = Color.Transparent;
            buttonCopyFirmwareToClipboard.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            comboBoxMicrocontroller.BackColor = Color.Black;
            comboBoxMicrocontroller.ForeColor = Color.White;
            richTextBoxFirmware.BackColor = Color.Black;
            richTextBoxFirmware.ForeColor = Color.White;
            buttonCopyFirmwareToClipboard.BackColor = Color.FromArgb(32, 32, 32);
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void buttonCopyFirmwareToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxFirmware.Text);
            Toast.ShowToast(this, Resources.MessageFirmwareCopied, 2000);
        }

        private void comboBoxMicrocontroller_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxMicrocontroller.SelectedIndex)
            {
                case 0: // Arduino
                    richTextBoxFirmware.Text = SerialPortHelper.ArduinoFirmwareCode;
                    break;
                case 1: // Raspberry Pi
                    richTextBoxFirmware.Text = SerialPortHelper.RaspberryPiFirmwareCode;
                    break;
                case 2: // ESP32
                    richTextBoxFirmware.Text = SerialPortHelper.ESP32FirmwareCode;
                    break;
            }
        }

        private void GetFirmwareWindow_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme(); // Apply the theme again in case of system theme changes
        }
    }
}
