using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoBleeper
{
    public static class NotificationUtils
    {
        private static Form baseForm; // Parent form for the notification
        private static NotifyIcon primaryNotifyIcon; // Primary notify icon for the application
        private static Form savedSenderForm; // Form that sends the notification
        public static void SetPrimaryNotifyIcon(Form parentForm, NotifyIcon notifyIcon)
        {
            baseForm = parentForm;
            primaryNotifyIcon = notifyIcon; // Set the primary notify icon when main window is shown
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static bool IsWindowObscured(Form targetForm) // Check if the target form is obscured by other windows
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            return foregroundWindow != targetForm?.Handle; // Returns true if another window is in the foreground
        }
        public static void CreateAndShowNotification(Form senderForm, string title, string message, ToolTipIcon iconType = ToolTipIcon.Info, int duration = 5000)
        {
            if (primaryNotifyIcon == null)
                return; // No notify icon available to show notification
            // Show notification only if the application window is obscured
            if (baseForm != null)
            {
                if(string.IsNullOrEmpty(title) && string.IsNullOrEmpty(message))
                    return; // No content to show in notification
                if(string.IsNullOrEmpty(title))
                    title = " "; // Prevent empty title
                if(string.IsNullOrEmpty(message))
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
        public static void CreateAndShowNotificationIfObscured(Form senderForm, string title, string message, ToolTipIcon iconType = ToolTipIcon.Info, int duration = 5000)
        {
            // Show notification only if the application window is obscured
            if (senderForm != null && IsWindowObscured(senderForm))
            {
                CreateAndShowNotification(senderForm, title, message, iconType, duration);
            }
        }
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
