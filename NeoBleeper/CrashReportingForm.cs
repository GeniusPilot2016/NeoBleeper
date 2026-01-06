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
        string crashType;
        string crashMessage;
        string stackTrace;
        DateTime CrashTime;
        bool darkTheme = false;
        public CrashReportingForm(string CrashType, string CrashMessage, string StackTrace)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
            richTextBoxCrashReport.Font = new Font("Courier New", richTextBoxCrashReport.Font.Size);
            SetTheme();
            GenerateCrashReport(CrashType, CrashMessage, StackTrace);
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
        /// <remarks>This method updates the control's appearance according to the selected theme in
        /// application settings. If the theme is set to follow the system, the method applies a dark or light theme
        /// based on the system's current theme preference. The method also enables double buffering to improve
        /// rendering performance and forces a UI update to ensure changes take effect immediately.</remarks>
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
            richTextBoxCrashReport.BackColor = Color.Black;
            richTextBoxCrashReport.ForeColor = Color.White;
            buttonCopyCrashReport.BackColor = Color.FromArgb(32, 32, 32);
            buttonCopyCrashReport.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }


        private void LightTheme()
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

        /// <summary>
        /// Generates a detailed crash report and updates the crash report display with the provided crash information.
        /// </summary>
        /// <remarks>This method also plays a system notification sound to alert the user when a crash
        /// occurs. The generated report includes application version, system information, and the supplied crash
        /// details. The crash report is displayed in the user interface for review or further action.</remarks>
        /// <param name="crashType">The type or category of the crash. Used to identify the nature of the error that occurred.</param>
        /// <param name="crashMessage">A descriptive message explaining the cause or context of the crash.</param>
        /// <param name="stackTrace">The stack trace associated with the crash, providing call sequence details for debugging purposes.</param>
        private void GenerateCrashReport(string crashType, string crashMessage, string stackTrace)
        {
            SystemSounds.Hand.Play(); // Play a sound to notify the user of the crash
            string completeReport = string.Empty;
            CrashTime = DateTime.Now;
            this.crashType = crashType;
            this.crashMessage = crashMessage;
            this.stackTrace = stackTrace;
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
            completeReport += $"Crash Type: {crashType}\r\n";
            completeReport += $"Crash Message: {crashMessage}\r\n";
            completeReport += $"Stack Trace: {stackTrace}\r\n";
            richTextBoxCrashReport.Text = completeReport;
        }

        /// <summary>
        /// Displays a crash report dialog with the specified crash type, message, and stack trace information.
        /// </summary>
        /// <param name="CrashType">The category or type of the crash to display in the report. This value is shown to help identify the nature
        /// of the error.</param>
        /// <param name="CrashMessage">The error message describing the cause or details of the crash. This message is presented to the user in the
        /// crash report dialog.</param>
        /// <param name="StackTrace">The stack trace associated with the crash, providing context for debugging. This information is included in
        /// the crash report dialog.</param>
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
            SetTheme();
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
