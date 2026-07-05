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

using System.Diagnostics;
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
            Laser = 27, Whip = 28, ScratchPush = 29, ScratchPull = 30, StickClick = 31,
            MetronomeClick = 33, MetronomeBell = 34, BassDrum = 35, KickDrum = 36,
            SideStick = 37, SnareCrossStick = 37, SnareDrum = 38, HandClap = 39,
            ElectricSnareDrum = 40, FloorTom2 = 41, HiHatClosed = 42, FloorTom1 = 43,
            HiHatFoot = 44, LowTom = 45, HiHatOpen = 46, LowMidTom = 47, HighMidTom = 48,
            CrashCymbal = 49, HighTom = 50, RideCymbal = 51, ChinaCymbal = 52,
            RideBell = 53, Tambourine = 54, SplashCymbal = 55, Cowbell = 56,
            CrashCymbal2 = 57, Vibraslap = 58, RideCymbal2 = 59, HighBongo = 60,
            LowBongo = 61, CongaDeadStroke = 62, Conga = 63, Tumba = 64, HighTimbale = 65,
            LowTimbale = 66, HighAgogo = 67, LowAgogo = 68, Cabasa = 69, Maracas = 70,
            WhistleShort = 71, WhistleLong = 72, GuiroShort = 73, GuiroLong = 74,
            Claves = 75, HighWoodblock = 76, LowWoodblock = 77, CuicaHigh = 78,
            CuicaLow = 79, TriangleMute = 80, TriangleOpen = 81, Shaker = 82,
            SleighBell = 83, BellTree = 84, Castanets = 85, SurduDeadStroke = 86,
            Surdu = 87, Güiro = 73, Clave = 75, WoodBlock = 76, SnareDrumRod = 91,
            OceanDrum = 92, SnareDrumBrush = 93
        }

        public enum PercussionOutputChoice
        {
            SystemSpeaker,
            SoundDevice
        }

        private static int ClampPercussionFrequency(double frequency)
        {
            // The previous noise code jumped up to 13 kHz. That often sounds like alien/foreign beeps,
            // and many beep/synth backends do not handle those values cleanly.
            return (int)Math.Round(Math.Clamp(frequency, 37.0, 4000.0));
        }

        private static void StartPulse(PercussionOutputChoice outputChoice, int frequency, bool forceWhiteNoise = false)
        {
            frequency = ClampPercussionFrequency(frequency);

            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    // A system speaker cannot produce real white noise. We therefore use short gated clicks
                    // elsewhere instead of holding a random high-pitched beep.
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                    break;

                case PercussionOutputChoice.SoundDevice:
                    if (forceWhiteNoise)
                    {
                        // Noise-like percussion must be noise even when the normal beep waveform is sine/square/triangle.
                        SoundRenderingEngine.WaveSynthEngine.StartSynth(
                            NAudio.Wave.SampleProviders.SignalGeneratorType.White,
                            frequency);
                        break;
                    }

                    switch (TemporarySettings.CreatingSounds.soundDeviceBeepWaveform)
                    {
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Square:
                            SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.Square, frequency);
                            break;
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Sine:
                            SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.Sin, frequency);
                            break;
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Triangle:
                            SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.Triangle, frequency);
                            break;
                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Noise:
                            SoundRenderingEngine.WaveSynthEngine.StartSynth(NAudio.Wave.SampleProviders.SignalGeneratorType.White, frequency);
                            break;
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

        private static void PreciseWaitMs(double ms, CancellationToken cancellationToken)
        {
            if (ms <= 0) return;

            var sw = Stopwatch.StartNew();
            double targetTicks = ms * Stopwatch.Frequency / 1000.0;

            while (sw.ElapsedTicks < targetTicks)
            {
                if (cancellationToken.IsCancellationRequested) return;
            }
        }

        private readonly struct PercussionProfile
        {
            public readonly bool HasTonalBody;
            public readonly double StartFreq;
            public readonly double EndFreq;
            public readonly int BodyDurationMs;

            public readonly bool HasNoiseBody;
            public readonly int NoiseDurationMs;
            public readonly int NoiseBaseFreq;
            public readonly int AccentHits;

            public PercussionProfile(
                bool hasTonalBody,
                double startFreq,
                double endFreq,
                int bodyDurationMs,
                bool hasNoiseBody = false,
                int noiseDurationMs = 0,
                int noiseBaseFreq = 1800,
                int accentHits = 1)
            {
                HasTonalBody = hasTonalBody;
                StartFreq = startFreq;
                EndFreq = endFreq;
                BodyDurationMs = bodyDurationMs;
                HasNoiseBody = hasNoiseBody;
                NoiseDurationMs = noiseDurationMs;
                NoiseBaseFreq = noiseBaseFreq;
                AccentHits = Math.Max(1, accentHits);
            }
        }

        private static PercussionProfile GetProfile(MidiPercussion percussion) => percussion switch
        {
            // Kicks: clear low transient. Longer than before so the hit is noticeable.
            MidiPercussion.KickDrum or MidiPercussion.BassDrum
                => new PercussionProfile(true, 220, 52, 72),

            // Snares: tonal crack + forced white-noise tail. This avoids sine/square random beeps.
            MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareCrossStick
                or MidiPercussion.SideStick or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush
                => new PercussionProfile(true, 330, 185, 24, true, 86, 2100, 2),

            // Clap: several close noise accents, like hands instead of one faint hiss.
            MidiPercussion.HandClap
                => new PercussionProfile(false, 0, 0, 0, true, 105, 1900, 3),

            // Toms: resonant tonal sweeps.
            MidiPercussion.HighTom or MidiPercussion.HighMidTom
                => new PercussionProfile(true, 330, 125, 72),
            MidiPercussion.LowTom or MidiPercussion.LowMidTom
                => new PercussionProfile(true, 235, 88, 82),
            MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2
                => new PercussionProfile(true, 165, 62, 94),

            // Hats/shakers: short gated noise, not high-frequency random tones.
            MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot
                => new PercussionProfile(false, 0, 0, 0, true, 46, 3000, 1),
            MidiPercussion.Cabasa or MidiPercussion.Maracas or MidiPercussion.Shaker or MidiPercussion.Castanets
                => new PercussionProfile(false, 0, 0, 0, true, 58, 2600, 2),

            // Cymbals: sustained gated white noise. Still capped by maxDurationMs in PlayPercussion.
            MidiPercussion.HiHatOpen
                => new PercussionProfile(false, 0, 0, 0, true, 180, 3100, 1),
            MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal
                => new PercussionProfile(false, 0, 0, 0, true, 240, 3300, 2),
            MidiPercussion.SplashCymbal
                => new PercussionProfile(false, 0, 0, 0, true, 155, 3400, 2),
            MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2
                => new PercussionProfile(false, 0, 0, 0, true, 135, 2900, 1),

            // Wood/blocks/clicks: short, strong, recognisable attacks.
            MidiPercussion.StickClick or MidiPercussion.MetronomeClick or MidiPercussion.Claves
                or MidiPercussion.HighWoodblock or MidiPercussion.Clave or MidiPercussion.WoodBlock
                => new PercussionProfile(true, 1450, 950, 26),
            MidiPercussion.LowWoodblock
                => new PercussionProfile(true, 950, 620, 34),
            MidiPercussion.Cowbell
                => new PercussionProfile(true, 880, 880, 48),

            // Metals/bells: tonal, not noise.
            MidiPercussion.RideBell or MidiPercussion.MetronomeBell
                => new PercussionProfile(true, 2400, 2350, 95),
            MidiPercussion.TriangleOpen or MidiPercussion.TriangleMute
                => new PercussionProfile(true, 3200, 3150, percussion == MidiPercussion.TriangleOpen ? 135 : 62),
            MidiPercussion.Tambourine or MidiPercussion.SleighBell
                => new PercussionProfile(true, 2100, 2050, 46, true, 55, 3200, 2),

            // Latin percussion.
            MidiPercussion.HighBongo
                => new PercussionProfile(true, 520, 250, 52),
            MidiPercussion.LowBongo
                => new PercussionProfile(true, 410, 205, 58),
            MidiPercussion.CongaDeadStroke or MidiPercussion.Conga
                => new PercussionProfile(true, 310, 165, 72),
            MidiPercussion.Tumba
                => new PercussionProfile(true, 260, 128, 82),
            MidiPercussion.HighTimbale
                => new PercussionProfile(true, 620, 330, 62, true, 30, 2500, 1),
            MidiPercussion.LowTimbale
                => new PercussionProfile(true, 465, 240, 68, true, 32, 2300, 1),
            MidiPercussion.HighAgogo
                => new PercussionProfile(true, 1600, 1600, 55),
            MidiPercussion.LowAgogo
                => new PercussionProfile(true, 1120, 1120, 62),

            // SFX kept short to avoid musical/foreign beeps.
            MidiPercussion.Laser
                => new PercussionProfile(true, 1650, 270, 80),
            MidiPercussion.Whip
                => new PercussionProfile(false, 0, 0, 0, true, 48, 2600, 3),
            MidiPercussion.ScratchPush or MidiPercussion.ScratchPull
                => new PercussionProfile(false, 0, 0, 0, true, 62, 1700, 3),

            _ => new PercussionProfile(true, 760, 480, 35, true, 22, 2200, 1)
        };

        public static void PlayPercussion(
            MidiPercussion percussion,
            CancellationToken cancellationToken = default,
            int maxDurationMs = 500,
            int velocity = 100)
        {
            PercussionOutputChoice outputChoice = TemporarySettings.CreatingSounds.createBeepWithSoundDevice == true
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;

            var profile = GetProfile(percussion);

            double velocityScale = Math.Clamp(velocity, 1, 127) / 127.0;
            int safeMaxDurationMs = Math.Max(8, maxDurationMs);

            // With this engine there is no true volume envelope, so make quiet hits shorter but never inaudibly tiny.
            int bodyMs = Math.Min(
                safeMaxDurationMs,
                Math.Max(profile.HasTonalBody ? 12 : 0, (int)Math.Round(profile.BodyDurationMs * (0.72 + 0.38 * velocityScale))));

            int noiseMs = Math.Min(
                safeMaxDurationMs,
                Math.Max(profile.HasNoiseBody ? 18 : 0, (int)Math.Round(profile.NoiseDurationMs * (0.72 + 0.38 * velocityScale))));

            double pitchBoost = 0.96 + 0.10 * velocityScale;

            try
            {
                if (profile.HasTonalBody && bodyMs > 0)
                {
                    PlayPitchGlide(
                        outputChoice,
                        profile.StartFreq * pitchBoost,
                        profile.EndFreq,
                        bodyMs,
                        cancellationToken);

                    StopPulse(outputChoice);
                }

                if (profile.HasNoiseBody && noiseMs > 0 && !cancellationToken.IsCancellationRequested)
                {
                    PlayNoiseBurst(
                        outputChoice,
                        noiseMs,
                        profile.NoiseBaseFreq,
                        profile.AccentHits,
                        velocityScale,
                        cancellationToken);
                }
            }
            finally
            {
                StopPulse(outputChoice);
            }
        }

        private static void PlayNoiseBurst(
    PercussionOutputChoice outputChoice,
    int totalDurationMs,
    int baseFrequency,
    int accentHits,
    double velocityScale,
    CancellationToken cancellationToken)
        {
            if (totalDurationMs <= 0) return;

            var rnd = Random.Shared;
            long startTicks = Stopwatch.GetTimestamp();
            long durationTicks = (long)(totalDurationMs * Stopwatch.Frequency / 1000.0);

            // Initial accents (claps, crashes, shakers) still read as distinct hits, so keep those.
            int accentOnMs = Math.Max(2, (int)Math.Round(2.0 + 3.0 * velocityScale));
            int accentGapMs = 4;

            for (int i = 0; i < accentHits; i++)
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (Stopwatch.GetTimestamp() - startTicks >= durationTicks) return;

                int accentFreq = ClampPercussionFrequency(baseFrequency + rnd.Next(-90, 91));
                StartPulse(outputChoice, accentFreq, forceWhiteNoise: outputChoice == PercussionOutputChoice.SoundDevice);
                PreciseWaitMs(accentOnMs, cancellationToken);
                StopPulse(outputChoice);
                PreciseWaitMs(accentGapMs + i, cancellationToken);
            }

            if (outputChoice == PercussionOutputChoice.SoundDevice)
            {
                // The sound device can render true continuous white noise, so just hold it and
                // let it decay naturally instead of gating it on/off (that gating was a
                // system-speaker-only workaround and only made the sound-device path choppier too).
                long remainStart = Stopwatch.GetTimestamp();
                long remainTicks = durationTicks - (remainStart - startTicks);
                if (remainTicks > 0)
                {
                    StartPulse(outputChoice, baseFrequency, forceWhiteNoise: true);
                    PreciseWaitMs(remainTicks * 1000.0 / Stopwatch.Frequency, cancellationToken);
                    StopPulse(outputChoice);
                }
                return;
            }

            // --- System speaker path ---
            // Sustain phase: keep the tone continuously ON and only retune the frequency to fake a
            // noise texture. No StopPulse between steps -> no audible gaps -> reads as one sound.
            // Decay phase (final ~40% of the hit): switch to real gated clicks, spaced further apart
            // over time, which is what an actual decaying noise burst sounds like.
            double decayStartMs = totalDurationMs * 0.6;
            bool pulseActive = false;

            while (true)
            {
                long now = Stopwatch.GetTimestamp();
                if (cancellationToken.IsCancellationRequested || now - startTicks >= durationTicks) break;

                double elapsedMs = (now - startTicks) * 1000.0 / Stopwatch.Frequency;
                double t = Math.Clamp(elapsedMs / totalDurationMs, 0.0, 1.0);
                int jitter = rnd.Next(-180, 181);
                int freq = ClampPercussionFrequency(baseFrequency * (1.0 - 0.25 * t) + jitter);

                if (elapsedMs < decayStartMs)
                {
                    // Continuous sustain: retune without stopping.
                    StartPulse(outputChoice, freq);
                    pulseActive = true;
                    PreciseWaitMs(2.0 + 2.0 * rnd.NextDouble(), cancellationToken);
                }
                else
                {
                    double decayT = (elapsedMs - decayStartMs) / Math.Max(1.0, totalDurationMs - decayStartMs);
                    double onMs = Math.Max(1.0, (3.5 - 2.2 * decayT) * (0.75 + 0.35 * velocityScale));
                    double offMs = 1.5 + 8.0 * decayT;

                    StartPulse(outputChoice, freq);
                    pulseActive = true;
                    PreciseWaitMs(onMs, cancellationToken);
                    StopPulse(outputChoice);
                    pulseActive = false;
                    PreciseWaitMs(offMs, cancellationToken);
                }
            }

            if (pulseActive) StopPulse(outputChoice);
        }

        private static void PlayPitchGlide(
            PercussionOutputChoice outputChoice,
            double startFreq,
            double endFreq,
            int totalDurationMs,
            CancellationToken cancellationToken)
        {
            if (totalDurationMs <= 0) return;

            startFreq = ClampPercussionFrequency(startFreq);
            endFreq = ClampPercussionFrequency(endFreq);

            if (Math.Abs(endFreq - startFreq) < 0.01)
            {
                StartPulse(outputChoice, ClampPercussionFrequency(startFreq));
                PreciseWaitMs(totalDurationMs, cancellationToken);
                return;
            }

            const int stepMs = 2;
            int elapsed = 0;

            double logStart = Math.Log(startFreq);
            double logEnd = Math.Log(endFreq);

            while (elapsed < totalDurationMs)
            {
                if (cancellationToken.IsCancellationRequested) break;

                double t = totalDurationMs <= 1 ? 1.0 : (double)elapsed / totalDurationMs;

                // A slightly curved decay keeps kicks/toms punchy at the start and avoids melodic glides.
                double curvedT = 1.0 - Math.Pow(1.0 - t, 2.35);
                double currentLogFreq = logStart + (logEnd - logStart) * curvedT;
                int freq = ClampPercussionFrequency(Math.Exp(currentLogFreq));

                StartPulse(outputChoice, freq);

                int step = Math.Min(stepMs, totalDurationMs - elapsed);
                PreciseWaitMs(step, cancellationToken);
                elapsed += step;
            }
        }
    }
}