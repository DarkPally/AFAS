using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;

namespace AFAS.Library.Core
{
    public class FileCatcherInfo
    {
        public string Key { get; set; }

        public bool IsRegEx { get; set; }

        public string RelativePath { get; set; }
        //强制统一以/开头以/为结尾
        //不填即默认初始化为data/data/[PackageName]/
        public string RootPath { get; set; }
       
    }
}
