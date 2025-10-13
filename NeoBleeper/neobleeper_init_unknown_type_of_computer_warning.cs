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

using System.Media;

namespace NeoBleeper
{
    public partial class neobleeper_init_unknown_type_of_computer_warning : Form
    {
        bool darkTheme = false;
        public neobleeper_init_unknown_type_of_computer_warning()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            set_theme();
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
                        if (check_system_theme.IsDarkTheme())
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
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            button_yes.BackColor = Color.FromArgb(32, 32, 32);
            button_no.BackColor = Color.FromArgb(32, 32, 32);
        }

        private void light_theme()
        {
            darkTheme = false;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            button_yes.BackColor = Color.Transparent;
            button_no.BackColor = Color.Transparent;
        }
        private void button_yes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            if (checkBoxDontShowAgain.Checked)
            {
                Settings1.Default.dont_show_system_speaker_warnings_again = true;
                Settings1.Default.Save();
            }
            this.Dispose();
        }

        private void button_no_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Dispose();
        }

        private void neobleeper_init_unknown_type_of_computer_warning_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void neobleeper_init_unknown_type_of_computer_warning_Shown(object sender, EventArgs e)
        {
            SystemSounds.Exclamation.Play();
        }
    }
}
