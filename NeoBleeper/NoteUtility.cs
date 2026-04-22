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

using System.Text.RegularExpressions;

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
        /// <remarks>The method supports natural notes (A–G) and sharps (e.g., "C#4"). The reference
        /// octave is the 4th octave (e.g., "A4" for 440 Hz). Flat notes are not supported; use the equivalent sharp
        /// notation (e.g., "D#" instead of "Eb").</remarks>
        /// <param name="noteName">The note name and octave in standard format (e.g., "A4", "C#3"). The note must consist of a letter (A–G), an
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
        /// Calculates the duration of a musical note in milliseconds based on tempo, note type, and optional modifiers.
        /// </summary>
        /// <remarks>If an unsupported note type, modifier, or articulation is provided, the method
        /// defaults to the base note length without the corresponding adjustment. This method does not validate the
        /// musical correctness of combined modifiers and articulations.</remarks>
        /// <param name="bpm">The tempo in beats per minute. Must be greater than zero.</param>
        /// <param name="noteType">The type of note to calculate the length for. Supported values include "Whole", "Half", "Quarter", "1/8",
        /// "1/16", and "1/32".</param>
        /// <param name="modifier">An optional modifier that alters the note length. Supported values are "Dot" (dotted note) and "Tri"
        /// (triplet). If not specified or unrecognized, no modifier is applied.</param>
        /// <param name="articulation">An optional articulation that further adjusts the note length. Supported values are "Sta" (staccato), "Spi"
        /// (spiccato), and "Fer" (fermata). If not specified or unrecognized, no articulation is applied.</param>
        /// <returns>The length of the note in milliseconds, adjusted for the specified tempo, note type, modifier, and
        /// articulation.</returns>
        public static double CalculateNoteLength(int bpm, string noteType, string modifier = "", string articulation = "")
        {
            int millisecondsPerBeat = (int)Math.Floor(60000.0 / bpm);
            int baseLength = noteType switch
            {
                "Whole" => millisecondsPerBeat * 4,
                "Half" => millisecondsPerBeat * 2,
                "Quarter" => millisecondsPerBeat,
                "1/8" => millisecondsPerBeat / 2,
                "1/16" => millisecondsPerBeat / 4,
                "1/32" => millisecondsPerBeat / 8,
                _ => millisecondsPerBeat
            };
            switch (modifier)
            {
                case "Dot":
                    baseLength = (int)(baseLength * 1.5);
                    break;
                case "Tri":
                    baseLength /= 3;
                    break;
                default:
                    break;
            }
            switch (articulation)
            {
                case "Sta":
                    baseLength /= 2;
                    break;
                case "Spi":
                    baseLength /= 4;
                    break;
                default:
                    break;
            }
            return baseLength;
        }

        /// <summary>
        /// Calculates the duration, in milliseconds, of a musical note based on tempo, note type, and optional
        /// modifiers.
        /// </summary>
        /// <remarks>If an unsupported note type, modifier, or articulation is provided, the method
        /// defaults to standard quarter note length and ignores unrecognized modifiers or articulations. This method
        /// does not validate input strings beyond the supported values.</remarks>
        /// <param name="bpm">The tempo in beats per minute. Must be greater than zero.</param>
        /// <param name="noteType">The type of note to calculate the length for. Supported values include "Whole", "Half", "Quarter", "1/8",
        /// "1/16", and "1/32".</param>
        /// <param name="modifier">An optional modifier that alters the note length. Supported values are "Dot" (for dotted notes) and "Tri"
        /// (for triplet notes). If not specified, no modifier is applied.</param>
        /// <param name="articulation">An optional articulation that further modifies the note length. Use "Fer" to apply a fermata (doubling the
        /// duration). If not specified, no articulation is applied.</param>
        /// <returns>The calculated length of the note, in milliseconds.</returns>
        public static double CalculateLineLength(int bpm, string noteType, string modifier = "", string articulation = "")
        {
            if (bpm == 0) bpm = 1;
            int millisecondsPerBeat = (int)Math.Floor(60000.0 / bpm);
            int baseLength = noteType switch
            {
                "Whole" => millisecondsPerBeat * 4,
                "Half" => millisecondsPerBeat * 2,
                "Quarter" => millisecondsPerBeat,
                "1/8" => millisecondsPerBeat / 2,
                "1/16" => millisecondsPerBeat / 4,
                "1/32" => millisecondsPerBeat / 8,
                _ => millisecondsPerBeat
            };
            switch (modifier)
            {
                case "Dot":
                    baseLength = (int)(baseLength * 1.5);
                    break;
                case "Tri":
                    baseLength /= 3;
                    break;
            }
            return baseLength;
        }
        public static (int totalRhythm_int, int noteSound_int) CalculateNoteDurations(string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio)
        {
            if(bpm == 0) 
                bpm = 1;

            var (lengthName_checked, modifier_checked, articulation_checked) = UseOriginalValueOrDefault(lengthName, modifier, articulation);
            if (string.IsNullOrEmpty(lengthName))
                lengthName = "Quarter";
            // Essential values for note duration calculations
            double noteSound_double = FixRoundingErrors(CalculateNoteLength(bpm, lengthName_checked, modifier_checked, articulation_checked));
            double totalRhythm_double = FixRoundingErrors(CalculateLineLength(bpm, lengthName_checked, modifier_checked, articulation_checked));

            int totalRhythm_int = (int)Math.Truncate(totalRhythm_double);
            int noteSound_int = Math.Min((int)Math.Truncate(noteSound_double), totalRhythm_int);

            if (articulation == "Fer")
            {
                // Random variation for fermata duration (between 50% and 100% of the original note sound duration)
                int extraFermataDuration = (int)(noteSound_double * (0.5 + 0.5 * Random.Shared.NextDouble()));

                totalRhythm_int += extraFermataDuration;
                noteSound_int = Math.Min(noteSound_int + extraFermataDuration, totalRhythm_int);
            }

            noteSound_int = (int)(noteSound_int * noteSilenceRatio);
            return (totalRhythm_int, noteSound_int);
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
            // Define the threshold and adjustment values based on the assembly constants
            const double threshold = 1e-7;
            const double adjustment = 1e-10;

            // Check if the input value exceeds the threshold
            if (inputValue >= 0)
            {
                if (inputValue > threshold)
                {
                    inputValue += adjustment;
                }
            }
            else
            {
                if (inputValue < (threshold * -1))
                {
                    inputValue -= adjustment;
                }
            }
            // Return the corrected value
            return inputValue;
        }

        private static (string returnedLength, string returnedModifier, string returnedArticulation) UseOriginalValueOrDefault(string length, string modifier, string articulation)
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
}
