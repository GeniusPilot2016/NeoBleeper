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

using System.Runtime.InteropServices;

namespace BeepBridge
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            buttonEnableBeepSlider.Enabled = !IsBeepSliderAlreadyEnabled(); // Disable the button if the beep slider is already enabled as default.
        }

        private void buttonEnableBeepSlider_Click(object sender, EventArgs e)
        {
            // The magic button that fits the 8 years that I was trying playing tune through my old laptop's system speaker.
            try
            {
                progressBar1.Visible = true; // Show the progress bar to indicate that the process is underway.
                
                // Take restoration point before making any changes to the system. This allows users to easily revert any changes made by the Beep Bridge in case something goes wrong.
                TakeRestorationPoint();

                // Take backup of the current settings of the beeping channel of the sound chip. This is important to ensure that users can restore their original settings if they want to revert the changes made by the Beep Bridge.
                TakeBackupOfCurrentSettings();

                // Enable the beep slider in the mixer and unmute the beeping channel of the sound chip. This allows users to control the volume of the system speaker and use it for playing tunes through the Beep Bridge.
                ShowBeepSlider();

                labelStatus.Enabled = true;
                labelStatus.ForeColor = Color.Green; // Set the status label color to green to indicate success.
                labelStatus.Text = "Beep slider enabled successfully! The computer should be rebooted for the changes to take effect."; // Display a success message in the status label.

                // Ask the user to reboot the computer for the changes to take effect. This is important because some changes made by the Beep Bridge may require a reboot to be applied properly.
                AskAndReboot(); // Prompt the user to reboot the computer for the changes to take effect.
            }
            catch (Exception ex)
            {
                labelStatus.ForeColor = Color.Red; // Set the status label color to red to indicate an error.
                labelStatus.Text = $"An error occurred: {ex.Message}"; // Display the error message in the status label.
            }
            finally
            {
                progressBar1.Visible = false; // Hide the progress bar after the process is complete.
            }
        }

        private void buttonRevertChanges_Click(object sender, EventArgs e)
        {
            // Revert the changes to the system speaker settings. This is important in case the user wants to restore the original settings after using the Beep Bridge.
        }

        private void TakeRestorationPoint()
        {
            // This method takes restoration point that can be used in "System Restore" feature of Windows. This allows users to easily revert any changes made by the Beep Bridge in case something goes wrong.
            labelStatus.Enabled = true;
            labelStatus.Text = "Taking restoration point...";
        }

        private void TakeBackupOfCurrentSettings()
        {
            // This method takes backup of the current settings of the beeping channel of sound chip. This is important to ensure that users can restore their original settings if they want to revert the changes made by the Beep Bridge.
            labelStatus.Enabled = true;
            labelStatus.Text = "Taking backup of current settings...";
        }

        private void ShowBeepSlider()
        {
            // This method shows the beep slider in the mixer and unmutes the beeping channel of the sound chip.
            labelStatus.Enabled = true;
            labelStatus.Text = "Enabling beep slider...";
            // Code to enable the beep slider goes here.
        }

        // Windows Multimedia API Sabitleri
        private const uint MMSYSERR_NOERROR = 0;
        private const uint MIXER_GETLINEINFOF_DESTINATION = 0x00000000;
        private const uint MIXER_GETLINEINFOF_SOURCE = 0x00000001;
        private const uint MIXER_GETLINEINFOF_COMPONENTTYPE = 0x00000003;
        private const uint MIXERLINE_COMPONENTTYPE_SRC_PCSPKR = 0x00000007;

        // Windows Multimedia API Yapýlarý
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MIXERLINE
        {
            public uint cbStruct;
            public uint dwDestination;
            public uint dwSource;
            public uint dwLineID;
            public uint fdwLine;
            public UIntPtr dwUser;
            public uint dwComponentType;
            public uint cChannels;
            public uint cConnections;
            public uint cControls;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string szShortName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szName;
            public uint dwType;
            public uint dwDeviceID;
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
        }

        // Windows Multimedia API P/Invoke Tanýmlarý
        [DllImport("winmm.dll")]
        private static extern uint mixerGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern uint mixerOpen(out IntPtr phmx, uint uMxId, IntPtr dwCallback, IntPtr dwInstance, uint fdwOpen);

        [DllImport("winmm.dll")]
        private static extern uint mixerClose(IntPtr hmx);

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        private static extern uint mixerGetLineInfo(IntPtr hmx, ref MIXERLINE pmxl, uint fdwInfo);

        private bool IsBeepSliderAlreadyEnabled()
        {
            // Bu metod mikserdeki tüm hatlarý derinlemesine tarar ve isim bazlý eþleþme yapar.
            // SRC_PCSPKR kontrolü bazý sürücülerde güvenilmez olduðu için isim taramasý (Beep/Bip) eklenmiþtir.

            uint numDevs = mixerGetNumDevs();
            for (uint i = 0; i < numDevs; i++)
            {
                IntPtr hmx;
                if (mixerOpen(out hmx, i, IntPtr.Zero, IntPtr.Zero, 0) == MMSYSERR_NOERROR)
                {
                    // Her mikser cihazýndaki varýþ noktalarýný (playback, record vb.) tarýyoruz.
                    // GENEL NOT: Çoðu ses kartýnda Playback (Hoparlör) 0. varýþ noktasýdýr.
                    for (uint dest = 0; dest < 5; dest++)
                    {
                        MIXERLINE mxlDest = new MIXERLINE();
                        mxlDest.cbStruct = (uint)Marshal.SizeOf(mxlDest);
                        mxlDest.dwDestination = dest;

                        if (mixerGetLineInfo(hmx, ref mxlDest, MIXER_GETLINEINFOF_DESTINATION) != MMSYSERR_NOERROR)
                            break;

                        // Varýþ noktasýnýn ismini kontrol et (Örn: "PC Beep" ana çýkýþ olarak tanýmlanmýþsa)
                        if (IsBeepRelatedName(mxlDest.szName) || IsBeepRelatedName(mxlDest.szShortName))
                        {
                            mixerClose(hmx);
                            return true;
                        }

                        // Bu varýþ noktasýna baðlý tüm kaynak hatlarýný (Internal Beep, Line In, Mic vb.) tara
                        uint connections = mxlDest.cConnections;
                        for (uint src = 0; src < connections; src++)
                        {
                            MIXERLINE mxlSrc = new MIXERLINE();
                            mxlSrc.cbStruct = (uint)Marshal.SizeOf(mxlSrc);
                            mxlSrc.dwDestination = dest;
                            mxlSrc.dwSource = src;

                            if (mixerGetLineInfo(hmx, ref mxlSrc, MIXER_GETLINEINFOF_SOURCE) == MMSYSERR_NOERROR)
                            {
                                // Kaynak hattý ismi "Beep" veya "Bip" içeriyor mu? 
                                // Ya da standart SRC_PCSPKR tipinde mi?
                                if (IsBeepRelatedName(mxlSrc.szName) ||
                                    IsBeepRelatedName(mxlSrc.szShortName) ||
                                    mxlSrc.dwComponentType == MIXERLINE_COMPONENTTYPE_SRC_PCSPKR)
                                {
                                    mixerClose(hmx);
                                    return true;
                                }
                            }
                        }
                    }
                    mixerClose(hmx);
                }
            }
            return false;
        }

        private bool IsBeepRelatedName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string lower = name.ToLowerInvariant();
            // Names for system speaker-related lines are often named as "Beep", "PC Beep", "PC Speaker", "System Beep"
            return lower.Contains("beep") || lower.Contains("pc speaker") || lower.Contains("system beep") || lower.Contains("pc beep");
        }
        private bool IsBackupPresent()
        {
            // This method checks if the backup of the current settings is present. If it is not, then it means that the user has not taken a backup before enabling the beep slider, and they should be prompted to take a backup before proceeding.
            return false; // Placeholder return value
        }

        private void AskAndReboot()
        {
            // This method prompts the user to reboot the computer for the changes to take effect. This is important because some changes made by the Beep Bridge may require a reboot to be applied properly.
            DialogResult result = MessageBox.Show("The computer needs to be rebooted for the changes to take effect. Do you want to reboot now?", "Reboot Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Code to reboot the computer goes here.
            }
        }

        private bool IsBeepSliderEnabledAsDefault()
        {
            // This method checks if the beep slider is enabled as default in the system. If it is, then there is no need to enable it again, and the user can directly use the Beep Bridge without taking any backup or restoration point.

            // This is important to check because if the beep slider is already enabled as default, then there is no need to take a backup or restoration point, and the user can directly use the Beep Bridge without any additional steps.
            bool isBeepSliderAlreadyEnabled = IsBeepSliderAlreadyEnabled();
            // If backup is not present, it means the beep slider is enabled as default.
            bool isBackupPresent = !IsBackupPresent(); 
            return isBeepSliderAlreadyEnabled && isBackupPresent; // If the beep slider is already enabled and there is no backup, it means the beep slider is enabled as default.
        }
    }
}
