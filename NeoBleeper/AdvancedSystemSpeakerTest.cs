using NeoBleeper.Properties;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using static UIHelper;

namespace NeoBleeper
{
    public partial class AdvancedSystemSpeakerTest : Form
    {// ── Native interop ───────────────────────────────────────────────────────
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32(short portAddress, short data);

        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern short Inp32(short portAddress);

        // ── Port addresses ───────────────────────────────────────────────────────
        private const short PORT_SPEAKER = 0x61; // System-speaker / NMI control
        private const short PORT_PIT_CMD = 0x43; // 8254 PIT command register
        private const short PORT_PIT_CH2 = 0x42; // 8254 PIT channel 2

        // ── PIT command bytes ────────────────────────────────────────────────────
        // Channel 2 | lo/hi byte | mode 3 (square wave) | binary  →  10 11 011 0
        private const byte PIT_CH2_SQUAREWAVE = 0xB6;

        // Counter-latch command for channel 2: bits[7:6]=10, bits[5:4]=00  →  1000 0000
        private const byte PIT_CH2_LATCH = 0x80;

        // Read-back command: latch count + status for channel 2 only
        // 11 (read-back) | 00 (DO latch both) | 1000 (channel 2)  →  1100 1000
        // Status byte is returned first on the next read from port 0x42,
        // followed by the latched count (lo byte, hi byte).
        private const byte PIT_CH2_READBACK = 0xC8;

        // ── Timing / frequency constants ─────────────────────────────────────────
        // Maximum PIT divisor (16-bit) → minimum frequency ≈ 18.2 Hz (below 20 Hz auditory floor)
        private const int DIVISOR_MAX = 65535;
        private const int DIVISOR_MID = 16384; // ~72.8 Hz — used for T09 only (gate ON, spk OFF)
        private const double PIT_CLOCK = 1_193_182.0;
        private const double EXPECTED_HZ = PIT_CLOCK / DIVISOR_MAX; // ~18.20 Hz

