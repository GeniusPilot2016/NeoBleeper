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
            Laser = 27,
            Whip = 28,
            ScratchPush = 29,
            ScratchPull = 30,
            StickClick = 31,
            MetronomeClick = 33,
            MetronomeBell = 34,
            BassDrum = 35,
            KickDrum = 36,
            SideStick = 37,
            SnareCrossStick = 37,
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
            GuiroLong = 74,
            Claves = 75,
            HighWoodblock = 76,
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
            Güiro = 73,
            Clave = 75,
            WoodBlock = 76,
            SnareDrumRod = 91,
            OceanDrum = 92,
            SnareDrumBrush = 93
        }
        public static void PlayPercussion(MidiPercussion percussion, CancellationToken cancellationToken = default)
        {
            // Default values for percussion sounds
            int minFreq = 300;
            int maxFreq = 3000;
            int totalDurationMs = 150;
            int stepDurationMs = 8; // Step duration for pitch glide, if applicable

            bool isPitchGlide = false;
            double startFreq = 0;
            double endFreq = 0;

            // Percussion sound characteristics based on MIDI percussion number
            switch (percussion)
            {
                // Category 1: Pitch Glide Percussion (Kick Drum, Bass Drum, Laser
                case MidiPercussion.KickDrum:
                case MidiPercussion.BassDrum:
                    isPitchGlide = true; startFreq = 180; endFreq = 42; totalDurationMs = 110;
                    break;
                case MidiPercussion.Laser:
                    isPitchGlide = true; startFreq = 1600; endFreq = 250; totalDurationMs = 250;
                    break;
                case MidiPercussion.Whip:
                    isPitchGlide = true; startFreq = 3000; endFreq = 500; totalDurationMs = 120;
                    break;

                // Category 2: Long Cymbal & Hi-Hat Sounds
                // Kept audibly brighter than snare/toms for differentiation, but capped well
                // under 2000 Hz - the 2500-5500 Hz range used earlier kept getting flagged as
                // too high/piercing even after the original 5000-14000 Hz range was already
                // reduced once. This is a firm ceiling, not another halfway step.
                case MidiPercussion.HiHatOpen:
                case MidiPercussion.CrashCymbal:
                case MidiPercussion.CrashCymbal2:
                case MidiPercussion.SplashCymbal:
                case MidiPercussion.ChinaCymbal:
                case MidiPercussion.RideCymbal:
                case MidiPercussion.RideCymbal2:
                case MidiPercussion.RideBell:
                case MidiPercussion.Tambourine:
                case MidiPercussion.SleighBell:
                case MidiPercussion.BellTree:
                case MidiPercussion.Vibraslap:
                case MidiPercussion.WhistleLong:
                case MidiPercussion.OceanDrum:
                    minFreq = 500; maxFreq = 1800; totalDurationMs = 350;
                    break;

                // Category 3: Short Cymbal & Hi-Hat Sounds
                // Same rationale as Category 2 above.
                case MidiPercussion.HiHatClosed:
                case MidiPercussion.HiHatFoot:
                case MidiPercussion.StickClick:
                case MidiPercussion.MetronomeClick:
                case MidiPercussion.Claves:        // Scopes the Clave value (75)
                case MidiPercussion.HighWoodblock: // Scopes the HighWoodblock value (76)
                case MidiPercussion.LowWoodblock:
                case MidiPercussion.Cabasa:
                case MidiPercussion.Maracas:
                case MidiPercussion.Shaker:
                case MidiPercussion.TriangleMute:
                case MidiPercussion.TriangleOpen:
                case MidiPercussion.Castanets:
                case MidiPercussion.WhistleShort:
                    minFreq = 700; maxFreq = 2000; totalDurationMs = 50;
                    break;

                // Category 4: Snare Drum & Noise Percussion
                case MidiPercussion.SnareDrum:
                case MidiPercussion.ElectricSnareDrum:
                case MidiPercussion.SideStick:      // Scopes the SideStick value (37)
                case MidiPercussion.SnareDrumRod:
                case MidiPercussion.SnareDrumBrush:
                case MidiPercussion.HandClap:
                case MidiPercussion.ScratchPush:
                case MidiPercussion.ScratchPull:
                case MidiPercussion.GuiroShort:     // Scopes the GuiroShort value (73)
                case MidiPercussion.GuiroLong:
                    minFreq = 200; maxFreq = 1300; totalDurationMs = 160;
                    break;

                // Category 5: Tom Drums & Low Percussion
                // Real toms/bongos/congas have a clear pitched "thump" with a fast downward
                // pitch bend, much like the kick drum - they are not a noise source. Switched
                // from random-frequency noise to a pitch glide for a far more realistic,
                // recognizable drum-like attack instead of a low buzz.
                case MidiPercussion.FloorTom1:
                case MidiPercussion.FloorTom2:
                case MidiPercussion.LowTom:
                case MidiPercussion.LowMidTom:
                case MidiPercussion.HighMidTom:
                case MidiPercussion.HighTom:
                case MidiPercussion.HighBongo:
                case MidiPercussion.LowBongo:
                case MidiPercussion.Conga:
                case MidiPercussion.CongaDeadStroke:
                case MidiPercussion.Tumba:
                case MidiPercussion.HighTimbale:
                case MidiPercussion.LowTimbale:
                case MidiPercussion.CuicaHigh:
                case MidiPercussion.CuicaLow:
                case MidiPercussion.Surdu:
                case MidiPercussion.SurduDeadStroke:
                    isPitchGlide = true; startFreq = 260; endFreq = 85; totalDurationMs = 170;
                    break;

                // Category 6: Miscellaneous Percussion (Cowbell, Agogo, Metronome Bell)
                // These are definite-pitch instruments in a real kit, not noise sources.
                // Held as a steady tone (glide with equal start/end) instead of hopping
                // between random frequencies, for a clean "clonk" rather than static.
                case MidiPercussion.Cowbell:
                    isPitchGlide = true; startFreq = 800; endFreq = 800; totalDurationMs = 140;
                    break;
                case MidiPercussion.HighAgogo:
                    isPitchGlide = true; startFreq = 950; endFreq = 950; totalDurationMs = 120;
                    break;
                case MidiPercussion.LowAgogo:
                    isPitchGlide = true; startFreq = 650; endFreq = 650; totalDurationMs = 120;
                    break;
                case MidiPercussion.MetronomeBell:
                    isPitchGlide = true; startFreq = 1000; endFreq = 1000; totalDurationMs = 100;
                    break;
            }

            // Engine of the percussion sound generation in loop
            System.Random random = new System.Random();
            int elapsed = 0;

            // --- Attack + decay noise envelope ---
            // Real percussion isn't a flat, constant-texture sound for its whole duration -
            // it's a sharp, wideband noisy transient (the stick/mallet strike) that then
            // decays: the noise narrows and thins out as the sound fades. Using one constant
            // texture for the whole duration (what earlier revisions did) always ends up
            // sounding like either a flat drone or a string of discrete beeps, because
            // there's no shape to it - a real hit's initial "crack" and its fading tail have
            // different character, and that contrast is a big part of what reads as
            // percussive rather than tonal.
            //
            // Attack phase (short, ~20% of total duration): the LFSR picks frequencies from
            // across the FULL category range, stepped very fast - this is the noisy "crack".
            // Decay phase (remaining ~80%): frequency settles into a narrow low band near the
            // bottom of the range (so it doesn't wander into audibly distinct pitches), and
            // the step duration progressively lengthens, thinning out the noise density as
            // the hit fades - approximating amplitude decay without actual volume control.
            int attackMs = Math.Max(6, (int)(totalDurationMs * 0.2));
            attackMs = Math.Min(attackMs, totalDurationMs);
            int decayMs = Math.Max(1, totalDurationMs - attackMs);

            int decayCenter = minFreq + (int)((maxFreq - minFreq) * 0.12);
            int decayJitter = Math.Max(1, (int)((maxFreq - minFreq) * 0.06));
            int decayBandMin = Math.Max(minFreq, decayCenter - decayJitter);
            int decayBandMax = Math.Min(maxFreq, decayCenter + decayJitter);
            int decayRangeSize = Math.Max(1, decayBandMax - decayBandMin);
            int attackRangeSize = Math.Max(1, maxFreq - minFreq);

            ushort lfsr = (ushort)random.Next(1, ushort.MaxValue); // non-zero seed, unique per hit

            while (elapsed < totalDurationMs)
            {
                // Check for cancellation between steps so Stop() (or the next note event)
                // can interrupt a long cymbal/hi-hat sound instead of blocking until it
                // finishes on its own.
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                double currentFreq;
                int currentStepMs;

                if (isPitchGlide)
                {
                    // Slide the frequency from startFreq to endFreq over the total duration
                    // (startFreq == endFreq for held tones like cowbell/agogo, which just
                    // produces a steady pitch).
                    double t = (double)elapsed / totalDurationMs;
                    currentFreq = startFreq + (endFreq - startFreq) * t;
                    currentStepMs = stepDurationMs;
                }
                else
                {
                    // Advance the LFSR by one step. Standard (non-cryptographic) 16-bit
                    // Fibonacci LFSR - doesn't need to be perfect, just uncorrelated-looking
                    // from one step to the next.
                    int feedbackBit = ((lfsr >> 0) ^ (lfsr >> 2) ^ (lfsr >> 3) ^ (lfsr >> 5)) & 1;
                    lfsr = (ushort)((lfsr >> 1) | (feedbackBit << 15));

                    if (elapsed < attackMs)
                    {
                        // Attack: fast, wideband, chaotic - the percussive "crack".
                        currentFreq = minFreq + (lfsr % attackRangeSize);
                        currentStepMs = 2;
                    }
                    else
                    {
                        // Decay: narrow low band, with the step duration growing from fast
                        // to slow as the hit fades out, thinning the noise density over time.
                        double decayProgress = (double)(elapsed - attackMs) / decayMs; // 0..1
                        currentStepMs = 2 + (int)(decayProgress * 6); // 2ms -> 8ms
                        currentFreq = decayBandMin + (lfsr % decayRangeSize);
                    }
                }

                // Trigger the note without gap for the current frequency and step duration
                NotePlayer.PlayNoteWithoutGap((int)currentFreq, currentStepMs);

                elapsed += currentStepMs;
            }
        }
    }
}