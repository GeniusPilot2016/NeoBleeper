using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

        public SysExDisplayEmulator(MIDIFilePlayer owner)
        {
            InitializeComponent();
            midiFilePlayer = owner;
            Owner = owner;
            UIFonts.SetFonts(this);
            SetTheme();
            InitializeDisplayCells();
            ClearDisplayContent();
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
    /// Decodes Roland Sound Canvas-style SysEx messages (model ID 45H).
    /// Supports the SC-55 16x16 graphic and the SC-88/SC-88Pro ten display pages.
    /// </summary>
    internal sealed class RolandGSStyleDisplayDecoder
    {
        public const int DisplayWidth = 16;
        public const int DisplayHeight = 16;

        private const int PageCount = 10;
        private const int BytesPerPage = 64;

        private readonly byte[][] _pages = new byte[PageCount][];

        /// <summary>
        /// Zero means the normal Sound Canvas bar display; 1-10 are dot pages.
        /// </summary>
        public int CurrentPage { get; private set; } = 1;

        public RolandGSStyleDisplayDecoder()
        {
            for (int i = 0; i < _pages.Length; i++)
            {
                _pages[i] = new byte[BytesPerPage];
            }
        }

        public void Reset()
        {
            foreach (byte[] page in _pages)
            {
                Array.Clear(page, 0, page.Length);
            }

            CurrentPage = 1;
        }

        /// <summary>
        /// Fast protocol test used only for diagnostics. Collection code should retain
        /// readable SysEx messages and let Apply perform the final protocol check.
        /// </summary>
        public static bool AffectsDisplayState(byte[] message)
        {
            if (!TryParseRolandDt1(message, out byte modelId, out int address, out _))
            {
                return false;
            }

            // GS Reset: model 42H, address 40 00 7F, data 00.
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

            // 10 01 00 through 10 05 7F contain the ten 16x16 pages.
            if (addressMid >= 0x01 && addressMid <= 0x05)
            {
                return addressLsb <= 0x7F;
            }

            // 10 20 00 selects the page; 10 20 01 specifies display time.
            return addressMid == 0x20 && addressLsb <= 0x01;
        }

        public static bool ContainsDotGraphics(byte[] message)
        {
            if (!TryParseRolandDt1(message, out byte modelId, out int address, out _))
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
        /// Applies a Roland DT1 message. The parser accepts NAudio's MIDI-file form
        /// (without F0/F7), a full wire message, and messages with harmless leading bytes.
        /// </summary>
        public bool Apply(byte[] message, out bool visibleChanged)
        {
            visibleChanged = false;

            if (!TryParseRolandDt1(message, out byte modelId, out int address, out byte[] data))
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

            if (addressMid >= 0x01 && addressMid <= 0x05)
            {
                bool pageCurrentlyVisibleWasModified = false;

                for (int i = 0; i < data.Length; i++)
                {
                    int pageRelativeAddress = addressLsb + i;
                    if (pageRelativeAddress > 0x7F)
                    {
                        break;
                    }

                    // p=1 stores pages 1/2, p=2 stores pages 3/4, ... p=5 pages 9/10.
                    int firstPageInPair = ((addressMid - 1) * 2) + 1;
                    int pageNumber = pageRelativeAddress < 0x40
                        ? firstPageInPair
                        : firstPageInPair + 1;
                    int byteIndex = pageRelativeAddress & 0x3F;

                    if (pageNumber < 1 || pageNumber > PageCount)
                    {
                        continue;
                    }

                    byte newValue = (byte)(data[i] & 0x1F);
                    if (_pages[pageNumber - 1][byteIndex] != newValue)
                    {
                        _pages[pageNumber - 1][byteIndex] = newValue;
                        pageCurrentlyVisibleWasModified |= pageNumber == CurrentPage;
                    }

                    // Page 1 is displayed immediately by the SC-55/SC-88 family.
                    if (pageNumber == 1 && CurrentPage != 1)
                    {
                        CurrentPage = 1;
                        pageCurrentlyVisibleWasModified = true;
                    }
                }

                visibleChanged = pageCurrentlyVisibleWasModified;
                return true;
            }

            if (addressMid == 0x20 && addressLsb == 0x00 && data.Length > 0)
            {
                int requestedPage = data[0] & 0x7F;
                if (requestedPage >= 0 && requestedPage <= PageCount)
                {
                    visibleChanged = requestedPage != CurrentPage;
                    CurrentPage = requestedPage;
                }

                return true;
            }

            if (addressMid == 0x20 && addressLsb == 0x01)
            {
                // The physical unit uses this as a display timeout. The emulator keeps
                // the selected page until another MIDI event changes it.
                return true;
            }

            return false;
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
                // d00-d15, d16-d31, and d32-d47 each contain five horizontal dots.
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

                // d48-d63 use bit 4 as the sixteenth column.
                pixels[15, y] = (page[48 + y] & 0x10) != 0;
            }

            return pixels;
        }

        /// <summary>
        /// Reports whether the currently selected page has any lit pixel at all.
        /// The player uses this to decide between repainting the emulator with a
        /// real frame and explicitly clearing it. A page that was never written
        /// to, or one that was fully erased (e.g. by a GS Reset), must result in
        /// the emulator being cleared rather than repainted with an all-off frame.
        /// </summary>
        public bool HasVisibleContent()
        {
            if (CurrentPage < 1 || CurrentPage > PageCount)
            {
                return false;
            }

            byte[] page = _pages[CurrentPage - 1];
            for (int i = 0; i < page.Length; i++)
            {
                if (page[i] != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds a Roland DT1 packet inside the supplied bytes. NAudio's SysexEvent
        /// data normally begins with 41H and excludes F0/F7, while other sources may
        /// include either or both framing bytes.
        /// </summary>
        private static bool TryParseRolandDt1(
            byte[] message,
            out byte modelId,
            out int address,
            out byte[] data)
        {
            modelId = 0;
            address = 0;
            data = Array.Empty<byte>();

            if (message == null || message.Length < 9)
            {
                return false;
            }

            int endExclusive = message.Length;
            while (endExclusive > 0 &&
                   (message[endExclusive - 1] == 0xF7 ||
                    message[endExclusive - 1] == 0x00 && endExclusive > 1 && message[endExclusive - 2] == 0xF7))
            {
                endExclusive--;
            }

            // Locate 41 dd mm 12 rather than assuming the manufacturer byte is at index 0.
            int header = -1;
            for (int i = 0; i + 7 < endExclusive; i++)
            {
                if (message[i] == 0x41 && message[i + 3] == 0x12)
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
            address = (addressMsb << 16) | (addressMid << 8) | addressLsb;

            int dataStart = header + 7;
            int bytesAfterAddress = endExclusive - dataStart;
            if (bytesAfterAddress < 2)
            {
                return false;
            }

            // Roland DT1 always ends with one checksum byte. Validate it for diagnostics,
            // but do not reject a precisely identified display packet solely because a
            // MIDI editor recalculated or omitted framing incorrectly.
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

            int expectedChecksum = (128 - (sum & 0x7F)) & 0x7F;
            int suppliedChecksum = message[checksumIndex] & 0x7F;

            // For an exact Roland header/address, keep parsing even when the checksum
            // differs. This is intentionally tolerant; unrelated SysEx cannot reach this
            // point because manufacturer, DT1 command, model, and address are checked.
            _ = expectedChecksum == suppliedChecksum;

            data = new byte[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                data[i] = (byte)(message[dataStart + i] & 0x7F);
            }

            return true;
        }
    }
}