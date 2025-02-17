using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class MIDI_file_player : Form
    {
        PrivateFontCollection fonts = new PrivateFontCollection();
        public MIDI_file_player(string filename)
        {
            InitializeComponent();
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Regular.ttf");
            fonts.AddFontFile(Application.StartupPath + "Resources/HarmonyOS_Sans_Bold.ttf");
            foreach (Control ctrl in Controls)
            {
                if (ctrl.Controls != null)
                {
                    ctrl.Font = new Font(fonts.Families[0], 9);
                    label_note1.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note2.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note3.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note4.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note5.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note6.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note7.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note8.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note9.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note10.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note11.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note12.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note13.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note14.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note15.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note16.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note17.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note18.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note19.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note20.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note21.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note22.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note23.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note24.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note25.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note26.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note27.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note28.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note29.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note30.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note31.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                    label_note32.Font = new Font(fonts.Families[0], 10, FontStyle.Bold);
                }
            }
            textBox1.Text = filename;

        }
        private bool IsMidiFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            byte[] header = new byte[4];
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(header, 0, 4);
            }

            // MIDI dosyaları "MThd" ile başlar
            return header[0] == 'M' && header[1] == 'T' && header[2] == 'h' && header[3] == 'd';
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "MIDI Files|*.mid|All Files|*.*";
            openFileDialog.ShowDialog(this);
            if (openFileDialog.FileName != string.Empty)
            {
                if (IsMidiFile(openFileDialog.FileName))
                {
                    textBox1 .Text = openFileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("This file is not a valid MIDI file or the file is corrupted.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
