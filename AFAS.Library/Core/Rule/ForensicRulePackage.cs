using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Neo.IronLua;

namespace AFAS.Library
{
    public class ForensicRulePackage
    {
        //包名，com.xx.xx
        public string Name { get; set; }
        public string Version { get; set; }
        //包名描述，中文名称
        public string Desc { get; set; }

        public class ScriptItem
        {
            public string Name { get; set; }
            public string Content { get; set; }

            [JsonIgnore]
            public LuaChunk Chunk { get; set; }
        }

        public List<ScriptItem> Scripts { get; set; }

        public class RootPathPrepareItem
        {
            public string Name { get; set; }
            public List<string> PathRegexes { get; set; }
        }
        public List<RootPathPrepareItem> RootPathPrepares { get; set; }

        [JsonIgnore]
        public string OrgText { get; set; }
        [JsonIgnore]
        public string PackageFilePath { get; set; }

        public List<ForensicRuleItemInfo> Items { get; set; }

        public void Init()
        {
            if(Scripts!=null)
            {
                foreach (var it in Scripts)
                {
                    it.Chunk = AFASManager.Lua.Engine.CompileChunk(
                            it.Content,
                            "buffer.lua",
                            new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default });
                }
            }
            
            foreach (var it in Items)
            {
                it.Package = this;
                it.Init();
            }
        }
    }
}
