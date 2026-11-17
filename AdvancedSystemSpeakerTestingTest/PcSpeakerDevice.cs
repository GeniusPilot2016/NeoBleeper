using System;
using NeoBleeper;

namespace AdvancedSystemSpeakerProbe;

/// <summary>
/// C# command encoder for the exact PcSpeaker.p ioctl_batch protocol.
/// Only PIT counter 2 and writable bits 0-1 of PPI port 0x61 are used.
/// </summary>
internal sealed class PcSpeakerDevice
{
    private readonly PawnIOWrapper _pawnio;
    private const string EntryPoint = "ioctl_batch";

    public const byte SquareWaveMode = 0xB6;
    public const byte CountLatch = 0x80;
    public const byte CountAndStatusReadback = 0xC8;

    private const int MaxCommands = 64;
    private const int SpeakerMask = 0x03;

    private enum Op : byte
    {
        Nop = 0,
        PitMode = 1,
        PitCount = 2,
        PitWrite = 3,
        PitRead = 4,
        PpiWrite = 5,
        PpiRead = 6,
        Stall = 7
    }

    private static long Make(Op op, ulong payload = 0) =>
        unchecked((long)(((ulong)op << 56) | (payload & 0x00FFFFFFFFFFFFFFUL)));

    private static long Mode(byte mode) => Make(Op.PitMode, mode);
    private static long Count(int divisor) => Make(Op.PitCount, (uint)divisor & 0xFFFFUL);
    private static long PitRead() => Make(Op.PitRead);
    private static long PpiRead() => Make(Op.PpiRead);
    private static long PpiWrite(byte value, byte mask = SpeakerMask) =>
        Make(Op.PpiWrite, (ulong)((mask & SpeakerMask) << 8 | (value & SpeakerMask)));
    private static long Stall(int microseconds)
    {
        if (microseconds < 0 || microseconds > 50)
            throw new ArgumentOutOfRangeException(nameof(microseconds), "PcSpeaker.p permits at most 50us per stall command.");
        return Make(Op.Stall, (uint)microseconds);
    }

    public PcSpeakerDevice(PawnIOWrapper pawnio) => _pawnio = pawnio;

    private long[] Run(bool readResults, params long[] commands)
    {
        if (commands.Length is < 1 or > MaxCommands)
            throw new ArgumentOutOfRangeException(nameof(commands), "A batch must have 1..64 commands.");
        var result = _pawnio.Execute(EntryPoint, commands, readResults ? commands.Length : 0);
        if (readResults && result.Length != commands.Length)
            throw new InvalidOperationException($"Unexpected PcSpeaker return count ({result.Length}/{commands.Length}).");
        return result;
    }

    public byte ReadPort61() => (byte)(Run(true, PpiRead())[0] & 0xFF);

    public byte WriteControl(byte value, byte mask = SpeakerMask) =>
        (byte)(Run(true, PpiWrite(value, mask))[0] & 0xFF);

    public void RestoreControl(byte originalPort61) => WriteControl((byte)(originalPort61 & SpeakerMask));

    public void ProgramPit(int divisor)
    {
        if (divisor < 2 || divisor > 65535)
            throw new ArgumentOutOfRangeException(nameof(divisor));
        Run(false, Mode(SquareWaveMode), Count(divisor));
    }

    public int LatchCount()
    {
        // The three commands execute in ONE kernel call to avoid interleaved PIT reads.
        var result = Run(true, Mode(CountLatch), PitRead(), PitRead());
        return (int)((result[1] & 0xFF) | ((result[2] & 0xFF) << 8));
    }

    public (byte Status, int Count) ReadbackStatusAndCount()
    {
        var result = Run(true, Mode(CountAndStatusReadback), PitRead(), PitRead(), PitRead());
        return ((byte)(result[1] & 0xFF),
            (int)((result[2] & 0xFF) | ((result[3] & 0xFF) << 8)));
    }

    public void Pulse100Microseconds(int divisor)
    {
        // PcSpeaker.p validates stalls <=50us each, <=1000us in a batch.
        // The speaker-enable interval is contained wholly within one kernel batch.
        // It is nominally 100us plus a few instructions of command overhead.
        Run(false,
            PpiWrite(0),
            Mode(SquareWaveMode),
            Count(divisor),
            PpiWrite(3),
            Stall(50),
            Stall(50),
            PpiWrite(0));
    }

    public (byte Off, byte On, byte After) VerifySpeakerDataControl()
    {
        // This verifies only that the PPI control bit reads back correctly.
        // It does NOT test electrical impedance, speaker wiring or sound output.
        // Speaker-data is briefly asserted inside one short kernel batch.
        var result = Run(true,
            PpiWrite(1), PpiRead(),
            PpiWrite(3), PpiRead(),
            PpiWrite(1), PpiRead());
        return ((byte)result[1], (byte)result[3], (byte)result[5]);
    }
}
