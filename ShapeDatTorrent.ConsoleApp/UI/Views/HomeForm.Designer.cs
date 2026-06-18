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
            TorrentPathTB = new FileSystemBrowser();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            FilterPathTB = new FileSystemBrowser();
            ExcludedPathTB = new FileSystemBrowser();
            OutputPathTB = new FileSystemBrowser();
            MaxSizeNUM = new NumericUpDown();
            ConsoleRTB = new RichTextBox();
            ValidationLB = new Label();
            StrictFilteringChB = new CheckBox();
            label5 = new Label();
            flowLayoutPanel2 = new FlowLayoutPanel();
            ProduceBT = new Button();
            OpenLogBT = new Button();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            MainTLP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MaxSizeNUM).BeginInit();
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
            toolTip1.SetToolTip(pictureBox1, "More 90s. Less gray...");
            // 
            // MainTLP
            // 
            MainTLP.ColumnCount = 2;
            MainTLP.ColumnStyles.Add(new ColumnStyle());
            MainTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainTLP.Controls.Add(label6, 0, 4);
            MainTLP.Controls.Add(label1, 0, 0);
            MainTLP.Controls.Add(TorrentPathTB, 1, 0);
            MainTLP.Controls.Add(label2, 0, 1);
            MainTLP.Controls.Add(label3, 0, 2);
            MainTLP.Controls.Add(label4, 0, 3);
            MainTLP.Controls.Add(FilterPathTB, 1, 1);
            MainTLP.Controls.Add(ExcludedPathTB, 1, 2);
            MainTLP.Controls.Add(OutputPathTB, 1, 3);
            MainTLP.Controls.Add(MaxSizeNUM, 1, 4);
            MainTLP.Controls.Add(ConsoleRTB, 1, 8);
            MainTLP.Controls.Add(ValidationLB, 1, 6);
            MainTLP.Controls.Add(StrictFilteringChB, 1, 5);
            MainTLP.Controls.Add(label5, 0, 5);
            MainTLP.Dock = DockStyle.Fill;
            MainTLP.Location = new Point(4, 95);
            MainTLP.Name = "MainTLP";
            MainTLP.RowCount = 9;
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
            MainTLP.RowStyles.Add(new RowStyle());
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
            label6.Size = new Size(118, 37);
            label6.TabIndex = 10;
            label6.Text = "Max Size (GB):";
            label6.TextAlign = ContentAlignment.MiddleRight;
            toolTip1.SetToolTip(label6, "The maximum final footprint for a .torrent in GBs");
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 11.25F);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(118, 40);
            label1.TabIndex = 0;
            label1.Text = "* Master Torrent:";
            label1.TextAlign = ContentAlignment.MiddleRight;
            toolTip1.SetToolTip(label1, "This is the .torrent file that you wish to split/filter");
            // 
            // TorrentPathTB
            // 
            TorrentPathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TorrentPathTB.BackColor = Color.Transparent;
            TorrentPathTB.ExtensionFilter = "Torrent files (*.torrent)|*.torrent|All files (*.*)|*.*";
            TorrentPathTB.Font = new Font("Segoe UI", 11.25F);
            TorrentPathTB.Location = new Point(126, 2);
            TorrentPathTB.Margin = new Padding(2);
            TorrentPathTB.Mode = FileBrowserMode.File;
            TorrentPathTB.Name = "TorrentPathTB";
            TorrentPathTB.Prompt = "Choose the .torrent file to split/filter";
            TorrentPathTB.Size = new Size(448, 36);
            TorrentPathTB.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 11.25F);
            label2.Location = new Point(3, 40);
            label2.Name = "label2";
            label2.Size = new Size(118, 40);
            label2.TabIndex = 1;
            label2.Text = "Filter DAT:";
            label2.TextAlign = ContentAlignment.MiddleRight;
            toolTip1.SetToolTip(label2, "The .dat to use to filter the files within the .torrent");
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 11.25F);
            label3.Location = new Point(3, 80);
            label3.Name = "label3";
            label3.Size = new Size(118, 40);
            label3.TabIndex = 2;
            label3.Text = "Excluded DAT:";
            label3.TextAlign = ContentAlignment.MiddleRight;
            toolTip1.SetToolTip(label3, "Any .dat that details what files will have been excluded from the output .torrents");
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 11.25F);
            label4.Location = new Point(3, 120);
            label4.Name = "label4";
            label4.Size = new Size(118, 40);
            label4.TabIndex = 3;
            label4.Text = "Output folder:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            toolTip1.SetToolTip(label4, "Where to write the .torrent output files and report to (if generated)");
            // 
            // FilterPathTB
            // 
            FilterPathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            FilterPathTB.BackColor = Color.Transparent;
            FilterPathTB.ExtensionFilter = "Dat files (*.dat)|*.dat";
            FilterPathTB.Font = new Font("Segoe UI", 11.25F);
            FilterPathTB.Location = new Point(126, 42);
            FilterPathTB.Margin = new Padding(2);
            FilterPathTB.Mode = FileBrowserMode.File;
            FilterPathTB.Name = "FilterPathTB";
            FilterPathTB.Prompt = "Choose the filter .dat (eg. a ReTool .dat)";
            FilterPathTB.Size = new Size(448, 36);
            FilterPathTB.TabIndex = 5;
            // 
            // ExcludedPathTB
            // 
            ExcludedPathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ExcludedPathTB.BackColor = Color.Transparent;
            ExcludedPathTB.ExtensionFilter = "Dat files (*.dat)|*.dat";
            ExcludedPathTB.Font = new Font("Segoe UI", 11.25F);
            ExcludedPathTB.Location = new Point(126, 82);
            ExcludedPathTB.Margin = new Padding(2);
            ExcludedPathTB.Mode = FileBrowserMode.File;
            ExcludedPathTB.Name = "ExcludedPathTB";
            ExcludedPathTB.Prompt = "Choose the 'excluded' .dat (eg. ReTool's excluded .dat)";
            ExcludedPathTB.Size = new Size(448, 36);
            ExcludedPathTB.TabIndex = 6;
            // 
            // OutputPathTB
            // 
            OutputPathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            OutputPathTB.BackColor = Color.Transparent;
            OutputPathTB.ExtensionFilter = "All files (*.*)|*.*";
            OutputPathTB.Font = new Font("Segoe UI", 11.25F);
            OutputPathTB.Location = new Point(126, 122);
            OutputPathTB.Margin = new Padding(2);
            OutputPathTB.Mode = FileBrowserMode.Folder;
            OutputPathTB.Name = "OutputPathTB";
            OutputPathTB.Prompt = "Choose where to output the files to";
            OutputPathTB.Size = new Size(448, 36);
            OutputPathTB.TabIndex = 7;
            // 
            // MaxSizeNUM
            // 
            MaxSizeNUM.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            MaxSizeNUM.BackColor = Color.FromArgb(32, 32, 32);
            MaxSizeNUM.Font = new Font("Segoe UI", 11.25F);
            MaxSizeNUM.ForeColor = Color.Gainsboro;
            MaxSizeNUM.Location = new Point(129, 165);
            MaxSizeNUM.Margin = new Padding(5);
            MaxSizeNUM.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            MaxSizeNUM.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
            MaxSizeNUM.Name = "MaxSizeNUM";
            MaxSizeNUM.Size = new Size(442, 27);
            MaxSizeNUM.TabIndex = 11;
            MaxSizeNUM.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // ConsoleRTB
            // 
            ConsoleRTB.BackColor = Color.FromArgb(16, 16, 16);
            ConsoleRTB.BorderStyle = BorderStyle.None;
            MainTLP.SetColumnSpan(ConsoleRTB, 2);
            ConsoleRTB.Dock = DockStyle.Fill;
            ConsoleRTB.Font = new Font("Monospac821 BT", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ConsoleRTB.ForeColor = Color.Gainsboro;
            ConsoleRTB.Location = new Point(8, 259);
            ConsoleRTB.Margin = new Padding(8);
            ConsoleRTB.Name = "ConsoleRTB";
            ConsoleRTB.Size = new Size(560, 150);
            ConsoleRTB.TabIndex = 12;
            ConsoleRTB.Text = "";
            ConsoleRTB.WordWrap = false;
            // 
            // ValidationLB
            // 
            ValidationLB.AutoSize = true;
            ValidationLB.Dock = DockStyle.Fill;
            ValidationLB.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ValidationLB.ForeColor = Color.DarkGray;
            ValidationLB.Image = Properties.Resources.information;
            ValidationLB.ImageAlign = ContentAlignment.MiddleLeft;
            ValidationLB.Location = new Point(127, 227);
            ValidationLB.Name = "ValidationLB";
            ValidationLB.Padding = new Padding(2);
            ValidationLB.Size = new Size(446, 24);
            ValidationLB.TabIndex = 13;
            ValidationLB.Text = "      Message";
            ValidationLB.TextAlign = ContentAlignment.MiddleLeft;
            ValidationLB.Visible = false;
            // 
            // StrictFilteringChB
            // 
            StrictFilteringChB.AutoSize = true;
            StrictFilteringChB.Font = new Font("Segoe UI", 11.25F);
            StrictFilteringChB.Location = new Point(127, 200);
            StrictFilteringChB.Name = "StrictFilteringChB";
            StrictFilteringChB.Padding = new Padding(5, 0, 0, 0);
            StrictFilteringChB.Size = new Size(125, 24);
            StrictFilteringChB.TabIndex = 4;
            StrictFilteringChB.Text = "Strict Filtering";
            toolTip1.SetToolTip(StrictFilteringChB, "Strict filetering means file tags have ot match precisely. Default of false is safest");
            StrictFilteringChB.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Segoe UI", 11.25F);
            label5.ForeColor = Color.Gray;
            label5.Location = new Point(5, 202);
            label5.Margin = new Padding(5);
            label5.Name = "label5";
            label5.Size = new Size(114, 20);
            label5.TabIndex = 8;
            label5.Text = "* required";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoSize = true;
            flowLayoutPanel2.Controls.Add(ProduceBT);
            flowLayoutPanel2.Controls.Add(OpenLogBT);
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
            ProduceBT.Click += ProduceBT_Click;
            // 
            // OpenLogBT
            // 
            OpenLogBT.BackColor = Color.FromArgb(64, 64, 64);
            OpenLogBT.FlatAppearance.BorderColor = Color.Gray;
            OpenLogBT.FlatStyle = FlatStyle.Flat;
            OpenLogBT.Image = Properties.Resources.document_list;
            OpenLogBT.ImageAlign = ContentAlignment.MiddleRight;
            OpenLogBT.Location = new Point(299, 5);
            OpenLogBT.Name = "OpenLogBT";
            OpenLogBT.Size = new Size(132, 31);
            OpenLogBT.TabIndex = 1;
            OpenLogBT.Text = " Open Log";
            OpenLogBT.TextAlign = ContentAlignment.MiddleLeft;
            OpenLogBT.TextImageRelation = TextImageRelation.ImageBeforeText;
            OpenLogBT.UseVisualStyleBackColor = false;
            OpenLogBT.Click += OpenLogBT_Click;
            // 
            // toolTip1
            // 
            toolTip1.BackColor = Color.FromArgb(64, 64, 64);
            toolTip1.ForeColor = Color.Ivory;
            toolTip1.OwnerDraw = true;
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
            FormClosing += HomeForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            MainTLP.ResumeLayout(false);
            MainTLP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MaxSizeNUM).EndInit();
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
        private CheckBox StrictFilteringChB;
        private FileSystemBrowser FilterPathTB;
        private FileSystemBrowser ExcludedPathTB;
        private FileSystemBrowser OutputPathTB;
        private Label label5;
        private FileSystemBrowser TorrentPathTB;
        private FlowLayoutPanel flowLayoutPanel2;
        private Button ProduceBT;
        private ToolTip toolTip1;
        private Label label6;
        private NumericUpDown MaxSizeNUM;
        private RichTextBox ConsoleRTB;
        private Label ValidationLB;
        private Button OpenLogBT;
    }
}