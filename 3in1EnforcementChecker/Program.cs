using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

public class Program
{
    private const string EvalPolicyId = "784c4414-79f4-4c32-a6a5-f0fb42a51d0d";
    private const string EnforcedPolicyId = "8F9CB695-5D48-48D6-A329-7202B44607E3";

    [DllImport("inpoutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
    private static extern bool IsInpOutDriverOpen();

    [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
    private static extern int Inp32(int portAddress);

    public static void Main(string[] args)
    {
        PrintASCIIArtLogo();
        Console.WriteLine("\nTarget: Windows 11 24H2 and above (April 2026 update) and inpoutx64.sys driver\n");

        // Step 1: Enforcement Mode Check 
        CheckEnforcementStatus();

        // Step 2: InpOut64 Presence & Status Check
        CheckInpOutStatus();

        // Step 3: System Timer (PIT) Read Test
        ApplyPITReadTest();
    }

    private static void CheckEnforcementStatus()
    {
        Console.WriteLine("--- [Step 1: System Policy Mode] ---");
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "citool.exe",
                    Arguments = "-lp -json",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var result = JsonSerializer.Deserialize<CiToolOutput>(output);
            var evalPolicy = result?.Policies?.FirstOrDefault(p => p.PolicyId.Equals(EvalPolicyId, StringComparison.OrdinalIgnoreCase));
            var enforcedPolicy = result?.Policies?.FirstOrDefault(p => p.PolicyId.Equals(EnforcedPolicyId, StringComparison.OrdinalIgnoreCase));

            if (enforcedPolicy is { IsEnforced: true, IsAuthorized: true })
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ System is in Enforcement Mode");
            }
            else if (evalPolicy is { IsEnforced: true, IsAuthorized: true })
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ System is in Evaluation (Audit) Mode");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Required policies are not active");
            }
        }
        catch { Console.WriteLine("Error: Unable to check system policy status via citool."); }
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void CheckInpOutStatus()
    {
        Console.WriteLine("--- [Step 2: inpoutx64.sys Presence & Enforcement Check] ---");

        const string targetDriver = "inpoutx64.sys";
        string systemDriversPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers", targetDriver);

        // 1. Physical Presence Check
        if (File.Exists(systemDriversPath))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[PRESENT] ");
            Console.ResetColor();
            Console.WriteLine($"Found at: {systemDriversPath}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[NOT PRESENT] ");
            Console.ResetColor();
            Console.WriteLine($"{targetDriver} not found in standard drivers directory.");
        }

        Console.WriteLine("\nCode Integrity Log History (Searching file name and paths):");

        string query = "*[System[EventID=3076 or EventID=3077]]";
        var logQuery = new EventLogQuery("Microsoft-Windows-CodeIntegrity/Operational", PathType.LogName, query);
        bool anyLogFound = false;

