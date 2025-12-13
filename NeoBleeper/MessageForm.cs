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

        /// <summary>
        /// Converts a MessageBoxIcon value to the corresponding ToolTipIcon value.
        /// </summary>
        /// <param name="icon">The MessageBoxIcon value to convert.</param>
        /// <returns>A ToolTipIcon value that corresponds to the specified MessageBoxIcon. Returns ToolTipIcon.None if the icon
        /// does not match a known value.</returns>
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

        /// <summary>
        /// Applies the current application theme to the control based on user or system settings.
        /// </summary>
        /// <remarks>This method updates the control's appearance to match the selected theme. It
        /// automatically determines whether to use a light or dark theme according to user preferences or the system
        /// theme setting. Call this method after changing theme-related settings to ensure the control reflects the
        /// updated theme.</remarks>
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

        /// <summary>
        /// Applies the specified theme to the control based on the provided theme identifier.
        /// </summary>
        /// <remarks>This method updates the control's appearance immediately to reflect the selected
        /// theme. If 0 is specified, the method detects the system's current theme and applies the corresponding
        /// appearance.</remarks>
        /// <param name="theme">An integer representing the theme to apply. Specify 0 to use the system's current theme, 1 for the light
        /// theme, or 2 for the dark theme.</param>
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

        /// <summary>
        /// Plays the system sound associated with the specified message box icon type.
        /// </summary>
        /// <remarks>This method plays the default system sound for the given icon type. If the icon type
        /// is not recognized, no sound is played.</remarks>
        /// <param name="icon">The type of message box icon for which to play the corresponding system sound.</param>
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

        /// <summary>
        /// Removes the specified buttons from the action buttons table layout and updates the layout accordingly.
        /// </summary>
        /// <remarks>After removal, the method adjusts the column count and width of the table layout
        /// panel to reflect the changes, and re-centers the panel within the form. This method should be called only
        /// when the specified buttons are currently present in the table layout panel.</remarks>
        /// <param name="button">An array of buttons to remove from the table layout. Each button in the array must be a child of the table
        /// layout panel.</param>
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

        /// <summary>
        /// Sets the icon displayed in the message box to match the specified message box icon type.
        /// </summary>
        /// <remarks>If an unrecognized icon value is provided, no icon is displayed.</remarks>
        /// <param name="icon">The type of icon to display in the message box. Specifies the visual style of the icon, such as Information,
        /// Warning, Error, or Question.</param>
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

        /// <summary>
        /// Updates the displayed message text and adjusts the layout of the form to accommodate the new message.
        /// </summary>
        /// <remarks>This method recalculates the size and position of UI elements to ensure the message
        /// is fully visible and properly aligned. The form's dimensions may increase to fit multi-line or long
        /// messages.</remarks>
        /// <param name="message">The message text to display. If null, the message and layout are not updated.</param>
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

        /// <summary>
        /// Configures the dialog buttons according to the specified MessageBoxButtons value.
        /// </summary>
        /// <remarks>This method updates the button text, images, and dialog results to match the standard
        /// button arrangement for the given MessageBoxButtons option. It also adjusts the button layout to accommodate
        /// localized text lengths.</remarks>
        /// <param name="buttons">A MessageBoxButtons value that determines which buttons are displayed and how they are labeled and assigned
        /// dialog results.</param>
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

        /// <summary>
        /// Resizes the columns of the action button table layout panel to fit the text and icons of the contained
        /// buttons, adjusting the panel and window width as needed.
        /// </summary>
        /// <remarks>This method measures the text and icon size of each button in the first row of the
        /// table layout panel and sets the column widths accordingly. The panel and window are resized if necessary to
        /// ensure all buttons are fully visible. Layout is suspended during the operation to prevent flickering. This
        /// method should be called after any changes to button text, font, or images to ensure proper sizing.</remarks>
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

        /// <summary>
        /// Displays a modal message dialog box with the specified text, title, buttons, and icon, and returns the
        /// user's response.
        /// </summary>
        /// <remarks>The dialog box is shown modally and blocks interaction with other windows until the
        /// user closes it. Use this method to prompt the user for a simple response or to display informational
        /// messages.</remarks>
        /// <param name="message">The text to display in the message dialog box. Cannot be null.</param>
        /// <param name="title">The text to display in the title bar of the dialog box. If empty, a default title may be used.</param>
        /// <param name="buttons">A value that specifies which buttons to display in the dialog box. The default is MessageBoxButtons.OK.</param>
        /// <param name="icon">A value that specifies which icon to display in the dialog box. The default is MessageBoxIcon.None.</param>
        /// <returns>A DialogResult value indicating which button the user clicked in the dialog box.</returns>
        public static DialogResult Show(String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }

        /// <summary>
        /// Displays a modal message dialog box with the specified message, title, buttons, and icon, centered on the
        /// given parent form.
        /// </summary>
        /// <remarks>The dialog is shown as a modal window and blocks interaction with the parent form
        /// until closed. The dialog is always centered on the specified parent form.</remarks>
        /// <param name="form">The parent form that owns the message dialog. The dialog will be centered on this form. Cannot be null.</param>
        /// <param name="message">The text to display in the message dialog.</param>
        /// <param name="title">The caption to display in the title bar of the dialog. If empty, a default title may be used.</param>
        /// <param name="buttons">A value that specifies which buttons to display in the dialog box. The default is MessageBoxButtons.OK.</param>
        /// <param name="icon">A value that specifies which icon to display in the dialog box. The default is MessageBoxIcon.None.</param>
        /// <returns>A DialogResult value indicating which button the user clicked in the dialog box.</returns>
        public static DialogResult Show(Form form, String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            messageForm.StartPosition = FormStartPosition.CenterParent; // Center on parent form
            messageForm.Owner = form; // Set the owner to the provided form
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }

        /// <summary>
        /// Displays a message box with the specified theme, message, title, buttons, and icon, and returns the result.
        /// </summary>
        /// <remarks>Use this method to display a themed message box when you need to customize its
        /// appearance according to a specific theme. The method blocks execution until the user closes the message
        /// box.</remarks>
        /// <param name="theme">The identifier of the theme to apply to the message box. The value must correspond to a valid theme
        /// supported by the application.</param>
        /// <param name="message">The text to display in the message box.</param>
        /// <param name="title">The text to display in the title bar of the message box. If not specified, the title bar will be empty.</param>
        /// <param name="buttons">A value that specifies which buttons to display in the message box. The default is MessageBoxButtons.OK.</param>
        /// <param name="icon">A value that specifies which icon to display in the message box. The default is MessageBoxIcon.None.</param>
        /// <returns>A DialogResult value indicating which button the user clicked in the message box.</returns>
        public static DialogResult Show(int theme, String message, String title = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            MessageForm messageForm = new MessageForm(message, title, buttons, icon);
            messageForm.IsThemeManuallySet = true;
            messageForm.SetThemeManually(theme);
            DialogResult dialogResult = messageForm.ShowDialog();
            return dialogResult;
        }

        /// <summary>
        /// Displays a modal message dialog with the specified message, title, buttons, icon, and theme, centered on the
        /// given parent form.
        /// </summary>
        /// <remarks>This method blocks the calling thread until the user closes the dialog. The dialog is
        /// shown as a modal window and must be closed before the user can interact with the parent form. The theme is
        /// applied manually based on the provided theme identifier.</remarks>
        /// <param name="form">The parent form that owns the message dialog. The dialog will be centered on this form. Cannot be null.</param>
        /// <param name="theme">The theme identifier to apply to the message dialog. The value determines the visual appearance of the
        /// dialog.</param>
        /// <param name="message">The text to display in the body of the message dialog. Cannot be null.</param>
        /// <param name="title">The text to display in the title bar of the message dialog. If not specified, the title bar will be empty.</param>
        /// <param name="buttons">A value that specifies which buttons to display in the message dialog, such as OK or Yes/No. The default is
        /// MessageBoxButtons.OK.</param>
        /// <param name="icon">A value that specifies which icon to display in the message dialog. The default is MessageBoxIcon.None.</param>
        /// <returns>A DialogResult value indicating which button the user clicked to close the dialog.</returns>
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
