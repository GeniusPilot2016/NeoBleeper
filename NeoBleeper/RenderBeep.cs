using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Management;
using System.Runtime.InteropServices;
using static NeoBleeper.TemporarySettings;

namespace NeoBleeper
{
    public class RenderBeep
    {
        internal static readonly object AudioLock = new object();
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

            static SynthMisc()
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
            private readonly BiQuadFilter filter;
            private readonly double gain;

            public FilteredWaveProvider(ISampleProvider source, BiQuadFilter filter, double gain)
            {
                this.source = source;
                this.filter = filter;
                this.gain = gain;
            }

            public WaveFormat WaveFormat => source.WaveFormat;

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
            private readonly CachedSound cached;
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
        public static class VoiceSynthesizer
        {
            private static readonly Dictionary<int, (WaveOutEvent waveOut, ISampleProvider provider)> channels = new();
            public static void StartVoice(int channelId, int baseFrequency)
            {
                const int sampleRate = 44100;
                double normalizedPitch = VoiceInternalSettings.Pitch;
                // Semitone-based pitch conversion (2^(semitones/12)) — önceki versiyon yanlış ölçeklendiriyordu
                double modulatedFrequency = baseFrequency * Math.Pow(2.0, VoiceInternalSettings.Pitch) * (VoiceInternalSettings.Range);
                double masterVolume = VoiceInternalSettings.VoiceVolume / 400.0;

                // Tek periyodik kaynak oluştur (saw) ve önceden render ederek cache'e koy
                SignalGenerator renderSource = new SignalGenerator()
                {
                    Type = SignalGeneratorType.SawTooth,
                    Frequency = modulatedFrequency,
                    // Saw kazancını biraz azaltarak yüksek harmonikleri kısıyoruz
                    Gain = masterVolume * (VoiceInternalSettings.SawVolume / 1200.0)
                };

                int renderSeconds = 2; // 2s'lik döngü buffer'ı
                WaveFormat wf = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
                int totalSamples = sampleRate * renderSeconds;
                float[] renderBuffer = new float[totalSamples];
                int read = 0;
                while (read < totalSamples)
                {
                    int r = renderSource.Read(renderBuffer, read, totalSamples - read);
                    if (r == 0) break;
                    read += r;
                }
                if (read < totalSamples)
                {
                    Array.Clear(renderBuffer, read, totalSamples - read);
                }
                var cachedVoiced = new CachedSound(renderBuffer, wf);

                // White noise cached (sibilance / formant noise)
                SignalGenerator noiseGen = new SignalGenerator() { Type = SignalGeneratorType.White, Frequency = 0, Gain = masterVolume * (VoiceInternalSettings.NoiseVolume / 800.0) };
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
                // Gürültü ölçeğini artırdık — formant içi breathiness artar
                double noiseToFormantScale = VoiceInternalSettings.NoiseVolume / 1000.0 * (VoiceInternalSettings.NoiseVolume > 0 ? 1.0 : 0.0);

                var lowPass = BiQuadFilter.LowPassFilter(sampleRate, VoiceInternalSettings.CutoffFrequency, 1.0f);

                double syb1Vol = VoiceInternalSettings.Sybillance1Volume * 0.18;
                double syb2Vol = VoiceInternalSettings.Sybillance2Volume * 0.15;
                double syb3Vol = VoiceInternalSettings.Sybillance3Volume * 0.12;
                double syb4Vol = VoiceInternalSettings.Sybillance4Volume * 0.10;

                var providers = new List<ISampleProvider>();

                // Aynı cached voiced source'dan bağımsız okuyucular kullanılır
                for (int i = 0; i < 4; i++)
                {
                    double fCenter = formantFreqs[i];
                    double fVol = formantVols[i];
                    // Q'yu azaltıyoruz — çok dar Q'lar korna benzeri rezonansa neden oluyordu
                    float dynamicQ = (float)(0.5f + (i * 0.3f)) * BaseFormantQ(baseFrequency) * 1.6f;
                    var filter = MakeBP(sampleRate, fCenter, dynamicQ);
                    var voicedReader = new CachedSoundSampleProvider(cachedVoiced, loop: true);
                    // Formant kazancını makul seviyeye çekiyoruz
                    providers.Add(new FilteredWaveProvider(voicedReader, filter, Math.Min(fVol * 1.0, 2.0)));

                    // formant içi gürültü (breathy component)
                    var noiseFilter = MakeBP(sampleRate, fCenter, dynamicQ * 1.1f);
                    var noiseReader = new CachedSoundSampleProvider(cachedNoise, loop: true);
                    providers.Add(new FilteredWaveProvider(noiseReader, noiseFilter, noiseToFormantScale * fVol * 1.2));
                }

                // sibilance: ayrı cached noise okuyucuları kullan
                var sibilanceFilter1 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance1Frequency, (float)VoiceInternalSettings.Sybillance1Range * 1.5f);
                var sibilanceFilter2 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance2Frequency, (float)VoiceInternalSettings.Sybillance2Range * 1.5f);
                var sibilanceFilter3 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance3Frequency, (float)VoiceInternalSettings.Sybillance3Range * 1.5f);
                var sibilanceFilter4 = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate, VoiceInternalSettings.Sybillance4Frequency, (float)VoiceInternalSettings.Sybillance4Range * 1.5f);

                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sibilanceFilter1, syb1Vol));
                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sibilanceFilter2, syb2Vol));
                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sibilanceFilter3, syb3Vol));
                providers.Add(new FilteredWaveProvider(new CachedSoundSampleProvider(cachedNoise, true), sibilanceFilter4, syb4Vol));

                var mixed = new MixingSampleProvider(providers) { ReadFully = true };
                var finalFiltered = new FilteredWaveProvider(mixed, lowPass, 1.0);
                var volumeControlled = new VolumeSampleProvider(finalFiltered) { Volume = 1.0f };

                if (channels.TryGetValue(channelId, out var tuple))
                {
                    tuple.waveOut.Stop();
                    tuple.waveOut.Dispose();
                }

                var waveOut = new WaveOutEvent
                {
                    DesiredLatency = 50,
                    NumberOfBuffers = 4,
                    Volume = 1.0f
                };
                waveOut.Init(volumeControlled);
                waveOut.Play();

                channels[channelId] = (waveOut, volumeControlled);
            }
            public static void StopVoice(int channelId)
            {
                if (channels.TryGetValue(channelId, out var tuple))
                {
                    tuple.waveOut.Stop();
                    tuple.waveOut.Dispose();
                    channels.Remove(channelId);
                }
            }
        }
    }
}