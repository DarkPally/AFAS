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

        //对DataTable结果进行筛选
        public string Select { get; set; }

        public string Desc { get; set; }

        //是否把抓到的数据放到文件表下
        public bool CatchToFileDataTree { get; set; } = false;
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
                        new LuaCompileOptions() { DebugEngine = LuaStackTraceDebugger.Default });
                
            }
        }
    }
}
