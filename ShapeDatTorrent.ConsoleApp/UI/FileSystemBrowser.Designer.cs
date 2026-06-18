namespace ShapeDatTorrent.ConsoleApp.UI
{
    partial class FileSystemBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BrowseBT = new Button();
            PathTB = new TextBox();
            ClearBT = new Button();
            SuspendLayout();
            // 
            // BrowseBT
            // 
            BrowseBT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BrowseBT.BackColor = Color.FromArgb(64, 64, 64);
            BrowseBT.BackgroundImage = Properties.Resources.folder_horizontal_open;
            BrowseBT.BackgroundImageLayout = ImageLayout.Center;
            BrowseBT.FlatAppearance.BorderColor = Color.Gray;
            BrowseBT.FlatStyle = FlatStyle.Flat;
            BrowseBT.Location = new Point(293, 2);
            BrowseBT.Name = "BrowseBT";
            BrowseBT.Size = new Size(40, 25);
            BrowseBT.TabIndex = 0;
            BrowseBT.UseVisualStyleBackColor = false;
            BrowseBT.Click += BrowseBT_Click;
            // 
            // PathTB
            // 
            PathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PathTB.BackColor = Color.FromArgb(48, 48, 48);
            PathTB.BorderStyle = BorderStyle.FixedSingle;
            PathTB.ForeColor = Color.Gainsboro;
            PathTB.Location = new Point(3, 2);
            PathTB.Name = "PathTB";
            PathTB.ShortcutsEnabled = false;
            PathTB.Size = new Size(284, 25);
            PathTB.TabIndex = 1;
            // 
            // ClearBT
            // 
            ClearBT.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ClearBT.BackColor = Color.FromArgb(64, 64, 64);
            ClearBT.BackgroundImage = Properties.Resources.cross;
            ClearBT.BackgroundImageLayout = ImageLayout.Center;
            ClearBT.FlatAppearance.BorderColor = Color.Gray;
            ClearBT.FlatStyle = FlatStyle.Flat;
            ClearBT.Location = new Point(339, 2);
            ClearBT.Name = "ClearBT";
            ClearBT.Size = new Size(26, 25);
            ClearBT.TabIndex = 2;
            ClearBT.UseVisualStyleBackColor = false;
            ClearBT.Click += ClearBT_Click;
            // 
            // FileSystemBrowser
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(ClearBT);
            Controls.Add(PathTB);
            Controls.Add(BrowseBT);
            Margin = new Padding(2);
            Name = "FileSystemBrowser";
            Size = new Size(368, 32);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BrowseBT;
        private TextBox PathTB;
        private Button ClearBT;
    }
}
