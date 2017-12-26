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
    public class DataCatchInfo: ForensicRuleItemInfo
    {
        public enum DataType
        {
            None,
            //FileAttribute,
            Binary,
            Text,
            Xml,
            Database,
            DatabaseWithRegEx,
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public DataType Type { get; set; } 
        
        public string TableKey { get; set; }

        //xml,database表名
        //Text类型为正则
        //Binary类型为脚本
        public string DataPath { get; set; }

        public string Desc { get; set; }
        [JsonIgnore]
        public LuaChunk Chunk { get; set; }

        public override void Init()
        {   
            
            if(TableKey==null || TableKey=="")
            {
                TableKey = DataPath;
            }

            if(Type==DataType.Binary)
            {
                Chunk = AFASManager.Lua.Engine.CompileChunk(
                        DataPath,
                        "buffer.lua",
                        new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default }
                        , new KeyValuePair<string, Type>("filePath", typeof(string)));
                
            }
        }
    }
}
