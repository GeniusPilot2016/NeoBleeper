using NeoBleeper;
using System.Drawing.Text;

namespace BeepStopper
{
    public partial class main_window : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public main_window()
        {
            InitializeComponent(); 
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            foreach (Control ctrl in Controls)
            {
                ctrl.Font = new Font(fonts.Families[0], 9);
            }
            instructionLabel.Font = new Font(fonts.Families[0], 9, FontStyle.Bold);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Stop the beeping without force-shutdown
            try
            {
                RenderBeep.BeepClass.StopBeep();
                MessageBox.Show("Beep is successfully stopped!", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                // Show the error message
                MessageBox.Show("An error occurred: " + ex.Message, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
