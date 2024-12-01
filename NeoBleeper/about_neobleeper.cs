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
    public partial class about_neobleeper : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public about_neobleeper()
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                    lbl_version.Font = new Font(fonts.Families[0], 14);
                    lbl_name.Font = new Font(fonts.Families[0], 36, FontStyle.Bold);
                }
            }
            listView1.Items.Add("GeniusPilot2016");
            listView1.Items[0].SubItems.Add("Designing and programming");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            get_stickers_window get_stickers = new get_stickers_window();
            get_stickers.ShowDialog();
        }

        private void about_neobleeper_Load(object sender, EventArgs e)
        {

        }

        private void about_neobleeper_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void about_neobleeper_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://icons8.com/") { UseShellExecute = true });
        }
    }
}
