using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFAS.Library.Core
{
    /*
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
    */

    public class ForensicRuleItemInfo
    {
        public string Key { get; set; }
        public virtual void Init()
        {

        }
    }

    /*
    public class ForensicRuleItem
    {
        public ForensicRuleItemType Type { get; set; }
        public ForensicRuleItemInfo ItemInfo { get; set; }

        public void Init()
        {
            ItemInfo.Init();
        }

    }
    */
}
