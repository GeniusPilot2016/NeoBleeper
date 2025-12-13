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
using static UIHelper;

namespace NeoBleeper
{
    public partial class VoiceInternalsWindow : Form
    {
        MainWindow mainWindow;
        bool darkTheme = false;
        public VoiceInternalsWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            UIFonts.SetFonts(this);
            SetTheme();
            trackBarFormant1Vol.Value = SetReverseTrackBarValue(trackBarFormant1Vol, TemporarySettings.VoiceInternalSettings.formant1Volume);
            trackBarFormant2Vol.Value = SetReverseTrackBarValue(trackBarFormant2Vol, TemporarySettings.VoiceInternalSettings.formant2Volume);
            trackBarFormant3Vol.Value = SetReverseTrackBarValue(trackBarFormant3Vol, TemporarySettings.VoiceInternalSettings.formant3Volume);
            trackBarFormant4Vol.Value = SetReverseTrackBarValue(trackBarFormant4Vol, TemporarySettings.VoiceInternalSettings.formant4Volume);
            trackBarFormant1Hz.Value = SetReverseTrackBarValue(trackBarFormant1Hz, TemporarySettings.VoiceInternalSettings.formant1Frequency);
            trackBarFormant2Hz.Value = SetReverseTrackBarValue(trackBarFormant2Hz, TemporarySettings.VoiceInternalSettings.formant2Frequency);
            trackBarFormant3Hz.Value = SetReverseTrackBarValue(trackBarFormant3Hz, TemporarySettings.VoiceInternalSettings.formant3Frequency);
            trackBarFormant4Hz.Value = SetReverseTrackBarValue(trackBarFormant4Hz, TemporarySettings.VoiceInternalSettings.formant4Frequency);
            trackBarVoiceVol.Value = SetReverseTrackBarValue(trackBarVoiceVol, TemporarySettings.VoiceInternalSettings.voiceVolume);
            trackBarSawVol.Value = SetReverseTrackBarValue(trackBarSawVol, TemporarySettings.VoiceInternalSettings.sawVolume);
            trackBarNoiseVol.Value = SetReverseTrackBarValue(trackBarNoiseVol, TemporarySettings.VoiceInternalSettings.noiseVolume);
            trackBarSybillance1Ra.Value = SetReverseTrackBarValue(trackBarSybillance1Ra, (int)(TemporarySettings.VoiceInternalSettings.sybillance1Range * 1000));
            trackBarSybillance2Ra.Value = SetReverseTrackBarValue(trackBarSybillance2Ra, (int)(TemporarySettings.VoiceInternalSettings.sybillance2Range * 1000));
            trackBarSybillance3Ra.Value = SetReverseTrackBarValue(trackBarSybillance3Ra, (int)(TemporarySettings.VoiceInternalSettings.sybillance3Range * 1000));
            trackBarSybillance4Ra.Value = SetReverseTrackBarValue(trackBarSybillance4Ra, (int)(TemporarySettings.VoiceInternalSettings.sybillance4Range * 1000));
            trackBarSybillance1Vol.Value = SetReverseTrackBarValue(trackBarSybillance1Vol, (int)(TemporarySettings.VoiceInternalSettings.sybillance1Volume * 1000));
            trackBarSybillance2Vol.Value = SetReverseTrackBarValue(trackBarSybillance2Vol, (int)(TemporarySettings.VoiceInternalSettings.sybillance2Volume * 1000));
            trackBarSybillance3Vol.Value = SetReverseTrackBarValue(trackBarSybillance3Vol, (int)(TemporarySettings.VoiceInternalSettings.sybillance3Volume * 1000));
            trackBarSybillance4Vol.Value = SetReverseTrackBarValue(trackBarSybillance4Vol, (int)(TemporarySettings.VoiceInternalSettings.sybillance4Volume * 1000));
            trackBarSybillance1Hz.Value = SetReverseTrackBarValue(trackBarSybillance1Hz, TemporarySettings.VoiceInternalSettings.sybillance1Frequency);
            trackBarSybillance2Hz.Value = SetReverseTrackBarValue(trackBarSybillance2Hz, TemporarySettings.VoiceInternalSettings.sybillance2Frequency);
            trackBarSybillance3Hz.Value = SetReverseTrackBarValue(trackBarSybillance3Hz, TemporarySettings.VoiceInternalSettings.sybillance3Frequency);
            trackBarSybillance4Hz.Value = SetReverseTrackBarValue(trackBarSybillance4Hz, TemporarySettings.VoiceInternalSettings.sybillance4Frequency);
            comboBoxNote1Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.note1OutputDeviceIndex;
            comboBoxNote2Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.note2OutputDeviceIndex;
            comboBoxNote3Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.note3OutputDeviceIndex;
            comboBoxNote4Option.SelectedIndex = TemporarySettings.VoiceInternalSettings.note4OutputDeviceIndex;
            labelFormant1Hz.Text = TemporarySettings.VoiceInternalSettings.formant1Frequency.ToString() + " Hz";
            labelFormant2Hz.Text = TemporarySettings.VoiceInternalSettings.formant2Frequency.ToString() + " Hz";
            labelFormant3Hz.Text = TemporarySettings.VoiceInternalSettings.formant3Frequency.ToString() + " Hz";
            labelFormant4Hz.Text = TemporarySettings.VoiceInternalSettings.formant4Frequency.ToString() + " Hz";
            labelFormant1Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant1Volume.ToString());
            labelFormant2Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant2Volume.ToString());
            labelFormant3Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant3Volume.ToString());
            labelFormant4Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant4Volume.ToString());
            labelVoiceVol.Text = TemporarySettings.VoiceInternalSettings.voiceVolume.ToString();
            labelSawVol.Text = TemporarySettings.VoiceInternalSettings.sawVolume.ToString();
            labelNoiseVol.Text = TemporarySettings.VoiceInternalSettings.noiseVolume.ToString();
            labelSybillance1Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance1Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance2Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance2Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance3Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance3Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance4Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance4Range.ToString("0.##", CultureInfo.InvariantCulture);
            labelSybillance1Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance1Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance2Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance2Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance3Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance3Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance4Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance4Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
            labelSybillance1Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance1Frequency.ToString() + " Hz";
            labelSybillance2Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance2Frequency.ToString() + " Hz";
            labelSybillance3Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance3Frequency.ToString() + " Hz";
            labelSybillance4Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance4Frequency.ToString() + " Hz";
            double minPitch = 0.5, maxPitch = 2.0;
            int pitchSpan = trackBarTimbre.Maximum - trackBarTimbre.Minimum;
            trackBarTimbre.Value = trackBarTimbre.Minimum + (int)Math.Round(((TemporarySettings.VoiceInternalSettings.timbre - minPitch) / (maxPitch - minPitch)) * (pitchSpan > 0 ? pitchSpan : 1));
            comboBoxPlayNoteOnLineOption.SelectedIndex = TemporarySettings.VoiceInternalSettings.playingVoiceOnLineOptions == TemporarySettings.VoiceInternalSettings.PlayingVoiceOnLineOptions.PlayVoiceOnCheckedLines ? 1 : 0;
            double minRange = 0, maxRange = 1.0;
            int rangeSpan = trackBarRandomizedFormantFreqRange.Maximum - trackBarRandomizedFormantFreqRange.Minimum;
            trackBarRandomizedFormantFreqRange.Value = trackBarRandomizedFormantFreqRange.Minimum + (int)Math.Round(((TemporarySettings.VoiceInternalSettings.randomizedFrequencyRange - minRange) / (maxRange - minRange)) * (rangeSpan > 0 ? rangeSpan : 1));
            trackBarCutoffHz.Value = SetReverseTrackBarValue(trackBarCutoffHz, TemporarySettings.VoiceInternalSettings.cutoffFrequency);
            labelCutoffHz.Text = TemporarySettings.VoiceInternalSettings.cutoffFrequency.ToString();
            BringToFront();
        }

        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    SetTheme();
                }
            }
        }

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method updates the control's appearance according to the selected theme
        /// preference. If the theme is set to follow the system, the method detects the system's light or dark mode and
        /// applies the corresponding theme. The method also enables double buffering to improve rendering performance
        /// and forces a UI update to ensure changes take effect immediately.</remarks>
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
                            DarkTheme();
                        }
                        else
                        {
                            LightTheme();
                        }
                        break;

                    case 1:
                        LightTheme();
                        break;

                    case 2:
                        DarkTheme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
        }
        private void DarkTheme()
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
        private void LightTheme()
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

        /// <summary>
        /// Calculates the value of a TrackBar control as if its direction were reversed.
        /// </summary>
        /// <remarks>Use this method when you need to interpret a TrackBar's value in reverse, such as for
        /// right-to-left or bottom-to-top scenarios, since the TrackBar control does not natively support reversed
        /// direction.</remarks>
        /// <param name="trackBar">The TrackBar control for which to compute the reversed value. Cannot be null.</param>
        /// <returns>An integer representing the TrackBar's value as if the minimum and maximum were swapped.</returns>
        private int GetTrackBarReverseValue(TrackBar trackBar) // Because TrackBar control does not support reversed direction natively
        {
            return trackBar.Maximum - trackBar.Value + trackBar.Minimum;
        }

        /// <summary>
        /// Calculates the value to set on a TrackBar control to achieve a reversed direction effect.
        /// </summary>
        /// <remarks>Use this method when you want the TrackBar to increase in value as the slider moves
        /// from right to left or top to bottom, instead of the default direction. This is useful because the TrackBar
        /// control does not natively support reversed orientation.</remarks>
        /// <param name="trackBar">The TrackBar control for which to calculate the reversed value. Cannot be null.</param>
        /// <param name="value">The intended logical value to represent on the TrackBar, within the control's minimum and maximum range.</param>
        /// <returns>The value to assign to the TrackBar's Value property so that the control appears reversed.</returns>
        private int SetReverseTrackBarValue(TrackBar trackBar, int value) // Because TrackBar control does not support reversed direction natively
        {
            return trackBar.Maximum - value + trackBar.Minimum;
        }

        private void trackBarFormant1Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant1Volume != GetTrackBarReverseValue(trackBarFormant1Vol))
            {
                TemporarySettings.VoiceInternalSettings.formant1Volume = GetTrackBarReverseValue(trackBarFormant1Vol);
                labelFormant1Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant1Volume.ToString());
                Logger.Log("Formant 1 Volume set to " + TemporarySettings.VoiceInternalSettings.formant1Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant1Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant1Frequency != GetTrackBarReverseValue(trackBarFormant1Hz))
            {
                TemporarySettings.VoiceInternalSettings.formant1Frequency = GetTrackBarReverseValue(trackBarFormant1Hz);
                labelFormant1Hz.Text = TemporarySettings.VoiceInternalSettings.formant1Frequency.ToString() + " Hz";
                Logger.Log("Formant 1 Frequency set to " + TemporarySettings.VoiceInternalSettings.formant1Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant2Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant2Volume != GetTrackBarReverseValue(trackBarFormant2Vol))
            {
                TemporarySettings.VoiceInternalSettings.formant2Volume = GetTrackBarReverseValue(trackBarFormant2Vol);
                labelFormant2Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant2Volume.ToString());
                Logger.Log("Formant 2 Volume set to " + TemporarySettings.VoiceInternalSettings.formant2Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant2Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant2Frequency != GetTrackBarReverseValue(trackBarFormant2Hz))
            {
                TemporarySettings.VoiceInternalSettings.formant2Frequency = GetTrackBarReverseValue(trackBarFormant2Hz);
                labelFormant2Hz.Text = TemporarySettings.VoiceInternalSettings.formant2Frequency.ToString() + " Hz";
                Logger.Log("Formant 2 Frequency set to " + TemporarySettings.VoiceInternalSettings.formant2Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant3Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant3Volume != GetTrackBarReverseValue(trackBarFormant3Vol))
            {
                TemporarySettings.VoiceInternalSettings.formant3Volume = GetTrackBarReverseValue(trackBarFormant3Vol);
                labelFormant3Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant3Volume.ToString());
                Logger.Log("Formant 3 Volume set to " + TemporarySettings.VoiceInternalSettings.formant3Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant3Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant3Frequency != GetTrackBarReverseValue(trackBarFormant3Hz))
            {
                TemporarySettings.VoiceInternalSettings.formant3Frequency = GetTrackBarReverseValue(trackBarFormant3Hz);
                labelFormant3Hz.Text = TemporarySettings.VoiceInternalSettings.formant3Frequency.ToString() + " Hz";
                Logger.Log("Formant 3 Frequency set to " + TemporarySettings.VoiceInternalSettings.formant3Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant4Vol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant4Volume != GetTrackBarReverseValue(trackBarFormant4Vol))
            {
                TemporarySettings.VoiceInternalSettings.formant4Volume = GetTrackBarReverseValue(trackBarFormant4Vol);
                labelFormant4Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant4Volume.ToString());
                Logger.Log("Formant 4 Volume set to " + TemporarySettings.VoiceInternalSettings.formant4Volume.ToString() + "%", Logger.LogTypes.Info);
            }
        }

        private void trackBarFormant4Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.formant4Frequency != GetTrackBarReverseValue(trackBarFormant4Hz))
            {
                TemporarySettings.VoiceInternalSettings.formant4Frequency = GetTrackBarReverseValue(trackBarFormant4Hz);
                labelFormant4Hz.Text = TemporarySettings.VoiceInternalSettings.formant4Frequency.ToString() + " Hz";
                Logger.Log("Formant 4 Frequency set to " + TemporarySettings.VoiceInternalSettings.formant4Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarVoiceVol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.voiceVolume != GetTrackBarReverseValue(trackBarVoiceVol))
            {
                TemporarySettings.VoiceInternalSettings.voiceVolume = GetTrackBarReverseValue(trackBarVoiceVol);
                labelVoiceVol.Text = TemporarySettings.VoiceInternalSettings.voiceVolume.ToString();
                Logger.Log("Voice Volume set to " + TemporarySettings.VoiceInternalSettings.voiceVolume.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarSawVol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.sawVolume != GetTrackBarReverseValue(trackBarSawVol))
            {
                TemporarySettings.VoiceInternalSettings.sawVolume = GetTrackBarReverseValue(trackBarSawVol);
                labelSawVol.Text = TemporarySettings.VoiceInternalSettings.sawVolume.ToString();
                Logger.Log("Saw Volume set to " + TemporarySettings.VoiceInternalSettings.sawVolume.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarNoiseVol_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.noiseVolume != GetTrackBarReverseValue(trackBarNoiseVol))
            {
                TemporarySettings.VoiceInternalSettings.noiseVolume = GetTrackBarReverseValue(trackBarNoiseVol);
                labelNoiseVol.Text = TemporarySettings.VoiceInternalSettings.noiseVolume.ToString();
                Logger.Log("Noise Volume set to " + TemporarySettings.VoiceInternalSettings.noiseVolume.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance1Ra_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance1Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance1Range != value)
            {
                {
                    TemporarySettings.VoiceInternalSettings.sybillance1Range = value;
                    labelSybillance1Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance1Range.ToString("0.##", CultureInfo.InvariantCulture);
                    Logger.Log("Sybillance 1 Range set to " + TemporarySettings.VoiceInternalSettings.sybillance1Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
                }
            }
        }

        private void trackBarSybillance1Vol_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance1Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance1Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance1Volume = value;
                labelSybillance1Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance1Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 1 Volume set to " + TemporarySettings.VoiceInternalSettings.sybillance1Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance1Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.sybillance1Frequency != GetTrackBarReverseValue(trackBarSybillance1Hz))
            {
                TemporarySettings.VoiceInternalSettings.sybillance1Frequency = GetTrackBarReverseValue(trackBarSybillance1Hz);
                labelSybillance1Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance1Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 1 Frequency set to " + TemporarySettings.VoiceInternalSettings.sybillance1Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance2Ra_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance2Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance2Range != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance2Range = value;
                labelSybillance2Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance2Range.ToString("0.##", CultureInfo.InvariantCulture);
                Logger.Log("Sybillance 2 Range set to " + TemporarySettings.VoiceInternalSettings.sybillance2Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance2Vol_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance2Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance2Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance2Volume = value;
                labelSybillance2Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance2Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 2 Volume set to " + TemporarySettings.VoiceInternalSettings.sybillance2Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance2Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.sybillance2Frequency != GetTrackBarReverseValue(trackBarSybillance2Hz))
            {
                TemporarySettings.VoiceInternalSettings.sybillance2Frequency = GetTrackBarReverseValue(trackBarSybillance2Hz);
                labelSybillance2Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance2Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 2 Frequency set to " + TemporarySettings.VoiceInternalSettings.sybillance2Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance3Ra_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance3Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance3Range != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance3Range = value;
                labelSybillance3Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance3Range.ToString("0.##", CultureInfo.InvariantCulture);
                Logger.Log("Sybillance 3 Range set to " + TemporarySettings.VoiceInternalSettings.sybillance3Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance3Vol_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance3Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance3Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance3Volume = value;
                labelSybillance3Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance3Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 3 Volume set to " + TemporarySettings.VoiceInternalSettings.sybillance3Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance3Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.sybillance3Frequency != GetTrackBarReverseValue(trackBarSybillance3Hz))
            {
                TemporarySettings.VoiceInternalSettings.sybillance3Frequency = GetTrackBarReverseValue(trackBarSybillance3Hz);
                labelSybillance3Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance3Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 3 Frequency set to " + TemporarySettings.VoiceInternalSettings.sybillance3Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance4Ra_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance4Ra) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance4Range != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance4Range = value;
                labelSybillance4Ra.Text = TemporarySettings.VoiceInternalSettings.sybillance4Range.ToString("0.##", CultureInfo.InvariantCulture);
                Logger.Log("Sybillance 4 Range set to " + TemporarySettings.VoiceInternalSettings.sybillance4Range.ToString("0.##", CultureInfo.InvariantCulture), Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance4Vol_Scroll(object sender, EventArgs e)
        {
            double value = GetTrackBarReverseValue(trackBarSybillance4Vol) / 1000.0;
            if (TemporarySettings.VoiceInternalSettings.sybillance4Volume != value)
            {
                TemporarySettings.VoiceInternalSettings.sybillance4Volume = value;
                labelSybillance4Vol.Text = TemporarySettings.VoiceInternalSettings.sybillance4Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x";
                Logger.Log("Sybillance 4 Volume set to " + TemporarySettings.VoiceInternalSettings.sybillance4Volume.ToString("0.##", CultureInfo.InvariantCulture) + "x", Logger.LogTypes.Info);
            }
        }

        private void trackBarSybillance4Hz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.sybillance4Frequency != GetTrackBarReverseValue(trackBarSybillance4Hz))
            {
                TemporarySettings.VoiceInternalSettings.sybillance4Frequency = GetTrackBarReverseValue(trackBarSybillance4Hz);
                labelSybillance4Hz.Text = TemporarySettings.VoiceInternalSettings.sybillance4Frequency.ToString() + " Hz";
                Logger.Log("Sybillance 4 Frequency set to " + TemporarySettings.VoiceInternalSettings.sybillance4Frequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        private void trackBarPitch_Scroll(object sender, EventArgs e)
        {
            double minPitch = 0.5;
            double maxPitch = 2.0;
            int span = trackBarTimbre.Maximum - trackBarTimbre.Minimum;
            double normalized = span > 0 ? (trackBarTimbre.Value - trackBarTimbre.Minimum) / (double)span : 0.0;
            double value = minPitch + normalized * (maxPitch - minPitch);
            if (Math.Abs(TemporarySettings.VoiceInternalSettings.timbre - value) > 1e-9)
            {
                TemporarySettings.VoiceInternalSettings.timbre = value;
                Logger.Log("Timbre set to " + TemporarySettings.VoiceInternalSettings.timbre.ToString(), Logger.LogTypes.Info);
            }
        }

        private void trackBarRange_Scroll(object sender, EventArgs e)
        {
            double minRange = 0, maxRange = 1.0;
            int span = trackBarRandomizedFormantFreqRange.Maximum - trackBarRandomizedFormantFreqRange.Minimum;
            double normalized = span > 0 ? (trackBarRandomizedFormantFreqRange.Value - trackBarRandomizedFormantFreqRange.Minimum) / (double)span : 0.0;
            double value = minRange + normalized * (maxRange - minRange);
            if (Math.Abs(TemporarySettings.VoiceInternalSettings.randomizedFrequencyRange - value) > 1e-9)
            {
                TemporarySettings.VoiceInternalSettings.randomizedFrequencyRange = value;
                Logger.Log("Randomized frequency range set to " + TemporarySettings.VoiceInternalSettings.randomizedFrequencyRange.ToString(), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote1Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote1Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.note1OutputDeviceIndex)
            {
                if (comboBoxNote1Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(0);
                }
                TemporarySettings.VoiceInternalSettings.note1OutputDeviceIndex = comboBoxNote1Option.SelectedIndex;
                Logger.Log("Note 1 Output Device set to index " + (TemporarySettings.VoiceInternalSettings.note1OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote2Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote2Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.note2OutputDeviceIndex)
            {
                if (comboBoxNote2Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(1);
                }
                TemporarySettings.VoiceInternalSettings.note2OutputDeviceIndex = comboBoxNote2Option.SelectedIndex;
                Logger.Log("Note 2 Output Device set to " + (TemporarySettings.VoiceInternalSettings.note2OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote3Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote3Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.note3OutputDeviceIndex)
            {
                if (comboBoxNote3Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(2);
                }
                TemporarySettings.VoiceInternalSettings.note3OutputDeviceIndex = comboBoxNote3Option.SelectedIndex;
                Logger.Log("Note 3 Output Device set to " + (TemporarySettings.VoiceInternalSettings.note3OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void comboBoxNote4Option_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNote4Option.SelectedIndex != TemporarySettings.VoiceInternalSettings.note4OutputDeviceIndex)
            {
                if (comboBoxNote4Option.SelectedIndex == 1)
                {
                    SoundRenderingEngine.VoiceSynthesisEngine.StopVoice(3);
                }
                TemporarySettings.VoiceInternalSettings.note4OutputDeviceIndex = comboBoxNote4Option.SelectedIndex;
                Logger.Log("Note 4 Output Device set to " + (TemporarySettings.VoiceInternalSettings.note4OutputDeviceIndex == 0 ? "Voice system" : "System speaker/Sound device beep"), Logger.LogTypes.Info);
            }
        }

        private void trackBarCutoffHz_Scroll(object sender, EventArgs e)
        {
            if (TemporarySettings.VoiceInternalSettings.cutoffFrequency != GetTrackBarReverseValue(trackBarCutoffHz))
            {
                TemporarySettings.VoiceInternalSettings.cutoffFrequency = GetTrackBarReverseValue(trackBarCutoffHz);
                labelCutoffHz.Text = TemporarySettings.VoiceInternalSettings.cutoffFrequency.ToString();
                Logger.Log("Cutoff Frequency set to " + TemporarySettings.VoiceInternalSettings.cutoffFrequency.ToString() + " Hz", Logger.LogTypes.Info);
            }
        }

        /// <summary>
        /// Sets the formant volume and frequency presets for all four formants, updating the corresponding UI controls
        /// and internal settings.
        /// </summary>
        /// <remarks>This method updates both the UI elements (such as track bars and labels) and the
        /// internal voice settings to reflect the specified presets. Values outside the valid range of the controls may
        /// result in unexpected behavior.</remarks>
        /// <param name="formantVol1">The volume preset for formant 1. Typically specified as a percentage value within the valid range of the
        /// associated control.</param>
        /// <param name="formantVol2">The volume preset for formant 2. Typically specified as a percentage value within the valid range of the
        /// associated control.</param>
        /// <param name="formantVol3">The volume preset for formant 3. Typically specified as a percentage value within the valid range of the
        /// associated control.</param>
        /// <param name="formantVol4">The volume preset for formant 4. Typically specified as a percentage value within the valid range of the
        /// associated control.</param>
        /// <param name="formant1Freq">The frequency preset for formant 1, in hertz. Must be within the valid range supported by the associated
        /// control.</param>
        /// <param name="formant2Freq">The frequency preset for formant 2, in hertz. Must be within the valid range supported by the associated
        /// control.</param>
        /// <param name="formant3Freq">The frequency preset for formant 3, in hertz. Must be within the valid range supported by the associated
        /// control.</param>
        /// <param name="formant4Freq">The frequency preset for formant 4, in hertz. Must be within the valid range supported by the associated
        /// control.</param>
        private void SetPresets(int formantVol1, int formantVol2, int formantVol3, int formantVol4,
            int formant1Freq, int formant2Freq, int formant3Freq, int formant4Freq)
        {
            trackBarFormant1Vol.Value = SetReverseTrackBarValue(trackBarFormant1Vol, formantVol1);
            trackBarFormant2Vol.Value = SetReverseTrackBarValue(trackBarFormant2Vol, formantVol2);
            trackBarFormant3Vol.Value = SetReverseTrackBarValue(trackBarFormant3Vol, formantVol3);
            trackBarFormant4Vol.Value = SetReverseTrackBarValue(trackBarFormant4Vol, formantVol4);
            trackBarFormant1Hz.Value = SetReverseTrackBarValue(trackBarFormant1Hz, formant1Freq);
            trackBarFormant2Hz.Value = SetReverseTrackBarValue(trackBarFormant2Hz, formant2Freq);
            trackBarFormant3Hz.Value = SetReverseTrackBarValue(trackBarFormant3Hz, formant3Freq);
            trackBarFormant4Hz.Value = SetReverseTrackBarValue(trackBarFormant4Hz, formant4Freq);
            TemporarySettings.VoiceInternalSettings.formant1Volume = GetTrackBarReverseValue(trackBarFormant1Vol);
            TemporarySettings.VoiceInternalSettings.formant2Volume = GetTrackBarReverseValue(trackBarFormant2Vol);
            TemporarySettings.VoiceInternalSettings.formant3Volume = GetTrackBarReverseValue(trackBarFormant3Vol);
            TemporarySettings.VoiceInternalSettings.formant4Volume = GetTrackBarReverseValue(trackBarFormant4Vol);
            TemporarySettings.VoiceInternalSettings.formant1Frequency = GetTrackBarReverseValue(trackBarFormant1Hz);
            TemporarySettings.VoiceInternalSettings.formant2Frequency = GetTrackBarReverseValue(trackBarFormant2Hz);
            TemporarySettings.VoiceInternalSettings.formant3Frequency = GetTrackBarReverseValue(trackBarFormant3Hz);
            TemporarySettings.VoiceInternalSettings.formant4Frequency = GetTrackBarReverseValue(trackBarFormant4Hz);
            labelFormant1Hz.Text = TemporarySettings.VoiceInternalSettings.formant1Frequency.ToString() + " Hz";
            labelFormant2Hz.Text = TemporarySettings.VoiceInternalSettings.formant2Frequency.ToString() + " Hz";
            labelFormant3Hz.Text = TemporarySettings.VoiceInternalSettings.formant3Frequency.ToString() + " Hz";
            labelFormant4Hz.Text = TemporarySettings.VoiceInternalSettings.formant4Frequency.ToString() + " Hz";
            labelFormant1Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant1Volume.ToString());
            labelFormant2Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant2Volume.ToString());
            labelFormant3Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant3Volume.ToString());
            labelFormant4Vol.Text = Resources.TextPercent.Replace("{number}", TemporarySettings.VoiceInternalSettings.formant4Volume.ToString());
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
            mainWindow.checkBox_use_voice_system.Checked = false;
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
                    await mainWindow.StopAllVoices(); // Stop all voices when switching to this mode
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
