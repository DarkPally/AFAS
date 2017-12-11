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

        void FileCatch(ForensicRuleItem item)
        {
            FileCatcher catcher;
            if (IsPC)
            {
                catcher = new FileCatcherPC()
                {
                    Info = item.FileCatcherInfo,
                    PCPath= PCPath,
                    Environment=this,
                };
            }
            else
            {
                catcher = new FileCatcherAndroid()
                {
                    Info = item.FileCatcherInfo,
                    PCPath = PCPath,
                    Environment = this,
                };
            }

            var res=catcher.DoWork();
            CatchFilePaths.Add(catcher.Info.Key, res);

        }

        void FileProcess(ForensicRuleItem item)
        {

        }
        void DataCatch(ForensicRuleItem item)
        {

        }
        void DataProcess(ForensicRuleItem item)
        {

        }
        void DataAssociate(ForensicRuleItem item)
        {

        }
        void ResultMark(ForensicRuleItem item)
        {

        }

        void DoWork()
        {
            foreach(var it in RulePackage.Items)
            {
                switch(it.ItemType)
                {
                    case ForensicRuleItemType.FileCatch:
                        FileCatch(it);
                        break;
                    case ForensicRuleItemType.FileProcess:
                        DataCatch(it);
                        break;
                    case ForensicRuleItemType.DataCatch:
                        DataCatch(it);
                        break;
                    case ForensicRuleItemType.DataProcess:
                        DataProcess(it);
                        break;
                    case ForensicRuleItemType.DataAssociate:
                        DataAssociate(it);
                        break;
                    case ForensicRuleItemType.ResultMark:
                        ResultMark(it);
                        break;
                }
            }
        }
    }
}
