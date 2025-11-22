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

using NeoBleeper.Properties;
using static UIHelper;

namespace NeoBleeper
{
    public partial class synchronized_play_window : Form
    {
        bool darkTheme = false;
        bool waiting = false;
        bool is_playing = false;
        private main_window mainWindow;
        private PreciseTimer preciseTimer;

        public synchronized_play_window(main_window mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            this.mainWindow.MusicStopped += MainWindow_MusicStopped;
            dateTimePicker1.Value = DateTime.Now.AddMinutes(1);
            lbl_current_system_time.Text = DateTime.Now.ToString("HH:mm:ss");
            UIFonts.setFonts(this);
            set_theme();
            preciseTimer = new PreciseTimer(1); // Store as field
            preciseTimer.Tick += preciseTimer_Tick;
            preciseTimer.Start();
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
        private void synchronized_play_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop and dispose the timer before closing
            preciseTimer?.Stop();
            preciseTimer?.Dispose();
            preciseTimer = null;
            if (mainWindow != null)
            {
                mainWindow.MusicStopped -= MainWindow_MusicStopped;
            }
        }
        private void preciseTimer_Tick(object sender, EventArgs e)
        {
            if (waiting)
            {
                if (mainWindow.listViewNotes.Items.Count > 0 && !is_playing)
                {
                    var targetTime = dateTimePicker1.Value.ToUniversalTime();
                    var currentTime = DateTime.UtcNow;

                    // Check if the target time has been reached or exceeded
                    if (isCurrentTimeIsEqualOrGreaterThan())
                    {
                        Logger.Log($"Target time reached! Starting music immediately.", Logger.LogTypes.Info);
                        Logger.Log($"Target: {targetTime:HH:mm:ss.fff}, Current: {currentTime:HH:mm:ss.fff}", Logger.LogTypes.Info);

                        // Start playing music and update the UI
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)start_playing);
                        }
                        else
                        {
                            start_playing();
                        }
                    }
                }
                else
                {
                    StopWaiting();
                }
            }
        }
        private void MainWindow_MusicStopped(object sender, EventArgs e)
        {
            stop_playing();
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
            groupBox_time.ForeColor = Color.White;
            groupBox_position.ForeColor = Color.White;
            button_wait.BackColor = Color.FromArgb(32, 32, 32);
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            groupBox_time.ForeColor = SystemColors.ControlText;
            groupBox_position.ForeColor = SystemColors.ControlText;
            button_wait.BackColor = Color.Transparent;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                SafeBeginInvoke(() =>
                {
                    SuspendLayout();
                    string current_time = DateTime.Now.ToString("HH:mm:ss");
                    lbl_current_system_time.Text = current_time;
                    ResumeLayout(performLayout: true);
                });
            });
        }

        private async void start_playing()
        {
            if (waiting)
            {
                var targetTime = dateTimePicker1.Value.ToUniversalTime();
                var actualStartTime = DateTime.UtcNow;
                var startDelay = (actualStartTime - targetTime).TotalMilliseconds;

                Logger.Log($"Music started at {actualStartTime:HH:mm:ss.fff}", Logger.LogTypes.Info);
                Logger.Log($"Target was {targetTime:HH:mm:ss.fff}", Logger.LogTypes.Info);
                Logger.Log($"Start delay: {startDelay}ms", Logger.LogTypes.Info);

                is_playing = true;
                waiting = false;

                // Update UI
                UpdateUIForPlaying();

                // Start music playback
                if (radioButton_play_beginning_of_music.Checked)
                {
                    mainWindow.play_all();
                }
                else if (radioButton_play_currently_selected_line.Checked)
                {
                    mainWindow.play_from_selected_line();
                }
            }
        }

        private void UpdateUIForPlaying()
        {
            SafeBeginInvoke(() =>
            {
                SuspendLayout();
                button_wait.Text = Resources.TextStopPlaying;
                lbl_waiting.Text = Resources.TextPlaying;
                lbl_waiting.BackColor = Color.Yellow;
                ResumeLayout(performLayout: true);
            });
        }
        private bool isCurrentTimeIsEqualOrGreaterThan()
        {
            int Hour = dateTimePicker1.Value.Hour;
            int Minute = dateTimePicker1.Value.Minute;
            int Second = dateTimePicker1.Value.Second;
            int currentHour = DateTime.Now.Hour;
            int currentMinute = DateTime.Now.Minute;
            int currentSecond = DateTime.Now.Second;
            if (currentHour <= 12)
            {
                return (Hour < currentHour) ||
                       (Hour == currentHour && Minute < currentMinute) ||
                       (Hour == currentHour && Minute == currentMinute && Second <= currentSecond);
            }
            else
            {
                return (Hour == currentHour && Minute < currentMinute) ||
                       (Hour == currentHour && Minute == currentMinute && Second <= currentSecond);
            }
        }

        private async void stop_playing()
        {
            try
            {
                if (is_playing)
                {
                    // Disable the event handler to prevent the music from stopping again
                    mainWindow.MusicStopped -= MainWindow_MusicStopped;
                    mainWindow.stop_playing(); // Stop the music
                    mainWindow.MusicStopped += MainWindow_MusicStopped;
                }
                is_playing = false;
                waiting = false; // Set waiting to false
                UpdateUIForStopped(); // Update the UI to reflect that music has stopped
            }
            catch (Exception ex)
            {
                MessageForm.Show(Resources.MessageAnErrorOccurredWhileStoppingMusic + ex.Message);
            }
        }
        private void SafeBeginInvoke(Action action)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            try
            {
                // Use BeginInvoke to avoid cross-thread operation exceptions
                this.BeginInvoke((MethodInvoker)delegate { if (!this.IsDisposed) action(); });
            }
            catch (ObjectDisposedException) { /* ignore */ }
        }
        private void UpdateUIForStopped()
        {
            SafeBeginInvoke(() =>
            {
                SuspendLayout();
                dateTimePicker1.Enabled = true;
                button_wait.Text = Resources.TextStartWaiting;
                lbl_waiting.Text = Resources.TextCurrentlyNotWaiting;
                lbl_waiting.BackColor = Color.Red;
                ResumeLayout(true);
            });
        }
        private async void button_wait_Click(object sender, EventArgs e)
        {
            if (waiting == false && mainWindow.listViewNotes.Items.Count > 0)
            {
                dateTimePicker1.Enabled = false; // Disable the date time picker while waiting
                waiting = true;
                if (mainWindow.is_music_playing)
                {
                    mainWindow.stop_playing(); // Stop the music if it is playing
                }
                if (dateTimePicker1.Value.ToUniversalTime() <= DateTime.UtcNow)
                {
                    start_playing();
                }
                else
                {
                    Logger.Log("Waiting for " + dateTimePicker1.Value.ToString("HH:mm:ss") + " to start playing music", Logger.LogTypes.Info);
                    SafeBeginInvoke(() =>
                    {
                        SuspendLayout();
                        button_wait.Text = Properties.Resources.TextStopWaiting;
                        lbl_waiting.Text = Properties.Resources.TextCurrentlyWaiting;
                        lbl_waiting.BackColor = Color.Lime;
                        ResumeLayout(performLayout: true);
                    });
                }
            }
            else
            {
                StopWaiting(); // Stop waiting if already waiting or no music is available
            }
        }
        private void StopWaiting()
        {
            dateTimePicker1.Enabled = true; // Enable the date time picker when not waiting
            if (is_playing == true || mainWindow.is_music_playing)
            {
                mainWindow.stop_playing();
            }
            waiting = false;
            Logger.Log("Stopped waiting for music to play", Logger.LogTypes.Info);
            SafeBeginInvoke(() =>
            {
                SuspendLayout();
                button_wait.Text = Resources.TextStartWaiting;
                lbl_waiting.Text = Resources.TextCurrentlyNotWaiting;
                lbl_waiting.BackColor = Color.Red;
                ResumeLayout(performLayout: true);
            });
        }
        private void radioButton_play_beginning_of_music_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_play_beginning_of_music.Checked)
            {
                Logger.Log("Playing from beginning of music is checked", Logger.LogTypes.Info);
            }
        }

        private void radioButton_play_currently_selected_line_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_play_currently_selected_line.Checked)
            {
                Logger.Log("Playing from currently selected line is checked", Logger.LogTypes.Info);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Logger.Log($"Date time picker value changed to {dateTimePicker1.Value}", Logger.LogTypes.Info);
        }

        private void synchronized_play_window_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void synchronized_play_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.checkBox_synchronized_play.Checked = false;
        }
    }
}
