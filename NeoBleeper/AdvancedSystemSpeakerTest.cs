using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class AdvancedSystemSpeakerTest : Form
    {
        bool darkTheme = false;
        bool currentlyTesting = false;
        public AdvancedSystemSpeakerTest()
        {
            InitializeComponent();
            set_theme();
            UIFonts.setFonts(this);
            StartTest();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SETTINGCHANGE = 0x001A;
            base.WndProc(ref m);

            if (m.Msg == WM_SETTINGCHANGE)
            {
                if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme();
                }
            }
        }
        private void set_theme()
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
                            dark_theme();
                        }
                        else
                        {
                            light_theme();
                        }
                        break;

                    case 1:
                        light_theme();
                        break;

                    case 2:
                        dark_theme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
            }
        }
        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }
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
                Logger.Log("Performing electrical feedback test...", Logger.LogTypes.Info);
                label2.Visible = true;
                await Task.Delay(500);
                bool electricalFeedbackTestPassed = SoundRenderingEngine.SystemSpeakerBeepEngine.CheckElectricalFeedbackOnPort();
                Logger.Log("Electrical feedback test result: " + (electricalFeedbackTestPassed ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label2.Text = "        " + Resources.ElectricalFeedbackTest + ((electricalFeedbackTestPassed) ? Resources.TextPassed : Resources.TextFailed);
                label2.ImageIndex = (electricalFeedbackTestPassed) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Performing port state stability test...", Logger.LogTypes.Info);
                label3.Visible = true;
                await Task.Delay(500);
                bool portStabilityTestPassed = SoundRenderingEngine.SystemSpeakerBeepEngine.CheckPortStateStability();
                Logger.Log("Port state stability test result: " + (portStabilityTestPassed ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label3.Text = "        " + Resources.PortStateStabilityTest + ((portStabilityTestPassed) ? Resources.TextPassed : Resources.TextFailed);
                label3.ImageIndex = (portStabilityTestPassed) ? 1 : 2;
                await Task.Delay(500);
                Logger.Log("Performing advanced frequency sweep test...", Logger.LogTypes.Info);
                label4.Visible = true;
                await Task.Delay(500);
                bool frequencySweepTestPassed = SoundRenderingEngine.SystemSpeakerBeepEngine.AdvancedFrequencySweepTest();
                Logger.Log("Advanced frequency sweep test result: " + (frequencySweepTestPassed ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label4.Text = "        " + Resources.AdvancedFrequencySweepTest + ((frequencySweepTestPassed) ? Resources.TextPassed : Resources.TextFailed);
                label4.ImageIndex = (frequencySweepTestPassed) ? 1 : 2;
                bool result = electricalFeedbackTestPassed || portStabilityTestPassed || frequencySweepTestPassed;
                await Task.Delay(500);
                Logger.Log("Overall advanced system speaker test result: " + (result ? "PASSED" : "FAILED"), Logger.LogTypes.Info);
                label5.Visible = true;
                label5.Text = "        " + Resources.OverallResult + ((result) ? Resources.TextPassed : Resources.TextFailed);
                label5.ImageIndex = (result) ? 1 : 2;
                if (!result)
                {
                    label6.Visible = true;
                }
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
            }
            finally
            {
                currentlyTesting = false;
            }
        }

        private void AdvancedSystemSpeakerTest_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }

        private void AdvancedSystemSpeakerTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (currentlyTesting)
            {
                e.Cancel = true;
            }
        }
    }
}
