using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class GetInformations
    {
        public static (string version, string status) GetVersionAndStatus()
        {
            int MajorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            int MinorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int PatchVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
            int RevisionVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

            string version = $"{MajorVersion}.{MinorVersion}.{PatchVersion}{(RevisionVersion != 0 ? $" Revision {RevisionVersion}" : string.Empty)}";

            // Extract the status (e.g., "alpha") and capitalize the first letter
            string status = System.Reflection.Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
                .Cast<System.Reflection.AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?.InformationalVersion?.Split('-').Skip(1).FirstOrDefault()?.Split('+')[0] ?? string.Empty; // Default to empty string if not found

            status = char.ToUpper(status[0]) + status.Substring(1);
            return (version, status);
        }
        public static string getSystemInfo() 
        {
            string systemInfo = "";
            String osVersion = System.Environment.OSVersion.VersionString;
            systemInfo += $"\r\nOperating System: {osVersion}\r\n";
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
                $"Presence of a system speaker: {(RenderBeep.BeepClass.isSystemSpeakerExist() == true ? "Yes" : "No")}\r\n" +
                $"System Directory: {Environment.SystemDirectory}\r\n" +
                $".NET Version: {Environment.Version}\r\n";
            systemInfo += systemProperties;
            return systemInfo;
        }
    }
}
