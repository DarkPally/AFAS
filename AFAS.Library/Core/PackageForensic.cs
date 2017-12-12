using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.Library.Core
{
    public class PackageForensic
    {
        public bool IsPC { get; set; }
        public string PCPath { get; set; }

        public ForensicRulePackage RulePackage { get; set; }

        public Dictionary<string, List<string>> CatchFilePaths = new Dictionary<string, List<string>>();

        void FileCatch(ForensicRuleItemInfo item)
        {
            FileCatcher catcher;
            if (IsPC)
            {
                catcher = new FileCatcherPC()
                {
                    Info = item as FileCatcherInfo,
                    Environment=this,
                };
            }
            else
            {
                catcher = new FileCatcherAndroid()
                {
                    Info = item as FileCatcherInfo,
                    Environment = this,
                };
            }

            var res=catcher.DoWork();
            CatchFilePaths.Add(catcher.Info.Key, res);

        }

        void FileProcess(ForensicRuleItemInfo item)
        {
            var fp = new FileProcesser()
            {
                Environment = this,
                Info = item as FileProcesserInfo
            };

            fp.DoWork();
        }
        void DataCatch(ForensicRuleItemInfo item)
        {

        }
        void DataProcess(ForensicRuleItemInfo item)
        {

        }
        void DataAssociate(ForensicRuleItemInfo item)
        {

        }
        void ResultMark(ForensicRuleItemInfo item)
        {

        }

        void DoWork()
        {
            foreach(var it in RulePackage.Items)
            {
                if(it is FileCatcherInfo)
                {
                    FileCatch(it);
                }
                else if (it is FileProcesserInfo)
                {
                    FileProcess(it);
                }
                
            }
        }
    }
}
