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
    public static void setLanguageByName(string languageName) // Set the application's culture based on a human-readable language name
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