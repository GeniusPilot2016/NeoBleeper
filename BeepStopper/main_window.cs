using BeepStopper.Properties;
using NeoBleeper;
using System.Drawing.Text;
using static NBPML_File;

namespace BeepStopper
{
    public partial class main_window : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public main_window()
        {
            InitializeComponent();
            UIFonts.setFonts(this);
            TitleBarHelper.ApplyCustomTitleBar(this, Color.White);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Stop the beeping without force-shutdown
            try
            {
                if (RenderBeep.BeepClass.isSystemSpeakerBeepStuck())
                {
                    RenderBeep.BeepClass.StopBeep();
                    MessageBox.Show(Resources.BeepStoppedMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(Resources.SystemSpeakerIsNotStuckMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Show the error message
                MessageBox.Show(Resources.AnErrorOccuredMessage + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
