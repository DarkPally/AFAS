using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.IO;
using System.Data;
using AFAS.Library;

namespace AFAS.Library.AutoRule
{
    public class FileRuleSearcher
    {
        
        public string PCFilePath { get; set; }
        public string FilePath { get; set; }
        public string RootPath { get; set; }
        public string KeyWord { get; set; }
        public string Key { get; set; }

        public List<ForensicRuleItemInfo> SearchRule()
        {
            List<ForensicRuleItemInfo> res = new List<ForensicRuleItemInfo>();
            if (FileRuleSearchHelper.CheckIsSqlite3(PCFilePath))
            {
                using (var t = new DbRuleSearcher(PCFilePath))
                {
                    var tDC = t.SearchRule(KeyWord);
                    if(tDC.Count>0)
                    {
                        res.Add(new FileCatchInfo()
                        {
                            RootPath = RootPath,
                            RelativePath = FilePath.Substring(RootPath.Length),
                            Key="File_"+ Key,
                        });
                        int i = tDC.Count != 1?1:0;                        
                        tDC.ForEach(c =>
                        {
                            c.Key = "File_" + Key;
                            if(i==0)
                            {
                                (c as DataCatchInfo).TableKey = "Table_" + Key;
                            }
                            else
                            {
                                (c as DataCatchInfo).TableKey = "Table_" + Key+"_"+i;
                                ++i;
                            }
                            
                        });
                        res.AddRange(tDC);
                    }
                }
            }
            return res;
        }

    }
}
