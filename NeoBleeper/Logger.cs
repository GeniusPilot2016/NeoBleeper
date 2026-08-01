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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NeoBleeper
{
    public static class Logger
    {
        private readonly struct LogEntry
        {
            public LogEntry(long timestamp, string message, LogTypes type)
            {
                Timestamp = timestamp;
                Message = message;
                Type = type;
            }

            public long Timestamp { get; }
            public string Message { get; }
            public LogTypes Type { get; }
        }

        // Log() deliberately performs no file I/O and no kernel wake-up.
        // The dedicated writer polls this lock-free queue in short intervals.
        private static readonly ConcurrentQueue<LogEntry> _pendingLogs = new();
        private static readonly AutoResetEvent _shutdownEvent = new(false);
        private static readonly Thread _writerThread;
        private static readonly string _logPath;
        private static readonly DateTime _clockBaseTime;
        private static readonly long _clockBaseTimestamp;

        private const int PollIntervalMilliseconds = 10;
        private const int FlushIntervalMilliseconds = 50;
        private const int MaximumBatchSize = 4096;

        // Debug output is produced by the background writer, never by Log(),
        // so an attached debugger does not block timing-sensitive callers.
        private static volatile bool _mirrorEntriesToDebugOutput = true;

        /// <summary>
        /// Enables or disables mirroring formatted entries to the debugger output.
        /// File logging is always enabled.
        /// </summary>
        public static bool MirrorEntriesToDebugOutput
        {
            get => _mirrorEntriesToDebugOutput;
            set => _mirrorEntriesToDebugOutput = value;
        }
        private static readonly Lazy<(Regex Regex, string Replacement)[]> _maskRules =
            new(CreateMaskRules, LazyThreadSafetyMode.ExecutionAndPublication);

        private static int _shutdownStarted;

        static Logger()
        {
            _clockBaseTimestamp = Stopwatch.GetTimestamp();
            _clockBaseTime = DateTime.Now;

            string exePath = AppContext.BaseDirectory
                ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                ?? ".";

            _logPath = Path.Combine(exePath, "DebugLog.txt");
            _writerThread = new Thread(WriterLoop)
            {
                IsBackground = true,
                Name = "NeoBleeper log writer",
                Priority = ThreadPriority.BelowNormal
            };
            _writerThread.Start();

            AppDomain.CurrentDomain.ProcessExit += static (_, _) => Shutdown();
        }

        private static string BuildHeader()
        {
            string logText = string.Empty;

            logText += "\r\n  _   _            ____  _                           \r\n" +
                " | \\ | |          |  _ \\| |                          \r\n" +
                " |  \\| | ___  ___ | |_) | | ___  ___ _ __   ___ _ __ \r\n" +
                " | . ` |/ _ \\/ _ \\|  _ <| |/ _ \\/ _ \\ '_ \\ / _ \\ '__|\r\n" +
                " | |\\  |  __/ (_) | |_) | |  __/  __/ |_) |  __/ |   \r\n" +
                " |_| \\_|\\___|\\___/|____/|_|\\___|\\___| .__/ \\___|_|   \r\n" +
                "                                    | |              \r\n" +
                "                                    |_|              \r\n";
            logText += "\nFrom Something Unreal to Open Sound – Reviving the Legacy, One Note at a Time. \r\n";
            logText += "\nhttps://github.com/GeniusPilot2016/NeoBleeper \r\n\n";
            logText += $"NeoBleeper Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}\r\n";
            logText += GetInformations.GetSystemInfo();
            string[] funFacts = new string[]
            {
                "The system speaker was introduced in 1981 as part of the original IBM PC.",
                "The system speaker can only play one note at a time (monophonic).",
                "Early PC games used the system speaker to create iconic 8-bit soundtracks.",
                "The speaker's sound is controlled by sending specific frequencies to the hardware.",
                "Despite being largely obsolete, the system speaker is still used for diagnostic beeps in modern PCs.",
                "Music and mathematics are deeply connected through harmonic frequencies.",
                "The oldest known musical instrument is over 40,000 years old.",
                "Music is often called the 'universal language' for its emotional resonance.",
                "Early 'beep music' paved the way for modern chiptunes.",
                "The limitations of early sound hardware inspired the development of advanced sound cards.",
                "The system speaker generates sound using square waves, giving it a distinct tone.",
                "Some early software used the system speaker to play Morse code for communication.",
                "Beep codes emitted by the system speaker help diagnose hardware issues during boot.",
                "Hobbyists have created entire songs using sequences of system speaker beeps.",
                "Robbi-985 (aka SomethingUnreal) was a pioneer in composing music for the IBM PC's system speaker and he's often known for his work which he made music from Windows XP and 98 sounds in 2008, which can be found on his YouTube channel and linked here: https://www.youtube.com/watch?v=dsU3B0W3TMs",
                "Robbi-985 (aka SomethingUnreal) developed programs called 'BaWaMI (Basic Waveform MIDI Software Synthesizer)' and 'Bleeper Music Maker' to compose music for the system speaker.",
                "Robbi-985 (aka SomethingUnreal) created a unique style of music using only the system speaker, showcasing its potential beyond simple beeps.",
                "Robbi-985 (aka SomethingUnreal) was added realistic percussion sound effects using system speaker in his 'BaWaMI (Basic Waveform MIDI Software Synthesizer)' program, which is linked here: https://www.youtube.com/watch?v=iScYrXE76gw",
                "This program is inspired by Robbi-985's (aka SomethingUnreal's) Bleeper Music Maker, which is abandoned in 2011 due to changes in beep.sys in Windows 7 and later.",
                "Shiru8bit is a modern composer who creates music using the system speaker, continuing the legacy of early beep music.",
                "Shiru8bit released an album titled 'System Beeps' using the DOS program, which uses the system speaker and developed by himself. The album can be found here: https://shiru8bit.bandcamp.com/album/system-beeps",
                "Listening to music stimulates almost every part of the brain.",
                "The tempo of music can influence your heart rate and mood.",
                "Astronauts play music in space to boost morale and stay connected to Earth.",
                "The modern 'chiptune' genre mimics retro gaming sounds from early hardware.",
                "The note 'A' above middle C is universally tuned to 440 Hz.",
                "The system speaker operates at voltages as low as 5V, making it one of the most power-efficient audio devices ever created.",
                "In the 1980s, programmers discovered they could create polyphonic-sounding music by rapidly switching between different frequencies on the system speaker.",
                "The system speaker was originally intended only for error notifications, not music playback.",
                "Some vintage games like 'Stunts' and early 'Commander Keen' titles pushed the system speaker to its absolute limits with complex musical arrangements.",
                "The system speaker's frequency range is typically between 100 Hz and 10 kHz, far narrower than human hearing.",
                "The first computer-generated music was created in 1957 at Bell Labs using an IBM 704 computer.",
                "Chiptune artists often use hardware limitations as creative constraints, similar to how poets use meter and rhyme.",
                "The Commodore 64's SID chip could produce three-voice polyphony and became legendary in the chiptune community.",
                "Modern DAWs can simulate the exact sound characteristics of vintage computer speakers and sound chips.",
                "Some musicians today intentionally use 1-bit audio (like system speaker output) as an artistic choice for its raw, digital aesthetic.",
                "The system speaker's simple design has made it a favorite among hobbyists and retro computing enthusiasts for DIY projects.",
                "The system speaker is often overlooked in favor of more advanced audio hardware, but it remains a nostalgic symbol of early personal computing.",
                "The system speaker can produce a surprisingly wide range of sounds, from simple beeps to complex melodies, despite its basic design.",
                "The system speaker's sound is generated by rapidly turning the speaker on and off at specific frequencies, creating square waveforms.",
                "The system speaker was a key feature in early IBM PCs, helping to establish the foundation for computer audio.",
                "The system speaker's legacy lives on in modern computing, where its influence can be seen in the design of sound cards and audio software.",
                "The system speaker is a testament to the ingenuity of early computer engineers, who found ways to create engaging audio experiences with limited resources.",
                "The system speaker's distinctive sound has become an iconic part of computing history, evoking nostalgia for the early days of personal computers.",
                "The system speaker's simplicity has made it a popular choice for educational purposes, helping students understand the basics of sound generation and audio programming.",
                "The system speaker's sound can be modified by changing the duty cycle of the square wave, allowing for different timbres and effects.",
                "SomethingUnreal's BaWaMI is a MIDI synthesizer that allows users to create music using the system speaker, showcasing the creative potential of this humble hardware component.",
                "NeoBleeper continues the tradition of beep music by enabling users to compose tunes for the system speaker using modern AI technology.",
                "NeoBleeper is world's first AI-enabled music creation software for the system speaker, bridging the gap between retro computing and cutting-edge AI advancements.",
                "NeoBleeper's development was inspired by the pioneering work of Robbi-985 (aka SomethingUnreal) and other early beep music composers, highlighting the enduring appeal of system speaker music.",
                "NeoBleeper aims to revive interest in the system speaker by making it accessible to a new generation of musicians and programmers.",
                "NeoBleeper's AI capabilities allow users to generate complex musical arrangements for the system speaker, pushing the boundaries of what this classic hardware can achieve.",
                "The system speaker's enduring legacy is a testament to the creativity and innovation of early computer enthusiasts, whose work continues to inspire new generations of musicians and programmers.",
                "NeoBleeper's fusion of AI technology with the system speaker represents a unique blend of past and future, celebrating the rich history of computer audio while embracing the possibilities of modern innovation.",
                "NeoBleeper not only pays homage to the pioneers of beep music but also paves the way for future explorations in the realm of system speaker compositions.",
                "The system speaker is sometimes called the 'PC beeper' or 'internal buzzer' in technical documentation.",
                "Some BIOS setups allow users to customize the system speaker's beep patterns for different events.",
                "The system speaker can be used to play simple tunes in BASIC using the 'SOUND' command.",
                "In some embedded systems, the system speaker is used for both audio feedback and as a simple alarm.",
                "The system speaker is immune to most software-based audio driver failures, making it reliable for critical alerts.",
                "Certain Linux distributions still use the system speaker for terminal bell notifications.",
                "The system speaker's output can be captured and analyzed using an oscilloscope to study square wave properties.",
                "Some modern motherboards omit the system speaker entirely, but enthusiasts often add their own for retro compatibility.",
                "The system speaker can be programmed directly via I/O ports, bypassing the operating system.",
                "In early laptops, the system speaker was sometimes replaced by a piezoelectric buzzer to save space.",
                "The system speaker's sound can be heard even when the main audio system is muted or disabled.",
                "Some hackers have used the system speaker to transmit data acoustically between computers.",
                "The system speaker is often used in hardware stress tests to indicate progress or errors.",
                "The system speaker's simple design makes it ideal for teaching basic electronics and programming concepts.",
                "In some old arcade machines, a similar speaker was used for game sound effects before dedicated sound chips became common.",
                "The system speaker can be used to play simple melodies in DOS using the 'BEEP' command.",
                "Some early virus programs used the system speaker to play warning tunes or sound effects.",
                "The system speaker is one of the few components that can operate without any drivers or operating system support.",
                "In some server rooms, the system speaker is used to alert technicians of hardware failures even when remote monitoring is unavailable.",
                "The system speaker's square wave output is ideal for generating simple digital signals for timing experiments.",
                "Some retro enthusiasts have created adapters to connect the system speaker output to external amplifiers for louder sound.",
                "The system speaker can be used to play sound effects in text-based adventure games, adding atmosphere to gameplay.",
                "In some industrial PCs, the system speaker is used for process alarms and status notifications.",
                "The system speaker is sometimes used in microcontroller projects to demonstrate basic sound synthesis.",
                "The system speaker's legacy continues in modern embedded systems as a simple, reliable alert mechanism.",
                "Some old demo scene productions used the system speaker for synchronized music and graphics effects.",
                "The system speaker can be used to play simple ringtones or notification sounds in custom operating systems.",
                "The system speaker is often the first sound device to work after a fresh OS installation, before drivers are loaded.",
                "Some educational kits include a system speaker to teach students about binary signals and frequency.",
                "The system speaker's beep is still used in some BIOSes to indicate successful POST (Power-On Self-Test).",
                "The Intel 8253/8254 Programmable Interval Timer (PIT) chip was used to control the system speaker's frequency in early PCs.",
                "The system speaker uses Port 61h and the PIT's Channel 2 to generate sound on x86 architecture.",
                "Some clever programmers created speech synthesis on the system speaker using pulse-width modulation techniques.",
                "The game 'Space Quest' by Sierra used the system speaker to create atmospheric sound effects before sound cards were common.",
                "Windows NT-based systems virtualized system speaker access through the beep.sys driver, changing how programs could use it.",
                "The system speaker's maximum loudness is typically around 85 decibels, similar to city traffic noise.",
                "Some early modems used the system speaker to play connection handshake sounds so users could diagnose connection issues.",
                "The IBM PC's system speaker was connected to a 2.25-inch cone speaker, though modern implementations vary widely.",
                "Programmers in the demoscene created 'tracker' music formats specifically optimized for system speaker playback.",
                "The system speaker can theoretically produce frequencies from about 18 Hz to over 1 MHz, though only a fraction is audible.",
                "Some BIOS manufacturers created musical POST sequences as Easter eggs, playing short tunes on successful boot.",
                "The system speaker was essential for accessibility features in early computing, providing audio feedback for visually impaired users.",
                "QBasic's PLAY command allowed musicians to compose music for the system speaker using a simple notation language.",
                "The system speaker's piezoelectric variant can last for decades without degradation, unlike cone speakers.",
                "Some vintage PC enthusiasts collect motherboards specifically for their unique system speaker sound characteristics.",
                "The Tandy 1000 series featured an enhanced 3-voice sound chip that was backward compatible with the standard PC speaker.",
                "Early text-to-speech programs like 'SAM' (Software Automatic Mouth) used clever algorithms to produce recognizable speech through the system speaker.",
                "The system speaker's timing precision made it useful for generating accurate clock signals in some scientific applications.",
                "Some bootloader programs use the system speaker to indicate loading progress with different pitch patterns.",
                "The FreeDOS project maintains system speaker support to ensure compatibility with vintage software and games.",
                "AI music generation can sketch melodies that are later adapted to the system speaker by simplifying harmony into fast arpeggios.",
                "Because the system speaker is monophonic, AI-arranged music often relies on rhythm and melodic contour to imply chords.",
                "An AI can propose chord progressions, but system speaker music typically encodes them as rapid note alternation to simulate harmony.",
                "AI-generated drum patterns can be approximated on the system speaker using short, percussive bursts and pitch jumps.",
                "Turning rich AI compositions into system speaker music is an exercise in musical reduction: keep the hook, drop the layers.",
                "AI can optimize note density for system speaker playback so melodies remain recognizable without polyphony.",
                "The system speaker outputs a square wave, so AI 'sound choices' must be translated into music via rhythm, register, and articulation.",
                "AI can suggest counter-melodies, but system speaker music must schedule them as time-sliced fragments between main notes.",
                "AI-assisted composition can test many musical ideas quickly, then the chosen result is quantized for stable system speaker pitch timing.",
                "When mapping AI music to the system speaker, musical groove often matters more than complex harmony due to the single-voice limit.",
                "AI can generate motifs and variations, which fit well with system speaker music where repetition strengthens musical identity.",
                "System speaker arrangements often use octave jumps for impact; AI can learn where those jumps support musical tension and release.",
                "AI can generate long-form musical structure, while the system speaker version focuses on clear sections and a memorable lead line.",
                "AI can generate melodies in any key, but system speaker music benefits from pitch ranges that avoid overly piercing frequencies.",
                "A common workflow is: AI generates symbolic music (notes), then a system speaker renderer converts it into frequency and duration events.",
                "AI can help decide which musical voice to keep when collapsing multi-track music into a single system speaker line.",
                "System speaker constraints encourage chiptune-like writing; AI can be guided to produce music that survives extreme simplification.",
                "AI can propose expressive timing, but system speaker music often needs strict quantization to keep rhythm and pitch consistent.",
                "Because system speaker output is essentially one waveform, AI must express musical color through phrasing instead of instrumentation.",
                "AI can generate many musical candidates, and the system speaker version becomes a distilled lead-sheet of the most memorable notes.",
            };
            int funFactIndex = new Random().Next(funFacts.Length);
            logText += $"\r\nFun Fact: {funFacts[funFactIndex]}\r\n\r\n";
            return logText.TrimEnd();
        }



        /// <summary>
        /// Specifies the severity level of a log entry.
        /// </summary>
        /// <remarks>Use this enumeration to indicate the importance or type of information being logged.
        /// The values represent informational messages, warnings, and error conditions.</remarks>
        public enum LogTypes
        {
            Info,
            Warning,
            Error
        }

        /// <summary>
        /// Forces the logger type to initialize before entering timing-sensitive code.
        /// Call this once during application startup.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Initialize()
        {
            // Calling this method triggers the static constructor before this body runs.
        }

        /// <summary>
        /// Enqueues a log entry without performing file I/O, masking, formatting, or signaling.
        /// </summary>
        /// <param name="message">The message to write to DebugLog.txt.</param>
        /// <param name="logTypes">The severity of the entry.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string message, LogTypes logTypes)
        {
            if (Volatile.Read(ref _shutdownStarted) != 0)
            {
                return;
            }

            // Stopwatch.GetTimestamp() is substantially cheaper than DateTime.Now.
            // No event is signaled here: avoiding a kernel transition is important for
            // audio/render loops and other latency-sensitive callers.
            _pendingLogs.Enqueue(
                new LogEntry(Stopwatch.GetTimestamp(), message ?? string.Empty, logTypes));
        }

        /// <summary>
        /// Processes queued log entries on a dedicated thread and writes them to DebugLog.txt.
        /// </summary>
        private static void WriterLoop()
        {
            try
            {
                using FileStream stream = new FileStream(
                    _logPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.ReadWrite,
                    bufferSize: 64 * 1024,
                    options: FileOptions.SequentialScan);

                using StreamWriter writer = new StreamWriter(
                    stream,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                    bufferSize: 64 * 1024)
                {
                    AutoFlush = false
                };

                string header;
                try
                {
                    header = BuildHeader();
                }
                catch (Exception ex)
                {
                    header = "NeoBleeper Debug Log";
                    WriteDebugLine(
                        $"Logger: header generation error: {ex.GetType().Name}: {ex.Message}");
                }

                writer.WriteLine(header);
                if (_mirrorEntriesToDebugOutput)
                {
                    WriteDebugLine(header);
                }
                writer.Flush();

                var batch = new StringBuilder(64 * 1024);
                var flushTimer = Stopwatch.StartNew();
                bool hasUnflushedData = false;

                while (Volatile.Read(ref _shutdownStarted) == 0 || !_pendingLogs.IsEmpty)
                {
                    batch.Clear();
                    int processed = 0;

                    while (processed < MaximumBatchSize &&
                           _pendingLogs.TryDequeue(out LogEntry entry))
                    {
                        string message;
                        try
                        {
                            message = MaskSensitiveInformations(entry.Message);
                        }
                        catch (Exception ex)
                        {
                            // Never write the unmasked source text when masking fails.
                            message = "[REDACTED_MASKING_ERROR]";
                            WriteDebugLine(
                                $"Logger: masking error: {ex.GetType().Name}: {ex.Message}");
                        }

                        DateTime timestamp = ConvertTimestamp(entry.Timestamp);
                        string typeName = GetLogTypeName(entry.Type);

                        batch.Append('[')
                             .Append(timestamp.ToString("HH:mm:ss"))
                             .Append("] - [")
                             .Append(typeName)
                             .Append("] ")
                             .Append(message)
                             .Append("\r\n");

                        if (_mirrorEntriesToDebugOutput)
                        {
                            WriteDebugLine(
                                $"[{timestamp:HH:mm:ss}] - [{typeName}] {message}");
                        }

                        processed++;
                    }

                    if (processed != 0)
                    {
                        // One write per batch dramatically lowers StreamWriter overhead.
                        writer.Write(batch);
                        hasUnflushedData = true;
                    }

                    bool shuttingDown = Volatile.Read(ref _shutdownStarted) != 0;

                    // Flushing less often is important. Flush() is performed entirely on
                    // this background thread, and DebugLog.txt is still updated promptly.
                    if (hasUnflushedData &&
                        (shuttingDown || flushTimer.ElapsedMilliseconds >= FlushIntervalMilliseconds))
                    {
                        writer.Flush();
                        hasUnflushedData = false;
                        flushTimer.Restart();
                    }

                    if (processed == 0 && !shuttingDown)
                    {
                        // Log() never signals this event. It is used only to wake the
                        // polling writer immediately during shutdown.
                        _shutdownEvent.WaitOne(PollIntervalMilliseconds);
                    }
                    else if (processed == MaximumBatchSize)
                    {
                        // Let latency-sensitive application threads run between large batches.
                        Thread.Yield();
                    }
                }

                writer.Flush();
            }
            catch (Exception ex)
            {
                WriteDebugLine($"Logger: file write error: {ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes to Visual Studio's Debug output without blocking Log().
        /// In Release builds, Debug.WriteLine calls are normally removed, so
        /// Debugger.Log is used when a debugger is attached.
        /// </summary>
        private static void WriteDebugLine(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#else
            if (Debugger.IsAttached)
            {
                Debugger.Log(0, "NeoBleeper", message + Environment.NewLine);
            }
#endif
        }

        private static DateTime ConvertTimestamp(long timestamp)
        {
            long elapsedStopwatchTicks = timestamp - _clockBaseTimestamp;
            double elapsedSeconds = elapsedStopwatchTicks / (double)Stopwatch.Frequency;
            return _clockBaseTime.AddSeconds(elapsedSeconds);
        }

        private static string GetLogTypeName(LogTypes type)
        {
            return type switch
            {
                LogTypes.Warning => "Warning",
                LogTypes.Error => "Error",
                _ => "Info"
            };
        }

        /// <summary>
        /// Flushes queued log entries and stops the background file writer.
        /// This is called automatically during normal process shutdown.
        /// </summary>
        public static void Shutdown()
        {
            if (Interlocked.Exchange(ref _shutdownStarted, 1) != 0)
            {
                return;
            }

            _shutdownEvent.Set();

            if (Thread.CurrentThread != _writerThread)
            {
                _writerThread.Join();
            }
        }


        /// <summary>
        /// Builds the compiled masking rules once, on the background writer thread.
        /// </summary>
        private static (Regex Regex, string Replacement)[] CreateMaskRules()
        {
            var patterns = new (string Pattern, string Replacement, RegexOptions Options)[]
            {
        // Private key headers
        (
            @"-----BEGIN (?:RSA |EC |DSA |OPENSSH )?PRIVATE KEY-----",
            "[REDACTED_PRIVATE_KEY]",
            RegexOptions.IgnoreCase
        ),

        // Passwords, secrets, and tokens assigned in configuration/code
        (
            @"\b(?:password|passwd|pwd|secret|token|api[_-]?key|" +
            @"client[_-]?secret)\s*[:=]\s*[""']?" +
            @"[A-Za-z0-9._~+/=-]{8,}[""']?",
            "[REDACTED_SECRET]",
            RegexOptions.IgnoreCase
        ),

        // Google API keys
        (
            @"\bAIzaSy[A-Za-z0-9_-]{33}\b",
            "[REDACTED_API_KEY]",
            RegexOptions.None
        ),

        // OpenAI-style keys
        (
            @"\bsk-(?:proj-|svcacct-)?[A-Za-z0-9_-]{20,}\b",
            "[REDACTED_API_KEY]",
            RegexOptions.IgnoreCase
        ),

        // AWS access key IDs
        (
            @"\b(?:AKIA|ASIA)[A-Z0-9]{16}\b",
            "[REDACTED_AWS_KEY]",
            RegexOptions.None
        ),

        // Bearer tokens
        (
            @"\bBearer\s+[A-Za-z0-9._~+/=-]{20,}",
            "Bearer [REDACTED_TOKEN]",
            RegexOptions.IgnoreCase
        ),

        // JSON Web Tokens
        (
            @"(?<![A-Za-z0-9_-])" +
            @"eyJ[A-Za-z0-9_-]{5,}\." +
            @"[A-Za-z0-9_-]{5,}\." +
            @"[A-Za-z0-9_-]{5,}" +
            @"(?![A-Za-z0-9_-])",
            "[REDACTED_JWT]",
            RegexOptions.None
        ),

        // Email addresses
        (
            @"(?<![A-Za-z0-9._%+-])" +
            @"[A-Za-z0-9._%+-]+@" +
            @"[A-Za-z0-9.-]+\.[A-Za-z]{2,}" +
            @"(?![A-Za-z0-9._%+-])",
            "[REDACTED_EMAIL]",
            RegexOptions.IgnoreCase
        ),

        // UUIDs
        (
            @"\b[0-9A-F]{8}-[0-9A-F]{4}-" +
            @"[1-5][0-9A-F]{3}-[89AB][0-9A-F]{3}-" +
            @"[0-9A-F]{12}\b",
            "[REDACTED_UUID]",
            RegexOptions.IgnoreCase
        ),

        // Windows full paths
        (
            @"[A-Za-z]:\\" +
            @"(?:[^\\/:*?""<>|\r\n]+\\)*" +
            @"[^\\/:*?""<>|\r\n]*",
            "[REDACTED_PATH]",
            RegexOptions.None
        ),

        // Unix/macOS user home paths
        (
            @"(?<!\w)/(?:home|Users)/" +
            @"[^/\s]+(?:/[^\s""']*)?",
            "[REDACTED_PATH]",
            RegexOptions.None
        ),

        // Credit-card-number-like strings
        (
            @"(?<!\d)\d(?:[ -]?\d){12,18}(?!\d)",
            "[REDACTED_NUMBER]",
            RegexOptions.None
        ),

        // IPv4 addresses
        (
            @"\b(?:(?:25[0-5]|2[0-4]\d|1?\d{1,2})\.){3}" +
            @"(?:25[0-5]|2[0-4]\d|1?\d{1,2})\b",
            "[REDACTED_IP]",
            RegexOptions.None
        ),

        // MAC addresses
        (
            @"\b(?:[0-9A-F]{2}[:-]){5}[0-9A-F]{2}\b",
            "[REDACTED_MAC]",
            RegexOptions.IgnoreCase
        ),

        // Long Base64 values
        (
            @"(?<![A-Za-z0-9+/=])" +
            @"[A-Za-z0-9+/]{40,}={0,2}" +
            @"(?![A-Za-z0-9+/=])",
            "[REDACTED_BASE64]",
            RegexOptions.None
        ),

        // Generic long tokens or keys
        (
            @"(?<![A-Za-z0-9_-])" +
            @"[A-Za-z0-9_-]{40,}" +
            @"(?![A-Za-z0-9_-])",
            "[REDACTED_SECRET]",
            RegexOptions.None
        )
            };

            var rules = new List<(Regex Regex, string Replacement)>(patterns.Length);
            TimeSpan timeout = TimeSpan.FromMilliseconds(250);

            foreach (var (pattern, replacement, options) in patterns)
            {
                try
                {
                    rules.Add((
                        new Regex(
                            pattern,
                            options | RegexOptions.Compiled | RegexOptions.CultureInvariant,
                            timeout),
                        replacement));
                }
                catch (ArgumentException)
                {
                    // Skip invalid patterns without disabling the logger.
                }
            }

            return rules.ToArray();
        }

        /// <summary>
        /// Masks sensitive information before it is written to the log file.
        /// </summary>
        /// <param name="text">The log message to sanitize.</param>
        /// <returns>The sanitized log message.</returns>
        private static string MaskSensitiveInformations(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !MayContainSensitiveInformation(text))
            {
                return text;
            }

            string result = text;

            foreach (var (regex, replacement) in _maskRules.Value)
            {
                try
                {
                    result = regex.Replace(result, replacement);
                }
                catch (RegexMatchTimeoutException)
                {
                    // Skip only the pattern that timed out.
                }
            }

            return result;
        }
        /// <summary>
        /// Avoids running every regular expression for ordinary log messages.
        /// This check is intentionally conservative: uncertain messages still go
        /// through the complete masking pipeline.
        /// </summary>
        private static bool MayContainSensitiveInformation(string text)
        {
            int digitCount = 0;
            int tokenRun = 0;
            bool hasDot = false;
            bool hasHyphen = false;
            bool hasColon = false;

            foreach (char character in text)
            {
                if (char.IsDigit(character))
                {
                    digitCount++;
                }

                if (char.IsLetterOrDigit(character) ||
                    character is '_' or '-' or '+' or '/' or '=')
                {
                    tokenRun++;
                    if (tokenRun >= 40)
                    {
                        return true;
                    }
                }
                else
                {
                    tokenRun = 0;
                }

                switch (character)
                {
                    case '@':
                    case '\\':
                        return true;
                    case '.':
                        hasDot = true;
                        break;
                    case '-':
                        hasHyphen = true;
                        break;
                    case ':':
                        hasColon = true;
                        break;
                }
            }

            if (digitCount >= 13 ||
                (digitCount >= 4 && (hasDot || hasColon || hasHyphen)))
            {
                return true;
            }

            return text.IndexOf("-----BEGIN", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("passwd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("pwd", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("api_key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("api-key", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("client_secret", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("client-secret", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("Bearer ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("AIzaSy", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("AKIA", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("ASIA", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("sk-", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   text.IndexOf("eyJ", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("/home/", StringComparison.Ordinal) >= 0 ||
                   text.IndexOf("/Users/", StringComparison.Ordinal) >= 0;
        }

    }
}