        try
        {
            using var reader = new EventLogReader(logQuery);
            var seenEntries = new HashSet<string>();

            for (var eventDetail = reader.ReadEvent(); eventDetail != null; eventDetail = reader.ReadEvent())
            {
                string xml = eventDetail.ToXml();
                // XML içerisinde ham arama (Hızlı ön filtreleme)
                if (!xml.Contains(targetDriver, StringComparison.OrdinalIgnoreCase)) continue;

                XDocument doc = XDocument.Parse(xml);
                XNamespace ns = "http://schemas.microsoft.com/win/2004/08/events/event";
                var data = doc.Descendants(ns + "Data").ToDictionary(x => x.Attribute("Name")?.Value ?? "", x => x.Value);

                // Check both "File Name" and "File Path" fields for the target driver
                string fileName = data.GetValueOrDefault("File Name", "");
                string filePath = data.GetValueOrDefault("File Path", "");

                // Check if either the file name or file path contains the target driver name (case-insensitive)
                bool isMatch = fileName.Contains(targetDriver, StringComparison.OrdinalIgnoreCase) ||
                              filePath.Contains(targetDriver, StringComparison.OrdinalIgnoreCase);

                if (!isMatch) continue;

                anyLogFound = true;
                string process = data.GetValueOrDefault("Process Name", "Unknown Process");
                string uniqueKey = $"{eventDetail.Id}|{process}|{eventDetail.TimeCreated?.Ticks}";

                if (seenEntries.Add(uniqueKey))
                {
                    if (eventDetail.Id == 3077)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[ENFORCEMENT APPLIED] ");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[AUDITED]             ");
                    }

                    Console.ResetColor();
                    // Show the logged file path if available, otherwise show the file name
                    string sourcePath = !string.IsNullOrEmpty(filePath) ? filePath : fileName;
                    Console.WriteLine($"| Logged: {eventDetail.TimeCreated} | Path: {sourcePath} | By: {process}");
                }
            }

            if (!anyLogFound)
            {
                Console.WriteLine($"No visual logs found for any file path containing '{targetDriver}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        Console.WriteLine();
    }

    public static void ApplyPITReadTest()
    {
        Console.WriteLine("--- [Step 3: System Timer (PIT) Read Test] ---");

        // 1. Driver Handle Check
        // If the driver is blocked by Code Integrity or HVCI, it may fail to open or return dummy data.
        try
        {
            if (!IsInpOutDriverOpen())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ FAILED: InpOut driver could not be opened.");
                Console.WriteLine("Possible Reason: Blocked by Code Integrity (CI) or HVCI.");
                Console.ResetColor();
                return;
            }
        }
        catch (DllNotFoundException)
        {
            Console.WriteLine("Error: inpoutx64.dll found but could not be loaded.");
            return;
        }

        Console.WriteLine("✅ Driver handle is open. Reading system timer Channel 0 (Port 0x40)...");

        // 2. Functionality Check - Read from PIT Channel 0 (Port 0x40)
        // Port 0x40 is a timer channel that should return changing values as the system runs. If the driver is stubbed or blocked, it may return a static value (often 0xFF or 0x00). 
        // It proves it's working if it changes over a short interval.
        int val1 = Inp32(0x40) & 0xFF;
        Thread.Sleep(1); // Short delay to allow for potential value change
        int val2 = Inp32(0x40) & 0xFF;

        if (val1 == val2 && (val1 == 0xFF || val1 == 0x00))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠️ WARNING: Static value (0x{val1:X2}) detected.");
            Console.WriteLine("The driver is open but returning dummy data (Stubbed/Blocked by security).");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ SUCCESS: System timer values are changing (0x{val1:X2} -> 0x{val2:X2}).");
            Console.WriteLine("I/O enforcement is NOT active for this driver.");
        }
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintASCIIArtLogo()
    {
        string asciiArt = @"  ____        _            __    _____ _____    _____ _               _             
 |___ \      (_)          /_ |  / ____|_   _|  / ____| |             | |            
   __) |_____ _ _ __ ______| | | |      | |   | |    | |__   ___  ___| | _____ _ __ 
  |__ <______| | '_ \______| | | |      | |   | |    | '_ \ / _ \/ __| |/ / _ \ '__|
  ___) |     | | | | |     | | | |____ _| |_  | |____| | | |  __/ (__|   <  __/ |   
 |____/      |_|_|_|_|_    |_|  \_____|_____|  \_____|_| |_|\___|\___|_|\_\___|_|   
  / _|           |_   _|            / __ \      | |        / /| || |                
 | |_ ___  _ __    | |  _ __  _ __ | |  | |_   _| |___  __/ /_| || |_               
 |  _/ _ \| '__|   | | | '_ \| '_ \| |  | | | | | __\ \/ / '_ \__   _|              
 | || (_) | |     _| |_| | | | |_) | |__| | |_| | |_ >  <| (_) | | |               
 |_| \___/|_|    |_____|_| |_| .__/ \____/ \__,_|\__/_/\_\\___/  |_|                
                             | |                                                    
                             |_|                                                    ";

        (int R, int G, int B)[] palette = [(0, 255, 255), (0, 255, 0), (255, 255, 0), (255, 0, 255)];
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        string[] lines = asciiArt.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            double t = (double)i / Math.Max(1, lines.Length - 1);
            double scaledT = t * (palette.Length - 1);
            int idx = (int)Math.Floor(scaledT);
            int nextIdx = Math.Min(idx + 1, palette.Length - 1);
            double lerpFactor = scaledT - idx;

            int r = (int)(palette[idx].R + (palette[nextIdx].R - palette[idx].R) * lerpFactor);
            int g = (int)(palette[idx].G + (palette[nextIdx].G - palette[idx].G) * lerpFactor);
            int b = (int)(palette[idx].B + (palette[nextIdx].B - palette[idx].B) * lerpFactor);

            Console.Write($"\x1b[38;2;{r};{g};{b}m{lines[i].TrimEnd()}\n");
        }
        Console.Write("\x1b[0m");
    }

    private record CiToolOutput([property: JsonPropertyName("Policies")] List<Policy> Policies);
    private record Policy(
        [property: JsonPropertyName("PolicyID")] string PolicyId,
        [property: JsonPropertyName("IsEnforced")] bool IsEnforced,
        [property: JsonPropertyName("IsAuthorized")] bool IsAuthorized);
}