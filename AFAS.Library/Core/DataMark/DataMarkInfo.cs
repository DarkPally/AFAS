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
        //互斥 优先script
        public string TableDescScript { get; set; }

        public bool? OnlyShowDesc { get; set; }
        public bool? NotShowAtRoot { get; set; }
        public List<ColumnDescInfo> ColumnDescs { get; set; }


        public override void Init()
        {   
            
        }
    }
}
