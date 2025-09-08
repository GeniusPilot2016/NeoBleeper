using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class HighPrecisionSleep
    {
        // Single shared timer for all sleep requests
        private static readonly MultimediaTimer _sharedTimer;
        private static readonly object _sync = new();
        // dueTime (ms since epoch) -> list of waiters
        private static readonly SortedList<long, List<object>> _schedule = new();

        static HighPrecisionSleep()
        {
            _sharedTimer = new MultimediaTimer { Interval = 1 };
            _sharedTimer.Tick += SharedTimer_Tick;
            _sharedTimer.Start();
        }

        private static void SharedTimer_Tick(object? sender, EventArgs e)
        {
            long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            List<KeyValuePair<long, List<object>>> due = null;
            lock (_sync)
            {
                if (_schedule.Count == 0) return;
                int idx = 0;
                for (; idx < _schedule.Count; idx++)
                {
                    if (_schedule.Keys[idx] <= now) continue;
                    break;
                }
                if (idx == 0) return;
                due = _schedule.Take(idx).ToList();
                for (int i = 0; i < idx; i++) _schedule.RemoveAt(0);
            }

            if (due != null)
            {
                foreach (var kv in due)
                {
                    foreach (var waiter in kv.Value)
                    {
                        if (waiter is ManualResetEventSlim mre)
                            mre.Set();
                    }
                }
            }
        }

        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0) return;

            var mre = new ManualResetEventSlim(false);
            long due = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond + milliseconds;

            lock (_sync)
            {
                if (!_schedule.TryGetValue(due, out var list))
                {
                    list = new List<object>();
                    _schedule.Add(due, list);
                }
                list.Add(mre);
            }

            // Wait synchronously
            mre.Wait();
        }

        public static Task SleepAsync(int milliseconds)
        {
            // Return completed task for non-positive durations
            if (milliseconds <= 0) return Task.CompletedTask;
            return Task.Run(() => Sleep(milliseconds));
        }

        public static void Shutdown()
        {
            lock (_sync)
            {
                _sharedTimer?.Stop();
                _sharedTimer?.Dispose();
                _schedule.Clear();
            }
        }
    }
}