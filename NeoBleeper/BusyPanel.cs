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

using System.ComponentModel;
using System.Media;

namespace NeoBleeper
{
    public class BusyPanel : Panel
    {
        public BusyPanel()
        {
            this.Click += OnClick;
            this.KeyPress += OnKeyPress;
        }
        private float opacity = 1f;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DefaultValue(1f)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Opacity
        {
            get => opacity;
            set
            {
                opacity = Math.Max(0, Math.Min(1, value));
                this.Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!this.Visible)
                return;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb((int)(opacity * 255), this.BackColor)))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private void OnClick(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play(); // Play a beep sound on click
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            SystemSounds.Beep.Play(); // Play a beep sound on key press
        }
    }
    public class BusyFormHelper
    {
        private BusyPanel busyPanel = new BusyPanel
        {
            BackColor = Color.Gray,
            Opacity = 0f,
            Size = new Size(0, 0),
            Location = new Point(0, 0),
            Visible = true
        };
        public void SetFormBusy(Form form, bool busy)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() => SetFormBusy(form, busy)));
                return;
            }
            form.SuspendLayout();
            if (busy)
            {
                busyPanel.Size = form.ClientSize;
                busyPanel.Location = new Point(0, 0);
                if (!form.Controls.Contains(busyPanel))
                {
                    form.Controls.Add(busyPanel);
                    busyPanel.BringToFront();
                }
            }
            form.UseWaitCursor = busy;
            form.Invalidate();
            busyPanel.Visible = busy;
            form.ResumeLayout();
            RefreshTrackBarsInForm(form);
        }

        private void RefreshTrackBar(Control ctrl)
        {
            if (ctrl is TrackBar trackBar)
            {
                trackBar.Refresh();
            }
        }
        private void RefreshTrackBarsInFormRecursively(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                RefreshTrackBar(ctrl);
                if (ctrl.HasChildren)
                {
                    RefreshTrackBarsInFormRecursively(ctrl);
                }
            }
        }
        private void RefreshTrackBarsInForm(Form form)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() => RefreshTrackBarsInForm(form)));
                return;
            }
            RefreshTrackBarsInFormRecursively(form);
        }
    }
}
