using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace EUpdate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // list with files indicating a possible valid setup
        private List<string> validNames = new List<string>
        {
            "eagle.exe",
            "eaglecon.exe"
        };
        ConcurrentBag<string> eagleFiles = new ConcurrentBag<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // get the EAGLE install location from user input
        // TODO: research more precise ways of detecting an EAGLE install
        private void browseB_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                string eagleDir;
                // if the user selected a folder
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // get the files in that folder
                    eagleDir = dialog.SelectedPath;
                    string[] fileEntries = Directory.GetFileSystemEntries(eagleDir);
                    foreach (string fileEntry in fileEntries)
                    {
                        Debug.WriteLine("Processing file " + fileEntry);
                        // check if EAGLE is installed there
                        if (validNames.Any(x => fileEntry.Contains(x)))
                        {
                            // if yes proceed
                            locationTB.Text = eagleDir;
                            return;
                        }
                    }
                    // if no show error and abort
                    MessageBox.Show(this, "No valid EAGLE install is detected in the location provided.",
                        "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void scanB_Click(object sender, RoutedEventArgs e)
        {
            // get all logical drives and search all of them
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                SearchForEagle(drive.Name);
            }
        }

        // searches for an EAGLE install on a logical drive
        private void SearchForEagle(string driveName)
        {
            DirectoryInfo drInfo = new DirectoryInfo(driveName);
            WalkDirectoryTree(drInfo,"eagle");
            foreach (string file in eagleFiles)
            {
                Debug.WriteLine("Scan found " + file);
            }
        }


        // credit to this solution
        // https://codereview.stackexchange.com/questions/74156/fastest-way-searching-specific-files
        private void WalkDirectoryTree(DirectoryInfo dr, string searchname)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            try
            {
                files = dr.GetFiles(searchname + "*.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    eagleFiles.Add(fi.Name);
                }
                subDirs = dr.GetDirectories();
                Parallel.ForEach(subDirs, dir => WalkDirectoryTree(dir, searchname));
            }
        }
    }
}
