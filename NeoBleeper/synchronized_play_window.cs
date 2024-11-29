﻿using System;
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
    public partial class synchronized_play_window : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public synchronized_play_window()
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            PrivateFontCollection black_font = new PrivateFontCollection(); ;
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                    lbl_hour_minute_second.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
                    lbl_current_time.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
                    lbl_current_system_time.Font = new Font(fonts.Families[0], 15, FontStyle.Bold);
                    lbl_waiting.Font = new Font(fonts.Families[0], 11, FontStyle.Bold);
                    dateTimePicker1.Font = new Font(fonts.Families[0], 11, FontStyle.Bold);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string current_time = DateTime.Now.ToString("HH:mm:ss");
            lbl_current_system_time.Text = current_time;
        }

        private void synchronized_play_window_Load(object sender, EventArgs e)
        {

        }

        private void synchronized_play_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }
    }
}
