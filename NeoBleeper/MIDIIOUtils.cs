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
        public static void InitializeMidi()
        {
            try
            {
                int deviceCount = MidiOut.NumberOfDevices;
                if (deviceCount == 0)
                {
                    Console.WriteLine("No MIDI output devices found. Ensure the MIDI driver is installed.");
                    _midiOut = null; // Prevent further errors
                    return;
                }

                // Initialize the MIDI output device
                _midiOut = new MidiOut(Program.MIDIDevices.MIDIOutputDeviceChannel);
            }
            catch (MmException ex)
            {
                // Handle exception (log, show message, etc.)
                Console.WriteLine($"Error initializing MIDI: {ex.Message}");
                _midiOut = null; // Important: Set to null to prevent further errors
            }
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
            DisposeMidi(); // Dispose of the old device
            try
            {
                _midiOut = new MidiOut(deviceNumber);
            }
            catch (MmException ex)
            {
                // Handle exception (log, show message, etc.)
                Console.WriteLine($"Error changing MIDI device: {ex.Message}");
                _midiOut = null; // Important: Set to null to prevent further errors
            }
        }
        public static void ChangeInstrument(MidiOut midiOut, int programNumber, int channel)
        {
            if (_midiOut == null)
            {
                Debug.WriteLine("MIDI device is not available.");
                return;
            }

            try
            {
                midiOut.Send(MidiMessage.ChangePatch(programNumber, channel + 1).RawData);
            }
            catch (MmException ex)
            {
                Debug.WriteLine($"Error sending MIDI message: {ex.Message}");
                DisposeMidi(); // Dispose of the invalid device
                Console.WriteLine("MIDI device disconnected. Please reconnect or select a new device.");
            }
        }
        public static void ChangeChannel(MidiOut midiOut, int channel)
        {
            if (_midiOut == null)
            {
                Debug.WriteLine("MIDI device is not available.");
                return;
            }

            try
            {
                midiOut.Send(MidiMessage.ChangeControl(0, 0, channel + 1).RawData);
            }
            catch (MmException ex)
            {
                Debug.WriteLine($"Error sending MIDI message: {ex.Message}");
                DisposeMidi(); // Dispose of the invalid device
                Console.WriteLine("MIDI device disconnected. Please reconnect or select a new device.");
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

        public static Task PlayMidiNote(int note, int length) // Make it asynchronous
        {
            return PlayMidiNoteAsync(note, length); // Directly return the Task
        }

        public static async Task PlayMidiNoteAsync(int note, int length) // Make async
        {
            if (_midiOut == null) return;

            _midiOut.Send(MidiMessage.StartNote(note, DynamicVelocity(), 1).RawData);
            await Task.Delay(length); // Use Task.Delay
            _midiOut.Send(MidiMessage.StopNote(note, 0, 1).RawData);
        }
        public static int FrequencyToMidiNote(double frequency)
        {
            double note = 69 + 12 * Math.Log(frequency / 440.0, 2);
            return (int)Math.Round(note);
        }

        public static async Task PlayMidiNote(MidiOut midiOut, double frequency, int length)
        {
            int note = FrequencyToMidiNote(frequency);
            midiOut.Send(MidiMessage.StartNote(note, MIDIIOUtils.DynamicVelocity(), 1).RawData);
            await Task.Delay(length);
            midiOut.Send(MidiMessage.StopNote(note, 0, 1).RawData);
        }
        public static void CheckMIDIDriver()
        {
            int deviceCount = MidiOut.NumberOfDevices;
            if (deviceCount == 0)
            {
                Console.WriteLine("No MIDI output devices found. Ensure the MIDI driver is installed.");
            }
            else
            {
                Console.WriteLine($"Found {deviceCount} MIDI output device(s):");
                for (int i = 0; i < deviceCount; i++)
                {
                    Console.WriteLine($"Device {i}: {MidiOut.DeviceInfo(i).ProductName}");
                }
            }
        }
    }
}
