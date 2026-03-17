// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

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

        /// <summary>
        /// Starts the multimedia timer if it is not already running.
        /// </summary>
        /// <remarks>This method has no effect if the timer is already running. Once started, the timer
        /// will trigger callbacks at the specified interval until it is stopped. The timer uses system-level resources
        /// and may affect system timer resolution while active.</remarks>
        /// <exception cref="InvalidOperationException">Thrown if the timer cannot be started due to a failure in the underlying system call.</exception>
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

        /// <summary>
        /// Stops the timer and releases any associated system resources.
        /// </summary>
        /// <remarks>If the timer is not currently running, this method has no effect. After calling this
        /// method, the timer cannot be restarted unless it is reinitialized.</remarks>
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

        /// <summary>
        /// Handles timer callback events and invokes registered tick event handlers.
        /// </summary>
        /// <remarks>This method is intended to be called by the timer infrastructure when a timer event
        /// occurs. Any exceptions thrown by event handlers are caught to prevent the timer from affecting application
        /// stability.</remarks>
        /// <param name="id">The identifier of the timer instance triggering the callback.</param>
        /// <param name="msg">The message code associated with the timer event.</param>
        /// <param name="user">A user-defined value passed to the timer callback, typically used for context.</param>
        /// <param name="dw1">Additional timer-specific data provided to the callback.</param>
        /// <param name="dw2">Additional timer-specific data provided to the callback.</param>
        private void TimerCallback(uint id, uint msg, UIntPtr user, UIntPtr dw1, UIntPtr dw2)
        {
            // It shouldn't affect the application if the event handler throws native exceptions, but it should catch them to prevent the timer from crashing the app
            var handler = Tick;
            if (handler == null) return;

            foreach (Delegate d in handler.GetInvocationList())
            {
                try
                {
                    if (d is EventHandler eh)
                    {
                        eh.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        // General safety fallback for non-EventHandler delegates, though in this design we expect only EventHandlers to be subscribed
                        d.DynamicInvoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    // Use Logger to log the exception, but catch any exceptions from Logger itself to avoid cascading failures
                    try
                    {
                        Logger.Log($"Timer callback handler threw: {ex.GetType().Name} {ex.Message}", Logger.LogTypes.Error);
                    }
                    catch
                    {
                        // Mute any exceptions from Logger to prevent further issues, as it's already a failure scenario
                    }
                }
            }
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