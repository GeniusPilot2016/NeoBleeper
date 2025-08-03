using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public void Start()
        {
            if (!_isRunning)
            {
                _stopwatch.Start();
                _timer.Change(_intervalMs, _intervalMs);
                _isRunning = true;
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                _stopwatch.Stop();
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _isRunning = false;
            }
        }

        public void Reset()
        {
            Stop();
            _stopwatch.Reset();
        }

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
                        Debug.WriteLine($"Exception during Tick event: {ex}");
                    }
                }
                else
                {
                    Tick?.Invoke(this, args);
                }
            }
        }

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
