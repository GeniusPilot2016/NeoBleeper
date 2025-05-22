using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class MusicCreating: Form
    {
        public MusicCreating()
        {
            InitializeComponent();
            setFonts();
            set_theme();
        }
        private void setFonts()
        {
            foreach (Control ctrl in Controls)
            {
                UIFonts uiFonts = UIFonts.Instance;
                if (ctrl.Controls != null)
                {
                    ctrl.Font = uiFonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
                }
                this.SuspendLayout();
            }
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
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
        }
        private void light_theme()
        {
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
        }
    }
}
