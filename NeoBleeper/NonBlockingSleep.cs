using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class NonBlockingSleep
    {
        System.Timers.Timer timer = new System.Timers.Timer();
        public NonBlockingSleep()
        {
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
        }
        public static void Sleep(int milliseconds)
        {
            if(milliseconds <= 0)
            {
                return;
            }
            NonBlockingSleep nbs = new NonBlockingSleep();
            nbs.timer.Interval = milliseconds;
            nbs.timer.Start();
            while (nbs.timer.Enabled)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }
        public void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
        }
    }
}
