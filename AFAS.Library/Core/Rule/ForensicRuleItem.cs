using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public ForensicRulePackage Package { get; set; }

        public string Key { get; set; }

        public virtual void Init()
        {

        }
    }
}
