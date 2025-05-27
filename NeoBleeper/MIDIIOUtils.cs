using NAudio.Midi;
using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NeoBleeper
{
    public static class MIDIIOUtils
    {
        public static MidiOut _midiOut; // Class-level variable for MIDI output
        public static MidiIn _midiIn; // Class-level variable for MIDI input
        public static void InitializeMidi(int retryCount = 3)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    _midiOut = new MidiOut(Program.MIDIDevices.MIDIOutputDevice);
                    return; // Break out of the loop if successful
                }
                catch (MmException ex)
                {
                    Debug.WriteLine($"Error initializing MIDI device (attempt {i + 1}): {ex.Message}");
                    _midiOut = null;

                    if (i < retryCount - 1)
                    {
                        System.Threading.Thread.Sleep(200 * (i + 1)); // Incremental backoff
                    }
                }
            }
            Debug.WriteLine("Failed to initialize MIDI device after multiple attempts");
        }
        public static void DisposeMidiOutput()
        {
            if (_midiOut != null)
            {
                _midiOut.Dispose();
                _midiOut = null;
            }
        }
        public static void DisposeMidiInput()
        {
            if (_midiIn != null)
            {
                _midiIn.Dispose();
                _midiIn = null;
            }
        }
        public static void ChangeInputDevice(int deviceNumber)
        {
            DisposeMidiInput(); // Release old MIDI device
            GC.Collect(); // Run garbage collector
            GC.WaitForPendingFinalizers(); // Wait for all finalizers to finish
            try
            {
                _midiIn = new MidiIn(deviceNumber);
            }
            catch (MmException ex)
            {
                Debug.WriteLine($"Error changing MIDI input device: {ex.Message}");
                _midiIn = null;
            }
        }
        public static void ChangeOutputDevice(int deviceNumber)
        {
            DisposeMidiOutput(); // Release old MIDI device
            GC.Collect(); // Run garbage collector
            GC.WaitForPendingFinalizers(); // Wait for all finalizers to finish
            try
            {
                _midiOut = new MidiOut(deviceNumber);
            }
            catch (MmException ex)
            {
                Debug.WriteLine($"Error changing MIDI output device: {ex.Message}");
                _midiOut = null;
            }
        }
        public static void ChangeInstrument(MidiOut midiOut, int programNumber, int channel)
        {
            try
            {
                midiOut.Send(MidiMessage.ChangePatch(programNumber, channel + 1).RawData);
            }
            catch (MmException)
            {
                Debug.WriteLine("MIDI output device not found.");
            }
        }
        public static int DynamicVelocity()
        {
            Random random = new Random();
            int minVelocity = 90;  // Minimum velocity
            int maxVelocity = 127; // Maximum velocity
            int dynamicVelocity = random.Next(minVelocity, maxVelocity);
            return dynamicVelocity;
        }

        public static async void PlayMidiNote(int note, int length, bool nonStopping = false) //Keep the old method for compatibility
        {
            try
            {
                await PlayMidiNoteAsync(note, length, nonStopping);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing MIDI note: {ex.Message}");
            }

        }

        public static async Task PlayMidiNoteAsync(int note, int length, bool nonStopping = false) // Make async
        {
            if (_midiOut == null) return;

            _midiOut.Send(MidiMessage.StartNote(note, DynamicVelocity(), Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await Task.Delay(length); // Use Task.Delay
            if (!nonStopping)
            {
                _midiOut.Send(MidiMessage.StopNote(note, 0, Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
        }
        public static int FrequencyToMidiNote(double frequency)
        {
            double note = 69 + 12 * Math.Log(frequency / 440.0, 2);
            return (int)Math.Round(note);
        }
        public static int MidiNoteToFrequency(int note)
        {
            return (int)(440.0 * Math.Pow(2, (note - 69) / 12.0));
        }

        public static async Task PlayMidiNote(MidiOut midiOut, double frequency, int length, bool nonStopping = false)
        {
            int note = FrequencyToMidiNote(frequency);
            midiOut.Send(MidiMessage.StartNote(note, MIDIIOUtils.DynamicVelocity(), Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await Task.Delay(length);
            if (!nonStopping)
            {
                midiOut.Send(MidiMessage.StopNote(note, 0, Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
        }

        internal static void StopAllNotes()
        {
            if (_midiOut != null)
            {
                for (int note = 0; note < 128; note++)
                {
                    _midiOut.Send(MidiMessage.StopNote(note, 0, Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
                }
            }
            else
            {
                Debug.WriteLine("MIDI output device not initialized.");
            }
        }

        internal static void StopMidiNote(int midiNote)
        {
            if (_midiOut != null)
            {
                _midiOut.Send(MidiMessage.StopNote(midiNote, 0, Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
            else
            {
                Debug.WriteLine("MIDI output device not initialized.");
            }
        }
    }
}
