using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NeoBleeper;

/// <summary>
/// Minimal x64 PawnIO interop, compatible with the NeoBleeper PawnIOWrapper API.
/// The driver must be installed separately; this class never installs or elevates.
/// </summary>
public sealed class PawnIOWrapper : IDisposable
{
    private const string LibraryName = "PawnIOLib.dll";
    private static readonly object NativeLock = new();
    private static IntPtr s_nativeLibrary;
    private IntPtr _handle;
    private bool _disposed;

    static PawnIOWrapper()
    {
        NativeLibrary.SetDllImportResolver(typeof(PawnIOWrapper).Assembly, ResolveLibrary);
    }

    private static IntPtr ResolveLibrary(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals(libraryName, LibraryName, StringComparison.OrdinalIgnoreCase))
            return IntPtr.Zero;

        lock (NativeLock)
        {
            if (s_nativeLibrary != IntPtr.Zero)
                return s_nativeLibrary;

            string? candidate = FindLibrary();
            if (candidate is null)
                throw new DllNotFoundException("PawnIOLib.dll not found. Install official PawnIO first.");

            s_nativeLibrary = NativeLibrary.Load(candidate);
            return s_nativeLibrary;
        }
    }

    private static string? FindLibrary()
    {
        // Use the documented registry install path, and fall back to %ProgramFiles%\PawnIO.
        string? installedPath = null;
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\PawnIO");
            installedPath = key?.GetValue("InstallLocation") as string;
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException)
        {
            // Fall back to the default install directory.
        }

        if (!string.IsNullOrWhiteSpace(installedPath))
        {
            string path = Path.Combine(installedPath.Trim().Trim('"'), LibraryName);
            if (File.Exists(path)) return path;
        }

        string fallback = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "PawnIO", LibraryName);
        return File.Exists(fallback) ? fallback : null;
    }

    [DllImport(LibraryName, CallingConvention = CallingConvention.Winapi, PreserveSig = false)]
    private static extern void pawnio_version(out uint version);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Winapi, PreserveSig = false)]
    private static extern void pawnio_open(out IntPtr handle);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Winapi, PreserveSig = false)]
    private static extern void pawnio_load(IntPtr handle, [In] byte[] blob, UIntPtr size);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Winapi,
        CharSet = CharSet.Ansi, ExactSpelling = true, PreserveSig = false)]
    private static extern void pawnio_execute(
        IntPtr handle, [MarshalAs(UnmanagedType.LPStr)] string name,
        [In] long[] inArray, UIntPtr inSize,
        [Out] long[] outArray, UIntPtr outSize,
        out UIntPtr returnSize);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Winapi, PreserveSig = false)]
    private static extern void pawnio_close(IntPtr handle);

    private static void ValidatePlatform()
    {
        if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
            throw new PlatformNotSupportedException("PcSpeaker.p requires 64-bit Windows and the x64 PawnIO driver.");
    }

    public static uint Version()
    {
        ValidatePlatform();
        pawnio_version(out uint version);
        return version;
    }

    public PawnIOWrapper()
    {
        ValidatePlatform();
        pawnio_open(out _handle);
        if (_handle == IntPtr.Zero)
            throw new InvalidOperationException("PawnIO did not return a valid device handle.");
    }

    public PawnIOWrapper(byte[] bytes) : this()
    {
        try { Load(bytes); }
        catch { Dispose(); throw; }
    }

    public void Load(byte[] bytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (bytes is null || bytes.Length == 0)
            throw new ArgumentException("The compiled, signed PawnIO module is empty.", nameof(bytes));
        pawnio_load(_handle, bytes, new UIntPtr((uint)bytes.Length));
    }

    public long[] Execute(string name, long[] input, int outLength)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(input);
        if (outLength < 0) throw new ArgumentOutOfRangeException(nameof(outLength));

        long[] output = new long[outLength];
        pawnio_execute(_handle, name, input, new UIntPtr((uint)input.Length),
            output, new UIntPtr((uint)output.Length), out UIntPtr returned);

        ulong count = returned.ToUInt64();
        if (count > (ulong)output.Length)
            throw new InvalidOperationException("PawnIO returned more cells than the supplied buffer.");
        Array.Resize(ref output, (int)count);
        return output;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        IntPtr h = _handle;
        _handle = IntPtr.Zero;
        try { if (h != IntPtr.Zero) pawnio_close(h); }
        finally { GC.SuppressFinalize(this); }
    }

    ~PawnIOWrapper()
    {
        if (_handle != IntPtr.Zero)
        {
            try { pawnio_close(_handle); }
            catch { /* finalizer must never throw */ }
        }
    }
}
