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
        /// <remarks>The method supports natural notes (A-G) and sharps (e.g., "C#4"). The reference
        /// octave is the 4th octave (e.g., "A4" for 440 Hz). Flat notes are not supported; use the equivalent sharp
        /// notation (e.g., "D#" instead of "Eb").</remarks>
        /// <param name="noteName">The note name and octave in standard format (e.g., "A4", "C#3"). The note must consist of a letter (A-G), an
        /// optional sharp symbol ('#'), and a single-digit octave number. If null or empty, the frequency for middle C
        /// (C4) is returned.</param>
        /// <returns>The frequency in hertz for the specified note and octave. Returns 0 if the note name is invalid. Returns the
        /// frequency for middle C (C4) if the input is null, empty, or cannot be parsed.</returns>
        public static double GetFrequencyFromNoteName(string noteName)
        {
            if (string.IsNullOrEmpty(noteName))
                return NoteUtility.BaseNoteFrequencyIn4thOctave.C;
            try
            {
                // Disassemble note name into note and octave
                var m = Regex.Match(noteName.ToUpperInvariant(), @"^([A-G])(#?)(\d+)$");
                if (!m.Success) return 0;
                string note = m.Groups[1].Value + (m.Groups[2].Value == "#" ? "#" : "");
                int octave = int.Parse(m.Groups[3].Value);
                // Basic frequency for the note in the 4th octave
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
                    _ => 0 // Invalid note
                };

                if (baseFrequency == 0)
                    return 0;

                // Calculate the frequency based on the octave
                int octaveDifference = octave - 4; // 4th octave is the reference octave
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
        /// <summary>
        /// Calculates the effective note length based on the specified articulation.
        /// </summary>
        /// <remarks>Use this method to modify note durations according to articulation markings commonly
        /// used in music notation. For example, staccato halves the note length, while spiccato quarters it.</remarks>
        /// <param name="length">The original length of the note to be adjusted.</param>
        /// <param name="articulation">The articulation type to apply. Supported values are "Sta" for staccato and "Spi" for spiccato. If not
        /// specified or unrecognized, no adjustment is made.</param>
        /// <returns>The adjusted note length after applying the specified articulation.</returns>
        public static double CalculateNoteLength(double length, string articulation = "")
        {
            // Use double precision for length calculations to maintain accuracy when applying articulations
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

        /// <summary>
        /// Calculates the duration, in milliseconds, of a musical note based on the specified beats per minute (BPM),
        /// note type, and optional modifier.
        /// </summary>
        /// <remarks>This method is useful for timing calculations in music applications, such as
        /// sequencers or metronomes. The calculation uses double precision to ensure accuracy for fractional note
        /// values.</remarks>
        /// <param name="bpm">The tempo in beats per minute. If set to 0, a default value of 1 is used to avoid division by zero.</param>
        /// <param name="noteType">The type of note to calculate the duration for. Supported values include "Whole", "Half", "Quarter", "1/8",
        /// "1/16", and "1/32". If an unsupported value is provided, the calculation defaults to a quarter note.</param>
        /// <param name="modifier">An optional modifier that alters the note duration. Supported values are "Dot" (increases duration by 50%)
        /// and "Tri" (divides duration by 3). If not specified or unrecognized, no modification is applied.</param>
        /// <returns>The duration of the specified note, in milliseconds, after applying the BPM and any modifier.</returns>
        public static double CalculateLineLength(int bpm, string noteType, string modifier = "")
        {
            if (bpm == 0) bpm = 1;
            // Use double precision for beat and note length calculations to avoid integer truncation
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

        /// <summary>
        /// Calculates the total rhythm slot and the audible note duration for a musical note based on its length,
        /// tempo, modifier, articulation, and silence ratio.
        /// </summary>
        /// <remarks>If the articulation is 'fermata', both the total rhythm slot and the audible duration
        /// are extended proportionally. The silence ratio is applied only to the audible portion of the note.</remarks>
        /// <param name="lengthName">The name of the note length (e.g., quarter, eighth) to determine the base duration.</param>
        /// <param name="bpm">The tempo in beats per minute. If set to 0, a default value of 1 is used.</param>
        /// <param name="modifier">A string representing any note length modifier (such as dot or triplet) to adjust the duration.</param>
        /// <param name="articulation">The articulation style applied to the note (e.g., staccato, legato, fermata), which may affect the duration.</param>
        /// <param name="noteSilenceRatio">The proportion of the note's duration that is audible, as a value between 0.0 and 1.0.</param>
        /// <returns>A tuple containing the total rhythm slot in integer units and the audible note duration in integer units.
        /// The audible duration will not exceed the total rhythm slot and will not be negative.</returns>
        public static (int totalRhythm_int, int noteSound_int) CalculateNoteDurations(
            string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio)
        {
            if (bpm == 0)
                bpm = 1;

            var (lengthName_checked, modifier_checked, articulation_checked) =
                UseOriginalValueOrDefault(lengthName, modifier, articulation);

            // --- Step 1: total rhythm slot (modifier applied exactly once here) ---
            double totalRhythm_double = FixRoundingErrors(
                CalculateLineLength(bpm, lengthName_checked, modifier_checked));

            // --- Step 2: audible note duration ---
            double noteSound_double = FixRoundingErrors(
                CalculateNoteLength(totalRhythm_double, articulation_checked));

            // --- Step 3: fermata extends both the slot and the sound proportionally ---
            if (articulation_checked == "Fer")
            {
                double extraFermataDuration = totalRhythm_double * (0.5 + 0.5 * Random.Shared.NextDouble());
                totalRhythm_double += extraFermataDuration;
                // Recalculate noteSound so it stays proportional to the extended slot
                noteSound_double = FixRoundingErrors(
                    CalculateNoteLength(totalRhythm_double, articulation_checked));
            }

            // --- Step 4: apply silence ratio to the audible portion only ---
            noteSound_double *= noteSilenceRatio;

            // --- Step 5: round once at the very end ---
            int totalRhythm_int = (int)Math.Round(totalRhythm_double, MidpointRounding.AwayFromZero);
            int noteSound_int = (int)Math.Round(noteSound_double, MidpointRounding.AwayFromZero);

            // Guard: audible sound must fit inside rhythm slot and must not be negative
            if (noteSound_int > totalRhythm_int) noteSound_int = totalRhythm_int;
            if (noteSound_int < 0) noteSound_int = 0;

            return (totalRhythm_int, noteSound_int);
        }

        /// <summary>
        /// Accumulation-safe variant of <see cref="CalculateNoteDurations"/> for multi-part
        /// synchronous playback.
        /// </summary>
        /// <remarks>
        /// When multiple simultaneous parts advance time by summing per-note integer millisecond
        /// values, floating-point rounding accumulates and parts drift out of sync. Example at
        /// 90 BPM: a quarter note is 666.666... ms, rounded to 667 ms. After 100 notes the
        /// accumulated error is ~33 ms — clearly audible.
        ///
        /// This method takes a shared, per-track cursorMs (a running double) and derives integer
        /// sleep/beep durations from the absolute cursor position rather than summing already-
        /// rounded integers, so rounding errors never compound across notes.
        ///
        /// Usage pattern:
        ///   double cursor = 0.0;
        ///   foreach (var note in track)
        ///   {
        ///       var (rhythmMs, soundMs, nextCursor) = NoteLengths.CalculateNoteDurationsAtPosition(
        ///           note.Length, bpm, note.Modifier, note.Articulation, note.SilenceRatio, cursor);
        ///
        ///       Console.Beep(note.Frequency, soundMs);
        ///       Thread.Sleep(rhythmMs - soundMs); // silence gap within the slot
        ///       cursor = nextCursor;              // advance cursor in double space
        ///   }
        /// </remarks>
        /// <param name="lengthName">Note type string (e.g., "Quarter", "1/8"). Unknown values default to "Quarter".</param>
        /// <param name="bpm">Tempo in beats per minute.</param>
        /// <param name="modifier">Optional modifier ("Dot" or "Tri"). Unknown values are ignored.</param>
        /// <param name="articulation">Optional articulation ("Sta", "Spi", or "Fer"). Unknown values are ignored.</param>
        /// <param name="noteSilenceRatio">Fraction of the total rhythm that the note actually sounds (0.0-1.0).</param>
        /// <param name="cursorMs">
        /// The current absolute playback position in milliseconds as a double. Must be maintained
        /// by the caller and advanced by nextCursorMs after each note.
        /// </param>
        /// <returns>
        /// A tuple of:
        ///   totalRhythm_int — integer ms to advance the clock for this note slot.
        ///   noteSound_int   — integer ms the note should sound.
        ///   nextCursorMs    — the new cursor value (double) to pass for the following note.
        /// </returns>
        public static (int totalRhythm_int, int noteSound_int, double nextCursorMs) CalculateNoteDurationsAtPosition(
            string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio,
            double cursorMs)
        {
            if (bpm == 0)
                bpm = 1;

            var (lengthName_checked, modifier_checked, articulation_checked) =
                UseOriginalValueOrDefault(lengthName, modifier, articulation);

            // Exact (double) rhythm slot with modifier applied once
            double totalRhythm_double = FixRoundingErrors(
                CalculateLineLength(bpm, lengthName_checked, modifier_checked));

            // Fermata extension
            if (articulation_checked == "Fer")
            {
                double extra = totalRhythm_double * (0.5 + 0.5 * Random.Shared.NextDouble());
                totalRhythm_double += extra;

            }

            // Audible portion (no modifier — already in totalRhythm_double)
            double noteSound_double = FixRoundingErrors(
                CalculateNoteLength(totalRhythm_double, articulation_checked)) * noteSilenceRatio;

            // FIX: derive integer durations from the absolute cursor so rounding errors
            // never accumulate across successive notes.
            double nextCursor = cursorMs + totalRhythm_double;
            int totalRhythm_int = (int)Math.Round(nextCursor) - (int)Math.Round(cursorMs);
            int noteSound_int = (int)Math.Round(noteSound_double, MidpointRounding.AwayFromZero);

            if (noteSound_int > totalRhythm_int) noteSound_int = totalRhythm_int;
            if (noteSound_int < 0) noteSound_int = 0;

            return (totalRhythm_int, noteSound_int, nextCursor);
        }

        /// <summary>
        /// Adjusts the specified floating-point value to reduce the impact of minor rounding errors near zero.
        /// </summary>
        /// <remarks>This method is useful when small floating-point inaccuracies could affect subsequent
        /// calculations or comparisons, particularly for values close to zero. The adjustment is only applied if the
        /// absolute value of the input exceeds a small threshold.</remarks>
        /// <param name="inputValue">The double-precision floating-point value to be corrected for potential rounding errors.</param>
        /// <returns>A double value with minor rounding errors adjusted. The returned value may be slightly increased or
        /// decreased if it is sufficiently far from zero; otherwise, it is returned unchanged.</returns>
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
            // FX / Click
            Laser = 27, Whip = 28, ScratchPush = 29, ScratchPull = 30, StickClick = 31,
            SquareClick = 32, MetronomeClick = 33, MetronomeBell = 34,

            // Kicks / Toms
            BassDrum = 35, KickDrum = 36, LowTom = 45, LowMidTom = 47, HighMidTom = 48,
            HighTom = 50, FloorTom1 = 43, FloorTom2 = 41,

            // Snares
            SideStick = 37, SnareCrossStick = 37, SnareDrum = 38, ElectricSnareDrum = 40,
            SnareDrumRod = 91, SnareDrumBrush = 93,

            // Cymbals / Hi-Hats
            HiHatClosed = 42, HiHatOpen = 46, HiHatFoot = 44, CrashCymbal = 49,
            CrashCymbal2 = 57, RideCymbal = 51, RideCymbal2 = 59, ChinaCymbal = 52,
            SplashCymbal = 55, RideBell = 53,

            // Latin / Percussion
            HandClap = 39, Tambourine = 54, Vibraslap = 58, Cowbell = 56,
            HighBongo = 60, LowBongo = 61, CongaDeadStroke = 62, Conga = 63, Tumba = 64,
            HighTimbale = 65, LowTimbale = 66, HighAgogo = 67, LowAgogo = 68,
            Cabasa = 69, Maracas = 70, Shaker = 82, SleighBell = 83, BellTree = 84,
            Castanets = 85, SurduDeadStroke = 86, Surdu = 87, CuicaHigh = 78, CuicaLow = 79,

            // Wood / Scrapers
            GuiroShort = 73, GuiroLong = 74, Güiro = 73, Claves = 75, Clave = 75,
            HighWoodblock = 76, LowWoodblock = 77, WoodBlock = 76,

            // Other
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

            // Convert milliseconds directly to a TimeSpan for high-precision mapping
            TimeSpan timeout = TimeSpan.FromMicroseconds(ms * 1000);

            try
            {
                // Blocks the thread completely until either the timeout hits
                // OR the cancellation token triggers the ct.WaitHandle.
                // Returns true if timeout hit, returns false if canceled.
                bool timedOut = ct.WaitHandle.WaitOne(timeout);

                if (!timedOut && ct.IsCancellationRequested)
                {
                    // Thread woke up because of cancellation
                    return;
                }
            }
            catch (ObjectDisposedException)
            {
                // Safe guard in case the token source is disposed mid-wait
                return;
            }
        }

        /// <summary>
        /// Describes one percussion voice's synthesis recipe.
        /// </summary>
        /// <remarks>
        /// Neither output path has a real multi-partial timbre generator: the system speaker is a
        /// single monophonic square-wave oscillator (frequency + on/off only), and the sound-device
        /// path is a single-oscillator synth (square/triangle/white-noise). Realism therefore has to
        /// come from three levers that both paths actually support:
        ///   1. Frequency choice / sweep shape (pitched drums are simulated as a fast downward pitch
        ///      sweep, since a real drum's fundamental audibly drops as the head's tension "settles"
        ///      immediately after the strike).
        ///   2. Duration (how long the transient/decay lasts for that instrument in real life).
        ///   3. NoiseDensity — for noise-based voices rendered on the system speaker, this is the
        ///      per-sample gating probability used by <see cref="RenderGatedNoise"/>. A higher density
        ///      reads as louder/brighter/denser (snare, closed hi-hat); a lower density reads as
        ///      sparser/softer/airier (brushes, shakers, cymbal wash, ocean drum).
        ///   4. HitCount/HitGapMs — some instruments are not a single transient at all but a fast
        ///      cluster of discrete strikes (a hand clap is ~3 slaps ~15-20ms apart; a long güiro
        ///      stroke is a series of scrape ticks). Modeling that cluster is far closer to the real
        ///      instrument than one long tone.
        /// </remarks>
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

            public PercussionProfile(SynthWave w, bool s, int start, int end, int dur,
                double density = 0.5, int hits = 1, int gap = 0)
            {
                BodyWave = w; DoesSweep = s; BodyStartFreq = start; BodyEndFreq = end; DurationMs = dur;
                NoiseDensity = density; HitCount = Math.Max(1, hits); HitGapMs = gap;
            }
        }

        private static PercussionProfile GetProfile(MidiPercussion p, PercussionOutputChoice output) => p switch
        {
            // ================= Kicks & Toms (triangle pitch-drop sweep) =================
            // Real kick/tom fundamentals audibly drop right after the strike as head tension
            // settles, so a short fast sweep from a higher start pitch down to the true
            // fundamental reads far more like a drum than a static tone.
            MidiPercussion.KickDrum or MidiPercussion.BassDrum =>
                new PercussionProfile(SynthWave.Triangle, true, 170, 48, 60),
            MidiPercussion.HighTom =>
                new PercussionProfile(SynthWave.Triangle, true, 320, 140, 85),
            MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom =>
                new PercussionProfile(SynthWave.Triangle, true, 210, 105, 110),
            MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 =>
                new PercussionProfile(SynthWave.Triangle, true, 130, 65, 150),

            // ================= Snares (white noise, tuned brightness/density) =================
            // Sticks are bright and dense (snappy wire buzz); rods are a touch warmer and softer;
            // brushes are the warmest and softest, with a longer swish-like decay.
            MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum =>
                new PercussionProfile(SynthWave.Noise, false, 4200, 4200, 140, density: 0.60),
            MidiPercussion.SnareDrumRod =>
                new PercussionProfile(SynthWave.Noise, false, 3200, 3200, 110, density: 0.45),
            MidiPercussion.SnareDrumBrush =>
                new PercussionProfile(SynthWave.Noise, false, 2600, 2600, 160, density: 0.30),

            // ================= Cymbals =================
            // Crash/china: bright, long, dense wash. Ride: similarly bright but sparser gating,
            // which reads more like a shimmering "ping" than a full wash.
            MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 7000, 7000, 480, density: 0.55),
            MidiPercussion.SplashCymbal =>
                new PercussionProfile(SynthWave.Noise, false, 7500, 7500, 220, density: 0.55),
            MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                new PercussionProfile(SynthWave.Noise, false, 6200, 6200, 550, density: 0.35),

            // ================= Hi-Hats =================
            // Closed: very short and dense (tight "chick"). Open: longer and sparser (airy decay).
            // Foot chick: short, muffled (lower frequency), moderate density.
            MidiPercussion.HiHatClosed =>
                new PercussionProfile(SynthWave.Noise, false, 8500, 8500, 45, density: 0.70),
            MidiPercussion.HiHatOpen =>
                new PercussionProfile(SynthWave.Noise, false, 8000, 8000, 320, density: 0.40),
            MidiPercussion.HiHatFoot =>
                new PercussionProfile(SynthWave.Noise, false, 3000, 3000, 35, density: 0.50),

            // ================= Hand claps / vibraslap (multi-hit) =================
            // A real hand clap is not one transient — it's ~3 rapid slaps only ~15-20ms apart.
            // Modeling that cluster (instead of one 100ms noise burst) is the single biggest
            // realism win available on this synth.
            MidiPercussion.HandClap =>
                new PercussionProfile(SynthWave.Noise, false, 2800, 2800, 25, density: 0.65, hits: 3, gap: 18),
            MidiPercussion.Vibraslap =>
                new PercussionProfile(SynthWave.Noise, false, 2200, 2200, 350, density: 0.25),

            // ================= Clicks / metronome (short dense noise "tick") =================
            MidiPercussion.SideStick or MidiPercussion.SnareCrossStick or MidiPercussion.StickClick or
            MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.MetronomeBell =>
                new PercussionProfile(SynthWave.Noise, false, 900, 900, 15, density: 0.80),

            // ================= Wood blocks / claves / castanets =================
            // Real wood strikes decay almost instantly — keep these very short and pitched by size.
            MidiPercussion.Claves or MidiPercussion.Clave or MidiPercussion.Castanets =>
                new PercussionProfile(SynthWave.Triangle, false, 2000, 2000, 15),
            MidiPercussion.HighWoodblock =>
                new PercussionProfile(SynthWave.Triangle, false, 1600, 1600, 18),
            MidiPercussion.LowWoodblock or MidiPercussion.WoodBlock =>
                new PercussionProfile(SynthWave.Triangle, false, 1100, 1100, 22),

            // ================= Bongos / congas / timbales / surdu =================
            // Muted ("dead stroke") hits are short with little pitch drop; open hits ring
            // longer and sweep further as the head relaxes.
            MidiPercussion.HighBongo =>
                new PercussionProfile(SynthWave.Triangle, true, 260, 150, 55),
            MidiPercussion.LowBongo =>
                new PercussionProfile(SynthWave.Triangle, true, 190, 105, 65),
            MidiPercussion.CongaDeadStroke =>
                new PercussionProfile(SynthWave.Triangle, true, 220, 150, 35),
            MidiPercussion.Conga =>
                new PercussionProfile(SynthWave.Triangle, true, 220, 100, 90),
            MidiPercussion.Tumba =>
                new PercussionProfile(SynthWave.Triangle, true, 170, 85, 100),
            MidiPercussion.HighTimbale =>
                new PercussionProfile(SynthWave.Triangle, true, 500, 300, 60),
            MidiPercussion.LowTimbale =>
                new PercussionProfile(SynthWave.Triangle, true, 380, 220, 75),
            MidiPercussion.SurduDeadStroke =>
                new PercussionProfile(SynthWave.Triangle, true, 150, 85, 50),
            MidiPercussion.Surdu =>
                new PercussionProfile(SynthWave.Triangle, true, 140, 60, 130),

            // ================= Bells / agogo / cowbell / triangle =================
            // A real orchestral triangle "open" rings for a genuinely long time; muted, it stops
            // almost instantly — the two articulations should sound very different in duration.
            MidiPercussion.Cowbell =>
                new PercussionProfile(SynthWave.Triangle, false, 800, 800, 90),
            MidiPercussion.RideBell =>
                new PercussionProfile(SynthWave.Triangle, false, 1400, 1400, 130),
            MidiPercussion.HighAgogo =>
                new PercussionProfile(SynthWave.Triangle, false, 950, 950, 60),
            MidiPercussion.LowAgogo =>
                new PercussionProfile(SynthWave.Triangle, false, 700, 700, 70),
            MidiPercussion.TriangleMute =>
                new PercussionProfile(SynthWave.Triangle, false, 3800, 3800, 25),
            MidiPercussion.TriangleOpen =>
                new PercussionProfile(SynthWave.Triangle, false, 3800, 3800, 400),
            MidiPercussion.SleighBell or MidiPercussion.BellTree =>
                new PercussionProfile(SynthWave.Triangle, false, 2600, 2600, 220, hits: 4, gap: 25),

            // ================= Shakers / scrapers =================
            // Sparse gating reads as a granular "shhh" texture rather than a solid tone, which is
            // much closer to how these instruments actually sound.
            MidiPercussion.Maracas =>
                new PercussionProfile(SynthWave.Noise, false, 5500, 5500, 55, density: 0.35),
            MidiPercussion.Cabasa =>
                new PercussionProfile(SynthWave.Noise, false, 5000, 5000, 70, density: 0.50),
            MidiPercussion.Shaker or MidiPercussion.Tambourine =>
                new PercussionProfile(SynthWave.Noise, false, 6000, 6000, 90, density: 0.30),
            MidiPercussion.GuiroShort or MidiPercussion.Güiro =>
                new PercussionProfile(SynthWave.Noise, false, 3500, 3500, 55, density: 0.60),
            // A long güiro stroke is a rapid run of individual scrape ticks, not one sustained burst.
            MidiPercussion.GuiroLong =>
                new PercussionProfile(SynthWave.Noise, false, 3500, 3500, 40, density: 0.60, hits: 5, gap: 18),
            MidiPercussion.ScratchPush or MidiPercussion.ScratchPull =>
                new PercussionProfile(SynthWave.Noise, false, 4200, 4200, 80, density: 0.45),
            MidiPercussion.OceanDrum =>
                new PercussionProfile(SynthWave.Noise, false, 1800, 1800, 500, density: 0.15),

            // ================= Exceptions (square, for recognizable FX tone) =================
            MidiPercussion.WhistleShort =>
                new PercussionProfile(SynthWave.Square, false, 1800, 1800, 130),
            MidiPercussion.WhistleLong =>
                new PercussionProfile(SynthWave.Square, false, 1800, 1800, 500),
            MidiPercussion.Laser =>
                new PercussionProfile(SynthWave.Square, true, 2500, 200, 150),
            MidiPercussion.Whip =>
                new PercussionProfile(SynthWave.Square, true, 1800, 150, 90),
            MidiPercussion.CuicaHigh =>
                new PercussionProfile(SynthWave.Triangle, true, 900, 500, 140),
            MidiPercussion.CuicaLow =>
                new PercussionProfile(SynthWave.Triangle, true, 500, 250, 180),

            _ => new PercussionProfile(SynthWave.Square, false, 400, 400, 30)
        };

        public static void PlayPercussion(MidiPercussion p, CancellationToken ct = default, int maxMs = 500, int velocity = 100)
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

            try
            {
                RenderProfile(output, sid, prof, ct);
            }
            finally { StopPulse(output, sid); }
        }

        /// <summary>
        /// Plays a percussion hit for an exact, caller-specified duration and awaits completion.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="PlayPercussion"/>, which is fire-and-forget and uses the instrument's own
        /// natural profile duration, this method is designed to be interleaved with melody notes inside
        /// a shared, single-voice time slot (see
        /// <c>MIDIFilePlayer.PlayNotesAndPercussionAlternatingAsync</c>). Because the system speaker
        /// (and the equivalent single-oscillator sound-device path) can only render one voice at a time,
        /// callers must always <c>await</c> this method to completion before starting any other voice —
        /// never call it "fire-and-forget" alongside a note that is still sounding, or the two will
        /// overlap on the hardware.
        ///
        /// The sweep-style profiles (e.g. kicks, toms) scale their step count down for very short slices
        /// so a fast alternation cycle doesn't try to cram a 6-step sweep into a couple of milliseconds.
        /// Multi-hit profiles (e.g. hand claps, long güiro scrapes) are compressed to fit exactly inside
        /// the caller-specified duration rather than being skipped.
        /// </remarks>
        /// <param name="p">The percussion instrument to play.</param>
        /// <param name="durationMs">The exact duration, in milliseconds, the hit should occupy. Values &lt;= 0 complete immediately without sound.</param>
        /// <param name="ct">A cancellation token that aborts playback early.</param>
        /// <param name="velocity">Reserved for future velocity-sensitive rendering; currently unused.</param>
        /// <returns>A task that completes when the percussion hit has finished sounding (or been canceled).</returns>
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

            try
            {
                RenderProfile(output, sid, prof, ct, durationMs);
            }
            finally { StopPulse(output, sid); }
        }

        /// <summary>
        /// Renders a full percussion profile: either a single hit (the common case) or, for
        /// instruments modeled as a fast cluster of strikes (hand claps, long güiro scrapes,
        /// sleigh bells), a sequence of hits separated by short gaps. When
        /// <paramref name="totalDurationOverrideMs"/> is supplied (the exact-duration playback
        /// path), the hits and gaps are proportionally compressed so the whole cluster still fits
        /// exactly inside the caller's time slot.
        /// </summary>
        private static void RenderProfile(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? totalDurationOverrideMs = null)
        {
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

            for (int hit = 0; hit < prof.HitCount; hit++)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;
                PlaySingleHit(output, sid, prof, ct, perHitMs);
                if (hit < prof.HitCount - 1)
                    PreciseWaitMs(prof.HitGapMs, ct);
            }
        }

        /// <summary>
        /// Renders exactly one strike of a profile for the given duration (or the profile's own
        /// natural duration if none is supplied).
        /// </summary>
        private static void PlaySingleHit(PercussionOutputChoice output, int sid, PercussionProfile prof,
            CancellationToken ct, int? durationMsOverride = null)
        {
            int duration = durationMsOverride ?? prof.DurationMs;
            if (duration <= 0) return;

            if (prof.BodyWave == SynthWave.Noise)
            {
                if (output == PercussionOutputChoice.SoundDevice)
                {
                    StartPulse(output, prof.BodyStartFreq, SynthWave.Noise);
                    PreciseWaitMs(duration, ct);
                }
                else
                {
                    RenderGatedNoise(output, sid, prof.BodyStartFreq, duration, prof.NoiseDensity, ct);
                }
            }
            else if (prof.DoesSweep)
            {
                // Scale step count down for very short slices so a fast alternation cycle
                // doesn't try to cram a 6-step sweep into a couple of milliseconds.
                int steps = durationMsOverride.HasValue
                    ? Math.Max(2, Math.Min(6, duration / 5))
                    : 6;
                for (int i = 0; i < steps; i++)
                {
                    if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;
                    double progress = (double)i / (steps - 1);
                    int freq = (int)(prof.BodyStartFreq - ((prof.BodyStartFreq - prof.BodyEndFreq) * progress));
                    StartPulse(output, freq, prof.BodyWave);
                    PreciseWaitMs((double)duration / steps, ct);
                }
            }
            else
            {
                StartPulse(output, prof.BodyEndFreq, prof.BodyWave);
                PreciseWaitMs(duration, ct);
            }
        }

        /// <summary>
        /// Ports the core noise-generation technique from the reference PC-speaker
        /// implementation: independent per-sample probability gating (equivalent to
        /// "Rnd &lt; NoiseVol"), sampled at a rate derived from the instrument's own
        /// channel frequency with the same brightness-dependent shift the reference
        /// applies before computing the sample period. This is genuine amplitude-
        /// domain randomness — not frequency modulation — which is what actually
        /// reads as broadband noise instead of a tone/squeak.
        /// </summary>
        private static void RenderGatedNoise(PercussionOutputChoice output, int sid, double baseFreq, int totalDurationMs, double noiseVol, CancellationToken ct)
        {
            // Same brightness shift as the reference: brighten low-frequency sources
            // more aggressively so the noise doesn't sound muddy relative to the
            // instrument it's representing.
            double sampleFreq = baseFreq < 3500 ? baseFreq + 5000 : baseFreq + 1200;
            double sampleDurMs = 1000.0 / (sampleFreq + 0.25);

            var sw = Stopwatch.StartNew();
            double nextSampleMs = 0;
            bool speakerOn = false;

            while (sw.Elapsed.TotalMilliseconds < totalDurationMs)
            {
                if (ct.IsCancellationRequested || !IsSessionActive(output, sid)) break;

                bool wantOn = Random.Shared.NextDouble() < noiseVol;
                if (wantOn != speakerOn)
                {
                    if (wantOn) StartPulse(output, (int)sampleFreq, SynthWave.Square);
                    else StopPulse(output, sid);
                    speakerOn = wantOn;
                }

                // Bounded poll to the next sample boundary only — mirrors the
                // reference's QueryPerformanceCounter loop, which never spins past
                // one sample period at a time.
                nextSampleMs += sampleDurMs;
                while (sw.Elapsed.TotalMilliseconds < nextSampleMs)
                {
                    if (ct.IsCancellationRequested) break;
                }
            }

            StopPulse(output, sid);
        }
    }
}