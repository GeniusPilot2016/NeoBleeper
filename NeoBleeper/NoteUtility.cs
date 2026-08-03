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

using NAudio.SoundFont;
using NAudio.Wave;
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
            Laser = 27, Whip = 28, ScratchPush = 29, ScratchPull = 30, StickClick = 31, SquareClick = 32,
            MetronomeClick = 33, MetronomeBell = 34, BassDrum = 35, KickDrum = 36, SideStick = 37,
            SnareCrossStick = SideStick, SnareDrum = 38, HandClap = 39, ElectricSnareDrum = 40, FloorTom2 = 41,
            HiHatClosed = 42, FloorTom1 = 43, HiHatFoot = 44, LowTom = 45, HiHatOpen = 46, LowMidTom = 47,
            HighMidTom = 48, CrashCymbal = 49, HighTom = 50, RideCymbal = 51, ChinaCymbal = 52, RideBell = 53,
            Tambourine = 54, SplashCymbal = 55, Cowbell = 56, CrashCymbal2 = 57, Vibraslap = 58, RideCymbal2 = 59,
            HighBongo = 60, LowBongo = 61, CongaDeadStroke = 62, Conga = 63, Tumba = 64, HighTimbale = 65,
            LowTimbale = 66, HighAgogo = 67, LowAgogo = 68, Cabasa = 69, Maracas = 70, WhistleShort = 71,
            WhistleLong = 72, GuiroShort = 73, Güiro = GuiroShort, GuiroLong = 74, Claves = 75, Clave = Claves,
            HighWoodblock = 76, WoodBlock = HighWoodblock, LowWoodblock = 77, CuicaHigh = 78, CuicaLow = 79,
            TriangleMute = 80, TriangleOpen = 81, Shaker = 82, SleighBell = 83, BellTree = 84, Castanets = 85,
            SurduDeadStroke = 86, Surdu = 87, SnareDrumRod = 91, OceanDrum = 92, SnareDrumBrush = 93
        }

        public enum PercussionOutputChoice { SystemSpeaker, SoundDevice }
        private enum SynthWave { Square, Triangle, Noise }

        // The motherboard speaker is monophonic. Sound-device percussion uses a separate
        // buffered stream, so it can overlap melody playback. Queue attacks in order and let the currently
        // sounding tail yield only after its attack has been audible. This prevents dropped
        // hits without time-slicing several tails, which changes the percussion character.
        private const double RetriggerGapMs = 0.35;
        private const double QueuePollMs = 1.0;

        private static readonly object _hardwareLock = new object();
        private static readonly object _queueLock = new object();
        private static readonly System.Collections.Generic.Queue<PercussionRequest> _pendingRequests =
            new System.Collections.Generic.Queue<PercussionRequest>();
        private static bool _queueWorkerRunning;

        // Percussion has its own sound-device stream. Sharing WaveSynthEngine with the
        // melody made StartSynth/StopSynth calls replace one another. The separate stream
        // renders the same pulse algorithm as the system speaker and is mixed by Windows.
        private const int PercussionSampleRate = 44100;
        private static readonly object _soundDeviceLock = new object();
        private static NAudio.Wave.WaveOutEvent? _percussionWaveOut;
        private static NAudio.Wave.SampleProviders.MixingSampleProvider? _percussionMixer;

        private sealed class FloatArraySampleProvider : NAudio.Wave.ISampleProvider
        {
            private readonly float[] _samples;
            private int _position;

            public FloatArraySampleProvider(float[] samples)
            {
                _samples = samples ?? System.Array.Empty<float>();
                WaveFormat = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(PercussionSampleRate, 1);
            }

            public NAudio.Wave.WaveFormat WaveFormat { get; }

            public int Read(float[] buffer, int offset, int count)
            {
                int available = _samples.Length - _position;
                int toCopy = System.Math.Min(available, count);
                if (toCopy <= 0)
                    return 0;

                System.Array.Copy(_samples, _position, buffer, offset, toCopy);
                _position += toCopy;
                return toCopy;
            }
        }

        private sealed class PercussionRequest
        {
            public readonly MidiPercussion Percussion;
            public readonly System.Threading.CancellationToken CancellationToken;
            // DurationMs controls the audible tail. CompletionDelayMs controls when the
            // caller may advance to the next rhythmic frame. They must be independent for
            // cymbals and other long-ring percussion.
            public readonly int DurationMs;
            public readonly int CompletionDelayMs;
            public readonly PercussionOutputChoice Output;
            public readonly PercussionProfile Profile;
            public readonly int Velocity;
            public readonly int RandomSeed;
            public readonly System.Threading.Tasks.TaskCompletionSource<bool>? Completion;

            public PercussionRequest(
                MidiPercussion percussion,
                System.Threading.CancellationToken cancellationToken,
                int durationMs,
                int completionDelayMs,
                PercussionOutputChoice output,
                PercussionProfile profile,
                int velocity,
                System.Threading.Tasks.TaskCompletionSource<bool>? completion)
            {
                Percussion = percussion;
                CancellationToken = cancellationToken;
                DurationMs = System.Math.Max(1, durationMs);
                CompletionDelayMs = System.Math.Max(1, completionDelayMs);
                Output = output;
                Profile = profile;
                Velocity = System.Math.Clamp(velocity, 1, 127);
                RandomSeed = System.Random.Shared.Next();
                Completion = completion;
            }
        }

        private static int ClampPercussionFrequency(double frequency) =>
            (int)System.Math.Round(System.Math.Clamp(frequency, 37.0, 15000.0));

        private static void StartPulseDirect(PercussionOutputChoice outputChoice, int frequency, SynthWave waveType)
        {
            frequency = ClampPercussionFrequency(frequency);
            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                    break;
                case PercussionOutputChoice.SoundDevice:
                    var naudioWave = waveType switch
                    {
                        SynthWave.Noise => NAudio.Wave.SampleProviders.SignalGeneratorType.White,
                        SynthWave.Triangle => NAudio.Wave.SampleProviders.SignalGeneratorType.Triangle,
                        _ => NAudio.Wave.SampleProviders.SignalGeneratorType.Square
                    };
                    SoundRenderingEngine.WaveSynthEngine.StartSynth(naudioWave, frequency);
                    break;
            }
        }

        private static void StopPulseDirect(PercussionOutputChoice outputChoice)
        {
            if (outputChoice == PercussionOutputChoice.SystemSpeaker)
                SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
            else
                SoundRenderingEngine.WaveSynthEngine.StopSynth();
        }

        private static void StopCurrentPulse(ref PercussionOutputChoice? currentOutput)
        {
            lock (_hardwareLock)
            {
                if (currentOutput.HasValue)
                    StopPulseDirect(currentOutput.Value);
            }

            currentOutput = null;
        }

        private static void StartOrUpdatePulse(
            PercussionOutputChoice output,
            int frequency,
            SynthWave wave,
            ref PercussionOutputChoice? currentOutput)
        {
            lock (_hardwareLock)
            {
                if (currentOutput.HasValue && currentOutput.Value != output)
                    StopPulseDirect(currentOutput.Value);

                StartPulseDirect(output, frequency, wave);
                currentOutput = output;
            }
        }

        private static void EnsurePercussionSoundDevice()
        {
            lock (_soundDeviceLock)
            {
                if (_percussionWaveOut != null && _percussionMixer != null)
                    return;

                var format = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(PercussionSampleRate, 1);
                _percussionMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(format)
                {
                    ReadFully = true
                };

                _percussionWaveOut = new NAudio.Wave.WaveOutEvent
                {
                    DesiredLatency = 40,
                    NumberOfBuffers = 3
                };
                _percussionWaveOut.Init(_percussionMixer);
                _percussionWaveOut.Play();
            }
        }

        private static void QueueMixedSoundDeviceHit(PercussionRequest request)
        {
            EnsurePercussionSoundDevice();
            float[] samples = RenderPercussionSamples(request);
            var hitProvider = new FloatArraySampleProvider(samples);

            lock (_soundDeviceLock)
            {
                // Each hit is a separate mixer input. Long cymbal tails and rapid drum
                // attacks now overlap instead of being appended behind one another.
                _percussionMixer?.AddMixerInput(hitProvider);
            }
        }

        private static float[] RenderPercussionSamples(PercussionRequest request)
        {
            // White noise is the only excitation source. Realism comes from several filtered
            // noise layers with different envelopes: impact, shell/body, rattle and air/tail.
            int sampleCount = System.Math.Max(1,
                (int)System.Math.Ceiling(request.DurationMs * PercussionSampleRate / 1000.0));
            var result = new float[sampleCount];
            double velocity01 = request.Velocity / 127.0;
            double masterGain = 0.34 + velocity01 * 0.42;

            uint rng = unchecked((uint)request.RandomSeed);
            double lpLow = 0.0, lpMid = 0.0, lpHigh = 0.0;
            double previousMid = 0.0;
            double dcBlockX = 0.0, dcBlockY = 0.0;

            bool kick = request.Percussion is MidiPercussion.KickDrum or MidiPercussion.BassDrum;
            bool tom = request.Percussion is MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2
                or MidiPercussion.LowTom or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom
                or MidiPercussion.HighTom or MidiPercussion.HighBongo or MidiPercussion.LowBongo
                or MidiPercussion.Conga or MidiPercussion.CongaDeadStroke or MidiPercussion.Tumba
                or MidiPercussion.HighTimbale or MidiPercussion.LowTimbale or MidiPercussion.Surdu
                or MidiPercussion.SurduDeadStroke;
            bool snare = request.Percussion is MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum
                or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush;
            bool cymbal = IsMetalCymbal(request.Percussion);
            bool clap = request.Percussion == MidiPercussion.HandClap;
            bool shaker = request.Percussion is MidiPercussion.Shaker or MidiPercussion.Maracas
                or MidiPercussion.Cabasa or MidiPercussion.Tambourine or MidiPercussion.SleighBell;

            for (int i = 0; i < sampleCount; i++)
            {
                double elapsedMs = i * 1000.0 / PercussionSampleRate;
                double progress = System.Math.Clamp(elapsedMs / request.DurationMs, 0.0, 1.0);

                rng ^= rng << 13;
                rng ^= rng >> 17;
                rng ^= rng << 5;
                double white = ((rng & 0x00FFFFFF) / 8388607.5) - 1.0;

                // Three independently useful noise bands, all derived from the same white-noise source.
                double lowCut = kick ? 145.0 : tom ? 320.0 : 700.0;
                double midCut = snare ? 2600.0 : cymbal ? 4300.0 : 1900.0;
                double highCut = cymbal ? 7600.0 : shaker ? 9000.0 : 6500.0;

                // Drums darken quickly; metal darkens slowly and irregularly.
                if (kick || tom) lowCut *= 1.0 - 0.48 * progress;
                if (snare) midCut *= 1.0 - 0.28 * progress;
                if (cymbal) highCut *= 1.0 - 0.38 * progress;

                double aLow = 1.0 - System.Math.Exp(-2.0 * System.Math.PI * lowCut / PercussionSampleRate);
                double aMid = 1.0 - System.Math.Exp(-2.0 * System.Math.PI * midCut / PercussionSampleRate);
                double aHigh = 1.0 - System.Math.Exp(-2.0 * System.Math.PI * highCut / PercussionSampleRate);
                lpLow += aLow * (white - lpLow);
                previousMid = lpMid;
                lpMid += aMid * (white - lpMid);
                lpHigh += aHigh * (white - lpHigh);

                double lowBand = lpLow;
                double midBand = lpMid - lpLow;
                double highBand = white - lpHigh;
                double edgeBand = lpMid - previousMid;

                double attack = System.Math.Exp(-elapsedMs / (kick ? 2.2 : cymbal ? 4.5 : 1.5));
                double body = System.Math.Exp(-elapsedMs / System.Math.Max(12.0, request.DurationMs *
                    (kick ? 0.18 : tom ? 0.30 : snare ? 0.22 : 0.42)));
                double tail = System.Math.Pow(System.Math.Max(0.0, 1.0 - progress),
                    cymbal ? 1.15 : shaker ? 1.7 : request.Profile.DecayShape);

                double source;
                if (kick)
                {
                    // A compact pressure transient followed by a low, non-tonal shell thump.
                    source = lowBand * (2.6 * body) + midBand * (0.75 * attack);
                }
                else if (tom)
                {
                    source = lowBand * (1.55 * body) + midBand * (1.10 * body) + edgeBand * (4.0 * attack);
                }
                else if (snare)
                {
                    double wireRattle = 0.72 + 0.28 * System.Math.Sin(elapsedMs * 0.37 + (request.RandomSeed & 31));
                    source = midBand * (1.45 * body) + highBand * (0.95 * tail * wireRattle)
                        + edgeBand * (5.0 * attack);
                }
                else if (clap)
                {
                    double t = elapsedMs;
                    double bursts = System.Math.Exp(-System.Math.Pow((t - 1.5) / 4.0, 2.0))
                        + 0.86 * System.Math.Exp(-System.Math.Pow((t - 24.0) / 5.5, 2.0))
                        + 0.70 * System.Math.Exp(-System.Math.Pow((t - 48.0) / 7.0, 2.0))
                        + 0.36 * System.Math.Exp(-System.Math.Max(0.0, t - 62.0) / 38.0);
                    source = (midBand * 1.4 + highBand * 0.75) * bursts;
                }
                else if (cymbal)
                {
                    // A cymbal must begin as a dense wash, not as a single hard click. Short
                    // hats keep a continuous noisy sustain for their first few milliseconds,
                    // then break into fine irregular grains as the metal closes and decays.
                    bool shortHat = request.Percussion is MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot;
                    // Closed/foot hats need a clearly audible noisy wash, not a one-sample
                    // impact followed by sparse grains. Hold the dense stage long enough for
                    // the ear to identify metal before the closing decay begins.
                    double washHoldMs = shortHat ? 48.0 : 28.0;
                    double washHold = elapsedMs < washHoldMs
                        ? 1.0
                        : System.Math.Exp(-(elapsedMs - washHoldMs) / (shortHat ? 52.0 : 110.0));

                    int grain = (int)(elapsedMs / (shortHat ? 0.85 : 0.62));
                    uint gh = unchecked((uint)(request.RandomSeed + grain * 2654435761u));
                    gh ^= gh >> 15;
                    gh *= 2246822519u;
                    gh ^= gh >> 13;
                    double randomGrain = (gh & 1023) / 1023.0;

                    // Keep the opening dense. Grain variation becomes stronger only in the
                    // tail, which preserves a recognisable metallic wash on very short hits.
                    double grainDepth = shortHat ? 0.04 + 0.22 * progress : 0.18 + 0.38 * progress;
                    double grainGain = 1.0 - grainDepth + grainDepth * randomGrain;
                    // Use broad mid/high noise only. The differentiator (edgeBand) is
                    // intentionally excluded from short hats because it creates a stick click.
                    double metallicAir = highBand * (shortHat ? 0.92 : 1.08)
                        + midBand * (shortHat ? 1.05 : 0.62);

                    // Crossfade into the wash over several milliseconds. A real closed hat
                    // has a fast attack, but not an instantaneous full-scale digital edge.
                    double metalFadeIn = shortHat
                        ? System.Math.Min(1.0, elapsedMs / 6.5)
                        : System.Math.Min(1.0, elapsedMs / 2.2);
                    // Short hats use no separate impulse layer. Their onset is simply the
                    // metallic wash blooming in, which prevents a stick-like leading click.
                    double broadAttack = shortHat
                        ? 0.0
                        : (highBand * 0.14 + midBand * 0.24) * attack * metalFadeIn;
                    source = (metallicAir * tail * washHold * grainGain + broadAttack) * metalFadeIn;
                }
                else if (shaker)
                {
                    int grain = (int)(elapsedMs / 1.8);
                    uint gh = unchecked((uint)(request.RandomSeed ^ (grain * 2246822519u)));
                    double grainGate = ((gh >> 9) & 255) / 255.0;
                    source = highBand * tail * (grainGate > 0.30 ? 1.2 : 0.18) + edgeBand * attack * 1.8;
                }
                else
                {
                    source = midBand * body + highBand * tail * 0.55 + edgeBand * attack * 2.5;
                }

                // Very short fade-in avoids a digital click without softening the attack.
                double fadeIn = System.Math.Min(1.0, elapsedMs /
                    (IsShortCymbal(request.Percussion) ? 6.5 : cymbal ? 2.4 : 0.35));
                double sample = source * masterGain * fadeIn;

                // Remove DC and use gentle saturation. This keeps stacked hits clear rather than hissy.
                double dcBlocked = sample - dcBlockX + 0.995 * dcBlockY;
                dcBlockX = sample;
                dcBlockY = dcBlocked;
                result[i] = (float)(System.Math.Tanh(dcBlocked * 1.18) * 0.86);
            }

            return result;
        }

        private static void PlayMixedSoundDeviceRequest(PercussionRequest request)
        {
            if (request.CancellationToken.IsCancellationRequested)
            {
                request.Completion?.TrySetCanceled();
                return;
            }

            QueueMixedSoundDeviceHit(request);

            double elapsed = 0.0;
            while (elapsed < request.CompletionDelayMs && !request.CancellationToken.IsCancellationRequested)
            {
                double slice = System.Math.Min(2.0, request.CompletionDelayMs - elapsed);
                PreciseWaitMs(slice, request.CancellationToken);
                elapsed += slice;
            }

            if (request.CancellationToken.IsCancellationRequested)
                request.Completion?.TrySetCanceled();
            else
                request.Completion?.TrySetResult(true);
        }

        private static void PreciseWaitMs(double ms, System.Threading.CancellationToken ct)
        {
            if (ms <= 0 || ct.IsCancellationRequested)
                return;

            long start = System.Diagnostics.Stopwatch.GetTimestamp();
            long targetTicks = start + (long)(ms * System.Diagnostics.Stopwatch.Frequency / 1000.0);
            while (System.Diagnostics.Stopwatch.GetTimestamp() < targetTicks)
            {
                if (ct.IsCancellationRequested)
                    return;
                System.Threading.Thread.SpinWait(4);
            }
        }

        private readonly struct PercussionProfile
        {
            public readonly SynthWave BodyWave;
            public readonly bool DoesSweep;
            public readonly int BodyStartFreq;
            public readonly int BodyEndFreq;
            public readonly int DurationMs;
            public readonly double NoiseDensity;
            public readonly double HoldRatio;
            public readonly int AttackFrequency;
            public readonly double AttackMs;
            public readonly double DecayShape;
            public readonly double PitchJitter;

            public PercussionProfile(
                SynthWave w, bool s, int start, int end, int dur,
                double density = 0.5, double holdRatio = 0.15,
                int attackFrequency = 0, double attackMs = 2.0,
                double decayShape = 1.8, double pitchJitter = 0.04)
            {
                BodyWave = w;
                DoesSweep = s;
                BodyStartFreq = start;
                BodyEndFreq = end;
                DurationMs = dur;
                NoiseDensity = System.Math.Clamp(density, 0.01, 1.0);
                HoldRatio = System.Math.Clamp(holdRatio, 0.01, 0.95);
                AttackFrequency = attackFrequency <= 0 ? start : attackFrequency;
                AttackMs = System.Math.Clamp(attackMs, 0.2, 20.0);
                DecayShape = System.Math.Clamp(decayShape, 0.5, 5.0);
                PitchJitter = System.Math.Clamp(pitchJitter, 0.0, 0.35);
            }
        }

        private static PercussionProfile GetProfile(MidiPercussion percussion, PercussionOutputChoice output)
        {
            SynthWave drumBodyWave = SynthWave.Noise;

            return percussion switch
            {
                MidiPercussion.KickDrum or MidiPercussion.BassDrum =>
                    new PercussionProfile(drumBodyWave, true, 165, 48, 150, holdRatio: 0.05,
                        attackFrequency: 260, attackMs: 3.0, decayShape: 2.8, pitchJitter: 0.015),

                MidiPercussion.HighTom =>
                    new PercussionProfile(drumBodyWave, true, 330, 175, 180, holdRatio: 0.08,
                        attackFrequency: 520, attackMs: 2.5, decayShape: 2.1, pitchJitter: 0.025),
                MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom =>
                    new PercussionProfile(drumBodyWave, true, 250, 115, 220, holdRatio: 0.08,
                        attackFrequency: 420, attackMs: 2.7, decayShape: 2.0, pitchJitter: 0.025),
                MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 =>
                    new PercussionProfile(drumBodyWave, true, 185, 72, 280, holdRatio: 0.10,
                        attackFrequency: 320, attackMs: 3.0, decayShape: 1.9, pitchJitter: 0.02),

                MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick =>
                    new PercussionProfile(SynthWave.Noise, false, 6100, 6100, 34, density: 0.72, holdRatio: 0.02,
                        attackFrequency: 9200, attackMs: 1.0, decayShape: 3.4, pitchJitter: 0.18),

                MidiPercussion.MetronomeBell =>
                    new PercussionProfile(SynthWave.Noise, true, 2150, 1650, 95, holdRatio: 0.03,
                        attackFrequency: 3900, attackMs: 1.2, decayShape: 2.0, pitchJitter: 0.01),
                MidiPercussion.Claves or MidiPercussion.Castanets =>
                    new PercussionProfile(SynthWave.Noise, false, 2550, 2550, 70, holdRatio: 0.03,
                        attackFrequency: 4700, attackMs: 1.1, decayShape: 2.8, pitchJitter: 0.035),

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum =>
                    new PercussionProfile(SynthWave.Noise, false, 3300, 3300, 190, density: 0.94, holdRatio: 0.035,
                        attackFrequency: 4300, attackMs: 2.8, decayShape: 2.25, pitchJitter: 0.24),
                MidiPercussion.SnareDrumRod =>
                    new PercussionProfile(SynthWave.Noise, false, 3000, 3000, 125, density: 0.78, holdRatio: 0.025,
                        attackFrequency: 3900, attackMs: 2.3, decayShape: 2.6, pitchJitter: 0.20),
                MidiPercussion.SnareDrumBrush =>
                    new PercussionProfile(SynthWave.Noise, false, 2400, 2400, 300, density: 0.50, holdRatio: 0.08,
                        attackFrequency: 5200, attackMs: 4.0, decayShape: 1.35, pitchJitter: 0.26),

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal =>
                    new PercussionProfile(SynthWave.Noise, false, 3900, 3900, 1400, density: 0.82, holdRatio: 0.015,
                        attackFrequency: 4700, attackMs: 1.6, decayShape: 1.30, pitchJitter: 0.34),
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                    new PercussionProfile(SynthWave.Noise, false, 3600, 3600, 1200, density: 0.68, holdRatio: 0.018,
                        attackFrequency: 4400, attackMs: 1.4, decayShape: 1.40, pitchJitter: 0.30),
                MidiPercussion.RideBell =>
                    new PercussionProfile(SynthWave.Noise, false, 3300, 3300, 650, density: 0.70, holdRatio: 0.02,
                        attackFrequency: 4200, attackMs: 1.1, decayShape: 1.55, pitchJitter: 0.28),

                MidiPercussion.HiHatClosed =>
                    new PercussionProfile(SynthWave.Noise, false, 4300, 4300, 190, density: 0.995, holdRatio: 0.085,
                        attackFrequency: 5600, attackMs: 0.55, decayShape: 2.25, pitchJitter: 0.20),
                MidiPercussion.HiHatOpen =>
                    new PercussionProfile(SynthWave.Noise, false, 5200, 5200, 620, density: 0.64, holdRatio: 0.018,
                        attackFrequency: 6200, attackMs: 1.6, decayShape: 1.35, pitchJitter: 0.30),
                MidiPercussion.HiHatFoot =>
                    new PercussionProfile(SynthWave.Noise, false, 3500, 3500, 205, density: 0.98, holdRatio: 0.080,
                        attackFrequency: 4800, attackMs: 0.70, decayShape: 2.15, pitchJitter: 0.18),

                MidiPercussion.HandClap =>
                    new PercussionProfile(SynthWave.Noise, false, 2800, 2800, 150, density: 0.88, holdRatio: 0.02,
                        attackFrequency: 6500, attackMs: 1.3, decayShape: 2.1, pitchJitter: 0.28),
                MidiPercussion.Tambourine or MidiPercussion.Shaker or MidiPercussion.Maracas =>
                    new PercussionProfile(SynthWave.Noise, false, 6200, 6200, 240, density: 0.58, holdRatio: 0.025,
                        attackFrequency: 9800, attackMs: 1.2, decayShape: 1.7, pitchJitter: 0.30),
                MidiPercussion.Cowbell =>
                    new PercussionProfile(SynthWave.Noise, true, 980, 760, 420, density: 0.2, holdRatio: 0.04,
                        attackFrequency: 1900, attackMs: 1.8, decayShape: 1.5, pitchJitter: 0.015),

                _ => new PercussionProfile(SynthWave.Noise, false, 2700, 2700, 130, density: 0.68, holdRatio: 0.04,
                    attackFrequency: 6200, attackMs: 1.8, decayShape: 2.0, pitchJitter: 0.18)
            };
        }

        private static bool IsCymbalOrLongRing(MidiPercussion p)
        {
            return p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2
                or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2
                or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal
                or MidiPercussion.RideBell or MidiPercussion.HiHatOpen;
        }

        private static bool IsShortCymbal(MidiPercussion p)
        {
            return p is MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot;
        }

        private static int GetMinimumShortCymbalTailMs(MidiPercussion p)
        {
            return p == MidiPercussion.HiHatFoot ? 115 : 105;
        }

        public static void PlayPercussion(MidiPercussion p, System.Threading.CancellationToken ct = default, int maxMs = 5000, int velocity = 100)
        {
            if (ct.IsCancellationRequested)
                return;

            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;
            var prof = GetProfile(p, output);

            int duration = IsCymbalOrLongRing(p)
                ? prof.DurationMs
                : IsShortCymbal(p)
                    ? System.Math.Max(GetMinimumShortCymbalTailMs(p), System.Math.Min(maxMs, prof.DurationMs))
                    : System.Math.Max(8, System.Math.Min(maxMs, prof.DurationMs));

            // Fire-and-forget playback must release the queue immediately after the hit is
            // submitted. Its rendered tail continues independently in the sound-device mixer.
            // The system-speaker path remains monophonic and protects its short attack.
            int queueReleaseMs = output == PercussionOutputChoice.SoundDevice
                ? 1
                : System.Math.Min(duration, GetMinimumAudibleAttackMs(prof));

            EnqueuePercussion(new PercussionRequest(p, ct, duration, queueReleaseMs, output, prof, velocity, null));
        }

        public static System.Threading.Tasks.Task PlayPercussionForDurationAsync(
            MidiPercussion p,
            int durationMs,
            System.Threading.CancellationToken ct = default,
            int velocity = 100)
        {
            if (ct.IsCancellationRequested)
                return System.Threading.Tasks.Task.FromCanceled(ct);

            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;
            var prof = GetProfile(p, output);

            if (durationMs <= 0)
                return System.Threading.Tasks.Task.CompletedTask;

            int requestedFrameMs = durationMs;

            // Keep a long-ring instrument's natural decay, but never make the caller await
            // that entire decay. For very short frames, extend only the audible attack so it
            // cannot collapse into silence; the completion still follows requestedFrameMs.
            int minimumAttackMs = GetMinimumAudibleAttackMs(prof);
            int audibleDurationMs = IsCymbalOrLongRing(p)
                ? prof.DurationMs
                : IsShortCymbal(p)
                    ? System.Math.Max(GetMinimumShortCymbalTailMs(p), durationMs)
                    : System.Math.Max(durationMs, minimumAttackMs);

            var completion = new System.Threading.Tasks.TaskCompletionSource<bool>(
                System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);

            EnqueuePercussion(new PercussionRequest(
                p,
                ct,
                audibleDurationMs,
                requestedFrameMs,
                output,
                prof,
                velocity,
                completion));
            return completion.Task;
        }

        /// <summary>
        /// Reserves a short attack slot for percussion without needlessly truncating the
        /// rendered sound-device tail. The MIDI scheduler awaits only sliceDurationMs.
        /// On the sound device, the independent mixer continues the natural envelope after
        /// the slot is released. The motherboard speaker is physically single-voice, so its
        /// audible duration is bounded to the protected attack slot to prevent it repeatedly
        /// overwriting the melody after ownership has passed back.
        /// </summary>
        public static System.Threading.Tasks.Task PlayPercussionSliceAsync(
            MidiPercussion p,
            int sliceDurationMs,
            System.Threading.CancellationToken ct = default,
            int velocity = 100)
        {
            if (ct.IsCancellationRequested)
                return System.Threading.Tasks.Task.FromCanceled(ct);

            if (sliceDurationMs <= 0)
                return System.Threading.Tasks.Task.CompletedTask;

            var output = TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;
            var prof = GetProfile(p, output);
            int protectedSlotMs = System.Math.Clamp(sliceDurationMs, 1, 24);

            int audibleDurationMs;
            if (output == PercussionOutputChoice.SoundDevice)
            {
                // The mixer owns a separate stream, so preserve the complete percussion
                // envelope while releasing the sequencer immediately after the attack slot.
                audibleDurationMs = IsCymbalOrLongRing(p)
                    ? prof.DurationMs
                    : IsShortCymbal(p)
                        ? System.Math.Max(GetMinimumShortCymbalTailMs(p), prof.DurationMs)
                        : System.Math.Max(protectedSlotMs, prof.DurationMs);
            }
            else
            {
                // Continuing to update the PC speaker after this slot would steal the only
                // hardware voice back from the melody and cause reciprocal choking.
                audibleDurationMs = protectedSlotMs;
            }

            var completion = new System.Threading.Tasks.TaskCompletionSource<bool>(
                System.Threading.Tasks.TaskCreationOptions.RunContinuationsAsynchronously);

            EnqueuePercussion(new PercussionRequest(
                p,
                ct,
                audibleDurationMs,
                protectedSlotMs,
                output,
                prof,
                velocity,
                completion));
            return completion.Task;
        }

        private static void EnqueuePercussion(PercussionRequest request)
        {
            bool startWorker = false;

            lock (_queueLock)
            {
                _pendingRequests.Enqueue(request);
                if (!_queueWorkerRunning)
                {
                    _queueWorkerRunning = true;
                    startWorker = true;
                }
            }

            if (startWorker)
                StartQueueWorker();
        }

        private static void StartQueueWorker()
        {
            // Keep the timing worker off the shared thread pool. The worker performs precise
            // waits, and occupying a pool thread can delay the async continuation that queues
            // the next rapid percussion event.
            var worker = new System.Threading.Thread(ProcessPercussionQueue)
            {
                IsBackground = true
            };
            worker.Start();
        }

        private static bool HasPendingRequest()
        {
            lock (_queueLock)
            {
                return _pendingRequests.Count > 0;
            }
        }

        private static bool TryDequeueRequest(out PercussionRequest? request)
        {
            lock (_queueLock)
            {
                if (_pendingRequests.Count == 0)
                {
                    request = null;
                    return false;
                }

                request = _pendingRequests.Dequeue();
                return true;
            }
        }

        private static void ProcessPercussionQueue()
        {
            PercussionOutputChoice? currentOutput = null;
            bool normalShutdown = false;

            try
            {
                while (true)
                {
                    if (!TryDequeueRequest(out PercussionRequest? request) || request == null)
                    {
                        StopCurrentPulse(ref currentOutput);

                        lock (_queueLock)
                        {
                            if (_pendingRequests.Count == 0)
                            {
                                _queueWorkerRunning = false;
                                normalShutdown = true;
                                return;
                            }
                        }

                        continue;
                    }

                    try
                    {
                        PlayQueuedRequest(request, ref currentOutput);
                    }
                    catch (System.Exception ex)
                    {
                        request.Completion?.TrySetException(ex);
                        StopCurrentPulse(ref currentOutput);
                        System.Diagnostics.Debug.WriteLine($"Percussion playback failed: {ex}");
                    }
                }
            }
            finally
            {
                StopCurrentPulse(ref currentOutput);

                if (!normalShutdown)
                {
                    bool restartWorker = false;
                    lock (_queueLock)
                    {
                        _queueWorkerRunning = false;
                        if (_pendingRequests.Count > 0)
                        {
                            _queueWorkerRunning = true;
                            restartWorker = true;
                        }
                    }

                    if (restartWorker)
                        StartQueueWorker();
                }
            }
        }

        private static void PlayQueuedRequest(
            PercussionRequest request,
            ref PercussionOutputChoice? currentOutput)
        {
            if (request.Output == PercussionOutputChoice.SoundDevice)
            {
                // Never touch WaveSynthEngine here: it may currently be playing a melody note.
                PlayMixedSoundDeviceRequest(request);
                return;
            }

            if (request.CancellationToken.IsCancellationRequested)
            {
                request.Completion?.TrySetCanceled();
                return;
            }

            StopCurrentPulse(ref currentOutput);
            PreciseWaitMs(RetriggerGapMs, request.CancellationToken);

            if (request.CancellationToken.IsCancellationRequested)
            {
                request.Completion?.TrySetCanceled();
                return;
            }

            double protectedAttackMs = GetProtectedAttackMs(request);
            long startedAt = System.Diagnostics.Stopwatch.GetTimestamp();
            int lastFrequency = int.MinValue;
            SynthWave lastWave = SynthWave.Square;
            bool started = false;
            bool completionSignaled = false;

            while (true)
            {
                if (request.CancellationToken.IsCancellationRequested)
                    break;

                double elapsedMs = ElapsedMilliseconds(startedAt);

                // Signal rhythmic completion independently from the audible tail. This lets
                // the sequencer enqueue the next rapid hit while a cymbal decay continues.
                if (started && !completionSignaled && elapsedMs >= request.CompletionDelayMs)
                {
                    request.Completion?.TrySetResult(true);
                    completionSignaled = true;
                }

                if (elapsedMs >= request.DurationMs)
                    break;

                // Once the recognizable attack has played, hand the monophonic output to the
                // next queued percussion. With no pending hit, the original tail continues.
                if (started && elapsedMs >= protectedAttackMs && HasPendingRequest())
                    break;

                GetPlaybackSignal(request, elapsedMs, out int frequency, out SynthWave wave, out bool audible);
                if (!audible)
                {
                    StopCurrentPulse(ref currentOutput);
                }
                else if (!started || !currentOutput.HasValue || frequency != lastFrequency || wave != lastWave)
                {
                    StartOrUpdatePulse(request.Output, frequency, wave, ref currentOutput);
                    lastFrequency = frequency;
                    lastWave = wave;
                    started = true;
                }

                double remainingMs = request.DurationMs - elapsedMs;
                double untilSignalChangeMs = GetTimeUntilSignalChangeMs(request, elapsedMs);
                double waitMs = System.Math.Min(QueuePollMs, System.Math.Min(remainingMs, untilSignalChangeMs));
                PreciseWaitMs(System.Math.Max(0.05, waitMs), request.CancellationToken);
            }

            StopCurrentPulse(ref currentOutput);

            if (!completionSignaled)
            {
                if (request.CancellationToken.IsCancellationRequested && !started)
                    request.Completion?.TrySetCanceled();
                else
                    request.Completion?.TrySetResult(started);
            }
        }

        private static int GetMinimumAudibleAttackMs(PercussionProfile profile)
        {
            // Low drums need roughly one to one-and-a-half cycles to be perceived; high/noise
            // percussion needs only a short transient. Keep this bounded to avoid queue latency.
            double cycleBasedMs = 1500.0 / System.Math.Max(37.0, profile.BodyStartFreq);
            return (int)System.Math.Ceiling(System.Math.Clamp(cycleBasedMs, 4.0, 10.0));
        }

        private static double GetProtectedAttackMs(PercussionRequest request)
        {
            // A closed/foot hi-hat needs enough continuous noisy material to be perceived as
            // metal. Releasing it after the generic 4 ms transient turns it into a stick click.
            if (IsShortCymbal(request.Percussion))
                return System.Math.Min(request.DurationMs, 42.0);

            return System.Math.Min(request.DurationMs, GetMinimumAudibleAttackMs(request.Profile));
        }

        private static double ElapsedMilliseconds(long startedAt)
        {
            long ticks = System.Diagnostics.Stopwatch.GetTimestamp() - startedAt;
            return ticks * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
        }

        private static bool UsesShortNoiseTransient(PercussionRequest request)
        {
            return request.DurationMs <= 60
                && !IsCymbalOrLongRing(request.Percussion)
                && request.Profile.BodyWave == SynthWave.Noise;
        }

        private static void GetPlaybackSignal(
            PercussionRequest request,
            double elapsedMs,
            out int frequency,
            out SynthWave wave,
            out bool audible)
        {
            GetPlaybackSignalCore(request, elapsedMs, false, out frequency, out wave, out audible);
        }

        private static bool IsMetalCymbal(MidiPercussion p)
        {
            return p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2
                or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2
                or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal
                or MidiPercussion.RideBell or MidiPercussion.HiHatClosed
                or MidiPercussion.HiHatOpen or MidiPercussion.HiHatFoot;
        }

        private static void GetPlaybackSignalCore(
            PercussionRequest request,
            double elapsedMs,
            bool emulateSystemSpeaker,
            out int frequency,
            out SynthWave wave,
            out bool audible)
        {
            var prof = request.Profile;
            bool systemSpeakerMethod = request.Output == PercussionOutputChoice.SystemSpeaker
                || emulateSystemSpeaker;
            double duration = System.Math.Max(1.0, request.DurationMs);
            double progress = System.Math.Clamp(elapsedMs / duration, 0.0, 1.0);
            double velocity01 = request.Velocity / 127.0;
            bool isNoiseInstrument = prof.BodyWave == SynthWave.Noise;

            // Keep drum attacks close to their body pitch. Extremely bright clicks make a
            // PC speaker expose a musical beep instead of a drum transient.
            if (elapsedMs < prof.AttackMs)
            {
                double attackFrequency = prof.AttackFrequency;
                if (systemSpeakerMethod)
                {
                    double maximumSpeakerAttack = isNoiseInstrument
                        ? System.Math.Max(1200.0, prof.BodyStartFreq * 1.18)
                        : prof.BodyStartFreq * 1.65;
                    attackFrequency = System.Math.Min(attackFrequency, maximumSpeakerAttack);
                }

                frequency = ClampPercussionFrequency(attackFrequency * (0.96 + 0.08 * velocity01));
                wave = systemSpeakerMethod ? SynthWave.Square : SynthWave.Noise;
                audible = true;
                return;
            }

            double envelope = System.Math.Pow(1.0 - progress, prof.DecayShape);
            int cell = (int)System.Math.Floor(elapsedMs / 3.25);
            uint hash = unchecked((uint)(request.RandomSeed + cell * 1103515245 + 12345));
            hash ^= hash << 13;
            hash ^= hash >> 17;
            hash ^= hash << 5;
            double random01 = (hash & 0x00FFFFFF) / 16777215.0;
            double jitter = ((random01 * 2.0) - 1.0) * prof.PitchJitter;

            if (IsMetalCymbal(request.Percussion))
            {
                // Both output modes use the same speaker-compatible metallic-noise method.
                // Longer, lower random pulse cells avoid the piercing FM-like squeak caused
                // by extremely fast high-frequency hopping, while irregular gaps prevent a
                // stable musical pitch from forming.
                double metalCellMs = request.Percussion switch
                {
                    MidiPercussion.HiHatClosed => 0.95,
                    MidiPercussion.HiHatFoot => 1.05,
                    MidiPercussion.RideBell => 1.35,
                    _ => 1.20
                };

                int metalCell = (int)System.Math.Floor(elapsedMs / metalCellMs);
                uint metalHash = unchecked((uint)(request.RandomSeed ^ (metalCell * 747796405)));
                metalHash = (metalHash ^ (metalHash >> 16)) * 2246822519u;
                metalHash ^= metalHash >> 13;

                // Use broad but restrained non-harmonic bands. These sit below the most
                // irritating PC-speaker range and darken naturally throughout the tail.
                double[] ratios = request.Percussion == MidiPercussion.RideBell
                    ? new double[] { 0.62, 0.79, 0.97, 1.18 }
                    : new double[] { 0.43, 0.56, 0.71, 0.88, 1.04, 1.21 };
                int band = (int)(metalHash % (uint)ratios.Length);

                double instrumentScale = request.Percussion switch
                {
                    MidiPercussion.HiHatClosed => 0.58,
                    MidiPercussion.HiHatOpen => 0.66,
                    MidiPercussion.HiHatFoot => 0.52,
                    MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 => 0.68,
                    MidiPercussion.RideBell => 0.76,
                    MidiPercussion.SplashCymbal => 0.74,
                    MidiPercussion.ChinaCymbal => 0.61,
                    _ => 0.65
                };

                double baseMetal = prof.BodyStartFreq * instrumentScale
                    * (1.0 - 0.34 * progress);
                frequency = ClampPercussionFrequency(baseMetal * ratios[band]);
                wave = SynthWave.Square;

                double metalRandom = ((metalHash >> 8) & 0xFFFF) / 65535.0;
                double densityStart = request.Percussion switch
                {
                    MidiPercussion.HiHatClosed => 0.99,
                    MidiPercussion.HiHatFoot => 0.97,
                    _ => 0.88
                };
                double densityEnd = request.Percussion switch
                {
                    MidiPercussion.HiHatClosed => 0.82,
                    MidiPercussion.HiHatFoot => 0.76,
                    MidiPercussion.RideBell => 0.30,
                    _ => 0.18
                };
                double metalDensity = densityEnd
                    + (densityStart - densityEnd) * System.Math.Pow(1.0 - progress, 0.72);
                metalDensity *= 0.82 + 0.18 * velocity01;

                // Preserve a solid initial wash, then become progressively sparse rather
                // than continuously squealing through the complete decay.
                double guaranteedWash = request.Percussion switch
                {
                    MidiPercussion.HiHatClosed => 0.58,
                    MidiPercussion.HiHatFoot => 0.52,
                    _ => 0.10
                };
                audible = progress < guaranteedWash || metalRandom <= metalDensity;
                return;
            }

            if (!isNoiseInstrument)
            {
                // Never pulse-gate the body of kicks, toms, bells, or wood percussion.
                // Continuous low-frequency energy makes them sound solid rather than faint.
                audible = true;

                if (prof.DoesSweep)
                {
                    double curved = 1.0 - System.Math.Exp(-5.0 * progress);
                    curved /= 1.0 - System.Math.Exp(-5.0);
                    double baseFreq = prof.BodyStartFreq * System.Math.Pow(
                        (double)prof.BodyEndFreq / prof.BodyStartFreq, curved);
                    frequency = ClampPercussionFrequency(baseFreq * (1.0 + jitter * 0.30));
                    wave = systemSpeakerMethod ? SynthWave.Square : SynthWave.Noise;
                    return;
                }

                frequency = ClampPercussionFrequency(prof.BodyStartFreq * (1.0 + jitter * 0.25));
                wave = systemSpeakerMethod ? SynthWave.Square : SynthWave.Noise;
                return;
            }

            // Noise instruments need interruptions on a PC speaker to imitate broadband
            // energy, but the attack and main body must remain dense enough to sound strong.
            double densityFloor = systemSpeakerMethod ? 0.40 : 0.72;
            double density = System.Math.Clamp(
                densityFloor + (prof.NoiseDensity - densityFloor) * envelope,
                densityFloor, 1.0);
            density *= 0.78 + 0.22 * velocity01;

            // Keep the first part fully present; gate only the later decay.
            audible = progress < 0.58 || random01 <= density;

            double darkening = 1.0 - 0.30 * progress;
            frequency = ClampPercussionFrequency(prof.BodyStartFreq * darkening * (1.0 + jitter * 0.45));
            wave = systemSpeakerMethod ? SynthWave.Square : SynthWave.Noise;

            if (request.Percussion == MidiPercussion.HandClap)
            {
                double t = elapsedMs - prof.AttackMs;
                bool inBurst = (t < 16.0) || (t >= 21.0 && t < 38.0) ||
                               (t >= 44.0 && t < 64.0) || t >= 72.0;
                audible &= inBurst;
            }
        }

        private static double GetTimeUntilSignalChangeMs(PercussionRequest request, double elapsedMs)
        {
            if (elapsedMs < request.Profile.AttackMs)
                return System.Math.Max(0.05, request.Profile.AttackMs - elapsedMs);

            // Use the same restrained metallic pulse-cell timing for both outputs.
            double cellMs = IsMetalCymbal(request.Percussion)
                ? request.Percussion switch
                {
                    MidiPercussion.HiHatClosed => 0.95,
                    MidiPercussion.HiHatFoot => 1.05,
                    MidiPercussion.RideBell => 1.35,
                    _ => 1.20
                }
                : 3.25;
            double remainder = elapsedMs % cellMs;
            return remainder < 0.0001 ? cellMs : cellMs - remainder;
        }

        public static int GetMidiFrameDurationMs(MidiPercussion percussion, int availableFrameMs, bool melodyAlsoPlaying)
        {
            int naturalMs = GetNaturalDurationMs(percussion);

            if (availableFrameMs <= 0)
                return 0;

            // Frame timing must describe when the next MIDI event may be scheduled, not how
            // long a cymbal is allowed to ring. The playback queue now owns the natural tail.
            if (!melodyAlsoPlaying)
                return System.Math.Min(availableFrameMs, naturalMs);

            return System.Math.Clamp(System.Math.Min(35, naturalMs), 1, availableFrameMs);
        }

        public static int GetNaturalDurationMs(MidiPercussion percussion)
        {
            return GetProfile(percussion, PercussionOutputChoice.SystemSpeaker).DurationMs;
        }
    }
}