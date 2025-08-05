using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class GNU_GPL_v3_license_text : Form
    {
        bool darkTheme = false;
        public GNU_GPL_v3_license_text()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
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
            Application.DoEvents();
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ForeColor = Color.White;
            close_button.BackColor = Color.FromArgb(32, 32, 32);
            TitleBarHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
            this.Refresh();
        }

        private void light_theme()
        {
            darkTheme = false;
            foreach (Control ctrl in Controls)
            {
                Application.DoEvents();
                ctrl.BackColor = SystemColors.Control;
                ctrl.ForeColor = SystemColors.ControlText;
                richTextBox1.BackColor = SystemColors.Window;
                richTextBox1.ForeColor = SystemColors.WindowText;
                close_button.BackColor = Color.Transparent;
                this.Refresh();
            }
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.LinkText) { UseShellExecute = true });
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GNU_GPL_v3_license_text_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}
