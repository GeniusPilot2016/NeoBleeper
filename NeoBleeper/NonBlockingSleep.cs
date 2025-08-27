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

        private static readonly long CachedFrequency = Stopwatch.Frequency;

        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return;
            }

            long cachedFrequency = CachedFrequency;
            long currentFrequency = Stopwatch.Frequency;
            long startTicks = Stopwatch.GetTimestamp();
            long targetTicks = startTicks + (milliseconds * currentFrequency) / 1000;
            long coarseTargetTicks = targetTicks - (currentFrequency / 1000000 * 50); // 50 microseconds before target
            int doEventsCounter = 0;
            long now = Stopwatch.GetTimestamp();
            SpinWait spinWait = new SpinWait();
            // Coarse waiting phase
            while (now < coarseTargetTicks)
            {
                long ticksLeft = coarseTargetTicks - now;
                double millisecondsLeft = (double)ticksLeft * 1000 / cachedFrequency;
                Application.DoEvents();
                spinWait.SpinOnce();
                now = Stopwatch.GetTimestamp();
            }

            // Fine-tuning phase
            
            while (Stopwatch.GetTimestamp() < targetTicks)
            {
                spinWait.SpinOnce();
            }
        }
    }
}