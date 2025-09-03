using NAudio.Utils;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace NeoBleeper
{
    public class HighPrecisionSleep // Class for precise sleep for multimedia usage
    {
        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
                return;

            using (var timer = new MultimediaTimer { Interval = milliseconds })
            {
                using (var mre = new ManualResetEvent(false))
                {
                    EventHandler? tickHandler = null;
                    tickHandler = (s, e) =>
                    {
                        mre.Set();
                        timer.Tick -= tickHandler; // Unsubscribe after the first tick
                    };

                    timer.Tick += tickHandler;
                    timer.Start();
                    mre.WaitOne();
                    timer.Stop();
                }
            }
        }
        public static async Task SleepAsync(int milliseconds)
        {
            await Task.Run(() => Sleep(milliseconds));
        }
    }
}