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
    public partial class play_beat_window : Form
    {
        public play_beat_window(main_window main_window)
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            set_theme();
            switch (TemporarySettings.BeatTypes.beatType)
            {
                case TemporarySettings.BeatTypes.BeatType.PlayOnAllBeats:
                    {
                        radioButton_play_sound_on_all_beats.Checked = true;
                        radioButton_play_sound_on_odd_beats.Checked = false;
                        radioButton_play_sound_on_even_beats.Checked = false;
                        break;
                    }
                case TemporarySettings.BeatTypes.BeatType.PlayOnOddBeats:
                    {
                        radioButton_play_sound_on_all_beats.Checked = false;
                        radioButton_play_sound_on_odd_beats.Checked = true;
                        radioButton_play_sound_on_even_beats.Checked = false;
                        break;
                    }
                case TemporarySettings.BeatTypes.BeatType.PlayOnEvenBeats:
                    {
                        radioButton_play_sound_on_all_beats.Checked = false;
                        radioButton_play_sound_on_even_beats.Checked = true;
                        radioButton_play_sound_on_odd_beats.Checked = false;
                        break;
                    }
            }
        }
        public void set_theme()
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
            this.Refresh();
        }
        private void light_theme()
        {
            Application.DoEvents();
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            this.Refresh();
        }

        private void beat_types_click(object sender, EventArgs e)
        {
            if (radioButton_play_sound_on_all_beats.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnAllBeats;
            }
            else if (radioButton_play_sound_on_odd_beats.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnOddBeats;
            }
            else if (radioButton_play_sound_on_even_beats.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnEvenBeats;
            }
        }

        private void play_beat_window_Load(object sender, EventArgs e)
        {

        }
    }
}
