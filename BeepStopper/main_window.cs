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
