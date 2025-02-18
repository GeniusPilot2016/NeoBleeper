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
                }
            }
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
            label_note1.BackColor = Settings1.Default.note_indicator_color;
            label_note2.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note3.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note4.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note5.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note6.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note7.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note8.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note9.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note10.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note11.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note12.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note13.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note14.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note15.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note16.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note17.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note18.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note19.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note20.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note21.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note22.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note23.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note24.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note25.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note26.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note27.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note28.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note29.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note30.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note31.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note32.BackColor = set_playing_note_color.GetPlayingNoteColor(Settings1.Default.note_indicator_color);
            label_note1.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note2.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note3.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note4.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note5.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note6.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note7.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note8.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note9.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note10.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note11.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note12.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note13.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note14.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note15.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note16.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note17.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note18.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note19.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note20.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note21.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note22.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note23.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note24.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note25.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note26.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note27.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note28.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note29.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note30.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note31.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
            label_note32.ForeColor = set_text_color.GetTextColor(Settings1.Default.note_indicator_color);
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

            // MIDI files always start with the header MThd
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
