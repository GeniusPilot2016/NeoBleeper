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
                return;
            long cachedFrequency = CachedFrequency;
            PreventSleep();

            long currentFrequency = Stopwatch.Frequency;
            long startTicks = Stopwatch.GetTimestamp();
            long targetTicks = startTicks + (microseconds * currentFrequency) / 1000000;
            // Coarse delay: Sleep until close to the target time
            long coarseTargetTicks = targetTicks - (currentFrequency / 1000000 * 50); // 50 microseconds before target
            int doEventsCounter = 0;
            long now = Stopwatch.GetTimestamp();
            while (now < coarseTargetTicks)
            {
                long ticksLeft = coarseTargetTicks - now;
                double microsecondsLeft = (double)ticksLeft * 1_000_000 / cachedFrequency;

                Thread.SpinWait(1);

                if (++doEventsCounter >= 100)
                {
                    if (Application.MessageLoop)
                        Application.DoEvents();
                    doEventsCounter = 0;
                }
                now = Stopwatch.GetTimestamp();
            }

            // Fine-tuning: Tight loop for the final microseconds
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
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