using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
                waveOut.NumberOfBuffers = 5;
                waveOut.Init(signalGenerator);
            }

            private static void SetCurrentProvider(ISampleProvider provider)
            {
                if (currentProvider != provider)
                {
                    bool wasPlaying = waveOut.PlaybackState == PlaybackState.Playing;
                    waveOut.Stop();
                    waveOut.Init(provider);
                    currentProvider = provider;
                    if (wasPlaying)
                    {
                        waveOut.Play();
                    }
                }
            }

            private static void PlaySound(int ms, bool nonStopping)
            {
                var stopwatch = Stopwatch.StartNew();

                if (!nonStopping)
                {
                    NonBlockingSleep.Sleep(5); 
                }
                waveOut.Play(); 

                stopwatch.Stop();
                var elapsedMicroseconds = stopwatch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
                var targetMicroseconds = (long)ms * 1000;
                var remainingMicroseconds = targetMicroseconds - elapsedMicroseconds;

                if (ms > 0 && remainingMicroseconds > 0)
                {
                    NonBlockingSleep.SleepMicroseconds(remainingMicroseconds);
                }

                if (!nonStopping)
                {
                    waveOut.Stop(); 
                }
            }

            public static void PlayWave(SignalGeneratorType type, int freq, int ms, bool nonStopping)
            {
                SetCurrentProvider(signalGenerator);
                signalGenerator.Frequency = freq;
                signalGenerator.Type = type;
                PlaySound(ms, nonStopping);
            }

            public static void PlayFilteredNoise(int freq, int ms, bool nonStopping)
            {
                if (bandPassNoise == null)
                {
                    bandPassNoise = new BandPassNoiseGenerator(whiteNoiseGenerator, 44100, freq, 1.0f);
                }
                else
                {
                    bandPassNoise.UpdateFrequency(freq, 44100, 1.0f);
                }

                SetCurrentProvider(bandPassNoise);
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