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

using NAudio;
using NAudio.Midi;

namespace NeoBleeper
{
    public static class MIDIIOUtils
    {
        public static MidiOut _midiOut; // Class-level variable for MIDI output
        public static MidiIn _midiIn; // Class-level variable for MIDI input

        /// <summary>
        /// Initializes the MIDI output device, retrying the operation if initialization fails.
        /// </summary>
        /// <remarks>If initialization fails, the method will retry up to the specified number of times
        /// with incremental delays between attempts. If all attempts fail, the MIDI output device will remain
        /// uninitialized and an error will be logged.</remarks>
        /// <param name="retryCount">The number of times to attempt initialization before giving up. Must be greater than zero. Defaults to 3.</param>
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

        /// <summary>
        /// Releases resources associated with the current MIDI output device and resets the output to an uninitialized
        /// state.
        /// </summary>
        /// <remarks>Call this method when the MIDI output device is no longer needed to free system
        /// resources. After calling this method, attempts to use the MIDI output device may result in errors until it
        /// is reinitialized.</remarks>
        public static void DisposeMidiOutput()
        {
            if (_midiOut != null)
            {
                _midiOut.Dispose();
                _midiOut = null;
            }
        }

        /// <summary>
        /// Releases resources associated with the current MIDI input device, if one is open.
        /// </summary>
        /// <remarks>Call this method to clean up the MIDI input device when it is no longer needed. After
        /// calling this method, the MIDI input device will be disposed and unavailable for further use until
        /// reinitialized.</remarks>
        public static void DisposeMidiInput()
        {
            if (_midiIn != null)
            {
                _midiIn.Dispose();
                _midiIn = null;
            }
        }

        /// <summary>
        /// Changes the active MIDI input device to the specified device number.
        /// </summary>
        /// <remarks>Releases any previously active MIDI input device before switching. If the specified
        /// device number is invalid or unavailable, the active MIDI input device will be set to null and an error will
        /// be logged.</remarks>
        /// <param name="deviceNumber">The zero-based index of the MIDI input device to activate.</param>
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

        /// <summary>
        /// Changes the current MIDI output device to the specified device number.
        /// </summary>
        /// <remarks>If the specified device number is invalid or the device cannot be opened, the output
        /// device will not be changed and MIDI output will be disabled until a valid device is selected. Any previously
        /// opened MIDI output device is released before switching.</remarks>
        /// <param name="deviceNumber">The zero-based index of the MIDI output device to select. Must correspond to a valid device available on the
        /// system.</param>
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

        /// <summary>
        /// Sends a MIDI program change message to set the instrument for the specified channel on the given MIDI output
        /// device.
        /// </summary>
        /// <remarks>If the specified MIDI output device is not available, the method logs an error and
        /// does not throw an exception. The method increments the channel by one to match the MIDI specification, which
        /// uses one-based channel numbering.</remarks>
        /// <param name="midiOut">The MIDI output device to which the program change message will be sent. Cannot be null.</param>
        /// <param name="programNumber">The program number of the instrument to select. Valid values are typically 0 to 127, depending on the MIDI
        /// device.</param>
        /// <param name="channel">The zero-based MIDI channel on which to change the instrument. Must be in the range 0 to 15.</param>
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

        /// <summary>
        /// Generates a random MIDI velocity value within a typical dynamic range.
        /// </summary>
        /// <remarks>This method is useful for simulating expressive dynamics in MIDI applications by
        /// providing a velocity value within a musically expressive range. Each call produces a new random
        /// value.</remarks>
        /// <returns>An integer representing a randomly selected velocity value between 90 (inclusive) and 127 (exclusive).</returns>
        public static int DynamicVelocity()
        {
            Random random = new Random();
            int minVelocity = 90;  // Minimum velocity
            int maxVelocity = 127; // Maximum velocity
            int dynamicVelocity = random.Next(minVelocity, maxVelocity);
            return dynamicVelocity;
        }

        /// <summary>
        /// Plays a MIDI note with the specified pitch and duration.
        /// </summary>
        /// <remarks>This method is provided for compatibility and invokes an asynchronous operation
        /// internally. Exceptions that occur during playback are logged but not propagated to the caller. For improved
        /// error handling, consider using the asynchronous PlayMidiNoteAsync method.</remarks>
        /// <param name="note">The MIDI note number to play. Valid values are typically in the range 0 to 127, where 60 represents middle
        /// C.</param>
        /// <param name="length">The duration of the note in milliseconds. Must be a positive integer.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, the note will not be stopped automatically after the specified duration.</param>
        public static async void PlayMidiNote(int note, int length, bool nonStopping = false) // Keep the old method for compatibility
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

