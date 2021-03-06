﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AFAS.Library
{
    public class AFASManager
    {
        private static readonly AFASManager instance = new AFASManager();
        public static AFASManager Instance { get { return instance; } }

        private AFASManager()
        {
            InitLua();
        }

        public static LuaManager Lua { get { return instance.luaManager; } }

        LuaManager luaManager;
        RuleManager ruleManager;

        public void InitLua()
        {
            if (luaManager == null)
            {
                luaManager = new LuaManager();
            }
        }
        public void Init()
        {
            if (ruleManager == null)
            {
                ruleManager = new RuleManager();
                ruleManager.LoadRulePackages();
            }

        }
        string lastPcPath;
        public List<string> PCRootFiles { get; set; } = new List<string>();
        public Dictionary<string, List<string>> RootPathFileNames = new Dictionary<string, List<string>>();

        public ForensicResult DoForensic(string PcPath,bool isFromPC)
        {
            Init();
            //if(lastPcPath!= PcPath)
            {
                lastPcPath = PcPath;
                PCRootFiles.Clear();
                RootPathFileNames.Clear();
            }

            var res = new List<ForensicResultItem>();

            //Parallel.ForEach(ruleManager.Packages, pack =>
            foreach(var pack in ruleManager.Packages)
            {
                var forensic = new PackageForensic()
                {
                    IsPC = isFromPC,
                    PCPath = PcPath,
                    RulePackage = pack,
                };
                forensic.DoWork();
                res.Add(forensic.Result);
            }
            //);

            return new ForensicResult() {
                Items=res,
            }.LoadTableColumnDesc();
        }

        public ForensicResult DoForensicByPackage(ForensicRulePackage pack,string PcPath, bool isFromPC)
        {
            Init();
            if (lastPcPath != PcPath)
            {
                lastPcPath = PcPath;
                PCRootFiles.Clear();
                RootPathFileNames.Clear();
            }

            var res = new List<ForensicResultItem>();

            var forensic = new PackageForensic()
            {
                IsPC = isFromPC,
                PCPath = PcPath,
                RulePackage = pack,
            };
            forensic.DoWork();
            res.Add(forensic.Result);

            return new ForensicResult()
            {
                Items = res,
            }.LoadTableColumnDesc();
        }
    }
}
