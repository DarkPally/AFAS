using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Mvvm;
using AFAS.Library;

namespace Tools.Common.ViewModel
{
    public class VMTimeAnalysis : ViewModelBase
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
                    RaisePropertyChanged("DataSource");
                }
            }
        }

        public DateTime RangeStart { get; set; }
        public DateTime RangeEnd { get; set; }


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
            
            VMMain.Instance.State = "时间分析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {

                    var fr= VMMain.Instance.VMForensicResult.DataSource as ForensicResult;
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
                    VMMain.Instance.State = "时间分析完成！";
                }
                catch(Exception e)
                {
                    VMMain.Instance.State = "时间分析出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }

        public DelegateCommand SelectDataByTime
        {
            get
            {
                return selectDataByTime ?? (selectDataByTime = new DelegateCommand(ExecuteSelectDataByTime));
            }
        }

        DelegateCommand selectDataByTime;

        public void ExecuteSelectDataByTime()
        {

            VMMain.Instance.State = "时间分析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {

                    var ds = VMMain.Instance.VMForensicResult.DataSource as ForensicResult;
                    var items = ds.GetItemWithMark("Time");
                    var newItems = new HashSet<ForensicResultItem>();

                    foreach (var it in items)
                    {
                        var timeCol = it.MarkInfo.ColumnDescs.First(c => c.Mark == "Time");
                        var timeColName = timeCol.Desc == null ? timeCol.Name : timeCol.Desc;
                        if (it.Table != null)
                        {
                            DataView dv = it.Table.DefaultView;
                            dv.RowFilter = String.Format("Convert({0}, 'System.DateTime') <= '{1}' And Convert({0}, 'System.DateTime') >='{2}'",
                                                    timeColName, RangeEnd, RangeStart);

                            DataTable newTable = dv.ToTable();
                            if (newTable.Rows.Count > 0)
                            {
                                newItems.Add(new ForensicResultItem()
                                {
                                    Desc=it.Desc,
                                    Table=newTable,
                                    MarkInfo= it.MarkInfo,
                                    ParentData=it,
                                    ParentItemNode= it.ParentItemNode,
                                });
                            }
                        }
                    }


                    bool flag = true;
                    Dictionary<ForensicResultItem, ForensicResultItem> buffer = new Dictionary<ForensicResultItem, ForensicResultItem>();
                    while(flag)
                    {
                        HashSet<ForensicResultItem> nextItems = new HashSet<ForensicResultItem>();
                        flag = false;
                        foreach (var it in newItems)
                        {
                            if (it.ParentItemNode != null)
                            {
                                flag = true;
                                if(it.ParentData!=null)
                                {
                                    if (!buffer.ContainsKey(it.ParentData))
                                    {
                                        buffer.Add(it.ParentData, it);
                                    }
                                    else
                                    {
                                        buffer[it.ParentData] = it;
                                    }
                                }
                               

                                if(!buffer.ContainsKey(it.ParentItemNode))
                                {
                                    buffer.Add(it.ParentItemNode, new ForensicResultItem()
                                    {
                                        Desc = it.ParentItemNode.Desc,
                                        ParentItemNode= it.ParentItemNode.ParentItemNode,
                                    });
                                }
                                it.ParentItemNode = buffer[it.ParentItemNode];
                                it.ParentItemNode.Children.Add(it);
                                
                                if(!nextItems.Contains(it.ParentItemNode))
                                {
                                    nextItems.Add(it.ParentItemNode);
                                }
                            }
                        }
                        if(flag)
                        {

                            newItems = nextItems;
                        }
                    }
                    VMMain.Instance.VMForensicResult.DataDisplay = new ForensicResult()
                    {
                        Items = newItems.ToList(),
                    };
                    VMMain.Instance.State = "时间分析完成！";
                    
                }
                catch (Exception e)
                {
                    VMMain.Instance.State = "时间分析出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }

    }
}
