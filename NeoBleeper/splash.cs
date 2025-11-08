using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class splash : Form
    {
        bool completed = false;
        public splash()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            UIFonts.setFonts(this);
            labelVersion.Text = $"Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}\r\n";
        }
        public void updateStatus(string status, int percent = 0, bool completed = false)
        {
            if (!this.IsHandleCreated || this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => updateStatus(status, percent, completed)));
                return;
            }
            ResponsiveWait(1000);
            Application.DoEvents();
            labelStatus.Text = status;
            if (completed)
            {
                progressBar1.Value = 100;
            }
            else
            {
                if (progressBar1.Value + percent <= 100)
                {
                    progressBar1.Value += percent;
                }
                else
                {
                    progressBar1.Value = 100;
                }
            }
            Application.DoEvents();
            ResponsiveWait(1000);
            this.completed = completed;
        }
        public void ResponsiveWait(int milliseconds)
        {
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < milliseconds)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }

        private void splash_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!completed)
            {
                try
                {
                    Logger.Log("Startup of application interrupted by user. Closing application...", Logger.LogTypes.Info);
                    Program.UninitializeMIDI(); // Uninitialize MIDI devices
                    if (!(RuntimeInformation.ProcessArchitecture == Architecture.Arm64))
                    {
                        Logger.Log("System speaker beep is being stopped if needed...", Logger.LogTypes.Info);
                        SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeepIfNeeded();
                    }
                    else
                    {
                        Logger.Log("Skipping system speaker beep stop on ARM64 architecture.", Logger.LogTypes.Info);
                    }
                    Logger.Log("Application is closing. Cleanup done.", Logger.LogTypes.Info);
                    Environment.Exit(0); // Force exit the application after cleaning up
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error during application closure: {ex.Message}", Logger.LogTypes.Error);
                }
            }
        }

        Point mouseDownScreenPoint = Point.Empty;
        Point formStartLocation = Point.Empty;
        bool isDragging = false;

        private void splash_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Start dragging
                isDragging = true;
                mouseDownScreenPoint = MousePosition;
                formStartLocation = this.Location;
            }
        }

        private void splash_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Use Cursor.Position for better reliability
                Point currentMousePosition = Cursor.Position;

                // Calculate the difference
                Point diff = new Point(
                    currentMousePosition.X - mouseDownScreenPoint.X,
                    currentMousePosition.Y - mouseDownScreenPoint.Y
                );

                // Update the form's location
                this.Location = new Point(
                    formStartLocation.X + diff.X,
                    formStartLocation.Y + diff.Y
                );
            }
        }

        private void splash_MouseUp(object sender, MouseEventArgs e)
        {
            // Stop dragging
            if (e.Button == MouseButtons.Left)
            {
                StopDragging();
            }
        }

        private void splash_Deactivate(object sender, EventArgs e)
        {
            StopDragging();
        }
        private void StopDragging()
        {
            isDragging = false;
        }
    }
}
