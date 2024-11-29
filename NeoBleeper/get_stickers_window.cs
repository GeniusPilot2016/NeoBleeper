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

namespace NeoBleeper
{
    public partial class get_stickers_window : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public get_stickers_window()
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
        }

        private void get_stickers_window_Load(object sender, EventArgs e)
        {

        }

        private void get_stickers_window_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
