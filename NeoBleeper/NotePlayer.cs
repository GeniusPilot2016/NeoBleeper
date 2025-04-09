
namespace NeoBleeper
{
    public static class NotePlayer
    {
        public static void play_note(int frequency, int length, bool nonStopping = false) // Create a beep with the specified frequency and length
        {
            switch (Program.creating_sounds.create_beep_with_soundcard) // Create a beep with the soundcard or the system speaker
            {
                case false: // System speaker
                    {
                        switch (Program.creating_sounds.is_system_speaker_muted) // Mute the system speaker
                        {
                            case false: // If the system speaker is not muted, create a beep with the system speaker
                                {
                                    if (frequency >= 37 && frequency <= 32767) // If the frequency is in range, create a beep with the system speaker
                                    {
                                        RenderBeep.BeepClass.Beep(frequency, length, nonStopping);
                                    }
                                    else // If the frequency is out of range, sleep for the length of the note
                                    {
                                        NonBlockingSleep.Sleep(length);
                                    }
                                    break;
                                }
                            case true: // If the system speaker is muted, sleep for the length of the note
                                {
                                    NonBlockingSleep.Sleep(length);
                                    break;
                                }
                        }
                        break;
                    }
                case true: // Soundcard
                    {
                        switch (Program.creating_sounds.soundcard_beep_waveform)
                        {
                            case 0: // Square wave
                                {
                                    RenderBeep.SynthMisc.SquareWave(frequency, length, nonStopping);
                                    break;
                                }
                            case 1: // Sine wave
                                {
                                    RenderBeep.SynthMisc.SineWave(frequency, length, nonStopping);
                                    break;
                                }
                            case 2: // Triangle wave
                                {
                                    RenderBeep.SynthMisc.TriangleWave(frequency, length, nonStopping);
                                    break;
                                }
                            case 3: // Noise
                                {
                                    RenderBeep.SynthMisc.Noise(frequency, length, nonStopping);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
        public static void StopAllNotes() // Stop all notes
        {
            switch (Program.creating_sounds.create_beep_with_soundcard) // Create a beep with the soundcard or the system speaker
            {
                case false: // System speaker
                    {
                        RenderBeep.BeepClass.StopBeep(); // Stop the beep from the system speaker
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
            if(Program.eligability_of_create_beep_from_system_speaker.is_system_speaker_present == true)
            {
                RenderBeep.BeepClass.Beep(frequency, length, false); // Create a beep with the system speaker
            }
            else
            {
                return; // If the system speaker is not present, do nothing
            }
        }
    }
}
