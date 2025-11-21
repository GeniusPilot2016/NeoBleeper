// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using static UIHelper;

namespace NeoBleeper
{
    public partial class play_beat_window : Form
    {
        bool darkTheme = false;
        private main_window mainWindow;
        public play_beat_window(main_window mainWindow)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
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
                        radioButton_play_sound_on_checked_lines.Checked = false;
                        break;
                    }
                case TemporarySettings.BeatTypes.BeatType.PlayOnOddBeats:
                    {
                        radioButton_play_sound_on_all_beats.Checked = false;
                        radioButton_play_sound_on_odd_beats.Checked = true;
                        radioButton_play_sound_on_even_beats.Checked = false;
                        radioButton_play_sound_on_checked_lines.Checked = false;
                        break;
                    }
                case TemporarySettings.BeatTypes.BeatType.PlayOnEvenBeats:
                    {
                        radioButton_play_sound_on_all_beats.Checked = false;
                        radioButton_play_sound_on_even_beats.Checked = true;
                        radioButton_play_sound_on_odd_beats.Checked = false;
                        radioButton_play_sound_on_checked_lines.Checked = false;
                        break;
                    }
                case TemporarySettings.BeatTypes.BeatType.PlayOnCheckedLines:
                    {
                        radioButton_play_sound_on_all_beats.Checked = false;
                        radioButton_play_sound_on_checked_lines.Checked = true;
                        radioButton_play_sound_on_odd_beats.Checked = false;
                        radioButton_play_sound_on_even_beats.Checked = false;
                        break;
                    }
            }
        }

        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme();
                }
            }
        }

        public void set_theme()
        {
            this.SuspendLayout(); // Suspend layout to batch updates
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering

            try
            {
                switch (Settings1.Default.theme)
                {
                    case 0:
                        if (SystemThemeUtility.IsDarkTheme())
                        {
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;

                    case 1:
                        light_theme();
                        break;

                    case 2:
                        dark_theme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
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
            else if (radioButton_play_sound_on_checked_lines.Checked == true)
            {
                TemporarySettings.BeatTypes.beatType = TemporarySettings.BeatTypes.BeatType.PlayOnCheckedLines;
                Logger.Log("Set to play sound on checked lines.", Logger.LogTypes.Info);
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
