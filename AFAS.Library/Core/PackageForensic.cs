﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Neo.IronLua;

namespace AFAS.Library
{
    public class PackageForensic
    {
        public bool IsPC { get; set; }
        public string PCPath { get; set; }

        public ForensicRulePackage RulePackage { get; set; }

        public dynamic LuaEnvironment { get; } = AFASManager.Lua.CreateEnvironment();
        //FileKey
        public Dictionary<string, List<FileCatchResultItem>> CatchFilePaths = new Dictionary<string, List<FileCatchResultItem>>();
        //TableKey
        public Dictionary<string, List<DataResultItem>> CatchDataTables = new Dictionary<string, List<DataResultItem>>();

        public DataResultItem Result { get; set; }
        public DataResultItem DataSource { get; set; }

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
            var da = new DataAssociate()
            {
                Environment = this,
                Info = item as DataAssociateInfo,

            };
            da.DoWork();
        }
        void DataMark(ForensicRuleItemInfo item)
        {
            var dm = new DataMark()
            {
                Environment = this,
                Info = item as DataMarkInfo,

            };
            dm.DoWork();
        }

        void Init()
        {
            if (PCPath.Last() != '\\') PCPath += '\\';
            DataSource = new DataResultItem()
            {
                Desc = "数据集合",
            };
            Result = new DataResultItem()
            {
                Desc = RulePackage.Desc,
            };
            Result.Children.Add(DataSource);
        }
        public void DoWork()
        {
            Init();

            foreach (var it in RulePackage.Items)
            {
                if(it is FileCatchInfo)
                {
                    FileCatch(it);
                }
                else if (it is FileProcessInfo)
                {
                    FileProcess(it);
                }
                else if (it is DataCatchInfo)
                {
                    DataCatch(it);
                }
                else if (it is DataProcessInfo)
                {
                    DataProcess(it);
                }
                else if (it is DataAssociateInfo)
                {
                    DataAssociate(it);
                }
                else if (it is DataMarkInfo)
                {
                    DataMark(it);
                }
            }
        }
    }
}
