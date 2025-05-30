using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public class RenderBeep
    {
        public static class BeepClass
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
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03));
                NonBlockingSleep.Sleep(ms);
                if(nonStopping==false) // If nonStopping is true, the beep will not stop
                {
                    StopBeep();
                    NonBlockingSleep.Sleep(5); // Small delay to ensure the sound stops
                }
            }
            public static void StopBeep()
            {
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
            }
        }
        public static class SynthMisc
        {
            public static readonly WaveOutEvent waveOut = new WaveOutEvent();
            private static readonly SignalGenerator signalGenerator = new SignalGenerator() { Gain = 0.15 };
            private static BandPassNoiseGenerator bandPassNoise;
            private static ISampleProvider currentProvider; // To keep track of the current provider

            static SynthMisc()
            {
                currentProvider = signalGenerator;
                waveOut.DesiredLatency = 1;
                waveOut.NumberOfBuffers = 75;
                waveOut.Init(signalGenerator);
            }

            public static void PlayWave(SignalGeneratorType type, int freq, int ms, bool nonStopping)
            {
                signalGenerator.Frequency = freq;
                signalGenerator.Type = type;

                // Change the provider to signalGenerator if it's not already set
                if (currentProvider != signalGenerator)
                {
                    bool wasPlaying = waveOut.PlaybackState == PlaybackState.Playing;
                    waveOut.Stop();
                    waveOut.Init(signalGenerator);
                    currentProvider = signalGenerator;
                    if (wasPlaying) // Restart if it was playing before
                        waveOut.Play();
                }
                waveOut.Play();
                NonBlockingSleep.Sleep(ms);
                if (!nonStopping)
                {
                    waveOut.Stop();
                    NonBlockingSleep.Sleep(2); // Small delay to ensure the sound stops
                }
            }

            public static void PlayFilteredNoise(int freq, int ms, bool nonStopping)
            {
                if (bandPassNoise == null)
                {
                    var whiteNoise = new SignalGenerator()
                    {
                        Type = SignalGeneratorType.Pink,
                        Gain = 0.5
                    };

                    bandPassNoise = new BandPassNoiseGenerator(whiteNoise, 44100, freq, 1.0f);
                }
                else
                {
                    bandPassNoise.UpdateFrequency(freq, 44100, 1.0f);
                }

                // Change the provider to bandPassNoise if it's not already set
                if (currentProvider != bandPassNoise)
                {
                    bool wasPlaying = waveOut.PlaybackState == PlaybackState.Playing;
                    waveOut.Stop();
                    waveOut.Init(bandPassNoise);
                    currentProvider = bandPassNoise;;
                    if (wasPlaying) // Restart if it was playing before
                        waveOut.Play();
                }
                waveOut.Play();
                NonBlockingSleep.Sleep(ms);

                if (!nonStopping)
                {
                    waveOut.Stop();
                    NonBlockingSleep.Sleep(2); // Small delay to ensure the sound stops
                }
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
