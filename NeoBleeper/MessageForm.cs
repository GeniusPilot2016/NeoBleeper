using GenerativeAI.Types;
using NeoBleeper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoBleeper
{
    public partial class MessageForm : Form
    {
        bool darkTheme = false;
        bool IsThemeManuallySet = false; // Flag to indicate if theme is manually set
        int theme = 0; // 0: System, 1: Light, 2: Dark
        public MessageForm(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            InitializeComponent();
            this.Text = title;
            labelMessage.Text = message;
            AssignButtons(buttons);
            AssignIcon(icon);
            writeMessage(message);
            set_theme();
            UIFonts.setFonts(this);
            PlaySound(icon);
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SETTINGCHANGE = 0x001A;
            base.WndProc(ref m);

            if (m.Msg == WM_SETTINGCHANGE)
            {
                if(IsThemeManuallySet && theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    set_theme_manually(theme);
                }
                else if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
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
        private void set_theme_manually(int theme)
        {
            this.SuspendLayout(); // Suspend layout to batch updates
            this.DoubleBuffered = true; // Enable double buffering for smoother rendering

            try
            {
                switch (theme)
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
        private void dark_theme()
        {
            darkTheme = true;
            this.BackColor = Color.FromArgb(32, 32, 32);
            this.ForeColor = Color.White;
            button1.BackColor = Color.FromArgb(32, 32, 32);
            button1.ForeColor = Color.White;
            button2.BackColor = Color.FromArgb(32, 32, 32);
            button2.ForeColor = Color.White;
            button3.BackColor = Color.FromArgb(32, 32, 32);
            button3.ForeColor = Color.White;
            UIHelper.ApplyCustomTitleBar(this, Color.Black, darkTheme);
        }


        private void light_theme()
        {
            darkTheme = false;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            button1.BackColor = Color.Transparent;
            button1.ForeColor = SystemColors.ControlText;
            button2.BackColor = Color.Transparent;
            button2.ForeColor = SystemColors.ControlText;
            button3.BackColor = Color.Transparent;
            button3.ForeColor = SystemColors.ControlText;
            UIHelper.ApplyCustomTitleBar(this, Color.White, darkTheme);
        }

        private void PlaySound(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    SystemSounds.Asterisk.Play();
                    break;
                case MessageBoxIcon.Warning:
                    SystemSounds.Exclamation.Play();
                    break;
                case MessageBoxIcon.Question:
                    SystemSounds.Question.Play();
                    break;
                case MessageBoxIcon.Error:
                    SystemSounds.Hand.Play();
                    break;
            }
        }
        private void RemoveSelectedCell(Button[] button)
        {
            int cellSize = tableLayoutPanelActionButtons.GetColumnWidths()[0];
            foreach (var btn in button)
            {
                tableLayoutPanelActionButtons.Controls.Remove(btn);
                tableLayoutPanelActionButtons.ColumnCount--;
                tableLayoutPanelActionButtons.Width -= cellSize;
            }
            tableLayoutPanelActionButtons.Location = new Point(
                (this.ClientSize.Width - tableLayoutPanelActionButtons.Width) / 2,
                tableLayoutPanelActionButtons.Location.Y
            );
        }
        private void AssignIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    pictureBoxIcon.Image = Resources.icons8_information_48;
                    break;
                case MessageBoxIcon.Warning:
                    pictureBoxIcon.Image = Resources.icons8_warning_48;
                    break;
                case MessageBoxIcon.Error:
                    pictureBoxIcon.Image = Resources.icons8_error_48;
                    break;
                case MessageBoxIcon.Question:
                    pictureBoxIcon.Image = Resources.icons8_question_48;
                    break;
                default:
                    pictureBoxIcon.Image = null;
                    break;
            }
        }
        private void writeMessage(string message)
        {
            if (message != null)
            {
                double dpiScaleFactor = UIHelper.GetDPIScaleFactor(this);
                int padding = (int)(50 * dpiScaleFactor);
                Size formerPreferredSize = labelMessage.PreferredSize; // Store former size
                labelMessage.AutoSize = true; // Ensure label resizes to fit text
                labelMessage.Text = message;

                // Use PreferredSize to get the actual size required for the text
                Size preferredSize = labelMessage.PreferredSize;

                // Adjust form size based on the label's preferred size
                this.Width = Math.Max(this.Width, preferredSize.Width + labelMessage.Location.X * 2);
                this.Height = Math.Max(this.Height, preferredSize.Height + labelMessage.Location.Y + tableLayoutPanelActionButtons.Height + padding);

                this.PerformLayout(); // Refresh layout
            }
        }
        private void AssignButtons(MessageBoxButtons buttons)
        {
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    RemoveSelectedCell(new Button[] { button2, button3 });
                    button1.ImageIndex = 0;
                    button1.Text = Resources.ButtonOK;
                    button1.DialogResult = DialogResult.OK;
                    break;
                case MessageBoxButtons.YesNo:
                    RemoveSelectedCell(new Button[] { button3 });
                    button1.ImageIndex = 0;
                    button1.Text = Resources.ButtonYes;
                    button1.DialogResult = DialogResult.Yes;
                    button2.ImageIndex = 1;
                    button2.Text = Resources.ButtonNo;
                    button2.DialogResult = DialogResult.No;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    button1.ImageIndex = 0;
                    button1.Text = Resources.ButtonYes;
                    button1.DialogResult = DialogResult.Yes;
                    button2.ImageIndex = 1;
                    button2.Text = Resources.ButtonNo;
                    button2.DialogResult = DialogResult.No;
                    button3.ImageIndex = 3;
                    button3.Text = Resources.ButtonCancel;
                    button3.DialogResult = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.RetryCancel:
                    RemoveSelectedCell(new Button[] { button3 });
                    button1.ImageIndex = 2;
                    button1.Text = Resources.ButtonRetry;
                    button1.DialogResult = DialogResult.Retry;
                    button2.ImageIndex = 3;
                    button2.Text = Resources.ButtonCancel;
                    button2.DialogResult = DialogResult.Cancel;
                    break;

            }
        }
        public static DialogResult Show(String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }
        public static DialogResult Show(Form form, String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            messageForm.StartPosition = FormStartPosition.CenterParent; // Center on parent form
            messageForm.Owner = form; // Set the owner to the provided form
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }
        public static DialogResult Show(int theme, String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            messageForm.IsThemeManuallySet = true;
            messageForm.set_theme_manually(theme);
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }
        public static DialogResult Show(Form form, int theme, String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            messageForm.StartPosition = FormStartPosition.CenterParent; // Center on parent form
            messageForm.Owner = form; // Set the owner to the provided form
            messageForm.IsThemeManuallySet = true;
            messageForm.set_theme_manually(theme);
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }

        private void MessageForm_SystemColorsChanged(object sender, EventArgs e)
        {
            set_theme();
        }
    }
}
