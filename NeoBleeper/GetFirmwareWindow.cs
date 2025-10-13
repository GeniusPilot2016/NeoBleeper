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

namespace NeoBleeper
{
    public partial class GetFirmwareWindow : Form
    {
        public GetFirmwareWindow()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            richTextBoxFirmware.Font = new Font("Courier New", richTextBoxFirmware.Font.Size);
            set_theme();
            comboBoxMicrocontroller.SelectedIndex = 0; // Default to Arduino 
        }
        private void set_theme()
        {
            this.SuspendLayout();
            switch (Settings1.Default.theme)
            {
                case 0: // System theme
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
                case 1: // Light theme
                    light_theme();
                    break;
                case 2: // Dark theme
                    dark_theme();
                    break;
            }
            this.ResumeLayout();
        }
        private void light_theme()
        {
            UIHelper.ApplyCustomTitleBar(this, Color.White, false);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            comboBoxMicrocontroller.BackColor = SystemColors.Window;
            comboBoxMicrocontroller.ForeColor = SystemColors.WindowText;
            richTextBoxFirmware.BackColor = SystemColors.Window;
            richTextBoxFirmware.ForeColor = SystemColors.WindowText;
            buttonCopyFirmwareToClipboard.BackColor = Color.Transparent;
            buttonCopyFirmwareToClipboard.ForeColor = SystemColors.ControlText;
        }
        private void dark_theme()
        {
            UIHelper.ApplyCustomTitleBar(this, Color.Black, true);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            comboBoxMicrocontroller.BackColor = Color.Black;
            comboBoxMicrocontroller.ForeColor = Color.White;
            richTextBoxFirmware.BackColor = Color.Black;
            richTextBoxFirmware.ForeColor = Color.White;
            buttonCopyFirmwareToClipboard.BackColor = Color.FromArgb(32, 32, 32);
        }
        private void buttonCopyFirmwareToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxFirmware.Text);
            Toast toast = new Toast(this, Resources.MessageFirmwareCopied, 2000);
            toast.Show();
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
            set_theme(); // Apply the theme again in case of system theme changes
        }
    }
}
