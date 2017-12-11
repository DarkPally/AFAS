using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.Library.Core
{

    public enum ForensicRuleItemType
    {
        None,
        FileCatch,
        FileProcess,
        DataCatch,
        DataProcess,
        DataAssociate,
        ResultMark,
    }

    public class ForensicRuleItem
    {
        public ForensicRuleItemType ItemType { get; set; }
        public FileCatcherInfo FileCatcherInfo { get; set; }

    }
}
