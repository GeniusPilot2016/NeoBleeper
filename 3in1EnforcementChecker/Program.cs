using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Globalization;

public class Program
{
    private const string EvalPolicyId = "784c4414-79f4-4c32-a6a5-f0fb42a51d0d";
    private const string EnforcedPolicyId = "8F9CB695-5D48-48D6-A329-7202B44607E3";

    [DllImport("inpoutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
    private static extern bool IsInpOutDriverOpen();

    [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
    private static extern int Inp32(int portAddress);

    [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
    private static extern int Out32(int portAddress, int length);

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

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern uint FormatMessage(
    uint dwFlags,
    IntPtr lpSource,
    uint dwMessageId,
    uint dwLanguageId,
    System.Text.StringBuilder lpBuffer,
    uint nSize,
    IntPtr Arguments);

    const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
    const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
    const uint LANG_EN_US = 0x0409;

    private static string GetSystemMessageEnglish(int hr)
    {
        int win32 = hr & 0xFFFF; // 0x80070005 -> 5
        var sb = new System.Text.StringBuilder(512);
        uint res = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
                                 IntPtr.Zero, (uint)win32, LANG_EN_US, sb, (uint)sb.Capacity, IntPtr.Zero);
        if (res != 0)
            return sb.ToString().Trim();
        return null;
    }

    private static string TurnErrorCodeIntoString(int code)
    {
        // Önce İngilizce anlamı dene
        var msgEn = GetSystemMessageEnglish(code);
        if (!string.IsNullOrEmpty(msgEn))
            return msgEn.Trim(); // sadece metin

        // Yerel dildeki mesajı dene (fallback)
        try
        {
            var ex = new System.ComponentModel.Win32Exception(code);
            if (!string.IsNullOrEmpty(ex.Message))
                return ex.Message;
        }
        catch { /* ignore */ }

        // Hiçbir anlam yoksa hex ile bilinmiyor mesajı
        return $"Unknown error code: 0x{((uint)code):X8}";
    }

    private static bool IsErrorCode(string input)
    {
        return Regex.IsMatch(input, @"^0x[0-9A-Fa-f]{8}$");
    }

    private static void CheckEnforcementStatus()
    {
        Console.WriteLine("--- [Step 1: System Policy Mode] ---");
        bool isElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        Console.WriteLine(isElevated ? "Process: Elevated (admin)" : "Process: Not elevated (no admin)");

        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "citool.exe",
                    Arguments = "-lp -json",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Warning: Failed to start citool.exe. {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Note: citool.exe is a Microsoft internal tool and may not be present on all systems. If you have access to it, place it in the same directory as this program or ensure it's in the PATH.");
                Console.WriteLine();
                return;
            }

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            // İlk tercih: JSON parse
            CiToolOutput? result = null;
            if (!string.IsNullOrWhiteSpace(stdout))
            {
                try { result = JsonSerializer.Deserialize<CiToolOutput>(stdout); }
                catch { result = null; }
            }

            // Eğer JSON parse başarılı ve policy'ler varsa, normal akış
            if (result?.Policies != null && result.Policies.Count > 0)
            {
                
                var evalPolicy = result.Policies.FirstOrDefault(p => p.PolicyId.Equals(EvalPolicyId, StringComparison.OrdinalIgnoreCase));
                var enforcedPolicy = result.Policies.FirstOrDefault(p => p.PolicyId.Equals(EnforcedPolicyId, StringComparison.OrdinalIgnoreCase));

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
                Console.ResetColor();
                Console.WriteLine();
                return;
            }

            // JSON parse yoksa; fallback: metin içinde GUID ve yanındaki IsEnforced/IsAuthorized değerlerini regex ile yakala
            string combined = (stdout ?? "") + "\n" + (stderr ?? "");
            var guidRegex = new Regex(@"[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}", RegexOptions.Compiled);
            var boolRegex = new Regex(@"IsEnforced""?\s*[:=]\s*(true|false)|IsAuthorized""?\s*[:=]\s*(true|false)|Enforced\s*[:=]\s*(true|false)|Authorized\s*[:=]\s*(true|false)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var found = new Dictionary<string, (bool? IsEnforced, bool? IsAuthorized)>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in guidRegex.Matches(combined))
            {
                string id = m.Value;
                if (!string.Equals(id, EvalPolicyId, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(id, EnforcedPolicyId, StringComparison.OrdinalIgnoreCase))
                    continue;

                int idx = m.Index;
                int ctxStart = Math.Max(0, idx);
                int ctxEnd = Math.Min(combined.Length, idx + 400); // 400 chars after GUID
                string ctx = combined.Substring(ctxStart, ctxEnd - ctxStart);

                bool? isEnf = null, isAuth = null;
                // İlk en yakın IsEnforced / IsAuthorized eşleşmelerini ara
                var enMatch = Regex.Match(ctx, @"IsEnforced""?\s*[:=]\s*(true|false)|Enforced\s*[:=]\s*(true|false)", RegexOptions.IgnoreCase);
                if (enMatch.Success)
                {
                    string val = enMatch.Groups.Cast<Group>().Select(g => g.Value).FirstOrDefault(v => !string.IsNullOrEmpty(v) && (v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("false", StringComparison.OrdinalIgnoreCase)));
                    if (!string.IsNullOrEmpty(val)) isEnf = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                var auMatch = Regex.Match(ctx, @"IsAuthorized""?\s*[:=]\s*(true|false)|Authorized\s*[:=]\s*(true|false)", RegexOptions.IgnoreCase);
                if (auMatch.Success)
                {
                    string val = auMatch.Groups.Cast<Group>().Select(g => g.Value).FirstOrDefault(v => !string.IsNullOrEmpty(v) && (v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("false", StringComparison.OrdinalIgnoreCase)));
                    if (!string.IsNullOrEmpty(val)) isAuth = val.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                found[id] = (isEnf, isAuth);
            }

            // Evaluation mode
            if (found.TryGetValue(EnforcedPolicyId, out var enforcedVals) && enforcedVals.IsEnforced == true && enforcedVals.IsAuthorized == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ System is in Enforcement Mode (detected from text output)");
                Console.ResetColor();
            }
            else if (found.TryGetValue(EvalPolicyId, out var evalVals) && evalVals.IsEnforced == true && evalVals.IsAuthorized == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ System is in Evaluation (Audit) Mode (detected from text output)");
                Console.ResetColor();
            }
            else if (found.Count > 0)
            {
                // GUID is found but no clear enforcement/auth status - partial info
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Partial policy info detected in citool output:");
                foreach (var kv in found)
                    Console.WriteLine($" - {kv.Key}  Enforced: {kv.Value.IsEnforced?.ToString() ?? "unknown"}  Authorized: {kv.Value.IsAuthorized?.ToString() ?? "unknown"}");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Note: Run it as administrator for more complete information. Partial data may indicate limited access to policy details.");
                Console.ResetColor();
            }
            else
            {
                // Nothing is found - show raw output for troubleshooting (decoded)
                if (!string.IsNullOrWhiteSpace(stdout))
                {
                    PrintOutputWithDecodedErrors("The citool failed to provide information:", stdout);
                }

                if (!string.IsNullOrWhiteSpace(stderr))
                {
                    PrintOutputWithDecodedErrors("citool error output:", stderr);
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Note: No recognizable policy information found in citool output. This may be due to lack of access permissions, an unexpected output format, or the tool not functioning correctly on this system.");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Unable to check system policy status via citool. {ex.Message}");
        }

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

                    // decode tokens inside event fields so they display human-readable reasons
                    process = DecodeErrorTokensInline(process);
                    sourcePath = DecodeErrorTokensInline(sourcePath);

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
        Console.WriteLine();

        // ── 1. Driver handle check ───────────────────────────────────────────────
        try
        {
            if (IsInpOutDriverOpen() == false)
            {
                WriteResult(false,
                    "FAILED: InpOut driver could not be opened.",
                    "Possible reason: blocked by Code Integrity (CI) or HVCI.");
                return;
            }
        }
        catch (DllNotFoundException)
        {
            WriteResult(false,
                "Error: inpoutx64.dll was found but could not be loaded.",
                "Ensure the DLL matches the process bitness (x64).");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Driver handle is open.");
        Console.ResetColor();
        Console.WriteLine();

        // ── 2. PIT Channel 2 round-trip write/readback test ──────────────────────
        //
        // Strategy:
        //   0x43 (control port) is write-only — we cannot verify what we write to it.
        //   0x42 (Channel 2 data port) IS readable.
        //
        //   We program Channel 2 in LSB-only, Mode 0 (interrupt on terminal count).
        //   Control byte: 0x90
        //     Bits 7-6 = 10  → Channel 2
        //     Bits 5-4 = 01  → LSB only
        //     Bits 3-1 = 000 → Mode 0
        //     Bit  0   = 0   → Binary
        //
        //   We then write a distinctive, non-trivial divisor byte (0xA7) to 0x42
        //   and immediately read 0x42 back. Because Mode 0 counts DOWN from the
        //   loaded value and we read back within microseconds, a real, unblocked
        //   driver will return a value at or very near 0xA7.
        //
        //   A stubbed driver (CI/HVCI blocked) almost always returns a static
        //   dummy value (typically 0x00 or 0xFF) regardless of what was written,
        //   because it intercepts the port I/O without forwarding it to hardware.
        //
        //   Threshold: we allow the counter to have decremented by up to 0x20
        //   (32 ticks) to absorb scheduling jitter on the reading thread.
        //   If the read-back is within [0xA7 - 0x20, 0xA7], we accept it as real.
        //
        // Caveats:
        //   – A very sophisticated stub that echoes the last written byte would
        //     defeat this test. In practice, CI/HVCI stubs do not do this.
        //   – If 0x43 writes are silently dropped, Channel 2 may still be in a
        //     prior mode that affects how 0x42 reads back. The threshold window
        //     is deliberately generous to tolerate this.

        const int WRITE_VALUE = 0xA7;
        const int MAX_DECREMENT = 0x20;
        const int EXPECTED_MIN = WRITE_VALUE - MAX_DECREMENT; // 0x87

        Console.WriteLine("Testing PIT Channel 2 (port 0x42) write/readback...");
        Console.WriteLine($"  Writing 0x{WRITE_VALUE:X2} via LSB-only, Mode 0 (control byte 0x90 → port 0x43)");
        Console.WriteLine();

        // Save original port 0x61 state so we can restore it
        int original61 = Inp32(0x61) & 0xFF;

        // Disable the speaker output while testing (bit 1 = 0), gate on (bit 0 = 1)
        // This prevents any audible click if Channel 2 was previously driving the speaker
        int gated = (original61 & 0xFC) | 0x01;
        Out32(0x61, gated);

        // Program Channel 2: LSB-only, Mode 0, binary
        Out32(0x43, 0x90);

        // Load the divisor — immediately read back before the counter decrements far
        Out32(0x42, WRITE_VALUE);
        int readback = Inp32(0x42) & 0xFF;

        // Restore port 0x61 exactly as found
        Out32(0x61, original61);

        // ── 3. Evaluate ──────────────────────────────────────────────────────────
        Console.WriteLine($"  Written : 0x{WRITE_VALUE:X2}");
        Console.WriteLine($"  Readback: 0x{readback:X2}");
        Console.WriteLine($"  Accepted range: [0x{EXPECTED_MIN:X2}, 0x{WRITE_VALUE:X2}]");
        Console.WriteLine();

        bool isStub = readback == 0x00 || readback == 0xFF;
        bool inRange = readback >= EXPECTED_MIN && readback <= WRITE_VALUE;

        if (isStub)
        {
            // Strongest signal: classic dummy values
            WriteResult(false,
                $"WARNING: Readback is 0x{readback:X2} — a classic stub/dummy value.",
                "The driver is open but is not forwarding I/O to hardware.",
                "Likely blocked by Code Integrity (CI), HVCI, or a hypervisor.");
        }
        else if (inRange)
        {
            // Counter decremented a plausible amount — real hardware
            int decrement = WRITE_VALUE - readback;
            WriteResult(true,
                $"SUCCESS: Readback 0x{readback:X2} is within {decrement} ticks of the written value.",
                "PIT Channel 2 is responding to real port I/O.",
                "I/O enforcement is NOT active for this driver.");
        }
        else
        {
            // Out-of-range but not 0x00/0xFF — ambiguous
            // Could be: Channel 2 was already counting in a different mode,
            // or the counter decremented more than expected due to heavy load.
            WriteResult(null,
                $"AMBIGUOUS: Readback 0x{readback:X2} is outside the expected range.",
                "Possible causes:",
                "  • Channel 2 was already running in a different mode (mode bits ignored by stub)",
                "  • Very high system load caused more ticks than the threshold allows",
                "  • A partial stub that forwards some but not all port writes",
                "Retry the test on a lightly loaded system, or inspect with a port monitor.");
        }

        Console.WriteLine();
    }
    private static void WriteResult(bool? success, params string[] lines)
    {
        Console.ForegroundColor = success switch
        {
            true => ConsoleColor.Green,
            false => ConsoleColor.Yellow,
            null => ConsoleColor.DarkYellow,
        };

        string icon = success switch
        {
            true => "✅",
            false => "⚠️",
            null => "❓",
        };

        Console.WriteLine($"{icon} {lines[0]}");
        for (int i = 1; i < lines.Length; i++)
            Console.WriteLine($"   {lines[i]}");

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
                             |_|                                                   ";

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

    private static void PrintOutputWithDecodedErrors(string title, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var hexRegex = new Regex(@"0x[0-9A-Fa-f]{8}", RegexOptions.Compiled);
        var decRegex = new Regex(@"-?\d{9,10}", RegexOptions.Compiled); // büyük negatif HRESULT'ları yakalamak için

        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        foreach (var line in lines)
        {
            string original = line;
            var replacements = new List<(int start, int length, string repl)>();

            // Hex eşleşmeleri
            foreach (Match m in hexRegex.Matches(original))
            {
                string token = m.Value;
                if (!IsErrorCode(token)) continue;
                if (uint.TryParse(token.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint u))
                {
                    int code = unchecked((int)u);
                    string friendly = TurnErrorCodeIntoString(code) ?? $"Unknown error code: 0x{u:X8}";
                    bool inQuotes = IsSurroundedByQuotes(original, m.Index, m.Length);
                    string rep = inQuotes ? EscapeForJson(friendly) : $"\"{EscapeForJson(friendly)}\"";
                    replacements.Add((m.Index, m.Length, rep));
                }
            }

            // Negatif ondalık HRESULT eşleşmeleri
            foreach (Match m in decRegex.Matches(original))
            {
                string token = m.Value;
                if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intVal) && intVal < 0)
                {
                    string friendly = TurnErrorCodeIntoString(intVal) ?? $"Unknown error code: 0x{((uint)intVal):X8}";
                    bool inQuotes = IsSurroundedByQuotes(original, m.Index, m.Length);
                    string rep = inQuotes ? EscapeForJson(friendly) : $"\"{EscapeForJson(friendly)}\"";
                    replacements.Add((m.Index, m.Length, rep));
                }
            }

            if (replacements.Count == 0)
            {
                // Eğer herhangi bir token değişmedi ise sadece orijinal satırı yaz
                if (!string.IsNullOrWhiteSpace(original))
                    Console.WriteLine(original);
                continue;
            }

            // Sıralama ve çakışan eşleşmeleri atlama
            replacements.Sort((a, b) => a.start.CompareTo(b.start));
            var filtered = new List<(int start, int length, string repl)>();
            int lastEnd = -1;
            foreach (var r in replacements)
            {
                if (r.start < lastEnd) continue;
                filtered.Add(r);
                lastEnd = r.start + r.length;
            }

            // Yeni satırı oluştur
            var sb = new System.Text.StringBuilder();
            int pos = 0;
            foreach (var r in filtered)
            {
                if (pos < r.start) sb.Append(original.Substring(pos, r.start - pos));
                sb.Append(r.repl);
                pos = r.start + r.length;
            }
            if (pos < original.Length) sb.Append(original.Substring(pos));

            string outputLine = sb.ToString();
            if (string.IsNullOrWhiteSpace(outputLine))
            {
                // boş çıktıysa başlık yazma
                continue;
            }

            // Eğer çıktı tek alanlı geçerli bir JSON nesnesiyse, başlığı ve değeri aynı satırda yaz
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(outputLine);
                var root = doc.RootElement;
                if (root.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    int propCount = 0;
                    string singleStringValue = null;
                    foreach (var p in root.EnumerateObject())
                    {
                        propCount++;
                        if (propCount == 1 && p.Value.ValueKind == System.Text.Json.JsonValueKind.String)
                            singleStringValue = p.Value.GetString();
                        if (propCount > 1) break;
                    }

                    if (propCount == 1 && singleStringValue != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{title} {singleStringValue}");
                        Console.ResetColor();
                        continue;
                    }
                }
            }
            catch
            {
                // JSON parse başarısızsa devam et, normal çıktı yazdırılacak
            }

            // Genel durumda başlığı ve çözümlenmiş satırı yaz
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(title);
            Console.ResetColor();
            Console.WriteLine(outputLine);
        }

        // Yerel yardımcılar
        static bool IsSurroundedByQuotes(string s, int startIndex, int length)
        {
            int left = startIndex - 1;
            while (left >= 0 && char.IsWhiteSpace(s[left])) left--;
            int right = startIndex + length;
            while (right < s.Length && char.IsWhiteSpace(s[right])) right++;
            return left >= 0 && s[left] == '"' && right < s.Length && s[right] == '"';
        }

        static string EscapeForJson(string input)
        {
            if (input == null) return "";
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }

    // Yeni yardımcı: event alanları içinde bulunan hex/negatif HRESULT tokenlarını inline olarak çözer
    private static string DecodeErrorTokensInline(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var hexRegex = new Regex(@"0x[0-9A-Fa-f]{8}", RegexOptions.Compiled);
        var decRegex = new Regex(@"-?\d{9,10}", RegexOptions.Compiled);

        string result = input;

        foreach (Match m in hexRegex.Matches(input))
        {
            string token = m.Value;
            if (!IsErrorCode(token)) continue;
            if (uint.TryParse(token.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint u))
            {
                int code = unchecked((int)u);
                string friendly = TurnErrorCodeIntoString(code);
                if (!string.IsNullOrEmpty(friendly))
                    result = result.Replace(token, friendly);
            }
        }

        foreach (Match m in decRegex.Matches(input))
        {
            string token = m.Value;
            if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intVal) && intVal < 0)
            {
                string friendly = TurnErrorCodeIntoString(intVal);
                if (!string.IsNullOrEmpty(friendly))
                    result = result.Replace(token, friendly);
            }
        }

        return result;
    }

    private record CiToolOutput([property: JsonPropertyName("Policies")] List<Policy> Policies);
    private record Policy(
        [property: JsonPropertyName("PolicyID")] string PolicyId,
        [property: JsonPropertyName("IsEnforced")] bool IsEnforced,
        [property: JsonPropertyName("IsAuthorized")] bool IsAuthorized);
}