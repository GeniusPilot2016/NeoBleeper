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
        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return; // Negative sleep time is not valid
            }
            long frequency = Stopwatch.Frequency;
            long targetTicks = frequency * milliseconds / 1000;
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedTicks < targetTicks)
            {
                Application.DoEvents();
            }
            stopwatch.Stop();
            return; // Sleep completed
        }
    }
}