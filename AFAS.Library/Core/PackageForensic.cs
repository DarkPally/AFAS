using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AFAS.Library.Core
{
    public class PackageForensic
    {
        public bool IsPC { get; set; }
        public string PCPath { get; set; }

        public ForensicRulePackage RulePackage { get; set; }

        //FileKey
        public Dictionary<string, List<string>> CatchFilePaths = new Dictionary<string, List<string>>();

        //TableKey
        public Dictionary<string, List<DataTable>> CatchDataTables = new Dictionary<string, List<DataTable>>();

        void FileCatch(ForensicRuleItemInfo item)
        {
            FileCatch catcher;
            if (IsPC)
            {
                catcher = new FileCatchPC()
                {
                    Info = item as FileCatchInfo,
                    Environment=this,
                };
            }
            else
            {
                catcher = new FileCatchAndroid()
                {
                    Info = item as FileCatchInfo,
                    Environment = this,
                };
            }

            catcher.DoWork();
            

        }
        void FileProcess(ForensicRuleItemInfo item)
        {
            var fp = new FileProcess()
            {
                Environment = this,
                Info = item as FileProcessInfo
            };

            fp.DoWork();
        }
        void DataCatch(ForensicRuleItemInfo item)
        {
            var dc = new DataCatch()
            {
                Environment = this,
                Info = item as DataCatchInfo,
                
            };
            dc.DoWork();
        }
        void DataProcess(ForensicRuleItemInfo item)
        {
            var dp = new DataProcess()
            {
                Environment = this,
                Info = item as DataProcessInfo,

            };
            dp.DoWork();
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
                if(it is FileCatchInfo)
                {
                    FileCatch(it);
                }
                else if (it is FileProcessInfo)
                {
                    FileProcess(it);
                }
                
            }
        }
    }
}
