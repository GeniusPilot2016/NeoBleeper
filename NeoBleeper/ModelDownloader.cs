using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class ModelDownloader : Form
    {
        public ModelDownloader(string modelName)
        {
            InitializeComponent();
            label1.Text = label1.Text.Replace("{modelName}", modelName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageForm.Show("The model is being downloaded. Do you want to cancel the download?", "Downloading Model", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        private void AIModelDownloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void AIModelDownloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
