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
        public about_neobleeper()
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
            this.Refresh();
        }

        private void light_theme()
        {
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
            this.Refresh();
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
    }
}
