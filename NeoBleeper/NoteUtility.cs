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

        private static int ClampPercussionFrequency(double frequency) => (int)Math.Round(Math.Clamp(frequency, 37.0, 15000.0));
        private static bool IsSessionActive(PercussionOutputChoice choice, int sessionId) =>
            choice == PercussionOutputChoice.SystemSpeaker ? Volatile.Read(ref _activeSpeakerSession) == sessionId : Volatile.Read(ref _activeDeviceSession) == sessionId;

        private static void StartPulse(PercussionOutputChoice outputChoice, int frequency, SynthWave waveType)
        {
            frequency = ClampPercussionFrequency(frequency);
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

        private static void StopPulse(PercussionOutputChoice outputChoice, int sessionId)
        {
            if (outputChoice == PercussionOutputChoice.SystemSpeaker && Volatile.Read(ref _activeSpeakerSession) == sessionId) SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
            else if (outputChoice == PercussionOutputChoice.SoundDevice && Volatile.Read(ref _activeDeviceSession) == sessionId) SoundRenderingEngine.WaveSynthEngine.StopSynth();
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
            public readonly double HoldRatio; // Percentage of playback at full volume before decay begins

            public PercussionProfile(SynthWave w, bool s, int start, int end, int dur,
                double density = 0.5, int hits = 1, int gap = 0, double holdRatio = 0.15)
            {
                BodyWave = w; DoesSweep = s; BodyStartFreq = start; BodyEndFreq = end; DurationMs = dur;
                NoiseDensity = density; HitCount = Math.Max(1, hits); HitGapMs = gap;
                HoldRatio = Math.Clamp(holdRatio, 0.0, 1.0);
            }
        }

        // --- CORE PERCUSSION PROFILES ---
        private static PercussionProfile GetProfile(MidiPercussion p, PercussionOutputChoice output) => p switch
        {
            MidiPercussion.KickDrum or MidiPercussion.BassDrum =>
                new PercussionProfile(SynthWave.Triangle, true, 170, 48, 180, holdRatio: 0.25),
            MidiPercussion.HighTom =>
                new PercussionProfile(SynthWave.Triangle, true, 320, 140, 280, holdRatio: 0.20),
            MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom =>
                new PercussionProfile(SynthWave.Triangle, true, 210, 105, 380, holdRatio: 0.20),
            MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 =>
                new PercussionProfile(SynthWave.Triangle, true, 130, 65, 550, holdRatio: 0.20),

            MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum =>
                new PercussionProfile(SynthWave.Noise, false, 4200, 4200, 350, density: 0.60, holdRatio: 0.15),
            MidiPercussion.SnareDrumRod =>
                new PercussionProfile(SynthWave.Noise, false, 3200, 3200, 250, density: 0.45, holdRatio: 0.15),
            MidiPercussion.SnareDrumBrush =>
                new PercussionProfile(SynthWave.Noise, false, 2600, 2600, 450, density: 0.30, holdRatio: 0.20),

            MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 7000, 7000, 3500, density: 0.55, holdRatio: 0.10),
            MidiPercussion.SplashCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 7500, 7500, 1000, density: 0.55, holdRatio: 0.15),
            MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                new PercussionProfile(SynthWave.Noise, false, 6200, 6200, 2800, density: 0.35, holdRatio: 0.12),

            MidiPercussion.HiHatClosed =>
                new PercussionProfile(SynthWave.Noise, false, 8500, 8500, 50, density: 0.70, holdRatio: 0.30),
            MidiPercussion.HiHatOpen =>
                new PercussionProfile(SynthWave.Noise, false, 8000, 8000, 1200, density: 0.40, holdRatio: 0.10),
            MidiPercussion.HiHatFoot =>
                new PercussionProfile(SynthWave.Noise, false, 3000, 3000, 100, density: 0.50, holdRatio: 0.25),

            MidiPercussion.HandClap =>
                new PercussionProfile(SynthWave.Noise, false, 2800, 2800, 50, density: 0.65, hits: 3, gap: 15, holdRatio: 0.20),
            MidiPercussion.Vibraslap =>
                new PercussionProfile(SynthWave.Noise, false, 2200, 2200, 1600, density: 0.25, holdRatio: 0.20),

            MidiPercussion.SideStick or MidiPercussion.SnareCrossStick or MidiPercussion.StickClick or
            MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.MetronomeBell =>
                new PercussionProfile(SynthWave.Noise, false, 900, 900, 40, density: 0.80, holdRatio: 0.35),

            MidiPercussion.Claves or MidiPercussion.Clave or MidiPercussion.Castanets =>
                new PercussionProfile(SynthWave.Triangle, false, 2000, 2000, 80, holdRatio: 0.30),
            MidiPercussion.HighWoodblock =>
                new PercussionProfile(SynthWave.Triangle, false, 1600, 1600, 100, holdRatio: 0.30),
            MidiPercussion.LowWoodblock or MidiPercussion.WoodBlock =>
                new PercussionProfile(SynthWave.Triangle, false, 1100, 1100, 140, holdRatio: 0.30),

            MidiPercussion.HighBongo =>
                new PercussionProfile(SynthWave.Triangle, true, 260, 150, 180, holdRatio: 0.20),
            MidiPercussion.LowBongo =>
                new PercussionProfile(SynthWave.Triangle, true, 190, 105, 240, holdRatio: 0.20),
            MidiPercussion.CongaDeadStroke =>
                new PercussionProfile(SynthWave.Triangle, true, 220, 150, 80, holdRatio: 0.25),
            MidiPercussion.Conga =>
                new PercussionProfile(SynthWave.Triangle, true, 220, 100, 260, holdRatio: 0.20),
            MidiPercussion.Tumba =>
                new PercussionProfile(SynthWave.Triangle, true, 170, 85, 320, holdRatio: 0.20),
            MidiPercussion.HighTimbale =>
                new PercussionProfile(SynthWave.Triangle, true, 500, 300, 220, holdRatio: 0.15),
            MidiPercussion.LowTimbale =>
                new PercussionProfile(SynthWave.Triangle, true, 380, 220, 280, holdRatio: 0.15),
            MidiPercussion.SurduDeadStroke =>
                new PercussionProfile(SynthWave.Triangle, true, 150, 85, 110, holdRatio: 0.25),
            MidiPercussion.Surdu =>
                new PercussionProfile(SynthWave.Triangle, true, 140, 60, 450, holdRatio: 0.20),

            MidiPercussion.Cowbell =>
                new PercussionProfile(SynthWave.Triangle, false, 800, 800, 450, holdRatio: 0.15),
            MidiPercussion.RideBell =>
                new PercussionProfile(SynthWave.Triangle, false, 1400, 1400, 800, holdRatio: 0.15),
            MidiPercussion.HighAgogo =>
                new PercussionProfile(SynthWave.Triangle, false, 950, 950, 220, holdRatio: 0.15),
            MidiPercussion.LowAgogo =>
                new PercussionProfile(SynthWave.Triangle, false, 700, 700, 300, holdRatio: 0.15),
            MidiPercussion.TriangleMute =>
                new PercussionProfile(SynthWave.Triangle, false, 3800, 3800, 60, holdRatio: 0.30),
            MidiPercussion.TriangleOpen =>
                new PercussionProfile(SynthWave.Triangle, false, 3800, 3800, 2000, holdRatio: 0.15),
            MidiPercussion.SleighBell or MidiPercussion.BellTree =>
                new PercussionProfile(SynthWave.Triangle, false, 2600, 2600, 200, hits: 4, gap: 20, holdRatio: 0.15),

            MidiPercussion.Maracas =>
                new PercussionProfile(SynthWave.Noise, false, 5500, 5500, 150, density: 0.35, holdRatio: 0.15),
            MidiPercussion.Cabasa =>
                new PercussionProfile(SynthWave.Noise, false, 5000, 5000, 200, density: 0.50, holdRatio: 0.15),
            MidiPercussion.Shaker or MidiPercussion.Tambourine =>
                new PercussionProfile(SynthWave.Noise, false, 6000, 6000, 250, density: 0.30, holdRatio: 0.15),
            MidiPercussion.GuiroShort or MidiPercussion.Güiro =>
                new PercussionProfile(SynthWave.Noise, false, 3500, 3500, 180, density: 0.60, holdRatio: 0.20),
            MidiPercussion.GuiroLong =>
                new PercussionProfile(SynthWave.Noise, false, 3500, 3500, 120, density: 0.60, hits: 5, gap: 15, holdRatio: 0.15),
            MidiPercussion.ScratchPush or MidiPercussion.ScratchPull =>
                new PercussionProfile(SynthWave.Noise, false, 4200, 4200, 180, density: 0.45, holdRatio: 0.20),
            MidiPercussion.OceanDrum =>
                new PercussionProfile(SynthWave.Noise, false, 1800, 1800, 2500, density: 0.15, holdRatio: 0.20),

            MidiPercussion.WhistleShort =>
                new PercussionProfile(SynthWave.Square, false, 1800, 1800, 250, holdRatio: 0.30),
            MidiPercussion.WhistleLong =>
                new PercussionProfile(SynthWave.Square, false, 1800, 1800, 1200, holdRatio: 0.40),
            MidiPercussion.Laser =>
                new PercussionProfile(SynthWave.Square, true, 2500, 200, 300, holdRatio: 0.15),
            MidiPercussion.Whip =>
                new PercussionProfile(SynthWave.Square, true, 1800, 150, 200, holdRatio: 0.15),
            MidiPercussion.CuicaHigh =>
                new PercussionProfile(SynthWave.Triangle, true, 900, 500, 250, holdRatio: 0.20),
            MidiPercussion.CuicaLow =>
                new PercussionProfile(SynthWave.Triangle, true, 500, 250, 350, holdRatio: 0.20),

            _ => new PercussionProfile(SynthWave.Square, false, 400, 400, 50, holdRatio: 0.30)
        };

        private static bool NeedsFadeOut(MidiPercussion p)
        {
            return p == MidiPercussion.CrashCymbal ||
                   p == MidiPercussion.CrashCymbal2 ||
                   p == MidiPercussion.ChinaCymbal ||
                   p == MidiPercussion.SplashCymbal ||
                   p == MidiPercussion.RideCymbal ||
                   p == MidiPercussion.RideCymbal2 ||
                   p == MidiPercussion.HiHatOpen ||
                   p == MidiPercussion.Vibraslap ||
                   p == MidiPercussion.OceanDrum ||
                   p == MidiPercussion.TriangleOpen ||
                   p == MidiPercussion.RideBell ||
                   p == MidiPercussion.Cowbell ||
                   p == MidiPercussion.SnareDrum ||
                   p == MidiPercussion.ElectricSnareDrum ||
                   p == MidiPercussion.SnareDrumRod ||
                   p == MidiPercussion.SnareDrumBrush ||
                   p == MidiPercussion.Tambourine ||
                   p == MidiPercussion.Shaker ||
                   p == MidiPercussion.Maracas ||
                   p == MidiPercussion.Cabasa ||
                   p == MidiPercussion.SleighBell ||
                   p == MidiPercussion.BellTree;
        }

        // --- PLAYBACK ENGINE ---

        public static void PlayPercussion(MidiPercussion p, CancellationToken ct = default, int maxMs = 5000, int velocity = 100)
        {
            int sid = Interlocked.Increment(ref _globalSessionId);
            Task.Run(() => ExecutePercussionPlayback(p, sid, ct, maxMs, velocity), ct);
        }

        private static void ExecutePercussionPlayback(MidiPercussion p, int sid, CancellationToken ct, int maxMs, int vel)
        {
            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice ?
                PercussionOutputChoice.SoundDevice : PercussionOutputChoice.SystemSpeaker;
            if (output == PercussionOutputChoice.SystemSpeaker) Interlocked.Exchange(ref _activeSpeakerSession, sid);
            else Interlocked.Exchange(ref _activeDeviceSession, sid);

            var prof = GetProfile(p, output);
            bool fadeOut = NeedsFadeOut(p);

            // Respect original profile decay length up to the maximum capped parameter
            int finalDuration = Math.Min(maxMs, prof.DurationMs);

            try
            {
                RenderProfile(output, sid, prof, ct, totalDurationOverrideMs: finalDuration, fadeOut: fadeOut);
            }
            finally { StopPulse(output, sid); }
        }

        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct = default, int velocity = 100)
        {
            if (durationMs <= 0) return Task.CompletedTask;
            int sid = Interlocked.Increment(ref _globalSessionId);
            return Task.Run(() => ExecutePercussionPlaybackForDuration(p, sid, ct, durationMs, velocity), ct);
        }

        private static void ExecutePercussionPlaybackForDuration(MidiPercussion p, int sid, CancellationToken ct, int durationMs, int vel)
        {
            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice ?
                PercussionOutputChoice.SoundDevice : PercussionOutputChoice.SystemSpeaker;
            if (output == PercussionOutputChoice.SystemSpeaker) Interlocked.Exchange(ref _activeSpeakerSession, sid);
            else Interlocked.Exchange(ref _activeDeviceSession, sid);

            var prof = GetProfile(p, output);
            bool fadeOut = NeedsFadeOut(p);

            try
            {
                RenderProfile(output, sid, prof, ct, durationMs, fadeOut: fadeOut);
            }
            finally { StopPulse(output, sid); }
        }

        private static void RenderProfile(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? totalDurationOverrideMs = null, bool fadeOut = false)
        {
            if (prof.HitCount <= 1)
            {
                PlaySingleHit(output, sid, prof, ct, totalDurationOverrideMs, fadeOut);
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

            for (int hit = 0; hit < prof.HitCount; hit++)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;
                PlaySingleHit(output, sid, prof, ct, perHitMs, fadeOut);
                if (hit < prof.HitCount - 1)
                    PreciseWaitMs(prof.HitGapMs, ct);
            }
        }

        private static void PlaySingleHit(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? durationMsOverride = null, bool fadeOut = false)
        {
            int duration = durationMsOverride ?? prof.DurationMs;
            if (duration <= 0) return;

            if (prof.BodyWave == SynthWave.Noise)
            {
                if (output == PercussionOutputChoice.SoundDevice && !fadeOut)
                {
                    StartPulse(output, prof.BodyStartFreq, SynthWave.Noise);
                    PreciseWaitMs(duration, ct);
                }
                else
                {
                    // Decay math is always anchored to prof.DurationMs
                    RenderGatedNoise(output, sid, prof.BodyStartFreq, duration, prof.NoiseDensity, prof.HoldRatio, prof.DurationMs, ct, fadeOut);
                }
            }
            else if (prof.DoesSweep)
            {
                // Sweeps are now rendered dynamically relative to the physical profile length
                RenderSweepTone(output, sid, prof, duration, ct);
            }
            else
            {
                if (fadeOut)
                {
                    // Gated decay is always anchored to prof.DurationMs
                    RenderGatedTone(output, sid, prof.BodyEndFreq, duration, prof.BodyWave, prof.HoldRatio, prof.DurationMs, ct);
                }
                else
                {
                    StartPulse(output, prof.BodyEndFreq, prof.BodyWave);
                    PreciseWaitMs(duration, ct);
                }
            }
        }

        // --- GATED RENDERING ENGINES ---

        private static void RenderGatedNoise(PercussionOutputChoice output, int sid, double baseFreq, int totalDurationMs, double noiseVol, double holdRatio, int originalDurationMs, CancellationToken ct, bool fadeOut = false)
        {
            double sampleFreq = baseFreq < 3500 ? baseFreq + 5000 : baseFreq + 1200;
            double sampleDurMs = 1000.0 / (sampleFreq + 0.25);

            var sw = Stopwatch.StartNew();
            double nextSampleMs = 0;
            bool speakerOn = false;
            double initialNoiseVol = noiseVol;

            while (sw.Elapsed.TotalMilliseconds < totalDurationMs)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                double currentNoiseVol = noiseVol;
                if (fadeOut)
                {
                    double elapsed = sw.Elapsed.TotalMilliseconds;
                    double progress = elapsed / originalDurationMs;

                    if (progress >= 1.0) break;

                    if (progress < holdRatio)
                    {
                        currentNoiseVol = initialNoiseVol;
                    }
                    else
                    {
                        double decayProgress = (progress - holdRatio) / (1.0 - holdRatio);
                        currentNoiseVol = initialNoiseVol * (1.0 - Math.Clamp(decayProgress, 0.0, 1.0));
                    }

                    // FIX: Cut off cleanly before the decay gets low enough to cause sputtering clicks
                    if (currentNoiseVol < 0.04) break;
                }

                bool wantOn = Random.Shared.NextDouble() < currentNoiseVol;
                if (wantOn != speakerOn)
                {
                    if (wantOn) StartPulse(output, (int)sampleFreq, SynthWave.Square);
                    else StopPulse(output, sid);
                    speakerOn = wantOn;
                }

                nextSampleMs += sampleDurMs;
                while (sw.Elapsed.TotalMilliseconds < nextSampleMs)
                {
                    if (ct.IsCancellationRequested) break;
                }
            }

            StopPulse(output, sid);
        }

        private static void RenderGatedTone(PercussionOutputChoice output, int sid, int frequency, int totalDurationMs, SynthWave waveType, double holdRatio, int originalDurationMs, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            const double cycleDurMs = 4.0;
            bool speakerOn = false; // Track the actual hardware state

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

                // FIX: Gate the sound off early if volume is negligible to avoid micro-clicks
                if (volume < 0.02) break;

                double onTime = cycleDurMs * volume;
                double offTime = cycleDurMs * (1.0 - volume);

                if (onTime > 0)
                {
                    // FIX: Only call StartPulse if the synthesizer isn't already playing
                    if (!speakerOn)
                    {
                        StartPulse(output, frequency, waveType);
                        speakerOn = true;
                    }
                    PreciseWaitMs(onTime, ct);
                }

                if (offTime > 0)
                {
                    // FIX: Only call StopPulse if the synthesizer is currently on
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
            const double stepDurMs = 8.0;
            int lastFreq = -1; // Keep track of the active frequency

            while (sw.Elapsed.TotalMilliseconds < totalDurationMs)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                double elapsed = sw.Elapsed.TotalMilliseconds;
                double progress = elapsed / prof.DurationMs;
                if (progress > 1.0) progress = 1.0;

                int freq = (int)(prof.BodyStartFreq - ((prof.BodyStartFreq - prof.BodyEndFreq) * progress));

                // FIX: Only re-trigger the pulse if the frequency has actually shifted values
                if (freq != lastFreq)
                {
                    StartPulse(output, freq, prof.BodyWave);
                    lastFreq = freq;
                }

                PreciseWaitMs(stepDurMs, ct);
            }
        }
    }
}