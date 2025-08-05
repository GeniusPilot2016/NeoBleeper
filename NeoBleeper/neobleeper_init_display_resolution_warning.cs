using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class neobleeper_init_display_resolution_warning : Form
    {
        bool darkTheme = false;
        public neobleeper_init_display_resolution_warning()
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
            button_close.BackColor = Color.FromArgb(32, 32, 32);
            TitleBarHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
            this.Refresh();
        }

        private void light_theme()
        {
            Application.DoEvents();
            darkTheme = true;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            button_close.BackColor = Color.Transparent;
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
            this.Refresh();
        }

        private void button_close_the_program_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Dispose();
        }

        private void neobleeper_init_display_resolution_warning_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}
