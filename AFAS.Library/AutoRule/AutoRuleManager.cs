using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using Newtonsoft.Json;

namespace AFAS.Library.AutoRule
{

    public class AutoRuleManager
    {
        public AndroidFileExtracter androidFileExtracter { get; set; }
        public string androidDevice { get; set; }

        public string RootPath { get; set; }
        public string KeyWord { get; set; }
        public string Key { get; set; }

        public string LocalPCRoot { get; set; }

        public List<ForensicRuleItemInfo> GenerateRule(string fileSource,string key)
        {
            var pcPath = LocalPCRoot + fileSource.Replace('/', '\\');
            androidFileExtracter.CopyFileFromDevice(androidDevice, fileSource, pcPath);
            var seacher = new FileRuleSearcher()
            {
                RootPath = RootPath,
                KeyWord = KeyWord,
                Key = key,
                FilePath = fileSource,
                PCFilePath = pcPath
            };
            return seacher.SearchRule();
        }

        public List<ForensicRuleItemInfo> GenerateRule(List<string> filePaths)
        {
            if (filePaths.Count == 1) return GenerateRule(filePaths[0], Key);
            int i = 1;
            var res = new List<ForensicRuleItemInfo>();
            filePaths.ForEach(c =>
            {
                res.AddRange(GenerateRule(c, Key + i));
                ++i;
            });
            return res;
        }

        public string ConvertRuleToJson(List<ForensicRuleItemInfo> src)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            return  JsonConvert.SerializeObject(src, jsonSerializerSettings);
        }
    };
}
