using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class TitleBarHelper
{
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_9586 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_FRAMECHANGED = 0x0020;

    public static void ApplyCustomTitleBar(Form form, Color color, bool darkMode = false)
    {
        FormBorderStyle originalBorderStyle = form.FormBorderStyle;
        if (!(form.IsDisposed || form.Disposing))
        {

            // Set title bar color
            var colorRef = ColorTranslator.ToWin32(color);
            DwmSetWindowAttribute(form.Handle, 35, ref colorRef, 4); // DWMWA_CAPTION_COLOR

            // Enable dark mode (if desired)
            if (darkMode)
            {
                if (Environment.OSVersion.Version < new Version(10, 0, 18362))
                {
                    var useImmersiveDarkMode = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_9586;
                    DwmSetWindowAttribute(form.Handle, useImmersiveDarkMode, ref colorRef, 4);
                }
                else
                {
                    var useImmersiveDarkMode = DWMWA_USE_IMMERSIVE_DARK_MODE;
                    int value = 1;
                    DwmSetWindowAttribute(form.Handle, useImmersiveDarkMode, ref value, 4);
                }
            }
            //Remove minimize/maximize buttons
            int style = GetWindowLong(form.Handle, GWL_STYLE);
            style &= ~(WS_MAXIMIZEBOX | WS_MINIMIZEBOX);
            SetWindowLong(form.Handle, GWL_STYLE, style);

            //Update the window to reflect changes
            SetWindowPos(form.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
            // Set the form's border style to None to remove the default title bar
            form.FormBorderStyle = originalBorderStyle;
        }
    }

    public static void ApplyToAllOpenForms(bool darkMode = false)
    {
        foreach (Form form in Application.OpenForms)
        {
            Color color = darkMode ? Color.Black : Color.White;
            ApplyCustomTitleBar(form, color, darkMode);
        }
    }
}