using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

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

        public void SaveForensicResult(List<DataResultItem> res,string path)
        {
            var fs = File.Create(path);
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.Formatting = Formatting.Indented;

            var st = JsonConvert.SerializeObject(res, jsonSerializerSettings);
            var by = Encoding.Default.GetBytes(st);
            fs.Write(by,0, by.Length);
        }

        public List<DataResultItem> LoadForensicResult(string path)
        {
            var t = File.ReadAllText(path, Encoding.Default);
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

            return JsonConvert.DeserializeObject<List<DataResultItem>>(t, jsonSerializerSettings);            
        }
    }
}
