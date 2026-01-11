// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NeoBleeper
{
    /// <summary>
    /// Provides custom theme management for WinForms controls with reliable dark theme support.
    /// </summary>
    public static class CustomThemeManager
    {
        private static readonly Dictionary<int, Color> originalColors = new Dictionary<int, Color>();
        private static bool isInitialized = false;

        // Win32 API declarations
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetSysColors(int cElements, int[] lpaElements, int[] lpaRgbValues);

        [DllImport("user32.dll")]
        private static extern int GetSysColor(int nIndex);

        // System color indices
        private const int COLOR_SCROLLBAR = 0;
        private const int COLOR_BACKGROUND = 1;
        private const int COLOR_ACTIVECAPTION = 2;
        private const int COLOR_INACTIVECAPTION = 3;
        private const int COLOR_MENU = 4;
        private const int COLOR_WINDOW = 5;
        private const int COLOR_WINDOWFRAME = 6;
        private const int COLOR_MENUTEXT = 7;
        private const int COLOR_WINDOWTEXT = 8;
        private const int COLOR_CAPTIONTEXT = 9;
        private const int COLOR_ACTIVEBORDER = 10;
        private const int COLOR_INACTIVEBORDER = 11;
        private const int COLOR_APPWORKSPACE = 12;
        private const int COLOR_HIGHLIGHT = 13;
        private const int COLOR_HIGHLIGHTTEXT = 14;
        private const int COLOR_BTNFACE = 15;
        private const int COLOR_BTNSHADOW = 16;
        private const int COLOR_GRAYTEXT = 17;
        private const int COLOR_BTNTEXT = 18;
        private const int COLOR_INACTIVECAPTIONTEXT = 19;
        private const int COLOR_BTNHIGHLIGHT = 20;

        /// <summary>
        /// Applies custom system colors to create a dark or light theme.
        /// </summary>
        /// <param name="isDarkTheme">true for dark theme, false for light theme</param>
        public static void SetCustomSystemColors(bool isDarkTheme)
        {
            if (!isInitialized)
            {
                SaveOriginalColors();
                isInitialized = true;
            }

            if (isDarkTheme)
            {
                ApplyDarkTheme();
            }
            else
            {
                RestoreOriginalColors();
            }
        }

        /// <summary>
        /// Saves the original system colors for later restoration.
        /// </summary>
        private static void SaveOriginalColors()
        {
            int[] colorIndices = new[]
            {
                COLOR_WINDOW, COLOR_WINDOWTEXT, COLOR_BTNFACE, COLOR_BTNTEXT,
                COLOR_GRAYTEXT, COLOR_HIGHLIGHT, COLOR_HIGHLIGHTTEXT, COLOR_MENU,
                COLOR_MENUTEXT, COLOR_SCROLLBAR, COLOR_BTNSHADOW, COLOR_BTNHIGHLIGHT
            };

            foreach (int index in colorIndices)
            {
                originalColors[index] = ColorTranslator.FromWin32(GetSysColor(index));
            }
        }

        /// <summary>
        /// Applies a dark theme by setting custom system colors.
        /// </summary>
        private static void ApplyDarkTheme()
        {
            // Define dark theme colors
            Color darkBackground = Color.FromArgb(32, 32, 32);
            Color darkControl = Color.FromArgb(45, 45, 48);
            Color darkText = Color.FromArgb(255, 255, 255);
            Color darkHighlight = Color.FromArgb(0, 122, 204);
            Color darkHighlightText = Color.FromArgb(255, 255, 255);
            Color darkGrayText = Color.FromArgb(153, 153, 153);
            Color darkBorder = Color.FromArgb(63, 63, 70);

            int[] elements = new[]
            {
                COLOR_WINDOW, COLOR_WINDOWTEXT, COLOR_BTNFACE, COLOR_BTNTEXT,
                COLOR_GRAYTEXT, COLOR_HIGHLIGHT, COLOR_HIGHLIGHTTEXT, COLOR_MENU,
                COLOR_MENUTEXT, COLOR_SCROLLBAR, COLOR_BTNSHADOW, COLOR_BTNHIGHLIGHT
            };

            int[] colors = new[]
            {
                ColorTranslator.ToWin32(Color.Black), // Window background
                ColorTranslator.ToWin32(darkText), // Window text
                ColorTranslator.ToWin32(darkControl), // Button face
                ColorTranslator.ToWin32(darkText), // Button text
                ColorTranslator.ToWin32(darkGrayText), // Gray text
                ColorTranslator.ToWin32(darkHighlight), // Highlight
                ColorTranslator.ToWin32(darkHighlightText), // Highlight text
                ColorTranslator.ToWin32(darkControl), // Menu background
                ColorTranslator.ToWin32(darkText), // Menu text
                ColorTranslator.ToWin32(darkControl), // Scrollbar
                ColorTranslator.ToWin32(darkBorder), // Button shadow
                ColorTranslator.ToWin32(darkBorder) // Button highlight
            };

            SetSysColors(elements.Length, elements, colors);
        }

        /// <summary>
        /// Restores the original system colors.
        /// </summary>
        public static void RestoreOriginalColors()
        {
            if (!isInitialized || originalColors.Count == 0)
                return;

            var elements = new List<int>();
            var colors = new List<int>();

            foreach (var kvp in originalColors)
            {
                elements.Add(kvp.Key);
                colors.Add(ColorTranslator.ToWin32(kvp.Value));
            }

            SetSysColors(elements.Count, elements.ToArray(), colors.ToArray());
        }

        /// <summary>
        /// Applies theme colors to a specific control and its children recursively.
        /// </summary>
        /// <param name="control">The control to theme</param>
        /// <param name="isDarkTheme">true for dark theme, false for light theme</param>
        public static void ApplyThemeToControl(Control control, bool isDarkTheme)
        {
            if (control == null)
                return;

            if (isDarkTheme)
            {
                // Dark theme colors
                Color darkBackground = Color.FromArgb(32, 32, 32);
                Color darkControl = Color.FromArgb(45, 45, 48);
                Color darkText = Color.FromArgb(255, 255, 255);

                if (control is Form form)
                {
                    form.BackColor = darkBackground;
                    form.ForeColor = darkText;
                }
                else if (control is Button button)
                {
                    button.BackColor = darkControl;
                    button.ForeColor = darkText;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = Color.FromArgb(63, 63, 70);
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = Color.Black;
                    textBox.ForeColor = darkText;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = Color.Black;
                    comboBox.ForeColor = darkText;
                    comboBox.FlatStyle = FlatStyle.Flat;
                }
                else
                {
                    control.BackColor = darkBackground;
                    control.ForeColor = darkText;
                }
            }
            else
            {
                // Light theme - restore to system defaults
                control.BackColor = SystemColors.Control;
                control.ForeColor = SystemColors.ControlText;

                if (control is TextBox textBox)
                {
                    textBox.BackColor = SystemColors.Window;
                    textBox.ForeColor = SystemColors.WindowText;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = SystemColors.Window;
                    comboBox.ForeColor = SystemColors.WindowText;
                }
            }

            // Recursively apply to child controls
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, isDarkTheme);
            }
        }
    }
}