using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.Library
{
    public class ForensicRulePackage
    {
        //包名，com.xx.xx
        public string Name { get; set; }
        public string Version { get; set; }
        //包名描述，中文名称
        public string Desc { get; set; }

        public List<ForensicRuleItemInfo> Items { get; set; }

        public void Init()
        {
            foreach(var it in Items)
            {
                it.Package = this;
                it.Init();
            }
        }
    }
}
