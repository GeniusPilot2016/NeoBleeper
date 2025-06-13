using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class NonBlockingSleep
    {
        // Windows multimedia timer functions
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        private static extern uint TimeGetTime();

        // High precision kernel timer functions
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        // Cache the performance frequency to avoid repeated calls
        private static readonly long PerformanceFrequency;

        // Static constructor to initialize the performance frequency
        static NonBlockingSleep()
        {
            QueryPerformanceFrequency(out PerformanceFrequency);
        }

        // Power management functions to prevent CPU throttling during critical timing operations
        [DllImport("kernel32.dll")]
        private static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

        [Flags]
        private enum ExecutionState : uint
        {
            EsSystemRequired = 0x00000001,
            EsDisplayRequired = 0x00000002,
            EsContinuous = 0x80000000,
            EsAwayModeRequired = 0x00000040
        }

        /// <summary>
        /// Ultra-high-precision non-blocking sleep implementation for multimedia applications
        /// </summary>
        /// <param name="milliseconds">Sleep time in milliseconds</param>
        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
                return;

            TimeBeginPeriod(1);
            var previousExecutionState = SetThreadExecutionState(ExecutionState.EsSystemRequired);

            try
            {
                QueryPerformanceCounter(out long startTime);
                long targetTime = startTime + (milliseconds * PerformanceFrequency / 1000);

                // Thresholds for different sleep strategies
                long spinThreshold = PerformanceFrequency / 500;  // 2ms
                long yieldThreshold = PerformanceFrequency / 100; // 10ms
                long sleepThreshold = PerformanceFrequency / 25;  // 40ms - yeni eşik

                while (true)
                {
                    QueryPerformanceCounter(out long currentTime);
                    long remainingTicks = targetTime - currentTime;

                    if (remainingTicks <= 0)
                        break;

                    // Remaining time in milliseconds
                    if (remainingTicks < spinThreshold)
                    {
                        // Spin wait for very short durations
                        Thread.SpinWait(500);
                    }
                    // Smaller durations: smaller than yieldThreshold
                    else if (remainingTicks < yieldThreshold)
                    {
                        // Call DoEvents for UI responsiveness
                        if (remainingTicks > spinThreshold * 3)
                        {
                            Thread.Yield();
                            Application.DoEvents();
                        }
                        else
                        {
                            // Yield the thread to allow other threads to run
                            Thread.Yield();
                        }
                    }
                    // Middle durations: between yieldThreshold and sleepThreshold
                    else if (remainingTicks < sleepThreshold)
                    {
                        // More efficient sleep for moderate durations
                        Thread.Sleep(0);

                        // Handle UI responsiveness for longer waits
                        if (remainingTicks > yieldThreshold * 2)
                            Application.DoEvents();
                    }
                    // Longer durations: greater than sleepThreshold
                    else
                    {
                        // More efficient sleep for longer waits
                        int sleepTimeMs = (int)(remainingTicks * 800 / PerformanceFrequency);
                        if (sleepTimeMs > 2)
                            Thread.Sleep(Math.Min(sleepTimeMs - 2, 10)); // Maksimum 10ms sleep ile aşırı uyumayı önle
                        else
                            Thread.Sleep(0);

                        Application.DoEvents();
                    }
                }
            }
            finally
            {
                SetThreadExecutionState(previousExecutionState);
                TimeEndPeriod(1);
            }
        }

        /// <summary>
        /// Releases the currently held time slice and allows other threads to be scheduled
        /// </summary>
        public static void SleepPrecise(int milliseconds)
        {
            if (milliseconds <= 0) return;

            TimeBeginPeriod(1);
            try
            {
                // For very small durations, use specialized techniques
                if (milliseconds <= 2)
                {
                    // Start timestamp
                    long start = TimeGetTime();
                    long target = start + milliseconds;

                    // Busy wait for extremely short durations
                    while (TimeGetTime() < target)
                    {
                        // Minimal yield to avoid 100% CPU usage but maintain precision
                        Thread.SpinWait(100);
                    }
                }
                else
                {
                    // Use standard Sleep for longer durations with compensation
                    // Sleep typically overshoots by a small amount, so we sleep for slightly less time
                    Thread.Sleep(milliseconds - 1);

                    // Fine tune the remaining time with spin wait
                    long start = TimeGetTime();
                    long target = start + 1; // The remaining 1ms we reserved

                    while (TimeGetTime() < target)
                    {
                        Thread.SpinWait(10);
                    }
                }
            }
            finally
            {
                TimeEndPeriod(1);
            }
        }
    }
}