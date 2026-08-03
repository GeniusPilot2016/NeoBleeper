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

        private const double RetriggerGapMs = 0.35;

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
            (int)Math.Round(Math.Clamp(frequency, 50.0, 15000.0));

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
            int sampleCount = System.Math.Max(1,
                (int)System.Math.Ceiling(request.DurationMs * PercussionSampleRate / 1000.0));
            var result = new float[sampleCount];
            double velocity01 = request.Velocity / 127.0;
            double masterGain = 0.5 + velocity01 * 0.35;

            uint rng = unchecked((uint)request.RandomSeed);
            double lpLow = 0.0, lpMid = 0.0, lpHigh = 0.0;
            double previousMid = 0.0;
            double brownNoise = 0.0; // Added for body warmth/anti-squeakiness
            double dcBlockX = 0.0, dcBlockY = 0.0;
            double tonePhase = 0.0;

            bool kick = request.Percussion is MidiPercussion.KickDrum or MidiPercussion.BassDrum;
            bool tom = request.Percussion is MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2
                or MidiPercussion.LowTom or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom
                or MidiPercussion.HighTom or MidiPercussion.HighBongo or MidiPercussion.LowBongo
                or MidiPercussion.Conga or MidiPercussion.CongaDeadStroke or MidiPercussion.Tumba
                or MidiPercussion.HighTimbale or MidiPercussion.LowTimbale or MidiPercussion.Surdu
                or MidiPercussion.SurduDeadStroke;
            bool snare = request.Percussion is MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum
                or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush;
            bool cymbal = IsMetalCymbal(request.Percussion);
            bool shaker = request.Percussion is MidiPercussion.Shaker or MidiPercussion.Maracas
                or MidiPercussion.Cabasa or MidiPercussion.Tambourine or MidiPercussion.SleighBell
                or MidiPercussion.GuiroShort or MidiPercussion.GuiroLong or MidiPercussion.OceanDrum;
            bool isTonalSquare = request.Profile.BodyWave == SynthWave.Square;

            for (int i = 0; i < sampleCount; i++)
            {
                double elapsedMs = i * 1000.0 / PercussionSampleRate;
                double progress = System.Math.Clamp(elapsedMs / request.DurationMs, 0.0, 1.0);

                rng ^= rng << 13;
                rng ^= rng >> 17;
                rng ^= rng << 5;
                double white = ((rng & 0x00FFFFFF) / 8388607.5) - 1.0;

                // Blend integrated brown noise for organic acoustic weight (eliminates thin squeakiness)
                brownNoise = Math.Clamp(brownNoise + white * 0.04, -1.0, 1.0);

                double lowCut = kick ? 120.0 : tom ? 250.0 : 800.0;
                double midCut = snare ? 2500.0 : cymbal ? 5000.0 : 2000.0;
                double highCut = cymbal ? 10000.0 : shaker ? 11000.0 : 7500.0;

                double aLow = 1.0 - System.Math.Exp(-2.0 * System.Math.PI * lowCut / PercussionSampleRate);
                double aMid = 1.0 - System.Math.Exp(-2.0 * System.Math.PI * midCut / PercussionSampleRate);
                double aHigh = 1.0 - System.Math.Exp(-2.0 * System.Math.PI * highCut / PercussionSampleRate);
                lpLow += aLow * ((white * 0.5 + brownNoise * 0.5) - lpLow);
                previousMid = lpMid;
                lpMid += aMid * (white - lpMid);
                lpHigh += aHigh * (white - lpHigh);

                double lowBand = lpLow;
                double midBand = lpMid - lpLow;
                double highBand = white - lpHigh;
                double edgeBand = lpMid - previousMid;

                double attack = System.Math.Exp(-elapsedMs / 3.0);
                double body = System.Math.Exp(-elapsedMs / System.Math.Max(30.0, request.DurationMs * 0.55));
                double tail = System.Math.Pow(System.Math.Max(0.0, 1.0 - progress),
                    cymbal ? 0.7 : shaker ? 1.2 : request.Profile.DecayShape);

                double toneVal = 0.0;
                if (isTonalSquare || request.Profile.BodyWave == SynthWave.Sine)
                {
                    double currentFreq = request.Profile.BodyStartFreq;
                    if (request.Profile.DoesSweep)
                    {
                        double logSweep = 1.0 - System.Math.Pow(progress, 0.45);
                        currentFreq = request.Profile.BodyEndFreq + (request.Profile.BodyStartFreq - request.Profile.BodyEndFreq) * logSweep;
                    }
                    double phaseInc = currentFreq / PercussionSampleRate;
                    tonePhase = (tonePhase + phaseInc) % 1.0;
                    toneVal = request.Profile.BodyWave == SynthWave.Square
                        ? (tonePhase < 0.5 ? 0.8 : -0.8)
                        : Math.Sin(tonePhase * 2.0 * Math.PI);
                }

                double source;
                if (kick)
                {
                    source = (toneVal * 0.9 + lowBand * 1.5 + brownNoise * 0.6) * body + edgeBand * 1.2 * attack;
                }
                else if (tom)
                {
                    source = (toneVal * 0.7 + lowBand * 1.1 + brownNoise * 0.4) * body + midBand * 0.7 + edgeBand * 1.0 * attack;
                }
                else if (snare)
                {
                    source = (midBand * 1.2 + brownNoise * 0.3) * body + highBand * tail * 0.6 + edgeBand * 1.8 * attack;
                }
                else if (cymbal)
                {
                    double effectiveDuration = System.Math.Max(request.DurationMs, IsShortCymbal(request.Percussion) ? 150.0 : 400.0);
                    double cymbalProgress = System.Math.Clamp(elapsedMs / effectiveDuration, 0.0, 1.0);
                    double cymbalWash = System.Math.Pow(1.0 - cymbalProgress, IsShortCymbal(request.Percussion) ? 1.5 : 0.8);

                    source = (highBand * 0.85 + midBand * 0.4) * cymbalWash + edgeBand * 1.0 * attack;
                }
                else if (shaker)
                {
                    source = highBand * tail * 0.75 + edgeBand * 1.0 * attack;
                }
                else if (isTonalSquare || request.Profile.BodyWave == SynthWave.Sine)
                {
                    source = toneVal * body + edgeBand * 1.2 * attack;
                }
                else
                {
                    source = (toneVal * 0.4 + midBand * 0.8) * body + edgeBand * 1.8 * attack;
                }

                double fadeIn = System.Math.Min(1.0, elapsedMs / 2.0);
                double sample = source * masterGain * fadeIn;

                double dcBlocked = sample - dcBlockX + 0.995 * dcBlockY;
                dcBlockX = sample;
                dcBlockY = dcBlocked;
                result[i] = (float)(System.Math.Tanh(dcBlocked * 1.05) * 0.85);
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

        private static PercussionProfile GetProfile(MidiPercussion percussion, PercussionOutputChoice output)
        {
            return percussion switch
            {
                // Distinct, Rich Drum Profiles (Lower frequencies prevent squeakiness)
                MidiPercussion.KickDrum or MidiPercussion.BassDrum =>
                    new PercussionProfile(output == PercussionOutputChoice.SystemSpeaker ? SynthWave.Square : SynthWave.Noise,
                        true, 55, 110, 110, density: 0.95, holdRatio: 0.02, attackFrequency: 130, attackMs: 1.5, decayShape: 3.5, pitchJitter: 0.005),

                MidiPercussion.HighTom =>
                    new PercussionProfile(SynthWave.Noise, false, 190, 190, 115, density: 0.9, holdRatio: 0.05,
                        attackFrequency: 240, attackMs: 1.5, decayShape: 2.5, pitchJitter: 0.01),
                MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom =>
                    new PercussionProfile(SynthWave.Noise, false, 130, 130, 130, density: 0.9, holdRatio: 0.05,
                        attackFrequency: 180, attackMs: 1.8, decayShape: 2.3, pitchJitter: 0.01),
                MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 =>
                    new PercussionProfile(SynthWave.Noise, false, 90, 90, 150, density: 0.9, holdRatio: 0.06,
                        attackFrequency: 130, attackMs: 2.0, decayShape: 2.0, pitchJitter: 0.01),

                MidiPercussion.HighBongo or MidiPercussion.HighTimbale =>
                    new PercussionProfile(SynthWave.Noise, false, 220, 220, 85, density: 0.85, holdRatio: 0.05,
                        attackFrequency: 300, attackMs: 1.2, decayShape: 2.8, pitchJitter: 0.015),
                MidiPercussion.LowBongo or MidiPercussion.Conga or MidiPercussion.Tumba or MidiPercussion.LowTimbale =>
                    new PercussionProfile(SynthWave.Noise, false, 150, 150, 115, density: 0.85, holdRatio: 0.05,
                        attackFrequency: 220, attackMs: 1.5, decayShape: 2.4, pitchJitter: 0.015),
                MidiPercussion.CongaDeadStroke or MidiPercussion.Surdu or MidiPercussion.SurduDeadStroke =>
                    new PercussionProfile(SynthWave.Noise, false, 100, 100, 135, density: 0.85, holdRatio: 0.04,
                        attackFrequency: 150, attackMs: 1.8, decayShape: 3.0, pitchJitter: 0.015),

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush =>
                    new PercussionProfile(SynthWave.Noise, false, 320, 320, 120, density: 0.85, holdRatio: 0.02,
                        attackFrequency: 450, attackMs: 1.2, decayShape: 2.8, pitchJitter: 0.06),

                // Balanced Click and Wood Profiles
                MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets =>
                    new PercussionProfile(SynthWave.Noise, false, 1200, 1200, 22, density: 0.9, holdRatio: 0.01,
                        attackFrequency: 1600, attackMs: 0.4, decayShape: 4.5, pitchJitter: 0.03),

                MidiPercussion.Claves or MidiPercussion.HighWoodblock or MidiPercussion.WoodBlock =>
                    new PercussionProfile(SynthWave.Noise, false, 1400, 1400, 25, density: 0.85, holdRatio: 0.01,
                        attackFrequency: 1800, attackMs: 0.4, decayShape: 4.2, pitchJitter: 0.02),
                MidiPercussion.LowWoodblock =>
                    new PercussionProfile(SynthWave.Noise, false, 950, 950, 28, density: 0.85, holdRatio: 0.01,
                        attackFrequency: 1200, attackMs: 0.4, decayShape: 4.0, pitchJitter: 0.02),

                MidiPercussion.Whip =>
                    new PercussionProfile(SynthWave.Noise, false, 1000, 1000, 55, density: 0.9, holdRatio: 0.01,
                        attackFrequency: 1400, attackMs: 0.3, decayShape: 4.0, pitchJitter: 0.04),
                MidiPercussion.ScratchPush or MidiPercussion.ScratchPull =>
                    new PercussionProfile(SynthWave.Noise, true, 800, 2200, 95, density: 0.8, holdRatio: 0.02,
                        attackFrequency: 1200, attackMs: 0.8, decayShape: 2.2, pitchJitter: 0.08),
                MidiPercussion.GuiroShort =>
                    new PercussionProfile(SynthWave.Noise, false, 900, 900, 105, density: 0.85, holdRatio: 0.03,
                        attackFrequency: 1100, attackMs: 1.0, decayShape: 2.5, pitchJitter: 0.04),
                MidiPercussion.GuiroLong =>
                    new PercussionProfile(SynthWave.Noise, false, 750, 750, 310, density: 0.85, holdRatio: 0.05,
                        attackFrequency: 950, attackMs: 1.2, decayShape: 1.9, pitchJitter: 0.04),
                MidiPercussion.OceanDrum =>
                    new PercussionProfile(SynthWave.Noise, true, 250, 600, 600, density: 0.5, holdRatio: 0.1,
                        attackFrequency: 300, attackMs: 10.0, decayShape: 1.2, pitchJitter: 0.03),

                // Metallic / Tonal Profiles
                MidiPercussion.Cowbell =>
                    new PercussionProfile(SynthWave.Sine, false, 560, 560, 115, holdRatio: 0.05,
                        attackFrequency: 750, attackMs: 0.8, decayShape: 2.2, pitchJitter: 0.01),
                MidiPercussion.MetronomeBell =>
                    new PercussionProfile(SynthWave.Sine, false, 1200, 1200, 75, holdRatio: 0.02,
                        attackFrequency: 1500, attackMs: 0.6, decayShape: 2.8, pitchJitter: 0.01),
                MidiPercussion.RideBell =>
                    new PercussionProfile(SynthWave.Sine, false, 800, 800, 230, holdRatio: 0.02,
                        attackFrequency: 1100, attackMs: 0.8, decayShape: 1.8, pitchJitter: 0.01),
                MidiPercussion.TriangleOpen =>
                    new PercussionProfile(SynthWave.Sine, false, 3200, 3200, 360, holdRatio: 0.01,
                        attackFrequency: 3800, attackMs: 0.4, decayShape: 1.5, pitchJitter: 0.01),
                MidiPercussion.TriangleMute =>
                    new PercussionProfile(SynthWave.Sine, false, 3200, 3200, 48, holdRatio: 0.01,
                        attackFrequency: 3800, attackMs: 0.4, decayShape: 3.8, pitchJitter: 0.01),
                MidiPercussion.HighAgogo =>
                    new PercussionProfile(SynthWave.Sine, false, 900, 900, 95, holdRatio: 0.02,
                        attackFrequency: 1150, attackMs: 0.6, decayShape: 2.4, pitchJitter: 0.01),
                MidiPercussion.LowAgogo =>
                    new PercussionProfile(SynthWave.Sine, false, 600, 600, 115, holdRatio: 0.02,
                        attackFrequency: 780, attackMs: 0.6, decayShape: 2.4, pitchJitter: 0.01),
                MidiPercussion.Vibraslap =>
                    new PercussionProfile(SynthWave.Sine, false, 380, 380, 260, holdRatio: 0.02,
                        attackFrequency: 520, attackMs: 0.5, decayShape: 1.8, pitchJitter: 0.05),
                MidiPercussion.BellTree =>
                    new PercussionProfile(SynthWave.Sine, false, 1600, 3000, 400, holdRatio: 0.01,
                        attackFrequency: 2000, attackMs: 0.5, decayShape: 1.4, pitchJitter: 0.03),

                MidiPercussion.WhistleShort =>
                    new PercussionProfile(SynthWave.Sine, false, 1800, 1800, 75, holdRatio: 0.1,
                        attackFrequency: 1800, attackMs: 0.8, decayShape: 1.4, pitchJitter: 0.01),
                MidiPercussion.WhistleLong =>
                    new PercussionProfile(SynthWave.Sine, false, 1800, 1800, 290, holdRatio: 0.1,
                        attackFrequency: 1800, attackMs: 0.8, decayShape: 1.2, pitchJitter: 0.01),
                MidiPercussion.CuicaHigh =>
                    new PercussionProfile(SynthWave.Sine, true, 450, 750, 95, holdRatio: 0.05,
                        attackFrequency: 450, attackMs: 1.8, decayShape: 2.2, pitchJitter: 0.015),
                MidiPercussion.CuicaLow =>
                    new PercussionProfile(SynthWave.Sine, true, 260, 450, 115, holdRatio: 0.05,
                        attackFrequency: 260, attackMs: 1.8, decayShape: 2.2, pitchJitter: 0.015),
                MidiPercussion.Laser =>
                    new PercussionProfile(SynthWave.Square, true, 1000, 150, 85, holdRatio: 0.01,
                        attackFrequency: 1200, attackMs: 1.0, decayShape: 3.0, pitchJitter: 0.01),

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal =>
                    new PercussionProfile(SynthWave.Noise, false, 3500, 3500, 800, density: 0.7, holdRatio: 0.01,
                        attackFrequency: 4500, attackMs: 1.2, decayShape: 1.6, pitchJitter: 0.1),
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                    new PercussionProfile(SynthWave.Noise, false, 3200, 3200, 650, density: 0.6, holdRatio: 0.01,
                        attackFrequency: 4100, attackMs: 1.0, decayShape: 1.7, pitchJitter: 0.08),

                MidiPercussion.HiHatClosed =>
                    new PercussionProfile(SynthWave.Noise, false, 3800, 3800, 90, density: 0.85, holdRatio: 0.02,
                        attackFrequency: 4800, attackMs: 0.5, decayShape: 3.0, pitchJitter: 0.06),
                MidiPercussion.HiHatOpen =>
                    new PercussionProfile(SynthWave.Noise, false, 3600, 3600, 380, density: 0.6, holdRatio: 0.01,
                        attackFrequency: 4600, attackMs: 1.2, decayShape: 1.8, pitchJitter: 0.08),
                MidiPercussion.HiHatFoot =>
                    new PercussionProfile(SynthWave.Noise, false, 3400, 3400, 105, density: 0.8, holdRatio: 0.02,
                        attackFrequency: 4300, attackMs: 0.6, decayShape: 2.8, pitchJitter: 0.05),

                MidiPercussion.HandClap =>
                    new PercussionProfile(SynthWave.Noise, false, 130, 130, 145, density: 0.85, holdRatio: 0.02,
                        attackFrequency: 150, attackMs: 0.5, decayShape: 2.0, pitchJitter: 0.12),

                _ => new PercussionProfile(SynthWave.Noise, false, 250, 250, 105, density: 0.6, holdRatio: 0.02,
                    attackFrequency: 350, attackMs: 1.2, decayShape: 2.5, pitchJitter: 0.06)
            };
        }

        private static bool IsKick(MidiPercussion p) =>
            p is MidiPercussion.KickDrum or MidiPercussion.BassDrum;

        private static bool IsTomOrBongo(MidiPercussion p) =>
            p is MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 or MidiPercussion.LowTom
                or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom or MidiPercussion.HighTom
                or MidiPercussion.HighBongo or MidiPercussion.LowBongo or MidiPercussion.Conga
                or MidiPercussion.CongaDeadStroke or MidiPercussion.Tumba or MidiPercussion.HighTimbale
                or MidiPercussion.LowTimbale or MidiPercussion.Surdu or MidiPercussion.SurduDeadStroke;

        private static bool IsSnare(MidiPercussion p) =>
            p is MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum
                or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush;

        private static bool IsClick(MidiPercussion p) =>
            p is MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick
                or MidiPercussion.MetronomeClick or MidiPercussion.Castanets or MidiPercussion.Claves
                or MidiPercussion.HighWoodblock or MidiPercussion.WoodBlock or MidiPercussion.LowWoodblock;

        private static bool IsCymbalOrLongRing(MidiPercussion p) =>
            p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2
                or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2
                or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal
                or MidiPercussion.RideBell or MidiPercussion.HiHatOpen
                or MidiPercussion.TriangleOpen or MidiPercussion.OceanDrum
                or MidiPercussion.BellTree or MidiPercussion.Vibraslap;

        private static bool IsShortCymbal(MidiPercussion p) =>
            p is MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot or MidiPercussion.TriangleMute;

        private static bool IsMetalCymbal(MidiPercussion p) =>
            IsCymbalOrLongRing(p) || IsShortCymbal(p);

        public static void PlayPercussion(MidiPercussion p, System.Threading.CancellationToken ct = default, int maxMs = 5000, int velocity = 100)
        {
            if (ct.IsCancellationRequested) return;

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p, output);

            int duration = IsCymbalOrLongRing(p)
                ? System.Math.Max(prof.DurationMs, 400)
                : IsShortCymbal(p)
                    ? System.Math.Max(120, System.Math.Min(maxMs, prof.DurationMs))
                    : System.Math.Max(80, System.Math.Min(maxMs, prof.DurationMs));

            EnqueuePercussion(new PercussionRequest(p, ct, duration, duration, output, prof, velocity, null));
        }

        public static Task PlayPercussionForDurationAsync(
            MidiPercussion p,
            int durationMs,
            CancellationToken ct = default,
            int velocity = 100)
        {
            if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p, output);

            if (durationMs <= 0) return Task.CompletedTask;

            int minBodyMs = (int)Math.Ceiling(GetMinimumBodyMs(p));
            int audibleDurationMs = IsCymbalOrLongRing(p)
                ? prof.DurationMs
                : Math.Max(durationMs, minBodyMs);

            var completion = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            EnqueuePercussion(new PercussionRequest(p, ct, audibleDurationMs, durationMs, output, prof, velocity, completion));
            return completion.Task;
        }

        public static Task PlayPercussionSliceAsync(
            MidiPercussion p, int sliceDurationMs, CancellationToken ct = default, int velocity = 100)
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

            if (startWorker) StartQueueWorker();
        }

        private static void StartQueueWorker()
        {
            Task.Run(ProcessPercussionQueue);
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
                                if (ct.IsCancellationRequested)
                                    tcs.TrySetCanceled(ct);
                                else
                                    tcs.TrySetResult(true);
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

                lock (_queueLock)
                {
                    double minBodyMs = GetMinimumBodyMs(request.Percussion);
                    if (_pendingRequests.Count > 0 && elapsedMs >= Math.Min(minBodyMs, request.DurationMs))
                        break;
                }

                GetPlaybackSignalCore(request, elapsedMs, out int frequency, out bool audible);

                if (!audible)
                {
                    StopCurrentPulse(ref currentOutput);
                    lastFrequency = int.MinValue;
                }
                else if (!started || !currentOutput.HasValue || frequency != lastFrequency)
                {
                    StartOrUpdatePulse(PercussionOutputChoice.SystemSpeaker, frequency, SynthWave.Square, ref currentOutput);
                    lastFrequency = frequency;
                    started = true;
                }

                double untilChangeMs = GetTimeUntilSignalChangeMs(request, elapsedMs);
                PreciseWaitMs(Math.Max(0.1, untilChangeMs), request.CancellationToken);
            }

            StopCurrentPulse(ref currentOutput);

            if (!completionSignaled)
                request.Completion?.TrySetResult(started);
        }

        private static double ElapsedMilliseconds(long startedAt)
        {
            long ticks = Stopwatch.GetTimestamp() - startedAt;
            return ticks * 1000.0 / Stopwatch.Frequency;
        }

        private static double GetTimeUntilSignalChangeMs(PercussionRequest request, double elapsedMs)
        {
            if (request.Output == PercussionOutputChoice.SystemSpeaker && request.Profile.BodyWave == SynthWave.Noise)
            {
                bool isShort = request.DurationMs <= 60;
                if (request.Percussion == MidiPercussion.HandClap) return 6.0;
                if (IsKick(request.Percussion)) return isShort ? 2.0 : 5.0;
                if (IsTomOrBongo(request.Percussion)) return isShort ? 2.0 : 4.5;
                if (IsClick(request.Percussion)) return 1.0;
                if (IsMetalCymbal(request.Percussion)) return 2.0;

                return isShort ? 1.5 : 3.5;
            }

            if (elapsedMs < request.Profile.AttackMs)
                return Math.Max(0.1, request.Profile.AttackMs - elapsedMs);

            return 2.5;
        }

        private static void GetPlaybackSignalCore(
            PercussionRequest request,
            double elapsedMs,
            out int frequency,
            out bool audible)
        {
            var prof = request.Profile;
            double duration = System.Math.Max(1.0, request.DurationMs);
            double progress = System.Math.Clamp(elapsedMs / duration, 0.0, 1.0);
            double velocity01 = request.Velocity / 127.0;

            bool isShortClickPercussion = prof.DurationMs <= 50 && prof.BodyWave == SynthWave.Noise && IsClick(request.Percussion);
            if (isShortClickPercussion)
            {
                double smoothDecay = System.Math.Max(0.0, 1.0 - (progress * 1.4));
                frequency = ClampPercussionFrequency(prof.BodyStartFreq * (0.95 + 0.1 * smoothDecay));
                audible = progress < 0.80 && smoothDecay > 0.05;
                return;
            }

            if (elapsedMs < prof.AttackMs)
            {
                frequency = ClampPercussionFrequency(prof.AttackFrequency * (0.98 + 0.04 * velocity01));
                audible = true;
                return;
            }

            if (prof.BodyWave == SynthWave.Square || prof.BodyWave == SynthWave.Sine)
            {
                if (prof.DoesSweep)
                {
                    double logSweep = 1.0 - System.Math.Pow(progress, 0.45);
                    double baseFreq = prof.BodyEndFreq + (prof.BodyStartFreq - prof.BodyEndFreq) * logSweep;
                    frequency = ClampPercussionFrequency(baseFreq);
                    audible = progress < 0.92;
                    return;
                }
                else
                {
                    frequency = ClampPercussionFrequency(prof.BodyStartFreq);
                    audible = progress < 0.95;
                    return;
                }
            }

            if (duration > 100.0)
            {
                double decayThresh = System.Math.Pow(1.0 - progress, prof.DecayShape);
                double slotSizeMs = System.Math.Clamp(duration / 12.0, 0.4, 1.25);
                int mSlotCheck = (int)(elapsedMs / slotSizeMs);
                uint sHash = unchecked((uint)(request.RandomSeed + mSlotCheck * 2654435761u));
                double sRandom = ((sHash & 0xFFFF) / 65535.0);

                if (sRandom > decayThresh)
                {
                    frequency = 0;
                    audible = false;
                    return;
                }
            }

            double slotSizeMsJitter = System.Math.Clamp(duration / 12.0, 0.4, 1.25);
            int mSlot = (int)(elapsedMs / slotSizeMsJitter);
            uint h = unchecked((uint)(request.RandomSeed ^ (mSlot * 1103515245u)));
            double jitter = ((h & 0xFF) / 255.0);

            if (IsMetalCymbal(request.Percussion))
            {
                double cymbalFreq = (jitter > 0.5) ? 3800.0 : 2800.0; // Lowered to prevent squeaky highs
                frequency = ClampPercussionFrequency(cymbalFreq + (jitter - 0.5) * 400.0);
            }
            else if (request.Percussion == MidiPercussion.HandClap || request.Percussion == MidiPercussion.Tambourine || request.Percussion == MidiPercussion.Shaker)
            {
                double t = elapsedMs - prof.AttackMs;
                double familyEnvelope = System.Math.Max(0.0, 1.0 - (progress * 1.2));
                if (t >= 0 && t < 120.0)
                {
                    double multiPeak = System.Math.Exp(-System.Math.Abs(t - 15.0) / 30.0) + 0.7 * System.Math.Exp(-System.Math.Abs(t - 45.0) / 40.0);
                    familyEnvelope *= System.Math.Clamp(multiPeak, 0.2, 1.0);
                }

                frequency = ClampPercussionFrequency(1100.0 + (jitter * 250.0)); // Adjusted band
                audible = familyEnvelope > 0.04 && progress < 0.98;
                return;
            }
            else if (IsClick(request.Percussion))
            {
                frequency = ClampPercussionFrequency(prof.BodyStartFreq * (0.95 + 0.1 * jitter));
                audible = progress < 0.75;
                return;
            }
            else
            {
                double bodyDensity = System.Math.Clamp(prof.NoiseDensity, 0.15, 1.0);
                double snareFreq = 900.0 + jitter * 300.0 + (1.0 - bodyDensity) * 100.0; // Warm mid band
                frequency = ClampPercussionFrequency(snareFreq);
            }

            audible = progress < 0.98;
        }

        private static void PreciseWaitMs(double ms, CancellationToken ct)
        {
            if (ms <= 0 || ct.IsCancellationRequested) return;

            long start = Stopwatch.GetTimestamp();
            long targetTicks = start + (long)(ms * Stopwatch.Frequency / 1000.0);
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                if (ct.IsCancellationRequested) return;
            }
        }

        public static void GetPlaybackSignalCore(
            MidiPercussion percussion,
            double elapsedMs,
            out int frequency,
            out bool audible)
        {
            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(percussion, output);

            var dummyReq = new PercussionRequest(
                percussion,
                CancellationToken.None,
                prof.DurationMs,
                prof.DurationMs,
                output,
                prof,
                100,
                null);

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

        public static int GetNaturalDurationMs(MidiPercussion percussion) =>
            GetProfile(percussion, PercussionOutputChoice.SystemSpeaker).DurationMs;

        private static PercussionOutputChoice GetPercussionPlaybackOutput()
        {
            return TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;
        }

        private static double GetMinimumBodyMs(MidiPercussion p)
        {
            if (IsClick(p)) return 12.0;
            if (IsKick(p)) return 55.0;
            if (IsSnare(p)) return 45.0;
            if (IsTomOrBongo(p)) return 50.0;
            if (p == MidiPercussion.HandClap) return 40.0;
            return 35.0;
        }
    }
}