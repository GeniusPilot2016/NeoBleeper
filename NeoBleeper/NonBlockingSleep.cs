using NAudio.Utils;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class NonBlockingSleep // Class for non-async sleep that blocks the thread, but still processes UI messages
    {
        // MsgWaitForMultipleObjects P/Invoke
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint MsgWaitForMultipleObjects(uint nCount, IntPtr[] pHandles, bool bWaitAll, uint dwMilliseconds, uint dwWakeMask);

        private const uint QS_ALLINPUT = 0x04FF;
        private const uint WAIT_OBJECT_0 = 0x00000000;
        private const uint WAIT_OBJECT_0_MSG = WAIT_OBJECT_0 + 1; // event signaled = WAIT_OBJECT_0, messages = WAIT_OBJECT_0 + nCount
        private const uint WAIT_TIMEOUT = 0x00000102;
        private const uint WAIT_FAILED = 0xFFFFFFFF;
        private const uint INFINITE = 0xFFFFFFFF;

        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
                return;

            using (var timer = new MultimediaTimer { Interval = milliseconds })
            using (var mre = new ManualResetEvent(false))
            {
                object lockObj = new object();

                timer.Tick += (s, e) =>
                {
                    lock (lockObj)
                    {
                        if (!mre.SafeWaitHandle.IsClosed && !mre.SafeWaitHandle.IsInvalid)
                        {
                            mre.Set();
                        }
                    }
                };

                try
                {
                    timer.Start();

                    IntPtr handle = mre.SafeWaitHandle.DangerousGetHandle();
                    while (true && Application.OpenForms.Count > 0)
                    {
                        uint result = MsgWaitForMultipleObjects(1, new[] { handle }, false, INFINITE, QS_ALLINPUT);
                        if (result == WAIT_OBJECT_0)
                        {
                            break;
                        }
                        else if (result == WAIT_OBJECT_0_MSG)
                        {
                            Application.DoEvents();
                            continue;
                        }
                        else if (result == WAIT_TIMEOUT)
                        {
                            continue;
                        }
                        else if (result == WAIT_FAILED)
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                        else
                        {
                            Application.DoEvents();
                        }
                        Thread.Sleep(1);
                    }
                }
                finally
                {
                    lock (lockObj)
                    {
                        timer.Stop();
                    }
                }
            }
        }
    }
}