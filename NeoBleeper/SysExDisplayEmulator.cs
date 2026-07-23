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
            ConfigureSysExTextBox();
            SetTheme();
            InitializeDisplayCells();
            ClearDisplayContent();
        }

        private void ConfigureSysExTextBox()
        {
            // Keep the TextBox enabled so its existing appearance is preserved.
            textBoxSysExText.ReadOnly = true;
            textBoxSysExText.TabStop = false;
            textBoxSysExText.ShortcutsEnabled = false;
            textBoxSysExText.Cursor = Cursors.Default;

            _sysExTextSelectionFilter =
                new SysExTextSelectionFilter(textBoxSysExText);
            Application.AddMessageFilter(_sysExTextSelectionFilter);

            _sysExTextMarqueeTimer.Interval =
                SysExTextMarqueeIntervalMilliseconds;
            _sysExTextMarqueeTimer.Tick +=
                SysExTextMarqueeTimer_Tick;

            textBoxSysExText.TextChanged +=
                TextBoxSysExText_TextChanged;
            textBoxSysExText.SizeChanged +=
                TextBoxSysExText_LayoutChanged;
            textBoxSysExText.FontChanged +=
                TextBoxSysExText_LayoutChanged;
            textBoxSysExText.GotFocus +=
                TextBoxSysExText_GotFocus;

            VisibleChanged +=
                SysExDisplayEmulator_VisibleChanged;
            Disposed +=
                SysExDisplayEmulator_Disposed;

            _sysExTextSource =
                textBoxSysExText.Text ?? string.Empty;

            UpdateSysExTextMarquee();
        }

        private void TextBoxSysExText_TextChanged(
            object sender,
            EventArgs e)
        {
            if (_settingSysExMarqueeText)
            {
                return;
            }

            // Only externally supplied decoded SysEx text becomes the source.
            _sysExTextSource =
                textBoxSysExText.Text ?? string.Empty;
            _sysExTextMarqueeOffset = 0;

            UpdateSysExTextMarquee();
        }

        private void TextBoxSysExText_LayoutChanged(
            object sender,
            EventArgs e)
        {
            _sysExTextMarqueeOffset = 0;
            UpdateSysExTextMarquee();
        }

        private void TextBoxSysExText_GotFocus(
            object sender,
            EventArgs e)
        {
            ClearSysExTextSelection();

            BeginInvoke(new Action(() =>
            {
                ClearSysExTextSelection();

                if (ActiveControl == textBoxSysExText)
                {
                    ActiveControl = null;
                }
            }));
        }

        private void SysExDisplayEmulator_VisibleChanged(
            object sender,
            EventArgs e)
        {
            _sysExTextMarqueeOffset = 0;
            UpdateSysExTextMarquee();
        }

        private void SysExDisplayEmulator_Disposed(
            object sender,
            EventArgs e)
        {
            _sysExTextMarqueeTimer.Stop();
            _sysExTextMarqueeTimer.Dispose();

            if (_sysExTextSelectionFilter != null)
            {
                Application.RemoveMessageFilter(
                    _sysExTextSelectionFilter);
                _sysExTextSelectionFilter = null;
            }
        }

        private void SysExTextMarqueeTimer_Tick(
            object sender,
            EventArgs e)
        {
            if (!NeedsSysExTextMarquee())
            {
                UpdateSysExTextMarquee();
                return;
            }

            string loopText =
                _sysExTextSource + SysExTextMarqueeGap;

            _sysExTextMarqueeOffset =
                (_sysExTextMarqueeOffset + 1) %
                loopText.Length;

            SetDisplayedSysExText(
                loopText.Substring(_sysExTextMarqueeOffset) +
                loopText.Substring(
                    0,
                    _sysExTextMarqueeOffset));
        }

        private void UpdateSysExTextMarquee()
        {
            bool shouldRun =
                Visible &&
                NeedsSysExTextMarquee();

            _sysExTextMarqueeTimer.Enabled = shouldRun;

            if (!shouldRun)
            {
                _sysExTextMarqueeOffset = 0;
            }

            SetDisplayedSysExText(_sysExTextSource);
        }

        private bool NeedsSysExTextMarquee()
        {
            if (string.IsNullOrEmpty(_sysExTextSource) ||
                textBoxSysExText.ClientSize.Width <= 0)
            {
                return false;
            }

            Size measuredSize = TextRenderer.MeasureText(
                _sysExTextSource,
                textBoxSysExText.Font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding |
                TextFormatFlags.SingleLine);

            return measuredSize.Width >
                   Math.Max(
                       1,
                       textBoxSysExText.ClientSize.Width - 4);
        }

        private void SetDisplayedSysExText(string text)
        {
            string safeText = text ?? string.Empty;

            if (textBoxSysExText.Text != safeText)
            {
                _settingSysExMarqueeText = true;

                try
                {
                    textBoxSysExText.Text = safeText;
                }
                finally
                {
                    _settingSysExMarqueeText = false;
                }
            }

            ClearSysExTextSelection();
        }

        private void ClearSysExTextSelection()
        {
            textBoxSysExText.SelectionStart = 0;
            textBoxSysExText.SelectionLength = 0;

            if (textBoxSysExText.IsHandleCreated)
            {
                HideCaret(textBoxSysExText.Handle);
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

            for (int i = 0; i < _displayedText.Length; i++)
            {
                _displayedText[i] = ' ';
            }

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
                    out _,
                    out _))
            {
                return false;
            }

            int addressMsb = (address >> 16) & 0x7F;
            int addressMid = (address >> 8) & 0x7F;

            return modelId == 0x45 &&
                   addressMsb == 0x10 &&
                   addressMid >= 0x01 &&
                   addressMid <= 0x05;
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

                CurrentPage = 0;
                visibleChanged = changed || data.Length > 0;
                // Only model-45 display-letter DT1 packets set this flag.
                // Lyrics, MIDI meta text and dot-picture data never reach it.
                textChanged = changed || data.Length > 0;
                return true;
            }

            if (addressMid >= 0x01 && addressMid <= 0x05)
            {
                int firstPageInPair = ((addressMid - 1) * 2) + 1;
                PrepareAuthoritativeFrames(
                    firstPageInPair,
                    addressLsb,
                    data);

                bool visiblePageWasAddressed = false;
                bool visiblePageChanged = false;
                bool pageOneWasAddressed = false;

                for (int i = 0; i < data.Length; i++)
                {
                    int pageRelativeAddress = addressLsb + i;
                    if (pageRelativeAddress > 0x7F)
                    {
                        break;
                    }

                    int pageNumber = pageRelativeAddress < 0x40
                        ? firstPageInPair
                        : firstPageInPair + 1;
                    int byteIndex = pageRelativeAddress & 0x3F;

                    if (pageNumber < 1 || pageNumber > PageCount)
                    {
                        continue;
                    }

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
            int firstPageInPair,
            int addressLsb,
            byte[] data)
        {
            int dataOffset = 0;
            int relativeAddress = addressLsb;

            while (dataOffset < data.Length && relativeAddress <= 0x7F)
            {
                int pageNumber = relativeAddress < 0x40
                    ? firstPageInPair
                    : firstPageInPair + 1;
                int pageOffset = relativeAddress & 0x3F;
                int bytesAvailableInPage = BytesPerPage - pageOffset;
                int segmentLength = Math.Min(
                    bytesAvailableInPage,
                    data.Length - dataOffset);

                if (pageNumber >= 1 && pageNumber <= PageCount &&
                    pageOffset == 0 &&
                    segmentLength >= FullFrameWithoutLastColumnBytes)
                {
                    Array.Clear(
                        _pages[pageNumber - 1],
                        0,
                        BytesPerPage);
                }

                dataOffset += segmentLength;
                relativeAddress += segmentLength;
            }
        }

        public bool ExpireDisplay()
        {
            if (CurrentPage == 0)
            {
                return false;
            }

            CurrentPage = 0;
            return true;
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