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
                PlayWave(SignalGeneratorType.Pink, freq, ms, nonStopping);
            }
        }
    }
}
