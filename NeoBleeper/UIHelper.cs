using Microsoft.Win32;
using NeoBleeper;
using System.Globalization;
using System.Runtime.InteropServices;
public static class UIHelper
{
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern bool LockWindowUpdate(IntPtr hWndLock);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_9586 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;

    private static int cachedColorRef = -1;
    private static Color cachedColor;

    /// <summary>
    /// Applies a custom color and optional dark mode appearance to the title bar of a Windows Form on supported Windows
    /// versions.
    /// </summary>
    /// <remarks>This method is supported only on Windows 11 (build 22000 and above). On unsupported operating
    /// systems or Windows versions, the method has no effect. The method does not throw exceptions for unsupported
    /// platforms; it simply returns without making changes. The form's handle will be created if it does not already
    /// exist.</remarks>
    /// <param name="form">The form whose title bar appearance will be modified. Must not be disposed or disposing.</param>
    /// <param name="color">The color to apply to the form's title bar.</param>
    /// <param name="darkMode">true to enable dark mode appearance for the title bar; otherwise, false.</param>
    public static void ApplyCustomTitleBar(Form form, Color color, bool darkMode = false)
    {
        if (!OperatingSystem.IsWindows()) // Only works on Windows
        {
            return; // Non-Windows OS are not supported to prevent unwanted behavior
        }
        // Check if the Windows version is Windows 11 and above because title bar color customization is buggy on Windows 10 
        // (although the program doesn't support Windows 8.1 and below, this is just a safety check)
        if (Environment.OSVersion.Version.Major < 10 || (Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build < 22000))
        {
            return; // Windows 10 and below are not supported for title bar color customization
        }
        if (!(form.IsDisposed || form.Disposing))
        {
            // Ensure handle is created
            if (!form.IsHandleCreated)
            {
                form.CreateControl();
            }

            form.SuspendLayout();

            try
            {
                // Force cache invalidation
                cachedColorRef = -1;
                cachedColor = Color.Empty;

                // Convert color to Win32 format
                cachedColor = color;
                cachedColorRef = ColorTranslator.ToWin32(color);

                // STEP 1: Always reset both dark mode attributes first
                int resetValue = 0;
                DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref resetValue, 4);
                DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_9586, ref resetValue, 4);

                // STEP 3: Set the correct dark mode value
                int darkModeValue = darkMode ? 1 : 0;
                DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkModeValue, 4);
                DwmSetWindowAttribute(form.Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_9586, ref darkModeValue, 4);

                // STEP 4: Set the title bar color
                DwmSetWindowAttribute(form.Handle, 35, ref cachedColorRef, 4);

                // STEP 5: Force multiple types of window updates
                SetWindowPos(form.Handle, IntPtr.Zero, 0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);

                // Force immediate window refresh
                form.Refresh();

                // Force non-client area redraw (title bar area)
                SendMessage(form.Handle, 0x0085, IntPtr.Zero, IntPtr.Zero); // WM_NCPAINT

