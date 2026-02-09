using NAudio.CoreAudioApi;
using System.Data;
using System.Management;
using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Text;
using System.Globalization;

static class Program
{
    // Known PC Speaker/Beep channel GUIDs with their English names
    private static readonly Dictionary<string, string> PCBeepGUIDs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "{185FEDF0-9905-11D1-95A9-00C04FB925D3}", "PC Beep Volume" },
        { "{185FEDF1-9905-11D1-95A9-00C04FB925D3}", "PC Beep Mute" },
        { "{185FEDFF-9905-11D1-95A9-00C04FB925D3}", "PC Speaker" },
        { "{185FEE00-9905-11D1-95A9-00C04FB925D3}", "PC Speaker (Related)" }
    };

    // Common media category GUIDs with their English names (hardcoded fallback)
    private static readonly Dictionary<string, string> KnownMediaCategoryNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // PC Speaker/Beep related
        { "{185FEDF0-9905-11D1-95A9-00C04FB925D3}", "PC Beep Volume" },
        { "{185FEDF1-9905-11D1-95A9-00C04FB925D3}", "PC Beep Mute" },
        { "{185FEDFF-9905-11D1-95A9-00C04FB925D3}", "PC Speaker" },
        { "{185FEE00-9905-11D1-95A9-00C04FB925D3}", "PC Speaker (Related)" },
        
        // Common audio categories
        { "{6994AD04-93EF-11D0-A3CC-00A0C9223196}", "Master Volume" },
        { "{6994AD05-93EF-11D0-A3CC-00A0C9223196}", "Master Mute" },
        { "{185FEDE0-9905-11D1-95A9-00C04FB925D3}", "Line In Volume" },
        { "{185FEDE1-9905-11D1-95A9-00C04FB925D3}", "Line In Mute" },
        { "{185FEDE2-9905-11D1-95A9-00C04FB925D3}", "Microphone Volume" },
        { "{185FEDE3-9905-11D1-95A9-00C04FB925D3}", "Microphone Mute" },
        { "{185FEDE4-9905-11D1-95A9-00C04FB925D3}", "CD Audio Volume" },
        { "{185FEDE5-9905-11D1-95A9-00C04FB925D3}", "CD Audio Mute" },
        { "{185FEDE6-9905-11D1-95A9-00C04FB925D3}", "Auxiliary Volume" },
        { "{185FEDE7-9905-11D1-95A9-00C04FB925D3}", "Auxiliary Mute" },
        { "{185FEDE8-9905-11D1-95A9-00C04FB925D3}", "Wave Out Volume" },
        { "{185FEDE9-9905-11D1-95A9-00C04FB925D3}", "Wave Out Mute" },
        { "{185FEDEA-9905-11D1-95A9-00C04FB925D3}", "MIDI Out Volume" },
        { "{185FEDEB-9905-11D1-95A9-00C04FB925D3}", "MIDI Out Mute" },
        { "{185FEDEC-9905-11D1-95A9-00C04FB925D3}", "Headphones Volume" },
        { "{185FEDED-9905-11D1-95A9-00C04FB925D3}", "Headphones Mute" },
        { "{185FEDEE-9905-11D1-95A9-00C04FB925D3}", "Stereo Mix Volume" },
        { "{185FEDEF-9905-11D1-95A9-00C04FB925D3}", "Stereo Mix Mute" },
        { "{185FEDF2-9905-11D1-95A9-00C04FB925D3}", "Mono Mix Volume" },
        { "{185FEDF3-9905-11D1-95A9-00C04FB925D3}", "Mono Mix Mute" },
        { "{185FEDF4-9905-11D1-95A9-00C04FB925D3}", "Front Volume" },
        { "{185FEDF5-9905-11D1-95A9-00C04FB925D3}", "Front Mute" },
        { "{185FEDF6-9905-11D1-95A9-00C04FB925D3}", "Surround Volume" },
        { "{185FEDF7-9905-11D1-95A9-00C04FB925D3}", "Surround Mute" },
        { "{185FEDF8-9905-11D1-95A9-00C04FB925D3}", "Center Volume" },
        { "{185FEDF9-9905-11D1-95A9-00C04FB925D3}", "Center Mute" },
        { "{185FEDFA-9905-11D1-95A9-00C04FB925D3}", "LFE Volume" },
        { "{185FEDFB-9905-11D1-95A9-00C04FB925D3}", "LFE Mute" },
        { "{185FEDFC-9905-11D1-95A9-00C04FB925D3}", "S/PDIF Volume" },
        { "{185FEDFD-9905-11D1-95A9-00C04FB925D3}", "S/PDIF Mute" }
    };

    // P/Invoke declarations for loading string resources
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "LoadStringW")]
    private static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);

    private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
    private const uint LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x00000020;

    static void Main()
    {
        // To find and research about "PC Beep" or equivalent channel that stole about 8 years of my life (July 2018-January 2026)
        // (also it's the reason why "2023-[year]" is written in "About NeoBleeper" instead of "2018-[year]" because I couldn't start developing NeoBleeper until I mounted a system speaker to my desktop PC and played tunes "accidentally" through it in mid-2023)
        PrintColorASCIIArtLogo();
        ListAndMarkMediaChannels();
    }

    private static void PrintColorASCIIArtLogo()
    {
        Console.WriteLine();
        string ASCIIArt = "   _____                       _    _____ _                            _ \r\n  / ____|                     | |  / ____| |                          | |\r\n | (___   ___  _   _ _ __   __| | | |    | |__   __ _ _ __  _ __   ___| |\r\n  \\___ \\ / _ \\| | | | '_ \\ / _` | | |    | '_ \\ / _` | '_ \\| '_ \\ / _ \\ |\r\n  ____) | (_) | |_| | | | | (_| | | |____| | | | (_| | | | | | | |  __/ |\r\n |_____/ \\___/ \\__,_|_| |_|\\__,_| _\\_____|_| |_|\\__,_|_| |_|_| |_|\\___|_|\r\n |_   _|                         | |                                     \r\n   | |  _ __  ___ _ __   ___  ___| |_ ___  _ __                          \r\n   | | | '_ \\/ __| '_ \\ / _ \\/ __| __/ _ \\| '__|                         \r\n  _| |_| | | \\__ \\ |_) |  __/ (__| || (_) | |                            \r\n |_____|_| |_|___/ .__/ \\___|\\___|\\__\\___/|_|                            \r\n                 | |                                                     \r\n                 |_|                                                     ";
        if (!(Environment.OSVersion.Version.Major < 10 || (Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build < 22000)))
        {
            // 2-color gradient from Red to Pink
            int rStart = 255, gStart = 0, bStart = 0;
            int rEnd = 255, gEnd = 0, bEnd = 180;

            for (int i = 0; i < ASCIIArt.Length; i++)
            {
                double t = (double)i / ASCIIArt.Length;
                int r = (int)(rStart + (rEnd - rStart) * t);
                int g = (int)(gStart + (gEnd - gStart) * t);
                int b = (int)(bStart + (bEnd - bStart) * t);
                Console.Write($"\u001b[38;2;{r};{g};{b}m{ASCIIArt[i]}");
            }
            Console.Write("\u001b[0m");
            Console.WriteLine();
        }
        else
        {
            // Fallback: single color
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ASCIIArt);

        }

        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== NeoBleeper Sound Channel Inspector ===\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Enumerating all audio endpoints and hidden channels...\n");
        Console.ResetColor();
    }


    private static void ListAndMarkMediaChannels()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Sound Channel Names:");
        Console.ResetColor();

        string mediaCategoriesPath = @"SYSTEM\CurrentControlSet\Control\MediaCategories";

        try
        {
            // First, collect all visible/enabled channels
            var visibleChannels = GetVisibleMediaChannels();

            using (var key = Registry.LocalMachine.OpenSubKey(mediaCategoriesPath))
            {
                if (key == null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Warning: Media Categories registry key not found.");
                    Console.ResetColor();
                    return;
                }

                var subKeyNames = key.GetSubKeyNames();
                Console.WriteLine($"Found {subKeyNames.Length} total media categories\n");

                var enabledChannels = new List<(string name, string guid, bool isPCBeep)>();
                var disabledChannels = new List<(string name, string guid, bool isPCBeep)>();

                foreach (var subKeyName in subKeyNames)
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey == null) continue;

                        // Get English name (hardcoded if known, otherwise try to load from resources)
                        string englishName = GetEnglishChannelName(subKeyName, subKey);

                        // Check if this channel is visible/enabled
                        bool isEnabled = visibleChannels.Contains(subKeyName);

                        // Check if this is a PC Beep/Speaker related channel
                        bool isPCBeepChannel = PCBeepGUIDs.ContainsKey(subKeyName);

                        if (isEnabled)
                        {
                            enabledChannels.Add((englishName, subKeyName, isPCBeepChannel));
                        }
                        else
                        {
                            disabledChannels.Add((englishName, subKeyName, isPCBeepChannel));
                        }
                    }
                }

                // Sort enabled channels alphabetically by name
                enabledChannels.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));

                // Sort disabled channels: first by isPCBeep (true first), then by name
                disabledChannels.Sort((a, b) =>
                {
                    int beepCompare = b.isPCBeep.CompareTo(a.isPCBeep); // true before false
                    if (beepCompare != 0) return beepCompare;
                    return string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
                });

                // Print enabled channels
                foreach (var (name, guid, isPCBeep) in enabledChannels)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(name);
                    Console.ResetColor();

                    // Add special note for PC Beep channels
                    if (isPCBeep)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(" -> This is a PC Speaker/Beep channel!");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }

                // Print disabled channels
                foreach (var (name, guid, isPCBeep) in disabledChannels)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(name);
                    Console.ResetColor();

                    // Add special note for PC Beep channels
                    if (isPCBeep)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(" -> This is a PC Speaker/Beep channel!");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\nSummary: {enabledChannels.Count} enabled channels out of {subKeyNames.Length} total categories");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error reading Media Categories: {ex.Message}");
            Console.ResetColor();
        }
    }

    // -----------------------------------------------------------------------
    // Cached reverse-lookup: localized string -> English string
    // Built once from known multimedia DLLs by scanning all string IDs in bulk.
    // -----------------------------------------------------------------------

    private static readonly object _cacheLock = new object();
    private static Dictionary<string, string> _localizedToEnglishCache;

    /// <summary>
    /// Looks up a localized channel name in a pre-built cache that maps
    /// localized strings to their English equivalents from known MUI DLLs.
    /// The cache is built lazily on the first call (one-time cost).
    /// </summary>
    private static string TryResolveFromKnownMuiDlls(string localizedName)
    {
        EnsureMuiCacheBuilt();

        if (_localizedToEnglishCache != null &&
            _localizedToEnglishCache.TryGetValue(localizedName, out string englishName))
        {
            return englishName;
        }

        return null;
    }

    private static void EnsureMuiCacheBuilt()
    {
        if (_localizedToEnglishCache != null)
            return;

        lock (_cacheLock)
        {
            if (_localizedToEnglishCache != null)
                return;

            var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);

            // Known DLLs that contain media category strings
            string[] knownDlls = ["mmres.dll", "mmsys.cpl", "mmdevapi.dll"];

            foreach (string dll in knownDlls)
            {
                string dllPath = Path.Combine(system32, dll);
                if (!File.Exists(dllPath))
                    continue;

                string enUsMuiPath = TryFindEnUsMuiFile(dllPath);
                if (string.IsNullOrEmpty(enUsMuiPath) || !File.Exists(enUsMuiPath))
                    continue;

                // Also find the tr-TR (current language) MUI file
                string trTrMuiPath = TryFindCurrentLanguageMuiFile(dllPath);

                // Load the main DLL
                IntPtr hMain = LoadLibraryEx(dllPath, IntPtr.Zero,
                    LOAD_LIBRARY_AS_DATAFILE | LOAD_LIBRARY_AS_IMAGE_RESOURCE);

                // Load tr-TR MUI (if exists) — this has the localized strings
                IntPtr hLocalized = IntPtr.Zero;
                if (!string.IsNullOrEmpty(trTrMuiPath) && File.Exists(trTrMuiPath))
                {
                    hLocalized = LoadLibraryEx(trTrMuiPath, IntPtr.Zero,
                        LOAD_LIBRARY_AS_DATAFILE | LOAD_LIBRARY_AS_IMAGE_RESOURCE);
                }

                // If no tr-TR MUI, fall back to main DLL for localized strings
                if (hLocalized == IntPtr.Zero)
                    hLocalized = hMain;

                // Load the en-US MUI file
                IntPtr hEnUs = LoadLibraryEx(enUsMuiPath, IntPtr.Zero,
                    LOAD_LIBRARY_AS_DATAFILE | LOAD_LIBRARY_AS_IMAGE_RESOURCE);

                if (hEnUs == IntPtr.Zero)
                {
                    if (hLocalized != IntPtr.Zero && hLocalized != hMain) FreeLibrary(hLocalized);
                    if (hMain != IntPtr.Zero) FreeLibrary(hMain);
                    continue;
                }

                try
                {
                    // Build both string tables in bulk, then match by ID
                    var localizedStrings = LoadAllStringsFromModule(hLocalized);
                    var englishStrings = LoadAllStringsFromModule(hEnUs);

                    // --- DIAGNOSTIC ---
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  [CACHE-DIAG] {dll}: localized={localizedStrings.Count} strings, english={englishStrings.Count} strings");
                    // Print first 5 entries from each for inspection
                    int count = 0;
                    foreach (var (id, s) in localizedStrings)
                    {
                        englishStrings.TryGetValue(id, out string en);
                        Console.WriteLine($"    ID={id}: loc=\"{s}\" -> en=\"{en}\"");
                        if (++count >= 5) break;
                    }
                    Console.ResetColor();
                    // --- END DIAGNOSTIC ---

                    foreach (var (id, locStr) in localizedStrings)
                    {
                        if (englishStrings.TryGetValue(id, out string enStr) &&
                            !string.IsNullOrEmpty(enStr) &&
                            !cache.ContainsKey(locStr))
                        {
                            cache[locStr] = enStr;
                        }
                    }
                }
                finally
                {
                    FreeLibrary(hEnUs);
                    if (hLocalized != IntPtr.Zero && hLocalized != hMain) FreeLibrary(hLocalized);
                    if (hMain != IntPtr.Zero) FreeLibrary(hMain);
                }
            }

            _localizedToEnglishCache = cache;
        }
    }

    /// <summary>
    /// Loads all strings from a module's RT_STRING resources by enumerating
    /// resource names, avoiding the need for a brute-force ID scan.
    /// </summary>
    private static Dictionary<uint, string> LoadAllStringsFromModule(IntPtr hModule)
    {
        var result = new Dictionary<uint, string>();

        // Enumerate all RT_STRING blocks using EnumResourceNames
        var blockIds = new List<ushort>();
        EnumResNameProc callback = (hMod, lpType, lpName, lParam) =>
        {
            // lpName is an integer ID (MAKEINTRESOURCE) for RT_STRING blocks
            if (((long)lpName & 0xFFFF0000) == 0)
            {
                blockIds.Add((ushort)(long)lpName);
            }
            return true;
        };

        EnumResourceNames(hModule, (IntPtr)RT_STRING, callback, IntPtr.Zero);
        GC.KeepAlive(callback);

        // Language IDs to try when reading each block
        ushort[] langs = [0x0000, 0x0409, 0x041F, 0x0009];

        foreach (ushort blockId in blockIds)
        {
            // Each block contains string IDs: (blockId - 1) * 16  ..  (blockId - 1) * 16 + 15
            uint baseId = ((uint)blockId - 1) * 16;

            foreach (ushort lang in langs)
            {
                IntPtr hResInfo = FindResourceEx(hModule, (IntPtr)RT_STRING, (IntPtr)blockId, lang);
                if (hResInfo == IntPtr.Zero)
                    continue;

                IntPtr hResData = LoadResource(hModule, hResInfo);
                if (hResData == IntPtr.Zero)
                    continue;

                IntPtr pRes = LockResource(hResData);
                if (pRes == IntPtr.Zero)
                    continue;

                uint size = SizeofResource(hModule, hResInfo);
                if (size == 0)
                    continue;

                // Parse all 16 strings in this block
                int offsetBytes = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (offsetBytes + 2 > size)
                        break;

                    ushort charCount = (ushort)Marshal.ReadInt16(pRes, offsetBytes);
                    offsetBytes += 2;

                    int byteCount = charCount * 2;
                    if (offsetBytes + byteCount > size)
                        break;

                    if (charCount > 0)
                    {
                        uint stringId = baseId + (uint)i;
                        if (!result.ContainsKey(stringId))
                        {
                            result[stringId] = Marshal.PtrToStringUni(pRes + offsetBytes, charCount);
                        }
                    }

                    offsetBytes += byteCount;
                }

                // Found strings for this block, skip other languages
                break;
            }
        }

        return result;
    }

    private delegate bool EnumResNameProc(IntPtr hModule, IntPtr lpType, IntPtr lpName, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpType, EnumResNameProc lpEnumFunc, IntPtr lParam);


    /// <summary>
    /// Loads a string from a module trying multiple language IDs.
    /// </summary>
    private static string LoadAnyLangString(IntPtr hModule, uint stringId)
    {
        // Try common language tags
        ushort[] langs = [0x0000, 0x0409, 0x041F, 0x0009];
        foreach (ushort lang in langs)
        {
            string s = LoadStringFromModuleWithLang(hModule, stringId, lang);
            if (!string.IsNullOrEmpty(s))
                return s;
        }
        return null;
    }

    private static string TryFindCurrentLanguageMuiFile(string modulePath)
    {
        try
        {
            string dir = Path.GetDirectoryName(modulePath) ?? string.Empty;
            string fileName = Path.GetFileName(modulePath);
            string cultureName = CultureInfo.CurrentUICulture.Name; // e.g., "tr-TR"

            string candidate = Path.Combine(dir, cultureName, fileName + ".mui");
            if (File.Exists(candidate)) return candidate;

            string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
            candidate = Path.Combine(system32, cultureName, fileName + ".mui");
            if (File.Exists(candidate)) return candidate;
        }
        catch { }
        return null;
    }

    // Eklenen/Değiştirilen bölümler: LoadEnglishStringFromResource genişletildi; ek yardımcı metodlar eklendi; SetThreadUILanguage DllImport eklendi.
    // Yapılan değişiklikler, kaynak referanslarından "unlocalized" (LANG_NEUTRAL) veya İngilizce (en-US) stringleri güvenilir şekilde elde etmeyi amaçlar.
    // Aşağıdaki kod parçacığını mevcut `Program.cs` dosyanızdaki aynı isimli method'un yerine yapıştırın.
    // Ayrıca bu blokta kullanılan DllImport'lar (SetThreadUILanguage) mevcut P/Invoke bloğunuzla aynı bölüme eklenmelidir.

    private const ushort LANG_EN_US = 0x0409;

    [DllImport("kernel32.dll")]
    private static extern ushort SetThreadUILanguage(ushort langId);

    private static string LoadEnglishStringFromResource(string resourceReference)
    {
        if (string.IsNullOrWhiteSpace(resourceReference))
            return null;

        if (!resourceReference.StartsWith("@", StringComparison.Ordinal))
            return null;

        var parts = resourceReference.Substring(1).Split(new[] { ',' }, 2, StringSplitOptions.None);
        if (parts.Length != 2)
            return null;

        string modulePath = parts[0].Trim();
        string idPart = parts[1].Trim();

        if (!int.TryParse(idPart.TrimStart('-'), NumberStyles.Integer, CultureInfo.InvariantCulture, out int resourceIdSigned))
            return null;

        uint resourceId = (uint)Math.Abs(resourceIdSigned);

        modulePath = Environment.ExpandEnvironmentVariables(modulePath);

        if (!Path.IsPathRooted(modulePath))
        {
            string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
            modulePath = Path.Combine(system32, modulePath);
        }

        if (!modulePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
            !modulePath.EndsWith(".cpl", StringComparison.OrdinalIgnoreCase) &&
            !modulePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            modulePath += ".dll";
        }

        // 1) HIGHEST PRIORITY: Try en-US MUI file first — this is the most reliable
        //    source for English strings on a non-English OS.
        //    MUI files store strings as LANG_NEUTRAL inside the file itself.
        string muiCandidate = TryFindEnUsMuiFile(modulePath);
        if (!string.IsNullOrEmpty(muiCandidate) && File.Exists(muiCandidate))
        {
            IntPtr hMui = LoadLibraryEx(muiCandidate, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE | LOAD_LIBRARY_AS_IMAGE_RESOURCE);
            if (hMui != IntPtr.Zero)
            {
                try
                {
                    // MUI files typically embed strings as LANG_NEUTRAL
                    var s = LoadStringFromModuleWithLang(hMui, resourceId, 0x0000);
                    if (!string.IsNullOrEmpty(s))
                        return s;

                    // Also try explicit en-US language tag
                    s = LoadStringFromModuleWithLang(hMui, resourceId, LANG_EN_US);
                    if (!string.IsNullOrEmpty(s))
                        return s;

                    // Try LANG_ENGLISH_NEUTRAL (0x0009) — some resources use sublang-neutral English
                    s = LoadStringFromModuleWithLang(hMui, resourceId, 0x0009);
                    if (!string.IsNullOrEmpty(s))
                        return s;
                }
                finally
                {
                    FreeLibrary(hMui);
                }
            }
        }

        // 2) Try SHLoadIndirectString with thread UI language forced to en-US.
        //    This uses Windows' own MUI resolution with en-US preference.
        try
        {
            ushort prev = SetThreadUILanguage(LANG_EN_US);
            try
            {
                StringBuilder sb = new StringBuilder(512);
                int hr = SHLoadIndirectString(resourceReference, sb, (uint)sb.Capacity, IntPtr.Zero);
                if (hr == 0)
                {
                    string outStr = sb.ToString();
                    if (!string.IsNullOrWhiteSpace(outStr))
                        return outStr;
                }
            }
            finally
            {
                SetThreadUILanguage(prev);
            }
        }
        catch
        {
            // swallow — best-effort fallback
        }

        // 3) LAST RESORT: Try main module with en-US only (skip LANG_NEUTRAL here
        //    because on a Turkish OS, LANG_NEUTRAL resolves to Turkish).
        if (File.Exists(modulePath))
        {
            IntPtr hModule = LoadLibraryEx(modulePath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE | LOAD_LIBRARY_AS_IMAGE_RESOURCE);
            if (hModule != IntPtr.Zero)
            {
                try
                {
                    var s = LoadStringFromModuleWithLang(hModule, resourceId, LANG_EN_US);
                    if (!string.IsNullOrEmpty(s))
                        return s;
                }
                finally
                {
                    FreeLibrary(hModule);
                }
            }
        }

        return null;
    }

    // Yardımcı: en-US .mui dosyasının olası yollarını dener
    private static string TryFindEnUsMuiFile(string modulePath)
    {
        try
        {
            string dir = Path.GetDirectoryName(modulePath) ?? string.Empty;
            string fileName = Path.GetFileName(modulePath);

            // 1) same folder -> en-US subfolder
            string candidate = Path.Combine(dir, "en-US", fileName + ".mui");
            if (File.Exists(candidate)) return candidate;

            // 2) system32\en-US\<file>.mui
            string system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
            candidate = Path.Combine(system32, "en-US", fileName + ".mui");
            if (File.Exists(candidate)) return candidate;

            // 3) try adding .mui next to module (module.mui)
            candidate = modulePath + ".mui";
            if (File.Exists(candidate)) return candidate;

            // 4) try fallback to WinSxS — expensive, optional (not implemented here)
        }
        catch { }
        return null;
    }

    private static string LoadStringFromModuleWithLang(IntPtr hModule, uint stringId, ushort langId)
    {
        // Windows string tables:
        // - RT_STRING (type=6)
        // - Each resource block contains 16 strings.
        // - Block ID = (stringId / 16) + 1
        // - Index in block = stringId % 16
        uint blockId = (stringId / 16) + 1;
        int indexInBlock = (int)(stringId % 16);

        IntPtr hResInfo = FindResourceEx(hModule, (IntPtr)RT_STRING, (IntPtr)blockId, langId);
        if (hResInfo == IntPtr.Zero)
            return null;

        IntPtr hResData = LoadResource(hModule, hResInfo);
        if (hResData == IntPtr.Zero)
            return null;

        IntPtr pRes = LockResource(hResData);
        if (pRes == IntPtr.Zero)
            return null;

        uint size = SizeofResource(hModule, hResInfo);
        if (size == 0)
            return null;

        // Parse UTF-16 string-table format: sequence of (WORD length, WCHAR[length] text)
        int offsetBytes = 0;

        for (int i = 0; i < 16; i++)
        {
            if (offsetBytes + 2 > size)
                return null;

            ushort charCount = (ushort)Marshal.ReadInt16(pRes, offsetBytes);
            offsetBytes += 2;

            int byteCount = charCount * 2;
            if (offsetBytes + byteCount > size)
                return null;

            if (i == indexInBlock)
            {
                if (charCount == 0)
                    return null;

                return Marshal.PtrToStringUni(pRes + offsetBytes, charCount);
            }

            offsetBytes += byteCount;
        }

        return null;
    }

    private const int RT_STRING = 6;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpType, IntPtr lpName, ushort wLanguage);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

    [DllImport("kernel32.dll", SetLastError = false)]
    private static extern IntPtr LockResource(IntPtr hResData);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);
    private static HashSet<string> GetVisibleMediaChannels()
    {
        var visibleChannels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            // Check audio devices and their topology to find which media categories are actually in use
            using var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);

            foreach (var device in devices)
            {
                try
                {
                    // Try to extract GUIDs from device properties
                    var propertyStore = device.Properties;

                    // Common property keys that might contain category GUIDs
                    for (int i = 0; i < propertyStore.Count; i++)
                    {
                        try
                        {
                            var prop = propertyStore[i];
                            if (prop.Value != null)
                            {
                                string valueStr = prop.Value.ToString();
                                // Look for "GUID" patterns in the value
                                if (valueStr.Contains("{") && valueStr.Contains("}"))
                                {
                                    var guidMatch = System.Text.RegularExpressions.Regex.Match(
                                        valueStr,
                                        @"\{[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}\}"
                                    );

                                    if (guidMatch.Success)
                                    {
                                        visibleChannels.Add(guidMatch.Value);
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }

            // Also check the mixer device for enabled channels
            string mixerPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Render";
            try
            {
                using (var mixerKey = Registry.LocalMachine.OpenSubKey(mixerPath))
                {
                    if (mixerKey != null)
                    {
                        foreach (var deviceId in mixerKey.GetSubKeyNames())
                        {
                            using (var deviceKey = mixerKey.OpenSubKey($"{deviceId}\\Properties"))
                            {
                                if (deviceKey != null)
                                {
                                    foreach (var valueName in deviceKey.GetValueNames())
                                    {
                                        try
                                        {
                                            var value = deviceKey.GetValue(valueName);
                                            if (value != null)
                                            {
                                                string valueStr = value.ToString();
                                                if (valueStr.Contains("{") && valueStr.Contains("}"))
                                                {
                                                    var guidMatch = System.Text.RegularExpressions.Regex.Match(
                                                        valueStr,
                                                        @"\{[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}\}"
                                                    );

                                                    if (guidMatch.Success)
                                                    {
                                                        visibleChannels.Add(guidMatch.Value);
                                                    }
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }
        catch { }

        return visibleChannels;
    }

    private static string FormatRegistryValue(object value, RegistryValueKind kind)
    {
        if (value == null)
            return "(null)";

        switch (kind)
        {
            case RegistryValueKind.DWord:
            case RegistryValueKind.QWord:
                return $"{value} (0x{Convert.ToInt64(value):X})";

            case RegistryValueKind.String:
            case RegistryValueKind.ExpandString:
                return $"\"{value}\"";

            case RegistryValueKind.Binary:
                byte[] bytes = (byte[])value;
                if (bytes.Length > 16)
                    return $"[Binary {bytes.Length} bytes]";
                return BitConverter.ToString(bytes).Replace("-", " ");

            case RegistryValueKind.MultiString:
                string[] strings = (string[])value;
                return $"[{strings.Length} strings]";

            default:
                return value.ToString();
        }
    }
    // Ekle: SHLoadIndirectString bildirimi (mevcut P/Invoke bloklarının yanına)
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);
    // Complete mapping of standard Windows KS Category GUIDs to English names.
    // Since Windows writes these as localized plain text in the registry (without resource DLL refs),
    // hardcoding the English names for these known GUIDs is the ONLY way to get them in English on a non-English OS.
    private static readonly Dictionary<string, string> GuidToEnglishName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // --- KS Node Types (ksmedia.h) ---
        { "{00DFF077-96E3-11d2-AC4C-00C04F8EFB68}", "Stereo Mix" },
        { "{00DFF078-96E3-11d2-AC4C-00C04F8EFB68}", "Mono Mix" },
        { "{02B223C0-C557-11D0-8A2B-00A0C9255AC1}", "Mute" },
        { "{085AFF00-62CE-11CF-A5D6-28DB04C10000}", "Bridge" },
        { "{0A4252A0-7E70-11D0-A5D6-28DB04C10000}", "Splitter" },
        { "{144981E0-C558-11D0-8A2B-00A0C9255AC1}", "Delay" },
        { "{15DC9025-22AD-41b3-8875-F4CEB0299E20}", "Digital Audio (S/PDIF)" },
        { "{185FEDE0-9905-11D1-95A9-00C04FB925D3}", "Bass" },
        { "{185FEDE1-9905-11D1-95A9-00C04FB925D3}", "Treble" },
        { "{185FEDE2-9905-11D1-95A9-00C04FB925D3}", "3D Stereo" },
        { "{185FEDE3-9905-11D1-95A9-00C04FB925D3}", "Speakers" },
        { "{185FEDE4-9905-11D1-95A9-00C04FB925D3}", "Master Mute" },
        { "{185FEDE5-9905-11D1-95A9-00C04FB925D3}", "Wave Volume" },
        { "{185FEDE6-9905-11D1-95A9-00C04FB925D3}", "Wave Mute" },
        { "{185FEDE7-9905-11D1-95A9-00C04FB925D3}", "MIDI Volume" },
        { "{185FEDE8-9905-11D1-95A9-00C04FB925D3}", "MIDI Mute" },
        { "{185FEDE9-9905-11D1-95A9-00C04FB925D3}", "CD Volume" },
        { "{185FEDEA-9905-11D1-95A9-00C04FB925D3}", "CD Mute" },
        { "{185FEDEB-9905-11D1-95A9-00C04FB925D3}", "Line Volume" },
        { "{185FEDEC-9905-11D1-95A9-00C04FB925D3}", "Line Mute" },
        { "{185FEDED-9905-11D1-95A9-00C04FB925D3}", "Microphone Volume" },
        { "{185FEDEE-9905-11D1-95A9-00C04FB925D3}", "Microphone Mute" },
        { "{185FEDEF-9905-11D1-95A9-00C04FB925D3}", "Recording Source" },
        { "{185FEDF0-9905-11D1-95A9-00C04FB925D3}", "PC Speaker Volume" },
        { "{185FEDF1-9905-11D1-95A9-00C04FB925D3}", "PC Speaker Mute" },
        { "{185FEDF2-9905-11D1-95A9-00C04FB925D3}", "MIDI In Volume" },
        { "{185FEDF3-9905-11D1-95A9-00C04FB925D3}", "CD In Volume" },
        { "{185FEDF4-9905-11D1-95A9-00C04FB925D3}", "Line In Volume" },
        { "{185FEDF5-9905-11D1-95A9-00C04FB925D3}", "Mic In Volume" },
        { "{185FEDF6-9905-11D1-95A9-00C04FB925D3}", "Wave In Volume" },
        { "{185FEDF7-9905-11D1-95A9-00C04FB925D3}", "Volume Control" },
        { "{185FEDF8-9905-11D1-95A9-00C04FB925D3}", "MIDI" },
        { "{185FEDF9-9905-11D1-95A9-00C04FB925D3}", "Line In" },
        { "{185FEDFA-9905-11D1-95A9-00C04FB925D3}", "Recording Control" },
        { "{185FEDFB-9905-11D1-95A9-00C04FB925D3}", "CD Audio" },
        { "{185FEDFC-9905-11D1-95A9-00C04FB925D3}", "Auxiliary Volume" },
        { "{185FEDFD-9905-11D1-95A9-00C04FB925D3}", "Auxiliary Mute" },
        { "{185FEDFE-9905-11D1-95A9-00C04FB925D3}", "Auxiliary" },
        { "{185FEDFF-9905-11D1-95A9-00C04FB925D3}", "PC Speaker" },
        { "{185FEE00-9905-11D1-95A9-00C04FB925D3}", "Wave Out Mix" },
        { "{1A71EBE0-959E-11D1-B448-00A0C9255AC1}", "Bass Boost" },
        { "{1AD247EB-96E3-11d2-AC4C-00C04F8EFB68}", "Mono Out Volume" },
        { "{1AD247EC-96E3-11d2-AC4C-00C04F8EFB68}", "Mono Out Mute" },
        { "{1AD247ED-96E3-11d2-AC4C-00C04F8EFB68}", "Stereo Mix Volume" },
        { "{1E84C900-7E70-11D0-A5D6-28DB04C10000}", "Compressor" },
        { "{20173F20-C559-11D0-8A2B-00A0C9255AC1}", "Chorus" },
        { "{21FBB329-1A4A-48da-A076-2318A3C59B26}", "Digital Audio (DisplayPort)" },
        { "{22B0EAFD-96E3-11d2-AC4C-00C04F8EFB68}", "Stereo Mix Mute" },
        { "{22B0EAFE-96E3-11d2-AC4C-00C04F8EFB68}", "Mono Mix Volume" },
        { "{2721AE20-7E70-11D0-A5D6-28DB04C10000}", "Decompressor" },
        { "{2BC31D69-96E3-11d2-AC4C-00C04F8EFB68}", "Mono Mix Mute" },
        { "{2BC31D6A-96E3-11d2-AC4C-00C04F8EFB68}", "Microphone Boost" },
        { "{2BC31D6B-96E3-11d2-AC4C-00C04F8EFB68}", "Alternative Microphone" },
        { "{2CEAF780-C556-11D0-8A2B-00A0C9255AC1}", "Mux" },
        { "{2EB07EA0-7E70-11D0-A5D6-28DB04C10000}", "Data Transform" },
        { "{387BFC03-E7EF-4901-86E0-35B7C32B00EF}", "Digital Audio (HDMI)" },
        { "{3A264481-E52C-4b82-8E7A-C8E2F91DC380}", "Digital Audio (S/PDIF)" },
        { "{3A5ACC00-C557-11D0-8A2B-00A0C9255AC1}", "Volume" },
        { "{41887440-C558-11D0-8A2B-00A0C9255AC1}", "Loudness" },
        { "{423274A0-8B81-11D1-A050-0000F8004788}", "SW Synth" },
        { "{497B34AD-D67F-411c-8076-80D5B4250D67}", "HD Audio Headphone" },
        { "{4D837FE0-C555-11D0-8A2B-00A0C9255AC1}", "ADC" },
        { "{507AE360-C554-11D0-8A2B-00A0C9255AC1}", "DAC" },
        { "{55515860-C559-11D0-8A2B-00A0C9255AC1}", "3D Effects" },
        { "{57E24340-FC5B-4612-A562-72B11A29DFAE}", "Peak Meter" },
        { "{63FF5747-991F-11d2-AC4D-00C04F8EFB68}", "3D Depth" },
        { "{65E8773D-8F56-11D0-A3B9-00A0C9223196}", "Capture" },
        { "{65E8773E-8F56-11D0-A3B9-00A0C9223196}", "Render" },
        { "{6994AD04-93EF-11D0-A3CC-00A0C9223196}", "Master Volume" },
        { "{6994AD05-93EF-11D0-A3CC-00A0C9223196}", "Master Mute" },
        { "{7607E580-C557-11D0-8A2B-00A0C9255AC1}", "Tone" },
        { "{831C2C80-C558-11D0-8A2B-00A0C9255AC1}", "Dolby ProLogic Decoder" },
        { "{915DAEC4-A434-11d2-AC52-00C04F8EFB68}", "Video" },
        { "{941C7AC0-C559-11D0-8A2B-00A0C9255AC1}", "Device Specific" },
        { "{947FCC8F-33C8-4896-9B84-F9466BB75CF6}", "Internal Speaker/Headphone" },
        { "{9B46E708-992A-11d2-AC4D-00C04F8EFB68}", "Video Volume" },
        { "{9B46E709-992A-11d2-AC4D-00C04F8EFB68}", "Video Mute" },
        { "{9D41B4A0-C557-11D0-8A2B-00A0C9255AC1}", "Equalizer" },
        { "{9DB7B9E0-C555-11D0-8A2B-00A0C9255AC1}", "Sample Rate Converter" },
        { "{9F0670B4-991F-11d2-AC4D-00C04F8EFB68}", "3D Center" },
        { "{A2CBE478-AE84-49A1-8B72-4AD09B78ED34}", "Center" },
        { "{A9E69800-C558-11D0-8A2B-00A0C9255AC1}", "Stereo Wide" },
        { "{AD809C00-7B88-11D0-A5D6-28DB04C10000}", "Mixer" },
        { "{AF6878AC-E83F-11D0-958A-00C04FB925D3}", "Stereo Enhance" },
        { "{BF963D80-C559-11D0-8A2B-00A0C9255AC1}", "Acoustic Echo Cancel" },
        { "{C0EB67D4-E807-11D0-958A-00C04FB925D3}", "Demux" },
        { "{CB9BEFA0-A251-11D1-A050-0000F8004788}", "Microsoft GS Wavetable SW Synth" },
        { "{CF1DDA2C-9743-11D0-A3EE-00A0C9223196}", "Communication Transform" },
        { "{CF1DDA2D-9743-11D0-A3EE-00A0C9223196}", "Interface Transform" },
        { "{CF1DDA2E-9743-11D0-A3EE-00A0C9223196}", "Medium Transform" },
        { "{DA441A60-C556-11D0-8A2B-00A0C9255AC1}", "Sum" },
        { "{DFF21BE1-F70F-11D0-B917-00A0C9223196}", "Microphone" },
        { "{DFF21BE2-F70F-11D0-B917-00A0C9223196}", "Desktop Microphone" },
        { "{DFF21BE3-F70F-11D0-B917-00A0C9223196}", "Head Mounted Display Mic" },
        { "{DFF21BE4-F70F-11D0-B917-00A0C9223196}", "Omni-Directional Microphone" },
        { "{DFF21BE5-F70F-11D0-B917-00A0C9223196}", "Microphone Array" },
        { "{DFF21BE6-F70F-11D0-B917-00A0C9223196}", "Microphone Array" },
        { "{DFF21CE1-F70F-11D0-B917-00A0C9223196}", "Speakers" },
        { "{DFF21CE2-F70F-11D0-B917-00A0C9223196}", "Headphones" },
        { "{DFF21CE3-F70F-11D0-B917-00A0C9223196}", "Head Mounted Display Audio" },
        { "{DFF21CE4-F70F-11D0-B917-00A0C9223196}", "Desktop Speaker" },
        { "{DFF21CE5-F70F-11D0-B917-00A0C9223196}", "Room Speaker" },
        { "{DFF21CE6-F70F-11D0-B917-00A0C9223196}", "Communication Speaker" },
        { "{DFF21CE7-F70F-11D0-B917-00A0C9223196}", "Low Frequency Effects Speaker" },
        { "{DFF21DE1-F70F-11D0-B917-00A0C9223196}", "Handset" },
        { "{DFF21DE2-F70F-11D0-B917-00A0C9223196}", "Headset" },
        { "{DFF21DE3-F70F-11D0-B917-00A0C9223196}", "Speakerphone" },
        { "{DFF21DE4-F70F-11D0-B917-00A0C9223196}", "Echo Suppressing Speakerphone" },
        { "{DFF21DE5-F70F-11D0-B917-00A0C9223196}", "Echo Canceling Speakerphone" },
        { "{DFF21EE1-F70F-11D0-B917-00A0C9223196}", "Phone Line" },
        { "{DFF21EE2-F70F-11D0-B917-00A0C9223196}", "Telephone" },
        { "{DFF21EE3-F70F-11D0-B917-00A0C9223196}", "Down Line Phone" },
        { "{DFF21FE1-F70F-11D0-B917-00A0C9223196}", "Analog Connector" },
        { "{DFF21FE2-F70F-11D0-B917-00A0C9223196}", "Digital Audio Interface" },
        { "{DFF21FE3-F70F-11D0-B917-00A0C9223196}", "Line" },
        { "{DFF21FE4-F70F-11D0-B917-00A0C9223196}", "Wave" },
        { "{DFF21FE5-F70F-11D0-B917-00A0C9223196}", "SPDIF Interface" },
        { "{DFF21FE6-F70F-11D0-B917-00A0C9223196}", "1394 Digital Audio Stream" },
        { "{DFF21FE7-F70F-11D0-B917-00A0C9223196}", "1394 Digital Video Soundtrack" },
        { "{DFF220E1-F70F-11D0-B917-00A0C9223196}", "Level Calibration Noise Source" },
        { "{DFF220E2-F70F-11D0-B917-00A0C9223196}", "Equalization Noise" },
        { "{DFF220E3-F70F-11D0-B917-00A0C9223196}", "CD Player" },
        { "{DFF220E4-F70F-11D0-B917-00A0C9223196}", "DAT" },
        { "{DFF220E5-F70F-11D0-B917-00A0C9223196}", "DCC" },
        { "{DFF220E6-F70F-11D0-B917-00A0C9223196}", "Minidisk" },
        { "{DFF220E7-F70F-11D0-B917-00A0C9223196}", "Analog Tape" },
        { "{DFF220E8-F70F-11D0-B917-00A0C9223196}", "Phonograph" },
        { "{DFF220E9-F70F-11D0-B917-00A0C9223196}", "VCR Audio" },
        { "{DFF220EA-F70F-11D0-B917-00A0C9223196}", "Video Disc Audio" },
        { "{DFF220EB-F70F-11D0-B917-00A0C9223196}", "DVD Audio" },
        { "{DFF220EC-F70F-11D0-B917-00A0C9223196}", "TV Tuner Audio" },
        { "{DFF220ED-F70F-11D0-B917-00A0C9223196}", "Satellite Audio" },
        { "{DFF220EE-F70F-11D0-B917-00A0C9223196}", "Cable Audio" },
        { "{DFF220EF-F70F-11D0-B917-00A0C9223196}", "DSS Audio" },
        { "{DFF220F0-F70F-11D0-B917-00A0C9223196}", "Radio Receiver" },
        { "{DFF220F1-F70F-11D0-B917-00A0C9223196}", "Radio Transmitter" },
        { "{DFF220F2-F70F-11D0-B917-00A0C9223196}", "Multi-track Recorder" },
        { "{DFF220F3-F70F-11D0-B917-00A0C9223196}", "Synthesizer" },
        { "{DFF229E1-F70F-11D0-B917-00A0C9223196}", "Video Streaming" },
        { "{DFF229E2-F70F-11D0-B917-00A0C9223196}", "Video Input Terminal" },
        { "{DFF229E3-F70F-11D0-B917-00A0C9223196}", "Video Output Terminal" },
        { "{DFF229E4-F70F-11D0-B917-00A0C9223196}", "Video Selector" },
        { "{DFF229E5-F70F-11D0-B917-00A0C9223196}", "Video Processing" },
        { "{DFF229E6-F70F-11D0-B917-00A0C9223196}", "Video Camera Terminal" },
        { "{DFF229E7-F70F-11D0-B917-00A0C9223196}", "Video Input Media Transport Terminal" },
        { "{DFF229E8-F70F-11D0-B917-00A0C9223196}", "Video Output Media Transport Terminal" },
        { "{E573ADC0-C555-11D0-8A2B-00A0C9255AC1}", "SuperMix" },
        { "{E88C9BA0-C557-11D0-8A2B-00A0C9255AC1}", "AGC" },
        { "{EEF86A90-3742-4974-B8D2-5370E1C540F6}", "HD Audio Line Out" },
        { "{EF0328E0-C558-11D0-8A2B-00A0C9255AC1}", "Reverb" },
        { "{F06BB67D-5C2F-48ad-A307-B449E3B217D6}", "Disable Digital Output" },
        { "{F9B41DC3-96E2-11d2-AC4C-00C04F8EFB68}", "Mono Out" },
        { "{FB6C4281-0353-11d1-905F-0000C0CC16BA}", "Capture" },
        { "{FB6C4282-0353-11d1-905F-0000C0CC16BA}", "Preview" },
        { "{FB6C4283-0353-11d1-905F-0000C0CC16BA}", "Analog Video In" },
        { "{FB6C4284-0353-11d1-905F-0000C0CC16BA}", "VBI" },
        { "{FB6C4285-0353-11d1-905F-0000C0CC16BA}", "VP" },
        { "{FB6C4287-0353-11d1-905F-0000C0CC16BA}", "EDS" },
        { "{FB6C4288-0353-11d1-905F-0000C0CC16BA}", "Teletext" },
        { "{FB6C4289-0353-11d1-905F-0000C0CC16BA}", "CC" },
        { "{FB6C428A-0353-11d1-905F-0000C0CC16BA}", "Still" },
        { "{FB6C428B-0353-11d1-905F-0000C0CC16BA}", "Timecode" },
        { "{FB6C428C-0353-11d1-905F-0000C0CC16BA}", "VPVBI" },
        { "{FC576919-7CF0-463B-A72F-A5BF64C86EBA}", "MIDI Function" },
        { "{FD4F0300-9632-11D1-B448-00A0C9255AC1}", "Stereo Extender" },
    };

    private static string GetEnglishChannelName(string guid, RegistryKey subKey)
    {
        // 1. Orijinal İngilizce adı standart GUID sözlüğünden al
        if (GuidToEnglishName.TryGetValue(guid, out string englishName))
        {
            return englishName;
        }

        // 2. Sözlükte yoksa, muhtemelen 3. parti veya özel bir sürücü girdisidir.
        // Bu durumda elimizdeki tek isim registry'deki "Name" değeridir (genelde zaten İngilizce olur)
        string name = subKey.GetValue("Name") as string;
        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        return "(No Name)";
    }
}