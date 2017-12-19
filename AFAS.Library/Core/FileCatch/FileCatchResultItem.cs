using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFAS.Library.Android;
using System.Data;
namespace AFAS.Library.Core
{
    public class FileCatchResultItem 
    {
        public string FilePath { get; set; }

        //public string FileName { get; set; }

        public List<DataResultItem> DataItems { get; set; }
    }
}
