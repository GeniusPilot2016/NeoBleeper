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

using System.Diagnostics;
using System.Reflection;

namespace NeoBleeper
{
    public static class Logger
    {
        // Out with legacy "logenable" file without extension to enable logging, in with "DebugLog.txt" that always logs.
        static String logText;
        static Logger()
        {
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
                "Robbi-985 (aka SomethingUnreal) was a pioneer in composing music for the IBM PC's system speaker and he's known for his work which he made music from Windows XP and 98 sounds, which can be found on his YouTube channel and linked here: https://www.youtube.com/watch?v=dsU3B0W3TMs",
                "Robbi-985 (aka SomethingUnreal) developed programs called 'BaWaMI (Basic Waveform MIDI Software Synthesizer)' and 'Bleeper Music Maker' to compose music for the system speaker.",
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
                "The FreeDOS project maintains system speaker support to ensure compatibility with vintage software and games."
            };
            int funFactIndex = new Random().Next(funFacts.Length);
            logText += $"\r\nFun Fact: {funFacts[funFactIndex]}\r\n\r\n";
            Debug.WriteLine(logText);
            WriteLogToFile(logText.TrimEnd());
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
        /// Writes a log entry with the specified message and log type to the log file and debug output.
        /// </summary>
        /// <remarks>The log entry is timestamped and appended to the log file as well as written to the
        /// debug output. Use this method to record application events, warnings, or errors for diagnostic
        /// purposes.</remarks>
        /// <param name="message">The message to include in the log entry. This value can be any string to describe the event or information
        /// to log.</param>
        /// <param name="logTypes">The type of log entry to write. Specifies whether the message is informational, a warning, or an error.</param>
        public static void Log(string message, LogTypes logTypes)
        {
            string LoggingType;
            switch (logTypes)
            {
                case LogTypes.Info:
                    LoggingType = "Info";
                    break;
                case LogTypes.Warning:
                    LoggingType = "Warning";
                    break;
                case LogTypes.Error:
                    LoggingType = "Error";
                    break;
                default:
                    LoggingType = "Info";
                    break;
            }
            string logMessage = $"[{DateTime.Now:HH:mm:ss}] - [{LoggingType}] {message}";
            logText += logMessage + "\r\n";
            WriteLogToFile(logText);
            Debug.WriteLine(logMessage);
        }

        /// <summary>
        /// Writes the specified log content to a file named "DebugLog.txt" in the application's directory.
        /// </summary>
        /// <remarks>If the log file does not exist, it is created. If it exists, its contents are
        /// overwritten. This method is intended for diagnostic or debugging purposes and is not thread-safe.</remarks>
        /// <param name="content">The log content to write to the file. Can be any string, including empty or null.</param>
        private static void WriteLogToFile(string content)
        {
            try
            {
                string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string logPath = Path.Combine(exePath, "DebugLog.txt");
                using (FileStream fs = new FileStream(logPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(content);
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"File access error: {ex.Message}");
            }
        }
    }
}