                form.Invalidate(true);
                form.Update();
            }
            finally
            {
                form.ResumeLayout(false);
            }
        }
    }

    /// <summary>
    /// Forces the specified form to immediately repaint its client area and all child controls.
    /// </summary>
    /// <remarks>This method is useful when changes to the form or its controls require an immediate visual
    /// update, bypassing the normal paint scheduling. If the form is null, disposed, or disposing, this method does
    /// nothing.</remarks>
    /// <param name="form">The form to be updated. Must not be null, disposed, or disposing.</param>
    public static void ForceUpdateUI(Form form)
    {
        if (form == null || form.IsDisposed || form.Disposing)
            return;

        form.Invalidate(true);
        form.Update();
    }
    private static readonly Dictionary<string, string> LanguageNameToCulture = new()
    {
        {  "English", "en-US" },
        {  "Deutsch", "de-DE" },
        {  "Español", "es-ES" },
        {  "Français", "fr-FR" },
        {  "Italiano", "it-IT" },
        {  "Türkçe", "tr-TR" },
        {  "Русский", "ru-RU" },
        {  "українська", "uk-UA" },
        {  "Tiếng Việt", "vi-VN" }
    };

    /// <summary>
    /// Sets the application's culture based on the specified human-readable language name.
    /// </summary>
    /// <remarks>If the specified language name is not found or is invalid, the application's culture defaults
    /// to English (United States) ("en-US"). This method affects the default culture for all threads in the application
    /// domain.</remarks>
    /// <param name="languageName">The display name of the language to set as the application's culture. If the name is not recognized or is null
    /// or whitespace, the culture is not changed.</param>
    public static void SetLanguageByName(string languageName) // Set the application's culture based on a human-readable language name
    {
        try
        {
            if (string.IsNullOrWhiteSpace(languageName))
                return;
            if (!LanguageNameToCulture.TryGetValue(languageName, out string cultureCode))
                return;
            CultureInfo culture = new(cultureCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            CultureInfo culture = new("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
        }
    }

    /// <summary>
    /// Calculates the DPI (dots per inch) scale factor for the specified message form relative to the standard 96 DPI.
    /// </summary>
    /// <remarks>This method is useful for adapting UI elements to different display scaling settings. The
    /// returned value can be used to scale sizes or coordinates to match the display's DPI.</remarks>
    /// <param name="messageForm">The message form for which to determine the DPI scale factor. Cannot be null.</param>
    /// <returns>A double representing the scale factor of the form's DPI compared to 96 DPI. A value of 1.0 indicates standard
    /// DPI; values greater or less than 1.0 indicate higher or lower DPI, respectively.</returns>
    internal static double GetDPIScaleFactor(MessageForm messageForm)
    {
        int dpi = 96; // Default DPI
        using (Graphics g = messageForm.CreateGraphics())
        {
            dpi = (int)g.DpiX;
        }
        return dpi / 96.0;
    }
    public static class ThemeManager
    {
        public static event EventHandler? ThemeChanged;

        private static bool initialized = false;
        private static IMessageFilter? filter;

        // Windows messages to listen for theme changes
        private const int WM_SETTINGCHANGE = 0x001A;
        private const int WM_THEMECHANGED = 0x031A;
        private const int WM_SYSCOLORCHANGE = 0x0015;

        /// <summary>
        /// Initializes application-wide theme and input language management services. This method sets up message
        /// filters and event handlers required for responding to system theme and user preference changes.
        /// </summary>
        /// <remarks>Call this method once at application startup to enable automatic handling of theme
        /// changes and user preference updates. Subsequent calls have no effect. This method also initializes the input
        /// language manager, so separate initialization is not required in typical scenarios.</remarks>
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            // Add application-wide message filter to catch theme change messages
            filter = new ThemeMessageFilter();
            Application.AddMessageFilter(filter);

            // Also catch user preference changes from SystemEvents
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            // Initialize input language manager together so consumers need only call one initializer in common cases
            InputLanguageManager.Initialize();
        }

        private static void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            NotifyThemeChanged();
        }

        private static void SystemEvents_UserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e)
        {
            NotifyThemeChanged();
        }

        /// <summary>
        /// Releases resources and detaches event handlers associated with input language management. Call this method
        /// to clean up static resources when input language functionality is no longer needed.
        /// </summary>
        /// <remarks>This method should be called before application shutdown or when input language
        /// management is no longer required to prevent resource leaks. After calling this method, input language
        /// features provided by this component will be disabled until re-initialized.</remarks>
        public static void Cleanup()
        {
            if (!initialized) return;
            initialized = false;
            if (filter != null)
            {
                try { Application.RemoveMessageFilter(filter); } catch { }
                filter = null;
            }
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            InputLanguageManager.Cleanup();
        }

        // Instead of invoking the event synchronously for all subscribers,
        // invoke each subscriber in a safe/asynchronous way:

        /// <summary>
        /// Notifies all subscribers that the application's theme has changed by raising the ThemeChanged event
        /// asynchronously on each handler.
        /// </summary>
        /// <remarks>Each event handler is invoked asynchronously to avoid blocking the main thread. If a
        /// handler targets a Windows Forms control, the notification is marshaled to the control's UI thread when
        /// possible. Exceptions thrown by individual handlers are caught and do not prevent other subscribers from
        /// being notified.</remarks>
        private static void NotifyThemeChanged()
        {
            var handlers = ThemeChanged;
            if (handlers == null) return;

            foreach (Delegate d in handlers.GetInvocationList())
            {
                if (d is EventHandler handler)
                {
                    try
                    {
                        // If the target is a WinForms Control, marshal to its UI thread using BeginInvoke
                        if (handler.Target is System.Windows.Forms.Control ctrl && ctrl.IsHandleCreated)
                        {
                            try
                            {
                                ctrl.BeginInvoke(new Action(() =>
                                {
                                    try { handler.Invoke(null, EventArgs.Empty); } catch { }
                                }));
                            }
                            catch
                            {
                                // ignore any BeginInvoke failures and fallback to ThreadPool
                                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                                {
                                    try { handler.Invoke(null, EventArgs.Empty); } catch { }
                                });
                            }
                        }
                        else
                        {
                            // Non-control targets: run on ThreadPool to avoid blocking the message loop
                            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                            {
                                try { handler.Invoke(null, EventArgs.Empty); } catch { }
                            });
                        }
                    }
                    catch
                    {
                        // Swallow per-handler exceptions to avoid breaking other subscribers
                    }
                }
            }
        }

        // IMessageFilter implementation to catch Windows messages

        /// <summary>
        /// Filters Windows messages to detect system theme or color changes and notifies the application when such
        /// changes occur.
        /// </summary>
        /// <remarks>This class implements the IMessageFilter interface to monitor specific Windows
        /// messages related to theme and system color changes. It is typically used to ensure that the application
        /// responds appropriately when the user changes system themes or color settings.</remarks>
        private class ThemeMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_SETTINGCHANGE || m.Msg == WM_THEMECHANGED || m.Msg == WM_SYSCOLORCHANGE)
                {
                    NotifyThemeChanged();
                }
                return false; // Continue processing other messages
            }
        }
    }
    public static class InputLanguageManager
    {
        public static event EventHandler? InputLanguageChanged;

        private static bool initialized = false;
        private static IMessageFilter? filter;

        // Windows message for input language change
        private const int WM_INPUTLANGCHANGE = 0x0051;

        /// <summary>
        /// Initializes global input language change monitoring for the application.
        /// </summary>
        /// <remarks>This method installs a message filter to detect input language changes at the
        /// application level. It has no effect if called more than once. Call this method before relying on global
        /// input language change notifications, as Windows Forms does not provide a static event for input language
        /// changes.</remarks>
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            // Add a message filter to catch WM_INPUTLANGCHANGE globally
            filter = new InputLangMessageFilter();
            Application.AddMessageFilter(filter);

            // NOTE:
            // There is no static member called 'InputLanguage.InputLanguageChanged' in WinForms.
            // Per-control events exist (Control.InputLanguageChanged) but not a static global event.
            // We therefore only rely on the WM_INPUTLANGCHANGE message filter for a global notification.
        }

        // Cleanup method to remove filters/subscriptions if needed

        /// <summary>
        /// Releases resources and removes any installed message filters or subscriptions associated with the current
        /// context.
        /// </summary>
        /// <remarks>Call this method to clean up message filters or subscriptions that were previously
        /// set up by the application. This method is safe to call multiple times; subsequent calls after the first have
        /// no effect.</remarks>
        public static void Cleanup()
        {
            if (!initialized) return;
            initialized = false;

            if (filter != null)
            {
                try { Application.RemoveMessageFilter(filter); } catch { }
                filter = null;
            }
        }

        private class InputLangMessageFilter : IMessageFilter
        {
            /// <summary>
            /// Filters out a message before it is dispatched, allowing for custom processing of input language change
            /// messages.
            /// </summary>
            /// <remarks>If the message corresponds to an input language change, the
            /// InputLanguageChanged event is raised. This method can be used to monitor and respond to input language
            /// changes in the application.</remarks>
            /// <param name="m">A reference to the message to be processed. The message may be modified by the filter.</param>
            /// <returns>Always returns false to indicate that the message should continue to be processed by the next filter and
            /// the message loop.</returns>
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_INPUTLANGCHANGE)
                {
                    InputLanguageChanged?.Invoke(null, EventArgs.Empty);
                }
                return false;
            }
        }
    }
}