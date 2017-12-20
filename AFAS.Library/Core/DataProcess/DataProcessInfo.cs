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
    public class DataProcessInfo : ForensicRuleItemInfo
    {
        public enum ProcessType
        {
            None,
            Script,
            ScriptName,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProcessType Type { get; set; }

        public string ColumnName { get; set; }

        public string Script { get; set; }
        [JsonIgnore]
        public LuaChunk Chunk { get; set; }

        public override void Init()
        {   
            if(Type== ProcessType.Script)
            {
                Chunk = AFASManager.Lua.Engine.CompileChunk(
                        Script,
                        "buffer.lua",
                        new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default }
                        , new KeyValuePair<string, Type>("data", typeof(string)));
            }
        }
    }
}
