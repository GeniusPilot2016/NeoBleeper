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
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

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
            Laser = 27,
            Whip = 28,
            ScratchPush = 29,
            ScratchPull = 30,
            StickClick = 31,
            SquareClick = 32,
            MetronomeClick = 33,
            MetronomeBell = 34,

            BassDrum = 35,
            KickDrum = 36,

            SideStick = 37,
            SnareCrossStick = SideStick,

            SnareDrum = 38,
            HandClap = 39,
            ElectricSnareDrum = 40,

            FloorTom2 = 41,
            HiHatClosed = 42,
            FloorTom1 = 43,
            HiHatFoot = 44,
            LowTom = 45,
            HiHatOpen = 46,
            LowMidTom = 47,
            HighMidTom = 48,
            CrashCymbal = 49,
            HighTom = 50,
            RideCymbal = 51,
            ChinaCymbal = 52,
            RideBell = 53,
            Tambourine = 54,
            SplashCymbal = 55,
            Cowbell = 56,
            CrashCymbal2 = 57,
            Vibraslap = 58,
            RideCymbal2 = 59,

            HighBongo = 60,
            LowBongo = 61,
            CongaDeadStroke = 62,
            Conga = 63,
            Tumba = 64,
            HighTimbale = 65,
            LowTimbale = 66,
            HighAgogo = 67,
            LowAgogo = 68,
            Cabasa = 69,
            Maracas = 70,
            WhistleShort = 71,
            WhistleLong = 72,

            GuiroShort = 73,
            Güiro = GuiroShort,

            GuiroLong = 74,

            Claves = 75,
            Clave = Claves,

            HighWoodblock = 76,
            WoodBlock = HighWoodblock,

            LowWoodblock = 77,
            CuicaHigh = 78,
            CuicaLow = 79,
            TriangleMute = 80,
            TriangleOpen = 81,

            Shaker = 82,
            SleighBell = 83,
            BellTree = 84,
            Castanets = 85,
            SurduDeadStroke = 86,
            Surdu = 87,

            SnareDrumRod = 91,
            OceanDrum = 92,
            SnareDrumBrush = 93
        }

        public enum PercussionOutputChoice { SystemSpeaker, SoundDevice }
        private enum SynthWave { Square, Triangle, Noise }

        private static int _globalSessionId = 0;
        private static int _activeSpeakerSession = 0;
        private static int _activeDeviceSession = 0;
        private static readonly object _hardwareLock = new object();

        private const int MinimumSystemSpeakerHitMs = 20;
        private const int MinimumSoundDeviceHitMs = 20;
        private const int MinimumSoundDeviceAttackMs = 12;

        private static int ClampPercussionFrequency(double frequency) => (int)Math.Round(Math.Clamp(frequency, 37.0, 15000.0));

        private static int EnsureAudibleDuration(PercussionOutputChoice output, int durationMs)
        {
            int minimum = output == PercussionOutputChoice.SoundDevice
                ? MinimumSoundDeviceHitMs
                : MinimumSystemSpeakerHitMs;
            return Math.Max(minimum, durationMs);
        }

        private static int GetSoundDeviceAttackDuration(
            int totalDurationMs, double requestedRatio, int maximumAttackMs)
        {
            if (totalDurationMs <= 0) return 0;

            int minimum = Math.Min(MinimumSoundDeviceAttackMs, totalDurationMs);
            int maximum = Math.Clamp(maximumAttackMs, minimum, totalDurationMs);
            int requested = (int)Math.Round(
                totalDurationMs * Math.Clamp(requestedRatio, 0.0, 1.0),
                MidpointRounding.AwayFromZero);

            return Math.Clamp(requested, minimum, maximum);
        }

        private static bool IsSessionActive(PercussionOutputChoice choice, int sessionId) =>
            choice == PercussionOutputChoice.SystemSpeaker ? Volatile.Read(ref _activeSpeakerSession) == sessionId : Volatile.Read(ref _activeDeviceSession) == sessionId;

        private static void StartPulse(PercussionOutputChoice outputChoice, int frequency, SynthWave waveType, int sessionId)
        {
            frequency = ClampPercussionFrequency(frequency);
            lock (_hardwareLock)
            {
                if (!IsSessionActive(outputChoice, sessionId)) return;

                switch (outputChoice)
                {
                    case PercussionOutputChoice.SystemSpeaker:
                        SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                        break;
                    case PercussionOutputChoice.SoundDevice:
                        var naudioWave = waveType switch { SynthWave.Noise => NAudio.Wave.SampleProviders.SignalGeneratorType.White, SynthWave.Triangle => NAudio.Wave.SampleProviders.SignalGeneratorType.Triangle, _ => NAudio.Wave.SampleProviders.SignalGeneratorType.Square };
                        SoundRenderingEngine.WaveSynthEngine.StartSynth(naudioWave, frequency);
                        break;
                }
            }
        }

        private static void StopPulse(PercussionOutputChoice outputChoice, int sessionId)
        {
            lock (_hardwareLock)
            {
                if (outputChoice == PercussionOutputChoice.SystemSpeaker && Volatile.Read(ref _activeSpeakerSession) == sessionId) SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                else if (outputChoice == PercussionOutputChoice.SoundDevice && Volatile.Read(ref _activeDeviceSession) == sessionId) SoundRenderingEngine.WaveSynthEngine.StopSynth();
            }
        }

        private static void PreciseWaitMs(double ms, CancellationToken ct)
        {
            if (ms <= 0) return;
            TimeSpan timeout = TimeSpan.FromMicroseconds(ms * 1000);
            try
            {
                bool timedOut = ct.WaitHandle.WaitOne(timeout);
                if (!timedOut && ct.IsCancellationRequested) return;
            }
            catch (ObjectDisposedException) { return; }
        }

        private readonly struct PercussionProfile
        {
            public readonly SynthWave BodyWave;
            public readonly bool DoesSweep;
            public readonly int BodyStartFreq;
            public readonly int BodyEndFreq;
            public readonly int DurationMs;
            public readonly double NoiseDensity;
            public readonly int HitCount;
            public readonly int HitGapMs;
            public readonly double HoldRatio;

            public PercussionProfile(SynthWave w, bool s, int start, int end, int dur,
                double density = 0.5, int hits = 1, int gap = 0, double holdRatio = 0.15)
            {
                BodyWave = w; DoesSweep = s; BodyStartFreq = start; BodyEndFreq = end; DurationMs = dur;
                NoiseDensity = density; HitCount = Math.Max(1, hits); HitGapMs = gap;
                HoldRatio = Math.Clamp(holdRatio, 0.0, 1.0);
            }
        }

        // --- CORE PERCUSSION PROFILES ---
        private static PercussionProfile GetProfile(
    MidiPercussion percussion,
    PercussionOutputChoice output)
        {
            // Very short sounds can disappear when rendering is block/buffer based.
            // Give every one-shot enough time to reach the output device.
            static PercussionProfile Profile(
                SynthWave wave,
                bool pitchSweep,
                int startFrequency,
                int endFrequency,
                int durationMs,
                double density = 1.0,
                int hits = 1,
                int gap = 0,
                double holdRatio = 0.10)
            {
                const int minimumAudibleDurationMs = 45;
                const int minimumTailAfterLastHitMs = 35;

                hits = Math.Max(1, hits);
                gap = Math.Max(0, gap);

                var repeatedHitDuration =
                    ((hits - 1) * gap) + minimumTailAfterLastHitMs;

                var safeDuration = Math.Max(
                    durationMs,
                    Math.Max(minimumAudibleDurationMs, repeatedHitDuration));

                return new PercussionProfile(
                    wave,
                    pitchSweep,
                    Math.Max(1, startFrequency),
                    Math.Max(1, endFrequency),
                    safeDuration,
                    density: Math.Clamp(density, 0.01, 1.0),
                    hits: hits,
                    gap: gap,
                    holdRatio: Math.Clamp(holdRatio, 0.01, 0.95));
            }

            // Triangle waves retain the drum pitch sweep on normal audio
            // devices without the buzzy, PC-speaker-like edge of a square wave.
            SynthWave drumBodyWave = output == PercussionOutputChoice.SoundDevice
                ? SynthWave.Triangle
                : SynthWave.Square;

            return percussion switch
            {
                MidiPercussion.KickDrum or MidiPercussion.BassDrum =>
                    Profile(drumBodyWave, true, 160, 55, 45, holdRatio: 0.08),

                MidiPercussion.HighTom =>
                    Profile(drumBodyWave, true, 280, 180, 55),
                MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom =>
                    Profile(drumBodyWave, true, 210, 130, 65),
                MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 =>
                    Profile(drumBodyWave, true, 150, 90, 80),
                // A fixed-frequency triangle is heard as a tiny plain beep on a
                // buffered sound device. Use a compact noise transient for clicks.
                MidiPercussion.SideStick or
                MidiPercussion.StickClick or
                MidiPercussion.SquareClick or
                MidiPercussion.MetronomeClick =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        7200,
                        7200,
                        18,
                        density: 0.32,
                        holdRatio: 0.04),

                // Keep the explicitly pitched metronome bell separate from clicks.
                MidiPercussion.MetronomeBell =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        1900,
                        1450,
                        28,
                        holdRatio: 0.04),

                MidiPercussion.Claves or MidiPercussion.Castanets =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        2400,
                        2400,
                        30,
                        holdRatio: 0.10),

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum =>
                    Profile(SynthWave.Noise, false, 3200, 3200, 75, density: 0.90),
                MidiPercussion.SnareDrumRod =>
                    Profile(SynthWave.Noise, false, 2800, 2800, 60, density: 0.70),
                MidiPercussion.SnareDrumBrush =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        2400,
                        2400,
                        140,
                        density: 0.45,
                        holdRatio: 0.15),

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        5500,
                        5500,
                        850,
                        density: 0.45,
                        holdRatio: 0.04),
                MidiPercussion.ChinaCymbal =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        6200,
                        6200,
                        650,
                        density: 0.55,
                        holdRatio: 0.04),
                MidiPercussion.SplashCymbal =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        6000,
                        6000,
                        220,
                        density: 0.45,
                        holdRatio: 0.05),
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        5000,
                        5000,
                        900,
                        density: 0.25,
                        holdRatio: 0.05),

                MidiPercussion.HiHatClosed =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        7500,
                        7500,
                        25,
                        density: 0.80,
                        holdRatio: 0.12),
                MidiPercussion.HiHatOpen =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        7000,
                        7000,
                        300,
                        density: 0.30,
                        holdRatio: 0.05),
                MidiPercussion.HiHatFoot =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        2800,
                        2800,
                        35,
                        density: 0.60),

                MidiPercussion.HandClap =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        2200,
                        2200,
                        35,
                        density: 0.70,
                        hits: 3,
                        gap: 6),
                MidiPercussion.Vibraslap =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        1800,
                        1800,
                        600,
                        density: 0.20,
                        holdRatio: 0.08),
                // Pure triangle woodblocks sound like ordinary beeps. These
                // compact noise knocks preserve a dry attack; the low block is
                // slightly longer and denser so the two remain distinguishable.
                MidiPercussion.HighWoodblock =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        6200,
                        6200,
                        26,
                        density: 0.28,
                        holdRatio: 0.03),

                MidiPercussion.LowWoodblock =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        3600,
                        3600,
                        42,
                        density: 0.72,
                        holdRatio: 0.06),



                MidiPercussion.HighBongo =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        280,
                        220,
                        50,
                        holdRatio: 0.12),
                MidiPercussion.LowBongo =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        200,
                        150,
                        65,
                        holdRatio: 0.12),
                MidiPercussion.CongaDeadStroke =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        220,
                        190,
                        30,
                        holdRatio: 0.08),
                MidiPercussion.Conga =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        230,
                        170,
                        80,
                        holdRatio: 0.12),
                MidiPercussion.Tumba =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        170,
                        120,
                        100,
                        holdRatio: 0.12),
                MidiPercussion.HighTimbale =>
                    Profile(SynthWave.Triangle, true, 450, 350, 60),
                MidiPercussion.LowTimbale =>
                    Profile(SynthWave.Triangle, true, 340, 240, 75),
                MidiPercussion.SurduDeadStroke =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        130,
                        100,
                        40,
                        holdRatio: 0.08),
                MidiPercussion.Surdu =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        120,
                        75,
                        120,
                        holdRatio: 0.12),

                MidiPercussion.Cowbell =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        820,
                        820,
                        140,
                        holdRatio: 0.08),
                MidiPercussion.RideBell =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        1250,
                        1250,
                        280,
                        holdRatio: 0.08),
                MidiPercussion.HighAgogo =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        900,
                        900,
                        90,
                        holdRatio: 0.08),
                MidiPercussion.LowAgogo =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        650,
                        650,
                        120,
                        holdRatio: 0.08),
                MidiPercussion.TriangleMute =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        3400,
                        3400,
                        30,
                        holdRatio: 0.15),
                MidiPercussion.TriangleOpen =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        3400,
                        3400,
                        800,
                        holdRatio: 0.05),
                MidiPercussion.SleighBell or MidiPercussion.BellTree =>
                    Profile(
                        SynthWave.Triangle,
                        false,
                        2400,
                        2400,
                        80,
                        hits: 3,
                        gap: 10),

                MidiPercussion.Maracas =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        5000,
                        5000,
                        55,
                        density: 0.35,
                        holdRatio: 0.12),
                MidiPercussion.Cabasa =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        4500,
                        4500,
                        70,
                        density: 0.45),
                MidiPercussion.Shaker or MidiPercussion.Tambourine =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        5500,
                        5500,
                        85,
                        density: 0.30),

                MidiPercussion.GuiroShort =>
                Profile(
                    SynthWave.Noise,
                    false,
                    3200,
                    3200,
                    65,
                    density: 0.55,
                    holdRatio: 0.12),
                MidiPercussion.GuiroLong =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        3200,
                        3200,
                        50,
                        density: 0.55,
                        hits: 4,
                        gap: 10),
                MidiPercussion.ScratchPush or MidiPercussion.ScratchPull =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        3800,
                        3800,
                        70,
                        density: 0.40,
                        holdRatio: 0.12),
                MidiPercussion.OceanDrum =>
                    Profile(
                        SynthWave.Noise,
                        false,
                        1600,
                        1600,
                        1200,
                        density: 0.12,
                        holdRatio: 0.12),

                MidiPercussion.WhistleShort =>
                    Profile(
                        SynthWave.Square,
                        false,
                        1800,
                        1800,
                        110,
                        holdRatio: 0.20),
                MidiPercussion.WhistleLong =>
                    Profile(
                        SynthWave.Square,
                        false,
                        1800,
                        1800,
                        600,
                        holdRatio: 0.25),
                MidiPercussion.Laser =>
                    Profile(
                        SynthWave.Square,
                        true,
                        1800,
                        200,
                        150,
                        holdRatio: 0.08),
                MidiPercussion.Whip =>
                    Profile(
                        SynthWave.Square,
                        true,
                        1400,
                        150,
                        90,
                        holdRatio: 0.08),
                MidiPercussion.CuicaHigh =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        750,
                        480,
                        100,
                        holdRatio: 0.12),
                MidiPercussion.CuicaLow =>
                    Profile(
                        SynthWave.Triangle,
                        true,
                        420,
                        230,
                        130,
                        holdRatio: 0.12),

                // An audible generic percussion sound for unrecognized values.
                _ => Profile(
                    SynthWave.Noise,
                    false,
                    2200,
                    2200,
                    80,
                    density: 0.65,
                    holdRatio: 0.15)
            };
        }

        // --- PLAYBACK ENGINE ---

        private static bool EnforceCleanOnset(PercussionOutputChoice output, int sessionId)
        {
            lock (_hardwareLock)
            {
                // A queued, stale playback task must never stop a newer hit.
                // This race is especially visible with very short drum frames.
                if (!IsSessionActive(output, sessionId))
                    return false;

                switch (output)
                {
                    case PercussionOutputChoice.SystemSpeaker:
                        SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                        break;
                    case PercussionOutputChoice.SoundDevice:
                        SoundRenderingEngine.WaveSynthEngine.StopSynth();
                        break;
                }

                return IsSessionActive(output, sessionId);
            }
        }

        public static void PlayPercussion(MidiPercussion p, CancellationToken ct = default, int maxMs = 5000, int velocity = 100)
        {
            int sid = Interlocked.Increment(ref _globalSessionId);
            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice ?
                PercussionOutputChoice.SoundDevice : PercussionOutputChoice.SystemSpeaker;

            if (output == PercussionOutputChoice.SystemSpeaker)
                Interlocked.Exchange(ref _activeSpeakerSession, sid);
            else
                Interlocked.Exchange(ref _activeDeviceSession, sid);

            Task.Run(() => ExecutePercussionPlayback(p, sid, ct, maxMs, velocity, output), ct);
        }

        private static void ExecutePercussionPlayback(MidiPercussion p, int sid, CancellationToken ct, int maxMs, int vel, PercussionOutputChoice output)
        {
            if (!IsSessionActive(output, sid)) return;

            if (!EnforceCleanOnset(output, sid)) return;

            var prof = GetProfile(p, output);

            int finalDuration = EnsureAudibleDuration(output, Math.Min(maxMs, prof.DurationMs));

            try
            {
                RenderProfile(output, sid, prof, ct, totalDurationOverrideMs: finalDuration);
            }
            finally { StopPulse(output, sid); }
        }

        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct = default, int velocity = 100)
        {
            if (durationMs <= 0) return Task.CompletedTask;
            int sid = Interlocked.Increment(ref _globalSessionId);

            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice ?
                PercussionOutputChoice.SoundDevice : PercussionOutputChoice.SystemSpeaker;

            if (output == PercussionOutputChoice.SystemSpeaker)
                Interlocked.Exchange(ref _activeSpeakerSession, sid);
            else
                Interlocked.Exchange(ref _activeDeviceSession, sid);

            durationMs = EnsureAudibleDuration(output, durationMs);
            return Task.Run(() => ExecutePercussionPlaybackForDuration(p, sid, ct, durationMs, velocity, output), ct);
        }

        private static void ExecutePercussionPlaybackForDuration(MidiPercussion p, int sid, CancellationToken ct, int durationMs, int vel, PercussionOutputChoice output)
        {
            if (!IsSessionActive(output, sid)) return;

            if (!EnforceCleanOnset(output, sid)) return;

            var prof = GetProfile(p, output);

            try
            {
                RenderProfile(output, sid, prof, ct, durationMs);
            }
            finally { StopPulse(output, sid); }
        }

        public static int GetMidiFrameDurationMs(MidiPercussion percussion, int availableFrameMs, bool melodyAlsoPlaying)
        {
            if (availableFrameMs <= 0) return 0;

            int naturalDurationMs = GetNaturalDurationMs(percussion);
            if (!melodyAlsoPlaying)
                return Math.Min(availableFrameMs, naturalDurationMs);

            const int melodyFloorMs = 8;
            if (availableFrameMs <= melodyFloorMs + 1)
                return Math.Max(1, availableFrameMs / 2);

            int preferredAttackMs = percussion switch
            {
                MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot or
                MidiPercussion.StickClick or MidiPercussion.SquareClick or
                MidiPercussion.MetronomeClick => 26,

                MidiPercussion.SideStick or MidiPercussion.Claves or
                MidiPercussion.Castanets or MidiPercussion.TriangleMute => 34,

                MidiPercussion.KickDrum or MidiPercussion.BassDrum => 48,

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or
                MidiPercussion.SnareDrumRod => 56,

                MidiPercussion.HandClap => 96,

                MidiPercussion.HighTom or MidiPercussion.HighMidTom or
                MidiPercussion.LowMidTom or MidiPercussion.LowTom or
                MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 or
                MidiPercussion.HighBongo or MidiPercussion.LowBongo or
                MidiPercussion.CongaDeadStroke or MidiPercussion.Conga or
                MidiPercussion.Tumba or MidiPercussion.HighTimbale or
                MidiPercussion.LowTimbale or MidiPercussion.SurduDeadStroke or
                MidiPercussion.Surdu => 62,

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or
                MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal or
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 or
                MidiPercussion.RideBell or MidiPercussion.TriangleOpen => 50,

                MidiPercussion.GuiroLong or MidiPercussion.Vibraslap or
                MidiPercussion.BellTree or MidiPercussion.OceanDrum => 72,

                _ => 45
            };

            int maximumPercussionMs = availableFrameMs - melodyFloorMs;
            int selectedMs = Math.Min(preferredAttackMs, naturalDurationMs);
            selectedMs = Math.Min(selectedMs, maximumPercussionMs);
            return Math.Clamp(selectedMs, 1, availableFrameMs);
        }

        public static int GetNaturalDurationMs(MidiPercussion percussion)
        {
            return GetProfile(percussion, PercussionOutputChoice.SystemSpeaker).DurationMs;
        }

        private static void RenderProfile(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? totalDurationOverrideMs = null)
        {
            if (!IsSessionActive(output, sid)) return;

            if (prof.HitCount <= 1)
            {
                PlaySingleHit(output, sid, prof, ct, totalDurationOverrideMs);
                return;
            }

            int perHitMs;
            if (totalDurationOverrideMs.HasValue)
            {
                int totalGap = prof.HitGapMs * (prof.HitCount - 1);
                perHitMs = Math.Max(1, (totalDurationOverrideMs.Value - totalGap) / prof.HitCount);
            }
            else
            {
                perHitMs = prof.DurationMs;
            }

            if (perHitMs < 20) perHitMs = 20;

            for (int hit = 0; hit < prof.HitCount; hit++)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;
                PlaySingleHit(output, sid, prof, ct, perHitMs);
                if (hit < prof.HitCount - 1)
                    PreciseWaitMs(prof.HitGapMs, ct);
            }
        }

        private static void PlaySingleHit(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? durationMsOverride = null)
        {
            if (!IsSessionActive(output, sid)) return;

            int duration = durationMsOverride ?? prof.DurationMs;
            if (duration <= 0) return;
            if (duration < 20) duration = 20;

            if (prof.BodyWave == SynthWave.Noise)
            {
                RenderGatedNoise(output, sid, prof.BodyStartFreq, duration, prof.NoiseDensity, prof.HoldRatio, prof.DurationMs, ct);
            }
            else if (prof.DoesSweep)
            {
                RenderSweepTone(output, sid, prof, duration, ct);
            }
            else
            {
                RenderGatedTone(output, sid, prof.BodyEndFreq, duration, prof.BodyWave, prof.HoldRatio, prof.DurationMs, ct);
            }
        }

        // --- GATED RENDERING ENGINES ---

        private static void RenderGatedNoise(PercussionOutputChoice output, int sid, double baseFreq, int totalDurationMs, double noiseVol, double holdRatio, int originalDurationMs, CancellationToken ct)
        {
            double sampleFreq = baseFreq < 3500 ? baseFreq + 5000 : baseFreq + 1200;

            // Buffered devices cannot reproduce the sub-millisecond PWM used
            // by the PC speaker. Use one compact white-noise transient instead.
            if (output == PercussionOutputChoice.SoundDevice)
            {
                double attackRatio = Math.Max(holdRatio, Math.Min(0.65, noiseVol * 0.50));
                int maximumAttackMs = totalDurationMs <= 100
                    ? Math.Min(32, totalDurationMs)
                    : Math.Min(60, totalDurationMs);
                int audibleAttackMs = GetSoundDeviceAttackDuration(
                    totalDurationMs, attackRatio, maximumAttackMs);

                StartPulse(output, (int)sampleFreq, SynthWave.Noise, sid);
                PreciseWaitMs(audibleAttackMs, ct);
                StopPulse(output, sid);
                return;
            }

            double sampleDurMs = 1000.0 / (sampleFreq + 0.25);

            var sw = Stopwatch.StartNew();
            double nextSampleMs = 0;
            bool speakerOn = false;
            double initialNoiseVol = noiseVol;

            int envelopeDurationMs = Math.Min(originalDurationMs, Math.Max(1, totalDurationMs));

            while (sw.Elapsed.TotalMilliseconds < totalDurationMs)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                double elapsed = sw.Elapsed.TotalMilliseconds;
                double progress = elapsed / envelopeDurationMs;

                if (progress >= 1.0) break;

                double currentNoiseVol;
                if (progress < holdRatio)
                {
                    currentNoiseVol = initialNoiseVol;
                }
                else
                {
                    double decayProgress = (progress - holdRatio) / (1.0 - holdRatio);
                    currentNoiseVol = initialNoiseVol * (1.0 - Math.Clamp(decayProgress, 0.0, 1.0));
                }

                if (currentNoiseVol < 0.04) break;

                bool wantOn = Random.Shared.NextDouble() < currentNoiseVol;
                if (wantOn != speakerOn)
                {
                    if (wantOn) StartPulse(output, (int)sampleFreq, SynthWave.Square, sid);
                    else StopPulse(output, sid);
                    speakerOn = wantOn;
                }

                nextSampleMs += sampleDurMs;
                while (sw.Elapsed.TotalMilliseconds < nextSampleMs)
                {
                    if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;
                }
            }

            StopPulse(output, sid);
        }

        private static void RenderGatedTone(PercussionOutputChoice output, int sid, int frequency, int totalDurationMs, SynthWave waveType, double holdRatio, int originalDurationMs, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();

            // Preserve the profile's short attack instead of holding every
            // sound for most of its frame, which makes clicks and drums beep.
            if (output == PercussionOutputChoice.SoundDevice)
            {
                int maximumAttackMs = totalDurationMs <= 100
                    ? Math.Min(18, totalDurationMs)
                    : Math.Min(120, totalDurationMs);
                int audibleAttackMs = GetSoundDeviceAttackDuration(
                    totalDurationMs, holdRatio, maximumAttackMs);

                StartPulse(output, frequency, waveType, sid);
                PreciseWaitMs(audibleAttackMs, ct);
                StopPulse(output, sid);
                return;
            }

            const double cycleDurMs = 4.0;
            bool speakerOn = false;

            while (sw.Elapsed.TotalMilliseconds < totalDurationMs)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                double elapsed = sw.Elapsed.TotalMilliseconds;
                double progress = elapsed / originalDurationMs;
                if (progress >= 1.0) break;

                double volume = 1.0;
                if (progress > holdRatio)
                {
                    double decayProgress = (progress - holdRatio) / (1.0 - holdRatio);
                    volume = 1.0 - Math.Clamp(decayProgress, 0.0, 1.0);
                }

                if (volume < 0.02) break;

                double onTime = cycleDurMs * volume;
                double offTime = cycleDurMs * (1.0 - volume);

                if (onTime > 0)
                {
                    if (!speakerOn)
                    {
                        StartPulse(output, frequency, waveType, sid);
                        speakerOn = true;
                    }
                    PreciseWaitMs(onTime, ct);
                }

                if (offTime > 0)
                {
                    if (speakerOn)
                    {
                        StopPulse(output, sid);
                        speakerOn = false;
                    }
                    PreciseWaitMs(offTime, ct);
                }
            }

            if (speakerOn) StopPulse(output, sid);
        }

        private static void RenderSweepTone(PercussionOutputChoice output, int sid, PercussionProfile prof, int totalDurationMs, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();

            // A single 45 ms midpoint tone sounds like a beep. Render only the
            // attack portion as a compact downward glide with 8-12 ms steps.
            if (output == PercussionOutputChoice.SoundDevice)
            {
                double attackRatio = Math.Max(prof.HoldRatio, 0.30);
                int maximumSweepMs = totalDurationMs <= 100
                    ? Math.Min(28, totalDurationMs)
                    : Math.Min(120, totalDurationMs);
                int audibleSweepMs = GetSoundDeviceAttackDuration(
                    totalDurationMs, attackRatio, maximumSweepMs);
                audibleSweepMs = Math.Max(
                    Math.Min(20, totalDurationMs), audibleSweepMs);

                // Two points are enough for the shortest kick while avoiding a
                // sustained midpoint pitch. Longer attacks receive more points.
                int steps = Math.Clamp(
                    (int)Math.Ceiling(audibleSweepMs / 10.0), 2, 12);
                double stepTime = (double)audibleSweepMs / steps;

                for (int i = 0; i < steps; i++)
                {
                    if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                    double progress = (double)i / (steps - 1);
                    int freq = (int)Math.Round(
                        prof.BodyStartFreq -
                        ((prof.BodyStartFreq - prof.BodyEndFreq) * progress));

                    StartPulse(output, freq, prof.BodyWave, sid);
                    PreciseWaitMs(stepTime, ct);
                }
                StopPulse(output, sid);
                return;
            }

            const double cycleDurMs = 4.0;
            bool speakerOn = false;
            int lastFreq = int.MinValue;

            while (sw.Elapsed.TotalMilliseconds < totalDurationMs)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                double elapsed = sw.Elapsed.TotalMilliseconds;
                double progress = elapsed / prof.DurationMs;
                if (progress >= 1.0) break;

                int freq = (int)(prof.BodyStartFreq - ((prof.BodyStartFreq - prof.BodyEndFreq) * Math.Min(1.0, progress)));

                double volume = 1.0;
                if (progress > prof.HoldRatio)
                {
                    double decayProgress = (progress - prof.HoldRatio) / (1.0 - prof.HoldRatio);
                    volume = 1.0 - Math.Clamp(decayProgress, 0.0, 1.0);
                }

                if (volume < 0.02) break;

                double onTime = cycleDurMs * volume;
                double offTime = cycleDurMs * (1.0 - volume);

                if (onTime > 0)
                {
                    if (!speakerOn || freq != lastFreq)
                    {
                        StartPulse(output, freq, prof.BodyWave, sid);
                        lastFreq = freq;
                        speakerOn = true;
                    }
                    PreciseWaitMs(onTime, ct);
                }

                if (offTime > 0)
                {
                    if (speakerOn)
                    {
                        StopPulse(output, sid);
                        speakerOn = false;
                        lastFreq = int.MinValue;
                    }
                    PreciseWaitMs(offTime, ct);
                }
            }

            if (speakerOn) StopPulse(output, sid);
        }
    }
}