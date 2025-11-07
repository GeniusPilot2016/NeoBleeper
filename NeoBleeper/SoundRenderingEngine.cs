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
            public static void SpecifyStorageType()
            {
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    try
                    {
                        Logger.Log("Specifying storage type for resonance prevention...", Logger.LogTypes.Info);
                        Program.splashScreen.updateStatus(Resources.StatusSpecifyingStorageType);
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
                                    Program.splashScreen.updateStatus(Resources.StatusNVMeStorageType, 5);
                                    return;
                                }
                                else if (interfaceType.Equals("SCSI", StringComparison.OrdinalIgnoreCase) || interfaceType.Equals("SATA", StringComparison.OrdinalIgnoreCase) || model.Contains("SSD", StringComparison.OrdinalIgnoreCase))
                                {
                                    StorageType = systemStorageType.SSD;
                                    resonanceFrequency = 0; // SSDs have no resonance issues
                                    Logger.Log("Detected SSD storage. Resonance prevention is not necessary.", Logger.LogTypes.Info);
                                    Program.splashScreen.updateStatus(Resources.StatusSSDStorageType, 5);
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
                                    Program.splashScreen.updateStatus(localizedResonanceMessage, 5);
                                    return;
                                }
                            }
                            StorageType = systemStorageType.Other; // If no known type is found
                            resonanceFrequency = 0; // Assume no resonance issues for unknown types
                            Logger.Log("Storage type is unknown. Resonance prevention is applied in probable resonant frequencies to be safe.", Logger.LogTypes.Warning);
                            Program.splashScreen.updateStatus(Resources.StatusUnknownStorageType, 5);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Error specifying storage type: " + ex.Message, Logger.LogTypes.Error);
                        Program.splashScreen.updateStatus("Error specifying storage type. Falling back to HDD settings.");
                        StorageType = systemStorageType.HDD; // Fallback to HDD on error
                    }
                }
                else
                {
                    Logger.Log("Storage type specification skipped on ARM64 architecture due to ARM64 doesn't support system speaker access.", Logger.LogTypes.Info);
                }
            }
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
                AppDomain.CurrentDomain.ProcessExit += (s, e) => {
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
            public static void StopBeep() // Stop the system speaker (aka PC speaker) from beeping
            {
                if(RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    // Philosophical problem: How do you stop a beep that doesn't exist in most of Copilot+ devices?
                    return; // No operation on ARM64 devices such as most of Copilot+ devices, as system speaker access is not supported
                }
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
            }
            public static bool isSystemSpeakerBeepStuck()
            {
                try
                {
                    if(RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
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

            private static void UltraSoftDisableSpeaker(byte originalState)
            {
                // Close speaker data (bit 1) first
                Out32(0x61, (byte)((originalState & 0xFE) | 0x01));
                Program.splashScreen.ResponsiveWait(10);

                // Then close the gate (bit 0)
                Out32(0x61, (byte)(originalState & 0xFC));
                Program.splashScreen.ResponsiveWait(10);
            }

            private static bool CheckElectricalFeedbackOnPort()
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

            private static bool CheckPortStateStability()
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
            private static bool AdvancedFrequencySweepTest()
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
            public static bool isSystemSpeakerExist()
            {
                // No system speaker, no problem.
                // Because it's falling back to sound card beep if no system speaker is found.

                // Step 1: Check for the presence of a system speaker device using WMI
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    try
                    {
                        Program.splashScreen.updateStatus(Resources.StatusSystemSpeakerSensorStep1, 10);
                        bool isSystemSpeakerPresentInWMI = false;
                        string query = "SELECT * FROM Win32_PNPEntity WHERE DeviceID LIKE '%PNP0800%'";
                        using (var searcher = new ManagementObjectSearcher(query))
                        {
                            var devices = searcher.Get();
                            isSystemSpeakerPresentInWMI = (devices.Count > 0);
                        }

                        // Step 2: Check for electrical feedback on port 0x61 to determine if the system speaker output is physically functional if WMI check is inconclusive
                        Program.splashScreen.updateStatus(Resources.StatusSystemSpeakerSensorStep2, 10);
                        bool isSystemSpeakerOutputPhysicallyFunctional = IsFunctionalSystemSpeaker();

                        // Return true if electrical feedback is detected or if WMI check confirms presence
                        bool result = isSystemSpeakerPresentInWMI || isSystemSpeakerOutputPhysicallyFunctional;
                        if (result == true)
                        {
                            Program.splashScreen.updateStatus(Resources.StatusSystemSpeakerOutputPresent);
                        }
                        else
                        {
                            Program.splashScreen.updateStatus(Resources.StatusSystemSpeakerOutputNotPresent);
                        }
                        Program.isExistenceOfSystemSpeakerChecked = true; // Mark that the check has been performed
                        return result;
                    }
                    catch (Exception ex)
                    {
                        Program.isExistenceOfSystemSpeakerChecked = false; // Mark that the check failed
                        Logger.Log("Error during system speaker detection: " + ex.Message, Logger.LogTypes.Error);
                        Program.splashScreen.updateStatus("Error during system speaker detection. Assuming no system speaker.");
                        return false; // On error, assume no system speaker
                    }
                }
                else
                {
                    Logger.Log("System speaker detection skipped on ARM64 architecture due to ARM64 doesn't support system speaker access.", Logger.LogTypes.Info);
                    return false; // ARM64 devices such as most of Copilot+ devices do not support system speaker access
                }
            }
            public static bool checkMotherboardAffectedFromSystemSpeakerIssues() // Check if the motherboard is from a manufacturer known to have system speaker issues
            // Added according M084MM3D's report states that "i have a PRIME H610M-A WIFI, and the bleeper beeps but in a very bad way, like the beep doesnt hold and it sounds like noise"
            // and some software-based beep issue, such as Linux's Beep command, reports on ASUS motherboards in various forums and operating systems
            {
                if(RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    try
                    {
                        Program.splashScreen.updateStatus(Resources.StatusCheckingMotherboardManufacturerForSystemSpeakerIssues);
                        string[] affectedManufacturers = new string[] { "ASUSTek Computer Inc." }; // Such as ASUS motherboards
                        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                        var boards = searcher.Get();
                        foreach (ManagementObject board in boards)
                        {
                            string manufacturer = board["Manufacturer"]?.ToString() ?? "";
                            if (affectedManufacturers.Contains(manufacturer))
                            {
                                string message = Resources.StatusManufacturerHasKnownIssues.Replace("{manufacturer}", manufacturer);
                                Program.splashScreen.updateStatus(message, 5);
                                Program.isAffectedMotherboardManufacturerChecked = true; // Mark that the motherboard is affected
                                return true; // Return true if the manufacturer is in the affected list
                            }
                        }
                        Program.splashScreen.updateStatus(Resources.StatusManufacturerIsNotAffected, 5);
                        Program.isAffectedMotherboardManufacturerChecked = true; // Mark that the check has been performed
                        return false; // Return false if no match is found
                    }
                    catch
                    {
                        Program.splashScreen.updateStatus(Resources.StatusErrorCheckingManufacturer);
                        Program.isAffectedMotherboardManufacturerChecked = false; // Mark that the check failed
                        return false; // On error, assume not affected
                    }
                }
                else
                {
                    Logger.Log("Motherboard manufacturer check skipped on ARM64 architecture due to ARM64 doesn't support system speaker access.", Logger.LogTypes.Info);
                    return false; // ARM64 devices such as most of Copilot+ devices do not support system speaker access
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
            public static bool checkIfAnySoundDeviceExistAndEnabled()
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
                PlayWave(SignalGeneratorType.Triangle, freq, ms, nonStopping);
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
            public void UpdateGain(double newGain)
            {
                gain = newGain;
            }
            public WaveFormat WaveFormat => source.WaveFormat;
            public void UpdateFilter(BiQuadFilter newFilter)
            {
                filter = newFilter;
            }
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
            private static (SignalGenerator sineSource, SignalGenerator triangleSource) CreateSignalGenerators(double modulatedFrequency, double masterVolume)
            {
                var wf = masterMixer.WaveFormat;
                SignalGenerator sineSource = new SignalGenerator(wf.SampleRate, wf.Channels)
                {
                    Type = SignalGeneratorType.Sin,
                    Frequency = modulatedFrequency,
                    Gain = masterVolume * (VoiceInternalSettings.SawVolume / 1000.0) * 0.3
                };

                SignalGenerator triangleSource = new SignalGenerator(wf.SampleRate, wf.Channels)
                {
                    Type = SignalGeneratorType.SawTooth,
                    Frequency = modulatedFrequency,
                    Gain = masterVolume * (VoiceInternalSettings.SawVolume / 1000.0) * 1.2
                };

                return (sineSource, triangleSource);
            }
            public static void ChangeValues(int channelId, int baseFrequency)
            {
                lock (synthLock)
                {
                    if (channels.TryGetValue(channelId, out var tuple))
                    {
                        const int sampleRate = 44100;

                        double rawTimbre = TemporarySettings.VoiceInternalSettings.Timbre;
                        double rawRandomizedFrequencyRange = TemporarySettings.VoiceInternalSettings.RandomizedFrequencyRange;

                        double randomVariation = (Random.Shared.NextDouble() - 0.5) * 2.0 * rawRandomizedFrequencyRange * 16;
                        double finalPitchMultiplier = (1 + rawTimbre) * 0.25;
                        double modulatedFrequency = ((baseFrequency * finalPitchMultiplier) / 4) + randomVariation;

                        // Update frequency of signal generators
                        tuple.sineSource.Frequency = modulatedFrequency;
                        tuple.triangleSource.Frequency = modulatedFrequency;

                        // Take formant frequencies and volumes
                        double[] currentFormantFreqs = new double[] {
                VoiceInternalSettings.Formant1Frequency,
                VoiceInternalSettings.Formant2Frequency,
                VoiceInternalSettings.Formant3Frequency,
                VoiceInternalSettings.Formant4Frequency
            };

                        double[] currentFormantVols = new double[] {
                VoiceInternalSettings.Formant1Volume / 100.0,
                VoiceInternalSettings.Formant2Volume / 100.0,
                VoiceInternalSettings.Formant3Volume / 100.0,
                VoiceInternalSettings.Formant4Volume / 100.0
            };

                        double noiseToFormantScale = VoiceInternalSettings.NoiseVolume / 100.0 * (VoiceInternalSettings.NoiseVolume > 0 ? 1.0 : 0.0);
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
                        var newLowPass = BiQuadFilter.LowPassFilter(sampleRate, VoiceInternalSettings.CutoffFrequency, 1.0f);
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

                double rawTimbre = TemporarySettings.VoiceInternalSettings.Timbre;
                double rawRandomizedFrequencyRange = TemporarySettings.VoiceInternalSettings.RandomizedFrequencyRange;

                // Apply random variations
                double randomVariation = (Random.Shared.NextDouble() - 0.5) * 2.0 * rawRandomizedFrequencyRange * 16;

                double finalPitchMultiplier = (1 + rawTimbre) * 0.25;

                double modulatedFrequency = ((baseFrequency * finalPitchMultiplier / 4)) + randomVariation;

                double masterVolume = VoiceInternalSettings.VoiceVolume / 400.0;

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

                SignalGenerator noiseGen = new SignalGenerator() { Type = SignalGeneratorType.White, Frequency = 0, Gain = (masterVolume * (VoiceInternalSettings.NoiseVolume / 100.0))/10 };
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
        VoiceInternalSettings.Formant1Frequency,
        VoiceInternalSettings.Formant2Frequency,
        VoiceInternalSettings.Formant3Frequency,
        VoiceInternalSettings.Formant4Frequency
    };
                double[] formantVols = new double[] {
        VoiceInternalSettings.Formant1Volume / 100.0,
        VoiceInternalSettings.Formant2Volume / 100.0,
        VoiceInternalSettings.Formant3Volume / 100.0,
        VoiceInternalSettings.Formant4Volume / 100.0
    };
                double noiseToFormantScale = VoiceInternalSettings.NoiseVolume / 100.0 * (VoiceInternalSettings.NoiseVolume > 0 ? 1.0 : 0.0);

                var lowPass = BiQuadFilter.LowPassFilter(sampleRate, VoiceInternalSettings.CutoffFrequency, 1.0f);

                double syb1Vol = VoiceInternalSettings.Sybillance1Volume * 0.18;
                double syb2Vol = VoiceInternalSettings.Sybillance2Volume * 0.15;
                double syb3Vol = VoiceInternalSettings.Sybillance3Volume * 0.12;
                double syb4Vol = VoiceInternalSettings.Sybillance4Volume * 0.10;

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

                var sybillanceFilter1 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance1Frequency, (float)VoiceInternalSettings.Sybillance1Range * 1.5f);
                var sybillanceFilter2 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance2Frequency, (float)VoiceInternalSettings.Sybillance2Range * 1.5f);
                var sybillanceFilter3 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance3Frequency, (float)VoiceInternalSettings.Sybillance3Range * 1.5f);
                var sybillanceFilter4 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance4Frequency, (float)VoiceInternalSettings.Sybillance4Range * 1.5f);

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