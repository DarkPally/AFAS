using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;

namespace AFAS.Library.Core
{
    public class FileCatcher
    {

        //RegEx,Normal
        public string Type { get; set; }

        public string RelativePath { get; set; }
        //强制统一以/开头以/为结尾
        //不填即默认初始化为data/data/[PackageName]/
        public string RootPath { get; set; }

        //PC根目录,统一带有\\结尾
        public string PCPath { get; set; }

        protected static Dictionary<string, List<string>> RootPathFileNames = new Dictionary<string, List<string>>();

        static public void Init()
        {
            RootPathFileNames.Clear();
        }

        virtual protected bool checkRootPathExist()
        {
            return false;
        }

        virtual public List<string> DoWork()
        {
            var res = new List<string>();
            return res;
        }
    }
}
