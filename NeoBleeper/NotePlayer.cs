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
        public static async void play_note(int frequency, int length, bool nonStopping = false) // Create a beep with the specified frequency and length
        {
            if (TemporarySettings.MicrocontrollerSettings.useMicrocontroller) // If the microcontroller is enabled
            {
                CreateSoundWithMicroController(frequency, length, nonStopping); // Create a sound with the microcontroller
            }
            switch (TemporarySettings.creating_sounds.is_playback_muted) // Mute the system speaker
            {
                case false: // If the system speaker is not muted, create a beep with the system speaker
                    {
                        switch (TemporarySettings.creating_sounds.create_beep_with_soundcard) // Create a beep with the soundcard or the system speaker
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
                                    switch (TemporarySettings.creating_sounds.soundDeviceBeepWaveform)
                                    {
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Square: // Square wave
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.SquareWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Sine: // Sine wave
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.SineWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Triangle: // Triangle wave
                                            {
                                                SoundRenderingEngine.WaveSynthEngine.TriangleWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Noise: // Noise
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
        public static void StopAllNotes() // Stop all notes
        {
            switch (TemporarySettings.creating_sounds.create_beep_with_soundcard) // Create a beep with the soundcard or the system speaker
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
        public static void PlayOnlySystemSpeakerBeep(int frequency, int length) // Play only the system speaker beep
        {
            SoundRenderingEngine.SystemSpeakerBeepEngine.Beep(frequency, length, false); // Create a beep with the system speaker (aka PC speaker)
        }
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
