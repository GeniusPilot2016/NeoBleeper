﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace NeoBleeper
{
    public class NonBlockingSleep
    {
        // Windows API for high precision timing
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        
        [DllImport("kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        
        // Cache the frequency to avoid repeated system calls
        private static readonly long CachedFrequency = Stopwatch.Frequency;
        
        public static void Sleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return; // Negative sleep time is not valid
            }
            
            // Use microsecond precision for better accuracy
            SleepMicroseconds(milliseconds * 1000);
        }
        
        public static void SleepMicroseconds(long microseconds)
        {
            if (microseconds <= 0)
            {
                return;
            }
            
            // Calculate target ticks with higher precision
            long targetTicks = (CachedFrequency * microseconds) / 1000000;
            var stopwatch = Stopwatch.StartNew();
            
            // Adaptive waiting strategy for better precision
            const long spinThreshold = 15000; // ~15ms in ticks (adjust based on system)
            long remainingTicks = targetTicks;
            
            while (remainingTicks > 0)
            {
                long elapsedTicks = stopwatch.ElapsedTicks;
                remainingTicks = targetTicks - elapsedTicks;
                
                if (remainingTicks > spinThreshold)
                {
                    // For longer waits, use DoEvents to prevent UI blocking
                    Application.DoEvents();
                    // Add a very small Thread.Yield() to improve cooperative multitasking
                    Thread.Yield();
                }
                else if (remainingTicks > 0)
                {
                    // For the final precision phase, use tight spinning
                    // This provides sub-millisecond accuracy
                    continue;
                }
            }
            
            stopwatch.Stop();
        }
        
        // High precision sleep for very short durations
        public static void SleepNanoseconds(long nanoseconds)
        {
            if (nanoseconds <= 0)
            {
                return;
            }
            
            long targetTicks = (CachedFrequency * nanoseconds) / 1000000000;
            var stopwatch = Stopwatch.StartNew();
            
            // Pure spinning for nanosecond precision
            while (stopwatch.ElapsedTicks < targetTicks)
            {
                // Tight loop for maximum precision
            }
            
            stopwatch.Stop();
        }
    }
}