using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Mvvm;
using AFAS.Library;

namespace ForensicAnlysisTool.ViewModel
{
    public class VMTagAnalysis : ViewModelBase
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

        public int KeyWordCount { get; set; } = 10;
        public List<ForensicAnalysis.ExtractAlgorithm> Algorithms { get; } = new List<ForensicAnalysis.ExtractAlgorithm>()
        { ForensicAnalysis.ExtractAlgorithm.TextRank, ForensicAnalysis.ExtractAlgorithm.TF_IDF };
        public ForensicAnalysis.ExtractAlgorithm SelectedAlgorithm { get; set; } = ForensicAnalysis.ExtractAlgorithm.TextRank;

        ForensicAnalysis forensicAnalysis = new ForensicAnalysis();
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
            VMMain.Instance.State = "关键词分析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var fr=VMMain.Instance.VMForensicResult.DataSource as ForensicResult;
                    fr.Items.ForEach(c =>
                    {
                        c.Children.RemoveAll(s => s.Desc == "数据集合");
                    });
                    forensicAnalysis.KeywordCount = KeyWordCount;
                    forensicAnalysis.Algorithm = SelectedAlgorithm;
                    DataSource = forensicAnalysis.ExtractTagsWithWeight(fr);

                    VMMain.Instance.State = "关键词分析完成！";
                }
                catch(Exception e)
                {
                    VMMain.Instance.State = "关键词分析出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }
        public DelegateCommand DoWorkFromSelect
        {
            get
            {
                return doWorkFromSelect ?? (doWorkFromSelect = new DelegateCommand(ExecuteWorkFromSelect));
            }
        }

        DelegateCommand doWorkFromSelect;
        public void ExecuteWorkFromSelect()
        {

            VMMain.Instance.State = "关键词分析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var fr = VMMain.Instance.VMForensicResult.SelectedItem;
                    forensicAnalysis.KeywordCount = KeyWordCount;
                    forensicAnalysis.Algorithm = SelectedAlgorithm;
                    DataSource = forensicAnalysis.ExtractTagsWithWeight(fr);

                    VMMain.Instance.State = "关键词分析完成！";
                }
                catch (Exception e)
                {
                    VMMain.Instance.State = "关键词分析出现异常";
                    VMMain.Instance.Error = e.Message;
                }

            });
        }

    }
}
