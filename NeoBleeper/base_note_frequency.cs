
namespace NeoBleeper
{
    public class base_note_frequency
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
                "C" => base_note_frequency.base_note_frequency_in_4th_octave.C,
                "C#" => base_note_frequency.base_note_frequency_in_4th_octave.CS,
                "D" => base_note_frequency.base_note_frequency_in_4th_octave.D,
                "D#" => base_note_frequency.base_note_frequency_in_4th_octave.DS,
                "E" => base_note_frequency.base_note_frequency_in_4th_octave.E,
                "F" => base_note_frequency.base_note_frequency_in_4th_octave.F,
                "F#" => base_note_frequency.base_note_frequency_in_4th_octave.FS,
                "G" => base_note_frequency.base_note_frequency_in_4th_octave.G,
                "G#" => base_note_frequency.base_note_frequency_in_4th_octave.GS,
                "A" => base_note_frequency.base_note_frequency_in_4th_octave.A,
                "A#" => base_note_frequency.base_note_frequency_in_4th_octave.AS,
                "B" => base_note_frequency.base_note_frequency_in_4th_octave.B,
                _ => 0 // Invalid note
            };

            if (baseFrequency == 0)
                return 0;

            // Oktav farkını hesaplama
            int octaveDifference = octave - 4; // 4th octave is the reference octave
            return baseFrequency * Math.Pow(2, octaveDifference);
        }
    }
}
