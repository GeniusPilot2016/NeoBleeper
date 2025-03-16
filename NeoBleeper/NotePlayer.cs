using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public static class NotePlayer
    {
        public static void play_note(int frequency, int length) // Create a beep with the specified frequency and length
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
                                        RenderBeep.BeepClass.Beep(frequency, length);
                                    }
                                    else // If the frequency is out of range, sleep for the length of the note
                                    {
                                        Thread.Sleep(length);
                                    }
                                    break;
                                }
                            case true: // If the system speaker is muted, sleep for the length of the note
                                {
                                    Thread.Sleep(length);
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
                                    RenderBeep.SynthMisc.SquareWave(frequency, length);
                                    break;
                                }
                            case 1: // Sine wave
                                {
                                    RenderBeep.SynthMisc.SineWave(frequency, length);
                                    break;
                                }
                            case 2: // Triangle wave
                                {
                                    RenderBeep.SynthMisc.TriangleWave(frequency, length);
                                    break;
                                }
                            case 3: // Noise
                                {
                                    RenderBeep.SynthMisc.Noise(frequency, length);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
