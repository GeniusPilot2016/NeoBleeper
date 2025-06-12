using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class NonBlockingSleep
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        public static extern uint TimeBeginPeriod(uint uMilliseconds);
        
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        public static extern uint TimeEndPeriod(uint uMilliseconds);

        private static readonly double StopwatchFrequency = Stopwatch.Frequency / 1000.0; // Frekans/ms

        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return;
            }

            // Increase system timer resolution for more accurate timing
            TimeBeginPeriod(1);
            
            try
            {
                // Use Stopwatch for high-resolution timing
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Calculate target ticks based on the requested milliseconds
                long targetTicks = stopwatch.ElapsedTicks + (long)(milliseconds * StopwatchFrequency);

                while (stopwatch.ElapsedTicks < targetTicks)
                {
                    if (Application.MessageLoop && Application.OpenForms.Count > 0)
                    {
                        Application.DoEvents();
                    }

                    // Calculate remaining ticks and adjust sleep strategy
                    long remainingTicks = targetTicks - stopwatch.ElapsedTicks;
                    double remainingMs = remainingTicks / StopwatchFrequency;
                    if (remainingMs > 15)
                    {
                        Thread.Sleep(0); // To allow other threads to run
                    }
                    else if (remainingMs > 5)
                    {
                        Thread.Yield(); // Allow other threads to run, but yield the current thread
                    }
                    else if (remainingMs > 1)
                    {
                        // Set spinCount based on remaining milliseconds
                        int spinCount = (int)(remainingMs * 100);
                        Thread.SpinWait(spinCount);
                    }
                    else
                    {
                        // Ultra-low latency spin-waiting
                        // Adaptive spin-waiting based on remaining ticks
                        while (stopwatch.ElapsedTicks < targetTicks)
                        {
                            // Optimize for very short waits
                            Thread.SpinWait(10); 
                        }
                    }
                }
            }
            finally
            {
                // Restore system timer resolution
                TimeEndPeriod(1);
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
>>>>>>>>> Temporary merge branch 2
                }
            }
        }
    }
}
