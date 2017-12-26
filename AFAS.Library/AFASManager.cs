using System;
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

        public List<DataResultItem> DoForensic(string PcPath,bool isFromPC)
        {
            Init();
            var res = new List<DataResultItem>();

            Parallel.ForEach(ruleManager.Packages, pack =>
            {
                var forensic = new PackageForensic()
                {
                    IsPC = isFromPC,
                    PCPath = PcPath,
                    RulePackage = pack,
                };
                forensic.DoWork();
                res.Add(forensic.Result);
            });

            return res;
        }

        public List<DataResultItem> DoForensicByPackage(ForensicRulePackage pack,string PcPath, bool isFromPC)
        {
            Init();
            var res = new List<DataResultItem>();

            var forensic = new PackageForensic()
            {
                IsPC = isFromPC,
                PCPath = PcPath,
                RulePackage = pack,
            };
            forensic.DoWork();
            res.Add(forensic.Result);
            return res;
        }
    }
}
