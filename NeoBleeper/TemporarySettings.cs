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
    public class TemporarySettings
    {
        public static class EligibilityOfCreateBeepFromSystemSpeaker
        {
            public static bool isSystemSpeakerPresent;
            public static bool isChipsetAffectedFromSystemSpeakerIssues;
            public static DeviceType deviceType = DeviceType.Unknown;

            /// <summary>
            /// Specifies the type of device used in the system.
            /// </summary>
            /// <remarks>Use this enumeration to distinguish between different categories of devices,
            /// such as modular or compact computers. The value may be Unknown if the device type cannot be
            /// determined.</remarks>
            public enum DeviceType
            {
                Unknown,
                ModularComputers,
                CompactComputers
            }
        }

        public static class CreatingSounds
        {
            public static bool createBeepWithSoundDevice;
            public static SoundDeviceBeepWaveform soundDeviceBeepWaveform = SoundDeviceBeepWaveform.Square;

            /// <summary>
            /// Specifies the waveform type used by a sound device when generating a beep.
            /// </summary>
            /// <remarks>Use this enumeration to select the desired waveform for beep sounds produced
            /// by compatible sound devices. The available waveforms may affect the tonal quality and character of the
            /// beep.</remarks>
            public enum SoundDeviceBeepWaveform
            {
                Square,
                Sine,
                Triangle,
                Noise
            }
            public static bool isPlaybackMuted = false;
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

            /// <summary>
            /// Specifies the type of beat pattern to use when determining playback behavior.
            /// </summary>
            /// <remarks>Use this enumeration to select how beats are interpreted or triggered in
            /// playback scenarios. The available values allow for playback on all beats, only odd or even beats, or on
            /// user-checked lines, depending on the application's requirements.</remarks>
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

            /// <summary>
            /// Specifies the behavior of portamento when transitioning between notes.
            /// </summary>
            /// <remarks>Portamento is a musical effect that creates a smooth glide between pitches.
            /// The selected value determines whether sound is produced throughout the entire portamento or only for a
            /// specified portion of its duration.</remarks>
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
            public static int formant1Volume = 92;
            public static int formant2Volume = 100;
            public static int formant3Volume = 90;
            public static int formant4Volume = 60;

            // Formant frequencies (in Hz)
            public static int formant1Frequency = 3355;
            public static int formant2Frequency = 2558;
            public static int formant3Frequency = 1675;
            public static int formant4Frequency = 1180;

            // Voice Volume (1-400)
            public static int voiceVolume = 300;

            // Saw volume (1-1000)
            public static int sawVolume = 1000;

            // Noise volume (1-1000)
            public static int noiseVolume = 100;

            // Cutoff frequency (in Hz)
            public static int cutoffFrequency = 5000;

            // Sybillance ranges (0.2-1.5)
            public static double sybillance1Range = 0.2;
            public static double sybillance2Range = 0.2;
            public static double sybillance3Range = 0.2;
            public static double sybillance4Range = 0.2;

            // Sybillance volumes (0x-1x)
            public static double sybillance1Volume = 1;
            public static double sybillance2Volume = 1;
            public static double sybillance3Volume = 1;
            public static double sybillance4Volume = 1;

            // Sybillance frequencies (in Hz)
            public static int sybillance1Frequency = 200;
            public static int sybillance2Frequency = 200;
            public static int sybillance3Frequency = 200;
            public static int sybillance4Frequency = 200;

            // Pitch (between 0.5 and 2.0)
            public static double timbre = 0.9875;
            // Range (between 0 and 1.0)
            public static double randomizedFrequencyRange = 0.105;

            // Output device indexes
            // 0 = Voice system
            // 1 = System speaker/Sound device beep
            public static int note1OutputDeviceIndex = 1; // Default to system speaker/sound device beep
            public static int note2OutputDeviceIndex = 0; // Default to voice system
            public static int note3OutputDeviceIndex = 1; // Default to system speaker/sound device beep
            public static int note4OutputDeviceIndex = 1; // Default to system speaker/sound device beep
            public static PlayingVoiceOnLineOptions playingVoiceOnLineOptions = PlayingVoiceOnLineOptions.PlayVoiceOnAllLines;

            /// <summary>
            /// Specifies the options for playing a voice message on one or more lines.
            /// </summary>
            /// <remarks>Use this enumeration to control whether a voice message is played on all
            /// available lines or only on selected (checked) lines. The choice affects which lines will receive the
            /// playback when initiating a voice operation.</remarks>
            public enum PlayingVoiceOnLineOptions
            {
                PlayVoiceOnAllLines,
                PlayVoiceOnCheckedLines
            }
        }
    }
}
