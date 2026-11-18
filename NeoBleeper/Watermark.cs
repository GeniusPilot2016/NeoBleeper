using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class WatermarkControl : Control
    {
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WM_NCHITTEST = 0x0084;
        private const int HTTRANSPARENT = -1;

        private int _opacity = 110;
        private float _angle = -30f;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;

        private OverlayForm _overlay;
        private Form _host;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        [Description("The editable watermark text displayed on the control.")]
        [DefaultValue("RELEASE CANDIDATE\nFor Testing & Evaluation Only")]
        public override string Text
        {
            get => base.Text;
            set { base.Text = value; Refresh(); }
        }

        [Category("Watermark")]
        [Description("Text opacity from 0 (fully invisible) to 255 (fully solid).")]
        [DefaultValue(110)]
        public int Opacity
        {
            get => _opacity;
            set { _opacity = Math.Max(0, Math.Min(255, value)); Refresh(); }
        }

        [Category("Watermark")]
        [Description("Alignment of the text inside the control.")]
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set { _textAlign = value; Refresh(); }
        }

        [Category("Watermark")]
        [Description("Rotation angle in degrees (e.g., -30 for diagonal text).")]
        [DefaultValue(-30f)]
        public float Angle
        {
            get => _angle;
            set { _angle = value; Refresh(); }
        }

        public WatermarkControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);

            base.Text = "RELEASE CANDIDATE\nFor Testing & Evaluation Only";
            BackColor = Color.Transparent;
            ForeColor = Color.Gray;
            Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            Dock = DockStyle.Fill;
            TabStop = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TRANSPARENT;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST && !DesignMode)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }
            base.WndProc(ref m);
        }

        // ---------- Painting of the control itself ----------

        protected override void OnPaintBackground(PaintEventArgs pevent) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            // At runtime the overlay window draws the watermark; the control paints nothing.
            // In the designer, draw a preview so the text is visible while editing.
            if (!DesignMode) return;
            RenderWatermark(e.Graphics, ClientSize);
        }

        // ---------- Overlay management ----------

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
                BeginInvoke((MethodInvoker)AttachOverlay);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            DetachOverlay();
            base.OnHandleDestroyed(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            UpdateOverlay();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateOverlay();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            UpdateOverlay();
        }

        private void AttachOverlay()
        {
            if (IsDisposed || _overlay != null) return;

            _host = FindForm();
            if (_host == null) return;

            if (!_host.Visible)
            {
                // Owner must be shown before the owned overlay.
                EventHandler shown = null;
                shown = (s, a) => { _host.Shown -= shown; AttachOverlay(); };
                _host.Shown += shown;
                return;
            }

            _overlay = new OverlayForm();
            _overlay.Show(_host);

            _host.LocationChanged += HostChanged;
            _host.SizeChanged += HostChanged;
            _host.FormClosed += HostClosed;

            UpdateOverlay();
        }

        private void DetachOverlay()
        {
            if (_host != null)
            {
                _host.LocationChanged -= HostChanged;
                _host.SizeChanged -= HostChanged;
                _host.FormClosed -= HostClosed;
                _host = null;
            }
            if (_overlay != null)
            {
                _overlay.Dispose();
                _overlay = null;
            }
        }

        private void HostClosed(object sender, FormClosedEventArgs e) => DetachOverlay();
        private void HostChanged(object sender, EventArgs e) => UpdateOverlay();

        public override void Refresh()
        {
            base.Refresh();
            UpdateOverlay();
        }

        private void UpdateOverlay()
        {
            if (DesignMode || _overlay == null || _overlay.IsDisposed || !IsHandleCreated) return;

            bool shouldShow = Visible && _host != null &&
                              _host.WindowState != FormWindowState.Minimized &&
                              ClientSize.Width > 0 && ClientSize.Height > 0;

            if (!shouldShow)
            {
                if (_overlay.Visible) _overlay.Hide();
                return;
            }

            if (!_overlay.Visible) _overlay.Show(_host);

            Rectangle screen = RectangleToScreen(ClientRectangle);
            using (Bitmap bmp = new Bitmap(screen.Width, screen.Height, PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                RenderWatermark(g, screen.Size);
                _overlay.SetBitmap(bmp, screen.Location);
            }
        }

        // ---------- Shared text rendering ----------

        private void RenderWatermark(Graphics g, Size size)
        {
            if (string.IsNullOrEmpty(Text)) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Plain AntiAlias: ClearType/GridFit don't honor alpha on a transparent surface.
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            Color color = Color.FromArgb(_opacity, ForeColor);
            float padding = 16f;

            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            using (Brush brush = new SolidBrush(color))
            {
                SizeF textSize = g.MeasureString(Text, Font, size.Width, sf);

                float cx = size.Width / 2f;
                float cy = size.Height / 2f;

                if (_textAlign == ContentAlignment.TopLeft || _textAlign == ContentAlignment.MiddleLeft || _textAlign == ContentAlignment.BottomLeft)
                    cx = padding + textSize.Width / 2f;
                else if (_textAlign == ContentAlignment.TopRight || _textAlign == ContentAlignment.MiddleRight || _textAlign == ContentAlignment.BottomRight)
                    cx = size.Width - padding - textSize.Width / 2f;

                if (_textAlign == ContentAlignment.TopLeft || _textAlign == ContentAlignment.TopCenter || _textAlign == ContentAlignment.TopRight)
                    cy = padding + textSize.Height / 2f;
                else if (_textAlign == ContentAlignment.BottomLeft || _textAlign == ContentAlignment.BottomCenter || _textAlign == ContentAlignment.BottomRight)
                    cy = size.Height - padding - textSize.Height / 2f;

                g.TranslateTransform(cx, cy);
                if (_angle != 0f) g.RotateTransform(_angle);
                g.DrawString(Text, Font, brush, 0, 0, sf);
                g.ResetTransform();
            }
        }

        // ---------- Click-through per-pixel-alpha overlay window ----------

        private sealed class OverlayForm : Form
        {
            private const int WS_EX_LAYERED = 0x00080000;
            private const int WS_EX_TRANSPARENT = 0x00000020;
            private const int WS_EX_NOACTIVATE = 0x08000000;
            private const int WS_EX_TOOLWINDOW = 0x00000080;
            private const int ULW_ALPHA = 0x02;
            private const byte AC_SRC_OVER = 0x00;
            private const byte AC_SRC_ALPHA = 0x01;

            [StructLayout(LayoutKind.Sequential)]
            private struct POINT { public int x, y; }

            [StructLayout(LayoutKind.Sequential)]
            private struct SIZE { public int cx, cy; }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct BLENDFUNCTION
            {
                public byte BlendOp, BlendFlags, SourceConstantAlpha, AlphaFormat;
            }

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst,
                ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc,
                int crKey, ref BLENDFUNCTION pblend, int dwFlags);

            [DllImport("user32.dll")] private static extern IntPtr GetDC(IntPtr hWnd);
            [DllImport("user32.dll")] private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("gdi32.dll")] private static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
            [DllImport("gdi32.dll")] private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
            [DllImport("gdi32.dll")] private static extern bool DeleteObject(IntPtr hObject);

            public OverlayForm()
            {
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                TopMost = false;
            }

            protected override bool ShowWithoutActivation => true;

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                    return cp;
                }
            }

            public void SetBitmap(Bitmap bitmap, Point location)
            {
                if (!IsHandleCreated) return;

                IntPtr screenDc = GetDC(IntPtr.Zero);
                IntPtr memDc = CreateCompatibleDC(screenDc);
                IntPtr hBitmap = IntPtr.Zero;
                IntPtr oldBitmap = IntPtr.Zero;

                try
                {
                    hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                    oldBitmap = SelectObject(memDc, hBitmap);

                    SIZE size = new SIZE { cx = bitmap.Width, cy = bitmap.Height };
                    POINT src = new POINT { x = 0, y = 0 };
                    POINT dst = new POINT { x = location.X, y = location.Y };
                    BLENDFUNCTION blend = new BLENDFUNCTION
                    {
                        BlendOp = AC_SRC_OVER,
                        BlendFlags = 0,
                        SourceConstantAlpha = 255,
                        AlphaFormat = AC_SRC_ALPHA
                    };

                    UpdateLayeredWindow(Handle, screenDc, ref dst, ref size, memDc, ref src, 0, ref blend, ULW_ALPHA);
                }
                finally
                {
                    ReleaseDC(IntPtr.Zero, screenDc);
                    if (hBitmap != IntPtr.Zero)
                    {
                        SelectObject(memDc, oldBitmap);
                        DeleteObject(hBitmap);
                    }
                    DeleteDC(memDc);
                }
            }
        }
    }
}