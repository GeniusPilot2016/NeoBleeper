using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace NeoBleeper
{
    public partial class disable_create_beep_from_sound_card_warning : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public disable_create_beep_from_sound_card_warning()
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                }
            }
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
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            button_yes.BackColor = Color.FromArgb(32, 32, 32);
            button_no.BackColor = Color.FromArgb(32, 32, 32);
            this.Refresh();
        }

        private void light_theme()
        {
            Application.DoEvents();
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            button_yes.BackColor = Color.Transparent;
            button_no.BackColor = Color.Transparent;
            this.Refresh();
        }

        private void button_yes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Dispose();
        }
        private void button_no_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Dispose();
        }
    }
}
