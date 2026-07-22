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
            Laser = 27, Whip = 28, ScratchPush = 29, ScratchPull = 30, StickClick = 31,
            SquareClick = 32, MetronomeClick = 33, MetronomeBell = 34,

            BassDrum = 35, KickDrum = 36, LowTom = 45, LowMidTom = 47, HighMidTom = 48,
            HighTom = 50, FloorTom1 = 43, FloorTom2 = 41,

            SideStick = 37, SnareCrossStick = 37, SnareDrum = 38, ElectricSnareDrum = 40,
            SnareDrumRod = 91, SnareDrumBrush = 93,

            HiHatClosed = 42, HiHatOpen = 46, HiHatFoot = 44, CrashCymbal = 49,
            CrashCymbal2 = 57, RideCymbal = 51, RideCymbal2 = 59, ChinaCymbal = 52,
            SplashCymbal = 55, RideBell = 53,

            HandClap = 39, Tambourine = 54, Vibraslap = 58, Cowbell = 56,
            HighBongo = 60, LowBongo = 61, CongaDeadStroke = 62, Conga = 63, Tumba = 64,
            HighTimbale = 65, LowTimbale = 66, HighAgogo = 67, LowAgogo = 68,
            Cabasa = 69, Maracas = 70, Shaker = 82, SleighBell = 83, BellTree = 84,
            Castanets = 85, SurduDeadStroke = 86, Surdu = 87, CuicaHigh = 78, CuicaLow = 79,

            GuiroShort = 73, GuiroLong = 74, Güiro = 73, Claves = 75, Clave = 75,
            HighWoodblock = 76, LowWoodblock = 77, WoodBlock = 76,

            WhistleShort = 71, WhistleLong = 72, TriangleMute = 80, TriangleOpen = 81,
            OceanDrum = 92
        }

        public enum PercussionOutputChoice { SystemSpeaker, SoundDevice }
        private enum SynthWave { Square, Triangle, Noise }

        private static int _globalSessionId = 0;
        private static int _activeSpeakerSession = 0;
        private static int _activeDeviceSession = 0;
        private static readonly object _hardwareLock = new object();

        private static int ClampPercussionFrequency(double frequency) => (int)Math.Round(Math.Clamp(frequency, 37.0, 15000.0));

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
        //
        // These values are deliberately tuned for the existing one-voice pulse engine.
        // Noise profile frequencies also account for RenderGatedNoise's unchanged
        // carrier conversion:
        //     base < 3500  -> base + 5000
        //     base >= 3500 -> base + 1200
        private static PercussionProfile GetProfile(MidiPercussion p, PercussionOutputChoice output) => p switch
        {
            // Bass drums: fast attack pitch-drop, with the acoustic drum lower and longer.
            MidiPercussion.BassDrum =>
                new PercussionProfile(SynthWave.Triangle, true, 145, 42, 280, holdRatio: 0.045),
            MidiPercussion.KickDrum =>
                new PercussionProfile(SynthWave.Triangle, true, 190, 52, 190, holdRatio: 0.025),

            // Toms: progressively lower fundamentals and longer decays.
            MidiPercussion.HighTom =>
                new PercussionProfile(SynthWave.Triangle, true, 330, 185, 210, holdRatio: 0.070),
            MidiPercussion.HighMidTom =>
                new PercussionProfile(SynthWave.Triangle, true, 275, 145, 250, holdRatio: 0.075),
            MidiPercussion.LowMidTom =>
                new PercussionProfile(SynthWave.Triangle, true, 225, 112, 310, holdRatio: 0.080),
            MidiPercussion.LowTom =>
                new PercussionProfile(SynthWave.Triangle, true, 180, 84, 380, holdRatio: 0.085),
            MidiPercussion.FloorTom1 =>
                new PercussionProfile(SynthWave.Triangle, true, 148, 66, 480, holdRatio: 0.090),
            MidiPercussion.FloorTom2 =>
                new PercussionProfile(SynthWave.Triangle, true, 124, 52, 560, holdRatio: 0.095),

            // Snares: bright broadband attack; electric is tighter/brighter,
            // rod is drier, and brush is softer with a longer tail.
            MidiPercussion.SnareDrum =>
                new PercussionProfile(SynthWave.Noise, false, 2300, 2300, 220,
                    density: 0.60, holdRatio: 0.055),
            MidiPercussion.ElectricSnareDrum =>
                new PercussionProfile(SynthWave.Noise, false, 3100, 3100, 160,
                    density: 0.72, holdRatio: 0.040),
            MidiPercussion.SnareDrumRod =>
                new PercussionProfile(SynthWave.Noise, false, 1250, 1250, 110,
                    density: 0.38, holdRatio: 0.040),
            MidiPercussion.SnareDrumBrush =>
                new PercussionProfile(SynthWave.Noise, false, 650, 650, 500,
                    density: 0.23, holdRatio: 0.100),

            // Cymbals: long, sparse metallic tails; splash is short and China is darker/trashier.
            MidiPercussion.CrashCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 6500, 6500, 2600,
                    density: 0.52, holdRatio: 0.035),
            MidiPercussion.CrashCymbal2 =>
                new PercussionProfile(SynthWave.Noise, false, 7800, 7800, 3200,
                    density: 0.48, holdRatio: 0.030),
            MidiPercussion.ChinaCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 5200, 5200, 2100,
                    density: 0.68, holdRatio: 0.025),
            MidiPercussion.SplashCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 8200, 8200, 650,
                    density: 0.58, holdRatio: 0.045),
            MidiPercussion.RideCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 5600, 5600, 3000,
                    density: 0.27, holdRatio: 0.035),
            MidiPercussion.RideCymbal2 =>
                new PercussionProfile(SynthWave.Noise, false, 6800, 6800, 3400,
                    density: 0.24, holdRatio: 0.030),

            // Hi-hats: closed is an abrupt tick, open has a thinner sustained tail,
            // and the pedal sound is shorter/darker.
            MidiPercussion.HiHatClosed =>
                new PercussionProfile(SynthWave.Noise, false, 9000, 9000, 75,
                    density: 0.74, holdRatio: 0.100),
            MidiPercussion.HiHatOpen =>
                new PercussionProfile(SynthWave.Noise, false, 8200, 8200, 900,
                    density: 0.40, holdRatio: 0.025),
            MidiPercussion.HiHatFoot =>
                new PercussionProfile(SynthWave.Noise, false, 6000, 6000, 120,
                    density: 0.50, holdRatio: 0.060),

            // A clap is several closely-spaced broadband attacks.
            MidiPercussion.HandClap =>
                new PercussionProfile(SynthWave.Noise, false, 1800, 1800, 150,
                    density: 0.70, hits: 3, gap: 12, holdRatio: 0.025),

            // Vibraslap rattles repeatedly after its initial strike.
            MidiPercussion.Vibraslap =>
                new PercussionProfile(SynthWave.Noise, false, 1500, 1500, 1200,
                    density: 0.24, hits: 7, gap: 45, holdRatio: 0.045),

            // Dry stick/click sounds should be pitched transients rather than hiss.
            // SnareCrossStick is an alias of SideStick and therefore follows this profile.
            MidiPercussion.SideStick =>
                new PercussionProfile(SynthWave.Triangle, false, 1850, 1850, 68,
                    holdRatio: 0.025),
            MidiPercussion.StickClick =>
                new PercussionProfile(SynthWave.Triangle, false, 2350, 2350, 42,
                    holdRatio: 0.020),
            MidiPercussion.SquareClick =>
                new PercussionProfile(SynthWave.Square, false, 1550, 1550, 34,
                    holdRatio: 0.020),
            MidiPercussion.MetronomeClick =>
                new PercussionProfile(SynthWave.Square, false, 1200, 1200, 46,
                    holdRatio: 0.025),
            MidiPercussion.MetronomeBell =>
                new PercussionProfile(SynthWave.Triangle, false, 2450, 2450, 260,
                    holdRatio: 0.045),

            MidiPercussion.Claves =>
                new PercussionProfile(SynthWave.Triangle, false, 2550, 2550, 82,
                    holdRatio: 0.030),
            MidiPercussion.Castanets =>
                new PercussionProfile(SynthWave.Triangle, false, 2150, 2150, 66,
                    hits: 2, gap: 7, holdRatio: 0.020),
            MidiPercussion.HighWoodblock =>
                new PercussionProfile(SynthWave.Triangle, false, 1780, 1780, 105,
                    holdRatio: 0.040),
            MidiPercussion.LowWoodblock =>
                new PercussionProfile(SynthWave.Triangle, false, 940, 940, 155,
                    holdRatio: 0.045),

            // Hand drums.
            MidiPercussion.HighBongo =>
                new PercussionProfile(SynthWave.Triangle, true, 345, 215, 140,
                    holdRatio: 0.060),
            MidiPercussion.LowBongo =>
                new PercussionProfile(SynthWave.Triangle, true, 255, 150, 185,
                    holdRatio: 0.065),
            MidiPercussion.CongaDeadStroke =>
                new PercussionProfile(SynthWave.Triangle, true, 300, 225, 78,
                    holdRatio: 0.025),
            MidiPercussion.Conga =>
                new PercussionProfile(SynthWave.Triangle, true, 270, 150, 235,
                    holdRatio: 0.065),
            MidiPercussion.Tumba =>
                new PercussionProfile(SynthWave.Triangle, true, 195, 92, 315,
                    holdRatio: 0.075),
            MidiPercussion.HighTimbale =>
                new PercussionProfile(SynthWave.Triangle, true, 570, 365, 175,
                    holdRatio: 0.040),
            MidiPercussion.LowTimbale =>
                new PercussionProfile(SynthWave.Triangle, true, 430, 265, 225,
                    holdRatio: 0.045),
            MidiPercussion.SurduDeadStroke =>
                new PercussionProfile(SynthWave.Triangle, true, 125, 76, 100,
                    holdRatio: 0.030),
            MidiPercussion.Surdu =>
                new PercussionProfile(SynthWave.Triangle, true, 108, 48, 500,
                    holdRatio: 0.080),

            // Metallic pitched percussion.
            MidiPercussion.Cowbell =>
                new PercussionProfile(SynthWave.Triangle, false, 650, 650, 420,
                    holdRatio: 0.040),
            MidiPercussion.RideBell =>
                new PercussionProfile(SynthWave.Triangle, false, 2200, 2200, 900,
                    holdRatio: 0.035),
            MidiPercussion.HighAgogo =>
                new PercussionProfile(SynthWave.Triangle, false, 1120, 1120, 320,
                    holdRatio: 0.045),
            MidiPercussion.LowAgogo =>
                new PercussionProfile(SynthWave.Triangle, false, 720, 720, 390,
                    holdRatio: 0.050),
            MidiPercussion.TriangleMute =>
                new PercussionProfile(SynthWave.Triangle, false, 5200, 5200, 92,
                    holdRatio: 0.055),
            MidiPercussion.TriangleOpen =>
                new PercussionProfile(SynthWave.Triangle, false, 4800, 4800, 1800,
                    holdRatio: 0.025),
            MidiPercussion.SleighBell =>
                new PercussionProfile(SynthWave.Triangle, false, 4200, 4200, 650,
                    hits: 5, gap: 22, holdRatio: 0.040),
            MidiPercussion.BellTree =>
                new PercussionProfile(SynthWave.Triangle, false, 3200, 3200, 1400,
                    hits: 8, gap: 45, holdRatio: 0.035),

            // Shaken and scraped instruments.
            MidiPercussion.Maracas =>
                new PercussionProfile(SynthWave.Noise, false, 2500, 2500, 115,
                    density: 0.28, holdRatio: 0.045),
            MidiPercussion.Cabasa =>
                new PercussionProfile(SynthWave.Noise, false, 1800, 1800, 245,
                    density: 0.46, holdRatio: 0.045),
            MidiPercussion.Shaker =>
                new PercussionProfile(SynthWave.Noise, false, 3000, 3000, 165,
                    density: 0.23, holdRatio: 0.040),
            MidiPercussion.Tambourine =>
                new PercussionProfile(SynthWave.Noise, false, 7400, 7400, 400,
                    density: 0.36, holdRatio: 0.040),
            MidiPercussion.GuiroShort =>
                new PercussionProfile(SynthWave.Noise, false, 1200, 1200, 220,
                    density: 0.46, hits: 5, gap: 8, holdRatio: 0.035),
            MidiPercussion.GuiroLong =>
                new PercussionProfile(SynthWave.Noise, false, 1200, 1200, 900,
                    density: 0.43, hits: 12, gap: 16, holdRatio: 0.030),

            // Scratches are directional pitch sweeps rather than static noise.
            MidiPercussion.ScratchPush =>
                new PercussionProfile(SynthWave.Square, true, 4200, 900, 220,
                    holdRatio: 0.035),
            MidiPercussion.ScratchPull =>
                new PercussionProfile(SynthWave.Square, true, 900, 4200, 220,
                    holdRatio: 0.035),

            MidiPercussion.OceanDrum =>
                new PercussionProfile(SynthWave.Noise, false, 900, 900, 3000,
                    density: 0.13, holdRatio: 0.180),

            // Effects.
            MidiPercussion.WhistleShort =>
                new PercussionProfile(SynthWave.Triangle, false, 2200, 2200, 250,
                    holdRatio: 0.090),
            MidiPercussion.WhistleLong =>
                new PercussionProfile(SynthWave.Triangle, false, 2200, 2200, 1200,
                    holdRatio: 0.180),
            MidiPercussion.Laser =>
                new PercussionProfile(SynthWave.Square, true, 3200, 180, 350,
                    holdRatio: 0.020),
            MidiPercussion.Whip =>
                new PercussionProfile(SynthWave.Noise, false, 2500, 2500, 130,
                    density: 0.78, holdRatio: 0.015),
            MidiPercussion.CuicaHigh =>
                new PercussionProfile(SynthWave.Triangle, true, 540, 920, 260,
                    holdRatio: 0.075),
            MidiPercussion.CuicaLow =>
                new PercussionProfile(SynthWave.Triangle, true, 285, 510, 360,
                    holdRatio: 0.085),

            _ => new PercussionProfile(SynthWave.Square, false, 400, 400, 45,
                holdRatio: 0.30)
        };


        /// <summary>
        /// Returns the natural one-shot envelope length used for a General MIDI percussion key.
        /// General MIDI standardizes the key map, but not fixed acoustic durations; these values
        /// are the engine's natural release times and are not stretched by long MIDI note lengths.
        /// </summary>
        public static int GetNaturalDurationMs(MidiPercussion percussion)
        {
            return GetProfile(percussion, PercussionOutputChoice.SystemSpeaker).DurationMs;
        }

        /// <summary>
        /// Chooses how much of a monophonic frame should be assigned to the percussion attack.
        /// With no melody, the instrument may use its natural one-shot duration. When melody is
        /// also present, only the recognizable attack portion is used and at least 8 ms is kept
        /// for the melody whenever the frame is long enough.
        /// </summary>
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

        // --- PLAYBACK ENGINE ---

        private static void EnforceCleanOnset(PercussionOutputChoice output)
        {
            lock (_hardwareLock)
            {
                switch (output)
                {
                    case PercussionOutputChoice.SystemSpeaker:
                        SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                        break;
                    case PercussionOutputChoice.SoundDevice:
                        SoundRenderingEngine.WaveSynthEngine.StopSynth();
                        break;
                }
            }
        }

        // FIXED: The Session ID assignment now happens synchronously before Task.Run
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

        // FIXED: Now takes pre-determined output type and cleans up instantly if superseded
        private static void ExecutePercussionPlayback(MidiPercussion p, int sid, CancellationToken ct, int maxMs, int vel, PercussionOutputChoice output)
        {
            if (!IsSessionActive(output, sid)) return;

            EnforceCleanOnset(output);

            var prof = GetProfile(p, output);

            int finalDuration = Math.Min(Math.Max(1, maxMs), prof.DurationMs);

            try
            {
                RenderProfile(output, sid, prof, ct, totalDurationOverrideMs: finalDuration);
            }
            finally { StopPulse(output, sid); }
        }

        // FIXED: The Session ID assignment now happens synchronously before Task.Run
        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct = default, int velocity = 100)
        {
            if (durationMs <= 0) return Task.CompletedTask;
            durationMs = Math.Max(1, durationMs);
            int sid = Interlocked.Increment(ref _globalSessionId);

            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice ?
                PercussionOutputChoice.SoundDevice : PercussionOutputChoice.SystemSpeaker;

            if (output == PercussionOutputChoice.SystemSpeaker)
                Interlocked.Exchange(ref _activeSpeakerSession, sid);
            else
                Interlocked.Exchange(ref _activeDeviceSession, sid);

            return Task.Run(() => ExecutePercussionPlaybackForDuration(p, sid, ct, durationMs, velocity, output), ct);
        }

        private static void ExecutePercussionPlaybackForDuration(MidiPercussion p, int sid, CancellationToken ct, int durationMs, int vel, PercussionOutputChoice output)
        {
            if (!IsSessionActive(output, sid)) return;

            EnforceCleanOnset(output);

            var prof = GetProfile(p, output);

            // GM percussion is normally a one-shot envelope. A long MIDI note must not
            // stretch a short drum sample; the caller's duration is only an upper bound.
            int finalDuration = Math.Min(Math.Max(1, durationMs), prof.DurationMs);

            try
            {
                RenderProfile(output, sid, prof, ct, finalDuration);
            }
            finally { StopPulse(output, sid); }
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

            int requestedTotalMs = Math.Max(1, totalDurationOverrideMs ?? prof.DurationMs);

            // Keep multi-strike sounds inside the requested total duration. If the requested
            // slot is too short for the profile's normal gaps, reduce the gaps rather than
            // silently extending the frame and creating MIDI playback drift.
            int actualGapMs = prof.HitCount > 1
                ? Math.Min(prof.HitGapMs, Math.Max(0, (requestedTotalMs - prof.HitCount) / (prof.HitCount - 1)))
                : 0;

            int hitBudgetMs = Math.Max(prof.HitCount,
                requestedTotalMs - (actualGapMs * (prof.HitCount - 1)));
            int baseHitMs = Math.Max(1, hitBudgetMs / prof.HitCount);
            int remainderMs = Math.Max(0, hitBudgetMs - (baseHitMs * prof.HitCount));

            for (int hit = 0; hit < prof.HitCount; hit++)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                int thisHitMs = baseHitMs + (hit < remainderMs ? 1 : 0);
                PlaySingleHit(output, sid, prof, ct, thisHitMs);

                if (hit < prof.HitCount - 1 && actualGapMs > 0)
                    PreciseWaitMs(actualGapMs, ct);
            }
        }

        private static void PlaySingleHit(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? durationMsOverride = null)
        {
            if (!IsSessionActive(output, sid)) return;

            int duration = durationMsOverride ?? prof.DurationMs;
            if (duration <= 0) return;

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
        /// <summary>
        /// Chooses how much of a monophonic frame should be assigned to the percussion attack.
        /// With no melody, the instrument may use its natural one-shot duration. When melody is
        /// also present, only the recognizable attack portion is used and at least 8 ms is kept
        /// for the melody whenever the frame is long enough.
        /// </summary>
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

        /// <summary>
        /// Returns the natural one-shot envelope length used for a General MIDI percussion key.
        /// General MIDI standardizes the key map, but not fixed acoustic durations; these values
        /// are the engine's natural release times and are not stretched by long MIDI note lengths.
        /// </summary>
        public static int GetNaturalDurationMs(MidiPercussion percussion)
        {
            return GetProfile(percussion, PercussionOutputChoice.SystemSpeaker).DurationMs;
        }

    }
}