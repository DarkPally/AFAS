using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace AFAS.Library
{
    public class RuleManager
    {
        public string LibraryPath = "RuleLibrary";

        public List<ForensicRulePackage> Packages { get; set; }

        public void LoadRulePackages()
        {
            Packages = new List<ForensicRulePackage>();
            DirectoryInfo theFolder = new DirectoryInfo(LibraryPath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.AllDirectories);

            Parallel.ForEach(thefileInfo, f =>
            {
                try
                {
                    var t = File.ReadAllText(f.FullName, Encoding.Default);

                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                    jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

                    var temp = JsonConvert.DeserializeObject<ForensicRulePackage>(t, jsonSerializerSettings);
                    temp.Init();
                    lock (Packages)
                    {
                        Packages.Add(temp);
                    }
                }
                catch { return; }
            });
        }
    }
}
