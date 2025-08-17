using System;
using System.IO;
using System.Linq;

namespace NeoBleeper
{
    public class MIDIFileValidator
    {
        public static bool IsMidiFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // Step 1: Check for "MThd" header
                    byte[] header = br.ReadBytes(4);
                    if (header.Length < 4 || header[0] != 'M' || header[1] != 'T' || header[2] != 'h' || header[3] != 'd')
                        return false;

                    // Step 2: Header length (should be 6 bytes)
                    int headerLength = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                    if (headerLength != 6)
                        return false;

                    // Step 3: Format type (0, 1, or 2)
                    ushort formatType = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
                    if (formatType > 2)
                        return false;

                    // Step 4: Number of tracks (should be at least 1)
                    ushort trackCount = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
                    if (trackCount < 1)
                        return false;

                    // Step 5: Ticks per quarter note (should be positive)
                    ushort ticksPerQuarter = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
                    if (ticksPerQuarter == 0)
                        return false;

                    // Step 6: Validate each track
                    for (int i = 0; i < trackCount; i++)
                    {
                        // Check for "MTrk" track header
                        byte[] trackHeader = br.ReadBytes(4);
                        if (trackHeader.Length < 4 || trackHeader[0] != 'M' || trackHeader[1] != 'T' || trackHeader[2] != 'r' || trackHeader[3] != 'k')
                            return false;

                        // Track length
                        int trackLength = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                        if (trackLength <= 0)
                            return false;

                        // Validate track data
                        long trackStartPosition = br.BaseStream.Position;
                        long trackEndPosition = trackStartPosition + trackLength;
                        byte lastCommand = 0;
                        byte lastChannel = 0;

                        while (br.BaseStream.Position < trackEndPosition)
                        {
                            // Read delta time (variable-length quantity)
                            if (!ReadVariableLengthQuantity(br, out _))
                                return false;

                            // Read event type
                            byte eventType = br.ReadByte();
                            if ((eventType & 0xF0) == 0xF0) // Meta or system event
                            {
                                if (eventType == 0xFF) // Meta event
                                {
                                    byte metaType = br.ReadByte();
                                    if (!ReadVariableLengthQuantity(br, out int metaLength))
                                        return false;

                                    br.BaseStream.Seek(metaLength, SeekOrigin.Current); // Skip meta event data
                                }
                                else if (eventType == 0xF0 || eventType == 0xF7) // SysEx event
                                {
                                    if (!ReadVariableLengthQuantity(br, out int sysExLength))
                                        return false;

                                    br.BaseStream.Seek(sysExLength, SeekOrigin.Current); // Skip SysEx data
                                }
                                else
                                {
                                    return false; // Unknown system event
                                }
                            }
                            else // MIDI event or running status
                            {
                                byte command;
                                int channel;

                                if ((eventType & 0x80) == 0) // Running status
                                {
                                    // Use the previous command and the current byte as the data
                                    if (lastCommand == 0)
                                    {
                                        return false; // No previous command to use
                                    }
                                    command = lastCommand;
                                    channel = lastChannel;

                                    //Rewind one byte, so the switch statement can consume the eventType
                                    br.BaseStream.Seek(-1, SeekOrigin.Current);
                                }
                                else // New MIDI event
                                {
                                    command = (byte)(eventType & 0xF0);
                                    channel = eventType & 0x0F;

                                    // Remember the last command and channel
                                    lastCommand = command;
                                    lastChannel = (byte)channel;
                                }

                                // Validate MIDI event data
                                switch (command)
                                {
                                    case 0x80: // Note Off
                                    case 0x90: // Note On
                                    case 0xA0: // Polyphonic Key Pressure
                                    case 0xB0: // Control Change
                                    case 0xE0: // Pitch Bend
                                        br.BaseStream.Seek(2, SeekOrigin.Current); // Skip 2 data bytes
                                        break;
                                    case 0xC0: // Program Change
                                    case 0xD0: // Channel Pressure
                                        br.BaseStream.Seek(1, SeekOrigin.Current); // Skip 1 data byte
                                        break;
                                    default:
                                        return false; // Unknown MIDI event
                                }
                            }
                        }

                        // Ensure track ends at the correct position
                        if (br.BaseStream.Position != trackEndPosition)
                            return false;

                        // Ensure track ends with "End of Track" meta event
                        br.BaseStream.Seek(-3, SeekOrigin.Current); // Go back to the last 3 bytes
                        if (br.ReadByte() != 0xFF || br.ReadByte() != 0x2F || br.ReadByte() != 0x00)
                            return false;
                    }

                    // All checks passed
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool ReadVariableLengthQuantity(BinaryReader br, out int value)
        {
            value = 0;
            for (int i = 0; i < 4; i++) // Max 4 bytes for VLQ
            {
                byte b = br.ReadByte();
                value = (value << 7) | (b & 0x7F);
                if ((b & 0x80) == 0) // Last byte of VLQ
                    return true;
            }
            return false; // VLQ too long
        }
    }
}