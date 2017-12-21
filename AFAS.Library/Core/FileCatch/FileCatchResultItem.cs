using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using System.Data;
namespace AFAS.Library
{
    public class FileCatchResultItem 
    {
        public string Key { get; set; }
        public string FilePath { get; set; }
        public Dictionary<string,DataResultItem> DataItems { get; set; }
    }
}
