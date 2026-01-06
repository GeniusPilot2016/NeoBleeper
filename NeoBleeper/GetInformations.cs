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

using NeoBleeper.Properties;
using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public class GetInformations
    {
        public static string GlobalSystemInfo;
        static bool IsSystemSpeakerPresent = TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isSystemSpeakerPresent;
        static bool IsChipsetAffected = TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues;

        /// <summary>
        /// Retrieves the current assembly's version number and release status as a tuple.
        /// </summary>
        /// <remarks>The release status is determined from the assembly's informational version attribute,
        /// if available. If the status is not specified in the attribute, the returned status will be an empty string.
        /// This method is typically used to display version and release information in application diagnostics or about
        /// dialogs.</remarks>
        /// <returns>A tuple containing the version string and the release status. The version string is formatted as
        /// 'Major.Minor.Build' with an optional 'Revision' suffix if present. The status is a human-readable string
        /// such as 'Alpha', 'Beta', 'Release Candidate', or an empty string if no status is specified.</returns>
        public static (string version, string status) GetVersionAndStatus()
        {
            int MajorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
            int MinorVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
            int PatchVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
            int RevisionVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

            string version = $"{MajorVersion}.{MinorVersion}.{PatchVersion}{(RevisionVersion != 0 ? $" Revision {RevisionVersion}" : string.Empty)}";

            // Extract the status (e.g., "alpha", "beta", "rc") and capitalize the first letter
            string status = System.Reflection.Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
                .Cast<System.Reflection.AssemblyInformationalVersionAttribute>()
                .FirstOrDefault()?.InformationalVersion?.Split('-').Skip(1).FirstOrDefault()?.Split('+')[0] ?? string.Empty;

            if (!string.IsNullOrEmpty(status))
            {
                // Handle specific statuses like "rc" (release candidate)
                if (status.StartsWith("rc", StringComparison.OrdinalIgnoreCase) ||
                    status.Contains("release-candidate", StringComparison.OrdinalIgnoreCase))
                {
                    status = "Release Candidate";
                }
                else
                {
                    status = char.ToUpper(status[0]) + status.Substring(1);
                }
            }

            return (version, status);
        }

        /// <summary>
        /// Specifies the types of computers supported by the system.
        /// </summary>
        /// <remarks>Use this enumeration to indicate or determine the form factor or classification of a
        /// computer within the application. The values represent distinct categories, including modular and compact
        /// computers, as well as an unknown type for cases where the classification cannot be determined.</remarks>
        public enum computerTypes
        {
            ModularComputer,
            CompactComputer,
            Unknown,
        }
        public static Enum computerType = computerTypes.Unknown;

        /// <summary>
        /// Determines the type of computer chassis based on system enclosure information.
        /// </summary>
        /// <remarks>This method queries the system's enclosure information using Windows Management
        /// Instrumentation (WMI) to identify the chassis type. If the chassis type cannot be determined or an error
        /// occurs during the query, the method returns <c>computerTypes.Unknown</c>.</remarks>
        /// <returns>An enumeration value representing the detected computer chassis type. Returns a value from the
        /// <c>computerTypes</c> enumeration, such as <c>ModularComputer</c>, <c>CompactComputer</c>, or <c>Unknown</c>
        /// if the type cannot be determined.</returns>
        public static Enum GetTypeOfComputer()
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

        /// <summary>
        /// Retrieves the horizontal DPI (dots per inch) of the primary display screen.
        /// </summary>
        /// <remarks>The returned value reflects the system's current DPI setting for the primary display.
        /// This value is typically 96 on standard displays but may be higher on high-DPI screens. Use this method when
        /// scaling graphics or UI elements to match the display's resolution.</remarks>
        /// <returns>An integer representing the horizontal DPI of the primary screen.</returns>

        public static int GetPrimaryScreenDpi()
        {
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                // Return the horizontal DPI of the primary screen
                return (int)g.DpiX;
            }
        }
        /// <summary>
        /// Determines whether the primary display supports a minimum resolution of 1024 by 768 pixels.
        /// </summary>p
        /// <remarks>This method checks only the primary display. Use this method to verify that the
        /// application is running on a screen with sufficient resolution for certain UI layouts or features.</remarks>
        /// <returns>true if the primary screen's width is at least 1024 pixels and its height is at least 768 pixels; otherwise,
        /// false.</returns>
        public static bool IsResolutionSupported()
        {
            double dpiScale = GetPrimaryScreenDpi() / 96.0; // 96 DPI is the standard scale
            int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            return width >= (1024 * dpiScale) && height >= (768 * dpiScale); // Minimum required resolution adjusted for DPI scaling to prevent the main window is not fitting on screen
        }

        /// <summary>
        /// Retrieves a descriptive string representing the full chassis type or types of the current computer system.
        /// </summary>
        /// <remarks>This method queries the system's enclosure information using Windows Management
        /// Instrumentation (WMI). If multiple chassis types are reported, all are included in the result. The method
        /// returns "Unknown" if the information is unavailable or an error occurs during retrieval.</remarks>
        /// <returns>A comma-separated list of chassis type names describing the computer's enclosure, such as "Desktop",
        /// "Laptop", or "Tablet". Returns "Unknown" if the chassis type cannot be determined.</returns>
        public static string GetFullTypeOfComputer()
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
                                    typeNames.Add(chassisTypes[type - 1]);
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

        /// <summary>
        /// Retrieves a formatted string containing detailed information about the current system, including operating
        /// system, hardware, memory, power status, and environment details.
        /// </summary>
        /// <remarks>This method gathers information using various system APIs and may return partial or
        /// unavailable data depending on the platform, permissions, or hardware configuration. The output is intended
        /// for diagnostic or informational purposes and is not guaranteed to be stable across different operating
        /// systems or environments. On ARM64 devices, certain chipset and system speaker details are omitted as they
        /// are not applicable.</remarks>
        /// <returns>A multi-line string with key system information such as operating system version, architecture, processor
        /// details, memory statistics, power status, and other relevant properties. Some values may be reported as
        /// "Unknown" or "Unavailable" if they cannot be determined.</returns>
        public static string GetSystemInfo()
        {
            Program.splashScreen.UpdateStatus(Resources.StatusSystemInformationsGathering);
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
            string computerType = GetFullTypeOfComputer();
            string powerStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline ? "On battery power" : SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online ? "Plugged in" : "Unknown";
            String systemProperties = $"OS Architecture: {RuntimeInformation.OSArchitecture.ToString()}\r\n" +
                $"Process Architecture: {RuntimeInformation.ProcessArchitecture.ToString()}\r\n" +
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
                // Don't show status of manufacturer check on ARM64 devices as they are not possibly affected from system speaker issues
                ((RuntimeInformation.ProcessArchitecture != Architecture.Arm64) ?
                $"Status of affected chipset check: {(Program.isAffectedChipsetChecked == true ?
                    (IsChipsetAffected == true ? "Affected" : "Not Affected") :
                    "Unknown")}\r\n" : string.Empty) + // Conditional inclusion to determine unknown status if not checked yet and ARM64 architecture devices
                                                       // Don't show system speaker info on ARM64 devices as they don't have system speakers
                ((RuntimeInformation.ProcessArchitecture != Architecture.Arm64) ?
                $"Presence of a system speaker{(IsChipsetAffected == true && Program.isAffectedChipsetChecked == true ? " (may be inaccurate due to affected chipset)" : string.Empty)}: {(Program.isExistenceOfSystemSpeakerChecked == true ?
                    (IsSystemSpeakerPresent == true ? "Yes" : "No") :
                    "Unknown")}\r\n" : string.Empty) + // Conditional inclusion to determine unknown status if not checked yet and ARM64 architecture devices
                $"System Directory: {Environment.SystemDirectory}\r\n" +
                $".NET Version: {Environment.Version}\r\n";
            systemInfo += systemProperties;
            GlobalSystemInfo = systemInfo;
            Program.splashScreen.UpdateStatus(Resources.StatusSystemInformationsGathered, 10);
            return systemInfo;
        }
    }
}
