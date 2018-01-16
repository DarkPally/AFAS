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
    public class VMTagAnalysis : VMBase
    {
        public VMForensic VMForensic { get; set; }
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
            
            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {

                    var fr=VMForensic.DataSource as ForensicResult;
                    fr.Items.ForEach(c =>
                    {
                        c.Children.RemoveAll(s => s.Desc == "数据集合");
                    });
                    forensicAnalysis.KeywordCount = KeyWordCount;
                    forensicAnalysis.Algorithm = SelectedAlgorithm;
                    DataSource = forensicAnalysis.ExtractTagsWithWeight(fr);

                    State = "解析完成！";
                }
                catch(Exception e)
                {
                  State = "解析出现异常";
                  Error = e.Message;
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
            
            State = "解析中...";
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var fr = VMForensic.SelectedItem;
                    forensicAnalysis.KeywordCount = KeyWordCount;
                    forensicAnalysis.Algorithm = SelectedAlgorithm;
                    DataSource = forensicAnalysis.ExtractTagsWithWeight(fr);

                    State = "解析完成！";
                }
                catch (Exception e)
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
