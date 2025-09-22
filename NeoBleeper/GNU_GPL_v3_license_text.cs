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
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ForeColor = Color.White;
            close_button.BackColor = Color.FromArgb(32, 32, 32);
        }

        private void light_theme()
        {
            darkTheme = false;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
            foreach (Control ctrl in Controls)
            {
                ctrl.BackColor = SystemColors.Control;
                ctrl.ForeColor = SystemColors.ControlText;
                richTextBox1.BackColor = SystemColors.Window;
                richTextBox1.ForeColor = SystemColors.WindowText;
                close_button.BackColor = Color.Transparent;
            }
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
