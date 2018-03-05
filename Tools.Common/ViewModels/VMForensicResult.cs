using System;
using DevExpress.Mvvm;
using AFAS.Library;

namespace Tools.Common.ViewModel
{
    public class VMForensicResult : ViewModelBase
    {

        object dataSource;
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    DataDisplay = value;
                    RaisePropertyChanged("DataSource");
                }
            }
        }

        object dataDisplay;
        public object DataDisplay
        {
            get { return dataDisplay; }
            set
            {
                if (dataDisplay != value)
                {
                    dataDisplay = value;
                    RaisePropertyChanged("DataDisplay");
                }
            }
        }

        public DelegateCommand RestoreDataDisplay
        {
            get
            {
                return restoreDataDisplay ?? (restoreDataDisplay = new DelegateCommand(ExecuteRestoreDataDisplay));
            }
        }

        DelegateCommand restoreDataDisplay;

        public void ExecuteRestoreDataDisplay()
        {
            DataDisplay = DataSource;
        }

        public DelegateCommand ResultExport
        {
            get
            {
                return resultExport ?? (resultExport = new DelegateCommand(ExecuteResultExport));
            }
        }

        DelegateCommand resultExport;

        public void ExecuteResultExport()
        {
            var dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.InitialDirectory = System.Environment.CurrentDirectory + "\\SaveData\\";
            dialog.FileName = DateTime.Now.ToString("yyMMdd_HHmm") + ".txt";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ForensicResult.SaveForensicResult(DataSource as ForensicResult, dialog.FileName);
            }

        }

        public DelegateCommand ResultImport
        {
            get
            {
                return resultImport ?? (resultImport = new DelegateCommand(ExecuteResultImport));
            }
        }

        DelegateCommand resultImport;

        public void ExecuteResultImport()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = System.Environment.CurrentDirectory + "\\SaveData\\";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataSource = ForensicResult.LoadForensicResult(dialog.FileName);
            }
        }

        ForensicResultItem selectedItem;
        public ForensicResultItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    RaisePropertyChanged("State");
                }
            }
        }
    }
}