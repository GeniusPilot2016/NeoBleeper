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

namespace NeoBleeper
{
    public class GetInformations
    {
        static bool is_system_speaker_present = TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present;
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
        public enum computerTypes
        {
            ModularComputer,
            CompactComputer,
            Unknown,
        }
        public static Enum ComputerType = computerTypes.Unknown;
        public static Enum getTypeOfComputer()
        {
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("select ChassisTypes from Win32_SystemEnclosure"))
                {
                    foreach (var item in searcher.Get())
                    {
                        var types = (ushort[])item["ChassisTypes"];
                        if (types != null && types.Length > 0)
                        {
                            foreach (var type in types)
                            {
                                switch (type)
                                {
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                    case 17:
                                    case 18:
                                    case 22:
                                    case 23:
                                        return computerTypes.ModularComputer;
                                    case 8:
                                    case 9:
                                    case 10:
                                    case 11:
                                    case 13:
                                    case 14:
                                    case 15:
                                    case 16:
                                    case 24:
                                    case 30:
                                    case 31:
                                    case 32:
                                        return computerTypes.CompactComputer;
                                    default:
                                        return computerTypes.Unknown;
                                }
                            }
                        }
                    }
                }
                return computerTypes.Unknown;
            }
            catch
            {
                return computerTypes.Unknown;
            }
        }
        public static bool isResolutionSupported()
        {
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            return width >= 1024 && height >= 768;
        }
        public static string getFullTypeOfComputer()
        {
            string[] chassisTypes = new string[]
            {
                "Other",
                "Unknown", 
                "Desktop", 
                "Low Profile Desktop", 
                "Pizza Box", 
                "Mini Tower", 
                "Tower", 
                "Portable", 
                "Laptop", 
                "Notebook", 
                "Hand Held", 
                "Docking Station", 
                "All in One", 
                "Sub Notebook", 
                "Space-Saving", 
                "Lunch Box", 
                "Main System Chassis", 
                "Expansion Chassis", 
                "SubChassis", 
                "Bus Expansion Chassis", 
                "Peripheral Chassis", 
                "Storage Chassis", 
                "Rack Mount Chassis", 
                "Sealed-Case PC", 
                "Tablet",
                "Convertible",
                "Detachable",
            };
            try
            {
                using (var searcher = new System.Management.ManagementObjectSearcher("select ChassisTypes from Win32_SystemEnclosure"))
                {
                    foreach (var item in searcher.Get())
                    {
                        var types = (ushort[])item["ChassisTypes"];
                        if (types != null && types.Length > 0)
                        {
                            List<string> typeNames = new List<string>();
                            foreach (var type in types)
                            {
                                if (type < chassisTypes.Length)
                                {
                                    typeNames.Add(chassisTypes[type-1]);
                                }
                            }
                            return string.Join(", ", typeNames.Distinct());
                        }
                    }
                }
                return "Unknown";
            }
            catch
            {
                return "Unknown";
            }
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
            string computerType = getFullTypeOfComputer();
            string powerStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline ? "On battery power" : SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online ? "Plugged in" : "Unknown";
            String systemProperties = $"64-bit OS: {Environment.Is64BitOperatingSystem}\r\n" +
                $"64-bit Process: {Environment.Is64BitProcess}\r\n" +
                $"Device Manufacturer: {deviceManufacturer}\r\n" +
                $"Device Model: {deviceModel}\r\n" +
                $"Computer Type: {computerType}\r\n" +
                $"Processor Count: {Environment.ProcessorCount}\r\n" +
                $"Processor Speed: {processorSpeed}\r\n" +
                $"Processor Identifier: {Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")}\r\n" +
                $"Processor Revision: {Environment.GetEnvironmentVariable("PROCESSOR_REVISION")}\r\n" +
                $"Total Memory (MB): {new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024 / 1024}\r\n" +
                $"Available Memory (MB): {new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory / 1024 / 1024}\r\n" +
                $"Power Status: {powerStatus}\r\n" +
                $"Presence of a system speaker: {(is_system_speaker_present == true ? "Yes" : "No")}\r\n" +
                $"System Directory: {Environment.SystemDirectory}\r\n" +
                $".NET Version: {Environment.Version}\r\n";
            systemInfo += systemProperties;
            return systemInfo;
        }
    }
}
