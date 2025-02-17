namespace NeoBleeper
{
    partial class about_neobleeper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(about_neobleeper));
            pictureBox1 = new PictureBox();
            lbl_credit = new Label();
            lbl_name = new Label();
            lbl_version = new Label();
            imageList_about = new ImageList(components);
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            label1 = new Label();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.neobleeper_icon;
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // lbl_credit
            // 
            resources.ApplyResources(lbl_credit, "lbl_credit");
            lbl_credit.Name = "lbl_credit";
            // 
            // lbl_name
            // 
            resources.ApplyResources(lbl_name, "lbl_name");
            lbl_name.Name = "lbl_name";
            // 
            // lbl_version
            // 
            resources.ApplyResources(lbl_version, "lbl_version");
            lbl_version.Name = "lbl_version";
            // 
            // imageList_about
            // 
            imageList_about.ColorDepth = ColorDepth.Depth32Bit;
            imageList_about.ImageStream = (ImageListStreamer)resources.GetObject("imageList_about.ImageStream");
            imageList_about.TransparentColor = Color.Transparent;
            imageList_about.Images.SetKeyName(0, "icons8-icons8-48.png");
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2 });
            resources.ApplyResources(listView1, "listView1");
            listView1.FullRowSelect = true;
            listView1.Items.AddRange(new ListViewItem[] { (ListViewItem)resources.GetObject("listView1.Items"), (ListViewItem)resources.GetObject("listView1.Items1") });
            listView1.MultiSelect = false;
            listView1.Name = "listView1";
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(columnHeader2, "columnHeader2");
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.ImageList = imageList_about;
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // about_neobleeper
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(listView1);
            Controls.Add(lbl_version);
            Controls.Add(lbl_name);
            Controls.Add(lbl_credit);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "about_neobleeper";
            ShowIcon = false;
            ShowInTaskbar = false;
            FormClosing += about_neobleeper_FormClosing;
            FormClosed += about_neobleeper_FormClosed;
            Load += about_neobleeper_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lbl_credit;
        private Label lbl_name;
        private Label lbl_version;
        private ImageList imageList_about;
        private ListView listView1;
        private Label label1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Button button1;
    }
}