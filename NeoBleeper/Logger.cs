using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class Logger
    {
        // Out with legacy "logenable" file without extension to enable logging, in with "DebugLog.txt" that always logs.
        static String LogText;
        static Logger()
        {
            LogText += "\r\n  _   _            ____  _                           \r\n" +
                " | \\ | |          |  _ \\| |                          \r\n" +
                " |  \\| | ___  ___ | |_) | | ___  ___ _ __   ___ _ __ \r\n" +
                " | . ` |/ _ \\/ _ \\|  _ <| |/ _ \\/ _ \\ '_ \\ / _ \\ '__|\r\n" +
                " | |\\  |  __/ (_) | |_) | |  __/  __/ |_) |  __/ |   \r\n" +
                " |_| \\_|\\___|\\___/|____/|_|\\___|\\___| .__/ \\___|_|   \r\n" +
                "                                    | |              \r\n" +
                "                                    |_|              \r\n";
            LogText += "\nFrom Something Unreal to Open Sound – Reviving the Legacy, One Note at a Time. \r\n";
            LogText += "\nhttps://github.com/GeniusPilot2016/NeoBleeper \r\n\n";
            LogText += $"NeoBleeper Version {GetInformations.GetVersionAndStatus().version} {GetInformations.GetVersionAndStatus().status}\r\n";
            LogText += GetInformations.getSystemInfo();
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
                "SomethingUnreal's BaWaMI is a MIDI synthesizer that allows users to create music using the system speaker, showcasing the creative potential of this humble hardware component."
            };
            int funFactIndex = new Random().Next(funFacts.Length);
            LogText += $"\r\nFun Fact: {funFacts[funFactIndex]}\r\n\r\n"; 
            Debug.WriteLine(LogText);
            WriteLogToFile(LogText.TrimEnd());
        }
        public enum LogTypes
        {
            Info,
            Warning,
            Error
        }
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
            LogText += logMessage + "\r\n";
            WriteLogToFile(LogText);
            Debug.WriteLine(logMessage);
        }

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
