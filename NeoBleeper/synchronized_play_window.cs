using NeoBleeper.Properties;
using System.Diagnostics;
using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class synchronized_play_window : Form
    {
        bool waiting = false;
        bool is_playing = false;
        private main_window mainWindow;
        private PreciseTimer preciseTimer;

        public synchronized_play_window(main_window mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.mainWindow.MusicStopped += MainWindow_MusicStopped;
            dateTimePicker1.Value = DateTime.Now.AddMinutes(1);
            lbl_current_system_time.Text = DateTime.Now.ToString("HH:mm:ss");
            UIFonts.setFonts(this);
            set_theme();
            preciseTimer = new PreciseTimer(1); // Store as field
            preciseTimer.Tick += preciseTimer_Tick;
            preciseTimer.Start();
        }

        private void synchronized_play_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop and dispose the timer before closing
            preciseTimer?.Stop();
            preciseTimer?.Dispose();
            preciseTimer = null;
            this.Dispose();
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
                        Debug.WriteLine($"Target time reached! Starting music immediately.");
                        Debug.WriteLine($"Target: {targetTime:HH:mm:ss.fff}, Current: {currentTime:HH:mm:ss.fff}");

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
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            groupBox_time.ForeColor = Color.White;
            groupBox_position.ForeColor = Color.White;
            button_wait.BackColor = Color.FromArgb(32, 32, 32);
            this.Refresh();
        }
        private void light_theme()
        {
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            groupBox_time.ForeColor = SystemColors.ControlText;
            groupBox_position.ForeColor = SystemColors.ControlText;
            button_wait.BackColor = Color.Transparent;
            this.Refresh();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        SuspendLayout();
                        string current_time = DateTime.Now.ToString("HH:mm:ss");
                        lbl_current_system_time.Text = current_time;
                        ResumeLayout(performLayout: true);
                    });
                }
                else
                {
                    SuspendLayout();
                    string current_time = DateTime.Now.ToString("HH:mm:ss");
                    lbl_current_system_time.Text = current_time;
                    ResumeLayout(performLayout: true);
                }
            });
        }

        private async void start_playing()
        {
            if (waiting)
            {
                var targetTime = dateTimePicker1.Value.ToUniversalTime();
                var actualStartTime = DateTime.UtcNow;
                var startDelay = (actualStartTime - targetTime).TotalMilliseconds;

                Debug.WriteLine($"Music started at {actualStartTime:HH:mm:ss.fff}");
                Debug.WriteLine($"Target was {targetTime:HH:mm:ss.fff}");
                Debug.WriteLine($"Start delay: {startDelay}ms");

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
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    SuspendLayout();
                    button_wait.Text = Resources.TextStopPlaying;
                    lbl_waiting.Text = Resources.TextPlaying;
                    lbl_waiting.BackColor = Color.Yellow;
                    ResumeLayout(performLayout: true);
                });
            }
            else
            {
                SuspendLayout();
                button_wait.Text = Resources.TextStopPlaying;
                lbl_waiting.Text = Resources.TextPlaying;
                lbl_waiting.BackColor = Color.Yellow;
                ResumeLayout(performLayout: true);
            }
        }
        private bool isCurrentTimeIsEqualOrGreaterThan()
        {
            int Hour = dateTimePicker1.Value.Hour;
            int Minute = dateTimePicker1.Value.Minute;
            int Second = dateTimePicker1.Value.Second;
            int currentHour = DateTime.Now.Hour;
            int currentMinute = DateTime.Now.Minute;
            int currentSecond = DateTime.Now.Second;
            return (Hour < currentHour) ||
                   (Hour == currentHour && Minute < currentMinute) ||
                   (Hour == currentHour && Minute == currentMinute && Second <= currentSecond);
        }
        private void synchronized_play_window_Load(object sender, EventArgs e)
        {

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
                Task.Run(() =>
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            SuspendLayout();
                            dateTimePicker1.Enabled = true; // Enable the date time picker
                            button_wait.Text = Resources.TextStartWaiting;
                            lbl_waiting.Text = Resources.TextCurrentlyNotWaiting;
                            lbl_waiting.BackColor = Color.Red;
                            ResumeLayout(performLayout: true);
                        });
                    }
                    else
                    {
                        SuspendLayout();
                        dateTimePicker1.Enabled = true; // Enable the date time picker
                        button_wait.Text = Resources.TextStartWaiting;
                        lbl_waiting.Text = Resources.TextCurrentlyNotWaiting;
                        lbl_waiting.BackColor = Color.Red;
                        ResumeLayout(performLayout: true);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while stopping the music: " + ex.Message);
            }
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
                    Task.Run(() =>
                    {
                        Debug.WriteLine("Waiting for " + dateTimePicker1.Value.ToString("HH:mm:ss") + " to start playing music");
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                SuspendLayout();
                                button_wait.Text = Properties.Resources.TextStopWaiting;
                                lbl_waiting.Text = Properties.Resources.TextCurrentlyWaiting;
                                lbl_waiting.BackColor = Color.Lime;
                                ResumeLayout(performLayout: true);
                            });
                        }
                        else
                        {
                            SuspendLayout();
                            button_wait.Text = Properties.Resources.TextStopWaiting;
                            lbl_waiting.Text = Properties.Resources.TextCurrentlyWaiting;
                            lbl_waiting.BackColor = Color.Lime;
                            ResumeLayout(performLayout: true);
                        }
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
            Task.Run(() =>
            {
                Debug.WriteLine("Stopped waiting for music to play");
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        SuspendLayout();
                        button_wait.Text = Resources.TextStartWaiting;
                        lbl_waiting.Text = Resources.TextCurrentlyNotWaiting;
                        lbl_waiting.BackColor = Color.Red;
                        ResumeLayout(performLayout: true);
                    });
                }
                else
                {
                    SuspendLayout();
                    button_wait.Text = Resources.TextStartWaiting;
                    lbl_waiting.Text = Resources.TextCurrentlyNotWaiting;
                    lbl_waiting.BackColor = Color.Red;
                    ResumeLayout(performLayout: true);
                }
            });
        }
        private void radioButton_play_beginning_of_music_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_play_beginning_of_music.Checked)
            {
                Debug.WriteLine("Playing from beginning of music is checked");
            }
        }

        private void radioButton_play_currently_selected_line_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_play_currently_selected_line.Checked)
            {
                Debug.WriteLine("Playing from currently selected line is checked");
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine($"Date time picker value changed to {dateTimePicker1.Value}");
        }

        private void synchronized_play_window_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}
