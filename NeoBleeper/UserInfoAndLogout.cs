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
    public partial class UserInfoAndLogout : UserControl
    {
        UIFonts fonts = UIFonts.Instance;
        public UserInfoAndLogout()
        {
            InitializeComponent();
            setInfoFromSettings();
            setFonts();
        }
        private void setFonts()
        {
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Font = fonts.SetUIFont(ctrl.Font.Size, ctrl.Font.Style);
            }
        }
        private void setInfoFromSettings()
        {
            string imageBase64 = Settings1.Default.cachedProfileImageBase64;
            if (!string.IsNullOrEmpty(imageBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imageBase64);
                using (var ms = new System.IO.MemoryStream(imageBytes))
                {
                    Image profileImage = Image.FromStream(ms);
                    pictureBox1.Image = profileImage;
                }
            }
            label1.Text = Settings1.Default.cachedUserName;
        }
    }
}
