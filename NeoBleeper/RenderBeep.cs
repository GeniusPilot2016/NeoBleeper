using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NeoBleeper
{
    internal class RenderBeep
    {
        public static class BeepClass
        {
            [DllImport("inpoutx64.dll")]
            extern static void Out32(short PortAddress, short Data);
            [DllImport("inpoutx64.dll")]
            extern static char Inp32(short PortAddress);
            public static void Beep(int freq, int ms)
            {
                Application.DoEvents();
                Out32(0x43, 0xB6);
                int div = 0x1234dc / freq;
                Out32(0x42, (Byte)(div & 0xFF));
                Out32(0x42, (Byte)(div >> 8));
                System.Threading.Thread.Sleep(10);
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) | 0x03));
                System.Threading.Thread.Sleep(ms);
                StopBeep();
            }
            public static void StopBeep()
            {
                Out32(0x61, (Byte)(System.Convert.ToByte(Inp32(0x61)) & 0xFC));
            }
        }
    }
}
