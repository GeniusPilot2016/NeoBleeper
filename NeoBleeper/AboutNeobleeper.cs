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
    public partial class AboutNeobleeper : Form
    {
        bool darkTheme = false;
        public AboutNeobleeper()
        {
            InitializeComponent();
            UIFonts.SetFonts(this);
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            lbl_version.Text = $"Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}";
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
            BackColor = Color.FromArgb(32, 32, 32);
            ForeColor = Color.White;
            lbl_name.ForeColor = SystemColors.ControlText;
            lbl_version.ForeColor = SystemColors.ControlText;
            label1.ForeColor = Color.White;
            button_visit_icons8.BackColor = Color.FromArgb(32, 32, 32);
            button_view_license_text.BackColor = Color.FromArgb(32, 32, 32);
            button_explore_and_star_on_github.BackColor = Color.FromArgb(32, 32, 32);
            listView1.BackColor = Color.Black;
            listView1.ForeColor = Color.White;
            foreach (ListViewItem item in listView1.Items)
            {
                item.BackColor = Color.Black;
                item.ForeColor = Color.White;
            }
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }

        private void LightTheme()
        {
            darkTheme = false;
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            lbl_name.ForeColor = SystemColors.ControlText;
            lbl_version.ForeColor = SystemColors.ControlText;
            label1.ForeColor = SystemColors.ControlText;
            button_visit_icons8.BackColor = Color.Transparent;
            button_view_license_text.BackColor = Color.Transparent;
            button_explore_and_star_on_github.BackColor = Color.Transparent;
            listView1.BackColor = SystemColors.Window;
            listView1.ForeColor = SystemColors.WindowText;
            foreach (ListViewItem item in listView1.Items)
            {
                item.BackColor = SystemColors.Window;
                item.ForeColor = SystemColors.WindowText;
            }
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }


        private void button_view_license_text_Click(object sender, EventArgs e)
        {
            GNUGPLV3LicenseText gnuGplV3LicenseText = new GNUGPLV3LicenseText();
            gnuGplV3LicenseText.ShowDialog();
        }
        private void button_visit_icons8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://icons8.com/") { UseShellExecute = true });
        }

        private void button_explore_and_star_on_github_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/GeniusPilot2016/NeoBleeper") { UseShellExecute = true });
        }

        private void about_neobleeper_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }

        private void richTextBox_credit_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.LinkText))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.LinkText) { UseShellExecute = true });
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    int selectedIndex = listView1.SelectedItems[0].Index;
                    string url = string.Empty; // Initialize URL variable for any social media links of contributors
                    switch (selectedIndex)
                    {
                        case 0: // GeniusPilot2016 (the main developer of NeoBleeper)
                            url = "https://www.github.com/GeniusPilot2016";
                            break;
                        case 1: // Robbi-985 (aka SomethingUnreal) (the original developer of Bleeper Music Maker, the predecessor of NeoBleeper with VB6)
                            url = "https://www.youtube.com/@SomethingUnreal";
                            break;
                        case 2: // M084MM3D (the user who reported the system speaker issue in some chipsets and completed missing data about can computers that have system speaker, but doesn't listed as PNP0800 device)
                            url = "https://www.youtube.com/@M084MM3D";
                            break;
                    }
                    if (!string.IsNullOrEmpty(url))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                    }
                }
            }
        }
    }
}
