using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public partial class SplashScreen : Form
    {
        bool completed = false;
        bool willTerminated = false; // Flag to indicate if termination is in progress when form is closed
        public SplashScreen()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            UIFonts.SetFonts(this);
            labelVersion.Text = $"Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}\r\n";
            AddShadowToBackOfForm();
            RoundCornersOfForm();
        }
        /// <summary>
        /// Updates the status message and progress indicator for the operation.
        /// </summary>
        /// <remarks>If called from a thread other than the UI thread, this method marshals the update to
        /// the UI thread. The method ensures the UI remains responsive during status updates.</remarks>
        /// <param name="status">The status message to display to the user. Cannot be null.</param>
        /// <param name="percent">The amount, as a percentage, to increment the progress indicator. Must be between 0 and 100. Defaults to 0.</param>
        /// <param name="completed">A value indicating whether the operation is complete. If <see langword="true"/>, the progress indicator is
        /// set to 100%. Defaults to <see langword="false"/>.</param>
        public void UpdateStatus(string status, int percent = 0, bool completed = false)
        {
            if (!this.IsHandleCreated || this.IsDisposed || willTerminated) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateStatus(status, percent, completed)));
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

        /// <summary>
        /// Processes Windows messages and waits for the specified number of milliseconds, allowing the application to
        /// remain responsive during the wait period.
        /// </summary>
        /// <remarks>This method keeps the application's UI responsive by processing pending Windows
        /// messages during the wait. It is typically used in Windows Forms applications to avoid freezing the UI thread
        /// during short waits. Avoid using this method for long waits or in performance-critical code, as it may impact
        /// application responsiveness and CPU usage.</remarks>
        /// <param name="milliseconds">The number of milliseconds to wait. Must be non-negative.</param>
        public void ResponsiveWait(int milliseconds)
        {
            var sw = Stopwatch.StartNew();
            while ((sw.ElapsedMilliseconds < milliseconds) && !willTerminated)
            {
                Application.DoEvents();
                Thread.Sleep(1);
            }
        }

        private void splash_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveShadowFromBackOfForm();
            if (!completed)
            {
                try
                {
                    willTerminated = true; // Set the flag to indicate termination is in progress to prevent status updates
                    Logger.Log("Startup of application interrupted by user. Closing application...", Logger.LogTypes.Info);
                    notifyIconNeoBleeper.Visible = false; // Hide the tray icon
                    Program.UninitializeMIDI(); // Uninitialize MIDI devices
                    Program.UninitializeExtendedEvents(); // Uninitialize extended events
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
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error during application closure: {ex.Message}", Logger.LogTypes.Error);
                }
                finally
                {
                    Application.Exit(); // Signal application exit
                    Environment.Exit(0); // Ensure the process is terminated immediately
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

        /// <summary>
        /// Ends the current drag operation and resets the dragging state.
        /// </summary>
        private void StopDragging()
        {
            isDragging = false;
        }

        private void splash_Load(object sender, EventArgs e)
        {
            NotificationUtils.SetPrimaryNotifyIcon(this, notifyIconNeoBleeper); // Set the primary notify icon for notifications
        }

        private void notifyIconNeoBleeper_BalloonTipClicked(object sender, EventArgs e)
        {
            NotificationUtils.ActivateWindowWhenShownIconIsClicked(); // Activate the application when the balloon tip is clicked
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void RoundCornersOfForm()
        {
            int dpi = (int)(96 * this.DeviceDpi / 96f); // Get the current DPI
            int radius = 15 * dpi / 96; // Adjust radius based on DPI
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90); // Top-left corner
            path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90); // Top-right corner
            path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90); // Bottom-right corner
            path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90); // Bottom-left corner
            path.CloseFigure();
            this.Region = new Region(path);
        }
        public const int GCL_STYLE = -26;
        public const int CS_DROPSHADOW = 0x00020000;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetClassLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetClassLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Adds a drop shadow effect to the back of the form window.
        /// </summary>
        /// <remarks>This method modifies the window class style to enable a shadow behind the form. The
        /// effect may not be supported on all Windows versions or with certain window styles. Call this method after
        /// the form handle has been created.</remarks>
        private void AddShadowToBackOfForm()
        {
            int classStyle = GetClassLong(this.Handle, GCL_STYLE);
            classStyle |= CS_DROPSHADOW;
            SetClassLong(this.Handle, GCL_STYLE, classStyle);
        }

        /// <summary>
        /// Removes the drop shadow effect from the form's window.
        /// </summary>
        /// <remarks>This method modifies the window class style to disable the drop shadow typically
        /// rendered behind the form. Use this method when you need to remove visual shadow effects, such as for custom
        /// window rendering or to comply with specific UI requirements.</remarks>
        private void RemoveShadowFromBackOfForm()
        {
            int classStyle = GetClassLong(this.Handle, GCL_STYLE);
            classStyle &= ~CS_DROPSHADOW;
            SetClassLong(this.Handle, GCL_STYLE, classStyle);
        }
    }
}
