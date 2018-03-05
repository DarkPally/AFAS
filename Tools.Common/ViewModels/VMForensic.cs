using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Mvvm;

using AFAS.Library;

namespace Tools.Common.ViewModel
{
    public class VMForensic : ViewModelBase
    {
        public List<string> SourceTypes { get; } = new List<string>()
        { "从工作目录提取", "从ADB提取" };
        public string SelectedSourceType { get; set; } = "从工作目录提取";

        string path = System.Environment.CurrentDirectory + "\\temp\\";

        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged("Path");
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
            VMMain.Instance.State = "解析中...";

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (SelectedSourceType == "从工作目录提取")
                    {
                        doWorkFromPC();
                    }
                    else
                    {
                        doWorkFromADB();
                    }

                    VMMain.Instance.State = "解析完成！";
                }
                catch(Exception e)
                {
                    VMMain.Instance.State = "解析出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }

        void doWorkFromPC()
        {
            if (Path == null || Path == "")
            {
                VMMain.Instance.State = "工作目录为空，无法解析文件夹";
                return;
            }
            if (Path.Last() != '\\') Path += '\\';
            VMMain.Instance.VMForensicResult.DataSource = AFASManager.Instance.DoForensic(Path, true);
        }


        void doWorkFromADB()
        {
            VMMain.Instance.VMForensicResult.DataSource = AFASManager.Instance.DoForensic(Path, false);
        }


    }
}
