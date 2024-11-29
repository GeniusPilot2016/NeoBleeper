namespace NeoBleeper
{
    partial class get_stickers_window
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(get_stickers_window));
            radioButton_printable = new RadioButton();
            radioButton_digital_stickers = new RadioButton();
            printDialog1 = new PrintDialog();
            printDocument1 = new System.Drawing.Printing.PrintDocument();
            button_get_sticker = new Button();
            imageList_get_stickers = new ImageList(components);
            SuspendLayout();
            // 
            // radioButton_printable
            // 
            radioButton_printable.Anchor = AnchorStyles.Top;
            radioButton_printable.AutoSize = true;
            radioButton_printable.Checked = true;
            radioButton_printable.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            radioButton_printable.ImageIndex = 1;
            radioButton_printable.ImageList = imageList_get_stickers;
            radioButton_printable.Location = new Point(25, 12);
            radioButton_printable.Name = "radioButton_printable";
            radioButton_printable.Size = new Size(90, 20);
            radioButton_printable.TabIndex = 0;
            radioButton_printable.TabStop = true;
            radioButton_printable.Text = "Printable";
            radioButton_printable.TextImageRelation = TextImageRelation.ImageBeforeText;
            radioButton_printable.UseVisualStyleBackColor = true;
            // 
            // radioButton_digital_stickers
            // 
            radioButton_digital_stickers.Anchor = AnchorStyles.Top;
            radioButton_digital_stickers.AutoSize = true;
            radioButton_digital_stickers.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            radioButton_digital_stickers.ImageIndex = 0;
            radioButton_digital_stickers.ImageList = imageList_get_stickers;
            radioButton_digital_stickers.Location = new Point(124, 12);
            radioButton_digital_stickers.Name = "radioButton_digital_stickers";
            radioButton_digital_stickers.Size = new Size(122, 20);
            radioButton_digital_stickers.TabIndex = 1;
            radioButton_digital_stickers.Text = "Digital Stickers";
            radioButton_digital_stickers.TextImageRelation = TextImageRelation.ImageBeforeText;
            radioButton_digital_stickers.UseVisualStyleBackColor = true;
            // 
            // printDialog1
            // 
            printDialog1.UseEXDialog = true;
            // 
            // button_get_sticker
            // 
            button_get_sticker.Anchor = AnchorStyles.Bottom;
            button_get_sticker.Font = new Font("HarmonyOS Sans", 9F, FontStyle.Regular, GraphicsUnit.Point);
            button_get_sticker.ImageIndex = 2;
            button_get_sticker.ImageList = imageList_get_stickers;
            button_get_sticker.Location = new Point(65, 47);
            button_get_sticker.Name = "button_get_sticker";
            button_get_sticker.Size = new Size(128, 23);
            button_get_sticker.TabIndex = 2;
            button_get_sticker.Text = "&Get Your Stickers";
            button_get_sticker.TextAlign = ContentAlignment.MiddleRight;
            button_get_sticker.TextImageRelation = TextImageRelation.ImageBeforeText;
            button_get_sticker.UseVisualStyleBackColor = true;
            // 
            // imageList_get_stickers
            // 
            imageList_get_stickers.ColorDepth = ColorDepth.Depth32Bit;
            imageList_get_stickers.ImageStream = (ImageListStreamer)resources.GetObject("imageList_get_stickers.ImageStream");
            imageList_get_stickers.TransparentColor = Color.Transparent;
            imageList_get_stickers.Images.SetKeyName(0, "icons8-electronics-48 (1).png");
            imageList_get_stickers.Images.SetKeyName(1, "icons8-printer-48.png");
            imageList_get_stickers.Images.SetKeyName(2, "icons8-sticker-48.png");
            // 
            // get_stickers_window
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(264, 82);
            Controls.Add(button_get_sticker);
            Controls.Add(radioButton_digital_stickers);
            Controls.Add(radioButton_printable);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "get_stickers_window";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Get Your Free Stickers";
            FormClosed += get_stickers_window_FormClosed;
            Load += get_stickers_window_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButton_printable;
        private RadioButton radioButton_digital_stickers;
        private PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private Button button_get_sticker;
        private ImageList imageList_get_stickers;
    }
}