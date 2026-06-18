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
            BrowseBT.Location = new Point(317, 3);
            BrowseBT.Name = "BrowseBT";
            BrowseBT.Size = new Size(48, 25);
            BrowseBT.TabIndex = 0;
            BrowseBT.UseVisualStyleBackColor = false;
            BrowseBT.Click += BrowseBT_Click;
            // 
            // PathTB
            // 
            PathTB.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PathTB.BackColor = Color.FromArgb(48, 48, 48);
            PathTB.BorderStyle = BorderStyle.FixedSingle;
            PathTB.Location = new Point(3, 3);
            PathTB.Name = "PathTB";
            PathTB.ReadOnly = true;
            PathTB.Size = new Size(308, 25);
            PathTB.TabIndex = 1;
            // 
            // FileSystemBrowser
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Transparent;
            Controls.Add(PathTB);
            Controls.Add(BrowseBT);
            Margin = new Padding(2);
            Name = "FileSystemBrowser";
            Size = new Size(368, 31);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BrowseBT;
        private TextBox PathTB;
    }
}
