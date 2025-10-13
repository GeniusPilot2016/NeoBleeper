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
        public static class base_note_frequency_in_4th_octave
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
        public static double GetFrequencyFromNoteName(string noteName)
        {
            if (string.IsNullOrEmpty(noteName))
                return 0;

            // Disassemble note name into note and octave
            string note = noteName.Substring(0, noteName.Length - 1); // "C", "D#", vb.
            int octave = int.Parse(noteName.Substring(noteName.Length - 1)); // Octave number
            // Basic frequency for the note in the 4th octave
            double baseFrequency = note switch
            {
                "C" => NoteUtility.base_note_frequency_in_4th_octave.C,
                "C#" => NoteUtility.base_note_frequency_in_4th_octave.CS,
                "D" => NoteUtility.base_note_frequency_in_4th_octave.D,
                "D#" => NoteUtility.base_note_frequency_in_4th_octave.DS,
                "E" => NoteUtility.base_note_frequency_in_4th_octave.E,
                "F" => NoteUtility.base_note_frequency_in_4th_octave.F,
                "F#" => NoteUtility.base_note_frequency_in_4th_octave.FS,
                "G" => NoteUtility.base_note_frequency_in_4th_octave.G,
                "G#" => NoteUtility.base_note_frequency_in_4th_octave.GS,
                "A" => NoteUtility.base_note_frequency_in_4th_octave.A,
                "A#" => NoteUtility.base_note_frequency_in_4th_octave.AS,
                "B" => NoteUtility.base_note_frequency_in_4th_octave.B,
                _ => 0 // Invalid note
            };

            if (baseFrequency == 0)
                return 0;

            // Calculate the frequency based on the octave
            int octaveDifference = octave - 4; // 4th octave is the reference octave
            return baseFrequency * Math.Pow(2, octaveDifference);
        }
    }
    public static class NoteLengths
    {
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
