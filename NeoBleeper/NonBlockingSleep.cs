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

                if (remainingTime > 15)
                {
                    // For longer waits, use SpinWait to yield efficiently
                    Thread.SpinWait(1);
                }
                else if (remainingTime > 5)
                {
                    // For medium waits, yield without sleeping
                    Thread.Yield();
                }
                else
                {
                    // For very short remaining times, actively spin
                    // This ensures no thread switch occurs during critical timing
                    Thread.SpinWait(10);
                }
            }
        }
    }
}
