using Microsoft.Win32;
using NeoBleeper;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
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

    [StructLayout(LayoutKind.Sequential)]
    private struct Margins
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

    [StructLayout(LayoutKind.Sequential)]
    private struct AccentPolicy
    {
        public int AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    private enum AccentState
    {
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4
    }

    private enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowCompositionAttributeData
    {
        public int Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(IntPtr hWnd, ref WindowCompositionAttributeData data);


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
                                    try { handler.Invoke(ctrl, EventArgs.Empty); } catch { }
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
    private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
    private const int DWMWA_IMMERSIVE_DARK_MODE = 20;
    public static void SetFormBackgroundFluent(Form form, bool darkMode)
    {
        if (!OperatingSystem.IsWindows() || Environment.OSVersion.Version.Major < 10 || Environment.OSVersion.Version.Build < 22523 || Settings1.Default.ClassicBleeperMode)
            return;
        if (form == null || form.IsDisposed || form.Disposing) return;
        if (!form.IsHandleCreated)
            form.CreateControl();

        // Set the backdrop type to mica 
        int dark = darkMode ? 1 : 0;
        int backdropType = 2;
        DwmSetWindowAttribute(form.Handle, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, 4);
        DwmSetWindowAttribute(form.Handle, DWMWA_IMMERSIVE_DARK_MODE, ref dark, 4);
        DwmSetWindowAttribute(form.Handle, 35, ref cachedColorRef, 4);

        // Set the form's BackColor to a darker shade to enhance the acrylic effect, especially in dark mode. The TransparencyKey is set to the same color to make it fully transparent.
        form.BackColor = darkMode ? Color.FromArgb(30, 30, 31) :
            Color.FromArgb(SystemColors.Control.R - 16, SystemColors.Control.G - 16, SystemColors.Control.B - 17);
        if (form is PortamentoWindow)
        {
            ChangeTrackbarColors(form);
        }
        form.TransparencyKey = form.BackColor;
        form.FormClosing += (s, e) =>
        {
            RemoveFluentBackground(form, darkMode);
        };
    }

    private static void ChangeTrackbarColors(Form form)
    {
        foreach (Control ctrl in form.Controls)
        {
            if (ctrl is TrackBar trackBar)
            {
                trackBar.BackColor = form.BackColor;
            }
            if (ctrl.HasChildren)
            {
                ChangeTrackbarColorsRecursive(ctrl, form.BackColor);
            }
        }
    }

    private static void ChangeTrackbarColorsRecursive(Control ctrl, Color backColor)
    {
        foreach (Control child in ctrl.Controls)
        {
            if (child is TrackBar trackBar)
            {
                trackBar.BackColor = backColor;
            }
            if (child.HasChildren)
            {
                ChangeTrackbarColorsRecursive(child, backColor);
            }
        }
    }
    public static void RemoveFluentBackground(Form form, bool darkMode)
    {
        if (!OperatingSystem.IsWindows() || Environment.OSVersion.Version.Major < 10 || Environment.OSVersion.Version.Build < 22523)
            return;
        if (form == null || form.IsDisposed || form.Disposing) return;
        if (!form.IsHandleCreated)
            form.CreateControl();
        int backdropType = 0; // Default
        DwmSetWindowAttribute(form.Handle, DWMWA_SYSTEMBACKDROP_TYPE, ref backdropType, 4);
        DwmSetWindowAttribute(form.Handle, DWMWA_IMMERSIVE_DARK_MODE, ref backdropType, 4);
        DwmSetWindowAttribute(form.Handle, 35, ref cachedColorRef, 4);
        form.BackColor = darkMode ? Color.FromArgb(32, 32, 32) : SystemColors.Control;
        form.TransparencyKey = Color.Empty;
    }
    public static void ApplyLanguageChangeToUI()
    {
        System.Windows.Forms.Form[] forms =
            System.Windows.Forms.Application.OpenForms
            .Cast<System.Windows.Forms.Form>()
            .ToArray();

        foreach (System.Windows.Forms.Form form in forms)
        {
            if (form == null || form.IsDisposed)
                continue;

            float dpiFactor = form.DeviceDpi / 96.0f;

            form.SuspendLayout();

            try
            {
                System.ComponentModel.ComponentResourceManager resources =
                    new System.ComponentModel.ComponentResourceManager(
                        form.GetType());

                // ====================================================
                // FORM
                // ====================================================

                LocalizeForm(
                    form,
                    resources,
                    dpiFactor);

                // ====================================================
                // CONTROLS
                // ====================================================

                LocalizeControlTree(
                    form,
                    resources,
                    dpiFactor);

                // ====================================================
                // TOOL TIPS
                // ====================================================

                LocalizeToolTips(
                    form,
                    resources);

                // ====================================================
                // OTHER COMPONENTS
                // ====================================================

                LocalizeComponents(
                    form,
                    resources);
            }
            finally
            {
                form.ResumeLayout(true);
                form.PerformLayout();
            }
        }
    }


    // =================================================================
    // FORM
    // =================================================================

    private static void LocalizeForm(
        System.Windows.Forms.Form form,
        System.ComponentModel.ComponentResourceManager resources,
        float dpiFactor)
    {
        string text =
            resources.GetString("$this.Text");

        if (text != null)
            form.Text = text;

        object sizeObject =
            resources.GetObject("$this.Size");

        if (sizeObject is System.Drawing.Size size)
        {
            form.Size =
                new System.Drawing.Size(
                    (int)System.Math.Round(
                        size.Width * dpiFactor),
                    (int)System.Math.Round(
                        size.Height * dpiFactor));
        }

        object locationObject =
            resources.GetObject("$this.Location");

        if (locationObject is System.Drawing.Point location)
        {
            form.Location =
                new System.Drawing.Point(
                    (int)System.Math.Round(
                        location.X * dpiFactor),
                    (int)System.Math.Round(
                        location.Y * dpiFactor));
        }

        object minimumSizeObject =
            resources.GetObject("$this.MinimumSize");

        if (minimumSizeObject is System.Drawing.Size minimumSize)
        {
            form.MinimumSize =
                new System.Drawing.Size(
                    (int)System.Math.Round(
                        minimumSize.Width * dpiFactor),
                    (int)System.Math.Round(
                        minimumSize.Height * dpiFactor));
        }

        object maximumSizeObject =
            resources.GetObject("$this.MaximumSize");

        if (maximumSizeObject is System.Drawing.Size maximumSize)
        {
            form.MaximumSize =
                new System.Drawing.Size(
                    (int)System.Math.Round(
                        maximumSize.Width * dpiFactor),
                    (int)System.Math.Round(
                        maximumSize.Height * dpiFactor));
        }

        string accessibleName =
            resources.GetString(
                "$this.AccessibleName");

        if (accessibleName != null)
            form.AccessibleName = accessibleName;

        string accessibleDescription =
            resources.GetString(
                "$this.AccessibleDescription");

        if (accessibleDescription != null)
            form.AccessibleDescription =
                accessibleDescription;
    }


    // =================================================================
    // CONTROL TREE
    // =================================================================

    private static void LocalizeControlTree(
        System.Windows.Forms.Control parent,
        System.ComponentModel.ComponentResourceManager resources,
        float dpiFactor)
    {
        if (parent == null)
            return;

        foreach (System.Windows.Forms.Control control
            in parent.Controls)
        {
            if (control == null)
                continue;

            string name = control.Name;

            if (!string.IsNullOrEmpty(name))
            {
                // ====================================================
                // COMBOBOX
                // ====================================================

                if (control is System.Windows.Forms.ComboBox comboBox)
                {
                    LocalizeComboBox(
                        comboBox,
                        name,
                        resources);
                }
                else
                {
                    // =================================================
                    // NORMAL CONTROL TEXT
                    // =================================================

                    LocalizeControlText(
                        control,
                        name,
                        resources);
                }

                // ====================================================
                // SIZE / LOCATION
                // ====================================================

                LocalizeGeometry(
                    control,
                    name,
                    resources,
                    dpiFactor);

                // ====================================================
                // TOOLTIP
                // ====================================================

                LocalizeControlToolTip(
                    control,
                    name,
                    resources);

                // ====================================================
                // LISTBOX
                // ====================================================

                if (control is System.Windows.Forms.CheckedListBox checkedList)
                {
                    LocalizeCheckedListBox(
                        checkedList,
                        name,
                        resources);
                }
                else if (control is System.Windows.Forms.ListBox listBox)
                {
                    LocalizeListBox(
                        listBox,
                        name,
                        resources);
                }

                // ====================================================
                // DATAGRIDVIEW
                // ====================================================

                if (control is System.Windows.Forms.DataGridView dataGridView)
                {
                    LocalizeDataGridView(
                        dataGridView,
                        resources);
                }

                // ====================================================
                // LISTVIEW
                // ====================================================

                if (control is System.Windows.Forms.ListView listView)
                {
                    LocalizeListView(
                        listView,
                        resources,
                        listView.FindForm());
                }

                // ====================================================
                // TREEVIEW
                // ====================================================

                if (control is System.Windows.Forms.TreeView treeView)
                {
                    LocalizeTreeView(
                        treeView,
                        resources);
                }

                // ====================================================
                // TOOLSTRIP
                // ====================================================

                if (control is System.Windows.Forms.ToolStrip toolStrip)
                {
                    LocalizeToolStrip(
                        toolStrip,
                        resources);
                }

                // ====================================================
                // CONTEXT MENU
                // ====================================================

                if (control.ContextMenuStrip != null)
                {
                    LocalizeContextMenu(
                        control.ContextMenuStrip,
                        resources);
                }
            }

            // ========================================================
            // TAB CONTROL
            // ========================================================

            if (control is System.Windows.Forms.TabControl tabControl)
            {
                foreach (System.Windows.Forms.TabPage page
                    in tabControl.TabPages)
                {
                    if (!string.IsNullOrEmpty(page.Name))
                    {
                        string pageText =
                            resources.GetString(
                                page.Name + ".Text");

                        if (pageText != null)
                            page.Text = pageText;

                        string accessibleName =
                            resources.GetString(
                                page.Name +
                                ".AccessibleName");

                        if (accessibleName != null)
                            page.AccessibleName =
                                accessibleName;

                        string accessibleDescription =
                            resources.GetString(
                                page.Name +
                                ".AccessibleDescription");

                        if (accessibleDescription != null)
                            page.AccessibleDescription =
                                accessibleDescription;
                    }

                    LocalizeControlTree(
                        page,
                        resources,
                        dpiFactor);
                }
            }
            else if (control.HasChildren)
            {
                LocalizeControlTree(
                    control,
                    resources,
                    dpiFactor);
            }
        }
    }


    // =================================================================
    // NORMAL CONTROL TEXT
    // =================================================================

    private static void LocalizeControlText(
        System.Windows.Forms.Control control,
        string name,
        System.ComponentModel.ComponentResourceManager resources)
    {
        if (control == null)
            return;

        string text =
            resources.GetString(
                name + ".Text");

        if (text != null)
            control.Text = text;

        string accessibleName =
            resources.GetString(
                name + ".AccessibleName");

        if (accessibleName != null)
            control.AccessibleName =
                accessibleName;

        string accessibleDescription =
            resources.GetString(
                name + ".AccessibleDescription");

        if (accessibleDescription != null)
            control.AccessibleDescription =
                accessibleDescription;
    }


    // =================================================================
    // COMBOBOX
    //
    // Supported resource formats:
    //
    //     comboBox1.Items
    //     comboBox1.Items0
    //     comboBox1.Items1
    //     comboBox1.Items2
    //
    //     comboBox1.Item0
    //     comboBox1.Item1
    //     comboBox1.Item2
    //
    //     comboBox1.Item.0
    //     comboBox1.Item.1
    //
    //     comboBox1.Items.0
    //     comboBox1.Items.1
    //
    // Most importantly:
    //
    //     comboBox1.Items
    //
    // can be the FIRST ITEM as a string.
    //
    // A string is NEVER enumerated.
    // =================================================================

    private static void LocalizeComboBox(
        System.Windows.Forms.ComboBox comboBox,
        string name,
        System.ComponentModel.ComponentResourceManager resources)
    {
        if (comboBox == null)
            return;

        if (string.IsNullOrWhiteSpace(name))
            return;

        int originalCount =
            comboBox.Items.Count;

        if (originalCount == 0)
            return;

        int selectedIndex =
            comboBox.SelectedIndex;

        System.Collections.Generic.List<string> localizedItems =
            new System.Collections.Generic.List<string>();

        // ============================================================
        // First check whether .Items is an actual collection.
        // ============================================================

        object itemsObject = null;

        try
        {
            itemsObject =
                resources.GetObject(
                    name + ".Items");
        }
        catch
        {
            itemsObject = null;
        }

        // ============================================================
        // CASE 1:
        //
        // .Items is STRING[]
        // ============================================================

        if (itemsObject is string[] stringArray)
        {
            foreach (string item in stringArray)
            {
                localizedItems.Add(
                    item ?? string.Empty);
            }
        }

        // ============================================================
        // CASE 2:
        //
        // .Items is OBJECT[]
        // ============================================================

        else if (itemsObject is object[] objectArray)
        {
            foreach (object item in objectArray)
            {
                localizedItems.Add(
                    item?.ToString()
                    ?? string.Empty);
            }
        }

        // ============================================================
        // CASE 3:
        //
        // .Items is another collection.
        //
        // IMPORTANT:
        // string is NOT processed here.
        // ============================================================

        else if (itemsObject != null &&
                 itemsObject is
                     System.Collections.IEnumerable enumerable &&
                 !(itemsObject is string))
        {
            foreach (object item in enumerable)
            {
                localizedItems.Add(
                    item?.ToString()
                    ?? string.Empty);
            }
        }

        // ============================================================
        // CASE 4:
        //
        // .Items itself is a STRING.
        //
        // THIS IS THE IMPORTANT FIX.
        //
        // Treat it as item ZERO / first item.
        //
        // DO NOT enumerate it.
        // ============================================================

        else if (itemsObject is string firstItem)
        {
            localizedItems.Add(firstItem);

            // --------------------------------------------------------
            // Then search for the remaining items.
            // --------------------------------------------------------

            for (int i = 1;
                 i < originalCount;
                 i++)
            {
                string value =
                    GetComboBoxResourceItem(
                        name,
                        i,
                        resources);

                if (value == null)
                {
                    // Keep original untranslated item.
                    value =
                        comboBox.Items[i]
                        ?.ToString()
                        ?? string.Empty;
                }

                localizedItems.Add(value);
            }
        }

        // ============================================================
        // CASE 5:
        //
        // No usable .Items resource.
        //
        // Search every item individually.
        // ============================================================

        if (localizedItems.Count == 0)
        {
            for (int i = 0;
                 i < originalCount;
                 i++)
            {
                string value =
                    GetComboBoxResourceItem(
                        name,
                        i,
                        resources);

                if (value == null)
                {
                    value =
                        comboBox.Items[i]
                        ?.ToString()
                        ?? string.Empty;
                }

                localizedItems.Add(value);
            }
        }

        // ============================================================
        // SAFETY
        // ============================================================

        if (localizedItems.Count == 0)
            return;

        // ============================================================
        // Do not destroy a multi-item ComboBox because only one
        // translation was found.
        // ============================================================

        if (originalCount > 1 &&
            localizedItems.Count == 1)
        {
            return;
        }

        // ============================================================
        // If fewer localized items were found, preserve the remaining
        // original items.
        // ============================================================

        while (localizedItems.Count < originalCount)
        {
            int index =
                localizedItems.Count;

            string original =
                comboBox.Items[index]
                ?.ToString()
                ?? string.Empty;

            localizedItems.Add(original);
        }

        // ============================================================
        // Replace items
        // ============================================================

        comboBox.BeginUpdate();

        try
        {
            comboBox.Items.Clear();

            foreach (string item
                in localizedItems)
            {
                comboBox.Items.Add(item);
            }

            // ========================================================
            // Restore selection
            // ========================================================

            if (selectedIndex >= 0 &&
                selectedIndex <
                comboBox.Items.Count)
            {
                comboBox.SelectedIndex =
                    selectedIndex;
            }
            else if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
        finally
        {
            comboBox.EndUpdate();
        }

        // ============================================================
        // Set displayed text from selected localized item.
        // ============================================================

        if (comboBox.SelectedIndex >= 0 &&
            comboBox.SelectedIndex <
            comboBox.Items.Count)
        {
            comboBox.Text =
                comboBox.Items[
                    comboBox.SelectedIndex]
                ?.ToString()
                ?? string.Empty;
        }
    }


    // =================================================================
    // GET COMBOBOX ITEM RESOURCE
    //
    // Handles all common resource naming variations.
    // =================================================================

    private static string GetComboBoxResourceItem(
        string name,
        int index,
        System.ComponentModel.ComponentResourceManager resources)
    {
        string value;

        // ============================================================
        // ZERO-BASED
        // ============================================================

        value =
            resources.GetString(
                name + ".Items" + index);

        if (value != null)
            return value;

        value =
            resources.GetString(
                name + ".Item" + index);

        if (value != null)
            return value;

        value =
            resources.GetString(
                name + ".Items." + index);

        if (value != null)
            return value;

        value =
            resources.GetString(
                name + ".Item." + index);

        if (value != null)
            return value;

        // ============================================================
        // ONE-BASED
        //
        // Only useful if zero-based did not exist.
        // ============================================================

        int oneBased =
            index + 1;

        value =
            resources.GetString(
                name + ".Items" + oneBased);

        if (value != null)
            return value;

        value =
            resources.GetString(
                name + ".Item" + oneBased);

        if (value != null)
            return value;

        value =
            resources.GetString(
                name + ".Items." + oneBased);

        if (value != null)
            return value;

        value =
            resources.GetString(
                name + ".Item." + oneBased);

        if (value != null)
            return value;

        return null;
    }


    // =================================================================
    // GEOMETRY - MULTI-MONITOR & FACTOR SAFE
    // =================================================================

    private static void LocalizeGeometry(
        System.Windows.Forms.Control control,
        string name,
        System.ComponentModel.ComponentResourceManager resources,
        float dpiFactor)
    {
        // Mevcut (çalışma zamanındaki) konum ve boyutu baz alıyoruz
        int currentX = control.Location.X;
        int currentY = control.Location.Y;
        int currentWidth = control.Size.Width;
        int currentHeight = control.Size.Height;

        object locationObject = resources.GetObject(name + ".Location");
        if (locationObject is System.Drawing.Point location)
        {
            // Kayan noktalı matematik kullanarak piksel kaymalarını engelliyoruz
            currentX = (int)System.Math.Round(location.X * dpiFactor);
            currentY = (int)System.Math.Round(location.Y * dpiFactor);
        }

        object sizeObject = resources.GetObject(name + ".Size");
        if (sizeObject is System.Drawing.Size size)
        {
            currentWidth = (int)System.Math.Round(size.Width * dpiFactor);
            currentHeight = (int)System.Math.Round(size.Height * dpiFactor);
        }

        // Minimum ve Maximum sınırları layout hesaplamasından önce korumaya alıyoruz
        object minimumSizeObject = resources.GetObject(name + ".MinimumSize");
        if (minimumSizeObject is System.Drawing.Size minimumSize)
        {
            control.MinimumSize = new System.Drawing.Size(
                (int)System.Math.Round(minimumSize.Width * dpiFactor),
                (int)System.Math.Round(minimumSize.Height * dpiFactor));
        }

        object maximumSizeObject = resources.GetObject(name + ".MaximumSize");
        if (maximumSizeObject is System.Drawing.Size maximumSize)
        {
            control.MaximumSize = new System.Drawing.Size(
                (int)System.Math.Round(maximumSize.Width * dpiFactor),
                (int)System.Math.Round(maximumSize.Height * dpiFactor));
        }

        // BUG FIX: .Location ve .Size özelliklerine ayrı ayrı ham piksel set etmek,
        // her atamada WinForms Layout Engine'i tetikler ve Anchor/Dock/Padding kuralları
        // yüzünden nesnelerin ekrandan dışarı taşmasına veya kaybolmasına (corruption) neden olur.
        // SetBounds metodu tüm yeni geometriyi tek bir atomik operasyonda uygulayarak bug'ı çözer.
        control.SetBounds(
            currentX,
            currentY,
            currentWidth,
            currentHeight,
            System.Windows.Forms.BoundsSpecified.All);
    }


    // =================================================================
    // CONTROL TOOLTIP
    // =================================================================

    private static void LocalizeControlToolTip(
        System.Windows.Forms.Control control,
        string name,
        System.ComponentModel.ComponentResourceManager resources)
    {
        System.Reflection.PropertyInfo property =
            control.GetType().GetProperty(
                "ToolTipText",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

        if (property == null)
            return;

        if (!property.CanWrite)
            return;

        if (property.PropertyType != typeof(string))
            return;

        string text =
            resources.GetString(
                name + ".ToolTipText");

        if (text == null)
        {
            text =
                resources.GetString(
                    name + ".ToolTip");
        }

        if (text != null)
        {
            try
            {
                property.SetValue(
                    control,
                    text,
                    null);
            }
            catch
            {
            }
        }
    }


    // =================================================================
    // TOOLTIP COMPONENT
    // =================================================================

    private static void LocalizeToolTips(
        System.Windows.Forms.Form form,
        System.ComponentModel.ComponentResourceManager resources)
    {
        System.Reflection.FieldInfo[] fields =
            form.GetType().GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);

        foreach (System.Reflection.FieldInfo field
            in fields)
        {
            object value;

            try
            {
                value =
                    field.GetValue(form);
            }
            catch
            {
                continue;
            }

            if (value is System.Windows.Forms.ToolTip toolTip)
            {
                LocalizeToolTipTree(
                    form,
                    toolTip,
                    resources);
            }
        }
    }


    // =================================================================
    // TOOLTIP TREE
    // =================================================================

    private static void LocalizeToolTipTree(
        System.Windows.Forms.Control parent,
        System.Windows.Forms.ToolTip toolTip,
        System.ComponentModel.ComponentResourceManager resources)
    {
        foreach (System.Windows.Forms.Control control
            in parent.Controls)
        {
            if (control == null)
                continue;

            if (!string.IsNullOrEmpty(control.Name))
            {
                string text =
                    resources.GetString(
                        control.Name +
                        ".ToolTipText");

                if (text == null)
                {
                    text =
                        resources.GetString(
                            control.Name +
                            ".ToolTip");
                }

                if (text != null)
                {
                    toolTip.SetToolTip(
                        control,
                        text);
                }
            }

            if (control.HasChildren)
            {
                LocalizeToolTipTree(
                    control,
                    toolTip,
                    resources);
            }
        }
    }


    // =================================================================
    // COMPONENTS
    // =================================================================

    private static void LocalizeComponents(
        System.Windows.Forms.Form form,
        System.ComponentModel.ComponentResourceManager resources)
    {
        System.Reflection.FieldInfo[] fields =
            form.GetType().GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);

        foreach (System.Reflection.FieldInfo field
            in fields)
        {
            object component;

            try
            {
                component =
                    field.GetValue(form);
            }
            catch
            {
                continue;
            }

            if (component is System.Windows.Forms.ToolStrip toolStrip)
            {
                LocalizeToolStrip(
                    toolStrip,
                    resources);
            }

            if (component is System.Windows.Forms.ContextMenuStrip contextMenu)
            {
                LocalizeContextMenu(
                    contextMenu,
                    resources);
            }
        }
    }


    // =================================================================
    // TOOLSTRIP
    // =================================================================

    private static void LocalizeToolStrip(
        System.Windows.Forms.ToolStrip toolStrip,
        System.ComponentModel.ComponentResourceManager resources)
    {
        if (toolStrip == null)
            return;

        foreach (System.Windows.Forms.ToolStripItem item
            in toolStrip.Items)
        {
            LocalizeToolStripItem(
                item,
                resources);
        }
    }


    // =================================================================
    // TOOLSTRIP ITEM
    // =================================================================

    private static void LocalizeToolStripItem(
        System.Windows.Forms.ToolStripItem item,
        System.ComponentModel.ComponentResourceManager resources)
    {
        if (item == null)
            return;

        if (!string.IsNullOrEmpty(item.Name))
        {
            string text =
                resources.GetString(
                    item.Name + ".Text");

            if (text != null)
                item.Text = text;

            string toolTip =
                resources.GetString(
                    item.Name + ".ToolTipText");

            if (toolTip != null)
                item.ToolTipText = toolTip;

            string accessibleName =
                resources.GetString(
                    item.Name +
                    ".AccessibleName");

            if (accessibleName != null)
                item.AccessibleName =
                    accessibleName;

            string accessibleDescription =
                resources.GetString(
                    item.Name +
                    ".AccessibleDescription");

            if (accessibleDescription != null)
                item.AccessibleDescription =
                    accessibleDescription;
        }

        if (item is System.Windows.Forms.ToolStripDropDownItem dropDownItem)
        {
            foreach (System.Windows.Forms.ToolStripItem child
                in dropDownItem.DropDownItems)
            {
                LocalizeToolStripItem(
                    child,
                    resources);
            }
        }
    }


    // =================================================================
    // CONTEXT MENU
    // =================================================================

    private static void LocalizeContextMenu(
        System.Windows.Forms.ContextMenuStrip contextMenu,
        System.ComponentModel.ComponentResourceManager resources)
    {
        if (contextMenu == null)
            return;

        foreach (System.Windows.Forms.ToolStripItem item
            in contextMenu.Items)
        {
            LocalizeToolStripItem(
                item,
                resources);
        }
    }


    // =================================================================
    // LISTBOX
    // =================================================================

    private static void LocalizeListBox(
        System.Windows.Forms.ListBox listBox,
        string name,
        System.ComponentModel.ComponentResourceManager resources)
    {
        int count =
            listBox.Items.Count;

        if (count == 0)
            return;

        int selectedIndex =
            listBox.SelectedIndex;

        System.Collections.Generic.List<string> localized =
            new System.Collections.Generic.List<string>();

        for (int i = 0;
             i < count;
             i++)
        {
            string value =
                resources.GetString(
                    name + ".Item" + i);

            if (value == null)
            {
                value =
                    resources.GetString(
                        name + ".Items" + i);
            }

            if (value == null)
            {
                value =
                    resources.GetString(
                        name + ".Item." + i);
            }

            if (value == null)
            {
                value =
                    resources.GetString(
                        name + ".Items." + i);
            }

            if (value == null)
            {
                value =
                    listBox.Items[i]
                    ?.ToString()
                    ?? string.Empty;
            }

            localized.Add(value);
        }

        listBox.BeginUpdate();

        try
        {
            listBox.Items.Clear();

            foreach (string item in localized)
                listBox.Items.Add(item);

            if (selectedIndex >= 0 &&
                selectedIndex <
                listBox.Items.Count)
            {
                listBox.SelectedIndex =
                    selectedIndex;
            }
        }
        finally
        {
            listBox.EndUpdate();
        }
    }


    // =================================================================
    // CHECKED LISTBOX
    // =================================================================

    private static void LocalizeCheckedListBox(
        System.Windows.Forms.CheckedListBox list,
        string name,
        System.ComponentModel.ComponentResourceManager resources)
    {
        int count =
            list.Items.Count;

        if (count == 0)
            return;

        bool[] checkedStates =
            new bool[count];

        for (int i = 0; i < count; i++)
        {
            checkedStates[i] =
                list.GetItemChecked(i);
        }

        int selectedIndex =
            list.SelectedIndex;

        System.Collections.Generic.List<string> localized =
            new System.Collections.Generic.List<string>();

        for (int i = 0;
             i < count;
             i++)
        {
            string value =
                resources.GetString(
                    name + ".Item" + i);

            if (value == null)
            {
                value =
                    resources.GetString(
                        name + ".Items" + i);
            }

            if (value == null)
            {
                value =
                    resources.GetString(
                        name + ".Item." + i);
            }

            if (value == null)
            {
                value =
                    resources.GetString(
                        name + ".Items." + i);
            }

            if (value == null)
            {
                value =
                    list.Items[i]
                    ?.ToString()
                    ?? string.Empty;
            }

            localized.Add(value);
        }

        list.BeginUpdate();

        try
        {
            list.Items.Clear();

            foreach (string item in localized)
                list.Items.Add(item);

            for (int i = 0;
                 i < checkedStates.Length &&
                 i < list.Items.Count;
                 i++)
            {
                list.SetItemChecked(
                    i,
                    checkedStates[i]);
            }

            if (selectedIndex >= 0 &&
                selectedIndex <
                list.Items.Count)
            {
                list.SelectedIndex =
                    selectedIndex;
            }
        }
        finally
        {
            list.EndUpdate();
        }
    }


    // =================================================================
    // DATAGRIDVIEW
    // =================================================================

    private static void LocalizeDataGridView(
        System.Windows.Forms.DataGridView dataGridView,
        System.ComponentModel.ComponentResourceManager resources)
    {
        foreach (System.Windows.Forms.DataGridViewColumn column
            in dataGridView.Columns)
        {
            if (string.IsNullOrEmpty(column.Name))
                continue;

            string header =
                resources.GetString(
                    column.Name +
                    ".HeaderText");

            if (header != null)
                column.HeaderText = header;

            string toolTip =
                resources.GetString(
                    column.Name +
                    ".ToolTipText");

            if (toolTip != null)
                column.ToolTipText =
                    toolTip;
        }
    }


    // =================================================================
    // LISTVIEW
    // =================================================================

    private static void LocalizeListView(
    System.Windows.Forms.ListView listView,
    System.ComponentModel.ComponentResourceManager resources,
    System.Windows.Forms.Form form)
    {
        if (listView == null || resources == null)
            return;

        // ================================================================
        // LISTVIEW COLUMNS
        // ================================================================

        foreach (System.Windows.Forms.ColumnHeader column
            in listView.Columns)
        {
            if (column == null)
                continue;

            bool localized = false;

            // ------------------------------------------------------------
            // FIRST:
            // Find the actual designer field corresponding to this
            // ColumnHeader.
            //
            // Example:
            //
            // private ColumnHeader columnHeader1;
            //
            // resources.ApplyResources(this.columnHeader1,
            //                           "columnHeader1");
            // ------------------------------------------------------------

            if (form != null)
            {
                System.Reflection.FieldInfo[] fields =
                    form.GetType().GetFields(
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Public);

                foreach (System.Reflection.FieldInfo field in fields)
                {
                    if (!typeof(System.Windows.Forms.ColumnHeader)
                        .IsAssignableFrom(field.FieldType))
                    {
                        continue;
                    }

                    System.Windows.Forms.ColumnHeader fieldColumn;

                    try
                    {
                        fieldColumn =
                            field.GetValue(form)
                            as System.Windows.Forms.ColumnHeader;
                    }
                    catch
                    {
                        continue;
                    }

                    if (!ReferenceEquals(fieldColumn, column))
                        continue;

                    // ----------------------------------------------------
                    // We have found the EXACT designer field.
                    // ----------------------------------------------------

                    try
                    {
                        resources.ApplyResources(
                            column,
                            field.Name);

                        localized = true;
                    }
                    catch
                    {
                    }

                    // ----------------------------------------------------
                    // Explicit Text lookup as a fallback.
                    // ----------------------------------------------------

                    string text =
                        resources.GetString(
                            field.Name + ".Text");

                    if (text != null)
                    {
                        column.Text = text;
                        localized = true;
                    }

                    break;
                }
            }

            // ============================================================
            // SECOND:
            // Fallback to ColumnHeader.Name
            // ============================================================

            if (!localized &&
                !string.IsNullOrEmpty(column.Name))
            {
                try
                {
                    resources.ApplyResources(
                        column,
                        column.Name);
                }
                catch
                {
                }

                string text =
                    resources.GetString(
                        column.Name + ".Text");

                if (text != null)
                {
                    column.Text = text;
                    localized = true;
                }
            }
        }

        // ================================================================
        // LISTVIEW ITEMS
        // ================================================================

        foreach (System.Windows.Forms.ListViewItem item
            in listView.Items)
        {
            if (item == null)
                continue;

            if (string.IsNullOrEmpty(item.Name))
                continue;

            try
            {
                resources.ApplyResources(
                    item,
                    item.Name);
            }
            catch
            {
            }

            string text =
                resources.GetString(
                    item.Name + ".Text");

            if (text != null)
                item.Text = text;
        }

        // ================================================================
        // FORCE REDRAW
        // ================================================================

        listView.BeginUpdate();

        try
        {
            listView.Invalidate(true);
            listView.Update();
            listView.Refresh();
        }
        finally
        {
            listView.EndUpdate();
        }
    }


    // =================================================================
    // TREEVIEW
    // =================================================================

    private static void LocalizeTreeView(
        System.Windows.Forms.TreeView treeView,
        System.ComponentModel.ComponentResourceManager resources)
    {
        foreach (System.Windows.Forms.TreeNode node
            in treeView.Nodes)
        {
            LocalizeTreeNode(
                node,
                resources);
        }
    }


    // =================================================================
    // TREE NODE
    // =================================================================

    private static void LocalizeTreeNode(
        System.Windows.Forms.TreeNode node,
        System.ComponentModel.ComponentResourceManager resources)
    {
        if (!string.IsNullOrEmpty(node.Name))
        {
            string text =
                resources.GetString(
                    node.Name + ".Text");

            if (text != null)
                node.Text = text;
        }

        foreach (System.Windows.Forms.TreeNode child
            in node.Nodes)
        {
            LocalizeTreeNode(
                child,
                resources);
        }
    }
}


