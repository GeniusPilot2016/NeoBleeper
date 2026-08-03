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

        // The output engines are monophonic. Queue attacks in order and let the currently
        // sounding tail yield only after its attack has been audible. This prevents dropped
        // hits without time-slicing several tails, which changes the percussion character.
        private const double RetriggerGapMs = 0.35;
        private const double QueuePollMs = 1.0;

        private static readonly object _hardwareLock = new object();
        private static readonly object _queueLock = new object();
        private static readonly System.Collections.Generic.Queue<PercussionRequest> _pendingRequests =
            new System.Collections.Generic.Queue<PercussionRequest>();
        private static bool _queueWorkerRunning;

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
            public readonly System.Threading.Tasks.TaskCompletionSource<bool>? Completion;

            public PercussionRequest(
                MidiPercussion percussion,
                System.Threading.CancellationToken cancellationToken,
                int durationMs,
                int completionDelayMs,
                PercussionOutputChoice output,
                PercussionProfile profile,
                System.Threading.Tasks.TaskCompletionSource<bool>? completion)
            {
                Percussion = percussion;
                CancellationToken = cancellationToken;
                DurationMs = System.Math.Max(1, durationMs);
                CompletionDelayMs = System.Math.Max(1, completionDelayMs);
                Output = output;
                Profile = profile;
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

            public PercussionProfile(SynthWave w, bool s, int start, int end, int dur, double density = 0.5, double holdRatio = 0.15)
            {
                BodyWave = w;
                DoesSweep = s;
                BodyStartFreq = start;
                BodyEndFreq = end;
                DurationMs = dur;
                NoiseDensity = System.Math.Clamp(density, 0.01, 1.0);
                HoldRatio = System.Math.Clamp(holdRatio, 0.01, 0.95);
            }
        }

        private static PercussionProfile GetProfile(MidiPercussion percussion, PercussionOutputChoice output)
        {
            SynthWave drumBodyWave = output == PercussionOutputChoice.SoundDevice ? SynthWave.Triangle : SynthWave.Square;

            return percussion switch
            {
                MidiPercussion.KickDrum or MidiPercussion.BassDrum => new PercussionProfile(drumBodyWave, true, 160, 55, 45, holdRatio: 0.08),
                MidiPercussion.HighTom => new PercussionProfile(drumBodyWave, true, 280, 180, 55),
                MidiPercussion.LowTom or MidiPercussion.HighMidTom or MidiPercussion.LowMidTom => new PercussionProfile(drumBodyWave, true, 210, 130, 65),
                MidiPercussion.FloorTom1 or MidiPercussion.FloorTom2 => new PercussionProfile(drumBodyWave, true, 150, 90, 80),

                MidiPercussion.SideStick or MidiPercussion.StickClick or MidiPercussion.SquareClick or MidiPercussion.MetronomeClick =>
                    new PercussionProfile(SynthWave.Noise, false, 7200, 7200, 18, density: 0.32, holdRatio: 0.04),

                MidiPercussion.MetronomeBell => new PercussionProfile(SynthWave.Triangle, true, 1900, 1450, 28, holdRatio: 0.04),
                MidiPercussion.Claves or MidiPercussion.Castanets => new PercussionProfile(SynthWave.Triangle, false, 2400, 2400, 30, holdRatio: 0.10),

                MidiPercussion.SnareDrum or MidiPercussion.ElectricSnareDrum => new PercussionProfile(SynthWave.Noise, false, 3200, 3200, 75, density: 0.90),
                MidiPercussion.SnareDrumRod => new PercussionProfile(SynthWave.Noise, false, 2800, 2800, 60, density: 0.70),
                MidiPercussion.SnareDrumBrush => new PercussionProfile(SynthWave.Noise, false, 2400, 2400, 140, density: 0.45, holdRatio: 0.15),

                MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2 or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2 or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal or MidiPercussion.RideBell =>
                    new PercussionProfile(SynthWave.Noise, false, 5500, 5500, 850, density: 0.45, holdRatio: 0.04),

                MidiPercussion.HiHatClosed => new PercussionProfile(SynthWave.Noise, false, 7500, 7500, 25, density: 0.80, holdRatio: 0.12),
                MidiPercussion.HiHatOpen => new PercussionProfile(SynthWave.Noise, false, 7000, 7000, 300, density: 0.30, holdRatio: 0.05),
                MidiPercussion.HiHatFoot => new PercussionProfile(SynthWave.Noise, false, 2800, 2800, 35, density: 0.60),

                _ => new PercussionProfile(SynthWave.Noise, false, 2200, 2200, 80, density: 0.65, holdRatio: 0.15)
            };
        }

        private static bool IsCymbalOrLongRing(MidiPercussion p)
        {
            return p is MidiPercussion.CrashCymbal or MidiPercussion.CrashCymbal2
                or MidiPercussion.RideCymbal or MidiPercussion.RideCymbal2
                or MidiPercussion.ChinaCymbal or MidiPercussion.SplashCymbal
                or MidiPercussion.RideBell or MidiPercussion.HiHatOpen;
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
                : System.Math.Max(8, System.Math.Min(maxMs, prof.DurationMs));

            EnqueuePercussion(new PercussionRequest(p, ct, duration, duration, output, prof, null));
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

                GetPlaybackSignal(request, elapsedMs, out int frequency, out SynthWave wave);
                if (!started || frequency != lastFrequency || wave != lastWave)
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
            out SynthWave wave)
        {
            var prof = request.Profile;

            if (UsesShortNoiseTransient(request))
            {
                frequency = prof.BodyStartFreq;
                wave = request.Output == PercussionOutputChoice.SoundDevice ? SynthWave.Noise : SynthWave.Square;
                return;
            }

            if (prof.BodyWave == SynthWave.Noise)
            {
                // Both output paths use the sound-device/profile frequency values. Only the
                // waveform differs because the PC speaker cannot synthesize white noise.
                double sampleFreq = prof.BodyStartFreq;
                double altFreq = System.Math.Max(37.0, sampleFreq * 0.68);
                int phase = (int)(elapsedMs / 6.0);
                frequency = (int)(((phase & 1) == 0) ? sampleFreq : altFreq);
                wave = request.Output == PercussionOutputChoice.SoundDevice ? SynthWave.Noise : SynthWave.Square;
                return;
            }

            if (prof.DoesSweep)
            {
                int steps = System.Math.Clamp(request.DurationMs / 6, 2, 8);
                double stepMs = (double)request.DurationMs / steps;
                int step = System.Math.Clamp((int)(elapsedMs / stepMs), 0, steps - 1);
                double progress = (double)step / (steps - 1);
                frequency = (int)(prof.BodyStartFreq - ((prof.BodyStartFreq - prof.BodyEndFreq) * progress));
                wave = prof.BodyWave;
                return;
            }

            frequency = prof.BodyStartFreq;
            wave = prof.BodyWave;
        }

        private static double GetTimeUntilSignalChangeMs(PercussionRequest request, double elapsedMs)
        {
            if (UsesShortNoiseTransient(request))
                return request.DurationMs - elapsedMs;

            if (request.Profile.BodyWave == SynthWave.Noise)
            {
                double remainder = elapsedMs % 6.0;
                return remainder < 0.0001 ? 6.0 : 6.0 - remainder;
            }

            if (request.Profile.DoesSweep)
            {
                int steps = System.Math.Clamp(request.DurationMs / 6, 2, 8);
                double stepMs = (double)request.DurationMs / steps;
                double remainder = elapsedMs % stepMs;
                return remainder < 0.0001 ? stepMs : stepMs - remainder;
            }

            return request.DurationMs - elapsedMs;
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