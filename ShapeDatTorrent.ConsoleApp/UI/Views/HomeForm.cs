using ShapeDatTorrent.ConsoleApp.UI.Helpers;
using ShapeDatTorrent.ConsoleApp.UI.Models;
using ShapeDatTorrent.Core.DTOs;
using ShapeDatTorrent.Core.Engines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShapeDatTorrent.ConsoleApp.UI.Views
{
    public partial class HomeForm : Form
    {

        private const string validationLabelSpacing = "     ";

        public HomeForm()
        {
            InitializeComponent();

            TorrentPathTB.DataBindings.Add("Path", Properties.Settings.Default, 
                nameof(Properties.Settings.Default.TorrentFilepath), true, DataSourceUpdateMode.OnPropertyChanged);

            FilterPathTB.DataBindings.Add("Path", Properties.Settings.Default,
                nameof(Properties.Settings.Default.FilterDatPath), true, DataSourceUpdateMode.OnPropertyChanged);

            ExcludedPathTB.DataBindings.Add("Path", Properties.Settings.Default,
                nameof(Properties.Settings.Default.ExcludedDatPath), true, DataSourceUpdateMode.OnPropertyChanged);

            OutputPathTB.DataBindings.Add("Path", Properties.Settings.Default,
                nameof(Properties.Settings.Default.OutputFolder), true, DataSourceUpdateMode.OnPropertyChanged);

            MaxSizeNUM.DataBindings.Add("Value", Properties.Settings.Default,
                    nameof(Properties.Settings.Default.MaxSizeValue), true, DataSourceUpdateMode.OnPropertyChanged);

            StrictFilteringChB.DataBindings.Add("Checked", Properties.Settings.Default,
                nameof(Properties.Settings.Default.StrictMode), true, DataSourceUpdateMode.OnPropertyChanged);

            if (Properties.Settings.Default.WindowSize.Width != 0 && Properties.Settings.Default.WindowSize.Height != 0)
                this.Size = Properties.Settings.Default.WindowSize;

            toolTip1.Popup += ToolTip1_Popup;
            toolTip1.Draw += ToolTip1_Draw;
        }

        private const int MAX_TOOLTIP_WIDTH = 300;
        private const int PADDING = 8;

        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {
            using (Font customFont = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                // Tell GDI how much horizontal room we have for wrapping
                Size proposedSize = new Size(MAX_TOOLTIP_WIDTH - (PADDING * 2), int.MaxValue);

                TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;

                // Measure exactly how tall the text needs to be when wrapped
                Size textSize = TextRenderer.MeasureText(toolTip1.GetToolTip(e.AssociatedControl), customFont, proposedSize, flags);

                // Set the final box size including our padding
                e.ToolTipSize = new Size(
                    Math.Min(MAX_TOOLTIP_WIDTH, textSize.Width + (PADDING * 2)),
                    textSize.Height + (PADDING * 2)
                );
            }
        }

        private void ToolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            // 1. Clean Dark Background
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(32, 32, 32)))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            // 2. Subtle Neon Border
            using (Pen borderPen = new Pen(Color.FromArgb(128,128,128), 1))
            {
                Rectangle borderRect = e.Bounds;
                borderRect.Width -= 1;
                borderRect.Height -= 1;
                e.Graphics.DrawRectangle(borderPen, borderRect);
            }

            // 3. Crisp, Wrapped Text
            using (Font customFont = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                // Crucial flags for multiline wrapping
                TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.NoPadding;

                // Deflate bounds so text sits nicely inside the padding zone
                Rectangle textRect = e.Bounds;
                textRect.Inflate(-PADDING, -PADDING);

                TextRenderer.DrawText(e.Graphics, e.ToolTipText, customFont, textRect, Color.Gainsboro, flags);
            }
        }


        private async void ProduceBT_Click(object sender, EventArgs e)
        {
            await ProcessRequest();
        }

        private bool FormIsValid()
        {
            bool isValid = false;

            if (String.IsNullOrEmpty(TorrentPathTB.Path))
            {
                ShowValidationText(ValidationMessageType.Error, "Torrent cannot be null");
            }

            else if (!File.Exists(TorrentPathTB.Path))
            {
                ShowValidationText(ValidationMessageType.Error, "Torrent file does not exist");

            }

            else if (!String.IsNullOrEmpty(FilterPathTB.Path) && !File.Exists(FilterPathTB.Path))
            {
                ShowValidationText(ValidationMessageType.Error, "Filter .dat file does not exist");

            }

            else if (!String.IsNullOrEmpty(ExcludedPathTB.Path) && !File.Exists(ExcludedPathTB.Path))
            {
                ShowValidationText(ValidationMessageType.Error, "Excluded .dat file does not exist");

            }

            else if (!String.IsNullOrEmpty(OutputPathTB.Path) && !Directory.Exists(OutputPath()))
            {
                ShowValidationText(ValidationMessageType.Error, "Output folder does not exist");

            }
            else
            {
                isValid = true;
                ValidationLB.Visible = false;
                ConsoleRTB.Visible = true;
            }

            return isValid;
        }



        private void ShowValidationText(ValidationMessageType type, string message, bool supressVisibilityChange = false)
        {
            ValidationLB.Image = Models.ValidationMessageData.ValidationImages[type];
            ValidationLB.Text = $"{validationLabelSpacing}{message}";
            ValidationLB.Visible = true;

            if (supressVisibilityChange) return;

            ConsoleRTB.Visible = false;
        }

        private string OutputPath()
        {
            return string.IsNullOrWhiteSpace(OutputPathTB.Path)
                                     ? AppDomain.CurrentDomain.BaseDirectory
                                     : OutputPathTB.Path;
        }


        private async Task ProcessRequest()
        {
            if (!FormIsValid()) return;


            // Determine the output path, defaulting to the application folder if empty
            string finalOutputPath = OutputPath();

            var request = new ProcessingRequest
            {
                TorrentPath = TorrentPathTB.Path,
                KeptDatPath = FilterPathTB.Path,
                RemovedDatPath = ExcludedPathTB.Path,
                OutputDir = finalOutputPath,
                TargetBytes = (long)MaxSizeNUM.Value * 1024L * 1024L * 1024L,
                Strict = StrictFilteringChB.Checked
            };

            // 2. Disable UI so user doesn't double-click
            ProduceBT.Enabled = false;

            await Task.Run(() =>
            {
                var chunker = new TorrentChunker();

                // Subscribe to the log message
                chunker.OnLogMessage += (msg, color) =>
                {
                    UpdateRichTextBox(msg, color);
                };

                // This blocks the background thread, NOT the UI
                chunker.Process(request);
            });

            // 4. Re-enable UI when done
            ProduceBT.Enabled = true;
            UpdateRichTextBox("(above markdown compliant for checkboxes)", ConsoleColor.Gray);
            UpdateRichTextBox("Processing complete!\n", ConsoleColor.Green);

            ShowValidationText(ValidationMessageType.Success, $"New .torrent files produced.", true);


        }

        // Helper: Thread-safe UI update
        private void UpdateRichTextBox(string message, ConsoleColor color)
        {
            // If the call is from a background thread, marshal it to the UI thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateRichTextBox(message, color)));
                return;
            }

            // Now we are safely on the UI thread
            ConsoleRTB.SelectionColor = ColorHelpers.MapConsoleColor(color);
            ConsoleRTB.AppendText(message + Environment.NewLine);
            ConsoleRTB.ScrollToCaret();
        }

        private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowSize = this.Size;
            Properties.Settings.Default.Save();
        }

        private void OpenLogBT_Click(object sender, EventArgs e)
        {
            string logPath = Path.Combine(OutputPath(), "ShapeDatTorrent.log");
            if (File.Exists(logPath))
            {
                ValidationLB.Visible = false;
                // Opens the file using the default Windows viewer (e.g., Notepad)
                Process.Start(new ProcessStartInfo(logPath) { UseShellExecute = true });
                ShowValidationText(ValidationMessageType.Success, $"Log file opened.", true);

            }
            else
            {
                string shortLogPath = Path.Combine(Path.GetFileName(Path.GetDirectoryName(logPath)), Path.GetFileName(logPath));
                ShowValidationText(ValidationMessageType.Warning, $"Could not open log file: ...\\{shortLogPath}", true);

            }
        }
    }
}
