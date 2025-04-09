using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class MIDIFileLoading : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public MIDIFileLoading()
        {
            InitializeComponent();
            set_theme();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf"); foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
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
