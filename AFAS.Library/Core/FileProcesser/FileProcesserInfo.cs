using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using Neo.IronLua;
using Newtonsoft.Json;

namespace AFAS.Library.Core
{
    public class FileProcesserInfo : ForensicRuleItemInfo
    {
        public string Script { get; set; }
        
        [JsonIgnore]
        public LuaChunk Chunk { get; set; }

        public override void Init()
        {
            Chunk = AFASManager.Lua.Engine.CompileChunk(
                Script,
                "buffer.lua",
                new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default }
                , new KeyValuePair<string, Type>("filePath", typeof(string)));
        }
    }
}
