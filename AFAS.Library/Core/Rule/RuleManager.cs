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

        public void LoadRulePackages(bool includeOrgText = false)
        {
            Packages = new List<ForensicRulePackage>();
            DirectoryInfo theFolder = new DirectoryInfo(LibraryPath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.AllDirectories);

            Parallel.ForEach(thefileInfo, f =>
            {
                try
                {
                    var t = File.ReadAllText(f.FullName, Encoding.Default);
                    var temp = RuleManager.LoadPackageFromText(t);

                    if (includeOrgText)
                    {
                        temp.OrgText = t;
                        temp.PackageFilePath = f.FullName;
                    }

                    lock (Packages)
                    {
                        Packages.Add(temp);
                    }
                }
                catch { return; }
            });
        }

        static public ForensicRulePackage LoadPackageFromText(string text)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

            var temp = JsonConvert.DeserializeObject<ForensicRulePackage>(text, jsonSerializerSettings);
            temp.Init();
            return temp;

        }

        static public void SavePackage(ForensicRulePackage package,bool fromOrgText=true)
        {
            var fs=File.Open(package.PackageFilePath, FileMode.Create);
            if (!fromOrgText)
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                jsonSerializerSettings.Formatting = Formatting.Indented;
                package.OrgText = JsonConvert.SerializeObject(package, jsonSerializerSettings);
            }

            string source = package.OrgText;
            byte[] data = System.Text.Encoding.Default.GetBytes(source);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
        }
    }
}
