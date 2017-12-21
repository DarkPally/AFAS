using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AFAS.Library;

namespace AFAS.Test
{
    class JsonTest
    {      

        public static void Main1(string[] args)
        {
            var frp = PackageTest.GetRulePackageYY();

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            var st =JsonConvert.SerializeObject(frp, jsonSerializerSettings);
            Console.WriteLine(st);
        } // Main

        public static void Main2(string[] args)
        {
            var frp = new ForensicRulePackage()
            {
                Desc = "测试",
                Name = "Test",
                Items = new List<ForensicRuleItemInfo>()
               {
                   new FileCatchInfo(){Key="c1",RelativePath="ddd",RootPath="aaaa"},
                   new FileProcessInfo(){Script="ssss"},
                   new FileProcessInfo(){Script="ssss"}
               },
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.Formatting = Formatting.Indented;

            var st = JsonConvert.SerializeObject(frp, jsonSerializerSettings);
            Console.WriteLine(st);
        } // Main

    }
}
