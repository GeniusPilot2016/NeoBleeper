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

using NeoBleeper.Properties;

namespace NeoBleeper
{
    public partial class PortamentoWindow : Form
    {
        bool darkTheme = false;
        public long wantedPitch;
        public bool needToStopSound;
        public PortamentoWindow(main_window main_Window)
        {
            InitializeComponent();
            switch (TemporarySettings.PortamentoSettings.portamentoType)
            {
                case TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound:
                    radioButtonAlwaysProduceSound.Checked = true;
                    break;
                case TemporarySettings.PortamentoSettings.PortamentoType.ProduceSoundForLength:
                    radioButtonProduceSoundForManyMilliseconds.Checked = true;
                    break;
            }
            trackBarLength.Value = TemporarySettings.PortamentoSettings.length;
            trackBarPitchChangeSpeed.Value = TemporarySettings.PortamentoSettings.pitchChangeSpeed;
            labelLength.Text = TemporarySettings.PortamentoSettings.length.ToString() + Resources.TextMilliseconds;
            setLabelsToMiddle();
            UIFonts.setFonts(this);
            set_theme();
        }
        private void set_theme()
        {
            this.SuspendLayout(); // Suspend layout to batch updates
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering

            try
            {
                switch (Settings1.Default.theme)
                {
                    case 0:
                        if (check_system_theme.IsDarkTheme())
                        {
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;

                    case 1:
                        light_theme();
                        break;

                    case 2:
                        dark_theme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
            }
        }
        private void light_theme()
        {
            darkTheme = false;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    if (ctrl is GroupBox groupBox)
                    {
                        ctrl.BackColor = SystemColors.Control;
                        ctrl.ForeColor = SystemColors.ControlText;
                        foreach (Control childCtrl in groupBox.Controls)
                        {
                            childCtrl.BackColor = SystemColors.Control;
                            childCtrl.ForeColor = SystemColors.ControlText;
                        }
                    }
                    else
                    {
                        ctrl.BackColor = SystemColors.Control;
                        ctrl.ForeColor = SystemColors.ControlText;
                    }
                }
            }
        }
        private void dark_theme()
        {
            darkTheme = true;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    if (ctrl is GroupBox groupBox)
                    {
                        ctrl.BackColor = Color.FromArgb(32, 32, 32);
                        ctrl.ForeColor = Color.White;
                        foreach (Control childCtrl in groupBox.Controls)
                        {
                            childCtrl.BackColor = Color.FromArgb(32, 32, 32);
                            childCtrl.ForeColor = Color.White;
                        }
                    }
                    else
                    {
                        ctrl.BackColor = Color.FromArgb(32, 32, 32);
                        ctrl.ForeColor = Color.White;
                    }
                }
            }
        }
        private void setLabelsToMiddle()
        {
            labelLength.Location = new Point((int)((groupBox1.Width - labelLength.Width) / 2), (labelLength.Location.Y));
            label2.Location = new Point((int)((this.Width - label2.Width) / 2), (label2.Location.Y));
        }
        private void trackBarLength_Scroll(object sender, EventArgs e)
        {
            TemporarySettings.PortamentoSettings.length = trackBarLength.Value;
            finishTimer.Interval = TemporarySettings.PortamentoSettings.length;
            labelLength.Text = trackBarLength.Value.ToString() + Resources.TextMilliseconds;
            setLabelsToMiddle();
            Logger.Log("Portamento length set to " + trackBarLength.Value.ToString() + " mS", Logger.LogTypes.Info);
        }

        private void radioButtons_Checked_Changed(object sender, EventArgs e)
        {
            if (radioButtonAlwaysProduceSound.Checked)
            {
                TemporarySettings.PortamentoSettings.portamentoType = TemporarySettings.PortamentoSettings.PortamentoType.AlwaysProduceSound;
                if (wantedPitch > 37 && wantedPitch < 32767)
                {
                    NotePlayer.play_note((int)wantedPitch, 1, true); // Play the note immediately if the wanted pitch is valid
                }
                Logger.Log("Portamento type set to \"System speaker/sound device always produces sound\"", Logger.LogTypes.Info);
            }
            else if (radioButtonProduceSoundForManyMilliseconds.Checked)
            {
                NotePlayer.StopAllNotes(); // Stop all notes when switching to this mode
                TemporarySettings.PortamentoSettings.portamentoType = TemporarySettings.PortamentoSettings.PortamentoType.ProduceSoundForLength;
                Logger.Log("Portamento type set to \"System speaker/sound produces sound for roughly this many milliseconds\"", Logger.LogTypes.Info);
            }
        }

        private void trackBarPitchChangeSpeed_Scroll(object sender, EventArgs e)
        {
            TemporarySettings.PortamentoSettings.pitchChangeSpeed = trackBarPitchChangeSpeed.Value;
            Logger.Log("Portamento pitch change speed set to " + trackBarPitchChangeSpeed.Value.ToString(), Logger.LogTypes.Info);
        }

        private void finishTimer_Tick(object sender, EventArgs e)
        {
            needToStopSound = true; // Set the flag to stop the sound
        }

        private void PortamentoWindow_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void PortamentoWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            main_window mainWindow = new main_window();
            mainWindow.checkBox_bleeper_portamento.Checked = false;
        }
    }
}
