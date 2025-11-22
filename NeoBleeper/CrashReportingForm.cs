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
using System.Media;
using static UIHelper;

namespace NeoBleeper
{
    public partial class CrashReportingForm : Form
    {
        string CrashType;
        string CrashMessage;
        string StackTrace;
        DateTime CrashTime;
        bool darkTheme = false;
        public CrashReportingForm(string CrashType, string CrashMessage, string StackTrace)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.setFonts(this);
            richTextBoxCrashReport.Font = new Font("Courier New", richTextBoxCrashReport.Font.Size);
            set_theme();
            GenerateCrashReport(CrashType, CrashMessage, StackTrace);
        }

        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme();
                }
            }
        }
        private void set_theme()
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
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;

                    case 1:
                        light_theme();
                        break;

                    case 2:
                        dark_theme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            richTextBoxCrashReport.BackColor = Color.Black;
            richTextBoxCrashReport.ForeColor = Color.White;
            buttonCopyCrashReport.BackColor = Color.FromArgb(32, 32, 32);
            buttonCopyCrashReport.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }


        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            richTextBoxCrashReport.BackColor = SystemColors.Window;
            richTextBoxCrashReport.ForeColor = SystemColors.WindowText;
            buttonCopyCrashReport.BackColor = Color.Transparent;
            buttonCopyCrashReport.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private void GenerateCrashReport(string CrashType, string CrashMessage, string StackTrace)
        {
            SystemSounds.Hand.Play(); // Play a sound to notify the user of the crash
            string completeReport = string.Empty;
            CrashTime = DateTime.Now;
            this.CrashType = CrashType;
            this.CrashMessage = CrashMessage;
            this.StackTrace = StackTrace;
            completeReport = "\r\n  _   _            ____  _                           \r\n" +
                " | \\ | |          |  _ \\| |                          \r\n" +
                " |  \\| | ___  ___ | |_) | | ___  ___ _ __   ___ _ __ \r\n" +
                " | . ` |/ _ \\/ _ \\|  _ <| |/ _ \\/ _ \\ '_ \\ / _ \\ '__|\r\n" +
                " | |\\  |  __/ (_) | |_) | |  __/  __/ |_) |  __/ |   \r\n" +
                " |_| \\_|\\___|\\___/|____/|_|\\___|\\___| .__/ \\___|_|   \r\n" +
                "                                    | |              \r\n" +
                "                                    |_|              \r\n";
            completeReport += "\nFrom Something Unreal to Open Sound – Reviving the Legacy, One Note at a Time. \r\n";
            completeReport += "\nhttps://github.com/GeniusPilot2016/NeoBleeper \r\n\n";
            completeReport += $"NeoBleeper Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}\r\n";
            completeReport += GetInformations.GlobalSystemInfo;
            completeReport += "Crash Report Generated on: " + CrashTime.ToString() + "\r\n";
            completeReport += "--- NeoBleeper Crash Report ---\r\n";
            completeReport += $"Crash Type: {CrashType}\r\n";
            completeReport += $"Crash Message: {CrashMessage}\r\n";
            completeReport += $"Stack Trace: {StackTrace}\r\n";
            richTextBoxCrashReport.Text = completeReport;
        }
        public static void GenerateAndShowCrashReport(string CrashType, string CrashMessage, string StackTrace)
        {
            CrashReportingForm crashForm = new CrashReportingForm(CrashType, CrashMessage, StackTrace);
            crashForm.ShowDialog();
        }

        private void buttonCopyCrashReport_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxCrashReport.Text);
            Toast.ShowToast(this, Resources.CrashReportCopied, 2000);
        }

        private void CrashReportingForm_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void richTextBoxCrashReport_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.LinkText,
                UseShellExecute = true
            });
        }
    }
}
