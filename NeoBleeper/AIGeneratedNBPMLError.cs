using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class AIGeneratedNBPMLError : Form
    {
        public AIGeneratedNBPMLError(String text)
        {
            InitializeComponent();
            richTextBox1.Text = text;
            UIFonts.setFonts(this);
            richTextBox1.Font = new Font("Consolas", richTextBox1.Font.Size);
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
            this.BackColor = Color.FromArgb(32, 32, 32);
            label1.ForeColor = Color.White;
            button1.BackColor = Color.FromArgb(32, 32, 32);
            button1.ForeColor = Color.White;
        }
        private void light_theme()
        {
            this.BackColor = SystemColors.Control;
            label1.ForeColor = SystemColors.ControlText;
            button1.BackColor = Color.Transparent;
            button1.ForeColor = SystemColors.ControlText;
        }
    }
}
