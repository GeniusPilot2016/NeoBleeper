using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class TemporarySettings
    {
        public static class eligability_of_create_beep_from_system_speaker
        {
            public static bool is_system_speaker_present;
            public static DeviceType deviceType = DeviceType.Unknown;

            public enum DeviceType
            {
                Unknown,
                ModularComputers,
                CompactComputers
            }
        }

        public static class creating_sounds
        {
            public static bool create_beep_with_soundcard;
            public static SoundDeviceBeepWaveform soundDeviceBeepWaveform = SoundDeviceBeepWaveform.Square;

            public enum SoundDeviceBeepWaveform
            {
                Square,
                Sine,
                Triangle,
                Noise
            }
            public static bool permanently_enabled;
            public static bool is_system_speaker_muted = false;
        }

        public static class MIDIDevices
        {
            public static bool useMIDIinput = false;
            public static bool useMIDIoutput = false;
            public static int MIDIOutputDevice = 0;
            public static int MIDIOutputInstrument = 0; // Grand Piano    
            public static int MIDIOutputDeviceChannel = 0;
            public static int MIDIInputDevice = 0;
            public static event EventHandler MidiStatusChanged;

            public static void NotifyMidiStatusChanged()
            {
                MidiStatusChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static class BeatTypes
        {
            public static BeatType beatType = BeatType.PlayOnAllBeats;  
            public enum BeatType
            {
                PlayOnAllBeats,
                PlayOnOddBeats,
                PlayOnEvenBeats
            }
        }
        public static class PortamentoSettings
        {
            public static int length = 250;
            public static int pitchChangeSpeed = 12000;
            public static PortamentoType portamentoType = PortamentoType.ProduceSoundForLength;
            public enum PortamentoType
            {
                AlwaysProduceSound,
                ProduceSoundForLength,
            }
        }
    }
}
