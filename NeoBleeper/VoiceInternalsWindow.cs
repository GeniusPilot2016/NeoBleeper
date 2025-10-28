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
using System.Globalization;

namespace NeoBleeper
{
    public partial class VoiceInternalsWindow : Form
    {
        main_window main_Window;
        bool darkTheme = false;
        public VoiceInternalsWindow(main_window main_Window)
        {
            this.main_Window = main_Window;
            InitializeComponent();
            UIFonts.setFonts(this);
            set_theme();
            trackBarFormant1Vol.Value = setReverseTrackBarValue(trackBarFormant1Vol, TemporarySettings.VoiceInternalSettings.Formant1Volume);
            trackBarFormant2Vol.Value = setReverseTrackBarValue(trackBarFormant2Vol, TemporarySettings.VoiceInternalSettings.Formant2Volume);
            trackBarFormant3Vol.Value = setReverseTrackBarValue(trackBarFormant3Vol, TemporarySettings.VoiceInternalSettings.Formant3Volume);
            trackBarFormant4Vol.Value = setReverseTrackBarValue(trackBarFormant4Vol, TemporarySettings.VoiceInternalSettings.Formant4Volume);
            trackBarFormant1Hz.Value = setReverseTrackBarValue(trackBarFormant1Hz, TemporarySettings.VoiceInternalSettings.Formant1Frequency);
            trackBarFormant2Hz.Value = setReverseTrackBarValue(trackBarFormant2Hz, TemporarySettings.VoiceInternalSettings.Formant2Frequency);
            trackBarFormant3Hz.Value = setReverseTrackBarValue(trackBarFormant3Hz, TemporarySettings.VoiceInternalSettings.Formant3Frequency);
            trackBarFormant4Hz.Value = setReverseTrackBarValue(trackBarFormant4Hz, TemporarySettings.VoiceInternalSettings.Formant4Frequency);
            trackBarVoiceVol.Value = setReverseTrackBarValue(trackBarVoiceVol, TemporarySettings.VoiceInternalSettings.VoiceVolume);
            trackBarSawVol.Value = setReverseTrackBarValue(trackBarSawVol, TemporarySettings.VoiceInternalSettings.SawVolume);
            trackBarNoiseVol.Value = setReverseTrackBarValue(trackBarNoiseVol, TemporarySettings.VoiceInternalSettings.NoiseVolume);
            trackBarSybillance1Ra.Value = setReverseTrackBarValue(trackBarSybillance1Ra, (int)(TemporarySettings.VoiceInternalSettings.Sybillance1Range * 1000));
            trackBarSybillance2Ra.Value = setReverseTrackBarValue(trackBarSybillance2Ra, (int)(TemporarySettings.VoiceInternalSettings.Sybillance2Range * 1000));
            trackBarSybillance3Ra.Value = setReverseTrackBarValue(trackBarSybillance3Ra, (int)(TemporarySettings.VoiceInternalSettings.Sybillance3Range * 1000));
            trackBarSybillance4Ra.Value = setReverseTrackBarValue(trackBarSybillance4Ra, (int)(TemporarySettings.VoiceInternalSettings.Sybillance4Range * 1000));
            trackBarSybillance1Vol.Value = setReverseTrackBarValue(trackBarSybillance1Vol, (int)(TemporarySettings.VoiceInternalSettings.Sybillance1Volume * 1000));
            trackBarSybillance2Vol.Value = setReverseTrackBarValue(trackBarSybillance2Vol, (int)(TemporarySettings.VoiceInternalSettings.Sybillance2Volume * 1000));
            trackBarSybillance3Vol.Value = setReverseTrackBarValue(trackBarSybillance3Vol, (int)(TemporarySettings.VoiceInternalSettings.Sybillance3Volume * 1000));
            trackBarSybillance4Vol.Value = setReverseTrackBarValue(trackBarSybillance4Vol, (int)(TemporarySettings.VoiceInternalSettings.Sybillance4Volume * 1000));
            trackBarSybillance1Hz.Value = setReverseTrackBarValue(trackBarSybillance1Hz, TemporarySettings.VoiceInternalSettings.Sybillance1Frequency);
            trackBarSybillance2Hz.Value = setReverseTrackBarValue(trackBarSybillance2Hz, TemporarySettings.VoiceInternalSettings.Sybillance2Frequency);
            trackBarSybillance3Hz.Value = setReverseTrackBarValue(trackBarSybillance3Hz, TemporarySettings.VoiceInternalSettings.Sybillance3Frequency);
            trackBarSybillance4Hz.Value = setReverseTrackBarValue(trackBarSybillance4Hz, TemporarySettings.VoiceInternalSettings.Sybillance4Frequency);
            comboBoxNote1Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.Note1OutputDeviceIndex;
            comboBoxNote2Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.Note2OutputDeviceIndex;
            comboBoxNote3Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.Note3OutputDeviceIndex;
            comboBoxNote4Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.Note4OutputDeviceIndex;
            labelFormant1Hz.Text = TemporarySettings.VoiceInternalSettings.Formant1Frequency.ToString() + " Hz";
            labelFormant2Hz.Text = TemporarySettings.VoiceInternalSettings.Formant2Frequency.ToString() + " Hz";
            labelFormant3Hz.Text = TemporarySettings.VoiceInternalSettings.Formant3Frequency.ToString() + " Hz";
            labelFormant4Hz.Text = TemporarySettings.VoiceInternalSettings.Formant4Frequency.ToString() + " Hz";
            labelFormant1Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant1Volume.ToString());
            labelFormant2Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant2Volume.ToString());
            labelFormant3Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant3Volume.ToString());
            labelFormant4Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant4Volume.ToString());
            labelVoiceVol.Text = TemporarySettings.VoiceInternalSettings.VoiceVolume.ToString();
            labelSawVol.Text = TemporarySettings.VoiceInternalSettings.SawVolume.ToString();
            labelNoiseVol.Text = TemporarySettings.VoiceInternalSettings.NoiseVolume.ToString();
            labelSybillance1Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance1Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance2Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance2Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance3Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance3Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance4Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance4Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance1Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance1Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance2Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance2Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance3Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance3Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance4Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance4Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance1Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance1Frequency.ToString() + " Hz";
            labelSybillance2Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance2Frequency.ToString() + " Hz";
            labelSybillance3Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance3Frequency.ToString() + " Hz";
            labelSybillance4Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance4Frequency.ToString() + " Hz";
            double minPitch = 0.5, maxPitch = 2.0;
            int pitchSpan = trackBarTimbre.Maximum - trackBarTimbre.Minimum;
            trackBarTimbre.Value = trackBarTimbre.Minimum + (int)Math.Round(((TemporarySettings.VoiceInternalSettings.Timbre - minPitch) / (maxPitch - minPitch)) * (pitchSpan > 0 ? pitchSpan : 1));
            comboBoxPlayNoteOnLineOption.SelectedIndex = TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions == TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnCheckedLines ? 1 : 0;
            double minRange = 0, maxRange = 1.0;
            int rangeSpan = trackBarRandomizedFormantFreqRange.Maximum - trackBarRandomizedFormantFreqRange.Minimum;
            trackBarRandomizedFormantFreqRange.Value = trackBarRandomizedFormantFreqRange.Minimum + (int)Math.Round(((TemporarySettings.VoiceInternalSettings.RandomizedFrequencyRange - minRange) / (maxRange - minRange)) * (rangeSpan > 0 ? rangeSpan : 1));
            trackBarCutoffHz.Value = setReverseTrackBarValue(trackBarCutoffHz, TemporarySettings.VoiceInternalSettings.CutoffFrequency);
            labelCutoffHz.Text = TemporarySettings.VoiceInternalSettings.CutoffFrequency.ToString();
            BringToFront();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SETTINGCHANGE = 0x001A;
            base.WndProc(ref m);

            if (m.Msg == WM_SETTINGCHANGE)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme();
                }
            }
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
                        if (SystemThemeUtility.IsDarkTheme())
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
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            labelFormantControl.ForeColor = SystemColors.ControlText;
            labelF1.ForeColor = SystemColors.ControlText;
            labelF2.ForeColor = SystemColors.ControlText;
            labelF3.ForeColor = SystemColors.ControlText;
            labelF4.ForeColor = SystemColors.ControlText;
            labelS1.ForeColor = SystemColors.ControlText;
            labelS2.ForeColor = SystemColors.ControlText;
            labelS3.ForeColor = SystemColors.ControlText;
            labelS4.ForeColor = SystemColors.ControlText;
            labelSybillance.ForeColor = SystemColors.ControlText;
            labelVoice.ForeColor = SystemColors.ControlText;
            labelSaw.ForeColor = SystemColors.ControlText;
            labelNoise.ForeColor = SystemColors.ControlText;
            labelOscillator.ForeColor = SystemColors.ControlText;
            labelSybillanceMasking.ForeColor = SystemColors.ControlText;
            labelCutoff.ForeColor = SystemColors.ControlText;
            comboBoxNote1Option.ForeColor = Color.White;
            comboBoxNote2Option.ForeColor = Color.White;
            comboBoxNote3Option.ForeColor = Color.White;
            comboBoxNote4Option.ForeColor = Color.White;
            comboBoxNote1Option.BackColor = Color.Black;
            comboBoxNote2Option.BackColor = Color.Black;
            comboBoxNote3Option.BackColor = Color.Black;
            comboBoxNote4Option.BackColor = Color.Black;
            groupBoxRandomVariationsOfFormants.ForeColor = Color.White;
            groupBoxOutputOptions.ForeColor = Color.White;
            groupBoxKey.ForeColor = Color.White;
            buttonOpenVowel.BackColor = Color.FromArgb(32, 32, 32);
            buttonOpenBack.BackColor = Color.FromArgb(32, 32, 32);
            buttonMidFront.BackColor = Color.FromArgb(32, 32, 32);
            buttonCloseBack.BackColor = Color.FromArgb(32, 32, 32);
            buttonCloseFront.BackColor = Color.FromArgb(32, 32, 32);
            groupBoxPlayVoiceOnLineSettings.ForeColor = Color.White;
            comboBoxPlayNoteOnLineOption.BackColor = Color.Black;
            comboBoxPlayNoteOnLineOption.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            comboBoxNote1Option.BackColor = SystemColors.Window;
            comboBoxNote2Option.BackColor = SystemColors.Window;
            comboBoxNote3Option.BackColor = SystemColors.Window;
            comboBoxNote4Option.BackColor = SystemColors.Window;
            comboBoxNote1Option.ForeColor = SystemColors.WindowText;
            comboBoxNote2Option.ForeColor = SystemColors.WindowText;
            comboBoxNote3Option.ForeColor = SystemColors.WindowText;
            comboBoxNote4Option.ForeColor = SystemColors.WindowText;
            groupBoxRandomVariationsOfFormants.ForeColor = SystemColors.ControlText;
            groupBoxOutputOptions.ForeColor = SystemColors.ControlText;
            groupBoxKey.ForeColor = SystemColors.ControlText;
            buttonOpenVowel.BackColor = Color.Transparent;
            buttonOpenBack.BackColor = Color.Transparent;
            buttonMidFront.BackColor = Color.Transparent;
            buttonCloseBack.BackColor = Color.Transparent;
            buttonCloseFront.BackColor = Color.Transparent;
            groupBoxPlayVoiceOnLineSettings.ForeColor = SystemColors.ControlText;
            comboBoxPlayNoteOnLineOption.BackColor = SystemColors.Window;
            comboBoxPlayNoteOnLineOption.ForeColor = SystemColors.WindowText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private int getTrackBarReverseValue(TrackBar trackBar) // Because TrackBar control does not support reversed direction natively
        {
            return trackBar.Maximum - trackBar.Value + trackBar.Minimum;
        }
        private int setReverseTrackBarValue(TrackBar trackBar, int value) // Because TrackBar control does not support reversed direction natively
        {
            return trackBar.Maximum - value + trackBar.Minimum;
        }

        private void trackBarFormant1Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant1Volume != getTrackBarReverseValue(trackBarFormant1Vol))
            {
                TemporarySettings.VoiceInternalSettings.Formant1Volume = getTrackBarReverseValue(trackBarFormant1Vol);
                labelFormant1Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant1Volume.ToString());
                Logger.Log("Formant 1 Volume set to " + TemporarySettings.VoiceInternalSettings.Formant1Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant1Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant1Frequency != getTrackBarReverseValue(trackBarFormant1Hz))
            {
                TemporarySettings.VoiceInternalSettings.Formant1Frequency = getTrackBarReverseValue(trackBarFormant1Hz);
                labelFormant1Hz.Text = TemporarySettings.VoiceInternalSettings.Formant1Frequency.ToString() + " Hz";
                Logger.Log("Formant 1 Frequency set to " + TemporarySettings.VoiceInternalSettings.Formant1Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant2Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant2Volume != getTrackBarReverseValue(trackBarFormant2Vol))
            {
                TemporarySettings.VoiceInternalSettings.Formant2Volume = getTrackBarReverseValue(trackBarFormant2Vol);
                labelFormant2Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant2Volume.ToString());
                Logger.Log("Formant 2 Volume set to " + TemporarySettings.VoiceInternalSettings.Formant2Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant2Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant2Frequency != getTrackBarReverseValue(trackBarFormant2Hz))
            {
                TemporarySettings.VoiceInternalSettings.Formant2Frequency = getTrackBarReverseValue(trackBarFormant2Hz);
                labelFormant2Hz.Text = TemporarySettings.VoiceInternalSettings.Formant2Frequency.ToString() + " Hz";
                Logger.Log("Formant 2 Frequency set to " + TemporarySettings.VoiceInternalSettings.Formant2Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant3Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant3Volume != getTrackBarReverseValue(trackBarFormant3Vol))
            {
                TemporarySettings.VoiceInternalSettings.Formant3Volume = getTrackBarReverseValue(trackBarFormant3Vol);
                labelFormant3Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant3Volume.ToString());
                Logger.Log("Formant 3 Volume set to " + TemporarySettings.VoiceInternalSettings.Formant3Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant3Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant3Frequency != getTrackBarReverseValue(trackBarFormant3Hz))
            {
                TemporarySettings.VoiceInternalSettings.Formant3Frequency = getTrackBarReverseValue(trackBarFormant3Hz);
                labelFormant3Hz.Text = TemporarySettings.VoiceInternalSettings.Formant3Frequency.ToString() + " Hz";
                Logger.Log("Formant 3 Frequency set to " + TemporarySettings.VoiceInternalSettings.Formant3Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant4Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant4Volume != getTrackBarReverseValue(trackBarFormant4Vol))
            {
                TemporarySettings.VoiceInternalSettings.Formant4Volume = getTrackBarReverseValue(trackBarFormant4Vol);
                labelFormant4Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant4Volume.ToString());
                Logger.Log("Formant 4 Volume set to " + TemporarySettings.VoiceInternalSettings.Formant4Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant4Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Formant4Frequency != getTrackBarReverseValue(trackBarFormant4Hz))
            {
                TemporarySettings.VoiceInternalSettings.Formant4Frequency = getTrackBarReverseValue(trackBarFormant4Hz);
                labelFormant4Hz.Text = TemporarySettings.VoiceInternalSettings.Formant4Frequency.ToString() + " Hz";
                Logger.Log("Formant 4 Frequency set to " + TemporarySettings.VoiceInternalSettings.Formant4Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarVoiceVol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.VoiceVolume != getTrackBarReverseValue(trackBarVoiceVol))
            {
                TemporarySettings.VoiceInternalSettings.VoiceVolume = getTrackBarReverseValue(trackBarVoiceVol);
                labelVoiceVol.Text = TemporarySettings.VoiceInternalSettings.VoiceVolume.ToString();
                Logger.Log("Voice Volume set to " + TemporarySettings.VoiceInternalSettings.VoiceVolume.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarSawVol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.SawVolume != getTrackBarReverseValue(trackBarSawVol))
            {
                TemporarySettings.VoiceInternalSettings.SawVolume = getTrackBarReverseValue(trackBarSawVol);
                labelSawVol.Text = TemporarySettings.VoiceInternalSettings.SawVolume.ToString();
                Logger.Log("Saw Volume set to " + TemporarySettings.VoiceInternalSettings.SawVolume.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarNoiseVol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.NoiseVolume != getTrackBarReverseValue(trackBarNoiseVol))
            {
                TemporarySettings.VoiceInternalSettings.NoiseVolume = getTrackBarReverseValue(trackBarNoiseVol);
                labelNoiseVol.Text = TemporarySettings.VoiceInternalSettings.NoiseVolume.ToString();
                Logger.Log("Noise Volume set to " + TemporarySettings.VoiceInternalSettings.NoiseVolume.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance1Ra_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance1Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance1Range != value)
            {
                {
                    TemporarySettings.VoiceInternalSettings.Sybillance1Range = value;
                    labelSybillance1Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance1Range.ToString("0.##", CultureInfo.InvariantCulture);
                    Logger.Log("Sybillance 1 Range set to " + TemporarySettings.VoiceInternalSettings.Sybillance1Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
                }
            }
        }

        private void trackBarSybillance1Vol_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance1Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance1Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance1Volume = value;
                labelSybillance1Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance1Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 1 Volume set to " + TemporarySettings.VoiceInternalSettings.Sybillance1Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance1Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Sybillance1Frequency != getTrackBarReverseValue(trackBarSybillance1Hz))
            {
                TemporarySettings.VoiceInternalSettings.Sybillance1Frequency = getTrackBarReverseValue(trackBarSybillance1Hz);
                labelSybillance1Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance1Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 1 Frequency set to " + TemporarySettings.VoiceInternalSettings.Sybillance1Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance2Ra_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance2Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance2Range != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance2Range = value;
                labelSybillance2Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance2Range.ToString("0.##", CultureInfo.InvariantCulture);
                Logger.Log("Sybillance 2 Range set to " + TemporarySettings.VoiceInternalSettings.Sybillance2Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance2Vol_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance2Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance2Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance2Volume = value;
                labelSybillance2Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance2Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 2 Volume set to " + TemporarySettings.VoiceInternalSettings.Sybillance2Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance2Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Sybillance2Frequency != getTrackBarReverseValue(trackBarSybillance2Hz))
            {
                TemporarySettings.VoiceInternalSettings.Sybillance2Frequency = getTrackBarReverseValue(trackBarSybillance2Hz);
                labelSybillance2Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance2Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 2 Frequency set to " + TemporarySettings.VoiceInternalSettings.Sybillance2Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance3Ra_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance3Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance2Range != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance3Range = value;
                labelSybillance3Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance3Range.ToString("0.##", CultureInfo.InvariantCulture);
                Logger.Log("Sybillance 3 Range set to " + TemporarySettings.VoiceInternalSettings.Sybillance3Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance3Vol_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance3Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance3Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance3Volume = value;
                labelSybillance3Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance3Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 3 Volume set to " + TemporarySettings.VoiceInternalSettings.Sybillance3Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance3Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Sybillance3Frequency != getTrackBarReverseValue(trackBarSybillance3Hz))
            {
                TemporarySettings.VoiceInternalSettings.Sybillance3Frequency = getTrackBarReverseValue(trackBarSybillance3Hz);
                labelSybillance3Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance3Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 3 Frequency set to " + TemporarySettings.VoiceInternalSettings.Sybillance3Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance4Ra_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance4Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance4Range != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance4Range = value;
                labelSybillance4Ra.Text = TemporarySettings.VoiceInternalSettings.Sybillance4Range.ToString("0.##", CultureInfo.InvariantCulture);
                Logger.Log("Sybillance 4 Range set to " + TemporarySettings.VoiceInternalSettings.Sybillance4Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance4Vol_Scroll(object sender, EventArgs e)
        {
            double value = getTrackBarReverseValue(trackBarSybillance4Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.Sybillance4Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.Sybillance4Volume = value;
                labelSybillance4Vol.Text = TemporarySettings.VoiceInternalSettings.Sybillance4Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 4 Volume set to " + TemporarySettings.VoiceInternalSettings.Sybillance4Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance4Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.Sybillance4Frequency != getTrackBarReverseValue(trackBarSybillance4Hz))
            {
                TemporarySettings.VoiceInternalSettings.Sybillance4Frequency = getTrackBarReverseValue(trackBarSybillance4Hz);
                labelSybillance4Hz.Text = TemporarySettings.VoiceInternalSettings.Sybillance4Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 4 Frequency set to " + TemporarySettings.VoiceInternalSettings.Sybillance4Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarPitch_Scroll(object sender, EventArgs e)
        {
            double minPitch = 0.5;
            double maxPitch = 2.0;
            int span = trackBarTimbre.Maximum - trackBarTimbre.Minimum;
            double normalized = span > 0 ? (trackBarTimbre.Value - trackBarTimbre.Minimum) / (double)span : 0.0;
            double value = minPitch + normalized * (maxPitch - minPitch);
            if (Math.Abs(TemporarySettings.VoiceInternalSettings.Timbre - value) > 1e-9)
            {
                TemporarySettings.VoiceInternalSettings.Timbre = value;
                Logger.Log("Timbre set to " + TemporarySettings.VoiceInternalSettings.Timbre.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarRange_Scroll(object sender, EventArgs e)
        {
            double minRange = 0, maxRange = 1.0;
            int span = trackBarRandomizedFormantFreqRange.Maximum - trackBarRandomizedFormantFreqRange.Minimum;
            double normalized = span > 0 ? (trackBarRandomizedFormantFreqRange.Value - trackBarRandomizedFormantFreqRange.Minimum) / (double)span : 0.0;
            double value = minRange + normalized * (maxRange - minRange);
            if (Math.Abs(TemporarySettings.VoiceInternalSettings.RandomizedFrequencyRange - value) > 1e-9)
            {
                TemporarySettings.VoiceInternalSettings.RandomizedFrequencyRange = value;
                Logger.Log("Randomized frequency range set to " + TemporarySettings.VoiceInternalSettings.RandomizedFrequencyRange.ToString(), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote1Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote1Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.Note1OutputDeviceIndex)
            {
                if (comboBoxNote1Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(0);
                }
                TemporarySettings.VoiceInternalSettings.Note1OutputDeviceIndex = comboBoxNote1Option.SelectedIndex;
                Logger.Log("Note 1 Output Device set to index " + (TemporarySettings.VoiceInternalSettings.Note1OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote2Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote2Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.Note2OutputDeviceIndex)
            {
                if (comboBoxNote2Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(1);
                }
                TemporarySettings.VoiceInternalSettings.Note2OutputDeviceIndex = comboBoxNote2Option.SelectedIndex;
                Logger.Log("Note 2 Output Device set to " + (TemporarySettings.VoiceInternalSettings.Note2OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote3Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote3Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.Note3OutputDeviceIndex)
            {
                if (comboBoxNote3Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(2);
                }
                TemporarySettings.VoiceInternalSettings.Note3OutputDeviceIndex = comboBoxNote3Option.SelectedIndex;
                Logger.Log("Note 3 Output Device set to " + (TemporarySettings.VoiceInternalSettings.Note3OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote4Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote4Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.Note4OutputDeviceIndex)
            {
                if (comboBoxNote4Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(3);
                }
                TemporarySettings.VoiceInternalSettings.Note4OutputDeviceIndex = comboBoxNote4Option.SelectedIndex;
                Logger.Log("Note 4 Output Device set to " + (TemporarySettings.VoiceInternalSettings.Note4OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void trackBarCutoffHz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.CutoffFrequency != getTrackBarReverseValue(trackBarCutoffHz))
            {
                TemporarySettings.VoiceInternalSettings.CutoffFrequency = getTrackBarReverseValue(trackBarCutoffHz);
                labelCutoffHz.Text = TemporarySettings.VoiceInternalSettings.CutoffFrequency.ToString();
                Logger.Log("Cutoff Frequency set to " + TemporarySettings.VoiceInternalSettings.CutoffFrequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }
        private void SetPresets(int FormantVol1, int FormantVol2, int FormantVol3, int FormantVol4,
            int Formant1Freq, int Formant2Freq, int Formant3Freq, int Formant4Freq)
        {
            trackBarFormant1Vol.Value = setReverseTrackBarValue(trackBarFormant1Vol, FormantVol1);
            trackBarFormant2Vol.Value = setReverseTrackBarValue(trackBarFormant2Vol, FormantVol2);
            trackBarFormant3Vol.Value = setReverseTrackBarValue(trackBarFormant3Vol, FormantVol3);
            trackBarFormant4Vol.Value = setReverseTrackBarValue(trackBarFormant4Vol, FormantVol4);
            trackBarFormant1Hz.Value = setReverseTrackBarValue(trackBarFormant1Hz, Formant1Freq);
            trackBarFormant2Hz.Value = setReverseTrackBarValue(trackBarFormant2Hz, Formant2Freq);
            trackBarFormant3Hz.Value = setReverseTrackBarValue(trackBarFormant3Hz, Formant3Freq);
            trackBarFormant4Hz.Value = setReverseTrackBarValue(trackBarFormant4Hz, Formant4Freq);
            TemporarySettings.VoiceInternalSettings.Formant1Volume = getTrackBarReverseValue(trackBarFormant1Vol);
            TemporarySettings.VoiceInternalSettings.Formant2Volume = getTrackBarReverseValue(trackBarFormant2Vol);
            TemporarySettings.VoiceInternalSettings.Formant3Volume = getTrackBarReverseValue(trackBarFormant3Vol);
            TemporarySettings.VoiceInternalSettings.Formant4Volume = getTrackBarReverseValue(trackBarFormant4Vol);
            TemporarySettings.VoiceInternalSettings.Formant1Frequency = getTrackBarReverseValue(trackBarFormant1Hz);
            TemporarySettings.VoiceInternalSettings.Formant2Frequency = getTrackBarReverseValue(trackBarFormant2Hz);
            TemporarySettings.VoiceInternalSettings.Formant3Frequency = getTrackBarReverseValue(trackBarFormant3Hz);
            TemporarySettings.VoiceInternalSettings.Formant4Frequency = getTrackBarReverseValue(trackBarFormant4Hz);
            labelFormant1Hz.Text = TemporarySettings.VoiceInternalSettings.Formant1Frequency.ToString() + " Hz";
            labelFormant2Hz.Text = TemporarySettings.VoiceInternalSettings.Formant2Frequency.ToString() + " Hz";
            labelFormant3Hz.Text = TemporarySettings.VoiceInternalSettings.Formant3Frequency.ToString() + " Hz";
            labelFormant4Hz.Text = TemporarySettings.VoiceInternalSettings.Formant4Frequency.ToString() + " Hz";
            labelFormant1Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant1Volume.ToString());
            labelFormant2Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant2Volume.ToString());
            labelFormant3Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant3Volume.ToString());
            labelFormant4Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.Formant4Volume.ToString());
        }
        private void buttonOpenVowel_Click(object sender, EventArgs e) // Open vowel preset (A)
        {
            SetPresets(92, 100, 90, 60, 3355, 2558, 1675, 1180);
        }

        private void buttonCloseFront_Click(object sender, EventArgs e) // Close front preset (I)
        {
            SetPresets(65, 94, 80, 70, 4001, 2989, 2537, 426);
        }

        private void buttonCloseBack_Click(object sender, EventArgs e) // Close back preset (U)
        {
            SetPresets(60, 90, 90, 70, 3742, 2429, 2429, 512);
        }

        private void buttonMidFront_Click(object sender, EventArgs e) // Mid front preset (E)
        {
            SetPresets(96, 100, 98, 65, 3829, 2623, 1934, 814);
        }

        private void buttonOpenBack_Click(object sender, EventArgs e) // Open back preset (O)
        {
            SetPresets(87, 88, 88, 60, 3355, 2258, 1158, 900);
        }

        private void VoiceInternalsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            main_Window.checkBox_use_voice_system.Checked = false;
        }

        private async void comboBoxPlayNoteOnLineOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPlayNoteOnLineOption.SelectedIndex != (TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions == TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnAllLines ? 0 : 1))
            {
                if (comboBoxPlayNoteOnLineOption.SelectedIndex == 0)
                {
                    TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions = TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnAllLines;
                    Logger.Log("Set to play voice on all lines", Logger.LogTypes.Info);
                }
                else
                {
                    await main_Window.StopAllVoices(); // Stop all voices when switching to this mode
                    TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions = TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnCheckedLines;
                    Logger.Log("Set to play voice on checked lines only", Logger.LogTypes.Info);
                }
            }
        }

        private void timerControlApplier_Tick(object sender, EventArgs e)
        {
            SoundRenderingEngine.VoiceSynthesisEngine.ApplyValues(); // Apply new values
        }
    }
}
