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

            // Increase system timer resolution to minimum possible value
            TimeBeginPeriod(1);
            
            try
            {
                // Use Stopwatch for high-resolution timing
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Calculate target ticks based on the requested milliseconds
                long targetTicks = stopwatch.ElapsedTicks + (long)(milliseconds * StopwatchFrequency);
                
                // Ultra-minimal overhead compensation - virtually none for short durations
                double overheadCompensationMs = Math.Min(0.02, milliseconds * 0.001);
                long overheadTicks = (long)(overheadCompensationMs * StopwatchFrequency);
                long adjustedTargetTicks = targetTicks - overheadTicks;

                int iterationCount = 0;
                
                while (stopwatch.ElapsedTicks < adjustedTargetTicks)
                {
                    iterationCount++;
                    
                    // Process windows messages less frequently to reduce timing impact
                    if (iterationCount % 10 == 0 && Application.MessageLoop && Application.OpenForms.Count > 0)
                    {
                        // Use a more selective message filtering approach
                        Application.DoEvents();
                    }

                    // Calculate remaining time 
                    long remainingTicks = adjustedTargetTicks - stopwatch.ElapsedTicks;
                    double remainingMs = remainingTicks / StopwatchFrequency;
                    
                    // Hyper-optimized sleep strategy
                    if (remainingMs > 5)
                    {
                        // Use sleep(0) which yields to other threads but returns very quickly
                        Thread.Sleep(0);
                    }
                    else if (remainingMs > 1)
                    {
                        // Use Thread.Yield which is more predictable than Sleep(0) for short durations
                        Thread.Yield();
                    }
                    else
                    {
                        // Below 1ms, use pure busy-waiting with calibrated SpinWait
                        // Use a hybrid approach based on CPU characteristics
                        long cpuEstimate = (long)(Stopwatch.Frequency / 1000000.0); // Estimate CPU speed
                        int spinBase = Math.Max(10, (int)(cpuEstimate / 100)); 
                        
                        // Exponential spin-down approach for maximum precision
                        while (stopwatch.ElapsedTicks < adjustedTargetTicks)
                        {
                            // Recalculate with each iteration for highest precision
                            long microRemainingTicks = adjustedTargetTicks - stopwatch.ElapsedTicks;
                            double microRemainingMs = microRemainingTicks / StopwatchFrequency;
                            
                            if (microRemainingMs <= 0.01)
                            {
                                // At terminal approach (10 microseconds) use minimal spin
                                Thread.SpinWait(1);
                            }
                            else if (microRemainingMs <= 0.1)
                            {
                                // Under 0.1ms, very fine grained waiting
                                Thread.SpinWait(spinBase);
                            }
                            else if (microRemainingMs <= 0.5)
                            {
                                // Under 0.5ms
                                Thread.SpinWait(spinBase * 5);
                            }
                            else
                            {
                                // Adapted spin count for current processor
                                int dynamicSpinCount = (int)(microRemainingMs * 500 * 
                                    (Environment.ProcessorCount > 4 ? 1.5 : 1.0));
                                Thread.SpinWait(dynamicSpinCount);
                            }
                        }
                        break; // Exit main loop once busy-wait complete
                    }
                }
            }
            finally
            {
                // Restore system timer resolution
                TimeEndPeriod(1);
            }
        }
    }
}
