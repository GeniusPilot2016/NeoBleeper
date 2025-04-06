using NAudio.Midi;
using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class MIDIIOUtils
    {
        public static MidiOut _midiOut; // Class-level variable
        public static void InitializeMidi()
        {
            try
            {
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
        public static void ChangeInstrument(MidiOut midiOut, int programNumber, int channel)
        {
            midiOut.Send(MidiMessage.ChangePatch(programNumber, channel + 1).RawData);
        }
        public static int DynamicVelocity()
        {
            Random random = new Random();
            int minVelocity = 90;  // Minimum velocity
            int maxVelocity = 127; // Maximum velocity
            int dynamicVelocity = random.Next(minVelocity, maxVelocity);
            return dynamicVelocity;
        }

        public static void PlayMidiNote(int note, int length) //Keep the old method for compatibility
        {
            PlayMidiNoteAsync(note, length).Wait();
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
    }
}
