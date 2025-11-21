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

using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NeoBleeper
{
    // Usage: Call PowerManager.Initialize() once from the UI thread at application startup.
    // Subscribe to events to handle system power and session changes.
    // Call PowerManager.Cleanup() on application exit to unsubscribe and clean up resources.
    // Note: Events are raised on the UI thread if initialized from the UI thread.
    // PowerManager.SystemSleeping += (s, e) => { /* Handle system sleeping */ };
    // PowerManager.SystemResuming += (s, e) => { /* Handle system resuming */ };
    // PowerManager.PreparingToShutdown += (s, e) => { /* Handle preparing to shutdown */ };
    // PowerManager.Shutdown += (s, e) => { /* Handle shutdown */ };
    // PowerManager.PreparingToLogoff += (s, e) => { /* Handle preparing to logoff */ };
    // PowerManager.Logoff += (s, e) => { /* Handle logoff */ };
    // PowerManager.SystemHibernating += (s, e) => { /* Handle system hibernating */ };
    // Note: Windows may not always report hibernation separately; some systems just suspend.
    public static class PowerManager
    {
        // Events
        public static event EventHandler? SystemSleeping;
        public static event EventHandler? SystemResuming;
        public static event EventHandler? SystemHibernating; // Windows her zaman ayrı bildirmeyebilir (açıklamaya bakın)
        public static event EventHandler? PreparingToShutdown; // WM_QUERYENDSESSION (shutdown)
        public static event EventHandler? PreparingToLogoff;   // WM_QUERYENDSESSION (logoff)
        public static event EventHandler? Shutdown;            // WM_ENDSESSION (shutdown)
        public static event EventHandler? Logoff;              // WM_ENDSESSION (logoff)

        private static bool initialized = false;
        private static SynchronizationContext? uiContext;
        private static IMessageFilter? filter;

        // Windows message constants to listen for
        private const int WM_POWERBROADCAST = 0x0218;
        private const int PBT_APMSUSPEND = 0x0004;
        private const int PBT_APMRESUMESUSPEND = 0x0007;
        private const int WM_QUERYENDSESSION = 0x0011;
        private const int WM_ENDSESSION = 0x0016;
        private const uint ENDSESSION_LOGOFF = 0x80000000u;

        // Initialize: They should be called once from the UI thread at application startup
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            // Catch the UI thread's SynchronizationContext for event invocation
            uiContext = SynchronizationContext.Current;

            // Catching Windows messages via IMessageFilter implementation, which are WM_POWERBROADCAST, WM_QUERYENDSESSION, WM_ENDSESSION
            filter = new PowerMessageFilter();
            Application.AddMessageFilter(filter);

            // Subscribe to SystemEvents as a fallback for power mode changes and session ending
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            // Note: SystemEvents.SessionEnded does not exist; WM_ENDSESSION capture confirms Shutdown/Logoff.
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private static void Application_ApplicationExit(object? sender, EventArgs e)
        {
            Cleanup();
        }

        // Cleanup: Call to unsubscribe and clean up resources
        public static void Cleanup()
        {
            if (!initialized) return;
            initialized = false;

            if (filter != null)
            {
                try { Application.RemoveMessageFilter(filter); } catch { }
                filter = null;
            }

            try { SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged; } catch { }
            try { SystemEvents.SessionEnding -= SystemEvents_SessionEnding; } catch { }
            try { Application.ApplicationExit -= Application_ApplicationExit; } catch { }
        }

        // Fallback for SystemEvents.PowerModeChanged event
        private static void SystemEvents_PowerModeChanged(object? sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    RaiseOnUi(SystemSleeping);
                    break;
                case PowerModes.Resume:
                    RaiseOnUi(SystemResuming);
                    break;
            }
        }

        // SystemEvents.SessionEnding: It indicates logoff or shutdown is starting (WM_QUERYENDSESSION equivalent)
        private static void SystemEvents_SessionEnding(object? sender, SessionEndingEventArgs e)
        {
            if (e.Reason == SessionEndReasons.Logoff)
            {
                RaiseOnUi(PreparingToLogoff);
            }
            else 
            { 
                RaiseOnUi(PreparingToShutdown);
            }
        }

        // Message filter: WM_POWERBROADCAST, WM_QUERYENDSESSION, WM_ENDSESSION
        private class PowerMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                try
                {
                    if (m.Msg == WM_POWERBROADCAST)
                    {
                        int wParam = m.WParam.ToInt32();
                        if (wParam == PBT_APMSUSPEND)
                        {
                            // The system is suspending operation.
                            RaiseOnUi(SystemSleeping);

                            // Not all systems report hibernation separately; some just suspend.
                            RaiseOnUi(SystemHibernating);
                        }
                        else if (wParam == PBT_APMRESUMESUSPEND)
                        {
                            RaiseOnUi(SystemResuming);
                        }
                    }
                    else if (m.Msg == WM_QUERYENDSESSION)
                    {
                        // Logoff or shutdown is being initiated.
                        uint lParam = (uint)m.LParam.ToInt64();
                        bool isLogoff = (lParam & ENDSESSION_LOGOFF) != 0;
                        if (isLogoff)
                            RaiseOnUi(PreparingToLogoff);
                        else
                            RaiseOnUi(PreparingToShutdown);
                    }
                    else if (m.Msg == WM_ENDSESSION)
                    {
                        // Logoff or shutdown is in progress.
                        int wParam = m.WParam.ToInt32();
                        if (wParam != 0)
                        {
                            uint lParam = (uint)m.LParam.ToInt64();
                            bool isLogoff = (lParam & ENDSESSION_LOGOFF) != 0;
                            if (isLogoff)
                                RaiseOnUi(Logoff);
                            else
                                RaiseOnUi(Shutdown);
                        }
                    }
                }
                catch
                {
                    // Swallow any exceptions to avoid crashing the application
                }
                return false; // Continue processing other messages
            }
        }

        // Helper: Raise the event on the UI thread (posts to SynchronizationContext if available)
        private static void RaiseOnUi(EventHandler? handler)
        {
            if (handler == null) return;
            if (uiContext != null)
            {
                try
                {
                    uiContext.Post(_ => SafeInvoke(handler), null);
                    return;
                }
                catch
                {
                    // Directly call below if posting fails
                }
            }
            SafeInvoke(handler);
        }

        private static void SafeInvoke(EventHandler handler)
        {
            try
            {
                handler.Invoke(null, EventArgs.Empty);
            }
            catch
            {
                // Swallow exceptions from event handlers to avoid crashing the application
            }
        }
    }
}