namespace NeoBleeper
{
    partial class MessageForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            pictureBoxIcon = new PictureBox();
            labelMessage = new Label();
            button1 = new Button();
            icons = new ImageList(components);
            tableLayoutPanelActionButtons = new TableLayoutPanel();
            button3 = new Button();
            button2 = new Button();
            panel1 = new Panel();
            notifyIconMessage = new NotifyIcon(components);
            ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).BeginInit();
            tableLayoutPanelActionButtons.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBoxIcon
            // 
            pictureBoxIcon.Location = new Point(12, 12);
            pictureBoxIcon.Name = "pictureBoxIcon";
            pictureBoxIcon.Size = new Size(48, 48);
            pictureBoxIcon.TabIndex = 0;
            pictureBoxIcon.TabStop = false;
            // 
            // labelMessage
            // 
            labelMessage.AutoSize = true;
            labelMessage.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelMessage.Location = new Point(66, 28);
            labelMessage.MaximumSize = new Size(250, 896);
            labelMessage.Name = "labelMessage";
            labelMessage.Size = new Size(184, 16);
            labelMessage.TabIndex = 1;
            labelMessage.Text = "Message                                           ";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button1.Font = new Font("HarmonyOS Sans", 9F);
            button1.ImageList = icons;
            button1.Location = new Point(3, 7);
            button1.Name = "button1";
            button1.Size = new Size(83, 26);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            // 
            // icons
            // 
            icons.ColorDepth = ColorDepth.Depth32Bit;
            icons.ImageStream = (ImageListStreamer)resources.GetObject("icons.ImageStream");
            icons.TransparentColor = Color.Transparent;
            icons.Images.SetKeyName(0, "icons8-yes-48.png");
            icons.Images.SetKeyName(1, "icons8-no-48.png");
            icons.Images.SetKeyName(2, "icons8-retry-48.png");
            icons.Images.SetKeyName(3, "icons8-cancel-48.png");
            icons.Images.SetKeyName(4, "icons8-exit-48.png");
            icons.Images.SetKeyName(5, "icons8-forward-button-48.png");
            icons.Images.SetKeyName(6, "icons8-refresh-48.png");
            // 
            // tableLayoutPanelActionButtons
            // 
            tableLayoutPanelActionButtons.Anchor = AnchorStyles.Bottom;
            tableLayoutPanelActionButtons.ColumnCount = 3;
            tableLayoutPanelActionButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33778F));
            tableLayoutPanelActionButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3311119F));
            tableLayoutPanelActionButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33111F));
            tableLayoutPanelActionButtons.Controls.Add(button3, 2, 0);
            tableLayoutPanelActionButtons.Controls.Add(button2, 1, 0);
            tableLayoutPanelActionButtons.Controls.Add(button1, 0, 0);
            tableLayoutPanelActionButtons.Location = new Point(1, 71);
            tableLayoutPanelActionButtons.Name = "tableLayoutPanelActionButtons";
            tableLayoutPanelActionButtons.RowCount = 1;
            tableLayoutPanelActionButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanelActionButtons.Size = new Size(267, 40);
            tableLayoutPanelActionButtons.TabIndex = 3;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button3.Font = new Font("HarmonyOS Sans", 9F);
            button3.ImageList = icons;
            button3.Location = new Point(180, 7);
            button3.Name = "button3";
            button3.Size = new Size(84, 26);
            button3.TabIndex = 2;
            button3.Text = "button3";
            button3.TextAlign = ContentAlignment.MiddleRight;
            button3.TextImageRelation = TextImageRelation.ImageBeforeText;
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            button2.Font = new Font("HarmonyOS Sans", 9F);
            button2.ImageList = icons;
            button2.Location = new Point(92, 7);
            button2.Name = "button2";
            button2.Size = new Size(82, 26);
            button2.TabIndex = 1;
            button2.Text = "button2";
            button2.TextAlign = ContentAlignment.MiddleRight;
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            panel1.Location = new Point(233, 76);
            panel1.Name = "panel1";
            panel1.Size = new Size(35, 35);
            panel1.TabIndex = 4;
            // 
            // notifyIconMessage
            // 
            notifyIconMessage.Icon = (Icon)resources.GetObject("notifyIconMessage.Icon");
            notifyIconMessage.Text = "NeoBleeper";
            notifyIconMessage.BalloonTipClicked += notifyIconMessage_BalloonTipClicked;
            // 
            // MessageForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(269, 111);
            Controls.Add(tableLayoutPanelActionButtons);
            Controls.Add(labelMessage);
            Controls.Add(pictureBoxIcon);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(550, 1024);
            MinimizeBox = false;
            MinimumSize = new Size(285, 150);
            Name = "MessageForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Title";
            Shown += MessageForm_Shown;
            SystemColorsChanged += MessageForm_SystemColorsChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).EndInit();
            tableLayoutPanelActionButtons.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxIcon;
        private Label labelMessage;
        private Button button1;
        private TableLayoutPanel tableLayoutPanelActionButtons;
        private ImageList icons;
        private Button button3;
        private Button button2;
        private Panel panel1;
        private NotifyIcon notifyIconMessage;
    }
}