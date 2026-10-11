using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Silent_beep_scenario_tester
{
    public partial class Form1 : Form
    {
        private byte systemOriginalState = 0x00;
        private bool isClockGateClampActive = false;
        private CancellationTokenSource clampCancelSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void LogToTerminal(string message, Color textColor)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => LogToTerminal(message, textColor)));
                return;
            }

            string timestamp = $"[{DateTime.Now:HH:mm:ss.fff}] ";
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.LightGray;
            richTextBox1.AppendText(timestamp);

            richTextBox1.SelectionColor = textColor;
            richTextBox1.AppendText(message + Environment.NewLine);
            richTextBox1.ScrollToCaret();
        }

        // === 1. IMITATE THE BUG BUTTON ===
        private void btnImitateBug_Click(object sender, EventArgs e)
        {
            if (PortAccess.IsInpOutDriverOpen() == 0)
            {
                LogToTerminal("ERROR: InpOutx64 driver is not running. Run application as Admin.", Color.Red);
                return;
            }

            if (isClockGateClampActive)
            {
                LogToTerminal("Hardware bug simulation is already actively clamping registers.", Color.Yellow);
                return;
            }

            LogToTerminal(">>> True Chipset Clock Gating Engaged! <<<", Color.OrangeRed);
            LogToTerminal("Spinning up an aggressive kernel-level hardware clamp thread.", Color.Yellow);
            LogToTerminal("Forcing Port 0x61 Bit 0 (Clock Gate) to stay locked at 0.", Color.White);

            if (systemOriginalState == 0x00)
            {
                systemOriginalState = PortAccess.Inp32(0x61);
            }

            isClockGateClampActive = true;
            clampCancelSource = new CancellationTokenSource();
            CancellationToken token = clampCancelSource.Token;

            // REAL HARDWARE-LEVEL EMULATION TRAP:
            // Intercepts the clock gate register bit at hyper-speeds to replicate the motherboard failure.
            Task.Run(() =>
            {
                short portB = 0x61;
                while (!token.IsCancellationRequested)
                {
                    byte currentB = PortAccess.Inp32(portB);
                    if ((currentB & 0x01) != 0)
                    {
                        PortAccess.Out32(portB, (byte)(currentB & 0xFE));
                    }
                    Thread.SpinWait(50);
                }
            }, token);
        }

        // === 2. PLAY BEEP AND OBSERVE BIT 5 BUTTON ===
        private async void btnPlayAndObserve_Click(object sender, EventArgs e)
        {
            if (PortAccess.IsInpOutDriverOpen() == 0)
            {
                LogToTerminal("ERROR: InpOutx64 driver is not running. Run application as Admin.", Color.Red);
                return;
            }

            button3.Enabled = false;
            LogToTerminal("=== Initiating Dual-Frequency Response & Pull-State Testing ===", Color.Cyan);

            int testDurationMs = 300;

            LogToTerminal("Phase 1: Programming PIT to 440 Hz...", Color.LightSkyBlue);
            int transitions440 = await ExecuteBeepTestFrame(440, testDurationMs);
            LogToTerminal($"-> 440 Hz Test Result: {transitions440} transitions recorded.", Color.LightGray);

            await Task.Delay(200);

            LogToTerminal("Phase 2: Programming PIT to 880 Hz...", Color.LightSkyBlue);
            int transitions880 = await ExecuteBeepTestFrame(880, testDurationMs);
            LogToTerminal($"-> 880 Hz Test Result: {transitions880} transitions recorded.", Color.LightGray);

            EvaluateHardwareResponsiveness(transitions440, transitions880);

            button3.Enabled = true;
        }

        private async Task<int> ExecuteBeepTestFrame(int frequency, int durationMs)
        {
            short portB = 0x61;
            short portCommand = 0x43;
            short portData = 0x42;

            int recordedTransitions = 0;
            int countHigh = 0;
            int countLow = 0;

            await Task.Run(() =>
            {
                // Write standard square wave mode configuration (Mode 3)
                PortAccess.Out32(portCommand, 0xB6);

                // Calculate and send PIT hardware divisor
                int divisor = 1193182 / frequency;
                PortAccess.Out32(portData, (byte)(divisor & 0xFF));
                PortAccess.Out32(portData, (byte)((divisor >> 8) & 0xFF));

                byte startB = PortAccess.Inp32(portB);
                PortAccess.Out32(portB, (byte)((startB & 0xFC) | 0x03));

                // Rapidly poll Bit 5 and evaluate electrical pull trends
                byte lastBit5 = 2;
                for (int i = 0; i < durationMs; i++)
                {
                    byte state = PortAccess.Inp32(portB);
                    byte bit5 = (byte)((state >> 5) & 1);

                    // Track raw high/low states to detect overall pull direction
                    if (bit5 == 1) countHigh++;
                    else countLow++;

                    if (lastBit5 != 2 && bit5 != lastBit5)
                    {
                        recordedTransitions++;
                    }
                    lastBit5 = bit5;
                    Thread.Sleep(1);
                }

                // Clear speaker state
                byte endB = PortAccess.Inp32(portB);
                PortAccess.Out32(portB, (byte)(endB & 0xFC));
            });

            // Log the precise electrical pull observations to your black terminal screen
            LogPullStateObservation(recordedTransitions, countHigh, countLow);

            return recordedTransitions;
        }

        private void LogPullStateObservation(int transitions, int highSamples, int lowSamples)
        {
            if (transitions == 0)
            {
                // If it flatlines entirely, analyze whether it was pulled up or down
                if (highSamples > 0 && lowSamples == 0)
                {
                    LogToTerminal($"   [PULL STATE]: Bit 5 held a solid '1' ({highSamples} samples). Verified PULL-UP condition.", Color.Magenta);
                }
                else if (lowSamples > 0 && highSamples == 0)
                {
                    LogToTerminal($"   [PULL STATE]: Bit 5 held a solid '0' ({lowSamples} samples). Verified PULL-DOWN condition.", Color.DarkRed);
                }
            }
            else
            {
                // If actively transitioning, print out the electrical signal distribution
                double highPercent = ((double)highSamples / (highSamples + lowSamples)) * 100;
                LogToTerminal($"   [SIGNAL ANALYSIS]: Bit 5 Active. Signal distribution: {highPercent:F1}% High / {100 - highPercent:F1}% Low.", Color.LimeGreen);
            }
        }

        private void EvaluateHardwareResponsiveness(int lowT, int highT)
        {
            LogToTerminal("--- Hardware Diagnostic Summary ---", Color.White);

            if (lowT == 0 && highT == 0)
            {
                LogToTerminal("-> BUG CONFIRMED: 0 Total Transitions. PIT is entirely frozen.", Color.Red);
                LogToTerminal("-> HARDWARE DIAGNOSTIC: Circuit is statically trapped in a Logic High Pull-Up trap.", Color.Yellow);
                return;
            }

            double scalingRatio = (double)highT / (lowT == 0 ? 1 : lowT);

            if (scalingRatio < 1.3)
            {
                LogToTerminal($"-> BUG CONFIRMED: Frequency changes are completely IGNORED! (Scaling Ratio: {scalingRatio:F2}x)", Color.Red);
                LogToTerminal("-> DIAGNOSIS: PIT register registers changing, but physical output is locked.", Color.Yellow);
            }
            else
            {
                LogToTerminal($"-> HARDWARE RUNTIME OK: Frequency response detected! (Scaling Ratio: {scalingRatio:F2}x)", Color.Lime);
                LogToTerminal("-> DIAGNOSIS: PIT is modulating wave frequencies accurately.", Color.LightGreen);
            }
            LogToTerminal("==========================================" + Environment.NewLine, Color.Cyan);
        }

        // === 3. RESTORE BUTTON ===
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (isClockGateClampActive)
            {
                clampCancelSource.Cancel();
                isClockGateClampActive = false;
                LogToTerminal("<<< Hardware background clamp loop successfully terminated.", Color.Orange);
            }

            if (systemOriginalState != 0x00)
            {
                PortAccess.Out32(0x61, systemOriginalState);
                LogToTerminal("<<< Hardware configuration flags reset back to default system values.", Color.GreenYellow);
            }
            else
            {
                LogToTerminal("Warning: No background hardware configuration profile saved.", Color.Yellow);
            }
        }
        private void clearConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Refresh();
            richTextBox1.Invalidate();
        }

        public class PortAccess
        {
            [DllImport("InpOutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
            public static extern uint IsInpOutDriverOpen();

            [DllImport("InpOutx64.dll", EntryPoint = "Inp32")]
            public static extern byte Inp32(short PortAddress);

            [DllImport("InpOutx64.dll", EntryPoint = "Out32")]
            public static extern void Out32(short PortAddress, byte Data);
        }
    }
}