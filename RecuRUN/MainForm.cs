// <copyright file="MainForm.cs" company="PublicDomainDaily.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace RecuRUN
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        // Target directory path
        string directoryPath = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RecuRUN.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();
        }

        /// <summary>
        /// Ons the browse for folder button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnBrowseForFolderButtonClick(object sender, EventArgs e)
        {
            // Show folder browser dialog
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK && this.folderBrowserDialog.SelectedPath.Length > 0)
            {
                // Set target directory path
                this.directoryPath = this.folderBrowserDialog.SelectedPath;

                // Enable form controls
                this.executeRecursivelyButton.Enabled = true;
                this.create0ByteFilesButton.Enabled = true;
                this.commandLabel.Enabled = true;
                this.commandTextBox.Enabled = true;
                this.fileNameLabel.Enabled = true;
                this.fileNameTextBox.Enabled = true;

                // Focus command text box
                this.commandTextBox.Focus();
            }
        }

        /// <summary>
        /// Ons the execute recursively button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExecuteRecursivelyButtonClick(object sender, EventArgs e)
        {
            // Declare processed count 
            int processedCount = 0;

            // Iterate subdirectories
            foreach (string currentDirectoryPath in Directory.EnumerateDirectories(this.folderBrowserDialog.SelectedPath, "*", SearchOption.AllDirectories))
            {
                // Set batch file path
                string batchFilePath = Path.Combine(currentDirectoryPath, $"RecuRUN-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.bat");

                // Error handling & logging
                try
                {
                    // Write file
                    File.WriteAllText(batchFilePath, this.commandTextBox.Text);

                    // Set batch process
                    Process batchProcess = new Process();

                    // Set batch start info
                    ProcessStartInfo batchProcessStartInfo = new ProcessStartInfo(batchFilePath)
                    {
                        // Set start info values
                        WorkingDirectory = currentDirectoryPath,
                        WindowStyle = this.showConsoleToolStripMenuItem.Checked ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = true
                    };

                    // Assign process start info
                    batchProcess.StartInfo = batchProcessStartInfo;

                    // Start the process
                    batchProcess.Start();

                    // Let it run
                    batchProcess.WaitForExit();

                    // Remove batch file from disk
                    File.Delete(batchFilePath);

                    // Raise count
                    processedCount++;
                }
                catch (Exception ex)
                {
                    // Log error event
                    MessageBox.Show($"Error processing file:{Environment.NewLine}{batchFilePath}{Environment.NewLine}Message: {ex.Message}", "Aborting on error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Abort operation by exit
                    return;
                }
            }

            // Set processed count
            this.countToolStripStatusLabel.Text = (this.resetCountToolStripMenuItem.Checked ? processedCount : int.Parse(this.countToolStripStatusLabel.Text) + processedCount).ToString();

            // Advise user
            MessageBox.Show($"Successfully processed {processedCount} batch files.", $"Recursive batch", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Ons the create0 byte files button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCreate0ByteFilesButtonClick(object sender, EventArgs e)
        {
            // Check for a file name
            if (this.fileNameTextBox.TextLength == 0)
            {
                // Advise user
                MessageBox.Show("Please enter file name.", "File name missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Focus file name text box
                this.fileNameTextBox.Focus();

                // Halt flow
                return;
            }

            // Declare processed count 
            int processedCount = 0;

            // Iterate subdirectories
            foreach (string currentDirectoryPath in Directory.EnumerateDirectories(this.folderBrowserDialog.SelectedPath, "*", SearchOption.AllDirectories))
            {
                // Set zero byte file path
                string zeroByteFilePath = Path.Combine(currentDirectoryPath, this.fileNameTextBox.Text);

                // Error handling & logging
                try
                {
                    // Write file
                    File.Create(zeroByteFilePath);

                    // Raise count
                    processedCount++;
                }
                catch (Exception ex)
                {
                    // Log error event
                    MessageBox.Show($"Error creating file:{Environment.NewLine}{zeroByteFilePath}{Environment.NewLine}Message: {ex.Message}", "Aborting on error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Abort operation by exit
                    return;
                }
            }

            // Set processed count
            this.countToolStripStatusLabel.Text = (this.resetCountToolStripMenuItem.Checked ? processedCount : int.Parse(this.countToolStripStatusLabel.Text) + processedCount).ToString();

            // Advise user
            MessageBox.Show($"Successfully created {processedCount} zero-byte files.", $"Files created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Ons the exit tool strip menu item1 click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItem1Click(object sender, EventArgs e)
        {
            // Close program
            this.Close();
        }

        /// <summary>
        /// Ons the options tool strip menu item drop down item clicked.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOptionsToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Set tool strip menu item
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)e.ClickedItem;

            // Toggle checked
            toolStripMenuItem.Checked = !toolStripMenuItem.Checked;

            // Set topmost by check box
            this.TopMost = this.alwaysOnTopToolStripMenuItem.Checked;
        }

        /// <summary>
        /// Ons the daily releases public domain dailycom tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnDailyReleasesPublicDomainDailycomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open daily releases website
            Process.Start("https://publicdomaindaily.com");
        }

        /// <summary>
        /// Ons the original thread donation codercom tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open original forum thread
            Process.Start("https://www.donationcoder.com/forum/index.php?topic=51436.0");
        }

        /// <summary>
        /// Ons the source code githubcom tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open source code repository
            Process.Start("https://github.com/publicdomain/recurun");
        }

        /// <summary>
        /// Ons the about tool strip menu item click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code
        }
    }
}
