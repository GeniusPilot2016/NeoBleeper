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

using NAudio.SoundFont;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class NoteUtility
    {
        Random fermataRnd = new Random(); // Random instance for fermata duration variation
        public static class BaseNoteFrequencyIn4thOctave
        {
            public static double C = 261.63;
            public static double CS = 277.18;
            public static double D = 293.66;
            public static double DS = 311.13;
            public static double E = 329.63;
            public static double F = 349.23;
            public static double FS = 369.99;
            public static double G = 392.00;
            public static double GS = 415.30;
            public static double A = 440.00;
            public static double AS = 466.16;
            public static double B = 493.88;
        }
    }

    public class NoteFrequencies
    {
        /// <summary>
        /// Calculates the frequency, in hertz, corresponding to a given musical note name and octave.
        /// </summary>
        public static double GetFrequencyFromNoteName(string noteName)
        {
            if (string.IsNullOrEmpty(noteName))
                return NoteUtility.BaseNoteFrequencyIn4thOctave.C;
            try
            {
                var m = Regex.Match(noteName.ToUpperInvariant(), @"^([A-G])(#?)(\d+)$");
                if (!m.Success) return 0;
                string note = m.Groups[1].Value + (m.Groups[2].Value == "#" ? "#" : "");
                int octave = int.Parse(m.Groups[3].Value);
                double baseFrequency = note switch
                {
                    "C" => NoteUtility.BaseNoteFrequencyIn4thOctave.C,
                    "C#" => NoteUtility.BaseNoteFrequencyIn4thOctave.CS,
                    "D" => NoteUtility.BaseNoteFrequencyIn4thOctave.D,
                    "D#" => NoteUtility.BaseNoteFrequencyIn4thOctave.DS,
                    "E" => NoteUtility.BaseNoteFrequencyIn4thOctave.E,
                    "F" => NoteUtility.BaseNoteFrequencyIn4thOctave.F,
                    "F#" => NoteUtility.BaseNoteFrequencyIn4thOctave.FS,
                    "G" => NoteUtility.BaseNoteFrequencyIn4thOctave.G,
                    "G#" => NoteUtility.BaseNoteFrequencyIn4thOctave.GS,
                    "A" => NoteUtility.BaseNoteFrequencyIn4thOctave.A,
                    "A#" => NoteUtility.BaseNoteFrequencyIn4thOctave.AS,
                    "B" => NoteUtility.BaseNoteFrequencyIn4thOctave.B,
                    _ => 0
                };

                if (baseFrequency == 0)
                    return 0;

                int octaveDifference = octave - 4;
                return baseFrequency * Math.Pow(2, octaveDifference);
            }
            catch
            {
                return NoteUtility.BaseNoteFrequencyIn4thOctave.C;
            }
        }
    }

    public static class NoteLengths
    {
        public static double CalculateNoteLength(double length, string articulation = "")
        {
            switch (articulation)
            {
                case "Sta":
                    length = length / 2.0;
                    break;
                case "Spi":
                    length = length / 4.0;
                    break;
                default:
                    break;
            }
            return length;
        }

        public static double CalculateLineLength(int bpm, string noteType, string modifier = "")
        {
            if (bpm == 0) bpm = 1;
            double millisecondsPerBeat = 60000.0 / bpm;
            double baseLength = noteType switch
            {
                "Whole" => millisecondsPerBeat * 4.0,
                "Half" => millisecondsPerBeat * 2.0,
                "Quarter" => millisecondsPerBeat,
                "1/8" => millisecondsPerBeat / 2.0,
                "1/16" => millisecondsPerBeat / 4.0,
                "1/32" => millisecondsPerBeat / 8.0,
                _ => millisecondsPerBeat
            };
            switch (modifier)
            {
                case "Dot":
                    baseLength = baseLength * 1.5;
                    break;
                case "Tri":
                    baseLength = baseLength / 3.0;
                    break;
            }
            return baseLength;
        }

        public static (int totalRhythm_int, int noteSound_int) CalculateNoteDurations(
            string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio)
        {
            if (bpm == 0)
                bpm = 1;

            var (lengthName_checked, modifier_checked, articulation_checked) =
                UseOriginalValueOrDefault(lengthName, modifier, articulation);

            double totalRhythm_double = FixRoundingErrors(
                CalculateLineLength(bpm, lengthName_checked, modifier_checked));

            double noteSound_double = FixRoundingErrors(
                CalculateNoteLength(totalRhythm_double, articulation_checked));

            if (articulation_checked == "Fer")
            {
                double extraFermataDuration = totalRhythm_double * (0.5 + 0.5 * Random.Shared.NextDouble());
                totalRhythm_double += extraFermataDuration;
                noteSound_double = FixRoundingErrors(
                    CalculateNoteLength(totalRhythm_double, articulation_checked));
            }

            noteSound_double *= noteSilenceRatio;

            int totalRhythm_int = (int)Math.Round(totalRhythm_double, MidpointRounding.AwayFromZero);
            int noteSound_int = (int)Math.Round(noteSound_double, MidpointRounding.AwayFromZero);

            if (noteSound_int > totalRhythm_int) noteSound_int = totalRhythm_int;
            if (noteSound_int < 0) noteSound_int = 0;

            return (totalRhythm_int, noteSound_int);
        }

        public static (int totalRhythm_int, int noteSound_int, double nextCursorMs) CalculateNoteDurationsAtPosition(
            string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio,
            double cursorMs)
        {
            if (bpm == 0)
                bpm = 1;

            var (lengthName_checked, modifier_checked, articulation_checked) =
                UseOriginalValueOrDefault(lengthName, modifier, articulation);

            double totalRhythm_double = FixRoundingErrors(
                CalculateLineLength(bpm, lengthName_checked, modifier_checked));

            if (articulation_checked == "Fer")
            {
                double extra = totalRhythm_double * (0.5 + 0.5 * Random.Shared.NextDouble());
                totalRhythm_double += extra;
            }

            double noteSound_double = FixRoundingErrors(
                CalculateNoteLength(totalRhythm_double, articulation_checked)) * noteSilenceRatio;

            double nextCursor = cursorMs + totalRhythm_double;
            int totalRhythm_int = (int)Math.Round(nextCursor) - (int)Math.Round(cursorMs);
            int noteSound_int = (int)Math.Round(noteSound_double, MidpointRounding.AwayFromZero);

            if (noteSound_int > totalRhythm_int) noteSound_int = totalRhythm_int;
            if (noteSound_int < 0) noteSound_int = 0;

            return (totalRhythm_int, noteSound_int, nextCursor);
        }

        public static double FixRoundingErrors(double inputValue)
        {
            const double threshold = 1e-7;
            const double adjustment = 1e-10;

            if (inputValue >= 0)
            {
                if (inputValue > threshold)
                    inputValue += adjustment;
            }
            else
            {
                if (inputValue < (threshold * -1))
                    inputValue -= adjustment;
            }
            return inputValue;
        }

        private static (string returnedLength, string returnedModifier, string returnedArticulation)
            UseOriginalValueOrDefault(string length, string modifier, string articulation)
        {
            string[] allowedLengths = { "Whole", "Half", "Quarter", "1/8", "1/16", "1/32" };
            string[] allowedModifiers = { "Dot", "Tri" };
            string[] allowedArticulations = { "Sta", "Spi", "Fer" };

            string currentLength = allowedLengths.Contains(length) ? length : "Quarter";
            string currentModifier = allowedModifiers.Contains(modifier) ? modifier : string.Empty;
            string currentArticulation = allowedArticulations.Contains(articulation) ? articulation : string.Empty;
            return (currentLength, currentModifier, currentArticulation);
        }
    }

    public class PercussionSounds
    {
        public enum MidiPercussion : byte
        {
            Laser = 27, Whip = 28, ScratchPush = 29, ScratchPull = 30, StickClick = 31, SquareClick = 32,
            MetronomeClick = 33, MetronomeBell = 34, BassDrum = 35, KickDrum = 36, SideStick = 37,
            SnareCrossStick = SideStick, SnareDrum = 38, HandClap = 39, ElectricSnareDrum = 40, FloorTom2 = 41,
            HiHatClosed = 42, FloorTom1 = 43, HiHatFoot = 44, LowTom = 45, HiHatOpen = 46, LowMidTom = 47,
            HighMidTom = 48, CrashCymbal = 49, HighTom = 50, RideCymbal = 51, ChinaCymbal = 52, RideBell = 53,
            Tambourine = 54, SplashCymbal = 55, Cowbell = 56, CrashCymbal2 = 57, Vibraslap = 58, RideCymbal2 = 59,
            HighBongo = 60, LowBongo = 61, CongaDeadStroke = 62, Conga = 63, Tumba = 64, HighTimbale = 65,
            LowTimbale = 66, HighAgogo = 67, LowAgogo = 68, Cabasa = 69, Maracas = 70, WhistleShort = 71,
            WhistleLong = 72, GuiroShort = 73, Güiro = GuiroShort, GuiroLong = 74, Claves = 75, Clave = Claves,
            HighWoodblock = 76, WoodBlock = HighWoodblock, LowWoodblock = 77, CuicaHigh = 78, CuicaLow = 79,
            TriangleMute = 80, TriangleOpen = 81, Shaker = 82, SleighBell = 83, BellTree = 84, Castanets = 85,
            SurduDeadStroke = 86, Surdu = 87, SnareDrumRod = 91, OceanDrum = 92, SnareDrumBrush = 93
        }

        public enum PercussionOutputChoice { SystemSpeaker, SoundDevice }
        private enum SynthWave { Sine, Square, Triangle, Noise }

        private const double RetriggerGapMs = 0.5;
        private const double MinAudibleSystemSpeakerMs = 15.0;

        private static readonly object _hardwareLock = new object();
        private static readonly object _queueLock = new object();
        private static readonly Queue<PercussionRequest> _pendingRequests = new Queue<PercussionRequest>();
        private static bool _queueWorkerRunning;

        private const int PercussionSampleRate = 44100;
        private static readonly object _soundDeviceLock = new object();
        private static NAudio.Wave.WaveOutEvent? _percussionWaveOut;
        private static NAudio.Wave.SampleProviders.MixingSampleProvider? _percussionMixer;

        private sealed class FloatArraySampleProvider : NAudio.Wave.ISampleProvider
        {
            private readonly float[] _samples;
            private int _position;

            public FloatArraySampleProvider(float[] samples)
            {
                _samples = samples ?? Array.Empty<float>();
                WaveFormat = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(PercussionSampleRate, 1);
            }

            public NAudio.Wave.WaveFormat WaveFormat { get; }

            public int Read(float[] buffer, int offset, int count)
            {
                int available = _samples.Length - _position;
                int toCopy = Math.Min(available, count);
                if (toCopy <= 0) return 0;

                Array.Copy(_samples, _position, buffer, offset, toCopy);
                _position += toCopy;
                return toCopy;
            }
        }

        private sealed class PercussionRequest
        {
            public readonly MidiPercussion Percussion;
            public readonly CancellationToken CancellationToken;
            public readonly int DurationMs;
            public readonly int CompletionDelayMs;
            public readonly PercussionOutputChoice Output;
            public readonly PercussionProfile Profile;
            public readonly int Velocity;
            public readonly int RandomSeed;
            public readonly TaskCompletionSource<bool>? Completion;

            public PercussionRequest(
                MidiPercussion percussion,
                CancellationToken cancellationToken,
                int durationMs,
                int completionDelayMs,
                PercussionOutputChoice output,
                PercussionProfile profile,
                int velocity,
                TaskCompletionSource<bool>? completion)
            {
                Percussion = percussion;
                CancellationToken = cancellationToken;
                DurationMs = Math.Max(1, durationMs);
                CompletionDelayMs = Math.Max(1, completionDelayMs);
                Output = output;
                Profile = profile;
                Velocity = Math.Clamp(velocity, 1, 127);
                RandomSeed = Random.Shared.Next();
                Completion = completion;
            }
        }

        private static int ClampPercussionFrequency(double frequency) =>
            (int)Math.Round(Math.Clamp(frequency, 80.0, 2400.0)); // Strictly capped at 2400Hz to prevent squeaking/whistling

        private static void StartPulseDirect(PercussionOutputChoice outputChoice, int frequency, SynthWave waveType)
        {
            frequency = ClampPercussionFrequency(frequency);
            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                    break;
                case PercussionOutputChoice.SoundDevice:
                    var naudioWave = waveType switch
                    {
                        SynthWave.Noise => NAudio.Wave.SampleProviders.SignalGeneratorType.White,
                        SynthWave.Triangle => NAudio.Wave.SampleProviders.SignalGeneratorType.Triangle,
                        SynthWave.Sine => NAudio.Wave.SampleProviders.SignalGeneratorType.Sin,
                        _ => NAudio.Wave.SampleProviders.SignalGeneratorType.Square
                    };
                    SoundRenderingEngine.WaveSynthEngine.StartSynth(naudioWave, frequency);
                    break;
            }
        }

        private static void StopPulseDirect(PercussionOutputChoice outputChoice)
        {
            if (outputChoice == PercussionOutputChoice.SystemSpeaker)
                SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
            else
                SoundRenderingEngine.WaveSynthEngine.StopSynth();
        }

        private static void StopCurrentPulse(ref PercussionOutputChoice? currentOutput)
        {
            lock (_hardwareLock)
            {
                if (currentOutput.HasValue)
                    StopPulseDirect(currentOutput.Value);
            }
            currentOutput = null;
        }

        private static void StartOrUpdatePulse(
            PercussionOutputChoice output,
            int frequency,
            SynthWave wave,
            ref PercussionOutputChoice? currentOutput)
        {
            lock (_hardwareLock)
            {
                if (currentOutput.HasValue && currentOutput.Value != output)
                    StopPulseDirect(currentOutput.Value);

                StartPulseDirect(output, frequency, wave);
                currentOutput = output;
            }
        }

        private static void EnsurePercussionSoundDevice()
        {
            lock (_soundDeviceLock)
            {
                if (_percussionWaveOut != null && _percussionMixer != null)
                    return;

                var format = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(PercussionSampleRate, 1);
                _percussionMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(format)
                {
                    ReadFully = true
                };

                _percussionWaveOut = new NAudio.Wave.WaveOutEvent
                {
                    DesiredLatency = 40,
                    NumberOfBuffers = 3
                };
                _percussionWaveOut.Init(_percussionMixer);
                _percussionWaveOut.Play();
            }
        }

        private static void QueueMixedSoundDeviceHit(PercussionRequest request)
        {
            EnsurePercussionSoundDevice();
            float[] samples = RenderPercussionSamples(request);
            var hitProvider = new FloatArraySampleProvider(samples);

            lock (_soundDeviceLock)
            {
                _percussionMixer?.AddMixerInput(hitProvider);
            }
        }

        private static float[] RenderPercussionSamples(PercussionRequest request)
        {
            int sampleCount = Math.Max(1, (int)Math.Ceiling(request.DurationMs * PercussionSampleRate / 1000.0));
            var result = new float[sampleCount];
            double velocity01 = request.Velocity / 127.0;
            double masterGain = 0.70 + velocity01 * 0.40;

            uint rng = unchecked((uint)request.RandomSeed);
            double lpLow = 0.0, lpMid = 0.0, lpHigh = 0.0;
            double dcBlockX = 0.0, dcBlockY = 0.0;
            double tonePhase = 0.0;

            bool kick = IsKick(request.Percussion);
            bool tom = IsTomOrBongo(request.Percussion);
            bool snare = IsSnare(request.Percussion);
            bool cymbal = IsMetalCymbal(request.Percussion);
            bool shaker = request.Percussion is MidiPercussion.Shaker or MidiPercussion.Maracas or MidiPercussion.Cabasa;
            bool isTonalSquare = request.Profile.BodyWave == SynthWave.Square;

            for (int i = 0; i < sampleCount; i++)
            {
                double elapsedMs = i * 1000.0 / PercussionSampleRate;
                double progress = Math.Clamp(elapsedMs / request.DurationMs, 0.0, 1.0);

                rng ^= rng << 13;
                rng ^= rng >> 17;
                rng ^= rng << 5;
                double white = ((rng & 0x00FFFFFF) / 8388607.5) - 1.0;

                double lowCut = kick ? 120.0 : tom ? 220.0 : 600.0;
                double midCut = snare ? 2800.0 : cymbal ? 5000.0 : 2000.0;
                double highCut = cymbal ? 10000.0 : 7500.0;

                double aLow = 1.0 - Math.Exp(-2.0 * Math.PI * lowCut / PercussionSampleRate);
                double aMid = 1.0 - Math.Exp(-2.0 * Math.PI * midCut / PercussionSampleRate);
                double aHigh = 1.0 - Math.Exp(-2.0 * Math.PI * highCut / PercussionSampleRate);

                lpLow += aLow * (white - lpLow);
                lpMid += aMid * (white - lpMid);
                lpHigh += aHigh * (white - lpHigh);

                double lowBand = lpLow;
                double midBand = lpMid - lpLow;
                double highBand = white - lpHigh;

                double body = Math.Exp(-elapsedMs / Math.Max(25.0, request.DurationMs * 0.5));
                double tail = Math.Pow(Math.Max(0.0, 1.0 - progress), request.Profile.DecayShape);

                double toneVal = 0.0;
                if (isTonalSquare || request.Profile.BodyWave == SynthWave.Sine)
                {
                    double currentFreq = request.Profile.BodyStartFreq;
                    if (request.Profile.DoesSweep)
                    {
                        double logSweep = 1.0 - Math.Pow(progress, 0.45);
                        currentFreq = request.Profile.BodyEndFreq + (request.Profile.BodyStartFreq - request.Profile.BodyEndFreq) * logSweep;
                    }
                    tonePhase = (tonePhase + currentFreq / PercussionSampleRate) % 1.0;
                    toneVal = request.Profile.BodyWave == SynthWave.Square
                        ? (tonePhase < 0.5 ? 0.7 : -0.7)
                        : Math.Sin(tonePhase * 2.0 * Math.PI);
                }

                double source = 0.0;
                if (kick) source = toneVal * body * 1.2 + lowBand * body * 0.8;
                else if (tom) source = toneVal * body * 1.0 + midBand * body * 0.5;
                else if (snare) source = midBand * body * 1.4 + highBand * tail * 0.8;
                else if (cymbal) source = highBand * tail * 1.4 + midBand * tail * 0.4;
                else if (shaker) source = highBand * tail * 1.2;
                else source = toneVal * body * 1.0 + midBand * body * 0.8;

                double sample = source * masterGain;
                double dcBlocked = sample - dcBlockX + 0.995 * dcBlockY;
                dcBlockX = sample;
                dcBlockY = dcBlocked;

                result[i] = (float)(Math.Tanh(dcBlocked) * 0.90);
            }

            return result;
        }

        private readonly struct PercussionProfile
        {
            public readonly SynthWave BodyWave;
            public readonly bool DoesSweep;
            public readonly int BodyStartFreq;
            public readonly int BodyEndFreq;
            public readonly int DurationMs;
            public readonly double NoiseDensity;
            public readonly double HoldRatio;
            public readonly int AttackFrequency;
            public readonly double AttackMs;
            public readonly double DecayShape;
            public readonly double PitchJitter;

            public PercussionProfile(
                SynthWave w, bool s, int start, int end, int dur,
                double density = 0.5, double holdRatio = 0.15,
                int attackFrequency = 0, double attackMs = 2.0,
                double decayShape = 1.8, double pitchJitter = 0.04)
            {
                BodyWave = w;
                DoesSweep = s;
                BodyStartFreq = start;
                BodyEndFreq = end;
                DurationMs = dur;
                NoiseDensity = Math.Clamp(density, 0.01, 1.0);
                HoldRatio = Math.Clamp(holdRatio, 0.01, 0.95);
                AttackFrequency = attackFrequency <= 0 ? start : attackFrequency;
                AttackMs = Math.Clamp(attackMs, 0.2, 20.0);
                DecayShape = Math.Clamp(decayShape, 0.5, 5.0);
                PitchJitter = Math.Clamp(pitchJitter, 0.0, 0.25);
            }
        }

        private static PercussionProfile GetProfile(MidiPercussion percussion)
        {
            return percussion switch
            {
                MidiPercussion.KickDrum or MidiPercussion.BassDrum =>
                    new PercussionProfile(SynthWave.Noise, true, 160, 65, 110, density: 0.95, holdRatio: 0.02, attackFrequency: 180, attackMs: 1.5, decayShape: 3.2, pitchJitter: 0.005),

                MidiPercussion.HighTom =>
                    new PercussionProfile(SynthWave.Noise, true, 220, 120, 110, density: 0.9, holdRatio: 0.05, attackFrequency: 240, attackMs: 1.5, decayShape: 2.5, pitchJitter: 0.01),
                MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom =>
                    new PercussionProfile(SynthWave.Noise, true, 180, 95, 125, density: 0.9, holdRatio: 0.05, attackFrequency: 200, attackMs: 1.8, decayShape: 2.3, pitchJitter: 0.01),
                MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 =>
                    new PercussionProfile(SynthWave.Noise, true, 140, 75, 140, density: 0.9, holdRatio: 0.06, attackFrequency: 160, attackMs: 2.0, decayShape: 2.0, pitchJitter: 0.01),

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush =>
                    new PercussionProfile(SynthWave.Noise, false, 400, 400, 120, density: 0.85, holdRatio: 0.02, attackFrequency: 500, attackMs: 1.2, decayShape: 2.8, pitchJitter: 0.06),

                MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets =>
                    new PercussionProfile(SynthWave.Noise, false, 1000, 1000, 30, density: 0.9, holdRatio: 0.01, attackFrequency: 1200, attackMs: 0.4, decayShape: 4.5, pitchJitter: 0.03),

                MidiPercussion.Cowbell =>
                    new PercussionProfile(SynthWave.Sine, false, 750, 750, 120, holdRatio: 0.05, attackFrequency: 900, attackMs: 0.8, decayShape: 2.2, pitchJitter: 0.01),

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal =>
                    new PercussionProfile(SynthWave.Noise, false, 2000, 2000, 600, density: 0.7, holdRatio: 0.01, attackFrequency: 2200, attackMs: 1.2, decayShape: 1.6, pitchJitter: 0.1),
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                    new PercussionProfile(SynthWave.Noise, false, 1800, 1800, 500, density: 0.6, holdRatio: 0.01, attackFrequency: 2000, attackMs: 1.0, decayShape: 1.7, pitchJitter: 0.08),

                MidiPercussion.HiHatClosed =>
                    new PercussionProfile(SynthWave.Noise, false, 2000, 2000, 80, density: 0.85, holdRatio: 0.02, attackFrequency: 2200, attackMs: 0.5, decayShape: 3.0, pitchJitter: 0.06),
                MidiPercussion.HiHatOpen =>
                    new PercussionProfile(SynthWave.Noise, false, 1800, 1800, 350, density: 0.6, holdRatio: 0.01, attackFrequency: 2000, attackMs: 1.2, decayShape: 1.8, pitchJitter: 0.08),
                MidiPercussion.HiHatFoot =>
                    new PercussionProfile(SynthWave.Noise, false, 1900, 1900, 90, density: 0.8, holdRatio: 0.02, attackFrequency: 2100, attackMs: 0.6, decayShape: 2.8, pitchJitter: 0.05),

                MidiPercussion.HandClap =>
                    new PercussionProfile(SynthWave.Noise, false, 900, 900, 140, density: 0.85, holdRatio: 0.02, attackFrequency: 1100, attackMs: 0.5, decayShape: 2.0, pitchJitter: 0.12),

                _ => new PercussionProfile(SynthWave.Noise, false, 400, 400, 100, density: 0.6, holdRatio: 0.02, attackFrequency: 500, attackMs: 1.2, decayShape: 2.5, pitchJitter: 0.06)
            };
        }

        private static bool IsKick(MidiPercussion p) => p is MidiPercussion.KickDrum or MidiPercussion.BassDrum;
        private static bool IsTomOrBongo(MidiPercussion p) => p is MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 or MidiPercussion.LowTom or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom or MidiPercussion.HighTom;
        private static bool IsSnare(MidiPercussion p) => p is MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush;
        private static bool IsClick(MidiPercussion p) => p is MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets or MidiPercussion.Claves or MidiPercussion.WoodBlock;
        private static bool IsCymbalOrLongRing(MidiPercussion p) => p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal or MidiPercussion.HiHatOpen;
        private static bool IsShortCymbal(MidiPercussion p) => p is MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot or MidiPercussion.TriangleMute;
        private static bool IsMetalCymbal(MidiPercussion p) => IsCymbalOrLongRing(p) || IsShortCymbal(p);
        private static bool IsTonalNonCymbal(MidiPercussion p) => p is MidiPercussion.Cowbell or MidiPercussion.MetronomeBell or MidiPercussion.HighAgogo or MidiPercussion.LowAgogo;

        public static void PlayPercussion(MidiPercussion p, CancellationToken ct = default, int maxMs = 5000, int velocity = 100)
        {
            if (ct.IsCancellationRequested) return;

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);
            int duration = Math.Max(30, Math.Min(maxMs, prof.DurationMs));

            EnqueuePercussion(new PercussionRequest(p, ct, duration, duration, output, prof, velocity, null));
        }

        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct = default, int velocity = 100)
        {
            if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);

            if (durationMs <= 0) return Task.CompletedTask;

            int minBodyMs = (int)Math.Ceiling(GetMinimumBodyMs(p));
            int audibleDurationMs = Math.Max(durationMs, minBodyMs);

            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            EnqueuePercussion(new PercussionRequest(p, ct, audibleDurationMs, durationMs, output, prof, velocity, completion));
            return completion.Task;
        }

        public static Task PlayPercussionSliceAsync(MidiPercussion p, int sliceDurationMs, CancellationToken ct = default, int velocity = 100)
        {
            return PlayPercussionForDurationAsync(p, sliceDurationMs, ct, velocity);
        }

        private static void EnqueuePercussion(PercussionRequest request)
        {
            bool startWorker = false;
            lock (_queueLock)
            {
                _pendingRequests.Enqueue(request);
                if (!_queueWorkerRunning)
                {
                    _queueWorkerRunning = true;
                    startWorker = true;
                }
            }
            if (startWorker) Task.Run(ProcessPercussionQueue);
        }

        private static bool TryDequeueRequest(out PercussionRequest? request)
        {
            lock (_queueLock)
            {
                if (_pendingRequests.Count == 0)
                {
                    request = null;
                    return false;
                }
                request = _pendingRequests.Dequeue();
                return true;
            }
        }

        private static void ProcessPercussionQueue()
        {
            PercussionOutputChoice? currentOutput = null;
            try
            {
                while (true)
                {
                    if (!TryDequeueRequest(out PercussionRequest? request) || request == null)
                    {
                        StopCurrentPulse(ref currentOutput);
                        lock (_queueLock)
                        {
                            if (_pendingRequests.Count == 0)
                            {
                                _queueWorkerRunning = false;
                                return;
                            }
                        }
                        continue;
                    }

                    if (request.Output == PercussionOutputChoice.SoundDevice)
                    {
                        QueueMixedSoundDeviceHit(request);
                        if (request.Completion != null)
                        {
                            int delay = request.CompletionDelayMs;
                            var ct = request.CancellationToken;
                            var tcs = request.Completion;
                            Task.Delay(delay, ct).ContinueWith(t =>
                            {
                                if (ct.IsCancellationRequested) tcs.TrySetCanceled(ct);
                                else tcs.TrySetResult(true);
                            }, TaskContinuationOptions.ExecuteSynchronously);
                        }
                    }
                    else
                    {
                        PlayQueuedRequestSystemSpeaker(request, ref currentOutput);
                    }
                }
            }
            finally
            {
                StopCurrentPulse(ref currentOutput);
                lock (_queueLock) { _queueWorkerRunning = false; }
            }
        }

        private static void PlayQueuedRequestSystemSpeaker(PercussionRequest request, ref PercussionOutputChoice? currentOutput)
        {
            if (request.CancellationToken.IsCancellationRequested)
            {
                request.Completion?.TrySetCanceled();
                return;
            }

            StopCurrentPulse(ref currentOutput);
            PreciseWaitMs(RetriggerGapMs, request.CancellationToken);

            long startedAt = Stopwatch.GetTimestamp();
            int lastFrequency = int.MinValue;
            bool started = false;
            bool completionSignaled = false;
            double firstAudibleStartMs = -1;

            while (true)
            {
                if (request.CancellationToken.IsCancellationRequested) break;

                double elapsedMs = ElapsedMilliseconds(startedAt);

                if (!completionSignaled && elapsedMs >= request.CompletionDelayMs)
                {
                    request.Completion?.TrySetResult(true);
                    completionSignaled = true;
                }

                if (elapsedMs >= request.DurationMs) break;

                bool pastMinimumHold = firstAudibleStartMs >= 0 && (elapsedMs - firstAudibleStartMs) >= MinAudibleSystemSpeakerMs;

                bool queueWantsCutoff = false;
                lock (_queueLock)
                {
                    double minBodyMs = GetMinimumBodyMs(request.Percussion);
                    if (_pendingRequests.Count > 0 && elapsedMs >= Math.Min(minBodyMs, request.DurationMs))
                        queueWantsCutoff = true;
                }

                if (queueWantsCutoff && pastMinimumHold) break;

                GetPlaybackSignalCore(request, elapsedMs, out int frequency, out bool audible);

                if (!audible)
                {
                    if (pastMinimumHold)
                    {
                        StopCurrentPulse(ref currentOutput);
                        lastFrequency = int.MinValue;
                        firstAudibleStartMs = -1;
                    }
                }
                else if (!started || !currentOutput.HasValue || frequency != lastFrequency)
                {
                    StartOrUpdatePulse(PercussionOutputChoice.SystemSpeaker, frequency, SynthWave.Square, ref currentOutput);
                    lastFrequency = frequency;
                    started = true;
                    if (firstAudibleStartMs < 0) firstAudibleStartMs = elapsedMs;
                }

                double untilChangeMs = GetTimeUntilSignalChangeMs(request, elapsedMs);
                PreciseWaitMs(Math.Max(0.5, untilChangeMs), request.CancellationToken);
            }

            StopCurrentPulse(ref currentOutput);
            if (!completionSignaled) request.Completion?.TrySetResult(started);
        }

        private static double ElapsedMilliseconds(long startedAt)
        {
            long ticks = Stopwatch.GetTimestamp() - startedAt;
            return ticks * 1000.0 / Stopwatch.Frequency;
        }

        private static double GetTimeUntilSignalChangeMs(PercussionRequest request, double elapsedMs)
        {
            if (request.Profile.BodyWave == SynthWave.Noise || IsMetalCymbal(request.Percussion))
            {
                // Dynamic time-step jitter (0.6ms - 1.2ms) breaks hardware clock resonance
                uint tHash = unchecked((uint)(request.RandomSeed ^ ((int)(elapsedMs * 5.1) * 1103515245u + 12345u)));
                double tJitter = ((tHash & 0x00FFFFFF) / 16777215.0);
                return 0.6 + tJitter * 0.6;
            }

            if (elapsedMs < request.Profile.AttackMs)
                return Math.Max(0.5, request.Profile.AttackMs - elapsedMs);

            return 2.5;
        }

        private static void GetPlaybackSignalCore(
            PercussionRequest request,
            double elapsedMs,
            out int frequency,
            out bool audible)
        {
            var prof = request.Profile;
            double duration = Math.Max(1.0, request.DurationMs);
            double progress = Math.Clamp(elapsedMs / duration, 0.0, 1.0);

            // 1. Pure Tonal Instruments (Cowbell, Agogo, Metronome Bell)
            bool isPureTonal = prof.BodyWave == SynthWave.Square || prof.BodyWave == SynthWave.Sine;
            if (isPureTonal)
            {
                if (elapsedMs < prof.AttackMs && request.DurationMs >= 50)
                {
                    frequency = ClampPercussionFrequency(prof.AttackFrequency);
                    audible = true;
                    return;
                }

                if (prof.DoesSweep)
                {
                    double logSweep = 1.0 - Math.Pow(progress, 0.45);
                    double baseFreq = prof.BodyEndFreq + (prof.BodyStartFreq - prof.BodyEndFreq) * logSweep;
                    frequency = ClampPercussionFrequency(baseFreq);
                }
                else
                {
                    frequency = ClampPercussionFrequency(prof.BodyStartFreq);
                }
                audible = progress < 0.95;
                return;
            }

            // 2. Kicks & Toms (Low pitch sweep with low-range jitter)
            if (prof.DoesSweep)
            {
                double logSweep = 1.0 - Math.Pow(progress, 0.5);
                double centerFreq = prof.BodyEndFreq + (prof.BodyStartFreq - prof.BodyEndFreq) * logSweep;

                int slot = (int)(elapsedMs / 1.5);
                uint hash = unchecked((uint)(request.RandomSeed ^ (slot * 1664525u + 1013904223u)));
                double j = ((hash & 0x00FFFFFF) / 16777215.0);

                double factor = 0.65 + j * 0.45;
                frequency = ClampPercussionFrequency(centerFreq * factor);
                audible = progress < 0.95;
                return;
            }

            // 3. Noise Instruments (Snare, Hi-Hats, Cymbals, Claps, Clicks)
            // Non-periodic uniform pseudo-random frequency sampling
            uint nHash = unchecked((uint)(request.RandomSeed ^ ((int)(elapsedMs * 7.31) * 1664525u + 1013904223u)));
            double jitter = ((nHash & 0x00FFFFFF) / 16777215.0);

            if (IsMetalCymbal(request.Percussion))
            {
                // Continuous spectrum spread across 700 Hz - 2300 Hz
                double baseF = 700.0 + jitter * 1600.0;
                frequency = ClampPercussionFrequency(baseF);
                audible = progress < 0.90;
            }
            else if (IsClick(request.Percussion))
            {
                double baseF = 350.0 + jitter * 1250.0;
                frequency = ClampPercussionFrequency(baseF);
                audible = progress < 0.70;
            }
            else // Snare, Hand Clap, General Noise
            {
                // Full non-oscillating continuous noise dispersion
                double baseF = 200.0 + jitter * 2000.0;
                frequency = ClampPercussionFrequency(baseF);
                audible = progress < 0.90;
            }
        }

        private static void PreciseWaitMs(double ms, CancellationToken ct)
        {
            if (ms <= 0 || ct.IsCancellationRequested) return;

            long start = Stopwatch.GetTimestamp();
            long targetTicks = start + (long)(ms * Stopwatch.Frequency / 1000.0);
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                if (ct.IsCancellationRequested) return;
                Thread.SpinWait(10);
            }
        }

        public static void GetPlaybackSignalCore(MidiPercussion percussion, double elapsedMs, out int frequency, out bool audible)
        {
            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(percussion);
            var dummyReq = new PercussionRequest(percussion, CancellationToken.None, prof.DurationMs, prof.DurationMs, output, prof, 100, null);
            GetPlaybackSignalCore(dummyReq, elapsedMs, out frequency, out audible);
        }

        public static int GetMidiFrameDurationMs(MidiPercussion percussion, int availableFrameMs, bool melodyAlsoPlaying)
        {
            int naturalMs = GetNaturalDurationMs(percussion);
            if (availableFrameMs <= 0) return 0;
            if (!melodyAlsoPlaying) return Math.Min(availableFrameMs, naturalMs);

            int minBodyMs = (int)Math.Ceiling(GetMinimumBodyMs(percussion));
            return Math.Clamp(Math.Min(minBodyMs, naturalMs), 1, availableFrameMs);
        }

        public static int GetNaturalDurationMs(MidiPercussion percussion) => GetProfile(percussion).DurationMs;

        private static PercussionOutputChoice GetPercussionPlaybackOutput()
        {
            return TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;
        }

        private static double GetMinimumBodyMs(MidiPercussion p)
        {
            if (IsClick(p)) return 20.0;
            if (IsKick(p)) return 45.0;
            if (IsSnare(p)) return 45.0;
            if (IsShortCymbal(p)) return 35.0;
            if (IsCymbalOrLongRing(p)) return 60.0;
            return 30.0;
        }
    }
}