        /// <summary>
        /// Plays a MIDI note asynchronously for the specified duration.
        /// </summary>
        /// <remarks>If the MIDI output device is not initialized, the method returns without playing a
        /// note. The method changes the instrument and channel according to the current MIDI device settings before
        /// playing the note.</remarks>
        /// <param name="note">The MIDI note number to play. Valid values are typically in the range 0 to 127.</param>
        /// <param name="length">The duration, in milliseconds, for which the note is played.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, the note will not be stopped after the specified duration; otherwise, the
        /// note will be stopped automatically.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task PlayMidiNoteAsync(int note, int length, bool nonStopping = false) // Make async
        {
            if (_midiOut == null) return;

            ChangeInstrument(_midiOut, TemporarySettings.MIDIDevices.MIDIOutputInstrument, TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel);
            _midiOut.Send(MidiMessage.StartNote(note, DynamicVelocity(), TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            await HighPrecisionSleep.SleepAsync(length);
            if (!nonStopping)
            {
                _midiOut.Send(MidiMessage.StopNote(note, 0, TemporarySettings.MIDIDevices.MIDIOutputDeviceChannel + 1).RawData);
            }
        }

        /// <summary>
        /// Plays a MIDI note asynchronously using the specified note, length, instrument, and channel settings.
        /// </summary>
        /// <remarks>If the specified channel is 9 (MIDI channel 10, commonly used for percussion), the
        /// note is played as a percussion sound. If the MIDI output device is not initialized, the method returns
        /// without playing a note.</remarks>
        /// <param name="note">The MIDI note number to play. Valid values are typically in the range 0 to 127.</param>
        /// <param name="length">The duration, in milliseconds, for which the note is played.</param>
        /// <param name="instrument">The MIDI instrument number to use for playback. Valid values are typically in the range 0 to 127.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, the note will not be explicitly stopped after the specified length;
        /// otherwise, the note will be stopped automatically.</param>
        /// <param name="channel">The MIDI channel on which to play the note. If <see langword="null"/>, the default output channel is used.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
            }
        }

        /// <summary>
        /// Converts a frequency in hertz to the nearest MIDI note number.
        /// </summary>
        /// <remarks>A frequency of 440 Hz corresponds to MIDI note number 69 (A4). Values outside the
        /// standard MIDI note range (0–127) may be returned for frequencies outside the typical musical
        /// range.</remarks>
        /// <param name="frequency">The frequency in hertz to convert. Must be greater than 0.</param>
        /// <returns>The MIDI note number corresponding to the specified frequency, rounded to the nearest integer.</returns>
        public static int FrequencyToMidiNote(double frequency)
        {
            double note = 69 + 12 * Math.Log(frequency / 440.0, 2);
            return (int)Math.Round(note);
        }

        /// <summary>
        /// Converts a MIDI note number to its corresponding frequency in hertz (Hz).
        /// </summary>
        /// <remarks>The calculation is based on the equal-tempered scale, where A4 (MIDI note 69) is set
        /// to 440 Hz. Values outside the standard MIDI note range may produce frequencies outside the typical audible
        /// range.</remarks>
        /// <param name="note">The MIDI note number to convert. Typically ranges from 0 to 127, where 69 represents the standard A4 (440
        /// Hz).</param>
        /// <returns>The frequency in hertz (Hz) corresponding to the specified MIDI note number, rounded to the nearest integer.</returns>
        public static int MidiNoteToFrequency(int note)
        {
            return (int)(440.0 * Math.Pow(2, (note - 69) / 12.0));
        }

        /// <summary>
        /// Plays a MIDI note corresponding to the specified frequency for the given duration using the provided MIDI
        /// output device.
        /// </summary>
        /// <remarks>If <paramref name="nonStopping"/> is set to <see langword="true"/>, the caller is
        /// responsible for sending a note off message to stop the note. The method uses the current MIDI output device
        /// channel as configured in application settings.</remarks>
        /// <param name="midiOut">The MIDI output device used to send the note on and note off messages. Cannot be null.</param>
        /// <param name="frequency">The frequency, in hertz, of the note to play. Must be a positive value.</param>
        /// <param name="length">The duration, in milliseconds, for which the note should be played.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, the note will not be stopped automatically after the specified duration;
        /// otherwise, the note will be stopped when the duration elapses.</param>
        /// <returns>A task that represents the asynchronous operation of playing the MIDI note.</returns>
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

