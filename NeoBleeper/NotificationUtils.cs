using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public static class NotificationUtils
    {
        private static Form baseForm; // Parent form for the notification
        private static NotifyIcon primaryNotifyIcon; // Primary notify icon for the application
        private static Form savedSenderForm; // Form that sends the notification

        /// <summary>
        /// Sets the primary notify icon and associates it with the specified parent form.
        /// </summary>
        /// <remarks>Call this method to specify which notify icon should be treated as the application's
        /// primary icon, typically for handling notifications or user interactions from the system tray.</remarks>
        /// <param name="parentForm">The main application form to associate with the primary notify icon. Cannot be null.</param>
        /// <param name="notifyIcon">The notify icon to designate as the primary icon for the application. Cannot be null.</param>
        public static void SetPrimaryNotifyIcon(Form parentForm, NotifyIcon notifyIcon)
        {
            baseForm = parentForm;
            primaryNotifyIcon = notifyIcon; // Set the primary notify icon when main window is shown
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Determines whether the specified form is currently obscured by another window.
        /// </summary>
        /// <remarks>This method compares the specified form's window handle to the current foreground
        /// window. It does not account for partial obscuration or overlapping windows; it only checks if the form is
        /// not the active foreground window.</remarks>
        /// <param name="targetForm">The form to check for obscuration. Cannot be null.</param>
        /// <returns>true if another window is in the foreground and obscuring the specified form; otherwise, false.</returns>
        public static bool IsWindowObscured(Form targetForm) // Check if the target form is obscured by other windows
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            return foregroundWindow != targetForm?.Handle; // Returns true if another window is in the foreground
        }

        /// <summary>
        /// Displays a balloon notification in the system tray using the application's primary notify icon.
        /// </summary>
        /// <remarks>The notification is only shown if the application's primary notify icon is available
        /// and the main window is obscured. If both the title and message are empty or null, no notification is
        /// shown.</remarks>
        /// <param name="senderForm">The form that initiates the notification. This can be used to associate the notification with a specific
        /// window.</param>
        /// <param name="title">The title text to display in the notification balloon. If null or empty, a single space is used to prevent
        /// an empty title.</param>
        /// <param name="message">The message text to display in the notification balloon. If null or empty, a single space is used to prevent
        /// an empty message.</param>
        /// <param name="iconType">The icon to display in the notification balloon. The default is ToolTipIcon.Info.</param>
        /// <param name="duration">The duration, in milliseconds, that the notification balloon is displayed. The default is 5000 milliseconds.</param>
        public static void CreateAndShowNotification(Form senderForm, string title, string message, ToolTipIcon iconType = ToolTipIcon.Info, int duration = 5000)
        {
            if (primaryNotifyIcon == null)
                return; // No notify icon available to show notification
            // Show notification only if the application window is obscured
            if (baseForm != null)
            {
                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(message))
                    return; // No content to show in notification
                if (string.IsNullOrEmpty(title))
                    title = " "; // Prevent empty title
                if (string.IsNullOrEmpty(message))
                    message = " "; // Prevent empty message
                savedSenderForm = senderForm;
                if (baseForm.InvokeRequired)
                {
                    baseForm.Invoke(new Action(() =>
                    {
                        primaryNotifyIcon?.Visible = true; // Ensure the notify icon is visible
                        primaryNotifyIcon?.BalloonTipTitle = title;
                        primaryNotifyIcon?.BalloonTipText = message;
                        primaryNotifyIcon?.BalloonTipIcon = iconType;
                        primaryNotifyIcon?.ShowBalloonTip(duration);
                    }));
                }
                else
                {
                    primaryNotifyIcon?.Visible = true; // Ensure the notify icon is visible
                    primaryNotifyIcon?.BalloonTipTitle = title;
                    primaryNotifyIcon?.BalloonTipText = message;
                    primaryNotifyIcon?.BalloonTipIcon = iconType;
                    primaryNotifyIcon?.ShowBalloonTip(duration);
                }
            }
        }

        /// <summary>
        /// Displays a notification to the user if the specified form is currently obscured by other windows.
        /// </summary>
        /// <remarks>No notification is shown if the form is not obscured. This method is useful for
        /// alerting users to important information when the application window is not visible.</remarks>
        /// <param name="senderForm">The form to check for obscuration and to associate with the notification. Cannot be null.</param>
        /// <param name="title">The title text to display in the notification.</param>
        /// <param name="message">The message content to display in the notification.</param>
        /// <param name="iconType">The icon to display in the notification. The default is ToolTipIcon.Info.</param>
        /// <param name="duration">The duration, in milliseconds, for which the notification is displayed. The default is 5000.</param>
        public static void CreateAndShowNotificationIfObscured(Form senderForm, string title, string message, ToolTipIcon iconType = ToolTipIcon.Info, int duration = 5000)
        {
            // Show notification only if the application window is obscured
            if (senderForm != null && IsWindowObscured(senderForm))
            {
                CreateAndShowNotification(senderForm, title, message, iconType, duration);
            }
        }

        /// <summary>
        /// Brings the application's main window to the foreground when the notification area icon is clicked.
        /// </summary>
        /// <remarks>This method is typically called in response to a user clicking the application's
        /// notification area (system tray) icon. It attempts to activate and bring to the front the main application
        /// window, ensuring it is visible to the user. If no main window is available, the method has no
        /// effect.</remarks>
        public static void ActivateWindowWhenShownIconIsClicked() // Bring the application to the foreground
        {
            if (savedSenderForm == null) return;
            foreach (Form openForm in Application.OpenForms) // Bring all open forms to front
            {
                openForm.BringToFront();
                openForm.Activate();
                break;
            }
            savedSenderForm.BringToFront();
            savedSenderForm.Activate();
        }
    }
}
