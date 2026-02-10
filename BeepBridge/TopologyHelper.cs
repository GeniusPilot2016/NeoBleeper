using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

public static class TopologyHelper
{
    // Topology-based beep detection entry point
    [SupportedOSPlatform("windows")]
    public static bool TopologyContainsBeep()
    {
        try
        {
            using var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active | DeviceState.Disabled | DeviceState.Unplugged);
            var walkedHardwareDevices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var device in devices)
            {
                try
                {
                    if (EnumerateTopologyPartsForBeep(device, walkedHardwareDevices))
                        return true;
                }
                catch { /* ignore device-level errors and continue */ }
            }
        }
        catch { /* ignore enumerator errors */ }

        return false;
    }

    // ----------------------------
    // Topology walking + detection
    // ----------------------------
    [DllImport("ole32.dll")]
    private static extern int CoCreateInstance(
        ref Guid rclsid, IntPtr pUnkOuter, uint dwClsContext,
        ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);

    [SupportedOSPlatform("windows")]
    private static bool EnumerateTopologyPartsForBeep(MMDevice device, HashSet<string> walkedHardwareDevices)
    {
        var realDeviceField = typeof(MMDevice).GetField("_realDevice",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (realDeviceField == null)
        {
            realDeviceField = typeof(MMDevice).GetField("deviceInterface",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        }

        if (realDeviceField == null)
            return false;

        var realDevice = realDeviceField.GetValue(device);
        if (realDevice == null)
            return false;

        Guid iidDeviceTopology = typeof(IDeviceTopology).GUID;
        const uint CLSCTX_ALL = 0x17;

        var immDevice = (IMMDevice)realDevice;
        int hr = immDevice.Activate(ref iidDeviceTopology, CLSCTX_ALL, IntPtr.Zero, out object topoObj);

        if (hr != 0 || topoObj == null)
            return false;

        bool found = false;
        var topo = (IDeviceTopology)topoObj;
        topo.GetConnectorCount(out uint connectorCount);

        var visitedPartIds = new HashSet<string>();
        var endpointPartIds = new HashSet<string>();

        // Collect endpoint topology parts and scan
        for (uint c = 0; c < connectorCount && !found; c++)
        {
            topo.GetConnector(c, out IConnector connector);
            try
            {
                connector.IsConnected(out bool isConnected);
                if (!isConnected)
                    continue;

                // Try get HW device id
                string hwDeviceId = null;
                try { connector.GetDeviceIdConnectedTo(out hwDeviceId); } catch { }

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
                            CollectPartIds(part, endpointPartIds, new HashSet<string>());

                            if (WalkPartsForBeep(part, 2, new HashSet<string>(), endpointPartIds, isHardwareFilter: false))
                                found = true;

                            // fallback: try get HW device id from part topology
                            if (!found && hwDeviceId == null)
                            {
                                try
                                {
                                    part.GetTopologyObject(out IDeviceTopology partTopo);
                                    if (partTopo != null)
                                    {
                                        partTopo.GetDeviceId(out hwDeviceId);
                                        Marshal.ReleaseComObject(partTopo);
                                    }
                                }
                                catch { }
                            }
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

                // Walk hardware filter if hwDeviceId present and not yet walked
                if (!found)
                {
                    string hwDeviceIdVal = null;
                    try { connector.GetDeviceIdConnectedTo(out hwDeviceIdVal); } catch { }
                    if (!string.IsNullOrEmpty(hwDeviceIdVal) && walkedHardwareDevices.Add(hwDeviceIdVal))
                    {
                        if (WalkHardwareFilterForBeep(hwDeviceIdVal, endpointPartIds))
                            found = true;
                    }
                }
            }
            catch { }
            finally
            {
                Marshal.ReleaseComObject(connector);
            }
        }

        Marshal.ReleaseComObject(topoObj);
        return found;
    }

    [SupportedOSPlatform("windows")]
    private static bool WalkHardwareFilterForBeep(string hwDeviceId, HashSet<string> endpointPartIds)
    {
        try
        {
            Guid CLSID_MMDeviceEnumerator = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");
            Guid IID_IMMDeviceEnumerator = new Guid("A95664D2-9614-4F35-A746-DE8DB63617E6");
            const uint CLSCTX_ALL = 0x17;

            int hrCreate = CoCreateInstance(
                ref CLSID_MMDeviceEnumerator, IntPtr.Zero, CLSCTX_ALL,
                ref IID_IMMDeviceEnumerator, out object enumObj);

            if (hrCreate != 0 || enumObj == null)
                return false;

            var mmEnum = (IMMDeviceEnumeratorNative)enumObj;
            int hrGetDev = mmEnum.GetDevice(hwDeviceId, out IMMDevice hwDevice);
            if (hrGetDev != 0 || hwDevice == null)
            {
                Marshal.ReleaseComObject(enumObj);
                return false;
            }

            Guid iidTopo = typeof(IDeviceTopology).GUID;
            int hrAct = hwDevice.Activate(ref iidTopo, CLSCTX_ALL, IntPtr.Zero, out object hwTopoObj);
            if (hrAct != 0 || hwTopoObj == null)
            {
                Marshal.ReleaseComObject(enumObj);
                return false;
            }

            var hwTopo = (IDeviceTopology)hwTopoObj;
            bool found = WalkHardwareFilterTopologyDirectForBeep(hwTopo, new HashSet<string>(), endpointPartIds);

            Marshal.ReleaseComObject(hwTopoObj);
            Marshal.ReleaseComObject(enumObj);
            return found;
        }
        catch { return false; }
    }

    [SupportedOSPlatform("windows")]
    private static bool WalkHardwareFilterTopologyDirectForBeep(IDeviceTopology hwTopo, HashSet<string> visitedPartIds, HashSet<string> endpointPartIds)
    {
        // Walk subunits
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
                            if (WalkPartsForBeep(part, 2, visitedPartIds, endpointPartIds, isHardwareFilter: true))
                                return true;
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

        // Walk connectors
        hwTopo.GetConnectorCount(out uint hwConnCount);
        for (uint c = 0; c < hwConnCount; c++)
        {
            hwTopo.GetConnector(c, out IConnector hwConn);
            try
            {
                hwConn.GetDataFlow(out uint dataFlow);
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
                            // Optionally get name for flow indication (not required here)
                            if (WalkPartsForBeep(part, 2, visitedPartIds, endpointPartIds, isHardwareFilter: true))
                                return true;
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

        return false;
    }

    // Collect global IDs reachable from endpoint topology
    [SupportedOSPlatform("windows")]
    private static void CollectPartIds(IPart part, HashSet<string> ids, HashSet<string> visited)
    {
        try
        {
            part.GetGlobalId(out string globalId);
            if (string.IsNullOrEmpty(globalId) || !visited.Add(globalId))
                return;

            ids.Add(globalId);
        }
        catch { }

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

    // Walk parts and return true if any part name matches beep-related keywords
    [SupportedOSPlatform("windows")]
    private static bool WalkPartsForBeep(IPart part, int depth, HashSet<string> visitedPartIds,
        HashSet<string> endpointPartIds, bool isHardwareFilter)
    {
        string globalId = null;
        try { part.GetGlobalId(out globalId); } catch { globalId = null; }
        if (!string.IsNullOrEmpty(globalId) && !visitedPartIds.Add(globalId))
            return false;

        string name = null;
        try { part.GetName(out name); } catch { name = null; }
        uint partType = 0;
        try { part.GetPartType(out partType); } catch { partType = 0; }

        if (!string.IsNullOrWhiteSpace(name))
        {
            // Determine hidden status if needed (not required for detection)
            // Check name for beep-related keyword
            if (IsBeepRelatedName(name))
                return true;
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
                    if (WalkPartsForBeep(child, depth + 1, visitedPartIds, endpointPartIds, isHardwareFilter))
                    {
                        Marshal.ReleaseComObject(child);
                        Marshal.ReleaseComObject(partsList);
                        return true;
                    }
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
                    if (WalkPartsForBeep(child, depth + 1, visitedPartIds, endpointPartIds, isHardwareFilter))
                    {
                        Marshal.ReleaseComObject(child);
                        Marshal.ReleaseComObject(partsListOut);
                        return true;
                    }
                    Marshal.ReleaseComObject(child);
                }
                Marshal.ReleaseComObject(partsListOut);
            }
        }
        catch { }

        return false;
    }

    // Beep-related name check (same logic used in MainWindow)
    private static bool IsBeepRelatedName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        string lower = name.ToLowerInvariant();
        return lower.Contains("beep") ||
               lower.Contains("pc speaker") ||
               lower.Contains("system beep") ||
               lower.Contains("pc beep");
    }

    // ------------------------------------------------
    //  COM interface declarations (copied from Program.cs)
    // ------------------------------------------------

    [ComImport]
    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IMMDeviceEnumeratorNative
    {
        [PreserveSig]
        int EnumAudioEndpoints(int dataFlow, int dwStateMask, out IntPtr ppDevices);

        [PreserveSig]
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IntPtr ppEndpoint);

        [PreserveSig]
        int GetDevice([MarshalAs(UnmanagedType.LPWStr)] string pwstrId, out IMMDevice ppDevice);

        [PreserveSig]
        int RegisterEndpointNotificationCallback(IntPtr pClient);

        [PreserveSig]
        int UnregisterEndpointNotificationCallback(IntPtr pClient);
    }

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