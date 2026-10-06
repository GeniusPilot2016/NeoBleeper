using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace NeoBleeper
{
    public class ProgramPrecheck
    {
        public static bool IsARM64()
        {
            return RuntimeInformation.ProcessArchitecture == Architecture.Arm64;
        }

        public static bool IsPawnIOInstalled()
        {
            try
            {
                var pawnioPath = Environment.GetEnvironmentVariable("PAWNIO_ROOT");
                if (!string.IsNullOrEmpty(pawnioPath)) // Check if PawnIO is installed
                {
                    try
                    {
                        if (!File.Exists(Path.Combine(pawnioPath, "PawnIOLib.dll")))
                        {
                            return false; // DLL not found in the specified path
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            catch
            {
            }
            return false; // Placeholder implementation, as PawnIO is not relevant in this context
        }

        public static bool IsRanAsAdmin()
        {
            try
            {
                // Get the identity of the current user running the application
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);

                    // Check if the current context has the Built-In Administrator role
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
            }
            catch (Exception)
            {
                // Fallback for non-Windows platforms or permission errors
                return false;
            }
        }
    }
}
