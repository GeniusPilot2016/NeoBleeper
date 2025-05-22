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
    public partial class PortamentoWindow : Form
    {
        public PortamentoWindow(main_window main_Window)
        {
            InitializeComponent();
        }

        private void trackBarLength_Scroll(object sender, EventArgs e)
        {
            labelLength.Text = trackBarLength.Value.ToString() + " mS";
        }
    }
}
