using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using AFAS.Library;
using AFAS.Library.Android;
using System.Collections.ObjectModel;
namespace Tools.Common.ViewModel
{
    public class FileSearchItem
    {
        public bool IsSelected { get; set; }
        public string Path { get; set; }
    }
    public class VMAndroidFileSearcher : ViewModelBase
    {
        AndroidFileExtracter androidFileExtracter = null;
        string androidDevice;
        public string ObserverPath { get; set; }
        public string SearchWord { get; set; }
        object dataSource;
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    RaisePropertyChanged("DataSource");
                }
            }
        }

        string state = "准备就绪";
        public string State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    RaisePropertyChanged("State");
                }
            }
        }

        string error;
        public string Error
        {
            get { return error; }
            set
            {
                if (error != value)
                {
                    error = value;
                    RaisePropertyChanged("Error");
                }
            }
        }

        void InitAndroidFileExtractor()
        {            
            if (androidFileExtracter == null)
            {
                androidFileExtracter = new AndroidFileExtracter();
                androidFileExtracter.InitConnection();
                androidDevice = androidFileExtracter.Devices.First();
            }
        }

        public DelegateCommand SearchAndroidContent
        {
            get
            {
                return searchAndroidContent ?? (searchAndroidContent = new DelegateCommand(ExecuteSearchAndroidContent));
            }
        }

        DelegateCommand searchAndroidContent;

        public void ExecuteSearchAndroidContent()
        {
            State = "解析中...";

            Task.Factory.StartNew(() =>
            {
                try
                {
                    InitAndroidFileExtractor();
                    var t = androidFileExtracter.GrepFiles(androidDevice, ObserverPath, SearchWord);
                    
                    DataSource =
                        new ObservableCollection<FileSearchItem>(
                        t.FilePropertys.Select(c=>new FileSearchItem() { Path=c.Path}));
                    State = "解析完成！";
                }
                catch (Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
            
        }

        public DelegateCommand PrepareRuleGenerate
        {
            get
            {
                return prepareRuleGenerate ?? (prepareRuleGenerate = new DelegateCommand(ExecutePrepareRuleGenerate));
            }
        }

        DelegateCommand prepareRuleGenerate;

        public void ExecutePrepareRuleGenerate()
        {
            if(DataSource != null)
            {
                var t = DataSource as ObservableCollection<FileSearchItem>;
                var src = t.Where(c => c.IsSelected).Select(c => c.Path).ToList();
                VMMain.Instance.VMAutoRuleGenerator.FilePaths = src;
            }
            
            VMMain.Instance.VMAutoRuleGenerator.KeyWord = SearchWord;
            VMMain.Instance.VMAutoRuleGenerator.androidDevice = androidDevice;
            VMMain.Instance.VMAutoRuleGenerator.androidFileExtracter = androidFileExtracter;
            if (ObserverPath != null)
            {
                if (ObserverPath.Last() != '\\') ObserverPath += '\\';
                VMMain.Instance.VMAutoRuleGenerator.TargetRootPath = ObserverPath.Replace('\\', '/');
            }
            else
            {
                VMMain.Instance.VMAutoRuleGenerator.TargetRootPath = "";
            }
        }
    }
}
