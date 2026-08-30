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
using System.Diagnostics;
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

            static PawnIOWrapper pawnIO;
            static SystemSpeakerBeepEngine()
            {
                /*var sysspkrModuleBytes = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PcSpeaker.bin"));
                pawnIO = new PawnIOWrapper(sysspkrModuleBytes);*/
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
                    int offset = 0; // Optional offset before enabling the speaker, can be adjusted if needed
                    offset = (nonStopping == true ? 0 : 5);
                    StartBeep(freq, offset);
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
            /// Opens the system speaker (aka PC speaker) gate to start producing a beep sound at the specified frequency.
            /// </summary>
            /// <remarks>
            /// On platforms where the system speaker is not present or not supported (such as most ARM64-based devices), this method performs no operation. The frequency may be adjusted internally to avoid potential resonance issues with certain storage devices.
            /// </remarks>
            /// <param name="freq"></param>
            /// <param name="offset"></param>
            public static void StartBeep(int freq, int offset = 0)
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    // Philosophical problem: How do you start a beep that doesn't exist in most of Copilot+ devices?
                    return; // No operation on ARM64 devices such as most of Copilot+ devices, as system speaker access is not supported
                }
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
                if (offset > 0)
                {
                    HighPrecisionSleep.Sleep(offset); // Optional offset before enabling the speaker, if specified
                }
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03)); // Open the gate of the system speaker to start the beep
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

            private const short PortSpeakerControl = 0x61;
            private const short PortPitControl = 0x43;
            private const short PortPitChannel2 = 0x42;

            // Very short probe to minimize noticeability.
            // Still not guaranteed to be inaudible on every machine.
            private const int ProbeFrequencyHz = 9000;
            private const int ProbeDurationMicroseconds = 200;

            private static readonly Mutex SystemSpeakerMutex = new Mutex(false, "Global\\NeoBleeperSystemSpeakerMutex"); // Mutex to prevent concurrent access to system speaker checks

            /// <summary>
            /// Determines whether the system speaker is present and functioning correctly.
            /// </summary>
            /// <remarks>This method performs a series of hardware-level checks to verify the presence
            /// and operability of the system speaker. It attempts to acquire a mutex to ensure thread safety during the
            /// check. If the mutex cannot be acquired within the timeout period, the method returns false. This method
            /// is intended for use in environments where direct hardware access is permitted and may not be suitable
            /// for all platforms.</remarks>
            /// <returns>true if the system speaker is detected and passes all functional checks; otherwise, false.</returns>
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
                    if (!TryReadPort61(out _))
                        return false;

                    if (!CheckControlPortRoundTrip(out _))
                        return false;

                    if (!CheckPitChannel2Bit5Activity(ProbeFrequencyHz, 64, out _))
                        return false;

                    if (!TryMinimalAudibleProbe(ProbeFrequencyHz, ProbeDurationMicroseconds))
                        return false;

                    return true;
                }
                finally
                {
                    if (acquired)
                    {
                        SystemSpeakerMutex.ReleaseMutex();
                    }
                }
            }

            private static bool TryReadPort61(out byte value)
            {
                try
                {
                    value = ReadPortByte(PortSpeakerControl);
                    return true;
                }
                catch (Exception ex)
                {
                    value = 0;
                    return false;
                }
            }

            private static bool CheckControlPortRoundTrip(out string details)
            {
                byte originalState = 0;

                try
                {
                    originalState = ReadPortByte(PortSpeakerControl);

                    byte safeBase = (byte)(originalState & ~0x02);
                    byte variantA = (byte)(safeBase & ~0x01);
                    byte variantB = (byte)(safeBase | 0x01);

                    WritePortByte(PortSpeakerControl, variantA);
                    BusyWaitMicroseconds(500); // Thread.Sleep(1) yerine
                    byte readA = ReadPortByte(PortSpeakerControl);

                    WritePortByte(PortSpeakerControl, variantB);
                    BusyWaitMicroseconds(500); // Thread.Sleep(1) yerine
                    byte readB = ReadPortByte(PortSpeakerControl);

                    RestoreSpeakerControl(originalState);

                    bool aMatched = ((readA & 0x01) == (variantA & 0x01)) && ((readA & 0x02) == 0);
                    bool bMatched = ((readB & 0x01) == (variantB & 0x01)) && ((readB & 0x02) == 0);

                    details =
                        "Control-port round trip:\n" +
                        $"  Original: 0x{originalState:X2}\n" +
                        $"  WriteA  : 0x{variantA:X2} -> ReadA: 0x{readA:X2}\n" +
                        $"  WriteB  : 0x{variantB:X2} -> ReadB: 0x{readB:X2}\n" +
                        $"  Result  : {(aMatched && bMatched ? "PASS" : "FAIL")}";

                    return aMatched && bMatched;
                }
                catch (Exception ex)
                {
                    RestoreSpeakerControl(originalState);
                    details = $"Control-port round trip failed: {ex.Message}";
                    return false;
                }
            }

            private static bool CheckPitChannel2Bit5Activity(
                int frequencyHz,
                int sampleCount,
                out int transitions)
            {
                byte original61 = 0;

                try
                {
                    original61 = ReadPortByte(PortSpeakerControl);

                    int divisor = 1193182 / Math.Max(1, frequencyHz);
                    if (divisor < 1) divisor = 1;
                    if (divisor > 65535) divisor = 65535;

                    // Program PIT channel 2, mode 3 square wave, lobyte/hibyte.
                    WritePortByte(PortPitControl, 0xB6);
                    WritePortByte(PortPitChannel2, (byte)(divisor & 0xFF));
                    WritePortByte(PortPitChannel2, (byte)((divisor >> 8) & 0xFF));

                    // Gate timer 2 ON, keep speaker data OFF.
                    byte gated = (byte)((original61 | 0x01) & ~0x02);
                    WritePortByte(PortSpeakerControl, gated);
                    BusyWaitMicroseconds(500); // Thread.Sleep(1) yerine

                    transitions = 0;
                    bool? prev = null;

                    for (int i = 0; i < sampleCount; i++)
                    {
                        byte sample = ReadPortByte(PortSpeakerControl);
                        bool bit5 = (sample & 0x20) != 0;

                        if (prev.HasValue && prev.Value != bit5)
                            transitions++;

                        prev = bit5;

                        Thread.Sleep(0);
                    }

                    RestoreSpeakerControl(original61);

                    bool ok = transitions > 0;
                    return ok;
                }
                catch (Exception ex)
                {
                    RestoreSpeakerControl(original61);
                    transitions = 0;
                    return false;
                }
            }

            private static bool TryMinimalAudibleProbe(
            int frequencyHz,
            int durationUs)
            {
                byte original61 = 0;

                try
                {
                    original61 = ReadPortByte(PortSpeakerControl);

                    int divisor = 1193182 / Math.Max(1, frequencyHz);
                    if (divisor < 1) divisor = 1;
                    if (divisor > 65535) divisor = 65535;

                    // Program PIT channel 2.
                    WritePortByte(PortPitControl, 0xB6);
                    WritePortByte(PortPitChannel2, (byte)(divisor & 0xFF));
                    WritePortByte(PortPitChannel2, (byte)((divisor >> 8) & 0xFF));

                    // Enable gate + speaker only for a tiny duration.
                    byte onState = (byte)(original61 | 0x03);
                    WritePortByte(PortSpeakerControl, onState);

                    BusyWaitMicroseconds(durationUs); // mikro-saniye ile bekle

                    RestoreSpeakerControl(original61);

                    return true;
                }
                catch (Exception ex)
                {
                    RestoreSpeakerControl(original61);
                    return false;
                }
            }

            private static void BusyWaitMicroseconds(int microseconds)
            {
                if (microseconds <= 0)
                    return;

                long start = Stopwatch.GetTimestamp();
                long ticksToWait = (long)(microseconds * (Stopwatch.Frequency / 1_000_000.0));
                long target = start + Math.Max(1, ticksToWait);

                // Hafifçe spin-wait, CPU'yu tamamen kilitlememek için arada SpinWait yap
                while (Stopwatch.GetTimestamp() < target)
                    Thread.SpinWait(10);
            }


            private static byte ReadPortByte(short port)
            {
                return unchecked((byte)(Inp32(port) & 0xFF));
            }

            private static void WritePortByte(short port, byte value)
            {
                Out32(port, value);
            }

            private static void RestoreSpeakerControl(byte originalState)
            {
                try
                {
                    WritePortByte(PortSpeakerControl, originalState);
                    BusyWaitMicroseconds(200); // Thread.Sleep(1) yerine kısa bekleme
                }
                catch
                {
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
            public static bool IsPawnIOInstalled()
            {
                try
                {
                    var pawnioPath = Environment.GetEnvironmentVariable("PAWNIO_ROOT");
                    if (!string.IsNullOrEmpty(pawnioPath)) // Check if PawnIO is installed
                    {
                        try
                        {
                            if (!File.Exists(Path.Combine(pawnioPath, "PawnIOLib.dll")))
                            {
                                return false; // DLL not found in the specified path
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    return false;
                }
                catch
                {
                }
                return false; // Placeholder implementation, as PawnIO is not relevant in this context
            }
            private static void StartBeepPawnIO(int frequency)
            {
                pawnIO.Execute("ioctl_start", new long[] { frequency }, 0);
            }

            private static void StopBeepPawnIO()
            {
                pawnIO.Execute("ioctl_stop", Array.Empty<long>(), 0);
            }

            public static class PCBeepSliderChecker
            {
                private const uint CLSCTX_INPROC_SERVER = 0x1;

                private static readonly Guid CLSID_MMDeviceEnumerator =
                    new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");

                private static readonly Guid IID_IDeviceTopology =
                    new Guid("2A07407E-6497-4A18-9787-32F79BD0D98F");

                private enum EDataFlow
                {
                    eRender = 0,
                    eCapture = 1,
                    eAll = 2
                }

                private enum ERole
                {
                    eConsole = 0,
                    eMultimedia = 1,
                    eCommunications = 2
                }

                private enum PartType
                {
                    Connector = 0,
                    Subunit = 1
                }

                private enum ConnectorType
                {
                    Unknown_Connector = 0,
                    Physical_Internal = 1,
                    Physical_External = 2,
                    Software_IO = 3,
                    Software_Fixed = 4,
                    Network = 5
                }

                private enum DataFlow
                {
                    In = 0,
                    Out = 1
                }

                [ComImport]
                [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                private interface IMMDeviceEnumerator
                {
                    [PreserveSig]
                    int EnumAudioEndpoints(
                        EDataFlow dataFlow,
                        uint dwStateMask,
                        out IntPtr ppDevices);

                    [PreserveSig]
                    int GetDefaultAudioEndpoint(
                        EDataFlow dataFlow,
                        ERole role,
                        out IMMDevice ppEndpoint);

                    [PreserveSig]
                    int GetDevice(
                        [MarshalAs(UnmanagedType.LPWStr)] string pwstrId,
                        out IMMDevice ppDevice);

                    [PreserveSig]
                    int RegisterEndpointNotificationCallback(IntPtr pClient);

                    [PreserveSig]
                    int UnregisterEndpointNotificationCallback(IntPtr pClient);
                }

                [ComImport]
                [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                private interface IMMDevice
                {
                    [PreserveSig]
                    int Activate(
                        ref Guid iid,
                        uint dwClsCtx,
                        IntPtr pActivationParams,
                        [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

                    [PreserveSig]
                    int OpenPropertyStore(
                        uint stgmAccess,
                        out IntPtr ppProperties);

                    [PreserveSig]
                    int GetId(
                        [MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

                    [PreserveSig]
                    int GetState(out uint pdwState);
                }

                [ComImport]
                [Guid("2A07407E-6497-4A18-9787-32F79BD0D98F")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                private interface IDeviceTopology
                {
                    [PreserveSig]
                    int GetConnectorCount(out uint pCount);

                    [PreserveSig]
                    int GetConnector(
                        uint nIndex,
                        out IConnector ppConnector);

                    [PreserveSig]
                    int GetSubunitCount(out uint pCount);

                    [PreserveSig]
                    int GetSubunit(
                        uint nIndex,
                        out ISubunit ppSubunit);

                    [PreserveSig]
                    int GetPartById(
                        uint nId,
                        out IPart ppPart);

                    [PreserveSig]
                    int GetDeviceId(
                        [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrDeviceId);

                    [PreserveSig]
                    int GetSignalPath(
                        IPart pIPartFrom,
                        IPart pIPartTo,
                        [MarshalAs(UnmanagedType.Bool)] bool bRejectMixedPaths,
                        out IntPtr ppParts);
                }

                [ComImport]
                [Guid("9C2C4058-23F5-41DE-877A-DF3AF236A09E")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                private interface IConnector
                {
                    [PreserveSig]
                    int GetType(out ConnectorType pType);

                    [PreserveSig]
                    int GetDataFlow(out DataFlow pFlow);

                    [PreserveSig]
                    int ConnectTo(IConnector pConnectTo);

                    [PreserveSig]
                    int Disconnect();

                    [PreserveSig]
                    int IsConnected(
                        [MarshalAs(UnmanagedType.Bool)] out bool pbConnected);

                    [PreserveSig]
                    int GetConnectedTo(out IConnector ppConTo);

                    [PreserveSig]
                    int GetConnectorIdConnectedTo(
                        [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrConnectorId);

                    [PreserveSig]
                    int GetDeviceIdConnectedTo(
                        [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrDeviceId);
                }

                [ComImport]
                [Guid("82149A85-DBA6-4487-86BB-EA8F7FEFCC71")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                private interface ISubunit
                {
                }

                [ComImport]
                [Guid("AE2DE0E4-5BCA-4F2D-AA46-5D13F8FDB3A9")]
                [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
                private interface IPart
                {
                    [PreserveSig]
                    int GetName(
                        [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrName);

                    [PreserveSig]
                    int GetLocalId(out uint pnId);

                    [PreserveSig]
                    int GetGlobalId(
                        [MarshalAs(UnmanagedType.LPWStr)] out string ppwstrGlobalId);

                    [PreserveSig]
                    int GetPartType(out PartType pPartType);

                    [PreserveSig]
                    int GetSubType(out Guid pSubType);

                    [PreserveSig]
                    int GetControlInterfaceCount(out uint pCount);

                    [PreserveSig]
                    int GetControlInterface(
                        uint nIndex,
                        out IntPtr ppInterfaceDesc);

                    [PreserveSig]
                    int EnumPartsIncoming(out IntPtr ppParts);

                    [PreserveSig]
                    int EnumPartsOutgoing(out IntPtr ppParts);

                    [PreserveSig]
                    int GetTopologyObject(
                        out IDeviceTopology ppTopology);

                    [PreserveSig]
                    int Activate(
                        uint dwClsContext,
                        ref Guid refiid,
                        [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

                    [PreserveSig]
                    int RegisterControlChangeCallback(
                        ref Guid riid,
                        IntPtr pNotify);

                    [PreserveSig]
                    int UnregisterControlChangeCallback(IntPtr pNotify);
                }

                /// <summary>
                /// Returns true if the default playback device hardware topology
                /// contains a node named "PC Beep" or "PC Speaker".
                /// </summary>
                public static bool HasPcBeepOrPcSpeaker()
                {
                    Type type = Type.GetTypeFromCLSID(CLSID_MMDeviceEnumerator);

                    IMMDeviceEnumerator enumerator =
                        (IMMDeviceEnumerator)Activator.CreateInstance(type);

                    IMMDevice endpoint;

                    int hr = enumerator.GetDefaultAudioEndpoint(
                        EDataFlow.eRender,
                        ERole.eMultimedia,
                        out endpoint);

                    if (hr != 0 || endpoint == null)
                        return false;

                    object topologyObject;
                    Guid iid = IID_IDeviceTopology;

                    hr = endpoint.Activate(
                        ref iid,
                        CLSCTX_INPROC_SERVER,
                        IntPtr.Zero,
                        out topologyObject);

                    if (hr != 0 || topologyObject == null)
                        return false;

                    IDeviceTopology endpointTopology =
                        (IDeviceTopology)topologyObject;

                    uint connectorCount;

                    hr = endpointTopology.GetConnectorCount(
                        out connectorCount);

                    if (hr != 0)
                        return false;

                    for (uint connectorIndex = 0;
                         connectorIndex < connectorCount;
                         connectorIndex++)
                    {
                        IConnector endpointConnector;

                        hr = endpointTopology.GetConnector(
                            connectorIndex,
                            out endpointConnector);

                        if (hr != 0 || endpointConnector == null)
                            continue;

                        IConnector hardwareConnector;

                        hr = endpointConnector.GetConnectedTo(
                            out hardwareConnector);

                        if (hr != 0 || hardwareConnector == null)
                            continue;

                        IPart hardwarePart;

                        try
                        {
                            hardwarePart = (IPart)hardwareConnector;
                        }
                        catch
                        {
                            continue;
                        }

                        IDeviceTopology hardwareTopology;

                        hr = hardwarePart.GetTopologyObject(
                            out hardwareTopology);

                        if (hr != 0 || hardwareTopology == null)
                            continue;

                        if (TopologyContainsPcBeep(hardwareTopology))
                            return true;
                    }

                    return false;
                }

                private static bool TopologyContainsPcBeep(
                    IDeviceTopology topology)
                {
                    uint count;

                    int hr = topology.GetSubunitCount(out count);

                    if (hr != 0)
                        return false;

                    for (uint i = 0; i < count; i++)
                    {
                        ISubunit subunit;

                        hr = topology.GetSubunit(
                            i,
                            out subunit);

                        if (hr != 0 || subunit == null)
                            continue;

                        IPart part;

                        try
                        {
                            part = (IPart)subunit;
                        }
                        catch
                        {
                            continue;
                        }

                        string name;

                        hr = part.GetName(out name);

                        if (hr != 0 ||
                            String.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        // Useful while testing NeoBleeper:
                        Console.WriteLine(
                            "Audio topology node: " + name);

                        if (name.IndexOf(
                                "PC Beep",
                                StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }

                        if (name.IndexOf(
                                "PC Speaker",
                                StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }
        public static class WaveSynthEngine // Synthesize various waveforms of beeps and noises by emulating FMOD, that is used in Bleeper Music Maker, using NAudio
        {
            /// <summary>
            /// Passes ordinary beeps through with the original direct SignalGenerator.Gain
            /// behavior. Only a rapid sequence of gate edges is replayed sample-accurately
            /// inside Read, so the tight-loop pulse noise is not reduced to a faint beep.
            /// </summary>
            public sealed class RapidGateSampleProvider : ISampleProvider
            {
                private const double OpenGain = 0.15;
                private const int RapidEdgeCount = 2;

                private static readonly long RapidEdgeTicks =
                    Math.Max(1L, Stopwatch.Frequency * 2L / 1000L);
                private static readonly long RapidResetTicks =
                    Math.Max(1L, Stopwatch.Frequency * 10L / 1000L);

                private readonly SignalGenerator source;
                private readonly object gateLock = new object();
                private readonly System.Collections.Generic.Queue<GateCommand> commands =
                    new System.Collections.Generic.Queue<GateCommand>();
                private readonly System.Collections.Generic.List<GateCommand> rapidCandidate =
                    new System.Collections.Generic.List<GateCommand>(RapidEdgeCount);
                private readonly System.Collections.Generic.List<RenderedGateCommand> dueCommands =
                    new System.Collections.Generic.List<RenderedGateCommand>(64);

                private long renderedFrames;
                private long clockOriginTimestamp;
                private long clockOriginFrame;
                private long scheduleDelayFrames;
                private long lastTransitionTimestamp;
                private bool clockStarted;
                private volatile bool preciseGateMode;

                private int candidateStartState;
                private int renderedGateState;
                private volatile int requestedGateState;

                // --- Dual-Engine State ---
                // 1. Tone Generator Ramp (Smooth start/stop for full tones)
                private float toneGainLevel = 0.0f;
                private readonly float slewStepPerSample;

                // 2. 1-Bit Click Impulse Engine (Punchy, loud transient spikes for 1-bit clicks)
                private float clickImpulseLevel = 0.0f;
                private const float ClickPeakAmplitude = 0.35f; // Loud, crisp click volume
                private const float ClickDecayRate = 0.65f;     // ~0.2ms decay per click edge

                private readonly struct GateCommand
                {
                    public GateCommand(long timestamp, int state)
                    {
                        Timestamp = timestamp;
                        State = state;
                    }

                    public long Timestamp { get; }
                    public int State { get; }
                }

                private readonly struct RenderedGateCommand
                {
                    public RenderedGateCommand(long frame, int state)
                    {
                        Frame = frame;
                        State = state;
                    }

                    public long Frame { get; }
                    public int State { get; }
                }

                public RapidGateSampleProvider(SignalGenerator source)
                {
                    this.source = source ?? throw new ArgumentNullException(nameof(source));

                    // Ramp rate for standard tone generator mode (~0.09 ms)
                    int slewSamples = Math.Max(2, (int)(source.WaveFormat.SampleRate * 0.00009));
                    slewStepPerSample = (float)(OpenGain / slewSamples);
                }

                public WaveFormat WaveFormat => source.WaveFormat;

                public bool IsMuted => requestedGateState == 0;

                public void SetDirectGate(bool open)
                {
                    int newState = open ? 1 : 0;

                    lock (gateLock)
                    {
                        CancelPreciseModeLocked();
                        requestedGateState = newState;
                        renderedGateState = newState;
                        source.Gain = open ? OpenGain : 0.0;
                    }
                }

                public void SetAdaptiveGate(bool open)
                {
                    int newState = open ? 1 : 0;
                    long timestamp = Stopwatch.GetTimestamp();

                    lock (gateLock)
                    {
                        if (requestedGateState == newState)
                        {
                            return;
                        }

                        int previousState = requestedGateState;

                        long edgeGap = lastTransitionTimestamp == 0
                            ? long.MaxValue
                            : timestamp - lastTransitionTimestamp;

                        requestedGateState = newState;

                        if (preciseGateMode)
                        {
                            if (edgeGap > RapidResetTicks)
                            {
                                LeavePreciseModeLocked(previousState);
                                ApplyDirectGainLocked(newState);
                                BeginCandidateLocked(previousState, timestamp, newState);
                            }
                            else
                            {
                                commands.Enqueue(new GateCommand(timestamp, newState));
                                lastTransitionTimestamp = timestamp;
                            }

                            return;
                        }

                        ApplyDirectGainLocked(newState);

                        if (edgeGap > RapidEdgeTicks)
                        {
                            BeginCandidateLocked(previousState, timestamp, newState);
                        }
                        else
                        {
                            if (rapidCandidate.Count == 0)
                            {
                                candidateStartState = previousState;
                            }

                            rapidCandidate.Add(new GateCommand(timestamp, newState));
                            lastTransitionTimestamp = timestamp;
                        }

                        if (rapidCandidate.Count >= RapidEdgeCount)
                        {
                            EnterPreciseModeLocked();
                        }
                    }
                }

                public void ResetClosed()
                {
                    lock (gateLock)
                    {
                        CancelPreciseModeLocked();
                        requestedGateState = 0;
                        renderedGateState = 0;
                        source.Gain = 0.0;
                        renderedFrames = 0;
                        toneGainLevel = 0.0f;
                        clickImpulseLevel = 0.0f;
                    }
                }

                public int Read(float[] buffer, int offset, int count)
                {
                    int read = source.Read(buffer, offset, count);

                    int channels = WaveFormat.Channels;
                    int frameCount = read / channels;

                    if (frameCount <= 0)
                    {
                        return read;
                    }

                    long firstFrame = renderedFrames;
                    long frameAfterBuffer = firstFrame + frameCount;
                    dueCommands.Clear();

                    if (preciseGateMode)
                    {
                        lock (gateLock)
                        {
                            if (preciseGateMode)
                            {
                                if (!clockStarted && commands.Count != 0)
                                {
                                    GateCommand first = commands.Peek();
                                    clockOriginTimestamp = first.Timestamp;
                                    clockOriginFrame = firstFrame;
                                    scheduleDelayFrames = 0;
                                    clockStarted = true;
                                }

                                while (clockStarted && commands.Count != 0)
                                {
                                    GateCommand command = commands.Peek();
                                    long relativeFrames = TimestampDeltaToFrames(
                                        command.Timestamp - clockOriginTimestamp);
                                    long targetFrame = clockOriginFrame + relativeFrames + scheduleDelayFrames;

                                    if (targetFrame < firstFrame)
                                    {
                                        scheduleDelayFrames += firstFrame - targetFrame;
                                        targetFrame = firstFrame;
                                    }

                                    if (targetFrame >= frameAfterBuffer)
                                    {
                                        break;
                                    }

                                    commands.Dequeue();
                                    dueCommands.Add(new RenderedGateCommand(targetFrame, command.State));
                                }
                            }
                        }
                    }

                    int gateState = renderedGateState;
                    int commandIndex = 0;

                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        long absoluteFrame = firstFrame + frame;
                        int previousState = gateState;

                        if (dueCommands.Count > 0)
                        {
                            while (commandIndex < dueCommands.Count &&
                                   dueCommands[commandIndex].Frame <= absoluteFrame)
                            {
                                gateState = dueCommands[commandIndex].State;
                                commandIndex++;
                            }
                        }

                        // Trigger a high-energy transient impulse on every 1-bit state transition edge
                        if (preciseGateMode && gateState != previousState)
                        {
                            clickImpulseLevel = (gateState > previousState)
                                ? ClickPeakAmplitude
                                : -ClickPeakAmplitude;
                        }

                        int sampleIndex = offset + frame * channels;
                        float targetGain = (gateState != 0) ? (float)OpenGain : 0.0f;

                        // Smooth tone gain calculation for non-precise mode
                        if (toneGainLevel < targetGain)
                        {
                            toneGainLevel = Math.Min(targetGain, toneGainLevel + slewStepPerSample);
                        }
                        else if (toneGainLevel > targetGain)
                        {
                            toneGainLevel = Math.Max(targetGain, toneGainLevel - slewStepPerSample);
                        }

                        // Sample rendering
                        for (int channel = 0; channel < channels; channel++)
                        {
                            if (preciseGateMode)
                            {
                                // 1-Bit PC Speaker mode: render loud, punchy impulse transient
                                buffer[sampleIndex + channel] = clickImpulseLevel;
                            }
                            else
                            {
                                // Standard tone generator mode: render clean, unmuffled audio wave
                                float gainScalar = (float)(OpenGain > 0 ? toneGainLevel / OpenGain : 0.0);
                                buffer[sampleIndex + channel] *= gainScalar;
                            }
                        }

                        // Exponential impulse decay (~0.2ms snap)
                        clickImpulseLevel *= ClickDecayRate;
                        if (Math.Abs(clickImpulseLevel) < 1e-5f) clickImpulseLevel = 0.0f;
                    }

                    renderedGateState = gateState;
                    renderedFrames = frameAfterBuffer;
                    return read;
                }

                private void BeginCandidateLocked(int stateBeforeEdge, long timestamp, int newState)
                {
                    rapidCandidate.Clear();
                    candidateStartState = stateBeforeEdge;
                    rapidCandidate.Add(new GateCommand(timestamp, newState));
                    lastTransitionTimestamp = timestamp;
                }

                private void EnterPreciseModeLocked()
                {
                    commands.Clear();

                    foreach (GateCommand command in rapidCandidate)
                    {
                        commands.Enqueue(command);
                    }

                    renderedGateState = candidateStartState;
                    rapidCandidate.Clear();
                    clockStarted = false;
                    scheduleDelayFrames = 0;

                    source.Gain = OpenGain;
                    preciseGateMode = true;
                }

                private void LeavePreciseModeLocked(int stableState)
                {
                    commands.Clear();
                    rapidCandidate.Clear();
                    clockStarted = false;
                    scheduleDelayFrames = 0;
                    preciseGateMode = false;
                    renderedGateState = stableState;
                    source.Gain = stableState != 0 ? OpenGain : 0.0;
                }

                private void CancelPreciseModeLocked()
                {
                    commands.Clear();
                    rapidCandidate.Clear();
                    clockStarted = false;
                    scheduleDelayFrames = 0;
                    lastTransitionTimestamp = 0;
                    preciseGateMode = false;
                }

                private void ApplyDirectGainLocked(int state)
                {
                    source.Gain = state != 0 ? OpenGain : 0.0;
                }

                private long TimestampDeltaToFrames(long timestampDelta)
                {
                    if (timestampDelta <= 0)
                    {
                        return 0;
                    }

                    return (long)Math.Round(
                        timestampDelta * (double)WaveFormat.SampleRate /
                        Stopwatch.Frequency);
                }
            }

            private static readonly object AudioLock = new object();
            public static readonly WaveOutEvent waveOut = new WaveOutEvent();

            // Regular beeps still gate signalGenerator.Gain directly. The wrapper only
            // masks samples after it recognizes the rapid loop-generated pulse pattern.
            private static readonly SignalGenerator signalGenerator =
                new SignalGenerator() { Gain = 0.0 }; // Continuous silent PIT carrier
            private static readonly RapidGateSampleProvider rapidSignalGate =
                new RapidGateSampleProvider(signalGenerator);

            private static readonly SignalGenerator whiteNoiseGenerator =
                new SignalGenerator() { Type = SignalGeneratorType.Pink, Gain = 0.0 };
            private static BandPassNoiseGenerator bandPassNoise;
            private static ISampleProvider currentProvider; // To keep track of the current provider

            private static volatile bool isWaveOutRunning = false;
            private static volatile bool isInitialized = false;

            // FMOD? That's a F-problem, so we use NAudio instead.

            static WaveSynthEngine()
            {
                // Safe static initialization: Do NOT call waveOut.Init or waveOut.Play here to prevent TypeInitializationException
                currentProvider = rapidSignalGate;
                waveOut.DesiredLatency = 1;
                waveOut.NumberOfBuffers = 35;
                waveOut.Volume = 1.0f; // Ensure volume is at max to prevent stuck muted sound
            }

            /// <summary>
            /// Safely lazy-initializes NAudio hardware streams on demand without risking static constructor crashes.
            /// </summary>
            private static void EnsureInitialized()
            {
                if (isInitialized) return;

                lock (AudioLock)
                {
                    if (!isInitialized)
                    {
                        try
                        {
                            waveOut.Init(currentProvider);
                            waveOut.Play();
                            isWaveOutRunning = true;
                            isInitialized = true;
                        }
                        catch (COMException) { }
                        catch (InvalidOperationException) { }
                    }
                }
            }

            /// <summary>
            /// Determines whether any enabled sound device is present on the system.
            /// </summary>
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
            private static void SetCurrentProvider(ISampleProvider provider)
            {
                EnsureInitialized();
                lock (AudioLock)
                {
                    if (currentProvider != provider)
                    {
                        if (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            waveOut.Stop();
                            isWaveOutRunning = false;
                        }

                        if (provider == rapidSignalGate)
                        {
                            rapidSignalGate.ResetClosed();
                        }

                        waveOut.Init(provider); // Restart the provider if only changed
                        waveOut.Play();
                        isWaveOutRunning = true;
                        currentProvider = provider;
                    }
                }
            }

            /// <summary>
            /// Plays a sound for the specified duration, with optional control over whether playback is stopped automatically.
            /// </summary>
            private static void PlaySound(int ms, bool nonStopping)
            {
                int offset = (nonStopping == true ? 0 : 5); // Add a small offset to ensure the sound starts before the sleep duration
                StartSynth(signalGenerator.Type, (int)signalGenerator.Frequency, offset);

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
            /// Determines whether all audio wave outputs are currently muted based on the active audio provider's gain settings.
            /// </summary>
            public static bool AreWavesMutedEarly()
            {
                if (currentProvider == rapidSignalGate)
                {
                    return rapidSignalGate.IsMuted;
                }
                else if (currentProvider == bandPassNoise)
                {
                    return whiteNoiseGenerator.Gain == 0;
                }
                return true; // If no provider is active, consider it muted
            }

            /// <summary>
            /// Stops audio synthesis. Regular beeps close the original direct gate;
            /// only an already-detected rapid pulse burst uses the precise queue.
            /// </summary>
            public static void StopSynth()
            {
                SetSignalGate(false);
                whiteNoiseGenerator.Gain = 0;
            }

            /// <summary>
            /// Plays a synthesized audio wave of the specified type, frequency, and duration.
            /// </summary>
            public static void PlayWave(SignalGeneratorType type, int freq, int ms, bool nonStopping)
            {
                EnsureInitialized();

                if (currentProvider != rapidSignalGate)
                {
                    SetCurrentProvider(rapidSignalGate);
                }

                if (signalGenerator.Frequency != freq)
                {
                    signalGenerator.Frequency = freq;
                }

                if (signalGenerator.Type != type)
                {
                    signalGenerator.Type = type;
                }

                // Normal row tones use the direct gate so an immediate OFF -> ON row
                // transition is never mistaken for tight-loop pulse modulation.
                rapidSignalGate.SetDirectGate(true);

                if (ms > 0)
                {
                    HighPrecisionSleep.Sleep(ms);
                }

                if (!nonStopping)
                {
                    rapidSignalGate.SetDirectGate(false);
                }
            }

            private static void SetSignalGate(bool open)
            {
                rapidSignalGate.SetAdaptiveGate(open);
            }

            private static void SetWaveTypeFrequencyAndVolume(SignalGeneratorType type, int freq)
            {
                EnsureInitialized();
                if (currentProvider != rapidSignalGate)
                {
                    SetCurrentProvider(rapidSignalGate);
                }

                if (signalGenerator.Frequency != freq) signalGenerator.Frequency = freq;
                if (signalGenerator.Type != type) signalGenerator.Type = type;
                SetSignalGate(true);
            }

            /// <summary>
            /// Starts audio synthesis. Isolated calls retain the original direct Gain
            /// behavior; adaptive calls are applied sample-accurately inside Read.
            /// </summary>
            public static void StartSynth(SignalGeneratorType type, int freq, int offset = 0)
            {
                EnsureInitialized();

                if (offset > 0)
                {
                    HighPrecisionSleep.Sleep(offset);
                }

                if (currentProvider == rapidSignalGate)
                {
                    if (signalGenerator.Frequency != freq) signalGenerator.Frequency = freq;
                    if (signalGenerator.Type != type) signalGenerator.Type = type;
                    SetSignalGate(true);
                }
                else
                {
                    lock (AudioLock)
                    {
                        SetCurrentProvider(rapidSignalGate);
                        if (signalGenerator.Frequency != freq) signalGenerator.Frequency = freq;
                        if (signalGenerator.Type != type) signalGenerator.Type = type;
                        SetSignalGate(true);
                    }
                }

                // Safety check to ensure background stream thread stays alive
                if (!isWaveOutRunning)
                {
                    lock (AudioLock)
                    {
                        if (!isWaveOutRunning)
                        {
                            try
                            {
                                waveOut.Play();
                                isWaveOutRunning = true;
                            }
                            catch { }
                        }
                    }
                }
            }

            /// <summary>
            /// Plays a band-pass filtered noise sound at the specified center frequency for a given duration.
            /// </summary>
            public static void PlayFilteredNoise(int freq, int ms, bool nonStopping)
            {
                EnsureInitialized();
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
                        whiteNoiseGenerator.Gain = 0.5; // Restore noise gate
                    }
                }
                PlaySound(ms, nonStopping);
            }

            public static void SquareWave(int freq, int ms, bool nonStopping)
            {
                PlayWave(SignalGeneratorType.Square, freq, ms, nonStopping);
            }

            public static void SineWave(int freq, int ms, bool nonStopping)
            {
                PlayWave(SignalGeneratorType.Sin, freq, ms, nonStopping);
            }

            public static void TriangleWave(int freq, int ms, bool nonStopping)
            {
                PlayWave(SignalGeneratorType.SawTooth, freq, ms, nonStopping);
            }

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
                    DesiredLatency = 1,
                    NumberOfBuffers = 35,
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
