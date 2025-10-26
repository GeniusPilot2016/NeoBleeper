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

namespace NeoBleeper
{
    public partial class about_neobleeper : Form
    {
        bool darkTheme = false;
        public about_neobleeper()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            lbl_version.Text = $"Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}";
            set_theme();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SETTINGCHANGE = 0x001A;
            base.WndProc(ref m);

            if (m.Msg == WM_SETTINGCHANGE)
            {
                set_theme();
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
            }
        }

        private void dark_theme()
        {
            darkTheme = true;
            BackColor = Color.FromArgb(32, 32, 32);
            ForeColor = Color.White;
            lbl_name.ForeColor = SystemColors.ControlText;
            lbl_version.ForeColor = SystemColors.ControlText;
            label1.ForeColor = Color.White;
            button_visit_icons8.BackColor = Color.FromArgb(32, 32, 32);
            button_view_license_text.BackColor = Color.FromArgb(32, 32, 32);
            button_fork_me_on_github.BackColor = Color.FromArgb(32, 32, 32);
            listView1.BackColor = Color.Black;
            listView1.ForeColor = Color.White;
            foreach (ListViewItem item in listView1.Items)
            {
                item.BackColor = Color.Black;
                item.ForeColor = Color.White;
            }
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }

        private void light_theme()
        {
            darkTheme = false;
            BackColor = SystemColors.Control;
            ForeColor = SystemColors.ControlText;
            lbl_name.ForeColor = SystemColors.ControlText;
            lbl_version.ForeColor = SystemColors.ControlText;
            label1.ForeColor = SystemColors.ControlText;
            button_visit_icons8.BackColor = Color.Transparent;
            button_view_license_text.BackColor = Color.Transparent;
            button_fork_me_on_github.BackColor = Color.Transparent;
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
            GNU_GPL_v3_license_text gnu_gpl_v3_license_text = new GNU_GPL_v3_license_text();
            gnu_gpl_v3_license_text.ShowDialog();
        }
        private void button_visit_icons8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://icons8.com/") { UseShellExecute = true });
        }

        private void button_fork_me_on_github_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/GeniusPilot2016/NeoBleeper") { UseShellExecute = true });
        }

        private void about_neobleeper_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void richTextBox_credit_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.LinkText) { UseShellExecute = true });
        }
    }
}
