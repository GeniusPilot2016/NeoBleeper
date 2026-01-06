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

using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NeoBleeper.Properties;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static NeoBleeper.TemporarySettings;

namespace NeoBleeper
{
    public class SoundRenderingEngine
    {
        internal static readonly object AudioLock = new object();
        public static class SystemSpeakerBeepEngine // Drive the system speaker (aka PC speaker) directly by emulating beep.sys using inpoutx64.dll in modern Windows (Windows 7 and above)
                                                    // Note: This will not work in virtual machines or computers without a physical system speaker output
        {
            // Robbi-985 (aka SomethingUnreal) abandoned the Bleeper Music Maker in 2011 due to changes in beep.sys in Windows 7 and later.
            // We're fighting to bring it back to life in 2025... with direct hardware access like his BaWaMI (Basic Waveform MIDI Software Synthesizer) did. :D
            // Pro tip: When you create something cool, don't abandon it. Keep it alive and updated. :)

            static systemStorageType StorageType = systemStorageType.HDD; // Default to HDD to prevent resonance issues, should be set by the main program based on actual storage device
            static int storageRPM = 5400; // Default RPM for HDD, should be set by the main program based on actual storage device
            static int resonanceFrequency = 50; // Default resonance frequency to avoid, should be set by the main program based on actual storage device

            /// <summary>
            /// Detects the system's primary storage type and configures related settings to prevent resonance issues
            /// during operation.
            /// </summary>
            /// <remarks>This method examines the connected storage devices to determine whether the
            /// system uses NVMe, SSD, HDD, or another storage type. Based on the detected type, it sets internal
            /// parameters such as resonance frequency and storage type, which may affect hardware-related features. On
            /// ARM64 architectures, storage type detection is skipped because system speaker access is not supported.
            /// If detection fails or the storage type is unknown, the method applies conservative settings to minimize
            /// potential resonance issues.</remarks>
            public static void SpecifyStorageType()
            {
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    try
                    {
                        Logger.Log("Specifying storage type for resonance prevention...", Logger.LogTypes.Info);
                        Program.splashScreen.UpdateStatus(Resources.StatusSpecifyingStorageType);
                        string query = "SELECT * FROM Win32_DiskDrive";
                        using (var searcher = new ManagementObjectSearcher(query))
                        {
                            var devices = searcher.Get();
                            foreach (ManagementObject device in devices)
                            {
                                string model = device["Model"]?.ToString() ?? "";
                                string interfaceType = device["InterfaceType"]?.ToString() ?? "";
                                if (interfaceType.Equals("NVMe", StringComparison.OrdinalIgnoreCase) || model.Contains("NVMe", StringComparison.OrdinalIgnoreCase))
                                {
                                    StorageType = systemStorageType.NVMe;
                                    resonanceFrequency = 0; // NVMe drives have no resonance issues
                                    Logger.Log("Detected NVMe storage. Resonance prevention is not necessary.", Logger.LogTypes.Info);
                                    Program.splashScreen.UpdateStatus(Resources.StatusNVMeStorageType, 5);
                                    return;
                                }
                                else if (interfaceType.Equals("SCSI", StringComparison.OrdinalIgnoreCase) || interfaceType.Equals("SATA", StringComparison.OrdinalIgnoreCase) || model.Contains("SSD", StringComparison.OrdinalIgnoreCase))
                                {
                                    StorageType = systemStorageType.SSD;
                                    resonanceFrequency = 0; // SSDs have no resonance issues
                                    Logger.Log("Detected SSD storage. Resonance prevention is not necessary.", Logger.LogTypes.Info);
                                    Program.splashScreen.UpdateStatus(Resources.StatusSSDStorageType, 5);
                                    return;
                                }
                                else if (interfaceType.Equals("IDE", StringComparison.OrdinalIgnoreCase) || interfaceType.Equals("ATA", StringComparison.OrdinalIgnoreCase) || model.Contains("HDD", StringComparison.OrdinalIgnoreCase) || model.Contains("Hard Drive", StringComparison.OrdinalIgnoreCase))
                                {
                                    StorageType = systemStorageType.HDD;
                                    string rpmStr = device["TotalCylinders"]?.ToString() ?? ""; // Placeholder, as RPM is not directly available
                                                                                                // In real scenarios, RPM might be fetched from specific vendor tools or databases
                                                                                                // Default to 5400 RPM for simplicity
                                    storageRPM = 5400;
                                    resonanceFrequency = storageRPM / 120; // Approximate resonance frequency in Hz
                                    string localizedResonanceMessage = Resources.StatusHDDStorageType.Replace("{value}", resonanceFrequency.ToString());
                                    Logger.Log($"Detected HDD storage with approximate RPM of {storageRPM}. Avoiding resonance frequency of {resonanceFrequency} Hz.", Logger.LogTypes.Info);
                                    Program.splashScreen.UpdateStatus(localizedResonanceMessage, 5);
                                    return;
                                }
                            }
                            StorageType = systemStorageType.Other; // If no known type is found
                            resonanceFrequency = 0; // Assume no resonance issues for unknown types
                            Logger.Log("Storage type is unknown. Resonance prevention is applied in probable resonant frequencies to be safe.", Logger.LogTypes.Warning);
                            Program.splashScreen.UpdateStatus(Resources.StatusUnknownStorageType, 5);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error specifying storage type: " + ex.Message, Logger.LogTypes.Error);
                        Program.splashScreen.UpdateStatus("Error specifying storage type. Falling back to HDD settings.");
                        StorageType = systemStorageType.HDD; // Fallback to HDD on error
                    }
                }
                else
                {
                    Logger.Log("Storage type specification skipped on ARM64 architecture due to ARM64 doesn't support system speaker access.", Logger.LogTypes.Info);
                }
            }

            /// <summary>
            /// Specifies the types of storage devices recognized by the system.
            /// </summary>
            /// <remarks>Use this enumeration to identify the storage device type when handling
            /// operations that may be affected by device characteristics, such as susceptibility to resonance or
            /// performance differences. Certain storage types, such as HDDs, may be more vulnerable to physical
            /// resonance effects compared to SSDs or NVMe devices.</remarks>
            public enum systemStorageType // Enum for different types of storage devices to prevent critical crashes by preventing resonance frequencies on certain devices because the system speaker doesn't have resonance prevention unlike regular sound devices and it's usually inside of the computer case
                                          // Fun fact: Janet Jackson's "Rhythm Nation" has a bass frequency of 50 Hz, which can cause resonance in HDDs and lead to crashes
            {
                HDD,
                SSD,
                NVMe,
                Other
            }
            static SystemSpeakerBeepEngine()
            {
                // Safe stop to avoid stuck beeps on exit or crash

                void SafeStop()
                {
                    try
                    {
                        StopBeep();
                    }
                    catch
                    {
                        // Ignore exceptions to avoid crashing the application
                    }
                }

                // Regular exit situations
                AppDomain.CurrentDomain.ProcessExit += (s, e) =>
                {
                    SafeStop();
                };
                System.Windows.Forms.Application.ApplicationExit += (s, e) => SafeStop();
                Console.CancelKeyPress += (s, e) => { SafeStop(); /* Key presses such as Ctrl+C will terminate the process, so we just stop the beep here */ };

                // Unhandled exceptions (dispose as possible)
                // The BSoD or power outage scenarios, which are full system crashes, cannot be handled here.
                AppDomain.CurrentDomain.UnhandledException += (s, e) => SafeStop();
                TaskScheduler.UnobservedTaskException += (s, e) => { SafeStop(); e.SetObserved(); };
            }

            [DllImport("inpoutx64.dll")]
            extern static void Out32(short PortAddress, short Data);
            [DllImport("inpoutx64.dll")]
            extern static char Inp32(short PortAddress);

            /// <summary>
            /// Generates a beep sound using the system speaker at the specified frequency and duration.
            /// </summary>
            /// <remarks>On devices with ARM64 architecture, the system speaker is not supported and
            /// no sound will be produced, but the method will still wait for the specified duration. On other
            /// platforms, certain frequencies may be shifted to prevent potential hardware issues with specific storage
            /// devices.</remarks>
            /// <param name="freq">The frequency of the beep, in hertz. Must be a positive integer. Certain frequencies may be adjusted
            /// internally to avoid hardware resonance issues.</param>
            /// <param name="ms">The duration of the beep, in milliseconds. Must be a non-negative integer.</param>
            /// <param name="nonStopping">If set to <see langword="true"/>, the beep will continue after the specified duration until stopped by
            /// other means; otherwise, the beep stops automatically after the duration elapses.</param>
            public static void Beep(int freq, int ms, bool nonStopping) // Beep from the system speaker (aka PC speaker)
            {
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    // This program contains 100% recycled beeps from the golden age of the PC audio.
                    int[] probableResonantFrequencies = new int[] { 45, 50, 60, 100, 120 }; // Common resonant frequencies to avoid if StorageType is Other
                    if ((freq == resonanceFrequency && StorageType == systemStorageType.HDD) ||
                        (StorageType == systemStorageType.Other && probableResonantFrequencies.Contains(freq))) // Prevent resonance frequencies on HDDs to avoid critical crashes because the system speaker doesn't have resonance prevention unlike regular sound devices and it's usually inside of the computer case
                                                                                                                // Also, if the storage type is unknown, avoid common resonant frequencies to be safe
                    {
                        freq += 1; // Shift frequency by 1 Hz to avoid resonance
                    }
                    Out32(0x43, 0xB6); // Set the PIT to mode 3 (square wave generator) on channel 2 (the one connected to the system speaker)
                    int div = 0x1234dc / freq; // Calculate the divisor for the desired frequency (0x1234dc is the PIT input clock frequency of 1.193182 MHz)
                    Out32(0x42, (Byte)(div & 0xFF)); // Set the low byte of the divisor
                    Out32(0x42, (Byte)(div >> 8)); // Set the high byte of the divisor
                    if (!nonStopping)
                    {
                        HighPrecisionSleep.Sleep(5); // Small delay if not nonStopping to ensure the PIT is set up before enabling the speaker
                    }
                    Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03)); // Open the gate of the system speaker to start the beep
                    HighPrecisionSleep.Sleep(ms); // Wait for the specified duration
                    if (!nonStopping) // If nonStopping is true, the beep will not stop
                    {
                        StopBeep();
                    }
                }
                else
                {
                    HighPrecisionSleep.Sleep(ms); // On ARM64 devices such as most of Copilot+ devices, just sleep for the duration as system speaker access is not supported

                    // Sorry, your Copilot+ PC (most of Copilot+ PCs) with NPU can't "talk in beep language" :(
                    // But at least it can run NeoBleeper without crashing, right? :)
                }
            }

            /// <summary>
            /// Stops the system speaker (PC speaker) from producing a beep sound, if supported by the current platform.
            /// </summary>
            /// <remarks>On platforms where the system speaker is not present or not supported (such
            /// as most ARM64-based devices), this method performs no operation. This method has no effect if the system
            /// speaker is already silent.</remarks>
            public static void StopBeep() // Stop the system speaker (aka PC speaker) from beeping
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    // Philosophical problem: How do you stop a beep that doesn't exist in most of Copilot+ devices?
                    return; // No operation on ARM64 devices such as most of Copilot+ devices, as system speaker access is not supported
                }
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
            }

            /// <summary>
            /// Stops the system speaker beep if it is currently active or stuck.
            /// </summary>
            /// <remarks>This method checks whether the system speaker beep is in a stuck state and
            /// attempts to stop it if necessary. Any exceptions that occur during this process are suppressed. This
            /// method is typically used to ensure that unwanted or continuous beeping is silenced in scenarios where
            /// the system speaker may not stop beeping automatically.</remarks>
            public static void StopBeepIfNeeded()
            {
                try
                {
                    if (IsSystemSpeakerBeepStuck())
                    {
                        StopBeep();
                    }
                }
                catch
                {
                    return;
                }
            }

            /// <summary>
            /// Determines whether the system speaker is currently emitting a continuous beep, indicating it may be
            /// stuck in the 'on' state.
            /// </summary>
            /// <remarks>On ARM64 devices, such as most Copilot+ devices, system speaker access is not
            /// supported and this method always returns false. If an error occurs while checking the speaker status,
            /// the method also returns false.</remarks>
            /// <returns>true if the system speaker is detected to be continuously beeping; otherwise, false.</returns>
            public static bool IsSystemSpeakerBeepStuck()
            {
                try
                {
                    if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                    {
                        // Can the non-existent beep of most of Copilot+ devices be stuck?
                        return false; // ARM64 devices such as most of Copilot+ devices do not support system speaker access
                    }
                    // Check if the system speaker is currently beeping by reading the status of the speaker port
                    return ((Inp32(0x61) & 0x03) == 0x03);
                }
                catch (Exception)
                {
                    return false; // If an error occurs, assume the speaker is not stuck
                }
            }
            private const int ULTRASONIC_FREQ = 30000; // 30 kHz - Inaudible frequency to users but can still cause electrical feedback if the speaker is functional
            private const int PIT_BASE_FREQ = 1193180;

            /// <summary>
            /// Enables the PC speaker to emit an ultrasonic frequency by configuring the programmable interval timer
            /// (PIT) and updating the speaker control port.
            /// </summary>
            /// <remarks>This method is intended for low-level hardware control and should be used
            /// only in environments where direct access to hardware ports is permitted. The method temporarily modifies
            /// the speaker control port to enable ultrasonic output and may not be supported on all hardware or
            /// operating systems.</remarks>
            /// <param name="originalState">The original value of the speaker control port (I/O port 0x61) to preserve and restore non-related bits
            /// during speaker activation.</param>
            private static void UltraSoftEnableSpeaker(byte originalState)
            {
                // Configure PIT channel 2 for the ultrasonic frequency
                Out32(0x43, 0xB6);
                int div = PIT_BASE_FREQ / ULTRASONIC_FREQ;
                Out32(0x42, (byte)(div & 0xFF));
                Out32(0x42, (byte)(div >> 8));

                Program.splashScreen.ResponsiveWait(10); // To stabilize the timer

                // Open only the gate (bit 0), keep speaker data (bit 1) off
                Out32(0x61, (byte)(originalState | 0x01)); // Bit 1 = 0
                Program.splashScreen.ResponsiveWait(10);

                // Open speaker data (bit 1) now
                Out32(0x61, (byte)(originalState | 0x03)); // Bit 0 ve 1
            }

            /// <summary>
            /// Disables the system speaker by restoring the specified original port state.
            /// </summary>
            /// <remarks>This method is intended for low-level hardware control and should only be
            /// used in trusted contexts where direct port access is permitted. Incorrect usage may affect system audio
            /// behavior.</remarks>
            /// <param name="originalState">The original value of the port state to restore after disabling the speaker. Typically obtained prior to
            /// any modifications.</param>
            private static void UltraSoftDisableSpeaker(byte originalState)
            {
                // Close speaker data (bit 1) first
                Out32(0x61, (byte)((originalState & 0xFE) | 0x01));
                Program.splashScreen.ResponsiveWait(10);

                // Then close the gate (bit 0)
                Out32(0x61, (byte)(originalState & 0xFC));
                Program.splashScreen.ResponsiveWait(10);
            }

            /// <summary>
            /// Checks whether electrical feedback is present and responsive on the designated hardware port.
            /// </summary>
            /// <remarks>This method performs a series of hardware port reads and writes to determine
            /// if the electrical feedback mechanism is functioning correctly. It is intended for use in environments
            /// where direct hardware access is available. If an error occurs during the check, the method returns
            /// false.</remarks>
            /// <returns>true if electrical feedback is detected and the port responds as expected; otherwise, false.</returns>
            public static bool CheckElectricalFeedbackOnPort()
            {
                try
                {
                    byte originalState = (byte)Inp32(0x61);

                    // Ultra soft start
                    UltraSoftEnableSpeaker(originalState);
                    Program.splashScreen.ResponsiveWait(50);

                    List<byte> enabledSamples = new List<byte>();
                    for (int i = 0; i < 20; i++)
                    {
                        enabledSamples.Add((byte)Inp32(0x61));
                        Program.splashScreen.ResponsiveWait(1);
                    }
                    byte stateEnabled = enabledSamples[enabledSamples.Count / 2];

                    // Ultra soft close
                    UltraSoftDisableSpeaker(originalState);
                    Program.splashScreen.ResponsiveWait(50);

                    List<byte> disabledSamples = new List<byte>();
                    for (int i = 0; i < 20; i++)
                    {
                        disabledSamples.Add((byte)Inp32(0x61));
                        Program.splashScreen.ResponsiveWait(1);
                    }
                    byte stateDisabled = disabledSamples[disabledSamples.Count / 2];

                    // Restore original state
                    Out32(0x61, originalState);
                    Program.splashScreen.ResponsiveWait(20);

                    bool bit5VariesWhenEnabled = enabledSamples.Select(s => (byte)(s & 0x20)).Distinct().Count() > 1;
                    bool feedbackPresent = ((stateEnabled & 0x20) != (stateDisabled & 0x20)) || bit5VariesWhenEnabled;
                    bool gateResponsive = ((stateEnabled & 0x03) == 0x03) && ((stateDisabled & 0x03) == 0x00);

                    return feedbackPresent && gateResponsive;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            /// <summary>
            /// Checks whether the state of the hardware port at address 0x61 is stable and responsive by sampling its
            /// value and analyzing bit transitions.
            /// </summary>
            /// <remarks>This method is typically used to verify that the hardware port is functioning
            /// correctly before performing further operations that depend on its responsiveness. If an error occurs
            /// during the check, the method returns false.</remarks>
            /// <returns>true if the port's bit 5 shows sufficient variation and transitions during sampling, indicating a stable
            /// and responsive state; otherwise, false.</returns>
            public static bool CheckPortStateStability()
            {
                try
                {
                    byte originalState = (byte)Inp32(0x61);

                    // Ultra soft start
                    UltraSoftEnableSpeaker(originalState);
                    Program.splashScreen.ResponsiveWait(50);

                    List<byte> samples = new List<byte>();
                    for (int i = 0; i < 100; i++)
                    {
                        samples.Add((byte)Inp32(0x61));
                    }

                    // Ultra soft close
                    UltraSoftDisableSpeaker(originalState);
                    Out32(0x61, originalState);
                    Program.splashScreen.ResponsiveWait(20);

                    var bit5Values = samples.Select(s => (byte)(s & 0x20)).Distinct().ToList();
                    bool bit5Varies = bit5Values.Count > 1;

                    int bit5Transitions = 0;
                    for (int i = 0; i < samples.Count - 1; i++)
                    {
                        if ((samples[i] & 0x20) != (samples[i + 1] & 0x20))
                        {
                            bit5Transitions++;
                        }
                    }

                    bool sufficientTransitions = bit5Transitions >= 3;

                    return bit5Varies && sufficientTransitions;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            /// <summary>
            /// Performs a diagnostic test to verify whether high-frequency signals can be generated and detected using
            /// the system's programmable interval timer and speaker circuitry.
            /// </summary>
            /// <remarks>This test is intended to run silently by using only high frequencies that are
            /// typically inaudible to users. It is useful for determining whether the system's hardware supports
            /// frequency generation and detection at the specified ranges. The method restores the original hardware
            /// state after the test completes.</remarks>
            /// <returns>true if at least one of the tested high frequencies is successfully generated and detected; otherwise,
            /// false.</returns>
            public static bool AdvancedFrequencySweepTest()
            {
                try
                {
                    byte originalState = (byte)Inp32(0x61);
                    bool anyFrequencyWorks = false;

                    // Only high frequencies to avoid audible noise to users
                    int[] testFrequencies = { 30000, 35000, 38000 };

                    foreach (int freq in testFrequencies)
                    {
                        int div = PIT_BASE_FREQ / freq;
                        if (div < 1) continue;

                        // Configure the timer
                        Out32(0x43, 0xB6);
                        Out32(0x42, (byte)(div & 0xFF));
                        Out32(0x42, (byte)(div >> 8));
                        Program.splashScreen.ResponsiveWait(10);

                        // Open the gate
                        Out32(0x61, (byte)(originalState | 0x01));
                        Program.splashScreen.ResponsiveWait(10);

                        // Open the speaker data
                        Out32(0x61, (byte)(originalState | 0x03));
                        Program.splashScreen.ResponsiveWait(30);

                        List<byte> samples = new List<byte>();
                        for (int i = 0; i < 50; i++)
                        {
                            samples.Add((byte)Inp32(0x61));
                        }

                        // Close the speaker data
                        Out32(0x61, (byte)((originalState & 0xFE) | 0x01));
                        Program.splashScreen.ResponsiveWait(10);

                        // Close the gate
                        Out32(0x61, (byte)(originalState & 0xFC));
                        Program.splashScreen.ResponsiveWait(10);

                        int transitions = 0;
                        for (int i = 0; i < samples.Count - 1; i++)
                        {
                            if ((samples[i] & 0x20) != (samples[i + 1] & 0x20))
                                transitions++;
                        }

                        Console.WriteLine($"  {freq} Hz: {transitions} transitions (SILENT)");

                        if (transitions >= 2)
                        {
                            anyFrequencyWorks = true;
                            break;
                        }
                    }

                    Out32(0x61, originalState);
                    Program.splashScreen.ResponsiveWait(20);
                    return anyFrequencyWorks;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            private static readonly Mutex SystemSpeakerMutex = new Mutex(false, "Global\\NeoBleeperSystemSpeakerMutex"); // Mutex to prevent concurrent access to system speaker checks

            /// <summary>
            /// Determines whether the system speaker is functional based on a series of hardware checks.
            /// </summary>
            /// <remarks>This method attempts to acquire a mutex before performing hardware checks to
            /// ensure thread safety. If the mutex cannot be acquired within the timeout period, the method returns
            /// false. The checks performed may include electrical feedback validation, port state stability, and a
            /// frequency sweep test. This method is intended for internal use when verifying system speaker
            /// functionality.</remarks>
            /// <returns>true if at least one of the system speaker validation checks passes; otherwise, false.</returns>
            private static bool IsFunctionalSystemSpeaker()
            {
                bool acquired = false;
                try
                {
                    // Try to acquire the mutex with a timeout to avoid indefinite blocking
                    acquired = SystemSpeakerMutex.WaitOne(TimeSpan.FromSeconds(5));
                    if (!acquired)
                    {
                        // Handle the case where the mutex could not be acquired
                        return false;
                    }

                    // Perform the system speaker checks
                    bool electricalFeedbackValid = CheckElectricalFeedbackOnPort();
                    bool portStateStable = CheckPortStateStability();
                    bool frequencySweepWorks = AdvancedFrequencySweepTest();

                    return electricalFeedbackValid || portStateStable || frequencySweepWorks;
                }
                finally
                {
                    if (acquired)
                    {
                        SystemSpeakerMutex.ReleaseMutex();
                    }
                }
            }

            /// <summary>
            /// Determines whether a system speaker (PC speaker) is present and accessible on the current device.
            /// </summary>
            /// <remarks>On devices where a system speaker is not present or cannot be detected, the
            /// application will fall back to using the sound card for beep functionality. System speaker detection is
            /// not supported on ARM64 architectures; in such cases, this method always returns false.</remarks>
            /// <returns>true if a system speaker is detected and can be accessed; otherwise, false.</returns>
            public static bool IsSystemSpeakerExist()
            {
                // No system speaker, no problem.
                // Because it's falling back to sound card beep if no system speaker is found.

                // Step 1: Check for the presence of a system speaker device using WMI
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    try
                    {
                        Program.splashScreen.UpdateStatus(Resources.StatusSystemSpeakerSensorStep1, 10);
                        bool isSystemSpeakerPresentInWMI = false;
                        string query = "SELECT * FROM Win32_PNPEntity WHERE DeviceID LIKE '%PNP0800%'";
                        using (var searcher = new ManagementObjectSearcher(query))
                        {
                            var devices = searcher.Get();
                            isSystemSpeakerPresentInWMI = (devices.Count > 0);
                        }

                        // Step 2: Check for electrical feedback on port 0x61 to determine if the system speaker output is physically functional if WMI check is inconclusive
                        Program.splashScreen.UpdateStatus(Resources.StatusSystemSpeakerSensorStep2, 10);
                        bool isSystemSpeakerOutputPhysicallyFunctional = IsFunctionalSystemSpeaker();

                        // Return true if electrical feedback is detected or if WMI check confirms presence
                        bool result = isSystemSpeakerPresentInWMI || isSystemSpeakerOutputPhysicallyFunctional;
                        if (result == true)
                        {
                            Program.splashScreen.UpdateStatus(Resources.StatusSystemSpeakerOutputPresent);
                        }
                        else
                        {
                            Program.splashScreen.UpdateStatus(Resources.StatusSystemSpeakerOutputNotPresent);
                        }
                        Program.isExistenceOfSystemSpeakerChecked = true; // Mark that the check has been performed
                        return result;
                    }
                    catch (Exception ex)
                    {
                        Program.isExistenceOfSystemSpeakerChecked = false; // Mark that the check failed
                        Logger.Log("Error during system speaker detection: " + ex.Message, Logger.LogTypes.Error);
                        Program.splashScreen.UpdateStatus("Error during system speaker detection. Assuming no system speaker.");
                        return false; // On error, assume no system speaker
                    }
                }
                else
                {
                    Logger.Log("System speaker detection skipped on ARM64 architecture due to ARM64 doesn't support system speaker access.", Logger.LogTypes.Info);
                    return false; // ARM64 devices such as most of Copilot+ devices do not support system speaker access
                }
            }

            /// <summary>
            /// Specifies the manufacturer of a processor.
            /// </summary>
            /// <remarks>Use this enumeration to identify the vendor of a processor, such as Intel or
            /// AMD. The value 'Other' represents manufacturers not explicitly listed.</remarks>
            enum ProcessorManufacturer // Enum for known processor manufacturers
            {
                Intel,
                AMD,
                Other
            }

            /// <summary>
            /// Determines whether the current system's chipset is known to be affected by system speaker issues that
            /// may cause incorrect or degraded beep sounds.
            /// </summary>
            /// <remarks>This method checks for specific Intel and AMD chipset patterns that have been
            /// reported to exhibit system speaker problems, such as distorted or incomplete beeps. The check is not
            /// performed on ARM64 devices, which typically do not support system speaker access. If an error occurs
            /// during detection, the method returns false and assumes the chipset is not affected.</remarks>
            /// <returns>true if the chipset is identified as affected by known system speaker issues; otherwise, false.</returns>
            public static bool CheckIfChipsetAffectedFromSystemSpeakerIssues() // Check if the chipset known to have system speaker issues
            // Added according M084MM3D's report states that "i have a PRIME H610M-A WIFI, and the bleeper beeps but in a very bad way, like the beep doesnt hold and it sounds like noise"
            // and some software-based beep issue, such as Linux's Beep command, reports on ASUS motherboards in various forums and operating systems
            {
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    try
                    {
                        ProcessorManufacturer processorManufacturer = ProcessorManufacturer.Other;
                        Program.splashScreen.UpdateStatus(Resources.StatusCheckingChipsetForSystemSpeakerIssues);
                        // Known affected chipset patterns with system speaker issues
                        string affectedIntelChipsetPattern = @"\b([BZHQ][67][0-9]{2})\b"; // Affected Intel chipset pattern
                        string affectedAMDChipsetPattern = @"\b([BX][56]50|X670)\b"; // Affected AMD chipset pattern
                        var identifiersToSearch = new List<string>();

                        // 1. Take Win32_Processor information
                        using (var searcher = new ManagementObjectSearcher("SELECT Name, Manufacturer FROM Win32_Processor"))
                        {
                            foreach (var obj in searcher.Get())
                            {
                                identifiersToSearch.Add(obj["Name"]?.ToString() ?? string.Empty);
                                identifiersToSearch.Add(obj["Manufacturer"]?.ToString() ?? string.Empty);
                                if (obj["Manufacturer"] != null)
                                {
                                    string manufacturer = obj["Manufacturer"].ToString();
                                    if (manufacturer.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        processorManufacturer = ProcessorManufacturer.Intel;
                                    }
                                    else if (manufacturer.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        processorManufacturer = ProcessorManufacturer.AMD;
                                    }
                                    else
                                    {
                                        processorManufacturer = ProcessorManufacturer.Other;
                                    }
                                }
                            }
                        }

                        // 2. Take Win32_BaseBoard information
                        using (var searcher = new ManagementObjectSearcher("SELECT Product, Version, Name FROM Win32_BaseBoard"))
                        {
                            foreach (var obj in searcher.Get())
                            {
                                identifiersToSearch.Add(obj["Product"]?.ToString() ?? string.Empty);
                                identifiersToSearch.Add(obj["Version"]?.ToString() ?? string.Empty);
                                identifiersToSearch.Add(obj["Name"]?.ToString() ?? string.Empty);
                            }
                        }

                        // 3. Take Win32_ComputerSystemProduct information
                        using (var searcher = new ManagementObjectSearcher("SELECT Name, Version FROM Win32_ComputerSystemProduct"))
                        {
                            foreach (var obj in searcher.Get())
                            {
                                identifiersToSearch.Add(obj["Name"]?.ToString() ?? string.Empty);
                                identifiersToSearch.Add(obj["Version"]?.ToString() ?? string.Empty);
                            }
                        }

                        // Search for affected chipsets in the collected identifiers
                        foreach (var identifier in identifiersToSearch.Where(s => !string.IsNullOrEmpty(s)))
                        {
                            if (processorManufacturer == ProcessorManufacturer.Intel)
                            {
                                if (Regex.IsMatch(identifier, affectedIntelChipsetPattern, RegexOptions.IgnoreCase))
                                {
                                    string chipset = Regex.Match(identifier, affectedIntelChipsetPattern, RegexOptions.IgnoreCase).Value;
                                    string localizedAffectedMessage = Resources.StatusCheckingChipsetForSystemSpeakerIssues.Replace("{chipset}", chipset);
                                    Program.splashScreen.UpdateStatus(localizedAffectedMessage, 5);
                                    Program.isAffectedChipsetChecked = true; // Mark that the check has been performed
                                    return true; // Affected chipset found
                                }
                            }
                            else if (processorManufacturer == ProcessorManufacturer.AMD)
                            {
                                if (Regex.IsMatch(identifier, affectedAMDChipsetPattern, RegexOptions.IgnoreCase))
                                {
                                    string chipset = Regex.Match(identifier, affectedIntelChipsetPattern, RegexOptions.IgnoreCase).Value;
                                    string localizedAffectedMessage = Resources.StatusCheckingChipsetForSystemSpeakerIssues.Replace("{chipset}", chipset);
                                    Program.splashScreen.UpdateStatus(localizedAffectedMessage, 5);
                                    Program.isAffectedChipsetChecked = true; // Mark that the check has been performed
                                    return true; // Affected chipset found
                                }
                            }
                        }

                        Program.splashScreen.UpdateStatus(Resources.StatusChipsetIsNotAffected, 5);
                        Program.isAffectedChipsetChecked = true; // Mark that the check has been performed
                        return false; // Return false if no match is found or manufacturer of processor is neither Intel or AMD
                    }
                    catch
                    {
                        Program.splashScreen.UpdateStatus(Resources.StatusErrorCheckingChipset);
                        Program.isAffectedChipsetChecked = false; // Mark that the check failed
                        return false; // On error, assume not affected
                    }
                }
                else
                {
                    return false; // ARM64 devices such as most of Copilot+ devices do not support system speaker access
                }
            }

            /// <summary>
            /// Attempts to restore functionality of the system speaker on affected chipsets by simulating a sleep and
            /// wake-up sequence.
            /// </summary>
            /// <remarks>This method is intended for use on non-ARM64 systems where the system speaker
            /// may become unresponsive due to known hardware issues. It has no effect on systems that are not affected.
            /// The method is thread-safe and will not block indefinitely if the speaker is currently being reset by
            /// another process.</remarks>
            public static void AwakeSystemSpeakerIfNeeded() // Attempt to fix system speaker in some systems by simulating sleep and wake up
            {
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    if (TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues)
                    {
                        bool acquired = false;
                        // Try to acquire the mutex with a timeout to avoid indefinite blocking
                        acquired = SystemSpeakerMutex.WaitOne(TimeSpan.FromSeconds(5));
                        if (!acquired)
                        {
                            // Handle the case where the mutex could not be acquired
                            return;
                        }
                        try
                        {
                            Program.splashScreen.UpdateStatus(Resources.StatusWakingUpSystemSpeaker);
                            byte originalState = (byte)Inp32(0x61);

                            // 1. Close the speaker gate completely to ensure a clean state.
                            Out32(0x61, (byte)(originalState & 0xFC));
                            Program.splashScreen.ResponsiveWait(20);

                            // 2. Reset PIT channel 2 to a known state (e.g., mode 0, terminal count).
                            // This helps to stop any ongoing oscillations.
                            Out32(0x43, 0xB0); // Channel 2, LSB/MSB access, mode 0, binary
                            Out32(0x42, 0x00); // LSB
                            Out32(0x42, 0x00); // MSB
                            Program.splashScreen.ResponsiveWait(20);

                            // 3. "Tickle" the speaker gate by toggling it. This can help wake up the circuitry.
                            // Open only the gate (bit 0), keep speaker data (bit 1) off.
                            Out32(0x61, (byte)(originalState | 0x01));
                            Program.splashScreen.ResponsiveWait(50);
                            // Close it again.
                            Out32(0x61, (byte)(originalState & 0xFC));
                            Program.splashScreen.ResponsiveWait(50);

                            // 4. Restore the original state of the speaker port.
                            Out32(0x61, originalState);
                            Program.splashScreen.UpdateStatus(Resources.StatusSystemSpeakerWokenUp, 2);
                        }
                        catch (Exception ex)
                        {
                            Program.splashScreen.UpdateStatus(Resources.StatusErrorWakingUpSystemSpeaker + ex.Message);
                        }
                        finally
                        {
                            if (acquired)
                            {
                                SystemSpeakerMutex.ReleaseMutex();
                            }
                        }
                    }
                }
            }
        }
        public static class WaveSynthEngine // Synthesize various waveforms of beeps and noises by emulating FMOD, that is used in Bleeper Music Maker, using NAudio
        {
            public static readonly WaveOutEvent waveOut = new WaveOutEvent();
            private static readonly SignalGenerator signalGenerator = new SignalGenerator() { Gain = 0.15 };
            private static readonly SignalGenerator whiteNoiseGenerator = new SignalGenerator() { Type = SignalGeneratorType.Pink, Gain = 0.5 };
            private static BandPassNoiseGenerator bandPassNoise;
            private static ISampleProvider currentProvider; // To keep track of the current provider

            // FMOD? That's a F-problem, so we use NAudio instead.

            static WaveSynthEngine()
            {
                currentProvider = signalGenerator;
                waveOut.DesiredLatency = 50;
                waveOut.NumberOfBuffers = 4;
                waveOut.Volume = 1.0f; // Ensure volume is at max to prevent stuck muted sound
                waveOut.Init(signalGenerator);
            }

            /// <summary>
            /// Determines whether any enabled sound device is present on the system.
            /// </summary>
            /// <remarks>This method checks for sound devices that are both present and enabled, using
            /// system management queries. It may return false if no enabled sound devices are found or if an error
            /// occurs while accessing device information.</remarks>
            /// <returns>true if at least one enabled sound device is detected; otherwise, false.</returns>
            public static bool CheckIfAnySoundDeviceExistAndEnabled()
            {
                using (var enumerator = new MMDeviceEnumerator())
                {
                    try
                    {
                        var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                        bool status = device.AudioEndpointVolume.MasterVolumeLevelScalar >= 0.0f;
                        string query = "SELECT * FROM Win32_SoundDevice where Status = 'OK' and Availability = 3"; // Query for enabled sound devices
                        using (var searcher = new System.Management.ManagementObjectSearcher(query))
                        {
                            var devices = searcher.Get();
                            return devices.Count > 0; // Return true if any enabled sound device is found
                        }
                    }
                    catch (COMException)
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// Sets the current audio sample provider for playback.
            /// </summary>
            /// <remarks>If the specified provider differs from the current provider, playback is
            /// stopped and reinitialized with the new provider. This method is thread-safe.</remarks>
            /// <param name="provider">The audio sample provider to use for playback. Cannot be null.</param>
            private static void SetCurrentProvider(ISampleProvider provider)
            {
                lock (AudioLock)
                {
                    if (currentProvider != provider)
                    {
                        if (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            waveOut.Stop();
                        }
                        waveOut.Init(provider); // Restart the provider if only changed
                        currentProvider = provider;
                    }
                }
            }

            /// <summary>
            /// Plays a sound for the specified duration, with optional control over whether playback is stopped
            /// automatically.
            /// </summary>
            /// <remarks>This method is thread-safe. If called while audio is already playing, it will
            /// not restart playback. When <paramref name="nonStopping"/> is <see langword="false"/>, playback is
            /// stopped after the specified duration; otherwise, the caller is responsible for stopping
            /// playback.</remarks>
            /// <param name="ms">The duration, in milliseconds, to play the sound. Specify 0 to play without a timed stop.</param>
            /// <param name="nonStopping">If <see langword="true"/>, the sound continues playing after the method returns; otherwise, playback is
            /// stopped automatically after the specified duration.</param>
            private static void PlaySound(int ms, bool nonStopping)
            {
                lock (AudioLock)
                {
                    if (!nonStopping)
                    {
                        HighPrecisionSleep.Sleep(5);
                    }

                    // Ensure waveOut is not already playing
                    if (waveOut.PlaybackState != PlaybackState.Playing)
                    {
                        waveOut.Play();
                    }
                }

                if (ms > 0)
                {
                    HighPrecisionSleep.Sleep(ms);
                }

                // Stop playback by silencing the source
                if (!nonStopping)
                {
                    StopSynth();
                }
            }

            /// <summary>
            /// Determines whether all audio wave outputs are currently muted based on the active audio provider's gain
            /// settings.
            /// </summary>
            /// <remarks>This method checks the gain of the current audio provider to determine if
            /// audio output is effectively muted. If no provider is active, the method considers the output
            /// muted.</remarks>
            /// <returns>true if the active audio provider's gain is zero or if no provider is active; otherwise, false.</returns>
            public static bool AreWavesMutedEarly()
            {
                lock (AudioLock)
                {
                    if (currentProvider == signalGenerator)
                    {
                        return signalGenerator.Gain == 0;
                    }
                    else if (currentProvider == bandPassNoise)
                    {
                        return whiteNoiseGenerator.Gain == 0;
                    }
                    return true; // If no provider is active, consider it muted
                }
            }

            /// <summary>
            /// Stops audio synthesis by muting the currently active audio provider, if any.
            /// </summary>
            /// <remarks>This method is thread-safe and can be called at any time to immediately
            /// silence audio output. It has no effect if no audio provider is currently active.</remarks>
            public static void StopSynth()
            {
                lock (AudioLock)
                {
                    if (currentProvider == signalGenerator)
                    {
                        signalGenerator.Gain = 0;
                    }
                    else if (currentProvider == bandPassNoise)
                    {
                        whiteNoiseGenerator.Gain = 0;
                    }
                }
            }

            /// <summary>
            /// Plays a synthesized audio wave of the specified type, frequency, and duration.
            /// </summary>
            /// <param name="type">The type of waveform to generate for the audio signal.</param>
            /// <param name="freq">The frequency of the wave, in hertz. Must be a positive integer.</param>
            /// <param name="ms">The duration of the sound to play, in milliseconds. Must be greater than zero.</param>
            /// <param name="nonStopping">If set to <see langword="true"/>, the sound will play without interrupting any currently playing sound;
            /// otherwise, it may interrupt ongoing playback.</param>
            public static void PlayWave(SignalGeneratorType type, int freq, int ms, bool nonStopping)
            {
                lock (AudioLock)
                {
                    if (currentProvider != signalGenerator)
                    {
                        SetCurrentProvider(signalGenerator);
                    }
                    if (signalGenerator.Frequency != freq || signalGenerator.Type != type || signalGenerator.Gain == 0)
                    {
                        signalGenerator.Frequency = freq;
                        signalGenerator.Type = type;
                        signalGenerator.Gain = 0.15;
                    }
                }
                PlaySound(ms, nonStopping);
            }

            /// <summary>
            /// Plays a band-pass filtered noise sound at the specified center frequency for a given duration.
            /// </summary>
            /// <param name="freq">The center frequency, in hertz, of the band-pass filter to apply to the noise. Must be a positive
            /// integer.</param>
            /// <param name="ms">The duration, in milliseconds, for which the filtered noise will be played. Must be greater than zero.</param>
            /// <param name="nonStopping">If set to <see langword="true"/>, the sound will not be interrupted by other playback requests;
            /// otherwise, it may be stopped by subsequent calls.</param>

            public static void PlayFilteredNoise(int freq, int ms, bool nonStopping)
            {
                lock (AudioLock)
                {
                    if (bandPassNoise == null)
                    {
                        bandPassNoise = new BandPassNoiseGenerator(whiteNoiseGenerator, 44100, freq, 1.0f);
                    }
                    else
                    {
                        bandPassNoise.UpdateFrequency(freq, 44100, 1.0f);
                    }

                    if (currentProvider != bandPassNoise)
                    {
                        SetCurrentProvider(bandPassNoise);
                    }
                    if (whiteNoiseGenerator.Gain == 0)
                    {
                        whiteNoiseGenerator.Gain = 0.5; // Restore gain
                    }
                }
                PlaySound(ms, nonStopping);
            }

            /// <summary>
            /// Plays a square wave tone with the specified frequency and duration.
            /// </summary>
            /// <param name="freq">The frequency of the square wave, in hertz. Must be a positive integer.</param>
            /// <param name="ms">The duration of the tone, in milliseconds. Must be a non-negative integer.</param>
            /// <param name="nonStopping">true to allow overlapping tones to play without stopping previous ones; otherwise, false to stop any
            /// currently playing tone before starting the new one.</param>
            public static void SquareWave(int freq, int ms, bool nonStopping)
            {
                PlayWave(SignalGeneratorType.Square, freq, ms, nonStopping);
            }

            /// <summary>
            /// Plays a sine wave tone with the specified frequency and duration.
            /// </summary>
            /// <param name="freq">The frequency of the sine wave, in hertz. Must be a positive integer.</param>
            /// <param name="ms">The duration of the tone, in milliseconds. Must be a non-negative integer.</param>
            /// <param name="nonStopping">true to allow overlapping tones to play without stopping previous ones; otherwise, false to stop any
            /// currently playing tone before starting the new one.</param>
            public static void SineWave(int freq, int ms, bool nonStopping)
            {
                PlayWave(SignalGeneratorType.Sin, freq, ms, nonStopping);
            }

            /// <summary>
            /// Plays a triangle wave tone with the specified frequency and duration.
            /// </summary>
            /// <param name="freq">The frequency of the triangle wave, in hertz. Must be a positive integer.</param>
            /// <param name="ms">The duration of the tone, in milliseconds. Must be a non-negative integer.</param>
            /// <param name="nonStopping">true to allow overlapping tones to play without stopping previous ones; otherwise, false to stop any
            /// currently playing tone before starting the new one.</param>
            public static void TriangleWave(int freq, int ms, bool nonStopping)
            {
                PlayWave(SignalGeneratorType.Triangle, freq, ms, nonStopping);
            }

            /// <summary>
            /// Plays a noise with the specified frequency and duration.
            /// </summary>
            /// <param name="freq">The frequency of the noise, in hertz. Must be a positive integer.</param>
            /// <param name="ms">The duration of the noise, in milliseconds. Must be a non-negative integer.</param>
            /// <param name="nonStopping">true to allow overlapping noises to play without stopping previous ones; otherwise, false to stop any
            /// currently playing tone before starting the new one.</param>
            public static void Noise(int freq, int ms, bool nonStopping)
            {
                PlayFilteredNoise(freq, ms, nonStopping);
            }
        }
        public class BandPassNoiseGenerator : ISampleProvider
        {
            private readonly ISampleProvider noiseGenerator;
            private BiQuadFilter bandPassFilter;

            public BandPassNoiseGenerator(ISampleProvider noiseGenerator, int sampleRate, float centerFrequency, float bandwidth)
            {
                this.noiseGenerator = noiseGenerator;
                bandPassFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, centerFrequency, bandwidth);
            }

            public WaveFormat WaveFormat => noiseGenerator.WaveFormat;

            /// <summary>
            /// Reads a sequence of band-pass filtered noise samples into the specified buffer.
            /// </summary>
            /// <remarks>Each sample written to the buffer is processed through a band-pass filter
            /// before being stored. The method does not clear the buffer; only the written elements are
            /// modified.</remarks>
            /// <param name="buffer">The array of single-precision floating-point values that receives the filtered samples. Must not be
            /// null.</param>
            /// <param name="offset">The zero-based index in the buffer at which to begin storing the samples. Must be non-negative and less
            /// than the length of the buffer.</param>
            /// <param name="count">The maximum number of samples to read. Must be non-negative and the sum of offset and count must not
            /// exceed the length of the buffer.</param>
            /// <returns>The total number of samples read into the buffer. This value may be less than the requested count if the
            /// end of the data is reached.</returns>
            public int Read(float[] buffer, int offset, int count)
            {
                int samplesRead = noiseGenerator.Read(buffer, offset, count);
                for (int i = 0; i < samplesRead; i++)
                {
                    buffer[offset + i] = bandPassFilter.Transform(buffer[offset + i]);
                }
                return samplesRead;
            }

            // Update the center frequency dynamically  

            /// <summary>
            /// Updates the center frequency and reconfigures the band-pass filter with the specified parameters.
            /// </summary>
            /// <param name="newFrequency">The new center frequency, in hertz, to set for the band-pass filter.</param>
            /// <param name="sampleRate">The sample rate, in hertz, used to configure the filter. Must be greater than zero.</param>
            /// <param name="bandwidth">The bandwidth, in hertz, for the band-pass filter. Must be greater than zero.</param>
            public void UpdateFrequency(float newFrequency, int sampleRate, float bandwidth)
            {
                bandPassFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, newFrequency, bandwidth);
            }
        }
        public class FilteredWaveProvider : ISampleProvider
        {
            private readonly ISampleProvider source;
            private BiQuadFilter filter;
            private double gain;

            public ISampleProvider Source => source;

            public BiQuadFilter Filter => filter;

            public FilteredWaveProvider(ISampleProvider source, BiQuadFilter filter, double gain)
            {
                this.source = source;
                this.filter = filter;
                this.gain = gain;
            }

            /// <summary>
            /// Sets the gain value to the specified amount.
            /// </summary>
            /// <param name="newGain">The new gain value to set.</param>
            public void UpdateGain(double newGain)
            {
                gain = newGain;
            }
            public WaveFormat WaveFormat => source.WaveFormat;

            /// <summary>
            /// Replaces the current filter with the specified BiQuad filter.
            /// </summary>
            /// <param name="newFilter">The new BiQuadFilter instance to use. Cannot be null.</param>
            public void UpdateFilter(BiQuadFilter newFilter)
            {
                filter = newFilter;
            }

            /// <summary>
            /// Reads a sequence of samples from the source, applies filtering and gain adjustment, and writes the
            /// results to the specified buffer.
            /// </summary>
            /// <remarks>Each sample read from the source is processed through the filter and
            /// multiplied by the current gain before being written to the buffer. The method does not modify the
            /// contents of the buffer outside the specified range.</remarks>
            /// <param name="buffer">The array of floats that receives the filtered and gain-adjusted samples. Must not be null and must have
            /// sufficient space to accommodate the requested number of samples starting at the specified offset.</param>
            /// <param name="offset">The zero-based index in the buffer at which to begin storing the samples. Must be non-negative and less
            /// than the length of the buffer.</param>
            /// <param name="count">The maximum number of samples to read and process. Must be non-negative and the range defined by offset
            /// and count must not exceed the length of the buffer.</param>
            /// <returns>The number of samples read and written to the buffer. This value may be less than the requested count if
            /// the end of the source is reached.</returns>
            public int Read(float[] buffer, int offset, int count)
            {
                int samplesRead = source.Read(buffer, offset, count);
                for (int i = 0; i < samplesRead; i++)
                {
                    buffer[offset + i] = (float)(filter.Transform(buffer[offset + i]) * gain);
                }
                return samplesRead;
            }
        }
        public class CachedSound
        {
            public readonly float[] AudioData;
            public readonly WaveFormat WaveFormat;
            public CachedSound(float[] audioData, WaveFormat waveFormat)
            {
                AudioData = audioData;
                WaveFormat = waveFormat;
            }
        }

        public class CachedSoundSampleProvider : ISampleProvider
        {
            public readonly CachedSound cached;
            private long position;
            public bool Loop { get; set; } = true;
            public CachedSoundSampleProvider(CachedSound cached, bool loop = true)
            {
                this.cached = cached;
                Loop = loop;
                position = 0;
            }
            public WaveFormat WaveFormat => cached.WaveFormat;

            /// <summary>
            /// Reads a sequence of audio samples from the current position into the specified buffer.
            /// </summary>
            /// <remarks>If looping is enabled, reading continues from the beginning of the audio data
            /// when the end is reached, until the requested number of samples is read or the buffer is filled. If
            /// looping is disabled, reading stops at the end of the audio data and the number of samples read may be
            /// less than requested.</remarks>
            /// <param name="buffer">The array of floats that receives the audio samples read from the source. Must not be null.</param>
            /// <param name="offset">The zero-based index in the buffer at which to begin storing the audio samples.</param>
            /// <param name="count">The maximum number of audio samples to read. Must be non-negative and the range defined by offset and
            /// count must not exceed the length of the buffer.</param>
            /// <returns>The total number of audio samples read into the buffer. This value may be less than the number of
            /// samples requested if the end of the audio data is reached and looping is not enabled.</returns>
            public int Read(float[] buffer, int offset, int count)
            {
                int written = 0;
                while (written < count)
                {
                    int available = cached.AudioData.Length - (int)position;
                    if (available <= 0)
                    {
                        if (!Loop) break;
                        position = 0;
                        available = cached.AudioData.Length;
                    }
                    int toCopy = Math.Min(available, count - written);
                    Array.Copy(cached.AudioData, position, buffer, offset + written, toCopy);
                    position += toCopy;
                    written += toCopy;
                }
                return written;
            }
        }
        public static class VoiceSynthesisEngine // Voice synthesis by emulating FMOD that is used in Bleeper Music Maker using NAudio
        {
            // "Rubbish" system? At least it can synthesize voices better than nothing. 
            private static readonly object synthLock = new();
            // Single master mixer
            private static readonly MixingSampleProvider masterMixer;
            private static readonly WaveOutEvent masterWaveOut;
            private static readonly Dictionary<int, (
                RemovableSampleProvider removable,
                ISampleProvider provider,
                FilteredWaveProvider finalFiltered,
                SignalGenerator sineSource,
                SignalGenerator triangleSource,
                double[] formantFreqs,
                double[] formantVols,
                BiQuadFilter lowPass,
                List<FilteredWaveProvider> formantProviders
            )> channels = new();
            static VoiceSynthesisEngine()
            {
                const int sampleRate = 44100;
                masterMixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1)) { ReadFully = true };
                masterWaveOut = new WaveOutEvent
                {
                    DesiredLatency = 50,
                    NumberOfBuffers = 4,
                    Volume = 1.0f
                };
                masterWaveOut.Init(masterMixer);
                masterWaveOut.Play();
            }

            private class RemovableSampleProvider : ISampleProvider
            {
                private readonly ISampleProvider inner;
                private volatile bool removed;
                public RemovableSampleProvider(ISampleProvider inner) => this.inner = inner;
                public void Remove() => removed = true;
                public WaveFormat WaveFormat => inner.WaveFormat;

                /// <summary>
                /// Reads a sequence of samples from the current stream into the specified buffer.
                /// </summary>
                /// <param name="buffer">The array of floats that receives the samples read from the stream. Cannot be null.</param>
                /// <param name="offset">The zero-based index in the buffer at which to begin storing the data read from the stream. Must be
                /// non-negative and less than the length of the buffer.</param>
                /// <param name="count">The maximum number of samples to read. Must be non-negative and the sum of offset and count must not
                /// exceed the length of the buffer.</param>
                /// <returns>The total number of samples read into the buffer. This can be less than the number of samples
                /// requested if that many samples are not currently available, or zero if the end of the stream has
                /// been reached.</returns>
                public int Read(float[] buffer, int offset, int count)
                {
                    if (removed)
                    {
                        Array.Clear(buffer, offset, count);
                        return count;
                    }
                    return inner.Read(buffer, offset, count);
                }
            }

            /// <summary>
            /// Applies the current frequency values to all active voices.
            /// </summary>
            /// <remarks>This method updates each voice with its corresponding cached frequency if the
            /// voice is currently active. It is typically used to synchronize the state of all voices after frequency
            /// values have changed.</remarks>
            public static void ApplyValues()
            {
                int frequency = 0;
                bool isPlaying = false;
                for (int i = 0; i < 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            frequency = cachedFrequency1;
                            isPlaying = voice1Playing;
                            break;
                        case 1:
                            frequency = cachedFrequency2;
                            isPlaying = voice2Playing;
                            break;
                        case 2:
                            frequency = cachedFrequency3;
                            isPlaying = voice3Playing;
                            break;
                        case 3:
                            frequency = cachedFrequency4;
                            isPlaying = voice4Playing;
                            break;
                    }
                    if (isPlaying)
                    {
                        ChangeValues(i, frequency);
                    }
                }
            }

            /// <summary>
            /// Creates and configures signal generators for sine and triangle waveforms using the specified frequency
            /// and master volume.
            /// </summary>
            /// <remarks>The gain for each signal generator is calculated based on the provided master
            /// volume and internal settings. Both generators use the same sample rate and channel count as the master
            /// mixer.</remarks>
            /// <param name="modulatedFrequency">The frequency, in hertz, to use for both the sine and triangle signal generators. Must be a positive
            /// value.</param>
            /// <param name="masterVolume">The master volume level to apply to the generated signals. Must be a non-negative value.</param>
            /// <returns>A tuple containing the configured sine and triangle signal generators. The first item is the sine
            /// generator; the second is the triangle generator.</returns>
            private static (SignalGenerator sineSource, SignalGenerator triangleSource) CreateSignalGenerators(double modulatedFrequency, double masterVolume)
            {
                var wf = masterMixer.WaveFormat;
                SignalGenerator sineSource = new SignalGenerator(wf.SampleRate, wf.Channels)
                {
                    Type = SignalGeneratorType.Sin,
                    Frequency = modulatedFrequency,
                    Gain = masterVolume * (VoiceInternalSettings.sawVolume / 1000.0) * 0.3
                };

                SignalGenerator triangleSource = new SignalGenerator(wf.SampleRate, wf.Channels)
                {
                    Type = SignalGeneratorType.SawTooth,
                    Frequency = modulatedFrequency,
                    Gain = masterVolume * (VoiceInternalSettings.sawVolume / 1000.0) * 1.2
                };

                return (sineSource, triangleSource);
            }

            /// <summary>
            /// Updates the frequency and filter parameters for the specified audio channel based on the provided base
            /// frequency and current voice settings.
            /// </summary>
            /// <remarks>This method applies randomized pitch variation and updates formant and
            /// low-pass filter parameters according to the current voice settings. Thread safety is ensured for channel
            /// updates. If the specified channel does not exist, no action is taken.</remarks>
            /// <param name="channelId">The identifier of the audio channel to update. Must correspond to an existing channel.</param>
            /// <param name="baseFrequency">The base frequency, in hertz, used to calculate the new pitch and filter settings for the channel.</param>
            public static void ChangeValues(int channelId, int baseFrequency)
            {
                lock (synthLock)
                {
                    if (channels.TryGetValue(channelId, out var tuple))
                    {
                        const int sampleRate = 44100;

                        double rawTimbre = TemporarySettings.VoiceInternalSettings.timbre;
                        double rawRandomizedFrequencyRange = TemporarySettings.VoiceInternalSettings.randomizedFrequencyRange;

                        double randomVariation = (Random.Shared.NextDouble() - 0.5) * 2.0 * rawRandomizedFrequencyRange * 16;
                        double finalPitchMultiplier = (1 + rawTimbre) * 0.25;
                        double modulatedFrequency = ((baseFrequency * finalPitchMultiplier) / 4) + randomVariation;

                        // Update frequency of signal generators
                        tuple.sineSource.Frequency = modulatedFrequency;
                        tuple.triangleSource.Frequency = modulatedFrequency;

                        // Take formant frequencies and volumes
                        double[] currentFormantFreqs = new double[] {
                VoiceInternalSettings.formant1Frequency,
                VoiceInternalSettings.formant2Frequency,
                VoiceInternalSettings.formant3Frequency,
                VoiceInternalSettings.formant4Frequency
            };

                        double[] currentFormantVols = new double[] {
                VoiceInternalSettings.formant1Volume / 100.0,
                VoiceInternalSettings.formant2Volume / 100.0,
                VoiceInternalSettings.formant3Volume / 100.0,
                VoiceInternalSettings.formant4Volume / 100.0
            };

                        double noiseToFormantScale = VoiceInternalSettings.noiseVolume / 100.0 * (VoiceInternalSettings.noiseVolume > 0 ? 1.0 : 0.0);
                        float BaseFormantQ(int bf) => bf < 2000 ? 2.0f : 1.0f;

                        // Update formant providers
                        for (int i = 0; i < 4; i++)
                        {
                            double fCenter = currentFormantFreqs[i];
                            double fVol = currentFormantVols[i];
                            float dynamicQ = (float)(0.5f + (i * 0.3f)) * BaseFormantQ(baseFrequency) * 2.4f;

                            // Update voiced and noise provider for every note
                            int voicedIndex = i * 2;
                            int noiseIndex = i * 2 + 1;

                            if (voicedIndex < tuple.formantProviders.Count)
                            {
                                var voicedFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, (float)fCenter, dynamicQ);
                                tuple.formantProviders[voicedIndex].UpdateFilter(voicedFilter);
                                tuple.formantProviders[voicedIndex].UpdateGain(Math.Min(fVol * 1.0, 2.0));
                            }

                            if (noiseIndex < tuple.formantProviders.Count)
                            {
                                var noiseFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, (float)fCenter, dynamicQ * 1.1f);
                                tuple.formantProviders[noiseIndex].UpdateFilter(noiseFilter);
                                tuple.formantProviders[noiseIndex].UpdateGain(noiseToFormantScale * fVol * 1.2);
                            }
                        }

                        // Update lowpass filter
                        var newLowPass = BiQuadFilter.LowPassFilter(sampleRate, VoiceInternalSettings.cutoffFrequency, 1.0f);
                        tuple.finalFiltered.UpdateFilter(newLowPass);
                    }
                }
            }
            static int cachedFrequency1 = 0;
            static int cachedFrequency2 = 0;
            static int cachedFrequency3 = 0;
            static int cachedFrequency4 = 0;
            static bool voice1Playing = false;
            static bool voice2Playing = false;
            static bool voice3Playing = false;
            static bool voice4Playing = false;

            /// <summary>
            /// Starts audio synthesis on the specified voice channel using the given base frequency and current voice
            /// settings.
            /// </summary>
            /// <remarks>This method applies the current voice synthesis settings, including timbre,
            /// formant, and noise parameters, to the specified channel. If the channel is already active, its previous
            /// audio is stopped and replaced. Only one voice can be active per channel at a time. This method is not
            /// thread-safe and should be called from the main audio thread.</remarks>
            /// <param name="channelId">The zero-based index of the voice channel to start. Valid values are 0 through 3, each corresponding to
            /// a separate voice channel.</param>
            /// <param name="baseFrequency">The base frequency, in hertz, to use for the synthesized voice. Must be a positive integer.</param>
            public static void StartVoice(int channelId, int baseFrequency)
            {
                switch (channelId)
                {
                    case 0:
                        cachedFrequency1 = baseFrequency;
                        voice1Playing = true;
                        break;
                    case 1:
                        cachedFrequency2 = baseFrequency;
                        voice2Playing = true;
                        break;
                    case 2:
                        cachedFrequency3 = baseFrequency;
                        voice3Playing = true;
                        break;
                    case 3:
                        cachedFrequency4 = baseFrequency;
                        voice4Playing = true;
                        break;
                }
                const int sampleRate = 44100;

                double rawTimbre = TemporarySettings.VoiceInternalSettings.timbre;
                double rawRandomizedFrequencyRange = TemporarySettings.VoiceInternalSettings.randomizedFrequencyRange;

                // Apply random variations
                double randomVariation = (Random.Shared.NextDouble() - 0.5) * 2.0 * rawRandomizedFrequencyRange * 16;

                double finalPitchMultiplier = (1 + rawTimbre) * 0.25;

                double modulatedFrequency = ((baseFrequency * finalPitchMultiplier / 4)) + randomVariation;

                double masterVolume = VoiceInternalSettings.voiceVolume / 400.0;

                var (sineSource, triangleSource) = CreateSignalGenerators(modulatedFrequency, masterVolume);
                var mixingProvider = new MixingSampleProvider(new[] { sineSource, triangleSource });
                int renderSeconds = 1;
                WaveFormat wf = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
                int totalSamples = sampleRate * renderSeconds;
                float[] renderBuffer = new float[totalSamples];
                int read = 0;
                while (read < totalSamples)
                {
                    int r = mixingProvider.Read(renderBuffer, read, totalSamples - read);
                    if (r == 0) break;
                    read += r;
                }
                if (read < totalSamples) Array.Clear(renderBuffer, read, totalSamples - read);
                var cachedVoiced = new CachedSound(renderBuffer, wf);

                SignalGenerator noiseGen = new SignalGenerator() { Type = SignalGeneratorType.White, Frequency = 0, Gain = (masterVolume * (VoiceInternalSettings.noiseVolume / 100.0)) / 10 };
                float[] noiseBuffer = new float[totalSamples];
                read = 0;
                while (read < totalSamples)
                {
                    int r = noiseGen.Read(noiseBuffer, read, totalSamples - read);
                    if (r == 0) break;
                    read += r;
                }
                if (read < totalSamples) Array.Clear(noiseBuffer, read, totalSamples - read);
                var cachedNoise = new CachedSound(noiseBuffer, wf);

                float BaseFormantQ(int bf) => bf < 2000 ? 2.0f : 1.0f;
                BiQuadFilter MakeBP(int sr, double center, float q) => BiQuadFilter.BandPassFilterConstantPeakGain(sr, (float)center, q);

                double[] formantFreqs = new double[] {
        VoiceInternalSettings.formant1Frequency,
        VoiceInternalSettings.formant2Frequency,
        VoiceInternalSettings.formant3Frequency,
        VoiceInternalSettings.formant4Frequency
    };
                double[] formantVols = new double[] {
        VoiceInternalSettings.formant1Volume / 100.0,
        VoiceInternalSettings.formant2Volume / 100.0,
        VoiceInternalSettings.formant3Volume / 100.0,
        VoiceInternalSettings.formant4Volume / 100.0
    };
                double noiseToFormantScale = VoiceInternalSettings.noiseVolume / 100.0 * (VoiceInternalSettings.noiseVolume > 0 ? 1.0 : 0.0);

                var lowPass = BiQuadFilter.LowPassFilter(sampleRate, VoiceInternalSettings.cutoffFrequency, 1.0f);

                double syb1Vol = VoiceInternalSettings.sybillance1Volume * 0.18;
                double syb2Vol = VoiceInternalSettings.sybillance2Volume * 0.15;
                double syb3Vol = VoiceInternalSettings.sybillance3Volume * 0.12;
                double syb4Vol = VoiceInternalSettings.sybillance4Volume * 0.10;

                var providers = new List<ISampleProvider>();

                for (int i = 0; i < 4; i++)
                {
                    double fCenter = formantFreqs[i];
                    double fVol = formantVols[i];
                    float dynamicQ = (float)(0.5f + (i * 0.3f)) * BaseFormantQ(baseFrequency) * 2.4f;
                    var filter = MakeBP(sampleRate, fCenter, dynamicQ);
                    var voicedReader = new CachedSoundSampleProvider(cachedVoiced, loop: true);
                    providers.Add(new FilteredWaveProvider(voicedReader, filter, Math.Min(fVol * 1.0, 2.0)));

                    var noiseFilter = MakeBP(sampleRate, fCenter, dynamicQ * 1.1f);
                    var noiseReader = new CachedSoundSampleProvider(cachedNoise, loop: true);
                    providers.Add(new FilteredWaveProvider(noiseReader, noiseFilter, noiseToFormantScale * fVol * 1.2));
                }

                var sybillanceFilter1 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.sybillance1Frequency, (float)VoiceInternalSettings.sybillance1Range * 1.5f);
                var sybillanceFilter2 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.sybillance2Frequency, (float)VoiceInternalSettings.sybillance2Range * 1.5f);
                var sybillanceFilter3 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.sybillance3Frequency, (float)VoiceInternalSettings.sybillance3Range * 1.5f);
                var sybillanceFilter4 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.sybillance4Frequency, (float)VoiceInternalSettings.sybillance4Range * 1.5f);

                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sybillanceFilter1, syb1Vol));
                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sybillanceFilter2, syb2Vol));
                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sybillanceFilter3, syb3Vol));
                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sybillanceFilter4, syb4Vol));

                var mixed = new MixingSampleProvider(providers) { ReadFully = true };
                var finalFiltered = new FilteredWaveProvider(mixed, lowPass, 1.0);
                var volumeControlled = new VolumeSampleProvider(finalFiltered) { Volume = 1.0f };

                lock (synthLock)
                {
                    if (channels.TryGetValue(channelId, out var existing))
                    {
                        try
                        {
                            masterMixer.RemoveMixerInput(existing.removable);
                        }
                        catch
                        {
                            // Swallow exceptions to ensure safe removal
                        }
                        existing.removable.Remove();
                        channels.Remove(channelId);
                    }

                    // Take formant providers to list
                    var formantProviders = providers.Take(8).OfType<FilteredWaveProvider>().ToList();

                    var removable = new RemovableSampleProvider(volumeControlled);
                    masterMixer.AddMixerInput(removable);
                    channels[channelId] = (removable, volumeControlled, finalFiltered, sineSource, triangleSource, formantFreqs, formantVols, lowPass, formantProviders);
                }
            }

            /// <summary>
            /// Stops audio playback on the specified voice channel and releases associated resources.
            /// </summary>
            /// <remarks>If the specified channel is not currently active, this method has no effect.
            /// This method is thread-safe and can be called concurrently from multiple threads.</remarks>
            /// <param name="channelId">The identifier of the voice channel to stop. Must correspond to an active channel; otherwise, no action
            /// is taken.</param>
            public static void StopVoice(int channelId)
            {
                switch (channelId)
                {
                    case 0:
                        cachedFrequency1 = 0;
                        voice1Playing = false;
                        break;
                    case 1:
                        cachedFrequency1 = 0;
                        voice1Playing = false;
                        break;
                    case 2:
                        cachedFrequency1 = 0;
                        voice1Playing = false;
                        break;
                    case 3:
                        cachedFrequency1 = 0;
                        voice1Playing = false;
                        break;
                }
                lock (synthLock)
                {
                    if (channels.TryGetValue(channelId, out var tuple))
                    {
                        try
                        {
                            masterMixer.RemoveMixerInput(tuple.removable);
                        }
                        catch
                        {
                            // Swallow exceptions to ensure safe removal
                        }
                        tuple.removable.Remove();
                        channels.Remove(channelId);
                    }
                }
            }
        }
    }
}