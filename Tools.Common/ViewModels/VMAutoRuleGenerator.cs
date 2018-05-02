using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using AFAS.Library;
using AFAS.Library.Android;
using AFAS.Library.AutoRule;

namespace Tools.Common.ViewModel
{
    public class VMAutoRuleGenerator : ViewModelBase
    {
        string LocalPCPath { get; set; }

        public string TargetRootPath { get; set; }
        public string RuleKey { get; set; }

        string keyWord;
        public string KeyWord
        {
            get { return keyWord; }
            set
            {
                if (keyWord != value)
                {
                    keyWord = value;
                    RaisePropertyChanged("KeyWord");
                }
            }
        }

        string result;
        public string Result
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    result = value;
                    RaisePropertyChanged("Result");
                }
            }
        }

        public List<string> FilePaths { get; set; }

        public AndroidFileExtracter androidFileExtracter { get; set; }
        public string androidDevice { get; set; }

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


        public DelegateCommand DoWork
        {
            get
            {
                return doWork ?? (doWork = new DelegateCommand(ExecuteWork));
            }
        }
        DelegateCommand doWork;

        public void ExecuteWork()
        {
            State = "解析中...";
            LocalPCPath = VMMain.Instance.VMConfig.WorkPath;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var manager = new AutoRuleManager()
                    {
                        androidDevice = androidDevice,
                        androidFileExtracter = androidFileExtracter,
                        LocalPCRoot = LocalPCPath,
                        RootPath = TargetRootPath,
                        Key = RuleKey,
                        KeyWord = KeyWord,
                    };
                    var res=manager.GenerateRule(FilePaths);
                    Result = manager.ConvertRuleToJson(res);

                    State = "解析完成！";
                }
                catch (Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
            
        }
    }
}
