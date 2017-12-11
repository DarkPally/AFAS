using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.Library.Core
{
    public class ForensicRulePackage
    {
        //包名，com.xx.xx
        public string Name { get; set; }

        //包名描述，中文名称
        public string Desc { get; set; }

        public List<ForensicRuleItem> Items { get; set; }
    }
}
