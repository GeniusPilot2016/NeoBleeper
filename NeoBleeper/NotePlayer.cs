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
    public static class NotePlayer
    {

        /// <summary>
        /// Plays a single note with the specified frequency and duration using the available sound output device.
        /// </summary>
        /// <remarks>The output device and waveform used to play the note depend on the current
        /// application settings. If sound playback is muted or the frequency is out of range for the system speaker,
        /// the method will wait for the specified duration without producing sound. This method is asynchronous and
        /// returns immediately if <paramref name="nonStopping"/> is <see langword="true"/>.</remarks>
        /// <param name="frequency">The frequency of the note to play, in hertz. Must be between 37 and 32,767 when using the system speaker.</param>
        /// <param name="length">The duration of the note, in milliseconds. Must be a non-negative value.</param>
        /// <param name="nonStopping">If set to <see langword="true"/>, the note is played asynchronously and does not block the calling thread;
        /// otherwise, the method blocks until the note has finished playing.</param>
        public static async void PlayNote(int frequency, int length, bool nonStopping = false) // Create a beep with the specified frequency and length
        {
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller) // If the microcontroller is enabled
            {
                CreateSoundWithMicroController(frequency, length, nonStopping); // Create a sound with the microcontroller
            }
            switch (TemporarySettings.CreatingSounds.isPlaybackMuted) // Mute the system speaker
            {
                case false: // If the system speaker is not muted, create a beep with the system speaker
                    {
                        switch (TemporarySettings.CreatingSounds.createBeepWithSoundDevice) // Create a beep with the soundcard or the system speaker
                        {
                            case false: // System speaker
                                {
                                    if (frequency >= 37 && frequency <= 32767) // If the frequency is in range, create a beep with the system speaker
                                    {
                                        SoundRenderingEngine.SystemSpeakerBeepEngine.Beep(frequency, length, nonStopping); // Create a beep with the system speaker (aka PC speaker)
                                    }
                                    else // If the frequency is out of range, sleep for the length of the note
                                    {
                                        HighPrecisionSleep.Sleep(length); // Sleep for the length of the note
                                    }
                                    break;
                                }
                            case true: // Soundcard
                                {
                                    switch (TemporarySettings.CreatingSounds.soundDeviceBeepWaveform)
                                    {
                                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Square: // Square wave
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.SquareWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Sine: // Sine wave
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.SineWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Triangle: // Triangle wave
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.TriangleWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.CreatingSounds.SoundDeviceBeepWaveform.Noise: // Noise
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.Noise(frequency, length, nonStopping);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case true: // If the system speaker is muted, sleep for the length of the note
                    {
                        HighPrecisionSleep.Sleep(length); // Sleep for the length of the note
                        break;
                    }
            }
        }

        /// <summary>
        /// Plays a musical note at the specified frequency and duration without introducing a gap between consecutive
        /// notes.
        /// </summary>
        /// <remarks>This method is intended for scenarios where seamless, gapless playback of notes is
        /// required, such as in musical sequences. Calling this method will stop any currently playing notes to prevent
        /// overlap.</remarks>
        /// <param name="frequency">The frequency of the note to play, in hertz. Must be a positive integer representing the pitch of the note.</param>
        /// <param name="length">The duration of the note to play, in milliseconds. Must be a positive integer.</param>
        public static async void PlayNoteWithoutGap(int frequency, int length)
        {
            PlayNote(frequency, length, true); // Play the note in non-stopping mode to allow for gapless playback
            StopAllNotes(); // Stop all currently playing notes to prevent overlap and ensure gapless playback
        }

        /// <summary>
        /// Stops all currently playing notes, regardless of the output device.
        /// </summary>
        /// <remarks>This method halts sound output from both the system speaker and the sound device,
        /// depending on the current configuration. It is typically used to ensure that no notes continue to play after
        /// a stop or reset operation.</remarks>
        public static void StopAllNotes() // Stop all notes
        {
            switch (TemporarySettings.CreatingSounds.createBeepWithSoundDevice) // Create a beep with the soundcard or the system speaker
            {
                case false: // System speaker
                    {
                        SoundRenderingEngine.SystemSpeakerBeepEngine.StopBeep(); // Stop the beep from the system speaker (aka PC speaker)
                        break;
                    }
                case true: // Soundcard
                    {
                        SoundRenderingEngine.WaveSynthEngine.StopSynth();
                        break;
                    }
            }
        }

        /// <summary>
        /// Plays a beep sound using only the system speaker at the specified frequency and duration.
        /// </summary>
        /// <remarks>This method produces sound exclusively through the system (PC) speaker, regardless of
        /// other audio devices. The actual frequency and duration may be limited by hardware capabilities on some
        /// systems.</remarks>
        /// <param name="frequency">The frequency of the beep, in hertz. Must be a positive integer representing the pitch of the sound.</param>
        /// <param name="length">The duration of the beep, in milliseconds. Must be a positive integer specifying how long the beep will
        /// play.</param>
        public static void PlayOnlySystemSpeakerBeep(int frequency, int length) // Play only the system speaker beep
        {
            SoundRenderingEngine.SystemSpeakerBeepEngine.Beep(frequency, length, false); // Create a beep with the system speaker (aka PC speaker)
        }

        /// <summary>
        /// Stops any sound currently being produced by the connected microcontroller, such as a buzzer or motor sound.
        /// </summary>
        /// <remarks>This method determines the type of microcontroller device and sends the appropriate
        /// command to stop sound output. If an error occurs while communicating with the microcontroller, the error is
        /// logged and no exception is thrown to the caller. This method is asynchronous but returns void; callers
        /// cannot await its completion or catch exceptions directly.</remarks>
        public static async void StopMicrocontrollerSound() // Stop the sound from the microcontroller
        {
            try
            {
                switch (TemporarySettings.MicrocontrollerSettings.deviceType)
                {
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.DCMotorOrBuzzer:
                        await SerialPortHelper.StopBuzzerSound();
                        break;
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.StepperMotor:
                        await SerialPortHelper.StopMotorSound();
                        break;
                }
            }
            catch (Exception ex) // If an error occurs, log it
            {
                Logger.Log($"Error stopping microcontroller sound: {ex.Message}", Logger.LogTypes.Error); // Log the error message
            }
        }

        /// <summary>
        /// Sends a command to the connected microcontroller to play a sound at the specified frequency and duration.
        /// </summary>
        /// <remarks>The behavior of this method depends on the type of microcontroller device configured
        /// in the application settings. For devices configured as a DC motor or buzzer, the sound is played using the
        /// buzzer. For stepper motor devices, the sound is played using the motor. If an error occurs while
        /// communicating with the microcontroller, the error is logged and the exception is not propagated.</remarks>
        /// <param name="frequency">The frequency of the sound to play, in hertz. Must be a positive integer supported by the microcontroller.</param>
        /// <param name="length">The duration of the sound, in milliseconds. Must be a positive integer.</param>
        /// <param name="nonStopping">true to play the sound in non-stopping mode (allowing overlap or continuous play, depending on device
        /// capabilities); otherwise, false.</param>
        /// <returns>A task that represents the asynchronous operation of sending the sound command to the microcontroller.</returns>
        private static async Task CreateSoundWithMicroController(int frequency, int length, bool nonStopping = false) // Create a sound with the microcontroller
        {
            try
            {
                switch (TemporarySettings.MicrocontrollerSettings.deviceType)
                {
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.DCMotorOrBuzzer:
                        await SerialPortHelper.PlaySoundUsingBuzzer(frequency, length, nonStopping);
                        break;
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.StepperMotor:
                        int motorOctave = TemporarySettings.MicrocontrollerSettings.stepperMotorOctave; // Get the motor octave setting
                        await SerialPortHelper.PlaySoundUsingMotor(frequency, length, nonStopping);
                        break;
                }
            }
            catch (Exception ex) // If an error occurs, log it
            {
                Logger.Log($"Error creating sound with microcontroller: {ex.Message}", Logger.LogTypes.Error); // Log the error message
            }
        }
    }
}
