using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        // Lock the window to prevent flickering during updates
        LockWindowUpdate(form.Handle);

        try
        {
            // Sadece ana formu invalidate etmek çoðu durumda yeterlidir
            form.Invalidate(true); // true: tüm alt kontrolleri de kapsar
            form.Update();
        }
        finally
        {
            LockWindowUpdate(IntPtr.Zero);
        }
    }
}