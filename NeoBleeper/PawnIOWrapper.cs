using NeoBleeper.Properties;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public class PawnIOWrapper
    {
        // Alternative to InpOutx64 driver, which has some claims of causing
        // crashes or EXCEPTION_ACCESS_VIOLATION errors in some games, and is
        // expected to be blocked in future Windows 11 releases.

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

        public static void InstallPawnIO()
        {
            if (ProgramPrecheck.IsPawnIOInstalled())
            {
                // PawnIO is already installed
                return;
            }
            byte[] pawnIOInstallerBytes = Resources.PawnIO_setup;
            // Retrieve the embedded executable bytes from project resources
            byte[] installerBytes = Properties.Resources.PawnIO_setup;

            // Create a unique temporary file path
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"PawnIO_setup_{Guid.NewGuid()}.exe");

            try
            {
                // 1. Write the embedded bytes to the temporary file
                File.WriteAllBytes(tempFilePath, installerBytes);

                // 2. Configure the process with silent arguments and UAC elevation
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = tempFilePath,
                    Arguments = "-install -silent",
                    UseShellExecute = true, // Required for the "runas" verb to trigger UAC
                    Verb = "runas",         // Requests Administrator privileges
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                // 3. Start the installer and wait for completion
                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        int exitCode = process.ExitCode;

                        // PawnIO returns 0 for success, or 3010 (ERROR_SUCCESS_REBOOT_REQUIRED) if a reboot is needed
                        if (exitCode == 0)
                        {
                            Console.WriteLine("PawnIO installed successfully.");
                        }
                        else if (exitCode == 3010)
                        {
                            Console.WriteLine("PawnIO installed successfully, but a system reboot is required.");
                        }
                        else
                        {
                            Console.WriteLine($"Installation failed with exit code: {exitCode}");
                        }
                    }
                }
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                // Error 1223 occurs if the user clicks "No" on the UAC admin prompt
                Console.WriteLine("Installation was canceled by the user at the UAC prompt.");
            }
            finally
            {
                // 4. Delete the temporary installer from disk
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // Ignore file-lock errors if cleanup happens too quickly
                    }
                }
            }
        }
    }
}
