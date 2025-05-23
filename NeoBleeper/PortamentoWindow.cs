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
    public partial class PortamentoWindow : Form
    {
        public PortamentoWindow(main_window main_Window)
        {
            InitializeComponent();
            setFonts();
            set_theme();
        }
        private void setFonts()
        {
            UIFonts uiFonts = UIFonts.Instance;
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = uiFonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
                }
                foreach (Control childCtrl in groupBox1.Controls)
                {
                    if (childCtrl.Controls != null)
                    {
                        childCtrl.Font = uiFonts.SetUIFont(childCtrl.Font.Size, childCtrl.Font.Style);
                    }
                }
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
        private void light_theme()
        {
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            foreach(Control ctrl in Controls)
            {
                if(ctrl.Controls != null)
                {
                    if(ctrl is GroupBox groupBox)
                    {
                        ctrl.BackColor = SystemColors.Control;
                        ctrl.ForeColor = SystemColors.ControlText;
                        foreach (Control childCtrl in groupBox.Controls)
                        {
                            childCtrl.BackColor = SystemColors.Control;
                            childCtrl.ForeColor = SystemColors.ControlText;
                        }
                    }
                    else
                    {
                        ctrl.BackColor = SystemColors.Control;
                        ctrl.ForeColor = SystemColors.ControlText;
                    }
                }
            }
        }
        private void dark_theme()
        {
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    if (ctrl is GroupBox groupBox)
                    {
                        ctrl.BackColor = Color.FromArgb(32, 32, 32);
                        ctrl.ForeColor = Color.White; 
                        foreach (Control childCtrl in groupBox.Controls)
                        {
                            childCtrl.BackColor = Color.FromArgb(32, 32, 32);
                            childCtrl.ForeColor = Color.White;
                        }
                    }
                    else
                    {
                        ctrl.BackColor = Color.FromArgb(32, 32, 32);
                        ctrl.ForeColor = Color.White;
                    }
                }
            }
        }

        private void trackBarLength_Scroll(object sender, EventArgs e)
        {
            labelLength.Text = trackBarLength.Value.ToString() + " mS";
        }
    }
}
