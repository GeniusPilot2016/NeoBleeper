using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    public class TemporarySettings
    {
        public static class eligibility_of_create_beep_from_system_speaker
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
            public static bool is_playback_muted = false;
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
                PlayOnEvenBeats,
                PlayOnCheckedLines
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
        public static class MicrocontrollerSettings
        {
            public static bool useMicrocontroller = false;
            public static DeviceType deviceType = DeviceType.StepperMotor;
            public enum DeviceType
            {
                StepperMotor,
                DCMotorOrBuzzer,
            }
            public static int stepperMotorOctave = 2; // Default octave for motor
        }
        public static class VoiceInternalSettings
        {
            // Formant volumes (60%-100%)
            public static int Formant1Volume = 92;
            public static int Formant2Volume = 100;
            public static int Formant3Volume = 90;
            public static int Formant4Volume = 60;

            // Formant frequencies (in Hz)
            public static int Formant1Frequency = 3355;
            public static int Formant2Frequency = 2558;
            public static int Formant3Frequency = 1675;
            public static int Formant4Frequency = 1180;

            // Voice Volume (1-400)
            public static int VoiceVolume = 300;

            // Saw volume (1-1000)
            public static int SawVolume = 1000;

            // Noise volume (1-1000)
            public static int NoiseVolume = 100;

            // Cutoff frequency (in Hz)
            public static int CutoffFrequency = 8000;

            // Sybillance ranges (0.2-1.5)
            public static double Sybillance1Range = 0.2;
            public static double Sybillance2Range = 0.2;
            public static double Sybillance3Range = 0.2;
            public static double Sybillance4Range = 0.2;

            // Sybillance volumes (0x-1x)
            public static double Sybillance1Volume = 1;
            public static double Sybillance2Volume = 1;
            public static double Sybillance3Volume = 1;
            public static double Sybillance4Volume = 1;

            // Sybillance frequencies (in Hz)
            public static int Sybillance1Frequency = 200;
            public static int Sybillance2Frequency = 200;
            public static int Sybillance3Frequency = 200;
            public static int Sybillance4Frequency = 200;

            // Pitch (between 0.5 and 2.0)
            public static double Pitch = 0.9875;
            // Range (between 0 and 1.0)
            public static double Range = 0.105;

            // Output device indexes
            // 0 = Voice system
            // 1 = System speaker/Sound device beep
            public static int Note1OutputDeviceIndex = 1; // Default to system speaker/sound device beep
            public static int Note2OutputDeviceIndex = 0; // Default to voice system
            public static int Note3OutputDeviceIndex = 1; // Default to system speaker/sound device beep
            public static int Note4OutputDeviceIndex = 1; // Default to system speaker/sound device beep
            public static PlayingVoiceOnLineOptions playingVoiceOnLineOptions = PlayingVoiceOnLineOptions.PlayVoiceOnAllLines;
            public enum PlayingVoiceOnLineOptions
            {
                PlayVoiceOnAllLines,
                PlayVoiceOnCheckedLines
            }
        }
    }
}
