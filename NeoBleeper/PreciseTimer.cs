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

using System.Diagnostics;

namespace NeoBleeper
{
    public class PreciseTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly System.Threading.Timer _timer;
        private readonly int _intervalMs;
        private readonly SynchronizationContext _syncContext;
        private bool _isRunning;

        // Tick event
        public event EventHandler<TimerTickEventArgs> Tick;

        public PreciseTimer(int intervalMs = 100)
        {
            _intervalMs = intervalMs;
            _syncContext = SynchronizationContext.Current; // Capture the UI context
            _stopwatch = new Stopwatch();
            _timer = new System.Threading.Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Starts the timer if it is not already running.
        /// </summary>
        /// <remarks>Subsequent calls to this method have no effect if the timer is already
        /// running.</remarks>
        public void Start()
        {
            if (!_isRunning)
            {
                _stopwatch.Start();
                _timer.Change(_intervalMs, _intervalMs);
                _isRunning = true;
            }
        }

        /// <summary>
        /// Stops the timer if it is currently running.
        /// </summary>
        /// <remarks>Calling this method has no effect if the timer is not running.</remarks>
        public void Stop()
        {
            if (_isRunning)
            {
                _stopwatch.Stop();
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _isRunning = false;
            }
        }

        /// <summary>
        /// Resets the timer to its initial state, stopping it if it is currently running.
        /// </summary>
        /// <remarks>After calling this method, the elapsed time is set to zero and the timer is not
        /// running. Use this method to prepare the timer for a new measurement.</remarks>
        public void Reset()
        {
            Stop();
            _stopwatch.Reset();
        }

        /// <summary>
        /// Stops and then starts the operation, effectively resetting its state.
        /// </summary>
        /// <remarks>Use this method to reinitialize the operation as if starting from the beginning. Any
        /// progress or state from a previous run will be cleared.</remarks>
        public void Restart()
        {
            Reset();
            Start();
        }

        public TimeSpan Elapsed => _stopwatch.Elapsed;
        public bool IsRunning => _isRunning;

        private void OnTimerTick(object state)
        {
            if (_isRunning)
            {
                var args = new TimerTickEventArgs(_stopwatch.Elapsed);

                if (_syncContext != null)
                {
                    try
                    {
                        _syncContext.Post(_ => Tick?.Invoke(this, args), null);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Exception during Tick event: {ex}", Logger.LogTypes.Error);
                    }
                }
                else
                {
                    Tick?.Invoke(this, args);
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when you are finished using the instance to free unmanaged resources
        /// and perform necessary cleanup. After calling this method, the instance should not be used.</remarks>
        public void Dispose()
        {
            Stop();
            // Detach the event handler to prevent calls during shutdown.
            Tick = null;
            _timer?.Dispose();
        }
    }

    public class TimerTickEventArgs : EventArgs
    {
        public TimeSpan Elapsed { get; }

        public TimerTickEventArgs(TimeSpan elapsed)
        {
            Elapsed = elapsed;
        }
    }
}
