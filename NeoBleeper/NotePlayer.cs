
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
                                        RenderBeep.BeepClass.Beep(frequency, length, nonStopping); // Create a beep with the system speaker (aka PC speaker)
                                    }
                                    else // If the frequency is out of range, sleep for the length of the note
                                    {
                                        NonBlockingSleep.Sleep(length); // Sleep for the length of the note
                                    }
                                    break;
                                }
                            case true: // Soundcard
                                {
                                    switch (TemporarySettings.creating_sounds.soundDeviceBeepWaveform)
                                    {
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Square: // Square wave
                                            {
                                                RenderBeep.SynthMisc.SquareWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Sine: // Sine wave
                                            {
                                                RenderBeep.SynthMisc.SineWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Triangle: // Triangle wave
                                            {
                                                RenderBeep.SynthMisc.TriangleWave(frequency, length, nonStopping);
                                                break;
                                            }
                                        case TemporarySettings.creating_sounds.SoundDeviceBeepWaveform.Noise: // Noise
                                            {
                                                RenderBeep.SynthMisc.Noise(frequency, length, nonStopping);
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
                        NonBlockingSleep.Sleep(length); // Sleep for the length of the note
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
                        RenderBeep.BeepClass.StopBeep(); // Stop the beep from the system speaker (aka PC speaker)
                        break;
                    }
                case true: // Soundcard
                    {
                        RenderBeep.SynthMisc.waveOut.Stop(); // Stop the beep from the sound device
                        break;
                    }
            }
        }
        public static void PlayOnlySystemSpeakerBeep(int frequency, int length) // Play only the system speaker beep
        {
            if(TemporarySettings.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                RenderBeep.BeepClass.Beep(frequency, length, false); // Create a beep with the system speaker (aka PC speaker)
            }
            else
            {
                return; // If the system speaker is not present, do nothing
            }
        }
        public static void StopMicrocontrollerSound() // Stop the sound from the microcontroller
        {
            try
            {
                switch (TemporarySettings.MicrocontrollerSettings.deviceType)
                {
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.Buzzer:
                        throw new NotImplementedException("Buzzer support is not implemented yet.");
                        break;
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.Motor:
                        throw new NotImplementedException("Motor support is not implemented yet.");
                        break;
                }
            }
            catch (Exception ex) // If an error occurs, log it
            {
                Console.WriteLine($"Error stopping microcontroller sound: {ex.Message}"); // Log the error message
            }
        }
        private static async Task CreateSoundWithMicroController(int frequency, int length, bool nonStopping = false) // Create a sound with the microcontroller
        {
            try
            {
                switch (TemporarySettings.MicrocontrollerSettings.deviceType)
                {
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.Buzzer:
                        throw new NotImplementedException("Buzzer support is not implemented yet.");
                        break;
                    case TemporarySettings.MicrocontrollerSettings.DeviceType.Motor:
                        int motorOctave = TemporarySettings.MicrocontrollerSettings.motorOctave; // Get the motor octave setting
                        throw new NotImplementedException("Motor support is not implemented yet.");
                        break;
                }
            }
            catch (Exception ex) // If an error occurs, log it
            {
                Console.WriteLine($"Error creating sound with microcontroller: {ex.Message}"); // Log the error message
            }
        }
    }
}
