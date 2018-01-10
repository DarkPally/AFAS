using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Mvvm;
using Prism.Commands;

using AFAS.Library;

namespace AFAS.ViewModel
{
    public class VMAnalysis : VMBase
    {
        public VMAnalysis()
        {
           Path = System.Environment.CurrentDirectory + "\\SaveData\\";
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
            if (Path == null || Path == "")
            {
                State = "请输入数据文件地址";
                return;
            }

            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var fr=ForensicResult.LoadForensicResult(Path);
                    var fa = new ForensicAnalysis();
                    DataSource = fa.ExtractTagsWithWeight(fr);

                    State = "解析完成！";
                }
                catch(Exception e)
                {
                  State = "解析出现异常";
                  Error = e.Message;
                }

            });
        }

        public DelegateCommand DoWorkFromFile
        {
            get
            {
                return doWorkFromFile ?? (doWorkFromFile = new DelegateCommand(ExecuteWorkFromFile));
            }
        }

        DelegateCommand doWorkFromFile;

        public void ExecuteWorkFromFile()
        {
            if (Path == null || Path == "")
            {
                State = "请输入数据文件地址";
                return;
            }

            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var fa = new ForensicAnalysis();
                    DataSource = fa.ExtractTagsWithWeightFromTXTFile(Path);

                    State = "解析完成！";
                }
                catch(Exception e)
                {
                    State = "解析出现异常";
                    Error = e.Message;
                }

            });
        }
    }
}
