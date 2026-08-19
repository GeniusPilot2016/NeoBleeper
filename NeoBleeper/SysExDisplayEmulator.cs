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
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NeoBleeper
{
    public partial class SysExDisplayEmulator : Form
    {
        private const int DisplayWidth = 16;
        private const int DisplayHeight = 16;

        /*
         * Each TableLayoutPanel cell contains one Panel.
         * Changing the Panel.BackColor changes the visible cell color.
         */

        MIDIFilePlayer midiFilePlayer;
        private readonly Panel[,] _pixelCells =
            new Panel[DisplayWidth, DisplayHeight];
        bool darkTheme = false;

        private const int SysExTextMarqueeIntervalMilliseconds = 180;
        private const string SysExTextMarqueeGap = "    ";

        private readonly System.Windows.Forms.Timer _sysExTextMarqueeTimer =
            new System.Windows.Forms.Timer();

        private string _sysExTextSource = string.Empty;
        private int _sysExTextMarqueeOffset;
        private bool _settingSysExMarqueeText;
        private SysExTextSelectionFilter _sysExTextSelectionFilter;

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method selects and applies a light or dark theme according to the user's theme
        /// preference or the system's theme setting. It also enables double buffering to improve rendering performance
        /// and ensures that all UI changes are applied immediately. This method should be called when the theme needs
        /// to be updated, such as after a settings change.</remarks>
        private void SetTheme()
        {
            this.SuspendLayout(); // Suspend layout to batch updates
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering

            try
            {
                switch (Settings1.Default.theme)
                {
                    case 0:
                        if (SystemThemeUtility.IsDarkTheme())
                        {
                            darkTheme = true;
                            this.BackColor = Color.FromArgb(32, 32, 32);
                        }
                        else
                        {
                            darkTheme = false;
                            this.BackColor = SystemColors.Control;
                        }
                        break;

                    case 1:
                        darkTheme = false;
                        this.BackColor = SystemColors.Control;
                        break;

                    case 2:
                        darkTheme = true;
                        this.BackColor = Color.FromArgb(32, 32, 32);
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
            UIHelper.SetFormBackgroundFluent(this, darkTheme);
        }

        private enum DisplayColors
        {
            Classic = 0,
            Industrial = 1,
            CoolWhite = 2,
            WarmAmber = 3
        }
        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        public SysExDisplayEmulator(MIDIFilePlayer owner)
        {
            InitializeComponent();
            midiFilePlayer = owner;
            Owner = owner;
            labelSysExText.Font = SysExEmulatorFonts.GetSysExEmulatorFont(labelSysExText.Font.Size);
            SetTheme();
            InitializeDisplayCells();
            ClearDisplayContent();

            _sysExTextMarqueeTimer.Interval = SysExTextMarqueeIntervalMilliseconds;
            _sysExTextMarqueeTimer.Tick += SysExTextMarqueeTimer_Tick;
            labelSysExText.SizeChanged += LabelSysExText_SizeChanged;

            // Force the label to actually go blank at startup, independent of
            // whatever placeholder text the designer left on the control. This
            // writes straight to labelSysExText.Text (bypassing the "already
            // matches _sysExTextSource" guard in SetDisplayedSysExText) so a
            // stale design-time string can never survive into a real session.
            ApplyLabelText(string.Empty);
        }

        private void SysExDisplayEmulator_Disposed(
            object sender,
            EventArgs e)
        {
            _sysExTextMarqueeTimer.Stop();
            _sysExTextMarqueeTimer.Tick -= SysExTextMarqueeTimer_Tick;
            _sysExTextMarqueeTimer.Dispose();

            labelSysExText.SizeChanged -= LabelSysExText_SizeChanged;

            if (_sysExTextSelectionFilter != null)
            {
                Application.RemoveMessageFilter(_sysExTextSelectionFilter);
                _sysExTextSelectionFilter = null;
            }
        }

        /// <summary>
        /// Public entry point used by MIDIFilePlayer to push decoded SysEx display
        /// text into this emulator. Routes through SetDisplayedSysExText so fit
        /// checking and the marquee are always applied, regardless of caller thread.
        /// </summary>
        public void SetSysExText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetSysExText), text);
                return;
            }

            SetDisplayedSysExText(text);
        }

        /// <summary>
        /// Sets the text shown in the SysEx display line. If it fits inside the
        /// display at the control's current width it's shown statically. If not,
        /// instead of letting the control clip it (or show an ellipsis) it scrolls
        /// continuously as a marquee until replaced or resized back to fitting.
        /// </summary>
        private void SetDisplayedSysExText(string text)
        {
            string safeText = text ?? string.Empty;

            // Skip work only if the logical source is unchanged AND the label is
            // already showing the right thing (marquee already running for it, or
            // its static text already matches). Comparing the cached source alone
            // is not enough: the label can start out of sync with _sysExTextSource
            // (e.g. leftover designer placeholder text before the first real
            // update), and a mismatch must self-heal instead of silently no-op'ing
            // — this is also what previously prevented "clear" commands (empty
            // string) from actually blanking the label.
            bool sourceUnchanged = _sysExTextSource == safeText;
            if (sourceUnchanged &&
                (_sysExTextMarqueeTimer.Enabled || labelSysExText.Text == safeText))
            {
                return;
            }

            _sysExTextSource = safeText;
            _sysExTextMarqueeOffset = 0;
            _sysExTextMarqueeTimer.Stop();

            if (string.IsNullOrEmpty(safeText) || TextFitsDisplay(safeText))
            {
                ApplyLabelText(safeText);
            }
            else
            {
                // Prime with the looped frame immediately so there's no blank/static
                // tick before the first scroll step.
                ApplyLabelText(BuildMarqueeFrame(safeText, 0));
                _sysExTextMarqueeTimer.Start();
            }
        }

        /// <summary>
        /// Advances the marquee by one character per tick.
        /// </summary>
        private void SysExTextMarqueeTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_sysExTextSource))
            {
                _sysExTextMarqueeTimer.Stop();
                return;
            }

            string loopUnit = _sysExTextSource + SysExTextMarqueeGap;
            _sysExTextMarqueeOffset = (_sysExTextMarqueeOffset + 1) % loopUnit.Length;

            ApplyLabelText(BuildMarqueeFrame(_sysExTextSource, _sysExTextMarqueeOffset));
        }

        /// <summary>
        /// Builds the visible marquee window: (source + gap) doubled, then a
        /// slice starting "offset" characters in, so it wraps seamlessly.
        /// </summary>
        private static string BuildMarqueeFrame(string source, int offset)
        {
            string loopUnit = source + SysExTextMarqueeGap;
            string doubled = loopUnit + loopUnit;
            return doubled.Substring(offset, loopUnit.Length);
        }

        /// <summary>
        /// Re-checks fit when the display is resized (theme/DPI changes, etc.),
        /// starting or stopping the marquee as needed.
        /// </summary>
        private void LabelSysExText_SizeChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_sysExTextSource))
            {
                return;
            }

            if (TextFitsDisplay(_sysExTextSource))
            {
                _sysExTextMarqueeTimer.Stop();
                _sysExTextMarqueeOffset = 0;
                ApplyLabelText(_sysExTextSource);
            }
            else if (!_sysExTextMarqueeTimer.Enabled)
            {
                ApplyLabelText(BuildMarqueeFrame(_sysExTextSource, 0));
                _sysExTextMarqueeTimer.Start();
            }
        }

        /// <summary>
        /// True when "text" fits on one line at the display's current width —
        /// i.e. when a control with AutoEllipsis would NOT need to show "...".
        /// </summary>
        private bool TextFitsDisplay(string text)
        {
            int availableWidth = labelSysExText.ClientSize.Width;
            if (availableWidth <= 0)
            {
                return true;
            }

            Size measured = TextRenderer.MeasureText(
                text,
                labelSysExText.Font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding |
                TextFormatFlags.NoPrefix);

            return measured.Width <= availableWidth;
        }

        /// <summary>
        /// Writes labelSysExText.Text while keeping the existing
        /// _settingSysExMarqueeText guard around it. This checks the live
        /// control value (not any cached field), so it always reflects
        /// what is actually painted on screen.
        /// </summary>
        private void ApplyLabelText(string text)
        {
            if (labelSysExText.Text == text)
            {
                return;
            }

            _settingSysExMarqueeText = true;

            try
            {
                labelSysExText.Text = text;
            }
            finally
            {
                _settingSysExMarqueeText = false;
            }
        }

        /// <summary>
        /// Blocks mouse and keyboard messages that can select TextBox content.
        /// The control remains enabled, so its appearance does not change.
        /// </summary>
        private sealed class SysExTextSelectionFilter :
            IMessageFilter
        {
            private const int WmKeyDown = 0x0100;
            private const int WmKeyUp = 0x0101;
            private const int WmChar = 0x0102;
            private const int WmContextMenu = 0x007B;
            private const int WmMouseMove = 0x0200;
            private const int WmLButtonDown = 0x0201;
            private const int WmLButtonUp = 0x0202;
            private const int WmLButtonDoubleClick = 0x0203;
            private const int WmRButtonDown = 0x0204;
            private const int WmRButtonUp = 0x0205;
            private const int MouseKeyLeftButton = 0x0001;

            private readonly System.Windows.Forms.TextBox _textBox;

            public SysExTextSelectionFilter(
                System.Windows.Forms.TextBox textBox)
            {
                _textBox = textBox ??
                    throw new ArgumentNullException(nameof(textBox));
            }

            public bool PreFilterMessage(ref Message message)
            {
                if (!_textBox.IsHandleCreated ||
                    message.HWnd != _textBox.Handle)
                {
                    return false;
                }

                switch (message.Msg)
                {
                    case WmKeyDown:
                    case WmKeyUp:
                    case WmChar:
                    case WmContextMenu:
                    case WmLButtonDown:
                    case WmLButtonUp:
                    case WmLButtonDoubleClick:
                    case WmRButtonDown:
                    case WmRButtonUp:
                        return true;

                    case WmMouseMove:
                        return
                            (message.WParam.ToInt64() &
                             MouseKeyLeftButton) != 0;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns the selected emulator background and active-pixel colors.
        /// </summary>
        private (Color Background, Color Pixel) GetDisplayColors()
        {
            DisplayColors selectedColor =
                (DisplayColors)Settings1.Default.SysExEmulatorColor;

            switch (selectedColor)
            {
                case DisplayColors.Classic:
                    return
                    (
                        Color.FromArgb(173, 216, 23),
                        Color.FromArgb(35, 43, 18)
                    );

                case DisplayColors.Industrial:
                    return
                    (
                        Color.FromArgb(110, 185, 240),
                        Color.FromArgb(10, 25, 45)
                    );

                case DisplayColors.CoolWhite:
                    return
                    (
                        Color.FromArgb(220, 230, 242),
                        Color.FromArgb(28, 32, 38)
                    );

                case DisplayColors.WarmAmber:
                    return
                    (
                        Color.FromArgb(245, 150, 20),
                        Color.FromArgb(45, 25, 5)
                    );

                default:
                    return
                    (
                        Color.Black,
                        Color.Lime
                    );
            }
        }

        /// <summary>
        /// Configures sysexEmulatorBase as a fixed 16×16 display and places
        /// one Panel control inside every TableLayoutPanel cell.
        /// </summary>
        private void InitializeDisplayCells()
        {
            var displayColors = GetDisplayColors();

            sysexEmulatorBase.SuspendLayout();

            try
            {
                sysexEmulatorBase.Controls.Clear();
                sysexEmulatorBase.ColumnStyles.Clear();
                sysexEmulatorBase.RowStyles.Clear();

                sysexEmulatorBase.AutoSize = false;
                sysexEmulatorBase.AutoScroll = false;
                sysexEmulatorBase.ColumnCount = DisplayWidth;
                sysexEmulatorBase.RowCount = DisplayHeight;
                sysexEmulatorBase.GrowStyle =
                    TableLayoutPanelGrowStyle.FixedSize;

                /*
                 * Set this to None so the cells touch each other.
                 * Change it to Single if visible pixel borders are desired.
                 */
                sysexEmulatorBase.CellBorderStyle =
                    TableLayoutPanelCellBorderStyle.None;

                for (int x = 0; x < DisplayWidth; x++)
                {
                    sysexEmulatorBase.ColumnStyles.Add(
                        new ColumnStyle(
                            SizeType.Percent,
                            100f / DisplayWidth
                        )
                    );
                }

                for (int y = 0; y < DisplayHeight; y++)
                {
                    sysexEmulatorBase.RowStyles.Add(
                        new RowStyle(
                            SizeType.Percent,
                            100f / DisplayHeight
                        )
                    );
                }

                for (int y = 0; y < DisplayHeight; y++)
                {
                    for (int x = 0; x < DisplayWidth; x++)
                    {
                        Panel pixelCell = new Panel
                        {
                            Name = $"sysExPixel_{x}_{y}",
                            Dock = DockStyle.Fill,
                            Margin = Padding.Empty,
                            Padding = Padding.Empty,
                            BackColor = displayColors.Background,
                            TabStop = false
                        };

                        _pixelCells[x, y] = pixelCell;

                        /*
                         * The Panel is placed in column x and row y.
                         * Its BackColor is the visible color of that cell.
                         */
                        sysexEmulatorBase.Controls.Add(pixelCell, x, y);
                    }
                }

                sysexEmulatorBase.BackColor =
                    displayColors.Background;
            }
            finally
            {
                sysexEmulatorBase.ResumeLayout(true);
            }
        }

        /// <summary>
        /// Displays a complete pixel frame.
        /// ColorCode 0 means the pixel is off.
        /// Any value greater than 0 means the pixel is on.
        /// </summary>
        public void SetDisplayContent(
            IEnumerable<(int X, int Y, int ColorCode)> content)
        {
            if (InvokeRequired)
            {
                List<(int X, int Y, int ColorCode)> copiedContent =
                    content?.ToList()
                    ?? new List<(int X, int Y, int ColorCode)>();

                BeginInvoke(new Action(
                    () => SetDisplayContent(copiedContent)
                ));

                return;
            }

            // If the caller has nothing to show, clear the display instead of
            // rendering an empty frame. This keeps DisplayContent.Content and
            // the visible cells consistent with "nothing here" rather than
            // leaving stale state around.
            List<(int X, int Y, int ColorCode)> materializedContent =
                content?.ToList() ?? new List<(int X, int Y, int ColorCode)>();

            bool hasAnyLitPixel =
                materializedContent.Any(pixel => pixel.ColorCode > 0);

            if (!hasAnyLitPixel)
            {
                ClearDisplayContent();
                return;
            }

            DisplayContent.Content.Clear();
            DisplayContent.Content.AddRange(materializedContent);

            SetDisplayContent();
        }

        /// <summary>
        /// Displays a complete 16×16 Boolean pixel frame.
        /// The first array dimension is X and the second is Y.
        /// </summary>
        public void SetDisplayContent(bool[,] pixels)
        {
            if (pixels == null)
            {
                ClearDisplayContent();
                return;
            }

            if (pixels.GetLength(0) != DisplayWidth ||
                pixels.GetLength(1) != DisplayHeight)
            {
                throw new ArgumentException(
                    $"The pixel array must be " +
                    $"{DisplayWidth}×{DisplayHeight}.",
                    nameof(pixels)
                );
            }

            List<(int X, int Y, int ColorCode)> content =
                new List<(int X, int Y, int ColorCode)>();

            bool hasAnyLitPixel = false;

            for (int y = 0; y < DisplayHeight; y++)
            {
                for (int x = 0; x < DisplayWidth; x++)
                {
                    bool isOn = pixels[x, y];
                    hasAnyLitPixel |= isOn;

                    content.Add(
                        (
                            x,
                            y,
                            isOn ? 1 : 0
                        )
                    );
                }
            }

            // An all-off frame is functionally "no graphics" — clear instead
            // of storing 256 zero-value entries and repainting every cell.
            if (!hasAnyLitPixel)
            {
                ClearDisplayContent();
                return;
            }

            SetDisplayContent(content);
        }

        /// <summary>
        /// Updates one pixel without replacing the entire frame.
        /// </summary>
        public void SetPixel(int x, int y, bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(
                    () => SetPixel(x, y, enabled)
                ));

                return;
            }

            if (!IsValidCoordinate(x, y))
            {
                return;
            }

            var displayColors = GetDisplayColors();

            _pixelCells[x, y].BackColor =
                enabled
                    ? displayColors.Pixel
                    : displayColors.Background;

            int existingIndex =
                DisplayContent.Content.FindIndex(
                    pixel => pixel.X == x && pixel.Y == y
                );

            var newPixel =
                (X: x, Y: y, ColorCode: enabled ? 1 : 0);

            if (existingIndex >= 0)
            {
                DisplayContent.Content[existingIndex] = newPixel;
            }
            else
            {
                DisplayContent.Content.Add(newPixel);
            }

            // If this was the very last lit pixel being turned off, treat the
            // display as empty and fully clear it instead of leaving a frame
            // of all-zero entries behind.
            bool anyLitPixelRemains =
                DisplayContent.Content.Any(pixel => pixel.ColorCode > 0);

            if (!anyLitPixelRemains)
            {
                ClearDisplayContent();
            }
        }

        /// <summary>
        /// Applies DisplayContent.Content to the cell controls.
        /// All cells are reset first so this represents a complete frame.
        /// </summary>
        private void SetDisplayContent()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(SetDisplayContent));
                return;
            }

            // Nothing stored means there is nothing to draw — clear rather
            // than repaint an already-blank frame.
            if (DisplayContent.Content.Count == 0 ||
                !DisplayContent.Content.Any(pixel => pixel.ColorCode > 0))
            {
                ClearDisplayContent();
                return;
            }

            var displayColors = GetDisplayColors();

            sysexEmulatorBase.SuspendLayout();

            try
            {
                /*
                 * Reset all cell colors before drawing the new frame.
                 */
                SetAllCellColors(displayColors.Background);

                foreach (var pixel in DisplayContent.Content)
                {
                    if (!IsValidCoordinate(pixel.X, pixel.Y))
                    {
                        continue;
                    }

                    Panel cell = _pixelCells[pixel.X, pixel.Y];

                    cell.BackColor =
                        pixel.ColorCode > 0
                            ? displayColors.Pixel
                            : displayColors.Background;
                }

                sysexEmulatorBase.BackColor =
                    displayColors.Background;
            }
            finally
            {
                sysexEmulatorBase.ResumeLayout(false);
            }
        }

        /// <summary>
        /// Turns every display pixel off and clears the stored content.
        /// This is the single method responsible for representing "no
        /// graphics" — every code path that has nothing to show (an empty
        /// frame, an all-off frame, a GS Reset, or the last lit pixel being
        /// turned off) routes here instead of drawing a blank frame by hand.
        /// </summary>
        public void ClearDisplayContent()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(ClearDisplayContent));
                return;
            }

            DisplayContent.Content.Clear();

            Color backgroundColor =
                GetDisplayColors().Background;

            sysexEmulatorBase.SuspendLayout();

            try
            {
                SetAllCellColors(backgroundColor);
                sysexEmulatorBase.BackColor = backgroundColor;
            }
            finally
            {
                sysexEmulatorBase.ResumeLayout(false);
            }
        }

        /// <summary>
        /// Refreshes the display after the user changes the emulator color
        /// setting.
        /// </summary>
        public void RefreshDisplayColors()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(RefreshDisplayColors));
                return;
            }

            // Route through the same "is there anything lit" check so that a
            // color-scheme change on an empty display keeps it empty (fully
            // cleared) rather than materializing a blank frame.
            if (DisplayContent.Content.Count == 0 ||
                !DisplayContent.Content.Any(pixel => pixel.ColorCode > 0))
            {
                ClearDisplayContent();
                return;
            }

            SetDisplayContent();
        }

        /// <summary>
        /// Changes every TableLayoutPanel cell control to one color.
        /// </summary>
        private void SetAllCellColors(Color color)
        {
            for (int y = 0; y < DisplayHeight; y++)
            {
                for (int x = 0; x < DisplayWidth; x++)
                {
                    Panel pixelCell = _pixelCells[x, y];

                    if (pixelCell != null &&
                        pixelCell.BackColor != color)
                    {
                        pixelCell.BackColor = color;
                    }
                }
            }
        }

        private static bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 &&
                   x < DisplayWidth &&
                   y >= 0 &&
                   y < DisplayHeight;
        }

        /// <summary>
        /// Hides the emulator when the user closes it so the MIDI player
        /// can show the same instance again.
        /// </summary>
        private void SysExDisplayEmulator_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                midiFilePlayer.checkBoxShowSysExDisplayEmulator.Checked = false;
            }
        }

        /// <summary>
        /// Stores the current emulator frame.
        /// </summary>
        private static class DisplayContent
        {
            public static List<(int X, int Y, int ColorCode)> Content
            {
                get;
            }

            static DisplayContent()
            {
                Content =
                    new List<(int X, int Y, int ColorCode)>();
            }
        }
    }


    /// <summary>
    /// Decodes Roland SC-88/SC-88Pro display SysEx (model ID 45H).
    /// It supports ten 16×16 dot pages, Display Page, Display Time,
    /// the 32-character display buffer and GS Reset.
    /// </summary>
    internal sealed class RolandGSStyleDisplayDecoder
    {
        public const int DisplayWidth = 16;
        public const int DisplayHeight = 16;

        private const int PageCount = 10;
        private const int BytesPerPage = 64;
        private const int DotAddressMidStart = 0x01;
        private const int DotAddressMidEnd = 0x05;
        private const int DotAddressBlockSize = 0x80;
        private const int TotalDotBytes = PageCount * BytesPerPage;
        private const int DefaultDisplayTimeValue = 0x06;
        private const int FullFrameWithoutLastColumnBytes = 48;

        private readonly byte[][] _pages = new byte[PageCount][];
        private readonly char[] _displayedText = new char[32];
        private int _displayTimeValue = DefaultDisplayTimeValue;

        /// <summary>
        /// Page zero is the normal SC-88Pro bar display. Pages 1-10 are
        /// the stored Frame Draw dot pages.
        /// </summary>
        public int CurrentPage { get; private set; }

        public string DisplayedText => new string(_displayedText).TrimEnd();

        /// <summary>
        /// Clears the complete 32-character display-letter buffer.
        /// Returns true when at least one visible character was removed.
        /// </summary>
        private bool ClearDisplayedTextBuffer()
        {
            bool changed = false;

            for (int i = 0; i < _displayedText.Length; i++)
            {
                if (_displayedText[i] != ' ')
                {
                    _displayedText[i] = ' ';
                    changed = true;
                }
            }

            return changed;
        }

        /// <summary>
        /// Roland defines values 0-15 as 0-7.2 seconds, in 0.48-second steps.
        /// The factory/default value is 06 (2.88 seconds).
        /// </summary>
        public double DisplayTimeoutMilliseconds =>
            _displayTimeValue * 480.0;

        public RolandGSStyleDisplayDecoder()
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                _pages[i] = new byte[BytesPerPage];
            }

            Reset();
        }

        public void Reset()
        {
            foreach (byte[] page in _pages)
            {
                Array.Clear(page, 0, page.Length);
            }

            ClearDisplayedTextBuffer();

            CurrentPage = 0;
            _displayTimeValue = DefaultDisplayTimeValue;
        }

        public static bool AffectsDisplayState(byte[] message)
        {
            if (IsEmptySysExPacket(message))
            {
                return true;
            }

            if (!TryParseRolandDt1(
                    message,
                    out byte modelId,
                    out int address,
                    out _,
                    out _))
            {
                return false;
            }

            if (modelId == 0x42 && address == 0x40007F)
            {
                return true;
            }

            if (modelId != 0x45)
            {
                return false;
            }

            int addressMsb = (address >> 16) & 0x7F;
            int addressMid = (address >> 8) & 0x7F;
            int addressLsb = address & 0x7F;

            if (addressMsb != 0x10)
            {
                return false;
            }

            if (addressMid == 0x00)
            {
                return addressLsb <= 0x1F;
            }

            if (addressMid >= 0x01 && addressMid <= 0x05)
            {
                return addressLsb <= 0x7F;
            }

            return addressMid == 0x20 && addressLsb <= 0x01;
        }

        public static bool ContainsDotGraphics(byte[] message)
        {
            if (!TryParseRolandDt1(
                    message,
                    out byte modelId,
                    out int address,
                    out byte[] data,
                    out _))
            {
                return false;
            }

            int addressMsb = (address >> 16) & 0x7F;
            int addressMid = (address >> 8) & 0x7F;
            int addressLsb = address & 0x7F;
            int dotStartOffset = GetDotLinearAddress(
                addressMid,
                addressLsb);

            return modelId == 0x45 &&
                   addressMsb == 0x10 &&
                   dotStartOffset >= 0 &&
                   dotStartOffset < TotalDotBytes &&
                   data.Length > 0;
        }

        /// <summary>
        /// Returns every Frame Draw page touched by a dot-data DT1 packet.
        /// A single packet may cross 7-bit address boundaries and populate
        /// several or all of the ten pages.
        /// </summary>
        public static IReadOnlyList<int> GetDotGraphicsPagesTouched(
            byte[] message)
        {
            if (!TryParseRolandDt1(
                    message,
                    out byte modelId,
                    out int address,
                    out byte[] data,
                    out _) ||
                modelId != 0x45)
            {
                return Array.Empty<int>();
            }

            int addressMsb = (address >> 16) & 0x7F;
            int addressMid = (address >> 8) & 0x7F;
            int addressLsb = address & 0x7F;
            int dotStartOffset = GetDotLinearAddress(
                addressMid,
                addressLsb);

            if (addressMsb != 0x10 ||
                dotStartOffset < 0 ||
                dotStartOffset >= TotalDotBytes ||
                data.Length == 0)
            {
                return Array.Empty<int>();
            }

            int dotEndExclusive = Math.Min(
                TotalDotBytes,
                dotStartOffset + data.Length);
            int firstPage = (dotStartOffset / BytesPerPage) + 1;
            int lastPage =
                ((dotEndExclusive - 1) / BytesPerPage) + 1;

            int[] pages = new int[lastPage - firstPage + 1];
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i] = firstPage + i;
            }

            return pages;
        }

        /// <summary>
        /// Extracts only Roland model-45 display-letter data. Dot graphics,
        /// page commands, resets and unrelated SysEx messages return false.
        /// This is used to keep duplicated display text out of the lyric path.
        /// </summary>
        public static bool TryGetDisplayTextWrite(
            byte[] message,
            out int startIndex,
            out string text)
        {
            startIndex = 0;
            text = string.Empty;

            if (!TryParseRolandDt1(
                    message,
                    out byte modelId,
                    out int address,
                    out byte[] data,
                    out _))
            {
                return false;
            }

            int addressMsb = (address >> 16) & 0x7F;
            int addressMid = (address >> 8) & 0x7F;
            int addressLsb = address & 0x7F;

            if (modelId != 0x45 ||
                addressMsb != 0x10 ||
                addressMid != 0x00 ||
                addressLsb > 0x1F)
            {
                return false;
            }

            startIndex = addressLsb;
            int characterCount = Math.Min(
                data.Length,
                32 - startIndex);

            if (characterCount <= 0)
            {
                return true;
            }

            char[] characters = new char[characterCount];
            for (int i = 0; i < characterCount; i++)
            {
                characters[i] = DecodeDisplayCharacter(data[i]);
            }

            text = new string(characters);
            return true;
        }

        /// <summary>
        /// Used while reassembling split SMF F0/F7 events. A valid Roland
        /// checksum proves that the current fragment contains a complete DT1.
        /// A terminating F7 also proves completion, including framing-only clears.
        /// </summary>
        /// <summary>
        /// Detects the beginning of a Roland DT1 packet without requiring the
        /// checksum or terminator to be present. NAudio exposes SysEx payloads
        /// without F0/F7, so this is used only to distinguish a new packet from
        /// an SMF F7 continuation fragment.
        /// </summary>
        public static bool LooksLikeRolandDt1PacketStart(
            byte[] message)
        {
            if (message == null || message.Length == 0)
            {
                return false;
            }

            int header =
                message[0] == 0xF0 || message[0] == 0xF7
                    ? 1
                    : 0;

            return header + 6 < message.Length &&
                   message[header] == 0x41 &&
                   message[header + 3] == 0x12;
        }

        public static bool IsCompleteDisplayPacket(byte[] message)
        {
            if (message == null || message.Length == 0)
            {
                return false;
            }

            int lastIndex = message.Length - 1;
            if (message[lastIndex] == 0xF7 ||
                message[lastIndex] == 0x00 &&
                lastIndex > 0 &&
                message[lastIndex - 1] == 0xF7)
            {
                return true;
            }

            return TryParseRolandDt1(
                       message,
                       out _,
                       out _,
                       out _,
                       out bool checksumValid) &&
                   checksumValid;
        }

        public bool Apply(
            byte[] message,
            out bool visibleChanged,
            out bool restartDisplayTimeout,
            out bool textChanged)
        {
            visibleChanged = false;
            restartDisplayTimeout = false;
            textChanged = false;

            if (IsEmptySysExPacket(message))
            {
                CurrentPage = 0;
                visibleChanged = true;

                // A framing-only SysEx (just F0/F7, no payload) is used by many
                // files as a generic "clear display" command. That must blank
                // the 32-char text buffer too, not just the dot-graphics page —
                // otherwise stale text is left on screen indefinitely after the
                // pixels are cleared.
                ClearDisplayedTextBuffer();

                // A hide command is authoritative even when the decoder already
                // believes the buffer is blank. Publishing the clear again also
                // repairs a UI label that may have become out of sync.
                textChanged = true;

                return true;
            }

            if (!TryParseRolandDt1(
                    message,
                    out byte modelId,
                    out int address,
                    out byte[] data,
                    out _))
            {
                return false;
            }

            if (modelId == 0x42 &&
                address == 0x40007F &&
                data.Length > 0 &&
                data[0] == 0x00)
            {
                Reset();
                visibleChanged = true;
                textChanged = true;
                return true;
            }

            if (modelId != 0x45)
            {
                return false;
            }

            int addressMsb = (address >> 16) & 0x7F;
            int addressMid = (address >> 8) & 0x7F;
            int addressLsb = address & 0x7F;

            if (addressMsb != 0x10)
            {
                return false;
            }

            if (addressMid == 0x00 && addressLsb <= 0x1F)
            {
                bool changed = false;

                // Address 10 00 00 is the start of one complete displayed-letter
                // string. A shorter replacement (including a one-byte blank/hide
                // write) must discard the previous suffix instead of leaving stale
                // characters visible. Non-zero starts are retained as a tolerant
                // partial-write path for non-standard files.
                if (addressLsb == 0)
                {
                    changed |= ClearDisplayedTextBuffer();
                }

                for (int i = 0; i < data.Length; i++)
                {
                    int textIndex = addressLsb + i;
                    if (textIndex >= _displayedText.Length)
                    {
                        break;
                    }

                    char character = DecodeDisplayCharacter(data[i]);
                    if (_displayedText[textIndex] != character)
                    {
                        _displayedText[textIndex] = character;
                        changed = true;
                    }
                }

                // Display-letter writes update the independent text buffer.
                // They must not switch the selected dot-graphics page to page 0
                // or request a pixel repaint; doing so blanks the currently
                // displayed pixel art whenever text arrives.
                visibleChanged = false;

                // Only model-45 display-letter DT1 packets set this flag.
                // Lyrics, MIDI meta text and dot-picture data never reach it.
                textChanged = changed || data.Length > 0;
                return true;
            }

            int dotStartOffset = GetDotLinearAddress(
                addressMid,
                addressLsb);
            if (dotStartOffset >= 0)
            {
                PrepareAuthoritativeFrames(
                    dotStartOffset,
                    data);

                bool visiblePageWasAddressed = false;
                bool visiblePageChanged = false;
                bool pageOneWasAddressed = false;

                for (int i = 0; i < data.Length; i++)
                {
                    int dotOffset = dotStartOffset + i;
                    if (dotOffset >= TotalDotBytes)
                    {
                        break;
                    }

                    int pageNumber =
                        (dotOffset / BytesPerPage) + 1;
                    int byteIndex = dotOffset % BytesPerPage;

                    bool isVisiblePage = pageNumber == CurrentPage;
                    visiblePageWasAddressed |= isVisiblePage;
                    pageOneWasAddressed |= pageNumber == 1;

                    byte newValue = NormalizeDotDataByte(
                        byteIndex,
                        data[i]);
                    if (_pages[pageNumber - 1][byteIndex] != newValue)
                    {
                        _pages[pageNumber - 1][byteIndex] = newValue;
                        visiblePageChanged |= isVisiblePage;
                    }
                }

                // The SC-88/Pro temporarily displays Page 1 immediately when
                // Page 1 dot data is received, even without Display Page.
                if (pageOneWasAddressed)
                {
                    CurrentPage = 1;
                    visibleChanged = true;
                    restartDisplayTimeout = true;
                    return true;
                }

                visibleChanged =
                    visiblePageWasAddressed || visiblePageChanged;
                restartDisplayTimeout =
                    visibleChanged && CurrentPage > 0;
                return true;
            }

            if (addressMid == 0x20 &&
                addressLsb == 0x00 &&
                data.Length > 0)
            {
                int requestedPage = data[0] & 0x7F;
                if (requestedPage <= PageCount)
                {
                    CurrentPage = requestedPage;
                    visibleChanged = true;
                    restartDisplayTimeout = requestedPage > 0;

                    // Page 00 is Roland's Bar Display / hide command. The text
                    // occupies a separate UI control in the emulator, so it must
                    // be explicitly cleared and published as well as the pixels.
                    if (requestedPage == 0)
                    {
                        ClearDisplayedTextBuffer();
                        textChanged = true;
                    }
                }

                return true;
            }

            if (addressMid == 0x20 &&
                addressLsb == 0x01 &&
                data.Length > 0)
            {
                _displayTimeValue = Math.Min(15, data[0] & 0x7F);
                // Display Time sets the duration used by the next display action;
                // it is not itself a page-display trigger.
                return true;
            }

            return false;
        }

        /// <summary>
        /// A number of MIDI files contain SC-55-compatible 48-byte full frames
        /// and omit d48-d63 (the sixteenth column). Treat a boundary-aligned
        /// write of at least 48 bytes as a complete replacement and clear any
        /// omitted bytes first. Shorter writes retain Roland's partial-write behavior.
        /// </summary>
        private void PrepareAuthoritativeFrames(
            int dotStartOffset,
            byte[] data)
        {
            int dataOffset = 0;
            int dotOffset = dotStartOffset;

            while (dataOffset < data.Length &&
                   dotOffset < TotalDotBytes)
            {
                int pageNumber =
                    (dotOffset / BytesPerPage) + 1;
                int pageOffset = dotOffset % BytesPerPage;
                int bytesAvailableInPage = BytesPerPage - pageOffset;
                int bytesRemainingInDisplay =
                    TotalDotBytes - dotOffset;
                int segmentLength = Math.Min(
                    bytesAvailableInPage,
                    Math.Min(
                        data.Length - dataOffset,
                        bytesRemainingInDisplay));

                if (pageOffset == 0 &&
                    segmentLength >= FullFrameWithoutLastColumnBytes)
                {
                    Array.Clear(
                        _pages[pageNumber - 1],
                        0,
                        BytesPerPage);
                }

                dataOffset += segmentLength;
                dotOffset += segmentLength;
            }
        }

        /// <summary>
        /// Converts Roland's 7-bit dot-memory address into a linear offset.
        /// Address carry from 10 01 7F to 10 02 00 is significant because
        /// one DT1 packet may contain more than a single page pair.
        /// </summary>
        private static int GetDotLinearAddress(
            int addressMid,
            int addressLsb)
        {
            if (addressMid < DotAddressMidStart ||
                addressMid > DotAddressMidEnd ||
                addressLsb < 0 ||
                addressLsb >= DotAddressBlockSize)
            {
                return -1;
            }

            return
                ((addressMid - DotAddressMidStart) *
                 DotAddressBlockSize) +
                addressLsb;
        }

        public bool ExpireDisplay(out bool textChanged)
        {
            bool pageChanged = CurrentPage != 0;
            CurrentPage = 0;

            // Expiration is another hide transition. Clear the separate text
            // surface too, and tell the player to publish the empty value.
            textChanged = ClearDisplayedTextBuffer();
            return pageChanged;
        }

        public bool[,] GetPixels()
        {
            bool[,] pixels = new bool[DisplayWidth, DisplayHeight];
            if (CurrentPage < 1 || CurrentPage > PageCount)
            {
                return pixels;
            }

            byte[] page = _pages[CurrentPage - 1];
            for (int y = 0; y < DisplayHeight; y++)
            {
                for (int group = 0; group < 3; group++)
                {
                    byte value = page[(group * 16) + y];
                    for (int bitIndex = 0; bitIndex < 5; bitIndex++)
                    {
                        int x = (group * 5) + bitIndex;
                        int mask = 1 << (4 - bitIndex);
                        pixels[x, y] = (value & mask) != 0;
                    }
                }

                pixels[15, y] = (page[48 + y] & 0x10) != 0;
            }

            return pixels;
        }

        public bool HasVisibleContent()
        {
            if (CurrentPage < 1 || CurrentPage > PageCount)
            {
                return false;
            }

            byte[] page = _pages[CurrentPage - 1];
            for (int i = 0; i < 48; i++)
            {
                if ((page[i] & 0x1F) != 0)
                {
                    return true;
                }
            }

            for (int i = 48; i < 64; i++)
            {
                if ((page[i] & 0x10) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static byte NormalizeDotDataByte(int byteIndex, byte value)
        {
            return byteIndex < 48
                ? (byte)(value & 0x1F)
                : (byte)(value & 0x10);
        }

        private static char DecodeDisplayCharacter(byte value)
        {
            int character = value & 0x7F;
            return character >= 0x20 && character <= 0x7E
                ? (char)character
                : ' ';
        }

        private static bool IsEmptySysExPacket(byte[] message)
        {
            if (message == null)
            {
                return false;
            }

            if (message.Length == 0)
            {
                return true;
            }

            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] != 0xF0 && message[i] != 0xF7)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool TryParseRolandDt1(
            byte[] message,
            out byte modelId,
            out int address,
            out byte[] data,
            out bool checksumValid)
        {
            modelId = 0;
            address = 0;
            data = Array.Empty<byte>();
            checksumValid = false;

            if (message == null || message.Length < 9)
            {
                return false;
            }

            int endExclusive = message.Length;
            while (endExclusive > 0 &&
                   (message[endExclusive - 1] == 0xF7 ||
                    message[endExclusive - 1] == 0x00 &&
                    endExclusive > 1 &&
                    message[endExclusive - 2] == 0xF7))
            {
                endExclusive--;
            }

            int header = -1;
            for (int i = 0; i + 7 < endExclusive; i++)
            {
                if (message[i] == 0x41 &&
                    message[i + 3] == 0x12)
                {
                    header = i;
                    break;
                }
            }

            if (header < 0)
            {
                return false;
            }

            modelId = (byte)(message[header + 2] & 0x7F);
            int addressMsb = message[header + 4] & 0x7F;
            int addressMid = message[header + 5] & 0x7F;
            int addressLsb = message[header + 6] & 0x7F;
            address =
                (addressMsb << 16) |
                (addressMid << 8) |
                addressLsb;

            int dataStart = header + 7;
            int bytesAfterAddress = endExclusive - dataStart;
            if (bytesAfterAddress < 2)
            {
                return false;
            }

            int checksumIndex = endExclusive - 1;
            int dataLength = checksumIndex - dataStart;
            if (dataLength <= 0)
            {
                return false;
            }

            int sum = addressMsb + addressMid + addressLsb;
            for (int i = 0; i < dataLength; i++)
            {
                sum += message[dataStart + i] & 0x7F;
            }

            int expectedChecksum =
                (128 - (sum & 0x7F)) & 0x7F;
            int suppliedChecksum =
                message[checksumIndex] & 0x7F;
            checksumValid = expectedChecksum == suppliedChecksum;

            data = new byte[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = (byte)(message[dataStart + i] & 0x7F);
            }

            // Keep tolerant playback behavior for files edited by software that
            // recalculated the checksum incorrectly. The checksum is used only
            // to decide whether split SMF fragments are already complete.
            return true;
        }
    }
}