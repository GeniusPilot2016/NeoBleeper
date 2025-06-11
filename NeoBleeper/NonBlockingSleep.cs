using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class NonBlockingSleep
    {
        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            long targetTime = stopwatch.ElapsedMilliseconds + milliseconds;
            
            while (stopwatch.ElapsedMilliseconds < targetTime)
            {
                // Process Windows messages to keep UI responsive
                Application.DoEvents();
                
                if (targetTime - stopwatch.ElapsedMilliseconds > 10)
                {
                    // For longer waits, yield without sleeping
                    Thread.Yield();
                }
                else
                {
                    // For very short remaining times, burn CPU to ensure precision
                    // This ensures no thread switch occurs during critical timing
                }
            }
        }
    }
}
