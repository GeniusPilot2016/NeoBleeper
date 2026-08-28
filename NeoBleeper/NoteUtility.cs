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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Converts a MIDI velocity value (0 to 127) to a normalized gain scalar (0.0 to 1.0).
        /// </summary>
        public static double VelocityToGain(int velocity)
        {
            int clamped = Math.Clamp(velocity, 0, 127);
            return clamped / 127.0;
        }

        /// <summary>
        /// Converts a MIDI velocity value to a 1-bit system speaker pulse duty cycle (0.05 to 0.50).
        /// </summary>
        public static double VelocityToDutyCycle(int velocity)
        {
            double gain = VelocityToGain(velocity);
            return Math.Clamp(0.05 + gain * 0.45, 0.01, 0.50);
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
            return CalculateNoteDurations(lengthName, bpm, modifier, articulation, noteSilenceRatio, 127);
        }

        public static (int totalRhythm_int, int noteSound_int) CalculateNoteDurations(
            string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio, int velocity)
        {
            if (bpm == 0)
                bpm = 1;

            var (lengthName_checked, modifier_checked, articulation_checked) =
                UseOriginalValueOrDefault(lengthName, modifier, articulation);

            double totalRhythm_double = FixRoundingErrors(
                CalculateLineLength(bpm, lengthName_checked, modifier_checked));

            double velocityFactor = Math.Clamp(velocity / 127.0, 0.3, 1.0);
            double effectiveSilenceRatio = noteSilenceRatio * velocityFactor;

            double noteSound_double = FixRoundingErrors(
                CalculateNoteLength(totalRhythm_double, articulation_checked));

            if (articulation_checked == "Fer")
            {
                double extraFermataDuration = totalRhythm_double * (0.5 + 0.5 * Random.Shared.NextDouble());
                totalRhythm_double += extraFermataDuration;
                noteSound_double = FixRoundingErrors(
                    CalculateNoteLength(totalRhythm_double, articulation_checked));
            }

            noteSound_double *= effectiveSilenceRatio;

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
            return CalculateNoteDurationsAtPosition(lengthName, bpm, modifier, articulation, noteSilenceRatio, cursorMs, 127);
        }

        public static (int totalRhythm_int, int noteSound_int, double nextCursorMs) CalculateNoteDurationsAtPosition(
            string lengthName, int bpm, string modifier, string articulation, double noteSilenceRatio,
            double cursorMs, int velocity)
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

            double velocityFactor = Math.Clamp(velocity / 127.0, 0.3, 1.0);
            double effectiveSilenceRatio = noteSilenceRatio * velocityFactor;

            double noteSound_double = FixRoundingErrors(
                CalculateNoteLength(totalRhythm_double, articulation_checked)) * effectiveSilenceRatio;

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
        private enum SynthWave { Sine, Square, Triangle, Noise }

        private const double RetriggerGapMs = 0.5;
        private const double MinAudibleSystemSpeakerMs = 12.0;

        private static readonly object _hardwareLock = new object();
        private static readonly object _queueLock = new object();
        private static readonly Queue<PercussionRequest> _pendingRequests = new Queue<PercussionRequest>();
        private static bool _queueWorkerRunning;

        private const int PercussionSampleRate = 44100;
        // Sound Device PWM is emitted as a true binary carrier stream. 192 kHz / 24 kHz =
        // exactly 8 output samples per PWM period, which gives a stable ultrasonic carrier
        // while leaving enough duty-cycle resolution for the rendered percussion envelope.
        private const int SoundDevicePwmSampleRate = 192000;
        private const int SoundDevicePwmCarrierHz = 24000;
        private const int SoundDevicePwmSamplesPerPeriod = SoundDevicePwmSampleRate / SoundDevicePwmCarrierHz;

        private static readonly object _soundDeviceLock = new object();
        private static NAudio.Wave.WaveOutEvent? _percussionWaveOut;
        private static NAudio.Wave.SampleProviders.MixingSampleProvider? _percussionMixer;

        private sealed class FloatArraySampleProvider : NAudio.Wave.ISampleProvider
        {
            private readonly float[] _samples;
            private int _position;

            public FloatArraySampleProvider(float[] samples, int sampleRate)
            {
                _samples = samples ?? Array.Empty<float>();
                WaveFormat = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);
            }

            public NAudio.Wave.WaveFormat WaveFormat { get; }

            public int Read(float[] buffer, int offset, int count)
            {
                int available = _samples.Length - _position;
                int toCopy = Math.Min(available, count);
                if (toCopy <= 0) return 0;

                Array.Copy(_samples, _position, buffer, offset, toCopy);
                _position += toCopy;
                return toCopy;
            }
        }

        private sealed class PercussionRequest
        {
            public readonly MidiPercussion Percussion;
            public readonly CancellationToken CancellationToken;
            public readonly int DurationMs;
            public readonly int CompletionDelayMs;
            public readonly PercussionOutputChoice Output;
            public readonly PercussionProfile Profile;
            public readonly int Velocity;
            public readonly int RandomSeed;
            public readonly TaskCompletionSource<bool>? Completion;

            public PercussionRequest(
                MidiPercussion percussion,
                CancellationToken cancellationToken,
                int durationMs,
                int completionDelayMs,
                PercussionOutputChoice output,
                PercussionProfile profile,
                int velocity,
                TaskCompletionSource<bool>? completion)
            {
                Percussion = percussion;
                CancellationToken = cancellationToken;
                DurationMs = Math.Max(1, durationMs);
                CompletionDelayMs = Math.Max(1, completionDelayMs);
                Output = output;
                Profile = profile;
                Velocity = Math.Clamp(velocity, 1, 127);
                RandomSeed = Random.Shared.Next();
                Completion = completion;
            }
        }

        private static int ClampPercussionFrequency(double frequency, double minHz = 120.0, double maxHz = 1200.0) =>
            (int)Math.Round(Math.Clamp(frequency, minHz, maxHz));

        private static void StartPulseDirect(PercussionOutputChoice outputChoice, int frequency, SynthWave waveType)
        {
            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    // Small PC speaker drivers are weak at true bass but resonate fine well up into the
                    // treble range - that asymmetry is the opposite of what a full-range speaker does, and
                    // it's also exactly what the metal-cymbal frequency-hopping logic in
                    // GetPlaybackSignalCore depends on: it jitters the tone rapidly across a wide high-Hz
                    // band (up to ~8kHz) to fake a noise hiss on hardware that can only emit ONE square
                    // wave tone at a time. Capping the ceiling too low collapses that jitter range down to
                    // a narrow band, which is what was making cymbals sound dull instead of shimmery.
                    frequency = (int)Math.Clamp(frequency, 100.0, 9000.0);
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                    break;

                case PercussionOutputChoice.SoundDevice:
                    frequency = (int)Math.Clamp(frequency, 20.0, 12000.0);
                    var naudioWave = waveType switch
                    {
                        SynthWave.Noise => NAudio.Wave.SampleProviders.SignalGeneratorType.White,
                        SynthWave.Triangle => NAudio.Wave.SampleProviders.SignalGeneratorType.Triangle,
                        SynthWave.Sine => NAudio.Wave.SampleProviders.SignalGeneratorType.Sin,
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

                var format = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(SoundDevicePwmSampleRate, 1);
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
            // Keep this helper on the exact same path as every other mode: synthesize with the
            // Sound Device core, quantize to PCM, then convert that PCM to a one-bit PWM stream.
            float[] rendered = RenderPercussionSamples(request);
            byte[] pcm8 = ConvertFloatSamplesToUnsigned8BitPcm(rendered);
            PlayPCMSoundAsPWM(pcm8, PercussionOutputChoice.SoundDevice, request.Profile.BodyWave);
        }

        private static void QueueMixedSoundDevicePwmSamples(float[] pwmSamples)
        {
            EnsurePercussionSoundDevice();
            var hitProvider = new FloatArraySampleProvider(pwmSamples, SoundDevicePwmSampleRate);

            lock (_soundDeviceLock)
            {
                _percussionMixer?.AddMixerInput(hitProvider);
            }
        }

        /// <summary>
        /// Renders a percussion hit once with the Sound Device synthesis core, then routes that
        /// exact rendered signal to the selected output. Both Sound Device and System Speaker
        /// consume an unsigned 8-bit representation only as PWM duty-cycle information.
        /// </summary>
        private static void PlayRenderedPercussion(PercussionRequest request)
        {
            // Render once with the Sound Device synthesis core for every output mode. The common
            // unsigned-8-bit representation is then routed through PlayPCMSoundAsPWM. Both output
            // modes convert it to a binary PWM signal; neither mode reconstructs ordinary PCM.
            float[] samples = RenderPercussionSamples(request);
            byte[] pcm8 = ConvertFloatSamplesToUnsigned8BitPcm(samples);
            PlayPCMSoundAsPWM(pcm8, request.Output, request.Profile.BodyWave);
        }

        private static byte[] ConvertFloatSamplesToUnsigned8BitPcm(float[] samples)
        {
            var pcm = new byte[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                double normalized = Math.Clamp(samples[i], -1.0f, 1.0f);
                pcm[i] = (byte)Math.Round((normalized + 1.0) * 127.5, MidpointRounding.AwayFromZero);
            }
            return pcm;
        }

        private static float[] RenderPercussionSamples(PercussionRequest request)
        {
            int sampleCount = Math.Max(1, (int)Math.Ceiling(request.DurationMs * PercussionSampleRate / 1000.0));
            var result = new float[sampleCount];
            double velocity01 = request.Velocity / 127.0;
            double masterGain = 0.70 + velocity01 * 0.40;

            uint rng = unchecked((uint)request.RandomSeed);
            double lpLow = 0.0, lpMid = 0.0, lpHigh = 0.0;
            double dcBlockX = 0.0, dcBlockY = 0.0;
            double tonePhase = 0.0;

            bool kick = IsKick(request.Percussion);
            bool tom = IsTomOrBongo(request.Percussion);
            bool snare = IsSnare(request.Percussion);
            bool cymbal = IsMetalCymbal(request.Percussion);
            bool shaker = request.Percussion is MidiPercussion.Shaker or MidiPercussion.Maracas or MidiPercussion.Cabasa;
            bool isTonalSquare = request.Profile.BodyWave == SynthWave.Square;

            for (int i = 0; i < sampleCount; i++)
            {
                double elapsedMs = i * 1000.0 / PercussionSampleRate;
                double progress = Math.Clamp(elapsedMs / request.DurationMs, 0.0, 1.0);

                rng ^= rng << 13;
                rng ^= rng >> 17;
                rng ^= rng << 5;
                double white = ((rng & 0x00FFFFFF) / 8388607.5) - 1.0;

                double lowCut = kick ? 120.0 : tom ? 220.0 : 600.0;
                double midCut = snare ? 2800.0 : cymbal ? 5000.0 : 2000.0;
                double highCut = cymbal ? 10000.0 : 7500.0;

                double aLow = 1.0 - Math.Exp(-2.0 * Math.PI * lowCut / PercussionSampleRate);
                double aMid = 1.0 - Math.Exp(-2.0 * Math.PI * midCut / PercussionSampleRate);
                double aHigh = 1.0 - Math.Exp(-2.0 * Math.PI * highCut / PercussionSampleRate);

                lpLow += aLow * (white - lpLow);
                lpMid += aMid * (white - lpMid);
                lpHigh += aHigh * (white - lpHigh);

                // One-pole leaky-integrator filters attenuate noise amplitude proportionally to
                // sqrt(a/(2-a)) at steady state. Low cutoffs (kicks/toms) have tiny 'a' and were
                // coming out at a fraction of the level of higher-cutoff bands (snares/cymbals),
                // making low percussion nearly inaudible in Sound Device mode. Normalize each
                // band back to full-scale noise amplitude before mixing so loudness is
                // independent of the chosen cutoff frequency.
                double normLow = Math.Sqrt((2.0 - aLow) / Math.Max(aLow, 1e-6));
                double normMid = Math.Sqrt((2.0 - aMid) / Math.Max(aMid, 1e-6));

                double lowBand = lpLow * normLow;
                double midBand = (lpMid - lpLow) * normMid;
                double highBand = white - lpHigh; // already near full scale; aHigh is large

                double body = Math.Exp(-elapsedMs / Math.Max(25.0, request.DurationMs * 0.5));
                double tail = Math.Pow(Math.Max(0.0, 1.0 - progress), request.Profile.DecayShape);

                double toneVal = 0.0;
                if (isTonalSquare || request.Profile.BodyWave == SynthWave.Sine)
                {
                    double currentFreq = request.Profile.BodyStartFreq;
                    if (request.Profile.DoesSweep)
                    {
                        double logSweep = 1.0 - Math.Pow(progress, 0.45);
                        currentFreq = request.Profile.BodyEndFreq + (request.Profile.BodyStartFreq - request.Profile.BodyEndFreq) * logSweep;
                    }
                    tonePhase = (tonePhase + currentFreq / PercussionSampleRate) % 1.0;
                    toneVal = request.Profile.BodyWave == SynthWave.Square
                        ? (tonePhase < 0.5 ? 0.7 : -0.7)
                        : Math.Sin(tonePhase * 2.0 * Math.PI);
                }

                double source = 0.0;
                if (kick) source = toneVal * body * 1.2 + lowBand * body * 0.8;
                else if (tom) source = toneVal * body * 1.0 + midBand * body * 0.5;
                else if (snare) source = midBand * body * 1.4 + highBand * tail * 0.8;
                else if (cymbal) source = highBand * tail * 1.4 + midBand * tail * 0.4;
                else if (shaker) source = highBand * tail * 1.2;
                else source = toneVal * body * 1.0 + midBand * body * 0.8;

                double sample = source * masterGain;
                double dcBlocked = sample - dcBlockX + 0.995 * dcBlockY;
                dcBlockX = sample;
                dcBlockY = dcBlocked;

                result[i] = (float)(Math.Tanh(dcBlocked) * 0.90);
            }

            return result;
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
                NoiseDensity = Math.Clamp(density, 0.01, 1.0);
                HoldRatio = Math.Clamp(holdRatio, 0.01, 0.95);
                AttackFrequency = attackFrequency <= 0 ? start : attackFrequency;
                AttackMs = Math.Clamp(attackMs, 0.2, 20.0);
                DecayShape = Math.Clamp(decayShape, 0.5, 5.0);
                PitchJitter = Math.Clamp(pitchJitter, 0.0, 0.25);
            }
        }

        private static PercussionProfile GetProfile(MidiPercussion percussion)
        {
            return percussion switch
            {
                MidiPercussion.Laser =>
                    new PercussionProfile(SynthWave.Square, true, 800, 200, 350, holdRatio: 0.05, attackFrequency: 800, attackMs: 0.5, decayShape: 1.2, pitchJitter: 0.02),

                MidiPercussion.Whip =>
                    new PercussionProfile(SynthWave.Noise, true, 700, 200, 180, density: 0.8, holdRatio: 0.02, attackFrequency: 750, attackMs: 0.8, decayShape: 1.5, pitchJitter: 0.1),

                MidiPercussion.ScratchPush =>
                    new PercussionProfile(SynthWave.Noise, true, 300, 700, 140, density: 0.75, holdRatio: 0.05, attackFrequency: 350, attackMs: 1.5, decayShape: 1.8, pitchJitter: 0.15),
                MidiPercussion.ScratchPull =>
                    new PercussionProfile(SynthWave.Noise, true, 700, 300, 140, density: 0.75, holdRatio: 0.05, attackFrequency: 700, attackMs: 1.5, decayShape: 1.8, pitchJitter: 0.15),

                MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets =>
                    new PercussionProfile(SynthWave.Noise, false, 600, 600, 30, density: 0.9, holdRatio: 0.01, attackFrequency: 650, attackMs: 0.4, decayShape: 4.5, pitchJitter: 0.03),

                MidiPercussion.MetronomeBell =>
                    new PercussionProfile(SynthWave.Noise, true, 800, 600, 300, density: 0.5, holdRatio: 0.3, attackFrequency: 850, attackMs: 0.6, decayShape: 1.3, pitchJitter: 0.03),

                MidiPercussion.BassDrum or MidiPercussion.KickDrum =>
                    new PercussionProfile(SynthWave.Noise, true, 160, 55, 150, density: 0.95, holdRatio: 0.02, attackFrequency: 180, attackMs: 1.5, decayShape: 2.4, pitchJitter: 0.005),

                MidiPercussion.SideStick =>
                    new PercussionProfile(SynthWave.Noise, false, 500, 500, 40, density: 0.9, holdRatio: 0.01, attackFrequency: 550, attackMs: 0.4, decayShape: 4.0, pitchJitter: 0.03),

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush =>
                    new PercussionProfile(SynthWave.Noise, false, 400, 400, 120, density: 0.85, holdRatio: 0.02, attackFrequency: 450, attackMs: 1.2, decayShape: 2.8, pitchJitter: 0.06),

                MidiPercussion.HandClap =>
                    new PercussionProfile(SynthWave.Noise, false, 500, 500, 140, density: 0.85, holdRatio: 0.02, attackFrequency: 550, attackMs: 0.5, decayShape: 2.0, pitchJitter: 0.12),

                MidiPercussion.FloorTom2 =>
                    new PercussionProfile(SynthWave.Noise, true, 140, 65, 200, density: 0.9, holdRatio: 0.06, attackFrequency: 160, attackMs: 2.0, decayShape: 1.7, pitchJitter: 0.01),

                MidiPercussion.HiHatClosed =>
                    new PercussionProfile(SynthWave.Noise, false, 800, 800, 80, density: 0.85, holdRatio: 0.02, attackFrequency: 850, attackMs: 0.5, decayShape: 3.0, pitchJitter: 0.06),
                MidiPercussion.FloorTom1 =>
                    new PercussionProfile(SynthWave.Noise, true, 140, 65, 200, density: 0.9, holdRatio: 0.06, attackFrequency: 160, attackMs: 2.0, decayShape: 1.7, pitchJitter: 0.01),
                MidiPercussion.HiHatFoot =>
                    new PercussionProfile(SynthWave.Noise, false, 750, 750, 90, density: 0.8, holdRatio: 0.02, attackFrequency: 800, attackMs: 0.6, decayShape: 2.8, pitchJitter: 0.05),
                MidiPercussion.LowTom =>
                    new PercussionProfile(SynthWave.Noise, true, 180, 85, 180, density: 0.9, holdRatio: 0.05, attackFrequency: 200, attackMs: 1.8, decayShape: 1.9, pitchJitter: 0.01),
                MidiPercussion.HiHatOpen =>
                    new PercussionProfile(SynthWave.Noise, false, 800, 800, 450, density: 0.6, holdRatio: 0.05, attackFrequency: 850, attackMs: 1.2, decayShape: 2.2, pitchJitter: 0.08),
                MidiPercussion.LowMidTom =>
                    new PercussionProfile(SynthWave.Noise, true, 180, 85, 180, density: 0.9, holdRatio: 0.05, attackFrequency: 200, attackMs: 1.8, decayShape: 1.9, pitchJitter: 0.01),
                MidiPercussion.HighMidTom =>
                    new PercussionProfile(SynthWave.Noise, true, 180, 85, 180, density: 0.9, holdRatio: 0.05, attackFrequency: 200, attackMs: 1.8, decayShape: 1.9, pitchJitter: 0.01),

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal =>
                    new PercussionProfile(SynthWave.Noise, false, 750, 750, 700, density: 0.7, holdRatio: 0.08, attackFrequency: 800, attackMs: 1.2, decayShape: 2.2, pitchJitter: 0.1),
                MidiPercussion.HighTom =>
                    new PercussionProfile(SynthWave.Noise, true, 220, 110, 160, density: 0.9, holdRatio: 0.05, attackFrequency: 240, attackMs: 1.5, decayShape: 2.0, pitchJitter: 0.01),
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 =>
                    new PercussionProfile(SynthWave.Noise, false, 700, 700, 600, density: 0.6, holdRatio: 0.10, attackFrequency: 750, attackMs: 1.0, decayShape: 2.0, pitchJitter: 0.08),

                MidiPercussion.RideBell =>
                    new PercussionProfile(SynthWave.Noise, false, 850, 850, 500, density: 0.5, holdRatio: 0.15, attackFrequency: 900, attackMs: 0.8, decayShape: 1.8, pitchJitter: 0.04),

                MidiPercussion.Tambourine =>
                    new PercussionProfile(SynthWave.Noise, false, 800, 800, 180, density: 0.65, holdRatio: 0.02, attackFrequency: 850, attackMs: 0.8, decayShape: 2.0, pitchJitter: 0.15),

                MidiPercussion.Cowbell =>
                    new PercussionProfile(SynthWave.Noise, true, 650, 580, 220, density: 0.55, holdRatio: 0.1, attackFrequency: 700, attackMs: 1.0, decayShape: 1.3, pitchJitter: 0.04),

                MidiPercussion.Vibraslap =>
                    new PercussionProfile(SynthWave.Noise, false, 600, 600, 350, density: 0.5, holdRatio: 0.05, attackFrequency: 650, attackMs: 1.5, decayShape: 1.3, pitchJitter: 0.2),

                MidiPercussion.HighBongo =>
                    new PercussionProfile(SynthWave.Noise, true, 500, 350, 100, density: 0.85, holdRatio: 0.05, attackFrequency: 550, attackMs: 1.2, decayShape: 2.5, pitchJitter: 0.02),
                MidiPercussion.LowBongo =>
                    new PercussionProfile(SynthWave.Noise, true, 350, 220, 110, density: 0.85, holdRatio: 0.05, attackFrequency: 380, attackMs: 1.3, decayShape: 2.4, pitchJitter: 0.02),
                MidiPercussion.CongaDeadStroke =>
                    new PercussionProfile(SynthWave.Noise, true, 320, 260, 70, density: 0.85, holdRatio: 0.03, attackFrequency: 340, attackMs: 1.0, decayShape: 3.5, pitchJitter: 0.02),
                MidiPercussion.Conga =>
                    new PercussionProfile(SynthWave.Noise, true, 300, 200, 140, density: 0.85, holdRatio: 0.06, attackFrequency: 320, attackMs: 1.4, decayShape: 2.0, pitchJitter: 0.02),
                MidiPercussion.Tumba =>
                    new PercussionProfile(SynthWave.Noise, true, 250, 160, 150, density: 0.85, holdRatio: 0.06, attackFrequency: 270, attackMs: 1.5, decayShape: 1.9, pitchJitter: 0.02),
                MidiPercussion.HighTimbale =>
                    new PercussionProfile(SynthWave.Noise, true, 500, 350, 130, density: 0.8, holdRatio: 0.04, attackFrequency: 550, attackMs: 1.0, decayShape: 2.2, pitchJitter: 0.02),
                MidiPercussion.LowTimbale =>
                    new PercussionProfile(SynthWave.Noise, true, 400, 280, 150, density: 0.8, holdRatio: 0.04, attackFrequency: 440, attackMs: 1.2, decayShape: 2.0, pitchJitter: 0.02),

                MidiPercussion.HighAgogo =>
                    new PercussionProfile(SynthWave.Noise, true, 750, 680, 220, density: 0.55, holdRatio: 0.08, attackFrequency: 800, attackMs: 0.8, decayShape: 1.4, pitchJitter: 0.04),
                MidiPercussion.LowAgogo =>
                    new PercussionProfile(SynthWave.Noise, true, 550, 500, 260, density: 0.55, holdRatio: 0.08, attackFrequency: 600, attackMs: 0.9, decayShape: 1.3, pitchJitter: 0.04),

                MidiPercussion.Cabasa =>
                    new PercussionProfile(SynthWave.Noise, false, 800, 800, 130, density: 0.55, holdRatio: 0.02, attackFrequency: 850, attackMs: 0.5, decayShape: 2.5, pitchJitter: 0.1),
                MidiPercussion.Maracas =>
                    new PercussionProfile(SynthWave.Noise, false, 850, 850, 100, density: 0.5, holdRatio: 0.02, attackFrequency: 900, attackMs: 0.4, decayShape: 2.8, pitchJitter: 0.1),

                MidiPercussion.WhistleShort =>
                    new PercussionProfile(SynthWave.Noise, false, 700, 700, 160, density: 0.4, holdRatio: 0.35, attackFrequency: 700, attackMs: 3.0, decayShape: 1.5, pitchJitter: 0.02),
                MidiPercussion.WhistleLong =>
                    new PercussionProfile(SynthWave.Noise, true, 700, 550, 500, density: 0.4, holdRatio: 0.35, attackFrequency: 700, attackMs: 5.0, decayShape: 1.1, pitchJitter: 0.02),

                MidiPercussion.GuiroShort =>
                    new PercussionProfile(SynthWave.Noise, true, 600, 400, 90, density: 0.7, holdRatio: 0.05, attackFrequency: 650, attackMs: 1.0, decayShape: 2.0, pitchJitter: 0.05),
                MidiPercussion.GuiroLong =>
                    new PercussionProfile(SynthWave.Noise, true, 600, 350, 350, density: 0.7, holdRatio: 0.1, attackFrequency: 650, attackMs: 2.0, decayShape: 1.2, pitchJitter: 0.05),

                MidiPercussion.Claves =>
                    new PercussionProfile(SynthWave.Noise, false, 800, 800, 40, density: 0.85, holdRatio: 0.02, attackFrequency: 850, attackMs: 0.4, decayShape: 4.0, pitchJitter: 0.02),

                MidiPercussion.HighWoodblock =>
                    new PercussionProfile(SynthWave.Noise, false, 750, 750, 45, density: 0.85, holdRatio: 0.02, attackFrequency: 800, attackMs: 0.4, decayShape: 3.8, pitchJitter: 0.02),
                MidiPercussion.LowWoodblock =>
                    new PercussionProfile(SynthWave.Noise, false, 550, 550, 50, density: 0.85, holdRatio: 0.02, attackFrequency: 600, attackMs: 0.5, decayShape: 3.6, pitchJitter: 0.02),

                MidiPercussion.CuicaHigh =>
                    new PercussionProfile(SynthWave.Noise, true, 600, 350, 220, density: 0.7, holdRatio: 0.08, attackFrequency: 650, attackMs: 2.0, decayShape: 1.6, pitchJitter: 0.05),
                MidiPercussion.CuicaLow =>
                    new PercussionProfile(SynthWave.Noise, true, 400, 200, 260, density: 0.7, holdRatio: 0.08, attackFrequency: 430, attackMs: 2.2, decayShape: 1.5, pitchJitter: 0.05),

                MidiPercussion.TriangleMute =>
                    new PercussionProfile(SynthWave.Noise, false, 850, 850, 60, density: 0.4, holdRatio: 0.05, attackFrequency: 900, attackMs: 0.5, decayShape: 3.0, pitchJitter: 0.03),
                MidiPercussion.TriangleOpen =>
                    new PercussionProfile(SynthWave.Noise, false, 850, 850, 400, density: 0.35, holdRatio: 0.4, attackFrequency: 900, attackMs: 1.0, decayShape: 1.0, pitchJitter: 0.02),

                MidiPercussion.Shaker =>
                    new PercussionProfile(SynthWave.Noise, false, 800, 800, 110, density: 0.55, holdRatio: 0.02, attackFrequency: 850, attackMs: 0.5, decayShape: 2.6, pitchJitter: 0.1),

                MidiPercussion.SleighBell =>
                    new PercussionProfile(SynthWave.Noise, false, 850, 850, 200, density: 0.5, holdRatio: 0.1, attackFrequency: 900, attackMs: 0.6, decayShape: 1.8, pitchJitter: 0.15),

                MidiPercussion.BellTree =>
                    new PercussionProfile(SynthWave.Noise, true, 850, 450, 450, density: 0.45, holdRatio: 0.25, attackFrequency: 850, attackMs: 1.0, decayShape: 1.1, pitchJitter: 0.06),

                MidiPercussion.SurduDeadStroke =>
                    new PercussionProfile(SynthWave.Noise, true, 130, 90, 90, density: 0.9, holdRatio: 0.03, attackFrequency: 140, attackMs: 1.5, decayShape: 3.0, pitchJitter: 0.01),
                MidiPercussion.Surdu =>
                    new PercussionProfile(SynthWave.Noise, true, 130, 60, 220, density: 0.9, holdRatio: 0.06, attackFrequency: 145, attackMs: 2.0, decayShape: 2.0, pitchJitter: 0.01),

                MidiPercussion.OceanDrum =>
                    new PercussionProfile(SynthWave.Noise, false, 500, 500, 600, density: 0.45, holdRatio: 0.5, attackFrequency: 550, attackMs: 8.0, decayShape: 1.0, pitchJitter: 0.08),

                _ => new PercussionProfile(SynthWave.Noise, false, 400, 400, 100, density: 0.6, holdRatio: 0.02, attackFrequency: 450, attackMs: 1.2, decayShape: 2.5, pitchJitter: 0.06)
            };
        }

        private static bool IsKick(MidiPercussion p) => p is MidiPercussion.KickDrum or MidiPercussion.BassDrum or MidiPercussion.Surdu or MidiPercussion.SurduDeadStroke;
        private static bool IsTomOrBongo(MidiPercussion p) => p is MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 or MidiPercussion.LowTom or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom or MidiPercussion.HighTom
            or MidiPercussion.HighBongo or MidiPercussion.LowBongo or MidiPercussion.Conga or MidiPercussion.CongaDeadStroke or MidiPercussion.Tumba
            or MidiPercussion.HighTimbale or MidiPercussion.LowTimbale or MidiPercussion.CuicaHigh or MidiPercussion.CuicaLow;
        private static bool IsSnare(MidiPercussion p) => p is MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush or MidiPercussion.SideStick;
        private static bool IsClick(MidiPercussion p) => p is MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets
            or MidiPercussion.Claves or MidiPercussion.WoodBlock or MidiPercussion.HighWoodblock or MidiPercussion.LowWoodblock;
        private static bool IsCymbalOrLongRing(MidiPercussion p) => p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2
            or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal or MidiPercussion.HiHatOpen or MidiPercussion.RideBell;
        private static bool IsShortCymbal(MidiPercussion p) => p is MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot or MidiPercussion.TriangleMute or MidiPercussion.Tambourine;
        private static bool IsMetalCymbal(MidiPercussion p) => IsCymbalOrLongRing(p) || IsShortCymbal(p) || p is MidiPercussion.SleighBell or MidiPercussion.Shaker or MidiPercussion.Cabasa or MidiPercussion.Maracas;
        private static bool IsTonalNonCymbal(MidiPercussion p) => p is MidiPercussion.Laser;

        public static void PlayPercussion(MidiPercussion p, CancellationToken ct = default, int maxMs = 5000, int velocity = 100)
        {
            if (ct.IsCancellationRequested) return;

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);
            int duration = Math.Max(30, Math.Min(maxMs, prof.DurationMs));

            EnqueuePercussion(new PercussionRequest(p, ct, duration, duration, output, prof, velocity, null));
        }

        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct = default, int velocity = 100)
        {
            return PlayPercussionForDurationAsync(p, durationMs, ct, velocity, enforceMinimumAudibleBody: true);
        }

        /// <summary>
        /// Queued (background-worker) percussion playback. Suitable for solo/fire-and-forget
        /// hits that don't need to interleave with a melody note on the same frame. Do NOT use
        /// this from the melody+percussion alternating playback path — use
        /// <see cref="PlayPercussionSliceImmediateAsync"/> instead, since queued requests from
        /// different callers can be processed out of order relative to per-frame expectations.
        /// </summary>
        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct, int velocity, bool enforceMinimumAudibleBody)
        {
            if (ct.IsCancellationRequested) return Task.FromCanceled(ct);

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);

            if (durationMs <= 0) return Task.CompletedTask;

            int audibleDurationMs = ResolveAudibleDurationMs(p, durationMs, output, enforceMinimumAudibleBody);

            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            EnqueuePercussion(new PercussionRequest(p, ct, audibleDurationMs, durationMs, output, prof, velocity, completion));
            return completion.Task;
        }

        public static Task PlayPercussionSliceAsync(MidiPercussion p, int sliceDurationMs, CancellationToken ct = default, int velocity = 100)
        {
            return PlayPercussionForDurationAsync(p, sliceDurationMs, ct, velocity, enforceMinimumAudibleBody: true);
        }

        /// <summary>
        /// Plays a percussion hit for a frame slice that must interleave, on the same physical
        /// single-voice output, with a melody note the caller is about to play immediately
        /// afterward. Unlike the queued entry points above, this NEVER goes through the shared
        /// background worker/queue: it runs the hit directly, using its own fully local hardware
        /// state (no <c>ref</c> state shared with a concurrently running queued request), so it
        /// can never be delayed behind, or have its oscillator state clobbered by, unrelated
        /// queued hits. This is what fixes both "notes suppressed after some percussion" (hits
        /// no longer wait in a FIFO behind other hits) and "the wrong percussion sound plays"
        /// (no shared mutable state to race on).
        /// </summary>
        public static async Task PlayPercussionSliceImmediateAsync(
            MidiPercussion p,
            int sliceDurationMs,
            CancellationToken ct = default,
            int velocity = 100,
            bool enforceMinimumAudibleBody = true)
        {
            if (ct.IsCancellationRequested) return;
            if (sliceDurationMs <= 0) return;

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);
            int audibleDurationMs = ResolveAudibleDurationMs(p, sliceDurationMs, output, enforceMinimumAudibleBody);

            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var request = new PercussionRequest(p, ct, audibleDurationMs, sliceDurationMs, output, prof, velocity, completion);

            if (output == PercussionOutputChoice.SoundDevice)
            {
                // The same renderer is used for every output. Sound Device can consume its float
                // samples directly and mix them additively.
                PlayRenderedPercussion(request);
                _ = Task.Delay(request.CompletionDelayMs, ct).ContinueWith(t =>
                {
                    if (ct.IsCancellationRequested) completion.TrySetCanceled(ct);
                    else completion.TrySetResult(true);
                }, TaskContinuationOptions.ExecuteSynchronously);
            }
            else
            {
                // System Speaker playback is synchronous because the PCM waveform has to be
                // converted into precisely-timed one-bit PWM pulses.
                await Task.Run(() => PlayRenderedPercussion(request), ct).ConfigureAwait(false);
                if (ct.IsCancellationRequested) completion.TrySetCanceled(ct);
                else completion.TrySetResult(true);
            }

            await completion.Task.ConfigureAwait(false);
        }

        public static Task PlayPercussionSliceAsync(
            MidiPercussion p,
            int sliceDurationMs,
            CancellationToken ct,
            int velocity,
            bool enforceMinimumAudibleBody)
        {
            return PlayPercussionSliceImmediateAsync(p, sliceDurationMs, ct, velocity, enforceMinimumAudibleBody);
        }

        private static int ResolveAudibleDurationMs(
            MidiPercussion p,
            int requestedDurationMs,
            PercussionOutputChoice output,
            bool enforceMinimumAudibleBody)
        {
            // Sound Device hits are mixed independently of everything else, so stretching a
            // short slice up to a minimum audible body never costs melody anything and should
            // always happen there. System Speaker has one physical oscillator: stretching here
            // blocks whoever is waiting (melody) for the stretched length, not just the slice
            // it was given. Callers sharing the speaker with melody pass
            // enforceMinimumAudibleBody:false so a drum hit never eats into time budgeted for a
            // note.
            bool shouldEnforceMinimumBody = enforceMinimumAudibleBody || output == PercussionOutputChoice.SoundDevice;

            if (!shouldEnforceMinimumBody)
            {
                return requestedDurationMs;
            }

            int minBodyMs = (int)Math.Ceiling(GetMinimumBodyMs(p));
            return Math.Max(requestedDurationMs, minBodyMs);
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
            if (startWorker) Task.Run(ProcessPercussionQueue);
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
                                return;
                            }
                        }
                        continue;
                    }

                    // Always render and play via PCM->PWM pipeline across both output modes
                    PlayRenderedPercussion(request);

                    if (request.Completion != null)
                    {
                        int delay = request.CompletionDelayMs;
                        var ct = request.CancellationToken;
                        var tcs = request.Completion;
                        Task.Delay(delay, ct).ContinueWith(t =>
                        {
                            if (ct.IsCancellationRequested) tcs.TrySetCanceled(ct);
                            else tcs.TrySetResult(true);
                        }, TaskContinuationOptions.ExecuteSynchronously);
                    }
                }
            }
            finally
            {
                StopCurrentPulse(ref currentOutput);
                lock (_queueLock) { _queueWorkerRunning = false; }
            }
        }

        private static void PlayQueuedRequestCommon(PercussionRequest request, ref PercussionOutputChoice? currentOutput)
        {
            if (request.CancellationToken.IsCancellationRequested)
            {
                request.Completion?.TrySetCanceled();
                return;
            }

            StopCurrentPulse(ref currentOutput);

            long startedAt = Stopwatch.GetTimestamp();
            bool completionSignaled = false;

            while (true)
            {
                if (request.CancellationToken.IsCancellationRequested) break;

                double elapsedMs = ElapsedMilliseconds(startedAt);

                if (!completionSignaled && elapsedMs >= request.DurationMs)
                {
                    request.Completion?.TrySetResult(true);
                    completionSignaled = true;
                }

                if (elapsedMs >= request.DurationMs) break;

                // 1. Evaluate frequency & max-RMS pulse gate
                GetPlaybackSignalCore(request, elapsedMs, out int frequency, out bool audible);

                // 2. Drive PC Speaker oscillator state
                if (audible && frequency > 0)
                {
                    StartOrUpdatePulse(request.Output, frequency, request.Profile.BodyWave, ref currentOutput);
                }
                else
                {
                    StopCurrentPulse(ref currentOutput);
                }

                // 3. High-density duty-cycle step timing
                double stepMs = GetTimeUntilSignalChangeMs(request, elapsedMs, audible);
                PreciseWaitMs(stepMs, request.CancellationToken);
            }

            StopCurrentPulse(ref currentOutput);
            if (!completionSignaled) request.Completion?.TrySetResult(true);
        }

        private static double ElapsedMilliseconds(long startedAt)
        {
            long ticks = Stopwatch.GetTimestamp() - startedAt;
            return ticks * 1000.0 / Stopwatch.Frequency;
        }

        // Variable sub-millisecond to 2ms timing breaks arpeggios and produces true white/pink noise static sound.
        // Step length is now shaped per instrument family so each one has a distinct texture
        // instead of every percussion sound sharing the same duty-cycle rhythm.
        private static double GetTimeUntilSignalChangeMs(PercussionRequest request, double elapsedMs, bool isAudibleState)
        {
            double rawVel = Math.Clamp(request.Velocity / 127.0, 0.01, 1.0);
            double normVelocity = 0.20 + (0.80 * Math.Sqrt(rawVel));

            bool metalCymbal = IsMetalCymbal(request.Percussion);
            bool snare = IsSnare(request.Percussion);
            bool kickOrTom = IsKick(request.Percussion) || IsTomOrBongo(request.Percussion);

            if (isAudibleState)
            {
                uint h = unchecked((uint)(request.RandomSeed ^ ((int)(elapsedMs * 100.0) * 1664525u)));
                double jitter = (h & 0x00FFFFFF) / 16777215.0;

                // Cymbals/hi-hats need the fastest on/off switching to read as a continuous
                // hiss rather than individual clicks. Kicks/toms are presenting a single low
                // tone, not noise, so they can hold each step longer for a steadier pitch.
                double baseHoldMs = metalCymbal ? 0.22 : kickOrTom ? 1.3 : 0.7;
                return baseHoldMs * (1.0 + jitter * 0.8) * (0.8 + 0.6 * normVelocity);
            }

            // Silence gaps between noise bursts
            uint sHash = unchecked((uint)(request.RandomSeed ^ ((int)(elapsedMs * 100.0) * 1013904223u)));
            double gapJitter = (sHash & 0x00FFFFFF) / 16777215.0;

            double gapScalar = Math.Max(0.05, 1.3 - (1.0 * normVelocity));

            // Tighter gaps on snares/cymbals give a denser, crisper texture instead of an
            // audibly gappy buzz.
            if (metalCymbal || snare)
                return (0.08 + gapJitter * 0.18) * gapScalar;

            return (0.2 + gapJitter * 0.3) * gapScalar;
        }

        private static void GetPlaybackSignalCore(
            PercussionRequest request,
            double elapsedMs,
            out int frequency,
            out bool audible)
        {
            var prof = request.Profile;
            double duration = Math.Max(1.0, request.DurationMs);
            double progress = Math.Clamp(elapsedMs / duration, 0.0, 1.0);

            // Dynamic curve with a strong baseline floor (0.20 - 1.0) so soft hits never disappear
            double rawVel = Math.Clamp(request.Velocity / 127.0, 0.01, 1.0);
            double normVelocity = 0.20 + (0.80 * Math.Sqrt(rawVel));

            bool kick = IsKick(request.Percussion);
            bool tom = IsTomOrBongo(request.Percussion);
            bool snare = IsSnare(request.Percussion);
            bool click = IsClick(request.Percussion);
            bool metalCymbal = IsMetalCymbal(request.Percussion);
            bool shortCymbal = IsShortCymbal(request.Percussion);
            bool isPureTonal = prof.BodyWave == SynthWave.Square || prof.BodyWave == SynthWave.Sine;

            // ------------------------------------------------------------------
            // 1. INSTRUMENT-SHAPED ATTACK TRANSIENT
            // Every hit used to open with the exact same 1200-3200Hz click no matter what it
            // was, so a kick, a snare and a cymbal all "clicked" identically for their first
            // few ms. Each family now gets its own snap range/length so the very start of the
            // hit already carries some of that instrument's identity.
            // ------------------------------------------------------------------
            double attackSnapMs = 0.0;
            if (!isPureTonal)
            {
                double snapStartHz, snapEndHz;
                if (kick)
                {
                    // Short, low, punchy "thwack" - a tiny speaker can't move enough air for
                    // true sub-bass, so the impact is faked with a fast downward slide through
                    // the low-mid range instead of trying to hit true kick-drum pitch.
                    attackSnapMs = Math.Min(9.0 * normVelocity, duration * 0.3);
                    snapStartHz = 320.0 + 220.0 * normVelocity;
                    snapEndHz = 90.0 + 60.0 * normVelocity;
                }
                else if (tom)
                {
                    attackSnapMs = Math.Min(7.0 * normVelocity, duration * 0.3);
                    snapStartHz = 420.0 + 260.0 * normVelocity;
                    snapEndHz = 160.0 + 100.0 * normVelocity;
                }
                else if (snare || click)
                {
                    // Fast, bright crack
                    attackSnapMs = Math.Min(3.0 * normVelocity, duration * 0.35);
                    snapStartHz = 2600.0 + 900.0 * normVelocity;
                    snapEndHz = 1400.0 + 500.0 * normVelocity;
                }
                else if (metalCymbal)
                {
                    // Bright shimmer, held a touch longer than a snare crack
                    attackSnapMs = Math.Min(5.0 * normVelocity, duration * 0.3);
                    snapStartHz = 3800.0 + 1600.0 * normVelocity;
                    snapEndHz = 2200.0 + 900.0 * normVelocity;
                }
                else
                {
                    attackSnapMs = Math.Min(6.0 * normVelocity, duration * 0.3);
                    snapStartHz = 1200.0 + 600.0 * normVelocity;
                    snapEndHz = 700.0 + 300.0 * normVelocity;
                }

                if (elapsedMs < attackSnapMs && attackSnapMs > 0.01)
                {
                    double snapProgress = elapsedMs / attackSnapMs;
                    double snapSweep = 1.0 - Math.Pow(snapProgress, 0.35);
                    double snapFreq = snapEndHz + (snapStartHz - snapEndHz) * snapSweep;
                    frequency = ClampPercussionFrequency(snapFreq, 70.0, 6000.0);
                    audible = true;
                    return;
                }
            }

            // ------------------------------------------------------------------
            // 2. PURE TONAL PERCUSSIONS (currently: Laser)
            // ------------------------------------------------------------------
            if (isPureTonal)
            {
                double baseFreq = prof.BodyStartFreq;
                if (prof.DoesSweep)
                {
                    double logSweep = 1.0 - Math.Pow(progress, 0.45);
                    baseFreq = prof.BodyEndFreq + (prof.BodyStartFreq - prof.BodyEndFreq) * logSweep;
                }

                frequency = ClampPercussionFrequency(baseFreq * (0.85 + 0.35 * normVelocity), 90.0, 2200.0);

                double sustainWindow = Math.Clamp(prof.HoldRatio * normVelocity, 0.15, 0.90);
                double decayProgress = Math.Clamp((progress - sustainWindow) / Math.Max(0.01, 1.0 - sustainWindow), 0.0, 1.0);
                double keepProbability = progress < sustainWindow ? 1.0 : Math.Pow(1.0 - decayProgress, prof.DecayShape / normVelocity);

                int slot = (int)(elapsedMs / 1.5);
                uint gateHash = unchecked((uint)(request.RandomSeed ^ (slot * 2246822519u)));
                double gateRoll = ((gateHash & 0x00FFFFFF) / 16777215.0);

                audible = gateRoll < (keepProbability * 1.2) && progress < (0.98 * normVelocity);
                return;
            }

            // ------------------------------------------------------------------
            // 3. KICKS & TOMS (pitch-swept resonant bodies, past the attack)
            // ------------------------------------------------------------------
            if (kick || tom)
            {
                double bodyProgress = Math.Clamp((elapsedMs - attackSnapMs) / Math.Max(1.0, duration - attackSnapMs), 0.0, 1.0);

                int slot = (int)(elapsedMs / (kick ? 1.6 : 1.2));
                uint hash = unchecked((uint)(request.RandomSeed ^ (slot * 1664525u + 1013904223u)));
                double j = ((hash & 0x00FFFFFF) / 16777215.0);

                // Kicks stay noticeably lower than toms so the two remain distinguishable,
                // while staying inside a range a small speaker driver can actually reproduce.
                double lowFloor = kick ? (75.0 + 25.0 * normVelocity) : (150.0 + 60.0 * normVelocity);
                double lowCeil = kick ? (220.0 + 120.0 * normVelocity) : (420.0 + 160.0 * normVelocity);

                // The System Speaker driver is weak at true bass and resonates far better
                // higher up (see StartPulseDirect), so on that output the low end is
                // nearly inaudible at the frequencies above. Shift the whole window up
                // into a register the tiny driver can actually move air at. Sound Device
                // keeps the original range since real bass reproduction works there.
                if (request.Output == PercussionOutputChoice.SystemSpeaker)
                {
                    const double speakerBassLift = 140.0;
                    lowFloor += speakerBassLift;
                    lowCeil += speakerBassLift;
                }

                double hoppedFreq = lowFloor + j * (lowCeil - lowFloor);
                frequency = ClampPercussionFrequency(hoppedFreq, 60.0, 700.0);

                uint gateHash = unchecked((uint)(request.RandomSeed ^ (slot * 2246822519u)));
                double gateRoll = ((gateHash & 0x00FFFFFF) / 16777215.0);

                double keepProbability = Math.Pow(1.0 - bodyProgress, Math.Max(0.05, prof.DecayShape / (2.0 * normVelocity)));

                // On System Speaker, "not audible" means true silence (no mixed sample to fall
                // back on like Sound Device has), so a low keepProbability here reads as the
                // drum cutting out rather than just getting quieter. Give it a floor so kicks/
                // toms stay perceptibly present for their whole natural body instead of
                // flickering silent early when velocity is low.
                if (request.Output == PercussionOutputChoice.SystemSpeaker)
                    keepProbability = Math.Max(keepProbability, 0.35);

                audible = gateRoll < keepProbability && bodyProgress < (0.95 * normVelocity);
                return;
            }

            // ------------------------------------------------------------------
            // 4. NOISE PERCUSSIONS (Snares, Cymbals, Hi-Hats, Claps, Clicks)
            // ------------------------------------------------------------------
            int noiseSlot = (int)(elapsedMs / 0.8);

            uint nHash = unchecked((uint)(request.RandomSeed ^ (noiseSlot * 2654435761u)));
            double jitter = ((nHash & 0x00FFFFFF) / 16777215.0);
            // Second, independently-mixed hash stream blended into the frequency pick so it
            // doesn't trace the exact same pattern as the gate roll below - reduces the
            // "buzzy comb" artifact of driving both from one shared hash source.
            uint nHash2 = unchecked((uint)(request.RandomSeed * 2246822519u ^ (uint)(noiseSlot * 668265263u)));
            double jitter2 = ((nHash2 & 0x00FFFFFF) / 16777215.0);

            uint gateHash2 = unchecked((uint)(request.RandomSeed ^ (noiseSlot * 1013904223u)));
            double gateRoll2 = ((gateHash2 & 0x00FFFFFF) / 16777215.0);

            if (metalCymbal)
            {
                // Real cymbals have inharmonic partials (not integer-multiple overtones like a drum
                // membrane) - that clash between unrelated frequencies IS what "metallic" sounds like.
                // A single continuously-hopped frequency band (the old approach) just sounds like
                // filtered noise, not metal. Snapping between a small set of deliberately non-integer-
                // ratio partials, re-picked every slot, fakes that clash on hardware that can only
                // emit one pure tone at a time.
                double baseRegister = 2600.0 + 1400.0 * normVelocity;
                double[] partialRatios = { 1.0, 1.41, 1.89, 2.63, 3.37, 4.18 }; // inharmonic, not 1x/2x/3x
                int partialIdx = (int)((0.5 * jitter + 0.5 * jitter2) * partialRatios.Length);
                partialIdx = Math.Clamp(partialIdx, 0, partialRatios.Length - 1);
                double baseF = baseRegister * partialRatios[partialIdx];
                frequency = ClampPercussionFrequency(baseF, 1500.0, 9000.0);

                double activeDensityThreshold = 0.05 + (0.30 * (1.0 - normVelocity));
                double sustainLimit = shortCymbal ? (0.85 * normVelocity) : (0.95 * normVelocity);
                audible = gateRoll2 > activeDensityThreshold && progress < sustainLimit;
            }
            else if (click)
            {
                double baseF = 1400.0 + jitter * (2200.0 * normVelocity);
                frequency = ClampPercussionFrequency(baseF, 700.0, 4000.0);
                audible = progress < (0.55 * normVelocity);
            }
            else // Snares / Claps
            {
                double minF = 500.0 + (250.0 * normVelocity);
                double maxF = minF + (1300.0 + 1100.0 * normVelocity);
                double baseF = minF + (0.6 * jitter + 0.4 * jitter2) * (maxF - minF);
                frequency = ClampPercussionFrequency(baseF, 350.0, 3200.0);

                double activeDensityThreshold = 0.02 + (0.35 * (1.0 - normVelocity));
                audible = gateRoll2 > activeDensityThreshold && progress < (0.90 * normVelocity);
            }
        }

        private static void PreciseWaitMs(double ms, CancellationToken ct)
        {
            if (ms <= 0 || ct.IsCancellationRequested) return;

            long start = Stopwatch.GetTimestamp();
            long targetTicks = start + (long)(ms * Stopwatch.Frequency / 1000.0);
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                if (ct.IsCancellationRequested) return;
                Thread.SpinWait(10);
            }
        }

        public static void GetPlaybackSignalCore(MidiPercussion percussion, double elapsedMs, out int frequency, out bool audible, int velocity = 100)
        {
            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(percussion);
            var dummyReq = new PercussionRequest(percussion, CancellationToken.None, prof.DurationMs, prof.DurationMs, output, prof, velocity, null);
            GetPlaybackSignalCore(dummyReq, elapsedMs, out frequency, out audible);
        }

        public static int GetMidiFrameDurationMs(MidiPercussion percussion, int availableFrameMs, bool melodyAlsoPlaying)
        {
            int naturalMs = GetNaturalDurationMs(percussion);
            if (availableFrameMs <= 0) return 0;
            if (!melodyAlsoPlaying) return Math.Min(availableFrameMs, naturalMs);

            int minBodyMs = (int)Math.Ceiling(GetMinimumBodyMs(percussion));
            return Math.Clamp(Math.Min(minBodyMs, naturalMs), 1, availableFrameMs);
        }

        public static int GetNaturalDurationMs(MidiPercussion percussion) => GetProfile(percussion).DurationMs;

        private static PercussionOutputChoice GetPercussionPlaybackOutput()
        {
            return TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;
        }

        private static double GetMinimumBodyMs(MidiPercussion p)
        {
            if (IsClick(p)) return 20.0;
            if (IsKick(p)) return 60.0;
            if (IsTomOrBongo(p)) return 55.0;
            if (IsSnare(p)) return 45.0;
            if (IsShortCymbal(p)) return 35.0;
            if (IsCymbalOrLongRing(p)) return 180.0;
            if (IsTonalNonCymbal(p)) return 80.0;
            return 30.0;
        }

        private static void PlayPCMSoundAsPWM(byte[] pcmData, PercussionOutputChoice choice, SynthWave waveform)
        {
            if (pcmData == null || pcmData.Length == 0)
                return;

            // Robust peak detection: use the 99.5th percentile deviation instead of the
            // absolute max, so a single glitch/click sample can't suppress normalization,
            // while still capturing real (even narrow) transient peaks.
            var deviations = new double[pcmData.Length];
            for (int i = 0; i < pcmData.Length; i++)
                deviations[i] = Math.Abs(pcmData[i] - 128.0);

            Array.Sort(deviations);
            int idx = Math.Clamp((int)(deviations.Length * 0.995), 0, deviations.Length - 1);
            double maxDeviation = Math.Max(deviations[idx], 0.001);

            // Exact normalization factor: maps the (robust) peak audio sample to maximum range
            double normFactor = 128.0 / maxDeviation;
            normFactor = Math.Min(normFactor, 40.0); // avoid extreme gain on near-silent buffers



            if (choice == PercussionOutputChoice.SoundDevice)
            {
                // True software PWM for standard audio device
                double durationSeconds = pcmData.Length / (double)PercussionSampleRate;
                int pwmPeriodCount = Math.Max(1, (int)Math.Ceiling(durationSeconds * SoundDevicePwmCarrierHz));

                var pwm = new float[pwmPeriodCount * SoundDevicePwmSamplesPerPeriod];
                double dutyError = 0.0;

                for (int period = 0; period < pwmPeriodCount; period++)
                {
                    double t = (period + 0.5) / SoundDevicePwmCarrierHz;
                    double sourcePosition = t * PercussionSampleRate;
                    int i0 = Math.Clamp((int)Math.Floor(sourcePosition), 0, pcmData.Length - 1);
                    int i1 = Math.Min(i0 + 1, pcmData.Length - 1);
                    double frac = Math.Clamp(sourcePosition - i0, 0.0, 1.0);

                    double pcmValue = pcmData[i0] + (pcmData[i1] - pcmData[i0]) * frac;

                    // 1. Center normalize audio to -1.0 .. 1.0 using peak scale (preserves accurate decay dynamics)
                    double normAudio = ((pcmValue - 128.0) * normFactor) / 128.0;

                    // Apply extra gain boost for very short percussion (cymbals/hi-hats) whose
                    // transient energy is easily diluted by frame-averaging or PWM interpolation.
                    double durationMs = (pcmData.Length / (double)PercussionSampleRate) * 1000.0;
                    double shortSoundBoost = durationMs < 80.0
                        ? 1.0 + (80.0 - durationMs) / 80.0 * 0.85  // up to +85% extra gain for very short hits
                        : 1.0;

                    double nonLinearAudio = Math.Sign(normAudio) * Math.Pow(Math.Abs(normAudio), 0.85) * 1.7 * shortSoundBoost;
                    double dutyCycle = Math.Clamp(0.5 + (nonLinearAudio * 0.5), 0.0, 1.0);
                    double wantedOnSamples = dutyCycle * SoundDevicePwmSamplesPerPeriod + dutyError;
                    int onSamples = Math.Clamp((int)Math.Round(wantedOnSamples, MidpointRounding.AwayFromZero), 0, SoundDevicePwmSamplesPerPeriod);
                    dutyError = wantedOnSamples - onSamples;

                    int baseIndex = period * SoundDevicePwmSamplesPerPeriod;

                    for (int j = 0; j < SoundDevicePwmSamplesPerPeriod; j++)
                        pwm[baseIndex + j] = j < onSamples ? 1.0f : -1.0f;
                }

                QueueMixedSoundDevicePwmSamples(pwm);
                return;
            }

            // Hardware PWM with Peak-Transient Detection for Short Cymbals & Hi-Hats
            const int carrierHz = 18000;
            const double frameStepMs = 1000.0 / carrierHz;
            double totalDurationMs = (pcmData.Length / (double)PercussionSampleRate) * 1000.0;
            int totalFrames = Math.Max(1, (int)(totalDurationMs / frameStepMs));

            var sw = Stopwatch.StartNew();
            bool isBeeping = false;

            try
            {
                for (int frame = 0; frame < totalFrames; frame++)
                {
                    int startSample = (int)((frame / (double)totalFrames) * pcmData.Length);
                    int endSample = (int)(((frame + 1) / (double)totalFrames) * pcmData.Length);
                    startSample = Math.Clamp(startSample, 0, pcmData.Length - 1);
                    endSample = Math.Clamp(endSample, startSample + 1, pcmData.Length);

                    byte peakPcm = 128;
                    double maxAbsVal = 0.0;

                    for (int s = startSample; s < endSample; s++)
                    {
                        double val = Math.Abs((pcmData[s] - 128.0) / 128.0);
                        if (val > maxAbsVal)
                        {
                            maxAbsVal = val;
                            peakPcm = pcmData[s];
                        }
                    }

                    // Apply peak normalization to hardware beeper frames
                    double normAudio = Math.Clamp(((peakPcm - 128.0) * normFactor) / 128.0, -1.0, 1.0);

                    // Natural exponential curve allowing smooth release to 50% duty cycle at frame ends
                    double boosted = Math.Sign(normAudio) * Math.Pow(Math.Abs(normAudio), 0.85) * 1.5;
                    double dutyCycle = Math.Clamp(0.5 + (boosted * 0.495), 0.0, 1.0);

                    double activeTimeMs = frameStepMs * dutyCycle;
                    double targetActiveMs = (frame * frameStepMs) + activeTimeMs;
                    double targetFrameEndMs = (frame + 1) * frameStepMs;

                    if (dutyCycle > 0.005)
                    {
                        if (!isBeeping)
                        {
                            SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(carrierHz);
                            isBeeping = true;
                        }
                        LowCpuWaitUntil(sw, targetActiveMs);
                    }

                    if (dutyCycle < 0.995)
                    {
                        if (isBeeping)
                        {
                            SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                            isBeeping = false;
                        }
                        LowCpuWaitUntil(sw, targetFrameEndMs);
                    }
                }
            }
            finally
            {
                SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
            }
        }
        /// <summary>
        /// Hybrid precision wait used by software PWM.
        /// It yields the processor while enough time remains and busy-spins only for the final
        /// short interval. This avoids pinning a CPU core for the entire percussion duration.
        /// </summary>
        private static void LowCpuWaitUntil(Stopwatch sw, double targetMs)
        {
            const double finalSpinWindowMs = 0.06;

            while (true)
            {
                double remainingMs = targetMs - sw.Elapsed.TotalMilliseconds;
                if (remainingMs <= 0.0)
                    return;

                // For genuinely long gaps, give the scheduler most of the interval.
                if (remainingMs >= 2.0)
                {
                    int sleepMs = Math.Max(1, (int)Math.Floor(remainingMs - 1.0));
                    Thread.Sleep(sleepMs);
                    continue;
                }

                // A scheduler yield is much cheaper than burning the whole sub-ms interval.
                // Once close enough to the edge, switch to a short precision spin.
                if (remainingMs > finalSpinWindowMs)
                {
                    Thread.Yield();
                    continue;
                }

                while (sw.Elapsed.TotalMilliseconds < targetMs)
                    Thread.SpinWait(8);

                return;
            }
        }

    }
}