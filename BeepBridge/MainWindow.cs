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

using NAudio.Mixer;
using System;
using System.Diagnostics;
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

        private bool IsBeepSliderAlreadyEnabled()
        {
            try
            {
                try
                {
                    return TopologyHelper.TopologyContainsBeep();
                }
                catch (Exception exTopo)
                {
                    Debug.WriteLine($"TopologyHelper failed: {exTopo.Message}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"IsBeepSliderAlreadyEnabled unexpected error: {ex.Message}");
                return false;
            }
        }

        private bool IsBeepRelatedName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            string lower = name.ToLowerInvariant();
            // Names for system speaker-related lines are often named as "Beep", "PC Beep", "PC Speaker", "System Beep"
            return lower.Contains("beep") ||
                   lower.Contains("pc speaker") ||
                   lower.Contains("system beep") ||
                   lower.Contains("pc beep");
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