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
        /*
        public enum AssociateType
        {
            None,
            ParentAndChildRen,
        }
        */

        //父元素所在表
        public string ParentTableKey { get; set; }
        //父元素所在列
        public string ParentTableColumn { get; set; }

        //子元素所在表
        public string ChildTableKey { get; set; }

        //子元素关联用元素所在列，如果为空，ChildFileTableKey
        public string ChildTableAssociateColumn { get; set; }

        //如果为空，则说明用ChildTableKey得到的第一个表
        public string ChildFileTableAssociateColumn { get; set; }

        public override void Init()
        {   
            
        }
    }
}
