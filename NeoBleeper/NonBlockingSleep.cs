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
                if (Application.MessageLoop && Application.OpenForms.Count > 0)
                {
                    Application.DoEvents();
                }

                long remainingTime = targetTime - stopwatch.ElapsedMilliseconds;

                if (remainingTime > 20)
                {
                    // For longer waits (>20ms), sleep a tiny amount to reduce CPU usage
                    // while still maintaining responsiveness
                    Thread.Sleep(1);
                }
                else if (remainingTime > 10)
                {
                    // For medium waits (10-20ms), yield to other threads but don't sleep
                    Thread.SpinWait(1);
                }
                else if (remainingTime > 3)
                {
                    // For short waits (3-10ms), yield without sleeping
                    Thread.Yield();
                }
                else
                {
                    // For very short durations (<3ms), use aggressive CPU spinning
                    // This approach maximizes precision at the cost of CPU usage
                    Thread.SpinWait(30); // Higher spin count for tighter loops
                    
                    // Check more frequently within the tight loop
                    if (remainingTime < 1 && stopwatch.ElapsedMilliseconds >= targetTime - 1)
                    {
                        // Ultra-fine tuning for the final millisecond
                        // Pure CPU burn for maximum precision
                        while (stopwatch.ElapsedMilliseconds < targetTime)
                        {
                            // Empty loop - pure CPU spinning for maximum timing precision
                        }
                        break;
                    }
                }
            }
        }
    }
}
