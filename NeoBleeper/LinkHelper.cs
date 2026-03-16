using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoBleeper
{
    public class LinkHelper
    {
        /// <summary>
        /// This method attempts to open a specified URL in the user's default web browser. It checks if the URL is not null or empty before trying to open it. If an error occurs during this process, such as an invalid URL or an issue with the default browser, it logs the error and displays a message box to inform the user of the failure.
        /// </summary>
        /// <param name="url"></param>
        public static void OpenLink(string url, Form form = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to open the link: {ex.Message}", Logger.LogTypes.Error);
                    MessageForm.Show(form, Resources.MessageFailedToOpenLink, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
