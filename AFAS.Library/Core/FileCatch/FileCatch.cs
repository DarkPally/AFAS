using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;

namespace AFAS.Library.Core
{
    public class FileCatch
    {
        public PackageForensic Environment { get; set; }
        public FileCatchInfo Info { get; set; }
        //RegEx,Normal
        public bool IsRegEx { get { return Info.IsRegEx; } }

        public string RelativePath { get { return Info.RelativePath; } }
        //强制统一不以/开头以/为结尾
        //不填即默认初始化为data/data/[PackageName]/
        public string RootPath { get { return Info.RootPath; } }

        //PC根目录,统一带有\\结尾
        public string PCPath { get { return Environment.PCPath; } }

        protected static Dictionary<string, List<string>> RootPathFileNames = new Dictionary<string, List<string>>();

        static public void Init()
        {
            RootPathFileNames.Clear();
        }

        virtual protected bool checkRootPathExist()
        {
            return false;
        }

        virtual public void DoWork()
        {
        }
    }
}
