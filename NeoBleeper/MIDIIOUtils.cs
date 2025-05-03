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
        public static MidiOut _midiOut; // Class-level variable
        public static void InitializeMidi(int retryCount = 3)
        {
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    _midiOut = new MidiOut(Program.MIDIDevices.MIDIOutputDeviceChannel);
                    return; // Başarılı olursa döngüden çık
                }
                catch (MmException ex)
                {
                    Debug.WriteLine($"Error initializing MIDI (attempt {i + 1}): {ex.Message}");
                    _midiOut = null;

                    if (i < retryCount - 1)
                    {
                        System.Threading.Thread.Sleep(200 * (i + 1)); // Her denemede artan gecikme
                    }
                }
            }
            Debug.WriteLine("Failed to initialize MIDI after multiple attempts");
        }
        public static void DisposeMidi()
        {
            if (_midiOut != null)
            {
                _midiOut.Dispose();
                _midiOut = null;
            }
        }
        public static void ChangeDevice(int deviceNumber)
        {
            DisposeMidi(); // Release old MIDI device
            GC.Collect(); // Run garbage collector
            GC.WaitForPendingFinalizers(); // Wait for all finalizers to finish
            try
            {
                _midiOut = new MidiOut(deviceNumber);
            }
            catch (MmException ex)
            {
                Debug.WriteLine($"Error changing MIDI device: {ex.Message}");
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
                Debug.WriteLine("MIDI device not found.");
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

        public static async void PlayMidiNote(int note, int length) //Keep the old method for compatibility
        {
            try
            {
                await PlayMidiNoteAsync(note, length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing MIDI note: {ex.Message}");
            }

        }

        public static async Task PlayMidiNoteAsync(int note, int length) // Make async
        {
            if (_midiOut == null) return;

            _midiOut.Send(MidiMessage.StartNote(note, DynamicVelocity(), Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await Task.Delay(length); // Use Task.Delay
            _midiOut.Send(MidiMessage.StopNote(note, 0, Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
        }
        public static int FrequencyToMidiNote(double frequency)
        {
            double note = 69 + 12 * Math.Log(frequency / 440.0, 2);
            return (int)Math.Round(note);
        }

        public static async Task PlayMidiNote(MidiOut midiOut, double frequency, int length)
        {
            int note = FrequencyToMidiNote(frequency);
            midiOut.Send(MidiMessage.StartNote(note, MIDIIOUtils.DynamicVelocity(), Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await Task.Delay(length);
            midiOut.Send(MidiMessage.StopNote(note, 0, Program.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
        }
    }
}
