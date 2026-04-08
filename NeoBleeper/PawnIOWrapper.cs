using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoBleeper
{
    public class PawnIOWrapper
    {
        // Alternative to InpOutx64 driver, which is cross-signed and can be potentially affected after April 2026 update of Windows 11 24H2 and above.
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_version(out uint version);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_open(out IntPtr handle);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_load(IntPtr handle, byte[] blob, IntPtr size);

        [DllImport("PawnIOLib.dll", CharSet = CharSet.Ansi, PreserveSig = false)]
        private static extern void pawnio_execute(IntPtr handle, string name, long[] in_array, IntPtr in_size, long[] out_array, IntPtr out_size, out IntPtr return_size);

        [DllImport("PawnIOLib.dll", PreserveSig = false)]
        private static extern void pawnio_close(IntPtr handle);

        private readonly IntPtr _handle;

        /// <summary>
        /// Attempts to load the PawnIOLib.dll library from the directory specified by the PAWNIO_ROOT environment
        /// variable.
        /// </summary>
        /// <remarks>This method checks for the PAWNIO_ROOT environment variable and tries to load the
        /// PawnIOLib.dll from that location. If the environment variable is not set or the library cannot be loaded,
        /// the method returns false.</remarks>
        /// <returns>true if the library is successfully loaded; otherwise, false.</returns>
        private static bool TryLoadDll()
        {
            var pawnioPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "PawnIO");
            if (!string.IsNullOrEmpty(pawnioPath))
            {
                try
                {
                    LoadLibrary(Path.Combine(pawnioPath, "PawnIOLib.dll"));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves the version number of the underlying PawnIO library as an unsigned integer.
        /// </summary>
        /// <remarks>The returned value encodes the library version. The format of the version number may
        /// depend on the underlying native implementation. This method ensures the native library is loaded before
        /// retrieving the version.</remarks>
        /// <returns>A 32-bit unsigned integer representing the version of the PawnIO library.</returns>
        public static uint Version()
        {
            TryLoadDll();
            pawnio_version(out var version);
            return version;
        }

        public PawnIOWrapper()
        {
            TryLoadDll();
            pawnio_open(out _handle);
        }

        public PawnIOWrapper(byte[] bytes)
        {
            TryLoadDll();
            pawnio_open(out _handle);
            Load(bytes);
        }

        ~PawnIOWrapper()
        {
            if (_handle != IntPtr.Zero)
                pawnio_close(_handle);
        }

        /// <summary>
        /// Loads data from the specified byte array into the current instance.
        /// </summary>
        /// <param name="bytes">The byte array containing the data to load. Cannot be null.</param>
        public void Load(byte[] bytes)
        {
            pawnio_load(_handle, bytes, (IntPtr)bytes.Length);
        }

        /// <summary>
        /// Executes a named operation using the specified input array and returns the result as an array of 64-bit
        /// integers.
        /// </summary>
        /// <param name="name">The name of the operation to execute. This value determines which operation will be performed.</param>
        /// <param name="input">The input array of 64-bit integers to be processed by the operation. Cannot be null.</param>
        /// <param name="outLength">The expected length of the output array. Must be a non-negative integer.</param>
        /// <returns>An array of 64-bit integers containing the result of the operation. The length of the returned array may be
        /// less than or equal to the specified output length, depending on the operation's result.</returns>
        public long[] Execute(string name, long[] input, int outLength)
        {
            var outArray = new long[outLength];
            pawnio_execute(_handle, name, input, (IntPtr)input.Length, outArray, (IntPtr)outArray.Length, out var returnLength);
            Array.Resize(ref outArray, (int)returnLength);
            return outArray;
        }
    }
}
