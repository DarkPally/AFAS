using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Data;
using System.IO;
using AFAS.Library;
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
                Desc="直播吧",
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
                        Type=DataCatchInfo.DataType.Database,
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

        public static ForensicRulePackage GetRulePackageYY()
        {
            var t = new ForensicRulePackage()
            {
                Name = "com.duowan.mobile",
                Desc = "YY语音",
                Items = new List<ForensicRuleItemInfo>()
                {
                    new FileCatchInfo()
                    {
                        Key="File_User",
                        IsRegEx=true,
                        RelativePath=@"databases/(\d+)\.db",
                    },
                    new FileCatchInfo()
                    {
                        Key="File_Core",
                        RelativePath=@"databases/core.db",
                    },
                    new DataCatchInfo()
                    {
                        Key="File_Core",
                        Type=DataCatchInfo.DataType.Database,
                        DataPath="User_UserInfo",
                        TableKey="Table_Users",
                    },
                    new DataCatchInfo()
                    {
                        Key="File_User",
                        Type=DataCatchInfo.DataType.Database,
                        DataPath="im_friend_list",
                        TableKey="Table_Friends",
                    },
                    new DataCatchInfo()
                    {
                        Key="File_User",
                        Type=DataCatchInfo.DataType.DatabaseWithRegEx,
                        DataPath=@"im_1v1_new_msg_\d+",
                        TableKey="Table_Msgs",
                    },
                    new DataProcessInfo()
                    {
                        Key="File_User",
                        Type=DataProcessInfo.ProcessType.RegEx,
                        ColumnName="FileName",
                        Content="(?<AccountID>\\d+)\\.db",
                        OutputColumnName="AccountID",
                    },
                    new DataAssociateInfo()
                    {
                        ParentTableKey="Table_Users",
                        ParentTableColumn="userID",
                        ChildTableKey="Table_Friends",
                        ChildFileTableAssociateColumn="AccountID"
                    },
                    new DataAssociateInfo()
                    {
                        ParentTableKey="Table_Friends",
                        ParentTableColumn="id",
                        ChildTableKey="Table_Msgs",
                        ChildTableAssociateColumn="reverse2"
                    },

                    new DataMarkInfo()
                    {
                        Key="Table_Users",
                        TableDesc="登录用户"
                    },
                    new DataMarkInfo()
                    {
                        Key="Table_Friends",
                        TableDesc="好友列表",
                        NotShowAtRoot=true,
                    },
                    new DataMarkInfo()
                    {
                        Key="Table_Msgs",
                        NotShowAtRoot=true,
                        TableDesc="消息记录"
                    }

                }
            };
            t.Init();

            return t;

        }
    }
}
