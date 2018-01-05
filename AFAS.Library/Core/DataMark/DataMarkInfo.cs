using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IronLua;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AFAS.Library
{
    public class DataMarkInfo : ForensicRuleItemInfo
    {
        public class ColumnDescInfo
        {
            public string Name { get; set; }
            public string Desc { get; set; }
            public string Mark { get; set; }
        }

        //互斥
        public string TableDesc { get; set; }

        //0==仅root，1=仅child，2全部
        public int TableDescType { get; set; } = 0;
        //互斥 优先script
        public string TableDescScript { get; set; }

        [JsonIgnore]
        public LuaChunk TableDescScriptChunk { get; set; }

        public bool? OnlyShowDesc { get; set; }
        public bool? NotShowAtRoot { get; set; }
        public List<ColumnDescInfo> ColumnDescs { get; set; }
        

        public override void Init()
        {
            if(TableDescScript!=null)
            {
                TableDescScriptChunk = AFASManager.Lua.Engine.CompileChunk(
                                        TableDescScript,
                                        "buffer.lua",
                                        new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default });
            }
            
        }
    }
}
