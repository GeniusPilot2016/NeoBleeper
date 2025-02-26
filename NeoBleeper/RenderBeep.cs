﻿using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    internal class RenderBeep
    {
        public static class BeepClass
        {
            [DllImport("inpoutx64.dll")]
            extern static void Out32(short PortAddress, short Data);
            [DllImport("inpoutx64.dll")]
            extern static char Inp32(short PortAddress);
            public static void Beep(int freq, int ms)
            {
                Application.DoEvents();
                Out32(0x43, 0xB6);
                int div = 0x1234dc / freq;
                Out32(0x42, (Byte)(div & 0xFF));
                Out32(0x42, (Byte)(div >> 8));
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03));
                System.Threading.Thread.Sleep(ms);
                StopBeep();
            }
            public static void StopBeep()
            {
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
            }
        }
        public static class SynthMisc
        {
            private static readonly WaveOutEvent waveOut = new WaveOutEvent();
            private static readonly SignalGenerator signalGenerator = new SignalGenerator() { Gain = 0.35 };

            static SynthMisc()
            {
                waveOut.DesiredLatency = 20; // Gecikmeyi azaltmak için düşük bir değer ayarlayın
                waveOut.NumberOfBuffers = 50; // Buffer sayısını artırarak kesintileri azaltın
                waveOut.Init(signalGenerator);
            }

            public static void PlayWave(SignalGeneratorType type, int freq, int ms)
            {
                Application.DoEvents();
                signalGenerator.Frequency = freq;
                signalGenerator.Type = type;
                waveOut.Play();
                Thread.Sleep(ms);
                waveOut.Stop();
            }

            public static void SquareWave(int freq, int ms)
            {
                PlayWave(SignalGeneratorType.Square, freq, ms);
            }

            public static void SineWave(int freq, int ms)
            {
                PlayWave(SignalGeneratorType.Sin, freq, ms);
            }

            public static void TriangleWave(int freq, int ms)
            {
                PlayWave(SignalGeneratorType.Triangle, freq, ms);
            }

            public static void Noise(int freq, int ms)
            {
                PlayWave(SignalGeneratorType.White, freq, ms);
            }
        }
    }
}
