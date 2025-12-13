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

        /// <summary>
        /// Blocks the current thread for the specified number of milliseconds.
        /// </summary>
        /// <remarks>If the value of milliseconds is less than or equal to zero, the method returns
        /// immediately without blocking. This method suspends only the calling thread and does not affect other
        /// threads.</remarks>
        /// <param name="milliseconds">The number of milliseconds for which the current thread is to be suspended. Must be greater than zero.</param>
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

        /// <summary>
        /// Asynchronously suspends execution for the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The duration, in milliseconds, for which to suspend execution. Must be zero or greater.</param>
        /// <returns>A task that represents the asynchronous delay operation. The task is completed immediately if the specified
        /// duration is zero or negative.</returns>
        public static Task SleepAsync(int milliseconds)
        {
            // Return completed task for non-positive durations
            if (milliseconds <= 0) return Task.CompletedTask;
            return Task.Run(() => Sleep(milliseconds));
        }

        /// <summary>
        /// Shuts down the shared timer and releases all associated resources.
        /// </summary>
        /// <remarks>After calling this method, any scheduled operations managed by the shared timer are
        /// cancelled and cannot be resumed. This method is thread-safe and can be called multiple times without
        /// throwing exceptions.</remarks>
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