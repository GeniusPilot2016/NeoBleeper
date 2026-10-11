namespace Silent_beep_scenario_tester
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            button1 = new Button();
            button2 = new Button();
            richTextBox1 = new RichTextBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            clearConsoleToolStripMenuItem = new ToolStripMenuItem();
            button3 = new Button();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(57, 12);
            button1.Name = "button1";
            button1.Size = new Size(111, 33);
            button1.TabIndex = 0;
            button1.Text = "Imıtate the bug";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnImitateBug_Click;
            // 
            // button2
            // 
            button2.Location = new Point(193, 12);
            button2.Name = "button2";
            button2.Size = new Size(111, 33);
            button2.TabIndex = 1;
            button2.Text = "Restore";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnRestore_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ContextMenuStrip = contextMenuStrip1;
            richTextBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 162);
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Location = new Point(12, 117);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(338, 148);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { clearConsoleToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(146, 26);
            // 
            // clearConsoleToolStripMenuItem
            // 
            clearConsoleToolStripMenuItem.Name = "clearConsoleToolStripMenuItem";
            clearConsoleToolStripMenuItem.Size = new Size(145, 22);
            clearConsoleToolStripMenuItem.Text = "Clear console";
            clearConsoleToolStripMenuItem.Click += clearConsoleToolStripMenuItem_Click;
            // 
            // button3
            // 
            button3.Location = new Point(126, 65);
            button3.Name = "button3";
            button3.Size = new Size(111, 46);
            button3.TabIndex = 1;
            button3.Text = "Play beep and observe Bit 5";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btnPlayAndObserve_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(362, 277);
            Controls.Add(richTextBox1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "Form1";
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
        private RichTextBox richTextBox1;
        private Button button3;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem clearConsoleToolStripMenuItem;
    }
}
