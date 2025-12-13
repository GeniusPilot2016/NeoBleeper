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
    public partial class SynchronizedPlayWindow : Form
    {
        bool darkTheme = false;
        bool waiting = false;
        bool isPlaying = false;
        private MainWindow mainWindow;
        private PreciseTimer preciseTimer;

        public SynchronizedPlayWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            this.mainWindow.MusicStopped += MainWindow_MusicStopped;
            dateTimePicker1.Value = DateTime.Now.AddMinutes(1);
            lbl_current_system_time.Text = DateTime.Now.ToString("HH:mm:ss");
            UIFonts.SetFonts(this);
            SetTheme();
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
                    SetTheme();
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
                if (mainWindow.listViewNotes.Items.Count > 0 && !isPlaying)
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
                            this.Invoke((MethodInvoker)StartPlaying);
                        }
                        else
                        {
                            StartPlaying();
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
            StopPlaying();
        }

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method updates the control's appearance to match the selected theme. If the
        /// theme is set to follow the system, the method detects the system's light or dark mode and applies the
        /// corresponding theme. The method also enables double buffering to reduce flicker during the update and forces
        /// a UI refresh to ensure changes take effect immediately.</remarks>
        public void SetTheme()
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
                            DarkTheme();
                        }
                        else
                        {
                            LightTheme();
                        }
                        break;

                    case 1:
                        LightTheme();
                        break;

                    case 2:
                        DarkTheme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
        }

        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            groupBox_time.ForeColor = Color.White;
            groupBox_position.ForeColor = Color.White;
            button_wait.BackColor = Color.FromArgb(32, 32, 32);
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void LightTheme()
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

        /// <summary>
        /// Begins music playback if the application is in a waiting state.
        /// </summary>
        /// <remarks>This method initiates playback based on the user's selection, either from the
        /// beginning of the music or from the currently selected line. It also updates the user interface to reflect
        /// the playing state and logs the actual and target start times. This method is intended to be called
        /// internally and should not be invoked directly from external code.</remarks>
        private async void StartPlaying()
        {
            if (waiting)
            {
                var targetTime = dateTimePicker1.Value.ToUniversalTime();
                var actualStartTime = DateTime.UtcNow;
                var startDelay = (actualStartTime - targetTime).TotalMilliseconds;

                Logger.Log($"Music started at {actualStartTime:HH:mm:ss.fff}", Logger.LogTypes.Info);
                Logger.Log($"Target was {targetTime:HH:mm:ss.fff}", Logger.LogTypes.Info);
                Logger.Log($"Start delay: {startDelay}ms", Logger.LogTypes.Info);

                isPlaying = true;
                waiting = false;

                // Update UI
                UpdateUIForPlaying();

                // Start music playback
                if (radioButton_play_beginning_of_music.Checked)
                {
                    mainWindow.PlayAll();
                }
                else if (radioButton_play_currently_selected_line.Checked)
                {
                    mainWindow.PlayFromSelectedLine();
                }
            }
        }

        /// <summary>
        /// Updates the user interface to reflect the playing state.
        /// </summary>
        /// <remarks>Call this method to update UI elements when playback starts. This method is intended
        /// to be used internally to ensure the UI accurately represents the current playback status.</remarks>
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

        /// <summary>
        /// Determines whether the time selected in the date and time picker is equal to or earlier than the current
        /// system time.
        /// </summary>
        /// <remarks>This method compares only the time components (hour, minute, and second) of the
        /// selected value and the current system time. The date component is not considered in the
        /// comparison.</remarks>
        /// <returns>true if the selected time is equal to or earlier than the current system time; otherwise, false.</returns>
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

        /// <summary>
        /// Stops music playback and updates the user interface to reflect the stopped state.
        /// </summary>
        /// <remarks>If music is currently playing, this method stops playback and ensures that related
        /// event handlers are managed appropriately. After calling this method, the application will no longer be in a
        /// playing or waiting state.</remarks>
        private async void StopPlaying()
        {
            try
            {
                if (isPlaying)
                {
                    // Disable the event handler to prevent the music from stopping again
                    mainWindow.MusicStopped -= MainWindow_MusicStopped;
                    mainWindow.StopPlaying(); // Stop the music
                    mainWindow.MusicStopped += MainWindow_MusicStopped;
                }
                isPlaying = false;
                waiting = false; // Set waiting to false
                UpdateUIForStopped(); // Update the UI to reflect that music has stopped
            }
            catch (Exception ex)
            {
                MessageForm.Show(Resources.MessageAnErrorOccurredWhileStoppingMusic + ex.Message);
            }
        }

        /// <summary>
        /// Invokes the specified action asynchronously on the UI thread, if the control is not disposed and its handle
        /// is created.
        /// </summary>
        /// <remarks>Use this method to safely perform UI updates from a non-UI thread. The method checks
        /// whether the control is disposed or its handle is not created before attempting to invoke the action, helping
        /// to prevent cross-thread operation exceptions. If the control is disposed during invocation, the action will
        /// not be executed.</remarks>
        /// <param name="action">The action to execute on the UI thread. Cannot be null.</param>
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

        /// <summary>
        /// Updates the user interface to reflect that the waiting process has stopped.
        /// </summary>
        /// <remarks>Enables relevant controls and updates status indicators to indicate that the
        /// application is not currently waiting. This method should be called from the UI thread or using a mechanism
        /// that safely marshals the call to the UI thread.</remarks>
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
                if (mainWindow.isMusicPlaying)
                {
                    mainWindow.StopPlaying(); // Stop the music if it is playing
                }
                if (dateTimePicker1.Value.ToUniversalTime() <= DateTime.UtcNow)
                {
                    StartPlaying();
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

        /// <summary>
        /// Stops the waiting state and restores the user interface to allow new actions.
        /// </summary>
        /// <remarks>This method re-enables the date and time picker, updates related UI elements to
        /// indicate that waiting has ended, and stops any ongoing music playback if necessary. It should be called when
        /// the application needs to exit a waiting or monitoring mode and return to normal operation.</remarks>
        private void StopWaiting()
        {
            dateTimePicker1.Enabled = true; // Enable the date time picker when not waiting
            if (isPlaying == true || mainWindow.isMusicPlaying)
            {
                mainWindow.StopPlaying();
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
            SetTheme();
        }

        private void synchronized_play_window_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.checkBox_synchronized_play.Checked = false;
        }
    }
}
