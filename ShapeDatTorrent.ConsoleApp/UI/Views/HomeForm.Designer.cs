namespace ShapeDatTorrent.ConsoleApp.UI.Views
{
    partial class HomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeForm));
            pictureBox1 = new PictureBox();
            MainTLP = new TableLayoutPanel();
            label6 = new Label();
            label1 = new Label();
            fileSystemBrowser1 = new FileSystemBrowser();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            checkBox1 = new CheckBox();
            fileSystemBrowser2 = new FileSystemBrowser();
            fileSystemBrowser3 = new FileSystemBrowser();
            fileSystemBrowser4 = new FileSystemBrowser();
            label5 = new Label();
            numericUpDown1 = new NumericUpDown();
            richTextBox1 = new RichTextBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            ProduceBT = new Button();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            MainTLP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.logo;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Dock = DockStyle.Top;
            pictureBox1.Location = new Point(4, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(576, 91);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // MainTLP
            // 
            MainTLP.ColumnCount = 2;
            MainTLP.ColumnStyles.Add(new ColumnStyle());
            MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainTLP.Controls.Add(label6, 0, 4);
            MainTLP.Controls.Add(label1, 0, 0);
            MainTLP.Controls.Add(fileSystemBrowser1, 1, 0);
            MainTLP.Controls.Add(label2, 0, 1);
            MainTLP.Controls.Add(label3, 0, 2);
            MainTLP.Controls.Add(label4, 0, 3);
            MainTLP.Controls.Add(checkBox1, 1, 5);
            MainTLP.Controls.Add(fileSystemBrowser2, 1, 1);
            MainTLP.Controls.Add(fileSystemBrowser3, 1, 2);
            MainTLP.Controls.Add(fileSystemBrowser4, 1, 3);
            MainTLP.Controls.Add(label5, 1, 6);
            MainTLP.Controls.Add(numericUpDown1, 1, 4);
            MainTLP.Controls.Add(richTextBox1, 1, 7);
            MainTLP.Dock = DockStyle.Fill;
            MainTLP.Location = new Point(4, 95);
            MainTLP.Name = "MainTLP";
            MainTLP.RowCount = 8;
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            MainTLP.Size = new Size(576, 417);
            MainTLP.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Segoe UI", 11.25F);
            label6.Location = new Point(3, 160);
            label6.Name = "label6";
            label6.Size = new Size(112, 33);
            label6.TabIndex = 10;
            label6.Text = "Max Size (GB):";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            toolTip1.SetToolTip(label6, "The maximum final footprint for a .torrent in GBs");
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 11.25F);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(112, 40);
            label1.TabIndex = 0;
            label1.Text = "* Torrent File In:";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            toolTip1.SetToolTip(label1, "This is the .torrent file that you wish to split/filter");
            // 
            // fileSystemBrowser1
            // 
            fileSystemBrowser1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileSystemBrowser1.BackColor = Color.Transparent;
            fileSystemBrowser1.ExtensionFilter = "\"Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*\"";
            fileSystemBrowser1.Font = new Font("Segoe UI", 11.25F);
            fileSystemBrowser1.Location = new Point(120, 2);
            fileSystemBrowser1.Margin = new Padding(2);
            fileSystemBrowser1.Mode = FileBrowserMode.File;
            fileSystemBrowser1.Name = "fileSystemBrowser1";
            fileSystemBrowser1.Prompt = "Choose the .torrent file to split/filter";
            fileSystemBrowser1.Size = new Size(454, 36);
            fileSystemBrowser1.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 11.25F);
            label2.Location = new Point(3, 40);
            label2.Name = "label2";
            label2.Size = new Size(112, 40);
            label2.TabIndex = 1;
            label2.Text = "Filter DAT:";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            toolTip1.SetToolTip(label2, "The .dat to use to filter the files within the .torrent");
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 11.25F);
            label3.Location = new Point(3, 80);
            label3.Name = "label3";
            label3.Size = new Size(112, 40);
            label3.TabIndex = 2;
            label3.Text = "Excluded DAT:";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            toolTip1.SetToolTip(label3, "any .dat that details what files will have been excluded from the output .torrents");
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 11.25F);
            label4.Location = new Point(3, 120);
            label4.Name = "label4";
            label4.Size = new Size(112, 40);
            label4.TabIndex = 3;
            label4.Text = "Output folder:";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            toolTip1.SetToolTip(label4, "Where to write the .torrent output files and report to (if generated)");
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Segoe UI", 11.25F);
            checkBox1.Location = new Point(121, 196);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(120, 24);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "Strict Filtering";
            toolTip1.SetToolTip(checkBox1, "Strict filetering means file tags have ot match precisely. Default of false is safest");
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // fileSystemBrowser2
            // 
            fileSystemBrowser2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileSystemBrowser2.BackColor = Color.Transparent;
            fileSystemBrowser2.ExtensionFilter = "\"Dat files (*.dat)|*.dat\"";
            fileSystemBrowser2.Font = new Font("Segoe UI", 11.25F);
            fileSystemBrowser2.Location = new Point(120, 42);
            fileSystemBrowser2.Margin = new Padding(2);
            fileSystemBrowser2.Mode = FileBrowserMode.File;
            fileSystemBrowser2.Name = "fileSystemBrowser2";
            fileSystemBrowser2.Prompt = "Choose the filter .dat (eg. a ReTool .dat)";
            fileSystemBrowser2.Size = new Size(454, 36);
            fileSystemBrowser2.TabIndex = 5;
            // 
            // fileSystemBrowser3
            // 
            fileSystemBrowser3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileSystemBrowser3.BackColor = Color.Transparent;
            fileSystemBrowser3.ExtensionFilter = "\"Dat files (*.dat)|*.dat\"";
            fileSystemBrowser3.Font = new Font("Segoe UI", 11.25F);
            fileSystemBrowser3.Location = new Point(120, 82);
            fileSystemBrowser3.Margin = new Padding(2);
            fileSystemBrowser3.Mode = FileBrowserMode.File;
            fileSystemBrowser3.Name = "fileSystemBrowser3";
            fileSystemBrowser3.Prompt = "Choose the 'excluded' .dat (eg. ReTool's excluded .dat)";
            fileSystemBrowser3.Size = new Size(454, 36);
            fileSystemBrowser3.TabIndex = 6;
            // 
            // fileSystemBrowser4
            // 
            fileSystemBrowser4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fileSystemBrowser4.BackColor = Color.Transparent;
            fileSystemBrowser4.ExtensionFilter = "All files (*.*)|*.*";
            fileSystemBrowser4.Font = new Font("Segoe UI", 11.25F);
            fileSystemBrowser4.Location = new Point(120, 122);
            fileSystemBrowser4.Margin = new Padding(2);
            fileSystemBrowser4.Mode = FileBrowserMode.Folder;
            fileSystemBrowser4.Name = "fileSystemBrowser4";
            fileSystemBrowser4.Prompt = "Choose where to output the files to";
            fileSystemBrowser4.Size = new Size(454, 36);
            fileSystemBrowser4.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 11.25F);
            label5.Location = new Point(123, 228);
            label5.Margin = new Padding(5);
            label5.Name = "label5";
            label5.Size = new Size(75, 20);
            label5.TabIndex = 8;
            label5.Text = "* required";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            numericUpDown1.BackColor = Color.FromArgb(32, 32, 32);
            numericUpDown1.Font = new Font("Segoe UI", 11.25F);
            numericUpDown1.ForeColor = Color.Gainsboro;
            numericUpDown1.Location = new Point(121, 163);
            numericUpDown1.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(452, 27);
            numericUpDown1.TabIndex = 11;
            numericUpDown1.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(16, 16, 16);
            richTextBox1.BorderStyle = BorderStyle.None;
            MainTLP.SetColumnSpan(richTextBox1, 2);
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Font = new Font("Monospac821 BT", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            richTextBox1.ForeColor = Color.Lime;
            richTextBox1.Location = new Point(8, 261);
            richTextBox1.Margin = new Padding(8);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(560, 148);
            richTextBox1.TabIndex = 12;
            richTextBox1.Text = "Console";
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(ProduceBT);
            flowLayoutPanel2.Dock = DockStyle.Bottom;
            flowLayoutPanel2.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel2.Location = new Point(4, 512);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Padding = new Padding(2, 2, 2, 6);
            flowLayoutPanel2.Size = new Size(576, 45);
            flowLayoutPanel2.TabIndex = 3;
            // 
            // ProduceBT
            // 
            ProduceBT.BackColor = Color.FromArgb(64, 64, 64);
            ProduceBT.FlatAppearance.BorderColor = Color.Gray;
            ProduceBT.FlatStyle = FlatStyle.Flat;
            ProduceBT.Image = Properties.Resources.rocket_fly;
            ProduceBT.ImageAlign = ContentAlignment.MiddleRight;
            ProduceBT.Location = new Point(437, 5);
            ProduceBT.Name = "ProduceBT";
            ProduceBT.Size = new Size(132, 31);
            ProduceBT.TabIndex = 0;
            ProduceBT.Text = " Produce";
            ProduceBT.TextAlign = ContentAlignment.MiddleLeft;
            ProduceBT.TextImageRelation = TextImageRelation.ImageBeforeText;
            ProduceBT.UseVisualStyleBackColor = false;
            // 
            // toolTip1
            // 
            toolTip1.BackColor = Color.FromArgb(64, 64, 64);
            toolTip1.ForeColor = Color.Ivory;
            // 
            // HomeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(584, 561);
            Controls.Add(MainTLP);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(pictureBox1);
            ForeColor = Color.Gainsboro;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(600, 600);
            Name = "HomeForm";
            Padding = new Padding(4);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Shape-dat-Torrent";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            MainTLP.ResumeLayout(false);
            MainTLP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TableLayoutPanel MainTLP;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private CheckBox checkBox1;
        private FileSystemBrowser fileSystemBrowser2;
        private FileSystemBrowser fileSystemBrowser3;
        private FileSystemBrowser fileSystemBrowser4;
        private Label label5;
        private FileSystemBrowser fileSystemBrowser1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button ProduceBT;
        private ToolTip toolTip1;
        private Label label6;
        private NumericUpDown numericUpDown1;
        private RichTextBox richTextBox1;
    }
}