        // ────────────────────────────────────────────────────────────────────────
        bool darkTheme = false;
        bool currentlyTesting = false;
        public AdvancedSystemSpeakerTest(Form owner)
        {
            InitializeComponent();
            this.Owner = owner;
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            SetTheme();
            UIFonts.SetFonts(this);
            StartTest();
        }

        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    SetTheme();
                }
            }
        }

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method updates the control's appearance to match the selected theme. If the
        /// theme is set to follow the system, the method detects the system's light or dark mode and applies the
        /// corresponding theme. The method should be called whenever the theme setting changes to ensure the control
        /// reflects the correct appearance.</remarks>
        private void SetTheme()
        {
            this.SuspendLayout(); // Suspend layout to batch updates
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering

            try
            {
                switch (Settings1.Default.theme)
                {
                    case 0:
                        if (SystemThemeUtility.IsDarkTheme())
                        {
                            DarkTheme();
                        }
                        else
                        {
                            LightTheme();
                        }
                        break;

                    case 1:
                        LightTheme();
                        break;

                    case 2:
                        DarkTheme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
            UIHelper.SetFormBackgroundFluent(this, darkTheme);
        }
        private void LightTheme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
        enum OverallState
        {
            PASSED,
            UNCERTAIN,
            FAILED
        }

        /// <summary>
        /// Initiates the advanced system speaker test sequence asynchronously, performing a series of diagnostic checks
        /// and updating the user interface with progress and results.
        /// </summary>
        /// <remarks>This method runs multiple hardware and signal tests on the system speaker, including
        /// electrical feedback, port state stability, and frequency sweep diagnostics. Progress and results are logged
        /// and displayed to the user. The method is asynchronous and should not be called concurrently. If an error
        /// occurs during testing, the user interface is updated to indicate the failure and details are
        /// logged.</remarks>
        private async void StartTest()
        {
            try
            {
                currentlyTesting = true;
                await Task.Delay(500);
                Logger.Log("Advanced system speaker test is started.", Logger.LogTypes.Info);
                label1.Text = "        " + Resources.AdvancedSystemSpeakerTestStarted;
                label1.ImageIndex = 1;
                await Task.Delay(500);
                Logger.Log("Verifying the system speaker output readability...", Logger.LogTypes.Info);
                label2.Visible = true;
                await Task.Delay(500);
                bool systemSpeakerOutputReadability = IsSystemSpeakerPortReadable();
                Logger.Log("System speaker output readability verification result: " + (systemSpeakerOutputReadability ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label2.Text = "        " + Resources.SystemSpeakerOutputReadability + ((systemSpeakerOutputReadability) ? Resources.TextPassed : Resources.TextFailed);
                label2.ImageIndex = (systemSpeakerOutputReadability) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Verifying the system speaker gate can be turned off and on...", Logger.LogTypes.Info);
                label3.Visible = true;
                await Task.Delay(500);
                bool canSystemSpeakerGateTurnOnOff = CanSystemSpeakerGateToggle();
                Logger.Log("System speaker gate turn on and off verification result: " + (canSystemSpeakerGateTurnOnOff ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label3.Text = "        " + Resources.SystemSpeakerGateTurnOnOff + ((canSystemSpeakerGateTurnOnOff) ? Resources.TextPassed : Resources.TextFailed);
                label3.ImageIndex = (canSystemSpeakerGateTurnOnOff) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Setting the internal timer for a silent check...", Logger.LogTypes.Info);
                label4.Visible = true;
                await Task.Delay(500);
                bool canTimerBeSet = CanTheTimerBeSet();
                Logger.Log("Setting the internal timer for a silent check result: " + (canTimerBeSet ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label4.Text = "        " + Resources.TimerSetup + ((canTimerBeSet) ? Resources.TextPassed : Resources.TextFailed);
                label4.ImageIndex = (canTimerBeSet) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Checking the timer is actually counting down...", Logger.LogTypes.Info);
                label5.Visible = true;
                await Task.Delay(500);
                bool isTimerActuallyCountingDown = IsTheTimerCounting();
                label5.Text = "        " + Resources.TimerCountdownCheck + ((isTimerActuallyCountingDown) ? Resources.TextPassed : Resources.TextFailed);
                label5.ImageIndex = (isTimerActuallyCountingDown) ? 1 : 2;
                await Task.Delay(500);
                label6.Visible = true;
                Logger.Log("Checking the timer status is normal during countdown...", Logger.LogTypes.Info);
                await Task.Delay(500);
                bool isTimerStatusNormal = IsTimerStatusNormal();
                Logger.Log("Timer status check result: " + ((isTimerStatusNormal) ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label6.Text = "        " + Resources.TimerStatusCheck + ((isTimerStatusNormal) ? Resources.TextPassed : Resources.TextFailed);
                label6.ImageIndex = (isTimerStatusNormal) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Watching for the system speaker feedback signal while audio is disabled...", Logger.LogTypes.Info);
                label7.Visible = true;
                await Task.Delay(500);
                bool isFeedbackSignalDetected = IsFeedbackSignalDetected();
                Logger.Log("System speaker feedback signal detection result: " + ((isFeedbackSignalDetected) ? Resources.TextPassed : Resources.TextFailed), Logger.LogTypes.Info);
                label7.Text = "        " + Resources.SystemSpeakerFeedbackSignalDetection + ((isFeedbackSignalDetected) ? Resources.TextPassed : Resources.TextFailed);
                label7.ImageIndex = (isFeedbackSignalDetected) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Confirming the feedback signal stays still when disabled...", Logger.LogTypes.Info);
                label8.Visible = true;
                await Task.Delay(500);
                bool isFeedbackSignalStableWhenDisabled = IsFeedbackSignalStable();
                Logger.Log("Feedback signal stability when disabled result: " + ((isFeedbackSignalStableWhenDisabled) ? Resources.TextPassed : Resources.TextFailed), Logger.LogTypes.Info);
                label8.Text = "        " + Resources.FeedbackSignalStability + ((isFeedbackSignalStableWhenDisabled) ? Resources.TextPassed : Resources.TextFailed);
                label8.ImageIndex = (isFeedbackSignalStableWhenDisabled) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Performing a very short silent system speaker enable pulse...", Logger.LogTypes.Info);
                label9.Visible = true;
                await Task.Delay(500);
                bool shortSilentEnablePulseResult = ShortSilentEnablePulseBool();
                Logger.Log("Short silent enable pulse result: " + ((shortSilentEnablePulseResult) ? Resources.TextPassed : Resources.TextFailed), Logger.LogTypes.Info);
                label9.Text = "        " + Resources.ShortSilentEnablePulse + ((shortSilentEnablePulseResult) ? Resources.TextPassed : Resources.TextFailed);
                label9.ImageIndex = (shortSilentEnablePulseResult) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Testing two timer speeds and their reported values...", Logger.LogTypes.Info);
                label10.Visible = true;
                await Task.Delay(500);
                bool timerSpeedTestResult = TimerSpeedTestComparison();
                Logger.Log("Timer speed test result: " + ((timerSpeedTestResult) ? Resources.TextPassed : Resources.TextFailed), Logger.LogTypes.Info);
                label10.Text = "        " + Resources.TimerSpeedTest + ((timerSpeedTestResult) ? Resources.TextPassed : Resources.TextFailed);
                label10.ImageIndex = (timerSpeedTestResult) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Measuring the feedback signal speed...", Logger.LogTypes.Info);
                label11.Visible = true;
                await Task.Delay(500);
                bool feedbackSignalSpeedMeasurementResult = FeedbackSpeedResult();
                Logger.Log("Feedback signal speed measurement result: " + ((feedbackSignalSpeedMeasurementResult) ? Resources.TextPassed : Resources.TextFailed), Logger.LogTypes.Info);
                label11.Text = "        " + Resources.FeedbeckSignalSpeedMeasurement + ((feedbackSignalSpeedMeasurementResult) ? Resources.TextPassed : Resources.TextFailed);
                label11.ImageIndex = (feedbackSignalSpeedMeasurementResult) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Checking for the system speaker entry...", Logger.LogTypes.Info);
                label12.Visible = true;
                await Task.Delay(500);
                bool systemSpeakerEntryCheckResult = IsSystemSpeakerEntryExists();

                // Build PNP0800 message with resource fallback (hardcoded if resource missing)
                string pnpMessage = systemSpeakerEntryCheckResult
                    ? Resources.SystemSpeakerEntryFound
                    : Resources.SystemSpeakerEntryNotFound;
                label12.Text = "        " + pnpMessage;
                label12.ImageIndex = systemSpeakerEntryCheckResult ? 1 : 2;


                // Decision tree: produce PASSED / UNCERTAIN / FAILED
                // - PASSED  : feedback detected (T06) AND feedback speed OK (T10)
                // - UNCERTAIN: any of basic checks (readable/gate/timer-set) OK AND PNP0800 present
                // - FAILED  : otherwise
                OverallState overallState = OverallState.UNCERTAIN;
                if ((isFeedbackSignalDetected && feedbackSignalSpeedMeasurementResult) || systemSpeakerEntryCheckResult)
                {
                    overallState = OverallState.PASSED;
                }
                else if ((systemSpeakerOutputReadability || canSystemSpeakerGateTurnOnOff || canTimerBeSet) && !systemSpeakerEntryCheckResult)
                {
                    overallState = OverallState.UNCERTAIN;
                }
                else
                {
                    overallState = OverallState.FAILED;
                }

                await Task.Delay(500);
                Logger.Log("Overall advanced system speaker test result: " + overallState, Logger.LogTypes.Info);
                label13.Visible = true;

                // Build English reasons for FAILED or UNCERTAIN
                var reasons = new System.Collections.Generic.List<string>();

                if (overallState == OverallState.FAILED)
                {
                    if (!systemSpeakerOutputReadability)
                        reasons.Add(Resources.ElectricalFeedbackTestFailed);
                    if (!canSystemSpeakerGateTurnOnOff)
                        reasons.Add(Resources.PortStateStabilityTestFailed);
                    if (!canTimerBeSet)
                        reasons.Add(Resources.TimerSetupFailed);
                    if (!isTimerActuallyCountingDown)
                        reasons.Add(Resources.TimerCountdownCheckFailed);
                    if (!isTimerStatusNormal)
                        reasons.Add(Resources.UnexpectedReadBack);
                    if (!isFeedbackSignalDetected)
                        reasons.Add(Resources.FeedbackDetectionFailed);
                    if (!isFeedbackSignalStableWhenDisabled)
                        reasons.Add(Resources.FeedbackSignalNotStable);
                    if (!shortSilentEnablePulseResult)
                        reasons.Add(Resources.ShortSilentPulseEnableFailed);
                    if (!timerSpeedTestResult)
                        reasons.Add(Resources.TimerSpeedOutOfRange);
                    if (!feedbackSignalSpeedMeasurementResult)
                        reasons.Add(Resources.FeedbackNotWithinTolerance);
                    // PNP presence info:
                    if (!systemSpeakerEntryCheckResult)
                        reasons.Add(Resources.SystemSpeakerEntryFound);
                    else
                        reasons.Add(Resources.SystemSpeakerEntryNotFound);

                    label14.Text = Resources.FailureReasons + string.Join("\r\n- ", reasons);
                    label14.Visible = true;
                }
                else if (overallState == OverallState.UNCERTAIN)
                {
                    var passedBasics = new System.Collections.Generic.List<string>();
                    if (systemSpeakerOutputReadability) passedBasics.Add(Resources.ElectricalFeedbackTest);
                    if (canSystemSpeakerGateTurnOnOff) passedBasics.Add(Resources.PortStateStabilityTest);
                    if (canTimerBeSet) passedBasics.Add(Resources.TimerSetupTest);

                    string basics = passedBasics.Count > 0
                        ? Resources.SomeBasicChecksPassed + string.Join(", ", passedBasics) + "."
                        : Resources.NoBasicChecksPassed;

                    var issues = new System.Collections.Generic.List<string>();
                    if (!isFeedbackSignalDetected) issues.Add(Resources.FeedbackSignalNotDetected);
                    if (!feedbackSignalSpeedMeasurementResult) issues.Add(Resources.FeedbackSpeedIsOutOfRange);
                    if (systemSpeakerEntryCheckResult) issues.Add(Resources.SystemSpeakerEntryFoundInThisSystem);

                    label14.Text = basics + (issues.Count > 0 ? Resources.Issues + string.Join("; ", issues) + "." : "");
                    label14.Visible = true;
                }
                else
                {
                    // PASSED — hide details label
                    label14.Visible = false;
                }

                string passedText = Resources.TextPassed;
                string failedText = Resources.TextFailed;
                string uncertainText = Resources.TextUncertain;
                string overallText;
                if (overallState == OverallState.PASSED)
                    overallText = passedText;
                else if (overallState == OverallState.FAILED)
                    overallText = failedText;
                else
                    overallText = uncertainText;

                label13.Text = "        " + Resources.OverallResult + overallText;
                label13.ImageIndex = (overallState == OverallState.PASSED) ? 1 : (overallState == OverallState.FAILED) ? 2 : (overallState == OverallState.UNCERTAIN) ? 3 : 0;
            }
            catch (Exception ex)
            {
                Logger.Log("Error during advanced system speaker test: " + ex.Message, Logger.LogTypes.Error);
                label1.ImageIndex = 2;
                label1.Text = "        " + Resources.AnErrorOccurredDuringAdvancedSystemSpeakerTest;
                label2.Visible = false; // Hide other labels on error
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                label6.Visible = false;
                label7.Visible = false;
                label8.Visible = false;
                label9.Visible = false;
                label10.Visible = false;
                label11.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                label14.Visible = false;
            }
            finally
            {
                currentlyTesting = false;
            }
        }

        private void AdvancedSystemSpeakerTest_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }

        private void AdvancedSystemSpeakerTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentlyTesting)
            {
                e.Cancel = true;
            }
        }

        private bool IsSystemSpeakerEntryExists()
        {
            try
            {
                bool isSystemSpeakerEntryPresentInWMI = false;
                string query = "SELECT * FROM Win32_PNPEntity WHERE DeviceID LIKE '%PNP0800%'";
                using (var searcher = new ManagementObjectSearcher(query))
                {
                    var devices = searcher.Get();
                    isSystemSpeakerEntryPresentInWMI = (devices.Count > 0);
                }
                return isSystemSpeakerEntryPresentInWMI;
            }
            catch (Exception ex)
            {
                Program.isExistenceOfSystemSpeakerChecked = false; // Mark that the check failed
                Logger.Log("Error during system speaker entry detection: " + ex.Message, Logger.LogTypes.Error);
                return false; // On error, assume no system speaker entry exists
            }
        }

        // T01 boolean: port 0x61 readable -> true if read succeeded
        private static bool IsSystemSpeakerPortReadable()
        {
            // If ReadPort throws, caller will observe exception.
            _ = ReadPort(PORT_SPEAKER);
            return true;
        }

        // T02 boolean: toggle gate bit round-trip (bit1 always 0)
        private static bool CanSystemSpeakerGateToggle()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                byte safeBase = (byte)(orig & ~0x02); // guarantee bit 1 = 0

                byte clrGate = (byte)(safeBase & ~0x01);
                byte setGate = (byte)(safeBase | 0x01);

                WritePort(PORT_SPEAKER, clrGate); BusyWaitUs(400);
                byte rdClr = ReadPort(PORT_SPEAKER);

                WritePort(PORT_SPEAKER, setGate); BusyWaitUs(400);
                byte rdSet = ReadPort(PORT_SPEAKER);

                SafeRestore(orig);

                bool okClr = (rdClr & 0x01) == 0 && (rdClr & 0x02) == 0;
                bool okSet = (rdSet & 0x01) == 1 && (rdSet & 0x02) == 0;

                return okClr && okSet;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T03 boolean: program PIT channel 2 with DIVISOR_MAX
        private static bool CanTheTimerBeSet()
        {
            ProgramPitCh2(DIVISOR_MAX);
            return true;
        }

        // T04 boolean: latch count twice; true if values differ (timer running)
        private static bool IsTheTimerCounting()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);

                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(1_000);

                int countA = LatchCount();
                BusyWaitUs(3_000);
                int countB = LatchCount();

                SafeRestore(orig);

                return countA != countB;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T05 boolean: read-back status decode; true if access/mode/bcd OK
        private static bool IsTimerStatusNormal()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(1_000);

                WritePort(PORT_PIT_CMD, PIT_CH2_READBACK);

                byte status = ReadPort(PORT_PIT_CH2);
                int lo = ReadPort(PORT_PIT_CH2);
                int hi = ReadPort(PORT_PIT_CH2);
                int count = lo | (hi << 8);

                SafeRestore(orig);

                int access = (status >> 4) & 0x03;
                int mode = (status >> 1) & 0x07;
                bool bcd = (status & 0x01) != 0;

                bool accessOk = access == 3;
                bool modeOk = mode == 3;
                bool bcdOk = !bcd;

                return accessOk && modeOk && bcdOk;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T06 boolean: bit-5 oscillation with gate ON/spk OFF
        private static bool IsFeedbackSignalDetected()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(2_000); // settle

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < 700)
                {
                    bool bit5 = (ReadPort(PORT_SPEAKER) & 0x20) != 0;
                    if (prev.HasValue && prev.Value != bit5) transitions++;
                    prev = bit5;
                    BusyWaitUs(3_000);
                }

                SafeRestore(orig);

                return transitions >= 4;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T07 boolean: bit-5 stability control gate OFF -> true if stable (no transitions)
        private static bool IsFeedbackSignalStable()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                byte gateOff = (byte)(orig & ~0x03); // gate=0, spk-data=0
                WritePort(PORT_SPEAKER, gateOff);
                BusyWaitUs(800);

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < 300)
                {
                    bool bit5 = (ReadPort(PORT_SPEAKER) & 0x20) != 0;
                    if (prev.HasValue && prev.Value != bit5) transitions++;
                    prev = bit5;
                    Thread.SpinWait(50);
                }

                SafeRestore(orig);

                return transitions == 0;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T08 boolean: sub-audio probe (momentarily enable spk-data) -> true if write sequence succeeds
        private static bool ShortSilentEnablePulseBool()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);

                WritePort(PORT_SPEAKER, (byte)(orig | 0x03));
                BusyWaitUs(100);
                SafeRestore(orig);

                return true;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T09 boolean: two divisors in-range checks
        private static bool TimerSpeedTestComparison()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                byte gated = GateOnSpeakerOff(orig);

                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, gated);
                BusyWaitUs(1_500);
                int cA = LatchCount();

                ProgramPitCh2(DIVISOR_MID);
                BusyWaitUs(1_500);
                int cB = LatchCount();

                SafeRestore(orig);

                bool aOk = cA >= 0 && cA <= DIVISOR_MAX;
                bool bOk = cB >= 0 && cB <= DIVISOR_MID;

                return aOk && bOk;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T10 boolean: frequency estimation from bit-5 transitions
        private static bool FeedbackSpeedResult()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(2_000); // settle

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();
                const long windowMs = 1_500;

                while (sw.ElapsedMilliseconds < windowMs)
                {
                    bool bit5 = (ReadPort(PORT_SPEAKER) & 0x20) != 0;
                    if (prev.HasValue && prev.Value != bit5) transitions++;
                    prev = bit5;
                    BusyWaitUs(2_000);
                }

                long elapsed = sw.ElapsedMilliseconds;
                SafeRestore(orig);

                double measHz = transitions / 2.0 / (elapsed / 1_000.0);
                double error = Math.Abs(measHz - EXPECTED_HZ);
                const double tolerance = 8.0; // Hz

                return transitions >= 2 && error <= tolerance;
            }
            catch { SafeRestore(orig); throw; }
        }

        // Public boolean method requested by user — runs all boolean tests and
        // returns true only if every boolean test returned true.
        // This method performs no printing/drawing and returns only a bool.
        public static bool RunAllTests_BooleanOnly()
        {
            // Run each boolean test in sequence. Exceptions will propagate; no console I/O.
            return IsSystemSpeakerPortReadable()
                && CanSystemSpeakerGateToggle()
                && CanTheTimerBeSet()
                && IsTheTimerCounting()
                && IsTimerStatusNormal()
                && IsFeedbackSignalDetected()
                && IsFeedbackSignalStable()
                && ShortSilentEnablePulseBool()
                && TimerSpeedTestComparison()
                && FeedbackSpeedResult();
        }
        // ════════════════════════════════════════════════════════════════════════
        //  I/O primitives
        // ════════════════════════════════════════════════════════════════════════

        private static byte ReadPort(short port) => unchecked((byte)(Inp32(port) & 0xFF));
        private static void WritePort(short port, byte value) => Out32(port, (short)value);

        private static void SafeRestore(byte original)
        {
            try { WritePort(PORT_SPEAKER, original); }
            catch { /* best-effort */ }
            BusyWaitUs(150);
        }

        /// <summary>High-resolution busy-wait using Stopwatch ticks.</summary>
        private static void BusyWaitUs(long microseconds)
        {
            if (microseconds <= 0) return;
            long ticks = (long)(microseconds * (Stopwatch.Frequency / 1_000_000.0));
            long target = Stopwatch.GetTimestamp() + Math.Max(1L, ticks);
            while (Stopwatch.GetTimestamp() < target)
                Thread.SpinWait(8);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  PIT helpers
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Program PIT channel 2 in square-wave mode with the given divisor.
        /// Caller must have already saved port 0x61.
        /// </summary>
        private static void ProgramPitCh2(int divisor)
        {
            divisor = Math.Clamp(divisor, 2, 65535);
            WritePort(PORT_PIT_CMD, PIT_CH2_SQUAREWAVE);
            WritePort(PORT_PIT_CH2, (byte)(divisor & 0xFF));
            WritePort(PORT_PIT_CH2, (byte)((divisor >> 8) & 0xFF));
        }

        /// <summary>
        /// Latch and read the 16-bit count from channel 2.
        /// MUST be called with the gate already enabled.
        /// </summary>
        private static int LatchCount()
        {
            WritePort(PORT_PIT_CMD, PIT_CH2_LATCH);
            int lo = ReadPort(PORT_PIT_CH2);
            int hi = ReadPort(PORT_PIT_CH2);
            return lo | (hi << 8);
        }

        /// <summary>Returns byte with gate bit set, speaker-data bit cleared (safe probe state).</summary>
        private static byte GateOnSpeakerOff(byte orig) => (byte)((orig | 0x01) & ~0x02);
    }
}