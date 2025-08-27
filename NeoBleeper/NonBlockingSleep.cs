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

            long cachedFrequency = CachedFrequency;
            PreventSleep();
            long currentFrequency = Stopwatch.Frequency;
            long startTicks = Stopwatch.GetTimestamp();
            long targetTicks = startTicks + (milliseconds * currentFrequency) / 1000;
            long coarseTargetTicks = targetTicks - (currentFrequency / 1000000 * 50); // 50 microseconds before target
            int doEventsCounter = 0;
            long now = Stopwatch.GetTimestamp();

            // Coarse waiting phase
            while (now < coarseTargetTicks)
            {
                long ticksLeft = coarseTargetTicks - now;
                double millisecondsLeft = (double)ticksLeft * 1000 / cachedFrequency;

                if (millisecondsLeft > 1)
                {
                    // Sleep for a short duration to reduce CPU usage
                    Thread.Sleep((int)Math.Min(millisecondsLeft / 2, 1));
                }

                if (Application.MessageLoop && doEventsCounter++ % 10 == 0)
                {
                    Application.DoEvents();
                }

                now = Stopwatch.GetTimestamp();
            }

            // Fine-tuning phase
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                Thread.Sleep(0);
            }
        }
    }
}