        /// <summary>
        /// Plays a metronome beat on the specified MIDI output device, using an accented or regular sound based on the
        /// provided parameters.
        /// </summary>
        /// <remarks>The method sends a single MIDI note to the specified output device on channel 10,
        /// commonly used for percussion. The accented beat uses a higher-pitched note than the regular beat.</remarks>
        /// <param name="midiOut">The MIDI output device to which the metronome beat will be sent. Cannot be null.</param>
        /// <param name="isAccent">Indicates whether the beat should be accented. Set to <see langword="true"/> for an accented beat;
        /// otherwise, <see langword="false"/> for a regular beat.</param>
        /// <param name="length">The duration of the metronome beat, in milliseconds. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation of playing the metronome beat.</returns>
        public static async Task PlayMetronomeBeatOnMIDI(MidiOut midiOut, bool isAccent, int length)
        {
            string noteStr = isAccent ? "A#1" : "A1";
            double frequency = NoteFrequencies.GetFrequencyFromNoteName(noteStr);
            int note = FrequencyToMidiNote(frequency);
            await PlayMidiNoteAsync(note, length, 0, false, 9);
        }

        /// <summary>
        /// Stops all currently playing MIDI notes on the configured output device.
        /// </summary>
        /// <remarks>If no MIDI output device is initialized, the method logs an error and takes no
        /// action. This method is typically used to ensure that all notes are silenced, such as when resetting the MIDI
        /// state or stopping playback.</remarks>
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

        /// <summary>
        /// Sends a MIDI message to stop playback of the specified MIDI note on the configured output device.
        /// </summary>
        /// <remarks>If the MIDI output device is not initialized, the method logs an error and does not
        /// send a message.</remarks>
        /// <param name="midiNote">The MIDI note number to stop. Valid values are typically in the range 0 to 127.</param>
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

        /// <summary>
        /// Sends a MIDI Note Off message for the specified note and channel.
        /// </summary>
        /// <param name="noteNumber">The MIDI note number to stop. Valid values are typically in the range 0 to 127.</param>
        /// <param name="channel">The zero-based MIDI channel on which to send the message. Must be between 0 and 15.</param>
        public static void SendNoteOff(int noteNumber, int channel)
        {
            if (_midiOut == null) return;
            _midiOut.Send(MidiMessage.StopNote(noteNumber, 0, ClampChannel(channel + 1)).RawData);
        }

        /// <summary>
        /// Sends a MIDI 'Note On' message for the specified note, instrument, and channel.
        /// </summary>
        /// <param name="noteNumber">The MIDI note number to play. Valid values are typically in the range 0 to 127, where 60 represents middle
        /// C.</param>
        /// <param name="instrument">The MIDI program number specifying the instrument sound to use. Valid values are typically in the range 0 to
        /// 127.</param>
        /// <param name="channel">The MIDI channel on which to send the message. Valid values are typically in the range 0 to 15.</param>
        public static void SendNoteOn(int noteNumber, int instrument, int channel)
        {
            if (_midiOut == null) return;
            ChangeInstrument(_midiOut, instrument, channel);
            _midiOut.Send(MidiMessage.StartNote(noteNumber, DynamicVelocity(), ClampChannel(channel + 1)).RawData);
        }

        /// <summary>
        /// Sends a Note Off message for every MIDI note on all MIDI channels.
        /// </summary>
        /// <remarks>Use this method to ensure that all notes are turned off across all channels, which
        /// can be useful for resetting the state of a MIDI device or stopping any lingering sounds.</remarks>
        public static void SendNoteOffToAllNotes()
        {
            for (int note = 0; note < 128; note++)
            {
                for (int channel = 0; channel < 16; channel++)
                {
                    SendNoteOff(note, channel);
                }
            }
        }

        /// <summary>
        /// Restricts the specified channel value to a maximum of 16.
        /// </summary>
        /// <param name="channel">The channel value to clamp. Must be a non-negative integer.</param>
        /// <returns>The channel value if it is less than or equal to 16; otherwise, 16.</returns>
        private static int ClampChannel(int channel)
        {
            return Math.Min(channel, 16);
        }
    }
}
