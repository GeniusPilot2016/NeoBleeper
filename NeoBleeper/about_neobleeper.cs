namespace NeoBleeper
{
    public partial class about_neobleeper : Form
    {
        bool darkTheme = false;
        public about_neobleeper()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            int MajorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            int MinorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int PatchVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;

            string version = $"{MajorVersion}.{MinorVersion}.{PatchVersion}";

            // Extract the status (e.g., "alpha") and capitalize the first letter
            string status = System.Reflection.Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
                .Cast<System.Reflection.AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?.InformationalVersion?.Split('-').Skip(1).FirstOrDefault()?.Split('+')[0] ?? "Release";

            status = char.ToUpper(status[0]) + status.Substring(1);

            lbl_version.Text = $"Version {version} {status}";
            set_theme();
        }
        private void set_theme()
        {
            switch (Settings1.Default.theme)
            {
                case 0:
                    {
                        if (check_system_theme.IsDarkTheme() == true)
                        {
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;
                    }
                case 1:
                    {
                        light_theme();
                        break;
                    }
                case 2:
                    {
                        dark_theme();
                        break;
                    }
            }
        }

        private void dark_theme()
        {
            darkTheme = true;
            Application.DoEvents();
            BackColor = Color.FromArgb(32, 32, 32);
            ForeColor = Color.White;
            lbl_name.ForeColor = Color.White;
            lbl_version.ForeColor = Color.White;
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
            TitleBarHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
            this.Refresh();
        }

        private void light_theme()
        {
            darkTheme = false;
            Application.DoEvents();
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
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
            this.Refresh();
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
    }
}
