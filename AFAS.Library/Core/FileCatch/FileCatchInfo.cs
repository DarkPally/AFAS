using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;

namespace AFAS.Library.Core
{
    public class FileCatchInfo: ForensicRuleItemInfo
    {
        public bool IsRegEx { get; set; }

        public string RelativePath { get; set; }
        //强制统一不以/开头以/为结尾
        //不填即默认初始化为data/data/[PackageName]/
        public string RootPath { get; set; }

        public override void Init()
        {
            if (RelativePath.Last() != '/') RelativePath += '/';
            if (RootPath==null)
            {
                RootPath = String.Format("data/data/{0}/", Package.Name);
            }
            else
            {
                if (RootPath[0] == '/') RootPath = RootPath.Substring(1);
                if (RootPath.Last() != '/') RootPath += '/';
            }
        }
    }
}
