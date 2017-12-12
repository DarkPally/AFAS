using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AFAS.Library.Core;

namespace AFAS.Test
{
    class JsonTest
    {      

        public static void Main1(string[] args)
        {
            var frp = new ForensicRulePackage()
            {
                Desc = "测试",
                Name = "Test",
                Items = new List<ForensicRuleItemInfo>()
               {
                   new FileCatcherInfo(){Key="c1",RelativePath="ddd",RootPath="aaaa"},
                   new FileProcesserInfo(){Script="ssss"},
                   new FileProcesserInfo(){Script="ssss"}
               },
            };

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            jsonSerializerSettings.Formatting = Formatting.Indented;

            var st =JsonConvert.SerializeObject(frp, jsonSerializerSettings);
            Console.WriteLine(st);
        } // Main

    }
}
