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
    public class VMTimeAnalysis : VMBase
    {
        public VMForensic VMForensic { get; set; }
        public class MyPoint
        {
            public DateTime X { get; set; }

            public int Y { get; set; }
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
            Task.Factory.StartNew(() =>
            {
                try
                {

                    var fr=VMForensic.DataSource as ForensicResult;
                    var items=fr.GetItemWithMark("Time");
                    List<object> times=new List<object>();
                    foreach(var it in items)
                    {
                        times.AddRange(it.GetColumnDataByMark("Time"));
                    }
                    Dictionary<DateTime, int> timeCounter=new Dictionary<DateTime, int>();
                    foreach(var it in times)
                    {
                        var t = (DateTime.Parse(it as string)).Date;
                        if (timeCounter.ContainsKey(t)) timeCounter[t]++;
                        else timeCounter.Add(t, 1);
                    }
                    DataSource = timeCounter.Select(c => new MyPoint() { X = c.Key, Y = c.Value }).OrderBy(c => c.X);
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
