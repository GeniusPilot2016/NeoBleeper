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

using static UIHelper;

namespace NeoBleeper
{
    public partial class AIGeneratedNBPMLError : Form
    {
        bool darkTheme = false;
        public AIGeneratedNBPMLError(String text)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            richTextBox1.Text = text;
            UIFonts.SetFonts(this);
            richTextBox1.Font = new Font("Consolas", richTextBox1.Font.Size);
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
        /// <remarks>This method updates the control's appearance to match the selected theme. If the
        /// theme is set to follow the system, the method detects the system's light or dark mode and applies the
        /// corresponding theme. The method also enables double buffering to improve rendering performance and
        /// temporarily suspends layout updates to prevent flickering during the theme change.</remarks>
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
            label1.ForeColor = Color.White;
            button1.BackColor = Color.FromArgb(32, 32, 32);
            button1.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            label1.ForeColor = SystemColors.ControlText;
            button1.BackColor = Color.Transparent;
            button1.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AIGeneratedNBPMLError_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }
    }
}
