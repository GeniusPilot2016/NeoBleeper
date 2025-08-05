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
        public static class BeepClass // Manually drive the system speaker (aka PC speaker) using the inpoutx64.dll library in newer Windows versions
        {
            [DllImport("inpoutx64.dll")]
            extern static void Out32(short PortAddress, short Data);
            [DllImport("inpoutx64.dll")]
            extern static char Inp32(short PortAddress);
            public static void Beep(int freq, int ms, bool nonStopping) // Beep from the system speaker (aka PC speaker)
            {
                Out32(0x43, 0xB6);
                int div = 0x1234dc / freq;
                Out32(0x42, (Byte)(div & 0xFF));
                Out32(0x42, (Byte)(div >> 8));
                if (!nonStopping)
                {
                    NonBlockingSleep.Sleep(5); // Small delay to ensure the timer is set before starting the beep
                }
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03));
                NonBlockingSleep.Sleep(ms);
                if (!nonStopping) // If nonStopping is true, the beep will not stop
                {
                    StopBeep();
                }
            }
            public static void StopBeep() // Stop the system speaker (aka PC speaker) from beeping
            {
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
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
                        waveOut.Stop();
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
                        NonBlockingSleep.Sleep(5);
                    }

                    // Ensure waveOut is not already playing
                    if (waveOut.PlaybackState != PlaybackState.Playing)
                    {
                        waveOut.Play();
                    }
                }

                if (ms > 0)
                {
                    NonBlockingSleep.Sleep(nonStopping ? ms : Math.Max(1, ms - 1));
                }

                // Stop playback asynchronously to avoid blocking the current thread
                if (!nonStopping)
                {
                    Task.Run(() =>
                    {
                        lock (lockObject)
                        {
                            if (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                waveOut.Stop();
                            }
                        }
                    });
                    NonBlockingSleep.Sleep(1); // Add a minimal delay to prevent race conditions with the next note.
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
                    if (signalGenerator.Frequency != freq || signalGenerator.Type != type)
                    {
                        // Removed Task.Run to prevent race conditions. Settings are now applied synchronously.
                        signalGenerator.Frequency = freq;
                        signalGenerator.Type = type;
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