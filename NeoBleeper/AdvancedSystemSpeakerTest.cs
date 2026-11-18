using NeoBleeper.Properties;
using System.Diagnostics;
using System.Management;
using static UIHelper;

namespace NeoBleeper
{
    public partial class AdvancedSystemSpeakerTest : Form
    {
        // ── PawnIO PcSpeaker module access ───────────────────────────────────────
        // The module exposes a single IOCTL, "ioctl_batch". Each input cell is one
        // command: bits [63:56] = opcode, bits [55:0] = payload. The module only
        // touches PIT counter 2 and bits 0/1 of port 0x61, and it caps stalls at
        // 50 us per command / 1000 us per batch (interrupts are disabled while a
        // batch runs), so all long waits and polling loops stay in managed code.
        private const string IOCTL_BATCH = "ioctl_batch";

        private const ulong OP_NOP = 0x00;
        private const ulong OP_PIT_MODE = 0x01;      // [7:0] command byte for port 0x43
        private const ulong OP_PIT_CH2_COUNT = 0x02; // [15:0] divisor (lo byte, then hi byte)
        private const ulong OP_PIT_CH2_WRITE = 0x03; // [7:0] byte
        private const ulong OP_PIT_CH2_READ = 0x04;  // result = byte read from port 0x42
        private const ulong OP_PPI_WRITE = 0x05;     // [15:8] mask, [7:0] value; result = new port value
        private const ulong OP_PPI_READ = 0x06;      // result = byte read from port 0x61
        private const ulong OP_STALL = 0x07;         // [15:0] microseconds (max 50)

        private const int CMD_OPCODE_SHIFT = 56;
        private const ulong CMD_PAYLOAD_MASK = 0x00FFFFFFFFFFFFFF;

        // Only bits 0 (timer-2 gate) and 1 (speaker data enable) of port 0x61 are writable.
        private const byte PPI_B_TIMER2_GATE = 0x01;
        private const byte PPI_B_SPEAKER_EN = 0x02;
        private const byte PPI_B_WRITE_MASK = PPI_B_TIMER2_GATE | PPI_B_SPEAKER_EN;
        private const byte PPI_B_OUT2_STATUS = 0x20; // bit 5: timer-2 output (read-only)

        // ── PIT command bytes ────────────────────────────────────────────────────
        // Channel 2 | lo/hi byte | mode 3 (square wave) | binary  →  10 11 011 0
        private const byte PIT_CH2_SQUAREWAVE = 0xB6;

        // Counter-latch command for channel 2: bits[7:6]=10, bits[5:4]=00  →  1000 0000
        private const byte PIT_CH2_LATCH = 0x80;

        // Read-back command: latch count + status for channel 2 only (accepted by the module)
        // 11 (read-back) | 00 (DO latch both) | 1000 (channel 2) | 0 (reserved) →  1100 1000
        // Status byte is returned first on the next read from port 0x42,
        // followed by the latched count (lo byte, hi byte).
        private const byte PIT_CH2_READBACK = 0xC8;

