using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Data;
using System.IO;
using AFAS.Library.Core;
using AFAS.Library;

namespace AFAS.Test
{
    class PackageTest
    {
        public static void Main1(string[] args)
        {
            //var t = GetDataSetByXmlpath(@"C:\Users\zjf\Desktop\ww\www.txt");
            var pack = GetRulePackageZhiBo8();
            var forensic = new PackageForensic()
            {
                IsPC = true,
                PCPath = @"F:\Project\vs_project\AndroidApplicationForensics\AAF.Demo\bin\Debug\temp",
                RulePackage = pack,
            };
            forensic.DoWork();
            var t = forensic.Result;
        }

        public static ForensicRulePackage GetRulePackageZhiBo8()
        {
            var t= new ForensicRulePackage()
            {          
                Name = "android.zhibo8",
                Items = new List<ForensicRuleItemInfo>()
                {
                    new FileCatchInfo()
                    {
                        Key="File_History",
                        RelativePath="databases/database.db",
                    },
                    new DataCatchInfo()
                    {
                        Key="File_History",
                        Type=DataCatchInfo.FileType.Database,
                        DataPath="t_history_record",
                        TableKey="Table_History"
                    },
                    new DataMarkInfo()
                    {
                        Key="Table_History",
                        TableDesc="观看历史",
                    }
                }
            };
            t.Init();

            return t;

        }
    }
}
