﻿// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
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
        bool darkTheme = false;
        public GetFirmwareWindow()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            richTextBoxFirmware.Font = new Font("Courier New", richTextBoxFirmware.Font.Size);
            set_theme();
            comboBoxMicrocontroller.SelectedIndex = 0; // Default to Arduino 
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
            this.ResumeLayout();
        }
        private void light_theme()
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
        private void dark_theme()
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