        // ── Timing / frequency constants ─────────────────────────────────────────
        // Maximum PIT divisor (16-bit) → minimum frequency ≈ 18.2 Hz (below 20 Hz auditory floor)
        private const int DIVISOR_MAX = 65535;
        private const int DIVISOR_MID = 16384; // ~72.8 Hz — used for T09 only (gate ON, spk OFF)
        private const double PIT_CLOCK = 1_193_182.0;
        private const double EXPECTED_HZ = PIT_CLOCK / DIVISOR_MAX; // ~18.20 Hz
        private static PawnIOWrapper? _pawn;
        private static readonly object _pawnLock = new();
        private static PawnIOWrapper Pawn
        {
            get
            {
                lock (_pawnLock)
                {
                    return _pawn ??= new PawnIOWrapper(Resources.PcSpeakerModule);
                }
            }
        }

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
            UIHelper.ApplyCustomTitleBarAndBorder(this, Color.White, darkTheme);
        }
        private void DarkTheme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBarAndBorder(this, Color.Black, darkTheme);
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
        /// <remarks>This method runs multiple hardware and signal tests on the system speaker through the
        /// PawnIO PcSpeaker module, including electrical feedback, port state stability, and frequency sweep
        /// diagnostics. Progress and results are logged and displayed to the user. The method is asynchronous and
        /// should not be called concurrently. If an error occurs during testing, the user interface is updated to
        /// indicate the failure and details are logged.</remarks>
        private async void StartTest()
        {
            try
            {
                currentlyTesting = true;

                // Make sure the PawnIO driver is present (no-op when already installed).
                await Task.Run(() => PawnIOWrapper.InstallPawnIO());
                if (!ProgramPrecheck.IsPawnIOInstalled())
                {
                    throw new InvalidOperationException("PawnIO is not installed.");
                }

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


                OverallState overallState = OverallState.UNCERTAIN;
                // Summary for each situations
                var tests = new bool[]
                {
                    systemSpeakerOutputReadability,
                    canSystemSpeakerGateTurnOnOff,
                    canTimerBeSet,
                    isTimerActuallyCountingDown,
                    isTimerStatusNormal,
                    isFeedbackSignalDetected,
                    isFeedbackSignalStableWhenDisabled,
                    shortSilentEnablePulseResult,
                    timerSpeedTestResult,
                    feedbackSignalSpeedMeasurementResult
                };

                int passCount = tests.Count(t => t);
                int totalTests = tests.Length;
                int failCount = totalTests - passCount;

                bool t06Pass = isFeedbackSignalDetected;
                bool t10Pass = feedbackSignalSpeedMeasurementResult;

                // Decision tree
                // - PASSED if there's a system speaker entry
                // - If 6th and 10th step is failed => NOT DETECTED -> FAILED
                // - If all/most tests are passed and 6th and 10th step is passed => PASSED
                // - If 6th or 10th step is observed, but no other steps are observed => UNCERTAIN
                // - If few tests are passed, but other ones not => UNCERTAIN
                // - Other conditions => FAILED
                if (systemSpeakerEntryCheckResult)
                {
                    overallState = OverallState.PASSED;
                }
                else if (!t06Pass && !t10Pass)
                {
                    overallState = OverallState.FAILED; // NOT DETECTED
                }
                else if (passCount >= 8 && failCount == 0 && t06Pass && t10Pass)
                {
                    overallState = OverallState.PASSED; // SPEAKER CONFIRMED -> PASSED
                }
                else if (passCount >= 6 && failCount <= 1 && (t06Pass || t10Pass))
                {
                    overallState = OverallState.PASSED; // LIKELY PRESENT -> TREATED AS PASSED
                }
                else if (t06Pass || t10Pass)
                {
                    overallState = OverallState.UNCERTAIN;
                }
                else if (passCount >= 3)
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
                    if (systemSpeakerEntryCheckResult)
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

                    label14.Text = basics + (issues.Count > 0 ? "\r\n" + Resources.Issues + string.Join(string.Empty, issues) : "");
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
            // If ReadPpi throws (driver/module failure), caller will observe the exception.
            _ = ReadPpi();
            return true;
        }

        // T02 boolean: toggle gate bit round-trip (bit1 always 0)
        private static bool CanSystemSpeakerGateToggle()
        {
            byte orig = ReadPpi();
            try
            {
                // Clear gate + speaker data
                RunBatch(PpiWrite(PPI_B_WRITE_MASK, 0x00));
                BusyWaitUs(400);
                byte rdClr = ReadPpi();

                // Set gate, keep speaker data cleared
                RunBatch(PpiWrite(PPI_B_WRITE_MASK, PPI_B_TIMER2_GATE));
                BusyWaitUs(400);
                byte rdSet = ReadPpi();

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
            byte orig = ReadPpi();
            try
            {
                ProgramPitCh2(DIVISOR_MAX);

                GateOnSpeakerOff();
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
            byte orig = ReadPpi();
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                GateOnSpeakerOff();
                BusyWaitUs(1_000);

                // Read-back: status byte first, then latched count (lo, hi).
                long[] r = RunBatch(
                    Cmd(OP_PIT_MODE, PIT_CH2_READBACK),
                    Cmd(OP_PIT_CH2_READ),
                    Cmd(OP_PIT_CH2_READ),
                    Cmd(OP_PIT_CH2_READ));

                byte status = (byte)(r[1] & 0xFF);

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
            byte orig = ReadPpi();
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                GateOnSpeakerOff();
                BusyWaitUs(2_000); // settle

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < 700)
                {
                    bool bit5 = (ReadPpi() & PPI_B_OUT2_STATUS) != 0;
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
            byte orig = ReadPpi();
            try
            {
                RunBatch(PpiWrite(PPI_B_WRITE_MASK, 0x00)); // gate=0, spk-data=0
                BusyWaitUs(800);

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < 300)
                {
                    bool bit5 = (ReadPpi() & PPI_B_OUT2_STATUS) != 0;
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
            byte orig = ReadPpi();
            try
            {
                ProgramPitCh2(DIVISOR_MAX);

                // Enable gate + speaker data for ~100 us (2 x 50 us stalls, the per-command cap),
                // then restore the original gate/data bits, all inside one batch.
                RunBatch(
                    PpiWrite(PPI_B_WRITE_MASK, PPI_B_WRITE_MASK),
                    Stall(50),
                    Stall(50),
                    PpiWrite(PPI_B_WRITE_MASK, (byte)(orig & PPI_B_WRITE_MASK)));

                SafeRestore(orig);

                return true;
            }
            catch { SafeRestore(orig); throw; }
        }

        // T09 boolean: two divisors in-range checks
        private static bool TimerSpeedTestComparison()
        {
            byte orig = ReadPpi();
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                GateOnSpeakerOff();
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
            byte orig = ReadPpi();
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                GateOnSpeakerOff();
                BusyWaitUs(2_000); // settle

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();
                const long windowMs = 1_500;

                while (sw.ElapsedMilliseconds < windowMs)
                {
                    bool bit5 = (ReadPpi() & PPI_B_OUT2_STATUS) != 0;
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
        //  PawnIO command primitives
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Packs an opcode and payload into one module command cell.</summary>
        private static long Cmd(ulong opcode, ulong payload = 0)
            => unchecked((long)((opcode << CMD_OPCODE_SHIFT) | (payload & CMD_PAYLOAD_MASK)));

        /// <summary>PPI_WRITE: read-modify-write of port 0x61 limited to bits 0/1.</summary>
        private static long PpiWrite(byte mask, byte value)
            => Cmd(OP_PPI_WRITE, ((ulong)mask << 8) | value);

        /// <summary>STALL: busy-wait inside the module (max 50 us per command, 1000 us per batch).</summary>
        private static long Stall(int microseconds)
            => Cmd(OP_STALL, (ulong)(microseconds & 0xFFFF));

        /// <summary>
        /// Sends a batch to the PcSpeaker module and returns one result per command.
        /// The module validates the whole batch before executing any of it.
        /// </summary>
        private static long[] RunBatch(params long[] commands)
        {
            long[] results = Pawn.Execute(IOCTL_BATCH, commands, commands.Length);
            if (results.Length < commands.Length)
            {
                throw new InvalidOperationException(
                    $"PcSpeaker module returned {results.Length} results for {commands.Length} commands.");
            }
            return results;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  I/O primitives
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Reads port 0x61 (full byte, including read-only bit 5).</summary>
        private static byte ReadPpi() => (byte)(RunBatch(Cmd(OP_PPI_READ))[0] & 0xFF);

        /// <summary>
        /// Restores bits 0/1 of port 0x61 to their original values.
        /// The module cannot (and does not need to) write any other bit.
        /// </summary>
        private static void SafeRestore(byte original)
        {
            try { RunBatch(PpiWrite(PPI_B_WRITE_MASK, (byte)(original & PPI_B_WRITE_MASK))); }
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
            RunBatch(
                Cmd(OP_PIT_MODE, PIT_CH2_SQUAREWAVE),
                Cmd(OP_PIT_CH2_COUNT, (ulong)divisor)); // module writes lo byte, then hi byte
        }

        /// <summary>
        /// Latch and read the 16-bit count from channel 2.
        /// MUST be called with the gate already enabled.
        /// </summary>
        private static int LatchCount()
        {
            long[] r = RunBatch(
                Cmd(OP_PIT_MODE, PIT_CH2_LATCH),
                Cmd(OP_PIT_CH2_READ),
                Cmd(OP_PIT_CH2_READ));

            int lo = (int)(r[1] & 0xFF);
            int hi = (int)(r[2] & 0xFF);
            return lo | (hi << 8);
        }

        /// <summary>Sets the gate bit and clears the speaker-data bit (safe probe state).</summary>
        private static void GateOnSpeakerOff()
            => RunBatch(PpiWrite(PPI_B_WRITE_MASK, PPI_B_TIMER2_GATE));
    }
}