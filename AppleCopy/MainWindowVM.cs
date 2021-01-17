using Prism.Commands;
using System.Windows.Input;

namespace AppleCopy
{
    public class MainWindowVM : BaseViewModel
    {
        public ImageCopier CopyProcess
        {
            get;
            set;
        } = new ImageCopier();

        #region ICommands
        private ICommand _selectSourceDirCmd;
        public ICommand SelectSourceDirectoryCmd
        {
            get
            {
                return _selectSourceDirCmd ?? (_selectSourceDirCmd = new DelegateCommand(() => CopyProcess.SelectSourceDirCmd()));
            }
        }

        private ICommand _selectDestinationDirCmd;
        public ICommand SelectDestinationDirectoryCmd
        {
            get
            {
                return _selectDestinationDirCmd ?? (_selectDestinationDirCmd = new DelegateCommand(() => CopyProcess.SelectDestinationDirCmd()));
            }
        }

        private ICommand _copyFilesCmd;
        public ICommand CopyFilesCmd
        {
            get
            {
                return _copyFilesCmd ?? (_copyFilesCmd = new DelegateCommand(() => CopyProcess.Execute()));
            }
        }

        #endregion



    }
}
