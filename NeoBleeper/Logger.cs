using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class Logger
    {
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
            int MajorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            int MinorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int PatchVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;

            string version = $"{MajorVersion}.{MinorVersion}.{PatchVersion}";

            // Extract the status (e.g., "alpha") and capitalize the first letter
            string status = System.Reflection.Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
                .Cast<System.Reflection.AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?.InformationalVersion?.Split('-').Skip(1).FirstOrDefault()?.Split('+')[0] ?? "Release";

            status = char.ToUpper(status[0]) + status.Substring(1);

            LogText += $"NeoBleeper Version {version} {status}\r\n";
            String osVersion = System.Environment.OSVersion.VersionString;
            LogText += $"\r\nOperating System: {osVersion}\r\n";
            string processorSpeed = "Unknown";
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        processorSpeed = item["MaxClockSpeed"] + " MHz";
                        break;
                    }
                }
            }
            catch
            {
                processorSpeed = "Unavailable";
            }
            string deviceModel = "Unknown";
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("select Model from Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        deviceModel = item["Model"]?.ToString() ?? "Unknown";
                        break;
                    }
                }
            }
            catch
            {
                deviceModel = "Unavailable";
            }
            string deviceManufacturer = "Unknown";
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("select Manufacturer from Win32_ComputerSystem"))
                {
                    foreach (var item in searcher.Get())
                    {
                        deviceManufacturer = item["Manufacturer"]?.ToString() ?? "Unknown";
                        break;
                    }
                }
            }
            catch
            {
                deviceManufacturer = "Unavailable";
            }
            string powerStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline ? "On battery power" : SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online ? "Plugged in" : "Unknown";
            String systemProperties = $"64-bit OS: {Environment.Is64BitOperatingSystem}\r\n" +
                $"64-bit Process: {Environment.Is64BitProcess}\r\n" +
                $"Device Manufacturer: {deviceManufacturer}\r\n" +
                $"Device Model: {deviceModel}\r\n" +
                $"Processor Count: {Environment.ProcessorCount}\r\n" +
                $"Processor Speed: {processorSpeed}\r\n" +
                $"Processor Identifier: {Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")}\r\n" +
                $"Processor Revision: {Environment.GetEnvironmentVariable("PROCESSOR_REVISION")}\r\n" +
                $"Total Memory (MB): {new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024}\r\n" +
                $"Available Memory (MB): {new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory / 1024 / 1024}\r\n" +
                $"Power Status: {powerStatus}\r\n" +
                $"System Directory: {Environment.SystemDirectory}\r\n" +
                $".NET Version: {Environment.Version}\r\n";
            LogText += systemProperties;
            string[] funFacts = new string[]
            {
                "The system speaker was introduced in 1981 as part of the original IBM PC.",
                "It is monophonic, meaning it can only play one note at a time.",
                "Early PC games used the system speaker to create iconic 8-bit soundtracks.",
                "The speaker's sound is controlled by sending specific frequencies to the hardware.",
                "Despite being largely obsolete, it is still used for diagnostic beeps in modern PCs.",
                "Music and mathematics are deeply connected through harmonic frequencies.",
                "The oldest known musical instrument is over 40,000 years old.",
                "Music is often called the 'universal language' for its emotional resonance.",
                "Early 'beep music' paved the way for modern chiptunes.",
                "The limitations of early sound hardware inspired the development of advanced sound cards.",
                "The system speaker generates sound using square waves, giving it a distinct tone.",
                "Some early software used the system speaker to play Morse code for communication.",
                "Beep codes emitted by the system speaker help diagnose hardware issues during boot.",
                "Hobbyists have created entire songs using sequences of system speaker beeps.",
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
                "Some musicians today intentionally use 1-bit audio (like system speaker output) as an artistic choice for its raw, digital aesthetic."
            };
            int funFactIndex = new Random().Next(funFacts.Length);
            LogText += $"\r\nFun Fact: {funFacts[funFactIndex]}\r\n\r\n"; 
            Debug.WriteLine(LogText); 
            File.WriteAllText("DebugLog.txt", LogText.TrimEnd());
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
            File.WriteAllText("DebugLog.txt", LogText);
            Debug.WriteLine(logMessage);
        }
    }
}
