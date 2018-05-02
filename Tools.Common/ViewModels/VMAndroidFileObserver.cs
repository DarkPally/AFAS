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
    public class FilePropertyItem
    {
        public bool IsSelected { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Size { get; set; }
        public string AccessTime { get; set; }
        public string ModifyTime { get; set; }
        public FileOBState OBState { get; set; }

        public static FilePropertyItem Load(FilePropertyOB fp)
        {
            return new FilePropertyItem()
            {
                Size = fp.Size,
                AccessTime = fp.AccessTime,
                FilePath = fp.Path,
                ModifyTime = fp.ModifyTime,
                OBState = fp.OBState,
                IsSelected = (fp.OBState == FileOBState.Create || fp.OBState == FileOBState.Changed),
                FileName = fp.Path.Split('/').Last()
            };
        }
    }

    public class VMAndroidFileObserver : ViewModelBase
    {
        public AndroidFileObserver Observer { get; set; }
        AndroidFileExtracter androidFileExtracter = null;
        string androidDevice;
        public string ObserverPath { get; set; }

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

        public DelegateCommand InitObserver
        {
            get
            {
                return initObserver ?? (initObserver = new DelegateCommand(ExecuteInitObserver));
            }
        }

        DelegateCommand initObserver;

        public void ExecuteInitObserver()
        {
            State = "解析中...";

            Task.Factory.StartNew(() =>
            {
                try
                {
                    InitAndroidFileExtractor();

                    Observer = new AndroidFileObserver()
                    {
                        androidFileExtracter = androidFileExtracter,
                        androidDevice = androidDevice,
                        ObservePath = ObserverPath
                    };
                    Observer.Init();
                    DataSource = new ObservableCollection<FilePropertyItem>(
                        Observer.NewFileProperties.Select(c => FilePropertyItem.Load(c)));

                    State = "解析完成！";
                }
                catch (Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
            
        }

        public DelegateCommand UpdateObsever
        {
            get
            {
                return updateObsever ?? (updateObsever = new DelegateCommand(ExecuteUpdateObsever));
            }
        }

        DelegateCommand updateObsever;

        public void ExecuteUpdateObsever()
        {
            State = "解析中...";

            Task.Factory.StartNew(() =>
            {
                try
                {

                    Observer.Update();
                    DataSource = new ObservableCollection<FilePropertyItem>(
                        Observer.NewFileProperties.Select(c => FilePropertyItem.Load(c)));

                    State = "解析完成！";
                }
                catch (Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
        }

        public DelegateCommand ReplaceObseverOld
        {
            get
            {
                return replaceObseverOld ?? (replaceObseverOld = new DelegateCommand(ExecuteReplaceObseverOld));
            }
        }

        DelegateCommand replaceObseverOld;

        public void ExecuteReplaceObseverOld()
        {
            Observer.ReplaceOldOB();
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
            if (DataSource != null)
            {
                var t = DataSource as ObservableCollection<FilePropertyItem>;
                var src = t.Where(c => c.IsSelected).Select(c => c.FilePath).ToList();
                VMMain.Instance.VMAutoRuleGenerator.FilePaths = src;
            }            
            VMMain.Instance.VMAutoRuleGenerator.KeyWord = "";
            VMMain.Instance.VMAutoRuleGenerator.androidDevice = androidDevice;
            VMMain.Instance.VMAutoRuleGenerator.androidFileExtracter = androidFileExtracter;
            if (ObserverPath!=null)
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
