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
        public enum PercussionOutputChoice
        {
            SystemSpeaker,
            SoundDevice
        }

        private static void StartPulse(PercussionOutputChoice outputChoice, int frequency)
        {
            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                    break;
                case PercussionOutputChoice.SoundDevice:
                    switch (TemporarySettings.CreatingSounds.soundDeviceBeepWaveform)
                    {
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Square:
                            {
                                SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.Square, frequency);
                                break;
                            }
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Sine:
                            {
                                SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.Sin, frequency);
                                break;
                            }
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Triangle:
                            {
                                SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.Triangle, frequency);
                                break;
                            }
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Noise:
                            {
                                SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.White, frequency);
                                break;
                            }
                    }
                    break;
            }
        }

        private static void StopPulse(PercussionOutputChoice outputChoice)
        {
            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                    break;
                case PercussionOutputChoice.SoundDevice:
                    SoundRenderingEngine.WaveSynthEngine.StopSynth();
                    break;
            }
        }

        public static void PlayPercussion(MidiPercussion percussion, CancellationToken cancellationToken = default, int maxDurationMs = 55, int velocity = 100)
        {
            PercussionOutputChoice outputChoice = TemporarySettings.CreatingSounds.createBeepWithSoundDevice == true ? PercussionOutputChoice.SoundDevice : PercussionOutputChoice.SystemSpeaker;
            // PC speaker percussion has no volume control.  Long high-frequency beeps are
            // perceived as painful, so keep every hit short and keep the noise bands low.
            // The goal is the BaWaMI-style 1-bit percussion: a short transient/noisy tick,
            // not a realistic cymbal tail.
            int minFreq = 180;
            int maxFreq = 900;
            int totalDurationMs = 36;
            int stepDurationMs = 2;
            int grainDurationMs = 1; // noise grains must be very short, otherwise the texture turns into slow chirps

            bool isPitchGlide = false;
            double startFreq = 0;
            double endFreq = 0;

            switch (percussion)
            {
                // Low drums: short downward pitch bend.
                case MidiPercussion.KickDrum:
                case MidiPercussion.BassDrum:
                    isPitchGlide = true; startFreq = 145; endFreq = 55; totalDurationMs = 36; stepDurationMs = 2;
                    break;

                // Special effects.  Keep the laser/whip lower than before; PC speakers make
                // 2-3 kHz very piercing.
                case MidiPercussion.Laser:
                    isPitchGlide = true; startFreq = 900; endFreq = 220; totalDurationMs = 45; stepDurationMs = 2;
                    break;
                case MidiPercussion.Whip:
                    isPitchGlide = true; startFreq = 1200; endFreq = 300; totalDurationMs = 30; stepDurationMs = 2;
                    break;

                // Cymbals / open hats: noisy, but capped low and short.  The old 350 ms
                // duration was the main playback-scheduling freeze during dense drum parts.
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
                    minFreq = 520; maxFreq = 1350; totalDurationMs = 42; stepDurationMs = 2; grainDurationMs = 1;
                    break;

                // Closed hats / ticks / shakers: very short transient.
                case MidiPercussion.HiHatClosed:
                case MidiPercussion.HiHatFoot:
                case MidiPercussion.Cabasa:
                case MidiPercussion.Maracas:
                case MidiPercussion.Shaker:
                case MidiPercussion.TriangleMute:
                case MidiPercussion.TriangleOpen:
                case MidiPercussion.Castanets:
                case MidiPercussion.WhistleShort:
                    minFreq = 620; maxFreq = 1450; totalDurationMs = 22; stepDurationMs = 2; grainDurationMs = 1;
                    break;

                // Wooden/metal clicks: a definite short pitch reads better than random high noise.
                case MidiPercussion.StickClick:
                case MidiPercussion.MetronomeClick:
                case MidiPercussion.Claves:
                case MidiPercussion.HighWoodblock:
                case MidiPercussion.LowWoodblock:
                    isPitchGlide = true; startFreq = percussion == MidiPercussion.LowWoodblock ? 620 : 820; endFreq = startFreq; totalDurationMs = 15; stepDurationMs = 2;
                    break;

                // Snares/claps/scratches/guiros: low-mid noise burst.
                case MidiPercussion.SnareDrum:
                case MidiPercussion.ElectricSnareDrum:
                case MidiPercussion.SideStick:
                case MidiPercussion.SnareDrumRod:
                case MidiPercussion.SnareDrumBrush:
                case MidiPercussion.HandClap:
                case MidiPercussion.ScratchPush:
                case MidiPercussion.ScratchPull:
                case MidiPercussion.GuiroShort:
                case MidiPercussion.GuiroLong:
                    minFreq = 220; maxFreq = 1050; totalDurationMs = 32; stepDurationMs = 2; grainDurationMs = 1;
                    break;

                // Toms / bongos / congas: short downward pitch bends, with rough pitch by drum size.
                case MidiPercussion.FloorTom2:
                    isPitchGlide = true; startFreq = 165; endFreq = 70; totalDurationMs = 38; stepDurationMs = 2;
                    break;
                case MidiPercussion.FloorTom1:
                    isPitchGlide = true; startFreq = 185; endFreq = 78; totalDurationMs = 38; stepDurationMs = 2;
                    break;
                case MidiPercussion.LowTom:
                case MidiPercussion.LowMidTom:
                    isPitchGlide = true; startFreq = 230; endFreq = 92; totalDurationMs = 34; stepDurationMs = 2;
                    break;
                case MidiPercussion.HighMidTom:
                case MidiPercussion.HighTom:
                    isPitchGlide = true; startFreq = 310; endFreq = 125; totalDurationMs = 32; stepDurationMs = 2;
                    break;
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
                    isPitchGlide = true; startFreq = 360; endFreq = 145; totalDurationMs = 28; stepDurationMs = 2;
                    break;

                // Pitched percussion.
                case MidiPercussion.Cowbell:
                    isPitchGlide = true; startFreq = 620; endFreq = 620; totalDurationMs = 22; stepDurationMs = 2;
                    break;
                case MidiPercussion.HighAgogo:
                    isPitchGlide = true; startFreq = 760; endFreq = 760; totalDurationMs = 20; stepDurationMs = 2;
                    break;
                case MidiPercussion.LowAgogo:
                    isPitchGlide = true; startFreq = 520; endFreq = 520; totalDurationMs = 20; stepDurationMs = 2;
                    break;
                case MidiPercussion.MetronomeBell:
                    isPitchGlide = true; startFreq = 880; endFreq = 880; totalDurationMs = 18; stepDurationMs = 2;
                    break;
            }

            maxDurationMs = Math.Max(4, Math.Min(70, maxDurationMs));
            totalDurationMs = Math.Min(totalDurationMs, maxDurationMs);

            double velocityScale = Math.Max(1, Math.Min(127, velocity)) / 127.0;
            totalDurationMs = Math.Max(10, (int)Math.Round(totalDurationMs * (0.75 + (0.25 * velocityScale))));

            int ClampSpeakerFrequency(double value)
            {
                int freq = (int)Math.Round(value);
                if (freq < 37) return 37;
                if (freq > 1600) return 1600; // hard anti-piercing ceiling
                return freq;
            }

            ushort lfsr = (ushort)Random.Shared.Next(1, ushort.MaxValue);
            int elapsed = 0;
            // FIX: attack used to be a flat 3ms regardless of note length, so on a 30-40ms
            // snare/cymbal/hat hit almost the whole audible duration was the (narrow) decay
            // band below. Scale it with the total duration instead so the broadband "crack"
            // actually has time to be heard.
            int attackMs = Math.Max(2, Math.Min(totalDurationMs - 1, (int)Math.Round(totalDurationMs * 0.35)));
            int decayMs = Math.Max(1, totalDurationMs - attackMs);

            int range = Math.Max(1, maxFreq - minFreq);
            int attackCeiling = minFreq + (int)(range * 0.65); // wide, harsh band for the initial transient
            int attackRangeSize = Math.Max(1, attackCeiling - minFreq);

            // FIX: the decay band used to be only ~10% of the full frequency range
            // (decayCenter ± 5%), which made the tail sound like a near-constant single
            // pitch — i.e. a quiet "beep" — instead of a fading noise burst. Widen it to
            // ~50% of the range so it keeps a noisy, percussive character while still
            // trending lower than the attack for a natural decay feel.
            int decayCenter = minFreq + (int)(range * 0.30);
            int decayJitter = Math.Max(2, (int)(range * 0.25));
            int decayBandMin = Math.Max(minFreq, decayCenter - decayJitter);
            int decayBandMax = Math.Min(maxFreq, decayCenter + decayJitter);
            int decayRangeSize = Math.Max(1, decayBandMax - decayBandMin);

            // FIX: this is the piece the earlier pass was missing. BaWaMI's actual noise-based
            // percussion (see reference source: `If Rnd < NoiseVol Then PCSpkDirectOn Else
            // PCSpkDirectOff`) doesn't just hop pitch — for every tiny sample it flips a weighted
            // coin and either sounds the speaker or leaves it truly silent, with the "on"
            // probability (NoiseVol) tracking the note's live volume envelope so the crackle
            // visibly thins out as the note decays. That random on/off gating, not the pitch
            // hopping by itself, is what makes it read as *noise* with a real decay — without it,
            // a speaker that's always "on" just sounds like one continuous (if wobbly) tone,
            // i.e. a beep, no matter how much the pitch jumps around.
            // We have no live envelope to sample here, so the decay phase fades a gate
            // probability from DecayGateStart down to DecayGateFloor over the tail instead. The
            // attack phase is intentionally left ungated (always on) so the initial crack stays
            // solid and unambiguously audible.
            const double DecayGateStart = 0.90;
            const double DecayGateFloor = 0.22;

            // FIX: StartPulse/StopPulse are non-blocking -- unlike the old
            // NotePlayer.PlayNoteWithoutGap(freq, ms), which returned on its own after
            // playing for exactly `ms`, a started pulse just keeps going until something
            // explicitly stops it. That means every exit from this method -- normal
            // completion, cancellation, or an exception -- has to guarantee StopPulse
            // runs, or the tone/noise is left playing indefinitely.
            try
            {
                while (elapsed < totalDurationMs)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    double currentFreq;
                    int currentStepMs;
                    bool playGrain = true;

                    if (isPitchGlide)
                    {
                        double t = totalDurationMs <= 1 ? 1.0 : (double)elapsed / totalDurationMs;
                        // Slight curve makes drums hit then fall quickly, closer to a percussive thump.
                        double curved = 1.0 - Math.Pow(1.0 - t, 2.0);
                        currentFreq = startFreq + ((endFreq - startFreq) * curved);
                        currentStepMs = stepDurationMs;
                    }
                    else
                    {
                        int feedbackBit = ((lfsr >> 0) ^ (lfsr >> 2) ^ (lfsr >> 3) ^ (lfsr >> 5)) & 1;
                        lfsr = (ushort)((lfsr >> 1) | (feedbackBit << 15));

                        if (elapsed < attackMs)
                        {
                            currentFreq = minFreq + (lfsr % attackRangeSize);
                            currentStepMs = grainDurationMs;
                        }
                        else
                        {
                            double decayProgress = (double)(elapsed - attackMs) / decayMs;
                            // Keep grains tiny.  Long 4-6 ms grains sound like separate chirps,
                            // so the noisy/percussive texture becomes hard to notice.
                            currentStepMs = grainDurationMs + (decayProgress > 0.70 ? 1 : 0); // mostly 1 ms, 2 ms tail
                            currentFreq = decayBandMin + (lfsr % decayRangeSize);

                            double gateProbability = DecayGateStart + ((DecayGateFloor - DecayGateStart) * decayProgress);
                            playGrain = Random.Shared.NextDouble() < gateProbability;
                        }
                    }

                    currentStepMs = Math.Min(currentStepMs, totalDurationMs - elapsed);
                    if (currentStepMs <= 0)
                        break;

                    // NOTE: this assumes StartPulse can be called again, with a new frequency,
                    // while a pulse is already playing, and that it updates the pitch in place
                    // rather than audibly restarting -- i.e. it behaves like a register write on
                    // the SystemSpeaker path. Calling it every 1-2ms is fine for a raw port
                    // toggle; if WaveSynthEngine's SoundDevice path tears down/recreates its
                    // NAudio output on every call instead of just updating an already-running
                    // oscillator, calling it this fast will click/glitch and the grain size below
                    // (grainDurationMs) would need to be coarsened specifically for that path.
                    if (playGrain)
                        StartPulse(outputChoice, ClampSpeakerFrequency(currentFreq));
                    else
                        StopPulse(outputChoice); // true silence — this is what makes the decay actually decay

                    Thread.Sleep(currentStepMs);
                    elapsed += currentStepMs;
                }
            }
            finally
            {
                StopPulse(outputChoice); // always stop, however this method exits
            }
        }
    }
}