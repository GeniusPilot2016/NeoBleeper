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
        /// Handles timer events triggered by the system timer and raises the Tick event.
        /// </summary>
        /// <remarks>This method is intended to be used as a callback for system timer events and is not
        /// intended to be called directly by user code.</remarks>
        /// <param name="id">The identifier of the timer that generated the event.</param>
        /// <param name="msg">The system-defined message associated with the timer event.</param>
        /// <param name="user">A user-defined value associated with the timer, typically specified when the timer was created.</param>
        /// <param name="dw1">Additional message-specific information provided by the system.</param>
        /// <param name="dw2">Additional message-specific information provided by the system.</param>
        private void TimerCallback(uint id, uint msg, UIntPtr user, UIntPtr dw1, UIntPtr dw2)
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when you are finished using the object to free unmanaged resources
        /// and perform other cleanup operations. After calling this method, the object should not be used.</remarks>
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