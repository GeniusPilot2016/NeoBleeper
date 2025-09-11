using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class RenderBeep
    {
        public static class SystemSpeakerBeepEngine // System speaker (aka PC speaker) beep engine by manually driving the hardware timer and system speaker port
        {
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
                AppDomain.CurrentDomain.ProcessExit += (s, e) => SafeStop();
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
            public static void StopBeep() // Stop the system speaker (aka PC speaker) from beeping
            {
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
            }
            public static bool isSystemSpeakerBeepStuck()
            {
                try
                {
                    // Check if the system speaker is currently beeping by reading the status of the speaker port
                    return (Inp32(0x61) & 0x03) == 0x03;
                }
                catch (Exception)
                {
                    return false; // If an error occurs, assume the speaker is not stuck
                }
            }
            public static bool isSystemSpeakerExist()
            {
                string query = "SELECT * FROM Win32_PNPEntity WHERE DeviceID LIKE '%PNP0800%'";
                using (var searcher = new ManagementObjectSearcher(query))
                {
                    var devices = searcher.Get();
                    return devices.Count > 0;
                }
            }
        }
        public static class SynthMisc // Use NAudio to synthesize beeps and noises in various forms
        {
            public static readonly WaveOutEvent waveOut = new WaveOutEvent();
            private static readonly SignalGenerator signalGenerator = new SignalGenerator() { Gain = 0.15 };
            private static readonly SignalGenerator whiteNoiseGenerator = new SignalGenerator() { Type = SignalGeneratorType.Pink, Gain = 0.5 };
            private static BandPassNoiseGenerator bandPassNoise;
            private static ISampleProvider currentProvider; // To keep track of the current provider
            private static readonly object lockObject = new object();

            static SynthMisc()
            {
                currentProvider = signalGenerator;
                waveOut.DesiredLatency = 50;
                waveOut.NumberOfBuffers = 4;
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
                lock (lockObject)
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
                lock (lockObject)
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
                lock (lockObject)
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
                lock (lockObject)
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
                lock (lockObject)
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
    }
}