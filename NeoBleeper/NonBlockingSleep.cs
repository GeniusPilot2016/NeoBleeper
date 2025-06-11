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

            // Sistem zamanlayıcı çözünürlüğünü geçici olarak artır
            TimeBeginPeriod(1);
            
            try
            {
                // Yüksek çözünürlüklü süre ölçümü için
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                // Ticks'leri doğrudan hesapla (daha kesin)
                long targetTicks = stopwatch.ElapsedTicks + (long)(milliseconds * StopwatchFrequency);

                while (stopwatch.ElapsedTicks < targetTicks)
                {
                    if (Application.MessageLoop && Application.OpenForms.Count > 0)
                    {
                        Application.DoEvents();
                    }

                    // Kalan zamanı ticks olarak hesapla
                    long remainingTicks = targetTicks - stopwatch.ElapsedTicks;
                    double remainingMs = remainingTicks / StopwatchFrequency;
                    if (remainingMs > 15)
                    {
                        Thread.Sleep(0); // İşletim sistemine kontrol vermek için
                    }
                    else if (remainingMs > 5)
                    {
                        Thread.Yield(); // Diğer thread'lere izin ver
                    }
                    else if (remainingMs > 1)
                    {
                        // Hassas beklemeler için spin sayısını hedefe göre ayarla
                        int spinCount = (int)(remainingMs * 100);
                        Thread.SpinWait(spinCount);
                    }
                    else
                    {
                        // 1ms altındaki beklemeler için ultra-hassas döngü
                        // Donanım performansına dayalı adaptif döngü
                        while (stopwatch.ElapsedTicks < targetTicks)
                        {
                            // CPU'ya bir sinyal göndererek döngüyü optimize et
                            Thread.SpinWait(10); 
                        }
                    }
                }
            }
            finally
            {
                // Sistem zamanlayıcı çözünürlüğünü eski haline getir
                TimeEndPeriod(1);
            }
        }
    }
}
