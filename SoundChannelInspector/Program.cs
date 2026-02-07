using NAudio.CoreAudioApi;
using System.Data;
using System.Management;
using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Runtime.InteropServices;

static class Program
{
    static void Main()
    {
        // To find and research about "PC Beep" or equivalent channel that stole about 8 years of my life (July 2018-January 2026)
        // (also it's the reason why "2023-[year]" is written in "About NeoBleeper" instead of "2018-[year]" because I couldn't start developing NeoBleeper until I mounted a system speaker to my desktop PC and played tunes "accidentally" through it in mid-2023)
        PrintColorASCIIArtLogo();
        EnumerateAllEndpointChannels();
    }

    private static void PrintColorASCIIArtLogo()
    {
        Console.WriteLine();
        string ASCIIArt = "   _____                       _    _____ _                            _ \r\n  / ____|                     | |  / ____| |                          | |\r\n | (___   ___  _   _ _ __   __| | | |    | |__   __ _ _ __  _ __   ___| |\r\n  \\___ \\ / _ \\| | | | '_ \\ / _` | | |    | '_ \\ / _` | '_ \\| '_ \\ / _ \\ |\r\n  ____) | (_) | |_| | | | | (_| | | |____| | | | (_| | | | | | | |  __/ |\r\n |_____/ \\___/ \\__,_|_| |_|\\__,_| _\\_____|_| |_|\\__,_|_| |_|_| |_|\\___|_|\r\n |_   _|                         | |                                     \r\n   | |  _ __  ___ _ __   ___  ___| |_ ___  _ __                          \r\n   | | | '_ \\/ __| '_ \\ / _ \\/ __| __/ _ \\| '__|                         \r\n  _| |_| | | \\__ \\ |_) |  __/ (__| || (_) | |                            \r\n |_____|_| |_|___/ .__/ \\___|\\___|\\__\\___/|_|                            \r\n                 | |                                                     \r\n                 |_|                                                     ";

        // 2-color gradient from Red to Pink
        int rStart = 255, gStart = 80, bStart = 80;
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

        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== NeoBleeper Sound Channel Inspector ===\n");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Enumerating all audio endpoints and hidden channels...\n");
        Console.ResetColor();
    }

    [SupportedOSPlatform("windows")]
    private static void EnumerateAllEndpointChannels()
    {
        using var enumerator = new MMDeviceEnumerator();

        var allDevices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active | DeviceState.Disabled | DeviceState.Unplugged);

        if (allDevices.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No audio endpoints found.");
            Console.ResetColor();
            return;
        }

        // Track hardware filter device IDs already walked to avoid duplicates
        var walkedHardwareDevices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var device in allDevices)
        {
            try
            {
                string stateLabel = device.State switch
                {
                    DeviceState.Active => "Active",
                    DeviceState.Disabled => "Disabled (Hidden)",
                    DeviceState.Unplugged => "Unplugged (Hidden)",
                    DeviceState.NotPresent => "Not Present (Hidden)",
                    _ => device.State.ToString()
                };

                string flowLabel = device.DataFlow == DataFlow.Render ? "Output" : "Input";

                Console.ForegroundColor = device.State == DeviceState.Active ? ConsoleColor.Green : ConsoleColor.DarkGray;
                Console.WriteLine($"─────────────────────────────────────────────");
                Console.WriteLine($"  Device: {device.FriendlyName}");
                Console.WriteLine($"  Flow:   {flowLabel}");
                Console.WriteLine($"  State:  {stateLabel}");
                Console.ResetColor();

                if (device.State == DeviceState.Active)
                {
                    try
                    {
                        var epVol = device.AudioEndpointVolume;
                        int channelCount = epVol.Channels.Count;

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"  Master Volume: {Math.Round(epVol.MasterVolumeLevelScalar * 100)}%");
                        Console.WriteLine($"  Channel Count: {channelCount}");

                        for (int ch = 0; ch < channelCount; ch++)
                        {
                            float level = epVol.Channels[ch].VolumeLevelScalar;
                            Console.WriteLine($"    Channel {ch}: {Math.Round(level * 100)}%");
                        }
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"  (Could not read endpoint volume: {ex.Message})");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"  Volume: N/A (device is {stateLabel.ToLower()})");
                    Console.ResetColor();
                }

                try
                {
                    EnumerateTopologyParts(device, walkedHardwareDevices);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  (Could not read topology: {ex.Message})");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  Error reading device: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // ──────────────────────────────────────────────────────────
    //  DeviceTopology COM interop to discover slider/part names
    // ──────────────────────────────────────────────────────────

    [SupportedOSPlatform("windows")]
    private static void EnumerateTopologyParts(MMDevice device, HashSet<string> walkedHardwareDevices)
    {
        var realDeviceField = typeof(MMDevice).GetField("_realDevice",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (realDeviceField == null)
        {
            realDeviceField = typeof(MMDevice).GetField("deviceInterface",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }

        if (realDeviceField == null)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  (Could not access internal IMMDevice field via reflection)");
            Console.ResetColor();
            return;
        }

        var realDevice = realDeviceField.GetValue(device);
        if (realDevice == null)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  (Internal IMMDevice is null)");
            Console.ResetColor();
            return;
        }

        Guid iidDeviceTopology = typeof(IDeviceTopology).GUID;
        const uint CLSCTX_ALL = 0x17;

        var immDevice = (IMMDevice)realDevice;
        int hr = immDevice.Activate(ref iidDeviceTopology, CLSCTX_ALL, IntPtr.Zero, out object topoObj);

        if (hr != 0 || topoObj == null)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  (IDeviceTopology activation failed, HRESULT: 0x{hr:X8})");
            Console.ResetColor();
            return;
        }

        var topo = (IDeviceTopology)topoObj;
        topo.GetConnectorCount(out uint connectorCount);

        var visitedPartIds = new HashSet<string>();

        // Collect endpoint-visible part global IDs so we can mark others as hidden
        var endpointPartIds = new HashSet<string>();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  Endpoint Topology Parts:");
        Console.ResetColor();

        for (uint c = 0; c < connectorCount; c++)
        {
            topo.GetConnector(c, out IConnector connector);
            try
            {
                connector.IsConnected(out bool isConnected);
                if (!isConnected)
                    continue;

                connector.GetConnectedTo(out IConnector connectedTo);

                IntPtr connectedToPtr = Marshal.GetIUnknownForObject(connectedTo);
                try
                {
                    Guid iidPart = typeof(IPart).GUID;
                    int hrQI = Marshal.QueryInterface(connectedToPtr, ref iidPart, out IntPtr partPtr);
                    if (hrQI == 0 && partPtr != IntPtr.Zero)
                    {
                        try
                        {
                            var part = (IPart)Marshal.GetObjectForIUnknown(partPtr);

                            // Collect endpoint-visible IDs first
                            CollectPartIds(part, endpointPartIds, new HashSet<string>());

                            // Then print them (not hidden)
                            WalkParts(part, depth: 2, visitedPartIds, endpointPartIds, isHardwareFilter: false);

                            // Follow into the hardware filter topology via GetTopologyObject
                            try
                            {
                                part.GetTopologyObject(out IDeviceTopology hwTopo);
                                if (hwTopo != null)
                                {
                                    hwTopo.GetDeviceId(out string hwDeviceId);
                                    if (!string.IsNullOrEmpty(hwDeviceId) &&
                                        walkedHardwareDevices.Add(hwDeviceId))
                                    {
                                        WalkHardwareFilterTopology(hwTopo, visitedPartIds, endpointPartIds);
                                    }
                                    else
                                    {
                                        Marshal.ReleaseComObject(hwTopo);
                                    }
                                }
                            }
                            catch (COMException) { }
                        }
                        finally
                        {
                            Marshal.Release(partPtr);
                        }
                    }
                }
                finally
                {
                    Marshal.Release(connectedToPtr);
                }

                Marshal.ReleaseComObject(connectedTo);
            }
            catch (COMException)
            {
                // Not connected - skip
            }
            finally
            {
                Marshal.ReleaseComObject(connector);
            }
        }

        Marshal.ReleaseComObject(topoObj);
    }

    /// <summary>
    /// Recursively collects all global part IDs reachable from the endpoint topology
    /// so we can determine which parts in the hardware filter are "hidden".
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static void CollectPartIds(IPart part, HashSet<string> ids, HashSet<string> visited)
    {
        string globalId = null;
        try { part.GetGlobalId(out globalId); } catch { }
        if (string.IsNullOrEmpty(globalId) || !visited.Add(globalId))
            return;

        ids.Add(globalId);

        try
        {
            part.EnumPartsIncoming(out IPartsList list);
            if (list != null)
            {
                list.GetCount(out uint count);
                for (uint i = 0; i < count; i++)
                {
                    list.GetPart(i, out IPart child);
                    CollectPartIds(child, ids, visited);
                    Marshal.ReleaseComObject(child);
                }
                Marshal.ReleaseComObject(list);
            }
        }
        catch { }

        try
        {
            part.EnumPartsOutgoing(out IPartsList list);
            if (list != null)
            {
                list.GetCount(out uint count);
                for (uint i = 0; i < count; i++)
                {
                    list.GetPart(i, out IPart child);
                    CollectPartIds(child, ids, visited);
                    Marshal.ReleaseComObject(child);
                }
                Marshal.ReleaseComObject(list);
            }
        }
        catch { }
    }

    /// <summary>
    /// Walks the hardware filter device topology obtained via IPart::GetTopologyObject()
    /// to discover hidden mixer lines like PC Beep, S/PDIF, CD Audio, Aux, etc.
    /// </summary>
    [SupportedOSPlatform("windows")]
    private static void WalkHardwareFilterTopology(IDeviceTopology hwTopo, HashSet<string> visitedPartIds, HashSet<string> endpointPartIds)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"  Hardware Filter Topology (includes hidden channels):");
        Console.ResetColor();

        // Walk all subunits - these contain volume/mute controls for hidden lines
        hwTopo.GetSubunitCount(out uint subunitCount);
        for (uint s = 0; s < subunitCount; s++)
        {
            hwTopo.GetSubunit(s, out IntPtr subunitPtr);
            if (subunitPtr != IntPtr.Zero)
            {
                try
                {
                    Guid iidPart = typeof(IPart).GUID;
                    int hrQI = Marshal.QueryInterface(subunitPtr, ref iidPart, out IntPtr partPtr);
                    if (hrQI == 0 && partPtr != IntPtr.Zero)
                    {
                        try
                        {
                            var part = (IPart)Marshal.GetObjectForIUnknown(partPtr);
                            WalkParts(part, depth: 2, visitedPartIds, endpointPartIds, isHardwareFilter: true);
                        }
                        finally
                        {
                            Marshal.Release(partPtr);
                        }
                    }
                }
                finally
                {
                    Marshal.Release(subunitPtr);
                }
            }
        }

        // Walk all connectors - input connectors are where PC Beep, S/PDIF, etc. live
        hwTopo.GetConnectorCount(out uint hwConnCount);
        for (uint c = 0; c < hwConnCount; c++)
        {
            hwTopo.GetConnector(c, out IConnector hwConn);
            try
            {
                IntPtr hwConnPtr = Marshal.GetIUnknownForObject(hwConn);
                try
                {
                    Guid iidPart = typeof(IPart).GUID;
                    int hrQI = Marshal.QueryInterface(hwConnPtr, ref iidPart, out IntPtr partPtr);
                    if (hrQI == 0 && partPtr != IntPtr.Zero)
                    {
                        try
                        {
                            var part = (IPart)Marshal.GetObjectForIUnknown(partPtr);
                            WalkParts(part, depth: 2, visitedPartIds, endpointPartIds, isHardwareFilter: true);
                        }
                        finally
                        {
                            Marshal.Release(partPtr);
                        }
                    }
                }
                finally
                {
                    Marshal.Release(hwConnPtr);
                }
            }
            finally
            {
                Marshal.ReleaseComObject(hwConn);
            }
        }

        Marshal.ReleaseComObject(hwTopo);
    }

    [SupportedOSPlatform("windows")]
    private static void WalkParts(IPart part, int depth, HashSet<string> visitedPartIds,
        HashSet<string> endpointPartIds, bool isHardwareFilter)
    {
        string globalId = null;
        try { part.GetGlobalId(out globalId); } catch { }
        if (!string.IsNullOrEmpty(globalId) && !visitedPartIds.Add(globalId))
            return;

        string indent = new string(' ', depth * 2);

        part.GetName(out string name);
        part.GetPartType(out uint partType);

        string typeLabel = partType == 0 ? "Connector" : "Subunit";

        if (!string.IsNullOrWhiteSpace(name))
        {
            // Determine if this part is hidden:
            // A part is hidden if it exists in the hardware filter topology
            // but is NOT reachable from the endpoint topology
            bool isHidden = isHardwareFilter &&
                            !string.IsNullOrEmpty(globalId) &&
                            !endpointPartIds.Contains(globalId);

            bool hasVolume = false;
            try
            {
                Guid iidAudioVolumeLevel = new Guid("7FB7B48F-531D-44A2-BCB3-5AD5A134B3DC");
                part.Activate(0x17, ref iidAudioVolumeLevel, out object volObj);
                if (volObj != null)
                {
                    hasVolume = true;
                    Marshal.ReleaseComObject(volObj);
                }
            }
            catch { }

            bool hasMute = false;
            try
            {
                Guid iidAudioMute = new Guid("DF45AEEA-B74A-4B6B-AFAD-2366B6AA012E");
                part.Activate(0x17, ref iidAudioMute, out object muteObj);
                if (muteObj != null)
                {
                    hasMute = true;
                    Marshal.ReleaseComObject(muteObj);
                }
            }
            catch { }

            string markers = "";
            if (hasVolume) markers += " [Volume Slider]";
            if (hasMute) markers += " [Mute]";
            if (isHidden) markers += " [HIDDEN]";

            if (isHidden)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else if (hasVolume)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine($"{indent}├─ {name} ({typeLabel}){markers}");
            Console.ResetColor();
        }

        // Walk incoming parts
        try
        {
            part.EnumPartsIncoming(out IPartsList partsList);
            if (partsList != null)
            {
                partsList.GetCount(out uint count);
                for (uint i = 0; i < count; i++)
                {
                    partsList.GetPart(i, out IPart child);
                    WalkParts(child, depth + 1, visitedPartIds, endpointPartIds, isHardwareFilter);
                    Marshal.ReleaseComObject(child);
                }
                Marshal.ReleaseComObject(partsList);
            }
        }
        catch { }

        // Walk outgoing parts
        try
        {
            part.EnumPartsOutgoing(out IPartsList partsListOut);
            if (partsListOut != null)
            {
                partsListOut.GetCount(out uint count);
                for (uint i = 0; i < count; i++)
                {
                    partsListOut.GetPart(i, out IPart child);
                    WalkParts(child, depth + 1, visitedPartIds, endpointPartIds, isHardwareFilter);
                    Marshal.ReleaseComObject(child);
                }
                Marshal.ReleaseComObject(partsListOut);
            }
        }
        catch { }
    }

    // ──────────────────────────────────────────────
    //  COM interface declarations
    // ──────────────────────────────────────────────

    [ComImport]
    [Guid("D666063F-1587-4E43-81F1-B948E807363F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, uint dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

        [PreserveSig]
        int OpenPropertyStore(int stgmAccess, out IntPtr ppProperties);

        [PreserveSig]
        int GetId([MarshalAs(UnmanagedType.LPWStr)] out string ppstrId);

        [PreserveSig]
        int GetState(out int pdwState);
    }

    [ComImport]
    [Guid("2A07407E-6497-4A18-9787-32F79BD0D98F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDeviceTopology
    {
        [PreserveSig]
        int GetConnectorCount(out uint pCount);

        [PreserveSig]
        int GetConnector(uint nIndex, out IConnector ppConnector);

        [PreserveSig]
        int GetSubunitCount(out uint pCount);

        [PreserveSig]
        int GetSubunit(uint nIndex, out IntPtr ppSubunit);

        [PreserveSig]
        int GetPartById(uint nId, out IntPtr ppPart);

        [PreserveSig]
        int GetDeviceId([MarshalAs(UnmanagedType.LPWStr)] out string ppwstrDeviceId);

        [PreserveSig]
        int GetSignalPath(IPart pIPartFrom, IPart pIPartTo, bool bRejectMixedPaths, out IntPtr ppParts);
    }

    [ComImport]
    [Guid("9C2C4058-23F5-41DE-877A-DF3AF236A09E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IConnector
    {
        [PreserveSig]
        int GetType(out uint pType);

        [PreserveSig]
        int GetDataFlow(out uint pFlow);

        [PreserveSig]
        int ConnectTo(IConnector pConnectTo);

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int IsConnected(out bool pbConnected);

        [PreserveSig]
        int GetConnectedTo(out IConnector ppConTo);

        [PreserveSig]
        int GetConnectorIdConnectedTo([MarshalAs(UnmanagedType.LPWStr)] out string ppwstrConnectorId);

        [PreserveSig]
        int GetDeviceIdConnectedTo([MarshalAs(UnmanagedType.LPWStr)] out string ppwstrDeviceId);
    }

    [ComImport]
    [Guid("AE2DE0E4-5BCA-4F2D-AA46-5D13F8FDB3A9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPart
    {
        [PreserveSig]
        int GetName([MarshalAs(UnmanagedType.LPWStr)] out string ppwstrName);

        [PreserveSig]
        int GetLocalId(out uint pnId);

        [PreserveSig]
        int GetGlobalId([MarshalAs(UnmanagedType.LPWStr)] out string ppwstrGlobalId);

        [PreserveSig]
        int GetPartType(out uint pPartType);

        [PreserveSig]
        int GetSubType(out Guid pSubType);

        [PreserveSig]
        int GetControlInterfaceCount(out uint pCount);

        [PreserveSig]
        int GetControlInterface(uint nIndex, out IntPtr ppInterfaceDesc);

        [PreserveSig]
        int EnumPartsIncoming(out IPartsList ppParts);

        [PreserveSig]
        int EnumPartsOutgoing(out IPartsList ppParts);

        [PreserveSig]
        int GetTopologyObject(out IDeviceTopology ppTopology);

        [PreserveSig]
        int Activate(uint dwClsContext, ref Guid refiid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObject);

        [PreserveSig]
        int RegisterControlChangeCallback(ref Guid riid, IntPtr pNotify);

        [PreserveSig]
        int UnregisterControlChangeCallback(IntPtr pNotify);
    }

    [ComImport]
    [Guid("6DAA848C-5EB0-45CC-AEA5-998A2CDA1FFB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IPartsList
    {
        [PreserveSig]
        int GetCount(out uint pCount);

        [PreserveSig]
        int GetPart(uint nIndex, out IPart ppPart);
    }
}
