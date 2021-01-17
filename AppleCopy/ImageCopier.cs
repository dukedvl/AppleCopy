using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;

namespace AppleCopy
{
    //Decoupling this away from "MainViewModel", to reduce bloat
    public class ImageCopier : BaseViewModel
    {
        #region Properties

        /// <summary>
        /// Source directory containing Apple120,Apple121, etc.
        /// Usually easiest to direct-copy the total folder over, rather than dealing with non-OS devices/paths across C#
        /// </summary>
        public string SourceDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Destination directory, centralizing all pics/vids/etc. from the subfolders
        /// </summary>
        public string DestinationDirectory
        {
            get;
            set;
        }


        public bool SourceDirSelected
        {
            get => !SourceDirectory.IsNullOrEmpty();
        }


        private long sourceDirSize = 0;
        public long SourceDirectorySizeBytes
        {
            get
            {
                if (SourceDirSelected)
                {
                    //Initialize private storage variable, so we don't do this calculation alllll the time..
                    if (sourceDirSize == 0)
                    {
                        sourceDirSize = FileUtilities.GetDirectorySize(SourceDirectory);
                    }
                }

                return sourceDirSize;
            }
        }


        //"File Count: 2404
        private int sourceDirFileCount = 0;
        public int SourceDirFileCount
        {
            get
            {
                if (sourceDirFileCount == 0 && SourceDirSelected)
                {
                    sourceDirFileCount = FileUtilities.GetFileCount(SourceDirectory);
                }

                return sourceDirFileCount;
            }
        }

        // "Size: 3.24GB"
        public string SourceDirectorySizeString
        {
            get
            {
                if (SourceDirSelected)
                {
                    long DirectorySize = SourceDirectorySizeBytes;

                    double MegaByteSize = DirectorySize / 1E6;

                    if (MegaByteSize < 1024.0)
                    {
                        return $"{MegaByteSize:###0.0#} MB";
                    }
                    else
                    {
                        double GigaByteSize = MegaByteSize / 1E3;
                        return $"{GigaByteSize:####0.0#} GB";
                    }
                }
                else
                {
                    return "0.0 kB";
                }
            }
        }

        // "Estimated Time:  3h 15m 20s"
        public string EstimatedTransferTime
        {
            get
            {
                if (SourceDirSelected)
                {
                    //Transfer Rate ~ 33MB/s 
                    double timeInSeconds = SourceDirectorySizeBytes / 33E6;

                    TimeSpan transferEstimate = TimeSpan.FromSeconds(timeInSeconds);

                    return transferEstimate.GetCondensedTimeSpanString();
                }

                return "";
            }
        }

        /// <summary>
        /// True if mid-copy, false else
        /// </summary>
        public bool IsRunning
        {
            get;
            set;
        }

        /// <summary>
        /// Button text display, showing current status
        /// </summary>
        public string StatusString
        {
            get;
            set;
        } = "Copy";


        public bool TransferComplete
        {
            get;
            set;
        } = false;

        public int TotalFiles
        {
            get;
            set;
        }

        public int CompletedTransfers
        {
            get;
            set;
        }

        #endregion



        #region Methods
        /// <summary>
        /// Opens Folder Browse Dialog, selects Source Directory.  
        /// </summary>
        internal void SelectSourceDirCmd()
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                AllowNonFileSystemItems = true,
                EnsurePathExists = true,
                Multiselect = false,
                Title = "Select a source folder"
            };

            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SourceDirectory = openFileDialog.FileName;
            }

        }

        /// <summary>
        /// Opens FolderBrowseDialog, selects Destination Directory
        /// </summary>
        internal void SelectDestinationDirCmd()
        {
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.MyComputer,
                ShowNewFolderButton = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DestinationDirectory = openFileDialog.SelectedPath;
                OnPropertyChanged(nameof(DestinationDirectory));
            }

        }

        internal void Execute()
        {
            if (!TransferComplete)
            {

                StatusString = "Running..";

                var progress = new Progress<int>(report =>
                 {
                     CompletedTransfers = report;
                 });

                Task.Run(() =>
               {
                   IsRunning = true;

                   PerformTransfer(progress);

                   StatusString = "Open";
                   IsRunning = false;
                   TransferComplete = true;
               });
            }
            else
            {
                TransferComplete = false;

                Process.Start(DestinationDirectory);
            }

        }

        internal void PerformTransfer(IProgress<int> progressReporter)
        {
            CompletedTransfers = 0;

            TotalFiles = SourceDirFileCount;

            var filesToTransfer = FileUtilities.GetFilePaths(SourceDirectory);

            for (int fileCount = 0; fileCount < TotalFiles; fileCount++)
            {
                string TargetPath = Path.Combine(DestinationDirectory, Path.GetFileName(filesToTransfer[fileCount]));

                if (!File.Exists(TargetPath))
                {
                    File.Copy(filesToTransfer[fileCount], TargetPath);

                    progressReporter?.Report(fileCount);
                }
            }
        }

        #endregion

    }
}
