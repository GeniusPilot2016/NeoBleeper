using NAudio.Utils;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace NeoBleeper
{
    public class NonBlockingSleep // Class for non-async sleep that blocks the thread, but still processes UI messages
    {
        // Aktif timer'ları takip etmek için
        private static readonly ConcurrentBag<MultimediaTimer> _activeTimers = new ConcurrentBag<MultimediaTimer>();
        private static bool _shutdownInitialized = false;

        // MsgWaitForMultipleObjects P/Invoke
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint MsgWaitForMultipleObjects(uint nCount, IntPtr[] pHandles, bool bWaitAll, uint dwMilliseconds, uint dwWakeMask);

        private const uint QS_ALLINPUT = 0x04FF;
        private const uint WAIT_OBJECT_0 = 0x00000000;
        private const uint WAIT_OBJECT_0_MSG = WAIT_OBJECT_0 + 1; // event signaled = WAIT_OBJECT_0, messages = WAIT_OBJECT_0 + nCount
        private const uint WAIT_TIMEOUT = 0x00000102;
        private const uint WAIT_FAILED = 0xFFFFFFFF;
        private const uint INFINITE = 0xFFFFFFFF;

        static NonBlockingSleep()
        {
            InitializeShutdownHandlers();
        }

        private static void InitializeShutdownHandlers()
        {
            if (_shutdownInitialized) return;

            try
            {
                // Application shutdown event'leri
                Application.ApplicationExit += OnApplicationExit;

                // Process exit event'i
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

                // Unhandled exception event'i
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                _shutdownInitialized = true;
            }
            catch (Exception ex)
            {
                // Log the error if logger is available
                try
                {
                    Logger.Log($"Failed to initialize shutdown handlers: {ex.Message}", Logger.LogTypes.Error);
                }
                catch
                {
                    // Logger might not be available during early initialization
                }
            }
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            CleanupAllTimers();
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            CleanupAllTimers();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CleanupAllTimers();
        }

        private static void CleanupAllTimers()
        {
            try
            {
                while (_activeTimers.TryTake(out MultimediaTimer timer))
                {
                    try
                    {
                        timer?.Stop();
                        timer?.Dispose();
                    }
                    catch
                    {
                        // Timer zaten dispose edilmiş olabilir
                    }
                }
            }
            catch
            {
                // Best effort cleanup
            }
        }

        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
                return;

            using (var timer = new MultimediaTimer { Interval = milliseconds })
            {
                // Timer'ı aktif timer listesine ekle
                _activeTimers.Add(timer);

                using (var mre = new ManualResetEvent(false))
                {
                    object lockObj = new object();
                    bool disposed = false;

                    EventHandler? handler = null;
                    handler = (s, e) =>
                    {
                        lock (lockObj)
                        {
                            if (!disposed)
                            {
                                mre.Set(); // ManualResetEvent is thread-safe
                            }
                        }
                    };

                    timer.Tick += handler;

                    try
                    {
                        timer.Start();
                        IntPtr handle = mre.SafeWaitHandle.DangerousGetHandle();

                        uint pollInterval = Math.Min((uint)milliseconds / 10, 50);
                        if (pollInterval < 1) pollInterval = 1;

                        while (Application.OpenForms.Count > 0 && !Environment.HasShutdownStarted)
                        {
                            uint result = MsgWaitForMultipleObjects(1, new[] { handle }, false, pollInterval, QS_ALLINPUT);

                            if (result == WAIT_OBJECT_0)
                            {
                                break; // Timer is completed
                            }
                            else if (result == WAIT_OBJECT_0_MSG)
                            {
                                Application.DoEvents();
                            }

                            // Shutdown kontrolü
                            if (Environment.HasShutdownStarted)
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        lock (lockObj)
                        {
                            disposed = true;
                        }
                        timer.Stop();
                        timer.Tick -= handler;

                        // Timer'ı aktif listeden çıkarmaya çalış (best effort)
                        try
                        {
                            var tempList = new List<MultimediaTimer>();
                            while (_activeTimers.TryTake(out MultimediaTimer t))
                            {
                                if (t != timer)
                                    tempList.Add(t);
                            }
                            foreach (var t in tempList)
                                _activeTimers.Add(t);
                        }
                        catch
                        {
                            // Best effort cleanup
                        }
                    }
                }
            }
        }
    }
}