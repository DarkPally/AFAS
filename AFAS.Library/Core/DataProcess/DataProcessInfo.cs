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
            RegEx,
            Script,
            ScriptName,
            Json, //特殊，只有ColumName有效
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProcessType Type { get; set; }

        public string ColumnName { get; set; }
        public string OutputColumnName { get; set; }

        public string Content { get; set; }

        [JsonIgnore]
        public LuaChunk Chunk { get; set; }

        public override void Init()
        {   
            if(Type== ProcessType.Script)
            {
                Chunk = AFASManager.Lua.Engine.CompileChunk(
                        Content,
                        "buffer.lua",
                        new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default });
            }
            if (Type == ProcessType.ScriptName)
            {
                Chunk = Package.Scripts.First(c=>c.Name==Content).Chunk;
            }

            if (OutputColumnName==null)
            {
                OutputColumnName = ColumnName + "_proc";
            }
        }
    }
}
