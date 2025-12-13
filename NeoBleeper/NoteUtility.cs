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

namespace NeoBleeper
{
    public class NoteUtility
    {
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
                string note = noteName.Substring(0, noteName.Length - 1); // "C", "D#", vb.
                int octave = int.Parse(noteName.Substring(noteName.Length - 1)); // Octave number
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
                case "Fer":
                    baseLength *= 2;
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
            if (articulation == "Fer")
            {
                baseLength *= 2;
            }
            return baseLength;
        }
    }
}
