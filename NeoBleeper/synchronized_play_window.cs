using System.Diagnostics;
using System.Drawing.Text;

namespace NeoBleeper
{
    public partial class synchronized_play_window : Form
    {
        bool waiting = false;
        bool is_playing = false;
        private main_window mainWindow;

        public synchronized_play_window(main_window mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.mainWindow.MusicStopped += MainWindow_MusicStopped;
            dateTimePicker1.Value = DateTime.Now.AddMinutes(1);
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
                    if (ctrl is GroupBox groupBox)
                    {
                        foreach (Control groupedCtrl in groupBox.Controls)
                        {
                            groupedCtrl.Font = uiFonts.SetUIFont(groupedCtrl.Font.Size, groupedCtrl.Font.Style);
                        }
                    }
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
            if (waiting == true && is_playing == false)
            {
                if (dateTimePicker1.Value.ToUniversalTime() <= DateTime.UtcNow)
                {
                    start_playing();
                }
            }
            string current_time = DateTime.Now.ToString("HH:mm:ss");
            lbl_current_system_time.Text = current_time;
        }

        private async void start_playing()
        {
            if (waiting == true)
            {
                if (dateTimePicker1.Value.ToUniversalTime() <= DateTime.UtcNow)
                {
                    is_playing = true;
                    
                    waiting = false; // Set waiting to false after starting to play
                    Task.Run(() =>
                    {
                        this.SuspendLayout();
                        Debug.WriteLine("Starting to play music at " + DateTime.Now.ToString("HH:mm:ss"));
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                button_wait.Text = "Stop playing";
                                lbl_waiting.Text = "Playing";
                                lbl_waiting.BackColor = Color.Yellow;
                            });
                        }
                        else
                        {
                            button_wait.Text = "Stop playing";
                            lbl_waiting.Text = "Playing";
                            lbl_waiting.BackColor = Color.Yellow;
                        }
                        Debug.WriteLine("Playing music");
                        this.ResumeLayout();
                    });
                    // Call the play function
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
        }
        private void synchronized_play_window_Load(object sender, EventArgs e)
        {

        }

        private void synchronized_play_window_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
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
                Task.Run(() => {
                    this.SuspendLayout();
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            dateTimePicker1.Enabled = true; // Enable the date time picker
                            button_wait.Text = "Start waiting";
                            lbl_waiting.Text = "Currently not waiting";
                            lbl_waiting.BackColor = Color.Red;
                        });
                    }
                    else
                    {
                        dateTimePicker1.Enabled = true; // Enable the date time picker
                        button_wait.Text = "Start waiting";
                        lbl_waiting.Text = "Currently not waiting";
                        lbl_waiting.BackColor = Color.Red;
                    }
                    this.ResumeLayout();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while stopping the music: " + ex.Message);
            }
        }
        private async void button_wait_Click(object sender, EventArgs e)
        {
            if (waiting == false)
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
                        this.SuspendLayout();
                        Debug.WriteLine("Waiting for " + dateTimePicker1.Value.ToString("HH:mm:ss") + " to start playing music");
                        if (this.InvokeRequired)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                button_wait.Text = "Stop waiting";
                                lbl_waiting.Text = "Currently waiting";
                                lbl_waiting.BackColor = Color.Lime;
                            });
                        }
                        else
                        {
                            button_wait.Text = "Stop waiting";
                            lbl_waiting.Text = "Currently waiting";
                            lbl_waiting.BackColor = Color.Lime;
                        }
                        this.ResumeLayout();
                    });
                }
            }
            else
            {
                dateTimePicker1.Enabled = true; // Enable the date time picker when not waiting
                if (is_playing == true || mainWindow.is_music_playing)
                {
                    mainWindow.stop_playing();
                }
                waiting = false;
                Task.Run(() =>
                {
                    this.SuspendLayout();
                    Debug.WriteLine("Stopped waiting for music to play");
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            button_wait.Text = "Start waiting";
                            lbl_waiting.Text = "Currently not waiting";
                            lbl_waiting.BackColor = Color.Red;
                        });
                    }
                    else
                    {
                        button_wait.Text = "Start waiting";
                        lbl_waiting.Text = "Currently not waiting";
                        lbl_waiting.BackColor = Color.Red;
                    }
                    this.ResumeLayout();
                });
            }
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
    }
}
