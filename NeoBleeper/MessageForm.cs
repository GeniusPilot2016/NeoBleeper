using NeoBleeper.Properties;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static UIHelper;

namespace NeoBleeper
{
    public partial class MessageForm : Form
    {
        bool darkTheme = false;
        bool IsThemeManuallySet = false; // Flag to indicate if theme is manually set
        int theme = 0; // 0: System, 1: Light, 2: Dark
        MessageBoxIcon icon = MessageBoxIcon.None;
        string title = string.Empty;
        string message = string.Empty;
        public MessageForm(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ThemeManager_ThemeChanged;
            this.Text = title;
            this.title = title;
            labelMessage.Text = message;
            AssignButtons(buttons);
            AssignIcon(icon);
            WriteMessage(message);
            SetTheme();
            UIFonts.SetFonts(this);
        }
        private ToolTipIcon ConvertToToolTipIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Information:
                    return ToolTipIcon.Info;
                case MessageBoxIcon.Warning:
                    return ToolTipIcon.Warning;
                case MessageBoxIcon.Error:
                    return ToolTipIcon.Error;
                default:
                    return ToolTipIcon.None;
            }
        }
        private void ThemeManager_ThemeChanged(object? sender, EventArgs e)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                if (IsThemeManuallySet && theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    SetThemeManually(theme);
                }
                else if (Settings1.Default.theme == 0 && (darkTheme != SystemThemeUtility.IsDarkTheme()))
                {
                    SetTheme();
                }
            }
        }
        private void SetTheme()
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
                            DarkTheme();
                        }
                        else
                        {
                            LightTheme();
                        }
                        break;

                    case 1:
                        LightTheme();
                        break;

                    case 2:
                        DarkTheme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
                this.ResumeLayout();
            }
        }
        private void SetThemeManually(int theme)
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
                            DarkTheme();
                        }
                        else
                        {
                            LightTheme();
                        }
                        break;

                    case 1:
                        LightTheme();
                        break;

                    case 2:
                        DarkTheme();
                        break;
                }
            }
            finally
            {
                UIHelper.ForceUpdateUI(this); // Force update to apply changes
            }
        }
        private void DarkTheme()
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


        private void LightTheme()
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
            int cellSize = (int)(tableLayoutPanelActionButtons.Width / tableLayoutPanelActionButtons.ColumnCount);
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
            this.icon = icon;
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
        private void WriteMessage(string message)
        {
            if (message != null)
            {
                this.message = message;
                double dpiScaleFactor = UIHelper.GetDPIScaleFactor(this);
                int padding = (int)(50 * dpiScaleFactor);

                labelMessage.AutoSize = true;
                labelMessage.Text = message;

                // Use PreferredSize to get the actual size required for the text
                Size preferredSize = labelMessage.PreferredSize;

                // Calculate single line height
                int singleLineHeight = TextRenderer.MeasureText("A", labelMessage.Font).Height;

                // If label is multi-line, adjust its Y position to center it with pictureBoxIcon
                if (preferredSize.Height > singleLineHeight)
                {
                    // Set label's Y position to pictureBoxIcon's Top
                    if (preferredSize.Height >= pictureBoxIcon.Height)
                    {
                        labelMessage.Location = new Point(labelMessage.Location.X, pictureBoxIcon.Top);
                    }
                    else
                    {
                        int newY = pictureBoxIcon.Top + (pictureBoxIcon.Height - preferredSize.Height) / 2;
                        labelMessage.Location = new Point(labelMessage.Location.X, newY);
                    }
                }

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
                case MessageBoxButtons.AbortRetryIgnore:
                    button1.ImageIndex = 4;
                    button1.Text = Resources.ButtonAbort;
                    button1.DialogResult = DialogResult.Abort;
                    button2.ImageIndex = 2;
                    button2.Text = Resources.ButtonRetry;
                    button2.DialogResult = DialogResult.Retry;
                    button3.ImageIndex = 5;
                    button3.Text = Resources.ButtonIgnore;
                    button3.DialogResult = DialogResult.Ignore;
                    break;
                case MessageBoxButtons.OKCancel:
                    RemoveSelectedCell(new Button[] { button3 });
                    button1.ImageIndex = 0;
                    button1.Text = Resources.ButtonOK;
                    button1.DialogResult = DialogResult.OK;
                    button2.ImageIndex = 3;
                    button2.Text = Resources.ButtonCancel;
                    button2.DialogResult = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.CancelTryContinue:
                    button1.ImageIndex = 3;
                    button1.Text = Resources.ButtonCancel;
                    button1.DialogResult = DialogResult.Cancel;
                    button2.ImageIndex = 6;
                    button2.Text = Resources.ButtonTryAgain;
                    button2.DialogResult = DialogResult.TryAgain;
                    button3.ImageIndex = 5;
                    button3.Text = Resources.ButtonContinue;
                    button3.DialogResult = DialogResult.Continue;
                    break;
            }
            ResizeCellsByText(); // Resize cells based on button text length for some languages such as German
        }
        private void ResizeCellsByText()
        {
            double dpi = UIHelper.GetDPIScaleFactor(this);
            double padding = 20.0 * dpi; // Padding around text
            double iconSize = 25.0 * dpi;
            float totalWidth = 0;

            // Suspend layout to avoid flickering during resize
            tableLayoutPanelActionButtons.SuspendLayout();

            for (int columnNumber = 0; columnNumber < tableLayoutPanelActionButtons.ColumnCount; columnNumber++)
            {
                Control control = tableLayoutPanelActionButtons.GetControlFromPosition(columnNumber, 0);
                if (control is Button button)
                {
                    Size textSize = TextRenderer.MeasureText(button.Text, button.Font);

                    // Start width with text and base padding
                    float buttonWidth = textSize.Width + (float)padding;

                    // Add icon size if button has an image
                    if (button.Image != null || button.ImageIndex >= 0)
                    {
                        buttonWidth += (float)iconSize;
                    }

                    tableLayoutPanelActionButtons.ColumnStyles[columnNumber].Width = buttonWidth;
                    totalWidth += buttonWidth;
                }
            }

            // Resume layout after resizing
            tableLayoutPanelActionButtons.ResumeLayout(false);

            // Reset panel width
            tableLayoutPanelActionButtons.Width = (int)Math.Ceiling(totalWidth);

            // Resize window if necessary
            int requiredWidth = tableLayoutPanelActionButtons.Width + tableLayoutPanelActionButtons.Margin.Horizontal;
            if (this.ClientSize.Width < requiredWidth)
            {
                this.Width += requiredWidth - this.ClientSize.Width;
            }

            // Middle the panel
            tableLayoutPanelActionButtons.Location = new Point(
                (this.ClientSize.Width - tableLayoutPanelActionButtons.Width) / 2,
                tableLayoutPanelActionButtons.Location.Y
            );
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
            messageForm.SetThemeManually(theme);
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
            messageForm.SetThemeManually(theme);
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }
        private void MessageForm_SystemColorsChanged(object sender, EventArgs e)
        {
            SetTheme();
        }

        private async void MessageForm_Shown(object sender, EventArgs e)
        {
            if (NotificationUtils.IsWindowObscured(this)) // Useful when the computer hasn't any audio device or muted
            {
                NotificationUtils.CreateAndShowNotification(this, title, message, ConvertToToolTipIcon(icon), 3000);
            }
            else
            {
                PlaySound(icon); // Play sound when the form is shown
            }
        }
    }
}
