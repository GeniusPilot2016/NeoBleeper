﻿using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class MIDIFileLoading: Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public MIDIFileLoading()
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf"); foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                }
                this.SuspendLayout();
            }
        }
    }
}
