using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IronLua;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AFAS.Library.Core
{
    public class DataCatcherInfo: ForensicRuleItemInfo
    {
        public enum DataType
        {
            None,
            FileAttribute,
            Binary,
            Text,
            Xml,
            Database,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public DataType Type { get; set; }

        //所有类型的表名集合，xml,database直接用表名取数据
        public List<string> TableNames { get; set; }

        //Text类型下用正则提取
        public List<string> TextRegax { get; set; }

        //Binary类型下用脚本提取
        public List<string> BinaryScript { get; set; }

        [JsonIgnore]
        public LuaChunk Chunk { get; set; }

        public override void Init()
        {           
            Chunk = AFASManager.Lua.Engine.CompileChunk(
                BinaryScript,
                "buffer.lua",
                new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default }
                , new KeyValuePair<string, Type>("filePath", typeof(string)));
        }
    }
}
