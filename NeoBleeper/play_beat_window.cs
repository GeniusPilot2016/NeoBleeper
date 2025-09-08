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
    public partial class play_beat_window : Form
    {
        bool darkTheme = false;
        private main_window mainWindow;
        public play_beat_window(main_window mainWindow)
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            set_theme();
            this.mainWindow = mainWindow;
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
            this.Refresh();
        }

        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            TitleBarHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void beat_types_click(object sender, EventArgs e)
        {
            if (radioButton_play_sound_on_all_beats.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnAllBeats;
                Logger.Log("Set to play sound on all beats.", Logger.LogTypes.Info);
            }
            else if (radioButton_play_sound_on_odd_beats.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnOddBeats;
                Logger.Log("Set to play sound on odd beats.", Logger.LogTypes.Info);
            }
            else if (radioButton_play_sound_on_even_beats.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnEvenBeats;
                Logger.Log("Set to play sound on even beats.", Logger.LogTypes.Info);
            }
        }
        private void play_beat_window_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void play_beat_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.checkBox_play_beat_sound.Checked = false;
        }
    }
}
