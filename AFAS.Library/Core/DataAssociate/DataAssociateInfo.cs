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
    public class DataAssociateInfo : ForensicRuleItemInfo
    {
        
        public enum AssociateType
        {
            None,
            InSameFile,//同文件内的关联
            AcrossFile,//跨文件的关联
            ChildFileColumn,//跨文件的关联，且关联到子表的文件属性
        }
        //Key是指本节点

        //父元素所在表
        public string ParentTableKey { get; set; }
        //父元素所在列
        public string ParentTableColumn { get; set; }
        //子元素所在表
        public string ChildTableKey { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AssociateType Type { get; set; }
        //子元素关联用元素所在列，如果为空，ChildFileTableKey
        public string AssociateColumn { get; set; }

        public override void Init()
        {   
            
        }
    }
}
