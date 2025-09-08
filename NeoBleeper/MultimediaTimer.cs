using System;
using System.Runtime.InteropServices;

namespace NeoBleeper
{
    public class MultimediaTimer : IDisposable
    {
        private delegate void TimerEventDelegate(uint id, uint msg, UIntPtr user, UIntPtr dw1, UIntPtr dw2);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeSetEvent(uint msDelay, uint msResolution, TimerEventDelegate handler, UIntPtr userCtx, uint eventType);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeKillEvent(uint uTimerId);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeBeginPeriod(uint uPeriod);

        [DllImport("winmm.dll", SetLastError = true)]
        private static extern uint timeEndPeriod(uint uPeriod);

        private TimerEventDelegate _handler;
        private uint _timerId;
        private bool _disposed;

        public event EventHandler? Tick;

        public int Interval { get; set; } = 1; // ms

        // timeBeginPeriod management across multiple instances
        private static int _globalPeriodRefCount = 0;
        private static readonly object _globalLock = new();

        public void Start()
        {
            if (_timerId != 0) return;

            // timeBeginPeriod(1) should be called only once per application
            lock (_globalLock)
            {
                if (_globalPeriodRefCount == 0)
                {
                    timeBeginPeriod(1);
                }
                _globalPeriodRefCount++;
            }

            _handler = TimerCallback;
            _timerId = timeSetEvent((uint)Interval, 0, _handler, UIntPtr.Zero, 1); // TIME_PERIODIC = 1

            if (_timerId == 0)
            {
                // Catch the error code to provide more context
                int err = Marshal.GetLastWin32Error();
                // De-clean the global period count if timer setup fails
                lock (_globalLock)
                {
                    _globalPeriodRefCount--;
                    if (_globalPeriodRefCount == 0)
                    {
                        timeEndPeriod(1);
                    }
                }
                throw new InvalidOperationException($"Multimedia timer could not be started. timeSetEvent returned 0. Win32Error={err}");
            }
        }

        public void Stop()
        {
            if (_timerId != 0)
            {
                timeKillEvent(_timerId);
                _timerId = 0;

                lock (_globalLock)
                {
                    _globalPeriodRefCount--;
                    if (_globalPeriodRefCount == 0)
                    {
                        timeEndPeriod(1);
                    }
                }
            }
        }

        private void TimerCallback(uint id, uint msg, UIntPtr user, UIntPtr dw1, UIntPtr dw2)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}