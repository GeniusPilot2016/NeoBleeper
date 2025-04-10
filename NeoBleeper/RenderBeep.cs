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
                NonBlockingSleep.Sleep(1);
                Out32(0x42, (Byte)(div & 0xFF));
                Out32(0x42, (Byte)(div >> 8));
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03));
                NonBlockingSleep.Sleep(ms);
                if(nonStopping==false) // If nonStopping is true, the beep will not stop
                {
                    StopBeep();
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

            static SynthMisc()
            {
                waveOut.DesiredLatency = 50; // Set the desired latency to 20ms to reduce the delay
                waveOut.NumberOfBuffers = 35; // Reduce the number of buffers to reduce the delay
                waveOut.Init(signalGenerator);
            }

            public static void PlayWave(SignalGeneratorType type, int freq, int ms, bool nonStopping) // Create many kind of beeps from sound devices (external speakers, headphone, etc.)
            {
                signalGenerator.Frequency = freq;
                signalGenerator.Type = type;
                waveOut.Play();
                NonBlockingSleep.Sleep(ms);
                if(nonStopping== false) // If nonStopping is true, the beep will not stop
                {
                    waveOut.Stop();
                }
            }
            private static BandPassNoiseGenerator bandPassNoise;

            public static void PlayFilteredNoise(int freq, int ms, bool nonStopping)
            {
                if (bandPassNoise == null)
                {
                    // Create white noise and initialize the band-pass filter
                    var whiteNoise = new SignalGenerator()
                    {
                        Type = SignalGeneratorType.White,
                        Gain = 0.15
                    };

                    bandPassNoise = new BandPassNoiseGenerator(whiteNoise, 44100, freq, 1.0f);
                    waveOut.Init(bandPassNoise);
                }
                else
                {
                    // Update the frequency dynamically
                    bandPassNoise.UpdateFrequency(freq, 44100, 1.0f);
                }

                waveOut.Play();
                NonBlockingSleep.Sleep(ms);

                if (!nonStopping)
                {
                    waveOut.Stop();
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
