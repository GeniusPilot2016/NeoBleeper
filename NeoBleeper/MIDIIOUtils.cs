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

using NAudio.Midi;
using NAudio;

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
                    _midiOut = new MidiOut(TemporarySettings.MIDIDevices.MIDIOutputDevice);
                    return; // Break out of the loop if successful
                }
                catch (MmException ex)
                {
                    Logger.Log($"Error initializing MIDI device (attempt {i + 1}): {ex.Message}", Logger.LogTypes.Error);
                    _midiOut = null;

                    if (i < retryCount - 1)
                    {
                        System.Threading.Thread.Sleep(200 * (i + 1)); // Incremental backoff
                    }
                }
            }
            Logger.Log("Failed to initialize MIDI device after multiple attempts", Logger.LogTypes.Error);
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
                Logger.Log($"Error changing MIDI input device: {ex.Message}", Logger.LogTypes.Error);
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
                Logger.Log($"Error changing MIDI output device: {ex.Message}", Logger.LogTypes.Error);
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
                Logger.Log("MIDI output device not found.", Logger.LogTypes.Error);
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
                Logger.Log($"Error playing MIDI note: {ex.Message}", Logger.LogTypes.Error);
            }

        }

        public static async Task PlayMidiNoteAsync(int note, int length, bool nonStopping = false) // Make async
        {
            if (_midiOut == null) return;

            _midiOut.Send(MidiMessage.StartNote(note, DynamicVelocity(), TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await HighPrecisionSleep.SleepAsync(length);
            if (!nonStopping)
            {
                _midiOut.Send(MidiMessage.StopNote(note, 0, TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
        }
        public static async Task PlayMidiNoteAsync(int note, int length, int instrument, bool nonStopping = false, int? channel = null)
        {
            if (_midiOut == null) return;
            
            int midiChannel = channel ?? TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel;
            int originalInstrument = TemporarySettings.MIDIDevices.MIDIOutputInstrument;
            if (midiChannel == 9) // Channel 10 (percussion)
            {
                _midiOut.Send(MidiMessage.StartNote(note, DynamicVelocity(), midiChannel + 1).RawData);
                await HighPrecisionSleep.SleepAsync(length);
                if (!nonStopping)
                    _midiOut.Send(MidiMessage.StopNote(note, 0, midiChannel + 1).RawData);
            }
            else
            {
                ChangeInstrument(_midiOut, instrument, midiChannel);
                await PlayMidiNoteAsync(note, length, nonStopping);
                ChangeInstrument(_midiOut, originalInstrument, midiChannel);
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
            midiOut.Send(MidiMessage.StartNote(note, MIDIIOUtils.DynamicVelocity(), TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await HighPrecisionSleep.SleepAsync(length);
            if (!nonStopping)
            {
                midiOut.Send(MidiMessage.StopNote(note, 0, TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
        }

        internal static void StopAllNotes()
        {
            if (_midiOut != null)
            {
                for (int note = 0; note < 128; note++)
                {
                    _midiOut.Send(MidiMessage.StopNote(note, 0, TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
                }
            }
            else
            {
                Logger.Log("MIDI output device not initialized.", Logger.LogTypes.Error);
            }
        }

        internal static void StopMidiNote(int midiNote)
        {
            if (_midiOut != null)
            {
                _midiOut.Send(MidiMessage.StopNote(midiNote, 0, TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
            else
            {
                Logger.Log("MIDI output device not initialized.", Logger.LogTypes.Error);
            }
        }
    }
}
