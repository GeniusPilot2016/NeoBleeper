// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
// ... [License header omitted for brevity, retains GNU GPL v3]

using GenerativeAI.Types;
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
    internal enum SynthWave { Sine, Square, Triangle, Noise }

    public class NoteUtility
    {
        Random fermataRnd = new Random();

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

        public static double VelocityToGain(int velocity)
        {
            int clamped = Math.Clamp(velocity, 0, 127);
            return clamped / 127.0;
        }

        public static double VelocityToDutyCycle(int velocity)
        {
            double gain = VelocityToGain(velocity);
            return Math.Clamp(0.05 + gain * 0.45, 0.01, 0.50);
        }

        public static double VelocityToDynamicGain(int velocity)
        {
            double raw = Math.Clamp(velocity / 127.0, 0.01, 1.0);
            return 0.12 + 0.88 * Math.Pow(raw, 0.85);
        }
    }

    public class NoteFrequencies
    {
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

        [System.Runtime.InteropServices.DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint milliseconds);

        private static readonly object _hardwareLock = new object();
        private static readonly object _queueLock = new object();
        private static readonly Queue<PercussionRequest> _pendingRequests = new Queue<PercussionRequest>();
        private static bool _queueWorkerRunning;

        private static long _blendNoteSynthStamp;
        private static BlendNoteVoice? _blendVoice;

        private static readonly object _blendHitLock = new object();
        private static MidiPercussion? _blendHitPercussion;
        private static double _blendHitStartMs;
        private static double _blendHitEndMs;
        private static double _blendLastSliceEndMs;

        private static PercussionRequest? _blendDrumRequest;
        private static float[]? _blendDrumSamples;
        private static double _blendSpeakerPhase;
        private static volatile bool _blendSpeakerActive;
        private static int _blendSpeakerFreq = -1;
        private static readonly bool SpeakerBlendUsesPwm = false;

        private static double NowMs() =>
            Stopwatch.GetTimestamp() * 1000.0 / Stopwatch.Frequency;

        private const int PercussionSampleRate = 44100;
        private const int SoundDevicePwmSampleRate = 192000;
        private const int SoundDevicePwmCarrierHz = 24000;
        private const int SoundDevicePwmSamplesPerPeriod =
            SoundDevicePwmSampleRate / SoundDevicePwmCarrierHz;

        private static readonly object _soundDeviceLock = new object();
        private static NAudio.Wave.WaveOut? _percussionWaveOut;
        private static NAudio.Wave.SampleProviders.MixingSampleProvider? _percussionMixer;

        private sealed class FloatArraySampleProvider : ISampleProvider
        {
            private readonly float[] _samples;
            private int _position;
            private readonly TaskCompletionSource<bool> _drained;

            public FloatArraySampleProvider(float[] samples, int sampleRate)
            {
                _samples = samples ?? throw new ArgumentNullException(nameof(samples));
                WaveFormat =
                    NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(
                        sampleRate,
                        1);

                _drained =
                    new TaskCompletionSource<bool>(
                        TaskCreationOptions.RunContinuationsAsynchronously);
            }

            public WaveFormat WaveFormat { get; }
            public Task Drained => _drained.Task;

            public int Read(float[] buffer, int offset, int count)
            {
                if (buffer == null)
                    throw new ArgumentNullException(nameof(buffer));
                if (offset < 0 || count < 0 || offset + count > buffer.Length)
                    throw new ArgumentOutOfRangeException();

                int available = Math.Max(0, _samples.Length - _position);
                int toCopy = Math.Min(available, count);

                if (toCopy > 0)
                {
                    Array.Copy(_samples, _position, buffer, offset, toCopy);
                    _position += toCopy;
                }

                if (_position >= _samples.Length)
                    _drained.TrySetResult(true);

                return toCopy;
            }

            public int Read(Span<float> buffer)
            {
                int available = Math.Max(0, _samples.Length - _position);
                int toCopy = Math.Min(available, buffer.Length);

                if (toCopy > 0)
                {
                    for (int i = 0; i < toCopy; i++)
                        buffer[i] = _samples[_position + i];

                    _position += toCopy;
                }

                if (_position >= _samples.Length)
                    _drained.TrySetResult(true);

                return toCopy;
            }
        }

        private sealed class BlendNoteVoice : ISampleProvider
        {
            public WaveFormat WaveFormat { get; } =
                NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(SoundDevicePwmSampleRate, 1);

            private volatile float _freq = 440f;
            private volatile float _targetAmp;
            private float _env;
            private double _phase;

            public void SetNote(double freqHz, int velocity)
            {
                _freq = (float)Math.Clamp(freqHz, 20.0, 12000.0);
                _targetAmp = (float)(0.28 * NoteUtility.VelocityToDynamicGain(velocity));
            }

            public void Release() => _targetAmp = 0f;

            public int Read(float[] buffer, int offset, int count)
            {
                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException();
                return Read(buffer.AsSpan(offset, count));
            }

            public int Read(Span<float> buffer)
            {
                double inc = _freq / (double)SoundDevicePwmSampleRate;
                float target = _targetAmp;
                float step = 0.30f / (0.001f * SoundDevicePwmSampleRate);

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (_env < target)
                        _env = Math.Min(target, _env + step);
                    else if (_env > target)
                        _env = Math.Max(target, _env - step);

                    if (_env <= 0f)
                    {
                        buffer[i] = 0f;
                        continue;
                    }

                    _phase += inc;
                    if (_phase >= 1.0)
                        _phase -= 1.0;

                    buffer[i] = _phase < 0.5 ? _env : -_env;
                }

                return buffer.Length;
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
            public readonly double NoteFrequencyHz;

            public PercussionRequest(
                MidiPercussion percussion,
                CancellationToken cancellationToken,
                int durationMs,
                int completionDelayMs,
                PercussionOutputChoice output,
                PercussionProfile profile,
                int velocity,
                TaskCompletionSource<bool>? completion,
                double noteFrequencyHz = 0.0)
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
                NoteFrequencyHz = noteFrequencyHz > 0.0 ? noteFrequencyHz : 0.0;
            }
        }

        private static int ClampPercussionFrequency(
            double frequency,
            double minHz = 120.0,
            double maxHz = 1200.0) =>
            (int)Math.Round(Math.Clamp(frequency, minHz, maxHz));

        private static void StartPulseDirect(
            PercussionOutputChoice outputChoice,
            int frequency,
            SynthWave waveType)
        {
            switch (outputChoice)
            {
                case PercussionOutputChoice.SystemSpeaker:
                    frequency = (int)Math.Clamp(frequency, 100.0, 9000.0);
                    SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(frequency);
                    break;
                case PercussionOutputChoice.SoundDevice:
                    frequency = (int)Math.Clamp(frequency, 20.0, 12000.0);
                    var naudioWave =
                        waveType switch
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

        private static void EnsurePercussionSoundDevice()
        {
            lock (_soundDeviceLock)
            {
                if (_percussionWaveOut != null && _percussionMixer != null)
                    return;

                try { TimeBeginPeriod(1); } catch { }

                var format = NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(SoundDevicePwmSampleRate, 1);
                _percussionMixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(format) { ReadFully = true };

                _blendVoice = new BlendNoteVoice();
                _percussionMixer.AddMixerInput(_blendVoice);

                _percussionWaveOut = new NAudio.Wave.WaveOut
                {
                    BufferMilliseconds = 20,
                    NumberOfBuffers = 3
                };

                _percussionWaveOut.Init(_percussionMixer);
                _percussionWaveOut.Play();
            }
        }

        public static void StopAllSoundDevicePercussion()
        {
            lock (_soundDeviceLock)
            {
                if (_percussionMixer != null)
                {
                    _percussionMixer.RemoveAllMixerInputs();
                    if (_blendVoice != null)
                    {
                        _blendVoice.Release();
                        _percussionMixer.AddMixerInput(_blendVoice);
                    }
                }
            }

            lock (_blendHitLock)
            {
                _blendHitPercussion = null;
                _blendHitEndMs = 0.0;
                _blendHitStartMs = 0.0;
                _blendLastSliceEndMs = 0.0;
                _blendDrumRequest = null;
                _blendDrumSamples = null;
            }

            if (_blendSpeakerActive || _blendSpeakerFreq != -1)
            {
                lock (_hardwareLock)
                {
                    try { SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep(); } catch { }
                    _blendSpeakerFreq = -1;
                }
            }
        }

        private static void QueueMixedSoundDeviceHit(PercussionRequest request)
        {
            float[] rendered = RenderPercussionSamples(request);
            byte[] pcm8 = ConvertFloatSamplesToUnsigned8BitPcm(rendered);
            PlayPCMSoundAsPWM(pcm8, PercussionOutputChoice.SoundDevice, request.Profile.BodyWave, request.Velocity);
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

        private static void PlayRenderedPercussion(PercussionRequest request)
        {
            var sw = Stopwatch.StartNew();
            float[] samples = RenderPercussionSamples(request);
            byte[] pcm8 = ConvertFloatSamplesToUnsigned8BitPcm(samples);

            if (request.Output == PercussionOutputChoice.SoundDevice)
            {
                float[] pwm = BuildSoundDevicePwmSamples(pcm8, 2.1f, request.Velocity);
                QueueMixedSoundDevicePwmSamples(pwm);
                LowCpuWaitUntil(sw, request.DurationMs);
                return;
            }

            PlayPCMSoundAsPWM(pcm8, request.Output, request.Profile.BodyWave, request.Velocity);
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
            double masterGain = 0.85 + velocity01 * 0.70;
            uint rng = unchecked((uint)request.RandomSeed);

            double lpLow = 0.0, lpMid = 0.0, lpHigh = 0.0, dcBlockX = 0.0, dcBlockY = 0.0, tonePhase = 0.0;

            bool kick = IsKick(request.Percussion);
            bool tom = IsTomOrBongo(request.Percussion);
            bool snare = IsSnare(request.Percussion);
            bool cymbal = IsMetalCymbal(request.Percussion);
            bool shaker = request.Percussion is MidiPercussion.Shaker or MidiPercussion.Maracas or MidiPercussion.Cabasa;
            bool isTonalSquare = request.Profile.BodyWave == SynthWave.Square;

            double lowCut = kick ? 120.0 : tom ? 220.0 : 600.0;
            double midCut = snare ? 2800.0 : cymbal ? 5000.0 : 2000.0;
            double highCut = cymbal ? 10000.0 : 7500.0;

            double aLow = 1.0 - Math.Exp(-2.0 * Math.PI * lowCut / PercussionSampleRate);
            double aMid = 1.0 - Math.Exp(-2.0 * Math.PI * midCut / PercussionSampleRate);
            double aHigh = 1.0 - Math.Exp(-2.0 * Math.PI * highCut / PercussionSampleRate);

            double normLow = Math.Sqrt((2.0 - aLow) / Math.Max(aLow, 1e-6));
            double normMid = Math.Sqrt((2.0 - aMid) / Math.Max(aMid, 1e-6));

            for (int i = 0; i < sampleCount; i++)
            {
                double elapsedMs = i * 1000.0 / PercussionSampleRate;
                double progress = Math.Clamp(elapsedMs / request.DurationMs, 0.0, 1.0);

                rng ^= rng << 13; rng ^= rng >> 17; rng ^= rng << 5;
                double white = ((rng & 0x00FFFFFF) / 8388607.5) - 1.0;

                lpLow += aLow * (white - lpLow);
                lpMid += aMid * (white - lpMid);
                lpHigh += aHigh * (white - lpHigh);

                double lowBand = lpLow * normLow;
                double midBand = (lpMid - lpLow) * normMid;
                double highBand = white - lpHigh;

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
                    toneVal = request.Profile.BodyWave == SynthWave.Square ? (tonePhase < 0.5 ? 0.7 : -0.7) : Math.Sin(tonePhase * 2.0 * Math.PI);
                }

                double source = kick ? toneVal * body * 1.2 + lowBand * body * 0.8 :
                                tom ? toneVal * body * 1.0 + midBand * body * 0.5 :
                                snare ? midBand * body * 1.4 + highBand * tail * 0.8 :
                                cymbal ? highBand * tail * 1.4 + midBand * tail * 0.4 :
                                shaker ? highBand * tail * 1.2 : toneVal * body * 1.0 + midBand * body * 0.8;

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
                double density = 0.5, double holdRatio = 0.15, int attackFrequency = 0,
                double attackMs = 2.0, double decayShape = 1.8, double pitchJitter = 0.04)
            {
                BodyWave = w; DoesSweep = s; BodyStartFreq = start; BodyEndFreq = end; DurationMs = dur;
                NoiseDensity = Math.Clamp(density, 0.01, 1.0);
                HoldRatio = Math.Clamp(holdRatio, 0.01, 0.95);
                AttackFrequency = attackFrequency <= 0 ? start : attackFrequency;
                AttackMs = Math.Clamp(attackMs, 0.2, 20.0);
                DecayShape = Math.Clamp(decayShape, 0.5, 5.0);
                PitchJitter = Math.Clamp(pitchJitter, 0.0, 0.25);
            }
        }

        private static PercussionProfile Noise(
            bool sweep, int start, int end, int dur,
            double density, double hold, int attackFreq, double attackMs,
            double decay, double jitter) =>
            new PercussionProfile(SynthWave.Noise, sweep, start, end, dur, density, hold, attackFreq, attackMs, decay, jitter);

        private static PercussionProfile GetProfile(MidiPercussion percussion)
        {
            return percussion switch
            {
                MidiPercussion.Laser => new PercussionProfile(SynthWave.Square, true, 800, 200, 350, holdRatio: 0.05, attackFrequency: 800, attackMs: 0.5, decayShape: 1.2, pitchJitter: 0.02),
                MidiPercussion.Whip => Noise(true, 700, 200, 180, 0.8, 0.02, 750, 0.8, 1.5, 0.1),
                MidiPercussion.ScratchPush => Noise(true, 300, 700, 140, 0.75, 0.05, 350, 1.5, 1.8, 0.15),
                MidiPercussion.ScratchPull => Noise(true, 700, 300, 140, 0.75, 0.05, 700, 1.5, 1.8, 0.15),
                MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets => Noise(false, 600, 600, 30, 0.9, 0.01, 650, 0.4, 4.5, 0.03),
                MidiPercussion.MetronomeBell => Noise(true, 800, 600, 300, 0.5, 0.3, 850, 0.6, 1.3, 0.03),
                MidiPercussion.BassDrum or MidiPercussion.KickDrum => Noise(true, 160, 55, 150, 0.95, 0.02, 180, 1.5, 2.4, 0.005),
                MidiPercussion.SideStick => Noise(false, 500, 500, 40, 0.9, 0.01, 550, 0.4, 4.0, 0.03),
                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush => Noise(false, 400, 400, 120, 0.85, 0.02, 450, 1.2, 2.8, 0.06),
                MidiPercussion.HandClap => Noise(false, 500, 500, 140, 0.85, 0.02, 550, 0.5, 2.0, 0.12),
                MidiPercussion.FloorTom2 or MidiPercussion.FloorTom1 => Noise(true, 140, 65, 200, 0.9, 0.06, 160, 2.0, 1.7, 0.01),
                MidiPercussion.HiHatClosed => Noise(false, 800, 800, 80, 0.85, 0.02, 850, 0.5, 3.0, 0.06),
                MidiPercussion.HiHatFoot => Noise(false, 750, 750, 90, 0.8, 0.02, 800, 0.6, 2.8, 0.05),
                MidiPercussion.LowTom or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom => Noise(true, 180, 85, 180, 0.9, 0.05, 200, 1.8, 1.9, 0.01),
                MidiPercussion.HiHatOpen => Noise(false, 800, 800, 450, 0.6, 0.05, 850, 1.2, 2.2, 0.08),
                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal => Noise(false, 750, 750, 700, 0.7, 0.08, 800, 1.2, 2.2, 0.1),
                MidiPercussion.HighTom => Noise(true, 220, 110, 160, 0.9, 0.05, 240, 1.5, 2.0, 0.01),
                MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 => Noise(false, 700, 700, 600, 0.6, 0.10, 750, 1.0, 2.0, 0.08),
                MidiPercussion.RideBell => Noise(false, 850, 850, 500, 0.5, 0.15, 900, 0.8, 1.8, 0.04),
                MidiPercussion.Tambourine => Noise(false, 800, 800, 180, 0.65, 0.02, 850, 0.8, 2.0, 0.15),
                MidiPercussion.Cowbell => Noise(true, 650, 580, 220, 0.55, 0.1, 700, 1.0, 1.3, 0.04),
                MidiPercussion.Vibraslap => Noise(false, 600, 600, 350, 0.5, 0.05, 650, 1.5, 1.3, 0.2),
                MidiPercussion.HighBongo => Noise(true, 500, 350, 100, 0.85, 0.05, 550, 1.2, 2.5, 0.02),
                MidiPercussion.LowBongo => Noise(true, 350, 220, 110, 0.85, 0.05, 380, 1.3, 2.4, 0.02),
                MidiPercussion.CongaDeadStroke => Noise(true, 320, 260, 70, 0.85, 0.03, 340, 1.0, 3.5, 0.02),
                MidiPercussion.Conga => Noise(true, 300, 200, 140, 0.85, 0.06, 320, 1.4, 2.0, 0.02),
                MidiPercussion.Tumba => Noise(true, 250, 160, 150, 0.85, 0.06, 270, 1.5, 1.9, 0.02),
                MidiPercussion.HighTimbale => Noise(true, 500, 350, 130, 0.8, 0.04, 550, 1.0, 2.2, 0.02),
                MidiPercussion.LowTimbale => Noise(true, 400, 280, 150, 0.8, 0.04, 440, 1.2, 2.0, 0.02),
                MidiPercussion.HighAgogo => Noise(true, 750, 680, 220, 0.55, 0.08, 800, 0.8, 1.4, 0.04),
                MidiPercussion.LowAgogo => Noise(true, 550, 500, 260, 0.55, 0.08, 600, 0.9, 1.3, 0.04),
                MidiPercussion.Cabasa => Noise(false, 800, 800, 130, 0.55, 0.02, 850, 0.5, 2.5, 0.1),
                MidiPercussion.Maracas => Noise(false, 850, 850, 100, 0.5, 0.02, 900, 0.4, 2.8, 0.1),
                MidiPercussion.WhistleShort => Noise(false, 700, 700, 160, 0.4, 0.35, 700, 3.0, 1.5, 0.02),
                MidiPercussion.WhistleLong => Noise(true, 700, 550, 500, 0.4, 0.35, 700, 5.0, 1.1, 0.02),
                MidiPercussion.GuiroShort => Noise(true, 600, 400, 90, 0.7, 0.05, 650, 1.0, 2.0, 0.05),
                MidiPercussion.GuiroLong => Noise(true, 600, 350, 350, 0.7, 0.1, 650, 2.0, 1.2, 0.05),
                MidiPercussion.Claves => Noise(false, 800, 800, 40, 0.85, 0.02, 850, 0.4, 4.0, 0.02),
                MidiPercussion.HighWoodblock => Noise(false, 750, 750, 45, 0.85, 0.02, 800, 0.4, 3.8, 0.02),
                MidiPercussion.LowWoodblock => Noise(false, 550, 550, 50, 0.85, 0.02, 600, 0.5, 3.6, 0.02),
                MidiPercussion.CuicaHigh => Noise(true, 600, 350, 220, 0.7, 0.08, 650, 2.0, 1.6, 0.05),
                MidiPercussion.CuicaLow => Noise(true, 400, 200, 260, 0.7, 0.08, 430, 2.2, 1.5, 0.05),
                MidiPercussion.TriangleMute => Noise(false, 850, 850, 60, 0.4, 0.05, 900, 0.5, 3.0, 0.03),
                MidiPercussion.TriangleOpen => Noise(false, 850, 850, 400, 0.35, 0.4, 900, 1.0, 1.0, 0.02),
                MidiPercussion.Shaker => Noise(false, 800, 800, 110, 0.55, 0.02, 850, 0.5, 2.6, 0.1),
                MidiPercussion.SleighBell => Noise(false, 850, 850, 200, 0.5, 0.1, 900, 0.6, 1.8, 0.15),
                MidiPercussion.BellTree => Noise(true, 850, 450, 450, 0.45, 0.25, 850, 1.0, 1.1, 0.06),
                MidiPercussion.SurduDeadStroke => Noise(true, 130, 90, 90, 0.9, 0.03, 140, 1.5, 3.0, 0.01),
                MidiPercussion.Surdu => Noise(true, 130, 60, 220, 0.9, 0.06, 145, 2.0, 2.0, 0.01),
                MidiPercussion.OceanDrum => Noise(false, 500, 500, 600, 0.45, 0.5, 550, 8.0, 1.0, 0.08),
                _ => Noise(false, 400, 400, 100, 0.6, 0.02, 450, 1.2, 2.5, 0.06)
            };
        }

        private static bool IsKick(MidiPercussion p) => p is MidiPercussion.KickDrum or MidiPercussion.BassDrum or MidiPercussion.Surdu or MidiPercussion.SurduDeadStroke;
        private static bool IsTomOrBongo(MidiPercussion p) => p is MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 or MidiPercussion.LowTom or MidiPercussion.LowMidTom or MidiPercussion.HighMidTom or MidiPercussion.HighTom or MidiPercussion.HighBongo or MidiPercussion.LowBongo or MidiPercussion.Conga or MidiPercussion.CongaDeadStroke or MidiPercussion.Tumba or MidiPercussion.HighTimbale or MidiPercussion.LowTimbale or MidiPercussion.CuicaHigh or MidiPercussion.CuicaLow;
        private static bool IsSnare(MidiPercussion p) => p is MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum or MidiPercussion.SnareDrumRod or MidiPercussion.SnareDrumBrush or MidiPercussion.SideStick;
        private static bool IsClick(MidiPercussion p) => p is MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick or MidiPercussion.Castanets or MidiPercussion.Claves or MidiPercussion.WoodBlock or MidiPercussion.HighWoodblock or MidiPercussion.LowWoodblock;
        private static bool IsCymbalOrLongRing(MidiPercussion p) => p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal or MidiPercussion.HiHatOpen or MidiPercussion.RideBell;
        private static bool IsShortCymbal(MidiPercussion p) => p is MidiPercussion.HiHatClosed or MidiPercussion.HiHatFoot or MidiPercussion.TriangleMute or MidiPercussion.Tambourine;
        private static bool IsMetalCymbal(MidiPercussion p) => IsCymbalOrLongRing(p) || IsShortCymbal(p) || p is MidiPercussion.SleighBell or MidiPercussion.Shaker or MidiPercussion.Cabasa or MidiPercussion.Maracas;
        private static bool IsTonalNonCymbal(MidiPercussion p) => p is MidiPercussion.Laser;

        public static void PlayPercussion(MidiPercussion p, CancellationToken ct = default, int maxMs = 5000, int velocity = 100, int? requestedDurationMs = null)
        {
            if (ct.IsCancellationRequested) return;
            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);
            int baseDuration = requestedDurationMs ?? prof.DurationMs;
            int duration = Math.Max(1, Math.Min(maxMs, baseDuration));

            EnqueuePercussion(new PercussionRequest(p, ct, duration, duration, output, prof, velocity, null));
        }

        public static Task PlayPercussionForDurationAsync(MidiPercussion p, int durationMs, CancellationToken ct = default, int velocity = 100)
        {
            return PlayPercussionForDurationAsync(p, durationMs, ct, velocity, enforceMinimumAudibleBody: true);
        }

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

        public static async Task PlayPercussionSliceImmediateAsync(
            MidiPercussion p, int sliceDurationMs, CancellationToken ct = default,
            int velocity = 100, bool enforceMinimumAudibleBody = true, double noteFrequencyHz = 0.0)
        {
            if (ct.IsCancellationRequested || sliceDurationMs <= 0) return;

            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(p);
            int audibleDurationMs = ResolveAudibleDurationMs(p, sliceDurationMs, output, enforceMinimumAudibleBody);

            var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var request = new PercussionRequest(p, ct, audibleDurationMs, sliceDurationMs, output, prof, velocity, completion, noteFrequencyHz);
            bool isNotePercussionBlendSlice = !enforceMinimumAudibleBody;

            try
            {
                await Task.Run(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    if (isNotePercussionBlendSlice)
                        PlayPercussionBlendSlice(request);
                    else
                        PlayRenderedPercussion(request);
                }, ct).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                completion.TrySetCanceled(ct);
                return;
            }

            if (ct.IsCancellationRequested)
                completion.TrySetCanceled(ct);
            else
                completion.TrySetResult(true);

            try { await completion.Task.ConfigureAwait(false); } catch (OperationCanceledException) { }
        }

        private static void ScheduleBlendNoteRelease(long stamp)
        {
            Task.Run(async () =>
            {
                await Task.Delay(3).ConfigureAwait(false);
                if (Interlocked.Read(ref _blendNoteSynthStamp) == stamp)
                    _blendVoice?.Release();
            });
        }

        private static void SetBlendSpeakerFrequency(int targetFreq, SynthWave wave, bool force = false)
        {
            lock (_hardwareLock)
            {
                if (targetFreq < 0)
                {
                    if (_blendSpeakerFreq != -1 || force)
                    {
                        SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                        _blendSpeakerFreq = -1;
                    }
                }
                else if (force || _blendSpeakerFreq != targetFreq)
                {
                    StartPulseDirect(PercussionOutputChoice.SystemSpeaker, targetFreq, wave);
                    _blendSpeakerFreq = targetFreq;
                }
            }
        }

        /// <summary>
        /// Highly boosted speaker blend tones with forced high dynamic gain floors to ensure maximum loudness.
        /// </summary>
        private static void PlaySpeakerBlendTones(
            PercussionRequest request, Stopwatch sw, PercussionRequest drumRequest,
            double sliceStartMs, double hitStartMs, int naturalMs, bool hasNote)
        {
            // FIX: Force dynamic gain to absolute maximum (1.0) to completely eliminate velocity-based faintness.
            double dyn = 1.0;

            int noteFreq = hasNote
                ? ClampPercussionFrequency(request.NoteFrequencyHz, 120.0, 8000.0)
                : -1;

            bool lowDrum = IsKick(request.Percussion) || IsTomOrBongo(request.Percussion);
            bool sharp = IsSnare(request.Percussion) || IsClick(request.Percussion);

            // FIX: Further enlarged attack windows for maximum initial punch.
            double attackWindowMs = Math.Min(
                (lowDrum ? 45.0 : sharp ? 24.0 : 30.0),
                naturalMs * 0.85);

            double startShare = lowDrum ? 0.98 : sharp ? 0.95 : 0.90;
            double endShare = 0.65;

            double shareDecayMs = Math.Min(naturalMs, 250.0);
            double noteSegMs = hasNote ? Math.Clamp(2000.0 / noteFreq, 1.2, 4.0) : 1.2;

            double drumMs = 0.0;
            double totalMs = 0.0;
            double segmentEnd = 0.0;
            bool first = true;

            try
            {
                while (sw.Elapsed.TotalMilliseconds < request.DurationMs)
                {
                    request.CancellationToken.ThrowIfCancellationRequested();
                    double elapsedMs = sw.Elapsed.TotalMilliseconds;
                    double hitElapsedMs = (sliceStartMs + elapsedMs) - hitStartMs;
                    bool drumRinging = hitElapsedMs >= 0.0 && hitElapsedMs < naturalMs;

                    int percFreq = 0;
                    bool percAudible = false;

                    if (drumRinging)
                    {
                        GetPlaybackSignalCore(drumRequest, hitElapsedMs, out percFreq, out percAudible);
                    }

                    bool inAttack = drumRinging && hitElapsedMs < attackWindowMs;
                    double share = !drumRinging ? 0.0 : inAttack ? 1.0 : startShare + (endShare - startShare) * Math.Clamp(hitElapsedMs / shareDecayMs, 0.0, 1.0);
                    bool drumTurn = percAudible && (!hasNote || drumMs <= share * totalMs);

                    int target;
                    double segLen;

                    if (drumTurn)
                    {
                        target = ClampPercussionFrequency(percFreq, lowDrum ? 180.0 : 150.0, 9000.0);
                        segLen = lowDrum ? (inAttack ? 2.5 : 4.5) : (inAttack ? 0.6 : 1.0);
                        drumMs += segLen;
                    }
                    else if (hasNote)
                    {
                        target = noteFreq;
                        segLen = noteSegMs;
                    }
                    else
                    {
                        target = -1;
                        segLen = 1.2;
                    }

                    totalMs += segLen;
                    SetBlendSpeakerFrequency(target, request.Profile.BodyWave, force: first);
                    first = false;

                    segmentEnd = Math.Max(segmentEnd, elapsedMs) + segLen;
                    LowCpuWaitUntil(sw, Math.Min(request.DurationMs, segmentEnd));
                }

                if (hasNote)
                    SetBlendSpeakerFrequency(noteFreq, request.Profile.BodyWave);
            }
            finally
            {
                if (request.CancellationToken.IsCancellationRequested)
                    SetBlendSpeakerFrequency(-1, request.Profile.BodyWave, force: true);
            }
        }

        private static float[] RenderBlendDrumForSpeaker(PercussionRequest drumRequest)
        {
            byte[] pcm8 = ConvertFloatSamplesToUnsigned8BitPcm(RenderPercussionSamples(drumRequest));
            double maxDeviation = Math.Max(FindPercentileDeviation(pcm8, 0.995), 0.001);

            // FIX: Raised normFactor ceiling from 60.0 to 80.0 for maximum possible transient force.
            double normFactor = Math.Min(128.0 / maxDeviation, 80.0);

            var drum = new float[pcm8.Length];
            for (int i = 0; i < pcm8.Length; i++)
            {
                drum[i] = (float)Math.Clamp(((pcm8[i] - 128.0) * normFactor) / 128.0, -1.0, 1.0);
            }
            return drum;
        }

        private static double SquareIntegral(double p)
        {
            p -= Math.Floor(p);
            return p <= 0.5 ? p : 1.0 - p;
        }

        private static double AverageSquare(double phase, double phaseInc)
        {
            if (phaseInc <= 0.0 || phaseInc >= 0.5)
                return phase - Math.Floor(phase) < 0.5 ? 1.0 : -1.0;
            return (SquareIntegral(phase + phaseInc) - SquareIntegral(phase)) / phaseInc;
        }

        /// <summary>
        /// Max-gain PWM speaker blend mode with supercharged carrier scaling.
        /// </summary>
        /// <summary>
        /// Maximum-amplitude PWM speaker blend method with fully maximized mix weights and hardware gain scaling.
        /// </summary>
        private static void PlaySpeakerBlendPwm(
            PercussionRequest request, Stopwatch sw, float[]? drumSamples, double drumOffsetMs, double noteHz)
        {
            const int carrierHz = 24000;
            const double frameStepMs = 1000.0 / carrierHz;

            // FIX: Maximize dynamic gain scaling floor to 1.0.
            double dyn = 1.0;

            // FIX: Pushed weights and gain multipliers to the absolute limit for maximum PC speaker output energy.
            const double noteWeight = 0.95;
            double drumWeight = 0.90;
            double gain = 2.0;

            double phaseInc = noteHz > 0.0 ? noteHz * frameStepMs / 1000.0 : 0.0;
            double phase = _blendSpeakerPhase;

            int frame = 0, lastFrame = 0, iteration = 0;
            bool isBeeping = false;

            try
            {
                while (true)
                {
                    int nowFrame = (int)(sw.Elapsed.TotalMilliseconds / frameStepMs);
                    if (nowFrame > frame) frame = nowFrame;
                    double frameStartMs = frame * frameStepMs;

                    if (frameStartMs >= request.DurationMs) break;
                    if ((++iteration & 63) == 0) request.CancellationToken.ThrowIfCancellationRequested();

                    double noteVal = 0.0;
                    if (phaseInc > 0.0)
                    {
                        phase += phaseInc * (frame - lastFrame);
                        phase -= Math.Floor(phase);
                        noteVal = AverageSquare(phase, phaseInc);
                    }
                    lastFrame = frame;

                    double drumVal = 0.0;
                    if (drumSamples != null && drumSamples.Length > 1)
                    {
                        double pos = (drumOffsetMs + frameStartMs) * PercussionSampleRate / 1000.0;
                        if (pos >= 0.0 && pos < drumSamples.Length - 1)
                        {
                            int i0 = (int)pos;
                            double frac = pos - i0;
                            drumVal = drumSamples[i0] + (drumSamples[i0 + 1] - drumSamples[i0]) * frac;
                        }
                    }

                    double mixed = noteVal * noteWeight + drumVal * drumWeight;
                    double dutyCycle = Math.Clamp(0.5 + mixed * 0.5 * gain, 0.0, 1.0);

                    if (dutyCycle > 0.005)
                    {
                        if (!isBeeping)
                        {
                            SoundRenderingEngine.SystemSpeakerBeepEngine.StartBeep(carrierHz);
                            isBeeping = true;
                        }
                        LowCpuWaitUntil(sw, frameStartMs + frameStepMs * dutyCycle);
                    }

                    if (dutyCycle < 0.995)
                    {
                        if (isBeeping)
                        {
                            SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
                            isBeeping = false;
                        }
                        LowCpuWaitUntil(sw, frameStartMs + frameStepMs);
                    }

                    frame++;
                }
            }
            finally
            {
                _blendSpeakerPhase = phase;
                SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep();
            }
        }

        private static bool TrackBlendRun(
            PercussionRequest request, int naturalMs, double sliceStartMs,
            out double hitStartMs, out PercussionRequest drumRequest)
        {
            lock (_blendHitLock)
            {
                bool sameRun = _blendHitPercussion == request.Percussion &&
                               _blendDrumRequest != null &&
                               _blendDrumRequest.Velocity == request.Velocity &&
                               sliceStartMs - _blendLastSliceEndMs < 3.0 &&
                               sliceStartMs < _blendHitEndMs;

                if (!sameRun)
                {
                    _blendHitPercussion = request.Percussion;
                    _blendHitStartMs = sliceStartMs;
                    _blendHitEndMs = sliceStartMs + naturalMs;
                    _blendDrumSamples = null;

                    _blendDrumRequest = new PercussionRequest(
                        request.Percussion, request.CancellationToken, naturalMs, naturalMs,
                        request.Output, request.Profile, request.Velocity, null);
                }

                _blendLastSliceEndMs = sliceStartMs + request.DurationMs;
                hitStartMs = _blendHitStartMs;
                drumRequest = _blendDrumRequest!;
                return !sameRun;
            }
        }

        private static void PlayPercussionBlendSlice(PercussionRequest request)
        {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                bool hasNote = request.NoteFrequencyHz > 0.0;
                double sliceStartMs = NowMs() - sw.Elapsed.TotalMilliseconds;
                int naturalMs = Math.Max(request.Profile.DurationMs, (int)Math.Ceiling(GetMinimumBodyMs(request.Percussion)));

                bool triggerDrum = TrackBlendRun(request, naturalMs, sliceStartMs, out double hitStartMs, out PercussionRequest drumRequest);

                if (request.Output == PercussionOutputChoice.SoundDevice)
                {
                    EnsurePercussionSoundDevice();
                    float[]? drumPwm = null;

                    if (triggerDrum)
                    {
                        byte[] pcm8 = ConvertFloatSamplesToUnsigned8BitPcm(RenderPercussionSamples(drumRequest));
                        drumPwm = BuildSoundDevicePwmSamples(pcm8, 2.1f, drumRequest.Velocity);
                    }

                    long stamp = Interlocked.Increment(ref _blendNoteSynthStamp);
                    if (hasNote) _blendVoice!.SetNote(request.NoteFrequencyHz, request.Velocity);
                    else _blendVoice!.Release();

                    if (drumPwm != null) QueueMixedSoundDevicePwmSamples(drumPwm);

                    try
                    {
                        request.CancellationToken.ThrowIfCancellationRequested();
                        LowCpuWaitUntil(sw, request.DurationMs);
                    }
                    finally
                    {
                        ScheduleBlendNoteRelease(stamp);
                    }
                    return;
                }

                _blendSpeakerActive = true;
                try
                {
                    if (!SpeakerBlendUsesPwm)
                    {
                        PlaySpeakerBlendTones(request, sw, drumRequest, sliceStartMs, hitStartMs, naturalMs, hasNote);
                        return;
                    }

                    if (triggerDrum)
                    {
                        float[] rendered = RenderBlendDrumForSpeaker(drumRequest);
                        lock (_blendHitLock) { _blendDrumSamples = rendered; }
                    }

                    float[]? drumSamples;
                    lock (_blendHitLock) { drumSamples = _blendDrumSamples; }

                    double noteHz = hasNote ? ClampPercussionFrequency(request.NoteFrequencyHz, 100.0, 9000.0) : 0.0;
                    PlaySpeakerBlendPwm(request, sw, drumSamples, sliceStartMs - hitStartMs, noteHz);
                }
                finally
                {
                    _blendSpeakerActive = false;
                }
            }
            catch (OperationCanceledException) { }
        }

        private static float[] BuildSoundDevicePwmSamples(byte[] pcmData, float volume, int velocity = 100)
        {
            if (pcmData == null || pcmData.Length == 0) return Array.Empty<float>();

            double maxDeviation = Math.Max(FindPercentileDeviation(pcmData, 0.995), 0.001);
            double normFactor = Math.Min(128.0 / maxDeviation, 40.0);
            const double maxDepth = 0.45;
            double depth = maxDepth;

            const double VelocityRangeDb = 30.0;
            double vel01 = Math.Clamp(velocity, 1, 127) / 127.0;
            double velocityGain = Math.Pow(10.0, -(1.0 - vel01) * VelocityRangeDb / 20.0);

            double durationSeconds = pcmData.Length / (double)PercussionSampleRate;
            int pwmPeriodCount = Math.Max(1, (int)Math.Ceiling(durationSeconds * SoundDevicePwmCarrierHz));

            var pwm = new float[pwmPeriodCount * SoundDevicePwmSamplesPerPeriod];
            double dutyError = 0.0;
            double durationMs = durationSeconds * 1000.0;
            double shortSoundBoost = durationMs < 80.0 ? 1.0 + (80.0 - durationMs) / 80.0 * 0.80 : 1.0;

            float amp = (float)Math.Clamp(volume * 0.25 * velocityGain, 0.0001, 0.68);
            double dcX = 0.0, dcY = 0.0;

            for (int period = 0; period < pwmPeriodCount; period++)
            {
                double t = (period + 0.5) / SoundDevicePwmCarrierHz;
                double sourcePosition = t * PercussionSampleRate;

                int i0 = Math.Clamp((int)Math.Floor(sourcePosition), 0, pcmData.Length - 1);
                int i1 = Math.Min(i0 + 1, pcmData.Length - 1);
                double frac = Math.Clamp(sourcePosition - i0, 0.0, 1.0);

                double pcmValue = pcmData[i0] + (pcmData[i1] - pcmData[i0]) * frac;
                double normAudio = ((pcmValue - 128.0) * normFactor) / 128.0;

                double dcBlocked = normAudio - dcX + 0.995 * dcY;
                dcX = normAudio;
                dcY = dcBlocked;

                double shape = Math.Clamp(Math.Sign(dcBlocked) * Math.Pow(Math.Abs(dcBlocked), 0.85) * shortSoundBoost, -1.0, 1.0);
                double dutyCycle = Math.Clamp(0.5 + shape * depth, 0.03, 0.97);

                double wantedOnSamples = dutyCycle * SoundDevicePwmSamplesPerPeriod + dutyError;
                int onSamples = Math.Clamp((int)Math.Round(wantedOnSamples, MidpointRounding.AwayFromZero), 0, SoundDevicePwmSamplesPerPeriod);
                dutyError = wantedOnSamples - onSamples;

                int baseIndex = period * SoundDevicePwmSamplesPerPeriod;
                for (int j = 0; j < SoundDevicePwmSamplesPerPeriod; j++)
                {
                    pwm[baseIndex + j] = (j < onSamples ? 1.0f : -1.0f) * amp;
                }
            }

            int fadeSamples = Math.Min((int)(0.001 * SoundDevicePwmSampleRate), pwm.Length / 2);
            for (int i = 0; i < fadeSamples; i++)
            {
                float g = (float)i / fadeSamples;
                pwm[i] *= g;
                pwm[pwm.Length - 1 - i] *= g;
            }

            return pwm;
        }

        public static Task PlayPercussionSliceAsync(
            MidiPercussion p, int sliceDurationMs, CancellationToken ct, int velocity, bool enforceMinimumAudibleBody, double noteFrequencyHz = 0.0)
        {
            return PlayPercussionSliceImmediateAsync(p, sliceDurationMs, ct, velocity, enforceMinimumAudibleBody, noteFrequencyHz);
        }

        private static int ResolveAudibleDurationMs(MidiPercussion p, int requestedDurationMs, PercussionOutputChoice output, bool enforceMinimumAudibleBody)
        {
            if (output == PercussionOutputChoice.SoundDevice || !enforceMinimumAudibleBody)
                return Math.Max(1, requestedDurationMs);

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

                    PlayRenderedPercussion(request);

                    if (request.Completion != null)
                    {
                        if (request.CancellationToken.IsCancellationRequested)
                            request.Completion.TrySetCanceled(request.CancellationToken);
                        else
                            request.Completion.TrySetResult(true);
                    }
                }
            }
            finally
            {
                StopCurrentPulse(ref currentOutput);
                lock (_queueLock) { _queueWorkerRunning = false; }
            }
        }

        private static void GetPlaybackSignalCore(
            PercussionRequest request, double elapsedMs, out int frequency, out bool audible)
        {
            var prof = request.Profile;
            double duration = Math.Max(1.0, request.DurationMs);
            double progress = Math.Clamp(elapsedMs / duration, 0.0, 1.0);
            double normVelocity = NoteUtility.VelocityToDynamicGain(request.Velocity);

            bool kick = IsKick(request.Percussion);
            bool tom = IsTomOrBongo(request.Percussion);
            bool snare = IsSnare(request.Percussion);
            bool click = IsClick(request.Percussion);
            bool metalCymbal = IsMetalCymbal(request.Percussion);
            bool shortCymbal = IsShortCymbal(request.Percussion);

            bool isPureTonal = prof.BodyWave == SynthWave.Square || prof.BodyWave == SynthWave.Sine;

            double attackSnapMs = 0.0;
            if (!isPureTonal)
            {
                double snapStartHz, snapEndHz;
                if (kick)
                {
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
                    attackSnapMs = Math.Min(3.0 * normVelocity, duration * 0.35);
                    snapStartHz = 2600.0 + 900.0 * normVelocity;
                    snapEndHz = 1400.0 + 500.0 * normVelocity;
                }
                else if (metalCymbal)
                {
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
                    double snapFreq = snapEndHz + (snapStartHz - snapEndHz) * (1.0 - Math.Pow(snapProgress, 0.35));
                    frequency = ClampPercussionFrequency(snapFreq, 70.0, 6000.0);
                    audible = true;
                    return;
                }
            }

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
                double gateRoll = (gateHash & 0x00FFFFFF) / 16777215.0;

                audible = gateRoll < (keepProbability * 1.2) && progress < (0.98 * normVelocity);
                return;
            }

            if (kick || tom)
            {
                double bodyProgress = Math.Clamp((elapsedMs - attackSnapMs) / Math.Max(1.0, duration - attackSnapMs), 0.0, 1.0);
                int slot = (int)(elapsedMs / (kick ? 1.6 : 1.2));
                uint hash = unchecked((uint)(request.RandomSeed ^ (slot * 1664525u + 1013904223u)));
                double j = (hash & 0x00FFFFFF) / 16777215.0;

                double lowFloor = kick ? 75.0 + 25.0 * normVelocity : 150.0 + 60.0 * normVelocity;
                double lowCeil = kick ? 220.0 + 120.0 * normVelocity : 420.0 + 160.0 * normVelocity;

                if (request.Output == PercussionOutputChoice.SystemSpeaker)
                {
                    lowFloor += 140.0;
                    lowCeil += 140.0;
                }

                double hoppedFreq = lowFloor + j * (lowCeil - lowFloor);
                frequency = ClampPercussionFrequency(hoppedFreq, 60.0, 700.0);

                uint gateHash = unchecked((uint)(request.RandomSeed ^ (slot * 2246822519u)));
                double gateRoll = (gateHash & 0x00FFFFFF) / 16777215.0;
                double keepProbability = Math.Pow(1.0 - bodyProgress, Math.Max(0.05, prof.DecayShape / (2.0 * normVelocity)));

                if (request.Output == PercussionOutputChoice.SystemSpeaker)
                {
                    keepProbability = Math.Max(keepProbability, 0.35);
                }

                audible = gateRoll < keepProbability && bodyProgress < (0.95 * normVelocity);
                return;
            }

            int noiseSlot = (int)(elapsedMs / 0.8);
            uint nHash = unchecked((uint)(request.RandomSeed ^ (noiseSlot * 2654435761u)));
            double jitter = (nHash & 0x00FFFFFF) / 16777215.0;
            uint nHash2 = unchecked((uint)(request.RandomSeed * 2246822519u ^ (uint)(noiseSlot * 668265263u)));
            double jitter2 = (nHash2 & 0x00FFFFFF) / 16777215.0;
            uint gateHash2 = unchecked((uint)(request.RandomSeed ^ (noiseSlot * 1013904223u)));
            double gateRoll2 = (gateHash2 & 0x00FFFFFF) / 16777215.0;

            if (metalCymbal)
            {
                double baseRegister = 2600.0 + 1400.0 * normVelocity;
                double[] partialRatios = { 1.0, 1.41, 1.89, 2.63, 3.37, 4.18 };
                int partialIdx = Math.Clamp((int)((0.5 * jitter + 0.5 * jitter2) * partialRatios.Length), 0, partialRatios.Length - 1);
                double baseF = baseRegister * partialRatios[partialIdx];

                frequency = ClampPercussionFrequency(baseF, 1500.0, 9000.0);
                double activeDensityThreshold = 0.05 + (0.30 * (1.0 - normVelocity));
                double sustainLimit = shortCymbal ? 0.85 * normVelocity : 0.95 * normVelocity;

                audible = gateRoll2 > activeDensityThreshold && progress < sustainLimit;
            }
            else if (click)
            {
                double baseF = 1400.0 + jitter * (2200.0 * normVelocity);
                frequency = ClampPercussionFrequency(baseF, 700.0, 4000.0);
                audible = progress < (0.55 * normVelocity);
            }
            else
            {
                double minF = 500.0 + (250.0 * normVelocity);
                double maxF = minF + (1300.0 + 1100.0 * normVelocity);
                double baseF = minF + (0.6 * jitter + 0.4 * jitter2) * (maxF - minF);

                frequency = ClampPercussionFrequency(baseF, 350.0, 3200.0);
                double activeDensityThreshold = 0.02 + (0.35 * (1.0 - normVelocity));
                audible = gateRoll2 > activeDensityThreshold && progress < (0.90 * normVelocity);
            }
        }

        public static void GetPlaybackSignalCore(
            MidiPercussion percussion, double elapsedMs, out int frequency, out bool audible, int velocity = 100, int? durationMs = null)
        {
            var output = GetPercussionPlaybackOutput();
            var prof = GetProfile(percussion);
            var dummyReq = new PercussionRequest(
                percussion, CancellationToken.None, Math.Max(1, durationMs ?? prof.DurationMs),
                Math.Max(1, durationMs ?? prof.DurationMs), output, prof, velocity, null);

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

        private static PercussionOutputChoice GetPercussionPlaybackOutput() =>
            TemporarySettings.CreatingSounds.createBeepWithSoundDevice
                ? PercussionOutputChoice.SoundDevice
                : PercussionOutputChoice.SystemSpeaker;

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

        private static double FindPercentileDeviation(byte[] pcmData, double percentile)
        {
            int n = pcmData.Length;
            if (n == 0) return 0.0;

            var deviations = new double[n];
            for (int i = 0; i < n; i++)
            {
                deviations[i] = Math.Abs(pcmData[i] - 128.0);
            }

            int targetIdx = Math.Clamp((int)(n * percentile), 0, n - 1);
            return QuickSelect(deviations, 0, n - 1, targetIdx);
        }

        private static double QuickSelect(double[] arr, int lo, int hi, int k)
        {
            while (lo < hi)
            {
                double pivot = arr[(lo + hi) / 2];
                int i = lo, j = hi;

                while (i <= j)
                {
                    while (arr[i] < pivot) i++;
                    while (arr[j] > pivot) j--;
                    if (i <= j)
                    {
                        (arr[i], arr[j]) = (arr[j], arr[i]);
                        i++; j--;
                    }
                }

                if (k <= j) hi = j;
                else if (k >= i) lo = i;
                else break;
            }
            return arr[k];
        }

        private static void PlayPCMSoundAsPWM(
            byte[] pcmData, PercussionOutputChoice choice, SynthWave waveform, int velocity = 100)
        {
            if (pcmData == null || pcmData.Length == 0) return;

            float volume = 2.1f;
            double maxDeviation = Math.Max(FindPercentileDeviation(pcmData, 0.995), 0.001);
            double normFactor = Math.Min(128.0 / maxDeviation, 40.0);

            if (choice == PercussionOutputChoice.SoundDevice)
            {
                var sw2 = Stopwatch.StartNew();
                float[] pwm = BuildSoundDevicePwmSamples(pcmData, volume, velocity);
                QueueMixedSoundDevicePwmSamples(pwm);
                LowCpuWaitUntil(sw2, pwm.Length / (double)SoundDevicePwmSampleRate * 1000.0);
                return;
            }

            const int carrierHz = 24000;
            const double frameStepMs = 1000.0 / carrierHz;
            double totalDurationMs = (pcmData.Length / (double)PercussionSampleRate) * 1000.0;
            int totalFrames = Math.Max(1, (int)(totalDurationMs / frameStepMs));

            var sw = Stopwatch.StartNew();
            bool isBeeping = false;

            try
            {
                for (int frame = 0; frame < totalFrames; frame++)
                {
                    int startSample = Math.Clamp((int)((frame / (double)totalFrames) * pcmData.Length), 0, pcmData.Length - 1);
                    int endSample = Math.Clamp((int)(((frame + 1) / (double)totalFrames) * pcmData.Length), startSample + 1, pcmData.Length);

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

                    double normAudio = Math.Clamp(((peakPcm - 128.0) * normFactor) / 128.0, -1.0, 1.0);
                    double boosted = Math.Sign(normAudio) * Math.Pow(Math.Abs(normAudio), 0.85) * 0.85 * volume * Math.Pow(Math.Clamp(velocity, 1, 127) / 127.0, 0.85);
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

        private static void LowCpuWaitUntil(Stopwatch sw, double targetMs)
        {
            const double finalSpinWindowMs = 0.06;
            while (true)
            {
                double remainingMs = targetMs - sw.Elapsed.TotalMilliseconds;
                if (remainingMs <= 0.0) return;
                if (remainingMs >= 2.0)
                {
                    int sleepMs = Math.Max(1, (int)Math.Floor(remainingMs - 1.0));
                    Thread.Sleep(sleepMs);
                    continue;
                }
                if (remainingMs > finalSpinWindowMs)
                {
                    Thread.Yield();
                    continue;
                }
                while (sw.Elapsed.TotalMilliseconds < targetMs)
                {
                    Thread.SpinWait(8);
                }
                return;
            }
        }
    }
}