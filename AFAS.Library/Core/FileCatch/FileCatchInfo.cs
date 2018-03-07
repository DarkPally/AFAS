using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AFAS.Library
{
    public enum RootPathType
    {
        Default,
        Regex,
        PathPrepare,
    };

    public class FileCatchInfo: ForensicRuleItemInfo
    {
        public bool IsRegEx { get; set; }
        //包含文件名
        public string RelativePath { get; set; }
        //强制统一不以/开头以/为结尾
        //不填即默认初始化为data/data/[PackageName]/
        public string RootPath { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RootPathType RootPathType { get; set; } = RootPathType.Default;

        [JsonIgnore]
        public List<string> RootPathPrepareRegexes { get; set; }

        public override void Init()
        {
            if(RootPathType== RootPathType.Default)
            {
                if (RootPath == null)
                {
                    RootPath = String.Format("data/data/{0}/", Package.Name);
                }
                else
                {
                    if (RootPath[0] == '/') RootPath = RootPath.Substring(1);
                    if (RootPath.Last() != '/') RootPath += '/';
                }
            }
            else if(RootPathType == RootPathType.PathPrepare)
            {
                RootPathPrepareRegexes = Package.RootPathPrepares.First(c => c.Name == RootPath).PathRegexes;
            }
        }
    }
}
