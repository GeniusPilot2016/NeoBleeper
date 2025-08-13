using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class NonBlockingSleep
    {
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint esFlags);

        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_AWAYMODE_REQUIRED = 0x00000040;

        public static void PreventSleep()
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_AWAYMODE_REQUIRED);
        }

        private static readonly long CachedFrequency = Stopwatch.Frequency;

        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return;
            }

            SleepMicroseconds(milliseconds * 1000);
        }

        public static void SleepMicroseconds(long microseconds)
        {
            if (microseconds <= 0)
            {
                return;
            }

            PreventSleep();

            long startTicks = Stopwatch.GetTimestamp();
            long targetTicks = startTicks + (microseconds * CachedFrequency) / 1000000;
            long spinWaitThresholdTicks = CachedFrequency / 10000; // 100 microseconds
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                if (Stopwatch.GetTimestamp() > targetTicks)
                {
                    break;
                }
                long remainingTicks = targetTicks - Stopwatch.GetTimestamp();

                // Yield the thread for longer intervals, but still check for messages
                Application.DoEvents();
                Thread.Sleep(0);
            }
        }

        public static void SleepNanoseconds(long nanoseconds)
        {
            if (nanoseconds <= 0)
            {
                return;
            }

            long startTicks = Stopwatch.GetTimestamp();
            long targetTicks = startTicks + (nanoseconds * CachedFrequency) / 1000000000;

            // Pure spinning for nanosecond precision (use sparingly!)
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                // Tight loop for maximum precision
            }
        }
    }
}